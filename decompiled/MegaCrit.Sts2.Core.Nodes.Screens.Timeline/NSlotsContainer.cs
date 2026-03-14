using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/NSlotsContainer.cs")]
public class NSlotsContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName ProcessPanEvent = StringName.op_Implicit("ProcessPanEvent");

		public static readonly StringName ProcessGuiFocus = StringName.op_Implicit("ProcessGuiFocus");

		public static readonly StringName ProcessScrollEvent = StringName.op_Implicit("ProcessScrollEvent");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName OnToggleVisibility = StringName.op_Implicit("OnToggleVisibility");

		public static readonly StringName Reset = StringName.op_Implicit("Reset");

		public static readonly StringName SetEnabled = StringName.op_Implicit("SetEnabled");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName GetInitX = StringName.op_Implicit("GetInitX");

		public static readonly StringName _whatsMoved = StringName.op_Implicit("_whatsMoved");

		public static readonly StringName _dragStartPosition = StringName.op_Implicit("_dragStartPosition");

		public static readonly StringName _targetPosition = StringName.op_Implicit("_targetPosition");

		public static readonly StringName _isDragging = StringName.op_Implicit("_isDragging");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _epochSlots = StringName.op_Implicit("_epochSlots");
	}

	public class SignalName : SignalName
	{
	}

	private Control _whatsMoved;

	private Vector2 _dragStartPosition;

	private Vector2 _targetPosition;

	private bool _isDragging;

	private const float _scrollSpeed = 50f;

	private const float _trackpadScrollSpeed = 20f;

	private const float _bounceBackStrength = 36f;

	private const float _lerpSmoothness = 20f;

	private Tween? _tween;

	private Control _epochSlots;

	public float GetInitX => _whatsMoved.GlobalPosition.X;

	public override void _Ready()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		_whatsMoved = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%WhatsMoved"));
		_epochSlots = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%EpochSlots"));
		_targetPosition = _whatsMoved.Position;
		((GodotObject)this).Connect(SignalName.VisibilityChanged, Callable.From((Action)OnToggleVisibility), 0u);
	}

	public override void _EnterTree()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)((Node)this).GetViewport()).Connect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)ProcessGuiFocus), 0u);
	}

	public override void _ExitTree()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)((Node)this).GetViewport()).Disconnect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)ProcessGuiFocus));
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		ProcessPanEvent(inputEvent);
		ProcessScrollEvent(inputEvent);
	}

	private void ProcessPanEvent(InputEvent inputEvent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I8
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null && (long)val.ButtonIndex == 1)
		{
			if (val.Pressed)
			{
				_isDragging = true;
				_dragStartPosition = ((InputEventMouse)val).Position;
			}
			else
			{
				_isDragging = false;
			}
			return;
		}
		InputEventMouseMotion val2 = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
		if (val2 != null && _isDragging)
		{
			Vector2 val3 = ((InputEventMouse)val2).Position - _dragStartPosition;
			_targetPosition += new Vector2(val3.X, 0f);
			_dragStartPosition = ((InputEventMouse)val2).Position;
		}
	}

	private void ProcessGuiFocus(Control focusedControl)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)this).IsVisibleInTree() && NControllerManager.Instance.IsUsingController && ((Node)this).IsAncestorOf((Node)(object)focusedControl))
		{
			float num = _whatsMoved.GlobalPosition.X - ((Node)focusedControl).GetParent<Control>().GlobalPosition.X;
			_targetPosition = new Vector2(num, _targetPosition.Y);
		}
	}

	private void ProcessScrollEvent(InputEvent inputEvent)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I8
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I8
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Invalid comparison between Unknown and I8
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Invalid comparison between Unknown and I8
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null)
		{
			if ((long)val.ButtonIndex == 4)
			{
				_targetPosition -= new Vector2(50f, 0f);
			}
			else if ((long)val.ButtonIndex == 5)
			{
				_targetPosition += new Vector2(50f, 0f);
			}
			else if ((long)val.ButtonIndex == 7)
			{
				_targetPosition -= new Vector2(50f, 0f);
			}
			else if ((long)val.ButtonIndex == 6)
			{
				_targetPosition += new Vector2(50f, 0f);
			}
		}
		else
		{
			InputEventPanGesture val2 = (InputEventPanGesture)(object)((inputEvent is InputEventPanGesture) ? inputEvent : null);
			if (val2 != null)
			{
				_targetPosition += new Vector2((0f - val2.Delta.X) * 20f, 0f);
			}
		}
	}

	public override void _Process(double delta)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		Control whatsMoved = _whatsMoved;
		Vector2 position = _whatsMoved.Position;
		whatsMoved.Position = ((Vector2)(ref position)).Lerp(_targetPosition, (float)delta * 20f);
		if (!_isDragging)
		{
			float num = _targetPosition.X;
			float num2 = _epochSlots.Position.X - _whatsMoved.Size.X;
			float num3 = _epochSlots.Position.X + _epochSlots.Size.X - _whatsMoved.Size.X;
			if (num < num2)
			{
				num = Mathf.Lerp(num, num2, (float)delta * 36f);
			}
			else if (num > num3)
			{
				num = Mathf.Lerp(num, num3, (float)delta * 36f);
			}
			_targetPosition = new Vector2(num, _targetPosition.Y);
		}
	}

	private void OnToggleVisibility()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)this).Visible)
		{
			_targetPosition = _whatsMoved.Position;
			_dragStartPosition = Vector2.Zero;
		}
	}

	public void Reset()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		_whatsMoved.Position = new Vector2(-960f, _whatsMoved.Position.Y);
	}

	public async Task LerpToSlot(float slotPositionX)
	{
		float num = _whatsMoved.GlobalPosition.X - slotPositionX + 960f - 96f + (((Control)this).Size.X - 1920f) * 0.5f;
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)_whatsMoved, NodePath.op_Implicit("global_position:x"), Variant.op_Implicit(num), 2.5).SetEase((EaseType)2).SetTrans((TransitionType)7);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		_targetPosition = _whatsMoved.Position;
	}

	public void SetEnabled(bool enabled)
	{
		((Control)this).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)(enabled ? 2 : 1);
		_isDragging = false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessPanEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessGuiFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("focusedControl"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessScrollEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnToggleVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Reset, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetEnabled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("enabled"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessPanEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessPanEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessGuiFocus && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessGuiFocus(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessScrollEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnToggleVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnToggleVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Reset && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Reset();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetEnabled && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetEnabled(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessPanEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessGuiFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.OnToggleVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName.Reset)
		{
			return true;
		}
		if ((ref method) == MethodName.SetEnabled)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._whatsMoved)
		{
			_whatsMoved = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dragStartPosition)
		{
			_dragStartPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetPosition)
		{
			_targetPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			_isDragging = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._epochSlots)
		{
			_epochSlots = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName.GetInitX)
		{
			float getInitX = GetInitX;
			value = VariantUtils.CreateFrom<float>(ref getInitX);
			return true;
		}
		if ((ref name) == PropertyName._whatsMoved)
		{
			value = VariantUtils.CreateFrom<Control>(ref _whatsMoved);
			return true;
		}
		if ((ref name) == PropertyName._dragStartPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _dragStartPosition);
			return true;
		}
		if ((ref name) == PropertyName._targetPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetPosition);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isDragging);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._epochSlots)
		{
			value = VariantUtils.CreateFrom<Control>(ref _epochSlots);
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
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._whatsMoved, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._dragStartPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isDragging, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._epochSlots, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.GetInitX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._whatsMoved, Variant.From<Control>(ref _whatsMoved));
		info.AddProperty(PropertyName._dragStartPosition, Variant.From<Vector2>(ref _dragStartPosition));
		info.AddProperty(PropertyName._targetPosition, Variant.From<Vector2>(ref _targetPosition));
		info.AddProperty(PropertyName._isDragging, Variant.From<bool>(ref _isDragging));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._epochSlots, Variant.From<Control>(ref _epochSlots));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._whatsMoved, ref val))
		{
			_whatsMoved = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._dragStartPosition, ref val2))
		{
			_dragStartPosition = ((Variant)(ref val2)).As<Vector2>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetPosition, ref val3))
		{
			_targetPosition = ((Variant)(ref val3)).As<Vector2>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._isDragging, ref val4))
		{
			_isDragging = ((Variant)(ref val4)).As<bool>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val5))
		{
			_tween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._epochSlots, ref val6))
		{
			_epochSlots = ((Variant)(ref val6)).As<Control>();
		}
	}
}
