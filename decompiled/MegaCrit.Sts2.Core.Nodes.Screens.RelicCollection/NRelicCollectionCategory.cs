using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;

[ScriptPath("res://src/Core/Nodes/Screens/RelicCollection/NRelicCollectionCategory.cs")]
public class NRelicCollectionCategory : VBoxContainer
{
	public class MethodName : MethodName
	{
		public static readonly StringName CreateForSubcategory = StringName.op_Implicit("CreateForSubcategory");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName LoadIcon = StringName.op_Implicit("LoadIcon");

		public static readonly StringName ClearRelics = StringName.op_Implicit("ClearRelics");

		public static readonly StringName OnRelicEntryPressed = StringName.op_Implicit("OnRelicEntryPressed");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _collection = StringName.op_Implicit("_collection");

		public static readonly StringName _headerLabel = StringName.op_Implicit("_headerLabel");

		public static readonly StringName _relicsContainer = StringName.op_Implicit("_relicsContainer");

		public static readonly StringName _spacer = StringName.op_Implicit("_spacer");

		public static readonly StringName _icon = StringName.op_Implicit("_icon");
	}

	public class SignalName : SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("screens/relic_collection/relic_collection_subcategory");

	private static readonly List<RelicModel> _relicModelCache = new List<RelicModel>();

	private NRelicCollection _collection;

	private MegaRichTextLabel _headerLabel;

	private GridContainer _relicsContainer;

	private readonly List<NRelicCollectionCategory> _subCategories = new List<NRelicCollectionCategory>();

	private Control _spacer;

	private TextureRect _icon;

	public Control? DefaultFocusedControl
	{
		get
		{
			if (_subCategories.Any())
			{
				return _subCategories.First().DefaultFocusedControl;
			}
			if (GodotObject.IsInstanceValid((GodotObject)(object)_relicsContainer) && ((Node)_relicsContainer).GetChildCount(false) > 0)
			{
				return ((Node)_relicsContainer).GetChild<Control>(0, false);
			}
			return null;
		}
	}

	private NRelicCollectionCategory CreateForSubcategory()
	{
		return PreloadManager.Cache.GetScene(scenePath).Instantiate<NRelicCollectionCategory>((GenEditState)0);
	}

	public override void _Ready()
	{
		_headerLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Header"));
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		_relicsContainer = ((Node)this).GetNode<GridContainer>(NodePath.op_Implicit("%RelicsContainer"));
		_spacer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Spacer"));
		((CanvasItem)_icon).Visible = false;
	}

