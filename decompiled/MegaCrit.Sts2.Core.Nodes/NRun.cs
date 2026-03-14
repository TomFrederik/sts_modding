using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes;

[ScriptPath("res://src/Core/Nodes/NRun.cs")]
public class NRun : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName _Notification = StringName.op_Implicit("_Notification");

		public static readonly StringName SetCurrentRoom = StringName.op_Implicit("SetCurrentRoom");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName CombatRoom = StringName.op_Implicit("CombatRoom");

		public static readonly StringName TreasureRoom = StringName.op_Implicit("TreasureRoom");

		public static readonly StringName EventRoom = StringName.op_Implicit("EventRoom");

		public static readonly StringName RestSiteRoom = StringName.op_Implicit("RestSiteRoom");

		public static readonly StringName MapRoom = StringName.op_Implicit("MapRoom");

		public static readonly StringName MerchantRoom = StringName.op_Implicit("MerchantRoom");

		public static readonly StringName GlobalUi = StringName.op_Implicit("GlobalUi");

		public static readonly StringName RunMusicController = StringName.op_Implicit("RunMusicController");

		public static readonly StringName _cardScene = StringName.op_Implicit("_cardScene");

		public static readonly StringName _roomContainer = StringName.op_Implicit("_roomContainer");

		public static readonly StringName _testButton = StringName.op_Implicit("_testButton");

		public static readonly StringName _seedLabel = StringName.op_Implicit("_seedLabel");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/run.tscn";

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _cardScene;

	private RunState _state;

	private NSceneContainer _roomContainer;

	private Button _testButton;

	private MegaLabel _seedLabel;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/run.tscn");

	public static NRun? Instance => NGame.Instance?.CurrentRunNode;

	public NCombatRoom? CombatRoom
	{
		get
		{
			Control currentScene = _roomContainer.CurrentScene;
			if (!(currentScene is NCombatRoom result))
			{
				if (currentScene is NEventRoom nEventRoom)
				{
					return nEventRoom.EmbeddedCombatRoom;
				}
				return null;
			}
			return result;
		}
	}

	public NTreasureRoom? TreasureRoom => _roomContainer.CurrentScene as NTreasureRoom;

	public NEventRoom? EventRoom => _roomContainer.CurrentScene as NEventRoom;

	public NRestSiteRoom? RestSiteRoom => _roomContainer.CurrentScene as NRestSiteRoom;

	public NMapRoom? MapRoom => _roomContainer.CurrentScene as NMapRoom;

	public NMerchantRoom? MerchantRoom => _roomContainer.CurrentScene as NMerchantRoom;

	public NGlobalUi GlobalUi { get; private set; }

	public NRunMusicController RunMusicController { get; private set; }

	public ScreenStateTracker ScreenStateTracker { get; private set; }

	public static NRun Create(RunState state)
	{
		NRun nRun = PreloadManager.Cache.GetScene("res://scenes/run.tscn").Instantiate<NRun>((GenEditState)0);
		nRun._state = state;
		return nRun;
	}

	public override void _Ready()
	{
		_roomContainer = ((Node)this).GetNode<NSceneContainer>(NodePath.op_Implicit("%RoomContainer"));
		GlobalUi = ((Node)this).GetNode<NGlobalUi>(NodePath.op_Implicit("%GlobalUi"));
		GlobalUi.Initialize(_state);
		ScreenStateTracker = new ScreenStateTracker(GlobalUi.MapScreen, GlobalUi.CapstoneContainer, GlobalUi.Overlays);
		RunMusicController = ((Node)this).GetNode<NRunMusicController>(NodePath.op_Implicit("%RunMusicController"));
		RunMusicController.SetRunState(_state);
		RunMusicController.UpdateMusic();
		_seedLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%DebugSeed"));
		_seedLabel.SetTextAutoSize(_state.Rng.StringSeed);
	}

	public override void _Process(double delta)
	{
		RunManager.Instance.NetService.Update();
	}

	public override void _Notification(int what)
	{
		if ((long)what == 1006)
		{
			RunManager.Instance.CleanUp(graceful: false);
		}
	}

	public void SetCurrentRoom(Control? node)
	{
		if (node != null)
		{
			_roomContainer.SetCurrentScene(node);
			ActiveScreenContext.Instance.Update();
		}
	}

	public void ShowGameOverScreen(SerializableRun serializableRun)
	{
		NCapstoneContainer.Instance.Close();
		NMapScreen.Instance.Close();
		NOverlayStack.Instance.Push(NGameOverScreen.Create(_state, serializableRun));
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
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Notification, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("what"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCurrentRoom, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Notification && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((GodotObject)this)._Notification(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCurrentRoom && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCurrentRoom(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName._Notification)
		{
			return true;
		}
		if ((ref method) == MethodName.SetCurrentRoom)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.GlobalUi)
		{
			GlobalUi = VariantUtils.ConvertTo<NGlobalUi>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.RunMusicController)
		{
			RunMusicController = VariantUtils.ConvertTo<NRunMusicController>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardScene)
		{
			_cardScene = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._roomContainer)
		{
			_roomContainer = VariantUtils.ConvertTo<NSceneContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._testButton)
		{
			_testButton = VariantUtils.ConvertTo<Button>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._seedLabel)
		{
			_seedLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CombatRoom)
		{
			NCombatRoom combatRoom = CombatRoom;
			value = VariantUtils.CreateFrom<NCombatRoom>(ref combatRoom);
			return true;
		}
		if ((ref name) == PropertyName.TreasureRoom)
		{
			NTreasureRoom treasureRoom = TreasureRoom;
			value = VariantUtils.CreateFrom<NTreasureRoom>(ref treasureRoom);
			return true;
		}
		if ((ref name) == PropertyName.EventRoom)
		{
			NEventRoom eventRoom = EventRoom;
			value = VariantUtils.CreateFrom<NEventRoom>(ref eventRoom);
			return true;
		}
		if ((ref name) == PropertyName.RestSiteRoom)
		{
			NRestSiteRoom restSiteRoom = RestSiteRoom;
			value = VariantUtils.CreateFrom<NRestSiteRoom>(ref restSiteRoom);
			return true;
		}
		if ((ref name) == PropertyName.MapRoom)
		{
			NMapRoom mapRoom = MapRoom;
			value = VariantUtils.CreateFrom<NMapRoom>(ref mapRoom);
			return true;
		}
		if ((ref name) == PropertyName.MerchantRoom)
		{
			NMerchantRoom merchantRoom = MerchantRoom;
			value = VariantUtils.CreateFrom<NMerchantRoom>(ref merchantRoom);
			return true;
		}
		if ((ref name) == PropertyName.GlobalUi)
		{
			NGlobalUi globalUi = GlobalUi;
			value = VariantUtils.CreateFrom<NGlobalUi>(ref globalUi);
			return true;
		}
		if ((ref name) == PropertyName.RunMusicController)
		{
			NRunMusicController runMusicController = RunMusicController;
			value = VariantUtils.CreateFrom<NRunMusicController>(ref runMusicController);
			return true;
		}
		if ((ref name) == PropertyName._cardScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _cardScene);
			return true;
		}
		if ((ref name) == PropertyName._roomContainer)
		{
			value = VariantUtils.CreateFrom<NSceneContainer>(ref _roomContainer);
			return true;
		}
		if ((ref name) == PropertyName._testButton)
		{
			value = VariantUtils.CreateFrom<Button>(ref _testButton);
			return true;
		}
		if ((ref name) == PropertyName._seedLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _seedLabel);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._cardScene, (PropertyHint)17, "PackedScene", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._roomContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._testButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._seedLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CombatRoom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.TreasureRoom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.EventRoom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.RestSiteRoom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.MapRoom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.MerchantRoom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.GlobalUi, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.RunMusicController, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName globalUi = PropertyName.GlobalUi;
		NGlobalUi globalUi2 = GlobalUi;
		info.AddProperty(globalUi, Variant.From<NGlobalUi>(ref globalUi2));
		StringName runMusicController = PropertyName.RunMusicController;
		NRunMusicController runMusicController2 = RunMusicController;
		info.AddProperty(runMusicController, Variant.From<NRunMusicController>(ref runMusicController2));
		info.AddProperty(PropertyName._cardScene, Variant.From<PackedScene>(ref _cardScene));
		info.AddProperty(PropertyName._roomContainer, Variant.From<NSceneContainer>(ref _roomContainer));
		info.AddProperty(PropertyName._testButton, Variant.From<Button>(ref _testButton));
		info.AddProperty(PropertyName._seedLabel, Variant.From<MegaLabel>(ref _seedLabel));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.GlobalUi, ref val))
		{
			GlobalUi = ((Variant)(ref val)).As<NGlobalUi>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.RunMusicController, ref val2))
		{
			RunMusicController = ((Variant)(ref val2)).As<NRunMusicController>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardScene, ref val3))
		{
			_cardScene = ((Variant)(ref val3)).As<PackedScene>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._roomContainer, ref val4))
		{
			_roomContainer = ((Variant)(ref val4)).As<NSceneContainer>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._testButton, ref val5))
		{
			_testButton = ((Variant)(ref val5)).As<Button>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._seedLabel, ref val6))
		{
			_seedLabel = ((Variant)(ref val6)).As<MegaLabel>();
		}
	}
}
