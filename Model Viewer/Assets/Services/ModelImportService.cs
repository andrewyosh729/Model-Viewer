using Dummiesman;

namespace Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModelImportService : IModelImportService
    {
        private OBJLoader ObjLoader { get; } = new OBJLoader();

        public void ImportModel(string path)
        {
            ObjLoader.Load(path);
        }
    }

    public interface IModelImportService
    {
        void ImportModel(string path);
    }
}