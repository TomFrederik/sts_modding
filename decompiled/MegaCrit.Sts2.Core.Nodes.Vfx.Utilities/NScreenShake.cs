using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

[ScriptPath("res://src/Core/Nodes/Vfx/Utilities/NScreenShake.cs")]
public class NScreenShake : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetTarget = StringName.op_Implicit("SetTarget");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName Shake = StringName.op_Implicit("Shake");

		public static readonly StringName Rumble = StringName.op_Implicit("Rumble");

		public static readonly StringName AddTrauma = StringName.op_Implicit("AddTrauma");

		public static readonly StringName ClearTarget = StringName.op_Implicit("ClearTarget");

		public static readonly StringName StopRumble = StringName.op_Implicit("StopRumble");

		public static readonly StringName SetMultiplier = StringName.op_Implicit("SetMultiplier");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _shakeTarget = StringName.op_Implicit("_shakeTarget");

		public static readonly StringName _originalTargetPosition = StringName.op_Implicit("_originalTargetPosition");

		public static readonly StringName _multiplier = StringName.op_Implicit("_multiplier");
	}

	public class SignalName : SignalName
	{
	}

	private Control? _shakeTarget;

	private Vector2 _originalTargetPosition;

	private ScreenPunchInstance? _shakeInstance;

	private ScreenRumbleInstance? _rumbleInstance;

	private ScreenTraumaRumble _traumaRumble;

	private float _multiplier;

	private readonly Dictionary<ShakeStrength, float> _strength = new Dictionary<ShakeStrength, float>();

	private readonly Dictionary<ShakeDuration, double> _duration = new Dictionary<ShakeDuration, double>();

	public override void _Ready()
	{
		_traumaRumble = new ScreenTraumaRumble();
		_strength.Add(ShakeStrength.VeryWeak, 2f);
		_strength.Add(ShakeStrength.Weak, 5f);
		_strength.Add(ShakeStrength.Medium, 20f);
		_strength.Add(ShakeStrength.Strong, 40f);
		_strength.Add(ShakeStrength.TooMuch, 80f);
		_duration.Add(ShakeDuration.Short, 0.3);
		_duration.Add(ShakeDuration.Normal, 0.8);
		_duration.Add(ShakeDuration.Long, 1.2);
		_duration.Add(ShakeDuration.Forever, 999999.0);
	}

	public void SetTarget(Control targetScreen)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		_shakeTarget = targetScreen;
		_originalTargetPosition = targetScreen.Position;
	}

	public override void _Process(double delta)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Vector2.Zero;
		if (_rumbleInstance != null)
		{
			val = _rumbleInstance.Update(delta);
			if (_rumbleInstance.IsDone)
			{
				_rumbleInstance = null;
			}
		}
		if (_shakeInstance != null)
		{
			val = _shakeInstance.Update(delta);
			if (_shakeInstance.IsDone)
			{
				_shakeInstance = null;
			}
		}
		val += _traumaRumble.Update(delta);
		if (_shakeTarget != null && ((Node?)(object)_shakeTarget).IsValid())
		{
			_shakeTarget.Position = _originalTargetPosition + val;
		}
	}

	public void Shake(ShakeStrength strength, ShakeDuration duration, float degAngle)
	{
		if (_shakeTarget == null)
		{
			Log.Error("Missing screenShake target!");
		}
		else
		{
			_shakeInstance = new ScreenPunchInstance(_strength[strength] * _multiplier, _duration[duration], degAngle);
		}
	}

	public void Rumble(ShakeStrength strength, ShakeDuration duration, RumbleStyle style)
	{
		if (_shakeTarget == null)
		{
			Log.Error("Missing screenShake target!");
		}
		else
		{
			_rumbleInstance = new ScreenRumbleInstance(_strength[strength] * _multiplier, _duration[duration], 1f, style);
		}
	}

	public void AddTrauma(ShakeStrength strength)
	{
		_traumaRumble.AddTrauma(strength);
	}

	public void ClearTarget()
	{
		_shakeTarget = null;
		_shakeInstance = null;
		_rumbleInstance = null;
	}

	private void StopRumble()
	{
		_rumbleInstance = null;
	}

	public void SetMultiplier(float multiplier)
	{
		_multiplier = multiplier;
		_traumaRumble.SetMultiplier(multiplier);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTarget, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("targetScreen"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Shake, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("strength"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("degAngle"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Rumble, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("strength"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("style"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddTrauma, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("strength"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearTarget, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopRumble, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetMultiplier, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("multiplier"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetTarget && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTarget(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Shake && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			Shake(VariantUtils.ConvertTo<ShakeStrength>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<ShakeDuration>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Rumble && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			Rumble(VariantUtils.ConvertTo<ShakeStrength>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<ShakeDuration>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<RumbleStyle>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddTrauma && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AddTrauma(VariantUtils.ConvertTo<ShakeStrength>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearTarget && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearTarget();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopRumble && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopRumble();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetMultiplier && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetMultiplier(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTarget)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.Shake)
		{
			return true;
		}
		if ((ref method) == MethodName.Rumble)
		{
			return true;
		}
		if ((ref method) == MethodName.AddTrauma)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearTarget)
		{
			return true;
		}
		if ((ref method) == MethodName.StopRumble)
		{
			return true;
		}
		if ((ref method) == MethodName.SetMultiplier)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._shakeTarget)
		{
			_shakeTarget = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalTargetPosition)
		{
			_originalTargetPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._multiplier)
		{
			_multiplier = VariantUtils.ConvertTo<float>(ref value);
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
		if ((ref name) == PropertyName._shakeTarget)
		{
			value = VariantUtils.CreateFrom<Control>(ref _shakeTarget);
			return true;
		}
		if ((ref name) == PropertyName._originalTargetPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _originalTargetPosition);
			return true;
		}
		if ((ref name) == PropertyName._multiplier)
		{
			value = VariantUtils.CreateFrom<float>(ref _multiplier);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._shakeTarget, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._originalTargetPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._multiplier, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._shakeTarget, Variant.From<Control>(ref _shakeTarget));
		info.AddProperty(PropertyName._originalTargetPosition, Variant.From<Vector2>(ref _originalTargetPosition));
		info.AddProperty(PropertyName._multiplier, Variant.From<float>(ref _multiplier));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._shakeTarget, ref val))
		{
			_shakeTarget = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalTargetPosition, ref val2))
		{
			_originalTargetPosition = ((Variant)(ref val2)).As<Vector2>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._multiplier, ref val3))
		{
			_multiplier = ((Variant)(ref val3)).As<float>();
		}
	}
}
