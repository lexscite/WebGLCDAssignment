using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

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

    public void SetAsyncOperationHandle(AsyncOperationHandle handle) { ProcessAsyncOperationAsync(handle).Forget(); }

    private async UniTaskVoid ProcessAsyncOperationAsync(AsyncOperationHandle handle)
    {
        while (!handle.IsDone)
        {
            _progressBar.SetValue(handle.PercentComplete);
            await UniTask.Yield();
        }
    }
}
}