	public void LoadRelics(RelicRarity relicRarity, NRelicCollection collection, LocString header, HashSet<RelicModel> seenRelics, UnlockState unlockState, HashSet<RelicModel> allUnlockedRelics)
	{
		_subCategories.Clear();
		_headerLabel.Text = header.GetFormattedText();
		_collection = collection;
		_relicModelCache.Clear();
		_relicModelCache.AddRange(ModelDb.AllRelics.Where((RelicModel relic) => relic.Rarity == relicRarity));
		if (relicRarity == RelicRarity.Starter)
		{
			List<RelicModel> list = ModelDb.AllCharacters.SelectMany((CharacterModel c) => c.StartingRelics).ToList();
			NRelicCollectionCategory nRelicCollectionCategory = CreateForSubcategory();
			_subCategories.Add(nRelicCollectionCategory);
			((Node)(object)this).AddChildSafely((Node?)(object)nRelicCollectionCategory);
			((Node)this).MoveChild((Node)(object)nRelicCollectionCategory, ((Node)_headerLabel).GetIndex(false) + 1);
			((CanvasItem)nRelicCollectionCategory._spacer).Visible = false;
			((CanvasItem)nRelicCollectionCategory._headerLabel).Visible = false;
			nRelicCollectionCategory.LoadSubcategory(_collection, null, list, seenRelics, allUnlockedRelics);
			IEnumerable<RelicModel> relics = list.Select((RelicModel r) => ModelDb.Relic<TouchOfOrobas>().GetUpgradedStarterRelic(r));
			NRelicCollectionCategory nRelicCollectionCategory2 = CreateForSubcategory();
			_subCategories.Add(nRelicCollectionCategory2);
			((Node)(object)this).AddChildSafely((Node?)(object)nRelicCollectionCategory2);
			((Node)this).MoveChild((Node)(object)nRelicCollectionCategory2, ((Node)_headerLabel).GetIndex(false) + 2);
			((CanvasItem)nRelicCollectionCategory2._headerLabel).Visible = false;
			nRelicCollectionCategory2.LoadSubcategory(_collection, null, relics, seenRelics, allUnlockedRelics);
			return;
		}
		if (relicRarity == RelicRarity.Ancient)
		{
			int num = 4;
			List<ActModel> list2 = new List<ActModel>(num);
			CollectionsMarshal.SetCount(list2, num);
			Span<ActModel> span = CollectionsMarshal.AsSpan(list2);
			int num2 = 0;
			span[num2] = ModelDb.Act<Overgrowth>();
			num2++;
			span[num2] = ModelDb.Act<Underdocks>();
			num2++;
			span[num2] = ModelDb.Act<Hive>();
			num2++;
			span[num2] = ModelDb.Act<Glory>();
			List<ActModel> list3 = list2;
			if (ModelDb.Acts.Except(list3).Any())
			{
				throw new InvalidOperationException("The act list in NRelicCollectionCategory is out of date!");
			}
			List<AncientEventModel> list4 = list3.Select((ActModel a) => a.AllAncients).SelectMany((IEnumerable<AncientEventModel> a) => a).Concat(ModelDb.AllSharedAncients)
				.Distinct()
				.ToList();
			HashSet<AncientEventModel> hashSet = list3.Select((ActModel a) => a.GetUnlockedAncients(unlockState)).SelectMany((IEnumerable<AncientEventModel> a) => a).Concat(unlockState.SharedAncients)
				.Distinct()
				.ToHashSet();
			IReadOnlyDictionary<ModelId, AncientStats> ancientStats = SaveManager.Instance.Progress.AncientStats;
			LocString locString = new LocString("relic_collection", "UNKNOWN_ANCIENT");
			for (int num3 = 0; num3 < list4.Count; num3++)
			{
				AncientEventModel ancientEventModel = list4[num3];
				if (hashSet.Contains(ancientEventModel))
				{
					NRelicCollectionCategory nRelicCollectionCategory3 = CreateForSubcategory();
					_subCategories.Add(nRelicCollectionCategory3);
					((Node)(object)this).AddChildSafely((Node?)(object)nRelicCollectionCategory3);
					((Node)this).MoveChild((Node)(object)nRelicCollectionCategory3, ((Node)_headerLabel).GetIndex(false) + num3 + 1);
					StringComparer comparer = StringComparer.Create(LocManager.Instance.CultureInfo, CompareOptions.None);
					RelicModel[] array = ancientEventModel.AllPossibleOptions.Select((EventOption o) => o.Relic?.CanonicalInstance).OfType<RelicModel>().Intersect(_relicModelCache)
						.OrderBy<RelicModel, string>((RelicModel r) => r.Title.GetFormattedText(), comparer)
						.ToArray();
					bool flag = ancientStats.ContainsKey(ancientEventModel.Id) || array.Any((RelicModel r) => seenRelics.Contains(r));
					LocString locString2 = new LocString("relic_collection", "ANCIENT_SUBCATEGORY");
					locString2.Add("Ancient", flag ? ancientEventModel.Title : locString);
					nRelicCollectionCategory3.LoadSubcategory(_collection, locString2, array, seenRelics, allUnlockedRelics);
					nRelicCollectionCategory3.LoadIcon(ancientEventModel.RunHistoryIcon);
				}
			}
			return;
		}
		List<RelicModel> list5 = new List<RelicModel>();
		List<RelicModel> list6 = new List<RelicModel>();
		foreach (RelicPoolModel allCharacterRelicPool in ModelDb.AllCharacterRelicPools)
		{
			foreach (RelicModel item in _relicModelCache)
			{
				if (allCharacterRelicPool.AllRelicIds.Contains(item.Id))
				{
					list5.Add(item);
				}
			}
		}
		foreach (RelicModel item2 in _relicModelCache)
		{
			if (!list5.Contains(item2))
			{
				list6.Add(item2);
			}
		}
		StringComparer comparer2 = StringComparer.Create(LocManager.Instance.CultureInfo, CompareOptions.None);
		list6.Sort((RelicModel p1, RelicModel p2) => comparer2.Compare(p1.Title.GetFormattedText(), p2.Title.GetFormattedText()));
		LoadRelicNodes(list6.Concat(list5), seenRelics, allUnlockedRelics);
		_collection.AddRelics(list6);
		_collection.AddRelics(list5);
	}

	private void LoadSubcategory(NRelicCollection collection, LocString? header, IEnumerable<RelicModel> relics, HashSet<RelicModel> seenRelics, HashSet<RelicModel> unlockedRelics)
	{
		_headerLabel.Text = header?.GetFormattedText() ?? "";
		_collection = collection;
		_collection.AddRelics(relics);
		LoadRelicNodes(relics, seenRelics, unlockedRelics);
	}

	private void LoadIcon(Texture2D tex)
	{
		_icon.Texture = tex;
		((CanvasItem)_icon).Visible = true;
	}

