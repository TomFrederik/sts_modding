using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NRemoteLobbyPlayerContainer.cs")]
public class NRemoteLobbyPlayerContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshSoloLabelVisibility = StringName.op_Implicit("RefreshSoloLabelVisibility");

		public static readonly StringName Cleanup = StringName.op_Implicit("Cleanup");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _inviteButton = StringName.op_Implicit("_inviteButton");

		public static readonly StringName _soloLabel = StringName.op_Implicit("_soloLabel");

		public static readonly StringName _container = StringName.op_Implicit("_container");

		public static readonly StringName _displayLocalPlayer = StringName.op_Implicit("_displayLocalPlayer");
	}

	public class SignalName : SignalName
	{
	}

	private readonly List<NRemoteLobbyPlayer> _nodes = new List<NRemoteLobbyPlayer>();

	private StartRunLobby? _lobby;

	private NInvitePlayersButton _inviteButton;

	private MegaLabel _soloLabel;

	private Container _container;

	private bool _displayLocalPlayer;

	public override void _Ready()
	{
		_soloLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%SoloLabel"));
		_container = ((Node)this).GetNode<Container>(NodePath.op_Implicit("Container"));
		_inviteButton = ((Node)this).GetNode<NInvitePlayersButton>(NodePath.op_Implicit("%InviteButton"));
		_soloLabel.SetTextAutoSize(new LocString("main_menu_ui", "MULTIPLAYER_CHAR_SELECT.SOLO").GetFormattedText());
	}

	public void Initialize(StartRunLobby lobby, bool displayLocalPlayer)
	{
		foreach (NRemoteLobbyPlayer node in _nodes)
		{
			((Node)(object)node).QueueFreeSafely();
		}
		_nodes.Clear();
		if (!lobby.NetService.Type.IsMultiplayer())
		{
			return;
		}
		_displayLocalPlayer = displayLocalPlayer;
		_inviteButton.Initialize(lobby);
		_lobby = lobby;
		foreach (LobbyPlayer player in _lobby.Players)
		{
			OnPlayerConnected(player);
		}
		RefreshSoloLabelVisibility();
	}

	public void OnPlayerConnected(LobbyPlayer player)
	{
		StartRunLobby lobby = _lobby;
		if (lobby != null && (player.id != lobby.LocalPlayer.id || _displayLocalPlayer))
		{
			NRemoteLobbyPlayer nRemoteLobbyPlayer = NRemoteLobbyPlayer.Create(player, lobby.NetService.Platform, lobby.NetService.Type == NetGameType.Singleplayer);
			((Node)(object)_container).AddChildSafely((Node?)(object)nRemoteLobbyPlayer);
			((Node)_container).MoveChild(((Node)_inviteButton).GetParent(), ((Node)_container).GetChildCount(false) - 1);
			_nodes.Add(nRemoteLobbyPlayer);
			RefreshSoloLabelVisibility();
		}
	}

	public void OnPlayerDisconnected(LobbyPlayer player)
	{
		if (_lobby == null)
		{
			return;
		}
		int num = _nodes.FindIndex((NRemoteLobbyPlayer p) => p.PlayerId == player.id);
		if (num >= 0)
		{
			((Node)(object)_container).RemoveChildSafely((Node?)(object)_nodes[num]);
			_nodes.RemoveAt(num);
			foreach (NRemoteLobbyPlayer node in _nodes)
			{
				node.CancelShake();
			}
		}
		RefreshSoloLabelVisibility();
	}

	public void OnPlayerChanged(LobbyPlayer player)
	{
		StartRunLobby lobby = _lobby;
		if (lobby != null && (player.id != lobby.LocalPlayer.id || _displayLocalPlayer))
		{
			_nodes.FirstOrDefault((NRemoteLobbyPlayer p) => p.PlayerId == player.id)?.OnPlayerChanged(player);
		}
	}

	private void RefreshSoloLabelVisibility()
	{
		StartRunLobby lobby = _lobby;
		((CanvasItem)_soloLabel).Visible = (lobby == null || lobby.NetService.Type != NetGameType.Singleplayer) && lobby != null && lobby.Players.Count == 1;
	}

	public void Cleanup()
	{
		foreach (NRemoteLobbyPlayer node in _nodes)
		{
			((Node)(object)node).QueueFreeSafely();
		}
		_nodes.Clear();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshSoloLabelVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Cleanup, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshSoloLabelVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshSoloLabelVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Cleanup && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Cleanup();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshSoloLabelVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName.Cleanup)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._inviteButton)
		{
			_inviteButton = VariantUtils.ConvertTo<NInvitePlayersButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._soloLabel)
		{
			_soloLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._container)
		{
			_container = VariantUtils.ConvertTo<Container>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._displayLocalPlayer)
		{
			_displayLocalPlayer = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._inviteButton)
		{
			value = VariantUtils.CreateFrom<NInvitePlayersButton>(ref _inviteButton);
			return true;
		}
		if ((ref name) == PropertyName._soloLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _soloLabel);
			return true;
		}
		if ((ref name) == PropertyName._container)
		{
			value = VariantUtils.CreateFrom<Container>(ref _container);
			return true;
		}
		if ((ref name) == PropertyName._displayLocalPlayer)
		{
			value = VariantUtils.CreateFrom<bool>(ref _displayLocalPlayer);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._inviteButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._soloLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._container, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._displayLocalPlayer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._inviteButton, Variant.From<NInvitePlayersButton>(ref _inviteButton));
		info.AddProperty(PropertyName._soloLabel, Variant.From<MegaLabel>(ref _soloLabel));
		info.AddProperty(PropertyName._container, Variant.From<Container>(ref _container));
		info.AddProperty(PropertyName._displayLocalPlayer, Variant.From<bool>(ref _displayLocalPlayer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._inviteButton, ref val))
		{
			_inviteButton = ((Variant)(ref val)).As<NInvitePlayersButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._soloLabel, ref val2))
		{
			_soloLabel = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._container, ref val3))
		{
			_container = ((Variant)(ref val3)).As<Container>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._displayLocalPlayer, ref val4))
		{
			_displayLocalPlayer = ((Variant)(ref val4)).As<bool>();
		}
	}
}
