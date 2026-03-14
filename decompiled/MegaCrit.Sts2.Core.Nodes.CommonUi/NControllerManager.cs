using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.ControllerInput.ControllerConfigs;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NControllerManager.cs")]
public class NControllerManager : Node
{
	[Signal]
	public delegate void ControllerDetectedEventHandler();

	[Signal]
	public delegate void MouseDetectedEventHandler();

	[Signal]
	public delegate void ControllerTypeChangedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName OnControllerTypeChanged = StringName.op_Implicit("OnControllerTypeChanged");

		public static readonly StringName CheckForMouseInput = StringName.op_Implicit("CheckForMouseInput");

		public static readonly StringName CheckForControllerInput = StringName.op_Implicit("CheckForControllerInput");

		public static readonly StringName ControlModeChanged = StringName.op_Implicit("ControlModeChanged");

		public static readonly StringName OnScreenContextChanged = StringName.op_Implicit("OnScreenContextChanged");

		public static readonly StringName GetHotkeyIcon = StringName.op_Implicit("GetHotkeyIcon");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ShouldAllowControllerRebinding = StringName.op_Implicit("ShouldAllowControllerRebinding");

		public static readonly StringName IsUsingController = StringName.op_Implicit("IsUsingController");

		public static readonly StringName ControllerMappingType = StringName.op_Implicit("ControllerMappingType");

		public static readonly StringName _lastMousePosition = StringName.op_Implicit("_lastMousePosition");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _notifyTween = StringName.op_Implicit("_notifyTween");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName ControllerDetected = StringName.op_Implicit("ControllerDetected");

		public static readonly StringName MouseDetected = StringName.op_Implicit("MouseDetected");

