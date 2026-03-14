using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

namespace MegaCrit.Sts2.Core.Nodes.Animation;

[ScriptPath("res://src/Core/Nodes/Animation/NDecimillipedeSegmentDriver.cs")]
public class NDecimillipedeSegmentDriver : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName AttackShake = StringName.op_Implicit("AttackShake");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _leftSegment = StringName.op_Implicit("_leftSegment");

		public static readonly StringName _speed = StringName.op_Implicit("_speed");

		public static readonly StringName _magnitude = StringName.op_Implicit("_magnitude");

		public static readonly StringName _originPos = StringName.op_Implicit("_originPos");

		public static readonly StringName _noise = StringName.op_Implicit("_noise");

		public static readonly StringName _time = StringName.op_Implicit("_time");

		public static readonly StringName _decimillipedeStrikeOffset = StringName.op_Implicit("_decimillipedeStrikeOffset");

		public static readonly StringName _attackTween = StringName.op_Implicit("_attackTween");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private bool _leftSegment;

	private float _speed;

	private float _magnitude;

	private Vector2 _originPos;

	private FastNoiseLite _noise = new FastNoiseLite();

	private float _time;

	private Vector2 _decimillipedeStrikeOffset = Vector2.Zero;

	private Tween? _attackTween;

	public override void _Ready()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_originPos = ((Node2D)this).Position;
		_speed = (_leftSegment ? 0.1f : 0.05f);
		_magnitude = (_leftSegment ? 250f : 300f);
		_noise.NoiseType = (NoiseTypeEnum)3;
		_noise.Frequency = 1f;
	}

	public override void _Process(double delta)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		_time += (float)delta;
		float num = _time * _speed + (_leftSegment ? 0.25f : 0f);
		Vector2 val = new Vector2(((Noise)_noise).GetNoise1D(num), ((Noise)_noise).GetNoise1D(num + 0.25f)) * _magnitude;
		((Node2D)this).Position = _originPos + val + _decimillipedeStrikeOffset;
	}

	public void AttackShake()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		Tween? attackTween = _attackTween;
		if (attackTween != null)
		{
			attackTween.Kill();
		}
		_attackTween = ((Node)this).CreateTween();
		_attackTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("_decimillipedeStrikeOffset"), Variant.op_Implicit(Vector2.Left * 100f), 0.4).SetEase((EaseType)2).SetTrans((TransitionType)1);
		_attackTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("_decimillipedeStrikeOffset"), Variant.op_Implicit(Vector2.Right * 100f), 0.10000000149011612).SetEase((EaseType)2).SetTrans((TransitionType)1);
		_attackTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("_decimillipedeStrikeOffset"), Variant.op_Implicit(Vector2.Zero), 0.75).SetEase((EaseType)2).SetTrans((TransitionType)1);
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
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AttackShake, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.AttackShake && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AttackShake();
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
		if ((ref method) == MethodName.AttackShake)
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
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._leftSegment)
		{
			_leftSegment = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._speed)
		{
			_speed = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._magnitude)
		{
			_magnitude = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originPos)
		{
			_originPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noise)
		{
			_noise = VariantUtils.ConvertTo<FastNoiseLite>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._time)
		{
			_time = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._decimillipedeStrikeOffset)
		{
			_decimillipedeStrikeOffset = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._attackTween)
		{
			_attackTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._leftSegment)
		{
			value = VariantUtils.CreateFrom<bool>(ref _leftSegment);
			return true;
		}
		if ((ref name) == PropertyName._speed)
		{
			value = VariantUtils.CreateFrom<float>(ref _speed);
			return true;
		}
		if ((ref name) == PropertyName._magnitude)
		{
			value = VariantUtils.CreateFrom<float>(ref _magnitude);
			return true;
		}
		if ((ref name) == PropertyName._originPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _originPos);
			return true;
		}
		if ((ref name) == PropertyName._noise)
		{
			value = VariantUtils.CreateFrom<FastNoiseLite>(ref _noise);
			return true;
		}
		if ((ref name) == PropertyName._time)
		{
			value = VariantUtils.CreateFrom<float>(ref _time);
			return true;
		}
		if ((ref name) == PropertyName._decimillipedeStrikeOffset)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _decimillipedeStrikeOffset);
			return true;
		}
		if ((ref name) == PropertyName._attackTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _attackTween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._leftSegment, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._speed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._magnitude, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._originPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._noise, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._time, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._decimillipedeStrikeOffset, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._attackTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._leftSegment, Variant.From<bool>(ref _leftSegment));
		info.AddProperty(PropertyName._speed, Variant.From<float>(ref _speed));
		info.AddProperty(PropertyName._magnitude, Variant.From<float>(ref _magnitude));
		info.AddProperty(PropertyName._originPos, Variant.From<Vector2>(ref _originPos));
		info.AddProperty(PropertyName._noise, Variant.From<FastNoiseLite>(ref _noise));
		info.AddProperty(PropertyName._time, Variant.From<float>(ref _time));
		info.AddProperty(PropertyName._decimillipedeStrikeOffset, Variant.From<Vector2>(ref _decimillipedeStrikeOffset));
		info.AddProperty(PropertyName._attackTween, Variant.From<Tween>(ref _attackTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._leftSegment, ref val))
		{
			_leftSegment = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._speed, ref val2))
		{
			_speed = ((Variant)(ref val2)).As<float>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._magnitude, ref val3))
		{
			_magnitude = ((Variant)(ref val3)).As<float>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._originPos, ref val4))
		{
			_originPos = ((Variant)(ref val4)).As<Vector2>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._noise, ref val5))
		{
			_noise = ((Variant)(ref val5)).As<FastNoiseLite>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._time, ref val6))
		{
			_time = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._decimillipedeStrikeOffset, ref val7))
		{
			_decimillipedeStrikeOffset = ((Variant)(ref val7)).As<Vector2>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._attackTween, ref val8))
		{
			_attackTween = ((Variant)(ref val8)).As<Tween>();
		}
	}
}
