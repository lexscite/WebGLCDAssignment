using UnityEngine;

namespace WebGLCD
{
public class InitializationManager : MonoBehaviour
{
    [SerializeField]
    private AssetManager _assetManager;

    [SerializeField]
    private LoadingOverlayController _loadingOverlayController;

    private async void Start()
    {
        #if UNITY_EDITOR
        Caching.ClearCache();
        #endif

        // try
        // {
        //     const string key = "Core";
        //     var handle = _assetManager.LoadGroupAsync(key);
        //     _loadingOverlayController.ProcessAsyncOperation(handle);
        //     await handle.ToUniTask();
        //
        //     if (handle.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         Debug.Log("Scene loaded: " + key);
        //     }
        //     else
        //     {
        //         Debug.LogError("Failed to load scene: " + key);
        //     }
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError("Failed to initialize scene");
        //     Debug.LogException(e);
        // }
    }
}
}