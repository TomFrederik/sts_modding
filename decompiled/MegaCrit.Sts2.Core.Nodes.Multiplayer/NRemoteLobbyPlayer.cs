using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NRemoteLobbyPlayer.cs")]
public class NRemoteLobbyPlayer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshVisuals = StringName.op_Implicit("RefreshVisuals");

		public static readonly StringName CancelShake = StringName.op_Implicit("CancelShake");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName PlayerId = StringName.op_Implicit("PlayerId");

		public static readonly StringName _characterIcon = StringName.op_Implicit("_characterIcon");

		public static readonly StringName _readyIndicator = StringName.op_Implicit("_readyIndicator");

		public static readonly StringName _disconnectedIndicator = StringName.op_Implicit("_disconnectedIndicator");

		public static readonly StringName _nameplateLabel = StringName.op_Implicit("_nameplateLabel");

		public static readonly StringName _characterLabel = StringName.op_Implicit("_characterLabel");

		public static readonly StringName _platform = StringName.op_Implicit("_platform");

		public static readonly StringName _isSingleplayer = StringName.op_Implicit("_isSingleplayer");

		public static readonly StringName _playerId = StringName.op_Implicit("_playerId");

		public static readonly StringName _isReady = StringName.op_Implicit("_isReady");

		public static readonly StringName _isConnected = StringName.op_Implicit("_isConnected");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("ui/remote_lobby_player");

	private TextureRect _characterIcon;

	private Control _readyIndicator;

	private Control _disconnectedIndicator;

	private MegaLabel _nameplateLabel;

	private MegaLabel _characterLabel;

	private PlatformType _platform;

	private bool _isSingleplayer;

	private ScreenPunchInstance? _shake;

	private Vector2? _originalPosition;

	private ulong _playerId;

	private CharacterModel _character;

	private bool _isReady;

	private bool _isConnected;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public ulong PlayerId => _playerId;

	public static NRemoteLobbyPlayer Create(LobbyPlayer player, PlatformType platform, bool isSingleplayer)
	{
		NRemoteLobbyPlayer nRemoteLobbyPlayer = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NRemoteLobbyPlayer>((GenEditState)0);
		nRemoteLobbyPlayer._playerId = player.id;
		nRemoteLobbyPlayer._platform = platform;
		nRemoteLobbyPlayer._isSingleplayer = isSingleplayer;
		nRemoteLobbyPlayer._character = player.character;
		nRemoteLobbyPlayer._isReady = player.isReady;
		nRemoteLobbyPlayer._isConnected = true;
		return nRemoteLobbyPlayer;
	}

	public static NRemoteLobbyPlayer Create(LoadRunLobby runLobby, ulong playerId, PlatformType platform, bool isSingleplayer)
	{
		NRemoteLobbyPlayer nRemoteLobbyPlayer = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NRemoteLobbyPlayer>((GenEditState)0);
		nRemoteLobbyPlayer._playerId = playerId;
		nRemoteLobbyPlayer._isSingleplayer = isSingleplayer;
		nRemoteLobbyPlayer._platform = runLobby.NetService.Platform;
		nRemoteLobbyPlayer._character = ModelDb.GetById<CharacterModel>(runLobby.Run.Players.First((SerializablePlayer p) => p.NetId == playerId).CharacterId);
		nRemoteLobbyPlayer._isReady = runLobby.IsPlayerReady(playerId);
		nRemoteLobbyPlayer._isConnected = runLobby.ConnectedPlayerIds.Contains(playerId);
		return nRemoteLobbyPlayer;
	}

	public override void _Ready()
	{
		_nameplateLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%NameplateLabel"));
		_characterLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%CharacterLabel"));
		_characterIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%CharacterIcon"));
		_readyIndicator = (Control)(object)((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%ReadyIndicator"));
		_disconnectedIndicator = (Control)(object)((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%DisconnectedIndicator"));
		if (!_isSingleplayer)
		{
			_nameplateLabel.SetTextAutoSize(PlatformUtil.GetPlayerName(_platform, _playerId));
		}
		else
		{
			_characterLabel.SetTextAutoSize(string.Empty);
		}
		RefreshVisuals();
	}

	public void OnPlayerChanged(LobbyPlayer lobbyPlayer)
	{
		_playerId = lobbyPlayer.id;
		SetCharacter(lobbyPlayer.character);
		_isReady = lobbyPlayer.isReady;
		_isConnected = true;
		RefreshVisuals();
	}

	public void OnPlayerChanged(LoadRunLobby runLobby, ulong playerId)
	{
		SerializablePlayer serializablePlayer = runLobby.Run.Players.First((SerializablePlayer p) => p.NetId == playerId);
		SetCharacter(ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId));
		_isReady = runLobby.IsPlayerReady(playerId);
		_isConnected = runLobby.ConnectedPlayerIds.Contains(playerId);
		RefreshVisuals();
	}

	private void RefreshVisuals()
	{
		if (_isSingleplayer)
		{
			_nameplateLabel.SetTextAutoSize(_character.Title.GetFormattedText());
		}
		else
		{
			_characterLabel.SetTextAutoSize(_character.Title.GetFormattedText());
		}
		_characterIcon.Texture = _character.IconTexture;
		((CanvasItem)_readyIndicator).Visible = _isReady;
		((CanvasItem)_disconnectedIndicator).Visible = !_isConnected;
	}

	private void SetCharacter(CharacterModel character)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (_character != character)
		{
			_shake?.Cancel();
			CancelShake();
			_originalPosition = ((Control)this).Position;
			_shake = new ScreenPunchInstance(3f, 0.4000000059604645, 90f);
			_character = character;
		}
	}

	public void CancelShake()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		_shake = null;
		if (_originalPosition.HasValue)
		{
			((Control)this).Position = _originalPosition.Value;
			_originalPosition = null;
		}
	}

	public override void _Process(double delta)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		ScreenPunchInstance shake = _shake;
		if (shake != null && !shake.IsDone)
		{
			Vector2 val = _shake?.Update(delta) ?? Vector2.Zero;
			((Control)this).Position = _originalPosition.Value + val;
		}
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
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelShake, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshVisuals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CancelShake && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CancelShake();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.RefreshVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.CancelShake)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._characterIcon)
		{
			_characterIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._readyIndicator)
		{
			_readyIndicator = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._disconnectedIndicator)
		{
			_disconnectedIndicator = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nameplateLabel)
		{
			_nameplateLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterLabel)
		{
			_characterLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._platform)
		{
			_platform = VariantUtils.ConvertTo<PlatformType>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isSingleplayer)
		{
			_isSingleplayer = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playerId)
		{
			_playerId = VariantUtils.ConvertTo<ulong>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isReady)
		{
			_isReady = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isConnected)
		{
			_isConnected = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
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
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.PlayerId)
		{
			ulong playerId = PlayerId;
			value = VariantUtils.CreateFrom<ulong>(ref playerId);
			return true;
		}
		if ((ref name) == PropertyName._characterIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _characterIcon);
			return true;
		}
		if ((ref name) == PropertyName._readyIndicator)
		{
			value = VariantUtils.CreateFrom<Control>(ref _readyIndicator);
			return true;
		}
		if ((ref name) == PropertyName._disconnectedIndicator)
		{
			value = VariantUtils.CreateFrom<Control>(ref _disconnectedIndicator);
			return true;
		}
		if ((ref name) == PropertyName._nameplateLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _nameplateLabel);
			return true;
		}
		if ((ref name) == PropertyName._characterLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _characterLabel);
			return true;
		}
		if ((ref name) == PropertyName._platform)
		{
			value = VariantUtils.CreateFrom<PlatformType>(ref _platform);
			return true;
		}
		if ((ref name) == PropertyName._isSingleplayer)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isSingleplayer);
			return true;
		}
		if ((ref name) == PropertyName._playerId)
		{
			value = VariantUtils.CreateFrom<ulong>(ref _playerId);
			return true;
		}
		if ((ref name) == PropertyName._isReady)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isReady);
			return true;
		}
		if ((ref name) == PropertyName._isConnected)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isConnected);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._characterIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._readyIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._disconnectedIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._nameplateLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._platform, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isSingleplayer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._playerId, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isReady, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isConnected, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.PlayerId, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._characterIcon, Variant.From<TextureRect>(ref _characterIcon));
		info.AddProperty(PropertyName._readyIndicator, Variant.From<Control>(ref _readyIndicator));
		info.AddProperty(PropertyName._disconnectedIndicator, Variant.From<Control>(ref _disconnectedIndicator));
		info.AddProperty(PropertyName._nameplateLabel, Variant.From<MegaLabel>(ref _nameplateLabel));
		info.AddProperty(PropertyName._characterLabel, Variant.From<MegaLabel>(ref _characterLabel));
		info.AddProperty(PropertyName._platform, Variant.From<PlatformType>(ref _platform));
		info.AddProperty(PropertyName._isSingleplayer, Variant.From<bool>(ref _isSingleplayer));
		info.AddProperty(PropertyName._playerId, Variant.From<ulong>(ref _playerId));
		info.AddProperty(PropertyName._isReady, Variant.From<bool>(ref _isReady));
		info.AddProperty(PropertyName._isConnected, Variant.From<bool>(ref _isConnected));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._characterIcon, ref val))
		{
			_characterIcon = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._readyIndicator, ref val2))
		{
			_readyIndicator = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._disconnectedIndicator, ref val3))
		{
			_disconnectedIndicator = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._nameplateLabel, ref val4))
		{
			_nameplateLabel = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterLabel, ref val5))
		{
			_characterLabel = ((Variant)(ref val5)).As<MegaLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._platform, ref val6))
		{
			_platform = ((Variant)(ref val6)).As<PlatformType>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isSingleplayer, ref val7))
		{
			_isSingleplayer = ((Variant)(ref val7)).As<bool>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._playerId, ref val8))
		{
			_playerId = ((Variant)(ref val8)).As<ulong>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._isReady, ref val9))
		{
			_isReady = ((Variant)(ref val9)).As<bool>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._isConnected, ref val10))
		{
			_isConnected = ((Variant)(ref val10)).As<bool>();
		}
	}
}
