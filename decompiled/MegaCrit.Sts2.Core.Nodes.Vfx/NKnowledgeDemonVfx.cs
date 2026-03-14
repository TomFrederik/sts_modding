using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NKnowledgeDemonVfx.cs")]
public class NKnowledgeDemonVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName OnExplode = StringName.op_Implicit("OnExplode");

		public static readonly StringName OnTakeDamage = StringName.op_Implicit("OnTakeDamage");

		public static readonly StringName OnBurningStart = StringName.op_Implicit("OnBurningStart");

		public static readonly StringName OnEmbersStart = StringName.op_Implicit("OnEmbersStart");

		public static readonly StringName OnThinEmbersStart = StringName.op_Implicit("OnThinEmbersStart");

		public static readonly StringName OnBurningEnd = StringName.op_Implicit("OnBurningEnd");

		public static readonly StringName OnEmbersEnd = StringName.op_Implicit("OnEmbersEnd");

		public static readonly StringName OnThinEmbersEnd = StringName.op_Implicit("OnThinEmbersEnd");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _fireNode1 = StringName.op_Implicit("_fireNode1");

		public static readonly StringName _fireNode2 = StringName.op_Implicit("_fireNode2");

		public static readonly StringName _fireNode3 = StringName.op_Implicit("_fireNode3");

		public static readonly StringName _fireNode4 = StringName.op_Implicit("_fireNode4");

		public static readonly StringName _explosionParticles = StringName.op_Implicit("_explosionParticles");

		public static readonly StringName _damageParticles = StringName.op_Implicit("_damageParticles");

		public static readonly StringName _emberParticles = StringName.op_Implicit("_emberParticles");

		public static readonly StringName _thinEmberParticles = StringName.op_Implicit("_thinEmberParticles");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private Node2D _fireNode1;

	private Node2D _fireNode2;

	private Node2D _fireNode3;

	private Node2D _fireNode4;

	private GpuParticles2D _explosionParticles;

	private GpuParticles2D _damageParticles;

	private GpuParticles2D _emberParticles;

	private GpuParticles2D _thinEmberParticles;

	private Node2D _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_fireNode1 = ((Node)_parent).GetNode<Node2D>(NodePath.op_Implicit("FireSlot1/FireHolder1"));
		_fireNode2 = ((Node)_parent).GetNode<Node2D>(NodePath.op_Implicit("FireSlot2/FireHolder2"));
		_fireNode3 = ((Node)_parent).GetNode<Node2D>(NodePath.op_Implicit("FireSlot3/FireHolder3"));
		_fireNode4 = ((Node)_parent).GetNode<Node2D>(NodePath.op_Implicit("FireSlot4/FireHolder4"));
		_explosionParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("ExplosionParticles"));
		_damageParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("DamageParticles"));
		_emberParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("EmberParticles"));
		_thinEmberParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("ThinEmberParticles"));
		((CanvasItem)_fireNode1).Visible = false;
		((CanvasItem)_fireNode2).Visible = false;
		((CanvasItem)_fireNode3).Visible = false;
		((CanvasItem)_fireNode4).Visible = false;
		_explosionParticles.Emitting = false;
		_explosionParticles.OneShot = true;
		_damageParticles.Emitting = false;
		_damageParticles.OneShot = true;
		_emberParticles.Emitting = false;
		_thinEmberParticles.Emitting = false;
		OnBurningEnd();
		_animController.GetAnimationState().SetAnimation("idle_loop");
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
			case 't':
				if (eventName == "take_damage")
				{
					OnTakeDamage();
				}
				break;
			case 'b':
				if (eventName == "burning_end")
				{
					OnBurningEnd();
				}
				break;
			}
			break;
		case 7:
			if (eventName == "explode")
			{
				OnExplode();
			}
			break;
		case 13:
			if (eventName == "burning_start")
			{
				OnBurningStart();
			}
			break;
		case 12:
			if (eventName == "embers_start")
			{
				OnEmbersStart();
			}
			break;
		case 17:
			if (eventName == "thin_embers_start")
			{
				OnThinEmbersStart();
			}
			break;
		case 10:
			if (eventName == "embers_end")
			{
				OnEmbersEnd();
			}
			break;
		case 15:
			if (eventName == "thin_embers_end")
			{
				OnThinEmbersEnd();
			}
			break;
		case 8:
		case 9:
		case 14:
		case 16:
			break;
		}
	}

	private void OnExplode()
	{
		_explosionParticles.Restart();
	}

	private void OnTakeDamage()
	{
		_damageParticles.Restart();
	}

	private void OnBurningStart()
	{
		((CanvasItem)_fireNode1).Visible = true;
		((CanvasItem)_fireNode2).Visible = true;
		((CanvasItem)_fireNode3).Visible = true;
		((CanvasItem)_fireNode4).Visible = true;
	}

	private void OnEmbersStart()
	{
		_emberParticles.Restart();
	}

	private void OnThinEmbersStart()
	{
		_thinEmberParticles.Restart();
	}

	private void OnBurningEnd()
	{
		((CanvasItem)_fireNode1).Visible = false;
		((CanvasItem)_fireNode2).Visible = false;
		((CanvasItem)_fireNode3).Visible = false;
		((CanvasItem)_fireNode4).Visible = false;
	}

	private void OnEmbersEnd()
	{
		_emberParticles.Emitting = false;
	}

	private void OnThinEmbersEnd()
	{
		_thinEmberParticles.Emitting = false;
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
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnExplode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnTakeDamage, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnBurningStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEmbersStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnThinEmbersStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnBurningEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEmbersEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnThinEmbersEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnExplode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnExplode();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnTakeDamage && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnTakeDamage();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnBurningStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnBurningStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEmbersStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEmbersStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnThinEmbersStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnThinEmbersStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnBurningEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnBurningEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEmbersEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEmbersEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnThinEmbersEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnThinEmbersEnd();
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
		if ((ref method) == MethodName.OnExplode)
		{
			return true;
		}
		if ((ref method) == MethodName.OnTakeDamage)
		{
			return true;
		}
		if ((ref method) == MethodName.OnBurningStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEmbersStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnThinEmbersStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnBurningEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEmbersEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnThinEmbersEnd)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._fireNode1)
		{
			_fireNode1 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fireNode2)
		{
			_fireNode2 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fireNode3)
		{
			_fireNode3 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fireNode4)
		{
			_fireNode4 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._explosionParticles)
		{
			_explosionParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._damageParticles)
		{
			_damageParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._emberParticles)
		{
			_emberParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._thinEmberParticles)
		{
			_thinEmberParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
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
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._fireNode1)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _fireNode1);
			return true;
		}
		if ((ref name) == PropertyName._fireNode2)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _fireNode2);
			return true;
		}
		if ((ref name) == PropertyName._fireNode3)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _fireNode3);
			return true;
		}
		if ((ref name) == PropertyName._fireNode4)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _fireNode4);
			return true;
		}
		if ((ref name) == PropertyName._explosionParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _explosionParticles);
			return true;
		}
		if ((ref name) == PropertyName._damageParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _damageParticles);
			return true;
		}
		if ((ref name) == PropertyName._emberParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _emberParticles);
			return true;
		}
		if ((ref name) == PropertyName._thinEmberParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _thinEmberParticles);
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
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._fireNode1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fireNode2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fireNode3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fireNode4, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._explosionParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._damageParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._emberParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._thinEmberParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._fireNode1, Variant.From<Node2D>(ref _fireNode1));
		info.AddProperty(PropertyName._fireNode2, Variant.From<Node2D>(ref _fireNode2));
		info.AddProperty(PropertyName._fireNode3, Variant.From<Node2D>(ref _fireNode3));
		info.AddProperty(PropertyName._fireNode4, Variant.From<Node2D>(ref _fireNode4));
		info.AddProperty(PropertyName._explosionParticles, Variant.From<GpuParticles2D>(ref _explosionParticles));
		info.AddProperty(PropertyName._damageParticles, Variant.From<GpuParticles2D>(ref _damageParticles));
		info.AddProperty(PropertyName._emberParticles, Variant.From<GpuParticles2D>(ref _emberParticles));
		info.AddProperty(PropertyName._thinEmberParticles, Variant.From<GpuParticles2D>(ref _thinEmberParticles));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._fireNode1, ref val))
		{
			_fireNode1 = ((Variant)(ref val)).As<Node2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._fireNode2, ref val2))
		{
			_fireNode2 = ((Variant)(ref val2)).As<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._fireNode3, ref val3))
		{
			_fireNode3 = ((Variant)(ref val3)).As<Node2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._fireNode4, ref val4))
		{
			_fireNode4 = ((Variant)(ref val4)).As<Node2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._explosionParticles, ref val5))
		{
			_explosionParticles = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._damageParticles, ref val6))
		{
			_damageParticles = ((Variant)(ref val6)).As<GpuParticles2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._emberParticles, ref val7))
		{
			_emberParticles = ((Variant)(ref val7)).As<GpuParticles2D>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._thinEmberParticles, ref val8))
		{
			_thinEmberParticles = ((Variant)(ref val8)).As<GpuParticles2D>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val9))
		{
			_parent = ((Variant)(ref val9)).As<Node2D>();
		}
	}
}
