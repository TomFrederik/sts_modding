using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Nodes.Rooms;

[ScriptPath("res://src/Core/Nodes/Rooms/NCombatBackground.cs")]
public class NCombatBackground : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName SetForegroundLayer = StringName.op_Implicit("SetForegroundLayer");

		public static readonly StringName AddLayer = StringName.op_Implicit("AddLayer");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	public static NCombatBackground Create(BackgroundAssets bg)
	{
		if (bg.BackgroundScenePath == null)
		{
			throw new InvalidOperationException("Encounter does not have a background.");
		}
		string backgroundScenePath = bg.BackgroundScenePath;
		NCombatBackground nCombatBackground = PreloadManager.Cache.GetScene(backgroundScenePath).Instantiate<NCombatBackground>((GenEditState)0);
		nCombatBackground.SetLayers(bg);
		return nCombatBackground;
	}

	private void SetLayers(BackgroundAssets bg)
	{
		SetBackgroundLayers(bg.BgLayers);
		SetForegroundLayer(bg.FgLayer);
	}

	private void SetBackgroundLayers(IReadOnlyList<string> backgroundLayers)
	{
		for (int i = 0; i < backgroundLayers.Count; i++)
		{
			string layerName = $"Layer_{i:D2}";
			AddLayer(layerName, backgroundLayers[i]);
		}
	}

	private void SetForegroundLayer(string? foregroundLayer)
	{
		if (foregroundLayer != null)
		{
			AddLayer("Foreground", foregroundLayer);
		}
	}

	private void AddLayer(string layerName, string layerPath)
	{
		Node nodeOrNull = ((Node)this).GetNodeOrNull(NodePath.op_Implicit(layerName));
		if (nodeOrNull == null)
		{
			throw new InvalidOperationException("Layer node='" + layerName + "' not found in combat background scene.");
		}
		Control val = PreloadManager.Cache.GetScene(layerPath).Instantiate<Control>((GenEditState)0);
		((CanvasItem)val).Visible = true;
		nodeOrNull.AddChildSafely((Node?)(object)val);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName.SetForegroundLayer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("foregroundLayer"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddLayer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("layerName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("layerPath"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.SetForegroundLayer && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetForegroundLayer(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddLayer && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			AddLayer(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.SetForegroundLayer)
		{
			return true;
		}
		if ((ref method) == MethodName.AddLayer)
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
