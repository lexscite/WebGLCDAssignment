using System.Globalization;
using Cysharp.Threading.Tasks;
using VContainer;

namespace WebGLCD
{
public class SceneManager
{
    private const string Dir = "Assets/Scenes/Levels";

    [Inject]
    private ResourceManager _resourceManager;

    [Inject]
    private LoadingOverlayController _loadingOverlayController;

    private string _currentLevelSceneKey;

    public void ReloadMainScene() { UnityEngine.SceneManagement.SceneManager.LoadScene(0); }

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
        var managedScene = _resourceManager.LoadScene(_currentLevelSceneKey);
        var handle = managedScene.Handle;
        while (!handle.IsDone)
        {
            var percentComplete = handle.PercentComplete;
            var sizeMb = managedScene.TotalSize / 1024f / 1024f;
            var additionalInfo = managedScene.DependenciesRetrieved
                ? $"{(sizeMb * percentComplete).ToString("F2", CultureInfo.InvariantCulture)}/{sizeMb.ToString("F2", CultureInfo.InvariantCulture)}mb"
                : "calculating size...";
            _loadingOverlayController.UpdateProgressBar(percentComplete, additionalInfo);
            await UniTask.Yield();
        }

        _resourceManager.UnloadUnusedAssets();

        if (managedScene.State == ResourceState.Loaded)
        {
            _loadingOverlayController.StopLoading();
        }
    }
}
}