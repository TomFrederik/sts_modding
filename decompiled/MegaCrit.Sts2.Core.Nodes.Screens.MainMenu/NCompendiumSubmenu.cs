using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Bestiary;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.Nodes.Screens.PotionLab;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;
using MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.StatsScreen;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NCompendiumSubmenu.cs")]
public class NCompendiumSubmenu : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public static readonly StringName OpenCardLibrary = StringName.op_Implicit("OpenCardLibrary");

		public static readonly StringName OpenRelicCollection = StringName.op_Implicit("OpenRelicCollection");

		public static readonly StringName OpenPotionLab = StringName.op_Implicit("OpenPotionLab");

		public static readonly StringName OpenBestiary = StringName.op_Implicit("OpenBestiary");

		public static readonly StringName OpenLeaderboards = StringName.op_Implicit("OpenLeaderboards");

		public static readonly StringName OpenStatistics = StringName.op_Implicit("OpenStatistics");

		public static readonly StringName OpenRunHistory = StringName.op_Implicit("OpenRunHistory");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _confirmButton = StringName.op_Implicit("_confirmButton");

		public static readonly StringName _cardLibraryButton = StringName.op_Implicit("_cardLibraryButton");

		public static readonly StringName _relicCollectionButton = StringName.op_Implicit("_relicCollectionButton");

		public static readonly StringName _potionLabButton = StringName.op_Implicit("_potionLabButton");

		public static readonly StringName _bestiaryButton = StringName.op_Implicit("_bestiaryButton");

		public static readonly StringName _leaderboardsButton = StringName.op_Implicit("_leaderboardsButton");

		public static readonly StringName _statisticsButton = StringName.op_Implicit("_statisticsButton");

		public static readonly StringName _runHistoryButton = StringName.op_Implicit("_runHistoryButton");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/compendium_submenu");

	private NButton _confirmButton;

	private NShortSubmenuButton _cardLibraryButton;

	private NShortSubmenuButton _relicCollectionButton;

	private NShortSubmenuButton _potionLabButton;

	private NShortSubmenuButton _bestiaryButton;

	private NCompendiumBottomButton _leaderboardsButton;

	private NCompendiumBottomButton _statisticsButton;

	private NCompendiumBottomButton _runHistoryButton;

	private IRunState _runState;

	protected override Control InitialFocusedControl => (Control)(object)_cardLibraryButton;

	public static NCompendiumSubmenu? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NCompendiumSubmenu>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_cardLibraryButton = ((Node)this).GetNode<NShortSubmenuButton>(NodePath.op_Implicit("%CardLibraryButton"));
		((GodotObject)_cardLibraryButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenCardLibrary), 0u);
		_cardLibraryButton.SetIconAndLocalization("COMPENDIUM_CARD_LIBRARY");
		_relicCollectionButton = ((Node)this).GetNode<NShortSubmenuButton>(NodePath.op_Implicit("%RelicCollectionButton"));
		((GodotObject)_relicCollectionButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenRelicCollection), 0u);
		_relicCollectionButton.SetIconAndLocalization("COMPENDIUM_RELIC_COLLECTION");
		_potionLabButton = ((Node)this).GetNode<NShortSubmenuButton>(NodePath.op_Implicit("%PotionLabButton"));
		((GodotObject)_potionLabButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenPotionLab), 0u);
		_potionLabButton.SetIconAndLocalization("COMPENDIUM_POTION_LAB");
		_bestiaryButton = ((Node)this).GetNode<NShortSubmenuButton>(NodePath.op_Implicit("%BestiaryButton"));
		_bestiaryButton.Disable();
		_bestiaryButton.SetIconAndLocalization("COMPENDIUM_BESTIARY");
		_leaderboardsButton = ((Node)this).GetNode<NCompendiumBottomButton>(NodePath.op_Implicit("%LeaderboardsButton"));
		((GodotObject)_leaderboardsButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenLeaderboards), 0u);
		_leaderboardsButton.SetLocalization("LEADERBOARDS");
		_statisticsButton = ((Node)this).GetNode<NCompendiumBottomButton>(NodePath.op_Implicit("%StatisticsButton"));
		((GodotObject)_statisticsButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenStatistics), 0u);
		_statisticsButton.SetLocalization("STATISTICS");
		_runHistoryButton = ((Node)this).GetNode<NCompendiumBottomButton>(NodePath.op_Implicit("%RunHistoryButton"));
		((GodotObject)_runHistoryButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenRunHistory), 0u);
		_runHistoryButton.SetLocalization("RUN_HISTORY");
		int num = 4;
		List<Control> list = new List<Control>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<Control> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = (Control)(object)_cardLibraryButton;
		num2++;
		span[num2] = (Control)(object)_relicCollectionButton;
		num2++;
		span[num2] = (Control)(object)_potionLabButton;
		num2++;
		span[num2] = (Control)(object)_bestiaryButton;
		List<Control> list2 = list;
		num2 = 3;
		List<Control> list3 = new List<Control>(num2);
		CollectionsMarshal.SetCount(list3, num2);
		span = CollectionsMarshal.AsSpan(list3);
		num = 0;
		span[num] = (Control)(object)_leaderboardsButton;
		num++;
		span[num] = (Control)(object)_statisticsButton;
		num++;
		span[num] = (Control)(object)_runHistoryButton;
		List<Control> list4 = list3;
		for (int i = 0; i < list2.Count; i++)
		{
			list2[i].FocusNeighborTop = ((Node)list2[i]).GetPath();
			list2[i].FocusNeighborLeft = ((i > 0) ? ((Node)list2[i - 1]).GetPath() : ((Node)list2[i]).GetPath());
			list2[i].FocusNeighborRight = ((i < list2.Count - 1) ? ((Node)list2[i + 1]).GetPath() : ((Node)list2[i]).GetPath());
		}
		for (int j = 0; j < list4.Count; j++)
		{
			list4[j].FocusNeighborBottom = ((Node)list4[j]).GetPath();
			list4[j].FocusNeighborLeft = ((j > 0) ? ((Node)list4[j - 1]).GetPath() : ((Node)list4[j]).GetPath());
			list4[j].FocusNeighborRight = ((j < list4.Count - 1) ? ((Node)list4[j + 1]).GetPath() : ((Node)list4[j]).GetPath());
		}
		list2[0].FocusNeighborBottom = ((Node)list4[0]).GetPath();
		list2[1].FocusNeighborBottom = ((Node)list4[0]).GetPath();
		list2[2].FocusNeighborBottom = ((Node)list4[1]).GetPath();
		list2[3].FocusNeighborBottom = ((Node)list4[2]).GetPath();
		list4[0].FocusNeighborTop = ((Node)list2[1]).GetPath();
		list4[1].FocusNeighborTop = ((Node)list2[2]).GetPath();
		list4[2].FocusNeighborTop = ((Node)list2[3]).GetPath();
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		((CanvasItem)_leaderboardsButton).Visible = false;
		((CanvasItem)_runHistoryButton).Visible = NRunHistory.CanBeShown();
	}

	public void Initialize(IRunState runState)
	{
		_runState = runState;
	}

	private void OpenCardLibrary(NButton _)
	{
		NCardLibrary submenuType = _stack.GetSubmenuType<NCardLibrary>();
		submenuType.Initialize(_runState);
		_stack.Push(submenuType);
	}

	private void OpenRelicCollection(NButton _)
	{
		_stack.PushSubmenuType<NRelicCollection>();
	}

	private void OpenPotionLab(NButton _)
	{
		_stack.PushSubmenuType<NPotionLab>();
	}

	private void OpenBestiary(NButton _)
	{
		_stack.PushSubmenuType<NBestiary>();
	}

	private void OpenLeaderboards(NButton _)
	{
	}

	private void OpenStatistics(NButton _)
	{
		_stack.PushSubmenuType<NStatsScreen>();
	}

	private void OpenRunHistory(NButton _)
	{
		_stack.PushSubmenuType<NRunHistory>();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
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
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Expected O, but got Unknown
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Expected O, but got Unknown
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Expected O, but got Unknown
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Expected O, but got Unknown
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenCardLibrary, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenRelicCollection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenPotionLab, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenBestiary, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenLeaderboards, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenStatistics, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenRunHistory, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCompendiumSubmenu nCompendiumSubmenu = Create();
			ret = VariantUtils.CreateFrom<NCompendiumSubmenu>(ref nCompendiumSubmenu);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenCardLibrary && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenCardLibrary(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenRelicCollection && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenRelicCollection(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenPotionLab && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenPotionLab(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenBestiary && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenBestiary(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenLeaderboards && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenLeaderboards(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenStatistics && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenStatistics(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenRunHistory && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenRunHistory(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCompendiumSubmenu nCompendiumSubmenu = Create();
			ret = VariantUtils.CreateFrom<NCompendiumSubmenu>(ref nCompendiumSubmenu);
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
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenCardLibrary)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenRelicCollection)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenPotionLab)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenBestiary)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenLeaderboards)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenStatistics)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenRunHistory)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._confirmButton)
		{
			_confirmButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardLibraryButton)
		{
			_cardLibraryButton = VariantUtils.ConvertTo<NShortSubmenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicCollectionButton)
		{
			_relicCollectionButton = VariantUtils.ConvertTo<NShortSubmenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionLabButton)
		{
			_potionLabButton = VariantUtils.ConvertTo<NShortSubmenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bestiaryButton)
		{
			_bestiaryButton = VariantUtils.ConvertTo<NShortSubmenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leaderboardsButton)
		{
			_leaderboardsButton = VariantUtils.ConvertTo<NCompendiumBottomButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._statisticsButton)
		{
			_statisticsButton = VariantUtils.ConvertTo<NCompendiumBottomButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._runHistoryButton)
		{
			_runHistoryButton = VariantUtils.ConvertTo<NCompendiumBottomButton>(ref value);
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
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._confirmButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _confirmButton);
			return true;
		}
		if ((ref name) == PropertyName._cardLibraryButton)
		{
			value = VariantUtils.CreateFrom<NShortSubmenuButton>(ref _cardLibraryButton);
			return true;
		}
		if ((ref name) == PropertyName._relicCollectionButton)
		{
			value = VariantUtils.CreateFrom<NShortSubmenuButton>(ref _relicCollectionButton);
			return true;
		}
		if ((ref name) == PropertyName._potionLabButton)
		{
			value = VariantUtils.CreateFrom<NShortSubmenuButton>(ref _potionLabButton);
			return true;
		}
		if ((ref name) == PropertyName._bestiaryButton)
		{
			value = VariantUtils.CreateFrom<NShortSubmenuButton>(ref _bestiaryButton);
			return true;
		}
		if ((ref name) == PropertyName._leaderboardsButton)
		{
			value = VariantUtils.CreateFrom<NCompendiumBottomButton>(ref _leaderboardsButton);
			return true;
		}
		if ((ref name) == PropertyName._statisticsButton)
		{
			value = VariantUtils.CreateFrom<NCompendiumBottomButton>(ref _statisticsButton);
			return true;
		}
		if ((ref name) == PropertyName._runHistoryButton)
		{
			value = VariantUtils.CreateFrom<NCompendiumBottomButton>(ref _runHistoryButton);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._confirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardLibraryButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicCollectionButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionLabButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bestiaryButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leaderboardsButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._statisticsButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._runHistoryButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._confirmButton, Variant.From<NButton>(ref _confirmButton));
		info.AddProperty(PropertyName._cardLibraryButton, Variant.From<NShortSubmenuButton>(ref _cardLibraryButton));
		info.AddProperty(PropertyName._relicCollectionButton, Variant.From<NShortSubmenuButton>(ref _relicCollectionButton));
		info.AddProperty(PropertyName._potionLabButton, Variant.From<NShortSubmenuButton>(ref _potionLabButton));
		info.AddProperty(PropertyName._bestiaryButton, Variant.From<NShortSubmenuButton>(ref _bestiaryButton));
		info.AddProperty(PropertyName._leaderboardsButton, Variant.From<NCompendiumBottomButton>(ref _leaderboardsButton));
		info.AddProperty(PropertyName._statisticsButton, Variant.From<NCompendiumBottomButton>(ref _statisticsButton));
		info.AddProperty(PropertyName._runHistoryButton, Variant.From<NCompendiumBottomButton>(ref _runHistoryButton));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._confirmButton, ref val))
		{
			_confirmButton = ((Variant)(ref val)).As<NButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardLibraryButton, ref val2))
		{
			_cardLibraryButton = ((Variant)(ref val2)).As<NShortSubmenuButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicCollectionButton, ref val3))
		{
			_relicCollectionButton = ((Variant)(ref val3)).As<NShortSubmenuButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionLabButton, ref val4))
		{
			_potionLabButton = ((Variant)(ref val4)).As<NShortSubmenuButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._bestiaryButton, ref val5))
		{
			_bestiaryButton = ((Variant)(ref val5)).As<NShortSubmenuButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._leaderboardsButton, ref val6))
		{
			_leaderboardsButton = ((Variant)(ref val6)).As<NCompendiumBottomButton>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._statisticsButton, ref val7))
		{
			_statisticsButton = ((Variant)(ref val7)).As<NCompendiumBottomButton>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._runHistoryButton, ref val8))
		{
			_runHistoryButton = ((Variant)(ref val8)).As<NCompendiumBottomButton>();
		}
	}
}
