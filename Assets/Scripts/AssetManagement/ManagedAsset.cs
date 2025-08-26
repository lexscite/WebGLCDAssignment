using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace WebGLCD
{
public class ManagedAsset : ManagedResource
{
    public AsyncOperationHandle Handle { get; }

    public ManagedAsset(string key, AsyncOperationHandle handle)
        : base(key)
    {
        Handle = handle;
    }

    public async UniTask LoadAsync()
    {
        void OnError()
        {
            Count--;
            State = ResourceState.Failed;
            Addressables.Release(Handle);
        }

        State = ResourceState.Loading;

        try
        {
            Count++;
            await Handle;

            if (Handle.Status == AsyncOperationStatus.Succeeded)
            {
                State = ResourceState.Loaded;
                Debug.Log("Asset loaded: " + Key);
            }
            else
            {
                OnError();
                Debug.LogError("Failed to load asset: " + Key);
            }
        }
        catch (Exception e)
        {
            OnError();
            Debug.LogError($"Exception during loading asset {Key}: {e}");
        }
    }

    public void Release()
    {
        Count--;
        if (Count > 0) return;
        Unload();
    }

    public void Unload()
    {
        try
        {
            Addressables.Release(Handle);
            State = ResourceState.Unloaded;
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception releasing asset {Key}: {e}");
        }
    }
}
}