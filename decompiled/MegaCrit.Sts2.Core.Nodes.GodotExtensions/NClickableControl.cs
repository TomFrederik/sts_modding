using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

[ScriptPath("res://src/Core/Nodes/GodotExtensions/NClickableControl.cs")]
public class NClickableControl : Control
{
	[Signal]
	public delegate void ReleasedEventHandler(NClickableControl button);

	[Signal]
	public delegate void FocusedEventHandler(NClickableControl button);

	[Signal]
	public delegate void UnfocusedEventHandler(NClickableControl button);

	[Signal]
	public delegate void MouseReleasedEventHandler(InputEvent inputEvent);

	[Signal]
	public delegate void MousePressedEventHandler(InputEvent inputEvent);

	public class MethodName : MethodName
	{
		public static readonly StringName ConnectSignals = StringName.op_Implicit("ConnectSignals");

		public static readonly StringName OnVisibilityChanged = StringName.op_Implicit("OnVisibilityChanged");

		public static readonly StringName OnFocusHandler = StringName.op_Implicit("OnFocusHandler");

		public static readonly StringName OnUnFocusHandler = StringName.op_Implicit("OnUnFocusHandler");

		public static readonly StringName HandleMousePress = StringName.op_Implicit("HandleMousePress");

		public static readonly StringName HandleMouseRelease = StringName.op_Implicit("HandleMouseRelease");

		public static readonly StringName OnHoverHandler = StringName.op_Implicit("OnHoverHandler");

		public static readonly StringName OnUnhoverHandler = StringName.op_Implicit("OnUnhoverHandler");

		public static readonly StringName OnPressHandler = StringName.op_Implicit("OnPressHandler");

		public static readonly StringName OnReleaseHandler = StringName.op_Implicit("OnReleaseHandler");

		public static readonly StringName RefreshFocus = StringName.op_Implicit("RefreshFocus");

		public static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName CheckMouseDragThreshold = StringName.op_Implicit("CheckMouseDragThreshold");

		public static readonly StringName DebugPress = StringName.op_Implicit("DebugPress");

		public static readonly StringName DebugRelease = StringName.op_Implicit("DebugRelease");

		public static readonly StringName ForceClick = StringName.op_Implicit("ForceClick");

		public static readonly StringName SetEnabled = StringName.op_Implicit("SetEnabled");

		public static readonly StringName Enable = StringName.op_Implicit("Enable");

		public static readonly StringName Disable = StringName.op_Implicit("Disable");

		public static readonly StringName OnEnable = StringName.op_Implicit("OnEnable");

		public static readonly StringName OnDisable = StringName.op_Implicit("OnDisable");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName AllowFocusWhileDisabled = StringName.op_Implicit("AllowFocusWhileDisabled");

		public static readonly StringName IsFocused = StringName.op_Implicit("IsFocused");

		public static readonly StringName IsEnabled = StringName.op_Implicit("IsEnabled");

		public static readonly StringName _ignoreDragThreshold = StringName.op_Implicit("_ignoreDragThreshold");

		public static readonly StringName _isEnabled = StringName.op_Implicit("_isEnabled");

		public static readonly StringName _isHovered = StringName.op_Implicit("_isHovered");

		public static readonly StringName _isControllerFocused = StringName.op_Implicit("_isControllerFocused");

		public static readonly StringName _isControllerNavigable = StringName.op_Implicit("_isControllerNavigable");

		public static readonly StringName _beginDragPosition = StringName.op_Implicit("_beginDragPosition");

		public static readonly StringName _isPressed = StringName.op_Implicit("_isPressed");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName Released = StringName.op_Implicit("Released");

		public static readonly StringName Focused = StringName.op_Implicit("Focused");

		public static readonly StringName Unfocused = StringName.op_Implicit("Unfocused");

		public static readonly StringName MouseReleased = StringName.op_Implicit("MouseReleased");

		public static readonly StringName MousePressed = StringName.op_Implicit("MousePressed");
	}

	[Export(/*Could not decode attribute arguments.*/)]
	protected float _ignoreDragThreshold = -1f;

