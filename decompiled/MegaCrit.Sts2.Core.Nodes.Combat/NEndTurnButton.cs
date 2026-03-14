using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NEndTurnButton.cs")]
public class NEndTurnButton : NButton
{
	private enum State
	{
		Enabled,
		Disabled,
		Hidden
	}

	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName StartOrStopPulseVfx = StringName.op_Implicit("StartOrStopPulseVfx");

		public static readonly StringName GlowPulse = StringName.op_Implicit("GlowPulse");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public static readonly StringName CallReleaseLogic = StringName.op_Implicit("CallReleaseLogic");

		public static readonly StringName SecretEndTurnLogicViaFtue = StringName.op_Implicit("SecretEndTurnLogicViaFtue");

		public static readonly StringName ShouldShowPlayableCardsFtue = StringName.op_Implicit("ShouldShowPlayableCardsFtue");

		public new static readonly StringName OnEnable = StringName.op_Implicit("OnEnable");

		public new static readonly StringName OnDisable = StringName.op_Implicit("OnDisable");

		public static readonly StringName AnimOut = StringName.op_Implicit("AnimOut");

		public static readonly StringName AnimIn = StringName.op_Implicit("AnimIn");

		public static readonly StringName OnCombatEnded = StringName.op_Implicit("OnCombatEnded");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName HasPlayableCard = StringName.op_Implicit("HasPlayableCard");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public static readonly StringName UpdateShaderV = StringName.op_Implicit("UpdateShaderV");

		public static readonly StringName SetState = StringName.op_Implicit("SetState");

		public static readonly StringName RefreshEnabled = StringName.op_Implicit("RefreshEnabled");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName CanTurnBeEnded = StringName.op_Implicit("CanTurnBeEnded");

		public static readonly StringName ShowPos = StringName.op_Implicit("ShowPos");

		public static readonly StringName HidePos = StringName.op_Implicit("HidePos");

		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");

		public static readonly StringName _state = StringName.op_Implicit("_state");

		public static readonly StringName _isShiny = StringName.op_Implicit("_isShiny");

		public static readonly StringName _visuals = StringName.op_Implicit("_visuals");

		public static readonly StringName _glowTexture = StringName.op_Implicit("_glowTexture");

		public static readonly StringName _normalTexture = StringName.op_Implicit("_normalTexture");

		public static readonly StringName _image = StringName.op_Implicit("_image");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _glow = StringName.op_Implicit("_glow");

		public static readonly StringName _glowVfx = StringName.op_Implicit("_glowVfx");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _combatUi = StringName.op_Implicit("_combatUi");

		public static readonly StringName _viewport = StringName.op_Implicit("_viewport");

		public static readonly StringName _playerIconContainer = StringName.op_Implicit("_playerIconContainer");

		public static readonly StringName _longPressBar = StringName.op_Implicit("_longPressBar");

		public static readonly StringName _pulseTimer = StringName.op_Implicit("_pulseTimer");

		public static readonly StringName _positionTween = StringName.op_Implicit("_positionTween");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _glowVfxTween = StringName.op_Implicit("_glowVfxTween");

		public static readonly StringName _glowEnableTween = StringName.op_Implicit("_glowEnableTween");

		public static readonly StringName _endTurnWithNoPlayableCardsCount = StringName.op_Implicit("_endTurnWithNoPlayableCardsCount");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private const float _flyInOutDuration = 0.5f;

	private static readonly LocString _endTurnLoc = new LocString("gameplay_ui", "END_TURN_BUTTON");

	private CombatState? _combatState;

	private CardPile? _playerHand;

	private State _state = State.Hidden;

	private bool _isShiny;

	private HoverTip _hoverTip;

	private Control _visuals;

	private Texture2D _glowTexture;

	private Texture2D _normalTexture;

	private TextureRect _image;

	private ShaderMaterial _hsv;

	private Control _glow;

	private Control _glowVfx;

	private MegaLabel _label;

	private NCombatUi _combatUi;

	private Viewport _viewport;

	private NMultiplayerVoteContainer _playerIconContainer;

	private NEndTurnLongPressBar _longPressBar;

	private float _pulseTimer = 1f;

	private static readonly Vector2 _hoverTipOffset = new Vector2(-76f, -302f);

