using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.Fonts;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NShortSubmenuButton.cs")]
public class NShortSubmenuButton : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public static readonly StringName GetImagePath = StringName.op_Implicit("GetImagePath");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName ConnectSignals = StringName.op_Implicit("ConnectSignals");

		public static readonly StringName SetIconAndLocalization = StringName.op_Implicit("SetIconAndLocalization");

		public static readonly StringName _Notification = StringName.op_Implicit("_Notification");

		public static readonly StringName RefreshLabels = StringName.op_Implicit("RefreshLabels");

		public new static readonly StringName OnEnable = StringName.op_Implicit("OnEnable");

		public new static readonly StringName OnDisable = StringName.op_Implicit("OnDisable");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public static readonly StringName UpdateShaderParam = StringName.op_Implicit("UpdateShaderParam");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName _bgPanel = StringName.op_Implicit("_bgPanel");

		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _title = StringName.op_Implicit("_title");

		public static readonly StringName _description = StringName.op_Implicit("_description");

		public static readonly StringName _locKeyPrefix = StringName.op_Implicit("_locKeyPrefix");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _defaultV = StringName.op_Implicit("_defaultV");

		public static readonly StringName _focusV = StringName.op_Implicit("_focusV");

		public static readonly StringName _pressV = StringName.op_Implicit("_pressV");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private Control _bgPanel;

	private TextureRect _icon;

	private MegaLabel _title;

	private MegaRichTextLabel _description;

	private string? _locKeyPrefix;

	private ShaderMaterial _hsv;

	private static readonly StringName _v = new StringName("v");

	private float _defaultV;

	private float _focusV;

	private float _pressV;

	private Tween? _tween;

	public static string? GetImagePath(string key)
	{
		string imagePath = ImageHelper.GetImagePath("packed/main_menu/submenu_icon_" + key.ToLowerInvariant() + ".png");
		if (ResourceLoader.Exists(imagePath, ""))
		{
			return imagePath;
		}
		return null;
	}

	public override void _Ready()
	{
		if (((object)this).GetType() != typeof(NShortSubmenuButton))
		{
			throw new InvalidOperationException("Don't call base._Ready(). Use ConnectSignals() instead.");
		}
		ConnectSignals();
	}

	protected override void ConnectSignals()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		base.ConnectSignals();
		_bgPanel = ((Node)this).GetNode<Control>(NodePath.op_Implicit("BgPanel"));
		_hsv = (ShaderMaterial)((CanvasItem)_bgPanel).Material;
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Icon"));
		_title = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Title"));
		_description = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Description"));
		_defaultV = (float)_hsv.GetShaderParameter(_v);
		_focusV = _defaultV + 0.2f;
		_pressV = _defaultV - 0.2f;
	}

	public void SetIconAndLocalization(string locKeyPrefix)
	{
		_locKeyPrefix = locKeyPrefix;
		RefreshLabels();
		string imagePath = GetImagePath(locKeyPrefix);
		if (imagePath != null)
		{
			_icon.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D(imagePath);
		}
	}

	public override void _Notification(int what)
	{
		if ((long)what == 2010 && _locKeyPrefix != null && ((Node)this).IsNodeReady())
		{
			RefreshLabels();
		}
	}

	private void RefreshLabels()
	{
		LocString locString = new LocString("main_menu_ui", _locKeyPrefix + ".title");
		_title.SetTextAutoSize(locString.GetFormattedText());
		((Control)(object)_title).ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
		LocString locString2;
		if (base.IsEnabled)
		{
			locString2 = new LocString("main_menu_ui", _locKeyPrefix + ".description");
		}
		else
		{
			locString2 = new LocString("main_menu_ui", _locKeyPrefix + ".LOCKED.description");
			if (!locString2.Exists())
			{
				Log.Warn($"Short submenu button {((Node)this).Name} tried to find locked description for {_locKeyPrefix} but couldn't");
				locString2 = new LocString("main_menu_ui", _locKeyPrefix + ".description");
			}
		}
		_description.Text = locString2.GetFormattedText();
		((Control)(object)_description).ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.RichTextLabel.normalFont);
		((Control)(object)_description).ApplyLocaleFontSubstitution(FontType.Bold, ThemeConstants.RichTextLabel.boldFont);
		((Control)(object)_description).ApplyLocaleFontSubstitution(FontType.Italic, ThemeConstants.RichTextLabel.italicsFont);
	}

	protected override void OnEnable()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Modulate = Colors.White;
		((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%Lock"))).Visible = false;
	}

	protected override void OnDisable()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Modulate = Colors.DarkGray;
		((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%Lock"))).Visible = true;
	}

	protected override void OnFocus()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.02f), 0.05);
		_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderParam), _hsv.GetShaderParameter(_v), Variant.op_Implicit(_focusV), 0.05);
	}

	protected override void OnUnfocus()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderParam), _hsv.GetShaderParameter(_v), Variant.op_Implicit(_defaultV), 0.3);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.3);
	}

	protected override void OnPress()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		base.OnPress();
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.98f), 0.2).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.lightGray), 0.2);
	}

	protected override void OnRelease()
	{
		OnUnfocus();
	}

	private void UpdateShaderParam(float newV)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(newV));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName.GetImagePath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("key"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetIconAndLocalization, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("locKeyPrefix"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Notification, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("what"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshLabels, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderParam, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("newV"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.GetImagePath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string imagePath = GetImagePath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref imagePath);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetIconAndLocalization && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetIconAndLocalization(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Notification && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((GodotObject)this)._Notification(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshLabels && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshLabels();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEnable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEnable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDisable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDisable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderParam && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderParam(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.GetImagePath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string imagePath = GetImagePath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref imagePath);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.GetImagePath)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals)
		{
			return true;
		}
		if ((ref method) == MethodName.SetIconAndLocalization)
		{
			return true;
		}
		if ((ref method) == MethodName._Notification)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshLabels)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEnable)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDisable)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderParam)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._bgPanel)
		{
			_bgPanel = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._title)
		{
			_title = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			_description = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._locKeyPrefix)
		{
			_locKeyPrefix = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultV)
		{
			_defaultV = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._focusV)
		{
			_focusV = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pressV)
		{
			_pressV = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._bgPanel)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bgPanel);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._title)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _title);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _description);
			return true;
		}
		if ((ref name) == PropertyName._locKeyPrefix)
		{
			value = VariantUtils.CreateFrom<string>(ref _locKeyPrefix);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._defaultV)
		{
			value = VariantUtils.CreateFrom<float>(ref _defaultV);
			return true;
		}
		if ((ref name) == PropertyName._focusV)
		{
			value = VariantUtils.CreateFrom<float>(ref _focusV);
			return true;
		}
		if ((ref name) == PropertyName._pressV)
		{
			value = VariantUtils.CreateFrom<float>(ref _pressV);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._bgPanel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._title, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._description, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._locKeyPrefix, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._defaultV, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._focusV, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._pressV, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._bgPanel, Variant.From<Control>(ref _bgPanel));
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._title, Variant.From<MegaLabel>(ref _title));
		info.AddProperty(PropertyName._description, Variant.From<MegaRichTextLabel>(ref _description));
		info.AddProperty(PropertyName._locKeyPrefix, Variant.From<string>(ref _locKeyPrefix));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._defaultV, Variant.From<float>(ref _defaultV));
		info.AddProperty(PropertyName._focusV, Variant.From<float>(ref _focusV));
		info.AddProperty(PropertyName._pressV, Variant.From<float>(ref _pressV));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._bgPanel, ref val))
		{
			_bgPanel = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val2))
		{
			_icon = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._title, ref val3))
		{
			_title = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._description, ref val4))
		{
			_description = ((Variant)(ref val4)).As<MegaRichTextLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._locKeyPrefix, ref val5))
		{
			_locKeyPrefix = ((Variant)(ref val5)).As<string>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val6))
		{
			_hsv = ((Variant)(ref val6)).As<ShaderMaterial>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultV, ref val7))
		{
			_defaultV = ((Variant)(ref val7)).As<float>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._focusV, ref val8))
		{
			_focusV = ((Variant)(ref val8)).As<float>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._pressV, ref val9))
		{
			_pressV = ((Variant)(ref val9)).As<float>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val10))
		{
			_tween = ((Variant)(ref val10)).As<Tween>();
		}
	}
}
