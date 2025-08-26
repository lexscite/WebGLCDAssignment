using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WebGLCD
{
public class ErrorPopupView : MonoBehaviour
{
    public Action Dismissed;

    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private TMP_Text _tmpMessage;

    [SerializeField]
    private Button _btnDismiss;

    private void Start() { _btnDismiss.onClick.AddListener(OnDismissButtonClick); }

    private void OnDestroy() { _btnDismiss.onClick.RemoveListener(OnDismissButtonClick); }

    public void Show(string msg)
    {
        _tmpMessage.text = msg;
        _canvas.enabled = true;
    }

    public void Hide() { _canvas.enabled = false; }

    private void OnDismissButtonClick()
    {
        Dismissed?.Invoke();
        _canvas.enabled = false;
    }
}
}