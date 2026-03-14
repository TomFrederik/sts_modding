using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NInputSettingsEntry.cs")]
public class NInputSettingsEntry : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName UpdateInput = StringName.op_Implicit("UpdateInput");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName InputName = StringName.op_Implicit("InputName");

		public static readonly StringName _bg = StringName.op_Implicit("_bg");

		public static readonly StringName _inputLabel = StringName.op_Implicit("_inputLabel");

		public static readonly StringName _keyBindingLabel = StringName.op_Implicit("_keyBindingLabel");

		public static readonly StringName _controllerBindingIcon = StringName.op_Implicit("_controllerBindingIcon");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly Dictionary<StringName, string> _commandToLocTitle = new Dictionary<StringName, string>
	{
		{
			MegaInput.accept,
			"endTurn"
		},
		{
			MegaInput.select,
			"confirmCard"
		},
		{
			MegaInput.viewDiscardPile,
			"viewDiscard"
		},
		{
			MegaInput.viewDrawPile,
			"viewDraw"
		},
		{
			MegaInput.viewDeckAndTabLeft,
			"viewDeck"
		},
		{
			MegaInput.viewExhaustPileAndTabRight,
			"viewExhaust"
		},
		{
			MegaInput.viewMap,
			"viewMap"
		},
		{
			MegaInput.cancel,
			"cancel"
		},
		{
			MegaInput.peek,
			"peek"
		},
		{
			MegaInput.up,
			"up"
		},
		{
			MegaInput.topPanel,
			"topPanel"
		},
		{
			MegaInput.down,
			"down"
		},
		{
			MegaInput.left,
			"left"
		},
		{
			MegaInput.right,
			"right"
		},
		{
			MegaInput.selectCard1,
			"selectCard1"
		},
		{
			MegaInput.selectCard2,
			"selectCard2"
		},
		{
			MegaInput.selectCard3,
			"selectCard3"
		},
		{
			MegaInput.selectCard4,
			"selectCard4"
		},
		{
			MegaInput.selectCard5,
			"selectCard5"
		},
		{
			MegaInput.selectCard6,
			"selectCard6"
		},
		{
			MegaInput.selectCard7,
			"selectCard7"
		},
		{
			MegaInput.selectCard8,
			"selectCard8"
		},
		{
			MegaInput.selectCard9,
			"selectCard9"
		},
		{
			MegaInput.selectCard10,
			"selectCard10"
		},
		{
			MegaInput.releaseCard,
			"releaseCard"
		}
	};

	private const string _scenePath = "res://scenes/screens/settings_screen/input_settings_entry.tscn";

	private Control _bg;

	private MegaRichTextLabel _inputLabel;

	private MegaRichTextLabel _keyBindingLabel;

	private TextureRect _controllerBindingIcon;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/screens/settings_screen/input_settings_entry.tscn");

	public StringName InputName { get; private set; }

	public static NInputSettingsEntry Create(string commandName)
	{
		NInputSettingsEntry nInputSettingsEntry = ResourceLoader.Load<PackedScene>("res://scenes/screens/settings_screen/input_settings_entry.tscn", (string)null, (CacheMode)1).Instantiate<NInputSettingsEntry>((GenEditState)0);
		nInputSettingsEntry.InputName = StringName.op_Implicit(commandName);
		return nInputSettingsEntry;
	}

	public override void _Ready()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_inputLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%InputLabel"));
		_keyBindingLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%KeyBindingInputLabel"));
		_controllerBindingIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%ControllerBindingIcon"));
		_bg = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Bg"));
		string text = _commandToLocTitle[InputName];
		_inputLabel.Text = new LocString("settings_ui", "INPUT_SETTINGS.INPUT_TITLE." + text).GetFormattedText();
		((GodotObject)NInputManager.Instance).Connect(NInputManager.SignalName.InputRebound, Callable.From((Action)UpdateInput), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)UpdateInput), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)UpdateInput), 0u);
		((GodotObject)this).Connect(SignalName.VisibilityChanged, Callable.From((Action)UpdateInput), 0u);
	}

	public override void _ExitTree()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		base._ExitTree();
		((GodotObject)NInputManager.Instance).Disconnect(NInputManager.SignalName.InputRebound, Callable.From((Action)UpdateInput));
		((GodotObject)NControllerManager.Instance).Disconnect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)UpdateInput));
		((GodotObject)NControllerManager.Instance).Disconnect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)UpdateInput));
		((GodotObject)this).Disconnect(SignalName.VisibilityChanged, Callable.From((Action)UpdateInput));
	}

	private void UpdateInput()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)this).IsVisibleInTree())
		{
			if (NInputManager.remappableKeyboardInputs.Contains(InputName))
			{
				_keyBindingLabel.Text = ((object)NInputManager.Instance.GetShortcutKey(InputName)/*cast due to .constrained prefix*/).ToString();
			}
			else
			{
				_keyBindingLabel.Text = "";
			}
			if (NInputManager.remappableControllerInputs.Contains(InputName))
			{
				_controllerBindingIcon.Texture = NInputManager.Instance.GetHotkeyIcon(StringName.op_Implicit(InputName));
			}
			((CanvasItem)_controllerBindingIcon).Modulate = (Color)(NControllerManager.Instance.ShouldAllowControllerRebinding ? Colors.White : new Color(1f, 1f, 1f, 0.15f));
		}
	}

	protected override void OnFocus()
	{
		((CanvasItem)_bg).Visible = true;
	}

	protected override void OnUnfocus()
	{
		((CanvasItem)_bg).Visible = false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("commandName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NInputSettingsEntry nInputSettingsEntry = Create(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NInputSettingsEntry>(ref nInputSettingsEntry);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateInput && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateInput();
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
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NInputSettingsEntry nInputSettingsEntry = Create(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NInputSettingsEntry>(ref nInputSettingsEntry);
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateInput)
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
		if ((ref name) == PropertyName.InputName)
		{
			InputName = VariantUtils.ConvertTo<StringName>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bg)
		{
			_bg = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._inputLabel)
		{
			_inputLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._keyBindingLabel)
		{
			_keyBindingLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._controllerBindingIcon)
		{
			_controllerBindingIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
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
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InputName)
		{
			StringName inputName = InputName;
			value = VariantUtils.CreateFrom<StringName>(ref inputName);
			return true;
		}
		if ((ref name) == PropertyName._bg)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bg);
			return true;
		}
		if ((ref name) == PropertyName._inputLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _inputLabel);
			return true;
		}
		if ((ref name) == PropertyName._keyBindingLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _keyBindingLabel);
			return true;
		}
		if ((ref name) == PropertyName._controllerBindingIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _controllerBindingIcon);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)21, PropertyName.InputName, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bg, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._inputLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._keyBindingLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._controllerBindingIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		StringName inputName = PropertyName.InputName;
		StringName inputName2 = InputName;
		info.AddProperty(inputName, Variant.From<StringName>(ref inputName2));
		info.AddProperty(PropertyName._bg, Variant.From<Control>(ref _bg));
		info.AddProperty(PropertyName._inputLabel, Variant.From<MegaRichTextLabel>(ref _inputLabel));
		info.AddProperty(PropertyName._keyBindingLabel, Variant.From<MegaRichTextLabel>(ref _keyBindingLabel));
		info.AddProperty(PropertyName._controllerBindingIcon, Variant.From<TextureRect>(ref _controllerBindingIcon));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.InputName, ref val))
		{
			InputName = ((Variant)(ref val)).As<StringName>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._bg, ref val2))
		{
			_bg = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._inputLabel, ref val3))
		{
			_inputLabel = ((Variant)(ref val3)).As<MegaRichTextLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._keyBindingLabel, ref val4))
		{
			_keyBindingLabel = ((Variant)(ref val4)).As<MegaRichTextLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._controllerBindingIcon, ref val5))
		{
			_controllerBindingIcon = ((Variant)(ref val5)).As<TextureRect>();
		}
	}
}
