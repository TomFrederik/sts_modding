using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Assets;

[ScriptPath("res://src/Core/Assets/AtlasResourceLoader.cs")]
public class AtlasResourceLoader : ResourceFormatLoader
{
	public class MethodName : MethodName
	{
		public static readonly StringName _GetRecognizedExtensions = StringName.op_Implicit("_GetRecognizedExtensions");

		public static readonly StringName _HandlesType = StringName.op_Implicit("_HandlesType");

		public static readonly StringName _GetResourceType = StringName.op_Implicit("_GetResourceType");

		public static readonly StringName _RecognizePath = StringName.op_Implicit("_RecognizePath");

		public static readonly StringName _Exists = StringName.op_Implicit("_Exists");

		public static readonly StringName _Load = StringName.op_Implicit("_Load");

		public static readonly StringName _GetDependencies = StringName.op_Implicit("_GetDependencies");

		public static readonly StringName IsSpritePath = StringName.op_Implicit("IsSpritePath");

		public static readonly StringName HasFallback = StringName.op_Implicit("HasFallback");

		public static readonly StringName LoadFallback = StringName.op_Implicit("LoadFallback");

		public static readonly StringName GetFallbackPath = StringName.op_Implicit("GetFallbackPath");

		public static readonly StringName GetRelicFallbackPath = StringName.op_Implicit("GetRelicFallbackPath");

		public static readonly StringName GetPowerFallbackPath = StringName.op_Implicit("GetPowerFallbackPath");

		public static readonly StringName GetCardFallbackPath = StringName.op_Implicit("GetCardFallbackPath");

		public static readonly StringName GetPotionFallbackPath = StringName.op_Implicit("GetPotionFallbackPath");

		public static readonly StringName GetMissingTexture = StringName.op_Implicit("GetMissingTexture");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	private const string _atlasBasePath = "res://images/atlases/";

	private const string _spritesSuffix = ".sprites/";

	private static readonly StringName _typeAtlasTexture = new StringName("AtlasTexture");

	private static readonly StringName _typeTexture2D = new StringName("Texture2D");

	private static readonly StringName _typeResource = new StringName("Resource");

	private static readonly Regex _pathPattern = new Regex("^res://images/atlases/([^/]+)\\.sprites/(.+)\\.tres$", RegexOptions.Compiled);

	public override string[] _GetRecognizedExtensions()
	{
		return new string[1] { "tres" };
	}

	public override bool _HandlesType(StringName type)
	{
		if (!(type == _typeAtlasTexture) && !(type == _typeTexture2D))
		{
			return type == _typeResource;
		}
		return true;
	}

	public override string _GetResourceType(string path)
	{
		if (IsSpritePath(path))
		{
			return "AtlasTexture";
		}
		return "";
	}

	public override bool _RecognizePath(string path, StringName type)
	{
		return IsSpritePath(path);
	}

	public override bool _Exists(string path)
	{
		if (!IsSpritePath(path))
		{
			return false;
		}
		var (text, text2) = ParsePath(path);
		if (text == null || text2 == null)
		{
			return false;
		}
		if (!AtlasManager.IsAtlasLoaded(text))
		{
			AtlasManager.LoadAtlas(text);
		}
		if (!AtlasManager.HasSprite(text, text2))
		{
			return HasFallback(text, text2);
		}
		return true;
	}

	public override Variant _Load(string path, string originalPath, bool useSubThreads, int cacheMode)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (!IsSpritePath(path))
		{
			return default(Variant);
		}
		var (text, text2) = ParsePath(path);
		if (text == null || text2 == null)
		{
			Log.Warn("AtlasResourceLoader: Failed to parse path: " + path);
			return Variant.op_Implicit(7L);
		}
		if (!AtlasManager.IsAtlasLoaded(text))
		{
			AtlasManager.LoadAtlas(text);
		}
		AtlasTexture sprite = AtlasManager.GetSprite(text, text2);
		if (sprite != null)
		{
			return Variant.op_Implicit((GodotObject)(object)sprite);
		}
		Texture2D val = LoadFallback(text, text2);
		if (val != null)
		{
			return Variant.op_Implicit((GodotObject)(object)val);
		}
		if (!text2.StartsWith("mock_"))
		{
			Log.Warn($"AtlasResourceLoader: Missing sprite '{text2}' in {text} (requested: {path})");
		}
		return GetMissingTexture(text);
	}

	public override string[] _GetDependencies(string path, bool addTypes)
	{
		return Array.Empty<string>();
	}

	private static bool IsSpritePath(string path)
	{
		if (path.StartsWith("res://images/atlases/") && path.Contains(".sprites/"))
		{
			return path.EndsWith(".tres");
		}
		return false;
	}

	public static (string? AtlasName, string? SpriteName) ParsePath(string path)
	{
		Match match = _pathPattern.Match(path);
		if (!match.Success)
		{
			return (AtlasName: null, SpriteName: null);
		}
		return (AtlasName: match.Groups[1].Value, SpriteName: match.Groups[2].Value);
	}

