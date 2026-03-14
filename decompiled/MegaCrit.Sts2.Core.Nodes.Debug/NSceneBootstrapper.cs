using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Nodes.Debug;

[ScriptPath("res://src/Core/Nodes/Debug/NSceneBootstrapper.cs")]
public class NSceneBootstrapper : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _openConsole = StringName.op_Implicit("_openConsole");

		public static readonly StringName _game = StringName.op_Implicit("_game");
	}

	public class SignalName : SignalName
	{
	}

	private bool _openConsole;

	private NGame _game;

	public override void _Ready()
	{
		if (((Node)this).GetParent() is NGame game)
		{
			_game = game;
		}
		else
		{
			_game = SceneHelper.Instantiate<NGame>("game");
			_game.StartOnMainMenu = false;
			((Node)(object)this).AddChildSafely((Node?)(object)_game);
		}
		TaskHelper.RunSafely(StartNewRun());
	}

	private async Task StartNewRun()
	{
		Type type = BootstrapSettingsUtil.Get();
		if (type == null)
		{
			Log.Error("No type implementing IBootstrapSettings found in the project! To use the bootstrap scene, copy src/Core/Nodes/Debug/BootstrapSettings.cs.template and rename it to BootstrapSettings.cs");
			return;
		}
		IBootstrapSettings settings = (IBootstrapSettings)Activator.CreateInstance(type);
		if (settings.Language != null)
		{
			LocManager.Instance.SetLanguage(settings.Language);
		}
		PreloadManager.Enabled = settings.DoPreloading;
		string seed = settings.Seed ?? SeedHelper.GetRandomSeed();
		List<ActModel> list = ActModel.GetDefaultList().ToList();
		list[0] = settings.Act;
		RunState runState = RunState.CreateForNewRun(new _003C_003Ez__ReadOnlySingleElementList<Player>(Player.CreateForNewRun(settings.Character, SaveManager.Instance.GenerateUnlockStateFromProgress(), 1uL)), list.Select((ActModel a) => a.ToMutable()).ToList(), settings.Modifiers, settings.Ascension, seed);
		RunManager.Instance.SetUpNewSinglePlayer(runState, settings.SaveRunHistory);
		await PreloadManager.LoadRunAssets(new _003C_003Ez__ReadOnlySingleElementList<CharacterModel>(settings.Character));
		RunManager.Instance.Launch();
		_game.RootSceneContainer.SetCurrentScene((Control)(object)NRun.Create(runState));
		await RunManager.Instance.SetActInternal(0);
		RunManager.Instance.RunLocationTargetedBuffer.OnRunLocationChanged(runState.CurrentLocation);
		RunManager.Instance.MapSelectionSynchronizer.OnRunLocationChanged(runState.CurrentLocation);
		await settings.Setup(runState.Players[0]);
		switch (settings.RoomType)
		{
		case RoomType.Unassigned:
			await RunManager.Instance.EnterAct(0);
			break;
		case RoomType.Treasure:
		case RoomType.Shop:
		case RoomType.RestSite:
			await RunManager.Instance.EnterRoomDebug(settings.RoomType);
			RunManager.Instance.ActionExecutor.Unpause();
			break;
		case RoomType.Event:
		{
			AbstractRoom abstractRoom = await RunManager.Instance.EnterRoomDebug(settings.RoomType, MapPointType.Unassigned, settings.Event);
			if (abstractRoom != null && abstractRoom.IsVictoryRoom)
			{
				runState.CurrentActIndex = runState.Acts.Count - 1;
			}
			break;
		}
		default:
			await RunManager.Instance.EnterRoomDebug(settings.RoomType, MapPointType.Unassigned, settings.RoomType.IsCombatRoom() ? settings.Encounter.ToMutable() : null);
			break;
		}
		if (_openConsole)
		{
			NDevConsole node = ((Node)this).GetNode<NDevConsole>(NodePath.op_Implicit("/root/DevConsole/ConsoleScreen"));
			node.ShowConsole();
			node.MakeFullScreen();
			node.SetBackgroundColor(Colors.White);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._openConsole)
		{
			_openConsole = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._game)
		{
			_game = VariantUtils.ConvertTo<NGame>(ref value);
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
		if ((ref name) == PropertyName._openConsole)
		{
			value = VariantUtils.CreateFrom<bool>(ref _openConsole);
			return true;
		}
		if ((ref name) == PropertyName._game)
		{
			value = VariantUtils.CreateFrom<NGame>(ref _game);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._openConsole, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._game, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._openConsole, Variant.From<bool>(ref _openConsole));
		info.AddProperty(PropertyName._game, Variant.From<NGame>(ref _game));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._openConsole, ref val))
		{
			_openConsole = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._game, ref val2))
		{
			_game = ((Variant)(ref val2)).As<NGame>();
		}
	}
}
