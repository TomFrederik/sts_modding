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
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;

[ScriptPath("res://src/Core/Nodes/Screens/CardSelection/NDeckTransformSelectScreen.cs")]
public sealed class NDeckTransformSelectScreen : NCardGridSelectionScreen
{
	public new class MethodName : NCardGridSelectionScreen.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshConfirmButtonVisibility = StringName.op_Implicit("RefreshConfirmButtonVisibility");

		public static readonly StringName CloseSelection = StringName.op_Implicit("CloseSelection");

		public static readonly StringName CancelSelection = StringName.op_Implicit("CancelSelection");

		public static readonly StringName ConfirmSelection = StringName.op_Implicit("ConfirmSelection");

		public static readonly StringName OpenPreviewScreen = StringName.op_Implicit("OpenPreviewScreen");

		public static readonly StringName CompleteSelection = StringName.op_Implicit("CompleteSelection");

		public static readonly StringName ToggleShowUpgrades = StringName.op_Implicit("ToggleShowUpgrades");

		public static readonly StringName OnControllerStateUpdated = StringName.op_Implicit("OnControllerStateUpdated");
	}

	public new class PropertyName : NCardGridSelectionScreen.PropertyName
	{
		public new static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public new static readonly StringName FocusedControlFromTopBar = StringName.op_Implicit("FocusedControlFromTopBar");

		public static readonly StringName _previewContainer = StringName.op_Implicit("_previewContainer");

		public static readonly StringName _transformPreview = StringName.op_Implicit("_transformPreview");

		public static readonly StringName _confirmButton = StringName.op_Implicit("_confirmButton");

		public static readonly StringName _previewCancelButton = StringName.op_Implicit("_previewCancelButton");

		public static readonly StringName _previewConfirmButton = StringName.op_Implicit("_previewConfirmButton");

		public static readonly StringName _bottomTextContainer = StringName.op_Implicit("_bottomTextContainer");

		public static readonly StringName _infoLabel = StringName.op_Implicit("_infoLabel");

		public static readonly StringName _viewUpgrades = StringName.op_Implicit("_viewUpgrades");

		public static readonly StringName _closeButton = StringName.op_Implicit("_closeButton");
	}

	public new class SignalName : NCardGridSelectionScreen.SignalName
	{
	}

	private readonly HashSet<CardModel> _selectedCards = new HashSet<CardModel>();

	private Func<CardModel, CardTransformation> _cardToTransformation;

	private CardSelectorPrefs _prefs;

	private Control _previewContainer;

	private NTransformPreview _transformPreview;

	private NConfirmButton _confirmButton;

	private NBackButton _previewCancelButton;

	private NConfirmButton _previewConfirmButton;

	private Control _bottomTextContainer;

	private MegaRichTextLabel _infoLabel;

	private NTickbox _viewUpgrades;

	private NBackButton _closeButton;

	private static string ScenePath => SceneHelper.GetScenePath("screens/card_selection/deck_transform_select_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	protected override IEnumerable<Control> PeekButtonTargets => new global::_003C_003Ez__ReadOnlyArray<Control>((Control[])(object)new Control[3] { _previewContainer, _closeButton, _bottomTextContainer });

	public override Control? DefaultFocusedControl
	{
		get
		{
			if (((CanvasItem)_previewContainer).Visible)
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
			if (((CanvasItem)_previewContainer).Visible)
			{
				return null;
			}
			return _grid.FocusedControlFromTopBar;
		}
	}

	public override void _Ready()
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignalsAndInitGrid();
		_confirmButton = ((Node)this).GetNode<NConfirmButton>(NodePath.op_Implicit("Confirm"));
		_previewContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%PreviewContainer"));
		_transformPreview = ((Node)_previewContainer).GetNode<NTransformPreview>(NodePath.op_Implicit("TransformPreview"));
		_previewCancelButton = ((Node)_previewContainer).GetNode<NBackButton>(NodePath.op_Implicit("Cancel"));
		_previewConfirmButton = ((Node)_previewContainer).GetNode<NConfirmButton>(NodePath.op_Implicit("Confirm"));
		_closeButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%Close"));
		((GodotObject)_previewCancelButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CancelSelection), 0u);
		((GodotObject)_previewConfirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CompleteSelection), 0u);
		((GodotObject)_closeButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CloseSelection), 0u);
		((GodotObject)_confirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ConfirmSelection), 0u);
		if (_prefs.Cancelable)
		{
			_closeButton.Enable();
		}
		else
		{
			_closeButton.Disable();
		}
		RefreshConfirmButtonVisibility();
		_previewCancelButton.Disable();
		_previewConfirmButton.Disable();
		_bottomTextContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BottomText"));
		_infoLabel = ((Node)_bottomTextContainer).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%BottomLabel"));
		_infoLabel.Text = _prefs.Prompt.GetFormattedText();
		_viewUpgrades = ((Node)this).GetNode<NTickbox>(NodePath.op_Implicit("%Upgrades"));
		_viewUpgrades.IsTicked = false;
		((GodotObject)_viewUpgrades).Connect(NTickbox.SignalName.Toggled, Callable.From<NTickbox>((Action<NTickbox>)ToggleShowUpgrades), 0u);
		OnControllerStateUpdated();
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)OnControllerStateUpdated), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)OnControllerStateUpdated), 0u);
		((GodotObject)NInputManager.Instance).Connect(NInputManager.SignalName.InputRebound, Callable.From((Action)OnControllerStateUpdated), 0u);
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ViewUpgradesLabel")).SetTextAutoSize(new LocString("card_selection", "VIEW_UPGRADES").GetFormattedText());
	}

	public static NDeckTransformSelectScreen ShowScreen(IReadOnlyList<CardModel> cards, Func<CardModel, CardTransformation> cardToTransformation, CardSelectorPrefs prefs)
	{
		NDeckTransformSelectScreen nDeckTransformSelectScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NDeckTransformSelectScreen>((GenEditState)0);
		((Node)nDeckTransformSelectScreen).Name = StringName.op_Implicit("NDeckTransformSelectScreen");
		nDeckTransformSelectScreen._cards = cards;
		nDeckTransformSelectScreen._cardToTransformation = cardToTransformation;
		nDeckTransformSelectScreen._prefs = prefs;
		NOverlayStack.Instance.Push(nDeckTransformSelectScreen);
		return nDeckTransformSelectScreen;
	}

	private void RefreshConfirmButtonVisibility()
	{
		if (_prefs.MinSelect != _prefs.MaxSelect && _selectedCards.Count >= _prefs.MinSelect)
		{
			_confirmButton.Enable();
		}
		else
		{
			_confirmButton.Disable();
		}
	}

	protected override void OnCardClicked(CardModel card)
	{
		if (_selectedCards.Add(card))
		{
			_grid.HighlightCard(card);
			if (_prefs.MaxSelect == _selectedCards.Count)
			{
				OpenPreviewScreen();
			}
		}
		else
		{
			_selectedCards.Remove(card);
			_grid.UnhighlightCard(card);
		}
		RefreshConfirmButtonVisibility();
	}

	private void CloseSelection(NButton _)
	{
		_completionSource.SetResult(Array.Empty<CardModel>());
		_previewCancelButton.Disable();
		_previewConfirmButton.Disable();
		NOverlayStack.Instance.Remove(this);
	}

	private void CancelSelection(NButton _)
	{
		((CanvasItem)_previewContainer).Visible = false;
		_previewContainer.MouseFilter = (MouseFilterEnum)2;
		_transformPreview.Uninitialize();
		_previewCancelButton.Disable();
		_previewConfirmButton.Disable();
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
		if (_selectedCards.Count >= _prefs.MinSelect)
		{
			if (_prefs.RequireManualConfirmation)
			{
				OpenPreviewScreen();
			}
			else
			{
				CompleteSelection(_);
			}
		}
	}

	private void OpenPreviewScreen()
	{
		((Node)this).GetViewport().GuiReleaseFocus();
		((CanvasItem)_previewContainer).Visible = true;
		_previewContainer.MouseFilter = (MouseFilterEnum)0;
		_previewCancelButton.Enable();
		_previewConfirmButton.Enable();
		foreach (CardModel selectedCard in _selectedCards)
		{
			_grid.UnhighlightCard(selectedCard);
		}
		_transformPreview.Initialize(_selectedCards.Select(_cardToTransformation));
		_closeButton.Disable();
	}

	private void CompleteSelection(NButton _)
	{
		_completionSource.SetResult(_selectedCards);
		NOverlayStack.Instance.Remove(this);
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
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Expected O, but got Unknown
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshConfirmButtonVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		list.Add(new MethodInfo(MethodName.OpenPreviewScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CompleteSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
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
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshConfirmButtonVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshConfirmButtonVisibility();
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
		if ((ref method) == MethodName.OpenPreviewScreen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenPreviewScreen();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CompleteSelection && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CompleteSelection(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.RefreshConfirmButtonVisibility)
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
		if ((ref method) == MethodName.OpenPreviewScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.CompleteSelection)
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
		if ((ref name) == PropertyName._previewContainer)
		{
			_previewContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._transformPreview)
		{
			_transformPreview = VariantUtils.ConvertTo<NTransformPreview>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._confirmButton)
		{
			_confirmButton = VariantUtils.ConvertTo<NConfirmButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._previewCancelButton)
		{
			_previewCancelButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._previewConfirmButton)
		{
			_previewConfirmButton = VariantUtils.ConvertTo<NConfirmButton>(ref value);
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
		if ((ref name) == PropertyName._viewUpgrades)
		{
			_viewUpgrades = VariantUtils.ConvertTo<NTickbox>(ref value);
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
		if ((ref name) == PropertyName._previewContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _previewContainer);
			return true;
		}
		if ((ref name) == PropertyName._transformPreview)
		{
			value = VariantUtils.CreateFrom<NTransformPreview>(ref _transformPreview);
			return true;
		}
		if ((ref name) == PropertyName._confirmButton)
		{
			value = VariantUtils.CreateFrom<NConfirmButton>(ref _confirmButton);
			return true;
		}
		if ((ref name) == PropertyName._previewCancelButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _previewCancelButton);
			return true;
		}
		if ((ref name) == PropertyName._previewConfirmButton)
		{
			value = VariantUtils.CreateFrom<NConfirmButton>(ref _previewConfirmButton);
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
		if ((ref name) == PropertyName._viewUpgrades)
		{
			value = VariantUtils.CreateFrom<NTickbox>(ref _viewUpgrades);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._previewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._transformPreview, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._confirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._previewCancelButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._previewConfirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bottomTextContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infoLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewUpgrades, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._previewContainer, Variant.From<Control>(ref _previewContainer));
		info.AddProperty(PropertyName._transformPreview, Variant.From<NTransformPreview>(ref _transformPreview));
		info.AddProperty(PropertyName._confirmButton, Variant.From<NConfirmButton>(ref _confirmButton));
		info.AddProperty(PropertyName._previewCancelButton, Variant.From<NBackButton>(ref _previewCancelButton));
		info.AddProperty(PropertyName._previewConfirmButton, Variant.From<NConfirmButton>(ref _previewConfirmButton));
		info.AddProperty(PropertyName._bottomTextContainer, Variant.From<Control>(ref _bottomTextContainer));
		info.AddProperty(PropertyName._infoLabel, Variant.From<MegaRichTextLabel>(ref _infoLabel));
		info.AddProperty(PropertyName._viewUpgrades, Variant.From<NTickbox>(ref _viewUpgrades));
		info.AddProperty(PropertyName._closeButton, Variant.From<NBackButton>(ref _closeButton));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._previewContainer, ref val))
		{
			_previewContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._transformPreview, ref val2))
		{
			_transformPreview = ((Variant)(ref val2)).As<NTransformPreview>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._confirmButton, ref val3))
		{
			_confirmButton = ((Variant)(ref val3)).As<NConfirmButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._previewCancelButton, ref val4))
		{
			_previewCancelButton = ((Variant)(ref val4)).As<NBackButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._previewConfirmButton, ref val5))
		{
			_previewConfirmButton = ((Variant)(ref val5)).As<NConfirmButton>();
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
		if (info.TryGetProperty(PropertyName._viewUpgrades, ref val8))
		{
			_viewUpgrades = ((Variant)(ref val8)).As<NTickbox>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._closeButton, ref val9))
		{
			_closeButton = ((Variant)(ref val9)).As<NBackButton>();
		}
	}
}
