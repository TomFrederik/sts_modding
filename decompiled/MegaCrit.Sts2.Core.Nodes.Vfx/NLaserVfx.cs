using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NLaserVfx.cs")]
public class NLaserVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ExtendLaser = StringName.op_Implicit("ExtendLaser");

		public static readonly StringName RetractLaser = StringName.op_Implicit("RetractLaser");

		public static readonly StringName ResetLaser = StringName.op_Implicit("ResetLaser");

		public static readonly StringName SetLaserColor = StringName.op_Implicit("SetLaserColor");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _animNode = StringName.op_Implicit("_animNode");

		public static readonly StringName _targetingBone = StringName.op_Implicit("_targetingBone");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _color = new StringName("Color");

	private Node2D _animNode;

	private MegaSprite _animController;

	private Node2D _targetingBone;

	public override void _Ready()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_animNode = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("SpineSprite"));
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_animNode));
		_targetingBone = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("SpineSprite/TargetingBone"));
		_animController.GetAnimationState().SetAnimation("animation");
		((CanvasItem)_animNode).Visible = false;
	}

	public void ExtendLaser(Vector2 targetPos)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_animNode).Visible = true;
		_animController.GetAnimationState().SetAnimation("animation");
		_targetingBone.GlobalPosition = ((Node2D)this).GlobalPosition;
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_targetingBone, NodePath.op_Implicit("position"), Variant.op_Implicit(targetPos), 0.15000000596046448).SetTrans((TransitionType)5).SetEase((EaseType)1);
		val.Chain().TweenProperty((GodotObject)(object)_animNode, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.Red), 0.20000000298023224);
	}

	public void RetractLaser()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_targetingBone, NodePath.op_Implicit("position"), Variant.op_Implicit(((Node2D)this).Position), 0.15000000596046448).SetTrans((TransitionType)5).SetEase((EaseType)0);
		val.Chain().TweenProperty((GodotObject)(object)_animNode, NodePath.op_Implicit("visible"), Variant.op_Implicit(false), 0.0);
	}

	public void ResetLaser()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_targetingBone.Position = ((Node2D)this).Position;
	}

	private void SetLaserColor(Color color)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		((ShaderMaterial)_animController.GetAdditiveMaterial()).SetShaderParameter(_color, Variant.op_Implicit(color));
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
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ExtendLaser, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("targetPos"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RetractLaser, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ResetLaser, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetLaserColor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)20, StringName.op_Implicit("color"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ExtendLaser && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ExtendLaser(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RetractLaser && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RetractLaser();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ResetLaser && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ResetLaser();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetLaserColor && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetLaserColor(VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.ExtendLaser)
		{
			return true;
		}
		if ((ref method) == MethodName.RetractLaser)
		{
			return true;
		}
		if ((ref method) == MethodName.ResetLaser)
		{
			return true;
		}
		if ((ref method) == MethodName.SetLaserColor)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._animNode)
		{
			_animNode = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetingBone)
		{
			_targetingBone = VariantUtils.ConvertTo<Node2D>(ref value);
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
		if ((ref name) == PropertyName._animNode)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _animNode);
			return true;
		}
		if ((ref name) == PropertyName._targetingBone)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _targetingBone);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._animNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._targetingBone, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._animNode, Variant.From<Node2D>(ref _animNode));
		info.AddProperty(PropertyName._targetingBone, Variant.From<Node2D>(ref _targetingBone));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._animNode, ref val))
		{
			_animNode = ((Variant)(ref val)).As<Node2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetingBone, ref val2))
		{
			_targetingBone = ((Variant)(ref val2)).As<Node2D>();
		}
	}
}
