using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NVineShamblerVinesVfx.cs")]
public class NVineShamblerVinesVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnFrontEvent = StringName.op_Implicit("OnFrontEvent");

		public static readonly StringName AnimationEnded = StringName.op_Implicit("AnimationEnded");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _frontVinesNode = StringName.op_Implicit("_frontVinesNode");

		public static readonly StringName _backVinesNode = StringName.op_Implicit("_backVinesNode");

		public static readonly StringName _dirtBlast1 = StringName.op_Implicit("_dirtBlast1");

		public static readonly StringName _dirtBlast2 = StringName.op_Implicit("_dirtBlast2");

		public static readonly StringName _dirtBlast3 = StringName.op_Implicit("_dirtBlast3");

		public static readonly StringName _dirtBlast4 = StringName.op_Implicit("_dirtBlast4");
	}

	public class SignalName : SignalName
	{
	}

	private Node2D _frontVinesNode;

	private MegaSprite _frontVinesAnimController;

	private Node2D _backVinesNode;

	private MegaSprite _backVinesAnimController;

	private GpuParticles2D _dirtBlast1;

	private GpuParticles2D _dirtBlast2;

	private GpuParticles2D _dirtBlast3;

	private GpuParticles2D _dirtBlast4;

	public override void _Ready()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		_frontVinesNode = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("VinesFront"));
		_frontVinesAnimController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_frontVinesNode));
		_frontVinesAnimController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnFrontEvent));
		_frontVinesAnimController.ConnectAnimationCompleted(Callable.From<GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject>)AnimationEnded));
		_backVinesNode = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("VinesBackScene/VinesBack"));
		_backVinesAnimController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_backVinesNode));
		_dirtBlast1 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("DirtBlast1"));
		_dirtBlast3 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("DirtBlast3"));
		_dirtBlast2 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("VinesBackScene/DirtBlast2"));
		_dirtBlast4 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("VinesBackScene/DirtBlast4"));
		_dirtBlast1.Emitting = false;
		_dirtBlast1.OneShot = true;
		_dirtBlast2.Emitting = false;
		_dirtBlast2.OneShot = true;
		_dirtBlast3.Emitting = false;
		_dirtBlast3.OneShot = true;
		_dirtBlast4.Emitting = false;
		_dirtBlast4.OneShot = true;
		Vector2 backVineOffset = _backVinesNode.GlobalPosition - _frontVinesNode.GlobalPosition;
		((Node)_backVinesNode).Reparent(NCombatRoom.Instance.BackCombatVfxContainer, true);
		Callable val = Callable.From((Action)delegate
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			_backVinesNode.GlobalPosition = _frontVinesNode.GlobalPosition + backVineOffset;
		});
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		_frontVinesAnimController.GetAnimationState().SetAnimation("animation");
		_backVinesAnimController.GetAnimationState().SetAnimation("animation");
	}

	private void OnFrontEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		switch (new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName())
		{
		case "dirt_1":
			_dirtBlast1.Restart();
			break;
		case "dirt_2":
			_dirtBlast2.Restart();
			break;
		case "dirt_3":
			_dirtBlast3.Restart();
			break;
		case "dirt_4":
			_dirtBlast4.Restart();
			break;
		}
	}

	private void AnimationEnded(GodotObject _, GodotObject __, GodotObject ___)
	{
		((Node)(object)this).QueueFreeSafely();
		((Node)(object)_backVinesNode).QueueFreeSafely();
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
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFrontEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimationEnded, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFrontEvent && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			OnFrontEvent(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimationEnded && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			AnimationEnded(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]));
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
		if ((ref method) == MethodName.OnFrontEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimationEnded)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._frontVinesNode)
		{
			_frontVinesNode = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backVinesNode)
		{
			_backVinesNode = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dirtBlast1)
		{
			_dirtBlast1 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dirtBlast2)
		{
			_dirtBlast2 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dirtBlast3)
		{
			_dirtBlast3 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dirtBlast4)
		{
			_dirtBlast4 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
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
		if ((ref name) == PropertyName._frontVinesNode)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _frontVinesNode);
			return true;
		}
		if ((ref name) == PropertyName._backVinesNode)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _backVinesNode);
			return true;
		}
		if ((ref name) == PropertyName._dirtBlast1)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _dirtBlast1);
			return true;
		}
		if ((ref name) == PropertyName._dirtBlast2)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _dirtBlast2);
			return true;
		}
		if ((ref name) == PropertyName._dirtBlast3)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _dirtBlast3);
			return true;
		}
		if ((ref name) == PropertyName._dirtBlast4)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _dirtBlast4);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._frontVinesNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backVinesNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dirtBlast1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dirtBlast2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dirtBlast3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dirtBlast4, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._frontVinesNode, Variant.From<Node2D>(ref _frontVinesNode));
		info.AddProperty(PropertyName._backVinesNode, Variant.From<Node2D>(ref _backVinesNode));
		info.AddProperty(PropertyName._dirtBlast1, Variant.From<GpuParticles2D>(ref _dirtBlast1));
		info.AddProperty(PropertyName._dirtBlast2, Variant.From<GpuParticles2D>(ref _dirtBlast2));
		info.AddProperty(PropertyName._dirtBlast3, Variant.From<GpuParticles2D>(ref _dirtBlast3));
		info.AddProperty(PropertyName._dirtBlast4, Variant.From<GpuParticles2D>(ref _dirtBlast4));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._frontVinesNode, ref val))
		{
			_frontVinesNode = ((Variant)(ref val)).As<Node2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._backVinesNode, ref val2))
		{
			_backVinesNode = ((Variant)(ref val2)).As<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._dirtBlast1, ref val3))
		{
			_dirtBlast1 = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._dirtBlast2, ref val4))
		{
			_dirtBlast2 = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._dirtBlast3, ref val5))
		{
			_dirtBlast3 = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._dirtBlast4, ref val6))
		{
			_dirtBlast4 = ((Variant)(ref val6)).As<GpuParticles2D>();
		}
	}
}
