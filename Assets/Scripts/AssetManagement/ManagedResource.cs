using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace WebGLCD
{
public abstract class ManagedResource
{
    private const int MaxRetries = 3;
    private const float RetryDelay = 0.5f;

    public readonly string Key;
    public AsyncOperationHandle Handle { get; protected set; }
    public readonly List<DependencyInfo> DependencyInfos;

    public long TotalSize { get; private set; }

    public bool DependenciesRetrieved { get; private set; }

    public ResourceState State { get; protected set; }

    public int RefCount { get; protected set; }

    protected ManagedResource(string key)
    {
        Key = key;
        DependencyInfos = new List<DependencyInfo>();
    }

    public async UniTask LoadAsync(Action onFailed)
    {
        State = ResourceState.Loading;
        RefCount++;

        var attempt = 0;

        while (attempt < MaxRetries)
        {
            try
            {
                attempt++;

                Debug.Log($"\nStarted asset loading (attempt {attempt}): {Key}");
                await CollectInfoAsync();
                await Handle;

                if (Handle.Status == AsyncOperationStatus.Succeeded)
                {
                    State = ResourceState.Loaded;
                    Debug.Log("Asset loaded: " + Key);
                    return;
                }

                Debug.LogError("Asset loading error: " + Key);
            }
            catch (Exception e)
            {
                Debug.LogError($"Asset loading exception {Key}: {e}");
            }

            await UniTask.Delay((int)(RetryDelay * 1000));
        }

        State = ResourceState.Failed;
        if (Handle.IsValid()) Addressables.Release(Handle);
        RefCount--;
        Debug.LogError($"Asset {Key} failed to load after {MaxRetries} attempts");
        onFailed?.Invoke();
    }

    private async UniTask CollectInfoAsync()
    {
        var locationsHandle = Addressables.LoadResourceLocationsAsync(Key);
        await locationsHandle;

        var locations = locationsHandle.Result;
        var locationsToDownload = new HashSet<IResourceLocation>();

        foreach (var location in locations)
        {
            CollectDependencies(location, locationsToDownload);
        }

        if (locationsToDownload.Count > 0)
        {
            Debug.Log("Dependencies found:");
            foreach (var location in locationsToDownload)
            {
                var sizeHandle = Addressables.GetDownloadSizeAsync(location).Task;
                var size = sizeHandle.Result;

                TotalSize += size;
                var source = GetBundleSource(location);
                var dependency = new DependencyInfo(location.PrimaryKey, location.InternalId, source, size);
                DependencyInfos.Add(dependency);
                Debug.Log(dependency);
            }
        }

        Debug.Log($"Total size: {TotalSize / 1024f:F2}KB");
        if (Handle.IsValid()) Addressables.Release(locationsHandle);

        DependenciesRetrieved = true;
    }

    private void CollectDependencies(IResourceLocation loc, HashSet<IResourceLocation> set)
    {
        if (!set.Add(loc)) return;

        if (loc.Dependencies == null) return;

        foreach (var dep in loc.Dependencies)
        {
            CollectDependencies(dep, set);
        }
    }

    private AssetSource GetBundleSource(IResourceLocation loc)
    {
        var id = loc.InternalId;
        if (id.StartsWith("http://") || id.StartsWith("https://")) return AssetSource.Remote;
        return AssetSource.Local;
    }
}
}