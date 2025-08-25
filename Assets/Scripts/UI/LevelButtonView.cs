using System;
using UnityEngine;
using UnityEngine.UI;

namespace WebGLCD
{
public class LevelButtonView : MonoBehaviour
{
    public event Action<int> Clicked;

    [SerializeField]
    private int _levelNumber;

    [SerializeField]
    private Button _button;

    public int LevelNumber => _levelNumber;

    private void Awake() { _button.onClick.AddListener(OnClicked); }

    private void OnDestroy() { _button.onClick.RemoveListener(OnClicked); }

    private void OnClicked() { Clicked?.Invoke(_levelNumber); }
}
}