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
public class ResourceManager
{
    private readonly Dictionary<string, ManagedAsset> _assets;
    private readonly Dictionary<string, ManagedScene> _scenes;

    private ResourceManager()
    {
        _assets = new Dictionary<string, ManagedAsset>();
        _scenes = new Dictionary<string, ManagedScene>();
    }

    public AsyncOperationHandle<TObject> LoadAsset<TObject>(AssetReference reference) where TObject : Object
    {
        var key = reference.RuntimeKey.ToString();

        if (!_assets.TryGetValue(key, out var asset))
        {
            var handle = reference.LoadAssetAsync<TObject>();
            asset = new ManagedAsset(key, handle);
            _assets.Add(key, asset);
            asset.LoadAsync().Forget();
            return handle;
        }

        asset.AddReference();
        return asset.Handle.Convert<TObject>();
    }

    public void ReleaseAsset(AssetReference reference)
    {
        var key = reference.RuntimeKey.ToString();
        if (!_assets.TryGetValue(key, out var asset)) return;

        asset.Release();
        if (asset.State == ResourceState.Unloaded) _assets.Remove(key);
    }

    public AsyncOperationHandle<SceneInstance> LoadScene(string key)
    {
        if (_scenes.TryGetValue(key, out var scene))
        {
            Debug.LogError($"Trying to load already loaded scene: {key}");
            return scene.Handle;
        }

        var handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Additive);
        scene = new ManagedScene(key, handle);
        _scenes.Add(key, scene);
        scene.LoadAsync().Forget();
        return handle;
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
            if (asset.Count > 0) continue;
            asset.Unload();
            if (asset.State == ResourceState.Unloaded) unloadedKeys.Add(key);
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