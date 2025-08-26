using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace WebGLCD
{
public class Initializer : IStartable
{
    [Inject]
    private AssetManager _assetManager;

    [Inject]
    private LoadingOverlayController _loadingOverlayController;

    public void Start()
    {
        #if UNITY_EDITOR
        Caching.ClearCache();
        Debug.Log("Cache cleared");
        #endif
    }
}
}