	private static bool HasFallback(string atlasName, string spriteName)
	{
		string fallbackPath = GetFallbackPath(atlasName, spriteName);
		if (fallbackPath != null)
		{
			return ResourceLoader.Exists(fallbackPath, "");
		}
		return false;
	}

	private static Texture2D? LoadFallback(string atlasName, string spriteName)
	{
		string fallbackPath = GetFallbackPath(atlasName, spriteName);
		if (fallbackPath == null)
		{
			return null;
		}
		if (!ResourceLoader.Exists(fallbackPath, ""))
		{
			return null;
		}
		Log.Debug($"AtlasResourceLoader: Using fallback for {atlasName}/{spriteName}: {fallbackPath}");
		return ResourceLoader.Load<Texture2D>(fallbackPath, (string)null, (CacheMode)1);
	}

	private static string? GetFallbackPath(string atlasName, string spriteName)
	{
		switch (atlasName)
		{
		case "relic_atlas":
		case "relic_outline_atlas":
			return GetRelicFallbackPath(spriteName);
		case "power_atlas":
			return GetPowerFallbackPath(spriteName);
		case "card_atlas":
			return GetCardFallbackPath(spriteName);
		case "potion_atlas":
		case "potion_outline_atlas":
			return GetPotionFallbackPath(spriteName);
		default:
			return null;
		}
	}

	private static string? GetRelicFallbackPath(string spriteName)
	{
		string text = "res://images/relics/" + spriteName + ".png";
		if (ResourceLoader.Exists(text, ""))
		{
			return text;
		}
		string text2 = "res://images/relics/beta/" + spriteName + ".png";
		if (ResourceLoader.Exists(text2, ""))
		{
			return text2;
		}
		return null;
	}

	private static string? GetPowerFallbackPath(string spriteName)
	{
		string text = "res://images/powers/" + spriteName + ".png";
		if (ResourceLoader.Exists(text, ""))
		{
			return text;
		}
		string text2 = "res://images/powers/beta/" + spriteName + ".png";
		if (ResourceLoader.Exists(text2, ""))
		{
			return text2;
		}
		return null;
	}

	private static string? GetCardFallbackPath(string spriteName)
	{
		string text = "res://images/packed/card_portraits/" + spriteName + ".png";
		if (ResourceLoader.Exists(text, ""))
		{
			return text;
		}
		int num = spriteName.LastIndexOf('/');
		if (num > 0)
		{
			string value = spriteName.Substring(0, num);
			int num2 = num + 1;
			string value2 = spriteName.Substring(num2, spriteName.Length - num2);
			string text2 = $"res://images/packed/card_portraits/{value}/beta/{value2}.png";
			if (ResourceLoader.Exists(text2, ""))
			{
				return text2;
			}
		}
		return null;
	}

	private static string? GetPotionFallbackPath(string spriteName)
	{
		string text = "res://images/potions/" + spriteName + ".png";
		if (ResourceLoader.Exists(text, ""))
		{
			return text;
		}
		return null;
	}

