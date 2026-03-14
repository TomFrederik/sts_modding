using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NMechaKnightVfx.cs")]
public class NMechaKnightVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName OnAnimationStart = StringName.op_Implicit("OnAnimationStart");

		public static readonly StringName TurnOnFlameThrower = StringName.op_Implicit("TurnOnFlameThrower");

		public static readonly StringName TurnOffFlameThrower = StringName.op_Implicit("TurnOffFlameThrower");

		public static readonly StringName TurnOnEngine = StringName.op_Implicit("TurnOnEngine");

		public static readonly StringName TurnOffEngine = StringName.op_Implicit("TurnOffEngine");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _flameThrowerParticlesDark = StringName.op_Implicit("_flameThrowerParticlesDark");

		public static readonly StringName _flameThrowerParticlesLight = StringName.op_Implicit("_flameThrowerParticlesLight");

		public static readonly StringName _cinderParticles = StringName.op_Implicit("_cinderParticles");

		public static readonly StringName _glowParticles = StringName.op_Implicit("_glowParticles");

		public static readonly StringName _engineParticles = StringName.op_Implicit("_engineParticles");

		public static readonly StringName _engineParticlesDark = StringName.op_Implicit("_engineParticlesDark");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private GpuParticles2D _flameThrowerParticlesDark;

	private GpuParticles2D _flameThrowerParticlesLight;

	private GpuParticles2D _cinderParticles;

	private GpuParticles2D _glowParticles;

	private GpuParticles2D _engineParticles;

	private GpuParticles2D _engineParticlesDark;

	private Node2D _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_animController.ConnectAnimationStarted(Callable.From<GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject>)OnAnimationStart));
		_flameThrowerParticlesDark = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("FlameParticlesBone/FlameParticlesDark"));
		_flameThrowerParticlesLight = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("FlameParticlesBone/FlameParticlesLight"));
		_cinderParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("FlameParticlesBone/CinderParticles"));
		_glowParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("FlameParticlesBone/GlowParticles"));
		_engineParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("EngineSlot/EngineBone/EngineParticles"));
		_engineParticlesDark = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("EngineSlot/EngineBone/EngineParticlesDark"));
		TurnOffFlameThrower();
		TurnOffEngine();
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		switch (new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName())
		{
		case "flame_start":
			TurnOnFlameThrower();
			break;
		case "flame_end":
			TurnOffFlameThrower();
			break;
		case "engine_start":
			TurnOnEngine();
			break;
		case "engine_stop":
			TurnOffEngine();
			break;
		}
	}

	private void OnAnimationStart(GodotObject spineSprite, GodotObject animationState, GodotObject trackEntry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (new MegaAnimationState(Variant.op_Implicit(animationState)).GetCurrent(0).GetAnimation().GetName() != "attack")
		{
			TurnOffFlameThrower();
		}
		bool flag = new MegaAnimationState(Variant.op_Implicit(animationState)).GetCurrent(0).GetAnimation().GetName() == "hurt";
		((CanvasItem)_flameThrowerParticlesDark).Visible = !flag;
		((CanvasItem)_flameThrowerParticlesLight).Visible = !flag;
		((CanvasItem)_cinderParticles).Visible = !flag;
		((CanvasItem)_glowParticles).Visible = !flag;
	}

	private void TurnOnFlameThrower()
	{
		_flameThrowerParticlesDark.Restart();
		_flameThrowerParticlesLight.Restart();
		_cinderParticles.Restart();
		_glowParticles.Restart();
	}

	private void TurnOffFlameThrower()
	{
		_flameThrowerParticlesDark.Emitting = false;
		_flameThrowerParticlesLight.Emitting = false;
		_cinderParticles.Emitting = false;
		_glowParticles.Emitting = false;
	}

	private void TurnOnEngine()
	{
		_engineParticles.Restart();
		_engineParticlesDark.Restart();
	}

	private void TurnOffEngine()
	{
		_engineParticles.Emitting = false;
		_engineParticlesDark.Emitting = false;
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
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Expected O, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Expected O, but got Unknown
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("spineSprite"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("animationState"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("trackEntry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnFlameThrower, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffFlameThrower, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnEngine, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffEngine, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnAnimationStart && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			OnAnimationStart(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnFlameThrower && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnFlameThrower();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffFlameThrower && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffFlameThrower();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnEngine && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnEngine();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffEngine && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffEngine();
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
		if ((ref method) == MethodName.OnAnimationStart)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnFlameThrower)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffFlameThrower)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnEngine)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffEngine)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._flameThrowerParticlesDark)
		{
			_flameThrowerParticlesDark = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flameThrowerParticlesLight)
		{
			_flameThrowerParticlesLight = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cinderParticles)
		{
			_cinderParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._glowParticles)
		{
			_glowParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._engineParticles)
		{
			_engineParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._engineParticlesDark)
		{
			_engineParticlesDark = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
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
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._flameThrowerParticlesDark)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _flameThrowerParticlesDark);
			return true;
		}
		if ((ref name) == PropertyName._flameThrowerParticlesLight)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _flameThrowerParticlesLight);
			return true;
		}
		if ((ref name) == PropertyName._cinderParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _cinderParticles);
			return true;
		}
		if ((ref name) == PropertyName._glowParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _glowParticles);
			return true;
		}
		if ((ref name) == PropertyName._engineParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _engineParticles);
			return true;
		}
		if ((ref name) == PropertyName._engineParticlesDark)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _engineParticlesDark);
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
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._flameThrowerParticlesDark, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._flameThrowerParticlesLight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cinderParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._glowParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._engineParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._engineParticlesDark, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._flameThrowerParticlesDark, Variant.From<GpuParticles2D>(ref _flameThrowerParticlesDark));
		info.AddProperty(PropertyName._flameThrowerParticlesLight, Variant.From<GpuParticles2D>(ref _flameThrowerParticlesLight));
		info.AddProperty(PropertyName._cinderParticles, Variant.From<GpuParticles2D>(ref _cinderParticles));
		info.AddProperty(PropertyName._glowParticles, Variant.From<GpuParticles2D>(ref _glowParticles));
		info.AddProperty(PropertyName._engineParticles, Variant.From<GpuParticles2D>(ref _engineParticles));
		info.AddProperty(PropertyName._engineParticlesDark, Variant.From<GpuParticles2D>(ref _engineParticlesDark));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._flameThrowerParticlesDark, ref val))
		{
			_flameThrowerParticlesDark = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._flameThrowerParticlesLight, ref val2))
		{
			_flameThrowerParticlesLight = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._cinderParticles, ref val3))
		{
			_cinderParticles = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._glowParticles, ref val4))
		{
			_glowParticles = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._engineParticles, ref val5))
		{
			_engineParticles = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._engineParticlesDark, ref val6))
		{
			_engineParticlesDark = ((Variant)(ref val6)).As<GpuParticles2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val7))
		{
			_parent = ((Variant)(ref val7)).As<Node2D>();
		}
	}
}
