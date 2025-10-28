using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Assimp;
using UnityEngine.Rendering;
using Material = UnityEngine.Material;
using Mesh = Assimp.Mesh;
using PrimitiveType = UnityEngine.PrimitiveType;

namespace Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModelImportService : IModelImportService
    {
        public void CreateSphere()
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Sphere";
            MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Custom/PBR"));
            sphere.tag = "Model";
        }

        public void ImportModel(string path)
        {
            AssimpContext context = new AssimpContext();
            Scene scene = context.ImportFile(path,
                PostProcessSteps.Triangulate |
                PostProcessSteps.GenerateNormals |
                PostProcessSteps.JoinIdenticalVertices |
                PostProcessSteps.FlipWindingOrder |
                PostProcessSteps.MakeLeftHanded |
                PostProcessSteps.PreTransformVertices);
            GameObject parent = new GameObject(Path.GetFileName(path))
            {
                tag = "Model"
            };
            foreach (Mesh assimpMesh in scene.Meshes)
            {
                GameObject gameObject = new GameObject(assimpMesh.Name);
                gameObject.transform.SetParent(parent.transform);
                MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                Shader shader = Shader.Find("Custom/PBR");
                meshRenderer.material = new Material(shader);
                UnityEngine.Mesh unityMesh = new UnityEngine.Mesh();
                unityMesh.indexFormat = IndexFormat.UInt32;
                unityMesh.vertices = assimpMesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();
                unityMesh.normals = assimpMesh.Normals.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();
                unityMesh.uv = assimpMesh.TextureCoordinateChannels[0].Select(v => new Vector2(v.X, v.Y)).ToArray();
                unityMesh.triangles = assimpMesh.Faces.SelectMany(f => f.Indices.Take(3)).ToArray();
                meshFilter.mesh = unityMesh;
                gameObject.AddComponent<MeshCollider>().sharedMesh = unityMesh;

                Assimp.Material mat = scene.Materials[assimpMesh.MaterialIndex];
                if (mat.HasTextureDiffuse)
                {
                    string diffuseFilePath = mat.TextureDiffuse.FilePath;
                    diffuseFilePath = Path.Combine(Path.GetDirectoryName(path), diffuseFilePath);
                    TryLoadTexture(diffuseFilePath, "_MainTex", meshRenderer);
                }


                if (mat.HasTextureNormal)
                {
                    string normalFilePath = mat.TextureNormal.FilePath;
                    normalFilePath = Path.Combine(Path.GetDirectoryName(path), normalFilePath);
                    TryLoadTexture(normalFilePath, "_NormalMap", meshRenderer);
                }

                if (mat.HasTextureSpecular)
                {
                    meshRenderer.material.SetKeyword(new LocalKeyword(shader, "USE_ORM"), false);
                    string specularFilePath = mat.TextureSpecular.FilePath;
                    specularFilePath = Path.Combine(Path.GetDirectoryName(path), specularFilePath);
                    TryLoadTexture(specularFilePath, "_SpecularGlossinessMap", meshRenderer);
                }

                else // fallback
                {
                    foreach (TextureSlot textureSlot in mat.GetAllMaterialTextures())
                    {
                        if (textureSlot.TextureType is TextureType.Diffuse or TextureType.Normals)
                        {
                            continue;
                        }

                        string filePath = textureSlot.FilePath;
                        if (string.IsNullOrEmpty(filePath))
                        {
                            continue;
                        }

                        string fileName = Path.GetFileNameWithoutExtension(filePath).ToLower();
                        filePath = Path.Combine(Path.GetDirectoryName(path), filePath);


                        List<string> words = new List<string>();
                        if (fileName.Contains("occlusion"))
                        {
                            words.Add("occlusion");
                        }

                        if (fileName.Contains("roughness"))
                        {
                            words.Add("roughness");
                        }

                        if (fileName.Contains("metallic"))
                        {
                            words.Add("metallic");
                        }
                        
                        if (fileName.Contains("metallic"))
                        {
                            words.Add("metallic");
                        }

                        List<ORMPermutation> fullPermutations = GeneratePermutations(words);

                        foreach (ORMPermutation permutation in fullPermutations)
                        {
                            if (fileName.Contains(permutation.Name))
                            {
                                UnpackORM(filePath, permutation, meshRenderer);
                                meshRenderer.material.SetKeyword(new LocalKeyword(shader, "USE_ORM"), true);
                                if (fullPermutations.Count < 3)
                                {
                                    break;
                                }

                                return;
                            }
                        }
                    }
                }
            }
        }

        private struct ORMPermutation
        {
            public string Name { get; set; }
            public string Acronym { get; set; }
        }


        private List<ORMPermutation> GeneratePermutations(List<string> words)
        {
            List<ORMPermutation> results = new List<ORMPermutation>();
            int n = words.Count;

            Permute(words, new List<string>(), new bool[n], n, results);
            return results;
        }

        private void Permute(List<string> words, List<string> current, bool[] used, int length,
            List<ORMPermutation> results)
        {
            if (current.Count == length)
            {
                string name = string.Join("", current);
                string acronym = string.Join("", current.ConvertAll(w => w[0].ToString()));
                results.Add(new ORMPermutation { Name = name, Acronym = acronym });
                return;
            }

            for (int i = 0; i < words.Count; i++)
            {
                if (used[i])
                {
                    continue;
                }

                used[i] = true;
                current.Add(words[i]);
                Permute(words, current, used, length, results);
                current.RemoveAt(current.Count - 1);
                used[i] = false;
            }
        }

        private void UnpackORM(string filePath, ORMPermutation permutation, MeshRenderer renderer)
        {
            Texture2D ormTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
            ormTexture.LoadImage(File.ReadAllBytes(filePath), false);

            int width = ormTexture.width;
            int height = ormTexture.height;

            Color[] pixels = ormTexture.GetPixels();

            bool hasOcclusion = permutation.Acronym.Contains("o");
            bool hasRoughness = permutation.Acronym.Contains("r");
            bool hasMetallic = permutation.Acronym.Contains("m");

            Color[] occlusionPixels = null;
            Color[] roughnessPixels = null;
            Color[] metallicPixels = null;
            if (hasOcclusion)
            {
                occlusionPixels = new Color[pixels.Length];
            }

            if (hasRoughness)
            {
                roughnessPixels = new Color[pixels.Length];
            }

            if (hasMetallic)
            {
                metallicPixels = new Color[pixels.Length];
            }


            for (int i = 0; i < pixels.Length; i++)
            {
                Color p = pixels[i];
                if (occlusionPixels != null)
                {
                    occlusionPixels[i] = new Color(p[0], 0, 0);
                }

                if (roughnessPixels != null)
                {
                    roughnessPixels[i] = new Color(p[1], 0, 0);
                }

                if (metallicPixels != null)
                    metallicPixels[i] = new Color(p[2], 0, 0);
            }


            if (hasOcclusion)
            {
                Texture2D occlusionTex = new Texture2D(width, height, TextureFormat.RGB24, false);
                occlusionTex.SetPixels(occlusionPixels);
                occlusionTex.Apply();
                renderer.material.SetTexture("_OcclusionMap", occlusionTex);
            }

            if (hasRoughness)
            {
                Texture2D roughnessTex = new Texture2D(width, height, TextureFormat.RGB24, false);
                roughnessTex.SetPixels(roughnessPixels);
                roughnessTex.Apply();
                renderer.material.SetTexture("_RoughnessMap", roughnessTex);
            }

            if (hasMetallic)
            {
                Texture2D metallicTex = new Texture2D(width, height, TextureFormat.RGB24, false);
                metallicTex.SetPixels(metallicPixels);
                metallicTex.Apply();
                renderer.material.SetTexture("_MetallicMap", metallicTex);
            }

            Debug.Log("Successfully imported ORM texture: " + filePath);
        }

        private bool TryLoadTexture(string filePath, string textureName, MeshRenderer meshRenderer)
        {
            if (File.Exists(filePath))
            {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
                texture.LoadImage(File.ReadAllBytes(filePath), true);
                meshRenderer.material.SetTexture(textureName, texture);
                Debug.Log("Successfully imported texture: " + filePath);
                return true;
            }

            Debug.LogError("Failed to import texture: " + filePath);

            return false;
        }
    }


    public interface IModelImportService
    {
        void CreateSphere();
        void ImportModel(string path);
    }
}