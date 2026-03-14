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
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rewards;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens;

[ScriptPath("res://src/Core/Nodes/Screens/NRewardsScreen.cs")]
public class NRewardsScreen : Control, IOverlayScreen, IScreenContext
{
	[Signal]
	public delegate void CompletedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RewardCollectedFrom = StringName.op_Implicit("RewardCollectedFrom");

		public static readonly StringName RewardSkippedFrom = StringName.op_Implicit("RewardSkippedFrom");

		public static readonly StringName UpdateScreenState = StringName.op_Implicit("UpdateScreenState");

		public static readonly StringName RemoveButton = StringName.op_Implicit("RemoveButton");

		public static readonly StringName OnProceedButtonPressed = StringName.op_Implicit("OnProceedButtonPressed");

		public static readonly StringName AfterOverlayOpened = StringName.op_Implicit("AfterOverlayOpened");

		public static readonly StringName AfterOverlayClosed = StringName.op_Implicit("AfterOverlayClosed");

		public static readonly StringName TryEnableProceedButton = StringName.op_Implicit("TryEnableProceedButton");

		public static readonly StringName AfterOverlayShown = StringName.op_Implicit("AfterOverlayShown");

		public static readonly StringName AfterOverlayHidden = StringName.op_Implicit("AfterOverlayHidden");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName ProcessScrollEvent = StringName.op_Implicit("ProcessScrollEvent");

		public static readonly StringName ProcessGuiFocus = StringName.op_Implicit("ProcessGuiFocus");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName UpdateScrollPosition = StringName.op_Implicit("UpdateScrollPosition");

		public static readonly StringName HideWaitingForPlayersScreen = StringName.op_Implicit("HideWaitingForPlayersScreen");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName CanScroll = StringName.op_Implicit("CanScroll");

		public static readonly StringName ScrollLimitBottom = StringName.op_Implicit("ScrollLimitBottom");

		public static readonly StringName IsComplete = StringName.op_Implicit("IsComplete");

		public static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public static readonly StringName UseSharedBackstop = StringName.op_Implicit("UseSharedBackstop");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName FocusedControlFromTopBar = StringName.op_Implicit("FocusedControlFromTopBar");

		public static readonly StringName _proceedButton = StringName.op_Implicit("_proceedButton");

		public static readonly StringName _rewardsContainer = StringName.op_Implicit("_rewardsContainer");

		public static readonly StringName _scrollbar = StringName.op_Implicit("_scrollbar");

		public static readonly StringName _headerLabel = StringName.op_Implicit("_headerLabel");

		public static readonly StringName _rewardContainerMask = StringName.op_Implicit("_rewardContainerMask");

		public static readonly StringName _waitingForOtherPlayersOverlay = StringName.op_Implicit("_waitingForOtherPlayersOverlay");

		public static readonly StringName _rewardsWindow = StringName.op_Implicit("_rewardsWindow");

		public static readonly StringName _targetDragPos = StringName.op_Implicit("_targetDragPos");

		public static readonly StringName _scrollbarPressed = StringName.op_Implicit("_scrollbarPressed");

		public static readonly StringName _fadeTween = StringName.op_Implicit("_fadeTween");

		public static readonly StringName _lastRewardFocused = StringName.op_Implicit("_lastRewardFocused");

