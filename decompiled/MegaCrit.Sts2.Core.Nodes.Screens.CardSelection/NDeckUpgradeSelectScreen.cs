using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;

[ScriptPath("res://src/Core/Nodes/Screens/CardSelection/NDeckUpgradeSelectScreen.cs")]
public sealed class NDeckUpgradeSelectScreen : NCardGridSelectionScreen
{
	public new class MethodName : NCardGridSelectionScreen.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName CloseSelection = StringName.op_Implicit("CloseSelection");

		public static readonly StringName CancelSelection = StringName.op_Implicit("CancelSelection");

		public static readonly StringName ConfirmSelection = StringName.op_Implicit("ConfirmSelection");

		public static readonly StringName CheckIfSelectionComplete = StringName.op_Implicit("CheckIfSelectionComplete");

		public static readonly StringName ToggleShowUpgrades = StringName.op_Implicit("ToggleShowUpgrades");

		public static readonly StringName OnControllerStateUpdated = StringName.op_Implicit("OnControllerStateUpdated");
	}

	public new class PropertyName : NCardGridSelectionScreen.PropertyName
	{
		public static readonly StringName UseSingleSelection = StringName.op_Implicit("UseSingleSelection");

		public new static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public new static readonly StringName FocusedControlFromTopBar = StringName.op_Implicit("FocusedControlFromTopBar");

		public static readonly StringName _upgradeSinglePreviewContainer = StringName.op_Implicit("_upgradeSinglePreviewContainer");

		public static readonly StringName _singlePreview = StringName.op_Implicit("_singlePreview");

		public static readonly StringName _singlePreviewCancelButton = StringName.op_Implicit("_singlePreviewCancelButton");

		public static readonly StringName _singlePreviewConfirmButton = StringName.op_Implicit("_singlePreviewConfirmButton");

		public static readonly StringName _viewUpgrades = StringName.op_Implicit("_viewUpgrades");

		public static readonly StringName _bottomTextContainer = StringName.op_Implicit("_bottomTextContainer");

		public static readonly StringName _infoLabel = StringName.op_Implicit("_infoLabel");

		public static readonly StringName _upgradeMultiPreviewContainer = StringName.op_Implicit("_upgradeMultiPreviewContainer");

		public static readonly StringName _multiPreview = StringName.op_Implicit("_multiPreview");

		public static readonly StringName _multiPreviewCancelButton = StringName.op_Implicit("_multiPreviewCancelButton");

		public static readonly StringName _multiPreviewConfirmButton = StringName.op_Implicit("_multiPreviewConfirmButton");

		public static readonly StringName _closeButton = StringName.op_Implicit("_closeButton");
	}

	public new class SignalName : NCardGridSelectionScreen.SignalName
	{
	}

	private readonly HashSet<CardModel> _selectedCards = new HashSet<CardModel>();

	private CardSelectorPrefs _prefs;

	private IRunState _runState;

	private Control _upgradeSinglePreviewContainer;

	private NUpgradePreview _singlePreview;

	private NBackButton _singlePreviewCancelButton;

	private NConfirmButton _singlePreviewConfirmButton;

	private NTickbox _viewUpgrades;

	private Control _bottomTextContainer;

	private MegaRichTextLabel _infoLabel;

	private Control _upgradeMultiPreviewContainer;

	private Control _multiPreview;

	private NBackButton _multiPreviewCancelButton;

	private NConfirmButton _multiPreviewConfirmButton;

	private NBackButton _closeButton;

	private static string ScenePath => SceneHelper.GetScenePath("screens/card_selection/deck_upgrade_select_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	private bool UseSingleSelection => _prefs.MaxSelect == 1;

	protected override IEnumerable<Control> PeekButtonTargets => new global::_003C_003Ez__ReadOnlyArray<Control>((Control[])(object)new Control[3] { _upgradeSinglePreviewContainer, _upgradeMultiPreviewContainer, _closeButton });

	public override Control? DefaultFocusedControl
	{
		get
		{
			if (((CanvasItem)_upgradeSinglePreviewContainer).Visible || ((CanvasItem)_upgradeMultiPreviewContainer).Visible)
			{
				return null;
			}
			return _grid.DefaultFocusedControl;
		}
	}

	public override Control? FocusedControlFromTopBar
	{
		get
		{
			if (((CanvasItem)_upgradeSinglePreviewContainer).Visible || ((CanvasItem)_upgradeMultiPreviewContainer).Visible)
			{
				return null;
			}
			return _grid.FocusedControlFromTopBar;
		}
	}

	public override void _Ready()
	{
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignalsAndInitGrid();
		_upgradeSinglePreviewContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%UpgradeSinglePreviewContainer"));
		_singlePreview = ((Node)_upgradeSinglePreviewContainer).GetNode<NUpgradePreview>(NodePath.op_Implicit("UpgradePreview"));
		_singlePreviewCancelButton = ((Node)_upgradeSinglePreviewContainer).GetNode<NBackButton>(NodePath.op_Implicit("Cancel"));
		_singlePreviewConfirmButton = ((Node)_upgradeSinglePreviewContainer).GetNode<NConfirmButton>(NodePath.op_Implicit("Confirm"));
		_upgradeMultiPreviewContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%UpgradeMultiPreviewContainer"));
		_multiPreview = ((Node)_upgradeMultiPreviewContainer).GetNode<Control>(NodePath.op_Implicit("Cards"));
		_multiPreviewCancelButton = ((Node)_upgradeMultiPreviewContainer).GetNode<NBackButton>(NodePath.op_Implicit("Cancel"));
		_multiPreviewConfirmButton = ((Node)_upgradeMultiPreviewContainer).GetNode<NConfirmButton>(NodePath.op_Implicit("Confirm"));
		_closeButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%Close"));
		_bottomTextContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BottomText"));
		_infoLabel = ((Node)_bottomTextContainer).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%BottomLabel"));
		_infoLabel.Text = _prefs.Prompt.GetFormattedText();
		((GodotObject)_singlePreviewCancelButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CancelSelection), 0u);
		((GodotObject)_singlePreviewConfirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ConfirmSelection), 0u);
		((GodotObject)_multiPreviewCancelButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CancelSelection), 0u);
		((GodotObject)_multiPreviewConfirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ConfirmSelection), 0u);
		((GodotObject)_closeButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CloseSelection), 0u);
		if (_prefs.Cancelable)
		{
			_closeButton.Enable();
		}
		else
		{
			_closeButton.Disable();
		}
		((CanvasItem)_upgradeSinglePreviewContainer).Visible = false;
		_upgradeSinglePreviewContainer.MouseFilter = (MouseFilterEnum)2;
		((CanvasItem)_upgradeMultiPreviewContainer).Visible = false;
		_upgradeMultiPreviewContainer.MouseFilter = (MouseFilterEnum)2;
		_singlePreviewCancelButton.Disable();
		_singlePreviewConfirmButton.Disable();
		_multiPreviewCancelButton.Disable();
		_multiPreviewConfirmButton.Disable();
		_viewUpgrades = ((Node)this).GetNode<NTickbox>(NodePath.op_Implicit("%Upgrades"));
		_viewUpgrades.IsTicked = false;
		((GodotObject)_viewUpgrades).Connect(NTickbox.SignalName.Toggled, Callable.From<NTickbox>((Action<NTickbox>)ToggleShowUpgrades), 0u);
		OnControllerStateUpdated();
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)OnControllerStateUpdated), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)OnControllerStateUpdated), 0u);
		((GodotObject)NInputManager.Instance).Connect(NInputManager.SignalName.InputRebound, Callable.From((Action)OnControllerStateUpdated), 0u);
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ViewUpgradesLabel")).SetTextAutoSize(new LocString("card_selection", "VIEW_UPGRADES").GetFormattedText());
	}

	public static NDeckUpgradeSelectScreen ShowScreen(IReadOnlyList<CardModel> cards, CardSelectorPrefs prefs, IRunState runState)
	{
		NDeckUpgradeSelectScreen nDeckUpgradeSelectScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NDeckUpgradeSelectScreen>((GenEditState)0);
		((Node)nDeckUpgradeSelectScreen).Name = StringName.op_Implicit("NDeckUpgradeSelectScreen");
		nDeckUpgradeSelectScreen._cards = cards;
		nDeckUpgradeSelectScreen._prefs = prefs;
		nDeckUpgradeSelectScreen._runState = runState;
		NOverlayStack.Instance.Push(nDeckUpgradeSelectScreen);
		return nDeckUpgradeSelectScreen;
	}

	protected override void OnCardClicked(CardModel card)
	{
		if (_selectedCards.Add(card))
		{
			_grid.HighlightCard(card);
			if (UseSingleSelection)
			{
				((Node)this).GetViewport().GuiReleaseFocus();
				((CanvasItem)_upgradeSinglePreviewContainer).Visible = true;
				_upgradeSinglePreviewContainer.MouseFilter = (MouseFilterEnum)0;
				_singlePreview.Card = card;
				_singlePreviewCancelButton.Enable();
				_singlePreviewConfirmButton.Enable();
				_grid.SetCanScroll(canScroll: false);
				_closeButton.Disable();
			}
			else
			{
				if (_prefs.MaxSelect != _selectedCards.Count)
				{
					return;
				}
				((Node)this).GetViewport().GuiReleaseFocus();
				((CanvasItem)_upgradeMultiPreviewContainer).Visible = true;
				_upgradeMultiPreviewContainer.MouseFilter = (MouseFilterEnum)0;
				_multiPreviewCancelButton.Enable();
				_multiPreviewConfirmButton.Enable();
				foreach (CardModel selectedCard in _selectedCards)
				{
					_grid.UnhighlightCard(selectedCard);
					CardModel cardModel = _runState.CloneCard(selectedCard);
					cardModel.UpgradeInternal();
					cardModel.UpgradePreviewType = CardUpgradePreviewType.Deck;
					NCard nCard = NCard.Create(cardModel);
					((Node)(object)_multiPreview).AddChildSafely((Node?)(object)NPreviewCardHolder.Create(nCard, showHoverTips: true, scaleOnHover: false));
					nCard.ShowUpgradePreview();
					_grid.SetCanScroll(canScroll: false);
					_closeButton.Disable();
				}
			}
		}
		else
		{
			_selectedCards.Remove(card);
			_grid.UnhighlightCard(card);
		}
	}

	private void CloseSelection(NButton _)
	{
		_completionSource.SetResult(Array.Empty<CardModel>());
		_singlePreviewCancelButton.Disable();
		_singlePreviewConfirmButton.Disable();
		_multiPreviewCancelButton.Disable();
		_multiPreviewConfirmButton.Disable();
		NOverlayStack.Instance.Remove(this);
	}

	private void CancelSelection(NButton _)
	{
		if (UseSingleSelection)
		{
			((CanvasItem)_upgradeSinglePreviewContainer).Visible = false;
			_upgradeSinglePreviewContainer.MouseFilter = (MouseFilterEnum)2;
			_singlePreviewCancelButton.Disable();
			_singlePreviewConfirmButton.Disable();
		}
		else
		{
			((CanvasItem)_upgradeMultiPreviewContainer).Visible = false;
			_upgradeMultiPreviewContainer.MouseFilter = (MouseFilterEnum)2;
			for (int i = 0; i < ((Node)_multiPreview).GetChildCount(false); i++)
			{
				((Node)_multiPreview).GetChild(i, false).QueueFreeSafely();
			}
			_multiPreviewCancelButton.Disable();
			_multiPreviewConfirmButton.Disable();
		}
		_grid.SetCanScroll(canScroll: true);
		if (_prefs.Cancelable)
		{
			_closeButton.Enable();
		}
		foreach (CardModel selectedCard in _selectedCards)
		{
			_grid.UnhighlightCard(selectedCard);
		}
		((Control)(object)_grid.GetCardHolder(_selectedCards.Last()))?.TryGrabFocus();
		_selectedCards.Clear();
	}

	private void ConfirmSelection(NButton _)
	{
		if (_selectedCards.Count != 0)
		{
			CheckIfSelectionComplete();
		}
	}

	private void CheckIfSelectionComplete()
	{
		_singlePreviewCancelButton.Enable();
		_singlePreviewConfirmButton.Enable();
		if (_selectedCards.Count >= _prefs.MaxSelect)
		{
			_completionSource.SetResult(_selectedCards);
			NOverlayStack.Instance.Remove(this);
		}
	}

	private void ToggleShowUpgrades(NTickbox tickbox)
	{
		_grid.IsShowingUpgrades = tickbox.IsTicked;
	}

	private void OnControllerStateUpdated()
	{
		((CanvasItem)_viewUpgrades).Visible = !NControllerManager.Instance.IsUsingController;
		if (NControllerManager.Instance.IsUsingController)
		{
			_viewUpgrades.IsTicked = false;
			ToggleShowUpgrades(_viewUpgrades);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CloseSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConfirmSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CheckIfSelectionComplete, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleShowUpgrades, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tickbox"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnControllerStateUpdated, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CloseSelection && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CloseSelection(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CancelSelection && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CancelSelection(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ConfirmSelection && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ConfirmSelection(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CheckIfSelectionComplete && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CheckIfSelectionComplete();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleShowUpgrades && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ToggleShowUpgrades(VariantUtils.ConvertTo<NTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnControllerStateUpdated && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnControllerStateUpdated();
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
		if ((ref method) == MethodName.CloseSelection)
		{
			return true;
		}
		if ((ref method) == MethodName.CancelSelection)
		{
			return true;
		}
		if ((ref method) == MethodName.ConfirmSelection)
		{
			return true;
		}
		if ((ref method) == MethodName.CheckIfSelectionComplete)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleShowUpgrades)
		{
			return true;
		}
		if ((ref method) == MethodName.OnControllerStateUpdated)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._upgradeSinglePreviewContainer)
		{
			_upgradeSinglePreviewContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._singlePreview)
		{
			_singlePreview = VariantUtils.ConvertTo<NUpgradePreview>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._singlePreviewCancelButton)
		{
			_singlePreviewCancelButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._singlePreviewConfirmButton)
		{
			_singlePreviewConfirmButton = VariantUtils.ConvertTo<NConfirmButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._viewUpgrades)
		{
			_viewUpgrades = VariantUtils.ConvertTo<NTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bottomTextContainer)
		{
			_bottomTextContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._infoLabel)
		{
			_infoLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._upgradeMultiPreviewContainer)
		{
			_upgradeMultiPreviewContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._multiPreview)
		{
			_multiPreview = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._multiPreviewCancelButton)
		{
			_multiPreviewCancelButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._multiPreviewConfirmButton)
		{
			_multiPreviewConfirmButton = VariantUtils.ConvertTo<NConfirmButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._closeButton)
		{
			_closeButton = VariantUtils.ConvertTo<NBackButton>(ref value);
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
		if ((ref name) == PropertyName.UseSingleSelection)
		{
			bool useSingleSelection = UseSingleSelection;
			value = VariantUtils.CreateFrom<bool>(ref useSingleSelection);
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
		if ((ref name) == PropertyName._upgradeSinglePreviewContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _upgradeSinglePreviewContainer);
			return true;
		}
		if ((ref name) == PropertyName._singlePreview)
		{
			value = VariantUtils.CreateFrom<NUpgradePreview>(ref _singlePreview);
			return true;
		}
		if ((ref name) == PropertyName._singlePreviewCancelButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _singlePreviewCancelButton);
			return true;
		}
		if ((ref name) == PropertyName._singlePreviewConfirmButton)
		{
			value = VariantUtils.CreateFrom<NConfirmButton>(ref _singlePreviewConfirmButton);
			return true;
		}
		if ((ref name) == PropertyName._viewUpgrades)
		{
			value = VariantUtils.CreateFrom<NTickbox>(ref _viewUpgrades);
			return true;
		}
		if ((ref name) == PropertyName._bottomTextContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bottomTextContainer);
			return true;
		}
		if ((ref name) == PropertyName._infoLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _infoLabel);
			return true;
		}
		if ((ref name) == PropertyName._upgradeMultiPreviewContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _upgradeMultiPreviewContainer);
			return true;
		}
		if ((ref name) == PropertyName._multiPreview)
		{
			value = VariantUtils.CreateFrom<Control>(ref _multiPreview);
			return true;
		}
		if ((ref name) == PropertyName._multiPreviewCancelButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _multiPreviewCancelButton);
			return true;
		}
		if ((ref name) == PropertyName._multiPreviewConfirmButton)
		{
			value = VariantUtils.CreateFrom<NConfirmButton>(ref _multiPreviewConfirmButton);
			return true;
		}
		if ((ref name) == PropertyName._closeButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _closeButton);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.UseSingleSelection, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._upgradeSinglePreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._singlePreview, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._singlePreviewCancelButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._singlePreviewConfirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewUpgrades, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bottomTextContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infoLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._upgradeMultiPreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._multiPreview, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._multiPreviewCancelButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._multiPreviewConfirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._closeButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.FocusedControlFromTopBar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._upgradeSinglePreviewContainer, Variant.From<Control>(ref _upgradeSinglePreviewContainer));
		info.AddProperty(PropertyName._singlePreview, Variant.From<NUpgradePreview>(ref _singlePreview));
		info.AddProperty(PropertyName._singlePreviewCancelButton, Variant.From<NBackButton>(ref _singlePreviewCancelButton));
		info.AddProperty(PropertyName._singlePreviewConfirmButton, Variant.From<NConfirmButton>(ref _singlePreviewConfirmButton));
		info.AddProperty(PropertyName._viewUpgrades, Variant.From<NTickbox>(ref _viewUpgrades));
		info.AddProperty(PropertyName._bottomTextContainer, Variant.From<Control>(ref _bottomTextContainer));
		info.AddProperty(PropertyName._infoLabel, Variant.From<MegaRichTextLabel>(ref _infoLabel));
		info.AddProperty(PropertyName._upgradeMultiPreviewContainer, Variant.From<Control>(ref _upgradeMultiPreviewContainer));
		info.AddProperty(PropertyName._multiPreview, Variant.From<Control>(ref _multiPreview));
		info.AddProperty(PropertyName._multiPreviewCancelButton, Variant.From<NBackButton>(ref _multiPreviewCancelButton));
		info.AddProperty(PropertyName._multiPreviewConfirmButton, Variant.From<NConfirmButton>(ref _multiPreviewConfirmButton));
		info.AddProperty(PropertyName._closeButton, Variant.From<NBackButton>(ref _closeButton));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._upgradeSinglePreviewContainer, ref val))
		{
			_upgradeSinglePreviewContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._singlePreview, ref val2))
		{
			_singlePreview = ((Variant)(ref val2)).As<NUpgradePreview>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._singlePreviewCancelButton, ref val3))
		{
			_singlePreviewCancelButton = ((Variant)(ref val3)).As<NBackButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._singlePreviewConfirmButton, ref val4))
		{
			_singlePreviewConfirmButton = ((Variant)(ref val4)).As<NConfirmButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._viewUpgrades, ref val5))
		{
			_viewUpgrades = ((Variant)(ref val5)).As<NTickbox>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._bottomTextContainer, ref val6))
		{
			_bottomTextContainer = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._infoLabel, ref val7))
		{
			_infoLabel = ((Variant)(ref val7)).As<MegaRichTextLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._upgradeMultiPreviewContainer, ref val8))
		{
			_upgradeMultiPreviewContainer = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._multiPreview, ref val9))
		{
			_multiPreview = ((Variant)(ref val9)).As<Control>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._multiPreviewCancelButton, ref val10))
		{
			_multiPreviewCancelButton = ((Variant)(ref val10)).As<NBackButton>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._multiPreviewConfirmButton, ref val11))
		{
			_multiPreviewConfirmButton = ((Variant)(ref val11)).As<NConfirmButton>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._closeButton, ref val12))
		{
			_closeButton = ((Variant)(ref val12)).As<NBackButton>();
		}
	}
}
