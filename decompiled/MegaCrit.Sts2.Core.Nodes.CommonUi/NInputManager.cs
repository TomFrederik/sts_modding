using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NInputManager.cs")]
public class NInputManager : Node
{
	[Signal]
	public delegate void InputReboundEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _UnhandledKeyInput = StringName.op_Implicit("_UnhandledKeyInput");

		public static readonly StringName ProcessDebugKeyInput = StringName.op_Implicit("ProcessDebugKeyInput");

		public static readonly StringName ProcessShortcutKeyInput = StringName.op_Implicit("ProcessShortcutKeyInput");

		public static readonly StringName _UnhandledInput = StringName.op_Implicit("_UnhandledInput");

		public static readonly StringName GetShortcutKey = StringName.op_Implicit("GetShortcutKey");

		public static readonly StringName GetHotkeyIcon = StringName.op_Implicit("GetHotkeyIcon");

		public static readonly StringName ModifyShortcutKey = StringName.op_Implicit("ModifyShortcutKey");

		public static readonly StringName ModifyControllerButton = StringName.op_Implicit("ModifyControllerButton");

		public static readonly StringName ResetToDefaults = StringName.op_Implicit("ResetToDefaults");

		public static readonly StringName ResetToDefaultControllerMapping = StringName.op_Implicit("ResetToDefaultControllerMapping");

		public static readonly StringName OnControllerTypeChanged = StringName.op_Implicit("OnControllerTypeChanged");

		public static readonly StringName SaveControllerInputMapping = StringName.op_Implicit("SaveControllerInputMapping");

