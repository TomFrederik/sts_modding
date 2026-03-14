using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.ControllerInput.ControllerConfigs;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Platform.Steam;
using Steamworks;

namespace MegaCrit.Sts2.Core.ControllerInput;

public class SteamControllerInputStrategy : IControllerInputStrategy
{
	private Dictionary<EInputActionOrigin, StringName> _steamInputsToMegaInputs = new Dictionary<EInputActionOrigin, StringName>
	{
		{
			(EInputActionOrigin)1,
			Controller.faceButtonSouth
		},
		{
			(EInputActionOrigin)2,
			Controller.faceButtonEast
		},
		{
			(EInputActionOrigin)3,
			Controller.faceButtonWest
		},
		{
			(EInputActionOrigin)4,
			Controller.faceButtonNorth
		},
		{
			(EInputActionOrigin)5,
			Controller.leftBumper
		},
		{
			(EInputActionOrigin)6,
			Controller.rightBumper
		},
		{
			(EInputActionOrigin)25,
			Controller.leftTrigger
		},
		{
			(EInputActionOrigin)26,
			Controller.leftTrigger
		},
		{
			(EInputActionOrigin)27,
			Controller.rightTrigger
		},
		{
			(EInputActionOrigin)28,
			Controller.rightTrigger
		},
		{
			(EInputActionOrigin)31,
			Controller.joystickUp
		},
		{
			(EInputActionOrigin)32,
			Controller.joystickDown
		},
		{
			(EInputActionOrigin)33,
			Controller.joystickLeft
		},
		{
			(EInputActionOrigin)34,
			Controller.joystickRight
		},
		{
			(EInputActionOrigin)30,
			Controller.joystickPress
		},
		{
			(EInputActionOrigin)9,
			Controller.startButton
		},
		{
			(EInputActionOrigin)10,
			Controller.selectButton
		},
		{
			(EInputActionOrigin)50,
			Controller.faceButtonSouth
		},
		{
			(EInputActionOrigin)51,
			Controller.faceButtonEast
		},
		{
			(EInputActionOrigin)53,
			Controller.faceButtonWest
		},
		{
			(EInputActionOrigin)52,
			Controller.faceButtonNorth
		},
		{
			(EInputActionOrigin)54,
			Controller.leftBumper
		},
		{
			(EInputActionOrigin)55,
			Controller.rightBumper
		},
		{
			(EInputActionOrigin)79,
			Controller.leftTrigger
		},
		{
			(EInputActionOrigin)80,
			Controller.leftTrigger
		},
		{
			(EInputActionOrigin)81,
			Controller.rightTrigger
		},
		{
			(EInputActionOrigin)82,
			Controller.rightTrigger
		},
		{
			(EInputActionOrigin)95,
			Controller.dPadNorth
		},
		{
			(EInputActionOrigin)96,
			Controller.dPadSouth
		},
		{
			(EInputActionOrigin)97,
			Controller.dPadWest
		},
		{
			(EInputActionOrigin)98,
			Controller.dPadEast
		},
		{
			(EInputActionOrigin)85,
			Controller.joystickUp
		},
		{
			(EInputActionOrigin)86,
			Controller.joystickDown
		},
		{
			(EInputActionOrigin)87,
			Controller.joystickLeft
		},
		{
			(EInputActionOrigin)88,
			Controller.joystickRight
		},
		{
			(EInputActionOrigin)84,
			Controller.joystickPress
		},
		{
			(EInputActionOrigin)56,
			Controller.startButton
		},
		{
			(EInputActionOrigin)57,
			Controller.selectButton
		},
		{
			(EInputActionOrigin)74,
			Controller.ps4Touchpad
		},
		{
			(EInputActionOrigin)67,
			Controller.ps4Touchpad
		},
		{
			(EInputActionOrigin)60,
			Controller.ps4Touchpad
		},
		{
			(EInputActionOrigin)258,
			Controller.faceButtonSouth
		},
		{
			(EInputActionOrigin)259,
			Controller.faceButtonEast
		},
		{
			(EInputActionOrigin)261,
			Controller.faceButtonWest
		},
		{
			(EInputActionOrigin)260,
			Controller.faceButtonNorth
		},
		{
			(EInputActionOrigin)262,
			Controller.leftBumper
		},
		{
			(EInputActionOrigin)263,
			Controller.rightBumper
		},
		{
			(EInputActionOrigin)288,
			Controller.leftTrigger
		},
		{
			(EInputActionOrigin)289,
			Controller.leftTrigger
		},
		{
			(EInputActionOrigin)290,
			Controller.rightTrigger
		},
		{
			(EInputActionOrigin)291,
			Controller.rightTrigger
		},
		{
			(EInputActionOrigin)304,
			Controller.dPadNorth
		},
		{
			(EInputActionOrigin)305,
			Controller.dPadSouth
		},
		{
			(EInputActionOrigin)306,
			Controller.dPadWest
		},
		{
			(EInputActionOrigin)307,
			Controller.dPadEast
		},
		{
			(EInputActionOrigin)294,
			Controller.joystickUp
		},
		{
			(EInputActionOrigin)295,
			Controller.joystickDown
		},
		{
			(EInputActionOrigin)296,
			Controller.joystickLeft
		},
		{
			(EInputActionOrigin)297,
			Controller.joystickRight
		},
		{
			(EInputActionOrigin)293,
			Controller.joystickPress
		},
		{
			(EInputActionOrigin)264,
			Controller.startButton
		},
		{
			(EInputActionOrigin)265,
			Controller.selectButton
		},
		{
			(EInputActionOrigin)283,
			Controller.ps4Touchpad
		},
		{
			(EInputActionOrigin)276,
			Controller.ps4Touchpad
		},
		{
			(EInputActionOrigin)269,
			Controller.ps4Touchpad
		},
		{
			(EInputActionOrigin)114,
			Controller.faceButtonSouth
		},
		{
			(EInputActionOrigin)115,
			Controller.faceButtonEast
		},
		{
			(EInputActionOrigin)116,
			Controller.faceButtonWest
		},
		{
			(EInputActionOrigin)117,
			Controller.faceButtonNorth
		},
		{
			(EInputActionOrigin)118,
			Controller.leftBumper
		},
		{
			(EInputActionOrigin)119,
			Controller.rightBumper
		},
		{
			(EInputActionOrigin)122,
			Controller.leftTrigger
		},
		{
			(EInputActionOrigin)123,
			Controller.leftTrigger
		},
		{
			(EInputActionOrigin)124,
			Controller.rightTrigger
		},
		{
			(EInputActionOrigin)125,
			Controller.rightTrigger
		},
		{
			(EInputActionOrigin)138,
			Controller.dPadNorth
		},
		{
			(EInputActionOrigin)139,
			Controller.dPadSouth
		},
		{
			(EInputActionOrigin)140,
			Controller.dPadWest
		},
		{
			(EInputActionOrigin)141,
			Controller.dPadEast
		},
		{
			(EInputActionOrigin)128,
			Controller.joystickUp
		},
		{
			(EInputActionOrigin)129,
			Controller.joystickDown
		},
		{
			(EInputActionOrigin)130,
			Controller.joystickLeft
		},
		{
			(EInputActionOrigin)131,
			Controller.joystickRight
		},
		{
			(EInputActionOrigin)127,
			Controller.joystickPress
		},
		{
			(EInputActionOrigin)120,
			Controller.startButton
		},
		{
			(EInputActionOrigin)121,
			Controller.selectButton
		},
		{
			(EInputActionOrigin)153,
			Controller.faceButtonSouth
		},
		{
			(EInputActionOrigin)154,
			Controller.faceButtonEast
		},
		{
			(EInputActionOrigin)155,
			Controller.faceButtonWest
		},
		{
			(EInputActionOrigin)156,
			Controller.faceButtonNorth
		},
		{
			(EInputActionOrigin)157,
			Controller.leftBumper
		},
		{
			(EInputActionOrigin)158,
			Controller.rightBumper
		},
		{
			(EInputActionOrigin)161,
			Controller.leftTrigger
		},
		{
			(EInputActionOrigin)162,
			Controller.leftTrigger
		},
		{
			(EInputActionOrigin)163,
			Controller.rightTrigger
		},
		{
			(EInputActionOrigin)164,
			Controller.rightTrigger
		},
		{
			(EInputActionOrigin)177,
			Controller.dPadNorth
		},
		{
			(EInputActionOrigin)178,
			Controller.dPadSouth
		},
		{
			(EInputActionOrigin)179,
			Controller.dPadWest
		},
		{
			(EInputActionOrigin)180,
			Controller.dPadEast
		},
		{
			(EInputActionOrigin)159,
			Controller.startButton
		},
		{
			(EInputActionOrigin)160,
			Controller.selectButton
		},
		{
			(EInputActionOrigin)192,
			Controller.faceButtonEast
		},
		{
			(EInputActionOrigin)193,
			Controller.faceButtonSouth
		},
		{
			(EInputActionOrigin)194,
			Controller.faceButtonWest
		},
		{
			(EInputActionOrigin)195,
			Controller.faceButtonNorth
		},
		{
			(EInputActionOrigin)196,
			Controller.leftBumper
		},
		{
			(EInputActionOrigin)197,
			Controller.rightBumper
		},
		{
			(EInputActionOrigin)217,
			Controller.dPadNorth
		},
		{
			(EInputActionOrigin)218,
			Controller.dPadSouth
		},
		{
			(EInputActionOrigin)219,
			Controller.dPadWest
		},
		{
			(EInputActionOrigin)220,
			Controller.dPadEast
		},
		{
			(EInputActionOrigin)198,
			Controller.startButton
		},
		{
			(EInputActionOrigin)199,
			Controller.selectButton
		},
		{
			(EInputActionOrigin)333,
			Controller.faceButtonSouth
		},
		{
			(EInputActionOrigin)334,
			Controller.faceButtonEast
		},
		{
			(EInputActionOrigin)335,
			Controller.faceButtonWest
		},
		{
			(EInputActionOrigin)336,
			Controller.faceButtonNorth
		},
		{
			(EInputActionOrigin)337,
			Controller.leftBumper
		},
		{
			(EInputActionOrigin)338,
			Controller.rightBumper
		},
		{
			(EInputActionOrigin)356,
			Controller.leftTrigger
		},
		{
			(EInputActionOrigin)358,
			Controller.rightTrigger
		},
		{
			(EInputActionOrigin)360,
			Controller.joystickPress
		},
		{
			(EInputActionOrigin)367,
			Controller.joystickPress
		},
		{
			(EInputActionOrigin)378,
			Controller.dPadNorth
		},
		{
			(EInputActionOrigin)379,
			Controller.dPadSouth
		},
		{
			(EInputActionOrigin)380,
			Controller.dPadWest
		},
		{
			(EInputActionOrigin)381,
			Controller.dPadEast
		},
		{
			(EInputActionOrigin)339,
			Controller.startButton
		},
		{
			(EInputActionOrigin)340,
			Controller.selectButton
		}
	};

