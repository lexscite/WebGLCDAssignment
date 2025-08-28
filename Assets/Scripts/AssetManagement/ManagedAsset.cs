using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace WebGLCD
{
public class ManagedAsset<TObject> : ManagedAsset where TObject : UnityEngine.Object
{
    public AsyncOperationHandle<TObject> ConvertedHandle { get; }

    public ManagedAsset(string key)
        : base(key)
    {
        try
        {
            var handle = Addressables.LoadAssetAsync<TObject>(key);
            Handle = handle;
            ConvertedHandle = handle;
        }
        catch (Exception e)
        {
            Debug.LogError($"Asset creation exception ({key}): {e}");
            State = ResourceState.Failed;
        }
    }
}

public abstract class ManagedAsset : ManagedResource
{
    protected ManagedAsset(string key)
        : base(key) { }

    public void Release()
    {
        RefCount--;
        if (RefCount > 0) return;
        Unload();
    }

    public void Unload()
    {
        try
        {
            if (Handle.IsValid()) Addressables.Release(Handle);
            State = ResourceState.Unloaded;
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception releasing asset {Key}: {e}");
        }
    }
}
}