	protected bool _isEnabled = true;

	private bool _isHovered;

	private bool _isControllerFocused;

	private bool _isControllerNavigable;

	private Vector2 _beginDragPosition;

	private bool _isPressed;

	private static readonly StyleBoxEmpty _blankFocusStyle = new StyleBoxEmpty();

	private ReleasedEventHandler backing_Released;

	private FocusedEventHandler backing_Focused;

	private UnfocusedEventHandler backing_Unfocused;

	private MouseReleasedEventHandler backing_MouseReleased;

	private MousePressedEventHandler backing_MousePressed;

	protected virtual bool AllowFocusWhileDisabled => false;

	protected bool IsFocused { get; private set; }

	public bool IsEnabled => _isEnabled;

	public event ReleasedEventHandler Released
	{
		add
		{
			backing_Released = (ReleasedEventHandler)Delegate.Combine(backing_Released, value);
		}
		remove
		{
			backing_Released = (ReleasedEventHandler)Delegate.Remove(backing_Released, value);
		}
	}

	public event FocusedEventHandler Focused
	{
		add
		{
			backing_Focused = (FocusedEventHandler)Delegate.Combine(backing_Focused, value);
		}
		remove
		{
			backing_Focused = (FocusedEventHandler)Delegate.Remove(backing_Focused, value);
		}
	}

	public event UnfocusedEventHandler Unfocused
	{
		add
		{
			backing_Unfocused = (UnfocusedEventHandler)Delegate.Combine(backing_Unfocused, value);
		}
		remove
		{
			backing_Unfocused = (UnfocusedEventHandler)Delegate.Remove(backing_Unfocused, value);
		}
	}

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

