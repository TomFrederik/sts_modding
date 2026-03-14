using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NOilSpillVfx.cs")]
public class NOilSpillVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName OnAnimationStart = StringName.op_Implicit("OnAnimationStart");

		public static readonly StringName TurnOnSprayAttack = StringName.op_Implicit("TurnOnSprayAttack");

		public static readonly StringName TurnOffSprayAttack = StringName.op_Implicit("TurnOffSprayAttack");

		public static readonly StringName TurnOnSlamSpray = StringName.op_Implicit("TurnOnSlamSpray");

		public static readonly StringName TurnOffSlamSpray = StringName.op_Implicit("TurnOffSlamSpray");

		public static readonly StringName TurnOnDeathSpray = StringName.op_Implicit("TurnOnDeathSpray");

		public static readonly StringName TurnOffDeathSpray = StringName.op_Implicit("TurnOffDeathSpray");

		public static readonly StringName TurnOnDrool = StringName.op_Implicit("TurnOnDrool");

		public static readonly StringName TurnOffDrool = StringName.op_Implicit("TurnOffDrool");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _droolParticles = StringName.op_Implicit("_droolParticles");

		public static readonly StringName _sprayParticles = StringName.op_Implicit("_sprayParticles");

		public static readonly StringName _rainDropParticles = StringName.op_Implicit("_rainDropParticles");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private const int _slamSprayAmount = 800;

	private const int _sprayAttackAmount = 2000;

	private const int _deathSprayAmount = 500;

	private const float _slamSprayLifetime = 0.75f;

	private GpuParticles2D _droolParticles;

	private GpuParticles2D _sprayParticles;

	private GpuParticles2D _rainDropParticles;

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
		_droolParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("MouthDribbleBoneNode/DribbleParticles"));
		_sprayParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("MouthSpraySlot/SprayParticles"));
		_rainDropParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("MouthSpraySlot/RainDropParticles"));
		_rainDropParticles.OneShot = true;
		_droolParticles.Restart();
		_rainDropParticles.Restart();
		_sprayParticles.Restart();
		TurnOffSprayAttack();
		TurnOffSlamSpray();
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		string eventName = new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName();
		if (eventName == null)
		{
			return;
		}
		switch (eventName.Length)
		{
		case 11:
			switch (eventName[0])
			{
			case 's':
				if (eventName == "spray_start")
				{
					TurnOnSprayAttack();
				}
				break;
			case 'd':
				if (eventName == "drool_start")
				{
					TurnOnDrool();
				}
				break;
			}
			break;
		case 9:
			switch (eventName[0])
			{
			case 's':
				if (eventName == "spray_end")
				{
					TurnOffSprayAttack();
				}
				break;
			case 'd':
				if (eventName == "drool_end")
				{
					TurnOffDrool();
				}
				break;
			}
			break;
		case 16:
			if (eventName == "slam_spray_start")
			{
				TurnOnSlamSpray();
			}
			break;
		case 14:
			if (eventName == "slam_spray_end")
			{
				TurnOffSlamSpray();
			}
			break;
		case 17:
			if (eventName == "death_spray_start")
			{
				TurnOnDeathSpray();
			}
			break;
		case 15:
			if (eventName == "death_spray_end")
			{
				TurnOffDeathSpray();
			}
			break;
		case 10:
		case 12:
		case 13:
			break;
		}
	}

	private void OnAnimationStart(GodotObject spineSprite, GodotObject animationState, GodotObject trackEntry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		string name = new MegaAnimationState(Variant.op_Implicit(animationState)).GetCurrent(0).GetAnimation().GetName();
		if (name != "slam")
		{
			TurnOffSprayAttack();
		}
		if (name != "spray")
		{
			TurnOffSlamSpray();
		}
	}

	private void TurnOnSprayAttack()
	{
		_sprayParticles.Amount = 2000;
		_sprayParticles.Emitting = true;
	}

	private void TurnOffSprayAttack()
	{
		_sprayParticles.Emitting = false;
	}

	private void TurnOnSlamSpray()
	{
		_rainDropParticles.OneShot = false;
		_rainDropParticles.Amount = 800;
		_rainDropParticles.Explosiveness = 0f;
		_rainDropParticles.Lifetime = 0.75;
		_rainDropParticles.Restart();
	}

	private void TurnOffSlamSpray()
	{
		_rainDropParticles.Emitting = false;
	}

	private void TurnOnDeathSpray()
	{
		_sprayParticles.Amount = 500;
		_sprayParticles.Emitting = true;
	}

	private void TurnOffDeathSpray()
	{
		_sprayParticles.Emitting = false;
	}

	private void TurnOnDrool()
	{
		_droolParticles.Emitting = true;
	}

	private void TurnOffDrool()
	{
		_droolParticles.Emitting = false;
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
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
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
		list.Add(new MethodInfo(MethodName.TurnOnSprayAttack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffSprayAttack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnSlamSpray, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffSlamSpray, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnDeathSpray, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffDeathSpray, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnDrool, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffDrool, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.TurnOnSprayAttack && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnSprayAttack();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffSprayAttack && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffSprayAttack();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnSlamSpray && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnSlamSpray();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffSlamSpray && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffSlamSpray();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnDeathSpray && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnDeathSpray();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffDeathSpray && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffDeathSpray();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnDrool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnDrool();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffDrool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffDrool();
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
		if ((ref method) == MethodName.TurnOnSprayAttack)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffSprayAttack)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnSlamSpray)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffSlamSpray)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnDeathSpray)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffDeathSpray)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnDrool)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffDrool)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._droolParticles)
		{
			_droolParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sprayParticles)
		{
			_sprayParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rainDropParticles)
		{
			_rainDropParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
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
		if ((ref name) == PropertyName._droolParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _droolParticles);
			return true;
		}
		if ((ref name) == PropertyName._sprayParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _sprayParticles);
			return true;
		}
		if ((ref name) == PropertyName._rainDropParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _rainDropParticles);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._droolParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sprayParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rainDropParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._droolParticles, Variant.From<GpuParticles2D>(ref _droolParticles));
		info.AddProperty(PropertyName._sprayParticles, Variant.From<GpuParticles2D>(ref _sprayParticles));
		info.AddProperty(PropertyName._rainDropParticles, Variant.From<GpuParticles2D>(ref _rainDropParticles));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._droolParticles, ref val))
		{
			_droolParticles = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._sprayParticles, ref val2))
		{
			_sprayParticles = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._rainDropParticles, ref val3))
		{
			_rainDropParticles = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val4))
		{
			_parent = ((Variant)(ref val4)).As<Node2D>();
		}
	}
}
