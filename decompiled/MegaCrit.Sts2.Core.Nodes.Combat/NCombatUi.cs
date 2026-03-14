using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NCombatUi.cs")]
public class NCombatUi : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName Deactivate = StringName.op_Implicit("Deactivate");

		public static readonly StringName DisconnectSignals = StringName.op_Implicit("DisconnectSignals");

		public static readonly StringName AddToPlayContainer = StringName.op_Implicit("AddToPlayContainer");

		public static readonly StringName AnimIn = StringName.op_Implicit("AnimIn");

		public static readonly StringName OnHandSelectModeEntered = StringName.op_Implicit("OnHandSelectModeEntered");

		public static readonly StringName OnHandSelectModeExited = StringName.op_Implicit("OnHandSelectModeExited");

		public static readonly StringName OnPeekButtonReady = StringName.op_Implicit("OnPeekButtonReady");

		public static readonly StringName OnPeekButtonToggled = StringName.op_Implicit("OnPeekButtonToggled");

		public static readonly StringName Enable = StringName.op_Implicit("Enable");

		public static readonly StringName Disable = StringName.op_Implicit("Disable");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName DebugHideCombatUi = StringName.op_Implicit("DebugHideCombatUi");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName EnergyCounterContainer = StringName.op_Implicit("EnergyCounterContainer");

		public static readonly StringName EndTurnButton = StringName.op_Implicit("EndTurnButton");

		public static readonly StringName PingButton = StringName.op_Implicit("PingButton");

		public static readonly StringName DrawPile = StringName.op_Implicit("DrawPile");

		public static readonly StringName DiscardPile = StringName.op_Implicit("DiscardPile");

		public static readonly StringName ExhaustPile = StringName.op_Implicit("ExhaustPile");

		public static readonly StringName Hand = StringName.op_Implicit("Hand");

		public static readonly StringName PlayContainer = StringName.op_Implicit("PlayContainer");

		public static readonly StringName PlayQueue = StringName.op_Implicit("PlayQueue");

		public static readonly StringName CardPreviewContainer = StringName.op_Implicit("CardPreviewContainer");

		public static readonly StringName MessyCardPreviewContainer = StringName.op_Implicit("MessyCardPreviewContainer");

		public static readonly StringName _starCounter = StringName.op_Implicit("_starCounter");

		public static readonly StringName _energyCounter = StringName.op_Implicit("_energyCounter");

		public static readonly StringName _combatPilesContainer = StringName.op_Implicit("_combatPilesContainer");

		public static readonly StringName _playContainerPeekModeTween = StringName.op_Implicit("_playContainerPeekModeTween");

		public static readonly StringName _originalHandChildIndex = StringName.op_Implicit("_originalHandChildIndex");
	}

	public class SignalName : SignalName
	{
	}

	private NStarCounter _starCounter;

	private NEnergyCounter _energyCounter;

	private NCombatPilesContainer _combatPilesContainer;

	private readonly Dictionary<NCard, Vector2> _originalPlayContainerCardPositions = new Dictionary<NCard, Vector2>();

	private readonly Dictionary<NCard, Vector2> _originalPlayContainerCardScales = new Dictionary<NCard, Vector2>();

	private Tween? _playContainerPeekModeTween;

	private int _originalHandChildIndex;

	private CombatState _state;

	private static bool _isDebugSlowRewards;

	private static bool _isDebugHidden;

	private static bool _isDebugHidingHand;

	public Control EnergyCounterContainer { get; private set; }

	public NEndTurnButton EndTurnButton { get; private set; }

	private NPingButton PingButton { get; set; }

	public NDrawPileButton DrawPile => _combatPilesContainer.DrawPile;

	public NDiscardPileButton DiscardPile => _combatPilesContainer.DiscardPile;

	public NExhaustPileButton ExhaustPile => _combatPilesContainer.ExhaustPile;

	public NPlayerHand Hand { get; private set; }

	public Control PlayContainer { get; private set; }

	public NCardPlayQueue PlayQueue { get; private set; }

	public Control CardPreviewContainer { get; private set; }

	public NMessyCardPreviewContainer MessyCardPreviewContainer { get; private set; }

	private IEnumerable<NCard> PlayContainerCards => ((IEnumerable)((Node)PlayContainer).GetChildren(false)).OfType<NCard>();

	public static bool IsDebugHidingIntent { get; private set; }

	public static bool IsDebugHidingPlayContainer { get; private set; }

	public static bool IsDebugHidingHpBar { get; private set; }

	public static bool IsDebugHideTextVfx { get; private set; }

	public static bool IsDebugHideTargetingUi { get; private set; }

	public static bool IsDebugHideMpTargetingUi { get; private set; }

	public static bool IsDebugHideMpIntents { get; private set; }

	public event Action? DebugToggleIntent;

	public event Action? DebugToggleHpBar;

	public override void _Ready()
	{
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		EnergyCounterContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%EnergyCounterContainer"));
		_starCounter = ((Node)this).GetNode<NStarCounter>(NodePath.op_Implicit("%StarCounter"));
		EndTurnButton = ((Node)this).GetNode<NEndTurnButton>(NodePath.op_Implicit("%EndTurnButton"));
		PingButton = ((Node)this).GetNode<NPingButton>(NodePath.op_Implicit("%PingButton"));
		_combatPilesContainer = ((Node)this).GetNode<NCombatPilesContainer>(NodePath.op_Implicit("%CombatPileContainer"));
		Hand = ((Node)this).GetNode<NPlayerHand>(NodePath.op_Implicit("%Hand"));
		PlayContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%PlayContainer"));
		PlayQueue = ((Node)this).GetNode<NCardPlayQueue>(NodePath.op_Implicit("%PlayQueue"));
		CardPreviewContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CardPreviewContainer"));
		MessyCardPreviewContainer = ((Node)this).GetNode<NMessyCardPreviewContainer>(NodePath.op_Implicit("%MessyCardPreviewContainer"));
		if (!_isDebugHidden)
		{
			return;
		}
		foreach (Control item in ((IEnumerable)((Node)this).GetChildren(false)).OfType<Control>())
		{
			if ((object)item != Hand)
			{
				((CanvasItem)item).Modulate = (_isDebugHidden ? Colors.Transparent : Colors.White);
			}
		}
	}

	public override void _ExitTree()
	{
		DisconnectSignals();
	}

	public void Activate(CombatState state)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		CombatManager.Instance.CombatEnded += AnimOut;
		CombatManager.Instance.CombatEnded += PostCombatCleanUp;
		CombatManager.Instance.CombatWon += OnCombatWon;
		_state = state;
		Player me = LocalContext.GetMe(_state);
		_combatPilesContainer.Initialize(me);
		_starCounter.Initialize(me);
		EndTurnButton.Initialize(state);
		if (me.Character.ShouldAlwaysShowStarCounter)
		{
			EnergyCounterContainer.SetPosition(new Vector2(100f, 806f), true);
		}
		_energyCounter = NEnergyCounter.Create(me);
		((Node)(object)EnergyCounterContainer).AddChildSafely((Node?)(object)_energyCounter);
		((Node)_starCounter).Reparent((Node)(object)_energyCounter, true);
		((CanvasItem)this).Visible = true;
		AnimIn();
	}

	public void Deactivate()
	{
		DisconnectSignals();
		((CanvasItem)this).Visible = false;
	}

	private void DisconnectSignals()
	{
		CombatManager.Instance.CombatEnded -= AnimOut;
		CombatManager.Instance.CombatEnded -= PostCombatCleanUp;
		CombatManager.Instance.CombatWon -= OnCombatWon;
	}

	public void AddToPlayContainer(NCard card)
	{
		((Node)card).GetParent()?.RemoveChildSafely((Node?)(object)card);
		((Node)(object)PlayContainer).AddChildSafely((Node?)(object)card);
	}

	public NCard? GetCardFromPlayContainer(CardModel model)
	{
		return PlayContainerCards.FirstOrDefault((NCard n) => n.Model == model);
	}

	private void OnCombatWon(CombatRoom room)
	{
		if (room.Encounter.ShouldGiveRewards)
		{
			TaskHelper.RunSafely(ShowRewards(room));
		}
		else
		{
			TaskHelper.RunSafely(ProceedWithoutRewards());
		}
	}

	private async Task ProceedWithoutRewards()
	{
		await Cmd.Wait(1f);
		await RunManager.Instance.ProceedFromTerminalRewardsScreen();
	}

	private async Task ShowRewards(CombatRoom room)
	{
		float num = 0f;
		foreach (NCreature removingCreatureNode in NCombatRoom.Instance.RemovingCreatureNodes)
		{
			if (removingCreatureNode != null && removingCreatureNode.HasSpineAnimation && removingCreatureNode.IsPlayingDeathAnimation)
			{
				num = Math.Max(num, removingCreatureNode.GetCurrentAnimationTimeRemaining());
				continue;
			}
			MonsterModel monster = removingCreatureNode.Entity.Monster;
			if (monster != null && monster.HasDeathAnimLengthOverride)
			{
				num = Math.Max(num, removingCreatureNode.Entity.Monster.DeathAnimLengthOverride);
			}
		}
		if (_isDebugSlowRewards)
		{
			await Cmd.Wait(num + 3f);
		}
		else if (room.RoomType == RoomType.Boss)
		{
			await Cmd.CustomScaledWait(num * 0.5f, num + 1f);
		}
		else
		{
			await Cmd.CustomScaledWait(0.5f, num + 1f);
		}
		Player me = LocalContext.GetMe(_state);
		await RewardsCmd.OfferForRoomEnd(me, room);
	}

	private void AnimIn()
	{
		Hand.AnimIn();
		_energyCounter.AnimIn();
		_combatPilesContainer.AnimIn();
	}

	public void AnimOut(CombatRoom _)
	{
		Hand.AnimOut();
		PlayQueue.AnimOut();
		EndTurnButton.OnCombatEnded();
		PingButton.OnCombatEnded();
		_energyCounter.AnimOut();
		_combatPilesContainer.AnimOut();
	}

	private void PostCombatCleanUp(CombatRoom _)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)PlayContainer, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.Transparent), 0.25);
	}

	public void OnHandSelectModeEntered()
	{
		_originalHandChildIndex = ((Node)Hand).GetIndex(false);
		((CanvasItem)Hand).MoveToFront();
		ActiveScreenContext.Instance.Update();
	}

	public void OnHandSelectModeExited()
	{
		((Node)this).MoveChild((Node)(object)Hand, _originalHandChildIndex);
		ActiveScreenContext.Instance.Update();
	}

	public void OnPeekButtonReady(NPeekButton peekButton)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)peekButton).Connect(NPeekButton.SignalName.Toggled, Callable.From<NPeekButton>((Action<NPeekButton>)OnPeekButtonToggled), 0u);
	}

	private void OnPeekButtonToggled(NPeekButton peekButton)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		if (_playContainerPeekModeTween != null)
		{
			_playContainerPeekModeTween.Pause();
			_playContainerPeekModeTween.CustomStep(0.25);
			_playContainerPeekModeTween.Kill();
			_playContainerPeekModeTween = null;
		}
		Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
		Vector2 size = ((Rect2)(ref viewportRect)).Size;
		if (peekButton.IsPeeking)
		{
			((CanvasItem)PlayQueue).Hide();
			foreach (NCard playContainerCard in PlayContainerCards)
			{
				_originalPlayContainerCardPositions[playContainerCard] = ((Control)playContainerCard).Position;
				_originalPlayContainerCardScales[playContainerCard] = ((Control)playContainerCard).Scale;
				Vector2 globalPosition = ((Node2D)peekButton.CurrentCardMarker).GlobalPosition;
				Vector2 val = ((Control)playContainerCard).Scale * 0.5f;
				if (_playContainerPeekModeTween == null)
				{
					_playContainerPeekModeTween = ((Node)this).CreateTween();
				}
				_playContainerPeekModeTween.TweenProperty((GodotObject)(object)playContainerCard, NodePath.op_Implicit("global_position"), Variant.op_Implicit(globalPosition), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7);
				_playContainerPeekModeTween.Parallel().TweenProperty((GodotObject)(object)playContainerCard, NodePath.op_Implicit("scale"), Variant.op_Implicit(val), 0.25).SetEase((EaseType)1)
					.SetTrans((TransitionType)7);
			}
		}
		else
		{
			((CanvasItem)PlayQueue).Show();
			foreach (NCard playContainerCard2 in PlayContainerCards)
			{
				Vector2 value;
				Vector2 val2 = (_originalPlayContainerCardPositions.TryGetValue(playContainerCard2, out value) ? value : (size * 0.5f));
				Vector2 value2;
				Vector2 val3 = (_originalPlayContainerCardScales.TryGetValue(playContainerCard2, out value2) ? value2 : Vector2.One);
				if (_playContainerPeekModeTween == null)
				{
					_playContainerPeekModeTween = ((Node)this).CreateTween();
				}
				_playContainerPeekModeTween.TweenProperty((GodotObject)(object)playContainerCard2, NodePath.op_Implicit("position"), Variant.op_Implicit(val2), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7);
				_playContainerPeekModeTween.Parallel().TweenProperty((GodotObject)(object)playContainerCard2, NodePath.op_Implicit("scale"), Variant.op_Implicit(val3), 0.25).SetEase((EaseType)1)
					.SetTrans((TransitionType)7);
			}
			_originalPlayContainerCardPositions.Clear();
			_originalPlayContainerCardScales.Clear();
		}
		ActiveScreenContext.Instance.Update();
	}

	public void Enable()
	{
		NPlayerHand hand = Hand;
		if (hand != null && hand.IsInCardSelection)
		{
			NPeekButton peekButton = hand.PeekButton;
			if (peekButton != null && !peekButton.IsPeeking)
			{
				_combatPilesContainer.Disable();
				goto IL_003c;
			}
		}
		_combatPilesContainer.Enable();
		goto IL_003c;
		IL_003c:
		if (_state.CurrentSide == CombatSide.Player)
		{
			EndTurnButton.RefreshEnabled();
		}
		if (_state.PlayerCreatures.Count > 1)
		{
			PingButton.RefreshEnabled();
		}
		NPlayerHand.Mode currentMode = Hand.CurrentMode;
		if ((uint)(currentMode - 2) <= 1u)
		{
			Hand.PeekButton.Enable();
		}
	}

	public void Disable()
	{
		_combatPilesContainer.Disable();
		EndTurnButton.RefreshEnabled();
		PingButton.RefreshEnabled();
		Hand.PeekButton.Disable();
		Hand.CancelAllCardPlay();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(DebugHotkey.hideIntents, false))
		{
			IsDebugHidingIntent = !IsDebugHidingIntent;
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(IsDebugHidingIntent ? "Hide Intents" : "Show Intents"));
			this.DebugToggleIntent?.Invoke();
		}
		if (inputEvent.IsActionReleased(DebugHotkey.hideCombatUi, false))
		{
			_isDebugHidden = !_isDebugHidden;
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(_isDebugHidden ? "Hide Combat UI" : "Show Combat UI"));
			DebugHideCombatUi();
		}
		else if (inputEvent.IsActionReleased(DebugHotkey.hidePlayContainer, false))
		{
			IsDebugHidingPlayContainer = !IsDebugHidingPlayContainer;
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(IsDebugHidingPlayContainer ? "Hide Played Card" : "Show Played Card"));
			DebugHideCombatUi();
		}
		else if (inputEvent.IsActionReleased(DebugHotkey.hideHand, false))
		{
			_isDebugHidingHand = !_isDebugHidingHand;
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(_isDebugHidingHand ? "Hide Hand Cards" : "Show Hand Cards"));
			DebugHideCombatUi();
		}
		else if (inputEvent.IsActionReleased(DebugHotkey.hideHpBars, false))
		{
			IsDebugHidingHpBar = !IsDebugHidingHpBar;
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(IsDebugHidingHpBar ? "Hide HP Bars" : "Show HP Bars"));
			this.DebugToggleHpBar?.Invoke();
		}
		else if (inputEvent.IsActionReleased(DebugHotkey.hideTextVfx, false))
		{
			IsDebugHideTextVfx = !IsDebugHideTextVfx;
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(IsDebugHideTextVfx ? "Hide Text Vfx" : "Show Text Vfx"));
		}
		else if (inputEvent.IsActionReleased(DebugHotkey.hideTargetingUi, false))
		{
			IsDebugHideTargetingUi = !IsDebugHideTargetingUi;
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(IsDebugHideTargetingUi ? "Hide Targeting UI" : "Show Targeting UI"));
		}
		else if (inputEvent.IsActionReleased(DebugHotkey.slowRewards, false))
		{
			_isDebugSlowRewards = !_isDebugSlowRewards;
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(_isDebugSlowRewards ? "Slow Rewards Screens" : "Normal Rewards Screen"));
		}
		else if (inputEvent.IsActionReleased(DebugHotkey.hideMpTargeting, false))
		{
			IsDebugHideMpTargetingUi = !IsDebugHideMpTargetingUi;
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(IsDebugHideMpTargetingUi ? "Hide MP Targeting" : "Show MP Targeting"));
		}
		else if (inputEvent.IsActionReleased(DebugHotkey.hideMpIntents, false))
		{
			IsDebugHideMpIntents = !IsDebugHideMpIntents;
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(IsDebugHideMpIntents ? "Hide MP Intents" : "Show MP Intents"));
		}
	}

	private void DebugHideCombatUi()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		foreach (Control item in ((IEnumerable)((Node)this).GetChildren(false)).OfType<Control>())
		{
			if ((object)item == Hand)
			{
				((CanvasItem)item).Modulate = (_isDebugHidingHand ? Colors.Transparent : Colors.White);
			}
			else if (((Node)item).Name == PropertyName.PlayContainer)
			{
				((CanvasItem)item).Modulate = (IsDebugHidingPlayContainer ? Colors.Transparent : Colors.White);
			}
			else
			{
				((CanvasItem)item).Modulate = (_isDebugHidden ? Colors.Transparent : Colors.White);
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
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Expected O, but got Unknown
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(14);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Deactivate, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisconnectSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddToPlayContainer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHandSelectModeEntered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHandSelectModeExited, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPeekButtonReady, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("peekButton"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPeekButtonToggled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("peekButton"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Enable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Disable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugHideCombatUi, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.Deactivate && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Deactivate();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisconnectSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisconnectSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddToPlayContainer && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AddToPlayContainer(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHandSelectModeEntered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHandSelectModeEntered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHandSelectModeExited && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHandSelectModeExited();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPeekButtonReady && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPeekButtonReady(VariantUtils.ConvertTo<NPeekButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPeekButtonToggled && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPeekButtonToggled(VariantUtils.ConvertTo<NPeekButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Enable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Enable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Disable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Disable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugHideCombatUi && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DebugHideCombatUi();
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
		if ((ref method) == MethodName.Deactivate)
		{
			return true;
		}
		if ((ref method) == MethodName.DisconnectSignals)
		{
			return true;
		}
		if ((ref method) == MethodName.AddToPlayContainer)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimIn)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHandSelectModeEntered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHandSelectModeExited)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPeekButtonReady)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPeekButtonToggled)
		{
			return true;
		}
		if ((ref method) == MethodName.Enable)
		{
			return true;
		}
		if ((ref method) == MethodName.Disable)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugHideCombatUi)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.EnergyCounterContainer)
		{
			EnergyCounterContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.EndTurnButton)
		{
			EndTurnButton = VariantUtils.ConvertTo<NEndTurnButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.PingButton)
		{
			PingButton = VariantUtils.ConvertTo<NPingButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Hand)
		{
			Hand = VariantUtils.ConvertTo<NPlayerHand>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.PlayContainer)
		{
			PlayContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.PlayQueue)
		{
			PlayQueue = VariantUtils.ConvertTo<NCardPlayQueue>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.CardPreviewContainer)
		{
			CardPreviewContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.MessyCardPreviewContainer)
		{
			MessyCardPreviewContainer = VariantUtils.ConvertTo<NMessyCardPreviewContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._starCounter)
		{
			_starCounter = VariantUtils.ConvertTo<NStarCounter>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._energyCounter)
		{
			_energyCounter = VariantUtils.ConvertTo<NEnergyCounter>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._combatPilesContainer)
		{
			_combatPilesContainer = VariantUtils.ConvertTo<NCombatPilesContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playContainerPeekModeTween)
		{
			_playContainerPeekModeTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalHandChildIndex)
		{
			_originalHandChildIndex = VariantUtils.ConvertTo<int>(ref value);
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
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.EnergyCounterContainer)
		{
			Control energyCounterContainer = EnergyCounterContainer;
			value = VariantUtils.CreateFrom<Control>(ref energyCounterContainer);
			return true;
		}
		if ((ref name) == PropertyName.EndTurnButton)
		{
			NEndTurnButton endTurnButton = EndTurnButton;
			value = VariantUtils.CreateFrom<NEndTurnButton>(ref endTurnButton);
			return true;
		}
		if ((ref name) == PropertyName.PingButton)
		{
			NPingButton pingButton = PingButton;
			value = VariantUtils.CreateFrom<NPingButton>(ref pingButton);
			return true;
		}
		if ((ref name) == PropertyName.DrawPile)
		{
			NDrawPileButton drawPile = DrawPile;
			value = VariantUtils.CreateFrom<NDrawPileButton>(ref drawPile);
			return true;
		}
		if ((ref name) == PropertyName.DiscardPile)
		{
			NDiscardPileButton discardPile = DiscardPile;
			value = VariantUtils.CreateFrom<NDiscardPileButton>(ref discardPile);
			return true;
		}
		if ((ref name) == PropertyName.ExhaustPile)
		{
			NExhaustPileButton exhaustPile = ExhaustPile;
			value = VariantUtils.CreateFrom<NExhaustPileButton>(ref exhaustPile);
			return true;
		}
		if ((ref name) == PropertyName.Hand)
		{
			NPlayerHand hand = Hand;
			value = VariantUtils.CreateFrom<NPlayerHand>(ref hand);
			return true;
		}
		if ((ref name) == PropertyName.PlayContainer)
		{
			Control energyCounterContainer = PlayContainer;
			value = VariantUtils.CreateFrom<Control>(ref energyCounterContainer);
			return true;
		}
		if ((ref name) == PropertyName.PlayQueue)
		{
			NCardPlayQueue playQueue = PlayQueue;
			value = VariantUtils.CreateFrom<NCardPlayQueue>(ref playQueue);
			return true;
		}
		if ((ref name) == PropertyName.CardPreviewContainer)
		{
			Control energyCounterContainer = CardPreviewContainer;
			value = VariantUtils.CreateFrom<Control>(ref energyCounterContainer);
			return true;
		}
		if ((ref name) == PropertyName.MessyCardPreviewContainer)
		{
			NMessyCardPreviewContainer messyCardPreviewContainer = MessyCardPreviewContainer;
			value = VariantUtils.CreateFrom<NMessyCardPreviewContainer>(ref messyCardPreviewContainer);
			return true;
		}
		if ((ref name) == PropertyName._starCounter)
		{
			value = VariantUtils.CreateFrom<NStarCounter>(ref _starCounter);
			return true;
		}
		if ((ref name) == PropertyName._energyCounter)
		{
			value = VariantUtils.CreateFrom<NEnergyCounter>(ref _energyCounter);
			return true;
		}
		if ((ref name) == PropertyName._combatPilesContainer)
		{
			value = VariantUtils.CreateFrom<NCombatPilesContainer>(ref _combatPilesContainer);
			return true;
		}
		if ((ref name) == PropertyName._playContainerPeekModeTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _playContainerPeekModeTween);
			return true;
		}
		if ((ref name) == PropertyName._originalHandChildIndex)
		{
			value = VariantUtils.CreateFrom<int>(ref _originalHandChildIndex);
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
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.EnergyCounterContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.EndTurnButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.PingButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._starCounter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._energyCounter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._combatPilesContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DrawPile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DiscardPile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.ExhaustPile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Hand, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.PlayContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.PlayQueue, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CardPreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.MessyCardPreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._playContainerPeekModeTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._originalHandChildIndex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName energyCounterContainer = PropertyName.EnergyCounterContainer;
		Control energyCounterContainer2 = EnergyCounterContainer;
		info.AddProperty(energyCounterContainer, Variant.From<Control>(ref energyCounterContainer2));
		StringName endTurnButton = PropertyName.EndTurnButton;
		NEndTurnButton endTurnButton2 = EndTurnButton;
		info.AddProperty(endTurnButton, Variant.From<NEndTurnButton>(ref endTurnButton2));
		StringName pingButton = PropertyName.PingButton;
		NPingButton pingButton2 = PingButton;
		info.AddProperty(pingButton, Variant.From<NPingButton>(ref pingButton2));
		StringName hand = PropertyName.Hand;
		NPlayerHand hand2 = Hand;
		info.AddProperty(hand, Variant.From<NPlayerHand>(ref hand2));
		StringName playContainer = PropertyName.PlayContainer;
		energyCounterContainer2 = PlayContainer;
		info.AddProperty(playContainer, Variant.From<Control>(ref energyCounterContainer2));
		StringName playQueue = PropertyName.PlayQueue;
		NCardPlayQueue playQueue2 = PlayQueue;
		info.AddProperty(playQueue, Variant.From<NCardPlayQueue>(ref playQueue2));
		StringName cardPreviewContainer = PropertyName.CardPreviewContainer;
		energyCounterContainer2 = CardPreviewContainer;
		info.AddProperty(cardPreviewContainer, Variant.From<Control>(ref energyCounterContainer2));
		StringName messyCardPreviewContainer = PropertyName.MessyCardPreviewContainer;
		NMessyCardPreviewContainer messyCardPreviewContainer2 = MessyCardPreviewContainer;
		info.AddProperty(messyCardPreviewContainer, Variant.From<NMessyCardPreviewContainer>(ref messyCardPreviewContainer2));
		info.AddProperty(PropertyName._starCounter, Variant.From<NStarCounter>(ref _starCounter));
		info.AddProperty(PropertyName._energyCounter, Variant.From<NEnergyCounter>(ref _energyCounter));
		info.AddProperty(PropertyName._combatPilesContainer, Variant.From<NCombatPilesContainer>(ref _combatPilesContainer));
		info.AddProperty(PropertyName._playContainerPeekModeTween, Variant.From<Tween>(ref _playContainerPeekModeTween));
		info.AddProperty(PropertyName._originalHandChildIndex, Variant.From<int>(ref _originalHandChildIndex));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.EnergyCounterContainer, ref val))
		{
			EnergyCounterContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.EndTurnButton, ref val2))
		{
			EndTurnButton = ((Variant)(ref val2)).As<NEndTurnButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.PingButton, ref val3))
		{
			PingButton = ((Variant)(ref val3)).As<NPingButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.Hand, ref val4))
		{
			Hand = ((Variant)(ref val4)).As<NPlayerHand>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName.PlayContainer, ref val5))
		{
			PlayContainer = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName.PlayQueue, ref val6))
		{
			PlayQueue = ((Variant)(ref val6)).As<NCardPlayQueue>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName.CardPreviewContainer, ref val7))
		{
			CardPreviewContainer = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName.MessyCardPreviewContainer, ref val8))
		{
			MessyCardPreviewContainer = ((Variant)(ref val8)).As<NMessyCardPreviewContainer>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._starCounter, ref val9))
		{
			_starCounter = ((Variant)(ref val9)).As<NStarCounter>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._energyCounter, ref val10))
		{
			_energyCounter = ((Variant)(ref val10)).As<NEnergyCounter>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._combatPilesContainer, ref val11))
		{
			_combatPilesContainer = ((Variant)(ref val11)).As<NCombatPilesContainer>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._playContainerPeekModeTween, ref val12))
		{
			_playContainerPeekModeTween = ((Variant)(ref val12)).As<Tween>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalHandChildIndex, ref val13))
		{
			_originalHandChildIndex = ((Variant)(ref val13)).As<int>();
		}
	}
}
