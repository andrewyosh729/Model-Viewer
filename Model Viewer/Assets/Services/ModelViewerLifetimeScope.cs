using VContainer;
using VContainer.Unity;

namespace Services
{
    public class ModelViewerLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.Register<ModelImportService>(Lifetime.Singleton).As<IModelImportService>();
        }
    }
}