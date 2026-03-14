using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/GodotExtensions/NSlider.cs")]
public class NSlider : Range
{
	[Signal]
	public delegate void MouseReleasedEventHandler(InputEvent inputEvent);

	[Signal]
	public delegate void MousePressedEventHandler(InputEvent inputEvent);

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName SetValueBasedOnMousePosition = StringName.op_Implicit("SetValueBasedOnMousePosition");

		public static readonly StringName SetValueWithoutAnimation = StringName.op_Implicit("SetValueWithoutAnimation");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName UpdateHandlePosition = StringName.op_Implicit("UpdateHandlePosition");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _handle = StringName.op_Implicit("_handle");

		public static readonly StringName _currentHandlePosition = StringName.op_Implicit("_currentHandlePosition");

		public static readonly StringName _currentVelocity = StringName.op_Implicit("_currentVelocity");

		public static readonly StringName _isDragging = StringName.op_Implicit("_isDragging");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName MouseReleased = StringName.op_Implicit("MouseReleased");

		public static readonly StringName MousePressed = StringName.op_Implicit("MousePressed");
	}

	private Control _handle;

	private float _currentHandlePosition;

	private float _currentVelocity;

	private bool _isDragging;

	private MouseReleasedEventHandler backing_MouseReleased;

	private MousePressedEventHandler backing_MousePressed;

	public event MouseReleasedEventHandler MouseReleased
	{
		add
		{
			backing_MouseReleased = (MouseReleasedEventHandler)Delegate.Combine(backing_MouseReleased, value);
		}
		remove
		{
			backing_MouseReleased = (MouseReleasedEventHandler)Delegate.Remove(backing_MouseReleased, value);
		}
	}

	public event MousePressedEventHandler MousePressed
	{
		add
		{
			backing_MousePressed = (MousePressedEventHandler)Delegate.Combine(backing_MousePressed, value);
		}
		remove
		{
			backing_MousePressed = (MousePressedEventHandler)Delegate.Remove(backing_MousePressed, value);
		}
	}

	public override void _Ready()
	{
		_handle = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Handle"));
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I8
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		((Control)this)._GuiInput(inputEvent);
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val == null)
		{
			InputEventMouseMotion val2 = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
			if (val2 != null && _isDragging)
			{
				SetValueBasedOnMousePosition(((InputEventMouse)val2).Position);
			}
			return;
		}
		MouseButton buttonIndex = val.ButtonIndex;
		if (((long)(buttonIndex - 1) <= 1L) ? true : false)
		{
			_isDragging = ((InputEvent)val).IsPressed();
			SetValueBasedOnMousePosition(((InputEventMouse)val).Position);
			((GodotObject)this).EmitSignal(((InputEvent)val).IsPressed() ? SignalName.MousePressed : SignalName.MouseReleased, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)inputEvent) });
		}
	}

	private void SetValueBasedOnMousePosition(Vector2 mousePosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((Range)this).Value = (double)(mousePosition.X / ((Control)this).Size.X) * ((Range)this).MaxValue;
	}

	public void SetValueWithoutAnimation(double value)
	{
		_currentHandlePosition = (float)value;
		((Range)this).Value = value;
		UpdateHandlePosition();
	}

	public override void _Process(double delta)
	{
		_currentHandlePosition = MathHelper.SmoothDamp(_currentHandlePosition, (float)((Range)this).Value, ref _currentVelocity, 0.05f, (float)delta);
		UpdateHandlePosition();
	}

	private void UpdateHandlePosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		_handle.Position = new Vector2(((Control)this).Size.X * (float)((double)_currentHandlePosition / ((Range)this).MaxValue) - _handle.Size.X * 0.5f, (((Control)this).Size.Y - _handle.Size.Y) * 0.5f);
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetValueBasedOnMousePosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("mousePosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetValueWithoutAnimation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateHandlePosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetValueBasedOnMousePosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetValueBasedOnMousePosition(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetValueWithoutAnimation && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetValueWithoutAnimation(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateHandlePosition && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateHandlePosition();
			ret = default(godot_variant);
			return true;
		}
		return ((Range)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName.SetValueBasedOnMousePosition)
		{
			return true;
		}
		if ((ref method) == MethodName.SetValueWithoutAnimation)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateHandlePosition)
		{
			return true;
		}
		return ((Range)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._handle)
		{
			_handle = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentHandlePosition)
		{
			_currentHandlePosition = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentVelocity)
		{
			_currentVelocity = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			_isDragging = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._handle)
		{
			value = VariantUtils.CreateFrom<Control>(ref _handle);
			return true;
		}
		if ((ref name) == PropertyName._currentHandlePosition)
		{
			value = VariantUtils.CreateFrom<float>(ref _currentHandlePosition);
			return true;
		}
		if ((ref name) == PropertyName._currentVelocity)
		{
			value = VariantUtils.CreateFrom<float>(ref _currentVelocity);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isDragging);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._handle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._currentHandlePosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._currentVelocity, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isDragging, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._handle, Variant.From<Control>(ref _handle));
		info.AddProperty(PropertyName._currentHandlePosition, Variant.From<float>(ref _currentHandlePosition));
		info.AddProperty(PropertyName._currentVelocity, Variant.From<float>(ref _currentVelocity));
		info.AddProperty(PropertyName._isDragging, Variant.From<bool>(ref _isDragging));
		info.AddSignalEventDelegate(SignalName.MouseReleased, (Delegate)backing_MouseReleased);
		info.AddSignalEventDelegate(SignalName.MousePressed, (Delegate)backing_MousePressed);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._handle, ref val))
		{
			_handle = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentHandlePosition, ref val2))
		{
			_currentHandlePosition = ((Variant)(ref val2)).As<float>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentVelocity, ref val3))
		{
			_currentVelocity = ((Variant)(ref val3)).As<float>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._isDragging, ref val4))
		{
			_isDragging = ((Variant)(ref val4)).As<bool>();
		}
		MouseReleasedEventHandler mouseReleasedEventHandler = default(MouseReleasedEventHandler);
		if (info.TryGetSignalEventDelegate<MouseReleasedEventHandler>(SignalName.MouseReleased, ref mouseReleasedEventHandler))
		{
			backing_MouseReleased = mouseReleasedEventHandler;
		}
		MousePressedEventHandler mousePressedEventHandler = default(MousePressedEventHandler);
		if (info.TryGetSignalEventDelegate<MousePressedEventHandler>(SignalName.MousePressed, ref mousePressedEventHandler))
		{
			backing_MousePressed = mousePressedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(SignalName.MouseReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.MousePressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalMouseReleased(InputEvent inputEvent)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.MouseReleased, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)inputEvent) });
	}

	protected void EmitSignalMousePressed(InputEvent inputEvent)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.MousePressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)inputEvent) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.MouseReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_MouseReleased?.Invoke(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.MousePressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_MousePressed?.Invoke(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.MouseReleased)
		{
			return true;
		}
		if ((ref signal) == SignalName.MousePressed)
		{
			return true;
		}
		return ((Range)this).HasGodotClassSignal(ref signal);
	}
}
