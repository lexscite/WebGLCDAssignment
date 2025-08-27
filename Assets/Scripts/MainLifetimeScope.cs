using VContainer;
using VContainer.Unity;

namespace WebGLCD
{
public class MainLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<DevOverlayController>();
        builder.RegisterComponentInHierarchy<LoadingOverlayController>();
        builder.RegisterComponentInHierarchy<ErrorPopupController>();

        builder.RegisterEntryPoint<Debug>();
        builder.RegisterEntryPoint<Initializer>();

        builder.Register<ResourceManager>(Lifetime.Singleton);
        builder.Register<SceneManager>(Lifetime.Singleton);
    }
}
}