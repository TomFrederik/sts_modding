using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NEntomancerVfx.cs")]
public class NEntomancerVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName LaunchSwarm = StringName.op_Implicit("LaunchSwarm");

		public static readonly StringName CompleteSwarmAttack = StringName.op_Implicit("CompleteSwarmAttack");

		public static readonly StringName CancelSwarmAttack = StringName.op_Implicit("CancelSwarmAttack");

		public static readonly StringName TurnOffSwarm = StringName.op_Implicit("TurnOffSwarm");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _swarmParticles = StringName.op_Implicit("_swarmParticles");

		public static readonly StringName _attackingBugParticles = StringName.op_Implicit("_attackingBugParticles");

		public static readonly StringName _swarmTargetNode = StringName.op_Implicit("_swarmTargetNode");

		public static readonly StringName _basePosition = StringName.op_Implicit("_basePosition");

		public static readonly StringName _swarmTween = StringName.op_Implicit("_swarmTween");

		public static readonly StringName _swarming = StringName.op_Implicit("_swarming");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private GpuParticles2D _swarmParticles;

	private GpuParticles2D _attackingBugParticles;

	private Node2D _swarmTargetNode;

	private Vector2 _basePosition;

	private Tween? _swarmTween;

	private bool _swarming;

	private Node2D _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_swarmParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SwarmParticles"));
		_attackingBugParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SwarmParticles/AttackingBugParticles"));
		_swarmTargetNode = ((Node)_parent).GetNode<Node2D>(NodePath.op_Implicit("SwarmTargetNode"));
		_basePosition = ((Node2D)_swarmParticles).Position;
		_attackingBugParticles.Emitting = false;
	}

	private void LaunchSwarm()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		_swarming = true;
		_swarmTween = ((Node)this).CreateTween();
		_attackingBugParticles.Emitting = true;
		_swarmTween.SetEase((EaseType)1).SetTrans((TransitionType)7);
		_swarmTween.TweenProperty((GodotObject)(object)_swarmParticles, NodePath.op_Implicit("position"), Variant.op_Implicit(_swarmTargetNode.Position), 1.0);
		_swarmTween.SetEase((EaseType)2).SetTrans((TransitionType)7);
		_swarmTween.TweenProperty((GodotObject)(object)_swarmParticles, NodePath.op_Implicit("position"), Variant.op_Implicit(_basePosition), 1.5).SetDelay(0.0);
		_swarmTween.TweenCallback(Callable.From((Action)CompleteSwarmAttack)).SetDelay(0.009999999776482582);
	}

	private void CompleteSwarmAttack()
	{
		_attackingBugParticles.Emitting = false;
		_swarming = false;
	}

	private void CancelSwarmAttack()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (_swarming)
		{
			_swarmTween.Kill();
			((Node2D)_swarmParticles).Position = _basePosition;
			CompleteSwarmAttack();
		}
	}

	private void TurnOffSwarm()
	{
		_swarmParticles.Emitting = false;
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		string eventName = new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName();
		if (!(eventName == "launch_swarm"))
		{
			if (eventName == "turn_off_swarm")
			{
				TurnOffSwarm();
			}
		}
		else
		{
			LaunchSwarm();
		}
	}

	public override void _ExitTree()
	{
		Tween? swarmTween = _swarmTween;
		if (swarmTween != null)
		{
			swarmTween.Kill();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LaunchSwarm, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CompleteSwarmAttack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelSwarmAttack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffSwarm, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LaunchSwarm && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			LaunchSwarm();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CompleteSwarmAttack && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CompleteSwarmAttack();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CancelSwarmAttack && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CancelSwarmAttack();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffSwarm && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffSwarm();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAnimationEvent && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			OnAnimationEvent(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
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
		if ((ref method) == MethodName.LaunchSwarm)
		{
			return true;
		}
		if ((ref method) == MethodName.CompleteSwarmAttack)
		{
			return true;
		}
		if ((ref method) == MethodName.CancelSwarmAttack)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffSwarm)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAnimationEvent)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._swarmParticles)
		{
			_swarmParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._attackingBugParticles)
		{
			_attackingBugParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._swarmTargetNode)
		{
			_swarmTargetNode = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._basePosition)
		{
			_basePosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._swarmTween)
		{
			_swarmTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._swarming)
		{
			_swarming = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._swarmParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _swarmParticles);
			return true;
		}
		if ((ref name) == PropertyName._attackingBugParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _attackingBugParticles);
			return true;
		}
		if ((ref name) == PropertyName._swarmTargetNode)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _swarmTargetNode);
			return true;
		}
		if ((ref name) == PropertyName._basePosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _basePosition);
			return true;
		}
		if ((ref name) == PropertyName._swarmTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _swarmTween);
			return true;
		}
		if ((ref name) == PropertyName._swarming)
		{
			value = VariantUtils.CreateFrom<bool>(ref _swarming);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._swarmParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._attackingBugParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._swarmTargetNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._basePosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._swarmTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._swarming, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._swarmParticles, Variant.From<GpuParticles2D>(ref _swarmParticles));
		info.AddProperty(PropertyName._attackingBugParticles, Variant.From<GpuParticles2D>(ref _attackingBugParticles));
		info.AddProperty(PropertyName._swarmTargetNode, Variant.From<Node2D>(ref _swarmTargetNode));
		info.AddProperty(PropertyName._basePosition, Variant.From<Vector2>(ref _basePosition));
		info.AddProperty(PropertyName._swarmTween, Variant.From<Tween>(ref _swarmTween));
		info.AddProperty(PropertyName._swarming, Variant.From<bool>(ref _swarming));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._swarmParticles, ref val))
		{
			_swarmParticles = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._attackingBugParticles, ref val2))
		{
			_attackingBugParticles = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._swarmTargetNode, ref val3))
		{
			_swarmTargetNode = ((Variant)(ref val3)).As<Node2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._basePosition, ref val4))
		{
			_basePosition = ((Variant)(ref val4)).As<Vector2>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._swarmTween, ref val5))
		{
			_swarmTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._swarming, ref val6))
		{
			_swarming = ((Variant)(ref val6)).As<bool>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val7))
		{
			_parent = ((Variant)(ref val7)).As<Node2D>();
		}
	}
}
