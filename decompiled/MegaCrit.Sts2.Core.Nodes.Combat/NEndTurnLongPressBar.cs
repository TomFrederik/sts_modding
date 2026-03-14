using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NEndTurnLongPressBar.cs")]
public class NEndTurnLongPressBar : ColorRect
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Init = StringName.op_Implicit("Init");

		public static readonly StringName StartPress = StringName.op_Implicit("StartPress");

		public static readonly StringName CancelPress = StringName.op_Implicit("CancelPress");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName RecalculateBar = StringName.op_Implicit("RecalculateBar");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _outline = StringName.op_Implicit("_outline");

		public static readonly StringName _pressTimer = StringName.op_Implicit("_pressTimer");

		public static readonly StringName _isPressed = StringName.op_Implicit("_isPressed");

		public static readonly StringName _endTurnButton = StringName.op_Implicit("_endTurnButton");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _enabled = StringName.op_Implicit("_enabled");
	}

	public class SignalName : SignalName
	{
	}

	private Control _outline;

	private double _pressTimer;

	private const double _longPressDuration = 0.5;

	private bool _isPressed;

	private const float _targetWidth = 204f;

	private NEndTurnButton _endTurnButton;

	private Tween? _tween;

	private bool _enabled = true;

	public override void _Ready()
	{
		_outline = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BarOutline"));
	}

	public void Init(NEndTurnButton endTurnButton)
	{
		_endTurnButton = endTurnButton;
	}

	public void StartPress()
	{
		_isPressed = true;
	}

	public void CancelPress()
	{
		_isPressed = false;
	}

	public override void _Process(double delta)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (!_enabled)
		{
			return;
		}
		if (_isPressed)
		{
			_pressTimer += delta;
			if (_pressTimer > 0.5)
			{
				_enabled = false;
				((Control)this).Size = new Vector2(204f, 6f);
				_pressTimer = 0.0;
				_endTurnButton.CallReleaseLogic();
				TaskHelper.RunSafely(PlayAnim());
			}
			else
			{
				RecalculateBar();
			}
		}
		else if (_pressTimer > 0.0)
		{
			_pressTimer -= delta;
			if (_pressTimer < 0.0)
			{
				_pressTimer = 0.0;
				Color modulate = ((CanvasItem)this).Modulate;
				modulate.A = 0f;
				((CanvasItem)this).Modulate = modulate;
			}
			else
			{
				RecalculateBar();
			}
		}
	}

	private void RecalculateBar()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)(_pressTimer / 0.5);
		((Control)this).Size = new Vector2(num * 204f, 6f);
		((ColorRect)this).Color = new Color(num * 2.5f, 0.6f + num, 0.6f, 1f);
		Color modulate = ((CanvasItem)this).Modulate;
		modulate.A = Ease.CubicOut(num * 0.75f);
		((CanvasItem)this).Modulate = modulate;
	}

	private async Task PlayAnim()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		((ColorRect)this).Color = new Color(1f, 0.85f, 0.36f, 1f);
		_isPressed = false;
		_enabled = true;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Init, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("endTurnButton"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RecalculateBar, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Init && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Init(VariantUtils.ConvertTo<NEndTurnButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CancelPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CancelPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RecalculateBar && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RecalculateBar();
			ret = default(godot_variant);
			return true;
		}
		return ((ColorRect)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.Init)
		{
			return true;
		}
		if ((ref method) == MethodName.StartPress)
		{
			return true;
		}
		if ((ref method) == MethodName.CancelPress)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.RecalculateBar)
		{
			return true;
		}
		return ((ColorRect)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._outline)
		{
			_outline = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pressTimer)
		{
			_pressTimer = VariantUtils.ConvertTo<double>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isPressed)
		{
			_isPressed = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._endTurnButton)
		{
			_endTurnButton = VariantUtils.ConvertTo<NEndTurnButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enabled)
		{
			_enabled = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._outline)
		{
			value = VariantUtils.CreateFrom<Control>(ref _outline);
			return true;
		}
		if ((ref name) == PropertyName._pressTimer)
		{
			value = VariantUtils.CreateFrom<double>(ref _pressTimer);
			return true;
		}
		if ((ref name) == PropertyName._isPressed)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isPressed);
			return true;
		}
		if ((ref name) == PropertyName._endTurnButton)
		{
			value = VariantUtils.CreateFrom<NEndTurnButton>(ref _endTurnButton);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._enabled)
		{
			value = VariantUtils.CreateFrom<bool>(ref _enabled);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._pressTimer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isPressed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._endTurnButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._enabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._outline, Variant.From<Control>(ref _outline));
		info.AddProperty(PropertyName._pressTimer, Variant.From<double>(ref _pressTimer));
		info.AddProperty(PropertyName._isPressed, Variant.From<bool>(ref _isPressed));
		info.AddProperty(PropertyName._endTurnButton, Variant.From<NEndTurnButton>(ref _endTurnButton));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._enabled, Variant.From<bool>(ref _enabled));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._outline, ref val))
		{
			_outline = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._pressTimer, ref val2))
		{
			_pressTimer = ((Variant)(ref val2)).As<double>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._isPressed, ref val3))
		{
			_isPressed = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._endTurnButton, ref val4))
		{
			_endTurnButton = ((Variant)(ref val4)).As<NEndTurnButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val5))
		{
			_tween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._enabled, ref val6))
		{
			_enabled = ((Variant)(ref val6)).As<bool>();
		}
	}
}
