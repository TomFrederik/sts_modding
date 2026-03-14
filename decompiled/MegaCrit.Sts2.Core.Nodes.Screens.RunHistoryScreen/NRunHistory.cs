using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Exceptions;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Potions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;

[ScriptPath("res://src/Core/Nodes/Screens/RunHistoryScreen/NRunHistory.cs")]
public class NRunHistory : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnLeftButtonButtonReleased = StringName.op_Implicit("OnLeftButtonButtonReleased");

		public static readonly StringName OnRightButtonButtonReleased = StringName.op_Implicit("OnRightButtonButtonReleased");

		public static readonly StringName CanBeShown = StringName.op_Implicit("CanBeShown");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuShown = StringName.op_Implicit("OnSubmenuShown");

		public new static readonly StringName OnSubmenuHidden = StringName.op_Implicit("OnSubmenuHidden");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName SelectPlayer = StringName.op_Implicit("SelectPlayer");

		public static readonly StringName LoadGoldHpAndPotionInfo = StringName.op_Implicit("LoadGoldHpAndPotionInfo");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _screenContents = StringName.op_Implicit("_screenContents");

		public static readonly StringName _playerIconContainer = StringName.op_Implicit("_playerIconContainer");

		public static readonly StringName _hpLabel = StringName.op_Implicit("_hpLabel");

		public static readonly StringName _goldLabel = StringName.op_Implicit("_goldLabel");

		public static readonly StringName _potionHolder = StringName.op_Implicit("_potionHolder");

		public static readonly StringName _floorLabel = StringName.op_Implicit("_floorLabel");

		public static readonly StringName _timeLabel = StringName.op_Implicit("_timeLabel");

		public static readonly StringName _dateLabel = StringName.op_Implicit("_dateLabel");

		public static readonly StringName _seedLabel = StringName.op_Implicit("_seedLabel");

		public static readonly StringName _gameModeLabel = StringName.op_Implicit("_gameModeLabel");

		public static readonly StringName _buildLabel = StringName.op_Implicit("_buildLabel");

		public static readonly StringName _deathQuoteLabel = StringName.op_Implicit("_deathQuoteLabel");

		public static readonly StringName _mapPointHistory = StringName.op_Implicit("_mapPointHistory");

		public static readonly StringName _relicHistory = StringName.op_Implicit("_relicHistory");

		public static readonly StringName _deckHistory = StringName.op_Implicit("_deckHistory");

		public static readonly StringName _outOfDateVisual = StringName.op_Implicit("_outOfDateVisual");

		public static readonly StringName _index = StringName.op_Implicit("_index");

		public static readonly StringName _prevButton = StringName.op_Implicit("_prevButton");

		public static readonly StringName _nextButton = StringName.op_Implicit("_nextButton");

		public static readonly StringName _selectedPlayerIcon = StringName.op_Implicit("_selectedPlayerIcon");

		public static readonly StringName _screenTween = StringName.op_Implicit("_screenTween");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/run_history_screen/run_history");

	public const string locTable = "run_history";

	private static readonly LocString _leftQuote = new LocString("game_over_screen", "ENCOUNTER_QUOTE_LEFT");

	private static readonly LocString _rightQuote = new LocString("game_over_screen", "ENCOUNTER_QUOTE_RIGHT");

	private Control _screenContents;

	private Control _playerIconContainer;

	private MegaLabel _hpLabel;

	private MegaLabel _goldLabel;

	private Control _potionHolder;

	private MegaLabel _floorLabel;

	private MegaLabel _timeLabel;

	private MegaRichTextLabel _dateLabel;

	private MegaRichTextLabel _seedLabel;

	private MegaRichTextLabel _gameModeLabel;

	private MegaRichTextLabel _buildLabel;

	private MegaRichTextLabel _deathQuoteLabel;

	private NMapPointHistory _mapPointHistory;

	private NRelicHistory _relicHistory;

	private NDeckHistory _deckHistory;

	private Control _outOfDateVisual;

	private readonly List<string> _runNames = new List<string>();

	private int _index;

	private RunHistory _history;

	private NRunHistoryArrowButton _prevButton;

	private NRunHistoryArrowButton _nextButton;

	private NRunHistoryPlayerIcon? _selectedPlayerIcon;

	private Tween? _screenTween;

	protected override Control? InitialFocusedControl => null;

	public static string[] AssetPaths => new string[1] { _scenePath };

	public static NRunHistory? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NRunHistory>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_screenContents = ((Node)this).GetNode<Control>(NodePath.op_Implicit("ScreenContents"));
		_playerIconContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%PlayerIconContainer"));
		_hpLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%HpLabel"));
		_goldLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%GoldLabel"));
		_potionHolder = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%PotionHolders"));
		_floorLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%FloorNumLabel"));
		_timeLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%RunTimeLabel"));
		_dateLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%DateLabel"));
		_seedLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%SeedLabel"));
		_gameModeLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%GameModeLabel"));
		_buildLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%BuildLabel"));
		_deathQuoteLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%DeathQuoteLabel"));
		_mapPointHistory = ((Node)this).GetNode<NMapPointHistory>(NodePath.op_Implicit("%MapPointHistory"));
		_relicHistory = ((Node)this).GetNode<NRelicHistory>(NodePath.op_Implicit("%RelicHistory"));
		_deckHistory = ((Node)this).GetNode<NDeckHistory>(NodePath.op_Implicit("%DeckHistory"));
		_outOfDateVisual = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%OutOfDateVisual"));
		_prevButton = ((Node)this).GetNode<NRunHistoryArrowButton>(NodePath.op_Implicit("LeftArrow"));
		((GodotObject)_prevButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnLeftButtonButtonReleased), 0u);
		_nextButton = ((Node)this).GetNode<NRunHistoryArrowButton>(NodePath.op_Implicit("RightArrow"));
		((GodotObject)_nextButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnRightButtonButtonReleased), 0u);
		_prevButton.IsLeft = true;
		_mapPointHistory.SetDeckHistory(_deckHistory);
		_mapPointHistory.SetRelicHistory(_relicHistory);
	}

	private void OnLeftButtonButtonReleased(NButton _)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		TaskHelper.RunSafely(RefreshAndSelectRun(_index + 1));
		Tween? screenTween = _screenTween;
		if (screenTween != null)
		{
			screenTween.Kill();
		}
		_screenTween = ((Node)this).CreateTween().SetParallel(true);
		_screenTween.TweenProperty((GodotObject)(object)_screenContents, NodePath.op_Implicit("position"), Variant.op_Implicit(Vector2.Zero), 0.5).SetTrans((TransitionType)5).SetEase((EaseType)1)
			.From(Variant.op_Implicit(Vector2.Zero + new Vector2(-1000f, 0f)));
		_screenTween.TweenProperty((GodotObject)(object)_screenContents, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.4).SetTrans((TransitionType)0).From(Variant.op_Implicit(0f));
	}

	private void OnRightButtonButtonReleased(NButton _)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		TaskHelper.RunSafely(RefreshAndSelectRun(_index - 1));
		Tween? screenTween = _screenTween;
		if (screenTween != null)
		{
			screenTween.Kill();
		}
		_screenTween = ((Node)this).CreateTween().SetParallel(true);
		_screenTween.TweenProperty((GodotObject)(object)_screenContents, NodePath.op_Implicit("position"), Variant.op_Implicit(Vector2.Zero), 0.5).SetTrans((TransitionType)5).SetEase((EaseType)1)
			.From(Variant.op_Implicit(Vector2.Zero + new Vector2(1000f, 0f)));
		_screenTween.TweenProperty((GodotObject)(object)_screenContents, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.4).SetTrans((TransitionType)0).From(Variant.op_Implicit(0f));
	}

	public static bool CanBeShown()
	{
		return SaveManager.Instance.GetRunHistoryCount() > 0;
	}

	public override void OnSubmenuOpened()
	{
		_runNames.Clear();
		_runNames.AddRange(SaveManager.Instance.GetAllRunHistoryNames());
		_runNames.Reverse();
		TaskHelper.RunSafely(RefreshAndSelectRun(0));
	}

	protected override void OnSubmenuShown()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!CanBeShown())
		{
			throw new InvalidOperationException("Tried to show run history screen with no runs!");
		}
		Tween? screenTween = _screenTween;
		if (screenTween != null)
		{
			screenTween.Kill();
		}
		_screenTween = ((Node)this).CreateTween();
		_screenTween.TweenProperty((GodotObject)(object)_screenContents, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.4).From(Variant.op_Implicit(0f));
	}

	protected override void OnSubmenuHidden()
	{
		Tween? screenTween = _screenTween;
		if (screenTween != null)
		{
			screenTween.Kill();
		}
	}

	private Task RefreshAndSelectRun(int index)
	{
		if (index < 0 || index >= _runNames.Count)
		{
			Log.Error($"Invalid run index {index}, valid range is 0-{_runNames.Count - 1}");
			return Task.CompletedTask;
		}
		_prevButton.Disable();
		_nextButton.Disable();
		((CanvasItem)_outOfDateVisual).Visible = false;
		try
		{
			ReadSaveResult<RunHistory> readSaveResult = SaveManager.Instance.LoadRunHistory(_runNames[index]);
			if (readSaveResult.Success)
			{
				DisplayRun(readSaveResult.SaveData);
			}
			else
			{
				Log.Error($"Could not load run {_runNames[index]} at index {index}: {readSaveResult.ErrorMessage} ({readSaveResult.Status})");
				((CanvasItem)_outOfDateVisual).Visible = true;
			}
		}
		catch (Exception value)
		{
			Log.Error($"Exception {value} while loading run at index {index}");
			((CanvasItem)_outOfDateVisual).Visible = true;
			throw;
		}
		finally
		{
			_index = index;
			if (index < _runNames.Count - 1)
			{
				_prevButton.Enable();
			}
			if (index > 0)
			{
				_nextButton.Enable();
			}
			((CanvasItem)_prevButton).Visible = index < _runNames.Count - 1;
			((CanvasItem)_nextButton).Visible = index > 0;
		}
		return Task.CompletedTask;
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (!((CanvasItem)this).IsVisibleInTree() || ((CanvasItem)NDevConsole.Instance).Visible || !NControllerManager.Instance.IsUsingController)
		{
			return;
		}
		Control val = ((Node)this).GetViewport().GuiGetFocusOwner();
		bool flag = ((val is TextEdit || val is LineEdit) ? true : false);
		if (flag || !ActiveScreenContext.Instance.IsCurrent(this))
		{
			return;
		}
		Control val2 = ((Node)this).GetViewport().GuiGetFocusOwner();
		if ((val2 == null || !((Node)this).IsAncestorOf((Node)(object)val2)) && (inputEvent.IsActionPressed(MegaInput.left, false, false) || inputEvent.IsActionPressed(MegaInput.right, false, false) || inputEvent.IsActionPressed(MegaInput.up, false, false) || inputEvent.IsActionPressed(MegaInput.down, false, false) || inputEvent.IsActionPressed(MegaInput.select, false, false)))
		{
			Viewport viewport = ((Node)this).GetViewport();
			if (viewport != null)
			{
				viewport.SetInputAsHandled();
			}
			_mapPointHistory.DefaultFocusedControl?.TryGrabFocus();
		}
	}

	private void DisplayRun(RunHistory history)
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		_selectedPlayerIcon?.Deselect();
		_selectedPlayerIcon = null;
		foreach (NRunHistoryPlayerIcon item in ((IEnumerable)((Node)_playerIconContainer).GetChildren(false)).OfType<NRunHistoryPlayerIcon>())
		{
			((Node)(object)item).QueueFreeSafely();
		}
		_history = history;
		ulong localPlayerId = PlatformUtil.GetLocalPlayerId(history.PlatformType);
		LoadPlayerFloor(history);
		LoadGameModeDetails(history);
		LoadTimeDetails(history);
		_mapPointHistory.LoadHistory(history);
		bool flag = false;
		NRunHistoryPlayerIcon nRunHistoryPlayerIcon = null;
		foreach (RunHistoryPlayer player in history.Players)
		{
			NRunHistoryPlayerIcon playerIcon = PreloadManager.Cache.GetScene(NRunHistoryPlayerIcon.scenePath).Instantiate<NRunHistoryPlayerIcon>((GenEditState)0);
			if (nRunHistoryPlayerIcon == null)
			{
				nRunHistoryPlayerIcon = playerIcon;
			}
			((Node)(object)_playerIconContainer).AddChildSafely((Node?)(object)playerIcon);
			playerIcon.LoadRun(player, history);
			((GodotObject)playerIcon).Connect(NClickableControl.SignalName.MouseReleased, Callable.From<InputEvent>((Action<InputEvent>)delegate
			{
				SelectPlayer(playerIcon);
			}), 0u);
			if (player.Id == localPlayerId)
			{
				flag = true;
				SelectPlayer(playerIcon);
			}
		}
		if (!flag)
		{
			if (history.Players.Count > 1)
			{
				Log.Warn($"Local player with ID {localPlayerId} not found in multiplayer run history file! Defaulting to first player");
			}
			SelectPlayer(nRunHistoryPlayerIcon);
		}
	}

	private void SelectPlayer(NRunHistoryPlayerIcon playerIcon)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		_selectedPlayerIcon?.Deselect();
		_selectedPlayerIcon = playerIcon;
		playerIcon.Select();
		if (_history.Players.Count == 1)
		{
			CharacterModel byId = ModelDb.GetById<CharacterModel>(playerIcon.Player.Character);
			Color nameColor = byId.NameColor;
		}
		else
		{
			LocString locString = new LocString("run_history", "PLAYER_NAME");
			locString.Add("PlayerName", PlatformUtil.GetPlayerName(_history.PlatformType, playerIcon.Player.Id));
		}
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		Player player = Player.CreateForNewRun(ModelDb.GetById<CharacterModel>(playerIcon.Player.Character), unlockState, playerIcon.Player.Id);
		LoadGoldHpAndPotionInfo(playerIcon);
		LoadDeathQuote(_history, playerIcon.Player.Character);
		_mapPointHistory.SetPlayer(playerIcon.Player);
		_relicHistory.LoadRelics(player, playerIcon.Player.Relics);
		_deckHistory.LoadDeck(player, playerIcon.Player.Deck);
	}

	private void LoadGoldHpAndPotionInfo(NRunHistoryPlayerIcon icon)
	{
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		if (!_history.MapPointHistory.Any())
		{
			CharacterModel byId = ModelDb.GetById<CharacterModel>(icon.Player.Character);
			_hpLabel.SetTextAutoSize($"{byId.StartingHp}/{byId.StartingHp}");
			_goldLabel.SetTextAutoSize($"{byId.StartingGold}");
		}
		else
		{
			MapPointHistoryEntry mapPointHistoryEntry = _history.MapPointHistory.Last().Last();
			PlayerMapPointHistoryEntry playerMapPointHistoryEntry = mapPointHistoryEntry.PlayerStats.First((PlayerMapPointHistoryEntry stat) => stat.PlayerId == icon.Player.Id);
			_hpLabel.SetTextAutoSize($"{playerMapPointHistoryEntry.CurrentHp}/{playerMapPointHistoryEntry.MaxHp}");
			_goldLabel.SetTextAutoSize($"{playerMapPointHistoryEntry.CurrentGold}");
		}
		((Node)(object)_potionHolder).FreeChildren();
		RunHistoryPlayer runHistoryPlayer = _history.Players.First((RunHistoryPlayer player) => player.Id == icon.Player.Id);
		List<PotionModel> list = runHistoryPlayer.Potions.Select(PotionModel.FromSerializable).ToList();
		List<NPotionHolder> list2 = new List<NPotionHolder>();
		for (int num = 0; num < runHistoryPlayer.MaxPotionSlotCount; num++)
		{
			NPotionHolder nPotionHolder = NPotionHolder.Create(isUsable: false);
			((Node)(object)_potionHolder).AddChildSafely((Node?)(object)nPotionHolder);
			list2.Add(nPotionHolder);
		}
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		Player owner = Player.CreateForNewRun(ModelDb.GetById<CharacterModel>(icon.Player.Character), unlockState, icon.Player.Id);
		for (int num2 = 0; num2 < list.Count && num2 < runHistoryPlayer.MaxPotionSlotCount; num2++)
		{
			NPotion nPotion = NPotion.Create(list[num2]);
			nPotion.Model.Owner = owner;
			list2[num2].AddPotion(nPotion);
			((Control)nPotion).Position = Vector2.Zero;
		}
	}

	private void LoadPlayerFloor(RunHistory history)
	{
		int value = history.MapPointHistory.Sum((List<MapPointHistoryEntry> rooms) => rooms.Count);
		_floorLabel.SetTextAutoSize($"{value}");
	}

	private void LoadGameModeDetails(RunHistory history)
	{
		LocString locString = new LocString("run_history", "GAME_MODE.title");
		if (history.Players.Count > 1)
		{
			locString.Add("PlayerCount", new LocString("run_history", "PLAYER_COUNT.multiplayer"));
		}
		else
		{
			locString.Add("PlayerCount", new LocString("run_history", "PLAYER_COUNT.singleplayer"));
		}
		switch (history.GameMode)
		{
		case GameMode.Custom:
			locString.Add("GameMode", new LocString("run_history", "GAME_MODE.custom"));
			break;
		case GameMode.Daily:
			locString.Add("GameMode", new LocString("run_history", "GAME_MODE.daily"));
			break;
		case GameMode.Standard:
			locString.Add("GameMode", new LocString("run_history", "GAME_MODE.standard"));
			break;
		default:
			locString.Add("GameMode", new LocString("run_history", "GAME_MODE.unknown"));
			break;
		}
		_gameModeLabel.Text = "[right]" + locString.GetFormattedText() + "[/right]";
	}

	public static GameOverType GetGameOverType(RunHistory history)
	{
		if (history.Win)
		{
			return GameOverType.FalseVictory;
		}
		if (history.WasAbandoned)
		{
			return GameOverType.AbandonedRun;
		}
		if (history.KilledByEncounter != ModelId.none)
		{
			return GameOverType.CombatDeath;
		}
		if (history.KilledByEvent != ModelId.none)
		{
			return GameOverType.EventDeath;
		}
		Log.Warn("How did the game end??");
		return GameOverType.None;
	}

	public static string GetDeathQuote(RunHistory history, ModelId characterId, GameOverType gameOverType)
	{
		CharacterModel byId = ModelDb.GetById<CharacterModel>(characterId);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(_leftQuote.GetRawText());
		Rng rng = new Rng((uint)StringHelper.GetDeterministicHashCode(history.Seed));
		switch (gameOverType)
		{
		case GameOverType.AbandonedRun:
		{
			LocString randomWithPrefix2 = LocString.GetRandomWithPrefix("run_history", "MAP_POINT_HISTORY.abandon", rng);
			byId.AddDetailsTo(randomWithPrefix2);
			stringBuilder.Append(randomWithPrefix2.GetFormattedText());
			break;
		}
		case GameOverType.EventDeath:
		{
			EventModel eventModel;
			try
			{
				eventModel = ModelDb.GetById<EventModel>(history.KilledByEvent);
			}
			catch (ModelNotFoundException)
			{
				eventModel = ModelDb.Event<DeprecatedEvent>();
			}
			LocString locString2 = new LocString("events", eventModel.Id.Entry + ".loss");
			byId.AddDetailsTo(locString2);
			locString2.Add("event", eventModel.Title);
			stringBuilder.Append(locString2.GetFormattedText());
			break;
		}
		case GameOverType.CombatDeath:
		{
			EncounterModel encounterModel = SaveUtil.EncounterOrDeprecated(history.KilledByEncounter);
			LocString lossMessageFor = encounterModel.GetLossMessageFor(byId);
			stringBuilder.Append(lossMessageFor.GetFormattedText());
			break;
		}
		case GameOverType.FalseVictory:
		{
			LocString randomWithPrefix = LocString.GetRandomWithPrefix("run_history", "MAP_POINT_HISTORY.falseVictory", rng);
			byId.AddDetailsTo(randomWithPrefix);
			stringBuilder.Append(randomWithPrefix.GetFormattedText());
			break;
		}
		case GameOverType.None:
		case GameOverType.TrueVictory:
		{
			LocString locString = new LocString("run_history", "MAP_POINT_HISTORY.debug");
			byId.AddDetailsTo(locString);
			stringBuilder.Append(locString.GetFormattedText());
			break;
		}
		default:
			Log.Error("Unimplemented GameOverType: " + gameOverType);
			throw new ArgumentOutOfRangeException("gameOverType", gameOverType, null);
		}
		stringBuilder.Append(_rightQuote.GetRawText());
		return stringBuilder.ToString();
	}

	private void LoadDeathQuote(RunHistory history, ModelId characterId)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		CharacterModel byId = ModelDb.GetById<CharacterModel>(characterId);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(_leftQuote.GetRawText());
		Rng rng = new Rng((uint)StringHelper.GetDeterministicHashCode(history.Seed));
		if (history.Win)
		{
			((Control)_deathQuoteLabel).AddThemeColorOverride(ThemeConstants.RichTextLabel.defaultColor, StsColors.green);
			LocString randomWithPrefix = LocString.GetRandomWithPrefix("run_history", "MAP_POINT_HISTORY.falseVictory", rng);
			byId.AddDetailsTo(randomWithPrefix);
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(0, 1, stringBuilder2);
			handler.AppendFormatted(randomWithPrefix.GetFormattedText());
			stringBuilder3.Append(ref handler);
		}
		else if (history.WasAbandoned)
		{
			((Control)_deathQuoteLabel).AddThemeColorOverride(ThemeConstants.RichTextLabel.defaultColor, StsColors.red);
			LocString randomWithPrefix2 = LocString.GetRandomWithPrefix("run_history", "MAP_POINT_HISTORY.abandon", rng);
			byId.AddDetailsTo(randomWithPrefix2);
			stringBuilder.Append(randomWithPrefix2.GetFormattedText());
		}
		else if (history.KilledByEncounter != ModelId.none)
		{
			((Control)_deathQuoteLabel).AddThemeColorOverride(ThemeConstants.RichTextLabel.defaultColor, StsColors.red);
			EncounterModel encounterModel = SaveUtil.EncounterOrDeprecated(history.KilledByEncounter);
			LocString lossMessageFor = encounterModel.GetLossMessageFor(byId);
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(0, 1, stringBuilder2);
			handler.AppendFormatted(lossMessageFor.GetFormattedText());
			stringBuilder4.Append(ref handler);
		}
		else if (history.KilledByEvent != ModelId.none)
		{
			((Control)_deathQuoteLabel).AddThemeColorOverride(ThemeConstants.RichTextLabel.defaultColor, StsColors.red);
			EventModel eventModel;
			try
			{
				eventModel = ModelDb.GetById<EventModel>(history.KilledByEvent);
			}
			catch (ModelNotFoundException)
			{
				eventModel = ModelDb.Event<DeprecatedEvent>();
			}
			string text = eventModel.Id.Entry + ".loss";
			LocString locString = ((!LocString.Exists("events", text)) ? new LocString("run_history", "DEFAULT_EVENT_LOSS_MESSAGE") : new LocString("events", text));
			byId.AddDetailsTo(locString);
			locString.Add("event", eventModel.Title);
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(0, 1, stringBuilder2);
			handler.AppendFormatted(locString.GetFormattedText());
			stringBuilder5.Append(ref handler);
		}
		stringBuilder.Append(_rightQuote.GetRawText());
		_deathQuoteLabel.Text = stringBuilder.ToString();
	}

	private void LoadTimeDetails(RunHistory history)
	{
		DateTimeFormatInfo dateTimeFormat = LocManager.Instance.CultureInfo.DateTimeFormat;
		DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTimeOffset.FromUnixTimeSeconds(history.StartTime).UtcDateTime, TimeZoneInfo.Local);
		string value = dateTime.ToString("MMMM d, yyyy", dateTimeFormat);
		string value2 = dateTime.ToString("h:mm tt", dateTimeFormat);
		_dateLabel.Text = $"[right][gold]{value}[/gold], [blue]{value2}[/blue][/right]";
		_seedLabel.Text = "[right][gold]Seed[/gold]: " + history.Seed + "[/right]";
		_buildLabel.Text = "[right]" + history.BuildId + "[/right]";
		_timeLabel.SetTextAutoSize(TimeFormatting.Format(history.RunTime));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Expected O, but got Unknown
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Expected O, but got Unknown
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Expected O, but got Unknown
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnLeftButtonButtonReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRightButtonButtonReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CanBeShown, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SelectPlayer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("playerIcon"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LoadGoldHpAndPotionInfo, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("icon"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NRunHistory nRunHistory = Create();
			ret = VariantUtils.CreateFrom<NRunHistory>(ref nRunHistory);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnLeftButtonButtonReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnLeftButtonButtonReleased(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRightButtonButtonReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnRightButtonButtonReleased(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CanBeShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = CanBeShown();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuShown();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuHidden && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuHidden();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SelectPlayer && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SelectPlayer(VariantUtils.ConvertTo<NRunHistoryPlayerIcon>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LoadGoldHpAndPotionInfo && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			LoadGoldHpAndPotionInfo(VariantUtils.ConvertTo<NRunHistoryPlayerIcon>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NRunHistory nRunHistory = Create();
			ret = VariantUtils.CreateFrom<NRunHistory>(ref nRunHistory);
			return true;
		}
		if ((ref method) == MethodName.CanBeShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = CanBeShown();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnLeftButtonButtonReleased)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRightButtonButtonReleased)
		{
			return true;
		}
		if ((ref method) == MethodName.CanBeShown)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuHidden)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.SelectPlayer)
		{
			return true;
		}
		if ((ref method) == MethodName.LoadGoldHpAndPotionInfo)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._screenContents)
		{
			_screenContents = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playerIconContainer)
		{
			_playerIconContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hpLabel)
		{
			_hpLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._goldLabel)
		{
			_goldLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionHolder)
		{
			_potionHolder = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._floorLabel)
		{
			_floorLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._timeLabel)
		{
			_timeLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dateLabel)
		{
			_dateLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._seedLabel)
		{
			_seedLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._gameModeLabel)
		{
			_gameModeLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._buildLabel)
		{
			_buildLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deathQuoteLabel)
		{
			_deathQuoteLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mapPointHistory)
		{
			_mapPointHistory = VariantUtils.ConvertTo<NMapPointHistory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicHistory)
		{
			_relicHistory = VariantUtils.ConvertTo<NRelicHistory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deckHistory)
		{
			_deckHistory = VariantUtils.ConvertTo<NDeckHistory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outOfDateVisual)
		{
			_outOfDateVisual = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._index)
		{
			_index = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._prevButton)
		{
			_prevButton = VariantUtils.ConvertTo<NRunHistoryArrowButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nextButton)
		{
			_nextButton = VariantUtils.ConvertTo<NRunHistoryArrowButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectedPlayerIcon)
		{
			_selectedPlayerIcon = VariantUtils.ConvertTo<NRunHistoryPlayerIcon>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenTween)
		{
			_screenTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._screenContents)
		{
			value = VariantUtils.CreateFrom<Control>(ref _screenContents);
			return true;
		}
		if ((ref name) == PropertyName._playerIconContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _playerIconContainer);
			return true;
		}
		if ((ref name) == PropertyName._hpLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _hpLabel);
			return true;
		}
		if ((ref name) == PropertyName._goldLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _goldLabel);
			return true;
		}
		if ((ref name) == PropertyName._potionHolder)
		{
			value = VariantUtils.CreateFrom<Control>(ref _potionHolder);
			return true;
		}
		if ((ref name) == PropertyName._floorLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _floorLabel);
			return true;
		}
		if ((ref name) == PropertyName._timeLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _timeLabel);
			return true;
		}
		if ((ref name) == PropertyName._dateLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _dateLabel);
			return true;
		}
		if ((ref name) == PropertyName._seedLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _seedLabel);
			return true;
		}
		if ((ref name) == PropertyName._gameModeLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _gameModeLabel);
			return true;
		}
		if ((ref name) == PropertyName._buildLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _buildLabel);
			return true;
		}
		if ((ref name) == PropertyName._deathQuoteLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _deathQuoteLabel);
			return true;
		}
		if ((ref name) == PropertyName._mapPointHistory)
		{
			value = VariantUtils.CreateFrom<NMapPointHistory>(ref _mapPointHistory);
			return true;
		}
		if ((ref name) == PropertyName._relicHistory)
		{
			value = VariantUtils.CreateFrom<NRelicHistory>(ref _relicHistory);
			return true;
		}
		if ((ref name) == PropertyName._deckHistory)
		{
			value = VariantUtils.CreateFrom<NDeckHistory>(ref _deckHistory);
			return true;
		}
		if ((ref name) == PropertyName._outOfDateVisual)
		{
			value = VariantUtils.CreateFrom<Control>(ref _outOfDateVisual);
			return true;
		}
		if ((ref name) == PropertyName._index)
		{
			value = VariantUtils.CreateFrom<int>(ref _index);
			return true;
		}
		if ((ref name) == PropertyName._prevButton)
		{
			value = VariantUtils.CreateFrom<NRunHistoryArrowButton>(ref _prevButton);
			return true;
		}
		if ((ref name) == PropertyName._nextButton)
		{
			value = VariantUtils.CreateFrom<NRunHistoryArrowButton>(ref _nextButton);
			return true;
		}
		if ((ref name) == PropertyName._selectedPlayerIcon)
		{
			value = VariantUtils.CreateFrom<NRunHistoryPlayerIcon>(ref _selectedPlayerIcon);
			return true;
		}
		if ((ref name) == PropertyName._screenTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _screenTween);
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
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._screenContents, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._playerIconContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hpLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._goldLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._floorLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._timeLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dateLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._seedLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._gameModeLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._buildLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._deathQuoteLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mapPointHistory, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicHistory, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._deckHistory, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outOfDateVisual, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._index, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._prevButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._nextButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectedPlayerIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._screenTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._screenContents, Variant.From<Control>(ref _screenContents));
		info.AddProperty(PropertyName._playerIconContainer, Variant.From<Control>(ref _playerIconContainer));
		info.AddProperty(PropertyName._hpLabel, Variant.From<MegaLabel>(ref _hpLabel));
		info.AddProperty(PropertyName._goldLabel, Variant.From<MegaLabel>(ref _goldLabel));
		info.AddProperty(PropertyName._potionHolder, Variant.From<Control>(ref _potionHolder));
		info.AddProperty(PropertyName._floorLabel, Variant.From<MegaLabel>(ref _floorLabel));
		info.AddProperty(PropertyName._timeLabel, Variant.From<MegaLabel>(ref _timeLabel));
		info.AddProperty(PropertyName._dateLabel, Variant.From<MegaRichTextLabel>(ref _dateLabel));
		info.AddProperty(PropertyName._seedLabel, Variant.From<MegaRichTextLabel>(ref _seedLabel));
		info.AddProperty(PropertyName._gameModeLabel, Variant.From<MegaRichTextLabel>(ref _gameModeLabel));
		info.AddProperty(PropertyName._buildLabel, Variant.From<MegaRichTextLabel>(ref _buildLabel));
		info.AddProperty(PropertyName._deathQuoteLabel, Variant.From<MegaRichTextLabel>(ref _deathQuoteLabel));
		info.AddProperty(PropertyName._mapPointHistory, Variant.From<NMapPointHistory>(ref _mapPointHistory));
		info.AddProperty(PropertyName._relicHistory, Variant.From<NRelicHistory>(ref _relicHistory));
		info.AddProperty(PropertyName._deckHistory, Variant.From<NDeckHistory>(ref _deckHistory));
		info.AddProperty(PropertyName._outOfDateVisual, Variant.From<Control>(ref _outOfDateVisual));
		info.AddProperty(PropertyName._index, Variant.From<int>(ref _index));
		info.AddProperty(PropertyName._prevButton, Variant.From<NRunHistoryArrowButton>(ref _prevButton));
		info.AddProperty(PropertyName._nextButton, Variant.From<NRunHistoryArrowButton>(ref _nextButton));
		info.AddProperty(PropertyName._selectedPlayerIcon, Variant.From<NRunHistoryPlayerIcon>(ref _selectedPlayerIcon));
		info.AddProperty(PropertyName._screenTween, Variant.From<Tween>(ref _screenTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._screenContents, ref val))
		{
			_screenContents = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._playerIconContainer, ref val2))
		{
			_playerIconContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._hpLabel, ref val3))
		{
			_hpLabel = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._goldLabel, ref val4))
		{
			_goldLabel = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionHolder, ref val5))
		{
			_potionHolder = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._floorLabel, ref val6))
		{
			_floorLabel = ((Variant)(ref val6)).As<MegaLabel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._timeLabel, ref val7))
		{
			_timeLabel = ((Variant)(ref val7)).As<MegaLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._dateLabel, ref val8))
		{
			_dateLabel = ((Variant)(ref val8)).As<MegaRichTextLabel>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._seedLabel, ref val9))
		{
			_seedLabel = ((Variant)(ref val9)).As<MegaRichTextLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._gameModeLabel, ref val10))
		{
			_gameModeLabel = ((Variant)(ref val10)).As<MegaRichTextLabel>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._buildLabel, ref val11))
		{
			_buildLabel = ((Variant)(ref val11)).As<MegaRichTextLabel>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._deathQuoteLabel, ref val12))
		{
			_deathQuoteLabel = ((Variant)(ref val12)).As<MegaRichTextLabel>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._mapPointHistory, ref val13))
		{
			_mapPointHistory = ((Variant)(ref val13)).As<NMapPointHistory>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicHistory, ref val14))
		{
			_relicHistory = ((Variant)(ref val14)).As<NRelicHistory>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._deckHistory, ref val15))
		{
			_deckHistory = ((Variant)(ref val15)).As<NDeckHistory>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._outOfDateVisual, ref val16))
		{
			_outOfDateVisual = ((Variant)(ref val16)).As<Control>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._index, ref val17))
		{
			_index = ((Variant)(ref val17)).As<int>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._prevButton, ref val18))
		{
			_prevButton = ((Variant)(ref val18)).As<NRunHistoryArrowButton>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._nextButton, ref val19))
		{
			_nextButton = ((Variant)(ref val19)).As<NRunHistoryArrowButton>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectedPlayerIcon, ref val20))
		{
			_selectedPlayerIcon = ((Variant)(ref val20)).As<NRunHistoryPlayerIcon>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenTween, ref val21))
		{
			_screenTween = ((Variant)(ref val21)).As<Tween>();
		}
	}
}
