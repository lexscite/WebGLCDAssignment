using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace WebGLCD
{
public class ManagedAsset : ManagedResource
{
    private const int MaxRetries = 3;
    private const float RetryDelay = 0.5f;

    public AsyncOperationHandle Handle { get; }

    public ManagedAsset(string key, AsyncOperationHandle handle)
        : base(key)
    {
        Handle = handle;
    }

    public async UniTask LoadAsync()
    {
        State = ResourceState.Loading;
        Count++;

        var attempt = 0;

        while (attempt < MaxRetries)
        {
            try
            {
                attempt++;

                Debug.Log($"Started asset loading (attempt {attempt}): {Key}");
                await Handle;

                if (Handle.Status == AsyncOperationStatus.Succeeded)
                {
                    State = ResourceState.Loaded;
                    Debug.Log("Asset loaded: " + Key);
                    return;
                }

                OnError();
                Debug.LogError("Asset loading error: " + Key);
            }
            catch (Exception e)
            {
                OnError();
                Debug.LogError($"Asset loading exception {Key}: {e}");
            }

            await UniTask.Delay((int)(RetryDelay * 1000));
        }

        OnError();
        Debug.LogError($"Asset {Key} failed to load after {MaxRetries} attempts");
        State = ResourceState.Failed;

        return;

        void OnError()
        {
            Count--;
            Addressables.Release(Handle);
        }
    }

    public void AddReference()
    {
        Count++;
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