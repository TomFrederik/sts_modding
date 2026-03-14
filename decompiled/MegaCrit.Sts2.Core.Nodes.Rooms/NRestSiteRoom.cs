using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Rooms;

[ScriptPath("res://src/Core/Nodes/Rooms/NRestSiteRoom.cs")]
public class NRestSiteRoom : Control, IScreenContext, IRoomWithProceedButton
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName DisableOptions = StringName.op_Implicit("DisableOptions");

		public static readonly StringName EnableOptions = StringName.op_Implicit("EnableOptions");

		public static readonly StringName AnimateDescriptionDown = StringName.op_Implicit("AnimateDescriptionDown");

		public static readonly StringName AnimateDescriptionUp = StringName.op_Implicit("AnimateDescriptionUp");

		public static readonly StringName UpdateRestSiteOptions = StringName.op_Implicit("UpdateRestSiteOptions");

		public static readonly StringName RestSiteButtonHovered = StringName.op_Implicit("RestSiteButtonHovered");

		public static readonly StringName RestSiteButtonUnhovered = StringName.op_Implicit("RestSiteButtonUnhovered");

		public static readonly StringName OnPlayerChangedHoveredRestSiteOption = StringName.op_Implicit("OnPlayerChangedHoveredRestSiteOption");

		public static readonly StringName ShowProceedButton = StringName.op_Implicit("ShowProceedButton");

		public static readonly StringName OnProceedButtonReleased = StringName.op_Implicit("OnProceedButtonReleased");

		public static readonly StringName SetText = StringName.op_Implicit("SetText");

		public static readonly StringName FadeOutOptionDescription = StringName.op_Implicit("FadeOutOptionDescription");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName ExtinguishFireIfAble = StringName.op_Implicit("ExtinguishFireIfAble");

		public static readonly StringName OnActiveScreenUpdated = StringName.op_Implicit("OnActiveScreenUpdated");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ProceedButton = StringName.op_Implicit("ProceedButton");

		public static readonly StringName Header = StringName.op_Implicit("Header");

		public static readonly StringName Description = StringName.op_Implicit("Description");

		public static readonly StringName BgContainer = StringName.op_Implicit("BgContainer");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _choicesContainer = StringName.op_Implicit("_choicesContainer");

		public static readonly StringName _choicesScreen = StringName.op_Implicit("_choicesScreen");

		public static readonly StringName _proceedButton = StringName.op_Implicit("_proceedButton");

		public static readonly StringName _restSiteLighting = StringName.op_Implicit("_restSiteLighting");

		public static readonly StringName _descriptionTween = StringName.op_Implicit("_descriptionTween");

		public static readonly StringName _descriptionPositionTween = StringName.op_Implicit("_descriptionPositionTween");

		public static readonly StringName _choicesTween = StringName.op_Implicit("_choicesTween");

		public static readonly StringName _originalDescriptionYPos = StringName.op_Implicit("_originalDescriptionYPos");

		public static readonly StringName _lastFocused = StringName.op_Implicit("_lastFocused");
	}

	public class SignalName : SignalName
	{
	}

	public readonly List<NRestSiteCharacter> characterAnims = new List<NRestSiteCharacter>();

	private const float _lowDescriptionYPos = 885f;

	private const string _scenePath = "res://scenes/rooms/rest_site_room.tscn";

	private static bool _isDebugUiVisible;

	private RestSiteRoom _room;

	private IRunState _runState;

	private Control _choicesContainer;

	private Control _choicesScreen;

	private NProceedButton _proceedButton;

	private Control _restSiteLighting;

	private readonly List<Control> _characterContainers = new List<Control>();

	private readonly CancellationTokenSource _cts = new CancellationTokenSource();

	private Tween? _descriptionTween;

	private Tween? _descriptionPositionTween;

	private Tween? _choicesTween;

	private float _originalDescriptionYPos;

	private Control? _lastFocused;

	public static NRestSiteRoom? Instance => NRun.Instance?.RestSiteRoom;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/rooms/rest_site_room.tscn");

	public NProceedButton ProceedButton => _proceedButton;

	private MegaLabel Header { get; set; }

	private MegaRichTextLabel Description { get; set; }

	private Control BgContainer { get; set; }

	public IReadOnlyList<RestSiteOption> Options => _room.Options;

	public List<NRestSiteCharacter> Characters { get; } = new List<NRestSiteCharacter>();

	public Control? DefaultFocusedControl
	{
		get
		{
			if (_lastFocused != null)
			{
				return _lastFocused;
			}
			if (((Node)_choicesContainer).GetChildCount(false) <= 0)
			{
				return null;
			}
			return (Control?)(object)((Node)_choicesContainer).GetChild<NRestSiteButton>(0, false);
		}
	}

	public static NRestSiteRoom? Create(RestSiteRoom room, IRunState runState)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NRestSiteRoom nRestSiteRoom = PreloadManager.Cache.GetScene("res://scenes/rooms/rest_site_room.tscn").Instantiate<NRestSiteRoom>((GenEditState)0);
		nRestSiteRoom._room = room;
		nRestSiteRoom._runState = runState;
		return nRestSiteRoom;
	}

	public override void _Ready()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		Header = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Header"));
		Description = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Description"));
		((CanvasItem)Description).Modulate = Colors.Transparent;
		_choicesContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ChoicesContainer"));
		_choicesScreen = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ChoicesScreen"));
		_proceedButton = ((Node)this).GetNode<NProceedButton>(NodePath.op_Implicit("%ProceedButton"));
		BgContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("BgContainer"));
		_characterContainers.Add(((Node)this).GetNode<Control>(NodePath.op_Implicit("BgContainer/Character_1")));
		_characterContainers.Add(((Node)this).GetNode<Control>(NodePath.op_Implicit("BgContainer/Character_2")));
		_characterContainers.Add(((Node)this).GetNode<Control>(NodePath.op_Implicit("BgContainer/Character_3")));
		_characterContainers.Add(((Node)this).GetNode<Control>(NodePath.op_Implicit("BgContainer/Character_4")));
		Control val = _runState.Act.CreateRestSiteBackground();
		((Node)(object)BgContainer).AddChildSafely((Node?)(object)val);
		((Node)BgContainer).MoveChild((Node)(object)val, 0);
		_restSiteLighting = ((Node)val).GetNode<Control>(NodePath.op_Implicit("%RestSiteLighting"));
		Header.SetTextAutoSize(new LocString("rest_site_ui", "PROMPT").GetFormattedText());
		((GodotObject)_proceedButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnProceedButtonReleased), 0u);
		_proceedButton.UpdateText(NProceedButton.ProceedLoc);
		if (_isDebugUiVisible)
		{
			((CanvasItem)_choicesScreen).Modulate = Colors.Transparent;
		}
		NGame.Instance.SetScreenShakeTarget((Control)(object)this);
		_proceedButton.Disable();
		for (int i = 0; i < _runState.Players.Count; i++)
		{
			NRestSiteCharacter nRestSiteCharacter = NRestSiteCharacter.Create(_runState.Players[i], i);
			characterAnims.Add(nRestSiteCharacter);
			((Node)(object)_characterContainers[i]).AddChildSafely((Node?)(object)nRestSiteCharacter);
			((Node2D)nRestSiteCharacter).Position = Vector2.Zero;
			if (i % 2 == 1)
			{
				nRestSiteCharacter.FlipX();
			}
			Characters.Add(nRestSiteCharacter);
		}
		_originalDescriptionYPos = ((Control)Description).Position.Y;
		UpdateRestSiteOptions();
		TaskHelper.RunSafely(ShowFtueIfNeeded());
		RunManager.Instance.RestSiteSynchronizer.PlayerHoverChanged += OnPlayerChangedHoveredRestSiteOption;
		RunManager.Instance.RestSiteSynchronizer.BeforePlayerOptionChosen += OnBeforePlayerSelectedRestSiteOption;
		RunManager.Instance.RestSiteSynchronizer.AfterPlayerOptionChosen += OnAfterPlayerSelectedRestSiteOption;
	}

	public override void _EnterTree()
	{
		ActiveScreenContext.Instance.Updated += OnActiveScreenUpdated;
	}

	public override void _ExitTree()
	{
		_cts.Cancel();
		_cts.Dispose();
		Tween? descriptionTween = _descriptionTween;
		if (descriptionTween != null)
		{
			descriptionTween.Kill();
		}
		Tween? descriptionPositionTween = _descriptionPositionTween;
		if (descriptionPositionTween != null)
		{
			descriptionPositionTween.Kill();
		}
		Tween? choicesTween = _choicesTween;
		if (choicesTween != null)
		{
			choicesTween.Kill();
		}
		RunManager.Instance.RestSiteSynchronizer.PlayerHoverChanged -= OnPlayerChangedHoveredRestSiteOption;
		RunManager.Instance.RestSiteSynchronizer.BeforePlayerOptionChosen -= OnBeforePlayerSelectedRestSiteOption;
		RunManager.Instance.RestSiteSynchronizer.AfterPlayerOptionChosen -= OnAfterPlayerSelectedRestSiteOption;
		ActiveScreenContext.Instance.Updated -= OnActiveScreenUpdated;
	}

	public void AfterSelectingOption(RestSiteOption option)
	{
		TaskHelper.RunSafely(AfterSelectingOptionAsync(option));
	}

	private async Task ShowFtueIfNeeded()
	{
		if (!SaveManager.Instance.SeenFtue("rest_site_ftue"))
		{
			((CanvasItem)_choicesContainer).Visible = false;
			((CanvasItem)Header).Visible = false;
			await Cmd.Wait(0.5f, _cts.Token);
			((CanvasItem)_choicesContainer).Visible = true;
			((CanvasItem)Header).Visible = true;
			Control choicesContainer = _choicesContainer;
			Color modulate = ((CanvasItem)_choicesContainer).Modulate;
			modulate.A = 0f;
			((CanvasItem)choicesContainer).Modulate = modulate;
			MegaLabel header = Header;
			modulate = ((CanvasItem)Header).Modulate;
			modulate.A = 0f;
			((CanvasItem)header).Modulate = modulate;
			Tween val = ((Node)this).CreateTween();
			val.TweenProperty((GodotObject)(object)_choicesContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5);
			val.TweenProperty((GodotObject)(object)Header, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5);
			NModalContainer.Instance.Add((Node)(object)NRestSiteFtue.Create(_choicesContainer));
			SaveManager.Instance.MarkFtueAsComplete("rest_site_ftue");
		}
	}

	public void DisableOptions()
	{
		foreach (NRestSiteButton item in ((IEnumerable)((Node)_choicesContainer).GetChildren(false)).OfType<NRestSiteButton>())
		{
			item.Disable();
		}
	}

	public void EnableOptions()
	{
		foreach (NRestSiteButton item in ((IEnumerable)((Node)_choicesContainer).GetChildren(false)).OfType<NRestSiteButton>())
		{
			item.Enable();
		}
		ActiveScreenContext.Instance.Update();
	}

	public void AnimateDescriptionDown()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Tween? descriptionPositionTween = _descriptionPositionTween;
		if (descriptionPositionTween != null)
		{
			descriptionPositionTween.Kill();
		}
		_descriptionPositionTween = ((Node)this).CreateTween();
		_descriptionPositionTween.TweenProperty((GodotObject)(object)Description, NodePath.op_Implicit("position:y"), Variant.op_Implicit(885f), 0.800000011920929).SetTrans((TransitionType)5).SetEase((EaseType)1);
	}

	public void AnimateDescriptionUp()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Tween? descriptionPositionTween = _descriptionPositionTween;
		if (descriptionPositionTween != null)
		{
			descriptionPositionTween.Kill();
		}
		_descriptionPositionTween = ((Node)this).CreateTween();
		_descriptionPositionTween.TweenProperty((GodotObject)(object)Description, NodePath.op_Implicit("position:y"), Variant.op_Implicit(_originalDescriptionYPos), 0.800000011920929).SetTrans((TransitionType)5).SetEase((EaseType)1);
	}

	private void UpdateRestSiteOptions()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (!((Node?)(object)this).IsValid() || !((Node)this).IsInsideTree())
		{
			return;
		}
		foreach (Node child in ((Node)_choicesContainer).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
		foreach (RestSiteOption option in Options)
		{
			NRestSiteButton nRestSiteButton = NRestSiteButton.Create(option);
			((Node)(object)_choicesContainer).AddChildSafely((Node?)(object)nRestSiteButton);
			((GodotObject)nRestSiteButton).Connect(NClickableControl.SignalName.Focused, Callable.From<NRestSiteButton>((Action<NRestSiteButton>)RestSiteButtonHovered), 0u);
			((GodotObject)nRestSiteButton).Connect(NClickableControl.SignalName.Unfocused, Callable.From<NRestSiteButton>((Action<NRestSiteButton>)RestSiteButtonUnhovered), 0u);
		}
	}

	private void RestSiteButtonHovered(NRestSiteButton button)
	{
		RunManager.Instance.RestSiteSynchronizer.LocalOptionHovered(button.Option);
		_lastFocused = (Control?)(object)button;
	}

	private void RestSiteButtonUnhovered(NRestSiteButton button)
	{
		RunManager.Instance.RestSiteSynchronizer.LocalOptionHovered(null);
	}

	private void OnPlayerChangedHoveredRestSiteOption(ulong playerId)
	{
		if (_runState.Players.Count > 1)
		{
			NRestSiteCharacter nRestSiteCharacter = Characters.First((NRestSiteCharacter c) => c.Player.NetId == playerId);
			int? hoveredOptionIndex = RunManager.Instance.RestSiteSynchronizer.GetHoveredOptionIndex(playerId);
			RestSiteOption option = ((!hoveredOptionIndex.HasValue) ? null : RunManager.Instance.RestSiteSynchronizer.GetOptionsForPlayer(playerId)[hoveredOptionIndex.Value]);
			nRestSiteCharacter.ShowHoveredRestSiteOption(option);
		}
	}

	private void OnBeforePlayerSelectedRestSiteOption(RestSiteOption option, ulong playerId)
	{
		if (_runState.Players.Count > 1)
		{
			NRestSiteCharacter nRestSiteCharacter = Characters.First((NRestSiteCharacter c) => c.Player.NetId == playerId);
			nRestSiteCharacter.SetSelectingRestSiteOption(option);
		}
	}

	private void OnAfterPlayerSelectedRestSiteOption(RestSiteOption option, bool success, ulong playerId)
	{
		if (_runState.Players.Count <= 1)
		{
			return;
		}
		NRestSiteCharacter nRestSiteCharacter = Characters.First((NRestSiteCharacter c) => c.Player.NetId == playerId);
		nRestSiteCharacter.SetSelectingRestSiteOption(null);
		if (success)
		{
			nRestSiteCharacter.ShowSelectedRestSiteOption(option);
			if (!LocalContext.IsMe(nRestSiteCharacter.Player))
			{
				TaskHelper.RunSafely(option.DoRemotePostSelectVfx());
			}
		}
	}

	public NRestSiteButton? GetButtonForOption(RestSiteOption option)
	{
		foreach (NRestSiteButton item in ((IEnumerable)((Node)_choicesContainer).GetChildren(false)).OfType<NRestSiteButton>())
		{
			if (item.Option == option)
			{
				return item;
			}
		}
		return null;
	}

	public NRestSiteCharacter? GetCharacterForPlayer(Player player)
	{
		foreach (NRestSiteCharacter characterAnim in characterAnims)
		{
			if (characterAnim.Player == player)
			{
				return characterAnim;
			}
		}
		return null;
	}

	private async Task AfterSelectingOptionAsync(RestSiteOption option)
	{
		Task task = HideChoices(_cts.Token);
		Task task2 = option.DoLocalPostSelectVfx(_cts.Token);
		ExtinguishFireIfAble();
		global::_003C_003Ey__InlineArray2<Task> buffer = default(global::_003C_003Ey__InlineArray2<Task>);
		buffer[0] = task;
		buffer[1] = task2;
		await Task.WhenAll(buffer);
		UpdateRestSiteOptions();
		ShowProceedButton();
		if (Options.Count > 0)
		{
			await ShowChoices(_cts.Token);
			ActiveScreenContext.Instance.Update();
		}
	}

	private void ShowProceedButton()
	{
		if (!_proceedButton.IsEnabled)
		{
			_proceedButton.Enable();
			NMapScreen.Instance.SetTravelEnabled(enabled: true);
		}
	}

	private void OnProceedButtonReleased(NButton _)
	{
		NMapScreen.Instance.Open();
	}

	public void SetText(string formattedText)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Tween? descriptionTween = _descriptionTween;
		if (descriptionTween != null)
		{
			descriptionTween.Kill();
		}
		((CanvasItem)Description).Modulate = Colors.White;
		Description.Text = formattedText;
	}

	public void FadeOutOptionDescription()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Tween? descriptionTween = _descriptionTween;
		if (descriptionTween != null)
		{
			descriptionTween.Kill();
		}
		_descriptionTween = ((Node)this).CreateTween();
		_descriptionTween.TweenProperty((GodotObject)(object)Description, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(1f));
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (inputEvent.IsActionReleased(DebugHotkey.hideRestSite, false))
		{
			_isDebugUiVisible = !_isDebugUiVisible;
			((CanvasItem)_choicesScreen).Modulate = (_isDebugUiVisible ? Colors.Transparent : Colors.White);
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(_isDebugUiVisible ? "Hide RestSite UI" : "Show RestSite UI"));
		}
	}

	private void ExtinguishFireIfAble()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (RunManager.Instance.RestSiteSynchronizer.GetLocalOptions().Count > 0 || !((CanvasItem)_restSiteLighting).Visible)
		{
			return;
		}
		foreach (NRestSiteCharacter characterAnim in characterAnims)
		{
			characterAnim.HideFlameGlow();
		}
		foreach (Control characterContainer in _characterContainers)
		{
			((CanvasItem)characterContainer).Modulate = Colors.DarkGray;
		}
		((CanvasItem)_restSiteLighting).Visible = false;
		NRunMusicController.Instance?.TriggerCampfireGoingOut();
	}

	private async Task ShowChoices(CancellationToken ct)
	{
		Tween? choicesTween = _choicesTween;
		if (choicesTween != null)
		{
			choicesTween.Kill();
		}
		_choicesTween = ((Node)this).CreateTween();
		_choicesTween.TweenProperty((GodotObject)(object)_choicesScreen, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		await _choicesTween.AwaitFinished(ct);
	}

	private async Task HideChoices(CancellationToken ct)
	{
		foreach (NButton item in ((IEnumerable)((Node)_choicesContainer).GetChildren(false)).OfType<NButton>())
		{
			item.Disable();
		}
		Tween? choicesTween = _choicesTween;
		if (choicesTween != null)
		{
			choicesTween.Kill();
		}
		_choicesTween = ((Node)this).CreateTween();
		_choicesTween.TweenProperty((GodotObject)(object)_choicesScreen, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
		_lastFocused = null;
		await _choicesTween.AwaitFinished(ct);
	}

	private void OnActiveScreenUpdated()
	{
		this.UpdateControllerNavEnabled();
		if (ActiveScreenContext.Instance.IsCurrent(this) && Options.Count == 0)
		{
			ShowProceedButton();
		}
		else if (!_proceedButton.IsEnabled)
		{
			_proceedButton.Disable();
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
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Expected O, but got Unknown
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Expected O, but got Unknown
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Expected O, but got Unknown
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(18);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableOptions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnableOptions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateDescriptionDown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateDescriptionUp, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateRestSiteOptions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RestSiteButtonHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RestSiteButtonUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlayerChangedHoveredRestSiteOption, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowProceedButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnProceedButtonReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("formattedText"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FadeOutOptionDescription, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ExtinguishFireIfAble, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnActiveScreenUpdated, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.DisableOptions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableOptions();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnableOptions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnableOptions();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateDescriptionDown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateDescriptionDown();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateDescriptionUp && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateDescriptionUp();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateRestSiteOptions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateRestSiteOptions();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RestSiteButtonHovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RestSiteButtonHovered(VariantUtils.ConvertTo<NRestSiteButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RestSiteButtonUnhovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RestSiteButtonUnhovered(VariantUtils.ConvertTo<NRestSiteButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlayerChangedHoveredRestSiteOption && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPlayerChangedHoveredRestSiteOption(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowProceedButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowProceedButton();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnProceedButtonReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnProceedButtonReleased(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetText && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetText(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FadeOutOptionDescription && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			FadeOutOptionDescription();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ExtinguishFireIfAble && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ExtinguishFireIfAble();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenUpdated && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnActiveScreenUpdated();
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
		if ((ref method) == MethodName.DisableOptions)
		{
			return true;
		}
		if ((ref method) == MethodName.EnableOptions)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateDescriptionDown)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateDescriptionUp)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateRestSiteOptions)
		{
			return true;
		}
		if ((ref method) == MethodName.RestSiteButtonHovered)
		{
			return true;
		}
		if ((ref method) == MethodName.RestSiteButtonUnhovered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPlayerChangedHoveredRestSiteOption)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowProceedButton)
		{
			return true;
		}
		if ((ref method) == MethodName.OnProceedButtonReleased)
		{
			return true;
		}
		if ((ref method) == MethodName.SetText)
		{
			return true;
		}
		if ((ref method) == MethodName.FadeOutOptionDescription)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.ExtinguishFireIfAble)
		{
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenUpdated)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Header)
		{
			Header = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Description)
		{
			Description = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.BgContainer)
		{
			BgContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._choicesContainer)
		{
			_choicesContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._choicesScreen)
		{
			_choicesScreen = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			_proceedButton = VariantUtils.ConvertTo<NProceedButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._restSiteLighting)
		{
			_restSiteLighting = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._descriptionTween)
		{
			_descriptionTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._descriptionPositionTween)
		{
			_descriptionPositionTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._choicesTween)
		{
			_choicesTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalDescriptionYPos)
		{
			_originalDescriptionYPos = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastFocused)
		{
			_lastFocused = VariantUtils.ConvertTo<Control>(ref value);
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
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ProceedButton)
		{
			NProceedButton proceedButton = ProceedButton;
			value = VariantUtils.CreateFrom<NProceedButton>(ref proceedButton);
			return true;
		}
		if ((ref name) == PropertyName.Header)
		{
			MegaLabel header = Header;
			value = VariantUtils.CreateFrom<MegaLabel>(ref header);
			return true;
		}
		if ((ref name) == PropertyName.Description)
		{
			MegaRichTextLabel description = Description;
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref description);
			return true;
		}
		if ((ref name) == PropertyName.BgContainer)
		{
			Control bgContainer = BgContainer;
			value = VariantUtils.CreateFrom<Control>(ref bgContainer);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control bgContainer = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref bgContainer);
			return true;
		}
		if ((ref name) == PropertyName._choicesContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _choicesContainer);
			return true;
		}
		if ((ref name) == PropertyName._choicesScreen)
		{
			value = VariantUtils.CreateFrom<Control>(ref _choicesScreen);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			value = VariantUtils.CreateFrom<NProceedButton>(ref _proceedButton);
			return true;
		}
		if ((ref name) == PropertyName._restSiteLighting)
		{
			value = VariantUtils.CreateFrom<Control>(ref _restSiteLighting);
			return true;
		}
		if ((ref name) == PropertyName._descriptionTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _descriptionTween);
			return true;
		}
		if ((ref name) == PropertyName._descriptionPositionTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _descriptionPositionTween);
			return true;
		}
		if ((ref name) == PropertyName._choicesTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _choicesTween);
			return true;
		}
		if ((ref name) == PropertyName._originalDescriptionYPos)
		{
			value = VariantUtils.CreateFrom<float>(ref _originalDescriptionYPos);
			return true;
		}
		if ((ref name) == PropertyName._lastFocused)
		{
			value = VariantUtils.CreateFrom<Control>(ref _lastFocused);
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
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._choicesContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._choicesScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._proceedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.ProceedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._restSiteLighting, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._descriptionTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._descriptionPositionTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._choicesTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._originalDescriptionYPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lastFocused, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Header, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Description, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.BgContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName header = PropertyName.Header;
		MegaLabel header2 = Header;
		info.AddProperty(header, Variant.From<MegaLabel>(ref header2));
		StringName description = PropertyName.Description;
		MegaRichTextLabel description2 = Description;
		info.AddProperty(description, Variant.From<MegaRichTextLabel>(ref description2));
		StringName bgContainer = PropertyName.BgContainer;
		Control bgContainer2 = BgContainer;
		info.AddProperty(bgContainer, Variant.From<Control>(ref bgContainer2));
		info.AddProperty(PropertyName._choicesContainer, Variant.From<Control>(ref _choicesContainer));
		info.AddProperty(PropertyName._choicesScreen, Variant.From<Control>(ref _choicesScreen));
		info.AddProperty(PropertyName._proceedButton, Variant.From<NProceedButton>(ref _proceedButton));
		info.AddProperty(PropertyName._restSiteLighting, Variant.From<Control>(ref _restSiteLighting));
		info.AddProperty(PropertyName._descriptionTween, Variant.From<Tween>(ref _descriptionTween));
		info.AddProperty(PropertyName._descriptionPositionTween, Variant.From<Tween>(ref _descriptionPositionTween));
		info.AddProperty(PropertyName._choicesTween, Variant.From<Tween>(ref _choicesTween));
		info.AddProperty(PropertyName._originalDescriptionYPos, Variant.From<float>(ref _originalDescriptionYPos));
		info.AddProperty(PropertyName._lastFocused, Variant.From<Control>(ref _lastFocused));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Header, ref val))
		{
			Header = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.Description, ref val2))
		{
			Description = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.BgContainer, ref val3))
		{
			BgContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._choicesContainer, ref val4))
		{
			_choicesContainer = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._choicesScreen, ref val5))
		{
			_choicesScreen = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._proceedButton, ref val6))
		{
			_proceedButton = ((Variant)(ref val6)).As<NProceedButton>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._restSiteLighting, ref val7))
		{
			_restSiteLighting = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._descriptionTween, ref val8))
		{
			_descriptionTween = ((Variant)(ref val8)).As<Tween>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._descriptionPositionTween, ref val9))
		{
			_descriptionPositionTween = ((Variant)(ref val9)).As<Tween>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._choicesTween, ref val10))
		{
			_choicesTween = ((Variant)(ref val10)).As<Tween>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalDescriptionYPos, ref val11))
		{
			_originalDescriptionYPos = ((Variant)(ref val11)).As<float>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastFocused, ref val12))
		{
			_lastFocused = ((Variant)(ref val12)).As<Control>();
		}
	}
}
