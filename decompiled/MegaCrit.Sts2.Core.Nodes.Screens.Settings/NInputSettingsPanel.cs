using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NInputSettingsPanel.cs")]
public class NInputSettingsPanel : NSettingsPanel
{
	public new class MethodName : NSettingsPanel.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnViewportSizeChange = StringName.op_Implicit("OnViewportSizeChange");

		public new static readonly StringName OnVisibilityChange = StringName.op_Implicit("OnVisibilityChange");

		public static readonly StringName SetAsListeningEntry = StringName.op_Implicit("SetAsListeningEntry");

		public static readonly StringName _UnhandledKeyInput = StringName.op_Implicit("_UnhandledKeyInput");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");
	}

	public new class PropertyName : NSettingsPanel.PropertyName
	{
		public new static readonly StringName _minPadding = StringName.op_Implicit("_minPadding");

		public static readonly StringName _listeningEntry = StringName.op_Implicit("_listeningEntry");

		public static readonly StringName _resetToDefaultButton = StringName.op_Implicit("_resetToDefaultButton");

		public static readonly StringName _commandHeader = StringName.op_Implicit("_commandHeader");

		public static readonly StringName _keyboardHeader = StringName.op_Implicit("_keyboardHeader");

		public static readonly StringName _controllerHeader = StringName.op_Implicit("_controllerHeader");

		public static readonly StringName _steamInputPrompt = StringName.op_Implicit("_steamInputPrompt");
	}

	public new class SignalName : NSettingsPanel.SignalName
	{
	}

	private float _minPadding = 50f;

	private NInputSettingsEntry? _listeningEntry;

	private NButton _resetToDefaultButton;

	private MegaRichTextLabel _commandHeader;

	private MegaRichTextLabel _keyboardHeader;

	private MegaRichTextLabel _controllerHeader;

	private MegaRichTextLabel _steamInputPrompt;

	public override void _Ready()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		base._Ready();
		_resetToDefaultButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("%ResetToDefaultButton"));
		_commandHeader = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%CommandHeader"));
		_keyboardHeader = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%KeyboardHeader"));
		_controllerHeader = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%ControllerHeader"));
		_steamInputPrompt = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%SteamInputPrompt"));
		((GodotObject)_resetToDefaultButton).Connect(NClickableControl.SignalName.Released, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
		{
			NInputManager.Instance.ResetToDefaults();
		}), 0u);
		((GodotObject)((Node)this).GetViewport()).Connect(SignalName.SizeChanged, Callable.From((Action)OnViewportSizeChange), 0u);
		_commandHeader.Text = new LocString("settings_ui", "INPUT_SETTINGS.COMMAND_HEADER").GetFormattedText();
		_keyboardHeader.Text = new LocString("settings_ui", "INPUT_SETTINGS.KEYBOARD_HEADER").GetFormattedText();
		_controllerHeader.Text = new LocString("settings_ui", "INPUT_SETTINGS.CONTROLLER_HEADER").GetFormattedText();
		_steamInputPrompt.Text = new LocString("settings_ui", "INPUT_SETTINGS.STEAM_INPUT_DETECTED").GetFormattedText();
		IReadOnlyList<StringName> readOnlyList = NInputManager.remappableControllerInputs.Concat(NInputManager.remappableKeyboardInputs).Distinct().ToList();
		List<NInputSettingsEntry> list = ((IEnumerable)((Node)base.Content).GetChildren(false)).OfType<NInputSettingsEntry>().ToList();
		foreach (StringName item in readOnlyList)
		{
			NInputSettingsEntry entry = NInputSettingsEntry.Create(StringName.op_Implicit(item));
			((GodotObject)entry).Connect(NClickableControl.SignalName.Released, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
			{
				SetAsListeningEntry(entry);
			}), 0u);
			((Node)(object)base.Content).AddChildSafely((Node?)(object)entry);
			list.Add(entry);
		}
		for (int num = 0; num < list.Count; num++)
		{
			((Control)list[num]).FocusNeighborLeft = ((Node)list[num]).GetPath();
			((Control)list[num]).FocusNeighborRight = ((Node)list[num]).GetPath();
			((Control)list[num]).FocusNeighborTop = ((num > 0) ? ((Node)list[num - 1]).GetPath() : ((Node)list[num]).GetPath());
			((Control)list[num]).FocusNeighborBottom = ((num < list.Count - 1) ? ((Node)list[num + 1]).GetPath() : ((Node)list[num]).GetPath());
		}
		((Control)_resetToDefaultButton).FocusNeighborLeft = ((Node)_resetToDefaultButton).GetPath();
		((Control)_resetToDefaultButton).FocusNeighborRight = ((Node)_resetToDefaultButton).GetPath();
		((Control)_resetToDefaultButton).FocusNeighborTop = ((Node)_resetToDefaultButton).GetPath();
		((Control)_resetToDefaultButton).FocusNeighborBottom = ((Node)list[0]).GetPath();
		((Control)list[0]).FocusNeighborTop = ((Node)_resetToDefaultButton).GetPath();
		_firstControl = (Control?)(object)((IEnumerable)((Node)base.Content).GetChildren(false)).OfType<NInputSettingsEntry>().First();
	}

	private async Task RefreshSize()
	{
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		Vector2 size = ((Node)this).GetParent<Control>().Size;
		Vector2 minimumSize = ((Control)base.Content).GetMinimumSize();
		if (minimumSize.Y + _minPadding >= size.Y)
		{
			((Control)this).Size = new Vector2(((Control)base.Content).Size.X, minimumSize.Y + size.Y * 0.4f);
		}
	}

	private void OnViewportSizeChange()
	{
		TaskHelper.RunSafely(RefreshSize());
	}

	protected override void OnVisibilityChange()
	{
		base.OnVisibilityChange();
		_listeningEntry = null;
		((CanvasItem)_steamInputPrompt).Visible = !NControllerManager.Instance.ShouldAllowControllerRebinding;
		TaskHelper.RunSafely(RefreshSize());
	}

	private void SetAsListeningEntry(NInputSettingsEntry entry)
	{
		_listeningEntry = entry;
	}

	public override void _UnhandledKeyInput(InputEvent inputEvent)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (_listeningEntry == null || !NInputManager.remappableKeyboardInputs.Contains(_listeningEntry.InputName))
		{
			return;
		}
		InputEventKey val = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
		if (val != null)
		{
			NInputManager.Instance.ModifyShortcutKey(_listeningEntry.InputName, val.Keycode);
			Viewport viewport = ((Node)this).GetViewport();
			if (viewport != null)
			{
				viewport.SetInputAsHandled();
			}
			_listeningEntry = null;
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (_listeningEntry == null)
		{
			return;
		}
		StringName[] allControllerInputs = Controller.AllControllerInputs;
		foreach (StringName val in allControllerInputs)
		{
			if (inputEvent.IsActionReleased(val, false))
			{
				if (NInputManager.remappableControllerInputs.Contains(_listeningEntry.InputName) && NControllerManager.Instance.ShouldAllowControllerRebinding)
				{
					NInputManager.Instance.ModifyControllerButton(_listeningEntry.InputName, val);
				}
				Viewport viewport = ((Node)this).GetViewport();
				if (viewport != null)
				{
					viewport.SetInputAsHandled();
				}
				_listeningEntry = null;
				break;
			}
		}
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
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnViewportSizeChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnVisibilityChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAsListeningEntry, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("entry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._UnhandledKeyInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
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
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnViewportSizeChange && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnViewportSizeChange();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnVisibilityChange && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnVisibilityChange();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAsListeningEntry && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetAsListeningEntry(VariantUtils.ConvertTo<NInputSettingsEntry>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._UnhandledKeyInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._UnhandledKeyInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
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
		if ((ref method) == MethodName.OnViewportSizeChange)
		{
			return true;
		}
		if ((ref method) == MethodName.OnVisibilityChange)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAsListeningEntry)
		{
			return true;
		}
		if ((ref method) == MethodName._UnhandledKeyInput)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._minPadding)
		{
			_minPadding = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._listeningEntry)
		{
			_listeningEntry = VariantUtils.ConvertTo<NInputSettingsEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._resetToDefaultButton)
		{
			_resetToDefaultButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._commandHeader)
		{
			_commandHeader = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._keyboardHeader)
		{
			_keyboardHeader = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._controllerHeader)
		{
			_controllerHeader = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steamInputPrompt)
		{
			_steamInputPrompt = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._minPadding)
		{
			value = VariantUtils.CreateFrom<float>(ref _minPadding);
			return true;
		}
		if ((ref name) == PropertyName._listeningEntry)
		{
			value = VariantUtils.CreateFrom<NInputSettingsEntry>(ref _listeningEntry);
			return true;
		}
		if ((ref name) == PropertyName._resetToDefaultButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _resetToDefaultButton);
			return true;
		}
		if ((ref name) == PropertyName._commandHeader)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _commandHeader);
			return true;
		}
		if ((ref name) == PropertyName._keyboardHeader)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _keyboardHeader);
			return true;
		}
		if ((ref name) == PropertyName._controllerHeader)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _controllerHeader);
			return true;
		}
		if ((ref name) == PropertyName._steamInputPrompt)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _steamInputPrompt);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._minPadding, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._listeningEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._resetToDefaultButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._commandHeader, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._keyboardHeader, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._controllerHeader, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steamInputPrompt, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._minPadding, Variant.From<float>(ref _minPadding));
		info.AddProperty(PropertyName._listeningEntry, Variant.From<NInputSettingsEntry>(ref _listeningEntry));
		info.AddProperty(PropertyName._resetToDefaultButton, Variant.From<NButton>(ref _resetToDefaultButton));
		info.AddProperty(PropertyName._commandHeader, Variant.From<MegaRichTextLabel>(ref _commandHeader));
		info.AddProperty(PropertyName._keyboardHeader, Variant.From<MegaRichTextLabel>(ref _keyboardHeader));
		info.AddProperty(PropertyName._controllerHeader, Variant.From<MegaRichTextLabel>(ref _controllerHeader));
		info.AddProperty(PropertyName._steamInputPrompt, Variant.From<MegaRichTextLabel>(ref _steamInputPrompt));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._minPadding, ref val))
		{
			_minPadding = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._listeningEntry, ref val2))
		{
			_listeningEntry = ((Variant)(ref val2)).As<NInputSettingsEntry>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._resetToDefaultButton, ref val3))
		{
			_resetToDefaultButton = ((Variant)(ref val3)).As<NButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._commandHeader, ref val4))
		{
			_commandHeader = ((Variant)(ref val4)).As<MegaRichTextLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._keyboardHeader, ref val5))
		{
			_keyboardHeader = ((Variant)(ref val5)).As<MegaRichTextLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._controllerHeader, ref val6))
		{
			_controllerHeader = ((Variant)(ref val6)).As<MegaRichTextLabel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._steamInputPrompt, ref val7))
		{
			_steamInputPrompt = ((Variant)(ref val7)).As<MegaRichTextLabel>();
		}
	}
}