	private void LoadRelicNodes(IEnumerable<RelicModel> relics, HashSet<RelicModel> seenRelics, HashSet<RelicModel> unlockedRelics)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		foreach (Node child in ((Node)_relicsContainer).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
		foreach (RelicModel relic in relics)
		{
			ModelVisibility visibility = ((!unlockedRelics.Contains(relic)) ? ModelVisibility.Locked : (seenRelics.Contains(relic) ? ModelVisibility.Visible : ModelVisibility.NotSeen));
			NRelicCollectionEntry nRelicCollectionEntry = NRelicCollectionEntry.Create(relic, visibility);
			((Node)(object)_relicsContainer).AddChildSafely((Node?)(object)nRelicCollectionEntry);
			((GodotObject)nRelicCollectionEntry).Connect(NClickableControl.SignalName.Released, Callable.From<NRelicCollectionEntry>((Action<NRelicCollectionEntry>)OnRelicEntryPressed), 0u);
		}
	}

	public void ClearRelics()
	{
		foreach (Node child in ((Node)_relicsContainer).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
		foreach (NRelicCollectionCategory item in ((IEnumerable)((Node)this).GetChildren(false)).OfType<NRelicCollectionCategory>())
		{
			((Node)(object)item).QueueFreeSafely();
		}
	}

	private void OnRelicEntryPressed(NRelicCollectionEntry entry)
	{
		NGame.Instance.GetInspectRelicScreen().Open(_collection.Relics, entry.relic);
		_collection.SetLastFocusedRelic(entry);
	}

	public List<IReadOnlyList<Control>> GetGridItems()
	{
		List<IReadOnlyList<Control>> list = new List<IReadOnlyList<Control>>();
		if (_subCategories.Any())
		{
			foreach (NRelicCollectionCategory subCategory in _subCategories)
			{
				list.AddRange(subCategory.GetGridItems());
			}
		}
		else
		{
			for (int i = 0; i < ((Node)_relicsContainer).GetChildren(false).Count; i += _relicsContainer.Columns)
			{
				list.Add(((IEnumerable)((Node)_relicsContainer).GetChildren(false)).OfType<Control>().Skip(i).Take(_relicsContainer.Columns)
					.ToList());
			}
		}
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.CreateForSubcategory, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("VBoxContainer"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LoadIcon, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tex"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearRelics, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelicEntryPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("entry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.CreateForSubcategory && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NRelicCollectionCategory nRelicCollectionCategory = CreateForSubcategory();
			ret = VariantUtils.CreateFrom<NRelicCollectionCategory>(ref nRelicCollectionCategory);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LoadIcon && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			LoadIcon(VariantUtils.ConvertTo<Texture2D>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearRelics && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearRelics();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelicEntryPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnRelicEntryPressed(VariantUtils.ConvertTo<NRelicCollectionEntry>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((VBoxContainer)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.CreateForSubcategory)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.LoadIcon)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearRelics)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelicEntryPressed)
		{
			return true;
		}
		return ((VBoxContainer)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._collection)
		{
			_collection = VariantUtils.ConvertTo<NRelicCollection>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._headerLabel)
		{
			_headerLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicsContainer)
		{
			_relicsContainer = VariantUtils.ConvertTo<GridContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spacer)
		{
			_spacer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
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
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._collection)
		{
			value = VariantUtils.CreateFrom<NRelicCollection>(ref _collection);
			return true;
		}
		if ((ref name) == PropertyName._headerLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _headerLabel);
			return true;
		}
		if ((ref name) == PropertyName._relicsContainer)
		{
			value = VariantUtils.CreateFrom<GridContainer>(ref _relicsContainer);
			return true;
		}
		if ((ref name) == PropertyName._spacer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _spacer);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._collection, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._headerLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spacer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._collection, Variant.From<NRelicCollection>(ref _collection));
		info.AddProperty(PropertyName._headerLabel, Variant.From<MegaRichTextLabel>(ref _headerLabel));
		info.AddProperty(PropertyName._relicsContainer, Variant.From<GridContainer>(ref _relicsContainer));
		info.AddProperty(PropertyName._spacer, Variant.From<Control>(ref _spacer));
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._collection, ref val))
		{
			_collection = ((Variant)(ref val)).As<NRelicCollection>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._headerLabel, ref val2))
		{
			_headerLabel = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicsContainer, ref val3))
		{
			_relicsContainer = ((Variant)(ref val3)).As<GridContainer>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._spacer, ref val4))
		{
			_spacer = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val5))
		{
			_icon = ((Variant)(ref val5)).As<TextureRect>();
		}
	}
}
