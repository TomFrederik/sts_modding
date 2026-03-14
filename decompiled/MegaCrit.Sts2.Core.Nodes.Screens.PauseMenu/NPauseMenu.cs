using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.PauseMenu;

[ScriptPath("res://src/Core/Nodes/Screens/PauseMenu/NPauseMenu.cs")]
public class NPauseMenu : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshLabels = StringName.op_Implicit("RefreshLabels");

		public static readonly StringName OnBackOrResumeButtonPressed = StringName.op_Implicit("OnBackOrResumeButtonPressed");

		public static readonly StringName OnSettingsButtonPressed = StringName.op_Implicit("OnSettingsButtonPressed");

		public static readonly StringName OnCompendiumButtonPressed = StringName.op_Implicit("OnCompendiumButtonPressed");

		public static readonly StringName OnGiveUpButtonPressed = StringName.op_Implicit("OnGiveUpButtonPressed");

		public static readonly StringName OnDisconnectButtonPressed = StringName.op_Implicit("OnDisconnectButtonPressed");

		public static readonly StringName OnSaveAndQuitButtonPressed = StringName.op_Implicit("OnSaveAndQuitButtonPressed");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName Buttons = StringName.op_Implicit("Buttons");

		public static readonly StringName UseSharedBackstop = StringName.op_Implicit("UseSharedBackstop");

		public static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public new static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _buttonContainer = StringName.op_Implicit("_buttonContainer");

		public static readonly StringName _resumeButton = StringName.op_Implicit("_resumeButton");

		public static readonly StringName _settingsButton = StringName.op_Implicit("_settingsButton");

		public static readonly StringName _compendiumButton = StringName.op_Implicit("_compendiumButton");

		public static readonly StringName _giveUpButton = StringName.op_Implicit("_giveUpButton");

		public static readonly StringName _disconnectButton = StringName.op_Implicit("_disconnectButton");

		public static readonly StringName _saveAndQuitButton = StringName.op_Implicit("_saveAndQuitButton");

		public static readonly StringName _pausedLabel = StringName.op_Implicit("_pausedLabel");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly LocString _pausedLoc = new LocString("gameplay_ui", "PAUSE_MENU.PAUSED");

	private static readonly LocString _resumeLoc = new LocString("gameplay_ui", "PAUSE_MENU.RESUME");

	private static readonly LocString _settingsLoc = new LocString("gameplay_ui", "PAUSE_MENU.SETTINGS");

	private static readonly LocString _compendiumLoc = new LocString("gameplay_ui", "PAUSE_MENU.COMPENDIUM");

	private static readonly LocString _giveUpLoc = new LocString("gameplay_ui", "PAUSE_MENU.GIVE_UP");

	private static readonly LocString _disconnectLoc = new LocString("gameplay_ui", "PAUSE_MENU.DISCONNECT");

	private static readonly LocString _saveAndQuitLoc = new LocString("gameplay_ui", "PAUSE_MENU.SAVE_AND_QUIT");

	private NBackButton _backButton;

	private Control _buttonContainer;

	private NPauseMenuButton _resumeButton;

	private NPauseMenuButton _settingsButton;

	private NPauseMenuButton _compendiumButton;

	private NPauseMenuButton _giveUpButton;

	private NPauseMenuButton _disconnectButton;

	private NPauseMenuButton _saveAndQuitButton;

	private MegaLabel _pausedLabel;

	private IRunState _runState;

	protected override Control InitialFocusedControl => (Control)(object)_resumeButton;

	private NPauseMenuButton[] Buttons => new NPauseMenuButton[6] { _resumeButton, _settingsButton, _compendiumButton, _giveUpButton, _disconnectButton, _saveAndQuitButton };

	public bool UseSharedBackstop => true;

	public NetScreenType ScreenType => NetScreenType.PauseMenu;

	public override void _Ready()
	{
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_buttonContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ButtonContainer"));
		_resumeButton = ((Node)_buttonContainer).GetNode<NPauseMenuButton>(NodePath.op_Implicit("Resume"));
		_settingsButton = ((Node)_buttonContainer).GetNode<NPauseMenuButton>(NodePath.op_Implicit("Settings"));
		_compendiumButton = ((Node)_buttonContainer).GetNode<NPauseMenuButton>(NodePath.op_Implicit("Compendium"));
		_giveUpButton = ((Node)_buttonContainer).GetNode<NPauseMenuButton>(NodePath.op_Implicit("GiveUp"));
		_disconnectButton = ((Node)_buttonContainer).GetNode<NPauseMenuButton>(NodePath.op_Implicit("Disconnect"));
		_saveAndQuitButton = ((Node)_buttonContainer).GetNode<NPauseMenuButton>(NodePath.op_Implicit("SaveAndQuit"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%BackButton"));
		_pausedLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%PausedText/Label"));
		RefreshLabels();
		((GodotObject)_resumeButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnBackOrResumeButtonPressed), 0u);
		((GodotObject)_settingsButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnSettingsButtonPressed), 0u);
		((GodotObject)_compendiumButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnCompendiumButtonPressed), 0u);
		((GodotObject)_giveUpButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnGiveUpButtonPressed), 0u);
		((GodotObject)_disconnectButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnDisconnectButtonPressed), 0u);
		((GodotObject)_saveAndQuitButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnSaveAndQuitButtonPressed), 0u);
		((GodotObject)_backButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnBackOrResumeButtonPressed), 0u);
		_backButton.Disable();
		((CanvasItem)_giveUpButton).Visible = RunManager.Instance.NetService.Type != NetGameType.Client;
		((CanvasItem)_saveAndQuitButton).Visible = RunManager.Instance.NetService.Type != NetGameType.Client;
		((CanvasItem)_disconnectButton).Visible = RunManager.Instance.NetService.Type == NetGameType.Client;
		for (int i = 0; i < Buttons.Length; i++)
		{
			((Control)Buttons[i]).FocusNeighborLeft = ((Node)Buttons[i]).GetPath();
			((Control)Buttons[i]).FocusNeighborRight = ((Node)Buttons[i]).GetPath();
			((Control)Buttons[i]).FocusNeighborTop = ((i > 0) ? ((Node)Buttons[i - 1]).GetPath() : ((Node)Buttons[i]).GetPath());
			((Control)Buttons[i]).FocusNeighborBottom = ((i < Buttons.Length - 1) ? ((Node)Buttons[i + 1]).GetPath() : ((Node)Buttons[i]).GetPath());
		}
	}

	private void RefreshLabels()
	{
		_pausedLabel.SetTextAutoSize(_pausedLoc.GetFormattedText());
		((Node)_resumeButton).GetNode<MegaLabel>(NodePath.op_Implicit("Label")).SetTextAutoSize(_resumeLoc.GetFormattedText());
		((Node)_settingsButton).GetNode<MegaLabel>(NodePath.op_Implicit("Label")).SetTextAutoSize(_settingsLoc.GetFormattedText());
		((Node)_compendiumButton).GetNode<MegaLabel>(NodePath.op_Implicit("Label")).SetTextAutoSize(_compendiumLoc.GetFormattedText());
		((Node)_giveUpButton).GetNode<MegaLabel>(NodePath.op_Implicit("Label")).SetTextAutoSize(_giveUpLoc.GetFormattedText());
		((Node)_disconnectButton).GetNode<MegaLabel>(NodePath.op_Implicit("Label")).SetTextAutoSize(_disconnectLoc.GetFormattedText());
		((Node)_saveAndQuitButton).GetNode<MegaLabel>(NodePath.op_Implicit("Label")).SetTextAutoSize(_saveAndQuitLoc.GetFormattedText());
	}

	public void Initialize(IRunState runState)
	{
		_runState = runState;
		if (!RunManager.Instance.IsInProgress || _runState.IsGameOver)
		{
			_giveUpButton.Disable();
		}
		else
		{
			_giveUpButton.Enable();
		}
		((CanvasItem)_compendiumButton).Visible = !NGame.IsReleaseGame() || SaveManager.Instance.IsCompendiumAvailable();
	}

	private void OnBackOrResumeButtonPressed(NButton _)
	{
		SfxCmd.Play("event:/sfx/ui/map/map_close");
		NCapstoneContainer.Instance.Close();
		NRun.Instance.GlobalUi.TopBar.Pause.ToggleAnimState();
	}

	private void OnSettingsButtonPressed(NButton _)
	{
		_stack.PushSubmenuType<NSettingsScreen>();
	}

	private void OnCompendiumButtonPressed(NButton _)
	{
		NCompendiumSubmenu submenuType = _stack.GetSubmenuType<NCompendiumSubmenu>();
		submenuType.Initialize(_runState);
		_stack.Push(submenuType);
	}

	private void OnGiveUpButtonPressed(NButton _)
	{
		NModalContainer.Instance.Add((Node)(object)NAbandonRunConfirmPopup.Create(null));
	}

	private void OnDisconnectButtonPressed(NButton _)
	{
		if (RunManager.Instance.NetService.IsConnected)
		{
			NModalContainer.Instance.Add((Node)(object)NDisconnectConfirmPopup.Create());
		}
		else
		{
			TaskHelper.RunSafely(NGame.Instance.ReturnToMainMenuAfterRun());
		}
	}

	private void OnSaveAndQuitButtonPressed(NButton _)
	{
		TaskHelper.RunSafely(CloseToMenu());
	}

	private async Task CloseToMenu()
	{
		_resumeButton.Disable();
		_settingsButton.Disable();
		_compendiumButton.Disable();
		_giveUpButton.Disable();
		_disconnectButton.Disable();
		_saveAndQuitButton.Disable();
		_backButton.Disable();
		RunManager.Instance.ActionQueueSet.Reset();
		NRunMusicController.Instance.StopMusic();
		await NGame.Instance.ReturnToMainMenu();
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		NHotkeyManager.Instance.AddBlockingScreen((Node)(object)this);
	}

	public override void OnSubmenuClosed()
	{
		_backButton.Disable();
		((CanvasItem)this).Visible = false;
		NHotkeyManager.Instance.RemoveBlockingScreen((Node)(object)this);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
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
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Expected O, but got Unknown
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshLabels, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnBackOrResumeButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSettingsButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCompendiumButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnGiveUpButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisconnectButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSaveAndQuitButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshLabels && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshLabels();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnBackOrResumeButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnBackOrResumeButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSettingsButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnSettingsButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCompendiumButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCompendiumButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnGiveUpButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnGiveUpButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDisconnectButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnDisconnectButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSaveAndQuitButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnSaveAndQuitButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuClosed();
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
		if ((ref method) == MethodName.RefreshLabels)
		{
			return true;
		}
		if ((ref method) == MethodName.OnBackOrResumeButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSettingsButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCompendiumButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnGiveUpButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDisconnectButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSaveAndQuitButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._buttonContainer)
		{
			_buttonContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._resumeButton)
		{
			_resumeButton = VariantUtils.ConvertTo<NPauseMenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._settingsButton)
		{
			_settingsButton = VariantUtils.ConvertTo<NPauseMenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._compendiumButton)
		{
			_compendiumButton = VariantUtils.ConvertTo<NPauseMenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._giveUpButton)
		{
			_giveUpButton = VariantUtils.ConvertTo<NPauseMenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._disconnectButton)
		{
			_disconnectButton = VariantUtils.ConvertTo<NPauseMenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._saveAndQuitButton)
		{
			_saveAndQuitButton = VariantUtils.ConvertTo<NPauseMenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pausedLabel)
		{
			_pausedLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName.Buttons)
		{
			GodotObject[] buttons = (GodotObject[])(object)Buttons;
			value = VariantUtils.CreateFromSystemArrayOfGodotObject(buttons);
			return true;
		}
		if ((ref name) == PropertyName.UseSharedBackstop)
		{
			bool useSharedBackstop = UseSharedBackstop;
			value = VariantUtils.CreateFrom<bool>(ref useSharedBackstop);
			return true;
		}
		if ((ref name) == PropertyName.ScreenType)
		{
			NetScreenType screenType = ScreenType;
			value = VariantUtils.CreateFrom<NetScreenType>(ref screenType);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _backButton);
			return true;
		}
		if ((ref name) == PropertyName._buttonContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _buttonContainer);
			return true;
		}
		if ((ref name) == PropertyName._resumeButton)
		{
			value = VariantUtils.CreateFrom<NPauseMenuButton>(ref _resumeButton);
			return true;
		}
		if ((ref name) == PropertyName._settingsButton)
		{
			value = VariantUtils.CreateFrom<NPauseMenuButton>(ref _settingsButton);
			return true;
		}
		if ((ref name) == PropertyName._compendiumButton)
		{
			value = VariantUtils.CreateFrom<NPauseMenuButton>(ref _compendiumButton);
			return true;
		}
		if ((ref name) == PropertyName._giveUpButton)
		{
			value = VariantUtils.CreateFrom<NPauseMenuButton>(ref _giveUpButton);
			return true;
		}
		if ((ref name) == PropertyName._disconnectButton)
		{
			value = VariantUtils.CreateFrom<NPauseMenuButton>(ref _disconnectButton);
			return true;
		}
		if ((ref name) == PropertyName._saveAndQuitButton)
		{
			value = VariantUtils.CreateFrom<NPauseMenuButton>(ref _saveAndQuitButton);
			return true;
		}
		if ((ref name) == PropertyName._pausedLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _pausedLabel);
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
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._buttonContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._resumeButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._settingsButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._compendiumButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._giveUpButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._disconnectButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._saveAndQuitButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._pausedLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)28, PropertyName.Buttons, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.UseSharedBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.ScreenType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._buttonContainer, Variant.From<Control>(ref _buttonContainer));
		info.AddProperty(PropertyName._resumeButton, Variant.From<NPauseMenuButton>(ref _resumeButton));
		info.AddProperty(PropertyName._settingsButton, Variant.From<NPauseMenuButton>(ref _settingsButton));
		info.AddProperty(PropertyName._compendiumButton, Variant.From<NPauseMenuButton>(ref _compendiumButton));
		info.AddProperty(PropertyName._giveUpButton, Variant.From<NPauseMenuButton>(ref _giveUpButton));
		info.AddProperty(PropertyName._disconnectButton, Variant.From<NPauseMenuButton>(ref _disconnectButton));
		info.AddProperty(PropertyName._saveAndQuitButton, Variant.From<NPauseMenuButton>(ref _saveAndQuitButton));
		info.AddProperty(PropertyName._pausedLabel, Variant.From<MegaLabel>(ref _pausedLabel));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val))
		{
			_backButton = ((Variant)(ref val)).As<NBackButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._buttonContainer, ref val2))
		{
			_buttonContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._resumeButton, ref val3))
		{
			_resumeButton = ((Variant)(ref val3)).As<NPauseMenuButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._settingsButton, ref val4))
		{
			_settingsButton = ((Variant)(ref val4)).As<NPauseMenuButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._compendiumButton, ref val5))
		{
			_compendiumButton = ((Variant)(ref val5)).As<NPauseMenuButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._giveUpButton, ref val6))
		{
			_giveUpButton = ((Variant)(ref val6)).As<NPauseMenuButton>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._disconnectButton, ref val7))
		{
			_disconnectButton = ((Variant)(ref val7)).As<NPauseMenuButton>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._saveAndQuitButton, ref val8))
		{
			_saveAndQuitButton = ((Variant)(ref val8)).As<NPauseMenuButton>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._pausedLabel, ref val9))
		{
			_pausedLabel = ((Variant)(ref val9)).As<MegaLabel>();
		}
	}
}
