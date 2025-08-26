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

    private void Start()
    {
        _canvas.enabled = false;
        _closeButton.gameObject.SetActive(false);

        _closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnDestroy() { _closeButton.onClick.RemoveListener(OnCloseButtonClicked); }

    public void StartLoading()
    {
        _closeButton.gameObject.SetActive(false);
        _progressBar.SetValue(0f);
        _canvas.enabled = true;
    }

    public void StopLoading()
    {
        _canvas.enabled = true;
        _closeButton.gameObject.SetActive(true);
    }

    public void SetAsyncOperationHandle(AsyncOperationHandle handle) { ProcessAsyncOperationAsync(handle).Forget(); }

    public async UniTaskVoid ProcessAsyncOperationAsync(AsyncOperationHandle handle)
    {
        while (!handle.IsDone)
        {
            _progressBar.SetValue(handle.PercentComplete);
            await UniTask.Yield();
        }
    }

    private void OnCloseButtonClicked() { _canvas.enabled = false; }
}
}