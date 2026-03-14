using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NRemoteLoadLobbyPlayerContainer.cs")]
public class NRemoteLoadLobbyPlayerContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnPlayerConnected = StringName.op_Implicit("OnPlayerConnected");

		public static readonly StringName OnPlayerDisconnected = StringName.op_Implicit("OnPlayerDisconnected");

		public static readonly StringName OnPlayerChanged = StringName.op_Implicit("OnPlayerChanged");

		public static readonly StringName Cleanup = StringName.op_Implicit("Cleanup");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _othersLabel = StringName.op_Implicit("_othersLabel");

		public static readonly StringName _container = StringName.op_Implicit("_container");
	}

	public class SignalName : SignalName
	{
	}

	private readonly List<NRemoteLobbyPlayer> _nodes = new List<NRemoteLobbyPlayer>();

	private LoadRunLobby? _lobby;

	private MegaLabel? _othersLabel;

	private Control _container;

	public override void _Ready()
	{
		_othersLabel = ((Node)this).GetNodeOrNull<MegaLabel>(NodePath.op_Implicit("OthersLabel"));
		_container = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Container"));
	}

	public void Initialize(LoadRunLobby runLobby, bool displayLocalPlayer)
	{
		_lobby = runLobby;
		if (_othersLabel != null)
		{
			LocString locString = new LocString("main_menu_ui", "MULTIPLAYER_LOAD_MENU.OTHERS");
			locString.Add("others", runLobby.Run.Players.Count - 1);
			((Label)_othersLabel).Text = locString.GetFormattedText();
		}
		foreach (SerializablePlayer player in runLobby.Run.Players)
		{
			if (player.NetId != _lobby.NetService.NetId || displayLocalPlayer)
			{
				NRemoteLobbyPlayer nRemoteLobbyPlayer = NRemoteLobbyPlayer.Create(runLobby, player.NetId, runLobby.NetService.Platform, runLobby.NetService.Type == NetGameType.Singleplayer);
				((Node)(object)_container).AddChildSafely((Node?)(object)nRemoteLobbyPlayer);
				_nodes.Add(nRemoteLobbyPlayer);
			}
		}
	}

	public void OnPlayerConnected(ulong playerId)
	{
		OnPlayerChanged(playerId);
	}

	public void OnPlayerDisconnected(ulong playerId)
	{
		OnPlayerChanged(playerId);
	}

	public void OnPlayerChanged(ulong playerId)
	{
		if (_lobby != null && playerId != _lobby.NetService.NetId)
		{
			int num = _nodes.FindIndex((NRemoteLobbyPlayer p) => p.PlayerId == playerId);
			if (num >= 0)
			{
				NRemoteLobbyPlayer nRemoteLobbyPlayer = _nodes[num];
				nRemoteLobbyPlayer.OnPlayerChanged(_lobby, playerId);
			}
		}
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
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlayerConnected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlayerDisconnected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlayerChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Cleanup, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlayerConnected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPlayerConnected(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlayerDisconnected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPlayerDisconnected(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlayerChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPlayerChanged(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.OnPlayerConnected)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPlayerDisconnected)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPlayerChanged)
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
		if ((ref name) == PropertyName._othersLabel)
		{
			_othersLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._container)
		{
			_container = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName._othersLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _othersLabel);
			return true;
		}
		if ((ref name) == PropertyName._container)
		{
			value = VariantUtils.CreateFrom<Control>(ref _container);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._othersLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._container, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._othersLabel, Variant.From<MegaLabel>(ref _othersLabel));
		info.AddProperty(PropertyName._container, Variant.From<Control>(ref _container));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._othersLabel, ref val))
		{
			_othersLabel = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._container, ref val2))
		{
			_container = ((Variant)(ref val2)).As<Control>();
		}
	}
}
