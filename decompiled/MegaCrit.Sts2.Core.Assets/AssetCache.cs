using System.Collections.Concurrent;
using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Assets;

public class AssetCache
{
	private readonly ConcurrentDictionary<string, Resource> _cache = new ConcurrentDictionary<string, Resource>();

	private readonly HashSet<string> _missedCacheAssets = new HashSet<string>();

	private readonly HashSet<string> _failedAssets = new HashSet<string>();

	private Resource GetAsset(string path)
	{
		if (_cache.TryGetValue(path, out Resource value))
		{
			return value;
		}
		return LoadAsset(path);
	}

	public TS GetAsset<TS>(string path) where TS : Resource
	{
		return (TS)(object)GetAsset(path);
	}

	private Resource LoadAsset(string path)
	{
		if (_failedAssets.Contains(path))
		{
			throw new AssetLoadException("Asset previously failed to load: " + path + ". The game installation may be corrupted.");
		}
		_missedCacheAssets.Add(path);
		Log.Warn("Asset not cached: " + path);
		_cache[path] = ResourceLoader.Load<Resource>(path, (string)null, (CacheMode)1);
		return _cache[path];
	}

	public void MarkAssetFailed(string path)
	{
		_failedAssets.Add(path);
	}

	public AssetLoadingSession CreateSession(string name, IEnumerable<string> paths)
	{
		return new AssetLoadingSession(name, paths, _cache, this);
	}

	public void UnloadAssets(IEnumerable<string> assetsToUnloadSet)
	{
		foreach (string item in assetsToUnloadSet)
		{
			if (!_missedCacheAssets.Contains(item))
			{
				Resource val = RemoveAndGetResource(item);
				if (val != null)
				{
					((GodotObject)val).Dispose();
				}
			}
		}
	}

	public void UnloadMissedCacheAssets()
	{
		if (_missedCacheAssets.Count == 0)
		{
			return;
		}
		Log.Info($"Unloading {_missedCacheAssets.Count} missed cache assets");
		foreach (string missedCacheAsset in _missedCacheAssets)
		{
			Resource val = RemoveAndGetResource(missedCacheAsset);
			if (val != null)
			{
				((GodotObject)val).Dispose();
			}
		}
		_missedCacheAssets.Clear();
	}

	public IReadOnlySet<string> GetLoadedCacheAssets()
	{
		return new HashSet<string>(_cache.Keys);
	}

	public IEnumerable<string> GetCacheKeys()
	{
		return _cache.Keys;
	}

	private Resource? RemoveAndGetResource(string key)
	{
		if (_cache.TryRemove(key, out Resource value))
		{
			return value;
		}
		return null;
	}

	public PackedScene GetScene(string path)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		return (PackedScene)GetAsset(path);
	}

	public Texture2D GetTexture2D(string path)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		return (Texture2D)GetAsset(path);
	}

	public Material GetMaterial(string path)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		return (Material)GetAsset(path);
	}

	public CompressedTexture2D GetCompressedTexture2D(string path)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		return (CompressedTexture2D)GetAsset(path);
	}

	public bool ContainsKey(string s)
	{
		return _cache.ContainsKey(s);
	}

	public void SetAsset(string path, Resource resource)
	{
		_cache[path] = resource;
	}
}
