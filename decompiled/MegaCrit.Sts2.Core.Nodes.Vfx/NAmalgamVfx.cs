using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NAmalgamVfx.cs")]
public class NAmalgamVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName PoofToDeath = StringName.op_Implicit("PoofToDeath");

		public static readonly StringName RestartTorches = StringName.op_Implicit("RestartTorches");

		public static readonly StringName KillTorches = StringName.op_Implicit("KillTorches");

		public static readonly StringName PlayHit1 = StringName.op_Implicit("PlayHit1");

		public static readonly StringName PlayHit2 = StringName.op_Implicit("PlayHit2");

		public static readonly StringName PlayHit3 = StringName.op_Implicit("PlayHit3");

		public static readonly StringName PlayLaserBase = StringName.op_Implicit("PlayLaserBase");

		public static readonly StringName PlayLaserHit = StringName.op_Implicit("PlayLaserHit");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _hitFxParticles = StringName.op_Implicit("_hitFxParticles");

		public static readonly StringName _hitBoneNode = StringName.op_Implicit("_hitBoneNode");

		public static readonly StringName _deathBodyParticles = StringName.op_Implicit("_deathBodyParticles");

		public static readonly StringName _laserBaseParticles = StringName.op_Implicit("_laserBaseParticles");

		public static readonly StringName _hitParticles1 = StringName.op_Implicit("_hitParticles1");

		public static readonly StringName _hitParticles2 = StringName.op_Implicit("_hitParticles2");

		public static readonly StringName _hitParticles3 = StringName.op_Implicit("_hitParticles3");

		public static readonly StringName _constantSparks1 = StringName.op_Implicit("_constantSparks1");

		public static readonly StringName _constantSparks2 = StringName.op_Implicit("_constantSparks2");

		public static readonly StringName _constantSparks3 = StringName.op_Implicit("_constantSparks3");

		public static readonly StringName _torch1Node = StringName.op_Implicit("_torch1Node");

		public static readonly StringName _torch2Node = StringName.op_Implicit("_torch2Node");

		public static readonly StringName _torch3Node = StringName.op_Implicit("_torch3Node");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private GpuParticles2D _hitFxParticles;

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D _hitBoneNode;

	private CpuParticles2D _deathBodyParticles;

	private GpuParticles2D _laserBaseParticles;

	private GpuParticles2D _hitParticles1;

	private GpuParticles2D _hitParticles2;

	private GpuParticles2D _hitParticles3;

	private GpuParticles2D _constantSparks1;

	private GpuParticles2D _constantSparks2;

	private GpuParticles2D _constantSparks3;

	private Node2D _torch1Node;

	private Node2D _torch2Node;

	private Node2D _torch3Node;

	private Node _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_deathBodyParticles = _parent.GetNode<CpuParticles2D>(NodePath.op_Implicit("CPUDeathParticles"));
		_deathBodyParticles.Emitting = false;
		_deathBodyParticles.OneShot = true;
		_laserBaseParticles = _parent.GetNode<GpuParticles2D>(NodePath.op_Implicit("laserBaseBone/laserBaseParticles"));
		_laserBaseParticles.Emitting = false;
		_torch1Node = _parent.GetNode<Node2D>(NodePath.op_Implicit("torch1Slot/fire1_small_green"));
		((CanvasItem)_torch1Node).Visible = true;
		_torch2Node = _parent.GetNode<Node2D>(NodePath.op_Implicit("torch2Slot/fire2_small_green"));
		((CanvasItem)_torch2Node).Visible = true;
		_torch3Node = _parent.GetNode<Node2D>(NodePath.op_Implicit("torch3Slot/fire3_small_green"));
		((CanvasItem)_torch3Node).Visible = true;
		_hitParticles1 = _parent.GetNode<GpuParticles2D>(NodePath.op_Implicit("torch1UnscaledBone/hitParticles"));
		_hitParticles1.Emitting = false;
		_hitParticles1.OneShot = true;
		_hitParticles2 = _parent.GetNode<GpuParticles2D>(NodePath.op_Implicit("torch2UnscaledBone/hitParticles"));
		_hitParticles2.Emitting = false;
		_hitParticles2.OneShot = true;
		_hitParticles3 = _parent.GetNode<GpuParticles2D>(NodePath.op_Implicit("torch3UnscaledBone/hitParticles"));
		_hitParticles3.Emitting = false;
		_hitParticles3.OneShot = true;
		_constantSparks1 = _parent.GetNode<GpuParticles2D>(NodePath.op_Implicit("torch1Slot/constantParticles"));
		_constantSparks2 = _parent.GetNode<GpuParticles2D>(NodePath.op_Implicit("torch2Slot/constantParticles"));
		_constantSparks3 = _parent.GetNode<GpuParticles2D>(NodePath.op_Implicit("torch3Slot/constantParticles"));
		((CanvasItem)_hitFxParticles).Visible = false;
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
		case 4:
			switch (eventName[3])
			{
			case '1':
				if (eventName == "hit1")
				{
					PlayHit1();
				}
				break;
			case '2':
				if (eventName == "hit2")
				{
					PlayHit2();
				}
				break;
			case '3':
				if (eventName == "hit3")
				{
					PlayHit3();
				}
				break;
			}
			break;
		case 14:
			switch (eventName[6])
			{
			case 'b':
				if (eventName == "laser_base_off")
				{
					PlayLaserBase(starting: false);
				}
				break;
			case 'h':
				if (eventName == "laser_hit_fire")
				{
					PlayLaserHit(starting: true);
				}
				break;
			}
			break;
		case 7:
			if (eventName == "go_poof")
			{
				PoofToDeath();
			}
			break;
		case 11:
			if (eventName == "torches_out")
			{
				KillTorches();
			}
			break;
		case 10:
			if (eventName == "torches_on")
			{
				RestartTorches();
			}
			break;
		case 15:
			if (eventName == "laser_base_fire")
			{
				PlayLaserBase(starting: true);
			}
			break;
		case 13:
			if (eventName == "laser_hit_off")
			{
				PlayLaserHit(starting: false);
			}
			break;
		case 5:
		case 6:
		case 8:
		case 9:
		case 12:
			break;
		}
	}

	private void PoofToDeath()
	{
		_deathBodyParticles.Restart();
	}

	private void RestartTorches()
	{
		((CanvasItem)_torch1Node).Visible = true;
		((CanvasItem)_torch2Node).Visible = true;
		((CanvasItem)_torch3Node).Visible = true;
		_constantSparks1.Emitting = true;
		_constantSparks2.Emitting = true;
		_constantSparks3.Emitting = true;
	}

	private void KillTorches()
	{
		((CanvasItem)_torch1Node).Visible = false;
		((CanvasItem)_torch2Node).Visible = false;
		((CanvasItem)_torch3Node).Visible = false;
		_constantSparks1.Emitting = false;
		_constantSparks2.Emitting = false;
		_constantSparks3.Emitting = false;
	}

	private void PlayHit1()
	{
		_hitParticles1.Restart();
	}

	private void PlayHit2()
	{
		_hitParticles2.Restart();
	}

	private void PlayHit3()
	{
		_hitParticles3.Restart();
	}

	private void PlayLaserBase(bool starting)
	{
		if (starting)
		{
			((CanvasItem)_laserBaseParticles).Visible = true;
			_laserBaseParticles.Restart();
		}
		else
		{
			_laserBaseParticles.Emitting = false;
			((CanvasItem)_laserBaseParticles).Visible = false;
		}
	}

	private void PlayLaserHit(bool starting)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (starting)
		{
			((Node2D)_hitFxParticles).GlobalPosition = _hitBoneNode.GlobalPosition;
			((CanvasItem)_hitFxParticles).Visible = true;
			_hitFxParticles.Restart();
		}
		else
		{
			_hitFxParticles.Emitting = false;
			((CanvasItem)_hitFxParticles).Visible = false;
		}
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
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PoofToDeath, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RestartTorches, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.KillTorches, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayHit1, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayHit2, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayHit3, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayLaserBase, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("starting"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayLaserHit, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("starting"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
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
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.PoofToDeath && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PoofToDeath();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RestartTorches && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RestartTorches();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.KillTorches && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			KillTorches();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayHit1 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayHit1();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayHit2 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayHit2();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayHit3 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayHit3();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayLaserBase && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PlayLaserBase(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayLaserHit && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PlayLaserHit(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.PoofToDeath)
		{
			return true;
		}
		if ((ref method) == MethodName.RestartTorches)
		{
			return true;
		}
		if ((ref method) == MethodName.KillTorches)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayHit1)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayHit2)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayHit3)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayLaserBase)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayLaserHit)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._hitFxParticles)
		{
			_hitFxParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hitBoneNode)
		{
			_hitBoneNode = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deathBodyParticles)
		{
			_deathBodyParticles = VariantUtils.ConvertTo<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._laserBaseParticles)
		{
			_laserBaseParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hitParticles1)
		{
			_hitParticles1 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hitParticles2)
		{
			_hitParticles2 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hitParticles3)
		{
			_hitParticles3 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._constantSparks1)
		{
			_constantSparks1 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._constantSparks2)
		{
			_constantSparks2 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._constantSparks3)
		{
			_constantSparks3 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._torch1Node)
		{
			_torch1Node = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._torch2Node)
		{
			_torch2Node = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._torch3Node)
		{
			_torch3Node = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			_parent = VariantUtils.ConvertTo<Node>(ref value);
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
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._hitFxParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _hitFxParticles);
			return true;
		}
		if ((ref name) == PropertyName._hitBoneNode)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _hitBoneNode);
			return true;
		}
		if ((ref name) == PropertyName._deathBodyParticles)
		{
			value = VariantUtils.CreateFrom<CpuParticles2D>(ref _deathBodyParticles);
			return true;
		}
		if ((ref name) == PropertyName._laserBaseParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _laserBaseParticles);
			return true;
		}
		if ((ref name) == PropertyName._hitParticles1)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _hitParticles1);
			return true;
		}
		if ((ref name) == PropertyName._hitParticles2)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _hitParticles2);
			return true;
		}
		if ((ref name) == PropertyName._hitParticles3)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _hitParticles3);
			return true;
		}
		if ((ref name) == PropertyName._constantSparks1)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _constantSparks1);
			return true;
		}
		if ((ref name) == PropertyName._constantSparks2)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _constantSparks2);
			return true;
		}
		if ((ref name) == PropertyName._constantSparks3)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _constantSparks3);
			return true;
		}
		if ((ref name) == PropertyName._torch1Node)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _torch1Node);
			return true;
		}
		if ((ref name) == PropertyName._torch2Node)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _torch2Node);
			return true;
		}
		if ((ref name) == PropertyName._torch3Node)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _torch3Node);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			value = VariantUtils.CreateFrom<Node>(ref _parent);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._hitFxParticles, (PropertyHint)34, "GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._hitBoneNode, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._deathBodyParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._laserBaseParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hitParticles1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hitParticles2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hitParticles3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._constantSparks1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._constantSparks2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._constantSparks3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._torch1Node, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._torch2Node, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._torch3Node, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._hitFxParticles, Variant.From<GpuParticles2D>(ref _hitFxParticles));
		info.AddProperty(PropertyName._hitBoneNode, Variant.From<Node2D>(ref _hitBoneNode));
		info.AddProperty(PropertyName._deathBodyParticles, Variant.From<CpuParticles2D>(ref _deathBodyParticles));
		info.AddProperty(PropertyName._laserBaseParticles, Variant.From<GpuParticles2D>(ref _laserBaseParticles));
		info.AddProperty(PropertyName._hitParticles1, Variant.From<GpuParticles2D>(ref _hitParticles1));
		info.AddProperty(PropertyName._hitParticles2, Variant.From<GpuParticles2D>(ref _hitParticles2));
		info.AddProperty(PropertyName._hitParticles3, Variant.From<GpuParticles2D>(ref _hitParticles3));
		info.AddProperty(PropertyName._constantSparks1, Variant.From<GpuParticles2D>(ref _constantSparks1));
		info.AddProperty(PropertyName._constantSparks2, Variant.From<GpuParticles2D>(ref _constantSparks2));
		info.AddProperty(PropertyName._constantSparks3, Variant.From<GpuParticles2D>(ref _constantSparks3));
		info.AddProperty(PropertyName._torch1Node, Variant.From<Node2D>(ref _torch1Node));
		info.AddProperty(PropertyName._torch2Node, Variant.From<Node2D>(ref _torch2Node));
		info.AddProperty(PropertyName._torch3Node, Variant.From<Node2D>(ref _torch3Node));
		info.AddProperty(PropertyName._parent, Variant.From<Node>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._hitFxParticles, ref val))
		{
			_hitFxParticles = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._hitBoneNode, ref val2))
		{
			_hitBoneNode = ((Variant)(ref val2)).As<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._deathBodyParticles, ref val3))
		{
			_deathBodyParticles = ((Variant)(ref val3)).As<CpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._laserBaseParticles, ref val4))
		{
			_laserBaseParticles = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._hitParticles1, ref val5))
		{
			_hitParticles1 = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._hitParticles2, ref val6))
		{
			_hitParticles2 = ((Variant)(ref val6)).As<GpuParticles2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._hitParticles3, ref val7))
		{
			_hitParticles3 = ((Variant)(ref val7)).As<GpuParticles2D>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._constantSparks1, ref val8))
		{
			_constantSparks1 = ((Variant)(ref val8)).As<GpuParticles2D>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._constantSparks2, ref val9))
		{
			_constantSparks2 = ((Variant)(ref val9)).As<GpuParticles2D>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._constantSparks3, ref val10))
		{
			_constantSparks3 = ((Variant)(ref val10)).As<GpuParticles2D>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._torch1Node, ref val11))
		{
			_torch1Node = ((Variant)(ref val11)).As<Node2D>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._torch2Node, ref val12))
		{
			_torch2Node = ((Variant)(ref val12)).As<Node2D>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._torch3Node, ref val13))
		{
			_torch3Node = ((Variant)(ref val13)).As<Node2D>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val14))
		{
			_parent = ((Variant)(ref val14)).As<Node>();
		}
	}
}
