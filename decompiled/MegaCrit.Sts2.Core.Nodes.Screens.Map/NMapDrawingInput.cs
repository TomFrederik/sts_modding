using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Map;

[ScriptPath("res://src/Core/Nodes/Screens/Map/NMapDrawingInput.cs")]
public abstract class NMapDrawingInput : Control
{
	[Signal]
	public delegate void FinishedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName StopDrawing = StringName.op_Implicit("StopDrawing");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DrawingMode = StringName.op_Implicit("DrawingMode");

		public static readonly StringName _drawings = StringName.op_Implicit("_drawings");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName Finished = StringName.op_Implicit("Finished");
	}

	protected NMapDrawings _drawings;

	private FinishedEventHandler backing_Finished;

	public DrawingMode DrawingMode { get; private set; }

	public event FinishedEventHandler Finished
	{
		add
		{
			backing_Finished = (FinishedEventHandler)Delegate.Combine(backing_Finished, value);
		}
		remove
		{
			backing_Finished = (FinishedEventHandler)Delegate.Remove(backing_Finished, value);
		}
	}

	public static NMapDrawingInput Create(NMapDrawings drawings, DrawingMode drawingMode, bool stopOnMouseRelease = false)
	{
		NMapDrawingInput nMapDrawingInput = (stopOnMouseRelease ? new NMouseHeldMapDrawingInput() : ((!NControllerManager.Instance.IsUsingController) ? new NMouseModeMapDrawingInput() : NControllerMapDrawingInput.Create()));
		nMapDrawingInput._drawings = drawings;
		nMapDrawingInput.DrawingMode = drawingMode;
		nMapDrawingInput._drawings.SetDrawingModeLocal(drawingMode);
		return nMapDrawingInput;
	}

	public override void _Ready()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)StopDrawing), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)StopDrawing), 0u);
	}

	public override void _EnterTree()
	{
		ActiveScreenContext.Instance.Updated += StopDrawing;
	}

	public override void _ExitTree()
	{
		ActiveScreenContext.Instance.Updated -= StopDrawing;
	}

	public void StopDrawing()
	{
		if (_drawings.IsLocalDrawing())
		{
			_drawings.StopLineLocal();
		}
		_drawings.SetDrawingModeLocal(DrawingMode.None);
		EmitSignalFinished();
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("drawings"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)2, StringName.op_Implicit("drawingMode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("stopOnMouseRelease"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopDrawing, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			NMapDrawingInput nMapDrawingInput = Create(VariantUtils.ConvertTo<NMapDrawings>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<DrawingMode>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<NMapDrawingInput>(ref nMapDrawingInput);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
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
		if ((ref method) == MethodName.StopDrawing && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopDrawing();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			NMapDrawingInput nMapDrawingInput = Create(VariantUtils.ConvertTo<NMapDrawings>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<DrawingMode>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<NMapDrawingInput>(ref nMapDrawingInput);
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.StopDrawing)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.DrawingMode)
		{
			DrawingMode = VariantUtils.ConvertTo<DrawingMode>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._drawings)
		{
			_drawings = VariantUtils.ConvertTo<NMapDrawings>(ref value);
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
		if ((ref name) == PropertyName.DrawingMode)
		{
			DrawingMode drawingMode = DrawingMode;
			value = VariantUtils.CreateFrom<DrawingMode>(ref drawingMode);
			return true;
		}
		if ((ref name) == PropertyName._drawings)
		{
			value = VariantUtils.CreateFrom<NMapDrawings>(ref _drawings);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._drawings, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.DrawingMode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName drawingMode = PropertyName.DrawingMode;
		DrawingMode drawingMode2 = DrawingMode;
		info.AddProperty(drawingMode, Variant.From<DrawingMode>(ref drawingMode2));
		info.AddProperty(PropertyName._drawings, Variant.From<NMapDrawings>(ref _drawings));
		info.AddSignalEventDelegate(SignalName.Finished, (Delegate)backing_Finished);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.DrawingMode, ref val))
		{
			DrawingMode = ((Variant)(ref val)).As<DrawingMode>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._drawings, ref val2))
		{
			_drawings = ((Variant)(ref val2)).As<NMapDrawings>();
		}
		FinishedEventHandler finishedEventHandler = default(FinishedEventHandler);
		if (info.TryGetSignalEventDelegate<FinishedEventHandler>(SignalName.Finished, ref finishedEventHandler))
		{
			backing_Finished = finishedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.Finished, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalFinished()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Finished, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Finished && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_Finished?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Finished)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
