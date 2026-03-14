using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;

[ScriptPath("res://src/Core/Nodes/Screens/CardSelection/NCardRewardSelectionScreen.cs")]
public class NCardRewardSelectionScreen : Control, IOverlayScreen, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnAlternateRewardSelected = StringName.op_Implicit("OnAlternateRewardSelected");

		public static readonly StringName SelectCard = StringName.op_Implicit("SelectCard");

		public static readonly StringName InspectCard = StringName.op_Implicit("InspectCard");

		public static readonly StringName AfterOverlayOpened = StringName.op_Implicit("AfterOverlayOpened");

		public static readonly StringName PowerCardFtueCheck = StringName.op_Implicit("PowerCardFtueCheck");

		public static readonly StringName AfterOverlayClosed = StringName.op_Implicit("AfterOverlayClosed");

		public static readonly StringName AfterOverlayShown = StringName.op_Implicit("AfterOverlayShown");

		public static readonly StringName AfterOverlayHidden = StringName.op_Implicit("AfterOverlayHidden");

		public static readonly StringName UpdateControllerIcons = StringName.op_Implicit("UpdateControllerIcons");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public static readonly StringName UseSharedBackstop = StringName.op_Implicit("UseSharedBackstop");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _ui = StringName.op_Implicit("_ui");

		public static readonly StringName _banner = StringName.op_Implicit("_banner");

		public static readonly StringName _cardRow = StringName.op_Implicit("_cardRow");

		public static readonly StringName _rewardAlternativesContainer = StringName.op_Implicit("_rewardAlternativesContainer");

		public static readonly StringName _inspectPrompt = StringName.op_Implicit("_inspectPrompt");

		public static readonly StringName _cardTween = StringName.op_Implicit("_cardTween");

		public static readonly StringName _buttonTween = StringName.op_Implicit("_buttonTween");

		public static readonly StringName _lastFocusedControl = StringName.op_Implicit("_lastFocusedControl");
	}

	public class SignalName : SignalName
	{
	}

	private const ulong _noSelectionTimeMsec = 350uL;

	private Control _ui;

	private NCommonBanner _banner;

	private Control _cardRow;

	private IReadOnlyList<CardCreationResult> _options;

	private IReadOnlyList<CardRewardAlternative> _extraOptions;

	private Control _rewardAlternativesContainer;

	private Control _inspectPrompt;

	private TaskCompletionSource<Tuple<IEnumerable<NCardHolder>, bool>>? _completionSource;

	private Tween? _cardTween;

	private Tween? _buttonTween;

	private const float _cardXOffset = 350f;

	private static readonly Vector2 _bannerAnimPosOffset = new Vector2(0f, 50f);

	private Control? _lastFocusedControl;

	private static string ScenePath => SceneHelper.GetScenePath("screens/card_selection/card_reward_selection_screen");

	public static IEnumerable<string> AssetPaths => new string[1] { ScenePath }.Concat(NCardRewardAlternativeButton.AssetPaths);

	public NetScreenType ScreenType => NetScreenType.CardSelection;

	public bool UseSharedBackstop => true;

	public Control DefaultFocusedControl
	{
		get
		{
			if (_lastFocusedControl != null)
			{
				return _lastFocusedControl;
			}
			List<NGridCardHolder> list = ((IEnumerable)((Node)_cardRow).GetChildren(false)).OfType<NGridCardHolder>().ToList();
			return (Control)(object)list[list.Count / 2];
		}
	}

	public static NCardRewardSelectionScreen? ShowScreen(IReadOnlyList<CardCreationResult> options, IReadOnlyList<CardRewardAlternative> extraOptions)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NCardRewardSelectionScreen nCardRewardSelectionScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCardRewardSelectionScreen>((GenEditState)0);
		((Node)nCardRewardSelectionScreen).Name = StringName.op_Implicit("NCardRewardSelectionScreen");
		nCardRewardSelectionScreen._options = options;
		nCardRewardSelectionScreen._extraOptions = extraOptions;
		NOverlayStack.Instance.Push(nCardRewardSelectionScreen);
		return nCardRewardSelectionScreen;
	}

	public override void _Ready()
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		_ui = ((Node)this).GetNode<Control>(NodePath.op_Implicit("UI"));
		_cardRow = ((Node)this).GetNode<Control>(NodePath.op_Implicit("UI/CardRow"));
		_banner = ((Node)this).GetNode<NCommonBanner>(NodePath.op_Implicit("UI/Banner"));
		_rewardAlternativesContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("UI/RewardAlternatives"));
		_inspectPrompt = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%InspectPrompt"));
		_banner.label.SetTextAutoSize(new LocString("gameplay_ui", "CHOOSE_CARD_HEADER").GetRawText());
		_banner.AnimateIn();
		RefreshOptions(_options, _extraOptions);
		UpdateControllerIcons();
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)UpdateControllerIcons), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)UpdateControllerIcons), 0u);
		((GodotObject)NInputManager.Instance).Connect(NInputManager.SignalName.InputRebound, Callable.From((Action)UpdateControllerIcons), 0u);
	}

	public void RefreshOptions(IReadOnlyList<CardCreationResult> options, IReadOnlyList<CardRewardAlternative> extraOptions)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		_options = options;
		_extraOptions = extraOptions;
		Vector2 val = Vector2.Left * (float)(_options.Count - 1) * 350f * 0.5f;
		_lastFocusedControl = null;
		foreach (NGridCardHolder item in ((IEnumerable)((Node)_cardRow).GetChildren(false)).OfType<NGridCardHolder>())
		{
			((Node)(object)item).QueueFreeSafely();
		}
		foreach (NCardRewardAlternativeButton item2 in ((IEnumerable)((Node)_rewardAlternativesContainer).GetChildren(false)).OfType<NCardRewardAlternativeButton>())
		{
			((Node)(object)item2).QueueFreeSafely();
		}
		_cardTween = ((Node)this).CreateTween().SetParallel(true);
		for (int i = 0; i < _options.Count; i++)
		{
			CardCreationResult cardCreationResult = _options[i];
			NCard nCard = NCard.Create(cardCreationResult.Card);
			NGridCardHolder holder = NGridCardHolder.Create(nCard);
			((Node)(object)_cardRow).AddChildSafely((Node?)(object)holder);
			((GodotObject)holder).Connect(NCardHolder.SignalName.Pressed, Callable.From<NCardHolder>((Action<NCardHolder>)SelectCard), 0u);
			((GodotObject)holder).Connect(NCardHolder.SignalName.AltPressed, Callable.From<NCardHolder>((Action<NCardHolder>)InspectCard), 0u);
			((GodotObject)holder).Connect(SignalName.FocusEntered, Callable.From<Control>((Func<Control>)(() => _lastFocusedControl = (Control?)(object)holder)), 0u);
			nCard.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
			((Control)holder).Scale = holder.SmallScale;
			_cardTween.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("position"), Variant.op_Implicit(val + Vector2.Right * 350f * (float)i), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_cardTween.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7)
				.From(Variant.op_Implicit(Colors.Black));
			nCard.ActivateRewardScreenGlow();
			foreach (RelicModel modifyingRelic in cardCreationResult.ModifyingRelics)
			{
				modifyingRelic.Flash();
				nCard.FlashRelicOnCard(modifyingRelic);
			}
		}
		foreach (CardRewardAlternative rewardOption in _extraOptions)
		{
			NCardRewardAlternativeButton nCardRewardAlternativeButton = NCardRewardAlternativeButton.Create(rewardOption.Title.GetFormattedText(), rewardOption.Hotkey);
			((Node)(object)_rewardAlternativesContainer).AddChildSafely((Node?)(object)nCardRewardAlternativeButton);
			((GodotObject)nCardRewardAlternativeButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
			{
				OnAlternateRewardSelected(rewardOption.AfterSelected);
				TaskHelper.RunSafely(rewardOption.OnSelect());
			}), 0u);
		}
		List<NGridCardHolder> list = ((IEnumerable)((Node)_cardRow).GetChildren(false)).OfType<NGridCardHolder>().ToList();
		for (int num = 0; num < ((Node)_cardRow).GetChildCount(false); num++)
		{
			Control child = ((Node)_cardRow).GetChild<Control>(num, false);
			child.FocusNeighborBottom = ((Node)child).GetPath();
			child.FocusNeighborTop = ((Node)child).GetPath();
			child.FocusNeighborLeft = ((num > 0) ? ((Node)_cardRow).GetChild(num - 1, false).GetPath() : ((Node)_cardRow).GetChild(((Node)_cardRow).GetChildCount(false) - 1, false).GetPath());
			child.FocusNeighborRight = ((num < ((Node)_cardRow).GetChildCount(false) - 1) ? ((Node)_cardRow).GetChild(num + 1, false).GetPath() : ((Node)_cardRow).GetChild(0, false).GetPath());
		}
		if (NControllerManager.Instance.IsUsingController)
		{
			((Control)(object)list[list.Count / 2]).TryGrabFocus();
		}
	}

	public override void _ExitTree()
	{
		TaskCompletionSource<Tuple<IEnumerable<NCardHolder>, bool>> completionSource = _completionSource;
		if (completionSource != null)
		{
			Task<Tuple<IEnumerable<NCardHolder>, bool>> task = completionSource.Task;
			if (task != null && !task.IsCompleted)
			{
				_completionSource.SetResult(new Tuple<IEnumerable<NCardHolder>, bool>(Array.Empty<NCardHolder>(), item2: false));
			}
		}
		foreach (NGridCardHolder item in ((IEnumerable)((Node)_cardRow).GetChildren(false)).OfType<NGridCardHolder>())
		{
			((Node)(object)item).QueueFreeSafely();
		}
	}

	private void OnAlternateRewardSelected(PostAlternateCardRewardAction afterSelected)
	{
		if ((afterSelected != PostAlternateCardRewardAction.None && afterSelected != PostAlternateCardRewardAction.DoNothing) || 1 == 0)
		{
			_completionSource?.SetResult(new Tuple<IEnumerable<NCardHolder>, bool>(Array.Empty<NCardHolder>(), afterSelected == PostAlternateCardRewardAction.DismissScreenAndRemoveReward));
		}
	}

	private void SelectCard(NCardHolder cardHolder)
	{
		if (_completionSource == null)
		{
			throw new InvalidOperationException("CardsSelected must be awaited before a card is selected!");
		}
		_completionSource.SetResult(new Tuple<IEnumerable<NCardHolder>, bool>(new _003C_003Ez__ReadOnlySingleElementList<NCardHolder>(cardHolder), item2: true));
	}

	private void InspectCard(NCardHolder cardHolder)
	{
		if (!_completionSource.Task.IsCompleted)
		{
			NInspectCardScreen inspectCardScreen = NGame.Instance.GetInspectCardScreen();
			int num = 1;
			List<CardModel> list = new List<CardModel>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<CardModel> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = cardHolder.CardNode.Model;
			inspectCardScreen.Open(list, 0);
		}
	}

	public async Task<Tuple<IEnumerable<NCardHolder>, bool>> CardsSelected()
	{
		_completionSource = new TaskCompletionSource<Tuple<IEnumerable<NCardHolder>, bool>>();
		return await _completionSource.Task;
	}

	public void AfterOverlayOpened()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		PowerCardFtueCheck();
		_banner.AnimateIn();
		_buttonTween = ((Node)this).CreateTween();
		_buttonTween.SetParallel(true);
		_buttonTween.TweenProperty((GodotObject)(object)_rewardAlternativesContainer, NodePath.op_Implicit("position"), Variant.op_Implicit(_rewardAlternativesContainer.Position), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(_rewardAlternativesContainer.Position - _bannerAnimPosOffset));
		TaskHelper.RunSafely(DisableCardsForShortTimeAfterOpening());
	}

	private async Task DisableCardsForShortTimeAfterOpening()
	{
		foreach (NGridCardHolder item in ((IEnumerable)((Node)_cardRow).GetChildren(false)).OfType<NGridCardHolder>())
		{
			item.SetClickable(isClickable: false);
		}
		await Cmd.Wait(0.35f);
		if (!((Node?)(object)_cardRow).IsValid())
		{
			return;
		}
		foreach (NGridCardHolder item2 in ((IEnumerable)((Node)_cardRow).GetChildren(false)).OfType<NGridCardHolder>())
		{
			item2.SetClickable(isClickable: true);
		}
	}

	private void PowerCardFtueCheck()
	{
		if (!SaveManager.Instance.SeenFtue("power_card_ftue"))
		{
			IEnumerable<NGridCardHolder> source = ((IEnumerable)((Node)_cardRow).GetChildren(false)).OfType<NGridCardHolder>();
			NGridCardHolder nGridCardHolder = source.FirstOrDefault((NGridCardHolder h) => h.CardModel.Type == CardType.Power);
			if (nGridCardHolder != null)
			{
				NModalContainer.Instance.Add((Node)(object)NPowerCardFtue.Create((Control)(object)nGridCardHolder));
				SaveManager.Instance.MarkFtueAsComplete("power_card_ftue");
			}
		}
	}

	public void AfterOverlayClosed()
	{
		((Node)(object)this).QueueFreeSafely();
	}

	public void AfterOverlayShown()
	{
		((CanvasItem)this).Visible = true;
	}

	public void AfterOverlayHidden()
	{
		((CanvasItem)this).Visible = false;
	}

	private void UpdateControllerIcons()
	{
		((CanvasItem)_inspectPrompt).Visible = NControllerManager.Instance.IsUsingController;
		((Node)_inspectPrompt).GetNode<TextureRect>(NodePath.op_Implicit("ControllerIcon")).Texture = NInputManager.Instance.GetHotkeyIcon(StringName.op_Implicit(MegaInput.accept));
		((Node)_inspectPrompt).GetNode<MegaLabel>(NodePath.op_Implicit("Label")).SetTextAutoSize(new LocString("gameplay_ui", "TO_INSPECT_PROMPT").GetFormattedText());
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
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAlternateRewardSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("afterSelected"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SelectCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InspectCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PowerCardFtueCheck, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateControllerIcons, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnAlternateRewardSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnAlternateRewardSelected(VariantUtils.ConvertTo<PostAlternateCardRewardAction>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SelectCard && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SelectCard(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InspectCard && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			InspectCard(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PowerCardFtueCheck && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PowerCardFtueCheck();
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
		if ((ref method) == MethodName.UpdateControllerIcons && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateControllerIcons();
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
		if ((ref method) == MethodName.OnAlternateRewardSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.SelectCard)
		{
			return true;
		}
		if ((ref method) == MethodName.InspectCard)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.PowerCardFtueCheck)
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
		if ((ref method) == MethodName.UpdateControllerIcons)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._ui)
		{
			_ui = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			_banner = VariantUtils.ConvertTo<NCommonBanner>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardRow)
		{
			_cardRow = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rewardAlternativesContainer)
		{
			_rewardAlternativesContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._inspectPrompt)
		{
			_inspectPrompt = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			_cardTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._buttonTween)
		{
			_buttonTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastFocusedControl)
		{
			_lastFocusedControl = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName._ui)
		{
			value = VariantUtils.CreateFrom<Control>(ref _ui);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			value = VariantUtils.CreateFrom<NCommonBanner>(ref _banner);
			return true;
		}
		if ((ref name) == PropertyName._cardRow)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cardRow);
			return true;
		}
		if ((ref name) == PropertyName._rewardAlternativesContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _rewardAlternativesContainer);
			return true;
		}
		if ((ref name) == PropertyName._inspectPrompt)
		{
			value = VariantUtils.CreateFrom<Control>(ref _inspectPrompt);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _cardTween);
			return true;
		}
		if ((ref name) == PropertyName._buttonTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _buttonTween);
			return true;
		}
		if ((ref name) == PropertyName._lastFocusedControl)
		{
			value = VariantUtils.CreateFrom<Control>(ref _lastFocusedControl);
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
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._ui, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._banner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardRow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rewardAlternativesContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._inspectPrompt, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._buttonTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lastFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._ui, Variant.From<Control>(ref _ui));
		info.AddProperty(PropertyName._banner, Variant.From<NCommonBanner>(ref _banner));
		info.AddProperty(PropertyName._cardRow, Variant.From<Control>(ref _cardRow));
		info.AddProperty(PropertyName._rewardAlternativesContainer, Variant.From<Control>(ref _rewardAlternativesContainer));
		info.AddProperty(PropertyName._inspectPrompt, Variant.From<Control>(ref _inspectPrompt));
		info.AddProperty(PropertyName._cardTween, Variant.From<Tween>(ref _cardTween));
		info.AddProperty(PropertyName._buttonTween, Variant.From<Tween>(ref _buttonTween));
		info.AddProperty(PropertyName._lastFocusedControl, Variant.From<Control>(ref _lastFocusedControl));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._ui, ref val))
		{
			_ui = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._banner, ref val2))
		{
			_banner = ((Variant)(ref val2)).As<NCommonBanner>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardRow, ref val3))
		{
			_cardRow = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._rewardAlternativesContainer, ref val4))
		{
			_rewardAlternativesContainer = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._inspectPrompt, ref val5))
		{
			_inspectPrompt = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardTween, ref val6))
		{
			_cardTween = ((Variant)(ref val6)).As<Tween>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._buttonTween, ref val7))
		{
			_buttonTween = ((Variant)(ref val7)).As<Tween>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastFocusedControl, ref val8))
		{
			_lastFocusedControl = ((Variant)(ref val8)).As<Control>();
		}
	}
}
