using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

[ScriptPath("res://src/Core/Nodes/Vfx/Utilities/NSpriteAnimator.cs")]
public class NSpriteAnimator : Sprite2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _frames = StringName.op_Implicit("_frames");

		public static readonly StringName _fps = StringName.op_Implicit("_fps");

		public static readonly StringName _loop = StringName.op_Implicit("_loop");

		public static readonly StringName _randomizeRotation = StringName.op_Implicit("_randomizeRotation");

		public static readonly StringName _rotationRange = StringName.op_Implicit("_rotationRange");
	}

	public class SignalName : SignalName
	{
	}

	[ExportGroup("Animation Settings", "")]
	[Export(/*Could not decode attribute arguments.*/)]
	private Texture2D[] _frames;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _fps = 15f;

	[Export(/*Could not decode attribute arguments.*/)]
	private bool _loop;

	[ExportGroup("Rotation Settings", "")]
	[Export(/*Could not decode attribute arguments.*/)]
	private bool _randomizeRotation;

	[Export(/*Could not decode attribute arguments.*/)]
	private Vector2 _rotationRange;

	private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

	public override void _Ready()
	{
		if (_randomizeRotation)
		{
			((Node2D)this).RotationDegrees = new System.Random().Next((int)_rotationRange.X, (int)_rotationRange.Y);
		}
		TaskHelper.RunSafely(PlayAnimation());
	}

	public override void _ExitTree()
	{
		_cancelToken.Cancel();
	}

	private async Task PlayAnimation()
	{
		int i = 0;
		int interval = Mathf.RoundToInt(1000f / _fps);
		while (!_cancelToken.IsCancellationRequested)
		{
			((Sprite2D)this).Texture = _frames[i];
			i++;
			if (_loop)
			{
				i %= _frames.Length;
			}
			await Task.Delay(interval, _cancelToken.Token);
			if (_frames.Length <= i)
			{
				break;
			}
		}
		if (!_cancelToken.IsCancellationRequested)
		{
			((Node)(object)this).QueueFreeSafely();
		}
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
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		return ((Sprite2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Sprite2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._frames)
		{
			_frames = VariantUtils.ConvertToSystemArrayOfGodotObject<Texture2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fps)
		{
			_fps = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._loop)
		{
			_loop = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._randomizeRotation)
		{
			_randomizeRotation = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rotationRange)
		{
			_rotationRange = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._frames)
		{
			GodotObject[] frames = (GodotObject[])(object)_frames;
			value = VariantUtils.CreateFromSystemArrayOfGodotObject(frames);
			return true;
		}
		if ((ref name) == PropertyName._fps)
		{
			value = VariantUtils.CreateFrom<float>(ref _fps);
			return true;
		}
		if ((ref name) == PropertyName._loop)
		{
			value = VariantUtils.CreateFrom<bool>(ref _loop);
			return true;
		}
		if ((ref name) == PropertyName._randomizeRotation)
		{
			value = VariantUtils.CreateFrom<bool>(ref _randomizeRotation);
			return true;
		}
		if ((ref name) == PropertyName._rotationRange)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _rotationRange);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)0, StringName.op_Implicit("Animation Settings"), (PropertyHint)0, "", (PropertyUsageFlags)64, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._frames, (PropertyHint)23, "24/17:Texture2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._fps, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)1, PropertyName._loop, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)0, StringName.op_Implicit("Rotation Settings"), (PropertyHint)0, "", (PropertyUsageFlags)64, true));
		list.Add(new PropertyInfo((Type)1, PropertyName._randomizeRotation, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)5, PropertyName._rotationRange, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName frames = PropertyName._frames;
		GodotObject[] frames2 = (GodotObject[])(object)_frames;
		info.AddProperty(frames, Variant.CreateFrom(frames2));
		info.AddProperty(PropertyName._fps, Variant.From<float>(ref _fps));
		info.AddProperty(PropertyName._loop, Variant.From<bool>(ref _loop));
		info.AddProperty(PropertyName._randomizeRotation, Variant.From<bool>(ref _randomizeRotation));
		info.AddProperty(PropertyName._rotationRange, Variant.From<Vector2>(ref _rotationRange));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._frames, ref val))
		{
			_frames = ((Variant)(ref val)).AsGodotObjectArray<Texture2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._fps, ref val2))
		{
			_fps = ((Variant)(ref val2)).As<float>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._loop, ref val3))
		{
			_loop = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._randomizeRotation, ref val4))
		{
			_randomizeRotation = ((Variant)(ref val4)).As<bool>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._rotationRange, ref val5))
		{
			_rotationRange = ((Variant)(ref val5)).As<Vector2>();
		}
	}
}
