using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace WebGLCD
{
public class ManagedScene : ManagedResource
{
    private const int MaxRetries = 3;
    private const float RetryDelay = 0.5f;

    public AsyncOperationHandle<SceneInstance> Handle { get; }

    public ManagedScene(string key, AsyncOperationHandle<SceneInstance> handle)
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
                Debug.Log($"Started scene loading (attempt {attempt}): {Key}");
                await Handle;

                if (Handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Scene loaded: " + Key);
                    State = ResourceState.Unloaded;
                    return;
                }

                OnError();
                Debug.LogError("Scene loading error: " + Key);
            }
            catch (Exception e)
            {
                OnError();
                Debug.LogError($"Scene loading exception {Key}: {e}");
            }

            await UniTask.Delay((int)(RetryDelay * 1000));
        }

        OnError();
        Debug.LogError($"Scene {Key} failed to load after {MaxRetries} attempts");
        State = ResourceState.Failed;

        return;

        void OnError()
        {
            attempt++;
            Count--;
            State = ResourceState.Failed;
            Addressables.Release(Handle);
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