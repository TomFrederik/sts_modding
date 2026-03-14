using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.TopBar;

[ScriptPath("res://src/Core/Nodes/TopBar/NTopBarButton.cs")]
public abstract class NTopBarButton : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName InitTopBarButton = StringName.op_Implicit("InitTopBarButton");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnEnable = StringName.op_Implicit("OnEnable");

		public new static readonly StringName OnDisable = StringName.op_Implicit("OnDisable");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName UpdateScreenOpen = StringName.op_Implicit("UpdateScreenOpen");

		public static readonly StringName OnScreenClosed = StringName.op_Implicit("OnScreenClosed");

		public static readonly StringName CancelAnimations = StringName.op_Implicit("CancelAnimations");

		public static readonly StringName IsOpen = StringName.op_Implicit("IsOpen");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName IsScreenOpen = StringName.op_Implicit("IsScreenOpen");

		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	protected Control _icon;

	protected ShaderMaterial? _hsv;

	private const float _hoverAngle = -(float)Math.PI / 15f;

	private const float _hoverShaderV = 1.1f;

	protected const float _hoverAnimDur = 0.5f;

	protected static readonly Vector2 _hoverScale = Vector2.One * 1.1f;

	private CancellationTokenSource? _hoverAnimCancelToken;

	private const float _defaultV = 1f;

	protected const float _unhoverAnimDur = 1f;

	private CancellationTokenSource? _unhoverAnimCancelToken;

	private const float _pressDownV = 0.4f;

	protected const float _pressDownDur = 0.25f;

	private CancellationTokenSource? _pressDownCancelToken;

	protected bool IsScreenOpen { get; private set; }

	public override void _Ready()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		ConnectSignals();
		_icon = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Control/Icon"));
		_hsv = (ShaderMaterial)((CanvasItem)_icon).Material;
	}

	protected void InitTopBarButton()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		ConnectSignals();
		_icon = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Control/Icon"));
		_hsv = (ShaderMaterial)((CanvasItem)_icon).Material;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		CancelAnimations();
	}

	protected override void OnRelease()
	{
		_pressDownCancelToken?.Cancel();
		_hoverAnimCancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(AnimHover(_hoverAnimCancelToken));
	}

	protected override void OnPress()
	{
		base.OnPress();
		_hoverAnimCancelToken?.Cancel();
		_pressDownCancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(AnimPressDown(_pressDownCancelToken));
	}

	protected virtual async Task AnimPressDown(CancellationTokenSource cancelToken)
	{
		float timer = 0f;
		float startAngle = _icon.Rotation;
		float targetAngle = startAngle + (float)Math.PI * 2f / 15f;
		for (; timer < 0.25f; timer += (float)((Node)this).GetProcessDeltaTime())
		{
			if (cancelToken.IsCancellationRequested)
			{
				return;
			}
			_icon.Rotation = Mathf.LerpAngle(startAngle, targetAngle, Ease.CubicOut(timer / 0.25f));
			ShaderMaterial? hsv = _hsv;
			if (hsv != null)
			{
				hsv.SetShaderParameter(_v, Variant.op_Implicit(Mathf.Lerp(1.1f, 0.4f, Ease.CubicOut(timer / 0.25f))));
			}
			if (!((Node?)(object)this).IsValid() || !((Node)this).IsInsideTree())
			{
				return;
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		_icon.Rotation = targetAngle;
		ShaderMaterial? hsv2 = _hsv;
		if (hsv2 != null)
		{
			hsv2.SetShaderParameter(_v, Variant.op_Implicit(0.4f));
		}
	}

	protected override void OnFocus()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		if (IsScreenOpen)
		{
			ShaderMaterial? hsv = _hsv;
			if (hsv != null)
			{
				hsv.SetShaderParameter(_v, Variant.op_Implicit(1.1f));
			}
			_icon.Scale = _hoverScale;
			return;
		}
		ShaderMaterial? hsv2 = _hsv;
		if (hsv2 != null)
		{
			hsv2.SetShaderParameter(_v, Variant.op_Implicit(1.1f));
		}
		_icon.Scale = _hoverScale;
		_unhoverAnimCancelToken?.Cancel();
		_hoverAnimCancelToken?.Cancel();
		_hoverAnimCancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(AnimHover(_hoverAnimCancelToken));
	}

	protected override void OnEnable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.OnEnable();
		((CanvasItem)this).Modulate = Colors.White;
	}

	protected override void OnDisable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.OnDisable();
		((CanvasItem)this).Modulate = StsColors.disabledTopBarButton;
	}

	protected virtual async Task AnimHover(CancellationTokenSource cancelToken)
	{
		float timer = 0f;
		float startAngle = _icon.Rotation;
		for (; timer < 0.5f; timer += (float)((Node)this).GetProcessDeltaTime())
		{
			if (cancelToken.IsCancellationRequested)
			{
				return;
			}
			_icon.Rotation = Mathf.LerpAngle(startAngle, -(float)Math.PI / 15f, Ease.BackOut(timer / 0.5f));
			if (!((Node?)(object)this).IsValid() || !((Node)this).IsInsideTree())
			{
				return;
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		_icon.Rotation = -(float)Math.PI / 15f;
	}

	protected override void OnUnfocus()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (IsScreenOpen)
		{
			_pressDownCancelToken?.Cancel();
			ShaderMaterial? hsv = _hsv;
			if (hsv != null)
			{
				hsv.SetShaderParameter(_v, Variant.op_Implicit(1f));
			}
			_icon.Scale = Vector2.One;
		}
		else
		{
			_hoverAnimCancelToken?.Cancel();
			_pressDownCancelToken?.Cancel();
			_unhoverAnimCancelToken?.Cancel();
			_unhoverAnimCancelToken = new CancellationTokenSource();
			TaskHelper.RunSafely(AnimUnhover(_unhoverAnimCancelToken));
		}
	}

	protected virtual async Task AnimUnhover(CancellationTokenSource cancelToken)
	{
		float timer = 0f;
		float startAngle = _icon.Rotation;
		for (; timer < 1f; timer += (float)((Node)this).GetProcessDeltaTime())
		{
			if (cancelToken.IsCancellationRequested)
			{
				return;
			}
			_icon.Rotation = Mathf.LerpAngle(startAngle, 0f, Ease.ElasticOut(timer / 1f));
			ShaderMaterial? hsv = _hsv;
			if (hsv != null)
			{
				hsv.SetShaderParameter(_v, Variant.op_Implicit(Mathf.Lerp(1.1f, 1f, Ease.ExpoOut(timer / 1f))));
			}
			_icon.Scale = ((Vector2)(ref _hoverScale)).Lerp(Vector2.One, Ease.ExpoOut(timer / 1f));
			if (!((Node?)(object)this).IsValid() || !((Node)this).IsInsideTree())
			{
				return;
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		ShaderMaterial? hsv2 = _hsv;
		if (hsv2 != null)
		{
			hsv2.SetShaderParameter(_v, Variant.op_Implicit(1f));
		}
		_icon.Rotation = 0f;
		_icon.Scale = Vector2.One;
	}

	protected void UpdateScreenOpen()
	{
		bool flag = IsOpen();
		if (IsScreenOpen != flag)
		{
			IsScreenOpen = flag;
			if (!IsScreenOpen)
			{
				OnScreenClosed();
			}
		}
	}

	private void OnScreenClosed()
	{
		CancelAnimations();
		_unhoverAnimCancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(AnimUnhover(_unhoverAnimCancelToken));
	}

	private void CancelAnimations()
	{
		_hoverAnimCancelToken?.Cancel();
		_pressDownCancelToken?.Cancel();
		_unhoverAnimCancelToken?.Cancel();
	}

	protected abstract bool IsOpen();

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitTopBarButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateScreenOpen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnScreenClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelAnimations, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsOpen, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitTopBarButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitTopBarButton();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
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
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateScreenOpen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateScreenOpen();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnScreenClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnScreenClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CancelAnimations && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CancelAnimations();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsOpen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = IsOpen();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.InitTopBarButton)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
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
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateScreenOpen)
		{
			return true;
		}
		if ((ref method) == MethodName.OnScreenClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.CancelAnimations)
		{
			return true;
		}
		if ((ref method) == MethodName.IsOpen)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.IsScreenOpen)
		{
			IsScreenOpen = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
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
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsScreenOpen)
		{
			bool isScreenOpen = IsScreenOpen;
			value = VariantUtils.CreateFrom<bool>(ref isScreenOpen);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<Control>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.IsScreenOpen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName isScreenOpen = PropertyName.IsScreenOpen;
		bool isScreenOpen2 = IsScreenOpen;
		info.AddProperty(isScreenOpen, Variant.From<bool>(ref isScreenOpen2));
		info.AddProperty(PropertyName._icon, Variant.From<Control>(ref _icon));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsScreenOpen, ref val))
		{
			IsScreenOpen = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val2))
		{
			_icon = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val3))
		{
			_hsv = ((Variant)(ref val3)).As<ShaderMaterial>();
		}
	}
}
