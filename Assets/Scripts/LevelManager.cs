using Cysharp.Threading.Tasks;
using VContainer;

namespace WebGLCD
{
public class LevelManager
{
    private const string Dir = "Assets/Scenes/Levels";

    [Inject]
    private AssetManager _assetManager;

    [Inject]
    private LoadingOverlayController _loadingOverlayController;

    private string _currentLevelSceneKey;

    public async UniTask LoadAsync(string levelName)
    {
        _loadingOverlayController.StartLoading();

        if (!string.IsNullOrEmpty(_currentLevelSceneKey))
        {
            await _assetManager.UnloadSceneAsync(_currentLevelSceneKey);
        }

        var key = $"{Dir}/{levelName}.unity";
        var handle = _assetManager.LoadScene(key);
        _loadingOverlayController.SetAsyncOperationHandle(handle);
        await handle;

        _assetManager.UnloadUnusedAssets();

        _loadingOverlayController.StopLoading();
    }
}
}