	private InputHandle_t? _currentControllerHandle;

	private InputActionSetHandle_t? _currentActionSetHandle;

	private ControllerConfig? _controllerConfig;

	private readonly List<string> _pressedInputs = new List<string>();

	private readonly Dictionary<StringName, InputEventAction> _inputEvents = new Dictionary<StringName, InputEventAction>();

	private readonly IControllerInputStrategy _fallbackStrategy = new GodotControllerInputStrategy();

	private double _nextControllerCheckTime;

	private readonly Dictionary<StringName, InputDigitalActionHandle_t> _digitalActionHandleCache = new Dictionary<StringName, InputDigitalActionHandle_t>();

	private Dictionary<EInputActionOrigin, Texture2D> _fallbackSteamGlyphs = new Dictionary<EInputActionOrigin, Texture2D>();

	private InputAnalogActionHandle_t _joystickActionHandle;

	private Vector2 _joystickPosition;

	private InputEventJoypadMotion _joystickXAxis;

	private InputEventJoypadMotion _joystickYAxis;

	private StringName _up;

	private StringName _down;

	private StringName _left;

	private StringName _right;

	public ControllerConfig? ControllerConfig => _controllerConfig ?? _fallbackStrategy.ControllerConfig;

	public Dictionary<StringName, StringName> GetDefaultControllerInputMap
	{
		get
		{
			if (ControllerConfig == null)
			{
				return _fallbackStrategy.ControllerConfig.DefaultControllerInputMap;
			}
			return ControllerConfig.DefaultControllerInputMap;
		}
	}

