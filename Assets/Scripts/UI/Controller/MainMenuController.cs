using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace WebGLCD
{
public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private LevelSelectorView _levelSelector;

    [SerializeField]
    private Button _addressablesLogButton;

    [Inject]
    private SceneManager _sceneManager;

    [Inject]
    private ResourceManager _resourceManager;

    [Inject]
    private ErrorPopupController _errorPopupController;

    private void Start()
    {
        _resourceManager.LoadingFailed += OnAssetLoadingFailed;
        _levelSelector.LevelSelected += OnLevelSelected;
        _addressablesLogButton.onClick.AddListener(OnAddressablesLogButtonClicked);
        _errorPopupController.Dismissed += OnErrorPopupDismissed;
    }

    private void OnDestroy()
    {
        _resourceManager.LoadingFailed -= OnAssetLoadingFailed;
        _levelSelector.LevelSelected -= OnLevelSelected;
        _addressablesLogButton.onClick.RemoveListener(OnAddressablesLogButtonClicked);
        _errorPopupController.Dismissed -= OnErrorPopupDismissed;
    }

    private void OnAssetLoadingFailed() { _errorPopupController.Show("Network error"); }

    private void OnLevelSelected(string levelName) { _sceneManager.LoadAsync(levelName).Forget(); }

    private void OnAddressablesLogButtonClicked() { _resourceManager.LogLoadedAssets(); }

    private void OnErrorPopupDismissed() { _sceneManager.ReloadMainScene(); }
}
}