	private static readonly Vector2 _showPosRatio = new Vector2(1604f, 846f) / NGame.devResolution;

	private static readonly Vector2 _hidePosRatio = _showPosRatio + new Vector2(0f, 250f) / NGame.devResolution;

	private Tween? _positionTween;

	private Tween? _hoverTween;

	private Tween? _glowVfxTween;

	private Tween? _glowEnableTween;

	private const int _ftueDisableEndTurnCount = 3;

	private int _endTurnWithNoPlayableCardsCount;

	private static string EndTurnButtonPath => "res://images/packed/combat_ui/end_turn_button.png";

	private static string EndTurnButtonGlowPath => "res://images/packed/combat_ui/end_turn_button_glow.png";

	public static IEnumerable<string> AssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.Add(EndTurnButtonPath);
			list.Add(EndTurnButtonGlowPath);
			list.AddRange(NMultiplayerVoteContainer.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	private bool CanTurnBeEnded
	{
		get
		{
			if (!NCombatRoom.Instance.Ui.Hand.InCardPlay)
			{
				return NCombatRoom.Instance.Ui.Hand.CurrentMode == NPlayerHand.Mode.Play;
			}
			return false;
		}
	}

	private Vector2 ShowPos
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			Vector2 showPosRatio = _showPosRatio;
			Rect2 visibleRect = _viewport.GetVisibleRect();
			return showPosRatio * ((Rect2)(ref visibleRect)).Size;
		}
	}

	private Vector2 HidePos
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			Vector2 hidePosRatio = _hidePosRatio;
			Rect2 visibleRect = _viewport.GetVisibleRect();
			return hidePosRatio * ((Rect2)(ref visibleRect)).Size;
		}
	}

	protected override string[] Hotkeys => new string[1] { StringName.op_Implicit(MegaInput.accept) };

	public override void _Ready()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		ConnectSignals();
		_visuals = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Visuals"));
		_image = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Visuals/Image"));
		_hsv = (ShaderMaterial)((CanvasItem)_image).Material;
		_glow = (Control)(object)((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Visuals/Glow"));
		_glowVfx = (Control)(object)((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Visuals/GlowVfx"));
		_label = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("Visuals/Label"));
		_playerIconContainer = ((Node)this).GetNode<NMultiplayerVoteContainer>(NodePath.op_Implicit("PlayerIconContainer"));
		_longPressBar = ((Node)this).GetNode<NEndTurnLongPressBar>(NodePath.op_Implicit("%Bar"));
		_longPressBar.Init(this);
		_isEnabled = false;
		_combatUi = ((Node)this).GetParent<NCombatUi>();
		_viewport = ((Node)this).GetViewport();
		_hoverTip = new HoverTip(new LocString("static_hover_tips", "END_TURN.title"), new LocString("static_hover_tips", "END_TURN.description"));
		_glowTexture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D(EndTurnButtonGlowPath);
		_normalTexture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D(EndTurnButtonPath);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		CombatManager.Instance.TurnStarted += OnTurnStarted;
		CombatManager.Instance.AboutToSwitchToEnemyTurn += OnAboutToSwitchToEnemyTurn;
		CombatManager.Instance.PlayerEndedTurn += AfterPlayerEndedTurn;
		CombatManager.Instance.PlayerUnendedTurn += AfterPlayerUnendedTurn;
		CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Tween? positionTween = _positionTween;
		if (positionTween != null)
		{
			positionTween.Kill();
		}
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		CombatManager.Instance.TurnStarted -= OnTurnStarted;
		CombatManager.Instance.AboutToSwitchToEnemyTurn -= OnAboutToSwitchToEnemyTurn;
		CombatManager.Instance.PlayerEndedTurn -= AfterPlayerEndedTurn;
		CombatManager.Instance.PlayerUnendedTurn -= AfterPlayerUnendedTurn;
		CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
	}

	public void Initialize(CombatState state)
	{
		_combatState = state;
		_playerHand = PileType.Hand.GetPile(LocalContext.GetMe(_combatState));
		_playerIconContainer.Initialize(ShouldDisplayPlayerIcon, _combatState.Players);
	}

	private bool ShouldDisplayPlayerIcon(Player player)
	{
		return CombatManager.Instance.IsPlayerReadyToEndTurn(player);
	}

	private bool PlayerCanTakeAction(Player player)
	{
		if (player.Creature.IsAlive)
		{
			if (CombatManager.Instance.PlayersTakingExtraTurn.Count != 0)
			{
				return CombatManager.Instance.PlayersTakingExtraTurn.Contains(player);
			}
			return true;
		}
		return false;
	}

	private void AfterPlayerEndedTurn(Player player, bool canBackOut)
	{
		_playerIconContainer.RefreshPlayerVotes();
		if (LocalContext.IsMe(player))
		{
			StartOrStopPulseVfx();
			Player me = LocalContext.GetMe(player.Creature.CombatState);
			if (!CombatManager.Instance.AllPlayersReadyToEndTurn() && PlayerCanTakeAction(me) && canBackOut)
			{
				SetState(State.Enabled);
				_label.SetTextAutoSize(new LocString("gameplay_ui", "UNDO_END_TURN_BUTTON").GetFormattedText());
			}
			else
			{
				SetState(State.Disabled);
			}
		}
		if (CombatManager.Instance.AllPlayersReadyToEndTurn())
		{
			SetState(State.Disabled);
		}
	}

	private void AfterPlayerUnendedTurn(Player player)
	{
		_playerIconContainer.RefreshPlayerVotes();
		if (LocalContext.IsMe(player) && PlayerCanTakeAction(player))
		{
			SetState(State.Enabled);
			_endTurnLoc.Add("turnNumber", player.Creature.CombatState.RoundNumber);
			_label.SetTextAutoSize(_endTurnLoc.GetFormattedText());
			StartOrStopPulseVfx();
		}
	}

	private void OnAboutToSwitchToEnemyTurn(CombatState _)
	{
		SetState(State.Hidden);
	}

	private void OnTurnStarted(CombatState state)
	{
		if (state.CurrentSide == CombatSide.Player && CombatManager.Instance.IsInProgress)
		{
			_playerIconContainer.RefreshPlayerVotes(animate: false);
			Player me = LocalContext.GetMe(state);
			_endTurnLoc.Add("turnNumber", state.RoundNumber);
			_label.SetTextAutoSize(_endTurnLoc.GetFormattedText());
			if (PlayerCanTakeAction(me))
			{
				SetState(State.Enabled);
				return;
			}
			AnimIn();
			SetState(State.Disabled);
		}
	}

	private void OnCombatStateChanged(CombatState combatState)
	{
		StartOrStopPulseVfx();
	}

	private void StartOrStopPulseVfx()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !HasPlayableCard() && !CombatManager.Instance.IsPlayerReadyToEndTurn(LocalContext.GetMe(_combatState)) && _state == State.Enabled;
		if (_isShiny)
		{
			if (!flag)
			{
				_isShiny = false;
				Tween? glowEnableTween = _glowEnableTween;
				if (glowEnableTween != null)
				{
					glowEnableTween.Kill();
				}
				_glowEnableTween = ((Node)this).CreateTween().SetParallel(true);
				_glowEnableTween.TweenProperty((GodotObject)(object)_glow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
				Tween? glowVfxTween = _glowVfxTween;
				if (glowVfxTween != null)
				{
					glowVfxTween.Kill();
				}
				_glowVfxTween = ((Node)this).CreateTween();
				_glowVfxTween.TweenProperty((GodotObject)(object)_glowVfx, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			}
		}
		else if (flag)
		{
			_isShiny = true;
			Tween? glowVfxTween2 = _glowVfxTween;
			if (glowVfxTween2 != null)
			{
				glowVfxTween2.Kill();
			}
			GlowPulse();
			Tween? glowEnableTween2 = _glowEnableTween;
			if (glowEnableTween2 != null)
			{
				glowEnableTween2.Kill();
			}
			_glowEnableTween = ((Node)this).CreateTween().SetParallel(true);
			_glowEnableTween.TweenProperty((GodotObject)(object)_glow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.75f), 0.8).SetEase((EaseType)1).SetTrans((TransitionType)10);
			_glowEnableTween.TweenProperty((GodotObject)(object)_glow, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.5f), 0.8).SetEase((EaseType)1).SetTrans((TransitionType)10)
				.From(Variant.op_Implicit(Vector2.One * 0.45f));
		}
	}

	private void GlowPulse()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		_glowVfxTween = ((Node)this).CreateTween().SetParallel(true).SetLoops(0);
		_glowVfxTween.TweenProperty((GodotObject)(object)_glowVfx, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.7f), 1.5).From(Variant.op_Implicit(Vector2.One * 0.5f)).SetEase((EaseType)1)
			.SetTrans((TransitionType)3);
		_glowVfxTween.TweenProperty((GodotObject)(object)_glowVfx, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.5).From(Variant.op_Implicit(0.4f));
	}

	protected override void OnRelease()
	{
		if (!ShouldShowPlayableCardsFtue())
		{
			if (SaveManager.Instance.PrefsSave.IsLongPressEnabled)
			{
				_longPressBar.CancelPress();
			}
			else
			{
				CallReleaseLogic();
			}
		}
	}

	public void CallReleaseLogic()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (CanTurnBeEnded)
		{
			Tween? glowEnableTween = _glowEnableTween;
			if (glowEnableTween != null)
			{
				glowEnableTween.Kill();
			}
			_glowEnableTween = ((Node)this).CreateTween().SetParallel(true);
			_glowEnableTween.TweenProperty((GodotObject)(object)_glow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			Player me = LocalContext.GetMe(_combatState);
			int roundNumber = me.Creature.CombatState.RoundNumber;
			if (!CombatManager.Instance.IsPlayerReadyToEndTurn(me))
			{
				SetState(State.Disabled);
				RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new EndPlayerTurnAction(me, roundNumber));
			}
			else
			{
				SetState(State.Disabled);
				RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new UndoEndPlayerTurnAction(me, roundNumber));
			}
		}
	}

	public void SecretEndTurnLogicViaFtue()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Tween? glowEnableTween = _glowEnableTween;
		if (glowEnableTween != null)
		{
			glowEnableTween.Kill();
		}
		_glowEnableTween = ((Node)this).CreateTween().SetParallel(true);
		_glowEnableTween.TweenProperty((GodotObject)(object)_glow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		Player me = LocalContext.GetMe(_combatState);
		RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new EndPlayerTurnAction(me, me.Creature.CombatState.RoundNumber));
	}

	private bool ShouldShowPlayableCardsFtue()
	{
		if (SaveManager.Instance.SeenFtue("can_play_cards_ftue"))
		{
			return false;
		}
		bool flag = LocalContext.GetMe(_combatState).PlayerCombatState.HasCardsToPlay();
		if (flag)
		{
			NModalContainer.Instance.Add((Node)(object)NCanPlayCardsFtue.Create());
			SaveManager.Instance.MarkFtueAsComplete("can_play_cards_ftue");
		}
		else
		{
			_endTurnWithNoPlayableCardsCount++;
			if (_endTurnWithNoPlayableCardsCount == 3)
			{
				Log.Info($"Ended {3} turns without cards left to play. Good job! Disabling can_play_cards ftue.");
				SaveManager.Instance.MarkFtueAsComplete("can_play_cards_ftue");
			}
		}
		return flag;
	}

	protected override void OnEnable()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base.OnEnable();
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.CustomStep(999.0);
		}
		_image.Texture = _normalTexture;
		((CanvasItem)_image).Modulate = Colors.White;
		((CanvasItem)_label).Modulate = StsColors.cream;
	}

	protected override void OnDisable()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		base.OnDisable();
		NHoverTipSet.Remove((Control)(object)this);
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.CustomStep(999.0);
		}
		((CanvasItem)_image).Modulate = StsColors.gray;
		((CanvasItem)_label).Modulate = StsColors.gray;
		StartOrStopPulseVfx();
	}

	private void AnimOut()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		Tween? positionTween = _positionTween;
		if (positionTween != null)
		{
			positionTween.Kill();
		}
		_positionTween = ((Node)this).CreateTween();
		_positionTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(HidePos), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	private void AnimIn()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Tween? positionTween = _positionTween;
		if (positionTween != null)
		{
			positionTween.Kill();
		}
		_positionTween = ((Node)this).CreateTween();
		_positionTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(ShowPos), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)10);
	}

	public void OnCombatEnded()
	{
		SetState(State.Hidden);
	}

	protected override void OnFocus()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(1.5));
		_visuals.Position = new Vector2(0f, -2f);
		Player me = LocalContext.GetMe(_combatState);
		if (!CombatManager.Instance.IsPlayerReadyToEndTurn(me))
		{
			((CanvasItem)_label).Modulate = (me.PlayerCombatState.HasCardsToPlay() ? StsColors.red : Colors.Cyan);
			_combatUi.Hand.FlashPlayableHolders();
			((Control)NHoverTipSet.CreateAndShow((Control)(object)this, _hoverTip)).GlobalPosition = ((Control)this).GlobalPosition + _hoverTipOffset;
		}
		else
		{
			((CanvasItem)_label).Modulate = StsColors.cream;
		}
	}

	private bool HasPlayableCard()
	{
		if (_playerHand == null)
		{
			return false;
		}
		foreach (CardModel card in _playerHand.Cards)
		{
			if (card.CanPlay())
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnUnfocus()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (SaveManager.Instance.PrefsSave.IsLongPressEnabled)
		{
			_longPressBar.CancelPress();
		}
		NHoverTipSet.Remove((Control)(object)this);
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_hoverTween.TweenProperty((GodotObject)(object)_visuals, NodePath.op_Implicit("position"), Variant.op_Implicit(Vector2.Zero), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_hoverTween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("modulate"), Variant.op_Implicit(base.IsEnabled ? StsColors.cream : StsColors.gray), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	protected override void OnPress()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (CanTurnBeEnded)
		{
			if (SaveManager.Instance.PrefsSave.IsLongPressEnabled)
			{
				_longPressBar.StartPress();
			}
			Tween? hoverTween = _hoverTween;
			if (hoverTween != null)
			{
				hoverTween.Kill();
			}
			_hoverTween = ((Node)this).CreateTween().SetParallel(true);
			_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_hoverTween.TweenProperty((GodotObject)(object)_visuals, NodePath.op_Implicit("position"), Variant.op_Implicit(new Vector2(0f, 8f)), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
			_hoverTween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.DarkGray), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		}
	}

	private void UpdateShaderV(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(value));
	}

	private void SetState(State newState)
	{
		if (_state != newState)
		{
			if (newState == State.Hidden)
			{
				AnimOut();
			}
			if (newState == State.Enabled && _state == State.Hidden)
			{
				AnimIn();
			}
			_state = newState;
			RefreshEnabled();
		}
	}

	public void RefreshEnabled()
	{
		bool flag = NCombatRoom.Instance == null || NCombatRoom.Instance.Mode != CombatRoomMode.ActiveCombat || !ActiveScreenContext.Instance.IsCurrent(NCombatRoom.Instance) || NCombatRoom.Instance.Ui.Hand.IsInCardSelection;
		if (_state == State.Enabled && !flag)
		{
			Enable();
		}
		else
		{
			Disable();
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
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(21);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartOrStopPulseVfx, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GlowPulse, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CallReleaseLogic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SecretEndTurnLogicViaFtue, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShouldShowPlayableCardsFtue, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimOut, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCombatEnded, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HasPlayableCard, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderV, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("newState"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshEnabled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.StartOrStopPulseVfx && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartOrStopPulseVfx();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GlowPulse && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			GlowPulse();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CallReleaseLogic && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CallReleaseLogic();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SecretEndTurnLogicViaFtue && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SecretEndTurnLogicViaFtue();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShouldShowPlayableCardsFtue && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = ShouldShowPlayableCardsFtue();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.OnEnable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEnable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDisable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDisable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimOut && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimOut();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCombatEnded && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCombatEnded();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HasPlayableCard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag2 = HasPlayableCard();
			ret = VariantUtils.CreateFrom<bool>(ref flag2);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderV && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderV(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetState && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetState(VariantUtils.ConvertTo<State>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshEnabled && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshEnabled();
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
		if ((ref method) == MethodName.StartOrStopPulseVfx)
		{
			return true;
		}
		if ((ref method) == MethodName.GlowPulse)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.CallReleaseLogic)
		{
			return true;
		}
		if ((ref method) == MethodName.SecretEndTurnLogicViaFtue)
		{
			return true;
		}
		if ((ref method) == MethodName.ShouldShowPlayableCardsFtue)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEnable)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDisable)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimOut)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimIn)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCombatEnded)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.HasPlayableCard)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderV)
		{
			return true;
		}
		if ((ref method) == MethodName.SetState)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshEnabled)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._state)
		{
			_state = VariantUtils.ConvertTo<State>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isShiny)
		{
			_isShiny = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._visuals)
		{
			_visuals = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._glowTexture)
		{
			_glowTexture = VariantUtils.ConvertTo<Texture2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._normalTexture)
		{
			_normalTexture = VariantUtils.ConvertTo<Texture2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._image)
		{
			_image = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._glow)
		{
			_glow = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._glowVfx)
		{
			_glowVfx = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._combatUi)
		{
			_combatUi = VariantUtils.ConvertTo<NCombatUi>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._viewport)
		{
			_viewport = VariantUtils.ConvertTo<Viewport>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playerIconContainer)
		{
			_playerIconContainer = VariantUtils.ConvertTo<NMultiplayerVoteContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._longPressBar)
		{
			_longPressBar = VariantUtils.ConvertTo<NEndTurnLongPressBar>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pulseTimer)
		{
			_pulseTimer = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._positionTween)
		{
			_positionTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._glowVfxTween)
		{
			_glowVfxTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._glowEnableTween)
		{
			_glowEnableTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._endTurnWithNoPlayableCardsCount)
		{
			_endTurnWithNoPlayableCardsCount = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CanTurnBeEnded)
		{
			bool canTurnBeEnded = CanTurnBeEnded;
			value = VariantUtils.CreateFrom<bool>(ref canTurnBeEnded);
			return true;
		}
		if ((ref name) == PropertyName.ShowPos)
		{
			Vector2 showPos = ShowPos;
			value = VariantUtils.CreateFrom<Vector2>(ref showPos);
			return true;
		}
		if ((ref name) == PropertyName.HidePos)
		{
			Vector2 showPos = HidePos;
			value = VariantUtils.CreateFrom<Vector2>(ref showPos);
			return true;
		}
		if ((ref name) == PropertyName.Hotkeys)
		{
			string[] hotkeys = Hotkeys;
			value = VariantUtils.CreateFrom<string[]>(ref hotkeys);
			return true;
		}
		if ((ref name) == PropertyName._state)
		{
			value = VariantUtils.CreateFrom<State>(ref _state);
			return true;
		}
		if ((ref name) == PropertyName._isShiny)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isShiny);
			return true;
		}
		if ((ref name) == PropertyName._visuals)
		{
			value = VariantUtils.CreateFrom<Control>(ref _visuals);
			return true;
		}
		if ((ref name) == PropertyName._glowTexture)
		{
			value = VariantUtils.CreateFrom<Texture2D>(ref _glowTexture);
			return true;
		}
		if ((ref name) == PropertyName._normalTexture)
		{
			value = VariantUtils.CreateFrom<Texture2D>(ref _normalTexture);
			return true;
		}
		if ((ref name) == PropertyName._image)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _image);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._glow)
		{
			value = VariantUtils.CreateFrom<Control>(ref _glow);
			return true;
		}
		if ((ref name) == PropertyName._glowVfx)
		{
			value = VariantUtils.CreateFrom<Control>(ref _glowVfx);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._combatUi)
		{
			value = VariantUtils.CreateFrom<NCombatUi>(ref _combatUi);
			return true;
		}
		if ((ref name) == PropertyName._viewport)
		{
			value = VariantUtils.CreateFrom<Viewport>(ref _viewport);
			return true;
		}
		if ((ref name) == PropertyName._playerIconContainer)
		{
			value = VariantUtils.CreateFrom<NMultiplayerVoteContainer>(ref _playerIconContainer);
			return true;
		}
		if ((ref name) == PropertyName._longPressBar)
		{
			value = VariantUtils.CreateFrom<NEndTurnLongPressBar>(ref _longPressBar);
			return true;
		}
		if ((ref name) == PropertyName._pulseTimer)
		{
			value = VariantUtils.CreateFrom<float>(ref _pulseTimer);
			return true;
		}
		if ((ref name) == PropertyName._positionTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _positionTween);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._glowVfxTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _glowVfxTween);
			return true;
		}
		if ((ref name) == PropertyName._glowEnableTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _glowEnableTween);
			return true;
		}
		if ((ref name) == PropertyName._endTurnWithNoPlayableCardsCount)
		{
			value = VariantUtils.CreateFrom<int>(ref _endTurnWithNoPlayableCardsCount);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.CanTurnBeEnded, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._state, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isShiny, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._visuals, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._glowTexture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._normalTexture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._image, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._glow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._glowVfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._combatUi, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewport, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._playerIconContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._longPressBar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._pulseTimer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.ShowPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.HidePos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._positionTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._glowVfxTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._glowEnableTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._endTurnWithNoPlayableCardsCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._state, Variant.From<State>(ref _state));
		info.AddProperty(PropertyName._isShiny, Variant.From<bool>(ref _isShiny));
		info.AddProperty(PropertyName._visuals, Variant.From<Control>(ref _visuals));
		info.AddProperty(PropertyName._glowTexture, Variant.From<Texture2D>(ref _glowTexture));
		info.AddProperty(PropertyName._normalTexture, Variant.From<Texture2D>(ref _normalTexture));
		info.AddProperty(PropertyName._image, Variant.From<TextureRect>(ref _image));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._glow, Variant.From<Control>(ref _glow));
		info.AddProperty(PropertyName._glowVfx, Variant.From<Control>(ref _glowVfx));
		info.AddProperty(PropertyName._label, Variant.From<MegaLabel>(ref _label));
		info.AddProperty(PropertyName._combatUi, Variant.From<NCombatUi>(ref _combatUi));
		info.AddProperty(PropertyName._viewport, Variant.From<Viewport>(ref _viewport));
		info.AddProperty(PropertyName._playerIconContainer, Variant.From<NMultiplayerVoteContainer>(ref _playerIconContainer));
		info.AddProperty(PropertyName._longPressBar, Variant.From<NEndTurnLongPressBar>(ref _longPressBar));
		info.AddProperty(PropertyName._pulseTimer, Variant.From<float>(ref _pulseTimer));
		info.AddProperty(PropertyName._positionTween, Variant.From<Tween>(ref _positionTween));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._glowVfxTween, Variant.From<Tween>(ref _glowVfxTween));
		info.AddProperty(PropertyName._glowEnableTween, Variant.From<Tween>(ref _glowEnableTween));
		info.AddProperty(PropertyName._endTurnWithNoPlayableCardsCount, Variant.From<int>(ref _endTurnWithNoPlayableCardsCount));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._state, ref val))
		{
			_state = ((Variant)(ref val)).As<State>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._isShiny, ref val2))
		{
			_isShiny = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._visuals, ref val3))
		{
			_visuals = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._glowTexture, ref val4))
		{
			_glowTexture = ((Variant)(ref val4)).As<Texture2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._normalTexture, ref val5))
		{
			_normalTexture = ((Variant)(ref val5)).As<Texture2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._image, ref val6))
		{
			_image = ((Variant)(ref val6)).As<TextureRect>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val7))
		{
			_hsv = ((Variant)(ref val7)).As<ShaderMaterial>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._glow, ref val8))
		{
			_glow = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._glowVfx, ref val9))
		{
			_glowVfx = ((Variant)(ref val9)).As<Control>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val10))
		{
			_label = ((Variant)(ref val10)).As<MegaLabel>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._combatUi, ref val11))
		{
			_combatUi = ((Variant)(ref val11)).As<NCombatUi>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._viewport, ref val12))
		{
			_viewport = ((Variant)(ref val12)).As<Viewport>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._playerIconContainer, ref val13))
		{
			_playerIconContainer = ((Variant)(ref val13)).As<NMultiplayerVoteContainer>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._longPressBar, ref val14))
		{
			_longPressBar = ((Variant)(ref val14)).As<NEndTurnLongPressBar>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._pulseTimer, ref val15))
		{
			_pulseTimer = ((Variant)(ref val15)).As<float>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._positionTween, ref val16))
		{
			_positionTween = ((Variant)(ref val16)).As<Tween>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val17))
		{
			_hoverTween = ((Variant)(ref val17)).As<Tween>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._glowVfxTween, ref val18))
		{
			_glowVfxTween = ((Variant)(ref val18)).As<Tween>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._glowEnableTween, ref val19))
		{
			_glowEnableTween = ((Variant)(ref val19)).As<Tween>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._endTurnWithNoPlayableCardsCount, ref val20))
		{
			_endTurnWithNoPlayableCardsCount = ((Variant)(ref val20)).As<int>();
		}
	}
}
