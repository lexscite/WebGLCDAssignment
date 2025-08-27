using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace WebGLCD
{
[UsedImplicitly(ImplicitUseKindFlags.Assign)]
public class ResourceManager
{
    public event Action LoadingFailed;

    private readonly Dictionary<string, ManagedAsset> _assets;
    private readonly Dictionary<string, ManagedScene> _scenes;

    private ResourceManager()
    {
        _assets = new Dictionary<string, ManagedAsset>();
        _scenes = new Dictionary<string, ManagedScene>();
    }

    public ManagedAsset<TObject> LoadAsset<TObject>(AssetReference reference) where TObject : Object
    {
        var key = reference.RuntimeKey.ToString();
        return LoadAsset<TObject>(key);
    }

    public ManagedAsset<TObject> LoadAsset<TObject>(string key) where TObject : Object
    {
        if (!_assets.TryGetValue(key, out var asset))
        {
            asset = new ManagedAsset<TObject>(key);
            _assets.Add(key, asset);
            asset.LoadAsync(OnLoadingFailed).Forget();
            return (ManagedAsset<TObject>)asset;
        }

        asset.AddReference();

        if (asset.State == ResourceState.Failed) LoadingFailed?.Invoke();

        return (ManagedAsset<TObject>)asset;
    }

    public void ReleaseAsset(AssetReference reference)
    {
        var key = reference.RuntimeKey.ToString();
        if (!_assets.TryGetValue(key, out var asset)) return;

        asset.Release();
        if (asset.State == ResourceState.Unloaded) _assets.Remove(key);
    }

    public ManagedScene LoadScene(string key)
    {
        if (_scenes.TryGetValue(key, out var scene))
        {
            Debug.LogError($"Trying to load already loaded scene: {key}");
            return scene;
        }

        scene = new ManagedScene(key);
        _scenes.Add(key, scene);
        scene.LoadAsync(OnLoadingFailed).Forget();

        if (scene.State == ResourceState.Failed) LoadingFailed?.Invoke();

        return scene;
    }

    public async UniTask UnloadSceneAsync(string key)
    {
        if (!_scenes.TryGetValue(key, out var scene))
        {
            Debug.LogError($"Trying to unload already unloaded scene: {key}");
            return;
        }

        await scene.ReleaseAsync();
        if (scene.State == ResourceState.Unloaded)
        {
            _scenes.Remove(key);
        }
    }

    public void UnloadUnusedAssets()
    {
        var unloadedKeys = new List<string>();
        foreach (var (key, asset) in _assets)
        {
            if (asset.RefCount > 0) continue;
            asset.Unload();
            if (asset.State == ResourceState.Unloaded) unloadedKeys.Add(key);
        }

        foreach (var key in unloadedKeys)
        {
            _assets.Remove(key);
        }
    }

    private void OnLoadingFailed() { LoadingFailed?.Invoke(); }

    public void LogLoadedAssets()
    {
        var sb = new StringBuilder();

        var hasAssets = _assets.Count > 0;
        if (hasAssets)
        {
            sb.AppendLine($"Loaded assets {_assets.Count}:");

            foreach (var (_, asset) in _assets)
            {
                sb.AppendLine($"{asset.Key}: {asset.RefCount}");
                if (asset.DependencyInfos.Count > 0)
                {
                    foreach (var dependencyInfo in asset.DependencyInfos)
                    {
                        sb.AppendLine(dependencyInfo.ToString());
                    }
                }
            }
        }

        var hasScenes = _scenes.Count > 0;
        if (hasScenes)
        {
            if (hasAssets) sb.AppendLine();
            sb.AppendLine($"Loaded scenes {_scenes.Count}:");

            foreach (var (_, scene) in _scenes)
            {
                sb.AppendLine($"{scene.Key}");
                if (scene.DependencyInfos.Count > 0)
                {
                    foreach (var dependencyInfo in scene.DependencyInfos)
                    {
                        sb.AppendLine(dependencyInfo.ToString());
                    }
                }
            }
        }

        if (hasAssets || hasScenes)
        {
            Debug.Log(string.Empty);
            Debug.Log(sb.ToString());
        }
    }
}
}