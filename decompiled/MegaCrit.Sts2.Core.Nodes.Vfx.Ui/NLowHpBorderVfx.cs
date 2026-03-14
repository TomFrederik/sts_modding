using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Ui;

[ScriptPath("res://src/Core/Nodes/Vfx/Ui/NLowHpBorderVfx.cs")]
public class NLowHpBorderVfx : ColorRect
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetProperties = StringName.op_Implicit("SetProperties");

		public static readonly StringName RandomizeInitialOffset = StringName.op_Implicit("RandomizeInitialOffset");

		public static readonly StringName Play = StringName.op_Implicit("Play");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _duration = StringName.op_Implicit("_duration");

		public static readonly StringName _alphaMultiplierCurve = StringName.op_Implicit("_alphaMultiplierCurve");

		public static readonly StringName _noiseOffsetCurve = StringName.op_Implicit("_noiseOffsetCurve");

		public static readonly StringName _gradient = StringName.op_Implicit("_gradient");

		public static readonly StringName _originalMaterial = StringName.op_Implicit("_originalMaterial");

		public static readonly StringName _materialCopy = StringName.op_Implicit("_materialCopy");

		public static readonly StringName _isPlaying = StringName.op_Implicit("_isPlaying");

		public static readonly StringName _currentTimer = StringName.op_Implicit("_currentTimer");
	}

	public class SignalName : SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("vfx/ui/vfx_low_hp_border");

	[Export(/*Could not decode attribute arguments.*/)]
	private float _duration = 1f;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _alphaMultiplierCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _noiseOffsetCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Gradient? _gradient;

	private Material? _originalMaterial;

	private ShaderMaterial? _materialCopy;

	private bool _isPlaying;

	private double _currentTimer;

	private static readonly StringName _alphaMultiplierString = new StringName("alpha_multiplier");

	private static readonly StringName _noiseInitialOffsetString = new StringName("noise_initial_offset");

	private static readonly StringName _mainColorString = new StringName("main_color");

	public static NLowHpBorderVfx Create()
	{
		return PreloadManager.Cache.GetScene(scenePath).Instantiate<NLowHpBorderVfx>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		_isPlaying = false;
		_originalMaterial = ((CanvasItem)this).Material;
		_materialCopy = (ShaderMaterial)((Resource)_originalMaterial).Duplicate(true);
		((CanvasItem)this).Material = (Material)(object)_materialCopy;
		SetProperties(1f);
	}

	private void SetProperties(float interpolation)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		float num = _alphaMultiplierCurve.Sample(interpolation);
		Color val = _gradient.Sample(interpolation);
		_materialCopy.SetShaderParameter(_alphaMultiplierString, Variant.op_Implicit(num));
		_materialCopy.SetShaderParameter(_mainColorString, Variant.op_Implicit(val));
	}

	private void RandomizeInitialOffset()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		_materialCopy.SetShaderParameter(_noiseInitialOffsetString, Variant.op_Implicit(new Vector2(GD.Randf(), GD.Randf())));
	}

	public void Play()
	{
		if (_isPlaying)
		{
			_currentTimer = 0.0;
		}
		else
		{
			TaskHelper.RunSafely(PlaySequence());
		}
	}

	private async Task PlaySequence()
	{
		_isPlaying = true;
		_currentTimer = 0.0;
		RandomizeInitialOffset();
		SetProperties(0f);
		while (_currentTimer < (double)_duration)
		{
			float properties = (float)(_currentTimer / (double)_duration);
			SetProperties(properties);
			_currentTimer += ((Node)this).GetProcessDeltaTime();
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		SetProperties(1f);
		_isPlaying = false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("ColorRect"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetProperties, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("interpolation"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RandomizeInitialOffset, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Play, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NLowHpBorderVfx nLowHpBorderVfx = Create();
			ret = VariantUtils.CreateFrom<NLowHpBorderVfx>(ref nLowHpBorderVfx);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetProperties && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetProperties(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RandomizeInitialOffset && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RandomizeInitialOffset();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Play && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Play();
			ret = default(godot_variant);
			return true;
		}
		return ((ColorRect)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NLowHpBorderVfx nLowHpBorderVfx = Create();
			ret = VariantUtils.CreateFrom<NLowHpBorderVfx>(ref nLowHpBorderVfx);
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
		if ((ref method) == MethodName.SetProperties)
		{
			return true;
		}
		if ((ref method) == MethodName.RandomizeInitialOffset)
		{
			return true;
		}
		if ((ref method) == MethodName.Play)
		{
			return true;
		}
		return ((ColorRect)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._duration)
		{
			_duration = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._alphaMultiplierCurve)
		{
			_alphaMultiplierCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noiseOffsetCurve)
		{
			_noiseOffsetCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._gradient)
		{
			_gradient = VariantUtils.ConvertTo<Gradient>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalMaterial)
		{
			_originalMaterial = VariantUtils.ConvertTo<Material>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._materialCopy)
		{
			_materialCopy = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isPlaying)
		{
			_isPlaying = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentTimer)
		{
			_currentTimer = VariantUtils.ConvertTo<double>(ref value);
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
		if ((ref name) == PropertyName._duration)
		{
			value = VariantUtils.CreateFrom<float>(ref _duration);
			return true;
		}
		if ((ref name) == PropertyName._alphaMultiplierCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _alphaMultiplierCurve);
			return true;
		}
		if ((ref name) == PropertyName._noiseOffsetCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _noiseOffsetCurve);
			return true;
		}
		if ((ref name) == PropertyName._gradient)
		{
			value = VariantUtils.CreateFrom<Gradient>(ref _gradient);
			return true;
		}
		if ((ref name) == PropertyName._originalMaterial)
		{
			value = VariantUtils.CreateFrom<Material>(ref _originalMaterial);
			return true;
		}
		if ((ref name) == PropertyName._materialCopy)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _materialCopy);
			return true;
		}
		if ((ref name) == PropertyName._isPlaying)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isPlaying);
			return true;
		}
		if ((ref name) == PropertyName._currentTimer)
		{
			value = VariantUtils.CreateFrom<double>(ref _currentTimer);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._duration, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._alphaMultiplierCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._noiseOffsetCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._gradient, (PropertyHint)17, "Gradient", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._originalMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._materialCopy, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isPlaying, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._currentTimer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._duration, Variant.From<float>(ref _duration));
		info.AddProperty(PropertyName._alphaMultiplierCurve, Variant.From<Curve>(ref _alphaMultiplierCurve));
		info.AddProperty(PropertyName._noiseOffsetCurve, Variant.From<Curve>(ref _noiseOffsetCurve));
		info.AddProperty(PropertyName._gradient, Variant.From<Gradient>(ref _gradient));
		info.AddProperty(PropertyName._originalMaterial, Variant.From<Material>(ref _originalMaterial));
		info.AddProperty(PropertyName._materialCopy, Variant.From<ShaderMaterial>(ref _materialCopy));
		info.AddProperty(PropertyName._isPlaying, Variant.From<bool>(ref _isPlaying));
		info.AddProperty(PropertyName._currentTimer, Variant.From<double>(ref _currentTimer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._duration, ref val))
		{
			_duration = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._alphaMultiplierCurve, ref val2))
		{
			_alphaMultiplierCurve = ((Variant)(ref val2)).As<Curve>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._noiseOffsetCurve, ref val3))
		{
			_noiseOffsetCurve = ((Variant)(ref val3)).As<Curve>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._gradient, ref val4))
		{
			_gradient = ((Variant)(ref val4)).As<Gradient>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalMaterial, ref val5))
		{
			_originalMaterial = ((Variant)(ref val5)).As<Material>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._materialCopy, ref val6))
		{
			_materialCopy = ((Variant)(ref val6)).As<ShaderMaterial>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isPlaying, ref val7))
		{
			_isPlaying = ((Variant)(ref val7)).As<bool>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentTimer, ref val8))
		{
			_currentTimer = ((Variant)(ref val8)).As<double>();
		}
	}
}
