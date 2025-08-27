using UnityEngine;

namespace WebGLCD
{
public class LoadingOverlayController : MonoBehaviour
{
    [SerializeField]
    private LoadingOverlayView _view;

    [SerializeField]
    private ProgressBarView _progressBar;

    private void Start() { _view.Hide(); }

    public void StartLoading()
    {
        _view.SetCloseButtonActive(false);
        _progressBar.SetValue(0f);
        _view.Show();
    }

    public void StopLoading() { _view.SetCloseButtonActive(true); }

    public void UpdateProgressBar(float value, string additionalInfo = null)
    {
        _progressBar.SetValue(value, additionalInfo);
    }
}
}