using Dummiesman;
using UnityEngine;

namespace Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModelImportService : IModelImportService
    {
        private OBJLoader ObjLoader { get; } = new OBJLoader();

        public GameObject ImportModel(string path)
        {
            return ObjLoader.Load(path);
        }
    }

    public interface IModelImportService
    {
        GameObject ImportModel(string path);
    }
}