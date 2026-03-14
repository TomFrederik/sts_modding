using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.PotionLab;

[ScriptPath("res://src/Core/Nodes/Screens/PotionLab/NPotionLabCategory.cs")]
public class NPotionLabCategory : VBoxContainer
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ClearPotions = StringName.op_Implicit("ClearPotions");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _headerLabel = StringName.op_Implicit("_headerLabel");

		public static readonly StringName _potionContainer = StringName.op_Implicit("_potionContainer");
	}

	public class SignalName : SignalName
	{
	}

	private MegaRichTextLabel _headerLabel;

	private GridContainer _potionContainer;

	public Control? DefaultFocusedControl
	{
		get
		{
			if (((Node)_potionContainer).GetChildCount(false) <= 0)
			{
				return null;
			}
			return ((Node)_potionContainer).GetChild<Control>(0, false);
		}
	}

	public override void _Ready()
	{
		_headerLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("Header"));
		_potionContainer = ((Node)this).GetNode<GridContainer>(NodePath.op_Implicit("%PotionsContainer"));
	}

	public void LoadPotions(PotionRarity potionRarity, LocString header, HashSet<PotionModel> seenPotions, UnlockState unlockState, HashSet<PotionModel> allUnlockedPotions, PotionRarity? secondRarity = null)
	{
		_headerLabel.Text = header.GetFormattedText();
		IEnumerable<PotionModel> enumerable = ModelDb.AllPotions.Where((PotionModel relic) => relic.Rarity == potionRarity || relic.Rarity == secondRarity);
		List<PotionModel> list = new List<PotionModel>();
		List<PotionModel> list2 = new List<PotionModel>();
		foreach (PotionPoolModel allCharacterPotionPool in ModelDb.AllCharacterPotionPools)
		{
			foreach (PotionModel item in enumerable)
			{
				if (allCharacterPotionPool.AllPotionIds.Contains(item.Id))
				{
					list.Add(item);
				}
			}
		}
		foreach (PotionModel item2 in enumerable)
		{
			if (!list.Contains(item2))
			{
				list2.Add(item2);
			}
		}
		StringComparer comparer = StringComparer.Create(LocManager.Instance.CultureInfo, CompareOptions.None);
		list2.Sort((PotionModel p1, PotionModel p2) => comparer.Compare(p1.Title.GetFormattedText(), p2.Title.GetFormattedText()));
		foreach (PotionModel item3 in list2.Concat(list))
		{
			ModelVisibility visibility = ((!allUnlockedPotions.Contains(item3)) ? ModelVisibility.Locked : (seenPotions.Contains(item3) ? ModelVisibility.Visible : ModelVisibility.NotSeen));
			NLabPotionHolder child = NLabPotionHolder.Create(item3.ToMutable(), visibility);
			((Node)(object)_potionContainer).AddChildSafely((Node?)(object)child);
		}
	}

	public void ClearPotions()
	{
		foreach (Node child in ((Node)_potionContainer).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
	}

	public List<IReadOnlyList<Control>> GetGridItems()
	{
		List<IReadOnlyList<Control>> list = new List<IReadOnlyList<Control>>();
		for (int i = 0; i < ((Node)_potionContainer).GetChildren(false).Count; i += _potionContainer.Columns)
		{
			list.Add(((IEnumerable)((Node)_potionContainer).GetChildren(false)).OfType<Control>().Skip(i).Take(_potionContainer.Columns)
				.ToList());
		}
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearPotions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearPotions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearPotions();
			ret = default(godot_variant);
			return true;
		}
		return ((VBoxContainer)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearPotions)
		{
			return true;
		}
		return ((VBoxContainer)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._headerLabel)
		{
			_headerLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionContainer)
		{
			_potionContainer = VariantUtils.ConvertTo<GridContainer>(ref value);
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
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._headerLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _headerLabel);
			return true;
		}
		if ((ref name) == PropertyName._potionContainer)
		{
			value = VariantUtils.CreateFrom<GridContainer>(ref _potionContainer);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._headerLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._headerLabel, Variant.From<MegaRichTextLabel>(ref _headerLabel));
		info.AddProperty(PropertyName._potionContainer, Variant.From<GridContainer>(ref _potionContainer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._headerLabel, ref val))
		{
			_headerLabel = ((Variant)(ref val)).As<MegaRichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionContainer, ref val2))
		{
			_potionContainer = ((Variant)(ref val2)).As<GridContainer>();
		}
	}
}
