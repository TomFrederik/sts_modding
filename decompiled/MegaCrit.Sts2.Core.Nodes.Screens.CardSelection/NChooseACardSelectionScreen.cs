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
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;

[ScriptPath("res://src/Core/Nodes/Screens/CardSelection/NChooseACardSelectionScreen.cs")]
public class NChooseACardSelectionScreen : Control, IOverlayScreen, IScreenContext, ICardSelector
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName SelectHolder = StringName.op_Implicit("SelectHolder");

		public static readonly StringName OpenPreviewScreen = StringName.op_Implicit("OpenPreviewScreen");

		public static readonly StringName OnSkipButtonReleased = StringName.op_Implicit("OnSkipButtonReleased");

		public static readonly StringName AfterOverlayOpened = StringName.op_Implicit("AfterOverlayOpened");

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

		public static readonly StringName _banner = StringName.op_Implicit("_banner");

		public static readonly StringName _cardRow = StringName.op_Implicit("_cardRow");

		public static readonly StringName _skipButton = StringName.op_Implicit("_skipButton");

		public static readonly StringName _combatPiles = StringName.op_Implicit("_combatPiles");

		public static readonly StringName _inspectPrompt = StringName.op_Implicit("_inspectPrompt");

		public static readonly StringName _peekButton = StringName.op_Implicit("_peekButton");

		public static readonly StringName _openedTicks = StringName.op_Implicit("_openedTicks");

		public static readonly StringName _screenComplete = StringName.op_Implicit("_screenComplete");

		public static readonly StringName _cardSelected = StringName.op_Implicit("_cardSelected");

		public static readonly StringName _canSkip = StringName.op_Implicit("_canSkip");

		public static readonly StringName _cardTween = StringName.op_Implicit("_cardTween");

		public static readonly StringName _fadeTween = StringName.op_Implicit("_fadeTween");
	}

	public class SignalName : SignalName
	{
	}

	private const float _cardXSpacing = 340f;

	private const ulong _noSelectionTimeMsec = 350uL;

	private NCommonBanner _banner;

	private Control _cardRow;

	private NChoiceSelectionSkipButton _skipButton;

	private NCombatPilesContainer _combatPiles;

	private Control _inspectPrompt;

	private NPeekButton _peekButton;

	private readonly TaskCompletionSource<IEnumerable<CardModel>> _completionSource = new TaskCompletionSource<IEnumerable<CardModel>>();

	private ulong _openedTicks;

	private bool _screenComplete;

	private bool _cardSelected;

	private bool _canSkip;

	private Tween? _cardTween;

	private Tween? _fadeTween;

	private IReadOnlyList<CardModel> _cards;

	private static string ScenePath => SceneHelper.GetScenePath("screens/card_selection/choose_a_card_selection_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public NetScreenType ScreenType => NetScreenType.CardSelection;

	public bool UseSharedBackstop => true;

	public Control DefaultFocusedControl
	{
		get
		{
			if (_peekButton.IsPeeking)
			{
				return NCombatRoom.Instance.DefaultFocusedControl;
			}
			List<NGridCardHolder> list = ((IEnumerable)((Node)_cardRow).GetChildren(false)).OfType<NGridCardHolder>().ToList();
			return (Control)(object)list[list.Count / 2];
		}
	}

	public static NChooseACardSelectionScreen? ShowScreen(IReadOnlyList<CardModel> cards, bool canSkip)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NChooseACardSelectionScreen nChooseACardSelectionScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NChooseACardSelectionScreen>((GenEditState)0);
		((Node)nChooseACardSelectionScreen).Name = StringName.op_Implicit("NChooseACardSelectionScreen");
		nChooseACardSelectionScreen._cards = cards;
		nChooseACardSelectionScreen._canSkip = canSkip;
		NOverlayStack.Instance.Push(nChooseACardSelectionScreen);
		return nChooseACardSelectionScreen;
	}

	public override void _Ready()
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		_banner = ((Node)this).GetNode<NCommonBanner>(NodePath.op_Implicit("Banner"));
		_banner.label.SetTextAutoSize(new LocString("gameplay_ui", "CHOOSE_CARD_HEADER").GetRawText());
		_banner.AnimateIn();
		_cardRow = ((Node)this).GetNode<Control>(NodePath.op_Implicit("CardRow"));
		_combatPiles = ((Node)this).GetNode<NCombatPilesContainer>(NodePath.op_Implicit("%CombatPiles"));
		if (CombatManager.Instance.IsInProgress)
		{
			_combatPiles.Initialize(_cards.First().Owner);
		}
		_combatPiles.Disable();
		((CanvasItem)_combatPiles).Visible = false;
		_inspectPrompt = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%InspectPrompt"));
		Vector2 val = Vector2.Left * (float)(_cards.Count - 1) * 340f * 0.5f;
		_cardTween = ((Node)this).CreateTween().SetParallel(true);
		for (int i = 0; i < _cards.Count; i++)
		{
			CardModel card = _cards[i];
			NCard nCard = NCard.Create(card);
			NGridCardHolder nGridCardHolder = NGridCardHolder.Create(nCard);
			((Node)(object)_cardRow).AddChildSafely((Node?)(object)nGridCardHolder);
			((GodotObject)nGridCardHolder).Connect(NCardHolder.SignalName.Pressed, Callable.From<NCardHolder>((Action<NCardHolder>)SelectHolder), 0u);
			((GodotObject)nGridCardHolder).Connect(NCardHolder.SignalName.AltPressed, Callable.From<NCardHolder>((Action<NCardHolder>)OpenPreviewScreen), 0u);
			nCard.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
			((Control)nGridCardHolder).Scale = nGridCardHolder.SmallScale;
			_cardTween.TweenProperty((GodotObject)(object)nGridCardHolder, NodePath.op_Implicit("position"), Variant.op_Implicit(val + Vector2.Right * 340f * (float)i), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_cardTween.TweenProperty((GodotObject)(object)nGridCardHolder, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7)
				.From(Variant.op_Implicit(Colors.Black));
			nCard.ActivateRewardScreenGlow();
		}
		_skipButton = ((Node)this).GetNode<NChoiceSelectionSkipButton>(NodePath.op_Implicit("SkipButton"));
		if (_canSkip)
		{
			((GodotObject)_skipButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnSkipButtonReleased), 0u);
			_skipButton.AnimateIn();
		}
		else
		{
			_skipButton.Disable();
			((CanvasItem)_skipButton).Visible = false;
		}
		_peekButton = ((Node)this).GetNode<NPeekButton>(NodePath.op_Implicit("%PeekButton"));
		_peekButton.AddTargets(_banner, _cardRow, _skipButton, _inspectPrompt);
		((GodotObject)_peekButton).Connect(NPeekButton.SignalName.Toggled, Callable.From<NPeekButton>((Action<NPeekButton>)delegate
		{
			if (_peekButton.IsPeeking)
			{
				((Control)this).MouseFilter = (MouseFilterEnum)2;
				((CanvasItem)_combatPiles).Visible = true;
				_combatPiles.Enable();
				_skipButton.Disable();
			}
			else
			{
				((Control)this).MouseFilter = (MouseFilterEnum)0;
				((CanvasItem)_combatPiles).Visible = false;
				_combatPiles.Disable();
				ActiveScreenContext.Instance.Update();
				if (_canSkip)
				{
					_skipButton.Enable();
				}
			}
		}), 0u);
		for (int num = 0; num < ((Node)_cardRow).GetChildCount(false); num++)
		{
			Control child = ((Node)_cardRow).GetChild<Control>(num, false);
			child.FocusNeighborBottom = ((Node)child).GetPath();
			child.FocusNeighborTop = ((Node)child).GetPath();
			child.FocusNeighborLeft = ((num > 0) ? ((Node)_cardRow).GetChild(num - 1, false).GetPath() : ((Node)_cardRow).GetChild(((Node)_cardRow).GetChildCount(false) - 1, false).GetPath());
			child.FocusNeighborRight = ((num < ((Node)_cardRow).GetChildCount(false) - 1) ? ((Node)_cardRow).GetChild(num + 1, false).GetPath() : ((Node)_cardRow).GetChild(0, false).GetPath());
		}
		UpdateControllerIcons();
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)UpdateControllerIcons), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)UpdateControllerIcons), 0u);
		((GodotObject)NInputManager.Instance).Connect(NInputManager.SignalName.InputRebound, Callable.From((Action)UpdateControllerIcons), 0u);
	}

	public override void _ExitTree()
	{
		if (!_completionSource.Task.IsCompleted)
		{
			_completionSource.SetCanceled();
		}
		foreach (NGridCardHolder item in ((IEnumerable)((Node)_cardRow).GetChildren(false)).OfType<NGridCardHolder>())
		{
			((Node)(object)item).QueueFreeSafely();
		}
	}

	private void SelectHolder(NCardHolder cardHolder)
	{
		if (_completionSource == null)
		{
			throw new InvalidOperationException("CardsSelected must be awaited before a card is selected!");
		}
		if (Time.GetTicksMsec() - _openedTicks > 350)
		{
			CardModel cardModel = cardHolder.CardModel;
			_screenComplete = true;
			_cardSelected = true;
			_completionSource.SetResult(new CardModel[1] { cardModel });
		}
	}

	private void OpenPreviewScreen(NCardHolder cardHolder)
	{
		NInspectCardScreen inspectCardScreen = NGame.Instance.GetInspectCardScreen();
		int num = 1;
		List<CardModel> list = new List<CardModel>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<CardModel> span = CollectionsMarshal.AsSpan(list);
		int index = 0;
		span[index] = cardHolder.CardModel;
		inspectCardScreen.Open(list, 0);
	}

	public async Task<IEnumerable<CardModel>> CardsSelected()
	{
		IEnumerable<CardModel> result = await _completionSource.Task;
		NOverlayStack.Instance.Remove(this);
		return result;
	}

	private void OnSkipButtonReleased(NButton _)
	{
		_screenComplete = true;
		_completionSource.SetResult(Array.Empty<CardModel>());
	}

	public void AfterOverlayOpened()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Modulate = Colors.Transparent;
		_openedTicks = Time.GetTicksMsec();
		Tween? fadeTween = _fadeTween;
		if (fadeTween != null)
		{
			fadeTween.Kill();
		}
		_fadeTween = ((Node)this).CreateTween();
		_fadeTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.2);
	}

	public void AfterOverlayClosed()
	{
		Tween? fadeTween = _fadeTween;
		if (fadeTween != null)
		{
			fadeTween.Kill();
		}
		_peekButton.SetPeeking(isPeeking: false);
		((Node)(object)this).QueueFreeSafely();
	}

	public void AfterOverlayShown()
	{
		((CanvasItem)this).Visible = true;
		_peekButton.Enable();
		if (_canSkip && !_peekButton.IsPeeking)
		{
			_skipButton.Enable();
		}
	}

	public void AfterOverlayHidden()
	{
		_peekButton.Disable();
		_skipButton.Disable();
		((CanvasItem)this).Visible = false;
	}

	private void UpdateControllerIcons()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_inspectPrompt).Modulate = (NControllerManager.Instance.IsUsingController ? Colors.White : Colors.Transparent);
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
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SelectHolder, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenPreviewScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSkipButtonReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.SelectHolder && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SelectHolder(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenPreviewScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenPreviewScreen(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSkipButtonReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnSkipButtonReleased(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.SelectHolder)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenPreviewScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSkipButtonReleased)
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
		if ((ref name) == PropertyName._skipButton)
		{
			_skipButton = VariantUtils.ConvertTo<NChoiceSelectionSkipButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._combatPiles)
		{
			_combatPiles = VariantUtils.ConvertTo<NCombatPilesContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._inspectPrompt)
		{
			_inspectPrompt = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._peekButton)
		{
			_peekButton = VariantUtils.ConvertTo<NPeekButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._openedTicks)
		{
			_openedTicks = VariantUtils.ConvertTo<ulong>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenComplete)
		{
			_screenComplete = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardSelected)
		{
			_cardSelected = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._canSkip)
		{
			_canSkip = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			_cardTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			_fadeTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref name) == PropertyName._skipButton)
		{
			value = VariantUtils.CreateFrom<NChoiceSelectionSkipButton>(ref _skipButton);
			return true;
		}
		if ((ref name) == PropertyName._combatPiles)
		{
			value = VariantUtils.CreateFrom<NCombatPilesContainer>(ref _combatPiles);
			return true;
		}
		if ((ref name) == PropertyName._inspectPrompt)
		{
			value = VariantUtils.CreateFrom<Control>(ref _inspectPrompt);
			return true;
		}
		if ((ref name) == PropertyName._peekButton)
		{
			value = VariantUtils.CreateFrom<NPeekButton>(ref _peekButton);
			return true;
		}
		if ((ref name) == PropertyName._openedTicks)
		{
			value = VariantUtils.CreateFrom<ulong>(ref _openedTicks);
			return true;
		}
		if ((ref name) == PropertyName._screenComplete)
		{
			value = VariantUtils.CreateFrom<bool>(ref _screenComplete);
			return true;
		}
		if ((ref name) == PropertyName._cardSelected)
		{
			value = VariantUtils.CreateFrom<bool>(ref _cardSelected);
			return true;
		}
		if ((ref name) == PropertyName._canSkip)
		{
			value = VariantUtils.CreateFrom<bool>(ref _canSkip);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _cardTween);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _fadeTween);
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
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._banner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardRow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._skipButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._combatPiles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._inspectPrompt, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._peekButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._openedTicks, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._screenComplete, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._cardSelected, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._canSkip, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fadeTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._banner, Variant.From<NCommonBanner>(ref _banner));
		info.AddProperty(PropertyName._cardRow, Variant.From<Control>(ref _cardRow));
		info.AddProperty(PropertyName._skipButton, Variant.From<NChoiceSelectionSkipButton>(ref _skipButton));
		info.AddProperty(PropertyName._combatPiles, Variant.From<NCombatPilesContainer>(ref _combatPiles));
		info.AddProperty(PropertyName._inspectPrompt, Variant.From<Control>(ref _inspectPrompt));
		info.AddProperty(PropertyName._peekButton, Variant.From<NPeekButton>(ref _peekButton));
		info.AddProperty(PropertyName._openedTicks, Variant.From<ulong>(ref _openedTicks));
		info.AddProperty(PropertyName._screenComplete, Variant.From<bool>(ref _screenComplete));
		info.AddProperty(PropertyName._cardSelected, Variant.From<bool>(ref _cardSelected));
		info.AddProperty(PropertyName._canSkip, Variant.From<bool>(ref _canSkip));
		info.AddProperty(PropertyName._cardTween, Variant.From<Tween>(ref _cardTween));
		info.AddProperty(PropertyName._fadeTween, Variant.From<Tween>(ref _fadeTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._banner, ref val))
		{
			_banner = ((Variant)(ref val)).As<NCommonBanner>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardRow, ref val2))
		{
			_cardRow = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._skipButton, ref val3))
		{
			_skipButton = ((Variant)(ref val3)).As<NChoiceSelectionSkipButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._combatPiles, ref val4))
		{
			_combatPiles = ((Variant)(ref val4)).As<NCombatPilesContainer>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._inspectPrompt, ref val5))
		{
			_inspectPrompt = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._peekButton, ref val6))
		{
			_peekButton = ((Variant)(ref val6)).As<NPeekButton>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._openedTicks, ref val7))
		{
			_openedTicks = ((Variant)(ref val7)).As<ulong>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenComplete, ref val8))
		{
			_screenComplete = ((Variant)(ref val8)).As<bool>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardSelected, ref val9))
		{
			_cardSelected = ((Variant)(ref val9)).As<bool>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._canSkip, ref val10))
		{
			_canSkip = ((Variant)(ref val10)).As<bool>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardTween, ref val11))
		{
			_cardTween = ((Variant)(ref val11)).As<Tween>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._fadeTween, ref val12))
		{
			_fadeTween = ((Variant)(ref val12)).As<Tween>();
		}
	}
}
