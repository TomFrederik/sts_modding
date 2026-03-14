using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NVantomVfx.cs")]
public class NVantomVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName DissolveTail = StringName.op_Implicit("DissolveTail");

		public static readonly StringName StartSpray = StringName.op_Implicit("StartSpray");

		public static readonly StringName EndSpray = StringName.op_Implicit("EndSpray");

		public static readonly StringName StartDeathSpray = StringName.op_Implicit("StartDeathSpray");

		public static readonly StringName EndDeathSpray = StringName.op_Implicit("EndDeathSpray");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _tailShaderMat = StringName.op_Implicit("_tailShaderMat");

		public static readonly StringName _sprayParticles = StringName.op_Implicit("_sprayParticles");

		public static readonly StringName _deathSprayParticles = StringName.op_Implicit("_deathSprayParticles");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _step = new StringName("step");

	private ShaderMaterial? _tailShaderMat;

	private GpuParticles2D _sprayParticles;

	private GpuParticles2D _deathSprayParticles;

	private Node2D _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		Material? normalMaterial = new MegaSlotNode(Variant.op_Implicit((GodotObject)(object)((Node)_parent).GetNode(NodePath.op_Implicit("TailSlotNode")))).GetNormalMaterial();
		_tailShaderMat = (ShaderMaterial?)(object)((normalMaterial is ShaderMaterial) ? normalMaterial : null);
		_sprayParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SprayBoneNode/SprayParticles"));
		_deathSprayParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("DeathSpraySlotNode/DeathSprayParticles"));
		ShaderMaterial? tailShaderMat = _tailShaderMat;
		if (tailShaderMat != null)
		{
			tailShaderMat.SetShaderParameter(_step, Variant.op_Implicit(-0.1f));
		}
		_animController.GetAnimationState().SetAnimation("idle_loop");
		_animController.GetAnimationState().SetAnimation("_tracks/charged_0", loop: true, 1);
		_sprayParticles.Emitting = false;
		_deathSprayParticles.Emitting = false;
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		switch (new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName())
		{
		case "dissolve_tail":
			DissolveTail();
			break;
		case "spray_on":
			StartSpray();
			break;
		case "spray_off":
			EndSpray();
			break;
		case "death_spray_on":
			StartDeathSpray();
			break;
		case "death_spray_off":
			EndDeathSpray();
			break;
		}
	}

	private void DissolveTail()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (_tailShaderMat != null)
		{
			Tween val = ((Node)this).CreateTween();
			val.SetEase((EaseType)0);
			val.SetTrans((TransitionType)4);
			val.TweenProperty((GodotObject)(object)_tailShaderMat, NodePath.op_Implicit("shader_parameter/step"), Variant.op_Implicit(1f), 1.0);
			val.TweenCallback(Callable.From((Action)delegate
			{
				_animController.GetAnimationState().SetAnimation("_tracks/charge_up_1", loop: false, 1);
				_animController.GetAnimationState().AddAnimation("_tracks/charged_1", 0f, loop: true, 1);
			}));
		}
	}

	private void StartSpray()
	{
		_sprayParticles.Emitting = true;
	}

	private void EndSpray()
	{
		_sprayParticles.Emitting = false;
	}

	private void StartDeathSpray()
	{
		_deathSprayParticles.Emitting = true;
	}

	private void EndDeathSpray()
	{
		_deathSprayParticles.Emitting = false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DissolveTail, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartSpray, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndSpray, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartDeathSpray, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndDeathSpray, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAnimationEvent && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			OnAnimationEvent(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DissolveTail && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DissolveTail();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartSpray && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartSpray();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndSpray && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndSpray();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartDeathSpray && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartDeathSpray();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndDeathSpray && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndDeathSpray();
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
		if ((ref method) == MethodName.OnAnimationEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.DissolveTail)
		{
			return true;
		}
		if ((ref method) == MethodName.StartSpray)
		{
			return true;
		}
		if ((ref method) == MethodName.EndSpray)
		{
			return true;
		}
		if ((ref method) == MethodName.StartDeathSpray)
		{
			return true;
		}
		if ((ref method) == MethodName.EndDeathSpray)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._tailShaderMat)
		{
			_tailShaderMat = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sprayParticles)
		{
			_sprayParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deathSprayParticles)
		{
			_deathSprayParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			_parent = VariantUtils.ConvertTo<Node2D>(ref value);
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
		if ((ref name) == PropertyName._tailShaderMat)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _tailShaderMat);
			return true;
		}
		if ((ref name) == PropertyName._sprayParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _sprayParticles);
			return true;
		}
		if ((ref name) == PropertyName._deathSprayParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _deathSprayParticles);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _parent);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._tailShaderMat, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sprayParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._deathSprayParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._tailShaderMat, Variant.From<ShaderMaterial>(ref _tailShaderMat));
		info.AddProperty(PropertyName._sprayParticles, Variant.From<GpuParticles2D>(ref _sprayParticles));
		info.AddProperty(PropertyName._deathSprayParticles, Variant.From<GpuParticles2D>(ref _deathSprayParticles));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._tailShaderMat, ref val))
		{
			_tailShaderMat = ((Variant)(ref val)).As<ShaderMaterial>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._sprayParticles, ref val2))
		{
			_sprayParticles = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._deathSprayParticles, ref val3))
		{
			_deathSprayParticles = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val4))
		{
			_parent = ((Variant)(ref val4)).As<Node2D>();
		}
	}
}
