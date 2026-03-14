using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NMirrorVfx.cs")]
public class NMirrorVfx : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _mask1 = StringName.op_Implicit("_mask1");

		public static readonly StringName _reflection1 = StringName.op_Implicit("_reflection1");

		public static readonly StringName _mask2 = StringName.op_Implicit("_mask2");

		public static readonly StringName _reflection2 = StringName.op_Implicit("_reflection2");

		public static readonly StringName _mask3 = StringName.op_Implicit("_mask3");

		public static readonly StringName _reflection3 = StringName.op_Implicit("_reflection3");

		public static readonly StringName _noise = StringName.op_Implicit("_noise");

		public static readonly StringName _totalTime = StringName.op_Implicit("_totalTime");
	}

	public class SignalName : SignalName
	{
	}

	private Sprite2D _mask1;

	private Control _reflection1;

	private Sprite2D _mask2;

	private Control _reflection2;

	private Sprite2D _mask3;

	private Control _reflection3;

	private FastNoiseLite _noise = new FastNoiseLite();

	private float _totalTime;

	private const float _noiseSpeed = 2f;

	private static string ScenePath => SceneHelper.GetScenePath("vfx/whole_screen/mirror_vfx");

	public static NMirrorVfx? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(ScenePath).Instantiate<NMirrorVfx>((GenEditState)0);
	}

	public override void _Ready()
	{
		_mask1 = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("Mask1"));
		_reflection1 = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Mask1/Reflection"));
		_mask2 = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("Mask2"));
		_reflection2 = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Mask2/Reflection"));
		_mask3 = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("Mask3"));
		_reflection3 = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Mask3/Reflection"));
	}

	public override void _Process(double delta)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		_totalTime += (float)delta * 2f;
		_noise.Seed = 0;
		float num = 1.05f + ((Noise)_noise).GetNoise1D(_totalTime);
		((Node2D)_mask1).Scale = new Vector2(num, num);
		_reflection1.Scale = ((Node2D)_mask1).Scale;
		_noise.Seed = 1;
		num = ((Noise)_noise).GetNoise1D(_totalTime);
		((Node2D)_mask1).RotationDegrees = Mathf.Abs(num) * 10f;
		_noise.Seed = 2;
		num = 1.05f + ((Noise)_noise).GetNoise1D(_totalTime);
		((Node2D)_mask2).Scale = new Vector2(num, num);
		_reflection2.Scale = new Vector2(num, num);
		_noise.Seed = 3;
		num = ((Noise)_noise).GetNoise1D(_totalTime);
		((Node2D)_mask2).RotationDegrees = Mathf.Abs(num) * 20f;
		_noise.Seed = 4;
		num = 1.05f + ((Noise)_noise).GetNoise1D(_totalTime);
		((Node2D)_mask3).Scale = new Vector2(num, num);
		_reflection3.Scale = new Vector2(num, num);
		_noise.Seed = 5;
		num = ((Noise)_noise).GetNoise1D(_totalTime);
		((Node2D)_mask3).RotationDegrees = Mathf.Abs(num) * 30f;
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
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NMirrorVfx nMirrorVfx = Create();
			ret = VariantUtils.CreateFrom<NMirrorVfx>(ref nMirrorVfx);
			return true;
		}
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
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NMirrorVfx nMirrorVfx = Create();
			ret = VariantUtils.CreateFrom<NMirrorVfx>(ref nMirrorVfx);
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
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._mask1)
		{
			_mask1 = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._reflection1)
		{
			_reflection1 = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mask2)
		{
			_mask2 = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._reflection2)
		{
			_reflection2 = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mask3)
		{
			_mask3 = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._reflection3)
		{
			_reflection3 = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noise)
		{
			_noise = VariantUtils.ConvertTo<FastNoiseLite>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._totalTime)
		{
			_totalTime = VariantUtils.ConvertTo<float>(ref value);
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
		if ((ref name) == PropertyName._mask1)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _mask1);
			return true;
		}
		if ((ref name) == PropertyName._reflection1)
		{
			value = VariantUtils.CreateFrom<Control>(ref _reflection1);
			return true;
		}
		if ((ref name) == PropertyName._mask2)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _mask2);
			return true;
		}
		if ((ref name) == PropertyName._reflection2)
		{
			value = VariantUtils.CreateFrom<Control>(ref _reflection2);
			return true;
		}
		if ((ref name) == PropertyName._mask3)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _mask3);
			return true;
		}
		if ((ref name) == PropertyName._reflection3)
		{
			value = VariantUtils.CreateFrom<Control>(ref _reflection3);
			return true;
		}
		if ((ref name) == PropertyName._noise)
		{
			value = VariantUtils.CreateFrom<FastNoiseLite>(ref _noise);
			return true;
		}
		if ((ref name) == PropertyName._totalTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _totalTime);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._mask1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._reflection1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mask2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._reflection2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mask3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._reflection3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._noise, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._totalTime, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._mask1, Variant.From<Sprite2D>(ref _mask1));
		info.AddProperty(PropertyName._reflection1, Variant.From<Control>(ref _reflection1));
		info.AddProperty(PropertyName._mask2, Variant.From<Sprite2D>(ref _mask2));
		info.AddProperty(PropertyName._reflection2, Variant.From<Control>(ref _reflection2));
		info.AddProperty(PropertyName._mask3, Variant.From<Sprite2D>(ref _mask3));
		info.AddProperty(PropertyName._reflection3, Variant.From<Control>(ref _reflection3));
		info.AddProperty(PropertyName._noise, Variant.From<FastNoiseLite>(ref _noise));
		info.AddProperty(PropertyName._totalTime, Variant.From<float>(ref _totalTime));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._mask1, ref val))
		{
			_mask1 = ((Variant)(ref val)).As<Sprite2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._reflection1, ref val2))
		{
			_reflection1 = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._mask2, ref val3))
		{
			_mask2 = ((Variant)(ref val3)).As<Sprite2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._reflection2, ref val4))
		{
			_reflection2 = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._mask3, ref val5))
		{
			_mask3 = ((Variant)(ref val5)).As<Sprite2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._reflection3, ref val6))
		{
			_reflection3 = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._noise, ref val7))
		{
			_noise = ((Variant)(ref val7)).As<FastNoiseLite>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._totalTime, ref val8))
		{
			_totalTime = ((Variant)(ref val8)).As<float>();
		}
	}
}
