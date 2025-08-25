using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace UI
{
[ExecuteAlways]
public class AddressableImageResolver : MonoBehaviour
{
    [SerializeField]
    public AssetReferenceSprite _spriteReference;

    [SerializeField]
    public Image _targetImage;

    private AsyncOperationHandle<Sprite> _handle;

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            _spriteReference.LoadAssetAsync<Sprite>().Completed += OnSpriteLoaded;
        }
    }

    private void OnDisable()
    {
        if (_handle.IsValid()) Addressables.Release(_handle);
        if (_targetImage != null) _targetImage.sprite = null;
    }

    private void OnSpriteLoaded(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded && _targetImage != null)
            _targetImage.sprite = handle.Result;
    }
}
}