	public bool ShouldAllowControllerRebinding
	{
		get
		{
			if (!SteamInitializer.Initialized)
			{
				return _fallbackStrategy.ShouldAllowControllerRebinding;
			}
			return false;
		}
	}

	public async Task Init()
	{
		await ((GodotObject)NControllerManager.Instance).ToSignal((GodotObject)(object)((Node)NControllerManager.Instance).GetTree(), SignalName.ProcessFrame);
		if (SteamInitializer.Initialized)
		{
			try
			{
				SteamInput.Init(false);
				UpdateControllerConnections();
				_joystickXAxis = new InputEventJoypadMotion
				{
					Axis = (JoyAxis)0,
					Device = 0
				};
				_joystickYAxis = new InputEventJoypadMotion
				{
					Axis = (JoyAxis)1,
					Device = 0
				};
				_up = new StringName("Up");
				_down = new StringName("Down");
				_left = new StringName("Left");
				_right = new StringName("Right");
				_joystickActionHandle = SteamInput.GetAnalogActionHandle("Joystick");
				NInputManager.Instance.ResetToDefaultControllerMapping();
			}
			catch (InvalidOperationException ex)
			{
				Log.Error("Failed to initialize Steam Input: " + ex.Message);
			}
		}
		else
		{
			Log.Warn("Cannot initialize Steam Input because Steamworks is not initialized. Falling back to standard input.");
		}
		await _fallbackStrategy.Init();
	}

