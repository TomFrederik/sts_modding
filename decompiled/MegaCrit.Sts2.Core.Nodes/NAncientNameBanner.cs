using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.RichTextTags;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes;

[ScriptPath("res://src/Core/Nodes/NAncientNameBanner.cs")]
public class NAncientNameBanner : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName UpdateTransform = StringName.op_Implicit("UpdateTransform");

		public static readonly StringName UpdateGlyphSpace = StringName.op_Implicit("UpdateGlyphSpace");

		public static readonly StringName GetTextCenterGlyphIndex = StringName.op_Implicit("GetTextCenterGlyphIndex");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _titleLabel = StringName.op_Implicit("_titleLabel");

		public static readonly StringName _ancientBannerEffect = StringName.op_Implicit("_ancientBannerEffect");

		public static readonly StringName _epithetLabel = StringName.op_Implicit("_epithetLabel");

		public static readonly StringName _moveTween = StringName.op_Implicit("_moveTween");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private MegaRichTextLabel _titleLabel;

	private RichTextAncientBanner _ancientBannerEffect;

	private MegaLabel _epithetLabel;

	private static readonly string _path = SceneHelper.GetScenePath("ui/ancient_name_banner");

	private AncientEventModel _ancient;

	private Tween? _moveTween;

	private Tween? _tween;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_path);

	public static NAncientNameBanner? Create(AncientEventModel ancient)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NAncientNameBanner nAncientNameBanner = PreloadManager.Cache.GetScene(_path).Instantiate<NAncientNameBanner>((GenEditState)0);
		nAncientNameBanner._ancient = ancient;
		return nAncientNameBanner;
	}

	public override void _Ready()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		_titleLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Title"));
		string text = _ancient.Title.GetFormattedText().ToUpper();
		_ancientBannerEffect = new RichTextAncientBanner();
		_ancientBannerEffect.CenterCharacter = GetTextCenterGlyphIndex(text, ((Control)_titleLabel).GetThemeFont(ThemeConstants.RichTextLabel.normalFont, StringName.op_Implicit("RichTextLabel")), ((Control)_titleLabel).GetThemeFontSize(ThemeConstants.RichTextLabel.normalFontSize, StringName.op_Implicit("RichTextLabel")));
		((RichTextLabel)_titleLabel).InstallEffect(Variant.op_Implicit((GodotObject)(object)_ancientBannerEffect));
		((RichTextLabel)_titleLabel).BbcodeEnabled = true;
		_titleLabel.Text = "[ancient_banner]" + text + "[/ancient_banner]";
		_epithetLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Epithet"));
		_epithetLabel.SetTextAutoSize(_ancient.Epithet.GetFormattedText());
		TaskHelper.RunSafely(AnimateVfx());
	}

	private async Task AnimateVfx()
	{
		((Control)_epithetLabel).Position = new Vector2(0f, 18f);
		((CanvasItem)_epithetLabel).Modulate = Colors.Transparent;
		_moveTween = ((Node)this).CreateTween();
		_moveTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position:y"), Variant.op_Implicit(-100f), 4.0).SetEase((EaseType)1).SetTrans((TransitionType)8);
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateGlyphSpace), Variant.op_Implicit(1f), Variant.op_Implicit(0f), 3.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateTransform), Variant.op_Implicit(0f), Variant.op_Implicit(1f), 3.0).SetEase((EaseType)1).SetTrans((TransitionType)11);
		_tween.TweenProperty((GodotObject)(object)_epithetLabel, NodePath.op_Implicit("position:y"), Variant.op_Implicit(42f), 2.0).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.SetDelay(1.0);
		_tween.TweenProperty((GodotObject)(object)_epithetLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.0).SetDelay(1.5);
		_tween.Chain();
		_tween.TweenInterval(1.5);
		_tween.Chain();
		_tween.TweenProperty((GodotObject)(object)_titleLabel, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.Red), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenProperty((GodotObject)(object)_titleLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)_epithetLabel, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.Red), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenProperty((GodotObject)(object)_epithetLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		_moveTween.Kill();
		_moveTween = ((Node)this).CreateTween().SetParallel(true);
		((Control)this).Position = new Vector2(0f, -80f);
		((CanvasItem)_titleLabel).Modulate = Colors.White;
		((RichTextLabel)_titleLabel).HorizontalAlignment = (HorizontalAlignment)0;
		((RichTextLabel)_titleLabel).VerticalAlignment = (VerticalAlignment)2;
		((Control)_titleLabel).Position = Vector2.Zero;
		((Control)_titleLabel).AddThemeFontSizeOverride(ThemeConstants.RichTextLabel.normalFontSize, 54);
		((Control)_titleLabel).AddThemeColorOverride(ThemeConstants.RichTextLabel.fontOutlineColor, Colors.Transparent);
		((Control)_titleLabel).AddThemeColorOverride(ThemeConstants.RichTextLabel.fontShadowColor, Colors.Transparent);
		((Control)_titleLabel).AddThemeColorOverride(ThemeConstants.RichTextLabel.defaultColor, StsColors.cream);
		((Label)_epithetLabel).HorizontalAlignment = (HorizontalAlignment)0;
		((Label)_epithetLabel).VerticalAlignment = (VerticalAlignment)2;
		((CanvasItem)_epithetLabel).Modulate = new Color(1f, 1f, 1f, 0f);
		((Control)_epithetLabel).AddThemeFontSizeOverride(ThemeConstants.Label.fontSize, 18);
		((Control)_epithetLabel).AddThemeColorOverride(ThemeConstants.Label.fontOutlineColor, Colors.Transparent);
		((Control)_epithetLabel).AddThemeColorOverride(ThemeConstants.Label.fontShadowColor, Colors.Transparent);
		((Control)_epithetLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, StsColors.cream);
		_moveTween.TweenProperty((GodotObject)(object)_epithetLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.5f), 2.0).SetEase((EaseType)1).SetTrans((TransitionType)8);
		_moveTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position:x"), Variant.op_Implicit(48f), 2.0).SetEase((EaseType)1).From(Variant.op_Implicit(0))
			.SetTrans((TransitionType)8);
	}

	private void UpdateTransform(float obj)
	{
		_ancientBannerEffect.Rotation = obj;
	}

	private void UpdateGlyphSpace(float spacing)
	{
		_ancientBannerEffect.Spacing = spacing * 1000f;
	}

	private float GetTextCenterGlyphIndex(string text, Font font, int fontSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		TextParagraph val = new TextParagraph();
		try
		{
			Variant val2 = default(Variant);
			val.AddString(text, font, fontSize, "", val2);
			float num = 0f;
			TextServer primaryInterface = TextServerManager.Singleton.GetPrimaryInterface();
			Array<Dictionary> val3 = primaryInterface.ShapedTextGetGlyphs(val.GetLineRid(0));
			foreach (Dictionary item in val3)
			{
				val2 = ((IReadOnlyDictionary<Variant, Variant>)item).GetValueOrDefault(Variant.op_Implicit("advance"));
				float num2 = ((Variant)(ref val2)).AsSingle();
				num += num2;
			}
			float num3 = 0f;
			int num4 = 0;
			foreach (Dictionary item2 in val3)
			{
				val2 = ((IReadOnlyDictionary<Variant, Variant>)item2).GetValueOrDefault(Variant.op_Implicit("advance"));
				float num5 = ((Variant)(ref val2)).AsSingle();
				num3 += num5;
				if (num3 > num * 0.5f)
				{
					return (float)num4 + (num * 0.5f - (num3 - num5)) / num5;
				}
				num4++;
			}
			return 0f;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void _ExitTree()
	{
		Tween? moveTween = _moveTween;
		if (moveTween != null)
		{
			moveTween.Kill();
		}
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		((Control)_titleLabel).RemoveThemeFontSizeOverride(ThemeConstants.RichTextLabel.normalFontSize);
		((Control)_titleLabel).RemoveThemeColorOverride(ThemeConstants.RichTextLabel.fontOutlineColor);
		((Control)_titleLabel).RemoveThemeColorOverride(ThemeConstants.RichTextLabel.fontShadowColor);
		((Control)_titleLabel).RemoveThemeColorOverride(ThemeConstants.RichTextLabel.defaultColor);
		((Control)_epithetLabel).RemoveThemeFontSizeOverride(ThemeConstants.Label.fontSize);
		((Control)_epithetLabel).RemoveThemeColorOverride(ThemeConstants.Label.fontOutlineColor);
		((Control)_epithetLabel).RemoveThemeColorOverride(ThemeConstants.Label.fontShadowColor);
		((Control)_epithetLabel).RemoveThemeColorOverride(ThemeConstants.Label.fontColor);
		((Node)this)._ExitTree();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateTransform, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("obj"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateGlyphSpace, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("spacing"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetTextCenterGlyphIndex, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)24, StringName.op_Implicit("font"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Font"), false),
			new PropertyInfo((Type)2, StringName.op_Implicit("fontSize"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateTransform && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateTransform(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateGlyphSpace && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateGlyphSpace(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetTextCenterGlyphIndex && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			float textCenterGlyphIndex = GetTextCenterGlyphIndex(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Font>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<float>(ref textCenterGlyphIndex);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateTransform)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateGlyphSpace)
		{
			return true;
		}
		if ((ref method) == MethodName.GetTextCenterGlyphIndex)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._titleLabel)
		{
			_titleLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ancientBannerEffect)
		{
			_ancientBannerEffect = VariantUtils.ConvertTo<RichTextAncientBanner>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._epithetLabel)
		{
			_epithetLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._moveTween)
		{
			_moveTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._titleLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _titleLabel);
			return true;
		}
		if ((ref name) == PropertyName._ancientBannerEffect)
		{
			value = VariantUtils.CreateFrom<RichTextAncientBanner>(ref _ancientBannerEffect);
			return true;
		}
		if ((ref name) == PropertyName._epithetLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _epithetLabel);
			return true;
		}
		if ((ref name) == PropertyName._moveTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _moveTween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._titleLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ancientBannerEffect, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._epithetLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._moveTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._titleLabel, Variant.From<MegaRichTextLabel>(ref _titleLabel));
		info.AddProperty(PropertyName._ancientBannerEffect, Variant.From<RichTextAncientBanner>(ref _ancientBannerEffect));
		info.AddProperty(PropertyName._epithetLabel, Variant.From<MegaLabel>(ref _epithetLabel));
		info.AddProperty(PropertyName._moveTween, Variant.From<Tween>(ref _moveTween));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._titleLabel, ref val))
		{
			_titleLabel = ((Variant)(ref val)).As<MegaRichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._ancientBannerEffect, ref val2))
		{
			_ancientBannerEffect = ((Variant)(ref val2)).As<RichTextAncientBanner>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._epithetLabel, ref val3))
		{
			_epithetLabel = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._moveTween, ref val4))
		{
			_moveTween = ((Variant)(ref val4)).As<Tween>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val5))
		{
			_tween = ((Variant)(ref val5)).As<Tween>();
		}
	}
}
