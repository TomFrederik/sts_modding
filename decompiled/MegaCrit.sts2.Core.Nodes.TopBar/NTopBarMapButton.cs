using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MegaCrit.Sts2.Core.Nodes.TopBar;

[ScriptPath("res://src/Core/Nodes/TopBar/NTopBarMapButton.cs")]
public class NTopBarMapButton : NTopBarButton
{
	public new class MethodName : NTopBarButton.MethodName
	{
		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName IsOpen = StringName.op_Implicit("IsOpen");

		public static readonly StringName StartOscillation = StringName.op_Implicit("StartOscillation");

		public static readonly StringName StopOscillation = StringName.op_Implicit("StopOscillation");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public new class PropertyName : NTopBarButton.PropertyName
	{
		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");

		public static readonly StringName _oscillateTween = StringName.op_Implicit("_oscillateTween");
	}

	public new class SignalName : NTopBarButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly HoverTip _hoverTip = new HoverTip(new LocString("static_hover_tips", "MAP.title"), new LocString("static_hover_tips", "MAP.description"));

	private const float _defaultV = 0.9f;

	private Tween? _oscillateTween;

	protected override string[] Hotkeys => new string[1] { StringName.op_Implicit(MegaInput.viewMap) };

	protected override void OnRelease()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		base.OnRelease();
		if (IsOpen())
		{
			NCapstoneContainer? instance = NCapstoneContainer.Instance;
			if (instance != null && instance.InUse)
			{
				NCapstoneContainer.Instance.Close();
			}
			else
			{
				NMapScreen.Instance.Close();
			}
		}
		else
		{
			NCapstoneContainer.Instance.Close();
			NMapScreen.Instance.Open(isOpenedFromTopBar: true);
		}
		ShaderMaterial? hsv = _hsv;
		if (hsv != null)
		{
			hsv.SetShaderParameter(_v, Variant.op_Implicit(0.9f));
		}
	}

	protected override bool IsOpen()
	{
		return ((CanvasItem)NMapScreen.Instance).Visible;
	}

	public void StartOscillation()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Tween? oscillateTween = _oscillateTween;
		if (oscillateTween != null)
		{
			oscillateTween.Kill();
		}
		_oscillateTween = ((Node)this).CreateTween();
		_oscillateTween.SetLoops(0);
		_oscillateTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("rotation"), Variant.op_Implicit(-0.12f), 0.8).SetTrans((TransitionType)1).SetEase((EaseType)2);
		_oscillateTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("rotation"), Variant.op_Implicit(0.12f), 0.8).SetTrans((TransitionType)1).SetEase((EaseType)2);
	}

	public void StopOscillation()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Tween? oscillateTween = _oscillateTween;
		if (oscillateTween != null)
		{
			oscillateTween.Kill();
		}
		_oscillateTween = ((Node)this).CreateTween();
		_oscillateTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("rotation"), Variant.op_Implicit(0f), 0.5).SetTrans((TransitionType)11).SetEase((EaseType)1);
	}

	protected override void OnFocus()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _hoverTip);
		((Control)nHoverTipSet).GlobalPosition = ((Control)this).GlobalPosition + new Vector2(((Control)this).Size.X - ((Control)nHoverTipSet).Size.X, ((Control)this).Size.Y + 20f);
	}

	protected override void OnUnfocus()
	{
		base.OnUnfocus();
		NHoverTipSet.Remove((Control)(object)this);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsOpen, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartOscillation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopOscillation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsOpen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = IsOpen();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.StartOscillation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartOscillation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopOscillation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopOscillation();
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
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.IsOpen)
		{
			return true;
		}
		if ((ref method) == MethodName.StartOscillation)
		{
			return true;
		}
		if ((ref method) == MethodName.StopOscillation)
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
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._oscillateTween)
		{
			_oscillateTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Hotkeys)
		{
			string[] hotkeys = Hotkeys;
			value = VariantUtils.CreateFrom<string[]>(ref hotkeys);
			return true;
		}
		if ((ref name) == PropertyName._oscillateTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _oscillateTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._oscillateTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._oscillateTween, Variant.From<Tween>(ref _oscillateTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._oscillateTween, ref val))
		{
			_oscillateTween = ((Variant)(ref val)).As<Tween>();
		}
	}
}
