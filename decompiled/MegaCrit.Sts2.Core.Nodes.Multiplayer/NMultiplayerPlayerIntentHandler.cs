using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Potions;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NMultiplayerPlayerIntentHandler.cs")]
public class NMultiplayerPlayerIntentHandler : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnHitboxEntered = StringName.op_Implicit("OnHitboxEntered");

		public static readonly StringName OnHitboxExited = StringName.op_Implicit("OnHitboxExited");

		public static readonly StringName OnHoverChanged = StringName.op_Implicit("OnHoverChanged");

		public static readonly StringName RefreshHoverDisplay = StringName.op_Implicit("RefreshHoverDisplay");

		public static readonly StringName OnPeerInputStateChanged = StringName.op_Implicit("OnPeerInputStateChanged");

		public static readonly StringName OnPeerInputStateRemoved = StringName.op_Implicit("OnPeerInputStateRemoved");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName HideThinkyDots = StringName.op_Implicit("HideThinkyDots");

		public static readonly StringName RefreshHoverTips = StringName.op_Implicit("RefreshHoverTips");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName CardIntent = StringName.op_Implicit("CardIntent");

		public static readonly StringName _cardIntent = StringName.op_Implicit("_cardIntent");

		public static readonly StringName _relicIntent = StringName.op_Implicit("_relicIntent");

		public static readonly StringName _potionIntent = StringName.op_Implicit("_potionIntent");

		public static readonly StringName _powerIntent = StringName.op_Implicit("_powerIntent");

		public static readonly StringName _hitbox = StringName.op_Implicit("_hitbox");

		public static readonly StringName _targetingIndicator = StringName.op_Implicit("_targetingIndicator");

		public static readonly StringName _cardThinkyDots = StringName.op_Implicit("_cardThinkyDots");

		public static readonly StringName _relicThinkyDots = StringName.op_Implicit("_relicThinkyDots");

		public static readonly StringName _potionThinkyDots = StringName.op_Implicit("_potionThinkyDots");

		public static readonly StringName _powerThinkyDots = StringName.op_Implicit("_powerThinkyDots");

		public static readonly StringName _shouldShowHoverTip = StringName.op_Implicit("_shouldShowHoverTip");

		public static readonly StringName _hoverTips = StringName.op_Implicit("_hoverTips");

		public static readonly StringName _isInPlayerChoice = StringName.op_Implicit("_isInPlayerChoice");

		public static readonly StringName _cardInPlayAwaitingPlayerChoice = StringName.op_Implicit("_cardInPlayAwaitingPlayerChoice");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "combat/multiplayer_player_intent";

	private NMultiplayerCardIntent _cardIntent;

	private NRelic _relicIntent;

	private NPotion _potionIntent;

	private NPower _powerIntent;

	private Control _hitbox;

	private NRemoteTargetingIndicator _targetingIndicator;

	private MegaRichTextLabel _cardThinkyDots;

	private MegaRichTextLabel _relicThinkyDots;

	private MegaRichTextLabel _potionThinkyDots;

	private MegaRichTextLabel _powerThinkyDots;

	private bool _shouldShowHoverTip;

	private Player _player;

	private AbstractModel? _displayedModel;

	private NHoverTipSet? _hoverTips;

	private bool _isInPlayerChoice;

	private NCard? _cardInPlayAwaitingPlayerChoice;

	private Tween? _tween;

	public NMultiplayerCardIntent CardIntent => _cardIntent;

	public static NMultiplayerPlayerIntentHandler? Create(Player player)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
		{
			return null;
		}
		NMultiplayerPlayerIntentHandler nMultiplayerPlayerIntentHandler = PreloadManager.Cache.GetScene(SceneHelper.GetScenePath("combat/multiplayer_player_intent")).Instantiate<NMultiplayerPlayerIntentHandler>((GenEditState)0);
		nMultiplayerPlayerIntentHandler._player = player;
		return nMultiplayerPlayerIntentHandler;
	}

	public override void _Ready()
	{
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		_cardIntent = ((Node)this).GetNode<NMultiplayerCardIntent>(NodePath.op_Implicit("%CardIntent"));
		_relicIntent = ((Node)this).GetNode<NRelic>(NodePath.op_Implicit("%RelicIntent"));
		_potionIntent = ((Node)this).GetNode<NPotion>(NodePath.op_Implicit("%PotionIntent"));
		_powerIntent = ((Node)this).GetNode<NPower>(NodePath.op_Implicit("%PowerIntent"));
		_hitbox = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Hitbox"));
		_targetingIndicator = ((Node)this).GetNode<NRemoteTargetingIndicator>(NodePath.op_Implicit("%TargetingIndicator"));
		_cardThinkyDots = ((Node)_cardIntent).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("ThinkyDots"));
		_relicThinkyDots = ((Node)_relicIntent).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("ThinkyDots"));
		_potionThinkyDots = ((Node)_potionIntent).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("ThinkyDots"));
		_powerThinkyDots = ((Node)_powerIntent).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("ThinkyDots"));
		_targetingIndicator.Initialize(_player);
		((CanvasItem)_cardIntent).Visible = false;
		((CanvasItem)_relicIntent).Visible = false;
		((CanvasItem)_potionIntent).Visible = false;
		((CanvasItem)_powerIntent).Visible = false;
		HideThinkyDots();
		RunManager.Instance.ActionQueueSet.ActionEnqueued += OnActionEnqueued;
		if (!LocalContext.IsMe(_player))
		{
			((GodotObject)_hitbox).Connect(SignalName.FocusEntered, Callable.From((Action)OnHitboxEntered), 0u);
			((GodotObject)_hitbox).Connect(SignalName.FocusExited, Callable.From((Action)OnHitboxExited), 0u);
			((GodotObject)_hitbox).Connect(SignalName.MouseEntered, Callable.From((Action)OnHitboxEntered), 0u);
			((GodotObject)_hitbox).Connect(SignalName.MouseExited, Callable.From((Action)OnHitboxExited), 0u);
			RunManager.Instance.HoveredModelTracker.HoverChanged += OnHoverChanged;
			RunManager.Instance.InputSynchronizer.StateChanged += OnPeerInputStateChanged;
			RunManager.Instance.InputSynchronizer.StateRemoved += OnPeerInputStateRemoved;
		}
		((Node)this).ProcessMode = (ProcessModeEnum)4;
	}

	public override void _ExitTree()
	{
		RunManager.Instance.ActionQueueSet.ActionEnqueued -= OnActionEnqueued;
		if (!LocalContext.IsMe(_player))
		{
			RunManager.Instance.HoveredModelTracker.HoverChanged -= OnHoverChanged;
			RunManager.Instance.InputSynchronizer.StateChanged -= OnPeerInputStateChanged;
			RunManager.Instance.InputSynchronizer.StateRemoved -= OnPeerInputStateRemoved;
		}
	}

	private void OnHitboxEntered()
	{
		_shouldShowHoverTip = true;
		RefreshHoverTips();
	}

	private void OnHitboxExited()
	{
		_shouldShowHoverTip = false;
		RefreshHoverTips();
	}

	private void OnHoverChanged(ulong playerId)
	{
		if (_player.NetId == playerId)
		{
			RefreshHoverDisplay();
		}
	}

	private void RefreshHoverDisplay()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		if (_isInPlayerChoice)
		{
			return;
		}
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		HideThinkyDots();
		AbstractModel abstractModel = RunManager.Instance.HoveredModelTracker.GetHoveredModel(_player.NetId);
		if (NCombatUi.IsDebugHideMpIntents)
		{
			abstractModel = null;
		}
		if (_displayedModel == abstractModel)
		{
			return;
		}
		((CanvasItem)this).Modulate = StsColors.halfTransparentWhite;
		((CanvasItem)_cardIntent).Visible = false;
		((CanvasItem)_relicIntent).Visible = false;
		((CanvasItem)_potionIntent).Visible = false;
		((CanvasItem)_powerIntent).Visible = false;
		((CanvasItem)_hitbox).Visible = abstractModel != null;
		if (abstractModel != null)
		{
			if (!(abstractModel is CardModel card))
			{
				if (!(abstractModel is PotionModel model))
				{
					if (!(abstractModel is RelicModel model2))
					{
						if (!(abstractModel is PowerModel model3))
						{
							throw new InvalidOperationException($"Player {_player.NetId} hovering unsupported model {abstractModel}");
						}
						((CanvasItem)_powerIntent).Visible = true;
						_powerIntent.Model = model3;
						_hitbox.Position = ((Control)_powerIntent).Position;
						_hitbox.Size = ((Control)_powerIntent).Size;
					}
					else
					{
						((CanvasItem)_relicIntent).Visible = true;
						_relicIntent.Model = model2;
						_hitbox.Position = ((Control)_relicIntent).Position;
						_hitbox.Size = ((Control)_relicIntent).Size;
					}
				}
				else
				{
					((CanvasItem)_potionIntent).Visible = true;
					_potionIntent.Model = model;
					_hitbox.Position = ((Control)_potionIntent).Position;
					_hitbox.Size = ((Control)_potionIntent).Size;
				}
			}
			else
			{
				((CanvasItem)_cardIntent).Visible = true;
				_cardIntent.Card = card;
				_hitbox.Position = ((Control)_cardIntent).Position;
				_hitbox.Size = ((Control)_cardIntent).Size;
			}
		}
		RefreshHoverTips();
		_displayedModel = abstractModel;
	}

	private void OnPeerInputStateChanged(ulong playerId)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (playerId == _player.NetId)
		{
			bool isTargeting = RunManager.Instance.InputSynchronizer.GetIsTargeting(_player.NetId);
			if (isTargeting && !((CanvasItem)_targetingIndicator).Visible)
			{
				_targetingIndicator.StartDrawingFrom(Vector2.Zero);
				((Node)this).ProcessMode = (ProcessModeEnum)0;
			}
			else if (!isTargeting && ((CanvasItem)_targetingIndicator).Visible)
			{
				_targetingIndicator.StopDrawing();
				((Node)this).ProcessMode = (ProcessModeEnum)4;
			}
		}
	}

	private void OnPeerInputStateRemoved(ulong playerId)
	{
		if (playerId == _player.NetId && ((CanvasItem)_targetingIndicator).Visible)
		{
			_targetingIndicator.StopDrawing();
			((Node)this).ProcessMode = (ProcessModeEnum)4;
		}
	}

	public override void _Process(double delta)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 cursorPosition = NGame.Instance.RemoteCursorContainer.GetCursorPosition(_player.NetId);
		_targetingIndicator.UpdateDrawingTo(cursorPosition - ((Node2D)_targetingIndicator).GlobalPosition);
	}

	private void OnActionEnqueued(GameAction action)
	{
		if (action.OwnerId == _player.NetId)
		{
			action.BeforeExecuted += BeforeActionExecuted;
		}
	}

	private void BeforeActionExecuted(GameAction action)
	{
		action.BeforeExecuted -= BeforeActionExecuted;
		action.BeforePausedForPlayerChoice += BeforeActionPausedForPlayerChoice;
		action.BeforeReadyToResumeAfterPlayerChoice += BeforeActionReadyToResumeAfterPlayerChoice;
		action.AfterFinished += UnsubscribeFromAction;
		action.BeforeCancelled += UnsubscribeFromAction;
	}

	private void UnsubscribeFromAction(GameAction action)
	{
		action.BeforePausedForPlayerChoice -= BeforeActionPausedForPlayerChoice;
		action.BeforeReadyToResumeAfterPlayerChoice -= BeforeActionReadyToResumeAfterPlayerChoice;
		action.AfterFinished -= UnsubscribeFromAction;
		action.BeforeCancelled -= UnsubscribeFromAction;
	}

	private void BeforeActionPausedForPlayerChoice(GameAction action)
	{
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		AbstractModel abstractModel = null;
		if (action is PlayCardAction playCardAction)
		{
			abstractModel = playCardAction.PlayerChoiceContext?.LastInvolvedModel;
		}
		else if (action is UsePotionAction usePotionAction)
		{
			abstractModel = usePotionAction.PlayerChoiceContext?.LastInvolvedModel;
		}
		else if (action is GenericHookGameAction genericHookGameAction)
		{
			abstractModel = genericHookGameAction.ChoiceContext?.LastInvolvedModel;
		}
		if (abstractModel == null)
		{
			return;
		}
		_isInPlayerChoice = true;
		((CanvasItem)_cardIntent).Visible = false;
		((CanvasItem)_relicIntent).Visible = false;
		((CanvasItem)_potionIntent).Visible = false;
		((CanvasItem)_powerIntent).Visible = false;
		((CanvasItem)_hitbox).Visible = false;
		_cardInPlayAwaitingPlayerChoice = null;
		if (abstractModel is CardModel card)
		{
			NCard nCard = NCard.FindOnTable(card);
			((CanvasItem)_cardThinkyDots).Visible = true;
			((Node)_cardThinkyDots).ProcessMode = (ProcessModeEnum)3;
			if (nCard != null)
			{
				nCard.PlayPileTween?.FastForwardToCompletion();
				Tween val = ((Node)nCard).CreateTween();
				val.Parallel().TweenProperty((GodotObject)(object)nCard, NodePath.op_Implicit("position"), Variant.op_Implicit(((Control)_cardIntent).GlobalPosition + ((Control)_cardIntent).Size / 2f), (double)((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.2f : 0.3f));
				val.Parallel().TweenProperty((GodotObject)(object)nCard, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.25f), (double)((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.2f : 0.3f));
				_cardInPlayAwaitingPlayerChoice = nCard;
				((Node)_cardThinkyDots).Reparent(((Node)nCard).GetParent(), true);
				((Node)_hitbox).Reparent(((Node)nCard).GetParent(), true);
			}
			else
			{
				_cardIntent.Card = card;
				((CanvasItem)_cardIntent).Visible = true;
			}
			((CanvasItem)_hitbox).Visible = true;
			_hitbox.GlobalPosition = ((Control)_cardIntent).GlobalPosition;
			_hitbox.Size = ((Control)_cardIntent).Size;
		}
		else if (abstractModel is RelicModel model)
		{
			_relicIntent.Model = model;
			((CanvasItem)_relicIntent).Visible = true;
			((CanvasItem)_relicThinkyDots).Visible = true;
			((Node)_relicThinkyDots).ProcessMode = (ProcessModeEnum)3;
			((CanvasItem)_hitbox).Visible = true;
			_hitbox.Position = ((Control)_relicIntent).Position;
			_hitbox.Size = ((Control)_relicIntent).Size;
		}
		else if (abstractModel is PotionModel model2)
		{
			_potionIntent.Model = model2;
			((CanvasItem)_potionIntent).Visible = true;
			((CanvasItem)_potionThinkyDots).Visible = true;
			((Node)_potionThinkyDots).ProcessMode = (ProcessModeEnum)3;
			((CanvasItem)_hitbox).Visible = true;
			_hitbox.Position = ((Control)_potionIntent).Position;
			_hitbox.Size = ((Control)_potionIntent).Size;
		}
		else if (abstractModel is PowerModel model3)
		{
			_powerIntent.Model = model3;
			((CanvasItem)_powerIntent).Visible = true;
			((CanvasItem)_powerThinkyDots).Visible = true;
			((Node)_powerThinkyDots).ProcessMode = (ProcessModeEnum)3;
			((CanvasItem)_hitbox).Visible = true;
			_hitbox.Position = ((Control)_powerIntent).Position;
			_hitbox.Size = ((Control)_powerIntent).Size;
		}
		RefreshHoverTips();
		((CanvasItem)this).Modulate = StsColors.transparentWhite;
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).GetTree().CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25);
	}

	private void BeforeActionReadyToResumeAfterPlayerChoice(GameAction action)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).GetTree().CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentWhite), 0.15000000596046448);
		_tween.TweenCallback(Callable.From((Action)HideThinkyDots));
		_isInPlayerChoice = false;
		if (_cardInPlayAwaitingPlayerChoice != null)
		{
			((Node)_cardThinkyDots).Reparent((Node)(object)_cardIntent, true);
			((Node)_hitbox).Reparent((Node)(object)this, true);
			NCardPlayQueue.Instance.ReAddCardAfterPlayerChoice(_cardInPlayAwaitingPlayerChoice, action);
			_cardInPlayAwaitingPlayerChoice = null;
			RefreshHoverTips();
		}
	}

	private void HideThinkyDots()
	{
		((CanvasItem)_cardThinkyDots).Visible = false;
		((CanvasItem)_relicThinkyDots).Visible = false;
		((CanvasItem)_potionThinkyDots).Visible = false;
		((CanvasItem)_powerThinkyDots).Visible = false;
		((Node)_cardThinkyDots).ProcessMode = (ProcessModeEnum)4;
		((Node)_relicThinkyDots).ProcessMode = (ProcessModeEnum)4;
		((Node)_potionThinkyDots).ProcessMode = (ProcessModeEnum)4;
		((Node)_powerThinkyDots).ProcessMode = (ProcessModeEnum)4;
	}

	private void RefreshHoverTips()
	{
		if (LocalContext.IsMe(_player))
		{
			return;
		}
		if (NCombatUi.IsDebugHideTargetingUi)
		{
			_shouldShowHoverTip = false;
		}
		else if (!((CanvasItem)_hitbox).Visible)
		{
			_shouldShowHoverTip = false;
		}
		NHoverTipSet.Remove((Control)(object)this);
		if (_shouldShowHoverTip)
		{
			List<IHoverTip> list = new List<IHoverTip>();
			if (_cardInPlayAwaitingPlayerChoice != null)
			{
				list.Add(HoverTipFactory.FromCard(_cardInPlayAwaitingPlayerChoice.Model));
				list.AddRange(_cardInPlayAwaitingPlayerChoice.Model.HoverTips);
			}
			else if (((CanvasItem)_cardIntent).Visible)
			{
				list.Add(HoverTipFactory.FromCard(_cardIntent.Card));
				list.AddRange(_cardIntent.Card.HoverTips);
			}
			else if (((CanvasItem)_relicIntent).Visible)
			{
				list.AddRange(_relicIntent.Model.HoverTips);
			}
			else if (((CanvasItem)_potionIntent).Visible)
			{
				list.Add(HoverTipFactory.FromPotion(_potionIntent.Model));
				list.AddRange(_potionIntent.Model.HoverTips);
			}
			if (list.Count > 0)
			{
				NHoverTipSet.CreateAndShow((Control)(object)this, list, HoverTipAlignment.Right);
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
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHitboxEntered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHitboxExited, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHoverChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshHoverDisplay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPeerInputStateChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPeerInputStateRemoved, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideThinkyDots, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshHoverTips, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnHitboxEntered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHitboxEntered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHitboxExited && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHitboxExited();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHoverChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnHoverChanged(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshHoverDisplay && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshHoverDisplay();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPeerInputStateChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPeerInputStateChanged(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPeerInputStateRemoved && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPeerInputStateRemoved(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideThinkyDots && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideThinkyDots();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshHoverTips && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshHoverTips();
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
		if ((ref method) == MethodName.OnHitboxEntered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHitboxExited)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHoverChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshHoverDisplay)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPeerInputStateChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPeerInputStateRemoved)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.HideThinkyDots)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshHoverTips)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._cardIntent)
		{
			_cardIntent = VariantUtils.ConvertTo<NMultiplayerCardIntent>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicIntent)
		{
			_relicIntent = VariantUtils.ConvertTo<NRelic>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionIntent)
		{
			_potionIntent = VariantUtils.ConvertTo<NPotion>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._powerIntent)
		{
			_powerIntent = VariantUtils.ConvertTo<NPower>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hitbox)
		{
			_hitbox = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetingIndicator)
		{
			_targetingIndicator = VariantUtils.ConvertTo<NRemoteTargetingIndicator>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardThinkyDots)
		{
			_cardThinkyDots = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicThinkyDots)
		{
			_relicThinkyDots = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionThinkyDots)
		{
			_potionThinkyDots = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._powerThinkyDots)
		{
			_powerThinkyDots = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shouldShowHoverTip)
		{
			_shouldShowHoverTip = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTips)
		{
			_hoverTips = VariantUtils.ConvertTo<NHoverTipSet>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isInPlayerChoice)
		{
			_isInPlayerChoice = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardInPlayAwaitingPlayerChoice)
		{
			_cardInPlayAwaitingPlayerChoice = VariantUtils.ConvertTo<NCard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CardIntent)
		{
			NMultiplayerCardIntent cardIntent = CardIntent;
			value = VariantUtils.CreateFrom<NMultiplayerCardIntent>(ref cardIntent);
			return true;
		}
		if ((ref name) == PropertyName._cardIntent)
		{
			value = VariantUtils.CreateFrom<NMultiplayerCardIntent>(ref _cardIntent);
			return true;
		}
		if ((ref name) == PropertyName._relicIntent)
		{
			value = VariantUtils.CreateFrom<NRelic>(ref _relicIntent);
			return true;
		}
		if ((ref name) == PropertyName._potionIntent)
		{
			value = VariantUtils.CreateFrom<NPotion>(ref _potionIntent);
			return true;
		}
		if ((ref name) == PropertyName._powerIntent)
		{
			value = VariantUtils.CreateFrom<NPower>(ref _powerIntent);
			return true;
		}
		if ((ref name) == PropertyName._hitbox)
		{
			value = VariantUtils.CreateFrom<Control>(ref _hitbox);
			return true;
		}
		if ((ref name) == PropertyName._targetingIndicator)
		{
			value = VariantUtils.CreateFrom<NRemoteTargetingIndicator>(ref _targetingIndicator);
			return true;
		}
		if ((ref name) == PropertyName._cardThinkyDots)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _cardThinkyDots);
			return true;
		}
		if ((ref name) == PropertyName._relicThinkyDots)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _relicThinkyDots);
			return true;
		}
		if ((ref name) == PropertyName._potionThinkyDots)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _potionThinkyDots);
			return true;
		}
		if ((ref name) == PropertyName._powerThinkyDots)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _powerThinkyDots);
			return true;
		}
		if ((ref name) == PropertyName._shouldShowHoverTip)
		{
			value = VariantUtils.CreateFrom<bool>(ref _shouldShowHoverTip);
			return true;
		}
		if ((ref name) == PropertyName._hoverTips)
		{
			value = VariantUtils.CreateFrom<NHoverTipSet>(ref _hoverTips);
			return true;
		}
		if ((ref name) == PropertyName._isInPlayerChoice)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isInPlayerChoice);
			return true;
		}
		if ((ref name) == PropertyName._cardInPlayAwaitingPlayerChoice)
		{
			value = VariantUtils.CreateFrom<NCard>(ref _cardInPlayAwaitingPlayerChoice);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
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
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._cardIntent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicIntent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionIntent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._powerIntent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._targetingIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardThinkyDots, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicThinkyDots, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionThinkyDots, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._powerThinkyDots, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._shouldShowHoverTip, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTips, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isInPlayerChoice, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardInPlayAwaitingPlayerChoice, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CardIntent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._cardIntent, Variant.From<NMultiplayerCardIntent>(ref _cardIntent));
		info.AddProperty(PropertyName._relicIntent, Variant.From<NRelic>(ref _relicIntent));
		info.AddProperty(PropertyName._potionIntent, Variant.From<NPotion>(ref _potionIntent));
		info.AddProperty(PropertyName._powerIntent, Variant.From<NPower>(ref _powerIntent));
		info.AddProperty(PropertyName._hitbox, Variant.From<Control>(ref _hitbox));
		info.AddProperty(PropertyName._targetingIndicator, Variant.From<NRemoteTargetingIndicator>(ref _targetingIndicator));
		info.AddProperty(PropertyName._cardThinkyDots, Variant.From<MegaRichTextLabel>(ref _cardThinkyDots));
		info.AddProperty(PropertyName._relicThinkyDots, Variant.From<MegaRichTextLabel>(ref _relicThinkyDots));
		info.AddProperty(PropertyName._potionThinkyDots, Variant.From<MegaRichTextLabel>(ref _potionThinkyDots));
		info.AddProperty(PropertyName._powerThinkyDots, Variant.From<MegaRichTextLabel>(ref _powerThinkyDots));
		info.AddProperty(PropertyName._shouldShowHoverTip, Variant.From<bool>(ref _shouldShowHoverTip));
		info.AddProperty(PropertyName._hoverTips, Variant.From<NHoverTipSet>(ref _hoverTips));
		info.AddProperty(PropertyName._isInPlayerChoice, Variant.From<bool>(ref _isInPlayerChoice));
		info.AddProperty(PropertyName._cardInPlayAwaitingPlayerChoice, Variant.From<NCard>(ref _cardInPlayAwaitingPlayerChoice));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._cardIntent, ref val))
		{
			_cardIntent = ((Variant)(ref val)).As<NMultiplayerCardIntent>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicIntent, ref val2))
		{
			_relicIntent = ((Variant)(ref val2)).As<NRelic>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionIntent, ref val3))
		{
			_potionIntent = ((Variant)(ref val3)).As<NPotion>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._powerIntent, ref val4))
		{
			_powerIntent = ((Variant)(ref val4)).As<NPower>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._hitbox, ref val5))
		{
			_hitbox = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetingIndicator, ref val6))
		{
			_targetingIndicator = ((Variant)(ref val6)).As<NRemoteTargetingIndicator>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardThinkyDots, ref val7))
		{
			_cardThinkyDots = ((Variant)(ref val7)).As<MegaRichTextLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicThinkyDots, ref val8))
		{
			_relicThinkyDots = ((Variant)(ref val8)).As<MegaRichTextLabel>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionThinkyDots, ref val9))
		{
			_potionThinkyDots = ((Variant)(ref val9)).As<MegaRichTextLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._powerThinkyDots, ref val10))
		{
			_powerThinkyDots = ((Variant)(ref val10)).As<MegaRichTextLabel>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._shouldShowHoverTip, ref val11))
		{
			_shouldShowHoverTip = ((Variant)(ref val11)).As<bool>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTips, ref val12))
		{
			_hoverTips = ((Variant)(ref val12)).As<NHoverTipSet>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._isInPlayerChoice, ref val13))
		{
			_isInPlayerChoice = ((Variant)(ref val13)).As<bool>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardInPlayAwaitingPlayerChoice, ref val14))
		{
			_cardInPlayAwaitingPlayerChoice = ((Variant)(ref val14)).As<NCard>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val15))
		{
			_tween = ((Variant)(ref val15)).As<Tween>();
		}
	}
}
