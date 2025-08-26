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
    private LevelManager _levelManager;

    [Inject]
    private AssetManager _assetManager;

    private void Start()
    {
        _levelSelector.LevelSelected += OnLevelSelected;
        _addressablesLogButton.onClick.AddListener(OnAddressablesLogButtonClicked);
    }

    private void OnDestroy()
    {
        _levelSelector.LevelSelected -= OnLevelSelected;
        _addressablesLogButton.onClick.RemoveListener(OnAddressablesLogButtonClicked);
    }

    private void OnLevelSelected(string levelName) { _levelManager.LoadAsync(levelName).Forget(); }

    private void OnAddressablesLogButtonClicked() { _assetManager.LogLoadedAssets(); }
}
}