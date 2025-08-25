using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace WebGLCD
{
public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private LevelSelectorView _levelSelector;

    [FormerlySerializedAs("_sceneManager")]
    [SerializeField]
    private LevelManager _levelManager;

    private void Awake() { _levelSelector.LevelSelected += OnLevelSelected; }

    private void OnDestroy() { _levelSelector.LevelSelected -= OnLevelSelected; }

    private void OnLevelSelected(string levelName) { _levelManager.LoadAsync(levelName).Forget(); }
}
}