		public static readonly StringName ControllerTypeChanged = StringName.op_Implicit("ControllerTypeChanged");
	}

	private IControllerInputStrategy? _inputStrategy;

	private static readonly Vector2 _offscreenPos = Vector2.One * -1000f;

	private Vector2 _lastMousePosition;

	private MegaLabel _label;

	private Tween? _notifyTween;

	private ControllerDetectedEventHandler backing_ControllerDetected;

	private MouseDetectedEventHandler backing_MouseDetected;

	private ControllerTypeChangedEventHandler backing_ControllerTypeChanged;

	public static NControllerManager? Instance
	{
		get
		{
			if (NGame.Instance == null)
			{
				return null;
			}
			return NGame.Instance.InputManager.ControllerManager;
		}
	}

	public bool ShouldAllowControllerRebinding => _inputStrategy?.ShouldAllowControllerRebinding ?? true;

	public bool IsUsingController { get; private set; }

	public Dictionary<StringName, StringName> GetDefaultControllerInputMap
	{
		get
		{
			if (_inputStrategy == null)
			{
				return new SteamControllerConfig().DefaultControllerInputMap;
			}
			return _inputStrategy.GetDefaultControllerInputMap;
		}
	}

	public ControllerMappingType ControllerMappingType
	{
		get
		{
			if (_inputStrategy == null)
			{
				return ControllerMappingType.Default;
			}
			return _inputStrategy.ControllerConfig.ControllerMappingType;
		}
	}

	public event ControllerDetectedEventHandler ControllerDetected
	{
		add
		{
			backing_ControllerDetected = (ControllerDetectedEventHandler)Delegate.Combine(backing_ControllerDetected, value);
		}
		remove
		{
			backing_ControllerDetected = (ControllerDetectedEventHandler)Delegate.Remove(backing_ControllerDetected, value);
		}
	}

	public event MouseDetectedEventHandler MouseDetected
	{
		add
		{
			backing_MouseDetected = (MouseDetectedEventHandler)Delegate.Combine(backing_MouseDetected, value);
		}
		remove
		{
			backing_MouseDetected = (MouseDetectedEventHandler)Delegate.Remove(backing_MouseDetected, value);
		}
	}

	public event ControllerTypeChangedEventHandler ControllerTypeChanged
	{
		add
		{
			backing_ControllerTypeChanged = (ControllerTypeChangedEventHandler)Delegate.Combine(backing_ControllerTypeChanged, value);
		}
		remove
		{
			backing_ControllerTypeChanged = (ControllerTypeChangedEventHandler)Delegate.Remove(backing_ControllerTypeChanged, value);
		}
	}

	public async Task Init()
	{
		ActiveScreenContext.Instance.Updated += OnScreenContextChanged;
		_label = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("Label"));
		((CanvasItem)_label).Modulate = Colors.Transparent;
		_inputStrategy = new SteamControllerInputStrategy();
		await _inputStrategy.Init();
	}

	public override void _ExitTree()
	{
		ActiveScreenContext.Instance.Updated -= OnScreenContextChanged;
	}

	public override void _Process(double delta)
	{
		_inputStrategy?.ProcessInput();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (IsUsingController)
		{
			CheckForMouseInput(inputEvent);
		}
		else
		{
			CheckForControllerInput(inputEvent);
		}
	}

	public void OnControllerTypeChanged()
	{
		EmitSignalControllerTypeChanged();
	}

	private void CheckForMouseInput(InputEvent inputEvent)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		bool flag = inputEvent is InputEventMouseButton;
		InputEventMouseMotion val = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
		int num;
		if (val != null)
		{
			Vector2 velocity = val.Velocity;
			num = ((((Vector2)(ref velocity)).LengthSquared() > 100f) ? 1 : 0);
		}
		else
		{
			num = 0;
		}
		bool flag2 = (byte)num != 0;
		Viewport viewport = ((Node)this).GetViewport();
		if (flag || flag2)
		{
			IsUsingController = false;
			Input.WarpMouse(_lastMousePosition);
			if (viewport != null)
			{
				viewport.GuiReleaseFocus();
			}
			((GodotObject)this).EmitSignal(SignalName.MouseDetected, Array.Empty<Variant>());
			ControlModeChanged();
		}
	}

	private void CheckForControllerInput(InputEvent inputEvent)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (Controller.AllControllerInputs.Any((StringName i) => inputEvent.IsActionPressed(i, false, false)))
		{
			IsUsingController = true;
			Viewport viewport = ((Node)this).GetViewport();
			if (viewport != null)
			{
				Vector2I val = DisplayServer.MouseGetPosition();
				Vector2I val2 = DisplayServer.WindowGetPosition(0);
				_lastMousePosition = new Vector2((float)(val.X - val2.X), (float)(val.Y - val2.Y));
				viewport.WarpMouse(_offscreenPos);
			}
			ActiveScreenContext.Instance.FocusOnDefaultControl();
			((GodotObject)this).EmitSignal(SignalName.ControllerDetected, Array.Empty<Variant>());
			ControlModeChanged();
			if (viewport != null)
			{
				viewport.SetInputAsHandled();
			}
		}
	}

	private void ControlModeChanged()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Tween? notifyTween = _notifyTween;
		if (notifyTween != null)
		{
			notifyTween.Kill();
		}
		_notifyTween = ((Node)this).CreateTween();
		_notifyTween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25);
		_notifyTween.TweenInterval(0.5);
		_notifyTween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.Transparent), 0.75);
		if (IsUsingController)
		{
			_label.SetTextAutoSize(new LocString("main_menu_ui", "CONTROLLER_DETECTED").GetFormattedText());
		}
		else
		{
			_label.SetTextAutoSize(new LocString("main_menu_ui", "MOUSE_DETECTED").GetFormattedText());
		}
	}

	private void OnScreenContextChanged()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (IsUsingController)
		{
			Callable val = Callable.From((Action)delegate
			{
				ActiveScreenContext.Instance.FocusOnDefaultControl();
			});
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		}
		else
		{
			Vector2I val2 = DisplayServer.MouseGetPosition();
			Vector2I val3 = DisplayServer.WindowGetPosition(0);
			Input.WarpMouse(new Vector2((float)(val2.X - val3.X), (float)(val2.Y - val3.Y)));
		}
	}

	public Texture2D? GetHotkeyIcon(string hotkey)
	{
		return _inputStrategy?.GetHotkeyIcon(hotkey);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Expected O, but got Unknown
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnControllerTypeChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CheckForMouseInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CheckForControllerInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ControlModeChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnScreenContextChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetHotkeyIcon, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("hotkey"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnControllerTypeChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnControllerTypeChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CheckForMouseInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CheckForMouseInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CheckForControllerInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CheckForControllerInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ControlModeChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ControlModeChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnScreenContextChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnScreenContextChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetHotkeyIcon && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Texture2D hotkeyIcon = GetHotkeyIcon(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Texture2D>(ref hotkeyIcon);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.OnControllerTypeChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.CheckForMouseInput)
		{
			return true;
		}
		if ((ref method) == MethodName.CheckForControllerInput)
		{
			return true;
		}
		if ((ref method) == MethodName.ControlModeChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnScreenContextChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.GetHotkeyIcon)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsUsingController)
		{
			IsUsingController = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastMousePosition)
		{
			_lastMousePosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._notifyTween)
		{
			_notifyTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName.ShouldAllowControllerRebinding)
		{
			bool shouldAllowControllerRebinding = ShouldAllowControllerRebinding;
			value = VariantUtils.CreateFrom<bool>(ref shouldAllowControllerRebinding);
			return true;
		}
		if ((ref name) == PropertyName.IsUsingController)
		{
			bool shouldAllowControllerRebinding = IsUsingController;
			value = VariantUtils.CreateFrom<bool>(ref shouldAllowControllerRebinding);
			return true;
		}
		if ((ref name) == PropertyName.ControllerMappingType)
		{
			ControllerMappingType controllerMappingType = ControllerMappingType;
			value = VariantUtils.CreateFrom<ControllerMappingType>(ref controllerMappingType);
			return true;
		}
		if ((ref name) == PropertyName._lastMousePosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _lastMousePosition);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._notifyTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _notifyTween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.ShouldAllowControllerRebinding, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._lastMousePosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._notifyTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsUsingController, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.ControllerMappingType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName isUsingController = PropertyName.IsUsingController;
		bool isUsingController2 = IsUsingController;
		info.AddProperty(isUsingController, Variant.From<bool>(ref isUsingController2));
		info.AddProperty(PropertyName._lastMousePosition, Variant.From<Vector2>(ref _lastMousePosition));
		info.AddProperty(PropertyName._label, Variant.From<MegaLabel>(ref _label));
		info.AddProperty(PropertyName._notifyTween, Variant.From<Tween>(ref _notifyTween));
		info.AddSignalEventDelegate(SignalName.ControllerDetected, (Delegate)backing_ControllerDetected);
		info.AddSignalEventDelegate(SignalName.MouseDetected, (Delegate)backing_MouseDetected);
		info.AddSignalEventDelegate(SignalName.ControllerTypeChanged, (Delegate)backing_ControllerTypeChanged);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsUsingController, ref val))
		{
			IsUsingController = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastMousePosition, ref val2))
		{
			_lastMousePosition = ((Variant)(ref val2)).As<Vector2>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val3))
		{
			_label = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._notifyTween, ref val4))
		{
			_notifyTween = ((Variant)(ref val4)).As<Tween>();
		}
		ControllerDetectedEventHandler controllerDetectedEventHandler = default(ControllerDetectedEventHandler);
		if (info.TryGetSignalEventDelegate<ControllerDetectedEventHandler>(SignalName.ControllerDetected, ref controllerDetectedEventHandler))
		{
			backing_ControllerDetected = controllerDetectedEventHandler;
		}
		MouseDetectedEventHandler mouseDetectedEventHandler = default(MouseDetectedEventHandler);
		if (info.TryGetSignalEventDelegate<MouseDetectedEventHandler>(SignalName.MouseDetected, ref mouseDetectedEventHandler))
		{
			backing_MouseDetected = mouseDetectedEventHandler;
		}
		ControllerTypeChangedEventHandler controllerTypeChangedEventHandler = default(ControllerTypeChangedEventHandler);
		if (info.TryGetSignalEventDelegate<ControllerTypeChangedEventHandler>(SignalName.ControllerTypeChanged, ref controllerTypeChangedEventHandler))
		{
			backing_ControllerTypeChanged = controllerTypeChangedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(SignalName.ControllerDetected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.MouseDetected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.ControllerTypeChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalControllerDetected()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.ControllerDetected, Array.Empty<Variant>());
	}

	protected void EmitSignalMouseDetected()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.MouseDetected, Array.Empty<Variant>());
	}

	protected void EmitSignalControllerTypeChanged()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.ControllerTypeChanged, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.ControllerDetected && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_ControllerDetected?.Invoke();
		}
		else if ((ref signal) == SignalName.MouseDetected && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_MouseDetected?.Invoke();
		}
		else if ((ref signal) == SignalName.ControllerTypeChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_ControllerTypeChanged?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.ControllerDetected)
		{
			return true;
		}
		if ((ref signal) == SignalName.MouseDetected)
		{
			return true;
		}
		if ((ref signal) == SignalName.ControllerTypeChanged)
		{
			return true;
		}
		return ((Node)this).HasGodotClassSignal(ref signal);
	}
}
