using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Events;

[ScriptPath("res://src/Core/Nodes/Events/NAncientEventLayout.cs")]
public class NAncientEventLayout : NEventLayout
{
	public new class MethodName : NEventLayout.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public new static readonly StringName InitializeVisuals = StringName.op_Implicit("InitializeVisuals");

		public new static readonly StringName AnimateIn = StringName.op_Implicit("AnimateIn");

		public static readonly StringName ClearDialogue = StringName.op_Implicit("ClearDialogue");

		public new static readonly StringName OnSetupComplete = StringName.op_Implicit("OnSetupComplete");

		public new static readonly StringName AnimateButtonsIn = StringName.op_Implicit("AnimateButtonsIn");

		public static readonly StringName OnDialogueHitboxClicked = StringName.op_Implicit("OnDialogueHitboxClicked");

		public static readonly StringName SetDialogueLineAndAnimate = StringName.op_Implicit("SetDialogueLineAndAnimate");

		public static readonly StringName UpdateFakeNextButton = StringName.op_Implicit("UpdateFakeNextButton");

		public static readonly StringName HideNameBanner = StringName.op_Implicit("HideNameBanner");

		public static readonly StringName ShowNameBanner = StringName.op_Implicit("ShowNameBanner");

		public static readonly StringName UpdateBannerVisibility = StringName.op_Implicit("UpdateBannerVisibility");

