using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace WebGLCD
{
public class AssetManager : MonoBehaviour
{
    public AsyncOperationHandle<SceneInstance> LoadSceneAsync(string key)
    {
        return Addressables.LoadSceneAsync(key, LoadSceneMode.Additive);
    }

    public async UniTask UnloadSceneAsync(AsyncOperationHandle handle) { await Addressables.UnloadSceneAsync(handle); }

    public AsyncOperationHandle LoadGroupAsync(string key) { return Addressables.DownloadDependenciesAsync(key); }

    public async UniTask<Dictionary<string, long>> GetDownloadSizeAsync(string key)
    {
        try
        {
            var sizes = new Dictionary<string, long>();

            var locations = new List<IResourceLocation>();
            var dependenciesLocations = await GetDependenciesLocationsAsync(key);
            Debug.Log(dependenciesLocations.Count);
            locations.AddRange(dependenciesLocations);

            foreach (var location in locations)
            {
                var size = await Addressables.GetDownloadSizeAsync(location).ToUniTask();
                var groupName = GetGroupNameFromLocation(location);

                sizes.TryAdd(groupName, 0);
                sizes[groupName] += size;
            }

            foreach (var size in sizes)
            {
                Debug.Log($"{size.Key} {size.Value}");
            }

            return sizes;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
    }

    private async UniTask<List<IResourceLocation>> GetDependenciesLocationsAsync(string key)
    {
        var handle = Addressables.LoadResourceLocationsAsync(key, Addressables.MergeMode.Union);
        var locations = await handle.ToUniTask();
        Addressables.Release(handle);
        return new List<IResourceLocation>(locations);
    }

    private string GetGroupNameFromLocation(IResourceLocation location)
    {
        var path = location.InternalId.Replace("\\", "/");
        Debug.Log(location.InternalId);
        var parts = path.Split('/');
        for (var i = 0; i < parts.Length; i++)
        {
            if (parts[i].ToLower().Contains("data") && i + 1 < parts.Length)
            {
                return parts[i + 1];
            }
        }

        return "Unknown";
    }
}
}