using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NGridCardPreviewContainer.cs")]
public class NGridCardPreviewContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ForceMaxColumnsUntilEmpty = StringName.op_Implicit("ForceMaxColumnsUntilEmpty");

		public static readonly StringName ReformatElements = StringName.op_Implicit("ReformatElements");

		public static readonly StringName CheckAnyChildrenPresent = StringName.op_Implicit("CheckAnyChildrenPresent");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	private int? _forcedMaxColumns;

	public override void _Ready()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).Connect(SignalName.ChildEnteredTree, Callable.From<Node>((Action<Node>)ReformatElements), 0u);
		((GodotObject)this).Connect(SignalName.ChildExitingTree, Callable.From<Node>((Action<Node>)CheckAnyChildrenPresent), 0u);
	}

	public void ForceMaxColumnsUntilEmpty(int maxColumns)
	{
		_forcedMaxColumns = maxColumns;
	}

	private void ReformatElements(Node _)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = ((Control)this).Size / 2f + Vector2.Down * 50f;
		int childCount = ((Node)this).GetChildCount(false);
		Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
		int num = Mathf.FloorToInt(((Rect2)(ref viewportRect)).Size.X / (NCard.defaultSize.X + 25f));
		int num2 = Mathf.CeilToInt((float)childCount / (float)num);
		if (_forcedMaxColumns.HasValue)
		{
			num = Math.Min(num, _forcedMaxColumns.Value);
		}
		for (int i = 0; i < childCount; i++)
		{
			int num3 = i / num;
			int num4 = i % num;
			int num5 = Math.Min(num, childCount - num3 * num);
			float num6 = (float)(-(num2 - 1)) * (NCard.defaultSize.Y + 25f) * 0.5f;
			float num7 = (float)(-(num5 - 1)) * (NCard.defaultSize.X + 25f) * 0.5f;
			Vector2 val2 = val;
			Vector2 position = val2 + new Vector2(num7 + (NCard.defaultSize.X + 25f) * (float)num4, num6 + (NCard.defaultSize.Y + 25f) * (float)num3);
			Node child = ((Node)this).GetChild(i, false);
			Node2D val3 = (Node2D)(object)((child is Node2D) ? child : null);
			if (val3 != null)
			{
				val3.Position = position;
			}
			else
			{
				((Control)child).Position = position;
			}
		}
	}

	private void CheckAnyChildrenPresent(Node _)
	{
		if (((Node)this).GetChildCount(false) == 0)
		{
			_forcedMaxColumns = null;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ForceMaxColumnsUntilEmpty, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("maxColumns"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReformatElements, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CheckAnyChildrenPresent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ForceMaxColumnsUntilEmpty && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ForceMaxColumnsUntilEmpty(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ReformatElements && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ReformatElements(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CheckAnyChildrenPresent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CheckAnyChildrenPresent(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.ForceMaxColumnsUntilEmpty)
		{
			return true;
		}
		if ((ref method) == MethodName.ReformatElements)
		{
			return true;
		}
		if ((ref method) == MethodName.CheckAnyChildrenPresent)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
	}
}
