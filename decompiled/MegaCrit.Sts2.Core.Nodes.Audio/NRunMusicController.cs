using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Audio;

[ScriptPath("res://src/Core/Nodes/Audio/NRunMusicController.cs")]
public class NRunMusicController : Node
{
	private enum MusicProgressTrack
	{
		Init,
		Enemy,
		Merchant,
		Rest,
		Unknown,
		Treasure,
		Elite,
		CombatEnd,
		Elite2,
		MerchantEnd
	}

	private enum CampfireState
	{
		On,
		Off
	}

	public class MethodName : MethodName
	{
		public static readonly StringName GetTrack = StringName.op_Implicit("GetTrack");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName UpdateMusic = StringName.op_Implicit("UpdateMusic");

		public static readonly StringName PlayCustomMusic = StringName.op_Implicit("PlayCustomMusic");

		public static readonly StringName UpdateCustomTrack = StringName.op_Implicit("UpdateCustomTrack");

		public static readonly StringName StopCustomMusic = StringName.op_Implicit("StopCustomMusic");

		public static readonly StringName UpdateAmbience = StringName.op_Implicit("UpdateAmbience");

		public static readonly StringName UpdateTrack = StringName.op_Implicit("UpdateTrack");

		public static readonly StringName UpdateMusicParameter = StringName.op_Implicit("UpdateMusicParameter");

		public static readonly StringName ToggleMerchantTrack = StringName.op_Implicit("ToggleMerchantTrack");

		public static readonly StringName TriggerEliteSecondPhase = StringName.op_Implicit("TriggerEliteSecondPhase");

		public static readonly StringName TriggerCampfireGoingOut = StringName.op_Implicit("TriggerCampfireGoingOut");

		public static readonly StringName StopMusic = StringName.op_Implicit("StopMusic");

		public static readonly StringName LoadActBank = StringName.op_Implicit("LoadActBank");

		public static readonly StringName UnloadActBanks = StringName.op_Implicit("UnloadActBanks");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _proxy = StringName.op_Implicit("_proxy");

		public static readonly StringName _currentTrack = StringName.op_Implicit("_currentTrack");

		public static readonly StringName _currentAmbience = StringName.op_Implicit("_currentAmbience");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _stopAmbience = new StringName("stop_ambience");

	private static readonly StringName _stopMusic = new StringName("stop_music");

	private const string _musicProgressParameter = "Progress";

	private const string _updateGlobalParameterCallback = "update_global_parameter";

	private const string _updateMusicParameterCallback = "update_music_parameter";

	private const string _updateMusicCallback = "update_music";

	private const string _updateAmbienceCallback = "update_ambience";

	private const string _updateCampfireAmbienceCallback = "update_campfire_ambience";

	private const string _updateCustomTrack = "update_custom_track";

	private const string _loadActBanksCallback = "load_act_banks";

	private const string _unloadActBanksCallback = "unload_act_banks";

	private IRunState _runState = NullRunState.Instance;

	private Node _proxy;

	private string _currentTrack;

	private string _currentAmbience;

	public static NRunMusicController? Instance => NRun.Instance?.RunMusicController;

	private MusicProgressTrack GetTrack(RoomType roomType)
	{
		if (roomType.IsCombatRoom() && !CombatManager.Instance.IsInProgress)
		{
			return MusicProgressTrack.CombatEnd;
		}
		switch (roomType)
		{
		case RoomType.Shop:
			return MusicProgressTrack.Merchant;
		case RoomType.RestSite:
			return MusicProgressTrack.Rest;
		case RoomType.Treasure:
			return MusicProgressTrack.Treasure;
		case RoomType.Monster:
			return MusicProgressTrack.Enemy;
		case RoomType.Event:
			if (_runState.CurrentRoom is EventRoom eventRoom && eventRoom.CanonicalEvent is AncientEventModel)
			{
				return MusicProgressTrack.Init;
			}
			return MusicProgressTrack.Unknown;
		case RoomType.Elite:
			return MusicProgressTrack.Elite;
		case RoomType.Boss:
			return MusicProgressTrack.Elite;
		default:
			return MusicProgressTrack.Init;
		}
	}

	public override void _Ready()
	{
		_proxy = ((Node)this).GetNode<Node>(NodePath.op_Implicit("Proxy"));
	}

	public override void _ExitTree()
	{
		StopMusic();
	}

	public void SetRunState(IRunState runState)
	{
		_runState = runState;
	}

