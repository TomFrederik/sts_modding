using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Daily;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;
using MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Platform.Steam;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NMultiplayerSubmenu.cs")]
public class NMultiplayerSubmenu : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName UpdateButtons = StringName.op_Implicit("UpdateButtons");

		public static readonly StringName AbandonRun = StringName.op_Implicit("AbandonRun");

		public static readonly StringName StartLoad = StringName.op_Implicit("StartLoad");

		public static readonly StringName OnHostPressed = StringName.op_Implicit("OnHostPressed");

		public static readonly StringName FastHost = StringName.op_Implicit("FastHost");

		public static readonly StringName OpenJoinFriendsScreen = StringName.op_Implicit("OpenJoinFriendsScreen");

		public static readonly StringName OnJoinFriendsPressed = StringName.op_Implicit("OnJoinFriendsPressed");

		public new static readonly StringName OnSubmenuShown = StringName.op_Implicit("OnSubmenuShown");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _hostButton = StringName.op_Implicit("_hostButton");

		public static readonly StringName _loadButton = StringName.op_Implicit("_loadButton");

		public static readonly StringName _abandonButton = StringName.op_Implicit("_abandonButton");

		public static readonly StringName _joinButton = StringName.op_Implicit("_joinButton");

		public static readonly StringName _loadingOverlay = StringName.op_Implicit("_loadingOverlay");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/multiplayer_submenu");

	private NSubmenuButton _hostButton;

	private NSubmenuButton _loadButton;

	private NSubmenuButton _abandonButton;

	private NSubmenuButton _joinButton;

	private const string _keyHost = "HOST";

	private const string _keyLoad = "MP_LOAD";

	private const string _keyJoin = "JOIN";

	private const string _keyAbandon = "MP_ABANDON";

	private Control _loadingOverlay;

	protected override Control InitialFocusedControl
	{
		get
		{
			if (SaveManager.Instance.HasMultiplayerRunSave)
			{
				return (Control)(object)_loadButton;
			}
			return (Control)(object)_hostButton;
		}
	}

	public static NMultiplayerSubmenu? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NMultiplayerSubmenu>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_loadingOverlay = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%LoadingOverlay"));
		_hostButton = ((Node)this).GetNode<NSubmenuButton>(NodePath.op_Implicit("ButtonContainer/HostButton"));
		((GodotObject)_hostButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnHostPressed), 0u);
		_hostButton.SetIconAndLocalization("HOST");
		_loadButton = ((Node)this).GetNode<NSubmenuButton>(NodePath.op_Implicit("ButtonContainer/LoadButton"));
		((GodotObject)_loadButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)StartLoad), 0u);
		_loadButton.SetIconAndLocalization("MP_LOAD");
		_joinButton = ((Node)this).GetNode<NSubmenuButton>(NodePath.op_Implicit("ButtonContainer/JoinButton"));
		((GodotObject)_joinButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenJoinFriendsScreen), 0u);
		_joinButton.SetIconAndLocalization("JOIN");
		_abandonButton = ((Node)this).GetNode<NSubmenuButton>(NodePath.op_Implicit("ButtonContainer/AbandonButton"));
		((GodotObject)_abandonButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)AbandonRun), 0u);
		_abandonButton.SetIconAndLocalization("MP_ABANDON");
		UpdateButtons();
	}

	private void UpdateButtons()
	{
		((CanvasItem)_hostButton).Visible = !SaveManager.Instance.HasMultiplayerRunSave;
		((CanvasItem)_loadButton).Visible = SaveManager.Instance.HasMultiplayerRunSave;
		((CanvasItem)_abandonButton).Visible = SaveManager.Instance.HasMultiplayerRunSave;
	}

	private void AbandonRun(NButton _)
	{
		TaskHelper.RunSafely(TryAbandonMultiplayerRun());
	}

	private async Task TryAbandonMultiplayerRun()
	{
		LocString header = new LocString("main_menu_ui", "ABANDON_RUN_CONFIRMATION.header");
		LocString body = new LocString("main_menu_ui", "ABANDON_RUN_CONFIRMATION.body");
		LocString yesButton = new LocString("main_menu_ui", "GENERIC_POPUP.confirm");
		LocString noButton = new LocString("main_menu_ui", "GENERIC_POPUP.cancel");
		NGenericPopup nGenericPopup = NGenericPopup.Create();
		NModalContainer.Instance.Add((Node)(object)nGenericPopup);
		if (!(await nGenericPopup.WaitForConfirmation(body, header, noButton, yesButton)))
		{
			return;
		}
		ReadSaveResult<SerializableRun> readSaveResult = SaveManager.Instance.LoadAndCanonicalizeMultiplayerRunSave(PlatformUtil.GetLocalPlayerId(PlatformUtil.PrimaryPlatform));
		if (readSaveResult.Success && readSaveResult.SaveData != null)
		{
			try
			{
				SerializableRun saveData = readSaveResult.SaveData;
				SaveManager.Instance.UpdateProgressWithRunData(saveData, victory: false);
				RunHistoryUtilities.CreateRunHistoryEntry(saveData, victory: false, isAbandoned: true, saveData.PlatformType);
				if (saveData.DailyTime.HasValue)
				{
					PlatformUtil.GetLocalPlayerId(saveData.PlatformType);
					int score = ScoreUtility.CalculateScore(saveData, won: false);
					TaskHelper.RunSafely(DailyRunUtility.UploadScore(saveData.DailyTime.Value, score, saveData.Players));
				}
			}
			catch (Exception value)
			{
				Log.Error($"ERROR: Failed to upload run history/metrics: {value}");
			}
		}
		else
		{
			Log.Error($"ERROR: Failed to load multiplayer run save: status={readSaveResult.Status}. Deleting current run...");
		}
		SaveManager.Instance.DeleteCurrentMultiplayerRun();
		UpdateButtons();
	}

	private void StartLoad(NButton _)
	{
		PlatformType platformType = ((SteamInitializer.Initialized && !CommandLineHelper.HasArg("fastmp")) ? PlatformType.Steam : PlatformType.None);
		ReadSaveResult<SerializableRun> readSaveResult = SaveManager.Instance.LoadAndCanonicalizeMultiplayerRunSave(PlatformUtil.GetLocalPlayerId(platformType));
		if (!readSaveResult.Success || readSaveResult.SaveData == null)
		{
			Log.Warn("Broken multiplayer run save detected, disabling button");
			_loadButton.Disable();
			NErrorPopup modalToCreate = NErrorPopup.Create(new LocString("main_menu_ui", "INVALID_SAVE_POPUP.title"), new LocString("main_menu_ui", "INVALID_SAVE_POPUP.description_run"), new LocString("main_menu_ui", "INVALID_SAVE_POPUP.dismiss"), showReportBugButton: true);
			NModalContainer.Instance.Add((Node)(object)modalToCreate);
			NModalContainer.Instance.ShowBackstop();
		}
		else
		{
			StartHost(readSaveResult.SaveData);
		}
	}

	private void OnHostPressed(NButton _)
	{
		if (SaveManager.Instance.Progress.NumberOfRuns > 0)
		{
			_stack.PushSubmenuType<NMultiplayerHostSubmenu>();
		}
		else
		{
			TaskHelper.RunSafely(NMultiplayerHostSubmenu.StartHostAsync(GameMode.Standard, _loadingOverlay, _stack));
		}
	}

	public void FastHost(GameMode gameMode)
	{
		NMultiplayerHostSubmenu nMultiplayerHostSubmenu = _stack.PushSubmenuType<NMultiplayerHostSubmenu>();
		nMultiplayerHostSubmenu.StartHost(gameMode);
	}

	public void StartHost(SerializableRun run)
	{
		TaskHelper.RunSafely(StartHostAsync(run));
	}

	private async Task StartHostAsync(SerializableRun run)
	{
		PlatformType platformType = ((SteamInitializer.Initialized && !CommandLineHelper.HasArg("fastmp")) ? PlatformType.Steam : PlatformType.None);
		((CanvasItem)_loadingOverlay).Visible = true;
		try
		{
			NetHostGameService netService = new NetHostGameService();
			NetErrorInfo? netErrorInfo = null;
			if (platformType == PlatformType.Steam)
			{
				netErrorInfo = await netService.StartSteamHost(4);
			}
			else
			{
				netService.StartENetHost(33771, 4);
			}
			if (!netErrorInfo.HasValue)
			{
				if (run.Modifiers.Count > 0)
				{
					if (run.DailyTime.HasValue)
					{
						NDailyRunLoadScreen submenuType = _stack.GetSubmenuType<NDailyRunLoadScreen>();
						submenuType.InitializeAsHost(netService, run);
						_stack.Push(submenuType);
					}
					else
					{
						NCustomRunLoadScreen submenuType2 = _stack.GetSubmenuType<NCustomRunLoadScreen>();
						submenuType2.InitializeAsHost(netService, run);
						_stack.Push(submenuType2);
					}
				}
				else
				{
					NMultiplayerLoadGameScreen submenuType3 = _stack.GetSubmenuType<NMultiplayerLoadGameScreen>();
					submenuType3.InitializeAsHost(netService, run);
					_stack.Push(submenuType3);
				}
			}
			else
			{
				NErrorPopup nErrorPopup = NErrorPopup.Create(netErrorInfo.Value);
				if (nErrorPopup != null)
				{
					NModalContainer.Instance.Add((Node)(object)nErrorPopup);
				}
			}
		}
		finally
		{
			((CanvasItem)_loadingOverlay).Visible = false;
		}
	}

	private void OpenJoinFriendsScreen(NButton _)
	{
		OnJoinFriendsPressed();
	}

	public NJoinFriendScreen OnJoinFriendsPressed()
	{
		return _stack.PushSubmenuType<NJoinFriendScreen>();
	}

	protected override void OnSubmenuShown()
	{
		base.OnSubmenuShown();
		if (!SaveManager.Instance.SeenFtue("multiplayer_warning") && SaveManager.Instance.Progress.NumberOfRuns == 0 && !CommandLineHelper.HasArg("fastmp"))
		{
			NMultiplayerWarningPopup modalToCreate = NMultiplayerWarningPopup.Create();
			NModalContainer.Instance.Add((Node)(object)modalToCreate);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Expected O, but got Unknown
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Expected O, but got Unknown
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateButtons, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AbandonRun, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartLoad, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHostPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FastHost, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("gameMode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenJoinFriendsScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnJoinFriendsPressed, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NMultiplayerSubmenu nMultiplayerSubmenu = Create();
			ret = VariantUtils.CreateFrom<NMultiplayerSubmenu>(ref nMultiplayerSubmenu);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateButtons && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateButtons();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AbandonRun && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AbandonRun(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartLoad && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			StartLoad(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHostPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnHostPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FastHost && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			FastHost(VariantUtils.ConvertTo<GameMode>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenJoinFriendsScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenJoinFriendsScreen(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnJoinFriendsPressed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NJoinFriendScreen nJoinFriendScreen = OnJoinFriendsPressed();
			ret = VariantUtils.CreateFrom<NJoinFriendScreen>(ref nJoinFriendScreen);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuShown();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NMultiplayerSubmenu nMultiplayerSubmenu = Create();
			ret = VariantUtils.CreateFrom<NMultiplayerSubmenu>(ref nMultiplayerSubmenu);
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
		if ((ref method) == MethodName.UpdateButtons)
		{
			return true;
		}
		if ((ref method) == MethodName.AbandonRun)
		{
			return true;
		}
		if ((ref method) == MethodName.StartLoad)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHostPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.FastHost)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenJoinFriendsScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.OnJoinFriendsPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._hostButton)
		{
			_hostButton = VariantUtils.ConvertTo<NSubmenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._loadButton)
		{
			_loadButton = VariantUtils.ConvertTo<NSubmenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._abandonButton)
		{
			_abandonButton = VariantUtils.ConvertTo<NSubmenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._joinButton)
		{
			_joinButton = VariantUtils.ConvertTo<NSubmenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._loadingOverlay)
		{
			_loadingOverlay = VariantUtils.ConvertTo<Control>(ref value);
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
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._hostButton)
		{
			value = VariantUtils.CreateFrom<NSubmenuButton>(ref _hostButton);
			return true;
		}
		if ((ref name) == PropertyName._loadButton)
		{
			value = VariantUtils.CreateFrom<NSubmenuButton>(ref _loadButton);
			return true;
		}
		if ((ref name) == PropertyName._abandonButton)
		{
			value = VariantUtils.CreateFrom<NSubmenuButton>(ref _abandonButton);
			return true;
		}
		if ((ref name) == PropertyName._joinButton)
		{
			value = VariantUtils.CreateFrom<NSubmenuButton>(ref _joinButton);
			return true;
		}
		if ((ref name) == PropertyName._loadingOverlay)
		{
			value = VariantUtils.CreateFrom<Control>(ref _loadingOverlay);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._hostButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._loadButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._abandonButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._joinButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._loadingOverlay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._hostButton, Variant.From<NSubmenuButton>(ref _hostButton));
		info.AddProperty(PropertyName._loadButton, Variant.From<NSubmenuButton>(ref _loadButton));
		info.AddProperty(PropertyName._abandonButton, Variant.From<NSubmenuButton>(ref _abandonButton));
		info.AddProperty(PropertyName._joinButton, Variant.From<NSubmenuButton>(ref _joinButton));
		info.AddProperty(PropertyName._loadingOverlay, Variant.From<Control>(ref _loadingOverlay));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._hostButton, ref val))
		{
			_hostButton = ((Variant)(ref val)).As<NSubmenuButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._loadButton, ref val2))
		{
			_loadButton = ((Variant)(ref val2)).As<NSubmenuButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._abandonButton, ref val3))
		{
			_abandonButton = ((Variant)(ref val3)).As<NSubmenuButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._joinButton, ref val4))
		{
			_joinButton = ((Variant)(ref val4)).As<NSubmenuButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._loadingOverlay, ref val5))
		{
			_loadingOverlay = ((Variant)(ref val5)).As<Control>();
		}
	}
}
