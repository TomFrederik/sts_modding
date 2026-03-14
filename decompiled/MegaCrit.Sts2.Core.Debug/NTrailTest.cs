using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Debug;

[ScriptPath("res://src/Core/Debug/NTrailTest.cs")]
public class NTrailTest : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	public override void _Ready()
	{
		DelaySpawn();
	}

	private async Task DelaySpawn()
	{
		await Task.Delay(100);
		Control node = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Ironclad"));
		NCardTrailVfx child = NCardTrailVfx.Create(node, SceneHelper.GetScenePath("vfx/card_trail_ironclad"));
		((Node)this).GetParent().AddChildSafely((Node?)(object)child);
		Control node2 = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Silent"));
		child = NCardTrailVfx.Create(node2, SceneHelper.GetScenePath("vfx/card_trail_silent"));
		((Node)this).GetParent().AddChildSafely((Node?)(object)child);
		Control node3 = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Defect"));
		child = NCardTrailVfx.Create(node3, SceneHelper.GetScenePath("vfx/card_trail_defect"));
		((Node)this).GetParent().AddChildSafely((Node?)(object)child);
		Control node4 = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Regent"));
		child = NCardTrailVfx.Create(node4, SceneHelper.GetScenePath("vfx/card_trail_regent"));
		((Node)this).GetParent().AddChildSafely((Node?)(object)child);
		Control node5 = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Binder"));
		child = NCardTrailVfx.Create(node5, SceneHelper.GetScenePath("vfx/card_trail_necrobinder"));
		((Node)this).GetParent().AddChildSafely((Node?)(object)child);
	}

	public override void _Process(double delta)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mousePosition = ((Node)this).GetViewport().GetMousePosition();
		((Control)this).GlobalPosition = mousePosition;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName._Process)
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
