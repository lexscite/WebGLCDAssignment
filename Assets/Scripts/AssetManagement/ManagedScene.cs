using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace WebGLCD
{
public class ManagedScene : ManagedResource
{
    public AsyncOperationHandle<SceneInstance> Handle { get; }

    public ManagedScene(string key, AsyncOperationHandle<SceneInstance> handle)
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

        try
        {
            Count++;
            await Handle;

            if (Handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Scene loaded: " + Key);
                State = ResourceState.Unloaded;
            }
            else
            {
                OnError();
                Debug.LogError("Failed to load scene: " + Key);
            }
        }
        catch (Exception e)
        {
            OnError();
            Debug.LogError($"Exception loading scene {Key}: {e}");
        }
    }

    public async UniTask ReleaseAsync()
    {
        try
        {
            Count--;
            if (Count > 0) return;

            await Addressables.UnloadSceneAsync(Handle);
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception unloading scene {Key}: {e}");
        }
    }
}
}