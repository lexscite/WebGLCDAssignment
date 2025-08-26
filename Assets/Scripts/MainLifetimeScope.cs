using VContainer;
using VContainer.Unity;

namespace WebGLCD
{
public class MainLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<DevOverlayController>();
        builder.RegisterEntryPoint<Debug>();

        builder.RegisterEntryPoint<Initializer>();
        builder.Register<ResourceManager>(Lifetime.Singleton);
        builder.Register<LevelManager>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<LoadingOverlayController>();
    }
}
}