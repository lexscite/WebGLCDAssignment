using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace WebGLCD
{
public class LevelManager : MonoBehaviour
{
    private const string Dir = "Assets/Scenes/Levels";

    [SerializeField]
    private AssetManager _assetManager;

    [SerializeField]
    private LoadingOverlayController _loadingOverlayController;

    private AsyncOperationHandle<SceneInstance> _currentLevelSceneHandle;

    public async UniTask LoadAsync(string levelName)
    {
        if (_currentLevelSceneHandle.IsValid())
        {
            await _assetManager.UnloadSceneAsync(_currentLevelSceneHandle);
        }

        var key = $"{Dir}/{levelName}.unity";
        _currentLevelSceneHandle = _assetManager.LoadSceneAsync(key);
        _loadingOverlayController.ProcessAsyncOperation(_currentLevelSceneHandle);
        await _currentLevelSceneHandle.ToUniTask();

        if (_currentLevelSceneHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Scene loaded: " + key);
        }
        else
        {
            Debug.LogError("Failed to load scene: " + key);
        }
    }
}
}