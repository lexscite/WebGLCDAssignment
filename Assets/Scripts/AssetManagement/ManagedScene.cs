using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace WebGLCD
{
public class ManagedScene : ManagedResource
{
    public AsyncOperationHandle<SceneInstance> ConvertedHandle { get; }

    public ManagedScene(string key)
        : base(key)
    {
        var handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Additive);
        Handle = handle;
        ConvertedHandle = handle;
    }

    public async UniTask ReleaseAsync()
    {
        try
        {
            RefCount--;
            if (RefCount > 0) return;

            await Addressables.UnloadSceneAsync(Handle);
            State = ResourceState.Unloaded;
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception unloading scene {Key}: {e}");
        }
    }
}
}