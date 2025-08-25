using System;
using System.Collections.Generic;
using UnityEngine;

namespace WebGLCD
{
public class LevelSelectorView : MonoBehaviour
{
    public event Action<string> LevelSelected;

    [SerializeField]
    private List<LevelButtonView> _levelButtons;

    private void Awake()
    {
        foreach (var levelButton in _levelButtons)
        {
            levelButton.Clicked += OnLevelButtonClicked;
        }
    }

    private void OnDestroy()
    {
        foreach (var levelButton in _levelButtons)
        {
            levelButton.Clicked -= OnLevelButtonClicked;
        }
    }

    private void OnLevelButtonClicked(int levelNum)
    {
        var levelName = $"Level_{levelNum}";
        LevelSelected?.Invoke(levelName);
    }
}
}