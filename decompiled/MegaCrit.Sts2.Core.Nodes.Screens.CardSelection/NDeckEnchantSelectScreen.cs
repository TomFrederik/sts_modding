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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;

[ScriptPath("res://src/Core/Nodes/Screens/CardSelection/NDeckEnchantSelectScreen.cs")]
public sealed class NDeckEnchantSelectScreen : NCardGridSelectionScreen
{
	public new class MethodName : NCardGridSelectionScreen.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshConfirmButtonVisibility = StringName.op_Implicit("RefreshConfirmButtonVisibility");

		public static readonly StringName CloseSelection = StringName.op_Implicit("CloseSelection");

		public static readonly StringName CancelSelection = StringName.op_Implicit("CancelSelection");

		public static readonly StringName PreviewSelection = StringName.op_Implicit("PreviewSelection");

		public static readonly StringName ConfirmSelection = StringName.op_Implicit("ConfirmSelection");

		public static readonly StringName CheckIfSelectionComplete = StringName.op_Implicit("CheckIfSelectionComplete");
	}

	public new class PropertyName : NCardGridSelectionScreen.PropertyName
	{
		public static readonly StringName UseSingleSelection = StringName.op_Implicit("UseSingleSelection");

		public new static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public new static readonly StringName FocusedControlFromTopBar = StringName.op_Implicit("FocusedControlFromTopBar");

		public static readonly StringName _enchantmentAmount = StringName.op_Implicit("_enchantmentAmount");

		public static readonly StringName _enchantSinglePreviewContainer = StringName.op_Implicit("_enchantSinglePreviewContainer");

		public static readonly StringName _singlePreview = StringName.op_Implicit("_singlePreview");

		public static readonly StringName _singlePreviewCancelButton = StringName.op_Implicit("_singlePreviewCancelButton");

		public static readonly StringName _singlePreviewConfirmButton = StringName.op_Implicit("_singlePreviewConfirmButton");

		public static readonly StringName _confirmButton = StringName.op_Implicit("_confirmButton");

		public static readonly StringName _enchantMultiPreviewContainer = StringName.op_Implicit("_enchantMultiPreviewContainer");

		public static readonly StringName _multiPreview = StringName.op_Implicit("_multiPreview");

		public static readonly StringName _multiPreviewCancelButton = StringName.op_Implicit("_multiPreviewCancelButton");

		public static readonly StringName _multiPreviewConfirmButton = StringName.op_Implicit("_multiPreviewConfirmButton");

		public static readonly StringName _enchantmentDescriptionContainer = StringName.op_Implicit("_enchantmentDescriptionContainer");

		public static readonly StringName _enchantmentTitle = StringName.op_Implicit("_enchantmentTitle");

		public static readonly StringName _enchantmentDescription = StringName.op_Implicit("_enchantmentDescription");

		public static readonly StringName _enchantmentIcon = StringName.op_Implicit("_enchantmentIcon");

		public static readonly StringName _bottomTextContainer = StringName.op_Implicit("_bottomTextContainer");

		public static readonly StringName _infoLabel = StringName.op_Implicit("_infoLabel");

		public static readonly StringName _closeButton = StringName.op_Implicit("_closeButton");
	}

	public new class SignalName : NCardGridSelectionScreen.SignalName
	{
	}

	private readonly HashSet<CardModel> _selectedCards = new HashSet<CardModel>();

	private CardSelectorPrefs _prefs;

	private EnchantmentModel _enchantment;

	private int _enchantmentAmount;

	private Control _enchantSinglePreviewContainer;

	private NEnchantPreview _singlePreview;

	private NBackButton _singlePreviewCancelButton;

	private NConfirmButton _singlePreviewConfirmButton;

	private NConfirmButton _confirmButton;

	private Control _enchantMultiPreviewContainer;

	private Control _multiPreview;

	private NBackButton _multiPreviewCancelButton;

	private NConfirmButton _multiPreviewConfirmButton;

	private Control _enchantmentDescriptionContainer;

	private MegaLabel _enchantmentTitle;

	private MegaRichTextLabel _enchantmentDescription;

	private TextureRect _enchantmentIcon;

	private Control _bottomTextContainer;

	private MegaRichTextLabel _infoLabel;

	private NBackButton _closeButton;

	private static string ScenePath => SceneHelper.GetScenePath("screens/card_selection/deck_enchant_select_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	private bool UseSingleSelection => _prefs.MaxSelect == 1;

	protected override IEnumerable<Control> PeekButtonTargets => new global::_003C_003Ez__ReadOnlyArray<Control>((Control[])(object)new Control[5] { _enchantSinglePreviewContainer, _enchantMultiPreviewContainer, _enchantmentDescriptionContainer, _closeButton, _bottomTextContainer });

	public override Control? DefaultFocusedControl
	{
		get
		{
			if (((CanvasItem)_enchantSinglePreviewContainer).Visible || ((CanvasItem)_enchantMultiPreviewContainer).Visible)
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
			if (((CanvasItem)_enchantSinglePreviewContainer).Visible || ((CanvasItem)_enchantMultiPreviewContainer).Visible)
			{
				return null;
			}
			return _grid.FocusedControlFromTopBar;
		}
	}

	public override void _Ready()
	{
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignalsAndInitGrid();
		_confirmButton = ((Node)this).GetNode<NConfirmButton>(NodePath.op_Implicit("Confirm"));
		_enchantSinglePreviewContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%EnchantSinglePreviewContainer"));
		_singlePreview = ((Node)_enchantSinglePreviewContainer).GetNode<NEnchantPreview>(NodePath.op_Implicit("EnchantPreview"));
		_singlePreviewCancelButton = ((Node)_enchantSinglePreviewContainer).GetNode<NBackButton>(NodePath.op_Implicit("Cancel"));
		_singlePreviewConfirmButton = ((Node)_enchantSinglePreviewContainer).GetNode<NConfirmButton>(NodePath.op_Implicit("Confirm"));
		_enchantMultiPreviewContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%EnchantMultiPreviewContainer"));
		_multiPreview = ((Node)_enchantMultiPreviewContainer).GetNode<Control>(NodePath.op_Implicit("Cards"));
		_multiPreviewCancelButton = ((Node)_enchantMultiPreviewContainer).GetNode<NBackButton>(NodePath.op_Implicit("Cancel"));
		_multiPreviewConfirmButton = ((Node)_enchantMultiPreviewContainer).GetNode<NConfirmButton>(NodePath.op_Implicit("Confirm"));
		_enchantmentDescriptionContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%EnchantmentDescriptionContainer"));
		_enchantmentIcon = ((Node)_enchantmentDescriptionContainer).GetNode<TextureRect>(NodePath.op_Implicit("%EnchantmentIcon"));
		_enchantmentTitle = ((Node)_enchantmentDescriptionContainer).GetNode<MegaLabel>(NodePath.op_Implicit("%EnchantmentTitle"));
		_enchantmentDescription = ((Node)_enchantmentDescriptionContainer).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%EnchantmentDescription"));
		_closeButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%Close"));
		((GodotObject)_singlePreviewCancelButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CancelSelection), 0u);
		((GodotObject)_singlePreviewConfirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ConfirmSelection), 0u);
		((GodotObject)_multiPreviewCancelButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CancelSelection), 0u);
		((GodotObject)_multiPreviewConfirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ConfirmSelection), 0u);
		((GodotObject)_closeButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CloseSelection), 0u);
		((GodotObject)_confirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)PreviewSelection), 0u);
		if (_prefs.Cancelable)
		{
			_closeButton.Enable();
		}
		else
		{
			_closeButton.Disable();
		}
		EnchantmentModel enchantmentModel = _enchantment.ToMutable();
		enchantmentModel.Amount = _enchantmentAmount;
		enchantmentModel.RecalculateValues();
		_enchantmentTitle.SetTextAutoSize(enchantmentModel.Title.GetFormattedText());
		_enchantmentDescription.Text = enchantmentModel.DynamicDescription.GetFormattedText();
		_enchantmentIcon.Texture = (Texture2D)(object)enchantmentModel.Icon;
		((CanvasItem)_enchantSinglePreviewContainer).Visible = false;
		_enchantSinglePreviewContainer.MouseFilter = (MouseFilterEnum)2;
		((CanvasItem)_enchantMultiPreviewContainer).Visible = false;
		_enchantMultiPreviewContainer.MouseFilter = (MouseFilterEnum)2;
		RefreshConfirmButtonVisibility();
		_bottomTextContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BottomText"));
		_infoLabel = ((Node)_bottomTextContainer).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%BottomLabel"));
		_infoLabel.Text = "[center]" + _prefs.Prompt.GetFormattedText() + "[/center]";
	}

	public static NDeckEnchantSelectScreen ShowScreen(IReadOnlyList<CardModel> cards, EnchantmentModel enchantment, int amount, CardSelectorPrefs prefs)
	{
		NDeckEnchantSelectScreen nDeckEnchantSelectScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NDeckEnchantSelectScreen>((GenEditState)0);
		((Node)nDeckEnchantSelectScreen).Name = StringName.op_Implicit("NDeckEnchantSelectScreen");
		nDeckEnchantSelectScreen._cards = cards;
		nDeckEnchantSelectScreen._prefs = prefs;
		nDeckEnchantSelectScreen._enchantment = enchantment;
		nDeckEnchantSelectScreen._enchantmentAmount = amount;
		NOverlayStack.Instance.Push(nDeckEnchantSelectScreen);
		return nDeckEnchantSelectScreen;
	}

	protected override void OnCardClicked(CardModel card)
	{
		if (_selectedCards.Add(card))
		{
			_grid.HighlightCard(card);
			if (_prefs.MaxSelect == _selectedCards.Count)
			{
				PreviewSelection();
			}
		}
		else
		{
			_selectedCards.Remove(card);
			_grid.UnhighlightCard(card);
		}
		RefreshConfirmButtonVisibility();
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

	private void CloseSelection(NButton _)
	{
		_completionSource.SetResult(Array.Empty<CardModel>());
		NOverlayStack.Instance.Remove(this);
	}

	private void CancelSelection(NButton _)
	{
		if (UseSingleSelection)
		{
			_singlePreviewCancelButton.Disable();
			_singlePreviewConfirmButton.Disable();
			((CanvasItem)_enchantSinglePreviewContainer).Visible = false;
			_enchantSinglePreviewContainer.MouseFilter = (MouseFilterEnum)2;
		}
		else
		{
			_multiPreviewCancelButton.Disable();
			_multiPreviewConfirmButton.Disable();
			((CanvasItem)_enchantMultiPreviewContainer).Visible = false;
			_enchantMultiPreviewContainer.MouseFilter = (MouseFilterEnum)2;
			for (int i = 0; i < ((Node)_multiPreview).GetChildCount(false); i++)
			{
				((Node)_multiPreview).GetChild(i, false).QueueFreeSafely();
			}
		}
		_grid.SetCanScroll(canScroll: true);
		ActiveScreenContext.Instance.Update();
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

	private void PreviewSelection(NButton _)
	{
		PreviewSelection();
	}

	private void PreviewSelection()
	{
		if (UseSingleSelection)
		{
			_grid.SetCanScroll(canScroll: false);
			_closeButton.Disable();
			((Node)this).GetViewport().GuiReleaseFocus();
			((CanvasItem)_enchantSinglePreviewContainer).Visible = true;
			_enchantSinglePreviewContainer.MouseFilter = (MouseFilterEnum)0;
			_singlePreview.Init(_selectedCards.First(), _enchantment, _enchantmentAmount);
			_singlePreviewCancelButton.Enable();
			_singlePreviewConfirmButton.Enable();
			return;
		}
		_grid.SetCanScroll(canScroll: false);
		_closeButton.Disable();
		((Node)this).GetViewport().GuiReleaseFocus();
		((CanvasItem)_enchantMultiPreviewContainer).Visible = true;
		_enchantMultiPreviewContainer.MouseFilter = (MouseFilterEnum)0;
		_multiPreviewCancelButton.Enable();
		_multiPreviewConfirmButton.Enable();
		foreach (CardModel selectedCard in _selectedCards)
		{
			NCard nCard = NCard.Create(selectedCard);
			((Node)(object)_multiPreview).AddChildSafely((Node?)(object)NPreviewCardHolder.Create(nCard, showHoverTips: true, scaleOnHover: false));
			nCard.UpdateVisuals(selectedCard.Pile.Type, CardPreviewMode.Normal);
		}
	}

	private void ConfirmSelection(NButton inputEvent)
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
		if (_selectedCards.Count >= _prefs.MinSelect && _selectedCards.Count <= _prefs.MaxSelect)
		{
			_completionSource.SetResult(_selectedCards);
			NOverlayStack.Instance.Remove(this);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
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
		list.Add(new MethodInfo(MethodName.PreviewSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PreviewSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConfirmSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CheckIfSelectionComplete, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.PreviewSelection && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PreviewSelection(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PreviewSelection && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PreviewSelection();
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
		if ((ref method) == MethodName.PreviewSelection)
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
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._enchantmentAmount)
		{
			_enchantmentAmount = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantSinglePreviewContainer)
		{
			_enchantSinglePreviewContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._singlePreview)
		{
			_singlePreview = VariantUtils.ConvertTo<NEnchantPreview>(ref value);
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
		if ((ref name) == PropertyName._confirmButton)
		{
			_confirmButton = VariantUtils.ConvertTo<NConfirmButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantMultiPreviewContainer)
		{
			_enchantMultiPreviewContainer = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName._enchantmentDescriptionContainer)
		{
			_enchantmentDescriptionContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentTitle)
		{
			_enchantmentTitle = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentDescription)
		{
			_enchantmentDescription = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentIcon)
		{
			_enchantmentIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
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
		if ((ref name) == PropertyName._enchantmentAmount)
		{
			value = VariantUtils.CreateFrom<int>(ref _enchantmentAmount);
			return true;
		}
		if ((ref name) == PropertyName._enchantSinglePreviewContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _enchantSinglePreviewContainer);
			return true;
		}
		if ((ref name) == PropertyName._singlePreview)
		{
			value = VariantUtils.CreateFrom<NEnchantPreview>(ref _singlePreview);
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
		if ((ref name) == PropertyName._confirmButton)
		{
			value = VariantUtils.CreateFrom<NConfirmButton>(ref _confirmButton);
			return true;
		}
		if ((ref name) == PropertyName._enchantMultiPreviewContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _enchantMultiPreviewContainer);
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
		if ((ref name) == PropertyName._enchantmentDescriptionContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _enchantmentDescriptionContainer);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentTitle)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _enchantmentTitle);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentDescription)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _enchantmentDescription);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _enchantmentIcon);
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
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.UseSingleSelection, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._enchantmentAmount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantSinglePreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._singlePreview, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._singlePreviewCancelButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._singlePreviewConfirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._confirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantMultiPreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._multiPreview, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._multiPreviewCancelButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._multiPreviewConfirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentDescriptionContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentTitle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentDescription, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bottomTextContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infoLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._enchantmentAmount, Variant.From<int>(ref _enchantmentAmount));
		info.AddProperty(PropertyName._enchantSinglePreviewContainer, Variant.From<Control>(ref _enchantSinglePreviewContainer));
		info.AddProperty(PropertyName._singlePreview, Variant.From<NEnchantPreview>(ref _singlePreview));
		info.AddProperty(PropertyName._singlePreviewCancelButton, Variant.From<NBackButton>(ref _singlePreviewCancelButton));
		info.AddProperty(PropertyName._singlePreviewConfirmButton, Variant.From<NConfirmButton>(ref _singlePreviewConfirmButton));
		info.AddProperty(PropertyName._confirmButton, Variant.From<NConfirmButton>(ref _confirmButton));
		info.AddProperty(PropertyName._enchantMultiPreviewContainer, Variant.From<Control>(ref _enchantMultiPreviewContainer));
		info.AddProperty(PropertyName._multiPreview, Variant.From<Control>(ref _multiPreview));
		info.AddProperty(PropertyName._multiPreviewCancelButton, Variant.From<NBackButton>(ref _multiPreviewCancelButton));
		info.AddProperty(PropertyName._multiPreviewConfirmButton, Variant.From<NConfirmButton>(ref _multiPreviewConfirmButton));
		info.AddProperty(PropertyName._enchantmentDescriptionContainer, Variant.From<Control>(ref _enchantmentDescriptionContainer));
		info.AddProperty(PropertyName._enchantmentTitle, Variant.From<MegaLabel>(ref _enchantmentTitle));
		info.AddProperty(PropertyName._enchantmentDescription, Variant.From<MegaRichTextLabel>(ref _enchantmentDescription));
		info.AddProperty(PropertyName._enchantmentIcon, Variant.From<TextureRect>(ref _enchantmentIcon));
		info.AddProperty(PropertyName._bottomTextContainer, Variant.From<Control>(ref _bottomTextContainer));
		info.AddProperty(PropertyName._infoLabel, Variant.From<MegaRichTextLabel>(ref _infoLabel));
		info.AddProperty(PropertyName._closeButton, Variant.From<NBackButton>(ref _closeButton));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentAmount, ref val))
		{
			_enchantmentAmount = ((Variant)(ref val)).As<int>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantSinglePreviewContainer, ref val2))
		{
			_enchantSinglePreviewContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._singlePreview, ref val3))
		{
			_singlePreview = ((Variant)(ref val3)).As<NEnchantPreview>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._singlePreviewCancelButton, ref val4))
		{
			_singlePreviewCancelButton = ((Variant)(ref val4)).As<NBackButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._singlePreviewConfirmButton, ref val5))
		{
			_singlePreviewConfirmButton = ((Variant)(ref val5)).As<NConfirmButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._confirmButton, ref val6))
		{
			_confirmButton = ((Variant)(ref val6)).As<NConfirmButton>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantMultiPreviewContainer, ref val7))
		{
			_enchantMultiPreviewContainer = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._multiPreview, ref val8))
		{
			_multiPreview = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._multiPreviewCancelButton, ref val9))
		{
			_multiPreviewCancelButton = ((Variant)(ref val9)).As<NBackButton>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._multiPreviewConfirmButton, ref val10))
		{
			_multiPreviewConfirmButton = ((Variant)(ref val10)).As<NConfirmButton>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentDescriptionContainer, ref val11))
		{
			_enchantmentDescriptionContainer = ((Variant)(ref val11)).As<Control>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentTitle, ref val12))
		{
			_enchantmentTitle = ((Variant)(ref val12)).As<MegaLabel>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentDescription, ref val13))
		{
			_enchantmentDescription = ((Variant)(ref val13)).As<MegaRichTextLabel>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentIcon, ref val14))
		{
			_enchantmentIcon = ((Variant)(ref val14)).As<TextureRect>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._bottomTextContainer, ref val15))
		{
			_bottomTextContainer = ((Variant)(ref val15)).As<Control>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._infoLabel, ref val16))
		{
			_infoLabel = ((Variant)(ref val16)).As<MegaRichTextLabel>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._closeButton, ref val17))
		{
			_closeButton = ((Variant)(ref val17)).As<NBackButton>();
		}
	}
}
