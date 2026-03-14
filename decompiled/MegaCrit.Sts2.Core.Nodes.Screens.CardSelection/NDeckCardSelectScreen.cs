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
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;

[ScriptPath("res://src/Core/Nodes/Screens/CardSelection/NDeckCardSelectScreen.cs")]
public sealed class NDeckCardSelectScreen : NCardGridSelectionScreen
{
	public new class MethodName : NCardGridSelectionScreen.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshConfirmButtonVisibility = StringName.op_Implicit("RefreshConfirmButtonVisibility");

		public static readonly StringName PreviewSelection = StringName.op_Implicit("PreviewSelection");

		public static readonly StringName CloseSelection = StringName.op_Implicit("CloseSelection");

		public static readonly StringName CancelSelection = StringName.op_Implicit("CancelSelection");

		public static readonly StringName ConfirmSelection = StringName.op_Implicit("ConfirmSelection");

		public static readonly StringName CheckIfSelectionComplete = StringName.op_Implicit("CheckIfSelectionComplete");

		public new static readonly StringName AfterOverlayShown = StringName.op_Implicit("AfterOverlayShown");

		public new static readonly StringName AfterOverlayHidden = StringName.op_Implicit("AfterOverlayHidden");
	}

	public new class PropertyName : NCardGridSelectionScreen.PropertyName
	{
		public new static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public new static readonly StringName FocusedControlFromTopBar = StringName.op_Implicit("FocusedControlFromTopBar");

		public static readonly StringName _previewContainer = StringName.op_Implicit("_previewContainer");

		public static readonly StringName _previewCards = StringName.op_Implicit("_previewCards");

		public static readonly StringName _previewCancelButton = StringName.op_Implicit("_previewCancelButton");

		public static readonly StringName _previewConfirmButton = StringName.op_Implicit("_previewConfirmButton");

		public static readonly StringName _closeButton = StringName.op_Implicit("_closeButton");

		public static readonly StringName _confirmButton = StringName.op_Implicit("_confirmButton");

		public static readonly StringName _infoLabel = StringName.op_Implicit("_infoLabel");
	}

	public new class SignalName : NCardGridSelectionScreen.SignalName
	{
	}

	private readonly HashSet<CardModel> _selectedCards = new HashSet<CardModel>();

	private CardSelectorPrefs _prefs;

	private Control _previewContainer;

	private Control _previewCards;

	private NBackButton _previewCancelButton;

	private NConfirmButton _previewConfirmButton;

	private NBackButton _closeButton;

	private NConfirmButton _confirmButton;

	private MegaRichTextLabel _infoLabel;

	private static string ScenePath => SceneHelper.GetScenePath("screens/card_selection/deck_card_select_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	protected override IEnumerable<Control> PeekButtonTargets => new global::_003C_003Ez__ReadOnlyArray<Control>((Control[])(object)new Control[3] { _previewContainer, _closeButton, _confirmButton });

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
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignalsAndInitGrid();
		_previewContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%PreviewContainer"));
		_previewCards = ((Node)_previewContainer).GetNode<Control>(NodePath.op_Implicit("%Cards"));
		_previewCancelButton = ((Node)_previewContainer).GetNode<NBackButton>(NodePath.op_Implicit("%PreviewCancel"));
		_previewConfirmButton = ((Node)_previewContainer).GetNode<NConfirmButton>(NodePath.op_Implicit("%PreviewConfirm"));
		_closeButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%Close"));
		_confirmButton = ((Node)this).GetNode<NConfirmButton>(NodePath.op_Implicit("%Confirm"));
		_infoLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%BottomLabel"));
		((GodotObject)_previewCancelButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CancelSelection), 0u);
		((GodotObject)_previewConfirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ConfirmSelection), 0u);
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
		RefreshConfirmButtonVisibility();
		((CanvasItem)_previewContainer).Visible = false;
		_previewContainer.MouseFilter = (MouseFilterEnum)2;
		_previewCancelButton.Disable();
		_previewConfirmButton.Disable();
		_infoLabel.Text = _prefs.Prompt.GetFormattedText();
	}

	public static NDeckCardSelectScreen Create(IReadOnlyList<CardModel> cards, CardSelectorPrefs prefs)
	{
		NDeckCardSelectScreen nDeckCardSelectScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NDeckCardSelectScreen>((GenEditState)0);
		((Node)nDeckCardSelectScreen).Name = StringName.op_Implicit("NDeckCardSelectScreen");
		nDeckCardSelectScreen._cards = cards;
		nDeckCardSelectScreen._prefs = prefs;
		return nDeckCardSelectScreen;
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

	private void PreviewSelection(NButton _)
	{
		PreviewSelection();
	}

	private void PreviewSelection()
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		((Node)this).GetViewport().GuiReleaseFocus();
		((CanvasItem)_previewContainer).Visible = true;
		_previewContainer.MouseFilter = (MouseFilterEnum)0;
		_closeButton.Disable();
		_grid.SetCanScroll(canScroll: false);
		_previewCancelButton.Enable();
		_previewConfirmButton.Enable();
		foreach (CardModel selectedCard in _selectedCards)
		{
			_grid.UnhighlightCard(selectedCard);
			NCard nCard = NCard.Create(selectedCard);
			NPreviewCardHolder child = NPreviewCardHolder.Create(nCard, showHoverTips: true, scaleOnHover: false);
			((Node)(object)_previewCards).AddChildSafely((Node?)(object)child);
			nCard.UpdateVisuals(selectedCard.Pile.Type, CardPreviewMode.Normal);
		}
		Callable val = Callable.From((Action)delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			_previewCards.PivotOffset = _previewCards.Size / 2f;
			float num = 1f;
			if (_selectedCards.Count > 6)
			{
				num = 0.55f;
			}
			else if (_selectedCards.Count > 3)
			{
				num = 0.8f;
			}
			_previewCards.Scale = Vector2.One * num;
		});
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
	}

	private void CloseSelection(NButton _)
	{
		_completionSource.SetResult(Array.Empty<CardModel>());
		NOverlayStack.Instance.Remove(this);
	}

	private void CancelSelection(NButton _)
	{
		((CanvasItem)_previewContainer).Visible = false;
		_previewCancelButton.Disable();
		_previewConfirmButton.Disable();
		_grid.SetCanScroll(canScroll: true);
		_previewContainer.MouseFilter = (MouseFilterEnum)2;
		for (int i = 0; i < ((Node)_previewCards).GetChildCount(false); i++)
		{
			((Node)_previewCards).GetChild(i, false).QueueFreeSafely();
		}
		((Control)(object)_grid.GetCardHolder(_selectedCards.Last()))?.TryGrabFocus();
		_selectedCards.Clear();
		if (_prefs.Cancelable)
		{
			_closeButton.Enable();
		}
	}

	private void ConfirmSelection(NButton _)
	{
		CheckIfSelectionComplete();
	}

	private void CheckIfSelectionComplete()
	{
		if (_selectedCards.Count >= _prefs.MinSelect)
		{
			_completionSource.SetResult(_selectedCards);
			NOverlayStack.Instance.Remove(this);
		}
	}

	public override void AfterOverlayShown()
	{
		if (_prefs.Cancelable)
		{
			_closeButton.Enable();
		}
	}

	public override void AfterOverlayHidden()
	{
		_closeButton.Disable();
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
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshConfirmButtonVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PreviewSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PreviewSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		list.Add(new MethodInfo(MethodName.AfterOverlayShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.PreviewSelection)
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
		if ((ref method) == MethodName.AfterOverlayShown)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayHidden)
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
		if ((ref name) == PropertyName._previewCards)
		{
			_previewCards = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName._closeButton)
		{
			_closeButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._confirmButton)
		{
			_confirmButton = VariantUtils.ConvertTo<NConfirmButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._infoLabel)
		{
			_infoLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
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
		if ((ref name) == PropertyName._previewCards)
		{
			value = VariantUtils.CreateFrom<Control>(ref _previewCards);
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
		if ((ref name) == PropertyName._closeButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _closeButton);
			return true;
		}
		if ((ref name) == PropertyName._confirmButton)
		{
			value = VariantUtils.CreateFrom<NConfirmButton>(ref _confirmButton);
			return true;
		}
		if ((ref name) == PropertyName._infoLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _infoLabel);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._previewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._previewCards, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._previewCancelButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._previewConfirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._closeButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._confirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infoLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._previewContainer, Variant.From<Control>(ref _previewContainer));
		info.AddProperty(PropertyName._previewCards, Variant.From<Control>(ref _previewCards));
		info.AddProperty(PropertyName._previewCancelButton, Variant.From<NBackButton>(ref _previewCancelButton));
		info.AddProperty(PropertyName._previewConfirmButton, Variant.From<NConfirmButton>(ref _previewConfirmButton));
		info.AddProperty(PropertyName._closeButton, Variant.From<NBackButton>(ref _closeButton));
		info.AddProperty(PropertyName._confirmButton, Variant.From<NConfirmButton>(ref _confirmButton));
		info.AddProperty(PropertyName._infoLabel, Variant.From<MegaRichTextLabel>(ref _infoLabel));
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
		if (info.TryGetProperty(PropertyName._previewCards, ref val2))
		{
			_previewCards = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._previewCancelButton, ref val3))
		{
			_previewCancelButton = ((Variant)(ref val3)).As<NBackButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._previewConfirmButton, ref val4))
		{
			_previewConfirmButton = ((Variant)(ref val4)).As<NConfirmButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._closeButton, ref val5))
		{
			_closeButton = ((Variant)(ref val5)).As<NBackButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._confirmButton, ref val6))
		{
			_confirmButton = ((Variant)(ref val6)).As<NConfirmButton>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._infoLabel, ref val7))
		{
			_infoLabel = ((Variant)(ref val7)).As<MegaRichTextLabel>();
		}
	}
}
