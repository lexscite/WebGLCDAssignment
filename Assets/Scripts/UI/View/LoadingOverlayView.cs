using UnityEngine;
using UnityEngine.UI;

namespace WebGLCD
{
public class LoadingOverlayView : MonoBehaviour
{
    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private Button _closeButton;

    private void Start()
    {
        _closeButton.gameObject.SetActive(false);
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    public void Show() { _canvas.enabled = true; }

    public void Hide() { _canvas.enabled = false; }

    public void SetCloseButtonActive(bool active) { _closeButton.gameObject.SetActive(active); }

    private void OnCloseButtonClicked() { Hide(); }
}
}