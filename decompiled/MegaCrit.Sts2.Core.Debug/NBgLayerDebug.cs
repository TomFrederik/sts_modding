using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Debug;

[Tool]
[ScriptPath("res://src/Core/Debug/NBgLayerDebug.cs")]
public class NBgLayerDebug : Control
{
	public enum LayerVisibility
	{
		A,
		B,
		C
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName ReloadLayers = StringName.op_Implicit("ReloadLayers");

		public static readonly StringName UpdateLayers = StringName.op_Implicit("UpdateLayers");

		public static readonly StringName AddLayer = StringName.op_Implicit("AddLayer");

		public static readonly StringName ToLayerName = StringName.op_Implicit("ToLayerName");

		public static readonly StringName ClearLayers = StringName.op_Implicit("ClearLayers");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName VisibleLayer = StringName.op_Implicit("VisibleLayer");

		public static readonly StringName ReloadLayersCallable = StringName.op_Implicit("ReloadLayersCallable");

		public static readonly StringName _layerA = StringName.op_Implicit("_layerA");

		public static readonly StringName _layerB = StringName.op_Implicit("_layerB");

		public static readonly StringName _layerC = StringName.op_Implicit("_layerC");

		public static readonly StringName _visibleLayer = StringName.op_Implicit("_visibleLayer");
	}

	public class SignalName : SignalName
	{
	}

	private const string _layerNodePrefix = "Layer_";

	private PackedScene? _layerA;

	private PackedScene? _layerB;

	private PackedScene? _layerC;

	private LayerVisibility _visibleLayer;

	[Export(/*Could not decode attribute arguments.*/)]
	public LayerVisibility VisibleLayer
	{
		get
		{
			return _visibleLayer;
		}
		set
		{
			_visibleLayer = value;
			if (Engine.IsEditorHint())
			{
				UpdateLayers();
			}
		}
	}

	[ExportToolButton("Reload Layers")]
	private Callable ReloadLayersCallable => Callable.From((Action)ReloadLayers);

	public override void _EnterTree()
	{
		if (Engine.IsEditorHint())
		{
			ReloadLayers();
		}
	}

	private void ReloadLayers()
	{
		string sceneFilePath = ((Node)this).GetTree().GetEditedSceneRoot().SceneFilePath;
		if (sceneFilePath == null)
		{
			return;
		}
		string text;
		if (((object)((Node)this).Name).ToString() == "Foreground")
		{
			text = "fg";
		}
		else
		{
			string text2 = ((object)((Node)this).Name).ToString();
			int length = "Layer_".Length;
			if (!int.TryParse(text2.Substring(length, text2.Length - length), out var result))
			{
				return;
			}
			text = $"bg_{result:D2}";
		}
		string file = StringExtensions.GetFile(sceneFilePath);
		string text3 = file.Substring(0, file.LastIndexOf('_'));
		string text4 = Path.Combine(StringExtensions.GetBaseDir(sceneFilePath), "layers", text3 + "_" + text);
		string text5 = text4 + "_a.tscn";
		string text6 = text4 + "_b.tscn";
		string text7 = text4 + "_c.tscn";
		if (ResourceLoader.Exists(text5, ""))
		{
			_layerA = ResourceLoader.Load<PackedScene>(text5, (string)null, (CacheMode)1);
		}
		if (ResourceLoader.Exists(text6, ""))
		{
			_layerB = ResourceLoader.Load<PackedScene>(text6, (string)null, (CacheMode)1);
		}
		if (ResourceLoader.Exists(text7, ""))
		{
			_layerC = ResourceLoader.Load<PackedScene>(text7, (string)null, (CacheMode)1);
		}
		UpdateLayers();
	}

	private void UpdateLayers()
	{
		ClearLayers();
		if (_visibleLayer == LayerVisibility.A && _layerA != null)
		{
			AddLayer(LayerVisibility.A, _layerA);
		}
		if (_visibleLayer == LayerVisibility.B && _layerB != null)
		{
			AddLayer(LayerVisibility.B, _layerB);
		}
		if (_visibleLayer == LayerVisibility.C && _layerC != null)
		{
			AddLayer(LayerVisibility.C, _layerC);
		}
	}

	private void AddLayer(LayerVisibility name, PackedScene layerScene)
	{
		Control val = layerScene.Instantiate<Control>((GenEditState)0);
		((Node)val).Name = StringName.op_Implicit(ToLayerName(name));
		((Node)(object)this).AddChildSafely((Node?)(object)val);
	}

