using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WebGLCD
{
public class LevelSelectorView : MonoBehaviour
{
    public event Action<string> LevelSelected;
    public event Action UnloadRequested;

    [SerializeField]
    private List<LevelButtonView> _levelButtons;

    [SerializeField]
    private Button _unloadButton;

    private void Start()
    {
        foreach (var levelButton in _levelButtons)
        {
            levelButton.Clicked += OnLevelButtonClicked;
        }

        _unloadButton.onClick.AddListener(OnUnloadButtonClicked);
    }

    private void OnDestroy()
    {
        foreach (var levelButton in _levelButtons)
        {
            levelButton.Clicked -= OnLevelButtonClicked;
        }

        _unloadButton.onClick.RemoveListener(OnUnloadButtonClicked);
    }

    private void OnLevelButtonClicked(int levelNum)
    {
        var levelName = $"Level_{levelNum}";
        LevelSelected?.Invoke(levelName);
    }

    private void OnUnloadButtonClicked() { UnloadRequested?.Invoke(); }
}
}