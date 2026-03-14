using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Godot;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Assets;

public static class AtlasManager
{
	private class AtlasData
	{
		public required TpSheetData TpSheet { get; init; }

		public required Dictionary<string, Texture2D> PageTextures { get; init; }

		public required Dictionary<string, SpriteInfo> SpriteMap { get; init; }
	}

	private record SpriteInfo(Texture2D Atlas, TpSheetSprite Sprite);

	private static readonly string[] _knownAtlases = new string[12]
	{
		"ui_atlas", "compressed", "epoch_atlas", "relic_atlas", "relic_outline_atlas", "power_atlas", "card_atlas", "potion_atlas", "potion_outline_atlas", "stats_screen_atlas",
		"intent_atlas", "era_atlas"
	};

	private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	private static readonly ConcurrentDictionary<string, AtlasData> _atlases = new ConcurrentDictionary<string, AtlasData>();

	private static readonly ConcurrentDictionary<string, AtlasTexture> _spriteCache = new ConcurrentDictionary<string, AtlasTexture>();

	private static readonly Lock _loadLock = new Lock();

	private static readonly string[] _essentialAtlases = new string[2] { "ui_atlas", "compressed" };

	public static void LoadAllAtlases()
	{
		string[] knownAtlases = _knownAtlases;
		foreach (string atlasName in knownAtlases)
		{
			LoadAtlas(atlasName);
		}
	}

	public static void LoadEssentialAtlases()
	{
		string[] essentialAtlases = _essentialAtlases;
		foreach (string atlasName in essentialAtlases)
		{
			LoadAtlas(atlasName);
		}
	}

	public static void LoadAtlas(string atlasName)
	{
		if (_atlases.ContainsKey(atlasName))
		{
			return;
		}
		using (_loadLock.EnterScope())
		{
			if (!_atlases.ContainsKey(atlasName))
			{
				LoadAtlasInternal(atlasName);
			}
		}
	}

	private static void LoadAtlasInternal(string atlasName)
	{
		string text = "res://images/atlases/" + atlasName + ".tpsheet";
		if (!FileAccess.FileExists(text))
		{
			Log.Warn("AtlasManager: tpsheet not found: " + text);
			return;
		}
		FileAccess val = FileAccess.Open(text, (ModeFlags)1);
		try
		{
			if (val == null)
			{
				Log.Warn("AtlasManager: Failed to open " + text);
				return;
			}
			string asText = val.GetAsText(false);
			TpSheetData tpSheetData = JsonSerializer.Deserialize<TpSheetData>(asText, _jsonOptions);
			if (tpSheetData == null)
			{
				Log.Warn("AtlasManager: Failed to parse " + text);
				return;
			}
			Dictionary<string, Texture2D> dictionary = new Dictionary<string, Texture2D>();
			Dictionary<string, SpriteInfo> dictionary2 = new Dictionary<string, SpriteInfo>();
			foreach (TpSheetTexture texture in tpSheetData.Textures)
			{
				string text2 = "res://images/atlases/" + texture.Image;
				Texture2D val2 = ResourceLoader.Load<Texture2D>(text2, (string)null, (CacheMode)1);
				if (val2 == null)
				{
					Log.Warn("AtlasManager: Failed to load texture: " + text2);
					continue;
				}
				dictionary[texture.Image] = val2;
				foreach (TpSheetSprite sprite in texture.Sprites)
				{
					string key = NormalizeSpriteKey(sprite.Filename);
					dictionary2[key] = new SpriteInfo(val2, sprite);
				}
			}
			AtlasData value = new AtlasData
			{
				TpSheet = tpSheetData,
				PageTextures = dictionary,
				SpriteMap = dictionary2
			};
			_atlases[atlasName] = value;
			Log.Info($"AtlasManager: Loaded {atlasName} with {dictionary2.Count} sprites");
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static AtlasTexture? GetSprite(string atlasName, string spriteName)
	{
		if (!_atlases.TryGetValue(atlasName, out AtlasData value))
		{
			return null;
		}
		string key = NormalizeSpriteKey(spriteName);
		if (!value.SpriteMap.TryGetValue(key, out SpriteInfo spriteInfo))
		{
			return null;
		}
		string key2 = atlasName + "/" + spriteName;
		if (_spriteCache.TryGetValue(key2, out AtlasTexture value2))
		{
			if (GodotObject.IsInstanceValid((GodotObject)(object)value2))
			{
				return value2;
			}
			_spriteCache.TryRemove(key2, out AtlasTexture _);
		}
		return _spriteCache.GetOrAdd(key2, (string _) => CreateAtlasTexture(spriteInfo));
	}

	public static bool HasSprite(string atlasName, string spriteName)
	{
		if (!_atlases.TryGetValue(atlasName, out AtlasData value))
		{
			return false;
		}
		string key = NormalizeSpriteKey(spriteName);
		return value.SpriteMap.ContainsKey(key);
	}

	public static int GetSpriteCount(string atlasName)
	{
		if (!_atlases.TryGetValue(atlasName, out AtlasData value))
		{
			return 0;
		}
		return value.SpriteMap.Count;
	}

	public static bool IsAtlasLoaded(string atlasName)
	{
		return _atlases.ContainsKey(atlasName);
	}

	public static void Clear()
	{
		_atlases.Clear();
		_spriteCache.Clear();
	}

	private static AtlasTexture CreateAtlasTexture(SpriteInfo spriteInfo)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		TpSheetSprite sprite = spriteInfo.Sprite;
		AtlasTexture val = new AtlasTexture
		{
			Atlas = spriteInfo.Atlas,
			Region = new Rect2((float)sprite.Region.X, (float)sprite.Region.Y, (float)sprite.Region.W, (float)sprite.Region.H)
		};
		if (sprite.Margin.X != 0 || sprite.Margin.Y != 0 || sprite.Margin.W != 0 || sprite.Margin.H != 0)
		{
			val.Margin = new Rect2((float)sprite.Margin.X, (float)sprite.Margin.Y, (float)sprite.Margin.W, (float)sprite.Margin.H);
		}
		return val;
	}

	private static string NormalizeSpriteKey(string filename)
	{
		if (filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
		{
			return filename.Substring(0, filename.Length - 4);
		}
		return filename;
	}
}
