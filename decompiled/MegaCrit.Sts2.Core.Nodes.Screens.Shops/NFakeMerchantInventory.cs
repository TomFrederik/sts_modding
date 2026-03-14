using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Shops;

[ScriptPath("res://src/Core/Nodes/Screens/Shops/NFakeMerchantInventory.cs")]
public class NFakeMerchantInventory : NMerchantInventory
{
	public new class MethodName : NMerchantInventory.MethodName
	{
		public new static readonly StringName UpdateNavigation = StringName.op_Implicit("UpdateNavigation");
	}

	public new class PropertyName : NMerchantInventory.PropertyName
	{
	}

	public new class SignalName : NMerchantInventory.SignalName
	{
	}

	protected override void UpdateNavigation()
	{
		Control? relicContainer = _relicContainer;
		List<NMerchantSlot> list = ((relicContainer != null) ? ((IEnumerable)((Node)relicContainer).GetChildren(false)).OfType<NMerchantSlot>().ToList() : null) ?? new List<NMerchantSlot>();
		List<List<NMerchantSlot>> list2 = new List<List<NMerchantSlot>>();
		int num = 2;
		List<NMerchantSlot> list3 = new List<NMerchantSlot>(num);
		CollectionsMarshal.SetCount(list3, num);
		Span<NMerchantSlot> span = CollectionsMarshal.AsSpan(list3);
		int num2 = 0;
		span[num2] = list[0];
		num2++;
		span[num2] = list[1];
		list2.Add(list3);
		num2 = 3;
		List<NMerchantSlot> list4 = new List<NMerchantSlot>(num2);
		CollectionsMarshal.SetCount(list4, num2);
		span = CollectionsMarshal.AsSpan(list4);
		num = 0;
		span[num] = list[2];
		num++;
		span[num] = list[3];
		num++;
		span[num] = list[4];
		list2.Add(list4);
		num = 1;
		List<NMerchantSlot> list5 = new List<NMerchantSlot>(num);
		CollectionsMarshal.SetCount(list5, num);
		span = CollectionsMarshal.AsSpan(list5);
		num2 = 0;
		span[num2] = list[5];
		list2.Add(list5);
		for (int i = 0; i < list2.Count; i++)
		{
			for (int j = 0; j < list2[i].Count; j++)
			{
				((Control)list2[i][j]).FocusNeighborLeft = ((j > 0) ? ((Node)list2[i][j - 1]).GetPath() : ((Node)list2[i][j]).GetPath());
				((Control)list2[i][j]).FocusNeighborRight = ((j < list2[i].Count - 1) ? ((Node)list2[i][j + 1]).GetPath() : ((Node)list2[i][j]).GetPath());
				if (i > 0)
				{
					((Control)list2[i][j]).FocusNeighborTop = ((j < list2[i - 1].Count) ? ((Node)list2[i - 1][j]).GetPath() : ((Node)list2[i - 1][list2[i - 1].Count - 1]).GetPath());
				}
				else
				{
					((Control)list2[i][j]).FocusNeighborTop = ((Node)list2[i][j]).GetPath();
				}
				if (i < list2.Count - 1)
				{
					((Control)list2[i][j]).FocusNeighborBottom = ((j < list2[i + 1].Count) ? ((Node)list2[i + 1][j]).GetPath() : ((Node)list2[i + 1][list2[i + 1].Count - 1]).GetPath());
				}
				else
				{
					((Control)list2[i][j]).FocusNeighborBottom = ((Node)list2[i][j]).GetPath();
				}
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName.UpdateNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.UpdateNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateNavigation();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.UpdateNavigation)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
	}
}
