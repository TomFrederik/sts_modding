using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Achievements;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline.UnlockScreens;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/NTimelineScreen.cs")]
public class NTimelineScreen : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public new static readonly StringName OnSubmenuShown = StringName.op_Implicit("OnSubmenuShown");

		public new static readonly StringName OnSubmenuHidden = StringName.op_Implicit("OnSubmenuHidden");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnBackButtonPressed = StringName.op_Implicit("OnBackButtonPressed");

		public static readonly StringName GetEraTexturePath = StringName.op_Implicit("GetEraTexturePath");

		public static readonly StringName GetSlot = StringName.op_Implicit("GetSlot");

		public static readonly StringName OpenInspectScreen = StringName.op_Implicit("OpenInspectScreen");

		public static readonly StringName QueueMiscUnlock = StringName.op_Implicit("QueueMiscUnlock");

		public static readonly StringName SetScreenDraggability = StringName.op_Implicit("SetScreenDraggability");

		public static readonly StringName ShowBackstopAndHideUi = StringName.op_Implicit("ShowBackstopAndHideUi");

		public static readonly StringName OpenQueuedScreen = StringName.op_Implicit("OpenQueuedScreen");

		public static readonly StringName IsScreenQueued = StringName.op_Implicit("IsScreenQueued");

		public static readonly StringName IsInspectScreenQueued = StringName.op_Implicit("IsInspectScreenQueued");

		public static readonly StringName ShowHeaderAndActionsUi = StringName.op_Implicit("ShowHeaderAndActionsUi");

		public static readonly StringName DisableInput = StringName.op_Implicit("DisableInput");

		public static readonly StringName EnableInput = StringName.op_Implicit("EnableInput");

		public static readonly StringName RefreshBackButton = StringName.op_Implicit("RefreshBackButton");

		public static readonly StringName ResetScreen = StringName.op_Implicit("ResetScreen");

		public static readonly StringName GetReminderVfxHolder = StringName.op_Implicit("GetReminderVfxHolder");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _inspectScreen = StringName.op_Implicit("_inspectScreen");

		public static readonly StringName _reminderText = StringName.op_Implicit("_reminderText");

		public static readonly StringName _reminderVfxHolder = StringName.op_Implicit("_reminderVfxHolder");

		public static readonly StringName _backstop = StringName.op_Implicit("_backstop");

		public static readonly StringName _inputBlocker = StringName.op_Implicit("_inputBlocker");

		public static readonly StringName _lineContainer = StringName.op_Implicit("_lineContainer");

		public static readonly StringName _line = StringName.op_Implicit("_line");

		public static readonly StringName _epochSlotContainer = StringName.op_Implicit("_epochSlotContainer");

		public static readonly StringName _slotsContainer = StringName.op_Implicit("_slotsContainer");

		public new static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _isUiVisible = StringName.op_Implicit("_isUiVisible");

		public static readonly StringName _queuedInspectScreen = StringName.op_Implicit("_queuedInspectScreen");

		public static readonly StringName _lineGrowTween = StringName.op_Implicit("_lineGrowTween");

		public static readonly StringName _backstopTween = StringName.op_Implicit("_backstopTween");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("timeline_screen/timeline_screen");

	private const string _placeEpochSparksPath = "res://scenes/timeline_screen/place_epoch_sparks.tscn";

	private NEpochInspectScreen _inspectScreen;

	private NEpochReminderText _reminderText;

	private Control _reminderVfxHolder;

	private ColorRect _backstop;

	private Control _inputBlocker;

	private Control _lineContainer;

	private Control _line;

	private HBoxContainer _epochSlotContainer;

	private NSlotsContainer _slotsContainer;

	private NBackButton _backButton;

	private ProgressState _save;

	private bool _isUiVisible;

	private Dictionary<EpochEra, NEraColumn> _uniqueEpochEras = new Dictionary<EpochEra, NEraColumn>();

	private NEpochSlot? _queuedInspectScreen;

	private Queue<NUnlockScreen> _unlockScreens = new Queue<NUnlockScreen>();

	private Tween? _lineGrowTween;

	private Tween? _backstopTween;

	public static string[] AssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.Add(_scenePath);
			list.Add("res://scenes/timeline_screen/place_epoch_sparks.tscn");
			list.Add(NEpochHighlightVfx.scenePath);
			list.Add(NEpochOffscreenVfx.scenePath);
			list.Add(NEpochInspectScreen.lockedImagePath);
			list.AddRange(NUnlockTimelineScreen.AssetPaths);
			list.AddRange(NUnlockPotionsScreen.AssetPaths);
			list.AddRange(NUnlockRelicsScreen.AssetPaths);
			list.AddRange(NUnlockCardsScreen.AssetPaths);
			list.AddRange(NUnlockMiscScreen.AssetPaths);
			list.AddRange(NUnlockCharacterScreen.AssetPaths);
			list.AddRange(NEraColumn.assetPaths);
			list.AddRange(GetAllEraTexturePaths());
			return list.ToArray();
		}
	}

	public static NTimelineScreen Instance => NGame.Instance.MainMenu.SubmenuStack.GetSubmenuType<NTimelineScreen>();

	protected override Control? InitialFocusedControl => (Control?)(object)((IEnumerable<Node>)((Node)_epochSlotContainer).GetChildren(false)).SelectMany((Node c) => ((IEnumerable)c.GetChildren(false)).OfType<NEpochSlot>()).FirstOrDefault((NEpochSlot s) => s.model is NeowEpoch);

	public static NTimelineScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NTimelineScreen>((GenEditState)0);
	}

	public override void OnSubmenuOpened()
	{
		ResetScreen();
		DisableInput();
		SerializableEpoch serializableEpoch = SaveManager.Instance.Progress.Epochs.FirstOrDefault((SerializableEpoch e) => e.Id == EpochModel.GetId<NeowEpoch>());
		bool flag = serializableEpoch == null;
		bool flag2 = flag;
		if (!flag2)
		{
			EpochState state = serializableEpoch.State;
			bool flag3 = (uint)(state - 1) <= 1u;
			flag2 = flag3;
		}
		if (flag2)
		{
			SaveManager.Instance.Progress.ObtainEpoch(EpochModel.GetId<NeowEpoch>());
		}
		if (SaveManager.Instance.IsNeowDiscovered())
		{
			TaskHelper.RunSafely(FirstTimeLogic());
		}
		else
		{
			SfxCmd.Play("event:/sfx/ui/timeline/ui_timeline_open");
			TaskHelper.RunSafely(InitScreen());
		}
		SetScreenDraggability();
		AchievementsHelper.CheckTimelineComplete();
	}

	public override void OnSubmenuClosed()
	{
		base.OnSubmenuClosed();
		ResetScreen();
	}

	protected override void OnSubmenuShown()
	{
		((Node)this).ProcessMode = (ProcessModeEnum)0;
		RefreshBackButton();
	}

	protected override void OnSubmenuHidden()
	{
		((Node)this).ProcessMode = (ProcessModeEnum)4;
		NGame.Instance.MainMenu?.RefreshButtons();
	}

	public override void _Ready()
	{
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_epochSlotContainer = ((Node)this).GetNode<HBoxContainer>(NodePath.op_Implicit("%EpochSlots"));
		_reminderText = ((Node)this).GetNode<NEpochReminderText>(NodePath.op_Implicit("%EpochReminderText"));
		_reminderVfxHolder = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ReminderVfxHolder"));
		_inputBlocker = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%InputBlocker"));
		_backstop = ((Node)this).GetNode<ColorRect>(NodePath.op_Implicit("%SharedBackstop"));
		_inspectScreen = ((Node)this).GetNode<NEpochInspectScreen>(NodePath.op_Implicit("%EpochInspectScreen"));
		_line = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Line"));
		_lineContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%LineContainer"));
		_slotsContainer = ((Node)this).GetNode<NSlotsContainer>(NodePath.op_Implicit("%SlotsContainer"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("BackButton"));
		((GodotObject)_backButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnBackButtonPressed), 0u);
		_save = SaveManager.Instance.Progress;
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_slotsContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.0);
	}

	private static void OnBackButtonPressed(NButton obj)
	{
		SfxCmd.Play("event:/sfx/ui/map/map_close");
	}

	private async Task FirstTimeLogic()
	{
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		NTimelineTutorial nTimelineTutorial = SceneHelper.Instantiate<NTimelineTutorial>("timeline_screen/timeline_tutorial");
		((Node)(object)this).AddChildSafely((Node?)(object)nTimelineTutorial);
		nTimelineTutorial.Init(this);
	}

	public async Task SpawnFirstTimeTimeline()
	{
		SfxCmd.Play("event:/sfx/ui/timeline/ui_timeline_open");
		Log.Info("Running first time logic");
		List<EpochSlotData> slotsToAdd = new List<EpochSlotData>(1)
		{
			new EpochSlotData(EpochModel.GetId<NeowEpoch>(), EpochSlotState.Obtained)
		};
		await AddEpochSlots(slotsToAdd, isAnimated: true);
		SaveManager.Instance.UnlockSlot(EpochModel.GetId<NeowEpoch>());
		EnableInput();
	}

	private async Task InitScreen()
	{
		Log.Info("Initializing Timeline:");
		List<EpochSlotData> list = new List<EpochSlotData>();
		Tween? lineGrowTween = _lineGrowTween;
		if (lineGrowTween != null)
		{
			lineGrowTween.Kill();
		}
		_lineGrowTween = ((Node)this).CreateTween();
		_lineGrowTween.TweenProperty((GodotObject)(object)_lineContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5);
		foreach (SerializableEpoch epoch in _save.Epochs)
		{
			if (epoch.State != EpochState.ObtainedNoSlot)
			{
				list.Add(new EpochSlotData(epoch.Id, EpochSlotState.NotObtained));
			}
		}
		list = list.OrderBy((EpochSlotData a) => a.EraPosition).ToList();
		await AddEpochSlots(list, isAnimated: false);
		int num = 0;
		foreach (SerializableEpoch epoch2 in _save.Epochs)
		{
			EpochModel epochModel = EpochModel.Get(epoch2.Id);
			if (epoch2.State <= EpochState.ObtainedNoSlot)
			{
				continue;
			}
			foreach (Node child in ((Node)_uniqueEpochEras[epochModel.Era]).GetChildren(false))
			{
				if (child is NEpochSlot nEpochSlot && nEpochSlot.eraPosition == epochModel.EraPosition)
				{
					num++;
					nEpochSlot.SetState((epoch2.State >= EpochState.Revealed) ? EpochSlotState.Complete : EpochSlotState.Obtained);
					TaskHelper.RunSafely(child.GetParent<NEraColumn>().SpawnNameAndYear());
				}
			}
		}
		Log.Info($"{num} Epochs are complete");
		TaskHelper.RunSafely(NavigateToRevealableSlot());
	}

	private async Task NavigateToRevealableSlot()
	{
		if (SaveManager.Instance.GetDiscoveredEpochCount() == 0)
		{
			EnableInput();
			return;
		}
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		float getInitX = _slotsContainer.GetInitX;
		float slotPositionX = 0f;
		float num = float.MaxValue;
		foreach (NEraColumn value in _uniqueEpochEras.Values)
		{
			foreach (NEpochSlot item in ((Node)(object)value).GetChildrenRecursive<NEpochSlot>())
			{
				if (item.State == EpochSlotState.Obtained)
				{
					float num2 = Math.Abs(getInitX - ((Control)item).GlobalPosition.X);
					if (num2 < num)
					{
						slotPositionX = ((Control)item).GlobalPosition.X;
						num = num2;
					}
				}
			}
		}
		await TaskHelper.RunSafely(_slotsContainer.LerpToSlot(slotPositionX));
		EnableInput();
	}

	public async Task AddEpochSlots(List<EpochSlotData> slotsToAdd, bool isAnimated)
	{
		List<NEraColumn> list = new List<NEraColumn>();
		if (isAnimated)
		{
			foreach (NEraColumn value2 in _uniqueEpochEras.Values)
			{
				TaskHelper.RunSafely(value2.SaveBeforeAnimationPosition());
			}
		}
		foreach (EpochSlotData item in slotsToAdd)
		{
			if (!_uniqueEpochEras.TryGetValue(item.Era, out NEraColumn value))
			{
				NEraColumn nEraColumn = NEraColumn.Create(item);
				((Node)(object)_epochSlotContainer).AddChildSafely((Node?)(object)nEraColumn);
				int num = 0;
				foreach (Node child in ((Node)_epochSlotContainer).GetChildren(false))
				{
					if (child is NEraColumn nEraColumn2 && nEraColumn.era > nEraColumn2.era)
					{
						num = ((Node)nEraColumn2).GetIndex(false) + 1;
					}
				}
				((Node)_epochSlotContainer).MoveChild((Node)(object)nEraColumn, num);
				list.Add(nEraColumn);
				_uniqueEpochEras.Add(item.Era, nEraColumn);
			}
			else
			{
				value.AddSlot(item);
			}
		}
		Log.Info($" Created {slotsToAdd.Count} Epoch slots");
		Log.Info($" Created {list.Count} Era columns");
		if (isAnimated)
		{
			List<Vector2> list2 = PredictHBoxLayout(_epochSlotContainer);
			foreach (NEraColumn value3 in _uniqueEpochEras.Values)
			{
				value3.SetPredictedPosition(list2[((Node)value3).GetIndex(false)]);
			}
			await GrowTimelineAndAddEraIcons(list);
		}
		else
		{
			InitLineAndIcons(list);
		}
	}

	private List<Vector2> PredictHBoxLayout(HBoxContainer hbox)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = ((Control)hbox).GetThemeConstant(ThemeConstants.BoxContainer.separation, StringName.op_Implicit("HBoxContainer"));
		List<Control> list = (from c in ((IEnumerable)((Node)hbox).GetChildren(false)).OfType<Control>()
			where ((CanvasItem)c).Visible
			select c).ToList();
		int num3 = 0;
		foreach (Control item in list)
		{
			num += item.CustomMinimumSize.X;
			if ((item.SizeFlagsHorizontal & 2) != 0)
			{
				num3++;
			}
		}
		num += num2 * (float)Math.Max(list.Count - 1, 0);
		float num4 = ((Control)hbox).Size.X - num;
		float num5 = ((num3 > 0) ? (num4 / (float)num3) : 0f);
		float num6 = 0f;
		List<Vector2> list2 = new List<Vector2>();
		foreach (Control item2 in list)
		{
			float num7 = item2.CustomMinimumSize.X;
			if ((item2.SizeFlagsHorizontal & 2) != 0)
			{
				num7 += num5;
			}
			list2.Add(new Vector2(num6, 0f));
			num6 += num7 + num2;
		}
		return list2;
	}

	private async Task GrowTimelineAndAddEraIcons(List<NEraColumn> newlyCreatedColumns)
	{
		if (newlyCreatedColumns.Count > 0)
		{
			Tween? lineGrowTween = _lineGrowTween;
			if (lineGrowTween != null)
			{
				lineGrowTween.Kill();
			}
			_lineGrowTween = ((Node)this).CreateTween().SetParallel(true);
			_lineGrowTween.TweenProperty((GodotObject)(object)_lineContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5);
			_lineGrowTween.TweenProperty((GodotObject)(object)_line, NodePath.op_Implicit("custom_minimum_size:x"), Variant.op_Implicit((float)_uniqueEpochEras.Count * 226f), 2.0).SetEase((EaseType)2).SetTrans((TransitionType)7);
			await ((GodotObject)this).ToSignal((GodotObject)(object)_lineGrowTween, SignalName.Finished);
			Log.Info("Spawning slots...");
			foreach (NEraColumn newlyCreatedColumn in newlyCreatedColumns)
			{
				newlyCreatedColumn.SpawnIcon();
			}
			newlyCreatedColumns.Clear();
		}
		foreach (NEraColumn value in _uniqueEpochEras.Values)
		{
			TaskHelper.RunSafely(value.SpawnSlots(isAnimated: true));
		}
	}

	private void InitLineAndIcons(List<NEraColumn> newlyCreatedColumns)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (newlyCreatedColumns.Count == 0)
		{
			return;
		}
		_line.CustomMinimumSize = new Vector2((float)_uniqueEpochEras.Count * 226f, _line.CustomMinimumSize.Y);
		foreach (NEraColumn newlyCreatedColumn in newlyCreatedColumns)
		{
			newlyCreatedColumn.SpawnIcon();
		}
		foreach (NEraColumn value in _uniqueEpochEras.Values)
		{
			TaskHelper.RunSafely(value.SpawnSlots(isAnimated: false));
		}
		newlyCreatedColumns.Clear();
	}

	public static (Texture2D Texture, string Name) GetEraIcon(EpochEra era)
	{
		return (Texture: PreloadManager.Cache.GetTexture2D(GetEraTexturePath(era)), Name: StringHelper.Slugify(era.ToString()));
	}

	private static string GetEraTexturePath(EpochEra era)
	{
		if (era >= EpochEra.Seeds0)
		{
			return $"res://images/atlases/era_atlas.sprites/era_{(int)era}.tres";
		}
		return $"res://images/atlases/era_atlas.sprites/era_minus_{Math.Abs((int)era)}.tres";
	}

	private static IEnumerable<string> GetAllEraTexturePaths()
	{
		EpochEra[] values = Enum.GetValues<EpochEra>();
		foreach (EpochEra era in values)
		{
			yield return GetEraTexturePath(era);
		}
	}

	private NEpochSlot? GetSlot(EpochEra era, int position)
	{
		foreach (KeyValuePair<EpochEra, NEraColumn> uniqueEpochEra in _uniqueEpochEras)
		{
			if (uniqueEpochEra.Key == era)
			{
				return (NEpochSlot)(object)((Node)uniqueEpochEra.Value).GetChild(((Node)uniqueEpochEra.Value).GetChildCount(false) - position - 2, false);
			}
		}
		Log.Error($"Could not find Epoch slot: {era}, {position}");
		return null;
	}

	public void OpenInspectScreen(NEpochSlot slot, bool playAnimation)
	{
		if (playAnimation)
		{
			TaskHelper.RunSafely(((Node)slot).GetParent<NEraColumn>().SpawnNameAndYear());
		}
		_lastFocusedControl = (Control?)(object)slot;
		TaskHelper.RunSafely(_inspectScreen.Open(slot, slot.model, playAnimation));
	}

	public void QueueMiscUnlock(string text)
	{
		NUnlockMiscScreen nUnlockMiscScreen = NUnlockMiscScreen.Create();
		nUnlockMiscScreen.SetUnlocks(text);
		_unlockScreens.Enqueue(nUnlockMiscScreen);
	}

	public void QueueCharacterUnlock<T>(EpochModel epoch) where T : CharacterModel
	{
		_unlockScreens.Enqueue(NUnlockCharacterScreen.Create(epoch, ModelDb.Character<T>()));
	}

	public void QueueCardUnlock(IReadOnlyList<CardModel> cards)
	{
		NUnlockCardsScreen nUnlockCardsScreen = NUnlockCardsScreen.Create();
		nUnlockCardsScreen.SetCards(cards);
		_unlockScreens.Enqueue(nUnlockCardsScreen);
	}

	public void QueueRelicUnlock(List<RelicModel> relics)
	{
		NUnlockRelicsScreen nUnlockRelicsScreen = NUnlockRelicsScreen.Create();
		nUnlockRelicsScreen.SetRelics(relics);
		_unlockScreens.Enqueue(nUnlockRelicsScreen);
	}

	public void QueuePotionUnlock(List<PotionModel> potions)
	{
		NUnlockPotionsScreen nUnlockPotionsScreen = NUnlockPotionsScreen.Create();
		nUnlockPotionsScreen.SetPotions(potions);
		_unlockScreens.Enqueue(nUnlockPotionsScreen);
	}

	public void QueueTimelineExpansion(List<EpochSlotData> eraData)
	{
		NUnlockTimelineScreen nUnlockTimelineScreen = NUnlockTimelineScreen.Create();
		nUnlockTimelineScreen.SetUnlocks(eraData);
		_unlockScreens.Enqueue(nUnlockTimelineScreen);
	}

	public void SetScreenDraggability()
	{
		((Control)_slotsContainer).MouseFilter = (MouseFilterEnum)((_save.Epochs.Count <= 4) ? 2 : 0);
	}

	public void ShowBackstopAndHideUi()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_backstop).Visible = true;
		Tween? backstopTween = _backstopTween;
		if (backstopTween != null)
		{
			backstopTween.Kill();
		}
		_backstopTween = ((Node)this).CreateTween().SetParallel(true);
		_backstopTween.TweenProperty((GodotObject)(object)_slotsContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.1f), 0.4);
		_backstopTween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.5f), 0.4);
		_backButton.Disable();
		_reminderText.AnimateOut();
	}

	public async Task HideBackstopAndShowUi(bool showBackButton)
	{
		_backstopTween?.FastForwardToCompletion();
		_backstopTween = ((Node)this).CreateTween().SetParallel(true);
		_backstopTween.TweenProperty((GodotObject)(object)_slotsContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.4).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_backstopTween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.4).SetEase((EaseType)1).SetTrans((TransitionType)7);
		if (showBackButton)
		{
			RefreshBackButton();
		}
		await ((GodotObject)this).ToSignal((GodotObject)(object)_backstopTween, SignalName.Finished);
		((CanvasItem)_backstop).Visible = false;
	}

	public void OpenQueuedScreen()
	{
		NUnlockScreen nUnlockScreen = _unlockScreens.Dequeue();
		((Node)(object)this).AddChildSafely((Node?)(object)nUnlockScreen);
		nUnlockScreen.Open();
	}

	public bool IsScreenQueued()
	{
		return _unlockScreens.Count > 0;
	}

	private bool IsInspectScreenQueued()
	{
		return _queuedInspectScreen != null;
	}

	private async Task SpawnEraLabel(EpochEra era)
	{
		await _uniqueEpochEras[era].SpawnNameAndYear();
	}

	public void ShowHeaderAndActionsUi()
	{
		if (!_isUiVisible)
		{
			_isUiVisible = true;
		}
	}

	public void DisableInput()
	{
		((CanvasItem)_inputBlocker).Visible = true;
		_inputBlocker.MouseFilter = (MouseFilterEnum)0;
		_slotsContainer.SetEnabled(enabled: false);
		((Node)this).GetViewport().GuiReleaseFocus();
	}

	public void EnableInput()
	{
		if (_queuedInspectScreen == null && _unlockScreens.Count == 0)
		{
			RefreshBackButton();
			((CanvasItem)_inputBlocker).Visible = false;
			_inputBlocker.MouseFilter = (MouseFilterEnum)2;
			_slotsContainer.SetEnabled(enabled: true);
			ActiveScreenContext.Instance.Update();
		}
	}

	private void RefreshBackButton()
	{
		if (SaveManager.Instance.GetDiscoveredEpochCount() > 0)
		{
			if (((Node)_epochSlotContainer).GetChildCount(false) > 1)
			{
				_reminderText.AnimateIn();
			}
			_backButton.Disable();
		}
		else
		{
			_backButton.Enable();
		}
	}

	private void ResetScreen()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Control lineContainer = _lineContainer;
		Color modulate = ((CanvasItem)_lineContainer).Modulate;
		modulate.A = 0f;
		((CanvasItem)lineContainer).Modulate = modulate;
		Log.Info("Cleaning up Timeline screen...");
		_uniqueEpochEras = new Dictionary<EpochEra, NEraColumn>();
		_queuedInspectScreen = null;
		_unlockScreens = new Queue<NUnlockScreen>();
		((Node)(object)_epochSlotContainer).FreeChildren();
		_slotsContainer.Reset();
	}

	public Control GetReminderVfxHolder()
	{
		return _reminderVfxHolder;
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
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Expected O, but got Unknown
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Expected O, but got Unknown
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(22);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnBackButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("obj"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetEraTexturePath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("era"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetSlot, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("era"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("position"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenInspectScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("slot"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)1, StringName.op_Implicit("playAnimation"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.QueueMiscUnlock, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetScreenDraggability, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowBackstopAndHideUi, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenQueuedScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsScreenQueued, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsInspectScreenQueued, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowHeaderAndActionsUi, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnableInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshBackButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ResetScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetReminderVfxHolder, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NTimelineScreen nTimelineScreen = Create();
			ret = VariantUtils.CreateFrom<NTimelineScreen>(ref nTimelineScreen);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuClosed();
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
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnBackButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnBackButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetEraTexturePath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string eraTexturePath = GetEraTexturePath(VariantUtils.ConvertTo<EpochEra>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref eraTexturePath);
			return true;
		}
		if ((ref method) == MethodName.GetSlot && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NEpochSlot slot = GetSlot(VariantUtils.ConvertTo<EpochEra>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NEpochSlot>(ref slot);
			return true;
		}
		if ((ref method) == MethodName.OpenInspectScreen && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			OpenInspectScreen(VariantUtils.ConvertTo<NEpochSlot>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.QueueMiscUnlock && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			QueueMiscUnlock(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetScreenDraggability && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetScreenDraggability();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowBackstopAndHideUi && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowBackstopAndHideUi();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenQueuedScreen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenQueuedScreen();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsScreenQueued && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = IsScreenQueued();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.IsInspectScreenQueued && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag2 = IsInspectScreenQueued();
			ret = VariantUtils.CreateFrom<bool>(ref flag2);
			return true;
		}
		if ((ref method) == MethodName.ShowHeaderAndActionsUi && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowHeaderAndActionsUi();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableInput && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableInput();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnableInput && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnableInput();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshBackButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshBackButton();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ResetScreen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ResetScreen();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetReminderVfxHolder && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Control reminderVfxHolder = GetReminderVfxHolder();
			ret = VariantUtils.CreateFrom<Control>(ref reminderVfxHolder);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NTimelineScreen nTimelineScreen = Create();
			ret = VariantUtils.CreateFrom<NTimelineScreen>(ref nTimelineScreen);
			return true;
		}
		if ((ref method) == MethodName.OnBackButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnBackButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetEraTexturePath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string eraTexturePath = GetEraTexturePath(VariantUtils.ConvertTo<EpochEra>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref eraTexturePath);
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
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed)
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
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnBackButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.GetEraTexturePath)
		{
			return true;
		}
		if ((ref method) == MethodName.GetSlot)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenInspectScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.QueueMiscUnlock)
		{
			return true;
		}
		if ((ref method) == MethodName.SetScreenDraggability)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowBackstopAndHideUi)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenQueuedScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.IsScreenQueued)
		{
			return true;
		}
		if ((ref method) == MethodName.IsInspectScreenQueued)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowHeaderAndActionsUi)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableInput)
		{
			return true;
		}
		if ((ref method) == MethodName.EnableInput)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshBackButton)
		{
			return true;
		}
		if ((ref method) == MethodName.ResetScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.GetReminderVfxHolder)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._inspectScreen)
		{
			_inspectScreen = VariantUtils.ConvertTo<NEpochInspectScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._reminderText)
		{
			_reminderText = VariantUtils.ConvertTo<NEpochReminderText>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._reminderVfxHolder)
		{
			_reminderVfxHolder = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			_backstop = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._inputBlocker)
		{
			_inputBlocker = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lineContainer)
		{
			_lineContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._line)
		{
			_line = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._epochSlotContainer)
		{
			_epochSlotContainer = VariantUtils.ConvertTo<HBoxContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._slotsContainer)
		{
			_slotsContainer = VariantUtils.ConvertTo<NSlotsContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isUiVisible)
		{
			_isUiVisible = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._queuedInspectScreen)
		{
			_queuedInspectScreen = VariantUtils.ConvertTo<NEpochSlot>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lineGrowTween)
		{
			_lineGrowTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstopTween)
		{
			_backstopTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._inspectScreen)
		{
			value = VariantUtils.CreateFrom<NEpochInspectScreen>(ref _inspectScreen);
			return true;
		}
		if ((ref name) == PropertyName._reminderText)
		{
			value = VariantUtils.CreateFrom<NEpochReminderText>(ref _reminderText);
			return true;
		}
		if ((ref name) == PropertyName._reminderVfxHolder)
		{
			value = VariantUtils.CreateFrom<Control>(ref _reminderVfxHolder);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _backstop);
			return true;
		}
		if ((ref name) == PropertyName._inputBlocker)
		{
			value = VariantUtils.CreateFrom<Control>(ref _inputBlocker);
			return true;
		}
		if ((ref name) == PropertyName._lineContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _lineContainer);
			return true;
		}
		if ((ref name) == PropertyName._line)
		{
			value = VariantUtils.CreateFrom<Control>(ref _line);
			return true;
		}
		if ((ref name) == PropertyName._epochSlotContainer)
		{
			value = VariantUtils.CreateFrom<HBoxContainer>(ref _epochSlotContainer);
			return true;
		}
		if ((ref name) == PropertyName._slotsContainer)
		{
			value = VariantUtils.CreateFrom<NSlotsContainer>(ref _slotsContainer);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _backButton);
			return true;
		}
		if ((ref name) == PropertyName._isUiVisible)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isUiVisible);
			return true;
		}
		if ((ref name) == PropertyName._queuedInspectScreen)
		{
			value = VariantUtils.CreateFrom<NEpochSlot>(ref _queuedInspectScreen);
			return true;
		}
		if ((ref name) == PropertyName._lineGrowTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _lineGrowTween);
			return true;
		}
		if ((ref name) == PropertyName._backstopTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _backstopTween);
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
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._inspectScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._reminderText, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._reminderVfxHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._inputBlocker, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lineContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._line, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._epochSlotContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._slotsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isUiVisible, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._queuedInspectScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lineGrowTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstopTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._inspectScreen, Variant.From<NEpochInspectScreen>(ref _inspectScreen));
		info.AddProperty(PropertyName._reminderText, Variant.From<NEpochReminderText>(ref _reminderText));
		info.AddProperty(PropertyName._reminderVfxHolder, Variant.From<Control>(ref _reminderVfxHolder));
		info.AddProperty(PropertyName._backstop, Variant.From<ColorRect>(ref _backstop));
		info.AddProperty(PropertyName._inputBlocker, Variant.From<Control>(ref _inputBlocker));
		info.AddProperty(PropertyName._lineContainer, Variant.From<Control>(ref _lineContainer));
		info.AddProperty(PropertyName._line, Variant.From<Control>(ref _line));
		info.AddProperty(PropertyName._epochSlotContainer, Variant.From<HBoxContainer>(ref _epochSlotContainer));
		info.AddProperty(PropertyName._slotsContainer, Variant.From<NSlotsContainer>(ref _slotsContainer));
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._isUiVisible, Variant.From<bool>(ref _isUiVisible));
		info.AddProperty(PropertyName._queuedInspectScreen, Variant.From<NEpochSlot>(ref _queuedInspectScreen));
		info.AddProperty(PropertyName._lineGrowTween, Variant.From<Tween>(ref _lineGrowTween));
		info.AddProperty(PropertyName._backstopTween, Variant.From<Tween>(ref _backstopTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._inspectScreen, ref val))
		{
			_inspectScreen = ((Variant)(ref val)).As<NEpochInspectScreen>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._reminderText, ref val2))
		{
			_reminderText = ((Variant)(ref val2)).As<NEpochReminderText>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._reminderVfxHolder, ref val3))
		{
			_reminderVfxHolder = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstop, ref val4))
		{
			_backstop = ((Variant)(ref val4)).As<ColorRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._inputBlocker, ref val5))
		{
			_inputBlocker = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._lineContainer, ref val6))
		{
			_lineContainer = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._line, ref val7))
		{
			_line = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._epochSlotContainer, ref val8))
		{
			_epochSlotContainer = ((Variant)(ref val8)).As<HBoxContainer>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._slotsContainer, ref val9))
		{
			_slotsContainer = ((Variant)(ref val9)).As<NSlotsContainer>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val10))
		{
			_backButton = ((Variant)(ref val10)).As<NBackButton>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._isUiVisible, ref val11))
		{
			_isUiVisible = ((Variant)(ref val11)).As<bool>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._queuedInspectScreen, ref val12))
		{
			_queuedInspectScreen = ((Variant)(ref val12)).As<NEpochSlot>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._lineGrowTween, ref val13))
		{
			_lineGrowTween = ((Variant)(ref val13)).As<Tween>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstopTween, ref val14))
		{
			_backstopTween = ((Variant)(ref val14)).As<Tween>();
		}
	}
}
