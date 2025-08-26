using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using VContainer;
using WebGLCD;

namespace UI
{
public class AddressableImageResolver : MonoBehaviour
{
    [SerializeField]
    public AssetReferenceSprite _spriteReference;

    [SerializeField]
    public Image _targetImage;

    [Inject]
    private ResourceManager _resourceManager;

    private AsyncOperationHandle<Sprite> _handle;

    private void Start() { _resourceManager.LoadAsset<Sprite>(_spriteReference).Completed += OnSpriteLoaded; }

    private void OnDestroy()
    {
        _targetImage.sprite = null;
        _resourceManager.ReleaseAsset(_spriteReference);
    }

    private void OnSpriteLoaded(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded && _targetImage != null)
            _targetImage.sprite = handle.Result;
    }
}
}