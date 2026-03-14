using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Cards;

[ScriptPath("res://src/Core/Nodes/Vfx/Cards/NSpookyHandVfx.cs")]
public class NSpookyHandVfx : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName AnimateIn = StringName.op_Implicit("AnimateIn");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _elapsedPauseTime = StringName.op_Implicit("_elapsedPauseTime");

		public static readonly StringName _pauseCounter = StringName.op_Implicit("_pauseCounter");

		public static readonly StringName _isPaused = StringName.op_Implicit("_isPaused");

		public static readonly StringName _timer = StringName.op_Implicit("_timer");

		public static readonly StringName _totalPauses = StringName.op_Implicit("_totalPauses");

		public static readonly StringName _canPauseTimer = StringName.op_Implicit("_canPauseTimer");

		public static readonly StringName _intensity = StringName.op_Implicit("_intensity");

		public static readonly StringName _speed = StringName.op_Implicit("_speed");

		public static readonly StringName _duration = StringName.op_Implicit("_duration");

		public static readonly StringName _originalRotation = StringName.op_Implicit("_originalRotation");

		public static readonly StringName _targetScale = StringName.op_Implicit("_targetScale");
	}

	public class SignalName : SignalName
	{
	}

	private const float _pauseDuration = 0.05f;

	private float _elapsedPauseTime;

	private int _pauseCounter;

	private bool _isPaused;

	private float _timer;

	private int _totalPauses;

	private float _canPauseTimer;

	private float _intensity;

	private float _speed;

	private float _duration;

	private float _originalRotation;

	private Vector2 _targetScale;

	public override void _Ready()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		_totalPauses = Rng.Chaotic.NextInt(2, 7);
		_canPauseTimer = Rng.Chaotic.NextFloat(0.5f, 1.2f);
		_speed = Rng.Chaotic.NextFloat(3f, 5f);
		_intensity = Rng.Chaotic.NextFloat(0.1f, 0.3f);
		_originalRotation = ((Control)this).Rotation;
		_targetScale = ((Control)this).Scale;
		AnimateIn();
	}

	private void AnimateIn()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Scale = Vector2.Zero;
		Tween val = ((Node)this).CreateTween().SetParallel(true);
		((CanvasItem)this).Modulate = new Color(((CanvasItem)this).Modulate.R + Rng.Chaotic.NextFloat(-0.2f, 0.2f), ((CanvasItem)this).Modulate.G, ((CanvasItem)this).Modulate.B, 1f);
		val.TweenInterval(Rng.Chaotic.NextDouble(0.0, 0.4));
		val.Chain();
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(_targetScale), Rng.Chaotic.NextDouble(0.4, 0.5)).SetEase((EaseType)1).SetTrans((TransitionType)11);
		val.Chain();
		val.TweenInterval(Rng.Chaotic.NextDouble(0.3, 0.6));
		val.Chain();
		double num = Rng.Chaotic.NextDouble(0.4, 0.6);
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(_targetScale * 0.5f), num).SetEase((EaseType)0).SetTrans((TransitionType)10);
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), num).SetEase((EaseType)1).SetTrans((TransitionType)4);
	}

	public override void _Process(double delta)
	{
		float num = (float)delta;
		_duration += num * _speed;
		_canPauseTimer -= num;
		if (_isPaused)
		{
			_elapsedPauseTime += num;
			if (_elapsedPauseTime >= 0.05f)
			{
				_isPaused = false;
				_elapsedPauseTime = 0f;
				_pauseCounter++;
			}
			((Control)this).Rotation = _originalRotation + Mathf.Sin(_duration) * _intensity * 0.5f;
		}
		else
		{
			((Control)this).Rotation = _originalRotation + Mathf.Sin(_duration) * _intensity;
		}
		_timer += num;
		if (_canPauseTimer < 0f && _pauseCounter < _totalPauses)
		{
			_isPaused = true;
			_timer = 0f;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateIn();
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
		if ((ref method) == MethodName.AnimateIn)
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
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._elapsedPauseTime)
		{
			_elapsedPauseTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pauseCounter)
		{
			_pauseCounter = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isPaused)
		{
			_isPaused = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._timer)
		{
			_timer = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._totalPauses)
		{
			_totalPauses = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._canPauseTimer)
		{
			_canPauseTimer = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._intensity)
		{
			_intensity = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._speed)
		{
			_speed = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._duration)
		{
			_duration = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalRotation)
		{
			_originalRotation = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetScale)
		{
			_targetScale = VariantUtils.ConvertTo<Vector2>(ref value);
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
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._elapsedPauseTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _elapsedPauseTime);
			return true;
		}
		if ((ref name) == PropertyName._pauseCounter)
		{
			value = VariantUtils.CreateFrom<int>(ref _pauseCounter);
			return true;
		}
		if ((ref name) == PropertyName._isPaused)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isPaused);
			return true;
		}
		if ((ref name) == PropertyName._timer)
		{
			value = VariantUtils.CreateFrom<float>(ref _timer);
			return true;
		}
		if ((ref name) == PropertyName._totalPauses)
		{
			value = VariantUtils.CreateFrom<int>(ref _totalPauses);
			return true;
		}
		if ((ref name) == PropertyName._canPauseTimer)
		{
			value = VariantUtils.CreateFrom<float>(ref _canPauseTimer);
			return true;
		}
		if ((ref name) == PropertyName._intensity)
		{
			value = VariantUtils.CreateFrom<float>(ref _intensity);
			return true;
		}
		if ((ref name) == PropertyName._speed)
		{
			value = VariantUtils.CreateFrom<float>(ref _speed);
			return true;
		}
		if ((ref name) == PropertyName._duration)
		{
			value = VariantUtils.CreateFrom<float>(ref _duration);
			return true;
		}
		if ((ref name) == PropertyName._originalRotation)
		{
			value = VariantUtils.CreateFrom<float>(ref _originalRotation);
			return true;
		}
		if ((ref name) == PropertyName._targetScale)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetScale);
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
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._elapsedPauseTime, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._pauseCounter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isPaused, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._timer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._totalPauses, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._canPauseTimer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._intensity, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._speed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._duration, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._originalRotation, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._elapsedPauseTime, Variant.From<float>(ref _elapsedPauseTime));
		info.AddProperty(PropertyName._pauseCounter, Variant.From<int>(ref _pauseCounter));
		info.AddProperty(PropertyName._isPaused, Variant.From<bool>(ref _isPaused));
		info.AddProperty(PropertyName._timer, Variant.From<float>(ref _timer));
		info.AddProperty(PropertyName._totalPauses, Variant.From<int>(ref _totalPauses));
		info.AddProperty(PropertyName._canPauseTimer, Variant.From<float>(ref _canPauseTimer));
		info.AddProperty(PropertyName._intensity, Variant.From<float>(ref _intensity));
		info.AddProperty(PropertyName._speed, Variant.From<float>(ref _speed));
		info.AddProperty(PropertyName._duration, Variant.From<float>(ref _duration));
		info.AddProperty(PropertyName._originalRotation, Variant.From<float>(ref _originalRotation));
		info.AddProperty(PropertyName._targetScale, Variant.From<Vector2>(ref _targetScale));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._elapsedPauseTime, ref val))
		{
			_elapsedPauseTime = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._pauseCounter, ref val2))
		{
			_pauseCounter = ((Variant)(ref val2)).As<int>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._isPaused, ref val3))
		{
			_isPaused = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._timer, ref val4))
		{
			_timer = ((Variant)(ref val4)).As<float>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._totalPauses, ref val5))
		{
			_totalPauses = ((Variant)(ref val5)).As<int>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._canPauseTimer, ref val6))
		{
			_canPauseTimer = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._intensity, ref val7))
		{
			_intensity = ((Variant)(ref val7)).As<float>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._speed, ref val8))
		{
			_speed = ((Variant)(ref val8)).As<float>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._duration, ref val9))
		{
			_duration = ((Variant)(ref val9)).As<float>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalRotation, ref val10))
		{
			_originalRotation = ((Variant)(ref val10)).As<float>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetScale, ref val11))
		{
			_targetScale = ((Variant)(ref val11)).As<Vector2>();
		}
	}
}
