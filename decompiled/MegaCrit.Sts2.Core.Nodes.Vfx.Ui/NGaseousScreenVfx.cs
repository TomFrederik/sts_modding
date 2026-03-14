using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Ui;

[ScriptPath("res://src/Core/Nodes/Vfx/Ui/NGaseousScreenVfx.cs")]
public class NGaseousScreenVfx : AspectRatioContainer
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetProperties = StringName.op_Implicit("SetProperties");

		public static readonly StringName Play = StringName.op_Implicit("Play");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _gfx = StringName.op_Implicit("_gfx");

		public static readonly StringName _duration = StringName.op_Implicit("_duration");

		public static readonly StringName _alphaMultiplierCurve = StringName.op_Implicit("_alphaMultiplierCurve");

		public static readonly StringName _minBaseAlphaCurve = StringName.op_Implicit("_minBaseAlphaCurve");

		public static readonly StringName _erosionCurve = StringName.op_Implicit("_erosionCurve");

		public static readonly StringName _noiseAOffsetCurve = StringName.op_Implicit("_noiseAOffsetCurve");

		public static readonly StringName _noiseBOffsetCurve = StringName.op_Implicit("_noiseBOffsetCurve");

		public static readonly StringName _originalMaterial = StringName.op_Implicit("_originalMaterial");

		public static readonly StringName _materialCopy = StringName.op_Implicit("_materialCopy");

		public static readonly StringName _noiseAOffsetY = StringName.op_Implicit("_noiseAOffsetY");

		public static readonly StringName _noiseBOffsetY = StringName.op_Implicit("_noiseBOffsetY");
	}

	public class SignalName : SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("vfx/ui/vfx_gaseous_screen");

	[Export(/*Could not decode attribute arguments.*/)]
	private ColorRect _gfx;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _duration = 1f;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _alphaMultiplierCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _minBaseAlphaCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _erosionCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _noiseAOffsetCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _noiseBOffsetCurve;

	private Material? _originalMaterial;

	private ShaderMaterial? _materialCopy;

	private float _noiseAOffsetY;

	private float _noiseBOffsetY;

	private static readonly StringName _alphaMultiplierString = new StringName("alpha_multiplier");

	private static readonly StringName _minBaseAlphaString = new StringName("min_base_alpha");

	private static readonly StringName _noiseAOffsetString = new StringName("noise_a_static_offset");

	private static readonly StringName _noiseBOffsetString = new StringName("noise_b_static_offset");

	private static readonly StringName _erosionString = new StringName("erosion_base");

	public static NGaseousScreenVfx? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(scenePath).Instantiate<NGaseousScreenVfx>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		_originalMaterial = ((CanvasItem)_gfx).Material;
		_materialCopy = (ShaderMaterial)((Resource)_originalMaterial).Duplicate(true);
		((CanvasItem)_gfx).Material = (Material)(object)_materialCopy;
		SetProperties(1f);
		Play();
	}

	private void SetProperties(float interpolation)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		float num = _alphaMultiplierCurve.Sample(interpolation);
		float num2 = _minBaseAlphaCurve.Sample(interpolation);
		float num3 = _noiseAOffsetCurve.Sample(interpolation);
		float num4 = _noiseBOffsetCurve.Sample(interpolation);
		float num5 = _erosionCurve.Sample(interpolation);
		_materialCopy.SetShaderParameter(_alphaMultiplierString, Variant.op_Implicit(num));
		_materialCopy.SetShaderParameter(_minBaseAlphaString, Variant.op_Implicit(num2));
		_materialCopy.SetShaderParameter(_noiseAOffsetString, Variant.op_Implicit(new Vector2(num3, _noiseAOffsetY)));
		_materialCopy.SetShaderParameter(_noiseBOffsetString, Variant.op_Implicit(new Vector2(num4, _noiseBOffsetY)));
		_materialCopy.SetShaderParameter(_erosionString, Variant.op_Implicit(num5));
	}

	public void Play()
	{
		TaskHelper.RunSafely(PlaySequence());
	}

	private async Task PlaySequence()
	{
		_noiseAOffsetY = GD.Randf();
		_noiseBOffsetY = GD.Randf();
		double timer = 0.0;
		SetProperties(0f);
		while (timer < (double)_duration)
		{
			float properties = (float)(timer / (double)_duration);
			SetProperties(properties);
			timer += ((Node)this).GetProcessDeltaTime();
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		SetProperties(1f);
		((Node)(object)this).QueueFreeSafely();
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
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("AspectRatioContainer"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetProperties, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("interpolation"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Play, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NGaseousScreenVfx nGaseousScreenVfx = Create();
			ret = VariantUtils.CreateFrom<NGaseousScreenVfx>(ref nGaseousScreenVfx);
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
		if ((ref method) == MethodName.Play && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Play();
			ret = default(godot_variant);
			return true;
		}
		return ((AspectRatioContainer)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NGaseousScreenVfx nGaseousScreenVfx = Create();
			ret = VariantUtils.CreateFrom<NGaseousScreenVfx>(ref nGaseousScreenVfx);
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
		if ((ref method) == MethodName.Play)
		{
			return true;
		}
		return ((AspectRatioContainer)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._gfx)
		{
			_gfx = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
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
		if ((ref name) == PropertyName._minBaseAlphaCurve)
		{
			_minBaseAlphaCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._erosionCurve)
		{
			_erosionCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noiseAOffsetCurve)
		{
			_noiseAOffsetCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noiseBOffsetCurve)
		{
			_noiseBOffsetCurve = VariantUtils.ConvertTo<Curve>(ref value);
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
		if ((ref name) == PropertyName._noiseAOffsetY)
		{
			_noiseAOffsetY = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noiseBOffsetY)
		{
			_noiseBOffsetY = VariantUtils.ConvertTo<float>(ref value);
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
		if ((ref name) == PropertyName._gfx)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _gfx);
			return true;
		}
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
		if ((ref name) == PropertyName._minBaseAlphaCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _minBaseAlphaCurve);
			return true;
		}
		if ((ref name) == PropertyName._erosionCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _erosionCurve);
			return true;
		}
		if ((ref name) == PropertyName._noiseAOffsetCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _noiseAOffsetCurve);
			return true;
		}
		if ((ref name) == PropertyName._noiseBOffsetCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _noiseBOffsetCurve);
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
		if ((ref name) == PropertyName._noiseAOffsetY)
		{
			value = VariantUtils.CreateFrom<float>(ref _noiseAOffsetY);
			return true;
		}
		if ((ref name) == PropertyName._noiseBOffsetY)
		{
			value = VariantUtils.CreateFrom<float>(ref _noiseBOffsetY);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._gfx, (PropertyHint)34, "ColorRect", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._duration, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._alphaMultiplierCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._minBaseAlphaCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._erosionCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._noiseAOffsetCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._noiseBOffsetCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._originalMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._materialCopy, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._noiseAOffsetY, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._noiseBOffsetY, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._gfx, Variant.From<ColorRect>(ref _gfx));
		info.AddProperty(PropertyName._duration, Variant.From<float>(ref _duration));
		info.AddProperty(PropertyName._alphaMultiplierCurve, Variant.From<Curve>(ref _alphaMultiplierCurve));
		info.AddProperty(PropertyName._minBaseAlphaCurve, Variant.From<Curve>(ref _minBaseAlphaCurve));
		info.AddProperty(PropertyName._erosionCurve, Variant.From<Curve>(ref _erosionCurve));
		info.AddProperty(PropertyName._noiseAOffsetCurve, Variant.From<Curve>(ref _noiseAOffsetCurve));
		info.AddProperty(PropertyName._noiseBOffsetCurve, Variant.From<Curve>(ref _noiseBOffsetCurve));
		info.AddProperty(PropertyName._originalMaterial, Variant.From<Material>(ref _originalMaterial));
		info.AddProperty(PropertyName._materialCopy, Variant.From<ShaderMaterial>(ref _materialCopy));
		info.AddProperty(PropertyName._noiseAOffsetY, Variant.From<float>(ref _noiseAOffsetY));
		info.AddProperty(PropertyName._noiseBOffsetY, Variant.From<float>(ref _noiseBOffsetY));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._gfx, ref val))
		{
			_gfx = ((Variant)(ref val)).As<ColorRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._duration, ref val2))
		{
			_duration = ((Variant)(ref val2)).As<float>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._alphaMultiplierCurve, ref val3))
		{
			_alphaMultiplierCurve = ((Variant)(ref val3)).As<Curve>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._minBaseAlphaCurve, ref val4))
		{
			_minBaseAlphaCurve = ((Variant)(ref val4)).As<Curve>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._erosionCurve, ref val5))
		{
			_erosionCurve = ((Variant)(ref val5)).As<Curve>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._noiseAOffsetCurve, ref val6))
		{
			_noiseAOffsetCurve = ((Variant)(ref val6)).As<Curve>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._noiseBOffsetCurve, ref val7))
		{
			_noiseBOffsetCurve = ((Variant)(ref val7)).As<Curve>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalMaterial, ref val8))
		{
			_originalMaterial = ((Variant)(ref val8)).As<Material>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._materialCopy, ref val9))
		{
			_materialCopy = ((Variant)(ref val9)).As<ShaderMaterial>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._noiseAOffsetY, ref val10))
		{
			_noiseAOffsetY = ((Variant)(ref val10)).As<float>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._noiseBOffsetY, ref val11))
		{
			_noiseBOffsetY = ((Variant)(ref val11)).As<float>();
		}
	}
}
