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
    public readonly List<DependencyInfo> DependencyInfos;

    public long DownloadSize { get; private set; }

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

        var stopwatch = new Stopwatch();
        stopwatch.Start();

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
                sb.AppendLine("Dependencies found:");
                foreach (var location in locationsToDownload)
                {
                    var sizeHandle = Addressables.GetDownloadSizeAsync(location).Task;
                    var size = sizeHandle.Result;
                    var source = GetBundleSource(location, size);
                    if (source == AssetSource.Remote) DownloadSize += size;
                    if (source == AssetSource.None) continue;
                    var dependency = new DependencyInfo(location.PrimaryKey, location.InternalId, source, size);
                    DependencyInfos.Add(dependency);
                    sb.AppendLine(dependency.ToString());
                }
            }

            sb.AppendLine($"Total size: {DownloadSize / 1024f:F2}KB");
            DependenciesRetrieved = true;
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

    private AssetSource GetBundleSource(IResourceLocation location, long size)
    {
        if (location.Data is not AssetBundleRequestOptions) return AssetSource.None;

        var id = location.InternalId;
        if (!id.StartsWith("http://") && !id.StartsWith("https://")) return AssetSource.Local;

        return size > 0 ? AssetSource.Remote : AssetSource.Cache;
    }
}
}