	protected virtual void ConnectSignals()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Invalid comparison between Unknown and I8
		((GodotObject)this).Connect(SignalName.FocusEntered, Callable.From((Action)OnFocusHandler), 0u);
		((GodotObject)this).Connect(SignalName.FocusExited, Callable.From((Action)OnUnFocusHandler), 0u);
		((GodotObject)this).Connect(SignalName.MouseEntered, Callable.From((Action)OnHoverHandler), 0u);
		((GodotObject)this).Connect(SignalName.MouseExited, Callable.From((Action)OnUnhoverHandler), 0u);
		((GodotObject)this).Connect(SignalName.MousePressed, Callable.From<InputEvent>((Action<InputEvent>)HandleMousePress), 0u);
		((GodotObject)this).Connect(SignalName.MouseReleased, Callable.From<InputEvent>((Action<InputEvent>)HandleMouseRelease), 0u);
		((GodotObject)this).Connect(SignalName.VisibilityChanged, Callable.From((Action)OnVisibilityChanged), 0u);
		((Control)this).AddThemeStyleboxOverride(ThemeConstants.Control.focus, (StyleBox)(object)_blankFocusStyle);
		_isControllerNavigable = (long)((Control)this).FocusMode == 2;
		if (((Control)this).HasFocus())
		{
			OnFocusHandler();
		}
	}

	private void OnVisibilityChanged()
	{
		if (!((CanvasItem)this).IsVisibleInTree())
		{
			OnUnFocusHandler();
		}
	}

	private void OnFocusHandler()
	{
		_isControllerFocused = true;
		RefreshFocus();
	}

	private void OnUnFocusHandler()
	{
		_isControllerFocused = false;
		RefreshFocus();
	}

	private void HandleMousePress(InputEvent inputEvent)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Invalid comparison between Unknown and I8
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (_isEnabled && ((CanvasItem)this).IsVisibleInTree() && IsFocused)
		{
			InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
			if (val != null && (long)val.ButtonIndex == 1)
			{
				_isControllerFocused = false;
				_beginDragPosition = ((InputEventMouse)val).GlobalPosition;
				OnPressHandler();
			}
		}
	}

	private void HandleMouseRelease(InputEvent inputEvent)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I8
		if (_isEnabled && ((CanvasItem)this).IsVisibleInTree() && IsFocused)
		{
			InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
			if (val != null && (long)val.ButtonIndex == 1)
			{
				OnReleaseHandler();
			}
		}
	}

	private void OnHoverHandler()
	{
		_isHovered = true;
		if (!((Node)this).GetTree().Paused || NGame.IsReleaseGame())
		{
			RefreshFocus();
		}
	}

	private void OnUnhoverHandler()
	{
		_isHovered = false;
		if (!((Node)this).GetTree().Paused || NGame.IsReleaseGame())
		{
			RefreshFocus();
		}
	}

	protected void OnPressHandler()
	{
		_isPressed = true;
		OnPress();
	}

	protected void OnReleaseHandler()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (_isPressed)
		{
			_isPressed = false;
			OnRelease();
			((GodotObject)this).EmitSignal(SignalName.Released, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
		}
	}

	private void RefreshFocus()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (_isEnabled || AllowFocusWhileDisabled) && ((CanvasItem)this).IsVisibleInTree() && (_isHovered || _isControllerFocused);
		if (IsFocused != flag)
		{
			IsFocused = flag;
			if (IsFocused)
			{
				((GodotObject)this).EmitSignal(SignalName.Focused, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
				OnFocus();
			}
			else
			{
				((GodotObject)this).EmitSignal(SignalName.Unfocused, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
				OnUnfocus();
			}
		}
	}

	protected virtual void OnFocus()
	{
	}

	protected virtual void OnUnfocus()
	{
	}

	protected virtual void OnPress()
	{
	}

	protected virtual void OnRelease()
	{
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I8
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null && _isEnabled)
		{
			MouseButton buttonIndex = val.ButtonIndex;
			if (((long)(buttonIndex - 1) <= 1L) ? true : false)
			{
				((GodotObject)this).EmitSignal(((InputEvent)val).IsPressed() ? SignalName.MousePressed : SignalName.MouseReleased, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)inputEvent) });
			}
		}
		if (inputEvent.IsActionPressed(MegaInput.select, false, false))
		{
			OnPressHandler();
		}
		else if (inputEvent.IsActionReleased(MegaInput.select, false))
		{
			OnReleaseHandler();
		}
	}

	protected void CheckMouseDragThreshold(InputEvent inputEvent)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (_ignoreDragThreshold <= 0f || !_isPressed)
		{
			return;
		}
		InputEventMouseMotion val = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
		if (val != null)
		{
			Vector2 globalPosition = ((InputEventMouse)val).GlobalPosition;
			if (((Vector2)(ref globalPosition)).DistanceTo(_beginDragPosition) >= _ignoreDragThreshold)
			{
				_isPressed = false;
			}
		}
	}

	public void DebugPress()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.MousePressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)new InputEventMouseButton
		{
			ButtonIndex = (MouseButton)1,
			Pressed = true
		}) });
	}

	public void DebugRelease()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.MouseReleased, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)new InputEventMouseButton
		{
			ButtonIndex = (MouseButton)1,
			Pressed = false
		}) });
	}

	public void ForceClick()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		OnRelease();
		((GodotObject)this).EmitSignal(SignalName.Released, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
	}

	public void SetEnabled(bool enabled)
	{
		if (enabled)
		{
			Enable();
		}
		else
		{
			Disable();
		}
	}

	public void Enable()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!_isEnabled)
		{
			_isEnabled = true;
			((Control)this).FocusMode = (FocusModeEnum)(_isControllerNavigable ? 2 : 0);
			OnEnable();
			RefreshFocus();
			Callable val = Callable.From((Action)delegate
			{
				((Node)this).SetProcessInput(true);
			});
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		}
	}

	public void Disable()
	{
		if (_isEnabled)
		{
			_isEnabled = false;
			_isPressed = false;
			((Control)this).FocusMode = (FocusModeEnum)0;
			OnDisable();
			RefreshFocus();
			((Node)this).SetProcessInput(false);
		}
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
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
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Expected O, but got Unknown
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Expected O, but got Unknown
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(25);
		list.Add(new MethodInfo(MethodName.ConnectSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnVisibilityChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocusHandler, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnFocusHandler, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HandleMousePress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HandleMouseRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHoverHandler, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnhoverHandler, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPressHandler, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnReleaseHandler, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CheckMouseDragThreshold, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ForceClick, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetEnabled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("enabled"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Enable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Disable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.ConnectSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnVisibilityChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnVisibilityChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocusHandler && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocusHandler();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnFocusHandler && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnFocusHandler();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HandleMousePress && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			HandleMousePress(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HandleMouseRelease && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			HandleMouseRelease(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHoverHandler && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHoverHandler();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnhoverHandler && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnhoverHandler();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPressHandler && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPressHandler();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnReleaseHandler && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnReleaseHandler();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshFocus();
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
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CheckMouseDragThreshold && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CheckMouseDragThreshold(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DebugPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DebugRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ForceClick && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ForceClick();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetEnabled && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetEnabled(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Enable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Enable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Disable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Disable();
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
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.ConnectSignals)
		{
			return true;
		}
		if ((ref method) == MethodName.OnVisibilityChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocusHandler)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnFocusHandler)
		{
			return true;
		}
		if ((ref method) == MethodName.HandleMousePress)
		{
			return true;
		}
		if ((ref method) == MethodName.HandleMouseRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHoverHandler)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnhoverHandler)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPressHandler)
		{
			return true;
		}
		if ((ref method) == MethodName.OnReleaseHandler)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshFocus)
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
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName.CheckMouseDragThreshold)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugPress)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.ForceClick)
		{
			return true;
		}
		if ((ref method) == MethodName.SetEnabled)
		{
			return true;
		}
		if ((ref method) == MethodName.Enable)
		{
			return true;
		}
		if ((ref method) == MethodName.Disable)
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsFocused)
		{
			IsFocused = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ignoreDragThreshold)
		{
			_ignoreDragThreshold = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isEnabled)
		{
			_isEnabled = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isHovered)
		{
			_isHovered = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isControllerFocused)
		{
			_isControllerFocused = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isControllerNavigable)
		{
			_isControllerNavigable = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._beginDragPosition)
		{
			_beginDragPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isPressed)
		{
			_isPressed = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.AllowFocusWhileDisabled)
		{
			bool allowFocusWhileDisabled = AllowFocusWhileDisabled;
			value = VariantUtils.CreateFrom<bool>(ref allowFocusWhileDisabled);
			return true;
		}
		if ((ref name) == PropertyName.IsFocused)
		{
			bool allowFocusWhileDisabled = IsFocused;
			value = VariantUtils.CreateFrom<bool>(ref allowFocusWhileDisabled);
			return true;
		}
		if ((ref name) == PropertyName.IsEnabled)
		{
			bool allowFocusWhileDisabled = IsEnabled;
			value = VariantUtils.CreateFrom<bool>(ref allowFocusWhileDisabled);
			return true;
		}
		if ((ref name) == PropertyName._ignoreDragThreshold)
		{
			value = VariantUtils.CreateFrom<float>(ref _ignoreDragThreshold);
			return true;
		}
		if ((ref name) == PropertyName._isEnabled)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isEnabled);
			return true;
		}
		if ((ref name) == PropertyName._isHovered)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isHovered);
			return true;
		}
		if ((ref name) == PropertyName._isControllerFocused)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isControllerFocused);
			return true;
		}
		if ((ref name) == PropertyName._isControllerNavigable)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isControllerNavigable);
			return true;
		}
		if ((ref name) == PropertyName._beginDragPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _beginDragPosition);
			return true;
		}
		if ((ref name) == PropertyName._isPressed)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isPressed);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._ignoreDragThreshold, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)1, PropertyName.AllowFocusWhileDisabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsFocused, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isEnabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsEnabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isHovered, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isControllerFocused, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isControllerNavigable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._beginDragPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isPressed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		StringName isFocused = PropertyName.IsFocused;
		bool isFocused2 = IsFocused;
		info.AddProperty(isFocused, Variant.From<bool>(ref isFocused2));
		info.AddProperty(PropertyName._ignoreDragThreshold, Variant.From<float>(ref _ignoreDragThreshold));
		info.AddProperty(PropertyName._isEnabled, Variant.From<bool>(ref _isEnabled));
		info.AddProperty(PropertyName._isHovered, Variant.From<bool>(ref _isHovered));
		info.AddProperty(PropertyName._isControllerFocused, Variant.From<bool>(ref _isControllerFocused));
		info.AddProperty(PropertyName._isControllerNavigable, Variant.From<bool>(ref _isControllerNavigable));
		info.AddProperty(PropertyName._beginDragPosition, Variant.From<Vector2>(ref _beginDragPosition));
		info.AddProperty(PropertyName._isPressed, Variant.From<bool>(ref _isPressed));
		info.AddSignalEventDelegate(SignalName.Released, (Delegate)backing_Released);
		info.AddSignalEventDelegate(SignalName.Focused, (Delegate)backing_Focused);
		info.AddSignalEventDelegate(SignalName.Unfocused, (Delegate)backing_Unfocused);
		info.AddSignalEventDelegate(SignalName.MouseReleased, (Delegate)backing_MouseReleased);
		info.AddSignalEventDelegate(SignalName.MousePressed, (Delegate)backing_MousePressed);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsFocused, ref val))
		{
			IsFocused = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._ignoreDragThreshold, ref val2))
		{
			_ignoreDragThreshold = ((Variant)(ref val2)).As<float>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._isEnabled, ref val3))
		{
			_isEnabled = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._isHovered, ref val4))
		{
			_isHovered = ((Variant)(ref val4)).As<bool>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._isControllerFocused, ref val5))
		{
			_isControllerFocused = ((Variant)(ref val5)).As<bool>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._isControllerNavigable, ref val6))
		{
			_isControllerNavigable = ((Variant)(ref val6)).As<bool>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._beginDragPosition, ref val7))
		{
			_beginDragPosition = ((Variant)(ref val7)).As<Vector2>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._isPressed, ref val8))
		{
			_isPressed = ((Variant)(ref val8)).As<bool>();
		}
		ReleasedEventHandler releasedEventHandler = default(ReleasedEventHandler);
		if (info.TryGetSignalEventDelegate<ReleasedEventHandler>(SignalName.Released, ref releasedEventHandler))
		{
			backing_Released = releasedEventHandler;
		}
		FocusedEventHandler focusedEventHandler = default(FocusedEventHandler);
		if (info.TryGetSignalEventDelegate<FocusedEventHandler>(SignalName.Focused, ref focusedEventHandler))
		{
			backing_Focused = focusedEventHandler;
		}
		UnfocusedEventHandler unfocusedEventHandler = default(UnfocusedEventHandler);
		if (info.TryGetSignalEventDelegate<UnfocusedEventHandler>(SignalName.Unfocused, ref unfocusedEventHandler))
		{
			backing_Unfocused = unfocusedEventHandler;
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
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(SignalName.Released, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.Focused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.Unfocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
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

	protected void EmitSignalReleased(NClickableControl button)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Released, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)button) });
	}

	protected void EmitSignalFocused(NClickableControl button)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Focused, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)button) });
	}

	protected void EmitSignalUnfocused(NClickableControl button)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Unfocused, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)button) });
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
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Released && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Released?.Invoke(VariantUtils.ConvertTo<NClickableControl>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.Focused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Focused?.Invoke(VariantUtils.ConvertTo<NClickableControl>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.Unfocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Unfocused?.Invoke(VariantUtils.ConvertTo<NClickableControl>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.MouseReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
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
		if ((ref signal) == SignalName.Released)
		{
			return true;
		}
		if ((ref signal) == SignalName.Focused)
		{
			return true;
		}
		if ((ref signal) == SignalName.Unfocused)
		{
			return true;
		}
		if ((ref signal) == SignalName.MouseReleased)
		{
			return true;
		}
		if ((ref signal) == SignalName.MousePressed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
