using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;

[ScriptPath("res://src/Core/Nodes/Screens/GameOverScreen/NGameOverScreen.cs")]
public class NGameOverScreen : NClickableControl, IOverlayScreen, IScreenContext
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName DiscoveredAnyEpochs = StringName.op_Implicit("DiscoveredAnyEpochs");

		public static readonly StringName InitializeBannerAndQuote = StringName.op_Implicit("InitializeBannerAndQuote");

		public static readonly StringName OpenSummaryScreen = StringName.op_Implicit("OpenSummaryScreen");

		public static readonly StringName AddBadge = StringName.op_Implicit("AddBadge");

		public static readonly StringName PlayUnlockSfx = StringName.op_Implicit("PlayUnlockSfx");

		public static readonly StringName TweenScore = StringName.op_Implicit("TweenScore");

		public static readonly StringName GetScoreThreshold = StringName.op_Implicit("GetScoreThreshold");

		public static readonly StringName ShowLeaderboard = StringName.op_Implicit("ShowLeaderboard");

		public static readonly StringName HideSummary = StringName.op_Implicit("HideSummary");

		public static readonly StringName OpenRunHistoryScreen = StringName.op_Implicit("OpenRunHistoryScreen");

		public static readonly StringName OnMainMenuButtonPressed = StringName.op_Implicit("OnMainMenuButtonPressed");

		public static readonly StringName OpenTimeline = StringName.op_Implicit("OpenTimeline");

		public static readonly StringName ReturnToMainMenu = StringName.op_Implicit("ReturnToMainMenu");

		public static readonly StringName AfterOverlayOpened = StringName.op_Implicit("AfterOverlayOpened");

		public static readonly StringName MoveCreaturesToDifferentLayerAndDisableUi = StringName.op_Implicit("MoveCreaturesToDifferentLayerAndDisableUi");

		public static readonly StringName UpdateBackstopMaterial = StringName.op_Implicit("UpdateBackstopMaterial");

		public static readonly StringName AfterOverlayClosed = StringName.op_Implicit("AfterOverlayClosed");

		public static readonly StringName AfterOverlayShown = StringName.op_Implicit("AfterOverlayShown");

		public static readonly StringName AfterOverlayHidden = StringName.op_Implicit("AfterOverlayHidden");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public static readonly StringName UseSharedBackstop = StringName.op_Implicit("UseSharedBackstop");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _continueButton = StringName.op_Implicit("_continueButton");

		public static readonly StringName _viewRunButton = StringName.op_Implicit("_viewRunButton");

		public static readonly StringName _mainMenuButton = StringName.op_Implicit("_mainMenuButton");

		public static readonly StringName _leaderboardButton = StringName.op_Implicit("_leaderboardButton");

		public static readonly StringName _badgeContainer = StringName.op_Implicit("_badgeContainer");

		public static readonly StringName _scoreBar = StringName.op_Implicit("_scoreBar");

		public static readonly StringName _scoreFg = StringName.op_Implicit("_scoreFg");

		public static readonly StringName _scoreProgress = StringName.op_Implicit("_scoreProgress");

		public static readonly StringName _unlocksRemaining = StringName.op_Implicit("_unlocksRemaining");

		public static readonly StringName _score = StringName.op_Implicit("_score");

		public static readonly StringName _scoreThreshold = StringName.op_Implicit("_scoreThreshold");

		public static readonly StringName _scoreUnlockedEpochId = StringName.op_Implicit("_scoreUnlockedEpochId");

		public static readonly StringName _leaderboard = StringName.op_Implicit("_leaderboard");

		public static readonly StringName _creatureContainer = StringName.op_Implicit("_creatureContainer");

		public static readonly StringName _summaryContainer = StringName.op_Implicit("_summaryContainer");

		public static readonly StringName _fullBlackBackstop = StringName.op_Implicit("_fullBlackBackstop");

		public static readonly StringName _summaryBackstop = StringName.op_Implicit("_summaryBackstop");

		public static readonly StringName _backstop = StringName.op_Implicit("_backstop");

		public static readonly StringName _banner = StringName.op_Implicit("_banner");

		public static readonly StringName _deathQuote = StringName.op_Implicit("_deathQuote");

		public static readonly StringName _victoryDamageLabel = StringName.op_Implicit("_victoryDamageLabel");

		public static readonly StringName _uiNode = StringName.op_Implicit("_uiNode");

		public static readonly StringName _screenshakeContainer = StringName.op_Implicit("_screenshakeContainer");

		public static readonly StringName _discoveryLabel = StringName.op_Implicit("_discoveryLabel");

		public static readonly StringName _encounterQuote = StringName.op_Implicit("_encounterQuote");

		public static readonly StringName _isAnimatingSummary = StringName.op_Implicit("_isAnimatingSummary");

		public static readonly StringName _backstopMaterial = StringName.op_Implicit("_backstopMaterial");

		public static readonly StringName _quoteTween = StringName.op_Implicit("_quoteTween");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private static readonly StringName _threshold = new StringName("threshold");

	private RunState _runState;

	private SerializableRun _serializableRun;

	private RunHistory _history;

	private Player? _localPlayer;

	private NGameOverContinueButton _continueButton;

	private NViewRunButton _viewRunButton;

	private NReturnToMainMenuButton _mainMenuButton;

	private NGameOverContinueButton _leaderboardButton;

	private GridContainer _badgeContainer;

	private readonly List<NBadge> _badges = new List<NBadge>();

	private Control _scoreBar;

	private Control _scoreFg;

	private MegaLabel _scoreProgress;

	private MegaLabel _unlocksRemaining;

	private int _score;

	private int _scoreThreshold;

	private string? _scoreUnlockedEpochId;

	private NDailyRunLeaderboard _leaderboard;

	private Control _creatureContainer;

	private NRunSummary _summaryContainer;

	private ColorRect _fullBlackBackstop;

	private ColorRect _summaryBackstop;

	private ColorRect _backstop;

	private NCommonBanner _banner;

	private MegaRichTextLabel _deathQuote;

	private MegaRichTextLabel _victoryDamageLabel;

	private Control _uiNode;

	private Control _screenshakeContainer;

	private MegaLabel _discoveryLabel;

	private string _encounterQuote;

	private bool _isAnimatingSummary;

	private ShaderMaterial _backstopMaterial;

	private Tween? _quoteTween;

	private static string ScenePath => SceneHelper.GetScenePath("screens/game_over_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public NetScreenType ScreenType => NetScreenType.GameOver;

	public bool UseSharedBackstop => false;

	public Control DefaultFocusedControl => (Control)(object)this;

	public override void _Ready()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Expected O, but got Unknown
		bool win = _runState.CurrentRoom?.IsVictoryRoom ?? false;
		_history = RunManager.Instance.History ?? new RunHistory
		{
			Win = win
		};
		_score = ScoreUtility.CalculateScore(_serializableRun, _history.Win);
		_uiNode = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Ui"));
		_continueButton = ((Node)this).GetNode<NGameOverContinueButton>(NodePath.op_Implicit("%ContinueButton"));
		((GodotObject)_continueButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenSummaryScreen), 0u);
		_continueButton.Disable();
		_viewRunButton = ((Node)this).GetNode<NViewRunButton>(NodePath.op_Implicit("%ViewRunButton"));
		((GodotObject)_viewRunButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenRunHistoryScreen), 0u);
		_mainMenuButton = ((Node)this).GetNode<NReturnToMainMenuButton>(NodePath.op_Implicit("%MainMenuButton"));
		((GodotObject)_mainMenuButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnMainMenuButtonPressed), 0u);
		_badgeContainer = ((Node)this).GetNode<GridContainer>(NodePath.op_Implicit("%BadgeContainer"));
		_scoreBar = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ScoreBar"));
		_scoreFg = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ScoreFg"));
		_scoreProgress = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ScoreProgress"));
		_unlocksRemaining = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%UnlocksRemaining"));
		_screenshakeContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ScreenshakeContainer"));
		_leaderboardButton = ((Node)this).GetNode<NGameOverContinueButton>(NodePath.op_Implicit("%LeaderboardButton"));
		((GodotObject)_leaderboardButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ShowLeaderboard), 0u);
		_creatureContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CreatureContainer"));
		_summaryContainer = ((Node)this).GetNode<NRunSummary>(NodePath.op_Implicit("%RunSummaryContainer"));
		_backstop = ((Node)this).GetNode<ColorRect>(NodePath.op_Implicit("%Backstop"));
		_fullBlackBackstop = ((Node)this).GetNode<ColorRect>(NodePath.op_Implicit("%FullBlackBackstop"));
		_backstopMaterial = (ShaderMaterial)((CanvasItem)_backstop).Material;
		_summaryBackstop = ((Node)this).GetNode<ColorRect>(NodePath.op_Implicit("%SummaryBackstop"));
		_leaderboard = ((Node)this).GetNode<NDailyRunLeaderboard>(NodePath.op_Implicit("%DailyRunLeaderboard"));
		_banner = ((Node)this).GetNode<NCommonBanner>(NodePath.op_Implicit("%Banner"));
		_victoryDamageLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%VictoryDamageLabel"));
		_discoveryLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%DiscoveryLabel"));
		_discoveryLabel.SetTextAutoSize(new LocString("game_over_screen", "DISCOVERY_HEADER").GetFormattedText());
		_deathQuote = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%DeathQuoteLabel"));
		InitializeBannerAndQuote();
		ActiveScreenContext.Instance.Update();
		_leaderboardButton.Disable();
		_viewRunButton.Disable();
		_mainMenuButton.Disable();
		((CanvasItem)_leaderboard).Visible = false;
	}

	private bool DiscoveredAnyEpochs()
	{
		return _localPlayer.DiscoveredEpochs.Count > 0;
	}

	private void InitializeBannerAndQuote()
	{
		ModelId id = _localPlayer.Character.Id;
		if (_history.Win)
		{
			_banner.label.SetTextAutoSize(new LocString("game_over_screen", "BANNER.falseWin").GetFormattedText());
			_deathQuote.Text = string.Empty;
			long personalArchitectDamage = StatsManager.GetPersonalArchitectDamage();
			long? globalArchitectDamage = StatsManager.GetGlobalArchitectDamage();
			StringBuilder stringBuilder = new StringBuilder();
			if (globalArchitectDamage.HasValue)
			{
				LocString locString = new LocString("game_over_screen", "VICTORY_DAMAGE");
				locString.Add("PlayerDamage", _score);
				locString.Add("PersonalDamage", personalArchitectDamage.ToString("N0"));
				locString.Add("TotalDamage", globalArchitectDamage.Value.ToString("N0"));
				stringBuilder.Append(locString.GetFormattedText());
			}
			else
			{
				LocString locString2 = new LocString("game_over_screen", "VICTORY_DAMAGE_LOCAL");
				locString2.Add("PlayerDamage", _score);
				locString2.Add("PersonalDamage", personalArchitectDamage.ToString("N0"));
				stringBuilder.Append(locString2.GetFormattedText());
			}
			int ascensionLevel = _runState.AscensionLevel;
			if (ascensionLevel < 10 && ascensionLevel > 0 && _runState.AscensionLevel >= _localPlayer.MaxAscensionWhenRunStarted)
			{
				stringBuilder.Append("\n\n");
				LocString locString3 = new LocString("game_over_screen", "VICTORY_UNLOCKED_ASCENSION");
				locString3.Add("AscensionLevel", _runState.AscensionLevel + 1);
				stringBuilder.Append(locString3.GetFormattedText());
			}
			_victoryDamageLabel.Text = stringBuilder.ToString();
		}
		else
		{
			LocTable table = LocManager.Instance.GetTable("game_over_screen");
			IReadOnlyList<LocString> locStringsWithPrefix = table.GetLocStringsWithPrefix("BANNER.lose");
			_banner.label.SetTextAutoSize(Rng.Chaotic.NextItem(locStringsWithPrefix).GetFormattedText());
			IReadOnlyList<LocString> locStringsWithPrefix2 = table.GetLocStringsWithPrefix("QUOTES");
			_deathQuote.Text = Rng.Chaotic.NextItem(locStringsWithPrefix2).GetFormattedText();
		}
		_encounterQuote = NRunHistory.GetDeathQuote(_history, id, NRunHistory.GetGameOverType(_history));
	}

	private async Task AnimateInQuote()
	{
		if (((CanvasItem)_deathQuote).Modulate.A != 0f)
		{
			Tween? quoteTween = _quoteTween;
			if (quoteTween != null)
			{
				quoteTween.Kill();
			}
			_quoteTween = ((Node)this).CreateTween();
			_quoteTween.TweenProperty((GodotObject)(object)_deathQuote, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
			await ((GodotObject)this).ToSignal((GodotObject)(object)_quoteTween, SignalName.Finished);
			if (!((Node?)(object)this).IsValid())
			{
				return;
			}
			_deathQuote.Text = _encounterQuote;
			_quoteTween.Kill();
			await Cmd.Wait(1f);
		}
		if (((Node?)(object)this).IsValid())
		{
			Tween? quoteTween2 = _quoteTween;
			if (quoteTween2 != null)
			{
				quoteTween2.Kill();
			}
			_quoteTween = ((Node)this).CreateTween().SetParallel(true);
			if (_history.Win)
			{
				_quoteTween.TweenProperty((GodotObject)(object)_victoryDamageLabel, NodePath.op_Implicit("visible_ratio"), Variant.op_Implicit(1f), 2.0).SetEase((EaseType)1).SetTrans((TransitionType)1);
				_quoteTween.TweenProperty((GodotObject)(object)_victoryDamageLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 2.0);
				await ((GodotObject)this).ToSignal((GodotObject)(object)_quoteTween, SignalName.Finished);
			}
			else
			{
				_quoteTween.TweenProperty((GodotObject)(object)_deathQuote, NodePath.op_Implicit("position:y"), Variant.op_Implicit(156f), 2.0).SetEase((EaseType)1).SetTrans((TransitionType)5)
					.From(Variant.op_Implicit(90f));
				_quoteTween.TweenProperty((GodotObject)(object)_deathQuote, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.5);
			}
		}
	}

	public static NGameOverScreen? Create(RunState runState, SerializableRun serializableRun)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NGameOverScreen nGameOverScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NGameOverScreen>((GenEditState)0);
		nGameOverScreen._runState = runState;
		nGameOverScreen._serializableRun = serializableRun;
		nGameOverScreen._localPlayer = LocalContext.GetMe(runState);
		return nGameOverScreen;
	}

	private void OpenSummaryScreen(NButton _)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		_isAnimatingSummary = true;
		_continueButton.Disable();
		((CanvasItem)_victoryDamageLabel).Visible = false;
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_summaryBackstop, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.5);
		TaskHelper.RunSafely(AnimateInQuote());
		TaskHelper.RunSafely(AnimateRunSummary());
	}

	private async Task AnimateRunSummary()
	{
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_banner, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)_banner).Position.Y - 32f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		((CanvasItem)_summaryContainer).Visible = true;
		await AnimateBadges();
		await AnimateScoreBar();
		await AnimateDiscoveries();
		if (_history.GameMode == GameMode.Daily)
		{
			_leaderboard.Initialize(RunManager.Instance.DailyTime.Value, _runState.Players.Select((Player p) => p.NetId), allowPagination: false);
			((CanvasItem)_leaderboardButton).Visible = true;
			_leaderboardButton.Enable();
			return;
		}
		if (DiscoveredAnyEpochs())
		{
			_mainMenuButton.SetLabelForUnlock();
		}
		((CanvasItem)_mainMenuButton).Visible = true;
		_mainMenuButton.Enable();
	}

	private async Task AnimateBadges()
	{
		_badges.Clear();
		AddBadge("BADGE.floorsClimbed", "FloorCount", _runState.TotalFloor, "res://images/atlases/ui_atlas.sprites/top_bar/top_bar_floor.tres");
		List<MapPointRoomHistoryEntry> list = _serializableRun.MapPointHistory.SelectMany((List<MapPointHistoryEntry> actEntries) => actEntries).SelectMany((MapPointHistoryEntry e) => e.Rooms).ToList();
		int num = list.Count((MapPointRoomHistoryEntry r) => r.RoomType == RoomType.Elite);
		if (list.Count > 0 && list.Last().RoomType == RoomType.Elite)
		{
			num--;
		}
		AddBadge("BADGE.elitesKilled", "EliteCount", num);
		int amount = _serializableRun.MapPointHistory.SelectMany((List<MapPointHistoryEntry> actEntries) => actEntries).Sum((MapPointHistoryEntry e) => e.GetEntry(_localPlayer.NetId).GoldGained);
		AddBadge("BADGE.goldGained", "GoldAmount", amount);
		if (_localPlayer.Relics.Count >= 25)
		{
			AddBadge("BADGE.iLikeShiny", "RelicCount", _localPlayer.Relics.Count);
		}
		int gold = _localPlayer.Gold;
		if (gold >= 3000)
		{
			AddBadge("BADGE.goldenGod", "GoldAmount", gold);
		}
		else if (gold >= 2000)
		{
			AddBadge("BADGE.scrooge", "GoldAmount", gold);
		}
		else if (gold >= 1000)
		{
			AddBadge("BADGE.miser", "GoldAmount", gold);
		}
		if (_history.Win)
		{
			int count = _localPlayer.Deck.Cards.Count;
			if (count <= 10)
			{
				AddBadge("BADGE.tinyDeck", "DeckSize", count);
			}
			else if (count <= 20)
			{
				AddBadge("BADGE.smallDeck", "DeckSize", count);
			}
			else if (count >= 60)
			{
				AddBadge("BADGE.hugeDeck", "DeckSize", count);
			}
			else if (count >= 40)
			{
				AddBadge("BADGE.bigDeck", "DeckSize", count);
			}
			int startingHp = _localPlayer.Character.StartingHp;
			int maxHp = _localPlayer.Creature.MaxHp;
			int num2 = maxHp - startingHp;
			if ((float)maxHp / (float)startingHp < 0.50001f)
			{
				AddBadge("BADGE.famished", "HpDiff", num2);
			}
			else if (num2 >= 50)
			{
				AddBadge("BADGE.glutton", "HpDiff", num2);
			}
			else if (num2 >= 30)
			{
				AddBadge("BADGE.stuffed", "HpDiff", num2);
			}
			else if (num2 >= 15)
			{
				AddBadge("BADGE.wellFed", "HpDiff", num2);
			}
		}
		_badgeContainer.Columns = ((_badges.Count < 6) ? 1 : 2);
		await Cmd.Wait(0.5f);
		foreach (NBadge badge in _badges)
		{
			await badge.AnimateIn();
		}
	}

	private void AddBadge(string locEntryKey, string? locAmountKey = null, int amount = 0, string? iconPath = null)
	{
		LocString locString = new LocString("game_over_screen", locEntryKey);
		if (locAmountKey != null)
		{
			locString.Add(locAmountKey, amount);
		}
		Texture2D icon = ((iconPath == null) ? null : PreloadManager.Cache.GetTexture2D(iconPath));
		NBadge nBadge = NBadge.Create(locString.GetFormattedText(), icon);
		((Node)(object)_badgeContainer).AddChildSafely((Node?)(object)nBadge);
		_badges.Add(nBadge);
	}

	private async Task AnimateScoreBar()
	{
		int unlocksRemaining = SaveManager.Instance.GetUnlocksRemaining();
		LocString locString = new LocString("game_over_screen", "SCORE.unlocksRemaining");
		locString.Add("UnlockCount", unlocksRemaining);
		_unlocksRemaining.SetTextAutoSize(locString.GetFormattedText());
		if (unlocksRemaining > 0)
		{
			int currentScore = SaveManager.Instance.GetCurrentScore();
			_scoreThreshold = GetScoreThreshold(unlocksRemaining);
			_scoreProgress.SetTextAutoSize($"[{currentScore}/{_scoreThreshold}]");
			_scoreFg.Scale = new Vector2((float)currentScore / (float)_scoreThreshold, 1f);
			Tween scoreTween = ((Node)this).CreateTween();
			scoreTween.TweenProperty((GodotObject)(object)_scoreBar, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.3);
			await ((GodotObject)scoreTween).ToSignal((GodotObject)(object)scoreTween, SignalName.Finished);
			if (currentScore + _score >= _scoreThreshold)
			{
				Log.Info("New Unlock, yay!");
				MegaLabel node = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%UnlockText"));
				_scoreUnlockedEpochId = SaveManager.Instance.IncrementUnlock();
				currentScore -= _scoreThreshold;
				int newThreshold = GetScoreThreshold(unlocksRemaining - 1);
				string locEntryKey = ((newThreshold == 0) ? "SCORE.unlockedAllMessage" : "SCORE.unlockedEpochMessage");
				node.SetTextAutoSize(new LocString("game_over_screen", locEntryKey).GetFormattedText());
				scoreTween = ((Node)this).CreateTween().SetParallel(true);
				scoreTween.TweenInterval(1.0);
				scoreTween.Chain();
				scoreTween.TweenMethod(Callable.From<int>((Action<int>)TweenScore), Variant.op_Implicit(currentScore + _scoreThreshold), Variant.op_Implicit(_scoreThreshold), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
				scoreTween.TweenProperty((GodotObject)(object)_scoreFg, NodePath.op_Implicit("scale:x"), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
				scoreTween.Chain();
				scoreTween.TweenCallback(Callable.From((Action)PlayUnlockSfx));
				scoreTween.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25);
				scoreTween.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("position:y"), Variant.op_Implicit(-60f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)11);
				await ((GodotObject)scoreTween).ToSignal((GodotObject)(object)scoreTween, SignalName.Finished);
				if (_scoreUnlockedEpochId != null && !SaveManager.Instance.IsEpochRevealed(_scoreUnlockedEpochId))
				{
					EpochModel epochModel = EpochModel.Get(_scoreUnlockedEpochId);
					SaveManager.Instance.ObtainEpoch(_scoreUnlockedEpochId);
					((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NGainEpochVfx.Create(epochModel));
					_localPlayer.DiscoveredEpochs.Add(epochModel.Id);
					LocalContext.GetMe(_serializableRun).DiscoveredEpochs.Add(epochModel.Id);
				}
				LocString locString2 = new LocString("game_over_screen", "SCORE.unlocksRemaining");
				locString2.Add("UnlockCount", unlocksRemaining - 1);
				_unlocksRemaining.SetTextAutoSize(locString2.GetFormattedText());
				_scoreThreshold = newThreshold;
				currentScore += _score;
				if (newThreshold == 0 || currentScore == 0)
				{
					Log.Info("Player has gotten all unlocks or they've overflowed exactly 0");
					SaveManager.Instance.Progress.CurrentScore = 0;
				}
				else if (currentScore >= newThreshold)
				{
					Log.Info("Score is too awesome. Disallow double unlock.");
					scoreTween.Kill();
					scoreTween = ((Node)this).CreateTween().SetParallel(true);
					scoreTween.TweenInterval(0.5);
					scoreTween.Chain();
					scoreTween.TweenMethod(Callable.From<int>((Action<int>)TweenScore), Variant.op_Implicit(0), Variant.op_Implicit(newThreshold * 99 / 100), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
					scoreTween.TweenProperty((GodotObject)(object)_scoreFg, NodePath.op_Implicit("scale:x"), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7)
						.From(Variant.op_Implicit(0f));
					await ((GodotObject)scoreTween).ToSignal((GodotObject)(object)scoreTween, SignalName.Finished);
					SaveManager.Instance.Progress.CurrentScore = newThreshold - 1;
				}
				else
				{
					Log.Info("Animate overflow score.");
					scoreTween.Kill();
					scoreTween = ((Node)this).CreateTween().SetParallel(true);
					scoreTween.Chain();
					scoreTween.TweenInterval(0.5);
					scoreTween.Chain();
					scoreTween.TweenMethod(Callable.From<int>((Action<int>)TweenScore), Variant.op_Implicit(0), Variant.op_Implicit(currentScore), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
					scoreTween.TweenProperty((GodotObject)(object)_scoreFg, NodePath.op_Implicit("scale:x"), Variant.op_Implicit((float)currentScore / (float)newThreshold), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7)
						.From(Variant.op_Implicit(0f));
					await ((GodotObject)scoreTween).ToSignal((GodotObject)(object)scoreTween, SignalName.Finished);
					SaveManager.Instance.Progress.CurrentScore = currentScore;
				}
			}
			else
			{
				Log.Info("Not enough score to level up");
				scoreTween = ((Node)this).CreateTween().SetParallel(true);
				scoreTween.TweenInterval(0.5);
				scoreTween.TweenMethod(Callable.From<int>((Action<int>)TweenScore), Variant.op_Implicit(currentScore), Variant.op_Implicit(currentScore + _score), 1.0);
				scoreTween.TweenProperty((GodotObject)(object)_scoreFg, NodePath.op_Implicit("scale:x"), Variant.op_Implicit((float)(currentScore + _score) / (float)_scoreThreshold), 1.0);
				SaveManager.Instance.Progress.CurrentScore += _score;
			}
			SaveManager.Instance.SaveProgressFile();
		}
		else
		{
			Log.Info("This player has all unlocks. No action");
		}
	}

	private void PlayUnlockSfx()
	{
		Log.Info("TODO: Play the ding unlock sfx here pls");
	}

	private void TweenScore(int value)
	{
		_scoreProgress.SetTextAutoSize($"[{value}/{_scoreThreshold}]");
	}

	private int GetScoreThreshold(int unlocksRemaining)
	{
		return (18 - unlocksRemaining) switch
		{
			0 => 200, 
			1 => 500, 
			2 => 750, 
			3 => 1000, 
			4 => 1250, 
			5 => 1500, 
			6 => 1600, 
			7 => 1700, 
			8 => 1800, 
			9 => 1900, 
			10 => 2000, 
			11 => 2100, 
			12 => 2200, 
			13 => 2300, 
			14 => 2400, 
			15 => 2500, 
			16 => 2500, 
			17 => 2500, 
			_ => 0, 
		};
	}

	private void ShowLeaderboard(NButton _)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		Tween val = ((Node)this).CreateTween().SetParallel(true);
		NDailyRunLeaderboard leaderboard = _leaderboard;
		Color modulate = ((CanvasItem)_leaderboard).Modulate;
		modulate.A = 0f;
		((CanvasItem)leaderboard).Modulate = modulate;
		val.TweenProperty((GodotObject)(object)_leaderboard, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5);
		val.TweenProperty((GodotObject)(object)_summaryContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
		val.TweenProperty((GodotObject)(object)_deathQuote, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
		val.Chain().TweenCallback(Callable.From((Action)HideSummary));
		((CanvasItem)_leaderboard).Visible = true;
		_leaderboardButton.Disable();
		if (DiscoveredAnyEpochs())
		{
			_mainMenuButton.SetLabelForUnlock();
		}
		((CanvasItem)_mainMenuButton).Visible = true;
		_mainMenuButton.Enable();
	}

	private void HideSummary()
	{
		((CanvasItem)_summaryContainer).Visible = false;
		((CanvasItem)_deathQuote).Visible = false;
	}

	private async Task AnimateDiscoveries()
	{
		await _summaryContainer.AnimateInDiscoveries(_runState);
		_isAnimatingSummary = false;
	}

	private void OpenRunHistoryScreen(NButton _)
	{
		Control child = ResourceLoader.Load<PackedScene>("res://scenes/screens/run_history_screen/run_history_screen_via_game_over_screen.tscn", (string)null, (CacheMode)1).Instantiate<Control>((GenEditState)0);
		((Node)(object)this).AddChildSafely((Node?)(object)child);
	}

	private void OnMainMenuButtonPressed(NButton _)
	{
		if (RunManager.Instance.NetService.Type == NetGameType.Host)
		{
			RunManager.Instance.NetService.Disconnect(NetError.QuitGameOver);
		}
		_mainMenuButton.Disable();
		if (DiscoveredAnyEpochs())
		{
			OpenTimeline();
		}
		else
		{
			ReturnToMainMenu();
		}
	}

	private void OpenTimeline()
	{
		TaskHelper.RunSafely(TransitionOutToTimeline());
	}

	private void ReturnToMainMenu()
	{
		TaskHelper.RunSafely(TransitionOutToMainMenu());
	}

	private async Task TransitionOutToTimeline()
	{
		await NGame.Instance.GoToTimelineAfterRun();
	}

	private async Task TransitionOutToMainMenu()
	{
		await NGame.Instance.ReturnToMainMenuAfterRun();
	}

	public void AfterOverlayOpened()
	{
		MoveCreaturesToDifferentLayerAndDisableUi();
		TaskHelper.RunSafely(AnimateIn());
	}

	private void MoveCreaturesToDifferentLayerAndDisableUi()
	{
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		List<NCreatureVisuals> list = new List<NCreatureVisuals>();
		List<NCreature> list2;
		if (NCombatRoom.Instance != null)
		{
			if (NCombatRoom.Instance.Mode == CombatRoomMode.ActiveCombat)
			{
				NCombatRoom.Instance.Ui.AnimOut((CombatRoom)_runState.CurrentRoom);
			}
			list2 = NCombatRoom.Instance.CreatureNodes.ToList();
			list = list2.Select((NCreature c) => c.Visuals).ToList();
		}
		else if (NMerchantRoom.Instance != null)
		{
			list2 = new List<NCreature>();
			foreach (NMerchantCharacter playerVisual in NMerchantRoom.Instance.PlayerVisuals)
			{
				playerVisual.PlayAnimation("die");
				((Node)playerVisual).Reparent((Node)(object)_creatureContainer, true);
			}
		}
		else if (NRestSiteRoom.Instance != null)
		{
			list2 = new List<NCreature>();
			list = new List<NCreatureVisuals>();
			Vector2 val = default(Vector2);
			foreach (Player player in _runState.Players)
			{
				NCreatureVisuals nCreatureVisuals = player.Creature.CreateVisuals();
				list.Add(nCreatureVisuals);
				((Node)(object)_creatureContainer).AddChildSafely((Node?)(object)nCreatureVisuals);
				nCreatureVisuals.SpineBody.GetAnimationState().SetAnimation("die", loop: false);
				NRestSiteCharacter characterForPlayer = NRestSiteRoom.Instance.GetCharacterForPlayer(player);
				((Node2D)nCreatureVisuals).GlobalPosition = ((Node2D)characterForPlayer).GlobalPosition;
				((Node2D)nCreatureVisuals).Scale = ((Node2D)characterForPlayer).Scale;
				((CanvasItem)characterForPlayer).Visible = false;
				((Vector2)(ref val))._002Ector(100f, 100f);
				((Node2D)nCreatureVisuals).Position = ((Node2D)nCreatureVisuals).Position + val * new Vector2((float)Math.Sign(((Node2D)nCreatureVisuals).Scale.X), (float)Math.Sign(((Node2D)nCreatureVisuals).Scale.Y));
			}
		}
		else
		{
			list2 = new List<NCreature>();
			list = new List<NCreatureVisuals>();
			foreach (Player player2 in _runState.Players)
			{
				NCreatureVisuals nCreatureVisuals2 = player2.Creature.CreateVisuals();
				list.Add(nCreatureVisuals2);
				((Node)(object)_creatureContainer).AddChildSafely((Node?)(object)nCreatureVisuals2);
				nCreatureVisuals2.SpineBody.GetAnimationState().SetAnimation("die", loop: false);
			}
			float num = Math.Min(250f, (((Control)this).Size.X - 200f) / (float)(list.Count - 1));
			float num2 = (float)(list.Count - 1) * (0f - num) * 0.5f;
			foreach (NCreatureVisuals item in list)
			{
				((Node2D)item).Position = _creatureContainer.Size * 0.5f + new Vector2(num2, 200f);
				num2 += num;
			}
		}
		list2.Sort((NCreature c1, NCreature c2) => ((Node)c1).GetIndex(false).CompareTo(((Node)c2).GetIndex(false)));
		foreach (NCreature item2 in list2)
		{
			item2.AnimHideIntent();
			item2.AnimDisableUi();
		}
		foreach (NCreatureVisuals item3 in list)
		{
			((Node)item3).Reparent((Node)(object)_creatureContainer, true);
		}
	}

	private async Task AnimateIn()
	{
		Tween backstopTween = ((Node)this).CreateTween();
		((CanvasItem)_uiNode).Modulate = StsColors.transparentWhite;
		if (NEventRoom.Instance != null)
		{
			ColorRect fullBlackBackstop = _fullBlackBackstop;
			Color modulate = ((CanvasItem)_fullBlackBackstop).Modulate;
			modulate.A = 0f;
			((CanvasItem)fullBlackBackstop).Modulate = modulate;
			((CanvasItem)_fullBlackBackstop).Visible = true;
			backstopTween.TweenProperty((GodotObject)(object)_fullBlackBackstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.2);
			foreach (NCreatureVisuals item in ((IEnumerable)((Node)_creatureContainer).GetChildren(false)).OfType<NCreatureVisuals>())
			{
				modulate = ((CanvasItem)item).Modulate;
				modulate.A = 0f;
				((CanvasItem)item).Modulate = modulate;
				backstopTween.Parallel().TweenProperty((GodotObject)(object)item, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.2);
			}
		}
		Variant shaderParameter = _backstopMaterial.GetShaderParameter(_threshold);
		backstopTween.TweenMethod(Callable.From<float>((Action<float>)UpdateBackstopMaterial), shaderParameter, Variant.op_Implicit(1f), 1.5).SetEase((EaseType)2).SetTrans((TransitionType)1);
		await ((GodotObject)this).ToSignal((GodotObject)(object)backstopTween, SignalName.Finished);
		_banner.AnimateIn();
		backstopTween.Kill();
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_uiNode, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
		TaskHelper.RunSafely(AnimateInQuote());
		_continueButton.Enable();
	}

	private void UpdateBackstopMaterial(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_backstopMaterial.SetShaderParameter(_threshold, Variant.op_Implicit(value));
	}

	public void AfterOverlayClosed()
	{
		((Node)(object)this).QueueFreeSafely();
	}

	public void AfterOverlayShown()
	{
		NGame.Instance.SetScreenShakeTarget(_screenshakeContainer);
		((CanvasItem)this).Visible = true;
	}

	public void AfterOverlayHidden()
	{
		((CanvasItem)this).Visible = false;
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
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Expected O, but got Unknown
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Expected O, but got Unknown
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Expected O, but got Unknown
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(20);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DiscoveredAnyEpochs, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeBannerAndQuote, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenSummaryScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddBadge, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("locEntryKey"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("locAmountKey"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("amount"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("iconPath"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayUnlockSfx, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TweenScore, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetScoreThreshold, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("unlocksRemaining"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowLeaderboard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideSummary, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenRunHistoryScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMainMenuButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenTimeline, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReturnToMainMenu, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MoveCreaturesToDifferentLayerAndDisableUi, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateBackstopMaterial, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DiscoveredAnyEpochs && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = DiscoveredAnyEpochs();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.InitializeBannerAndQuote && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitializeBannerAndQuote();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenSummaryScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenSummaryScreen(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddBadge && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			AddBadge(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayUnlockSfx && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayUnlockSfx();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TweenScore && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			TweenScore(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetScoreThreshold && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			int scoreThreshold = GetScoreThreshold(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<int>(ref scoreThreshold);
			return true;
		}
		if ((ref method) == MethodName.ShowLeaderboard && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ShowLeaderboard(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideSummary && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideSummary();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenRunHistoryScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenRunHistoryScreen(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMainMenuButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMainMenuButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenTimeline && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenTimeline();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ReturnToMainMenu && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ReturnToMainMenu();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MoveCreaturesToDifferentLayerAndDisableUi && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			MoveCreaturesToDifferentLayerAndDisableUi();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateBackstopMaterial && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateBackstopMaterial(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayShown();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayHidden && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayHidden();
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
		if ((ref method) == MethodName.DiscoveredAnyEpochs)
		{
			return true;
		}
		if ((ref method) == MethodName.InitializeBannerAndQuote)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenSummaryScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.AddBadge)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayUnlockSfx)
		{
			return true;
		}
		if ((ref method) == MethodName.TweenScore)
		{
			return true;
		}
		if ((ref method) == MethodName.GetScoreThreshold)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowLeaderboard)
		{
			return true;
		}
		if ((ref method) == MethodName.HideSummary)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenRunHistoryScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMainMenuButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenTimeline)
		{
			return true;
		}
		if ((ref method) == MethodName.ReturnToMainMenu)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.MoveCreaturesToDifferentLayerAndDisableUi)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateBackstopMaterial)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayShown)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayHidden)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._continueButton)
		{
			_continueButton = VariantUtils.ConvertTo<NGameOverContinueButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._viewRunButton)
		{
			_viewRunButton = VariantUtils.ConvertTo<NViewRunButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mainMenuButton)
		{
			_mainMenuButton = VariantUtils.ConvertTo<NReturnToMainMenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leaderboardButton)
		{
			_leaderboardButton = VariantUtils.ConvertTo<NGameOverContinueButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._badgeContainer)
		{
			_badgeContainer = VariantUtils.ConvertTo<GridContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreBar)
		{
			_scoreBar = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreFg)
		{
			_scoreFg = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreProgress)
		{
			_scoreProgress = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._unlocksRemaining)
		{
			_unlocksRemaining = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._score)
		{
			_score = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreThreshold)
		{
			_scoreThreshold = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreUnlockedEpochId)
		{
			_scoreUnlockedEpochId = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leaderboard)
		{
			_leaderboard = VariantUtils.ConvertTo<NDailyRunLeaderboard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._creatureContainer)
		{
			_creatureContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._summaryContainer)
		{
			_summaryContainer = VariantUtils.ConvertTo<NRunSummary>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fullBlackBackstop)
		{
			_fullBlackBackstop = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._summaryBackstop)
		{
			_summaryBackstop = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			_backstop = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			_banner = VariantUtils.ConvertTo<NCommonBanner>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deathQuote)
		{
			_deathQuote = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._victoryDamageLabel)
		{
			_victoryDamageLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._uiNode)
		{
			_uiNode = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenshakeContainer)
		{
			_screenshakeContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._discoveryLabel)
		{
			_discoveryLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._encounterQuote)
		{
			_encounterQuote = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isAnimatingSummary)
		{
			_isAnimatingSummary = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstopMaterial)
		{
			_backstopMaterial = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._quoteTween)
		{
			_quoteTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ScreenType)
		{
			NetScreenType screenType = ScreenType;
			value = VariantUtils.CreateFrom<NetScreenType>(ref screenType);
			return true;
		}
		if ((ref name) == PropertyName.UseSharedBackstop)
		{
			bool useSharedBackstop = UseSharedBackstop;
			value = VariantUtils.CreateFrom<bool>(ref useSharedBackstop);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._continueButton)
		{
			value = VariantUtils.CreateFrom<NGameOverContinueButton>(ref _continueButton);
			return true;
		}
		if ((ref name) == PropertyName._viewRunButton)
		{
			value = VariantUtils.CreateFrom<NViewRunButton>(ref _viewRunButton);
			return true;
		}
		if ((ref name) == PropertyName._mainMenuButton)
		{
			value = VariantUtils.CreateFrom<NReturnToMainMenuButton>(ref _mainMenuButton);
			return true;
		}
		if ((ref name) == PropertyName._leaderboardButton)
		{
			value = VariantUtils.CreateFrom<NGameOverContinueButton>(ref _leaderboardButton);
			return true;
		}
		if ((ref name) == PropertyName._badgeContainer)
		{
			value = VariantUtils.CreateFrom<GridContainer>(ref _badgeContainer);
			return true;
		}
		if ((ref name) == PropertyName._scoreBar)
		{
			value = VariantUtils.CreateFrom<Control>(ref _scoreBar);
			return true;
		}
		if ((ref name) == PropertyName._scoreFg)
		{
			value = VariantUtils.CreateFrom<Control>(ref _scoreFg);
			return true;
		}
		if ((ref name) == PropertyName._scoreProgress)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _scoreProgress);
			return true;
		}
		if ((ref name) == PropertyName._unlocksRemaining)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _unlocksRemaining);
			return true;
		}
		if ((ref name) == PropertyName._score)
		{
			value = VariantUtils.CreateFrom<int>(ref _score);
			return true;
		}
		if ((ref name) == PropertyName._scoreThreshold)
		{
			value = VariantUtils.CreateFrom<int>(ref _scoreThreshold);
			return true;
		}
		if ((ref name) == PropertyName._scoreUnlockedEpochId)
		{
			value = VariantUtils.CreateFrom<string>(ref _scoreUnlockedEpochId);
			return true;
		}
		if ((ref name) == PropertyName._leaderboard)
		{
			value = VariantUtils.CreateFrom<NDailyRunLeaderboard>(ref _leaderboard);
			return true;
		}
		if ((ref name) == PropertyName._creatureContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _creatureContainer);
			return true;
		}
		if ((ref name) == PropertyName._summaryContainer)
		{
			value = VariantUtils.CreateFrom<NRunSummary>(ref _summaryContainer);
			return true;
		}
		if ((ref name) == PropertyName._fullBlackBackstop)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _fullBlackBackstop);
			return true;
		}
		if ((ref name) == PropertyName._summaryBackstop)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _summaryBackstop);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _backstop);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			value = VariantUtils.CreateFrom<NCommonBanner>(ref _banner);
			return true;
		}
		if ((ref name) == PropertyName._deathQuote)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _deathQuote);
			return true;
		}
		if ((ref name) == PropertyName._victoryDamageLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _victoryDamageLabel);
			return true;
		}
		if ((ref name) == PropertyName._uiNode)
		{
			value = VariantUtils.CreateFrom<Control>(ref _uiNode);
			return true;
		}
		if ((ref name) == PropertyName._screenshakeContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _screenshakeContainer);
			return true;
		}
		if ((ref name) == PropertyName._discoveryLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _discoveryLabel);
			return true;
		}
		if ((ref name) == PropertyName._encounterQuote)
		{
			value = VariantUtils.CreateFrom<string>(ref _encounterQuote);
			return true;
		}
		if ((ref name) == PropertyName._isAnimatingSummary)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isAnimatingSummary);
			return true;
		}
		if ((ref name) == PropertyName._backstopMaterial)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _backstopMaterial);
			return true;
		}
		if ((ref name) == PropertyName._quoteTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _quoteTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
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
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._continueButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewRunButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mainMenuButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leaderboardButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._badgeContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scoreBar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scoreFg, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scoreProgress, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unlocksRemaining, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._score, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._scoreThreshold, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._scoreUnlockedEpochId, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leaderboard, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._creatureContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._summaryContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fullBlackBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._summaryBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._banner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._deathQuote, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._victoryDamageLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._uiNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._screenshakeContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._discoveryLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._encounterQuote, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isAnimatingSummary, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstopMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._quoteTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.ScreenType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.UseSharedBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._continueButton, Variant.From<NGameOverContinueButton>(ref _continueButton));
		info.AddProperty(PropertyName._viewRunButton, Variant.From<NViewRunButton>(ref _viewRunButton));
		info.AddProperty(PropertyName._mainMenuButton, Variant.From<NReturnToMainMenuButton>(ref _mainMenuButton));
		info.AddProperty(PropertyName._leaderboardButton, Variant.From<NGameOverContinueButton>(ref _leaderboardButton));
		info.AddProperty(PropertyName._badgeContainer, Variant.From<GridContainer>(ref _badgeContainer));
		info.AddProperty(PropertyName._scoreBar, Variant.From<Control>(ref _scoreBar));
		info.AddProperty(PropertyName._scoreFg, Variant.From<Control>(ref _scoreFg));
		info.AddProperty(PropertyName._scoreProgress, Variant.From<MegaLabel>(ref _scoreProgress));
		info.AddProperty(PropertyName._unlocksRemaining, Variant.From<MegaLabel>(ref _unlocksRemaining));
		info.AddProperty(PropertyName._score, Variant.From<int>(ref _score));
		info.AddProperty(PropertyName._scoreThreshold, Variant.From<int>(ref _scoreThreshold));
		info.AddProperty(PropertyName._scoreUnlockedEpochId, Variant.From<string>(ref _scoreUnlockedEpochId));
		info.AddProperty(PropertyName._leaderboard, Variant.From<NDailyRunLeaderboard>(ref _leaderboard));
		info.AddProperty(PropertyName._creatureContainer, Variant.From<Control>(ref _creatureContainer));
		info.AddProperty(PropertyName._summaryContainer, Variant.From<NRunSummary>(ref _summaryContainer));
		info.AddProperty(PropertyName._fullBlackBackstop, Variant.From<ColorRect>(ref _fullBlackBackstop));
		info.AddProperty(PropertyName._summaryBackstop, Variant.From<ColorRect>(ref _summaryBackstop));
		info.AddProperty(PropertyName._backstop, Variant.From<ColorRect>(ref _backstop));
		info.AddProperty(PropertyName._banner, Variant.From<NCommonBanner>(ref _banner));
		info.AddProperty(PropertyName._deathQuote, Variant.From<MegaRichTextLabel>(ref _deathQuote));
		info.AddProperty(PropertyName._victoryDamageLabel, Variant.From<MegaRichTextLabel>(ref _victoryDamageLabel));
		info.AddProperty(PropertyName._uiNode, Variant.From<Control>(ref _uiNode));
		info.AddProperty(PropertyName._screenshakeContainer, Variant.From<Control>(ref _screenshakeContainer));
		info.AddProperty(PropertyName._discoveryLabel, Variant.From<MegaLabel>(ref _discoveryLabel));
		info.AddProperty(PropertyName._encounterQuote, Variant.From<string>(ref _encounterQuote));
		info.AddProperty(PropertyName._isAnimatingSummary, Variant.From<bool>(ref _isAnimatingSummary));
		info.AddProperty(PropertyName._backstopMaterial, Variant.From<ShaderMaterial>(ref _backstopMaterial));
		info.AddProperty(PropertyName._quoteTween, Variant.From<Tween>(ref _quoteTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._continueButton, ref val))
		{
			_continueButton = ((Variant)(ref val)).As<NGameOverContinueButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._viewRunButton, ref val2))
		{
			_viewRunButton = ((Variant)(ref val2)).As<NViewRunButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._mainMenuButton, ref val3))
		{
			_mainMenuButton = ((Variant)(ref val3)).As<NReturnToMainMenuButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._leaderboardButton, ref val4))
		{
			_leaderboardButton = ((Variant)(ref val4)).As<NGameOverContinueButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._badgeContainer, ref val5))
		{
			_badgeContainer = ((Variant)(ref val5)).As<GridContainer>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreBar, ref val6))
		{
			_scoreBar = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreFg, ref val7))
		{
			_scoreFg = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreProgress, ref val8))
		{
			_scoreProgress = ((Variant)(ref val8)).As<MegaLabel>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._unlocksRemaining, ref val9))
		{
			_unlocksRemaining = ((Variant)(ref val9)).As<MegaLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._score, ref val10))
		{
			_score = ((Variant)(ref val10)).As<int>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreThreshold, ref val11))
		{
			_scoreThreshold = ((Variant)(ref val11)).As<int>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreUnlockedEpochId, ref val12))
		{
			_scoreUnlockedEpochId = ((Variant)(ref val12)).As<string>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._leaderboard, ref val13))
		{
			_leaderboard = ((Variant)(ref val13)).As<NDailyRunLeaderboard>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._creatureContainer, ref val14))
		{
			_creatureContainer = ((Variant)(ref val14)).As<Control>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._summaryContainer, ref val15))
		{
			_summaryContainer = ((Variant)(ref val15)).As<NRunSummary>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._fullBlackBackstop, ref val16))
		{
			_fullBlackBackstop = ((Variant)(ref val16)).As<ColorRect>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._summaryBackstop, ref val17))
		{
			_summaryBackstop = ((Variant)(ref val17)).As<ColorRect>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstop, ref val18))
		{
			_backstop = ((Variant)(ref val18)).As<ColorRect>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._banner, ref val19))
		{
			_banner = ((Variant)(ref val19)).As<NCommonBanner>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._deathQuote, ref val20))
		{
			_deathQuote = ((Variant)(ref val20)).As<MegaRichTextLabel>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._victoryDamageLabel, ref val21))
		{
			_victoryDamageLabel = ((Variant)(ref val21)).As<MegaRichTextLabel>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._uiNode, ref val22))
		{
			_uiNode = ((Variant)(ref val22)).As<Control>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenshakeContainer, ref val23))
		{
			_screenshakeContainer = ((Variant)(ref val23)).As<Control>();
		}
		Variant val24 = default(Variant);
		if (info.TryGetProperty(PropertyName._discoveryLabel, ref val24))
		{
			_discoveryLabel = ((Variant)(ref val24)).As<MegaLabel>();
		}
		Variant val25 = default(Variant);
		if (info.TryGetProperty(PropertyName._encounterQuote, ref val25))
		{
			_encounterQuote = ((Variant)(ref val25)).As<string>();
		}
		Variant val26 = default(Variant);
		if (info.TryGetProperty(PropertyName._isAnimatingSummary, ref val26))
		{
			_isAnimatingSummary = ((Variant)(ref val26)).As<bool>();
		}
		Variant val27 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstopMaterial, ref val27))
		{
			_backstopMaterial = ((Variant)(ref val27)).As<ShaderMaterial>();
		}
		Variant val28 = default(Variant);
		if (info.TryGetProperty(PropertyName._quoteTween, ref val28))
		{
			_quoteTween = ((Variant)(ref val28)).As<Tween>();
		}
	}
}
