using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NKinPriestBeamVfx.cs")]
public class NKinPriestBeamVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName Fire = StringName.op_Implicit("Fire");

		public static readonly StringName OnTweenComplete = StringName.op_Implicit("OnTweenComplete");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _beam = StringName.op_Implicit("_beam");

		public static readonly StringName _beamHolder = StringName.op_Implicit("_beamHolder");

		public static readonly StringName _staticParticles = StringName.op_Implicit("_staticParticles");

		public static readonly StringName _baseBeamScale = StringName.op_Implicit("_baseBeamScale");

		public static readonly StringName _lengthTween = StringName.op_Implicit("_lengthTween");

		public static readonly StringName _rotationTween = StringName.op_Implicit("_rotationTween");
	}

	public class SignalName : SignalName
	{
	}

	private const float _beamMaxLengthScale = 4f;

	private const float _startRotation = 1f;

	private const float _endRotation = -1f;

	private Sprite2D _beam;

	private Node2D _beamHolder;

	private GpuParticles2D _staticParticles;

	private Vector2 _baseBeamScale;

	private Tween? _lengthTween;

	private Tween? _rotationTween;

	public override void _Ready()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		_beam = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("BeamHolder/Beam"));
		_staticParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("BeamHolder/StaticParticles"));
		_beamHolder = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("BeamHolder"));
		_baseBeamScale = ((Node2D)_beam).Scale;
		_staticParticles.Emitting = false;
		((CanvasItem)_staticParticles).Visible = false;
		((CanvasItem)_beamHolder).Visible = false;
	}

	public override void _Process(double delta)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(Rng.Chaotic.NextFloat(-0.05f, 0.05f), Rng.Chaotic.NextFloat(-0.7f, 0.7f));
		((Node2D)_beam).Scale = _baseBeamScale + val;
		Color modulate = ((CanvasItem)this).Modulate;
		modulate.A = Rng.Chaotic.NextFloat(0.8f, 1f);
		((CanvasItem)this).Modulate = modulate;
	}

	public void Fire()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		_staticParticles.Restart();
		((CanvasItem)_staticParticles).Visible = true;
		((CanvasItem)_beamHolder).Visible = true;
		_rotationTween = ((Node)this).CreateTween();
		((Node2D)this).RotationDegrees = 1f;
		_rotationTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("rotation_degrees"), Variant.op_Implicit(-1f), 0.800000011920929).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_beamHolder.Scale = Vector2.One;
		_lengthTween = ((Node)this).CreateTween();
		_lengthTween.TweenProperty((GodotObject)(object)_beamHolder, NodePath.op_Implicit("scale:x"), Variant.op_Implicit(4f), 0.3799999952316284).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_lengthTween.Chain().TweenProperty((GodotObject)(object)_beamHolder, NodePath.op_Implicit("scale:x"), Variant.op_Implicit(0.5), 0.6000000238418579).SetEase((EaseType)0)
			.SetTrans((TransitionType)5);
		_lengthTween.TweenCallback(Callable.From((Action)OnTweenComplete));
	}

	private void OnTweenComplete()
	{
		_rotationTween.Kill();
		_lengthTween.Kill();
		_staticParticles.Emitting = false;
		((CanvasItem)_staticParticles).Visible = false;
		((CanvasItem)_beamHolder).Visible = false;
	}

	public override void _ExitTree()
	{
		Tween? lengthTween = _lengthTween;
		if (lengthTween != null)
		{
			lengthTween.Kill();
		}
		Tween? rotationTween = _rotationTween;
		if (rotationTween != null)
		{
			rotationTween.Kill();
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
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Fire, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnTweenComplete, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.Fire && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Fire();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnTweenComplete && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnTweenComplete();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
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
		if ((ref method) == MethodName.Fire)
		{
			return true;
		}
		if ((ref method) == MethodName.OnTweenComplete)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._beam)
		{
			_beam = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._beamHolder)
		{
			_beamHolder = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._staticParticles)
		{
			_staticParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._baseBeamScale)
		{
			_baseBeamScale = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lengthTween)
		{
			_lengthTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rotationTween)
		{
			_rotationTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._beam)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _beam);
			return true;
		}
		if ((ref name) == PropertyName._beamHolder)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _beamHolder);
			return true;
		}
		if ((ref name) == PropertyName._staticParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _staticParticles);
			return true;
		}
		if ((ref name) == PropertyName._baseBeamScale)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _baseBeamScale);
			return true;
		}
		if ((ref name) == PropertyName._lengthTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _lengthTween);
			return true;
		}
		if ((ref name) == PropertyName._rotationTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _rotationTween);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._beam, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._beamHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._staticParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._baseBeamScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lengthTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rotationTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._beam, Variant.From<Sprite2D>(ref _beam));
		info.AddProperty(PropertyName._beamHolder, Variant.From<Node2D>(ref _beamHolder));
		info.AddProperty(PropertyName._staticParticles, Variant.From<GpuParticles2D>(ref _staticParticles));
		info.AddProperty(PropertyName._baseBeamScale, Variant.From<Vector2>(ref _baseBeamScale));
		info.AddProperty(PropertyName._lengthTween, Variant.From<Tween>(ref _lengthTween));
		info.AddProperty(PropertyName._rotationTween, Variant.From<Tween>(ref _rotationTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._beam, ref val))
		{
			_beam = ((Variant)(ref val)).As<Sprite2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._beamHolder, ref val2))
		{
			_beamHolder = ((Variant)(ref val2)).As<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._staticParticles, ref val3))
		{
			_staticParticles = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._baseBeamScale, ref val4))
		{
			_baseBeamScale = ((Variant)(ref val4)).As<Vector2>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._lengthTween, ref val5))
		{
			_lengthTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._rotationTween, ref val6))
		{
			_rotationTween = ((Variant)(ref val6)).As<Tween>();
		}
	}
}
