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
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens;

[ScriptPath("res://src/Core/Nodes/Screens/NDeckViewScreen.cs")]
public class NDeckViewScreen : NCardsViewScreen
{
	public new class MethodName : NCardsViewScreen.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public new static readonly StringName AfterCapstoneClosed = StringName.op_Implicit("AfterCapstoneClosed");

		public static readonly StringName OnPileContentsChanged = StringName.op_Implicit("OnPileContentsChanged");

		public static readonly StringName OnObtainedSort = StringName.op_Implicit("OnObtainedSort");

		public static readonly StringName OnCardTypeSort = StringName.op_Implicit("OnCardTypeSort");

		public static readonly StringName OnCostSort = StringName.op_Implicit("OnCostSort");

		public static readonly StringName OnAlphabetSort = StringName.op_Implicit("OnAlphabetSort");

		public static readonly StringName DisplayCards = StringName.op_Implicit("DisplayCards");
	}

	public new class PropertyName : NCardsViewScreen.PropertyName
	{
		public new static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public static readonly StringName _obtainedSorter = StringName.op_Implicit("_obtainedSorter");

		public static readonly StringName _typeSorter = StringName.op_Implicit("_typeSorter");

		public static readonly StringName _costSorter = StringName.op_Implicit("_costSorter");

		public static readonly StringName _alphabetSorter = StringName.op_Implicit("_alphabetSorter");

		public static readonly StringName _bg = StringName.op_Implicit("_bg");
	}

	public new class SignalName : NCardsViewScreen.SignalName
	{
	}

	private Player _player;

	private CardPile _pile;

	private NCardViewSortButton _obtainedSorter;

	private NCardViewSortButton _typeSorter;

	private NCardViewSortButton _costSorter;

	private NCardViewSortButton _alphabetSorter;

	private Control _bg;

	private readonly List<SortingOrders> _sortingPriority;

	private static string ScenePath => SceneHelper.GetScenePath("screens/deck_view_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public override NetScreenType ScreenType => NetScreenType.DeckView;

	public static NDeckViewScreen? ShowScreen(Player player)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NDeckViewScreen nDeckViewScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NDeckViewScreen>((GenEditState)0);
		nDeckViewScreen._player = player;
		NDebugAudioManager.Instance?.Play("map_open.mp3");
		NCapstoneContainer.Instance.Open(nDeckViewScreen);
		return nDeckViewScreen;
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
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		_cards = _pile.Cards.ToList();
		_infoText = new LocString("gameplay_ui", "DECK_PILE_INFO");
		_bg = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%SortingBg"));
		_obtainedSorter = ((Node)this).GetNode<NCardViewSortButton>(NodePath.op_Implicit("%ObtainedSorter"));
		_typeSorter = ((Node)this).GetNode<NCardViewSortButton>(NodePath.op_Implicit("%CardTypeSorter"));
		_costSorter = ((Node)this).GetNode<NCardViewSortButton>(NodePath.op_Implicit("%CostSorter"));
		_alphabetSorter = ((Node)this).GetNode<NCardViewSortButton>(NodePath.op_Implicit("%AlphabeticalSorter"));
		((GodotObject)_obtainedSorter).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnObtainedSort), 0u);
		((GodotObject)_typeSorter).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnCardTypeSort), 0u);
		((GodotObject)_costSorter).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnCostSort), 0u);
		((GodotObject)_alphabetSorter).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnAlphabetSort), 0u);
		_obtainedSorter.SetLabel(new LocString("gameplay_ui", "SORT_OBTAINED").GetRawText());
		_typeSorter.SetLabel(new LocString("gameplay_ui", "SORT_TYPE").GetRawText());
		_costSorter.SetLabel(new LocString("gameplay_ui", "SORT_COST").GetRawText());
		_alphabetSorter.SetLabel(new LocString("gameplay_ui", "SORT_ALPHABET").GetRawText());
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ViewUpgradesLabel")).SetTextAutoSize(new LocString("gameplay_ui", "VIEW_UPGRADES").GetFormattedText());
		ShaderMaterial val = (ShaderMaterial)_player.Character.CardPool.FrameMaterial;
		((CanvasItem)_bg).Material = (Material)(object)val;
		_obtainedSorter.SetHue(val);
		_typeSorter.SetHue(val);
		_costSorter.SetHue(val);
		_alphabetSorter.SetHue(val);
		ConnectSignals();
		DisplayCards();
		Control[] array = (Control[])(object)new Control[4] { _obtainedSorter, _typeSorter, _costSorter, _alphabetSorter };
		for (int i = 0; i < array.Length; i++)
		{
			array[i].FocusNeighborTop = ((Node)array[i]).GetPath();
			array[i].FocusNeighborBottom = ((_grid.DefaultFocusedControl != null) ? ((Node)_grid.DefaultFocusedControl).GetPath() : ((Node)array[i]).GetPath());
			array[i].FocusNeighborLeft = ((i > 0) ? ((Node)array[i - 1]).GetPath() : ((Node)array[i]).GetPath());
			array[i].FocusNeighborRight = ((i < array.Length - 1) ? ((Node)array[i + 1]).GetPath() : ((Node)array[i]).GetPath());
		}
	}

	public override void _EnterTree()
	{
		((Node)this)._EnterTree();
		_pile = PileType.Deck.GetPile(_player);
		_pile.ContentsChanged += OnPileContentsChanged;
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		_pile.ContentsChanged -= OnPileContentsChanged;
	}

	public override void AfterCapstoneClosed()
	{
		base.AfterCapstoneClosed();
		NRun.Instance?.GlobalUi.TopBar.Deck.ToggleAnimState();
	}

	private void OnPileContentsChanged()
	{
		_cards = _pile.Cards.ToList();
		DisplayCards();
	}

	private void OnObtainedSort(NButton button)
	{
		_sortingPriority.Remove(SortingOrders.Ascending);
		_sortingPriority.Remove(SortingOrders.Descending);
		if (_obtainedSorter.IsDescending)
		{
			_sortingPriority.Insert(0, SortingOrders.Descending);
		}
		else
		{
			_sortingPriority.Insert(0, SortingOrders.Ascending);
		}
		DisplayCards();
	}

	private void OnCardTypeSort(NButton button)
	{
		_sortingPriority.Remove(SortingOrders.TypeAscending);
		_sortingPriority.Remove(SortingOrders.TypeDescending);
		if (_typeSorter.IsDescending)
		{
			_sortingPriority.Insert(0, SortingOrders.TypeDescending);
		}
		else
		{
			_sortingPriority.Insert(0, SortingOrders.TypeAscending);
		}
		DisplayCards();
	}

	private void OnCostSort(NButton button)
	{
		_sortingPriority.Remove(SortingOrders.CostAscending);
		_sortingPriority.Remove(SortingOrders.CostDescending);
		if (_costSorter.IsDescending)
		{
			_sortingPriority.Insert(0, SortingOrders.CostDescending);
		}
		else
		{
			_sortingPriority.Insert(0, SortingOrders.CostAscending);
		}
		DisplayCards();
	}

	private void OnAlphabetSort(NButton button)
	{
		_sortingPriority.Remove(SortingOrders.AlphabetAscending);
		_sortingPriority.Remove(SortingOrders.AlphabetDescending);
		if (_alphabetSorter.IsDescending)
		{
			_sortingPriority.Insert(0, SortingOrders.AlphabetDescending);
		}
		else
		{
			_sortingPriority.Insert(0, SortingOrders.AlphabetAscending);
		}
		DisplayCards();
	}

	private void DisplayCards()
	{
		_grid.YOffset = 100;
		_grid.SetCards(_cards, _pile.Type, _sortingPriority);
		IEnumerable<NGridCardHolder> topRowOfCardNodes = _grid.GetTopRowOfCardNodes();
		if (topRowOfCardNodes == null)
		{
			return;
		}
		foreach (NGridCardHolder item in topRowOfCardNodes)
		{
			((Control)item).FocusNeighborTop = ((Node)_obtainedSorter).GetPath();
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
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Expected O, but got Unknown
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterCapstoneClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPileContentsChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnObtainedSort, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCardTypeSort, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCostSort, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAlphabetSort, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisplayCards, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.AfterCapstoneClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterCapstoneClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPileContentsChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPileContentsChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnObtainedSort && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnObtainedSort(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCardTypeSort && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCardTypeSort(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCostSort && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCostSort(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAlphabetSort && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnAlphabetSort(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisplayCards && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisplayCards();
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterCapstoneClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPileContentsChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnObtainedSort)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCardTypeSort)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCostSort)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAlphabetSort)
		{
			return true;
		}
		if ((ref method) == MethodName.DisplayCards)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._obtainedSorter)
		{
			_obtainedSorter = VariantUtils.ConvertTo<NCardViewSortButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._typeSorter)
		{
			_typeSorter = VariantUtils.ConvertTo<NCardViewSortButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._costSorter)
		{
			_costSorter = VariantUtils.ConvertTo<NCardViewSortButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._alphabetSorter)
		{
			_alphabetSorter = VariantUtils.ConvertTo<NCardViewSortButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bg)
		{
			_bg = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ScreenType)
		{
			NetScreenType screenType = ScreenType;
			value = VariantUtils.CreateFrom<NetScreenType>(ref screenType);
			return true;
		}
		if ((ref name) == PropertyName._obtainedSorter)
		{
			value = VariantUtils.CreateFrom<NCardViewSortButton>(ref _obtainedSorter);
			return true;
		}
		if ((ref name) == PropertyName._typeSorter)
		{
			value = VariantUtils.CreateFrom<NCardViewSortButton>(ref _typeSorter);
			return true;
		}
		if ((ref name) == PropertyName._costSorter)
		{
			value = VariantUtils.CreateFrom<NCardViewSortButton>(ref _costSorter);
			return true;
		}
		if ((ref name) == PropertyName._alphabetSorter)
		{
			value = VariantUtils.CreateFrom<NCardViewSortButton>(ref _alphabetSorter);
			return true;
		}
		if ((ref name) == PropertyName._bg)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bg);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName.ScreenType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._obtainedSorter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._typeSorter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._costSorter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._alphabetSorter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bg, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._obtainedSorter, Variant.From<NCardViewSortButton>(ref _obtainedSorter));
		info.AddProperty(PropertyName._typeSorter, Variant.From<NCardViewSortButton>(ref _typeSorter));
		info.AddProperty(PropertyName._costSorter, Variant.From<NCardViewSortButton>(ref _costSorter));
		info.AddProperty(PropertyName._alphabetSorter, Variant.From<NCardViewSortButton>(ref _alphabetSorter));
		info.AddProperty(PropertyName._bg, Variant.From<Control>(ref _bg));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._obtainedSorter, ref val))
		{
			_obtainedSorter = ((Variant)(ref val)).As<NCardViewSortButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._typeSorter, ref val2))
		{
			_typeSorter = ((Variant)(ref val2)).As<NCardViewSortButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._costSorter, ref val3))
		{
			_costSorter = ((Variant)(ref val3)).As<NCardViewSortButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._alphabetSorter, ref val4))
		{
			_alphabetSorter = ((Variant)(ref val4)).As<NCardViewSortButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._bg, ref val5))
		{
			_bg = ((Variant)(ref val5)).As<Control>();
		}
	}

	public NDeckViewScreen()
	{
		int num = 4;
		List<SortingOrders> list = new List<SortingOrders>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<SortingOrders> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = SortingOrders.Ascending;
		num2++;
		span[num2] = SortingOrders.TypeAscending;
		num2++;
		span[num2] = SortingOrders.CostAscending;
		span[num2 + 1] = SortingOrders.AlphabetAscending;
		_sortingPriority = list;
		base._002Ector();
	}
}