	private static string ToLayerName(LayerVisibility layer)
	{
		return $"{"Layer_"}{layer}";
	}

	private IEnumerable<Control> GetLayerNodes()
	{
		foreach (Node child in ((Node)this).GetChildren(false))
		{
			if (((object)child.Name).ToString().StartsWith("Layer_"))
			{
				yield return (Control)child;
			}
		}
	}

	private void ClearLayers()
	{
		foreach (Control layerNode in GetLayerNodes())
		{
			((Node)(object)this).RemoveChildSafely((Node?)(object)layerNode);
			((Node)(object)layerNode).QueueFreeSafely();
		}
	}

	public override void _ExitTree()
	{
		ClearLayers();
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
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReloadLayers, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateLayers, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddLayer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("name"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)24, StringName.op_Implicit("layerScene"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("PackedScene"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToLayerName, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("layer"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearLayers, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ReloadLayers && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ReloadLayers();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateLayers && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateLayers();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddLayer && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			AddLayer(VariantUtils.ConvertTo<LayerVisibility>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<PackedScene>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToLayerName && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = ToLayerName(VariantUtils.ConvertTo<LayerVisibility>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref method) == MethodName.ClearLayers && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearLayers();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.ToLayerName && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = ToLayerName(VariantUtils.ConvertTo<LayerVisibility>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
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
		if ((ref method) == MethodName.ReloadLayers)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateLayers)
		{
			return true;
		}
		if ((ref method) == MethodName.AddLayer)
		{
			return true;
		}
		if ((ref method) == MethodName.ToLayerName)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearLayers)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.VisibleLayer)
		{
			VisibleLayer = VariantUtils.ConvertTo<LayerVisibility>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._layerA)
		{
			_layerA = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._layerB)
		{
			_layerB = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._layerC)
		{
			_layerC = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._visibleLayer)
		{
			_visibleLayer = VariantUtils.ConvertTo<LayerVisibility>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.VisibleLayer)
		{
			LayerVisibility visibleLayer = VisibleLayer;
			value = VariantUtils.CreateFrom<LayerVisibility>(ref visibleLayer);
			return true;
		}
		if ((ref name) == PropertyName.ReloadLayersCallable)
		{
			Callable reloadLayersCallable = ReloadLayersCallable;
			value = VariantUtils.CreateFrom<Callable>(ref reloadLayersCallable);
			return true;
		}
		if ((ref name) == PropertyName._layerA)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _layerA);
			return true;
		}
		if ((ref name) == PropertyName._layerB)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _layerB);
			return true;
		}
		if ((ref name) == PropertyName._layerC)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _layerC);
			return true;
		}
		if ((ref name) == PropertyName._visibleLayer)
		{
			value = VariantUtils.CreateFrom<LayerVisibility>(ref _visibleLayer);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._layerA, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._layerB, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._layerC, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._visibleLayer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.VisibleLayer, (PropertyHint)2, "A,B,C", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)25, PropertyName.ReloadLayersCallable, (PropertyHint)39, "Reload Layers", (PropertyUsageFlags)4, true));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName visibleLayer = PropertyName.VisibleLayer;
		LayerVisibility visibleLayer2 = VisibleLayer;
		info.AddProperty(visibleLayer, Variant.From<LayerVisibility>(ref visibleLayer2));
		info.AddProperty(PropertyName._layerA, Variant.From<PackedScene>(ref _layerA));
		info.AddProperty(PropertyName._layerB, Variant.From<PackedScene>(ref _layerB));
		info.AddProperty(PropertyName._layerC, Variant.From<PackedScene>(ref _layerC));
		info.AddProperty(PropertyName._visibleLayer, Variant.From<LayerVisibility>(ref _visibleLayer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.VisibleLayer, ref val))
		{
			VisibleLayer = ((Variant)(ref val)).As<LayerVisibility>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._layerA, ref val2))
		{
			_layerA = ((Variant)(ref val2)).As<PackedScene>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._layerB, ref val3))
		{
			_layerB = ((Variant)(ref val3)).As<PackedScene>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._layerC, ref val4))
		{
			_layerC = ((Variant)(ref val4)).As<PackedScene>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._visibleLayer, ref val5))
		{
			_visibleLayer = ((Variant)(ref val5)).As<LayerVisibility>();
		}
	}
}
