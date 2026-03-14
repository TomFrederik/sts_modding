using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Achievements;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.StatsScreen;

[ScriptPath("res://src/Core/Nodes/Screens/StatsScreen/NAchievementHolder.cs")]
public class NAchievementHolder : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshUnlocked = StringName.op_Implicit("RefreshUnlocked");

		public static readonly StringName SetLockVisuals = StringName.op_Implicit("SetLockVisuals");

		public static readonly StringName SetDateLabel = StringName.op_Implicit("SetDateLabel");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName IsUnlocked = StringName.op_Implicit("IsUnlocked");

		public static readonly StringName _border = StringName.op_Implicit("_border");

		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _lock = StringName.op_Implicit("_lock");

		public static readonly StringName _iconHsv = StringName.op_Implicit("_iconHsv");

		public static readonly StringName _borderHsv = StringName.op_Implicit("_borderHsv");

		public static readonly StringName _infoLabel = StringName.op_Implicit("_infoLabel");

		public static readonly StringName _date = StringName.op_Implicit("_date");

		public static readonly StringName _achievement = StringName.op_Implicit("_achievement");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private static readonly StringName _h = new StringName("h");

	private const string _scenePath = "screens/stats_screen/achievement_holder";

	private TextureRect _border;

	private TextureRect _icon;

	private TextureRect _lock;

	private ShaderMaterial _iconHsv;

	private ShaderMaterial _borderHsv;

	private MegaRichTextLabel _infoLabel;

	private MegaLabel _date;

	private Achievement _achievement;

	private Tween? _tween;

	public bool IsUnlocked { get; private set; }

	public static NAchievementHolder? Create(Achievement achievement)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NAchievementHolder nAchievementHolder = PreloadManager.Cache.GetScene(SceneHelper.GetScenePath("screens/stats_screen/achievement_holder")).Instantiate<NAchievementHolder>((GenEditState)0);
		nAchievementHolder._achievement = achievement;
		nAchievementHolder.IsUnlocked = AchievementsUtil.IsUnlocked(achievement);
		return nAchievementHolder;
	}

	public override void _Ready()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		_border = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Border"));
		_lock = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Lock"));
		_borderHsv = (ShaderMaterial)((CanvasItem)_border).Material;
		_iconHsv = (ShaderMaterial)((CanvasItem)_icon).Material;
		_infoLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%InfoText"));
		_date = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%DateText"));
		RefreshUnlocked();
	}

	public static string GetPathForAchievement(Enum achievement)
	{
		string text = StringHelper.SnakeCase(achievement.ToString()).ToLowerInvariant();
		return ImageHelper.GetImagePath("packed/achievements/unlocked/" + text + ".png");
	}

	public void RefreshUnlocked()
	{
		IsUnlocked = AchievementsUtil.IsUnlocked(_achievement);
		string text = StringHelper.SnakeCase(_achievement.ToString()).ToLowerInvariant();
		_icon.Texture = PreloadManager.Cache.GetTexture2D(GetPathForAchievement(_achievement));
		text = text.ToUpperInvariant();
		if (IsUnlocked)
		{
			_infoLabel.Text = "[b][gold]" + new LocString("achievements", text + ".title").GetRawText() + "[/gold][/b]\n" + new LocString("achievements", text + ".description").GetFormattedText();
		}
		else
		{
			_infoLabel.Text = "[b][red]" + new LocString("achievements", "LOCKED.title").GetRawText() + "[/red][/b]\n" + new LocString("achievements", text + ".description").GetFormattedText();
		}
		SetLockVisuals();
		SetDateLabel();
	}

	private void SetLockVisuals()
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_lock).Visible = !IsUnlocked;
		if (IsUnlocked)
		{
			_borderHsv.SetShaderParameter(_h, Variant.op_Implicit(1f));
			_borderHsv.SetShaderParameter(_s, Variant.op_Implicit(1f));
			_borderHsv.SetShaderParameter(_v, Variant.op_Implicit(1f));
			_iconHsv.SetShaderParameter(_s, Variant.op_Implicit(1f));
			_iconHsv.SetShaderParameter(_v, Variant.op_Implicit(1f));
		}
		else
		{
			_borderHsv.SetShaderParameter(_h, Variant.op_Implicit(0.4f));
			_borderHsv.SetShaderParameter(_s, Variant.op_Implicit(0.4f));
			_borderHsv.SetShaderParameter(_v, Variant.op_Implicit(0.8f));
			_iconHsv.SetShaderParameter(_s, Variant.op_Implicit(0.2f));
			_iconHsv.SetShaderParameter(_v, Variant.op_Implicit(0.5f));
		}
	}

	private void SetDateLabel()
	{
		((CanvasItem)_date).Visible = IsUnlocked;
		if (IsUnlocked)
		{
			if (!SaveManager.Instance.Progress.UnlockedAchievements.TryGetValue(_achievement, out var value))
			{
				((CanvasItem)_date).Visible = false;
				return;
			}
			DateTimeFormatInfo dateTimeFormat = LocManager.Instance.CultureInfo.DateTimeFormat;
			DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime, TimeZoneInfo.Local);
			LocString locString = new LocString("achievements", "UNLOCK_DATE.text");
			string variable = dateTime.ToString(new LocString("achievements", "UNLOCK_DATE.format").GetRawText(), dateTimeFormat);
			locString.Add("Date", variable);
			_date.SetTextAutoSize(locString.GetFormattedText());
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("achievement"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshUnlocked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetLockVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetDateLabel, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NAchievementHolder nAchievementHolder = Create(VariantUtils.ConvertTo<Achievement>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NAchievementHolder>(ref nAchievementHolder);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshUnlocked && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshUnlocked();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetLockVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetLockVisuals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetDateLabel && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetDateLabel();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NAchievementHolder nAchievementHolder = Create(VariantUtils.ConvertTo<Achievement>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NAchievementHolder>(ref nAchievementHolder);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshUnlocked)
		{
			return true;
		}
		if ((ref method) == MethodName.SetLockVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.SetDateLabel)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.IsUnlocked)
		{
			IsUnlocked = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._border)
		{
			_border = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lock)
		{
			_lock = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._iconHsv)
		{
			_iconHsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._borderHsv)
		{
			_borderHsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._infoLabel)
		{
			_infoLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._date)
		{
			_date = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._achievement)
		{
			_achievement = VariantUtils.ConvertTo<Achievement>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsUnlocked)
		{
			bool isUnlocked = IsUnlocked;
			value = VariantUtils.CreateFrom<bool>(ref isUnlocked);
			return true;
		}
		if ((ref name) == PropertyName._border)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _border);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._lock)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _lock);
			return true;
		}
		if ((ref name) == PropertyName._iconHsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _iconHsv);
			return true;
		}
		if ((ref name) == PropertyName._borderHsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _borderHsv);
			return true;
		}
		if ((ref name) == PropertyName._infoLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _infoLabel);
			return true;
		}
		if ((ref name) == PropertyName._date)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _date);
			return true;
		}
		if ((ref name) == PropertyName._achievement)
		{
			value = VariantUtils.CreateFrom<Achievement>(ref _achievement);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._border, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lock, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._iconHsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._borderHsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infoLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._date, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsUnlocked, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._achievement, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName isUnlocked = PropertyName.IsUnlocked;
		bool isUnlocked2 = IsUnlocked;
		info.AddProperty(isUnlocked, Variant.From<bool>(ref isUnlocked2));
		info.AddProperty(PropertyName._border, Variant.From<TextureRect>(ref _border));
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._lock, Variant.From<TextureRect>(ref _lock));
		info.AddProperty(PropertyName._iconHsv, Variant.From<ShaderMaterial>(ref _iconHsv));
		info.AddProperty(PropertyName._borderHsv, Variant.From<ShaderMaterial>(ref _borderHsv));
		info.AddProperty(PropertyName._infoLabel, Variant.From<MegaRichTextLabel>(ref _infoLabel));
		info.AddProperty(PropertyName._date, Variant.From<MegaLabel>(ref _date));
		info.AddProperty(PropertyName._achievement, Variant.From<Achievement>(ref _achievement));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsUnlocked, ref val))
		{
			IsUnlocked = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._border, ref val2))
		{
			_border = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val3))
		{
			_icon = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._lock, ref val4))
		{
			_lock = ((Variant)(ref val4)).As<TextureRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._iconHsv, ref val5))
		{
			_iconHsv = ((Variant)(ref val5)).As<ShaderMaterial>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._borderHsv, ref val6))
		{
			_borderHsv = ((Variant)(ref val6)).As<ShaderMaterial>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._infoLabel, ref val7))
		{
			_infoLabel = ((Variant)(ref val7)).As<MegaRichTextLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._date, ref val8))
		{
			_date = ((Variant)(ref val8)).As<MegaLabel>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._achievement, ref val9))
		{
			_achievement = ((Variant)(ref val9)).As<Achievement>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val10))
		{
			_tween = ((Variant)(ref val10)).As<Tween>();
		}
	}
}
