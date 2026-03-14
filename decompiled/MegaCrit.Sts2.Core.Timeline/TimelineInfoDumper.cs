using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

namespace MegaCrit.Sts2.Core.Timeline;

[ScriptPath("res://src/Core/Timeline/TimelineInfoDumper.cs")]
public class TimelineInfoDumper : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName Dump = StringName.op_Implicit("Dump");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	public static void Dump()
	{
		List<EpochModel> allEpochs = GetAllEpochs();
		Console.Out.WriteLine("START TIMELINE INFO DUMPER");
		Console.Out.WriteLine("START TIMELINE INFO DUMPER");
		Console.Out.WriteLine("START TIMELINE INFO DUMPER");
		foreach (EpochModel item in allEpochs)
		{
			Console.Out.WriteLine($"\"{item.Id}\", \"{item.Era}\", \"{(int)item.Era}.{item.EraPosition}\", \"{item.Title}\", \"{item.Description.Replace("\r", "").Replace("\n", "")}\", \"{item.UnlockText}\", \"{item.BigPortraitPath}\"");
		}
		Console.Out.WriteLine("END TIMELINE INFO DUMPER");
		Console.Out.WriteLine("END TIMELINE INFO DUMPER");
		Console.Out.WriteLine("END TIMELINE INFO DUMPER");
	}

	public static List<EpochModel> GetAllEpochs()
	{
		return EpochModel.AllEpochIds.Select(EpochModel.Get).ToList();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName.Dump, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Dump && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Dump();
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Dump && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Dump();
			ret = default(godot_variant);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Dump)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
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
