using UnityEngine;

namespace WebGLCD
{
public class ErrorPopupController : MonoBehaviour
{
    [SerializeField]
    private ErrorPopupView _view;

    private void Start()
    {
        _view.Hide();
        _view.Dismissed += OnDismissed;
    }

    private void OnDestroy() { _view.Dismissed -= OnDismissed; }

    public void Show(string msg) { _view.Show(msg); }

    private void OnDismissed() { }
}
}