	public void ProcessInput()
	{
		if (!SteamInitializer.Initialized)
		{
			_fallbackStrategy.ProcessInput();
			return;
		}
		double num = (double)Time.GetTicksMsec() / 1000.0;
		if (num >= _nextControllerCheckTime)
		{
			_nextControllerCheckTime = num + 1.0;
			UpdateControllerConnections();
		}
		if (!_currentControllerHandle.HasValue)
		{
			_fallbackStrategy.ProcessInput();
			return;
		}
		try
		{
			SteamInput.RunFrame(true);
			ProcessDigitalInputs();
			ProcessAnalogInputs();
		}
		catch (InvalidOperationException ex)
		{
			Log.Error("Error running Steam Input frame: " + ex.Message);
			_currentControllerHandle = null;
			_fallbackStrategy.ProcessInput();
		}
	}

	private void ProcessDigitalInputs()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (_controllerConfig == null)
		{
			return;
		}
		foreach (KeyValuePair<string, StringName> item in _controllerConfig.SteamInputControllerMap)
		{
			if (!_digitalActionHandleCache.TryGetValue(StringName.op_Implicit(item.Key), out var value))
			{
				Log.Error("The input " + item.Key + " was not cached during initialization. Skipping...");
				continue;
			}
			bool flag = SteamInput.GetDigitalActionData(_currentControllerHandle.Value, value).bState == 1;
			bool flag2 = _pressedInputs.Contains(item.Key);
			if (flag && !flag2)
			{
				_pressedInputs.Add(item.Key);
			}
			else if (!flag && flag2)
			{
				_pressedInputs.Remove(item.Key);
			}
			if (flag && !flag2)
			{
				InputEventAction val = _inputEvents[StringName.op_Implicit(item.Key)];
				val.Pressed = true;
				Input.ParseInputEvent((InputEvent)(object)val);
			}
			else if (!flag && flag2)
			{
				InputEventAction val2 = _inputEvents[StringName.op_Implicit(item.Key)];
				val2.Pressed = false;
				Input.ParseInputEvent((InputEvent)(object)val2);
			}
		}
	}

	private void ProcessAnalogInputs()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		InputAnalogActionData_t analogActionData = SteamInput.GetAnalogActionData(_currentControllerHandle.Value, _joystickActionHandle);
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(analogActionData.x, analogActionData.y);
		if (((Vector2)(ref val)).DistanceTo(_joystickPosition) > 0.05f)
		{
			InputEventJoypadMotion joystickXAxis = _joystickXAxis;
			joystickXAxis.AxisValue = val.X;
			InputEventJoypadMotion joystickYAxis = _joystickYAxis;
			joystickYAxis.AxisValue = 0f - val.Y;
			Input.ParseInputEvent((InputEvent)(object)joystickXAxis);
			Input.ParseInputEvent((InputEvent)(object)joystickYAxis);
		}
		if (val.Y >= 0.5f && !_pressedInputs.Contains("Joy_Up"))
		{
			InputEventAction val2 = _inputEvents[_up];
			val2.Pressed = true;
			Input.ParseInputEvent((InputEvent)(object)val2);
			_pressedInputs.Add("Joy_Up");
		}
		else if (val.Y < 0.5f && _pressedInputs.Contains("Joy_Up"))
		{
			InputEventAction val3 = _inputEvents[_up];
			val3.Pressed = false;
			Input.ParseInputEvent((InputEvent)(object)val3);
			_pressedInputs.Remove("Joy_Up");
		}
		if (val.Y <= -0.5f && !_pressedInputs.Contains("Joy_Down"))
		{
			InputEventAction val4 = _inputEvents[_down];
			val4.Pressed = true;
			Input.ParseInputEvent((InputEvent)(object)val4);
			_pressedInputs.Add("Joy_Down");
		}
		else if (val.Y > -0.5f && _pressedInputs.Contains("Joy_Down"))
		{
			InputEventAction val5 = _inputEvents[_down];
			val5.Pressed = false;
			Input.ParseInputEvent((InputEvent)(object)val5);
			_pressedInputs.Remove("Joy_Down");
		}
		if (val.X <= -0.5f && !_pressedInputs.Contains("Joy_Left"))
		{
			InputEventAction val6 = _inputEvents[_left];
			val6.Pressed = true;
			Input.ParseInputEvent((InputEvent)(object)val6);
			_pressedInputs.Add("Joy_Left");
		}
		else if (val.X > -0.5f && _pressedInputs.Contains("Joy_Left"))
		{
			InputEventAction val7 = _inputEvents[_left];
			val7.Pressed = false;
			Input.ParseInputEvent((InputEvent)(object)val7);
			_pressedInputs.Remove("Joy_Left");
		}
		if (val.X >= 0.5f && !_pressedInputs.Contains("Joy_Right"))
		{
			InputEventAction val8 = _inputEvents[_right];
			val8.Pressed = true;
			Input.ParseInputEvent((InputEvent)(object)val8);
			_pressedInputs.Add("Joy_Right");
		}
		else if (val.X < 0.5f && _pressedInputs.Contains("Joy_Right"))
		{
			InputEventAction val9 = _inputEvents[_right];
			val9.Pressed = false;
			Input.ParseInputEvent((InputEvent)(object)val9);
			_pressedInputs.Remove("Joy_Right");
		}
		_joystickPosition = val;
	}

	private void UpdateControllerConnections()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			InputHandle_t[] array = (InputHandle_t[])(object)new InputHandle_t[16];
			if (SteamInput.GetConnectedControllers(array) == 0)
			{
				_currentControllerHandle = null;
				if (_controllerConfig == null)
				{
					UpdateControllerConfig((ESteamInputType)14);
				}
				return;
			}
			ESteamInputType val = (ESteamInputType)0;
			if (_currentControllerHandle.HasValue)
			{
				val = SteamInput.GetInputTypeForHandle(_currentControllerHandle.Value);
			}
			_currentControllerHandle = array[0];
			ESteamInputType inputTypeForHandle = SteamInput.GetInputTypeForHandle(_currentControllerHandle.Value);
			if (val != inputTypeForHandle)
			{
				UpdateControllerConfig(inputTypeForHandle);
				UpdateInputMap();
			}
			_currentActionSetHandle = SteamInput.GetActionSetHandle("Controls");
			SteamInput.ActivateActionSet(_currentControllerHandle.Value, _currentActionSetHandle.Value);
		}
		catch (InvalidOperationException ex)
		{
			Log.Error("Failed to connect to Steam controller: " + ex.Message);
			_currentControllerHandle = null;
		}
	}

	private void UpdateControllerConfig(ESteamInputType controllerType)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected I4, but got Unknown
		ControllerConfig controllerConfig = _controllerConfig;
		switch (controllerType - 1)
		{
		case 2:
			_controllerConfig = new XboxOneConfig();
			break;
		case 1:
			_controllerConfig = new Xbox360Config();
			break;
		case 4:
		case 11:
		case 12:
			_controllerConfig = new Ps4Config();
			break;
		case 7:
		case 8:
		case 9:
			_controllerConfig = new SwitchConfig();
			break;
		default:
			_controllerConfig = new SteamControllerConfig();
			break;
		}
		if (controllerConfig != null && controllerConfig.ControllerMappingType != _controllerConfig.ControllerMappingType)
		{
			NControllerManager.Instance?.OnControllerTypeChanged();
		}
	}

	private void UpdateInputMap()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		if (_controllerConfig == null)
		{
			return;
		}
		_inputEvents.Clear();
		_digitalActionHandleCache.Clear();
		foreach (KeyValuePair<string, StringName> item in _controllerConfig.SteamInputControllerMap)
		{
			_inputEvents[StringName.op_Implicit(item.Key)] = new InputEventAction
			{
				Action = item.Value
			};
		}
		foreach (string key in _controllerConfig.SteamInputControllerMap.Keys)
		{
			StringName val = StringName.op_Implicit(key);
			try
			{
				InputDigitalActionHandle_t digitalActionHandle = SteamInput.GetDigitalActionHandle(StringName.op_Implicit(val));
				_digitalActionHandleCache[val] = digitalActionHandle;
			}
			catch (InvalidOperationException ex)
			{
				Log.Error($"Failed to cache digital action handle for {val}: {ex.Message}");
			}
		}
	}

	public Texture2D? GetHotkeyIcon(string hotkey)
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		if (!SteamInitializer.Initialized || !_currentControllerHandle.HasValue)
		{
			return _fallbackStrategy.GetHotkeyIcon(hotkey);
		}
		Dictionary<string, StringName> source = ((ControllerConfig != null) ? ControllerConfig.SteamInputControllerMap : new SteamControllerConfig().SteamInputControllerMap);
		string key = source.FirstOrDefault((KeyValuePair<string, StringName> kvp) => kvp.Value == StringName.op_Implicit(hotkey)).Key;
		if (key == null)
		{
			return ControllerConfig?.GetButtonIcon(hotkey);
		}
		EInputActionOrigin[] array = (EInputActionOrigin[])(object)new EInputActionOrigin[8];
		if (!_digitalActionHandleCache.TryGetValue(StringName.op_Implicit(key), out var value))
		{
			Log.Error("The input " + key + " was not cached during initialization.");
			return ControllerConfig?.GetButtonIcon(key);
		}
		SteamInput.GetDigitalActionOrigins(_currentControllerHandle.Value, _currentActionSetHandle.Value, value, array);
		if (array.Length != 0 && array[0])
		{
			ESteamInputType inputTypeForHandle = SteamInput.GetInputTypeForHandle(_currentControllerHandle.Value);
			EInputActionOrigin val = SteamInput.TranslateActionOrigin(inputTypeForHandle, array[0]);
			if (_steamInputsToMegaInputs.TryGetValue(val, out StringName value2))
			{
				return ControllerConfig?.GetButtonIcon(StringName.op_Implicit(value2));
			}
			if (!_fallbackSteamGlyphs.ContainsKey(val))
			{
				string glyphSVGForActionOrigin = SteamInput.GetGlyphSVGForActionOrigin(val, 0u);
				Image val2 = Image.LoadFromFile(glyphSVGForActionOrigin);
				_fallbackSteamGlyphs.Add(val, (Texture2D)(object)ImageTexture.CreateFromImage(val2));
			}
			return _fallbackSteamGlyphs[val];
		}
		return ControllerConfig?.GetButtonIcon(hotkey);
	}
}
