using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Events;

[ScriptPath("res://src/Core/Nodes/Events/NEventLayout.cs")]
public class NEventLayout : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName InitializeVisuals = StringName.op_Implicit("InitializeVisuals");

		public static readonly StringName SetPortrait = StringName.op_Implicit("SetPortrait");

		public static readonly StringName AddVfxAnchoredToPortrait = StringName.op_Implicit("AddVfxAnchoredToPortrait");

		public static readonly StringName RemoveNodesOnPortrait = StringName.op_Implicit("RemoveNodesOnPortrait");

		public static readonly StringName SetTitle = StringName.op_Implicit("SetTitle");

		public static readonly StringName SetDescription = StringName.op_Implicit("SetDescription");

		public static readonly StringName AnimateIn = StringName.op_Implicit("AnimateIn");

		public static readonly StringName ClearOptions = StringName.op_Implicit("ClearOptions");

		public static readonly StringName OnSetupComplete = StringName.op_Implicit("OnSetupComplete");

		public static readonly StringName AnimateButtonsIn = StringName.op_Implicit("AnimateButtonsIn");

		public static readonly StringName DisableEventOptions = StringName.op_Implicit("DisableEventOptions");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName ApplyDebugUiVisibility = StringName.op_Implicit("ApplyDebugUiVisibility");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName VfxContainer = StringName.op_Implicit("VfxContainer");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _descriptionTween = StringName.op_Implicit("_descriptionTween");

		public static readonly StringName _optionsContainer = StringName.op_Implicit("_optionsContainer");

		public static readonly StringName _portrait = StringName.op_Implicit("_portrait");

		public static readonly StringName _title = StringName.op_Implicit("_title");

		public static readonly StringName _sharedEventLabel = StringName.op_Implicit("_sharedEventLabel");

		public static readonly StringName _description = StringName.op_Implicit("_description");
	}

	public class SignalName : SignalName
	{
	}

	public const string defaultScenePath = "res://scenes/events/default_event_layout.tscn";

	protected Tween? _descriptionTween;

	protected VBoxContainer _optionsContainer;

	private TextureRect? _portrait;

	private MegaLabel? _title;

	protected EventModel _event;

	protected MegaLabel? _sharedEventLabel;

	private static readonly LocString _sharedEventLoc = new LocString("events", "SHARED_EVENT_INFO");

	protected MegaRichTextLabel? _description;

	private static bool _isDebugUiVisible;

	public Control? VfxContainer { get; private set; }

	public IEnumerable<NEventOptionButton> OptionButtons => ((IEnumerable)((Node)_optionsContainer).GetChildren(false)).OfType<NEventOptionButton>();

	public virtual Control? DefaultFocusedControl => (Control?)(object)OptionButtons.FirstOrDefault();

	public override void _Ready()
	{
		_portrait = ((Node)this).GetNodeOrNull<TextureRect>(NodePath.op_Implicit("%Portrait"));
		_title = ((Node)this).GetNodeOrNull<MegaLabel>(NodePath.op_Implicit("%Title"));
		_description = ((Node)this).GetNodeOrNull<MegaRichTextLabel>(NodePath.op_Implicit("%EventDescription"));
		VfxContainer = ((Node)this).GetNodeOrNull<Control>(NodePath.op_Implicit("%VfxContainer"));
		_sharedEventLabel = ((Node)this).GetNodeOrNull<MegaLabel>(NodePath.op_Implicit("%SharedEventLabel"));
		_sharedEventLabel?.SetTextAutoSize(_sharedEventLoc.GetFormattedText());
		_optionsContainer = ((Node)this).GetNode<VBoxContainer>(NodePath.op_Implicit("%OptionsContainer"));
		MegaRichTextLabel? description = _description;
		if (description != null)
		{
			((RichTextLabel)description).SetText(string.Empty);
		}
		ApplyDebugUiVisibility();
	}

	public override void _EnterTree()
	{
		RunManager.Instance.EventSynchronizer.PlayerVoteChanged += OnPlayerVoteChanged;
	}

	public override void _ExitTree()
	{
		RunManager.Instance.EventSynchronizer.PlayerVoteChanged -= OnPlayerVoteChanged;
	}

	public virtual void SetEvent(EventModel eventModel)
	{
		_event = eventModel;
		InitializeVisuals();
		_event.OnRoomEnter();
	}

	protected virtual void InitializeVisuals()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		SetPortrait(_event.CreateInitialPortrait());
		if (_event.HasVfx)
		{
			Node2D val = _event.CreateVfx();
			NEventRoom.Instance.Layout.AddVfxAnchoredToPortrait((Node?)(object)val);
			val.Position = EventModel.VfxOffset;
		}
	}

	public void SetPortrait(Texture2D portrait)
	{
		if (_portrait == null)
		{
			throw new InvalidOperationException("Trying to set a portrait in an event layout that doesn't have one.");
		}
		_portrait.Texture = portrait;
	}

	public void AddVfxAnchoredToPortrait(Node? vfx)
	{
		((Node)(object)_portrait).AddChildSafely(vfx);
	}

	public void RemoveNodesOnPortrait()
	{
		foreach (Node child in ((Node)_portrait).GetChildren(false))
		{
			((Node)(object)_portrait).RemoveChildSafely(child);
		}
	}

	public void SetTitle(string title)
	{
		if (_title != null)
		{
			((Label)_title).Text = title;
		}
	}

	public void SetDescription(string description)
	{
		if (_description != null)
		{
			_description.SetTextAutoSize(description);
			AnimateIn();
		}
	}

	protected virtual void AnimateIn()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		if (_sharedEventLabel != null)
		{
			((CanvasItem)_sharedEventLabel).Modulate = StsColors.transparentWhite;
		}
		if (_description != null)
		{
			((CanvasItem)_description).Modulate = StsColors.transparentWhite;
			bool flag = SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast;
			Tween? descriptionTween = _descriptionTween;
			if (descriptionTween != null)
			{
				descriptionTween.Kill();
			}
			_descriptionTween = ((Node)this).CreateTween().SetParallel(true);
			_descriptionTween.TweenInterval(flag ? 0.2 : 0.5);
			_descriptionTween.Chain();
			if (_title != null)
			{
				_descriptionTween.TweenProperty((GodotObject)(object)_title, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), flag ? 0.25 : 0.5);
			}
			_descriptionTween.TweenProperty((GodotObject)(object)_description, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), flag ? 0.5 : 1.0).SetDelay(0.25);
			_descriptionTween.TweenProperty((GodotObject)(object)_description, NodePath.op_Implicit("visible_ratio"), Variant.op_Implicit(1f), flag ? 0.5 : 1.0).SetDelay(0.25).From(Variant.op_Implicit(0f))
				.SetEase((EaseType)1)
				.SetTrans((TransitionType)1);
			if (_sharedEventLabel != null)
			{
				_descriptionTween.TweenProperty((GodotObject)(object)_sharedEventLabel, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), flag ? 0.25 : 0.5).SetDelay(0.25);
			}
		}
	}

	public void ClearOptions()
	{
		foreach (Node item in ((IEnumerable<Node>)((Node)_optionsContainer).GetChildren(false)).ToList())
		{
			((Node)(object)_optionsContainer).RemoveChildSafely(item);
			item.QueueFreeSafely();
		}
	}

	public void AddOptions(IEnumerable<EventOption> options)
	{
		if (_sharedEventLabel != null)
		{
			MegaLabel? sharedEventLabel = _sharedEventLabel;
			EventModel eventModel = _event;
			((CanvasItem)sharedEventLabel).Visible = eventModel != null && eventModel.IsShared && !eventModel.IsFinished && _event.Owner.RunState.Players.Count > 1;
		}
		foreach (EventOption option in options)
		{
			NEventOptionButton nEventOptionButton = NEventOptionButton.Create(_event, option, ((Node)_optionsContainer).GetChildCount(false));
			((Node)(object)_optionsContainer).AddChildSafely((Node?)(object)nEventOptionButton);
			nEventOptionButton.RefreshVotes();
		}
		int childCount = ((Node)_optionsContainer).GetChildCount(false);
		if (childCount != 0)
		{
			NodePath path = ((Node)((Node)_optionsContainer).GetChild<Control>(0, false)).GetPath();
			NodePath path2 = ((Node)((Node)_optionsContainer).GetChild<Control>(childCount - 1, false)).GetPath();
			for (int i = 0; i < childCount; i++)
			{
				Control child = ((Node)_optionsContainer).GetChild<Control>(i, false);
				NodePath focusNeighborRight = (child.FocusNeighborLeft = ((Node)child).GetPath());
				child.FocusNeighborRight = focusNeighborRight;
				child.FocusNeighborTop = ((i > 0) ? ((Node)((Node)_optionsContainer).GetChild<Control>(i - 1, false)).GetPath() : path2);
				child.FocusNeighborBottom = ((i < childCount - 1) ? ((Node)((Node)_optionsContainer).GetChild<Control>(i + 1, false)).GetPath() : path);
			}
			AnimateButtonsIn();
		}
	}

	public virtual void OnSetupComplete()
	{
	}

	protected virtual void AnimateButtonsIn()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		foreach (NEventOptionButton button in OptionButtons)
		{
			Callable val = Callable.From((Action)delegate
			{
				button.AnimateIn();
			});
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		}
	}

	public async Task BeforeSharedOptionChosen(EventOption option)
	{
		NEventOptionButton chosenButton = null;
		foreach (NEventOptionButton optionButton in OptionButtons)
		{
			optionButton.Disable();
			if (optionButton.Option == option)
			{
				chosenButton = optionButton;
			}
		}
		if (chosenButton == null)
		{
			return;
		}
		EventSplitVoteAnimation eventSplitVoteAnimation = new EventSplitVoteAnimation(this, _event.Owner.RunState);
		await eventSplitVoteAnimation.TryPlay(chosenButton);
		foreach (NEventOptionButton optionButton2 in OptionButtons)
		{
			if (optionButton2.Option != option)
			{
				optionButton2.GrayOut();
			}
		}
		await chosenButton.FlashConfirmation();
	}

	private void OnPlayerVoteChanged(Player player)
	{
		foreach (NEventOptionButton optionButton in OptionButtons)
		{
			optionButton.RefreshVotes();
		}
	}

	public void DisableEventOptions()
	{
		foreach (NEventOptionButton optionButton in OptionButtons)
		{
			optionButton.Disable();
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(DebugHotkey.hideEventUi, false))
		{
			_isDebugUiVisible = !_isDebugUiVisible;
			ApplyDebugUiVisibility();
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(_isDebugUiVisible ? "Hide Event UI" : "Show Event UI"));
		}
	}

	private void ApplyDebugUiVisibility()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (_isDebugUiVisible)
		{
			((CanvasItem)_optionsContainer).Visible = false;
			if (_title != null)
			{
				((CanvasItem)_title).Modulate = Colors.Transparent;
			}
			if (_description != null)
			{
				((CanvasItem)_description).Visible = false;
			}
		}
		else
		{
			((CanvasItem)_optionsContainer).Visible = true;
			if (_description != null)
			{
				((CanvasItem)_description).Visible = true;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Expected O, but got Unknown
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(16);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetPortrait, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("portrait"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddVfxAnchoredToPortrait, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("vfx"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveNodesOnPortrait, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTitle, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("title"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetDescription, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("description"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearOptions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSetupComplete, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateButtonsIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableEventOptions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ApplyDebugUiVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitializeVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitializeVisuals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetPortrait && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetPortrait(VariantUtils.ConvertTo<Texture2D>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddVfxAnchoredToPortrait && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AddVfxAnchoredToPortrait(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemoveNodesOnPortrait && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RemoveNodesOnPortrait();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetTitle && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTitle(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetDescription && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetDescription(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearOptions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearOptions();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSetupComplete && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSetupComplete();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateButtonsIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateButtonsIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableEventOptions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableEventOptions();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ApplyDebugUiVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ApplyDebugUiVisibility();
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.InitializeVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.SetPortrait)
		{
			return true;
		}
		if ((ref method) == MethodName.AddVfxAnchoredToPortrait)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveNodesOnPortrait)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTitle)
		{
			return true;
		}
		if ((ref method) == MethodName.SetDescription)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateIn)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearOptions)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSetupComplete)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateButtonsIn)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableEventOptions)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.ApplyDebugUiVisibility)
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
		if ((ref name) == PropertyName._descriptionTween)
		{
			_descriptionTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._optionsContainer)
		{
			_optionsContainer = VariantUtils.ConvertTo<VBoxContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._portrait)
		{
			_portrait = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._title)
		{
			_title = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sharedEventLabel)
		{
			_sharedEventLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			_description = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
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
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref name) == PropertyName._descriptionTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _descriptionTween);
			return true;
		}
		if ((ref name) == PropertyName._optionsContainer)
		{
			value = VariantUtils.CreateFrom<VBoxContainer>(ref _optionsContainer);
			return true;
		}
		if ((ref name) == PropertyName._portrait)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _portrait);
			return true;
		}
		if ((ref name) == PropertyName._title)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _title);
			return true;
		}
		if ((ref name) == PropertyName._sharedEventLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _sharedEventLabel);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _description);
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
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.VfxContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._descriptionTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._optionsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._portrait, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._title, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sharedEventLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._description, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName vfxContainer = PropertyName.VfxContainer;
		Control vfxContainer2 = VfxContainer;
		info.AddProperty(vfxContainer, Variant.From<Control>(ref vfxContainer2));
		info.AddProperty(PropertyName._descriptionTween, Variant.From<Tween>(ref _descriptionTween));
		info.AddProperty(PropertyName._optionsContainer, Variant.From<VBoxContainer>(ref _optionsContainer));
		info.AddProperty(PropertyName._portrait, Variant.From<TextureRect>(ref _portrait));
		info.AddProperty(PropertyName._title, Variant.From<MegaLabel>(ref _title));
		info.AddProperty(PropertyName._sharedEventLabel, Variant.From<MegaLabel>(ref _sharedEventLabel));
		info.AddProperty(PropertyName._description, Variant.From<MegaRichTextLabel>(ref _description));
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
		if (info.TryGetProperty(PropertyName._descriptionTween, ref val2))
		{
			_descriptionTween = ((Variant)(ref val2)).As<Tween>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._optionsContainer, ref val3))
		{
			_optionsContainer = ((Variant)(ref val3)).As<VBoxContainer>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._portrait, ref val4))
		{
			_portrait = ((Variant)(ref val4)).As<TextureRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._title, ref val5))
		{
			_title = ((Variant)(ref val5)).As<MegaLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._sharedEventLabel, ref val6))
		{
			_sharedEventLabel = ((Variant)(ref val6)).As<MegaLabel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._description, ref val7))
		{
			_description = ((Variant)(ref val7)).As<MegaRichTextLabel>();
		}
	}
}
