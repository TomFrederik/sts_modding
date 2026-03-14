using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens;

[ScriptPath("res://src/Core/Nodes/Screens/NInspectCardScreen.cs")]
public class NInspectCardScreen : Control, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Close = StringName.op_Implicit("Close");

		public static readonly StringName OnRightButtonReleased = StringName.op_Implicit("OnRightButtonReleased");

		public static readonly StringName OnLeftButtonReleased = StringName.op_Implicit("OnLeftButtonReleased");

		public static readonly StringName ToggleShowUpgrade = StringName.op_Implicit("ToggleShowUpgrade");

		public static readonly StringName UpdateCardDisplay = StringName.op_Implicit("UpdateCardDisplay");

		public static readonly StringName SetCard = StringName.op_Implicit("SetCard");

		public static readonly StringName OnBackstopPressed = StringName.op_Implicit("OnBackstopPressed");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName IsShowingUpgradedCard = StringName.op_Implicit("IsShowingUpgradedCard");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _card = StringName.op_Implicit("_card");

		public static readonly StringName _backstop = StringName.op_Implicit("_backstop");

		public static readonly StringName _upgradeTickbox = StringName.op_Implicit("_upgradeTickbox");

		public static readonly StringName _leftButton = StringName.op_Implicit("_leftButton");

		public static readonly StringName _rightButton = StringName.op_Implicit("_rightButton");

		public static readonly StringName _hoverTipRect = StringName.op_Implicit("_hoverTipRect");

		public static readonly StringName _index = StringName.op_Implicit("_index");

		public static readonly StringName _openTween = StringName.op_Implicit("_openTween");

		public static readonly StringName _cardTween = StringName.op_Implicit("_cardTween");

		public static readonly StringName _cardPosition = StringName.op_Implicit("_cardPosition");

		public static readonly StringName _leftButtonX = StringName.op_Implicit("_leftButtonX");

		public static readonly StringName _rightButtonX = StringName.op_Implicit("_rightButtonX");

		public static readonly StringName _viewAllUpgraded = StringName.op_Implicit("_viewAllUpgraded");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/inspect_card_screen");

	private NCard _card;

	private NButton _backstop;

	private NTickbox _upgradeTickbox;

	private NButton _leftButton;

	private NButton _rightButton;

	private Control _hoverTipRect;

	private List<CardModel>? _cards;

	private int _index;

	private Tween? _openTween;

	private Tween? _cardTween;

	private Vector2 _cardPosition;

	private float _leftButtonX;

	private float _rightButtonX;

	private const double _arrowButtonDelay = 0.1;

	private bool _viewAllUpgraded;

	public static string[] AssetPaths => new string[1] { _scenePath };

	private bool IsShowingUpgradedCard => _upgradeTickbox.IsTicked;

	public Control? DefaultFocusedControl => null;

	public static NInspectCardScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NInspectCardScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		_card = ((Node)this).GetNode<NCard>(NodePath.op_Implicit("Card"));
		_cardPosition = ((Control)_card).Position;
		_hoverTipRect = ((Node)this).GetNode<Control>(NodePath.op_Implicit("HoverTipRect"));
		_backstop = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("Backstop"));
		((GodotObject)_backstop).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnBackstopPressed), 0u);
		_leftButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("LeftArrow"));
		((GodotObject)_leftButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			OnLeftButtonReleased();
		}), 0u);
		_rightButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("RightArrow"));
		((GodotObject)_rightButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			OnRightButtonReleased();
		}), 0u);
		_leftButtonX = ((Control)_leftButton).Position.X;
		_rightButtonX = ((Control)_rightButton).Position.X;
		_upgradeTickbox = ((Node)this).GetNode<NTickbox>(NodePath.op_Implicit("%Upgrade"));
		_upgradeTickbox.IsTicked = false;
		((GodotObject)_upgradeTickbox).Connect(NTickbox.SignalName.Toggled, Callable.From<NTickbox>((Action<NTickbox>)ToggleShowUpgrade), 0u);
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ShowUpgradeLabel")).SetTextAutoSize(new LocString("card_selection", "VIEW_UPGRADES").GetFormattedText());
		_rightButton.Disable();
		_leftButton.Disable();
		_upgradeTickbox.Disable();
		Close();
	}

	public void Open(List<CardModel> cards, int index, bool viewAllUpgraded = false)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		_cards = cards;
		((CanvasItem)this).Visible = true;
		((Control)this).MouseFilter = (MouseFilterEnum)0;
		_viewAllUpgraded = viewAllUpgraded;
		SetCard(index);
		((Control)_card).Scale = Vector2.One * 1.75f;
		((CanvasItem)_card).Modulate = StsColors.transparentBlack;
		((CanvasItem)_leftButton).Modulate = StsColors.transparentBlack;
		((CanvasItem)_rightButton).Modulate = StsColors.transparentBlack;
		_rightButton.Enable();
		_leftButton.Enable();
		Tween? openTween = _openTween;
		if (openTween != null)
		{
			openTween.Kill();
		}
		_openTween = ((Node)this).CreateTween().SetParallel(true);
		_openTween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.9f), 0.25);
		_openTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(0f));
		_openTween.TweenProperty((GodotObject)(object)_leftButton, NodePath.op_Implicit("position:x"), Variant.op_Implicit(_leftButtonX), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(_leftButtonX + 100f))
			.SetDelay(0.1);
		_openTween.TweenProperty((GodotObject)(object)_leftButton, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25).SetDelay(0.1);
		_openTween.TweenProperty((GodotObject)(object)_rightButton, NodePath.op_Implicit("position:x"), Variant.op_Implicit(_rightButtonX), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(_rightButtonX - 100f))
			.SetDelay(0.1);
		_openTween.TweenProperty((GodotObject)(object)_rightButton, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25).SetDelay(0.1);
		Tween? cardTween = _cardTween;
		if (cardTween != null)
		{
			cardTween.Kill();
		}
		_cardTween = ((Node)this).CreateTween().SetParallel(true);
		_cardTween.TweenProperty((GodotObject)(object)_card, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25);
		_cardTween.TweenProperty((GodotObject)(object)_card, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 2f), 0.15).SetEase((EaseType)1).SetTrans((TransitionType)11)
			.SetDelay(0.1);
		_upgradeTickbox.Enable();
		ActiveScreenContext.Instance.Update();
		NHotkeyManager.Instance.AddBlockingScreen((Node)(object)this);
		NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(MegaInput.cancel), Close);
		NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(MegaInput.pauseAndBack), Close);
		NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(MegaInput.left), OnLeftButtonReleased);
		NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(MegaInput.right), OnRightButtonReleased);
	}

	public void Close()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)this).Visible)
		{
			((Control)this).MouseFilter = (MouseFilterEnum)2;
			((Control)_leftButton).MouseFilter = (MouseFilterEnum)2;
			((Control)_rightButton).MouseFilter = (MouseFilterEnum)2;
			_rightButton.Disable();
			_leftButton.Disable();
			_upgradeTickbox.Disable();
			NHoverTipSet.Clear();
			((Node)this).SetProcessUnhandledInput(false);
			Tween? openTween = _openTween;
			if (openTween != null)
			{
				openTween.Kill();
			}
			_openTween = ((Node)this).CreateTween().SetParallel(true);
			_openTween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
			_openTween.TweenProperty((GodotObject)(object)_leftButton, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.1);
			_openTween.TweenProperty((GodotObject)(object)_rightButton, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.1);
			_openTween.TweenProperty((GodotObject)(object)_card, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentWhite), 0.1);
			_openTween.Chain().TweenCallback(Callable.From((Action)delegate
			{
				((CanvasItem)this).Visible = false;
				ActiveScreenContext.Instance.Update();
			}));
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(MegaInput.cancel), Close);
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(MegaInput.pauseAndBack), Close);
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(MegaInput.left), OnLeftButtonReleased);
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(MegaInput.right), OnRightButtonReleased);
			NHotkeyManager.Instance.RemoveBlockingScreen((Node)(object)this);
		}
	}

	private void OnRightButtonReleased()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)_rightButton).Visible)
		{
			SetCard(_index + 1);
			((CanvasItem)_card).Modulate = Colors.White;
			Tween? openTween = _openTween;
			if (openTween != null)
			{
				openTween.Kill();
			}
			_openTween = ((Node)this).CreateTween().SetParallel(true);
			_openTween.TweenProperty((GodotObject)(object)_card, NodePath.op_Implicit("position"), Variant.op_Implicit(_cardPosition), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5)
				.From(Variant.op_Implicit(_cardPosition + new Vector2(100f, 0f)));
		}
	}

	private void OnLeftButtonReleased()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)_leftButton).Visible)
		{
			SetCard(_index - 1);
			((CanvasItem)_card).Modulate = Colors.White;
			Tween? openTween = _openTween;
			if (openTween != null)
			{
				openTween.Kill();
			}
			_openTween = ((Node)this).CreateTween().SetParallel(true);
			_openTween.TweenProperty((GodotObject)(object)_card, NodePath.op_Implicit("position"), Variant.op_Implicit(_cardPosition), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5)
				.From(Variant.op_Implicit(_cardPosition + new Vector2(-100f, 0f)));
		}
	}

	private void ToggleShowUpgrade(NTickbox _)
	{
		_viewAllUpgraded = false;
		UpdateCardDisplay();
	}

	private void UpdateCardDisplay()
	{
		CardModel cardModel = _cards[_index];
		CardModel cardModel2 = (CardModel)_cards[_index].MutableClone();
		if (IsShowingUpgradedCard)
		{
			if (!cardModel.IsUpgraded && cardModel.IsUpgradable)
			{
				cardModel2.UpgradePreviewType = CardUpgradePreviewType.Deck;
				cardModel2.UpgradeInternal();
			}
			_card.Model = cardModel2;
			_card.ShowUpgradePreview();
		}
		else
		{
			if (cardModel2.IsUpgraded)
			{
				CardCmd.Downgrade(cardModel2);
			}
			_card.Model = cardModel2;
			_card.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
		}
		NHoverTipSet.Clear();
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, cardModel2.HoverTips);
		nHoverTipSet.SetAlignment(_hoverTipRect, HoverTip.GetHoverTipAlignment((Control)(object)this));
	}

	private void SetCard(int index)
	{
		_index = Math.Clamp(index, 0, _cards.Count - 1);
		((CanvasItem)_leftButton).Visible = _index > 0;
		((Control)_leftButton).MouseFilter = (MouseFilterEnum)(((CanvasItem)_leftButton).Visible ? 0 : 2);
		((CanvasItem)_rightButton).Visible = _index < _cards.Count - 1;
		((Control)_rightButton).MouseFilter = (MouseFilterEnum)(((CanvasItem)_rightButton).Visible ? 0 : 2);
		((CanvasItem)_upgradeTickbox).Visible = _cards[_index].MaxUpgradeLevel > 0;
		((Control)_upgradeTickbox).MouseFilter = (MouseFilterEnum)((_cards[_index].MaxUpgradeLevel > 0) ? 0 : 2);
		if (_cards[_index].IsUpgraded || _viewAllUpgraded)
		{
			_upgradeTickbox.IsTicked = true;
		}
		else
		{
			_upgradeTickbox.IsTicked = false;
		}
		UpdateCardDisplay();
	}

	private void OnBackstopPressed(NButton _)
	{
		Close();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
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
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Expected O, but got Unknown
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Close, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRightButtonReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnLeftButtonReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleShowUpgrade, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateCardDisplay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnBackstopPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
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
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NInspectCardScreen nInspectCardScreen = Create();
			ret = VariantUtils.CreateFrom<NInspectCardScreen>(ref nInspectCardScreen);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Close && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Close();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRightButtonReleased && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRightButtonReleased();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnLeftButtonReleased && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnLeftButtonReleased();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleShowUpgrade && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ToggleShowUpgrade(VariantUtils.ConvertTo<NTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateCardDisplay && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateCardDisplay();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCard && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCard(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnBackstopPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnBackstopPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NInspectCardScreen nInspectCardScreen = Create();
			ret = VariantUtils.CreateFrom<NInspectCardScreen>(ref nInspectCardScreen);
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
		if ((ref method) == MethodName.Close)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRightButtonReleased)
		{
			return true;
		}
		if ((ref method) == MethodName.OnLeftButtonReleased)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleShowUpgrade)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateCardDisplay)
		{
			return true;
		}
		if ((ref method) == MethodName.SetCard)
		{
			return true;
		}
		if ((ref method) == MethodName.OnBackstopPressed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._card)
		{
			_card = VariantUtils.ConvertTo<NCard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			_backstop = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._upgradeTickbox)
		{
			_upgradeTickbox = VariantUtils.ConvertTo<NTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftButton)
		{
			_leftButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightButton)
		{
			_rightButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTipRect)
		{
			_hoverTipRect = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._index)
		{
			_index = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._openTween)
		{
			_openTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			_cardTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardPosition)
		{
			_cardPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftButtonX)
		{
			_leftButtonX = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightButtonX)
		{
			_rightButtonX = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._viewAllUpgraded)
		{
			_viewAllUpgraded = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsShowingUpgradedCard)
		{
			bool isShowingUpgradedCard = IsShowingUpgradedCard;
			value = VariantUtils.CreateFrom<bool>(ref isShowingUpgradedCard);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._card)
		{
			value = VariantUtils.CreateFrom<NCard>(ref _card);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _backstop);
			return true;
		}
		if ((ref name) == PropertyName._upgradeTickbox)
		{
			value = VariantUtils.CreateFrom<NTickbox>(ref _upgradeTickbox);
			return true;
		}
		if ((ref name) == PropertyName._leftButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _leftButton);
			return true;
		}
		if ((ref name) == PropertyName._rightButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _rightButton);
			return true;
		}
		if ((ref name) == PropertyName._hoverTipRect)
		{
			value = VariantUtils.CreateFrom<Control>(ref _hoverTipRect);
			return true;
		}
		if ((ref name) == PropertyName._index)
		{
			value = VariantUtils.CreateFrom<int>(ref _index);
			return true;
		}
		if ((ref name) == PropertyName._openTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _openTween);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _cardTween);
			return true;
		}
		if ((ref name) == PropertyName._cardPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _cardPosition);
			return true;
		}
		if ((ref name) == PropertyName._leftButtonX)
		{
			value = VariantUtils.CreateFrom<float>(ref _leftButtonX);
			return true;
		}
		if ((ref name) == PropertyName._rightButtonX)
		{
			value = VariantUtils.CreateFrom<float>(ref _rightButtonX);
			return true;
		}
		if ((ref name) == PropertyName._viewAllUpgraded)
		{
			value = VariantUtils.CreateFrom<bool>(ref _viewAllUpgraded);
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
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._card, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._upgradeTickbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leftButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rightButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTipRect, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._index, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._openTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._cardPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._leftButtonX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._rightButtonX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._viewAllUpgraded, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsShowingUpgradedCard, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._card, Variant.From<NCard>(ref _card));
		info.AddProperty(PropertyName._backstop, Variant.From<NButton>(ref _backstop));
		info.AddProperty(PropertyName._upgradeTickbox, Variant.From<NTickbox>(ref _upgradeTickbox));
		info.AddProperty(PropertyName._leftButton, Variant.From<NButton>(ref _leftButton));
		info.AddProperty(PropertyName._rightButton, Variant.From<NButton>(ref _rightButton));
		info.AddProperty(PropertyName._hoverTipRect, Variant.From<Control>(ref _hoverTipRect));
		info.AddProperty(PropertyName._index, Variant.From<int>(ref _index));
		info.AddProperty(PropertyName._openTween, Variant.From<Tween>(ref _openTween));
		info.AddProperty(PropertyName._cardTween, Variant.From<Tween>(ref _cardTween));
		info.AddProperty(PropertyName._cardPosition, Variant.From<Vector2>(ref _cardPosition));
		info.AddProperty(PropertyName._leftButtonX, Variant.From<float>(ref _leftButtonX));
		info.AddProperty(PropertyName._rightButtonX, Variant.From<float>(ref _rightButtonX));
		info.AddProperty(PropertyName._viewAllUpgraded, Variant.From<bool>(ref _viewAllUpgraded));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._card, ref val))
		{
			_card = ((Variant)(ref val)).As<NCard>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstop, ref val2))
		{
			_backstop = ((Variant)(ref val2)).As<NButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._upgradeTickbox, ref val3))
		{
			_upgradeTickbox = ((Variant)(ref val3)).As<NTickbox>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftButton, ref val4))
		{
			_leftButton = ((Variant)(ref val4)).As<NButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightButton, ref val5))
		{
			_rightButton = ((Variant)(ref val5)).As<NButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTipRect, ref val6))
		{
			_hoverTipRect = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._index, ref val7))
		{
			_index = ((Variant)(ref val7)).As<int>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._openTween, ref val8))
		{
			_openTween = ((Variant)(ref val8)).As<Tween>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardTween, ref val9))
		{
			_cardTween = ((Variant)(ref val9)).As<Tween>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardPosition, ref val10))
		{
			_cardPosition = ((Variant)(ref val10)).As<Vector2>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftButtonX, ref val11))
		{
			_leftButtonX = ((Variant)(ref val11)).As<float>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightButtonX, ref val12))
		{
			_rightButtonX = ((Variant)(ref val12)).As<float>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._viewAllUpgraded, ref val13))
		{
			_viewAllUpgraded = ((Variant)(ref val13)).As<bool>();
		}
	}
}
