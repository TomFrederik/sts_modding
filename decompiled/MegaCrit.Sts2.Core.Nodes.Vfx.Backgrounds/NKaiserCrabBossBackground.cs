using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Backgrounds;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/Backgrounds/NKaiserCrabBossBackground.cs")]
public class NKaiserCrabBossBackground : Node
{
	private enum RightArmState
	{
		Default,
		Charging,
		Resting
	}

	public enum ArmSide
	{
		Left,
		Right
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName PlayHurtAnim = StringName.op_Implicit("PlayHurtAnim");

		public static readonly StringName PlayArmDeathAnim = StringName.op_Implicit("PlayArmDeathAnim");

		public static readonly StringName PlayBodyDeathAnim = StringName.op_Implicit("PlayBodyDeathAnim");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _leftArm = StringName.op_Implicit("_leftArm");

		public static readonly StringName _rightArm = StringName.op_Implicit("_rightArm");

		public static readonly StringName _rightArmState = StringName.op_Implicit("_rightArmState");
	}

	public class SignalName : SignalName
	{
	}

	private const int _bodyTrack = 0;

	private const int _leftArmTrack = 1;

	private const int _rightArmTrack = 2;

	private const int _reactionTrack = 3;

	private MegaSprite _animController;

	private Node2D _leftArm;

	private Node2D _rightArm;

	private RightArmState _rightArmState;

	public override void _Ready()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%Visuals"))));
		_leftArm = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%ArmBoneL"));
		_rightArm = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%ArmBoneR"));
		MegaAnimationState animationState = _animController.GetAnimationState();
		animationState.SetAnimation("right/idle_loop", loop: true, 2);
		animationState.SetAnimation("body/idle_loop");
		animationState.SetAnimation("left/idle_loop", loop: true, 1);
	}

	public async Task PlayAttackAnim(ArmSide side, string animation, float duration)
	{
		string text = side.ToString().ToLowerInvariant();
		MegaAnimationState animationState = _animController.GetAnimationState();
		animationState.SetAnimation(text + "/" + animation, loop: false, (side == ArmSide.Left) ? 1 : 2);
		animationState.SetAnimation("reactions/attack_" + text, loop: false, 3);
		animationState.AddAnimation(text + "/idle_loop", 0f, loop: true, (side == ArmSide.Left) ? 1 : 2);
		await Cmd.Wait(duration);
	}

	public void PlayHurtAnim(ArmSide side)
	{
		string text = side.ToString().ToLowerInvariant();
		MegaAnimationState animationState = _animController.GetAnimationState();
		animationState.SetAnimation("reactions/hurt_" + text, loop: false, 3);
		if (side == ArmSide.Left)
		{
			animationState.SetAnimation(text + "/hurt", loop: false, 1);
			animationState.AddAnimation(text + "/idle_loop", 0f, loop: true, 1);
			return;
		}
		switch (_rightArmState)
		{
		case RightArmState.Default:
			animationState.SetAnimation(text + "/hurt", loop: false, 2);
			animationState.AddAnimation("right/idle_loop", 0f, loop: true, 2);
			break;
		case RightArmState.Charging:
			animationState.SetAnimation(text + "/hurt_charged", loop: false, 2);
			animationState.AddAnimation("right/charged_loop", 0f, loop: true, 2);
			break;
		case RightArmState.Resting:
			animationState.SetAnimation(text + "/hurt_resting", loop: false, 2);
			animationState.AddAnimation("right/rest_loop", 0f, loop: true, 2);
			break;
		}
	}

	public void PlayArmDeathAnim(ArmSide side)
	{
		string text = side.ToString().ToLowerInvariant();
		MegaAnimationState animationState = _animController.GetAnimationState();
		animationState.SetAnimation("reactions/hurt_" + text, loop: false, 3);
		if (side == ArmSide.Left)
		{
			animationState.SetAnimation(text + "/die", loop: false, 1);
			return;
		}
		switch (_rightArmState)
		{
		case RightArmState.Default:
		case RightArmState.Charging:
			animationState.SetAnimation("right/die", loop: false, 2);
			break;
		case RightArmState.Resting:
			animationState.SetAnimation("right/die_resting", loop: false, 2);
			break;
		}
	}

	public async Task PlayRightSideChargeUpAnim(float duration)
	{
		_rightArmState = RightArmState.Charging;
		MegaAnimationState animationState = _animController.GetAnimationState();
		animationState.SetAnimation("right/charge_up", loop: false, 2);
		animationState.AddAnimation("right/charged_loop", 0f, loop: true, 2);
		animationState.SetAnimation("reactions/attack_right", loop: false, 3);
		await Cmd.Wait(duration);
	}

	public async Task PlayRightSideHeavy(float duration)
	{
		_rightArmState = RightArmState.Resting;
		MegaAnimationState animationState = _animController.GetAnimationState();
		animationState.SetAnimation("right/attack_heavy", loop: false, 2);
		animationState.AddAnimation("right/rest_loop", 0f, loop: true, 2);
		animationState.SetAnimation("reactions/attack_right", loop: false, 3);
		await Cmd.Wait(duration);
	}

	public async Task PlayRightRecharge(float duration)
	{
		_rightArmState = RightArmState.Default;
		MegaAnimationState animationState = _animController.GetAnimationState();
		animationState.SetAnimation("right/wake_up", loop: false, 2);
		animationState.AddAnimation("right/idle_loop", 0f, loop: true, 2);
		await Cmd.Wait(duration);
	}

	public void PlayBodyDeathAnim()
	{
		MegaAnimationState animationState = _animController.GetAnimationState();
		animationState.SetAnimation("body/die", loop: false);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayHurtAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("side"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayArmDeathAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("side"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayBodyDeathAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayHurtAnim && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PlayHurtAnim(VariantUtils.ConvertTo<ArmSide>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayArmDeathAnim && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PlayArmDeathAnim(VariantUtils.ConvertTo<ArmSide>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayBodyDeathAnim && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayBodyDeathAnim();
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
		if ((ref method) == MethodName.PlayHurtAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayArmDeathAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayBodyDeathAnim)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._leftArm)
		{
			_leftArm = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightArm)
		{
			_rightArm = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightArmState)
		{
			_rightArmState = VariantUtils.ConvertTo<RightArmState>(ref value);
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
		if ((ref name) == PropertyName._leftArm)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _leftArm);
			return true;
		}
		if ((ref name) == PropertyName._rightArm)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _rightArm);
			return true;
		}
		if ((ref name) == PropertyName._rightArmState)
		{
			value = VariantUtils.CreateFrom<RightArmState>(ref _rightArmState);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._leftArm, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rightArm, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._rightArmState, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._leftArm, Variant.From<Node2D>(ref _leftArm));
		info.AddProperty(PropertyName._rightArm, Variant.From<Node2D>(ref _rightArm));
		info.AddProperty(PropertyName._rightArmState, Variant.From<RightArmState>(ref _rightArmState));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._leftArm, ref val))
		{
			_leftArm = ((Variant)(ref val)).As<Node2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightArm, ref val2))
		{
			_rightArm = ((Variant)(ref val2)).As<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightArmState, ref val3))
		{
			_rightArmState = ((Variant)(ref val3)).As<RightArmState>();
		}
	}
}
