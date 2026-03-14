using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Rooms;

[ScriptPath("res://src/Core/Nodes/Rooms/NEventRoom.cs")]
public class NEventRoom : Control, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName SetPortrait = StringName.op_Implicit("SetPortrait");

		public static readonly StringName DisableOptionButtons = StringName.op_Implicit("DisableOptionButtons");

		public static readonly StringName OnEnteringEventCombat = StringName.op_Implicit("OnEnteringEventCombat");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Layout = StringName.op_Implicit("Layout");

		public static readonly StringName EmbeddedCombatRoom = StringName.op_Implicit("EmbeddedCombatRoom");

		public static readonly StringName VfxContainer = StringName.op_Implicit("VfxContainer");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _isPreFinished = StringName.op_Implicit("_isPreFinished");

		public static readonly StringName _eventContainer = StringName.op_Implicit("_eventContainer");
	}

	public class SignalName : SignalName
	{
	}

	private EventModel _event;

	private IRunState _runState = NullRunState.Instance;

	private bool _isPreFinished;

	private NSceneContainer _eventContainer;

	private const string _scenePath = "res://scenes/rooms/event_room.tscn";

	private readonly List<EventOption> _connectedOptions = new List<EventOption>();

	public static NEventRoom? Instance => NRun.Instance?.EventRoom;

	public NEventLayout? Layout => _eventContainer.CurrentScene as NEventLayout;

	public ICustomEventNode? CustomEventNode => _eventContainer.CurrentScene as ICustomEventNode;

	public NCombatRoom? EmbeddedCombatRoom => (Layout as NCombatEventLayout)?.EmbeddedCombatRoom;

	public Control? VfxContainer { get; private set; }

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/rooms/event_room.tscn");

	public Control? DefaultFocusedControl
	{
		get
		{
			IScreenContext customEventNode = CustomEventNode;
			if (customEventNode != null)
			{
				return customEventNode.DefaultFocusedControl;
			}
			return Layout?.DefaultFocusedControl;
		}
	}

	public static NEventRoom? Create(EventModel eventModel, IRunState? runState, bool isPreFinished)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NEventRoom nEventRoom = PreloadManager.Cache.GetScene("res://scenes/rooms/event_room.tscn").Instantiate<NEventRoom>((GenEditState)0);
		nEventRoom._event = eventModel;
		nEventRoom._isPreFinished = isPreFinished;
		if (runState != null)
		{
			nEventRoom._runState = runState;
		}
		return nEventRoom;
	}

	public override void _Ready()
	{
		if (_event.Node != null)
		{
			throw new InvalidOperationException("Tried to create event room, but event already has a node!");
		}
		_eventContainer = ((Node)this).GetNode<NSceneContainer>(NodePath.op_Implicit("%EventContainer"));
		NGame.Instance.SetScreenShakeTarget((Control)(object)_eventContainer);
		Control val = _event.CreateScene().Instantiate<Control>((GenEditState)0);
		_event.SetNode(val);
		_eventContainer.SetCurrentScene(val);
		VfxContainer = Layout?.VfxContainer;
		TaskHelper.RunSafely(SetupLayout());
	}

	public override void _ExitTree()
	{
		NGame.Instance.ClearScreenShakeTarget();
		_event.StateChanged -= RefreshEventState;
		_event.EnteringEventCombat -= OnEnteringEventCombat;
		foreach (EventOption connectedOption in _connectedOptions)
		{
			connectedOption.BeforeChosen -= BeforeOptionChosen;
		}
		_connectedOptions.Clear();
	}

	private async Task SetupLayout()
	{
		if (_event.Owner == null)
		{
			throw new InvalidOperationException("Event must be started before passed to NEventRoom!");
		}
		if (Layout == null)
		{
			return;
		}
		Layout.SetEvent(_event);
		SetTitle(_event.Title);
		_event.StateChanged += RefreshEventState;
		_event.EnteringEventCombat += OnEnteringEventCombat;
		await Cmd.Wait(0.2f);
		SetDescription(GetDescriptionOrFallback());
		if (_event is AncientEventModel ancientEventModel && !_isPreFinished)
		{
			ModelId id = _event.Owner.Character.Id;
			AncientStats statsForAncient = SaveManager.Instance.Progress.GetStatsForAncient(ancientEventModel.Id);
			int charVisits = statsForAncient?.GetVisitsAs(id) ?? 0;
			int totalVisits = statsForAncient?.TotalVisits ?? 0;
			IEnumerable<AncientDialogue> validDialogues = ancientEventModel.DialogueSet.GetValidDialogues(id, charVisits, totalVisits, !ancientEventModel.AnyCharacterDialogueBlacklist.Contains(_event.Owner.Character));
			AncientDialogue ancientDialogue = Rng.Chaotic.NextItem(validDialogues);
			foreach (AncientDialogueLine line in ancientDialogue.Lines)
			{
				line.LineText?.Add("Act1Name", _runState.Acts[0].Title);
			}
			((NAncientEventLayout)Layout).SetDialogue(ancientDialogue.Lines);
		}
		SetOptions(_event);
		Layout.OnSetupComplete();
	}

	public void SetPortrait(Texture2D portrait)
	{
		Layout.SetPortrait(portrait);
	}

	private void SetTitle(LocString title)
	{
		Layout.SetTitle(title.GetFormattedText());
	}

	private void SetDescription(LocString description)
	{
		if (description.Exists())
		{
			CharacterModel character = _event.Owner.Character;
			character.AddDetailsTo(description);
			description.Add("IsMultiplayer", _event.Owner.RunState.Players.Count > 1);
			_event.DynamicVars.AddTo(description);
			Layout.SetDescription(description.GetFormattedText());
		}
	}

	private void SetOptions(EventModel eventModel)
	{
		Layout.ClearOptions();
		IReadOnlyList<EventOption> readOnlyList = eventModel.CurrentOptions;
		if (eventModel.IsFinished)
		{
			readOnlyList = new _003C_003Ez__ReadOnlySingleElementList<EventOption>(new EventOption(eventModel, Proceed, "PROCEED", false, true));
		}
		foreach (EventOption item in readOnlyList)
		{
			item.BeforeChosen += BeforeOptionChosen;
			_connectedOptions.Add(item);
		}
		Layout.AddOptions(readOnlyList);
		ActiveScreenContext.Instance.Update();
	}

	public void OptionButtonClicked(EventOption option, int index)
	{
		if (option.IsLocked)
		{
			return;
		}
		if (option.IsProceed)
		{
			TaskHelper.RunSafely(option.Chosen());
			return;
		}
		if (!_event.IsShared)
		{
			Layout.ClearOptions();
		}
		RunManager.Instance.EventSynchronizer.ChooseLocalOption(index);
	}

	private async Task BeforeOptionChosen(EventOption option)
	{
		if (_event.Owner.RunState.Players.Count > 1 && RunManager.Instance.EventSynchronizer.IsShared && !option.IsProceed)
		{
			await Layout.BeforeSharedOptionChosen(option);
		}
		else if (!option.IsProceed)
		{
			DisableOptionButtons();
		}
	}

	private void RefreshEventState(EventModel eventModel)
	{
		SetDescription(GetDescriptionOrFallback());
		if (eventModel is AncientEventModel)
		{
			((NAncientEventLayout)Layout).ClearDialogue();
		}
		SetOptions(_event);
	}

	private void DisableOptionButtons()
	{
		Layout?.DisableEventOptions();
	}

	private void OnEnteringEventCombat()
	{
		DisableOptionButtons();
		if (Layout is NCombatEventLayout nCombatEventLayout)
		{
			nCombatEventLayout.HideEventVisuals();
		}
	}

	public static Task Proceed()
	{
		NMapScreen.Instance.SetTravelEnabled(enabled: true);
		NMapScreen.Instance.Open();
		return Task.CompletedTask;
	}

	private LocString GetDescriptionOrFallback()
	{
		return _event.Description ?? new LocString("events", "ERROR.description");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetPortrait, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("portrait"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableOptionButtons, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnteringEventCombat, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.SetPortrait && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetPortrait(VariantUtils.ConvertTo<Texture2D>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableOptionButtons && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableOptionButtons();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEnteringEventCombat && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEnteringEventCombat();
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.SetPortrait)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableOptionButtons)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEnteringEventCombat)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.VfxContainer)
		{
			VfxContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isPreFinished)
		{
			_isPreFinished = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._eventContainer)
		{
			_eventContainer = VariantUtils.ConvertTo<NSceneContainer>(ref value);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Layout)
		{
			NEventLayout layout = Layout;
			value = VariantUtils.CreateFrom<NEventLayout>(ref layout);
			return true;
		}
		if ((ref name) == PropertyName.EmbeddedCombatRoom)
		{
			NCombatRoom embeddedCombatRoom = EmbeddedCombatRoom;
			value = VariantUtils.CreateFrom<NCombatRoom>(ref embeddedCombatRoom);
			return true;
		}
		if ((ref name) == PropertyName.VfxContainer)
		{
			Control vfxContainer = VfxContainer;
			value = VariantUtils.CreateFrom<Control>(ref vfxContainer);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control vfxContainer = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref vfxContainer);
			return true;
		}
		if ((ref name) == PropertyName._isPreFinished)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isPreFinished);
			return true;
		}
		if ((ref name) == PropertyName._eventContainer)
		{
			value = VariantUtils.CreateFrom<NSceneContainer>(ref _eventContainer);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._isPreFinished, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._eventContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Layout, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.EmbeddedCombatRoom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.VfxContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName vfxContainer = PropertyName.VfxContainer;
		Control vfxContainer2 = VfxContainer;
		info.AddProperty(vfxContainer, Variant.From<Control>(ref vfxContainer2));
		info.AddProperty(PropertyName._isPreFinished, Variant.From<bool>(ref _isPreFinished));
		info.AddProperty(PropertyName._eventContainer, Variant.From<NSceneContainer>(ref _eventContainer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.VfxContainer, ref val))
		{
			VfxContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._isPreFinished, ref val2))
		{
			_isPreFinished = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._eventContainer, ref val3))
		{
			_eventContainer = ((Variant)(ref val3)).As<NSceneContainer>();
		}
	}
}
