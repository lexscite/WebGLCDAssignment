using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace WebGLCD
{
public class Initializer : IStartable
{
    [Inject]
    private ResourceManager _resourceManager;

    [Inject]
    private LoadingOverlayController _loadingOverlayController;

    public void Start()
    {
        Debug.LogWarning(
            "<b>Hello! Use the on-screen buttons to test all ResourceManager features.</b>\n\n<b>Significant info will be printed here.</b>\n");

        #if UNITY_EDITOR
        // Caching.ClearCache();
        Debug.Log("Cache cleared");
        #endif
    }
}
}