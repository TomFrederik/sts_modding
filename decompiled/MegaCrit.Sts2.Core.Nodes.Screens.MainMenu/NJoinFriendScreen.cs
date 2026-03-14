using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Connection;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;
using MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Platform.Steam;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NJoinFriendScreen.cs")]
public class NJoinFriendScreen : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public static readonly StringName RefreshButtonClicked = StringName.op_Implicit("RefreshButtonClicked");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName DebugFriendsButtons = StringName.op_Implicit("DebugFriendsButtons");

		public static readonly StringName _buttonContainer = StringName.op_Implicit("_buttonContainer");

		public static readonly StringName _loadingOverlay = StringName.op_Implicit("_loadingOverlay");

		public static readonly StringName _loadingFriendsIndicator = StringName.op_Implicit("_loadingFriendsIndicator");

		public static readonly StringName _noFriendsLabel = StringName.op_Implicit("_noFriendsLabel");

		public static readonly StringName _refreshButton = StringName.op_Implicit("_refreshButton");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/join_friend_submenu");

	private Control _buttonContainer;

	private Control _loadingOverlay;

	private Control _loadingFriendsIndicator;

	private MegaLabel _noFriendsLabel;

	private NJoinFriendRefreshButton _refreshButton;

	private Task? _refreshTask;

	private JoinFlow? _currentJoinFlow;

	protected override Control? InitialFocusedControl
	{
		get
		{
			if (((Node)_buttonContainer).GetChildCount(false) <= 0)
			{
				return (Control?)(object)_refreshButton;
			}
			return ((Node)_buttonContainer).GetChild<Control>(0, false);
		}
	}

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2]
	{
		_scenePath,
		NJoinFriendButton.scenePath
	});

	public bool DebugFriendsButtons => false;

	public static NJoinFriendScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NJoinFriendScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_buttonContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ButtonContainer"));
		_loadingOverlay = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%LoadingOverlay"));
		_loadingFriendsIndicator = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%LoadingIndicator"));
		_noFriendsLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%NoFriendsText"));
		_refreshButton = ((Node)this).GetNode<NJoinFriendRefreshButton>(NodePath.op_Implicit("%RefreshButton"));
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("TitleLabel")).SetTextAutoSize(new LocString("main_menu_ui", "JOIN_FRIENDS_MENU.title").GetFormattedText());
		_noFriendsLabel.SetTextAutoSize(new LocString("main_menu_ui", "JOIN_FRIENDS_MENU.noFriends").GetFormattedText());
		((GodotObject)_refreshButton).Connect(NClickableControl.SignalName.Released, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
		{
			RefreshButtonClicked();
		}), 0u);
		((CanvasItem)_loadingFriendsIndicator).Visible = false;
		((CanvasItem)_noFriendsLabel).Visible = false;
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		((CanvasItem)_loadingOverlay).Visible = false;
		if ((!SteamInitializer.Initialized || CommandLineHelper.HasArg("fastmp")) && !DebugFriendsButtons)
		{
			TaskHelper.RunSafely(FastMpJoin());
		}
		else
		{
			_refreshTask = TaskHelper.RunSafely(ShowFriends());
		}
	}

	public override void OnSubmenuClosed()
	{
		_currentJoinFlow?.CancelToken.Cancel();
	}

	private async Task FastMpJoin()
	{
		ulong netId = 1000uL;
		if (CommandLineHelper.TryGetValue("clientId", out string value))
		{
			netId = ulong.Parse(value);
		}
		DisplayServer.WindowSetTitle("Slay The Spire 2 (Client)", 0);
		await JoinGameAsync(new ENetClientConnectionInitializer(netId, "127.0.0.1", 33771));
	}

	private void RefreshButtonClicked()
	{
		Task refreshTask = _refreshTask;
		if (refreshTask == null || refreshTask.IsCompleted)
		{
			_refreshTask = TaskHelper.RunSafely(RefreshButtonClickedAsync());
		}
	}

	private async Task RefreshButtonClickedAsync()
	{
		((CanvasItem)_noFriendsLabel).Visible = false;
		if (SteamInitializer.Initialized)
		{
			((CanvasItem)_loadingFriendsIndicator).Visible = true;
			await Cmd.Wait(0.5f);
			((CanvasItem)_loadingFriendsIndicator).Visible = false;
		}
		await ShowFriends();
		InitialFocusedControl?.TryGrabFocus();
	}

	private async Task ShowFriends()
	{
		((CanvasItem)_loadingFriendsIndicator).Visible = true;
		foreach (Node child2 in ((Node)_buttonContainer).GetChildren(false))
		{
			child2.QueueFreeSafely();
		}
		if (SteamInitializer.Initialized)
		{
			IEnumerable<ulong> enumerable = await PlatformUtil.GetFriendsWithOpenLobbies(PlatformType.Steam);
			NButton nButton = null;
			foreach (ulong item in enumerable)
			{
				NJoinFriendButton nJoinFriendButton = NJoinFriendButton.Create(item);
				((Node)(object)_buttonContainer).AddChildSafely((Node?)(object)nJoinFriendButton);
				SteamClientConnectionInitializer connInitializer = SteamClientConnectionInitializer.FromPlayer(item);
				((GodotObject)nJoinFriendButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
				{
					JoinGame(connInitializer);
				}), 0u);
				if (nButton == null)
				{
					nButton = nJoinFriendButton;
				}
			}
		}
		if (DebugFriendsButtons)
		{
			for (int num = 0; num < Rng.Chaotic.NextInt(5, 20); num++)
			{
				ulong playerId = (ulong)(int)((num == 0) ? 1u : ((uint)(num * 1000)));
				NJoinFriendButton child = NJoinFriendButton.Create(playerId);
				((Node)(object)_buttonContainer).AddChildSafely((Node?)(object)child);
			}
		}
		ActiveScreenContext.Instance.Update();
		((CanvasItem)_loadingFriendsIndicator).Visible = false;
		((CanvasItem)_noFriendsLabel).Visible = ((Node)_buttonContainer).GetChildCount(false) == 0;
	}

	private void JoinGame(IClientConnectionInitializer connInitializer)
	{
		TaskHelper.RunSafely(JoinGameAsync(connInitializer));
	}

	public async Task JoinGameAsync(IClientConnectionInitializer connInitializer)
	{
		if (_currentJoinFlow?.NetService?.IsConnected == true)
		{
			Log.Warn($"Tried to join game with connection {connInitializer} while we were already joining a game! Ignoring this attempt");
			return;
		}
		((CanvasItem)_loadingOverlay).Visible = true;
		_currentJoinFlow = new JoinFlow();
		try
		{
			Log.Info($"Attempting to join game with connection initializer {connInitializer}");
			JoinResult joinResult = await _currentJoinFlow.Begin(connInitializer, ((Node)this).GetTree());
			if (joinResult.sessionState == RunSessionState.InLobby)
			{
				if (joinResult.gameMode == GameMode.Standard)
				{
					NCharacterSelectScreen submenuType = _stack.GetSubmenuType<NCharacterSelectScreen>();
					submenuType.InitializeMultiplayerAsClient(_currentJoinFlow.NetService, joinResult.joinResponse.Value);
					_stack.Push(submenuType);
					return;
				}
				if (joinResult.gameMode == GameMode.Daily)
				{
					NDailyRunScreen submenuType2 = _stack.GetSubmenuType<NDailyRunScreen>();
					submenuType2.InitializeMultiplayerAsClient(_currentJoinFlow.NetService, joinResult.joinResponse.Value);
					_stack.Push(submenuType2);
					return;
				}
				if (joinResult.gameMode != GameMode.Custom)
				{
					throw new ArgumentOutOfRangeException("gameMode", joinResult.gameMode, "Invalid game mode!");
				}
				NCustomRunScreen submenuType3 = _stack.GetSubmenuType<NCustomRunScreen>();
				submenuType3.InitializeMultiplayerAsClient(_currentJoinFlow.NetService, joinResult.joinResponse.Value);
				_stack.Push(submenuType3);
			}
			else if (joinResult.sessionState == RunSessionState.InLoadedLobby)
			{
				if (joinResult.gameMode == GameMode.Standard)
				{
					NMultiplayerLoadGameScreen submenuType4 = _stack.GetSubmenuType<NMultiplayerLoadGameScreen>();
					submenuType4.InitializeAsClient(_currentJoinFlow.NetService, joinResult.loadJoinResponse.Value);
					_stack.Push(submenuType4);
				}
				else if (joinResult.gameMode == GameMode.Daily)
				{
					NDailyRunLoadScreen submenuType5 = _stack.GetSubmenuType<NDailyRunLoadScreen>();
					submenuType5.InitializeAsClient(_currentJoinFlow.NetService, joinResult.loadJoinResponse.Value);
					_stack.Push(submenuType5);
				}
				else if (joinResult.gameMode == GameMode.Custom)
				{
					NCustomRunLoadScreen submenuType6 = _stack.GetSubmenuType<NCustomRunLoadScreen>();
					submenuType6.InitializeAsClient(_currentJoinFlow.NetService, joinResult.loadJoinResponse.Value);
					_stack.Push(submenuType6);
				}
			}
			else if (joinResult.sessionState == RunSessionState.Running)
			{
				throw new NotImplementedException("Rejoining a game is not yet implemented");
			}
		}
		catch (ClientConnectionFailedException ex)
		{
			Log.Error($"Received connection failed exception while joining game: {ex}");
			NErrorPopup nErrorPopup = NErrorPopup.Create(ex.info);
			if (nErrorPopup != null)
			{
				NModalContainer.Instance.Add((Node)(object)nErrorPopup);
			}
			_currentJoinFlow.NetService?.Disconnect(ex.info.GetReason());
		}
		catch (OperationCanceledException)
		{
			Log.Warn("Joining was canceled by user");
		}
		catch
		{
			Log.Error("Received unexpected exception while joining game! Disconnecting with InternalError");
			NErrorPopup nErrorPopup2 = NErrorPopup.Create(new NetErrorInfo(NetError.InternalError, selfInitiated: false));
			if (nErrorPopup2 != null)
			{
				NModalContainer.Instance.Add((Node)(object)nErrorPopup2);
			}
			_currentJoinFlow.NetService?.Disconnect(NetError.InternalError);
			throw;
		}
		finally
		{
			((CanvasItem)_loadingOverlay).Visible = false;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshButtonClicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NJoinFriendScreen nJoinFriendScreen = Create();
			ret = VariantUtils.CreateFrom<NJoinFriendScreen>(ref nJoinFriendScreen);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		if ((ref method) == MethodName.RefreshButtonClicked && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshButtonClicked();
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
			NJoinFriendScreen nJoinFriendScreen = Create();
			ret = VariantUtils.CreateFrom<NJoinFriendScreen>(ref nJoinFriendScreen);
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
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshButtonClicked)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._buttonContainer)
		{
			_buttonContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._loadingOverlay)
		{
			_loadingOverlay = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._loadingFriendsIndicator)
		{
			_loadingFriendsIndicator = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noFriendsLabel)
		{
			_noFriendsLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._refreshButton)
		{
			_refreshButton = VariantUtils.ConvertTo<NJoinFriendRefreshButton>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName.DebugFriendsButtons)
		{
			bool debugFriendsButtons = DebugFriendsButtons;
			value = VariantUtils.CreateFrom<bool>(ref debugFriendsButtons);
			return true;
		}
		if ((ref name) == PropertyName._buttonContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _buttonContainer);
			return true;
		}
		if ((ref name) == PropertyName._loadingOverlay)
		{
			value = VariantUtils.CreateFrom<Control>(ref _loadingOverlay);
			return true;
		}
		if ((ref name) == PropertyName._loadingFriendsIndicator)
		{
			value = VariantUtils.CreateFrom<Control>(ref _loadingFriendsIndicator);
			return true;
		}
		if ((ref name) == PropertyName._noFriendsLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _noFriendsLabel);
			return true;
		}
		if ((ref name) == PropertyName._refreshButton)
		{
			value = VariantUtils.CreateFrom<NJoinFriendRefreshButton>(ref _refreshButton);
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
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._buttonContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._loadingOverlay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._loadingFriendsIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._noFriendsLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._refreshButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.DebugFriendsButtons, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._buttonContainer, Variant.From<Control>(ref _buttonContainer));
		info.AddProperty(PropertyName._loadingOverlay, Variant.From<Control>(ref _loadingOverlay));
		info.AddProperty(PropertyName._loadingFriendsIndicator, Variant.From<Control>(ref _loadingFriendsIndicator));
		info.AddProperty(PropertyName._noFriendsLabel, Variant.From<MegaLabel>(ref _noFriendsLabel));
		info.AddProperty(PropertyName._refreshButton, Variant.From<NJoinFriendRefreshButton>(ref _refreshButton));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._buttonContainer, ref val))
		{
			_buttonContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._loadingOverlay, ref val2))
		{
			_loadingOverlay = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._loadingFriendsIndicator, ref val3))
		{
			_loadingFriendsIndicator = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._noFriendsLabel, ref val4))
		{
			_noFriendsLabel = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._refreshButton, ref val5))
		{
			_refreshButton = ((Variant)(ref val5)).As<NJoinFriendRefreshButton>();
		}
	}
}