		public static readonly StringName UpdateHotkeyDisplay = StringName.op_Implicit("UpdateHotkeyDisplay");
	}

	public new class PropertyName : NEventLayout.PropertyName
	{
		public static readonly StringName IsDialogueOnLastLine = StringName.op_Implicit("IsDialogueOnLastLine");

		public new static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _currentDialogueLine = StringName.op_Implicit("_currentDialogueLine");

		public static readonly StringName _ancientBgContainer = StringName.op_Implicit("_ancientBgContainer");

		public static readonly StringName _ancientNameBanner = StringName.op_Implicit("_ancientNameBanner");

		public static readonly StringName _bannerTween = StringName.op_Implicit("_bannerTween");

		public static readonly StringName _contentContainer = StringName.op_Implicit("_contentContainer");

		public static readonly StringName _originalContentContainerHeight = StringName.op_Implicit("_originalContentContainerHeight");

		public static readonly StringName _content = StringName.op_Implicit("_content");

		public static readonly StringName _dialogueContainer = StringName.op_Implicit("_dialogueContainer");

		public static readonly StringName _dialogueHitbox = StringName.op_Implicit("_dialogueHitbox");

		public static readonly StringName _fakeNextButtonContainer = StringName.op_Implicit("_fakeNextButtonContainer");

		public static readonly StringName _fakeNextButton = StringName.op_Implicit("_fakeNextButton");

		public static readonly StringName _fakeNextButtonControllerIcon = StringName.op_Implicit("_fakeNextButtonControllerIcon");

		public static readonly StringName _fakeNextButtonLabel = StringName.op_Implicit("_fakeNextButtonLabel");

		public static readonly StringName _contentTween = StringName.op_Implicit("_contentTween");
	}

	public new class SignalName : NEventLayout.SignalName
	{
	}

	public const string ancientScenePath = "res://scenes/events/ancient_event_layout.tscn";

	private const double _contentTweenDuration = 1.0;

	private AncientEventModel _ancientEvent;

	private readonly List<AncientDialogueLine> _dialogue = new List<AncientDialogueLine>();

	private int _currentDialogueLine;

	private NAncientBgContainer _ancientBgContainer;

	private Control? _ancientNameBanner;

	private Tween? _bannerTween;

	private Control _contentContainer;

	private float _originalContentContainerHeight;

	private VBoxContainer _content;

	private VBoxContainer _dialogueContainer;

	private NAncientDialogueHitbox _dialogueHitbox;

	private Control _fakeNextButtonContainer;

	private Control _fakeNextButton;

	private TextureRect _fakeNextButtonControllerIcon;

	private MegaLabel _fakeNextButtonLabel;

	private Tween? _contentTween;

	private bool IsDialogueOnLastLine => _currentDialogueLine >= _dialogue.Count - 1;

	public override Control? DefaultFocusedControl
	{
		get
		{
			if (!IsDialogueOnLastLine)
			{
				return null;
			}
			return (Control?)(object)base.OptionButtons.FirstOrDefault();
		}
	}

	public override void _Ready()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		base._Ready();
		_ancientBgContainer = ((Node)this).GetNode<NAncientBgContainer>(NodePath.op_Implicit("%AncientBgContainer"));
		_contentContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ContentContainer"));
		_content = ((Node)this).GetNode<VBoxContainer>(NodePath.op_Implicit("%Content"));
		_dialogueContainer = ((Node)this).GetNode<VBoxContainer>(NodePath.op_Implicit("%DialogueContainer"));
		_dialogueHitbox = ((Node)this).GetNode<NAncientDialogueHitbox>(NodePath.op_Implicit("%DialogueHitbox"));
		((GodotObject)_dialogueHitbox).Connect(NClickableControl.SignalName.Released, Callable.From<NClickableControl>((Action<NClickableControl>)OnDialogueHitboxClicked), 0u);
		((CanvasItem)_dialogueHitbox).Visible = false;
		_dialogueHitbox.Disable();
		_fakeNextButtonContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%FakeNextButtonContainer"));
		_fakeNextButton = ((Node)_fakeNextButtonContainer).GetNode<Control>(NodePath.op_Implicit("FakeNextButton"));
		_fakeNextButtonLabel = ((Node)_fakeNextButton).GetNode<MegaLabel>(NodePath.op_Implicit("Label"));
		_fakeNextButtonControllerIcon = ((Node)_fakeNextButton).GetNode<TextureRect>(NodePath.op_Implicit("ControllerIcon"));
		_originalContentContainerHeight = _contentContainer.Size.Y;
		_contentContainer.Size = new Vector2(_contentContainer.Size.X, _fakeNextButtonContainer.GlobalPosition.Y - _contentContainer.GlobalPosition.Y);
		UpdateHotkeyDisplay();
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)UpdateHotkeyDisplay), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)UpdateHotkeyDisplay), 0u);
		((GodotObject)NInputManager.Instance).Connect(NInputManager.SignalName.InputRebound, Callable.From((Action)UpdateHotkeyDisplay), 0u);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		ActiveScreenContext.Instance.Updated += UpdateBannerVisibility;
	}

	public override void _ExitTree()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		base._ExitTree();
		if (_ancientEvent.HasAmbientBgm)
		{
			SfxCmd.StopLoop(_ancientEvent.AmbientBgm);
		}
		ActiveScreenContext.Instance.Updated -= UpdateBannerVisibility;
		((GodotObject)NControllerManager.Instance).Disconnect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)UpdateHotkeyDisplay));
		((GodotObject)NControllerManager.Instance).Disconnect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)UpdateHotkeyDisplay));
		((GodotObject)NInputManager.Instance).Disconnect(NInputManager.SignalName.InputRebound, Callable.From((Action)UpdateHotkeyDisplay));
	}

	protected override void InitializeVisuals()
	{
		_ancientEvent = (AncientEventModel)_event;
		_ancientNameBanner = (Control?)(object)NAncientNameBanner.Create(_ancientEvent);
		((Node)(object)this).AddChildSafely((Node?)(object)_ancientNameBanner);
		UpdateBannerVisibility();
		AncientEventModel ancientEvent = _ancientEvent;
		if (ancientEvent != null && ancientEvent.Owner != null && ancientEvent.HealedAmount > 0)
		{
			TaskHelper.RunSafely(PlayHealVfxAfterFadeIn(_ancientEvent.Owner, _ancientEvent.HealedAmount));
		}
		foreach (Node child in ((Node)_ancientBgContainer).GetChildren(false))
		{
			((Node)(object)_ancientBgContainer).RemoveChildSafely(child);
		}
		((Node)(object)_ancientBgContainer).AddChildSafely((Node?)(object)_ancientEvent.CreateBackgroundScene().Instantiate<Control>((GenEditState)0));
		if (_ancientEvent.HasAmbientBgm)
		{
			SfxCmd.PlayLoop(_ancientEvent.AmbientBgm);
		}
	}

	protected override void AnimateIn()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (_description != null)
		{
			((CanvasItem)_description).Modulate = Colors.Transparent;
			Tween? descriptionTween = _descriptionTween;
			if (descriptionTween != null)
			{
				descriptionTween.Kill();
			}
			_descriptionTween = ((Node)this).CreateTween().SetParallel(true);
			if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast)
			{
				_descriptionTween.TweenInterval(0.2);
			}
			else
			{
				_descriptionTween.TweenInterval(0.5);
			}
			_descriptionTween.Chain();
			_descriptionTween.TweenProperty((GodotObject)(object)_description, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 1.0);
		}
	}

	public void SetDialogue(IReadOnlyList<AncientDialogueLine> lines)
	{
		_dialogue.Clear();
		_dialogue.AddRange(lines);
		_currentDialogueLine = 0;
		foreach (AncientDialogueLine line in lines)
		{
			NAncientDialogueLine child = NAncientDialogueLine.Create(line, _ancientEvent, _ancientEvent.Owner.Character);
			((Node)(object)_dialogueContainer).AddChildSafely((Node?)(object)child);
		}
	}

	public void ClearDialogue()
	{
		_dialogue.Clear();
		((Node)(object)_dialogueContainer).FreeChildren();
	}

	public override void OnSetupComplete()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		((Control)_dialogueContainer).ResetSize();
		((Control)_optionsContainer).ResetSize();
		((Control)_content).ResetSize();
		((Control)_content).Position = new Vector2(((Control)_content).Position.X, _contentContainer.Size.Y);
		SetDialogueLineAndAnimate(0);
	}

	protected override void AnimateButtonsIn()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		foreach (NEventOptionButton optionButton in base.OptionButtons)
		{
			((CanvasItem)optionButton).Modulate = Colors.White;
			optionButton.EnableButton();
		}
	}

	private async Task PlayHealVfxAfterFadeIn(Player player, decimal healAmount)
	{
		await Cmd.Wait(0.2f);
		PlayerFullscreenHealVfx.Play(player, healAmount, base.VfxContainer);
	}

	private void OnDialogueHitboxClicked(NClickableControl _)
	{
		SetDialogueLineAndAnimate(_currentDialogueLine + 1);
	}

	private void SetDialogueLineAndAnimate(int lineIndex)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		_currentDialogueLine = lineIndex;
		if (_contentTween != null)
		{
			_contentTween.Pause();
			_contentTween.CustomStep(1.0);
			_contentTween.Kill();
			_contentTween = null;
		}
		UpdateFakeNextButton();
		NAncientDialogueLine childOrNull = ((Node)_dialogueContainer).GetChildOrNull<NAncientDialogueLine>(_currentDialogueLine, false);
		childOrNull?.PlaySfx();
		float num = ((childOrNull != null) ? (((Control)childOrNull).Position.Y + ((Control)childOrNull).Size.Y) : 0f);
		if (IsDialogueOnLastLine)
		{
			((CanvasItem)_fakeNextButtonContainer).Visible = false;
			_contentContainer.Size = new Vector2(_contentContainer.Size.X, _originalContentContainerHeight);
			((CanvasItem)_dialogueHitbox).Visible = false;
			_dialogueHitbox.Disable();
			foreach (NEventOptionButton optionButton in base.OptionButtons)
			{
				optionButton.EnableButton();
			}
			num += ((Control)_optionsContainer).Size.Y + 10f;
		}
		else
		{
			((CanvasItem)_fakeNextButtonContainer).Visible = true;
			((CanvasItem)_dialogueHitbox).Visible = true;
			_dialogueHitbox.Enable();
		}
		if (((Node)_dialogueContainer).GetChildCount(false) > _currentDialogueLine)
		{
			((Node)_dialogueContainer).GetChild<NAncientDialogueLine>(_currentDialogueLine, false).SetSpeakerIconVisible();
		}
		foreach (NEventOptionButton optionButton2 in base.OptionButtons)
		{
			((Control)optionButton2).FocusMode = (FocusModeEnum)0;
		}
		_contentTween = ((Node)this).CreateTween();
		_contentTween.TweenProperty((GodotObject)(object)_content, NodePath.op_Implicit("position"), Variant.op_Implicit(new Vector2(((Control)_content).Position.X, _contentContainer.Size.Y - num)), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		if (IsDialogueOnLastLine)
		{
			_contentTween.Parallel().TweenCallback(Callable.From((Action)delegate
			{
				foreach (NEventOptionButton optionButton3 in base.OptionButtons)
				{
					((Control)optionButton3).FocusMode = (FocusModeEnum)2;
				}
				DefaultFocusedControl?.TryGrabFocus();
			})).SetDelay(0.8);
		}
		for (int num2 = 0; num2 < _currentDialogueLine; num2++)
		{
			NAncientDialogueLine child = ((Node)_dialogueContainer).GetChild<NAncientDialogueLine>(num2, false);
			child.SetTransparency((num2 != _currentDialogueLine) ? 0.25f : 1f);
		}
	}

	private void UpdateFakeNextButton()
	{
		LocString locString = ((_dialogue.Count > _currentDialogueLine) ? _dialogue[_currentDialogueLine].NextButtonText : null);
		if (locString != null)
		{
			_fakeNextButtonLabel.SetTextAutoSize(locString.GetFormattedText() ?? "");
		}
		else
		{
			((CanvasItem)_fakeNextButtonLabel).Visible = false;
		}
	}

	private void HideNameBanner()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (_ancientNameBanner != null)
		{
			Tween? bannerTween = _bannerTween;
			if (bannerTween != null)
			{
				bannerTween.Kill();
			}
			_bannerTween = ((Node)this).CreateTween();
			_bannerTween.TweenProperty((GodotObject)(object)_ancientNameBanner, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
		}
	}

	private void ShowNameBanner()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (_ancientNameBanner != null)
		{
			Tween? bannerTween = _bannerTween;
			if (bannerTween != null)
			{
				bannerTween.Kill();
			}
			_bannerTween = ((Node)this).CreateTween();
			_bannerTween.TweenProperty((GodotObject)(object)_ancientNameBanner, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5);
		}
	}

	private void UpdateBannerVisibility()
	{
		if (NEventRoom.Instance != null)
		{
			if (ActiveScreenContext.Instance.IsCurrent(NEventRoom.Instance))
			{
				ShowNameBanner();
			}
			else
			{
				HideNameBanner();
			}
		}
	}

	private void UpdateHotkeyDisplay()
	{
		((CanvasItem)_fakeNextButtonControllerIcon).Visible = NControllerManager.Instance.IsUsingController;
		string hotkey = _dialogueHitbox.GetHotkey();
		if (hotkey != null)
		{
			_fakeNextButtonControllerIcon.Texture = NInputManager.Instance.GetHotkeyIcon(hotkey);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
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
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(15);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearDialogue, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSetupComplete, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateButtonsIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDialogueHitboxClicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetDialogueLineAndAnimate, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("lineIndex"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateFakeNextButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideNameBanner, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowNameBanner, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateBannerVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateHotkeyDisplay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.AnimateIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearDialogue && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearDialogue();
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
		if ((ref method) == MethodName.OnDialogueHitboxClicked && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnDialogueHitboxClicked(VariantUtils.ConvertTo<NClickableControl>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetDialogueLineAndAnimate && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetDialogueLineAndAnimate(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateFakeNextButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateFakeNextButton();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideNameBanner && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideNameBanner();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowNameBanner && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowNameBanner();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateBannerVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateBannerVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateHotkeyDisplay && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateHotkeyDisplay();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
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
		if ((ref method) == MethodName.AnimateIn)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearDialogue)
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
		if ((ref method) == MethodName.OnDialogueHitboxClicked)
		{
			return true;
		}
		if ((ref method) == MethodName.SetDialogueLineAndAnimate)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateFakeNextButton)
		{
			return true;
		}
		if ((ref method) == MethodName.HideNameBanner)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowNameBanner)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateBannerVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateHotkeyDisplay)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._currentDialogueLine)
		{
			_currentDialogueLine = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ancientBgContainer)
		{
			_ancientBgContainer = VariantUtils.ConvertTo<NAncientBgContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ancientNameBanner)
		{
			_ancientNameBanner = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bannerTween)
		{
			_bannerTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._contentContainer)
		{
			_contentContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalContentContainerHeight)
		{
			_originalContentContainerHeight = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._content)
		{
			_content = VariantUtils.ConvertTo<VBoxContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dialogueContainer)
		{
			_dialogueContainer = VariantUtils.ConvertTo<VBoxContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dialogueHitbox)
		{
			_dialogueHitbox = VariantUtils.ConvertTo<NAncientDialogueHitbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fakeNextButtonContainer)
		{
			_fakeNextButtonContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fakeNextButton)
		{
			_fakeNextButton = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fakeNextButtonControllerIcon)
		{
			_fakeNextButtonControllerIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fakeNextButtonLabel)
		{
			_fakeNextButtonLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._contentTween)
		{
			_contentTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsDialogueOnLastLine)
		{
			bool isDialogueOnLastLine = IsDialogueOnLastLine;
			value = VariantUtils.CreateFrom<bool>(ref isDialogueOnLastLine);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._currentDialogueLine)
		{
			value = VariantUtils.CreateFrom<int>(ref _currentDialogueLine);
			return true;
		}
		if ((ref name) == PropertyName._ancientBgContainer)
		{
			value = VariantUtils.CreateFrom<NAncientBgContainer>(ref _ancientBgContainer);
			return true;
		}
		if ((ref name) == PropertyName._ancientNameBanner)
		{
			value = VariantUtils.CreateFrom<Control>(ref _ancientNameBanner);
			return true;
		}
		if ((ref name) == PropertyName._bannerTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _bannerTween);
			return true;
		}
		if ((ref name) == PropertyName._contentContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _contentContainer);
			return true;
		}
		if ((ref name) == PropertyName._originalContentContainerHeight)
		{
			value = VariantUtils.CreateFrom<float>(ref _originalContentContainerHeight);
			return true;
		}
		if ((ref name) == PropertyName._content)
		{
			value = VariantUtils.CreateFrom<VBoxContainer>(ref _content);
			return true;
		}
		if ((ref name) == PropertyName._dialogueContainer)
		{
			value = VariantUtils.CreateFrom<VBoxContainer>(ref _dialogueContainer);
			return true;
		}
		if ((ref name) == PropertyName._dialogueHitbox)
		{
			value = VariantUtils.CreateFrom<NAncientDialogueHitbox>(ref _dialogueHitbox);
			return true;
		}
		if ((ref name) == PropertyName._fakeNextButtonContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _fakeNextButtonContainer);
			return true;
		}
		if ((ref name) == PropertyName._fakeNextButton)
		{
			value = VariantUtils.CreateFrom<Control>(ref _fakeNextButton);
			return true;
		}
		if ((ref name) == PropertyName._fakeNextButtonControllerIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _fakeNextButtonControllerIcon);
			return true;
		}
		if ((ref name) == PropertyName._fakeNextButtonLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _fakeNextButtonLabel);
			return true;
		}
		if ((ref name) == PropertyName._contentTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _contentTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName._currentDialogueLine, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ancientBgContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ancientNameBanner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bannerTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._contentContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._originalContentContainerHeight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._content, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dialogueContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dialogueHitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fakeNextButtonContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fakeNextButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fakeNextButtonControllerIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fakeNextButtonLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._contentTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsDialogueOnLastLine, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._currentDialogueLine, Variant.From<int>(ref _currentDialogueLine));
		info.AddProperty(PropertyName._ancientBgContainer, Variant.From<NAncientBgContainer>(ref _ancientBgContainer));
		info.AddProperty(PropertyName._ancientNameBanner, Variant.From<Control>(ref _ancientNameBanner));
		info.AddProperty(PropertyName._bannerTween, Variant.From<Tween>(ref _bannerTween));
		info.AddProperty(PropertyName._contentContainer, Variant.From<Control>(ref _contentContainer));
		info.AddProperty(PropertyName._originalContentContainerHeight, Variant.From<float>(ref _originalContentContainerHeight));
		info.AddProperty(PropertyName._content, Variant.From<VBoxContainer>(ref _content));
		info.AddProperty(PropertyName._dialogueContainer, Variant.From<VBoxContainer>(ref _dialogueContainer));
		info.AddProperty(PropertyName._dialogueHitbox, Variant.From<NAncientDialogueHitbox>(ref _dialogueHitbox));
		info.AddProperty(PropertyName._fakeNextButtonContainer, Variant.From<Control>(ref _fakeNextButtonContainer));
		info.AddProperty(PropertyName._fakeNextButton, Variant.From<Control>(ref _fakeNextButton));
		info.AddProperty(PropertyName._fakeNextButtonControllerIcon, Variant.From<TextureRect>(ref _fakeNextButtonControllerIcon));
		info.AddProperty(PropertyName._fakeNextButtonLabel, Variant.From<MegaLabel>(ref _fakeNextButtonLabel));
		info.AddProperty(PropertyName._contentTween, Variant.From<Tween>(ref _contentTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._currentDialogueLine, ref val))
		{
			_currentDialogueLine = ((Variant)(ref val)).As<int>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._ancientBgContainer, ref val2))
		{
			_ancientBgContainer = ((Variant)(ref val2)).As<NAncientBgContainer>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._ancientNameBanner, ref val3))
		{
			_ancientNameBanner = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._bannerTween, ref val4))
		{
			_bannerTween = ((Variant)(ref val4)).As<Tween>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._contentContainer, ref val5))
		{
			_contentContainer = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalContentContainerHeight, ref val6))
		{
			_originalContentContainerHeight = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._content, ref val7))
		{
			_content = ((Variant)(ref val7)).As<VBoxContainer>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._dialogueContainer, ref val8))
		{
			_dialogueContainer = ((Variant)(ref val8)).As<VBoxContainer>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._dialogueHitbox, ref val9))
		{
			_dialogueHitbox = ((Variant)(ref val9)).As<NAncientDialogueHitbox>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._fakeNextButtonContainer, ref val10))
		{
			_fakeNextButtonContainer = ((Variant)(ref val10)).As<Control>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._fakeNextButton, ref val11))
		{
			_fakeNextButton = ((Variant)(ref val11)).As<Control>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._fakeNextButtonControllerIcon, ref val12))
		{
			_fakeNextButtonControllerIcon = ((Variant)(ref val12)).As<TextureRect>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._fakeNextButtonLabel, ref val13))
		{
			_fakeNextButtonLabel = ((Variant)(ref val13)).As<MegaLabel>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._contentTween, ref val14))
		{
			_contentTween = ((Variant)(ref val14)).As<Tween>();
		}
	}
}
