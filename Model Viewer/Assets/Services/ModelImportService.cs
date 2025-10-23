using System.IO;
using System.Linq;
using UnityEngine;
using Assimp;
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
                PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);
            GameObject parent = new GameObject(Path.GetFileName(path))
            {
                tag = "Model"
            };
            foreach (Mesh assimpMesh in scene.Meshes)
            {
                GameObject gameObject = new GameObject("SubMesh");
                gameObject.transform.SetParent(parent.transform);
                MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.material = new Material(Shader.Find("Custom/PBR"));
                UnityEngine.Mesh unityMesh = new UnityEngine.Mesh();
                unityMesh.vertices = assimpMesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();
                unityMesh.normals = assimpMesh.Normals.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();
                unityMesh.uv = assimpMesh.TextureCoordinateChannels[0].Select(v => new Vector2(v.X, v.Y)).ToArray();
                unityMesh.triangles = assimpMesh.Faces.SelectMany(f => f.Indices.Take(3)).ToArray();
                meshFilter.mesh = unityMesh;
                gameObject.AddComponent<MeshCollider>().sharedMesh = unityMesh;
            }
        }
    }

    public interface IModelImportService
    {
        void CreateSphere();
        void ImportModel(string path);
    }
}