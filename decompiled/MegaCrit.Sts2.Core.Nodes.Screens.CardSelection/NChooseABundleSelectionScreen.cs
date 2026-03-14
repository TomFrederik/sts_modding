using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
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
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;

[ScriptPath("res://src/Core/Nodes/Screens/CardSelection/NChooseABundleSelectionScreen.cs")]
public class NChooseABundleSelectionScreen : Control, IOverlayScreen, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnBundleClicked = StringName.op_Implicit("OnBundleClicked");

		public static readonly StringName OpenPreviewScreen = StringName.op_Implicit("OpenPreviewScreen");

		public static readonly StringName CancelSelection = StringName.op_Implicit("CancelSelection");

		public static readonly StringName ConfirmSelection = StringName.op_Implicit("ConfirmSelection");

		public static readonly StringName AfterOverlayOpened = StringName.op_Implicit("AfterOverlayOpened");

		public static readonly StringName AfterOverlayClosed = StringName.op_Implicit("AfterOverlayClosed");

		public static readonly StringName AfterOverlayShown = StringName.op_Implicit("AfterOverlayShown");

		public static readonly StringName AfterOverlayHidden = StringName.op_Implicit("AfterOverlayHidden");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public static readonly StringName UseSharedBackstop = StringName.op_Implicit("UseSharedBackstop");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _bundleRow = StringName.op_Implicit("_bundleRow");

		public static readonly StringName _bundlePreviewContainer = StringName.op_Implicit("_bundlePreviewContainer");

		public static readonly StringName _bundlePreviewCards = StringName.op_Implicit("_bundlePreviewCards");

		public static readonly StringName _previewCancelButton = StringName.op_Implicit("_previewCancelButton");

		public static readonly StringName _previewConfirmButton = StringName.op_Implicit("_previewConfirmButton");

		public static readonly StringName _selectedBundle = StringName.op_Implicit("_selectedBundle");

		public static readonly StringName _banner = StringName.op_Implicit("_banner");

		public static readonly StringName _peekButton = StringName.op_Implicit("_peekButton");

		public static readonly StringName _fadeTween = StringName.op_Implicit("_fadeTween");

		public static readonly StringName _cardTween = StringName.op_Implicit("_cardTween");
	}

	public class SignalName : SignalName
	{
	}

	private Control _bundleRow;

	private IReadOnlyList<IReadOnlyList<CardModel>> _bundles;

	private Control _bundlePreviewContainer;

	private Control _bundlePreviewCards;

	private NBackButton _previewCancelButton;

	private NConfirmButton _previewConfirmButton;

	private NCardBundle? _selectedBundle;

	private NCommonBanner _banner;

	private readonly TaskCompletionSource<IEnumerable<IReadOnlyList<CardModel>>> _completionSource = new TaskCompletionSource<IEnumerable<IReadOnlyList<CardModel>>>();

	private NPeekButton _peekButton;

	private Tween? _fadeTween;

	private const float _cardXSpacing = 400f;

	private Tween? _cardTween;

	private static string ScenePath => SceneHelper.GetScenePath("/screens/card_selection/choose_a_bundle_selection_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public NetScreenType ScreenType => NetScreenType.CardSelection;

	public bool UseSharedBackstop => true;

	public Control DefaultFocusedControl
	{
		get
		{
			if (!((CanvasItem)_bundlePreviewContainer).Visible)
			{
				return (Control)(object)((Node)_bundleRow).GetChild<NCardBundle>(0, false).Hitbox;
			}
			return ((Node)_bundlePreviewCards).GetChild<Control>(((Node)_bundlePreviewCards).GetChildCount(false) - 1, false);
		}
	}

	public override void _Ready()
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		_bundleRow = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BundleRow"));
		_bundlePreviewContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BundlePreviewContainer"));
		_bundlePreviewCards = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Cards"));
		_previewCancelButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%Cancel"));
		_previewConfirmButton = ((Node)this).GetNode<NConfirmButton>(NodePath.op_Implicit("%Confirm"));
		_banner = ((Node)this).GetNode<NCommonBanner>(NodePath.op_Implicit("Banner"));
		_banner.label.SetTextAutoSize(new LocString("gameplay_ui", "CHOOSE_A_PACK").GetRawText());
		_banner.AnimateIn();
		((GodotObject)_previewCancelButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CancelSelection), 0u);
		((GodotObject)_previewConfirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ConfirmSelection), 0u);
		_previewCancelButton.Disable();
		_previewConfirmButton.Disable();
		Vector2 val = Vector2.Left * (float)(_bundles.Count - 1) * 400f * 0.5f;
		for (int i = 0; i < _bundles.Count; i++)
		{
			NCardBundle nCardBundle = NCardBundle.Create(_bundles[i]);
			((Node)(object)_bundleRow).AddChildSafely((Node?)(object)nCardBundle);
			((GodotObject)nCardBundle).Connect(NCardBundle.SignalName.Clicked, Callable.From<NCardBundle>((Action<NCardBundle>)OnBundleClicked), 0u);
			((Control)nCardBundle).Scale = nCardBundle.smallScale;
			((Control)nCardBundle).Position = ((Control)nCardBundle).Position + (val + Vector2.Right * 400f * (float)i);
		}
		for (int j = 0; j < ((Node)_bundleRow).GetChildCount(false); j++)
		{
			NCardBundle child = ((Node)_bundleRow).GetChild<NCardBundle>(j, false);
			((Control)child.Hitbox).FocusNeighborLeft = ((j > 0) ? ((Node)((Node)_bundleRow).GetChild<NCardBundle>(j - 1, false).Hitbox).GetPath() : ((Node)((Node)_bundleRow).GetChild<NCardBundle>(((Node)_bundleRow).GetChildCount(false) - 1, false).Hitbox).GetPath());
			((Control)child.Hitbox).FocusNeighborRight = ((j < ((Node)_bundleRow).GetChildCount(false) - 1) ? ((Node)((Node)_bundleRow).GetChild<NCardBundle>(j + 1, false).Hitbox).GetPath() : ((Node)((Node)_bundleRow).GetChild<NCardBundle>(0, false).Hitbox).GetPath());
			((Control)child.Hitbox).FocusNeighborTop = ((Node)child.Hitbox).GetPath();
			((Control)child.Hitbox).FocusNeighborBottom = ((Node)child.Hitbox).GetPath();
		}
		((CanvasItem)_bundlePreviewContainer).Visible = false;
		_bundlePreviewContainer.MouseFilter = (MouseFilterEnum)2;
		_peekButton = ((Node)this).GetNode<NPeekButton>(NodePath.op_Implicit("%PeekButton"));
		_peekButton.AddTargets(_banner, _bundleRow, _bundlePreviewContainer);
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		if (!_completionSource.Task.IsCompleted)
		{
			_completionSource.SetCanceled();
		}
	}

	public static NChooseABundleSelectionScreen ShowScreen(IReadOnlyList<IReadOnlyList<CardModel>> bundles)
	{
		NChooseABundleSelectionScreen nChooseABundleSelectionScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NChooseABundleSelectionScreen>((GenEditState)0);
		((Node)nChooseABundleSelectionScreen).Name = StringName.op_Implicit("NChooseABundleSelectionScreen");
		nChooseABundleSelectionScreen._bundles = bundles;
		NOverlayStack.Instance.Push(nChooseABundleSelectionScreen);
		return nChooseABundleSelectionScreen;
	}

	private void OnBundleClicked(NCardBundle bundleNode)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		_banner.AnimateOut();
		_selectedBundle = bundleNode;
		((CanvasItem)_bundlePreviewContainer).Visible = true;
		_bundlePreviewContainer.MouseFilter = (MouseFilterEnum)0;
		((CanvasItem)_bundleRow).Visible = false;
		_previewCancelButton.Enable();
		_previewConfirmButton.Enable();
		Vector2 val = Vector2.Right * (float)(bundleNode.Bundle.Count - 1) * 400f * 0.5f;
		IReadOnlyList<NCard> readOnlyList = bundleNode.RemoveCardNodes();
		Tween? cardTween = _cardTween;
		if (cardTween != null)
		{
			cardTween.Kill();
		}
		_cardTween = ((Node)this).CreateTween().SetParallel(true);
		for (int i = 0; i < readOnlyList.Count; i++)
		{
			Vector2 globalPosition = ((Control)readOnlyList[i]).GlobalPosition;
			NPreviewCardHolder nPreviewCardHolder = NPreviewCardHolder.Create(readOnlyList[i], showHoverTips: true, scaleOnHover: true);
			((Node)(object)_bundlePreviewCards).AddChildSafely((Node?)(object)nPreviewCardHolder);
			((Control)nPreviewCardHolder).GlobalPosition = globalPosition;
			((GodotObject)nPreviewCardHolder).Connect(NCardHolder.SignalName.Pressed, Callable.From<NCardHolder>((Action<NCardHolder>)OpenPreviewScreen), 0u);
			readOnlyList[i].UpdateVisuals(PileType.None, CardPreviewMode.Normal);
			_cardTween.TweenProperty((GodotObject)(object)nPreviewCardHolder, NodePath.op_Implicit("position"), Variant.op_Implicit(val + Vector2.Left * 400f * (float)i), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		}
		for (int j = 0; j < ((Node)_bundlePreviewCards).GetChildCount(false); j++)
		{
			NPreviewCardHolder child = ((Node)_bundlePreviewCards).GetChild<NPreviewCardHolder>(j, false);
			((Control)child).FocusNeighborLeft = ((j < ((Node)_bundlePreviewCards).GetChildCount(false) - 1) ? ((Node)_bundlePreviewCards).GetChild(j + 1, false).GetPath() : ((Node)_bundlePreviewCards).GetChild(0, false).GetPath());
			((Control)child).FocusNeighborRight = ((j > 0) ? ((Node)_bundlePreviewCards).GetChild(j - 1, false).GetPath() : ((Node)_bundlePreviewCards).GetChild(((Node)_bundlePreviewCards).GetChildCount(false) - 1, false).GetPath());
			((Control)child).FocusNeighborTop = ((Node)child.Hitbox).GetPath();
			((Control)child).FocusNeighborBottom = ((Node)child.Hitbox).GetPath();
		}
		((Node)_bundlePreviewCards).GetChild<Control>(((Node)_bundlePreviewCards).GetChildCount(false) - 1, false).TryGrabFocus();
	}

	private void OpenPreviewScreen(NCardHolder cardHolder)
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

	private void CancelSelection(NButton _)
	{
		_banner.AnimateIn();
		((CanvasItem)_bundlePreviewContainer).Visible = false;
		_bundlePreviewContainer.MouseFilter = (MouseFilterEnum)2;
		Tween? cardTween = _cardTween;
		if (cardTween != null)
		{
			cardTween.Kill();
		}
		_selectedBundle?.ReAddCardNodes();
		((Control)(object)_selectedBundle?.Hitbox).TryGrabFocus();
		_previewCancelButton.Disable();
		_previewConfirmButton.Disable();
		_selectedBundle = null;
		((CanvasItem)_bundleRow).Visible = true;
	}

	private void ConfirmSelection(NButton _)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<NCard> cardNodes = _selectedBundle.CardNodes;
		foreach (NCard item in cardNodes)
		{
			NRun.Instance.GlobalUi.ReparentCard(item);
			Vector2 targetPosition = PileType.Deck.GetTargetPosition(item);
			NCardFlyVfx child = NCardFlyVfx.Create(item, targetPosition, isAddingToPile: true, item.Model.Owner.Character.TrailPath);
			NRun.Instance.GlobalUi.TopBar.TrailContainer.AddChildSafely((Node?)(object)child);
		}
		_completionSource.SetResult(new _003C_003Ez__ReadOnlySingleElementList<IReadOnlyList<CardModel>>(_selectedBundle.Bundle));
	}

	public async Task<IEnumerable<IReadOnlyList<CardModel>>> CardsSelected()
	{
		IEnumerable<IReadOnlyList<CardModel>> result = await _completionSource.Task;
		NOverlayStack.Instance.Remove(this);
		return result;
	}

	public void AfterOverlayOpened()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Modulate = Colors.Transparent;
		Tween? fadeTween = _fadeTween;
		if (fadeTween != null)
		{
			fadeTween.Kill();
		}
		_fadeTween = ((Node)this).CreateTween();
		_fadeTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.4);
	}

	public void AfterOverlayClosed()
	{
		Tween? fadeTween = _fadeTween;
		if (fadeTween != null)
		{
			fadeTween.Kill();
		}
		((Node)(object)this).QueueFreeSafely();
	}

	public void AfterOverlayShown()
	{
		((CanvasItem)this).Visible = true;
		if (((CanvasItem)_bundlePreviewContainer).Visible)
		{
			_previewCancelButton.Enable();
			_previewConfirmButton.Enable();
		}
	}

	public void AfterOverlayHidden()
	{
		((CanvasItem)this).Visible = false;
		_previewCancelButton.Disable();
		_previewConfirmButton.Disable();
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
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnBundleClicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("bundleNode"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenPreviewScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConfirmSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnBundleClicked && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnBundleClicked(VariantUtils.ConvertTo<NCardBundle>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenPreviewScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenPreviewScreen(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.OnBundleClicked)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenPreviewScreen)
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._bundleRow)
		{
			_bundleRow = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bundlePreviewContainer)
		{
			_bundlePreviewContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bundlePreviewCards)
		{
			_bundlePreviewCards = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName._selectedBundle)
		{
			_selectedBundle = VariantUtils.ConvertTo<NCardBundle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			_banner = VariantUtils.ConvertTo<NCommonBanner>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._peekButton)
		{
			_peekButton = VariantUtils.ConvertTo<NPeekButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			_fadeTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			_cardTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._bundleRow)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bundleRow);
			return true;
		}
		if ((ref name) == PropertyName._bundlePreviewContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bundlePreviewContainer);
			return true;
		}
		if ((ref name) == PropertyName._bundlePreviewCards)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bundlePreviewCards);
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
		if ((ref name) == PropertyName._selectedBundle)
		{
			value = VariantUtils.CreateFrom<NCardBundle>(ref _selectedBundle);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			value = VariantUtils.CreateFrom<NCommonBanner>(ref _banner);
			return true;
		}
		if ((ref name) == PropertyName._peekButton)
		{
			value = VariantUtils.CreateFrom<NPeekButton>(ref _peekButton);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _fadeTween);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _cardTween);
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
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._bundleRow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bundlePreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bundlePreviewCards, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._previewCancelButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._previewConfirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectedBundle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._banner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._peekButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fadeTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._bundleRow, Variant.From<Control>(ref _bundleRow));
		info.AddProperty(PropertyName._bundlePreviewContainer, Variant.From<Control>(ref _bundlePreviewContainer));
		info.AddProperty(PropertyName._bundlePreviewCards, Variant.From<Control>(ref _bundlePreviewCards));
		info.AddProperty(PropertyName._previewCancelButton, Variant.From<NBackButton>(ref _previewCancelButton));
		info.AddProperty(PropertyName._previewConfirmButton, Variant.From<NConfirmButton>(ref _previewConfirmButton));
		info.AddProperty(PropertyName._selectedBundle, Variant.From<NCardBundle>(ref _selectedBundle));
		info.AddProperty(PropertyName._banner, Variant.From<NCommonBanner>(ref _banner));
		info.AddProperty(PropertyName._peekButton, Variant.From<NPeekButton>(ref _peekButton));
		info.AddProperty(PropertyName._fadeTween, Variant.From<Tween>(ref _fadeTween));
		info.AddProperty(PropertyName._cardTween, Variant.From<Tween>(ref _cardTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._bundleRow, ref val))
		{
			_bundleRow = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._bundlePreviewContainer, ref val2))
		{
			_bundlePreviewContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._bundlePreviewCards, ref val3))
		{
			_bundlePreviewCards = ((Variant)(ref val3)).As<Control>();
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
		if (info.TryGetProperty(PropertyName._selectedBundle, ref val6))
		{
			_selectedBundle = ((Variant)(ref val6)).As<NCardBundle>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._banner, ref val7))
		{
			_banner = ((Variant)(ref val7)).As<NCommonBanner>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._peekButton, ref val8))
		{
			_peekButton = ((Variant)(ref val8)).As<NPeekButton>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._fadeTween, ref val9))
		{
			_fadeTween = ((Variant)(ref val9)).As<Tween>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardTween, ref val10))
		{
			_cardTween = ((Variant)(ref val10)).As<Tween>();
		}
	}
}