	public void UpdateMusic()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive)
		{
			string[] bgMusicOptions = _runState.Act.BgMusicOptions;
			string[] musicBankPaths = _runState.Act.MusicBankPaths;
			int num = new Rng(_runState.Rng.Seed).NextInt(0, bgMusicOptions.Length);
			LoadActBank(musicBankPaths[num]);
			_currentTrack = bgMusicOptions[num];
			((GodotObject)_proxy).Call(StringName.op_Implicit("update_music"), (Variant[])(object)new Variant[1] { Variant.op_Implicit(_currentTrack) });
			((GodotObject)_proxy).Call(StringName.op_Implicit("update_global_parameter"), (Variant[])(object)new Variant[2]
			{
				Variant.op_Implicit("Progress"),
				Variant.op_Implicit(0)
			});
			UpdateAmbience();
		}
	}

	public void PlayCustomMusic(string customMusic)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive)
		{
			((GodotObject)_proxy).Call(_stopMusic, Array.Empty<Variant>());
			((GodotObject)_proxy).Call(StringName.op_Implicit("update_music"), (Variant[])(object)new Variant[1] { Variant.op_Implicit(customMusic) });
		}
	}

	public void UpdateCustomTrack(string customTrack, float label)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive && RunManager.Instance.IsInProgress)
		{
			((GodotObject)_proxy).Call(StringName.op_Implicit("update_custom_track"), (Variant[])(object)new Variant[2]
			{
				Variant.op_Implicit(customTrack),
				Variant.op_Implicit(label)
			});
		}
	}

	public void StopCustomMusic()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive)
		{
			((GodotObject)_proxy).Call(_stopMusic, Array.Empty<Variant>());
			((GodotObject)_proxy).Call(StringName.op_Implicit("update_music"), (Variant[])(object)new Variant[1] { Variant.op_Implicit(_currentTrack) });
			((GodotObject)_proxy).Call(StringName.op_Implicit("update_global_parameter"), (Variant[])(object)new Variant[2]
			{
				Variant.op_Implicit("Progress"),
				Variant.op_Implicit(7)
			});
		}
	}

	public void UpdateAmbience()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive)
		{
			string ambientSfx = _runState.Act.AmbientSfx;
			EncounterModel encounterModel = (_runState.CurrentRoom as CombatRoom)?.Encounter;
			if (encounterModel != null && encounterModel.HasAmbientSfx)
			{
				ambientSfx = encounterModel.AmbientSfx;
			}
			if (_currentAmbience != ambientSfx)
			{
				_currentAmbience = ambientSfx;
				((GodotObject)_proxy).Call(StringName.op_Implicit("update_ambience"), (Variant[])(object)new Variant[1] { Variant.op_Implicit(_currentAmbience) });
			}
		}
	}

	public void UpdateTrack()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive)
		{
			MusicProgressTrack track = GetTrack(_runState.CurrentRoom.RoomType);
			UpdateTrack("Progress", (float)track);
			if (_runState.CurrentRoom is RestSiteRoom)
			{
				((GodotObject)_proxy).Call(StringName.op_Implicit("update_campfire_ambience"), (Variant[])(object)new Variant[1] { Variant.op_Implicit(0) });
			}
		}
	}

	private void UpdateTrack(string label, float trackIndex)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)_proxy).Call(StringName.op_Implicit("update_global_parameter"), (Variant[])(object)new Variant[2]
		{
			Variant.op_Implicit(label),
			Variant.op_Implicit(trackIndex)
		});
	}

	public void UpdateMusicParameter(string label, float trackIndex)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive)
		{
			((GodotObject)_proxy).Call(StringName.op_Implicit("update_music_parameter"), (Variant[])(object)new Variant[2]
			{
				Variant.op_Implicit(label),
				Variant.op_Implicit(trackIndex)
			});
		}
	}

	public void ToggleMerchantTrack()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive && _runState.CurrentRoom != null)
		{
			if (_runState.CurrentRoom.RoomType != RoomType.Shop)
			{
				throw new InvalidOperationException("You can only trigger the merchant transition in a merchant room");
			}
			NMapScreen? instance = NMapScreen.Instance;
			MusicProgressTrack musicProgressTrack = ((instance != null && ((CanvasItem)instance).IsVisible()) ? MusicProgressTrack.MerchantEnd : MusicProgressTrack.Merchant);
			((GodotObject)_proxy).Call(StringName.op_Implicit("update_global_parameter"), (Variant[])(object)new Variant[2]
			{
				Variant.op_Implicit("Progress"),
				Variant.op_Implicit((int)musicProgressTrack)
			});
		}
	}

	public void TriggerEliteSecondPhase()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive)
		{
			if (_runState.CurrentRoom.RoomType != RoomType.Elite)
			{
				throw new InvalidOperationException("You can only trigger the elite transition in an elite room");
			}
			((GodotObject)_proxy).Call(StringName.op_Implicit("update_global_parameter"), (Variant[])(object)new Variant[2]
			{
				Variant.op_Implicit("Progress"),
				Variant.op_Implicit(8)
			});
		}
	}

	public void TriggerCampfireGoingOut()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive)
		{
			if (_runState.CurrentRoom.RoomType != RoomType.RestSite)
			{
				throw new InvalidOperationException("You can only trigger the rest site transition in a rest site room");
			}
			((GodotObject)_proxy).Call(StringName.op_Implicit("update_campfire_ambience"), (Variant[])(object)new Variant[1] { Variant.op_Implicit(1) });
		}
	}

	public void StopMusic()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive)
		{
			((GodotObject)_proxy).Call(_stopMusic, Array.Empty<Variant>());
			((GodotObject)_proxy).Call(_stopAmbience, Array.Empty<Variant>());
			UnloadActBanks();
		}
	}

	private void LoadActBank(string bankPath)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Array val = new Array();
		val.Add(Variant.op_Implicit(bankPath));
		Array val2 = val;
		((GodotObject)_proxy).Call(StringName.op_Implicit("load_act_banks"), (Variant[])(object)new Variant[1] { Variant.op_Implicit(val2) });
	}

	private void UnloadActBanks()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)_proxy).Call(StringName.op_Implicit("unload_act_banks"), Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(17);
		list.Add(new MethodInfo(MethodName.GetTrack, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("roomType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateMusic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayCustomMusic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("customMusic"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateCustomTrack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("customTrack"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("label"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopCustomMusic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateAmbience, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateTrack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateTrack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("label"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("trackIndex"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateMusicParameter, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("label"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("trackIndex"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleMerchantTrack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TriggerEliteSecondPhase, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TriggerCampfireGoingOut, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopMusic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LoadActBank, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("bankPath"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UnloadActBanks, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.GetTrack && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			MusicProgressTrack track = GetTrack(VariantUtils.ConvertTo<RoomType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<MusicProgressTrack>(ref track);
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
		if ((ref method) == MethodName.UpdateMusic && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateMusic();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayCustomMusic && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PlayCustomMusic(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateCustomTrack && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			UpdateCustomTrack(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopCustomMusic && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopCustomMusic();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateAmbience && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateAmbience();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateTrack && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateTrack();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateTrack && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			UpdateTrack(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateMusicParameter && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			UpdateMusicParameter(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleMerchantTrack && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ToggleMerchantTrack();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TriggerEliteSecondPhase && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TriggerEliteSecondPhase();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TriggerCampfireGoingOut && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TriggerCampfireGoingOut();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopMusic && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopMusic();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LoadActBank && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			LoadActBank(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UnloadActBanks && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UnloadActBanks();
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.GetTrack)
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
		if ((ref method) == MethodName.UpdateMusic)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayCustomMusic)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateCustomTrack)
		{
			return true;
		}
		if ((ref method) == MethodName.StopCustomMusic)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateAmbience)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateTrack)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateMusicParameter)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleMerchantTrack)
		{
			return true;
		}
		if ((ref method) == MethodName.TriggerEliteSecondPhase)
		{
			return true;
		}
		if ((ref method) == MethodName.TriggerCampfireGoingOut)
		{
			return true;
		}
		if ((ref method) == MethodName.StopMusic)
		{
			return true;
		}
		if ((ref method) == MethodName.LoadActBank)
		{
			return true;
		}
		if ((ref method) == MethodName.UnloadActBanks)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._proxy)
		{
			_proxy = VariantUtils.ConvertTo<Node>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentTrack)
		{
			_currentTrack = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentAmbience)
		{
			_currentAmbience = VariantUtils.ConvertTo<string>(ref value);
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
		if ((ref name) == PropertyName._proxy)
		{
			value = VariantUtils.CreateFrom<Node>(ref _proxy);
			return true;
		}
		if ((ref name) == PropertyName._currentTrack)
		{
			value = VariantUtils.CreateFrom<string>(ref _currentTrack);
			return true;
		}
		if ((ref name) == PropertyName._currentAmbience)
		{
			value = VariantUtils.CreateFrom<string>(ref _currentAmbience);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._proxy, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._currentTrack, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._currentAmbience, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._proxy, Variant.From<Node>(ref _proxy));
		info.AddProperty(PropertyName._currentTrack, Variant.From<string>(ref _currentTrack));
		info.AddProperty(PropertyName._currentAmbience, Variant.From<string>(ref _currentAmbience));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._proxy, ref val))
		{
			_proxy = ((Variant)(ref val)).As<Node>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentTrack, ref val2))
		{
			_currentTrack = ((Variant)(ref val2)).As<string>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentAmbience, ref val3))
		{
			_currentAmbience = ((Variant)(ref val3)).As<string>();
		}
	}
}