	private static Variant GetMissingTexture(string atlasName)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		string text = ((!(atlasName == "card_atlas")) ? "res://images/powers/missing_power.png" : "res://images/packed/card_portraits/beta.png");
		string text2 = text;
		if (ResourceLoader.Exists(text2, ""))
		{
			Texture2D val = ResourceLoader.Load<Texture2D>(text2, (string)null, (CacheMode)1);
			if (val != null)
			{
				return Variant.op_Implicit((GodotObject)(object)val);
			}
		}
		return Variant.op_Implicit(7L);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Expected O, but got Unknown
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(16);
		list.Add(new MethodInfo(MethodName._GetRecognizedExtensions, new PropertyInfo((Type)34, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._HandlesType, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)21, StringName.op_Implicit("type"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GetResourceType, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._RecognizePath, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)21, StringName.op_Implicit("type"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Exists, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Load, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)131078, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("originalPath"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("useSubThreads"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("cacheMode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GetDependencies, new PropertyInfo((Type)34, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("addTypes"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsSpritePath, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HasFallback, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("atlasName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("spriteName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LoadFallback, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("atlasName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("spriteName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetFallbackPath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("atlasName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("spriteName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetRelicFallbackPath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("spriteName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetPowerFallbackPath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("spriteName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetCardFallbackPath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("spriteName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetPotionFallbackPath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("spriteName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetMissingTexture, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)131078, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("atlasName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._GetRecognizedExtensions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			string[] array = ((ResourceFormatLoader)this)._GetRecognizedExtensions();
			ret = VariantUtils.CreateFrom<string[]>(ref array);
			return true;
		}
		if ((ref method) == MethodName._HandlesType && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			bool flag = ((ResourceFormatLoader)this)._HandlesType(VariantUtils.ConvertTo<StringName>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName._GetResourceType && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = ((ResourceFormatLoader)this)._GetResourceType(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref method) == MethodName._RecognizePath && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			bool flag2 = ((ResourceFormatLoader)this)._RecognizePath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<StringName>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<bool>(ref flag2);
			return true;
		}
		if ((ref method) == MethodName._Exists && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			bool flag3 = ((ResourceFormatLoader)this)._Exists(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<bool>(ref flag3);
			return true;
		}
		if ((ref method) == MethodName._Load && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			Variant val = ((ResourceFormatLoader)this)._Load(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = VariantUtils.CreateFrom<Variant>(ref val);
			return true;
		}
		if ((ref method) == MethodName._GetDependencies && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			string[] array2 = ((ResourceFormatLoader)this)._GetDependencies(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<string[]>(ref array2);
			return true;
		}
		if ((ref method) == MethodName.IsSpritePath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			bool flag4 = IsSpritePath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<bool>(ref flag4);
			return true;
		}
		if ((ref method) == MethodName.HasFallback && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			bool flag5 = HasFallback(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<bool>(ref flag5);
			return true;
		}
		if ((ref method) == MethodName.LoadFallback && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			Texture2D val2 = LoadFallback(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<Texture2D>(ref val2);
			return true;
		}
		if ((ref method) == MethodName.GetFallbackPath && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			string fallbackPath = GetFallbackPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<string>(ref fallbackPath);
			return true;
		}
		if ((ref method) == MethodName.GetRelicFallbackPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string relicFallbackPath = GetRelicFallbackPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref relicFallbackPath);
			return true;
		}
		if ((ref method) == MethodName.GetPowerFallbackPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string powerFallbackPath = GetPowerFallbackPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref powerFallbackPath);
			return true;
		}
		if ((ref method) == MethodName.GetCardFallbackPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string cardFallbackPath = GetCardFallbackPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref cardFallbackPath);
			return true;
		}
		if ((ref method) == MethodName.GetPotionFallbackPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string potionFallbackPath = GetPotionFallbackPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref potionFallbackPath);
			return true;
		}
		if ((ref method) == MethodName.GetMissingTexture && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Variant missingTexture = GetMissingTexture(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Variant>(ref missingTexture);
			return true;
		}
		return ((ResourceFormatLoader)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.IsSpritePath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			bool flag = IsSpritePath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.HasFallback && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			bool flag2 = HasFallback(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<bool>(ref flag2);
			return true;
		}
		if ((ref method) == MethodName.LoadFallback && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			Texture2D val = LoadFallback(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<Texture2D>(ref val);
			return true;
		}
		if ((ref method) == MethodName.GetFallbackPath && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			string fallbackPath = GetFallbackPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<string>(ref fallbackPath);
			return true;
		}
		if ((ref method) == MethodName.GetRelicFallbackPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string relicFallbackPath = GetRelicFallbackPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref relicFallbackPath);
			return true;
		}
		if ((ref method) == MethodName.GetPowerFallbackPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string powerFallbackPath = GetPowerFallbackPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref powerFallbackPath);
			return true;
		}
		if ((ref method) == MethodName.GetCardFallbackPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string cardFallbackPath = GetCardFallbackPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref cardFallbackPath);
			return true;
		}
		if ((ref method) == MethodName.GetPotionFallbackPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string potionFallbackPath = GetPotionFallbackPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref potionFallbackPath);
			return true;
		}
		if ((ref method) == MethodName.GetMissingTexture && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Variant missingTexture = GetMissingTexture(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Variant>(ref missingTexture);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._GetRecognizedExtensions)
		{
			return true;
		}
		if ((ref method) == MethodName._HandlesType)
		{
			return true;
		}
		if ((ref method) == MethodName._GetResourceType)
		{
			return true;
		}
		if ((ref method) == MethodName._RecognizePath)
		{
			return true;
		}
		if ((ref method) == MethodName._Exists)
		{
			return true;
		}
		if ((ref method) == MethodName._Load)
		{
			return true;
		}
		if ((ref method) == MethodName._GetDependencies)
		{
			return true;
		}
		if ((ref method) == MethodName.IsSpritePath)
		{
			return true;
		}
		if ((ref method) == MethodName.HasFallback)
		{
			return true;
		}
		if ((ref method) == MethodName.LoadFallback)
		{
			return true;
		}
		if ((ref method) == MethodName.GetFallbackPath)
		{
			return true;
		}
		if ((ref method) == MethodName.GetRelicFallbackPath)
		{
			return true;
		}
		if ((ref method) == MethodName.GetPowerFallbackPath)
		{
			return true;
		}
		if ((ref method) == MethodName.GetCardFallbackPath)
		{
			return true;
		}
		if ((ref method) == MethodName.GetPotionFallbackPath)
		{
			return true;
		}
		if ((ref method) == MethodName.GetMissingTexture)
		{
			return true;
		}
		return ((ResourceFormatLoader)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
	}
}
