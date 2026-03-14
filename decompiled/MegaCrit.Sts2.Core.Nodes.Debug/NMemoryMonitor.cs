using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Nodes.Debug;

[ScriptPath("res://src/Core/Nodes/Debug/NMemoryMonitor.cs")]
public class NMemoryMonitor : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName StopMemoryMonitoring = StringName.op_Implicit("StopMemoryMonitoring");

		public static readonly StringName PrintMemoryUsage = StringName.op_Implicit("PrintMemoryUsage");

		public static readonly StringName FormatMemoryUsage = StringName.op_Implicit("FormatMemoryUsage");

		public static readonly StringName PrintLargestAssets = StringName.op_Implicit("PrintLargestAssets");

		public static readonly StringName GetFileSizeInMb = StringName.op_Implicit("GetFileSizeInMb");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _isMonitoring = StringName.op_Implicit("_isMonitoring");
	}

	public class SignalName : SignalName
	{
	}

	private bool _isMonitoring;

	public override void _EnterTree()
	{
	}

	public override void _ExitTree()
	{
		StopMemoryMonitoring();
	}

	private async Task StartMemoryMonitoring()
	{
		_isMonitoring = true;
		while (_isMonitoring)
		{
			PrintMemoryUsage();
			PrintLargestAssets();
			await Task.Delay(10000);
		}
	}

	private void StopMemoryMonitoring()
	{
		_isMonitoring = false;
	}

	private static void PrintMemoryUsage()
	{
		Log.Info($"GetStaticMemoryUsage={OS.GetStaticMemoryUsage()}");
		Log.Info($"GetStaticMemoryPeakUsage={OS.GetStaticMemoryPeakUsage()}");
	}

	private static string FormatMemoryUsage(long bytes)
	{
		string[] array = new string[5] { "B", "KB", "MB", "GB", "TB" };
		int num = 0;
		while (bytes >= 1024 && num < array.Length - 1)
		{
			num++;
			bytes /= (long)Math.Pow(1024.0, num);
		}
		return $"{bytes:0.##} {array[num]}";
	}

	private static void PrintLargestAssets()
	{
		var enumerable = (from assetPath in PreloadManager.Cache.GetCacheKeys()
			select new
			{
				Path = assetPath,
				Size = GetFileSizeInMb(assetPath)
			} into file
			orderby file.Size descending
			select file).Take(10);
		Log.Info("Top 10 largest files in asset cache:");
		foreach (var item in enumerable)
		{
			Log.Info($"Size: {item.Size:F3} MB, Path: {item.Path}");
		}
	}

	private static float GetFileSizeInMb(string godotPath)
	{
		string text = ProjectSettings.GlobalizePath(godotPath);
		if (File.Exists(text))
		{
			long length = new FileInfo(text).Length;
			return (float)length / 1048576f;
		}
		Log.Info("File does not exist: " + text);
		return 0f;
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
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopMemoryMonitoring, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PrintMemoryUsage, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FormatMemoryUsage, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("bytes"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PrintLargestAssets, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetFileSizeInMb, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("godotPath"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.StopMemoryMonitoring && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopMemoryMonitoring();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PrintMemoryUsage && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PrintMemoryUsage();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FormatMemoryUsage && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = FormatMemoryUsage(VariantUtils.ConvertTo<long>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref method) == MethodName.PrintLargestAssets && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PrintLargestAssets();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetFileSizeInMb && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			float fileSizeInMb = GetFileSizeInMb(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<float>(ref fileSizeInMb);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.PrintMemoryUsage && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PrintMemoryUsage();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FormatMemoryUsage && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = FormatMemoryUsage(VariantUtils.ConvertTo<long>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref method) == MethodName.PrintLargestAssets && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PrintLargestAssets();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetFileSizeInMb && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			float fileSizeInMb = GetFileSizeInMb(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<float>(ref fileSizeInMb);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.StopMemoryMonitoring)
		{
			return true;
		}
		if ((ref method) == MethodName.PrintMemoryUsage)
		{
			return true;
		}
		if ((ref method) == MethodName.FormatMemoryUsage)
		{
			return true;
		}
		if ((ref method) == MethodName.PrintLargestAssets)
		{
			return true;
		}
		if ((ref method) == MethodName.GetFileSizeInMb)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._isMonitoring)
		{
			_isMonitoring = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._isMonitoring)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isMonitoring);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._isMonitoring, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._isMonitoring, Variant.From<bool>(ref _isMonitoring));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._isMonitoring, ref val))
		{
			_isMonitoring = ((Variant)(ref val)).As<bool>();
		}
	}
}
