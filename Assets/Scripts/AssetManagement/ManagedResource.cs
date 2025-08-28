using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace WebGLCD
{
public abstract class ManagedResource
{
    private const int MaxRetries = 3;
    private const float RetryDelay = 0.5f;

    public readonly string Key;
    public AsyncOperationHandle Handle { get; protected set; }
    public readonly List<LocationInfo> LocationInfos;

    public long TotalSize { get; private set; }

    public bool InfoRetrieved { get; private set; }

    public ResourceState State { get; protected set; }

    public int RefCount { get; protected set; }

    protected ManagedResource(string key)
    {
        Key = key;
        LocationInfos = new List<LocationInfo>();
    }

    public async UniTask LoadAsync(Action onSuccess, Action onFailed)
    {
        if (State is ResourceState.Loaded)
        {
            RefCount++;
            return;
        }

        if (State is ResourceState.Loading)
        {
            RefCount++;
            await UniTask.WaitUntil(() => State != ResourceState.Loading);
            switch (State)
            {
            case ResourceState.Loaded:
                onSuccess?.Invoke();
                break;
            case ResourceState.Failed:
                RefCount--;
                break;
            }

            return;
        }

        State = ResourceState.Loading;
        RefCount++;

        var stopwatch = Stopwatch.StartNew();

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
                    stopwatch.Stop();
                    Debug.Log($"Asset loaded in {stopwatch.Elapsed.ToPrettyString()}: {Key}");
                    onSuccess?.Invoke();
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

        stopwatch.Stop();
        State = ResourceState.Failed;
        if (Handle.IsValid()) Addressables.Release(Handle);
        RefCount--;
        Debug.LogError($"Asset {Key} failed to load after {MaxRetries} attempts");
        onFailed?.Invoke();
    }

    private async UniTask CollectInfoAsync()
    {
        var locationsHandle = Addressables.LoadResourceLocationsAsync(Key);

        try
        {
            await locationsHandle;

            var locations = locationsHandle.Result;
            var locationsToDownload = new HashSet<IResourceLocation>();

            foreach (var location in locations)
            {
                CollectDependencies(location, locationsToDownload);
            }

            var sb = new StringBuilder();

            if (locationsToDownload.Count > 0)
            {
                sb.AppendLine("Locations found:");
                foreach (var location in locationsToDownload)
                {
                    var sizeHandle = Addressables.GetDownloadSizeAsync(location);
                    await sizeHandle;
                    var size = sizeHandle.Result;
                    var source = GetBundleSource(location);
                    TotalSize += size;
                    var dependency = new LocationInfo(location.PrimaryKey, location.InternalId, source, size);
                    LocationInfos.Add(dependency);
                    sb.AppendLine(dependency.ToString());
                }
            }

            sb.AppendLine($"Total size: {TotalSize / 1024f:F2}KB");
            InfoRetrieved = true;
            Debug.Log(sb.ToString());
        }
        finally
        {
            if (Handle.IsValid()) Addressables.Release(locationsHandle);
        }
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

    private ResourceLocationType GetBundleSource(IResourceLocation location)
    {
        if (location.Data is AssetBundleRequestOptions == false) return ResourceLocationType.None;

        if (location.InternalId.StartsWith("http://") || location.InternalId.StartsWith("https://"))
        {
            return ResourceLocationType.Remote;
        }

        return ResourceLocationType.Local;
    }
}
}