		public static readonly StringName _isTerminal = StringName.op_Implicit("_isTerminal");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName Completed = StringName.op_Implicit("Completed");
	}

	private const float _scrollLimitTop = 35f;

	private const int _scrollbarThreshold = 400;

	private IRunState _runState;

	private NProceedButton _proceedButton;

	private Control _rewardsContainer;

	private NScrollbar _scrollbar;

	private MegaLabel _headerLabel;

	private Control _rewardContainerMask;

	private Control _waitingForOtherPlayersOverlay;

	private Control _rewardsWindow;

	private Vector2 _targetDragPos;

	private bool _scrollbarPressed;

	private Tween? _fadeTween;

	private readonly List<Control> _rewardButtons = new List<Control>();

	private readonly List<Control> _skippedRewardButtons = new List<Control>();

	private Control? _lastRewardFocused;

	private bool _isTerminal;

	private readonly TaskCompletionSource _closedCompletionSource = new TaskCompletionSource();

	private static readonly LocString _waitingLoc = new LocString("gameplay_ui", "MULTIPLAYER_WAITING");

	private CompletedEventHandler backing_Completed;

	private bool CanScroll => _rewardsContainer.Size.Y >= 400f;

	private float ScrollLimitBottom => 35f - _rewardsContainer.Size.Y + 400f;

	private static string ScenePath => SceneHelper.GetScenePath("screens/rewards_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public Task ClosedTask => _closedCompletionSource.Task;

	public bool IsComplete { get; private set; }

	public NetScreenType ScreenType => NetScreenType.Rewards;

	public bool UseSharedBackstop => true;

	public Control DefaultFocusedControl
	{
		get
		{
			if (_rewardButtons.Count == 0)
			{
				return _rewardsContainer;
			}
			return _lastRewardFocused ?? _rewardButtons[0];
		}
	}

	public Control FocusedControlFromTopBar
	{
		get
		{
			if (_rewardButtons.Count <= 0)
			{
				return _rewardsContainer;
			}
			return _rewardButtons[0];
		}
	}

	public event CompletedEventHandler Completed
	{
		add
		{
			backing_Completed = (CompletedEventHandler)Delegate.Combine(backing_Completed, value);
		}
		remove
		{
			backing_Completed = (CompletedEventHandler)Delegate.Remove(backing_Completed, value);
		}
	}

	public static NRewardsScreen ShowScreen(bool isTerminal, IRunState runState)
	{
		NRewardsScreen nRewardsScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NRewardsScreen>((GenEditState)0);
		nRewardsScreen._isTerminal = isTerminal;
		nRewardsScreen._runState = runState;
		NOverlayStack.Instance.Push(nRewardsScreen);
		return nRewardsScreen;
	}

	public override void _Ready()
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		_proceedButton = ((Node)this).GetNode<NProceedButton>(NodePath.op_Implicit("ProceedButton"));
		_rewardContainerMask = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%RewardContainerMask"));
		_rewardsContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%RewardsContainer"));
		_scrollbar = ((Node)this).GetNode<NScrollbar>(NodePath.op_Implicit("%Scrollbar"));
		_headerLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%HeaderLabel"));
		_waitingForOtherPlayersOverlay = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%WaitingForOtherPlayers"));
		((Node)_waitingForOtherPlayersOverlay).GetNode<MegaLabel>(NodePath.op_Implicit("Label")).SetTextAutoSize(_waitingLoc.GetRawText());
		_rewardsWindow = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Rewards"));
		((CanvasItem)_rewardsWindow).Modulate = StsColors.transparentBlack;
		((GodotObject)_proceedButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnProceedButtonPressed), 0u);
		_proceedButton.SetPulseState(isPulsing: false);
		TryEnableProceedButton();
		_proceedButton.UpdateText(NProceedButton.SkipLoc);
		NDebugAudioManager.Instance?.Play("victory.mp3");
		((GodotObject)_scrollbar).Connect(NScrollbar.SignalName.MousePressed, Callable.From<InputEvent>((Action<InputEvent>)delegate
		{
			_scrollbarPressed = true;
		}), 0u);
		((GodotObject)_scrollbar).Connect(NScrollbar.SignalName.MouseReleased, Callable.From<InputEvent>((Action<InputEvent>)delegate
		{
			_scrollbarPressed = false;
		}), 0u);
		_targetDragPos = new Vector2(_rewardsContainer.Position.X, 35f);
		if (_runState.CurrentRoom is CombatRoom { GoldProportion: <1f })
		{
			_headerLabel.SetTextAutoSize(new LocString("gameplay_ui", "COMBAT_REWARD_HEADER_MUGGED").GetFormattedText());
		}
		else
		{
			_headerLabel.SetTextAutoSize(new LocString("gameplay_ui", "COMBAT_REWARD_HEADER_LOOT").GetFormattedText());
		}
		((GodotObject)((Node)this).GetViewport()).Connect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)ProcessGuiFocus), 0u);
		((GodotObject)_rewardsContainer).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			DefaultFocusedControl.TryGrabFocus();
		}), 0u);
		ActiveScreenContext.Instance.Update();
	}

	public void SetRewards(IEnumerable<Reward> rewards)
	{
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		foreach (Control rewardButton in _rewardButtons)
		{
			RemoveButton(rewardButton);
		}
		List<Reward> list = rewards.ToList();
		_rewardButtons.Clear();
		foreach (Reward item in list)
		{
			Control option;
			if (item is LinkedRewardSet linkedReward)
			{
				option = (Control)(object)NLinkedRewardSet.Create(linkedReward, this);
				((GodotObject)option).Connect(NLinkedRewardSet.SignalName.RewardClaimed, Callable.From<NLinkedRewardSet>((Action<NLinkedRewardSet>)RewardCollectedFrom), 0u);
			}
			else
			{
				option = (Control)(object)NRewardButton.Create(item, this);
				((GodotObject)option).Connect(NRewardButton.SignalName.RewardClaimed, Callable.From<NRewardButton>((Action<NRewardButton>)RewardCollectedFrom), 0u);
				((GodotObject)option).Connect(NRewardButton.SignalName.RewardSkipped, Callable.From<NRewardButton>((Action<NRewardButton>)RewardSkippedFrom), 0u);
				((GodotObject)option).Connect(SignalName.FocusEntered, Callable.From<Control>((Func<Control>)(() => _lastRewardFocused = option)), 0u);
			}
			item.MarkContentAsSeen();
			_rewardButtons.Add(option);
			((Node)(object)_rewardsContainer).AddChildSafely((Node?)(object)option);
		}
		UpdateScreenState();
		if (list.Count == 0)
		{
			TryEnableProceedButton();
		}
		if (_rewardsContainer.HasFocus())
		{
			DefaultFocusedControl.TryGrabFocus();
		}
		TaskHelper.RunSafely(RelicFtueCheck());
	}

	private async Task RelicFtueCheck()
	{
		if (SaveManager.Instance.SeenFtue("obtain_relic_ftue"))
		{
			return;
		}
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		foreach (NRewardButton item in _rewardButtons.OfType<NRewardButton>())
		{
			if (item.Reward is RelicReward)
			{
				NModalContainer.Instance.Add((Node)(object)NRelicRewardFtue.Create((Control)(object)item));
				SaveManager.Instance.MarkFtueAsComplete("obtain_relic_ftue");
				break;
			}
		}
	}

	public void RewardCollectedFrom(Control button)
	{
		int num = _rewardButtons.IndexOf(button);
		RemoveButton(button);
		_lastRewardFocused = ((_rewardButtons.Count > 0) ? _rewardButtons[Mathf.Min(num, _rewardButtons.Count - 1)] : null);
		UpdateScreenState();
		if (_rewardButtons.Count > 0 || _isTerminal)
		{
			TryEnableProceedButton();
			if (!_rewardButtons.Except(_skippedRewardButtons).Any())
			{
				_proceedButton.SetPulseState(isPulsing: true);
			}
		}
	}

	public void RewardSkippedFrom(Control button)
	{
		_skippedRewardButtons.Add(button);
		if (!_rewardButtons.Except(_skippedRewardButtons).Any())
		{
			_proceedButton.SetPulseState(isPulsing: true);
		}
	}

	private void UpdateScreenState()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (_rewardButtons.Count == 0)
		{
			if (_isTerminal)
			{
				Tween? fadeTween = _fadeTween;
				if (fadeTween != null)
				{
					fadeTween.Kill();
				}
				_fadeTween = ((Node)this).CreateTween().SetParallel(true);
				_fadeTween.TweenProperty((GodotObject)(object)((Node)this).GetNode<Control>(NodePath.op_Implicit("Rewards")), NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
				NOverlayStack.Instance.HideBackstop();
				_proceedButton.UpdateText(NProceedButton.ProceedLoc);
				_proceedButton.SetPulseState(isPulsing: true);
				_rewardsContainer.FocusMode = (FocusModeEnum)0;
				IsComplete = true;
				((GodotObject)this).EmitSignal(SignalName.Completed, Array.Empty<Variant>());
			}
			else
			{
				NOverlayStack.Instance.Remove(this);
			}
		}
		_rewardsContainer.ResetSize();
		((CanvasItem)_scrollbar).Visible = CanScroll;
		((Control)_scrollbar).MouseFilter = (MouseFilterEnum)(CanScroll ? 0 : 2);
		if (!CanScroll)
		{
			_targetDragPos.Y = 35f;
		}
		for (int i = 0; i < _rewardButtons.Count; i++)
		{
			_rewardButtons[i].FocusNeighborLeft = ((Node)_rewardButtons[i]).GetPath();
			_rewardButtons[i].FocusNeighborRight = ((Node)_rewardButtons[i]).GetPath();
			_rewardButtons[i].FocusNeighborTop = ((i > 0) ? ((Node)_rewardButtons[i - 1]).GetPath() : ((Node)_rewardButtons[i]).GetPath());
			_rewardButtons[i].FocusNeighborBottom = ((i < _rewardButtons.Count - 1) ? ((Node)_rewardButtons[i + 1]).GetPath() : ((Node)_rewardButtons[i]).GetPath());
		}
	}

	private void RemoveButton(Control button)
	{
		((Node)button).GetParent().RemoveChildSafely((Node?)(object)button);
		((Node)(object)button).QueueFreeSafely();
		int num = _rewardButtons.IndexOf(button);
		_rewardButtons.Remove(button);
		if (_rewardButtons.Count > 0)
		{
			num = Mathf.Min(num, _rewardButtons.Count - 1);
			_rewardButtons[num].TryGrabFocus();
		}
		else if (_rewardButtons.Contains(((Node)this).GetViewport().GuiGetFocusOwner()))
		{
			ActiveScreenContext.Instance.Update();
		}
	}

	private void OnProceedButtonPressed(NButton _)
	{
		if (RunManager.Instance.debugAfterCombatRewardsOverride != null && _isTerminal)
		{
			RunManager.Instance.debugAfterCombatRewardsOverride?.Invoke();
		}
		else if (_isTerminal && (_runState.CurrentRoom.RoomType == RoomType.Boss || _runState.CurrentRoom.IsVictoryRoom))
		{
			if (_runState.Map.SecondBossMapPoint != null && _runState.CurrentMapCoord == _runState.Map.BossMapPoint.coord)
			{
				TaskHelper.RunSafely(RunManager.Instance.ProceedFromTerminalRewardsScreen());
				return;
			}
			_proceedButton.Disable();
			if (RunManager.Instance.ActChangeSynchronizer.IsWaitingForOtherPlayers())
			{
				((CanvasItem)_waitingForOtherPlayersOverlay).Visible = true;
			}
			RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
		}
		else if (_isTerminal)
		{
			if (_proceedButton.IsSkip)
			{
				if (TestMode.IsOn || SaveManager.Instance.SeenFtue("combat_reward_ftue"))
				{
					TaskHelper.RunSafely(RunManager.Instance.ProceedFromTerminalRewardsScreen());
				}
				else
				{
					TaskHelper.RunSafely(RewardFtueCheck());
				}
				return;
			}
			if (_runState.ActFloor > 4)
			{
				SaveManager.Instance.MarkFtueAsComplete("combat_reward_ftue");
			}
			TaskHelper.RunSafely(RunManager.Instance.ProceedFromTerminalRewardsScreen());
		}
		else
		{
			NOverlayStack.Instance.Remove(this);
		}
	}

	public void AfterOverlayOpened()
	{
	}

	public void AfterOverlayClosed()
	{
		if (RunManager.Instance.IsInProgress && !RunManager.Instance.IsCleaningUp)
		{
			foreach (NRewardButton item in ((IEnumerable)((Node)_rewardsContainer).GetChildren(false)).OfType<NRewardButton>())
			{
				item.Reward?.OnSkipped();
			}
			foreach (NLinkedRewardSet item2 in ((IEnumerable)((Node)_rewardsContainer).GetChildren(false)).OfType<NLinkedRewardSet>())
			{
				item2.LinkedRewardSet.OnSkipped();
			}
			_closedCompletionSource.SetResult();
		}
		_proceedButton.Disable();
		((Node)(object)this).QueueFreeSafely();
	}

	private void TryEnableProceedButton()
	{
		if (Hook.ShouldProceedToNextMapPoint(_runState) && !_proceedButton.IsEnabled)
		{
			if (_isTerminal && _rewardButtons.Count == 0)
			{
				NOverlayStack.Instance.HideBackstop();
			}
			_proceedButton.Enable();
		}
	}

	public void AfterOverlayShown()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		TryEnableProceedButton();
		if (!IsComplete)
		{
			_fadeTween?.FastForwardToCompletion();
			_fadeTween = ((Node)this).CreateTween().SetParallel(true);
			_fadeTween.TweenProperty((GodotObject)(object)_rewardsWindow, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.5);
			_fadeTween.TweenProperty((GodotObject)(object)_rewardsWindow, NodePath.op_Implicit("position:y"), Variant.op_Implicit(_rewardsWindow.Position.Y), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)10)
				.From(Variant.op_Implicit(_rewardsWindow.Position.Y + 100f));
		}
	}

	public void AfterOverlayHidden()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		_proceedButton.Disable();
		if (!IsComplete)
		{
			_fadeTween?.FastForwardToCompletion();
			_fadeTween = ((Node)this).CreateTween();
			_fadeTween.TweenProperty((GodotObject)(object)_rewardsWindow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0), 0.25);
		}
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		if (((CanvasItem)this).IsVisibleInTree() && CanScroll)
		{
			ProcessScrollEvent(inputEvent);
		}
	}

	private void ProcessScrollEvent(InputEvent inputEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		_targetDragPos += new Vector2(0f, ScrollHelper.GetDragForScrollEvent(inputEvent));
	}

	private void ProcessGuiFocus(Control focusedControl)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)this).IsVisibleInTree() && CanScroll && NControllerManager.Instance.IsUsingController && _rewardButtons.Contains(focusedControl))
		{
			float num = 0f - focusedControl.Position.Y + _rewardContainerMask.Size.Y * 0.5f;
			num = Mathf.Clamp(num, ScrollLimitBottom, 35f);
			_targetDragPos = new Vector2(_targetDragPos.X, num);
		}
	}

	public override void _Process(double delta)
	{
		if (((CanvasItem)this).IsVisibleInTree())
		{
			UpdateScrollPosition(delta);
		}
	}

	private void UpdateScrollPosition(double delta)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		Vector2 position = _rewardsContainer.Position;
		if (!((Vector2)(ref position)).IsEqualApprox(_targetDragPos))
		{
			Control rewardsContainer = _rewardsContainer;
			position = _rewardsContainer.Position;
			rewardsContainer.Position = ((Vector2)(ref position)).Lerp(_targetDragPos, Mathf.Clamp((float)delta * 15f, 0f, 1f));
			position = _rewardsContainer.Position;
			if (((Vector2)(ref position)).DistanceTo(_targetDragPos) < 0.5f)
			{
				_rewardsContainer.Position = _targetDragPos;
			}
			if (!_scrollbarPressed && CanScroll)
			{
				_scrollbar.SetValueWithoutAnimation(Mathf.Clamp(_rewardsContainer.Position.Y / ScrollLimitBottom, 0f, 1f) * 100f);
			}
		}
		if (_scrollbarPressed)
		{
			_targetDragPos.Y = Mathf.Lerp(35f, ScrollLimitBottom, (float)((Range)_scrollbar).Value * 0.01f);
		}
		if (_targetDragPos.Y < Mathf.Min(ScrollLimitBottom, 0f))
		{
			_targetDragPos.Y = Mathf.Lerp(_targetDragPos.Y, ScrollLimitBottom, (float)delta * 12f);
		}
		else if (_targetDragPos.Y > Mathf.Max(ScrollLimitBottom, 0f))
		{
			_targetDragPos.Y = Mathf.Lerp(_targetDragPos.Y, 35f, (float)delta * 12f);
		}
	}

	private async Task RewardFtueCheck()
	{
		((CanvasItem)_proceedButton).Hide();
		NCombatRewardFtue nCombatRewardFtue = NCombatRewardFtue.Create(_rewardsContainer);
		NModalContainer.Instance.Add((Node)(object)nCombatRewardFtue);
		SaveManager.Instance.MarkFtueAsComplete("combat_reward_ftue");
		await nCombatRewardFtue.WaitForPlayerToConfirm();
		((CanvasItem)_proceedButton).Show();
	}

	public void HideWaitingForPlayersScreen()
	{
		((CanvasItem)_waitingForOtherPlayersOverlay).Visible = false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Expected O, but got Unknown
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Expected O, but got Unknown
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Expected O, but got Unknown
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(17);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RewardCollectedFrom, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RewardSkippedFrom, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateScreenState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnProceedButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TryEnableProceedButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessScrollEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessGuiFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("focusedControl"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateScrollPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideWaitingForPlayersScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RewardCollectedFrom && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RewardCollectedFrom(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RewardSkippedFrom && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RewardSkippedFrom(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateScreenState && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateScreenState();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemoveButton && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RemoveButton(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnProceedButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnProceedButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TryEnableProceedButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TryEnableProceedButton();
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
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessScrollEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessGuiFocus && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessGuiFocus(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateScrollPosition(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideWaitingForPlayersScreen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideWaitingForPlayersScreen();
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
		if ((ref method) == MethodName.RewardCollectedFrom)
		{
			return true;
		}
		if ((ref method) == MethodName.RewardSkippedFrom)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateScreenState)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveButton)
		{
			return true;
		}
		if ((ref method) == MethodName.OnProceedButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.TryEnableProceedButton)
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
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessGuiFocus)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.HideWaitingForPlayersScreen)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsComplete)
		{
			IsComplete = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			_proceedButton = VariantUtils.ConvertTo<NProceedButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rewardsContainer)
		{
			_rewardsContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollbar)
		{
			_scrollbar = VariantUtils.ConvertTo<NScrollbar>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._headerLabel)
		{
			_headerLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rewardContainerMask)
		{
			_rewardContainerMask = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._waitingForOtherPlayersOverlay)
		{
			_waitingForOtherPlayersOverlay = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rewardsWindow)
		{
			_rewardsWindow = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetDragPos)
		{
			_targetDragPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollbarPressed)
		{
			_scrollbarPressed = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			_fadeTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastRewardFocused)
		{
			_lastRewardFocused = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isTerminal)
		{
			_isTerminal = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CanScroll)
		{
			bool canScroll = CanScroll;
			value = VariantUtils.CreateFrom<bool>(ref canScroll);
			return true;
		}
		if ((ref name) == PropertyName.ScrollLimitBottom)
		{
			float scrollLimitBottom = ScrollLimitBottom;
			value = VariantUtils.CreateFrom<float>(ref scrollLimitBottom);
			return true;
		}
		if ((ref name) == PropertyName.IsComplete)
		{
			bool canScroll = IsComplete;
			value = VariantUtils.CreateFrom<bool>(ref canScroll);
			return true;
		}
		if ((ref name) == PropertyName.ScreenType)
		{
			NetScreenType screenType = ScreenType;
			value = VariantUtils.CreateFrom<NetScreenType>(ref screenType);
			return true;
		}
		if ((ref name) == PropertyName.UseSharedBackstop)
		{
			bool canScroll = UseSharedBackstop;
			value = VariantUtils.CreateFrom<bool>(ref canScroll);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName.FocusedControlFromTopBar)
		{
			Control defaultFocusedControl = FocusedControlFromTopBar;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			value = VariantUtils.CreateFrom<NProceedButton>(ref _proceedButton);
			return true;
		}
		if ((ref name) == PropertyName._rewardsContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _rewardsContainer);
			return true;
		}
		if ((ref name) == PropertyName._scrollbar)
		{
			value = VariantUtils.CreateFrom<NScrollbar>(ref _scrollbar);
			return true;
		}
		if ((ref name) == PropertyName._headerLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _headerLabel);
			return true;
		}
		if ((ref name) == PropertyName._rewardContainerMask)
		{
			value = VariantUtils.CreateFrom<Control>(ref _rewardContainerMask);
			return true;
		}
		if ((ref name) == PropertyName._waitingForOtherPlayersOverlay)
		{
			value = VariantUtils.CreateFrom<Control>(ref _waitingForOtherPlayersOverlay);
			return true;
		}
		if ((ref name) == PropertyName._rewardsWindow)
		{
			value = VariantUtils.CreateFrom<Control>(ref _rewardsWindow);
			return true;
		}
		if ((ref name) == PropertyName._targetDragPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetDragPos);
			return true;
		}
		if ((ref name) == PropertyName._scrollbarPressed)
		{
			value = VariantUtils.CreateFrom<bool>(ref _scrollbarPressed);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _fadeTween);
			return true;
		}
		if ((ref name) == PropertyName._lastRewardFocused)
		{
			value = VariantUtils.CreateFrom<Control>(ref _lastRewardFocused);
			return true;
		}
		if ((ref name) == PropertyName._isTerminal)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isTerminal);
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
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._proceedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rewardsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scrollbar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._headerLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rewardContainerMask, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._waitingForOtherPlayersOverlay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rewardsWindow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetDragPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.CanScroll, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._scrollbarPressed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.ScrollLimitBottom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fadeTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lastRewardFocused, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isTerminal, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsComplete, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.ScreenType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.UseSharedBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.FocusedControlFromTopBar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		StringName isComplete = PropertyName.IsComplete;
		bool isComplete2 = IsComplete;
		info.AddProperty(isComplete, Variant.From<bool>(ref isComplete2));
		info.AddProperty(PropertyName._proceedButton, Variant.From<NProceedButton>(ref _proceedButton));
		info.AddProperty(PropertyName._rewardsContainer, Variant.From<Control>(ref _rewardsContainer));
		info.AddProperty(PropertyName._scrollbar, Variant.From<NScrollbar>(ref _scrollbar));
		info.AddProperty(PropertyName._headerLabel, Variant.From<MegaLabel>(ref _headerLabel));
		info.AddProperty(PropertyName._rewardContainerMask, Variant.From<Control>(ref _rewardContainerMask));
		info.AddProperty(PropertyName._waitingForOtherPlayersOverlay, Variant.From<Control>(ref _waitingForOtherPlayersOverlay));
		info.AddProperty(PropertyName._rewardsWindow, Variant.From<Control>(ref _rewardsWindow));
		info.AddProperty(PropertyName._targetDragPos, Variant.From<Vector2>(ref _targetDragPos));
		info.AddProperty(PropertyName._scrollbarPressed, Variant.From<bool>(ref _scrollbarPressed));
		info.AddProperty(PropertyName._fadeTween, Variant.From<Tween>(ref _fadeTween));
		info.AddProperty(PropertyName._lastRewardFocused, Variant.From<Control>(ref _lastRewardFocused));
		info.AddProperty(PropertyName._isTerminal, Variant.From<bool>(ref _isTerminal));
		info.AddSignalEventDelegate(SignalName.Completed, (Delegate)backing_Completed);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsComplete, ref val))
		{
			IsComplete = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._proceedButton, ref val2))
		{
			_proceedButton = ((Variant)(ref val2)).As<NProceedButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._rewardsContainer, ref val3))
		{
			_rewardsContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollbar, ref val4))
		{
			_scrollbar = ((Variant)(ref val4)).As<NScrollbar>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._headerLabel, ref val5))
		{
			_headerLabel = ((Variant)(ref val5)).As<MegaLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._rewardContainerMask, ref val6))
		{
			_rewardContainerMask = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._waitingForOtherPlayersOverlay, ref val7))
		{
			_waitingForOtherPlayersOverlay = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._rewardsWindow, ref val8))
		{
			_rewardsWindow = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetDragPos, ref val9))
		{
			_targetDragPos = ((Variant)(ref val9)).As<Vector2>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollbarPressed, ref val10))
		{
			_scrollbarPressed = ((Variant)(ref val10)).As<bool>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._fadeTween, ref val11))
		{
			_fadeTween = ((Variant)(ref val11)).As<Tween>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastRewardFocused, ref val12))
		{
			_lastRewardFocused = ((Variant)(ref val12)).As<Control>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._isTerminal, ref val13))
		{
			_isTerminal = ((Variant)(ref val13)).As<bool>();
		}
		CompletedEventHandler completedEventHandler = default(CompletedEventHandler);
		if (info.TryGetSignalEventDelegate<CompletedEventHandler>(SignalName.Completed, ref completedEventHandler))
		{
			backing_Completed = completedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.Completed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalCompleted()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Completed, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Completed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_Completed?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Completed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
