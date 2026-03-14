using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NMainMenuBg.cs")]
public class NMainMenuBg : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnWindowChange = StringName.op_Implicit("OnWindowChange");

		public static readonly StringName ScaleBgIfNarrow = StringName.op_Implicit("ScaleBgIfNarrow");

		public static readonly StringName HideLogo = StringName.op_Implicit("HideLogo");

		public static readonly StringName ShowLogo = StringName.op_Implicit("ShowLogo");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _window = StringName.op_Implicit("_window");

		public static readonly StringName _bg = StringName.op_Implicit("_bg");

		public static readonly StringName _logo = StringName.op_Implicit("_logo");

		public static readonly StringName _logoTween = StringName.op_Implicit("_logoTween");
	}

	public class SignalName : SignalName
	{
	}

	private Window _window;

	private Control _bg;

	private Node2D _logo;

	private Tween? _logoTween;

	private static readonly Vector2 _defaultBgScale = Vector2.One * 1.01f;

	private const float _bgScaleRatioThreshold = 1.5f;

	public override void _Ready()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		_bg = ((Node)this).GetNode<Control>(NodePath.op_Implicit("BgContainer"));
		_logo = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%Logo"));
		_window = ((Node)this).GetTree().Root;
		((GodotObject)_window).Connect(SignalName.SizeChanged, Callable.From((Action)OnWindowChange), 0u);
	}

	private void OnWindowChange()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		ScaleBgIfNarrow((float)_window.Size.X / (float)_window.Size.Y);
	}

	private void ScaleBgIfNarrow(float ratio)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (ratio < 1.5f)
		{
			_bg.Scale = Vector2.One * 1.04f;
		}
		else
		{
			_bg.Scale = _defaultBgScale;
		}
	}

	public void HideLogo()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Tween? logoTween = _logoTween;
		if (logoTween != null)
		{
			logoTween.Kill();
		}
		_logoTween = ((Node)this).CreateTween();
		_logoTween.TweenProperty((GodotObject)(object)_logo, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	public void ShowLogo()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Tween? logoTween = _logoTween;
		if (logoTween != null)
		{
			logoTween.Kill();
		}
		_logoTween = ((Node)this).CreateTween();
		_logoTween.TweenProperty((GodotObject)(object)_logo, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ScaleBgIfNarrow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("ratio"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideLogo, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowLogo, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnWindowChange();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ScaleBgIfNarrow && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ScaleBgIfNarrow(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideLogo && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideLogo();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowLogo && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowLogo();
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
		if ((ref method) == MethodName.OnWindowChange)
		{
			return true;
		}
		if ((ref method) == MethodName.ScaleBgIfNarrow)
		{
			return true;
		}
		if ((ref method) == MethodName.HideLogo)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowLogo)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._window)
		{
			_window = VariantUtils.ConvertTo<Window>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bg)
		{
			_bg = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._logo)
		{
			_logo = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._logoTween)
		{
			_logoTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._window)
		{
			value = VariantUtils.CreateFrom<Window>(ref _window);
			return true;
		}
		if ((ref name) == PropertyName._bg)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bg);
			return true;
		}
		if ((ref name) == PropertyName._logo)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _logo);
			return true;
		}
		if ((ref name) == PropertyName._logoTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _logoTween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._window, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bg, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._logo, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._logoTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._window, Variant.From<Window>(ref _window));
		info.AddProperty(PropertyName._bg, Variant.From<Control>(ref _bg));
		info.AddProperty(PropertyName._logo, Variant.From<Node2D>(ref _logo));
		info.AddProperty(PropertyName._logoTween, Variant.From<Tween>(ref _logoTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._window, ref val))
		{
			_window = ((Variant)(ref val)).As<Window>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._bg, ref val2))
		{
			_bg = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._logo, ref val3))
		{
			_logo = ((Variant)(ref val3)).As<Node2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._logoTween, ref val4))
		{
			_logoTween = ((Variant)(ref val4)).As<Tween>();
		}
	}
}
