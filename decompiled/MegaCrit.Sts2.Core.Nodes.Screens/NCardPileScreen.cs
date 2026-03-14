using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens;

[ScriptPath("res://src/Core/Nodes/Screens/NCardPileScreen.cs")]
public class NCardPileScreen : Control, ICapstoneScreen, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnPileContentsChanged = StringName.op_Implicit("OnPileContentsChanged");

		public static readonly StringName OnReturnButtonPressed = StringName.op_Implicit("OnReturnButtonPressed");

		public static readonly StringName AfterCapstoneOpened = StringName.op_Implicit("AfterCapstoneOpened");

		public static readonly StringName AfterCapstoneClosed = StringName.op_Implicit("AfterCapstoneClosed");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public static readonly StringName UseSharedBackstop = StringName.op_Implicit("UseSharedBackstop");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _background = StringName.op_Implicit("_background");

		public static readonly StringName _grid = StringName.op_Implicit("_grid");

		public static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _bottomLabel = StringName.op_Implicit("_bottomLabel");

		public static readonly StringName _currentTween = StringName.op_Implicit("_currentTween");

		public static readonly StringName _closeHotkeys = StringName.op_Implicit("_closeHotkeys");
	}

	public class SignalName : SignalName
	{
	}

	private ColorRect _background;

	private NCardGrid _grid;

	private NButton _backButton;

	private MegaRichTextLabel _bottomLabel;

	private Tween? _currentTween;

	private string[] _closeHotkeys = Array.Empty<string>();

	private static string ScenePath => SceneHelper.GetScenePath("/screens/card_pile_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public NetScreenType ScreenType => NetScreenType.CardPile;

	public CardPile Pile { get; private set; }

	public bool UseSharedBackstop => true;

	public Control? DefaultFocusedControl => _grid.DefaultFocusedControl;

	public override void _Ready()
	{
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		_bottomLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%BottomLabel"));
		switch (Pile.Type)
		{
		case PileType.Draw:
			_bottomLabel.Text = "[center]" + new LocString("gameplay_ui", "DRAW_PILE_INFO").GetFormattedText();
			break;
		case PileType.Discard:
			_bottomLabel.Text = "[center]" + new LocString("gameplay_ui", "DISCARD_PILE_INFO").GetFormattedText();
			break;
		case PileType.Exhaust:
			_bottomLabel.Text = "[center]" + new LocString("gameplay_ui", "EXHAUST_PILE_INFO").GetFormattedText();
			break;
		default:
			((CanvasItem)_bottomLabel).Visible = false;
			Log.Info("CardPileScreen has no info text.");
			break;
		}
		_backButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("BackButton"));
		((GodotObject)_backButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnReturnButtonPressed), 0u);
		_backButton.Enable();
		_grid = ((Node)this).GetNode<NCardGrid>(NodePath.op_Implicit("CardGrid"));
		OnPileContentsChanged();
		_grid.InsetForTopBar();
		_background = ((Node)this).GetNode<ColorRect>(NodePath.op_Implicit("Background"));
		((CanvasItem)_background).Modulate = StsColors.transparentBlack;
		_currentTween = ((Node)this).CreateTween();
		_currentTween.TweenProperty((GodotObject)(object)_background, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.screenBackdrop), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		((Node)this).ProcessMode = (ProcessModeEnum)(((CanvasItem)this).Visible ? 0 : 4);
	}

	public override void _EnterTree()
	{
		((Node)this)._EnterTree();
		Pile.ContentsChanged += OnPileContentsChanged;
		string[] closeHotkeys = _closeHotkeys;
		foreach (string hotkey in closeHotkeys)
		{
			NHotkeyManager.Instance.PushHotkeyReleasedBinding(hotkey, NCapstoneContainer.Instance.Close);
		}
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		Pile.ContentsChanged -= OnPileContentsChanged;
		string[] closeHotkeys = _closeHotkeys;
		foreach (string hotkey in closeHotkeys)
		{
			NHotkeyManager.Instance.RemoveHotkeyReleasedBinding(hotkey, NCapstoneContainer.Instance.Close);
		}
	}

	public static NCardPileScreen ShowScreen(CardPile pile, string[] closeHotkeys)
	{
		NDebugAudioManager.Instance?.Play("map_open.mp3");
		NCardPileScreen nCardPileScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCardPileScreen>((GenEditState)0);
		((Node)nCardPileScreen).Name = StringName.op_Implicit($"{"NCardPileScreen"}-{pile.Type}");
		nCardPileScreen.Pile = pile;
		nCardPileScreen._closeHotkeys = closeHotkeys;
		NCapstoneContainer.Instance.Open(nCardPileScreen);
		return nCardPileScreen;
	}

	private void OnPileContentsChanged()
	{
		List<CardModel> list = Pile.Cards.ToList();
		if (Pile.Type == PileType.Draw)
		{
			list.Sort((CardModel c1, CardModel c2) => (c1.Rarity != c2.Rarity) ? c1.Rarity.CompareTo(c2.Rarity) : string.Compare(c1.Id.Entry, c2.Id.Entry, StringComparison.Ordinal));
		}
		NCardGrid grid = _grid;
		PileType type = Pile.Type;
		int num = 1;
		List<SortingOrders> list2 = new List<SortingOrders>(num);
		CollectionsMarshal.SetCount(list2, num);
		Span<SortingOrders> span = CollectionsMarshal.AsSpan(list2);
		int index = 0;
		span[index] = SortingOrders.Ascending;
		grid.SetCards(list, type, list2);
	}

	private void OnReturnButtonPressed(NButton _)
	{
		NCapstoneContainer.Instance.Close();
	}

	public void AfterCapstoneOpened()
	{
		((CanvasItem)this).Visible = true;
	}

	public void AfterCapstoneClosed()
	{
		((CanvasItem)this).Visible = false;
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPileContentsChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnReturnButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterCapstoneOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterCapstoneClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnPileContentsChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPileContentsChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnReturnButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnReturnButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterCapstoneOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterCapstoneOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterCapstoneClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterCapstoneClosed();
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
		if ((ref method) == MethodName.OnPileContentsChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnReturnButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterCapstoneOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterCapstoneClosed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._background)
		{
			_background = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._grid)
		{
			_grid = VariantUtils.ConvertTo<NCardGrid>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bottomLabel)
		{
			_bottomLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentTween)
		{
			_currentTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._closeHotkeys)
		{
			_closeHotkeys = VariantUtils.ConvertTo<string[]>(ref value);
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
		if ((ref name) == PropertyName._background)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _background);
			return true;
		}
		if ((ref name) == PropertyName._grid)
		{
			value = VariantUtils.CreateFrom<NCardGrid>(ref _grid);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _backButton);
			return true;
		}
		if ((ref name) == PropertyName._bottomLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _bottomLabel);
			return true;
		}
		if ((ref name) == PropertyName._currentTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _currentTween);
			return true;
		}
		if ((ref name) == PropertyName._closeHotkeys)
		{
			value = VariantUtils.CreateFrom<string[]>(ref _closeHotkeys);
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
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._background, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._grid, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bottomLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)34, PropertyName._closeHotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._background, Variant.From<ColorRect>(ref _background));
		info.AddProperty(PropertyName._grid, Variant.From<NCardGrid>(ref _grid));
		info.AddProperty(PropertyName._backButton, Variant.From<NButton>(ref _backButton));
		info.AddProperty(PropertyName._bottomLabel, Variant.From<MegaRichTextLabel>(ref _bottomLabel));
		info.AddProperty(PropertyName._currentTween, Variant.From<Tween>(ref _currentTween));
		info.AddProperty(PropertyName._closeHotkeys, Variant.From<string[]>(ref _closeHotkeys));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._background, ref val))
		{
			_background = ((Variant)(ref val)).As<ColorRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._grid, ref val2))
		{
			_grid = ((Variant)(ref val2)).As<NCardGrid>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val3))
		{
			_backButton = ((Variant)(ref val3)).As<NButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._bottomLabel, ref val4))
		{
			_bottomLabel = ((Variant)(ref val4)).As<MegaRichTextLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentTween, ref val5))
		{
			_currentTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._closeHotkeys, ref val6))
		{
			_closeHotkeys = ((Variant)(ref val6)).As<string[]>();
		}
	}
}
