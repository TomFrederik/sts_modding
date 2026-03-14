using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Potions;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NMultiplayerPlayerState.cs")]
public class NMultiplayerPlayerState : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnCreatureValueChanged = StringName.op_Implicit("OnCreatureValueChanged");

		public static readonly StringName RefreshValues = StringName.op_Implicit("RefreshValues");

		public static readonly StringName UpdateHealthBarWidth = StringName.op_Implicit("UpdateHealthBarWidth");

		public static readonly StringName UpdateSelectionReticleWidth = StringName.op_Implicit("UpdateSelectionReticleWidth");

		public static readonly StringName OnEnergyChanged = StringName.op_Implicit("OnEnergyChanged");

		public static readonly StringName OnStarsChanged = StringName.op_Implicit("OnStarsChanged");

		public static readonly StringName RefreshCombatValues = StringName.op_Implicit("RefreshCombatValues");

		public static readonly StringName OnCreatureHovered = StringName.op_Implicit("OnCreatureHovered");

		public static readonly StringName OnCreatureUnhovered = StringName.op_Implicit("OnCreatureUnhovered");

		public static readonly StringName FlashPlayerReady = StringName.op_Implicit("FlashPlayerReady");

		public static readonly StringName UpdateHighlightedState = StringName.op_Implicit("UpdateHighlightedState");

		public static readonly StringName BlockChanged = StringName.op_Implicit("BlockChanged");

		public static readonly StringName RefreshConnectedState = StringName.op_Implicit("RefreshConnectedState");

		public static readonly StringName OnPlayerVotesCleared = StringName.op_Implicit("OnPlayerVotesCleared");

		public static readonly StringName OnPlayerEndTurnPing = StringName.op_Implicit("OnPlayerEndTurnPing");

		public static readonly StringName FlashEndTurn = StringName.op_Implicit("FlashEndTurn");

		public static readonly StringName SetNextTweenTime = StringName.op_Implicit("SetNextTweenTime");

		public static readonly StringName OnPlayerScreenChanged = StringName.op_Implicit("OnPlayerScreenChanged");

		public static readonly StringName TweenLocationIconAway = StringName.op_Implicit("TweenLocationIconAway");

		public static readonly StringName TweenLocationIconIn = StringName.op_Implicit("TweenLocationIconIn");

		public static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Hitbox = StringName.op_Implicit("Hitbox");

		public static readonly StringName _healthBar = StringName.op_Implicit("_healthBar");

		public static readonly StringName _characterIcon = StringName.op_Implicit("_characterIcon");

		public static readonly StringName _nameplateLabel = StringName.op_Implicit("_nameplateLabel");

		public static readonly StringName _topContainer = StringName.op_Implicit("_topContainer");

		public static readonly StringName _turnEndIndicator = StringName.op_Implicit("_turnEndIndicator");

		public static readonly StringName _disconnectedIndicator = StringName.op_Implicit("_disconnectedIndicator");

		public static readonly StringName _networkProblemIndicator = StringName.op_Implicit("_networkProblemIndicator");

		public static readonly StringName _selectionReticle = StringName.op_Implicit("_selectionReticle");

		public static readonly StringName _locationIcon = StringName.op_Implicit("_locationIcon");

		public static readonly StringName _locationContainer = StringName.op_Implicit("_locationContainer");

		public static readonly StringName _energyContainer = StringName.op_Implicit("_energyContainer");

		public static readonly StringName _energyImage = StringName.op_Implicit("_energyImage");

		public static readonly StringName _energyCount = StringName.op_Implicit("_energyCount");

		public static readonly StringName _starContainer = StringName.op_Implicit("_starContainer");

		public static readonly StringName _starCount = StringName.op_Implicit("_starCount");

		public static readonly StringName _cardContainer = StringName.op_Implicit("_cardContainer");

		public static readonly StringName _cardImage = StringName.op_Implicit("_cardImage");

		public static readonly StringName _cardCount = StringName.op_Implicit("_cardCount");

		public static readonly StringName _locationIconTween = StringName.op_Implicit("_locationIconTween");

		public static readonly StringName _isMouseOver = StringName.op_Implicit("_isMouseOver");

		public static readonly StringName _isCreatureHovered = StringName.op_Implicit("_isCreatureHovered");

		public static readonly StringName _isHighlighted = StringName.op_Implicit("_isHighlighted");

		public static readonly StringName _focusedWhileTargeting = StringName.op_Implicit("_focusedWhileTargeting");

		public static readonly StringName _nextTweenTime = StringName.op_Implicit("_nextTweenTime");

		public static readonly StringName _currentLocationIcon = StringName.op_Implicit("_currentLocationIcon");
	}

	public class SignalName : SignalName
	{
	}

	private const ulong _delayBetweenTweensMsec = 500uL;

	private static readonly string _scenePath = SceneHelper.GetScenePath("ui/multiplayer_player_state");

	private static readonly string _cardScenePath = SceneHelper.GetScenePath("screens/run_history_screen/deck_history_entry");

	private const string _darkenedEnergyMatPath = "res://materials/ui/energy_orb_dark.tres";

	private const float _refHpBarWidth = 175f;

	private const float _refHpBarMaxHp = 80f;

	private const float _selectionReticlePadding = 6f;

	private NHealthBar _healthBar;

	private TextureRect _characterIcon;

	private MegaLabel _nameplateLabel;

	private HBoxContainer _topContainer;

	private TextureRect _turnEndIndicator;

	private TextureRect _disconnectedIndicator;

	private NMultiplayerNetworkProblemIndicator _networkProblemIndicator;

	private NSelectionReticle _selectionReticle;

	private TextureRect _locationIcon;

	private Control _locationContainer;

	private Control _energyContainer;

	private TextureRect _energyImage;

	private MegaLabel _energyCount;

	private Control _starContainer;

	private MegaLabel _starCount;

	private Control _cardContainer;

	private NTinyCard _cardImage;

	private MegaLabel _cardCount;

	private Tween? _locationIconTween;

	private bool _isMouseOver;

	private bool _isCreatureHovered;

	private bool _isHighlighted;

	private bool _focusedWhileTargeting;

	private ulong _nextTweenTime;

	private Texture2D? _currentLocationIcon;

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { _scenePath, _cardScenePath });

	public NButton Hitbox { get; private set; }

	public Player Player { get; private set; }

	public static NMultiplayerPlayerState Create(Player player)
	{
		NMultiplayerPlayerState nMultiplayerPlayerState = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NMultiplayerPlayerState>((GenEditState)0);
		nMultiplayerPlayerState.Player = player;
		return nMultiplayerPlayerState;
	}

	public override void _Ready()
	{
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		_nameplateLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%NameplateLabel"));
		_healthBar = ((Node)this).GetNode<NHealthBar>(NodePath.op_Implicit("%HealthBar"));
		_characterIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%CharacterIcon"));
		_turnEndIndicator = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%TurnEndIndicator"));
		_disconnectedIndicator = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%DisconnectedIndicator"));
		_networkProblemIndicator = ((Node)this).GetNode<NMultiplayerNetworkProblemIndicator>(NodePath.op_Implicit("%NetworkProblemIndicator"));
		_selectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("%SelectionReticle"));
		Hitbox = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("%Hitbox"));
		_locationIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%LocationIcon"));
		_locationContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%LocationContainer"));
		_topContainer = ((Node)this).GetNode<HBoxContainer>(NodePath.op_Implicit("TopInfoContainer"));
		_energyContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%EnergyCountContainer"));
		_energyImage = ((Node)_energyContainer).GetNode<TextureRect>(NodePath.op_Implicit("Image"));
		_energyCount = ((Node)_energyContainer).GetNode<MegaLabel>(NodePath.op_Implicit("EnergyCount"));
		_starContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%StarCountContainer"));
		_starCount = ((Node)_starContainer).GetNode<MegaLabel>(NodePath.op_Implicit("StarCount"));
		_cardContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CardCountContainer"));
		_cardImage = ((Node)_cardContainer).GetNode<NTinyCard>(NodePath.op_Implicit("TinyCard"));
		_cardCount = ((Node)_cardContainer).GetNode<MegaLabel>(NodePath.op_Implicit("CardCount"));
		((CanvasItem)_selectionReticle).Visible = true;
		_characterIcon.Texture = Player.Character.IconTexture;
		_nameplateLabel.SetTextAutoSize(PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, Player.NetId));
		_healthBar.SetCreature(Player.Creature);
		_networkProblemIndicator.Initialize(Player.NetId);
		((CanvasItem)_locationContainer).Visible = false;
		((CanvasItem)_energyContainer).Visible = false;
		_energyImage.Texture = ResourceLoader.Load<Texture2D>(Player.Character.CardPool.EnergyIconPath, (string)null, (CacheMode)1);
		((CanvasItem)_starContainer).Visible = false;
		((CanvasItem)_cardContainer).Visible = false;
		_cardImage.Set(Player.Character.CardPool, CardType.Attack, CardRarity.Common);
		((CanvasItem)_turnEndIndicator).Visible = false;
		_healthBar.FadeOutHpLabel(0f, 0f);
		Player.Creature.BlockChanged += BlockChanged;
		Player.Creature.CurrentHpChanged += OnCreatureValueChanged;
		Player.Creature.MaxHpChanged += OnCreatureValueChanged;
		Player.Creature.PowerApplied += OnPowerAppliedOrRemoved;
		Player.Creature.PowerIncreased += OnPowerIncreased;
		Player.Creature.PowerDecreased += OnPowerDecreased;
		Player.Creature.PowerRemoved += OnPowerAppliedOrRemoved;
		Player.Creature.Died += OnCreatureChanged;
		Player.RelicObtained += OnRelicObtained;
		Player.RelicRemoved += OnRelicRemoved;
		Player.PotionProcured += OnPotionProcured;
		Player.PotionDiscarded += OnPotionDiscarded;
		Player.Deck.CardAdded += OnCardObtained;
		Player.Deck.CardRemoved += OnCardRemovedFromDeck;
		CombatManager.Instance.PlayerEndedTurn += RefreshPlayerReadyIndicator;
		CombatManager.Instance.PlayerUnendedTurn += RefreshPlayerReadyIndicator;
		CombatManager.Instance.TurnStarted += OnTurnStarted;
		CombatManager.Instance.CombatSetUp += OnCombatSetUp;
		CombatManager.Instance.CombatEnded += OnCombatEnded;
		RunManager.Instance.FlavorSynchronizer.OnEndTurnPingReceived += OnPlayerEndTurnPing;
		RunManager.Instance.InputSynchronizer.ScreenChanged += OnPlayerScreenChanged;
		RunManager.Instance.MapSelectionSynchronizer.PlayerVoteChanged += OnPlayerVoteChanged;
		RunManager.Instance.MapSelectionSynchronizer.PlayerVoteCancelled += RefreshPlayerReadyIndicator;
		RunManager.Instance.MapSelectionSynchronizer.PlayerVotesCleared += OnPlayerVotesCleared;
		if (RunManager.Instance.RunLobby != null)
		{
			RunManager.Instance.RunLobby.RemotePlayerDisconnected += RefreshConnectedState;
			RunManager.Instance.RunLobby.LocalPlayerDisconnected += RefreshConnectedState;
			RunManager.Instance.RunLobby.PlayerRejoined += RefreshConnectedState;
		}
		((GodotObject)Hitbox).Connect(NClickableControl.SignalName.Focused, Callable.From<NButton>((Action<NButton>)OnFocus), 0u);
		((GodotObject)Hitbox).Connect(NClickableControl.SignalName.Unfocused, Callable.From<NButton>((Action<NButton>)OnUnfocus), 0u);
		((GodotObject)Hitbox).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnRelease), 0u);
		RefreshValues();
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		Player.Creature.BlockChanged -= BlockChanged;
		Player.Creature.CurrentHpChanged -= OnCreatureValueChanged;
		Player.Creature.MaxHpChanged -= OnCreatureValueChanged;
		Player.Creature.PowerApplied -= OnPowerAppliedOrRemoved;
		Player.Creature.PowerIncreased -= OnPowerIncreased;
		Player.Creature.PowerDecreased -= OnPowerDecreased;
		Player.Creature.PowerRemoved -= OnPowerAppliedOrRemoved;
		Player.Creature.Died -= OnCreatureChanged;
		Player.RelicObtained -= OnRelicObtained;
		Player.RelicRemoved -= OnRelicRemoved;
		Player.PotionProcured -= OnPotionProcured;
		Player.PotionDiscarded -= OnPotionDiscarded;
		Player.Deck.CardAdded -= OnCardObtained;
		Player.Deck.CardRemoved -= OnCardRemovedFromDeck;
		CombatManager.Instance.PlayerEndedTurn -= RefreshPlayerReadyIndicator;
		CombatManager.Instance.PlayerUnendedTurn -= RefreshPlayerReadyIndicator;
		CombatManager.Instance.TurnStarted -= OnTurnStarted;
		CombatManager.Instance.CombatSetUp -= OnCombatSetUp;
		CombatManager.Instance.CombatEnded -= OnCombatEnded;
		RunManager.Instance.FlavorSynchronizer.OnEndTurnPingReceived -= OnPlayerEndTurnPing;
		RunManager.Instance.InputSynchronizer.ScreenChanged -= OnPlayerScreenChanged;
		RunManager.Instance.MapSelectionSynchronizer.PlayerVoteChanged -= OnPlayerVoteChanged;
		RunManager.Instance.MapSelectionSynchronizer.PlayerVoteCancelled -= RefreshPlayerReadyIndicator;
		RunManager.Instance.MapSelectionSynchronizer.PlayerVotesCleared -= OnPlayerVotesCleared;
		if (RunManager.Instance.RunLobby != null)
		{
			RunManager.Instance.RunLobby.RemotePlayerDisconnected -= RefreshConnectedState;
			RunManager.Instance.RunLobby.LocalPlayerDisconnected -= RefreshConnectedState;
			RunManager.Instance.RunLobby.PlayerRejoined -= RefreshConnectedState;
		}
	}

	private void OnCombatSetUp(CombatState _)
	{
		if (!LocalContext.IsMe(Player))
		{
			((CanvasItem)_energyContainer).Visible = true;
			Control starContainer = _starContainer;
			int visible;
			if (!(Player.Character is Regent))
			{
				PlayerCombatState? playerCombatState = Player.PlayerCombatState;
				visible = ((playerCombatState != null && playerCombatState.Stars > 0) ? 1 : 0);
			}
			else
			{
				visible = 1;
			}
			((CanvasItem)starContainer).Visible = (byte)visible != 0;
			((CanvasItem)_cardContainer).Visible = true;
			Player.PlayerCombatState.EnergyChanged += OnEnergyChanged;
			Player.PlayerCombatState.StarsChanged += OnStarsChanged;
			Player.PlayerCombatState.Hand.CardAdded += OnCardAdded;
			Player.PlayerCombatState.Hand.CardRemoved += OnCardRemoved;
		}
	}

	private void OnCombatEnded(CombatRoom _)
	{
		((CanvasItem)_turnEndIndicator).Visible = false;
		if (!LocalContext.IsMe(Player))
		{
			((CanvasItem)_energyContainer).Visible = false;
			((CanvasItem)_starContainer).Visible = false;
			((CanvasItem)_cardContainer).Visible = false;
			if (Player.PlayerCombatState != null)
			{
				Player.PlayerCombatState.EnergyChanged -= OnEnergyChanged;
				Player.PlayerCombatState.StarsChanged -= OnStarsChanged;
				Player.PlayerCombatState.Hand.CardAdded -= OnCardAdded;
				Player.PlayerCombatState.Hand.CardRemoved -= OnCardRemoved;
			}
		}
	}

	private void OnCreatureValueChanged(int _, int __)
	{
		RefreshValues();
	}

	private void OnCreatureChanged(Creature _)
	{
		RefreshValues();
	}

	private void OnPowerAppliedOrRemoved(PowerModel _)
	{
		RefreshValues();
	}

	private void OnPowerDecreased(PowerModel _, bool __)
	{
		RefreshValues();
	}

	private void OnPowerIncreased(PowerModel _, int __, bool ___)
	{
		RefreshValues();
	}

	private void RefreshValues()
	{
		UpdateHealthBarWidth();
		_healthBar.RefreshValues();
	}

	private void UpdateHealthBarWidth()
	{
		_healthBar.UpdateWidthRelativeToReferenceValue(80f, 175f);
	}

	private void UpdateSelectionReticleWidth()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		Control val = null;
		foreach (Control item in ((IEnumerable)((Node)_topContainer).GetChildren(false)).OfType<Control>())
		{
			if (((CanvasItem)item).Visible && item.Size.X > 0f)
			{
				val = item;
			}
		}
		float num = val.GlobalPosition.X - ((Control)this).GlobalPosition.X + val.Size.X + 6f;
		float num2 = _healthBar.HpBarContainer.GlobalPosition.X - ((Control)this).GlobalPosition.X + _healthBar.HpBarContainer.Size.X + 6f;
		float num3 = Mathf.Max(num, num2);
		NSelectionReticle selectionReticle = _selectionReticle;
		StringName size = PropertyName.Size;
		Vector2 size2 = ((Control)_selectionReticle).Size;
		size2.X = num3;
		((GodotObject)selectionReticle).SetDeferred(size, Variant.op_Implicit(size2));
		((GodotObject)Hitbox).SetDeferred(PropertyName.Size, Variant.op_Implicit(new Vector2(num3, ((Control)this).Size.Y)));
	}

	private void OnEnergyChanged(int _, int __)
	{
		RefreshCombatValues();
	}

	private void OnStarsChanged(int _, int __)
	{
		RefreshCombatValues();
	}

	private void OnCardAdded(CardModel _)
	{
		RefreshCombatValues();
	}

	private void OnCardRemoved(CardModel _)
	{
		RefreshCombatValues();
	}

	private void RefreshCombatValues()
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		Control starContainer = _starContainer;
		int visible;
		if (!(Player.Character is Regent))
		{
			PlayerCombatState? playerCombatState = Player.PlayerCombatState;
			visible = ((playerCombatState != null && playerCombatState.Stars > 0) ? 1 : 0);
		}
		else
		{
			visible = 1;
		}
		((CanvasItem)starContainer).Visible = (byte)visible != 0;
		_energyCount.SetTextAutoSize(Player.PlayerCombatState.Energy.ToString());
		_starCount.SetTextAutoSize(Player.PlayerCombatState.Stars.ToString());
		_cardCount.SetTextAutoSize(Player.PlayerCombatState.Hand.Cards.Count.ToString());
		((Control)_energyCount).AddThemeColorOverride(ThemeConstants.Label.fontColor, (Player.PlayerCombatState.Energy == 0) ? StsColors.red : StsColors.cream);
		((Control)_energyCount).AddThemeColorOverride(ThemeConstants.Label.fontOutlineColor, (Player.PlayerCombatState.Energy == 0) ? StsColors.unplayableEnergyCostOutline : Player.Character.EnergyLabelOutlineColor);
		Material material = ((Player.PlayerCombatState.Energy == 0) ? PreloadManager.Cache.GetMaterial("res://materials/ui/energy_orb_dark.tres") : null);
		((CanvasItem)_energyImage).Material = material;
		((CanvasItem)_energyImage).Modulate = ((Player.PlayerCombatState.Energy == 0) ? Colors.DarkGray : Colors.White);
	}

	public void OnCreatureHovered()
	{
		_isCreatureHovered = true;
		UpdateHighlightedState();
	}

	public void OnCreatureUnhovered()
	{
		_isCreatureHovered = false;
		UpdateHighlightedState();
	}

	public void FlashPlayerReady()
	{
		FlashEndTurn();
	}

	private void UpdateHighlightedState()
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		bool flag = _isMouseOver || _isCreatureHovered;
		if (NTargetManager.Instance.IsInSelection && !NTargetManager.Instance.AllowedToTargetNode((Node)(object)this))
		{
			flag = false;
		}
		if (!NTargetManager.Instance.IsInSelection)
		{
			NPlayerHand? instance = NPlayerHand.Instance;
			if (instance != null && instance.InCardPlay)
			{
				flag = false;
			}
		}
		if (_isHighlighted == flag)
		{
			return;
		}
		_isHighlighted = flag;
		if (_isHighlighted)
		{
			_healthBar.FadeInHpLabel(0.1f);
			UpdateSelectionReticleWidth();
			_selectionReticle.OnSelect();
			if (_networkProblemIndicator.IsShown)
			{
				LocString locString;
				LocString title;
				if (RunManager.Instance.NetService.Type == NetGameType.Client)
				{
					locString = new LocString("static_hover_tips", "NETWORK_PROBLEM_CLIENT.description");
					title = new LocString("static_hover_tips", "NETWORK_PROBLEM_CLIENT.title");
				}
				else
				{
					locString = new LocString("static_hover_tips", "NETWORK_PROBLEM_HOST.description");
					title = new LocString("static_hover_tips", "NETWORK_PROBLEM_HOST.title");
				}
				locString.Add("Player", PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, Player.NetId));
				((Control)NHoverTipSet.CreateAndShow((Control)(object)this, new HoverTip(title, locString))).GlobalPosition = ((Control)this).GlobalPosition + Vector2.Down * 80f;
			}
		}
		else
		{
			_healthBar.FadeOutHpLabel(0.5f, 0f);
			_selectionReticle.OnDeselect();
			NHoverTipSet.Remove((Control)(object)this);
		}
	}

	private void BlockChanged(int oldBlock, int blockGain)
	{
		if (oldBlock == 0 && blockGain > 0)
		{
			_healthBar.AnimateInBlock(oldBlock, blockGain);
		}
		_healthBar.RefreshValues();
	}

	private void RefreshConnectedState(ulong _)
	{
		RefreshConnectedState();
	}

	private void RefreshConnectedState()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		bool flag = RunManager.Instance.RunLobby.ConnectedPlayerIds.Contains(Player.NetId);
		((CanvasItem)_disconnectedIndicator).Visible = !flag;
		((CanvasItem)_characterIcon).SelfModulate = (flag ? Colors.White : StsColors.gray);
	}

	private void OnRelicObtained(RelicModel relic)
	{
		if (!LocalContext.IsMe(Player))
		{
			TaskHelper.RunSafely(AnimateRelicObtained(relic));
		}
	}

	private async Task AnimateRelicObtained(RelicModel relic)
	{
		await WaitUntilNextTweenTime();
		NRelic relicImage = NRelic.Create(relic, NRelic.IconSize.Small);
		relicImage.Model = relic;
		await ObtainedAnimation((Control)(object)relicImage);
		((Node)(object)relicImage).QueueFreeSafely();
	}

	private void OnRelicRemoved(RelicModel relic)
	{
		if (!LocalContext.IsMe(Player))
		{
			TaskHelper.RunSafely(AnimateRelicRemoved(relic));
		}
	}

	private async Task AnimateRelicRemoved(RelicModel relic)
	{
		await WaitUntilNextTweenTime();
		NRelic relicImage = NRelic.Create(relic, NRelic.IconSize.Small);
		relicImage.Model = relic;
		await RemovedAnimation((Control)(object)relicImage);
		((Node)(object)relicImage).QueueFreeSafely();
	}

	private void OnCardObtained(CardModel card)
	{
		if (!LocalContext.IsMe(Player))
		{
			TaskHelper.RunSafely(AnimateCardObtained(card));
		}
	}

	private async Task AnimateCardObtained(CardModel card)
	{
		await WaitUntilNextTweenTime();
		NDeckHistoryEntry cardNode = NDeckHistoryEntry.Create(card, 1);
		await ObtainedAnimation((Control)(object)cardNode);
		((Node)(object)cardNode).QueueFreeSafely();
	}

	private void OnCardRemovedFromDeck(CardModel card)
	{
		if (!LocalContext.IsMe(Player))
		{
			TaskHelper.RunSafely(AnimateCardRemovedFromDeck(card));
		}
	}

	private async Task AnimateCardRemovedFromDeck(CardModel card)
	{
		await WaitUntilNextTweenTime();
		NDeckHistoryEntry cardNode = NDeckHistoryEntry.Create(card, 1);
		await RemovedAnimation((Control)(object)cardNode);
		((Node)(object)cardNode).QueueFreeSafely();
	}

	private void OnPotionProcured(PotionModel potion)
	{
		if (!LocalContext.IsMe(Player))
		{
			TaskHelper.RunSafely(AnimatePotionObtained(potion));
		}
	}

	private async Task AnimatePotionObtained(PotionModel potion)
	{
		await WaitUntilNextTweenTime();
		NPotion node = NPotion.Create(potion);
		await ObtainedAnimation((Control)(object)node);
		((Node)(object)node).QueueFreeSafely();
	}

	private void OnPotionDiscarded(PotionModel potion)
	{
		if (!LocalContext.IsMe(Player))
		{
			TaskHelper.RunSafely(AnimatePotionDiscarded(potion));
		}
	}

	private async Task AnimatePotionDiscarded(PotionModel potion)
	{
		await WaitUntilNextTweenTime();
		NPotion node = NPotion.Create(potion);
		await RemovedAnimation((Control)(object)node);
		((Node)(object)node).QueueFreeSafely();
	}

	private void OnPlayerVoteChanged(Player player, MapVote? _, MapVote? __)
	{
		RefreshPlayerReadyIndicator(player);
	}

	private void OnPlayerVotesCleared()
	{
		RefreshPlayerReadyIndicator(Player);
	}

	private void RefreshPlayerReadyIndicator(Player player, bool _)
	{
		RefreshPlayerReadyIndicator(player);
	}

	private void RefreshPlayerReadyIndicator(Player player)
	{
		if (CombatManager.Instance.IsInProgress)
		{
			((CanvasItem)_turnEndIndicator).Visible = CombatManager.Instance.IsPlayerReadyToEndTurn(Player);
		}
		else
		{
			((CanvasItem)_turnEndIndicator).Visible = RunManager.Instance.MapSelectionSynchronizer.GetVote(Player).HasValue;
		}
		if (((CanvasItem)_turnEndIndicator).Visible && player == Player)
		{
			FlashEndTurn();
		}
	}

	private void OnPlayerEndTurnPing(ulong playerId)
	{
		if (Player.NetId == playerId)
		{
			FlashEndTurn();
		}
	}

	private void FlashEndTurn()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		NUiFlashVfx nUiFlashVfx = NUiFlashVfx.Create(_turnEndIndicator.Texture, ((CanvasItem)_turnEndIndicator).SelfModulate);
		((Node)(object)_turnEndIndicator).AddChildSafely((Node?)(object)nUiFlashVfx);
		((GodotObject)nUiFlashVfx).SetDeferred(PropertyName.Size, Variant.op_Implicit(((Control)_turnEndIndicator).Size));
		((Control)nUiFlashVfx).Position = Vector2.Zero;
		TaskHelper.RunSafely(nUiFlashVfx.StartVfx());
	}

	private void OnTurnStarted(CombatState _)
	{
		((CanvasItem)_turnEndIndicator).Visible = CombatManager.Instance.IsPlayerReadyToEndTurn(Player);
	}

	private void SetNextTweenTime()
	{
		ulong ticksMsec = Time.GetTicksMsec();
		if (_nextTweenTime > ticksMsec)
		{
			_nextTweenTime += 500uL;
		}
		else
		{
			_nextTweenTime = ticksMsec + 500;
		}
	}

	private async Task WaitUntilNextTweenTime()
	{
		ulong nextTweenTime = _nextTweenTime;
		SetNextTweenTime();
		if (nextTweenTime >= Time.GetTicksMsec())
		{
			double num = (double)(nextTweenTime - Time.GetTicksMsec()) / 1000.0;
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree().CreateTimer(num, true, false, false), SignalName.Timeout);
		}
	}

	private async Task ObtainedAnimation(Control node)
	{
		((Node)(object)this).AddChildSafely((Node?)(object)node);
		node.Position = new Vector2(((Control)this).Size.X + 40f, 0f);
		node.Scale = Vector2.One * 1.1f;
		Tween val = ((Node)node).CreateTween();
		val.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.30000001192092896).SetEase((EaseType)0).SetTrans((TransitionType)9);
		val.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("position:x"), Variant.op_Implicit(((Control)this).Size.X - node.Size.X), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.SetDelay(0.30000001192092896);
		val.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.SetDelay(0.30000001192092896);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
	}

	private async Task RemovedAnimation(Control node)
	{
		((Node)(object)this).AddChildSafely((Node?)(object)node);
		node.Position = new Vector2(((Control)this).Size.X - node.Size.X, 0f);
		Color modulate = ((CanvasItem)node).Modulate;
		modulate.A = 0f;
		((CanvasItem)node).Modulate = modulate;
		Tween val = ((Node)node).CreateTween();
		val.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("position:x"), Variant.op_Implicit(((Control)this).Size.X + 40f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		val.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		val.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("scale:y"), Variant.op_Implicit(0f), 0.30000001192092896).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.SetDelay(0.5);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
	}

	private void OnPlayerScreenChanged(ulong playerId, NetScreenType _)
	{
		if (Player.NetId != playerId || LocalContext.IsMe(Player))
		{
			return;
		}
		Texture2D locationIcon = RunManager.Instance.InputSynchronizer.GetScreenType(playerId).GetLocationIcon();
		if (_currentLocationIcon != locationIcon)
		{
			_currentLocationIcon = locationIcon;
			if (locationIcon == null)
			{
				TweenLocationIconAway();
			}
			else
			{
				TweenLocationIconIn(locationIcon);
			}
		}
	}

	private void TweenLocationIconAway()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		Tween? locationIconTween = _locationIconTween;
		if (locationIconTween != null)
		{
			locationIconTween.Kill();
		}
		_locationIconTween = ((Node)_locationIcon).CreateTween();
		_locationIconTween.TweenProperty((GodotObject)(object)_locationIcon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.Zero), 0.4).SetTrans((TransitionType)10).SetEase((EaseType)0);
		_locationIconTween.TweenCallback(Callable.From<bool>((Func<bool>)(() => ((CanvasItem)_locationContainer).Visible = false)));
	}

	private void TweenLocationIconIn(Texture2D? texture)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		Tween? locationIconTween = _locationIconTween;
		if (locationIconTween != null)
		{
			locationIconTween.Kill();
		}
		_locationIconTween = ((Node)_locationIcon).CreateTween();
		if (!((CanvasItem)_locationContainer).Visible)
		{
			((CanvasItem)_locationContainer).Visible = true;
			_locationIcon.Texture = texture;
			_locationIconTween.TweenProperty((GodotObject)(object)_locationIcon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.4).SetTrans((TransitionType)10).SetEase((EaseType)1);
			return;
		}
		_locationIconTween.TweenProperty((GodotObject)(object)_locationIcon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.5f), 0.2).SetTrans((TransitionType)10).SetEase((EaseType)0);
		_locationIconTween.TweenCallback(Callable.From<Texture2D>((Func<Texture2D>)(() => _locationIcon.Texture = texture)));
		_locationIconTween.TweenProperty((GodotObject)(object)_locationIcon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.3).SetTrans((TransitionType)10).SetEase((EaseType)1);
	}

	protected void OnFocus(NButton _)
	{
		_isMouseOver = true;
		UpdateHighlightedState();
		if (NTargetManager.Instance.IsInSelection && NTargetManager.Instance.AllowedToTargetNode((Node)(object)this))
		{
			NTargetManager.Instance.OnNodeHovered((Node)(object)this);
			_focusedWhileTargeting = true;
		}
	}

	protected void OnUnfocus(NButton _)
	{
		_isMouseOver = false;
		UpdateHighlightedState();
		if (_focusedWhileTargeting)
		{
			NTargetManager.Instance.OnNodeUnhovered((Node)(object)this);
		}
		_focusedWhileTargeting = false;
	}

	protected void OnRelease(NButton _)
	{
		if (!NTargetManager.Instance.IsInSelection && NTargetManager.Instance.LastTargetingFinishedFrame != ((Node)this).GetTree().GetFrame())
		{
			NMultiplayerPlayerExpandedState screen = NMultiplayerPlayerExpandedState.Create(Player);
			NCapstoneContainer.Instance.Open(screen);
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
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Expected O, but got Unknown
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Expected O, but got Unknown
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Expected O, but got Unknown
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Expected O, but got Unknown
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(26);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCreatureValueChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshValues, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateHealthBarWidth, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateSelectionReticleWidth, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnergyChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnStarsChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshCombatValues, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCreatureHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCreatureUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FlashPlayerReady, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateHighlightedState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.BlockChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("oldBlock"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("blockGain"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshConnectedState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshConnectedState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlayerVotesCleared, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlayerEndTurnPing, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FlashEndTurn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetNextTweenTime, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlayerScreenChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TweenLocationIconAway, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TweenLocationIconIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("texture"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnCreatureValueChanged && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			OnCreatureValueChanged(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshValues && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshValues();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateHealthBarWidth && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateHealthBarWidth();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateSelectionReticleWidth && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateSelectionReticleWidth();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEnergyChanged && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			OnEnergyChanged(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnStarsChanged && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			OnStarsChanged(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshCombatValues && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshCombatValues();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCreatureHovered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCreatureHovered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCreatureUnhovered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCreatureUnhovered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FlashPlayerReady && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			FlashPlayerReady();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateHighlightedState && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateHighlightedState();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.BlockChanged && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			BlockChanged(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshConnectedState && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RefreshConnectedState(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshConnectedState && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshConnectedState();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlayerVotesCleared && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPlayerVotesCleared();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlayerEndTurnPing && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPlayerEndTurnPing(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FlashEndTurn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			FlashEndTurn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetNextTweenTime && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetNextTweenTime();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlayerScreenChanged && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			OnPlayerScreenChanged(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<NetScreenType>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TweenLocationIconAway && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TweenLocationIconAway();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TweenLocationIconIn && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			TweenLocationIconIn(VariantUtils.ConvertTo<Texture2D>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnFocus(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnUnfocus(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnRelease(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.OnCreatureValueChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshValues)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateHealthBarWidth)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateSelectionReticleWidth)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEnergyChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnStarsChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshCombatValues)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCreatureHovered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCreatureUnhovered)
		{
			return true;
		}
		if ((ref method) == MethodName.FlashPlayerReady)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateHighlightedState)
		{
			return true;
		}
		if ((ref method) == MethodName.BlockChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshConnectedState)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPlayerVotesCleared)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPlayerEndTurnPing)
		{
			return true;
		}
		if ((ref method) == MethodName.FlashEndTurn)
		{
			return true;
		}
		if ((ref method) == MethodName.SetNextTweenTime)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPlayerScreenChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.TweenLocationIconAway)
		{
			return true;
		}
		if ((ref method) == MethodName.TweenLocationIconIn)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Hitbox)
		{
			Hitbox = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._healthBar)
		{
			_healthBar = VariantUtils.ConvertTo<NHealthBar>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterIcon)
		{
			_characterIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nameplateLabel)
		{
			_nameplateLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._topContainer)
		{
			_topContainer = VariantUtils.ConvertTo<HBoxContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._turnEndIndicator)
		{
			_turnEndIndicator = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._disconnectedIndicator)
		{
			_disconnectedIndicator = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._networkProblemIndicator)
		{
			_networkProblemIndicator = VariantUtils.ConvertTo<NMultiplayerNetworkProblemIndicator>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._locationIcon)
		{
			_locationIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._locationContainer)
		{
			_locationContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._energyContainer)
		{
			_energyContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._energyImage)
		{
			_energyImage = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._energyCount)
		{
			_energyCount = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._starContainer)
		{
			_starContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._starCount)
		{
			_starCount = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardContainer)
		{
			_cardContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardImage)
		{
			_cardImage = VariantUtils.ConvertTo<NTinyCard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardCount)
		{
			_cardCount = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._locationIconTween)
		{
			_locationIconTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isMouseOver)
		{
			_isMouseOver = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isCreatureHovered)
		{
			_isCreatureHovered = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isHighlighted)
		{
			_isHighlighted = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._focusedWhileTargeting)
		{
			_focusedWhileTargeting = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nextTweenTime)
		{
			_nextTweenTime = VariantUtils.ConvertTo<ulong>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentLocationIcon)
		{
			_currentLocationIcon = VariantUtils.ConvertTo<Texture2D>(ref value);
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
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Hitbox)
		{
			NButton hitbox = Hitbox;
			value = VariantUtils.CreateFrom<NButton>(ref hitbox);
			return true;
		}
		if ((ref name) == PropertyName._healthBar)
		{
			value = VariantUtils.CreateFrom<NHealthBar>(ref _healthBar);
			return true;
		}
		if ((ref name) == PropertyName._characterIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _characterIcon);
			return true;
		}
		if ((ref name) == PropertyName._nameplateLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _nameplateLabel);
			return true;
		}
		if ((ref name) == PropertyName._topContainer)
		{
			value = VariantUtils.CreateFrom<HBoxContainer>(ref _topContainer);
			return true;
		}
		if ((ref name) == PropertyName._turnEndIndicator)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _turnEndIndicator);
			return true;
		}
		if ((ref name) == PropertyName._disconnectedIndicator)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _disconnectedIndicator);
			return true;
		}
		if ((ref name) == PropertyName._networkProblemIndicator)
		{
			value = VariantUtils.CreateFrom<NMultiplayerNetworkProblemIndicator>(ref _networkProblemIndicator);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _selectionReticle);
			return true;
		}
		if ((ref name) == PropertyName._locationIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _locationIcon);
			return true;
		}
		if ((ref name) == PropertyName._locationContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _locationContainer);
			return true;
		}
		if ((ref name) == PropertyName._energyContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _energyContainer);
			return true;
		}
		if ((ref name) == PropertyName._energyImage)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _energyImage);
			return true;
		}
		if ((ref name) == PropertyName._energyCount)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _energyCount);
			return true;
		}
		if ((ref name) == PropertyName._starContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _starContainer);
			return true;
		}
		if ((ref name) == PropertyName._starCount)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _starCount);
			return true;
		}
		if ((ref name) == PropertyName._cardContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cardContainer);
			return true;
		}
		if ((ref name) == PropertyName._cardImage)
		{
			value = VariantUtils.CreateFrom<NTinyCard>(ref _cardImage);
			return true;
		}
		if ((ref name) == PropertyName._cardCount)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _cardCount);
			return true;
		}
		if ((ref name) == PropertyName._locationIconTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _locationIconTween);
			return true;
		}
		if ((ref name) == PropertyName._isMouseOver)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isMouseOver);
			return true;
		}
		if ((ref name) == PropertyName._isCreatureHovered)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isCreatureHovered);
			return true;
		}
		if ((ref name) == PropertyName._isHighlighted)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isHighlighted);
			return true;
		}
		if ((ref name) == PropertyName._focusedWhileTargeting)
		{
			value = VariantUtils.CreateFrom<bool>(ref _focusedWhileTargeting);
			return true;
		}
		if ((ref name) == PropertyName._nextTweenTime)
		{
			value = VariantUtils.CreateFrom<ulong>(ref _nextTweenTime);
			return true;
		}
		if ((ref name) == PropertyName._currentLocationIcon)
		{
			value = VariantUtils.CreateFrom<Texture2D>(ref _currentLocationIcon);
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
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._healthBar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._nameplateLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._topContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._turnEndIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._disconnectedIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._networkProblemIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Hitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._locationIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._locationContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._energyContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._energyImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._energyCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._starContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._starCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._locationIconTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isMouseOver, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isCreatureHovered, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isHighlighted, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._focusedWhileTargeting, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._nextTweenTime, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentLocationIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName hitbox = PropertyName.Hitbox;
		NButton hitbox2 = Hitbox;
		info.AddProperty(hitbox, Variant.From<NButton>(ref hitbox2));
		info.AddProperty(PropertyName._healthBar, Variant.From<NHealthBar>(ref _healthBar));
		info.AddProperty(PropertyName._characterIcon, Variant.From<TextureRect>(ref _characterIcon));
		info.AddProperty(PropertyName._nameplateLabel, Variant.From<MegaLabel>(ref _nameplateLabel));
		info.AddProperty(PropertyName._topContainer, Variant.From<HBoxContainer>(ref _topContainer));
		info.AddProperty(PropertyName._turnEndIndicator, Variant.From<TextureRect>(ref _turnEndIndicator));
		info.AddProperty(PropertyName._disconnectedIndicator, Variant.From<TextureRect>(ref _disconnectedIndicator));
		info.AddProperty(PropertyName._networkProblemIndicator, Variant.From<NMultiplayerNetworkProblemIndicator>(ref _networkProblemIndicator));
		info.AddProperty(PropertyName._selectionReticle, Variant.From<NSelectionReticle>(ref _selectionReticle));
		info.AddProperty(PropertyName._locationIcon, Variant.From<TextureRect>(ref _locationIcon));
		info.AddProperty(PropertyName._locationContainer, Variant.From<Control>(ref _locationContainer));
		info.AddProperty(PropertyName._energyContainer, Variant.From<Control>(ref _energyContainer));
		info.AddProperty(PropertyName._energyImage, Variant.From<TextureRect>(ref _energyImage));
		info.AddProperty(PropertyName._energyCount, Variant.From<MegaLabel>(ref _energyCount));
		info.AddProperty(PropertyName._starContainer, Variant.From<Control>(ref _starContainer));
		info.AddProperty(PropertyName._starCount, Variant.From<MegaLabel>(ref _starCount));
		info.AddProperty(PropertyName._cardContainer, Variant.From<Control>(ref _cardContainer));
		info.AddProperty(PropertyName._cardImage, Variant.From<NTinyCard>(ref _cardImage));
		info.AddProperty(PropertyName._cardCount, Variant.From<MegaLabel>(ref _cardCount));
		info.AddProperty(PropertyName._locationIconTween, Variant.From<Tween>(ref _locationIconTween));
		info.AddProperty(PropertyName._isMouseOver, Variant.From<bool>(ref _isMouseOver));
		info.AddProperty(PropertyName._isCreatureHovered, Variant.From<bool>(ref _isCreatureHovered));
		info.AddProperty(PropertyName._isHighlighted, Variant.From<bool>(ref _isHighlighted));
		info.AddProperty(PropertyName._focusedWhileTargeting, Variant.From<bool>(ref _focusedWhileTargeting));
		info.AddProperty(PropertyName._nextTweenTime, Variant.From<ulong>(ref _nextTweenTime));
		info.AddProperty(PropertyName._currentLocationIcon, Variant.From<Texture2D>(ref _currentLocationIcon));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Hitbox, ref val))
		{
			Hitbox = ((Variant)(ref val)).As<NButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._healthBar, ref val2))
		{
			_healthBar = ((Variant)(ref val2)).As<NHealthBar>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterIcon, ref val3))
		{
			_characterIcon = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._nameplateLabel, ref val4))
		{
			_nameplateLabel = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._topContainer, ref val5))
		{
			_topContainer = ((Variant)(ref val5)).As<HBoxContainer>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._turnEndIndicator, ref val6))
		{
			_turnEndIndicator = ((Variant)(ref val6)).As<TextureRect>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._disconnectedIndicator, ref val7))
		{
			_disconnectedIndicator = ((Variant)(ref val7)).As<TextureRect>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._networkProblemIndicator, ref val8))
		{
			_networkProblemIndicator = ((Variant)(ref val8)).As<NMultiplayerNetworkProblemIndicator>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionReticle, ref val9))
		{
			_selectionReticle = ((Variant)(ref val9)).As<NSelectionReticle>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._locationIcon, ref val10))
		{
			_locationIcon = ((Variant)(ref val10)).As<TextureRect>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._locationContainer, ref val11))
		{
			_locationContainer = ((Variant)(ref val11)).As<Control>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._energyContainer, ref val12))
		{
			_energyContainer = ((Variant)(ref val12)).As<Control>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._energyImage, ref val13))
		{
			_energyImage = ((Variant)(ref val13)).As<TextureRect>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._energyCount, ref val14))
		{
			_energyCount = ((Variant)(ref val14)).As<MegaLabel>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._starContainer, ref val15))
		{
			_starContainer = ((Variant)(ref val15)).As<Control>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._starCount, ref val16))
		{
			_starCount = ((Variant)(ref val16)).As<MegaLabel>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardContainer, ref val17))
		{
			_cardContainer = ((Variant)(ref val17)).As<Control>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardImage, ref val18))
		{
			_cardImage = ((Variant)(ref val18)).As<NTinyCard>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardCount, ref val19))
		{
			_cardCount = ((Variant)(ref val19)).As<MegaLabel>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._locationIconTween, ref val20))
		{
			_locationIconTween = ((Variant)(ref val20)).As<Tween>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._isMouseOver, ref val21))
		{
			_isMouseOver = ((Variant)(ref val21)).As<bool>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._isCreatureHovered, ref val22))
		{
			_isCreatureHovered = ((Variant)(ref val22)).As<bool>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._isHighlighted, ref val23))
		{
			_isHighlighted = ((Variant)(ref val23)).As<bool>();
		}
		Variant val24 = default(Variant);
		if (info.TryGetProperty(PropertyName._focusedWhileTargeting, ref val24))
		{
			_focusedWhileTargeting = ((Variant)(ref val24)).As<bool>();
		}
		Variant val25 = default(Variant);
		if (info.TryGetProperty(PropertyName._nextTweenTime, ref val25))
		{
			_nextTweenTime = ((Variant)(ref val25)).As<ulong>();
		}
		Variant val26 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentLocationIcon, ref val26))
		{
			_currentLocationIcon = ((Variant)(ref val26)).As<Texture2D>();
		}
	}
}
