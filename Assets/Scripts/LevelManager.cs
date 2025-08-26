using Cysharp.Threading.Tasks;
using VContainer;

namespace WebGLCD
{
public class LevelManager
{
    private const string Dir = "Assets/Scenes/Levels";

    [Inject]
    private ResourceManager _resourceManager;

    [Inject]
    private LoadingOverlayController _loadingOverlayController;

    private string _currentLevelSceneKey;

    public async UniTask LoadAsync(string levelName)
    {
        var key = $"{Dir}/{levelName}.unity";
        if (key == _currentLevelSceneKey)
        {
            Debug.LogWarning($"Level \"{levelName}\" has already been loaded");
            return;
        }

        _loadingOverlayController.StartLoading();

        if (!string.IsNullOrEmpty(_currentLevelSceneKey))
        {
            await _resourceManager.UnloadSceneAsync(_currentLevelSceneKey);
        }

        _currentLevelSceneKey = key;
        var handle = _resourceManager.LoadScene(_currentLevelSceneKey);
        _loadingOverlayController.SetAsyncOperationHandle(handle);
        await handle;

        _resourceManager.UnloadUnusedAssets();

        _loadingOverlayController.StopLoading();
    }
}
}