using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NLogoAnimation.cs")]
public class NLogoAnimation : Control, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _bg = StringName.op_Implicit("_bg");

		public static readonly StringName _logoContainer = StringName.op_Implicit("_logoContainer");

		public static readonly StringName _logoSpineNode = StringName.op_Implicit("_logoSpineNode");

		public static readonly StringName _logoBgColor = StringName.op_Implicit("_logoBgColor");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _cancelled = StringName.op_Implicit("_cancelled");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/screens/main_menu/logo_animation.tscn";

	private Control _bg;

	private Control _logoContainer;

	private Node2D _logoSpineNode;

	private MegaSprite _spineSprite;

	private Color _logoBgColor = new Color("074254FF");

	private Tween? _tween;

	private bool _cancelled;

	public static string[] AssetPaths => new string[1] { "res://scenes/screens/main_menu/logo_animation.tscn" };

	public Control? DefaultFocusedControl => null;

	public static NLogoAnimation Create()
	{
		return PreloadManager.Cache.GetScene("res://scenes/screens/main_menu/logo_animation.tscn").Instantiate<NLogoAnimation>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		_bg = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Bg"));
		_logoContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Container"));
		_logoSpineNode = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("Container/SpineSprite"));
		_spineSprite = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_logoSpineNode));
		((CanvasItem)_logoSpineNode).Visible = false;
		Rect2 bounds = _spineSprite.GetSkeleton().GetBounds();
		float num = Math.Min(((Control)this).Size.X * 0.33f / ((Rect2)(ref bounds)).Size.X, ((Control)this).Size.Y * 0.33f / ((Rect2)(ref bounds)).Size.Y);
		_logoSpineNode.Scale = num * Vector2.One;
		_logoSpineNode.Position = -((Rect2)(ref bounds)).Size * _logoSpineNode.Scale * 0.5f;
	}

	public async Task PlayAnimation(CancellationToken token)
	{
		if (token.IsCancellationRequested)
		{
			_cancelled = true;
			return;
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenInterval(1.0);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		if (token.IsCancellationRequested)
		{
			_cancelled = true;
			return;
		}
		((CanvasItem)_logoSpineNode).Visible = true;
		_spineSprite.GetAnimationState().SetAnimation("animation", loop: false);
		NDebugAudioManager.Instance.Play("SOTE_Logo_Echoing_ShortTail.mp3");
		_tween.Kill();
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_logoSpineNode, NodePath.op_Implicit("position:y"), Variant.op_Implicit(_logoSpineNode.Position.Y), 0.5).From(Variant.op_Implicit(_logoSpineNode.Position.Y - 800f)).SetEase((EaseType)1)
			.SetTrans((TransitionType)10);
		_tween.TweenProperty((GodotObject)(object)_logoContainer, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.5);
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		while (!_spineSprite.GetAnimationState().GetCurrent(0).IsComplete())
		{
			if (token.IsCancellationRequested)
			{
				_cancelled = true;
				_tween.Kill();
				_tween = ((Node)this).CreateTween().SetParallel(true);
				_tween.TweenProperty((GodotObject)(object)_logoContainer, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentWhite), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5);
				await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
				break;
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		if (!_cancelled)
		{
			_tween.Kill();
			_tween = ((Node)this).CreateTween();
			_tween.TweenProperty((GodotObject)(object)_bg, NodePath.op_Implicit("modulate"), Variant.op_Implicit(_logoBgColor), 2.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
			_tween.Chain();
			_tween.TweenInterval(1.0);
			await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NLogoAnimation nLogoAnimation = Create();
			ret = VariantUtils.CreateFrom<NLogoAnimation>(ref nLogoAnimation);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NLogoAnimation nLogoAnimation = Create();
			ret = VariantUtils.CreateFrom<NLogoAnimation>(ref nLogoAnimation);
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._bg)
		{
			_bg = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._logoContainer)
		{
			_logoContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._logoSpineNode)
		{
			_logoSpineNode = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._logoBgColor)
		{
			_logoBgColor = VariantUtils.ConvertTo<Color>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cancelled)
		{
			_cancelled = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._bg)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bg);
			return true;
		}
		if ((ref name) == PropertyName._logoContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _logoContainer);
			return true;
		}
		if ((ref name) == PropertyName._logoSpineNode)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _logoSpineNode);
			return true;
		}
		if ((ref name) == PropertyName._logoBgColor)
		{
			value = VariantUtils.CreateFrom<Color>(ref _logoBgColor);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._cancelled)
		{
			value = VariantUtils.CreateFrom<bool>(ref _cancelled);
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
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._bg, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._logoContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._logoSpineNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._logoBgColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._cancelled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._bg, Variant.From<Control>(ref _bg));
		info.AddProperty(PropertyName._logoContainer, Variant.From<Control>(ref _logoContainer));
		info.AddProperty(PropertyName._logoSpineNode, Variant.From<Node2D>(ref _logoSpineNode));
		info.AddProperty(PropertyName._logoBgColor, Variant.From<Color>(ref _logoBgColor));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._cancelled, Variant.From<bool>(ref _cancelled));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._bg, ref val))
		{
			_bg = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._logoContainer, ref val2))
		{
			_logoContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._logoSpineNode, ref val3))
		{
			_logoSpineNode = ((Variant)(ref val3)).As<Node2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._logoBgColor, ref val4))
		{
			_logoBgColor = ((Variant)(ref val4)).As<Color>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val5))
		{
			_tween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._cancelled, ref val6))
		{
			_cancelled = ((Variant)(ref val6)).As<bool>();
		}
	}
}
