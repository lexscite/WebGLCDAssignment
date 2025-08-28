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

    [SerializeField]
    private Button _loadSpriteButton;

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
        _levelSelector.UnloadRequested += OnLevelUnloadRequested;
        _errorPopupController.Dismissed += OnErrorPopupDismissed;
        _addressablesLogButton.onClick.AddListener(OnAddressablesLogButtonClicked);
        _loadSpriteButton.onClick.AddListener(OnLoadSpriteButtonClicked);
    }

    private void OnDestroy()
    {
        _resourceManager.LoadingFailed -= OnAssetLoadingFailed;
        _levelSelector.LevelSelected -= OnLevelSelected;
        _levelSelector.UnloadRequested -= OnLevelUnloadRequested;
        _errorPopupController.Dismissed -= OnErrorPopupDismissed;
        _addressablesLogButton.onClick.RemoveListener(OnAddressablesLogButtonClicked);
        _loadSpriteButton.onClick.RemoveListener(OnLoadSpriteButtonClicked);
    }

    private void OnAssetLoadingFailed() { _errorPopupController.Show("Network error"); }

    private void OnLevelSelected(string levelName) { _sceneManager.LoadAsync(levelName).Forget(); }

    private void OnLevelUnloadRequested() { _sceneManager.UnloadAsync().Forget(); }

    private void OnAddressablesLogButtonClicked() { _resourceManager.LogLoadedAssets(); }

    private void OnErrorPopupDismissed() { _sceneManager.ReloadMainScene(); }

    private void OnLoadSpriteButtonClicked()
    {
        _resourceManager.LoadAsset<Sprite>("Assets/Textures/UI/Background.png[Background_0]",
            sprite => { _loadSpriteButton.image.sprite = sprite; });
    }
}
}