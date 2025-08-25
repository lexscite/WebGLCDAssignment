using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace WebGLCD
{
public class LoadingOverlayController : MonoBehaviour
{
    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private Button _closeButton;

    [SerializeField]
    private ProgressBarView _progressBar;

    private void Awake()
    {
        _canvas.enabled = false;
        _closeButton.gameObject.SetActive(false);

        _closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnDestroy() { _closeButton.onClick.RemoveListener(OnCloseButtonClicked); }

    public void ProcessAsyncOperation(AsyncOperationHandle handle) { ProcessAsyncOperationAsync(handle).Forget(); }

    private async UniTaskVoid ProcessAsyncOperationAsync(AsyncOperationHandle handle)
    {
        _closeButton.gameObject.SetActive(false);
        _progressBar.SetValue(0f);
        _canvas.enabled = true;
        while (!handle.IsDone)
        {
            _progressBar.SetValue(handle.PercentComplete);
            await UniTask.Yield();
        }

        _closeButton.gameObject.SetActive(true);
    }

    private void OnCloseButtonClicked() { _canvas.enabled = false; }
}
}