		public static readonly StringName SaveKeyboardInputMapping = StringName.op_Implicit("SaveKeyboardInputMapping");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ControllerManager = StringName.op_Implicit("ControllerManager");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName InputRebound = StringName.op_Implicit("InputRebound");
	}

	private readonly Dictionary<Key, StringName> _debugInputMap = new Dictionary<Key, StringName>
	{
		{
			(Key)49,
			DebugHotkey.hideTopBar
		},
		{
			(Key)50,
			DebugHotkey.hideIntents
		},
		{
			(Key)51,
			DebugHotkey.hideCombatUi
		},
		{
			(Key)52,
			DebugHotkey.hidePlayContainer
		},
		{
			(Key)53,
			DebugHotkey.hideHand
		},
		{
			(Key)54,
			DebugHotkey.hideHpBars
		},
		{
			(Key)55,
			DebugHotkey.hideTextVfx
		},
		{
			(Key)56,
			DebugHotkey.hideTargetingUi
		},
		{
			(Key)57,
			DebugHotkey.slowRewards
		},
		{
			(Key)48,
			DebugHotkey.hideVersionInfo
		},
		{
			(Key)45,
			DebugHotkey.speedDown
		},
		{
			(Key)61,
			DebugHotkey.speedUp
		},
		{
			(Key)4194332,
			DebugHotkey.hideRestSite
		},
		{
			(Key)4194334,
			DebugHotkey.hideEventUi
		},
		{
			(Key)4194335,
			DebugHotkey.hideProceedButton
		},
		{
			(Key)4194336,
			DebugHotkey.hideHoverTips
		},
		{
			(Key)4194337,
			DebugHotkey.hideMpCursors
		},
		{
			(Key)4194338,
			DebugHotkey.hideMpTargeting
		},
		{
			(Key)4194340,
			DebugHotkey.hideMpIntents
		},
		{
			(Key)4194341,
			DebugHotkey.hideMpHealthBars
		},
		{
			(Key)85,
			DebugHotkey.unlockCharacters
		}
	};

	public static readonly IReadOnlyList<StringName> remappableKeyboardInputs = new List<StringName>
	{
		MegaInput.select,
		MegaInput.cancel,
		MegaInput.viewMap,
		MegaInput.viewDeckAndTabLeft,
		MegaInput.viewDrawPile,
		MegaInput.viewDiscardPile,
		MegaInput.viewExhaustPileAndTabRight,
		MegaInput.accept,
		MegaInput.peek,
		MegaInput.up,
		MegaInput.down,
		MegaInput.left,
		MegaInput.right,
		MegaInput.selectCard1,
		MegaInput.selectCard2,
		MegaInput.selectCard3,
		MegaInput.selectCard4,
		MegaInput.selectCard5,
		MegaInput.selectCard6,
		MegaInput.selectCard7,
		MegaInput.selectCard8,
		MegaInput.selectCard9,
		MegaInput.selectCard10,
		MegaInput.releaseCard
	};

	public static readonly IReadOnlyList<StringName> remappableControllerInputs = new List<StringName>
	{
		MegaInput.select,
		MegaInput.cancel,
		MegaInput.viewMap,
		MegaInput.topPanel,
		MegaInput.viewDeckAndTabLeft,
		MegaInput.viewDrawPile,
		MegaInput.viewDiscardPile,
		MegaInput.viewExhaustPileAndTabRight,
		MegaInput.accept,
		MegaInput.peek,
		MegaInput.up,
		MegaInput.down,
		MegaInput.left,
		MegaInput.right
	};

	private Dictionary<StringName, Key> _keyboardInputMap = new Dictionary<StringName, Key>();

	private Dictionary<StringName, StringName> _controllerInputMap = new Dictionary<StringName, StringName>();

	private InputReboundEventHandler backing_InputRebound;

	public static NInputManager? Instance
	{
		get
		{
			if (NGame.Instance == null)
			{
				return null;
			}
			return NGame.Instance.InputManager;
		}
	}

	private static Dictionary<StringName, Key> DefaultKeyboardInputMap => new Dictionary<StringName, Key>
	{
		{
			MegaInput.accept,
			(Key)69
		},
		{
			MegaInput.select,
			(Key)4194309
		},
		{
			MegaInput.viewDiscardPile,
			(Key)83
		},
		{
			MegaInput.viewDeckAndTabLeft,
			(Key)68
		},
		{
			MegaInput.viewExhaustPileAndTabRight,
			(Key)88
		},
		{
			MegaInput.viewDrawPile,
			(Key)65
		},
		{
			MegaInput.viewMap,
			(Key)77
		},
		{
			MegaInput.cancel,
			(Key)4194305
		},
		{
			MegaInput.peek,
			(Key)32
		},
		{
			MegaInput.up,
			(Key)4194320
		},
		{
			MegaInput.down,
			(Key)4194322
		},
		{
			MegaInput.left,
			(Key)4194319
		},
		{
			MegaInput.right,
			(Key)4194321
		},
		{
			MegaInput.pauseAndBack,
			(Key)4194305
		},
		{
			MegaInput.selectCard1,
			(Key)49
		},
		{
			MegaInput.selectCard2,
			(Key)50
		},
		{
			MegaInput.selectCard3,
			(Key)51
		},
		{
			MegaInput.selectCard4,
			(Key)52
		},
		{
			MegaInput.selectCard5,
			(Key)53
		},
		{
			MegaInput.selectCard6,
			(Key)54
		},
		{
			MegaInput.selectCard7,
			(Key)55
		},
		{
			MegaInput.selectCard8,
			(Key)56
		},
		{
			MegaInput.selectCard9,
			(Key)57
		},
		{
			MegaInput.selectCard10,
			(Key)48
		},
		{
			MegaInput.releaseCard,
			(Key)4194322
		}
	};

	public NControllerManager ControllerManager { get; private set; }

	public event InputReboundEventHandler InputRebound
	{
		add
		{
			backing_InputRebound = (InputReboundEventHandler)Delegate.Combine(backing_InputRebound, value);
		}
		remove
		{
			backing_InputRebound = (InputReboundEventHandler)Delegate.Remove(backing_InputRebound, value);
		}
	}

	public override void _EnterTree()
	{
		ControllerManager = ((Node)this).GetNode<NControllerManager>(NodePath.op_Implicit("%ControllerManager"));
	}

	public override void _Ready()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)ControllerManager).Connect(NControllerManager.SignalName.ControllerTypeChanged, Callable.From((Action)OnControllerTypeChanged), 0u);
		TaskHelper.RunSafely(Init());
	}

	private async Task Init()
	{
		await ControllerManager.Init();
		SettingsSave settingsSave = SaveManager.Instance.SettingsSave;
		if (settingsSave.KeyboardMapping.Count > 0)
		{
			_keyboardInputMap = new Dictionary<StringName, Key>();
			foreach (KeyValuePair<string, string> item in settingsSave.KeyboardMapping)
			{
				_keyboardInputMap.Add(StringName.op_Implicit(item.Key), Enum.Parse<Key>(item.Value));
			}
		}
		else
		{
			_keyboardInputMap = DefaultKeyboardInputMap;
			SaveKeyboardInputMapping();
		}
		if (settingsSave.ControllerMapping.Count > 0 && settingsSave.ControllerMappingType == ControllerManager.ControllerMappingType)
		{
			_controllerInputMap = new Dictionary<StringName, StringName>();
			{
				foreach (KeyValuePair<string, string> item2 in settingsSave.ControllerMapping)
				{
					_controllerInputMap.Add(StringName.op_Implicit(item2.Key), StringName.op_Implicit(item2.Value));
				}
				return;
			}
		}
		_controllerInputMap = ControllerManager.GetDefaultControllerInputMap;
		SaveControllerInputMapping();
	}

	public override void _UnhandledKeyInput(InputEvent inputEvent)
	{
		ProcessShortcutKeyInput(inputEvent);
		ProcessDebugKeyInput(inputEvent);
	}

	private void ProcessDebugKeyInput(InputEvent inputEvent)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		InputEventKey val = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
		if (val == null || ((CanvasItem)NDevConsole.Instance).Visible || !NGame.IsTrailerMode)
		{
			return;
		}
		foreach (KeyValuePair<Key, StringName> item in _debugInputMap)
		{
			if (val.Keycode == item.Key)
			{
				InputEventAction val2 = new InputEventAction
				{
					Action = item.Value,
					Pressed = inputEvent.IsPressed()
				};
				Input.ParseInputEvent((InputEvent)(object)val2);
			}
		}
	}

	private void ProcessShortcutKeyInput(InputEvent inputEvent)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		if (NGame.Instance.Transition.InTransition)
		{
			return;
		}
		InputEventKey val = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
		if (val == null)
		{
			return;
		}
		foreach (KeyValuePair<StringName, Key> item in _keyboardInputMap)
		{
			if (val.Keycode == item.Value && !inputEvent.IsEcho())
			{
				InputEventAction val2 = new InputEventAction
				{
					Action = item.Key,
					Pressed = inputEvent.IsPressed()
				};
				Input.ParseInputEvent((InputEvent)(object)val2);
			}
		}
	}

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		if (NGame.Instance.Transition.InTransition)
		{
			return;
		}
		foreach (KeyValuePair<StringName, StringName> item in _controllerInputMap)
		{
			if (inputEvent.IsActionPressed(item.Value, false, false))
			{
				InputEventAction val = new InputEventAction
				{
					Action = item.Key,
					Pressed = true
				};
				Input.ParseInputEvent((InputEvent)(object)val);
			}
			else if (inputEvent.IsActionReleased(item.Value, false))
			{
				InputEventAction val2 = new InputEventAction
				{
					Action = item.Key,
					Pressed = false
				};
				Input.ParseInputEvent((InputEvent)(object)val2);
			}
		}
	}

	public Key GetShortcutKey(StringName input)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return _keyboardInputMap[input];
	}

	public Texture2D? GetHotkeyIcon(string hotkey)
	{
		if (_controllerInputMap.TryGetValue(StringName.op_Implicit(hotkey), out StringName value))
		{
			return ControllerManager.GetHotkeyIcon(StringName.op_Implicit(value));
		}
		return null;
	}

	public void ModifyShortcutKey(StringName input, Key shortcutKey)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		KeyValuePair<StringName, Key> keyValuePair = _keyboardInputMap.FirstOrDefault<KeyValuePair<StringName, Key>>((KeyValuePair<StringName, Key> kvp) => kvp.Value == shortcutKey && remappableKeyboardInputs.Contains(kvp.Key));
		if (keyValuePair.Key != (StringName)null)
		{
			Key value = _keyboardInputMap[input];
			_keyboardInputMap[keyValuePair.Key] = value;
		}
		_keyboardInputMap[input] = shortcutKey;
		SaveKeyboardInputMapping();
		EmitSignalInputRebound();
	}

	public void ModifyControllerButton(StringName input, StringName controllerInput)
	{
		KeyValuePair<StringName, StringName> keyValuePair = _controllerInputMap.FirstOrDefault<KeyValuePair<StringName, StringName>>((KeyValuePair<StringName, StringName> kvp) => kvp.Value == controllerInput && remappableControllerInputs.Contains(kvp.Key));
		if (keyValuePair.Key != (StringName)null)
		{
			StringName value = _controllerInputMap[input];
			_controllerInputMap[keyValuePair.Key] = value;
		}
		_controllerInputMap[input] = controllerInput;
		SaveControllerInputMapping();
		EmitSignalInputRebound();
	}

	public void ResetToDefaults()
	{
		_keyboardInputMap = DefaultKeyboardInputMap;
		_controllerInputMap = ControllerManager.GetDefaultControllerInputMap;
		SaveControllerInputMapping();
		SaveKeyboardInputMapping();
		EmitSignalInputRebound();
	}

	public void ResetToDefaultControllerMapping()
	{
		_controllerInputMap = ControllerManager.GetDefaultControllerInputMap;
		SaveControllerInputMapping();
		EmitSignalInputRebound();
	}

	private void OnControllerTypeChanged()
	{
		if (ControllerManager.ControllerMappingType != SaveManager.Instance.SettingsSave.ControllerMappingType)
		{
			_controllerInputMap = ControllerManager.GetDefaultControllerInputMap;
			SaveControllerInputMapping();
			EmitSignalInputRebound();
		}
	}

	private void SaveControllerInputMapping()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<StringName, StringName> item in _controllerInputMap)
		{
			dictionary.Add(((object)item.Key).ToString(), ((object)item.Value).ToString());
		}
		SaveManager.Instance.SettingsSave.ControllerMappingType = ControllerManager.ControllerMappingType;
		SaveManager.Instance.SettingsSave.ControllerMapping = dictionary;
		SaveManager.Instance.SaveSettings();
	}

	private void SaveKeyboardInputMapping()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<StringName, Key> item in _keyboardInputMap)
		{
			dictionary.Add(((object)item.Key).ToString(), ((object)item.Value/*cast due to .constrained prefix*/).ToString());
		}
		SaveManager.Instance.SettingsSave.KeyboardMapping = dictionary;
		SaveManager.Instance.SaveSettings();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(15);
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._UnhandledKeyInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessDebugKeyInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessShortcutKeyInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._UnhandledInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetShortcutKey, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)21, StringName.op_Implicit("input"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetHotkeyIcon, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("hotkey"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ModifyShortcutKey, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)21, StringName.op_Implicit("input"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("shortcutKey"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ModifyControllerButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)21, StringName.op_Implicit("input"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)21, StringName.op_Implicit("controllerInput"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ResetToDefaults, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ResetToDefaultControllerMapping, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnControllerTypeChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SaveControllerInputMapping, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SaveKeyboardInputMapping, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._UnhandledKeyInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._UnhandledKeyInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessDebugKeyInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessDebugKeyInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessShortcutKeyInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessShortcutKeyInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._UnhandledInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._UnhandledInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetShortcutKey && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Key shortcutKey = GetShortcutKey(VariantUtils.ConvertTo<StringName>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Key>(ref shortcutKey);
			return true;
		}
		if ((ref method) == MethodName.GetHotkeyIcon && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Texture2D hotkeyIcon = GetHotkeyIcon(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Texture2D>(ref hotkeyIcon);
			return true;
		}
		if ((ref method) == MethodName.ModifyShortcutKey && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			ModifyShortcutKey(VariantUtils.ConvertTo<StringName>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Key>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ModifyControllerButton && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			ModifyControllerButton(VariantUtils.ConvertTo<StringName>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<StringName>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ResetToDefaults && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ResetToDefaults();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ResetToDefaultControllerMapping && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ResetToDefaultControllerMapping();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnControllerTypeChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnControllerTypeChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SaveControllerInputMapping && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SaveControllerInputMapping();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SaveKeyboardInputMapping && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SaveKeyboardInputMapping();
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._UnhandledKeyInput)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessDebugKeyInput)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessShortcutKeyInput)
		{
			return true;
		}
		if ((ref method) == MethodName._UnhandledInput)
		{
			return true;
		}
		if ((ref method) == MethodName.GetShortcutKey)
		{
			return true;
		}
		if ((ref method) == MethodName.GetHotkeyIcon)
		{
			return true;
		}
		if ((ref method) == MethodName.ModifyShortcutKey)
		{
			return true;
		}
		if ((ref method) == MethodName.ModifyControllerButton)
		{
			return true;
		}
		if ((ref method) == MethodName.ResetToDefaults)
		{
			return true;
		}
		if ((ref method) == MethodName.ResetToDefaultControllerMapping)
		{
			return true;
		}
		if ((ref method) == MethodName.OnControllerTypeChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.SaveControllerInputMapping)
		{
			return true;
		}
		if ((ref method) == MethodName.SaveKeyboardInputMapping)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.ControllerManager)
		{
			ControllerManager = VariantUtils.ConvertTo<NControllerManager>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ControllerManager)
		{
			NControllerManager controllerManager = ControllerManager;
			value = VariantUtils.CreateFrom<NControllerManager>(ref controllerManager);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.ControllerManager, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName controllerManager = PropertyName.ControllerManager;
		NControllerManager controllerManager2 = ControllerManager;
		info.AddProperty(controllerManager, Variant.From<NControllerManager>(ref controllerManager2));
		info.AddSignalEventDelegate(SignalName.InputRebound, (Delegate)backing_InputRebound);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.ControllerManager, ref val))
		{
			ControllerManager = ((Variant)(ref val)).As<NControllerManager>();
		}
		InputReboundEventHandler inputReboundEventHandler = default(InputReboundEventHandler);
		if (info.TryGetSignalEventDelegate<InputReboundEventHandler>(SignalName.InputRebound, ref inputReboundEventHandler))
		{
			backing_InputRebound = inputReboundEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.InputRebound, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalInputRebound()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.InputRebound, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.InputRebound && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_InputRebound?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.InputRebound)
		{
			return true;
		}
		return ((Node)this).HasGodotClassSignal(ref signal);
	}
}
