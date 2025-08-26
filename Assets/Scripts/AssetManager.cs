using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace WebGLCD
{
[UsedImplicitly(ImplicitUseKindFlags.Assign)]
public class AssetManager
{
    private abstract class Resource
    {
        public readonly string Key;
        public int Count;
        public List<string> Dependencies;

        public AsyncOperationHandle Handle { get; }

        protected Resource(string key, AsyncOperationHandle handle)
        {
            Key = key;
            Handle = handle;
        }
    }

    private class Asset : Resource
    {
        private bool _released;

        public Asset(string key, AsyncOperationHandle handle)
            : base(key, handle) { }

        public void Release()
        {
            if (_released) return;
            Addressables.Release(Handle);
            _released = true;
        }
    }

    private class Scene : Resource
    {
        public Scene(string key, AsyncOperationHandle<SceneInstance> handle)
            : base(key, handle) { }

        public async UniTask UnloadAsync() { await Addressables.UnloadSceneAsync(Handle); }
    }

    private readonly Dictionary<string, Asset> _assets;
    private readonly Dictionary<string, Scene> _scenes;
    private readonly Dictionary<string, Resource> _resources;

    private AssetManager()
    {
        _resources = new Dictionary<string, Resource>();
        _assets = new Dictionary<string, Asset>();
        _scenes = new Dictionary<string, Scene>();
    }

    public AsyncOperationHandle<TObject> Load<TObject>(AssetReference reference) where TObject : Object
    {
        var key = reference.RuntimeKey.ToString();

        if (!_assets.TryGetValue(key, out var asset))
        {
            var handle = reference.LoadAssetAsync<TObject>();
            asset = new Asset(key, handle);
            _assets.Add(key, asset);
            _resources.Add(key, asset);
            _assets[key].Count++;
            LoadAsync(asset).Forget();
            return handle;
        }

        asset.Count++;
        return asset.Handle.Convert<TObject>();
    }

    public void Unload(AssetReference reference)
    {
        var key = reference.RuntimeKey.ToString();
        if (!_assets.TryGetValue(key, out var asset)) return;

        asset.Count--;
        if (asset.Count > 0) return;

        asset.Release();
        _assets.Remove(key);
        _resources.Remove(key);
    }

    private async UniTask LoadAsync(Asset asset)
    {
        void OnError()
        {
            _assets.Remove(asset.Key);
            _resources.Remove(asset.Key);
            Addressables.Release(asset.Handle);
        }

        try
        {
            var task = asset.Handle.ToUniTask();
            await task;

            if (asset.Handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Asset loaded: " + asset.Key);
            }
            else
            {
                OnError();
                Debug.LogError("Failed to load asset: " + asset.Key);
            }
        }
        catch (Exception e)
        {
            OnError();
            Debug.LogError($"Exception during loading asset {asset.Key}: {e}");
        }
    }

    public AsyncOperationHandle<SceneInstance> LoadScene(string key)
    {
        if (_scenes.TryGetValue(key, out var scene))
        {
            return scene.Handle.Convert<SceneInstance>();
        }

        var handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Additive);
        scene = new Scene(key, handle);
        scene.Count++;
        _scenes.Add(key, scene);
        _resources.Add(key, scene);
        LoadSceneAsync(scene).Forget();
        return handle;
    }

    private async UniTask LoadSceneAsync(Scene scene)
    {
        void OnError()
        {
            _scenes.Remove(scene.Key);
            _resources.Remove(scene.Key);
            Addressables.Release(scene.Handle);
        }

        try
        {
            var task = scene.Handle.ToUniTask();
            await task;

            if (scene.Handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Scene loaded: " + scene.Key);
            }
            else
            {
                OnError();
                Debug.LogError("Failed to load scene: " + scene.Key);
            }
        }
        catch (Exception e)
        {
            OnError();
            Debug.LogError($"Exception loading scene {scene.Key}: {e}");
        }
    }

    public async UniTask UnloadSceneAsync(string key)
    {
        await _scenes[key].UnloadAsync();
        Debug.Log("Scene unloaded: " + key);
        _scenes.Remove(key);
    }

    public void UnloadUnusedAssets()
    {
        var unloadedKeys = new List<string>();
        foreach (var (key, asset) in _assets)
        {
            if (asset.Count > 0) continue;

            asset.Release();
            unloadedKeys.Add(key);
        }

        foreach (var key in unloadedKeys)
        {
            _assets.Remove(key);
        }
    }

    public void LogLoadedAssets()
    {
        var sb = new StringBuilder();

        var hasAssets = _assets.Count > 0;
        if (hasAssets)
        {
            sb.AppendLine("Loaded assets:");

            foreach (var (_, asset) in _assets)
            {
                sb.AppendLine($"{asset.Key}: {asset.Count}");
            }
        }

        var hasScenes = _scenes.Count > 0;
        if (hasScenes)
        {
            sb.AppendLine();
            sb.AppendLine("Loaded scenes:");

            foreach (var (_, scene) in _scenes)
            {
                sb.AppendLine($"{scene.Key}");
            }
        }

        if (hasAssets || hasScenes) Debug.Log(sb.ToString());
    }
}
}