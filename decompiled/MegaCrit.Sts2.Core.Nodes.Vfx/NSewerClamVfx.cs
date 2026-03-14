using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NSewerClamVfx.cs")]
public class NSewerClamVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ScaleCoralTo = StringName.op_Implicit("ScaleCoralTo");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName OnDeathStart = StringName.op_Implicit("OnDeathStart");

		public static readonly StringName OnDeathEnd = StringName.op_Implicit("OnDeathEnd");

		public static readonly StringName OnDarknessStart = StringName.op_Implicit("OnDarknessStart");

		public static readonly StringName OnDarknessEnd = StringName.op_Implicit("OnDarknessEnd");

		public static readonly StringName OnChomp = StringName.op_Implicit("OnChomp");

		public static readonly StringName OnGrow = StringName.op_Implicit("OnGrow");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _deathParticles = StringName.op_Implicit("_deathParticles");

		public static readonly StringName _buffParticles = StringName.op_Implicit("_buffParticles");

		public static readonly StringName _chompParticles = StringName.op_Implicit("_chompParticles");

		public static readonly StringName _scaleNode = StringName.op_Implicit("_scaleNode");

		public static readonly StringName _keyDown = StringName.op_Implicit("_keyDown");

		public static readonly StringName _onState = StringName.op_Implicit("_onState");
	}

	public class SignalName : SignalName
	{
	}

	private const float _coralScaleAmount = 0.2f;

	private const float _maxCoralScale = 1.5f;

	private const float _coralTweenDelay = 0.5f;

	private MegaSprite _megaSprite;

	private GpuParticles2D _deathParticles;

	private GpuParticles2D _buffParticles;

	private GpuParticles2D _chompParticles;

	private Node2D _scaleNode;

	private bool _keyDown;

	private bool _onState;

	public override void _Ready()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		_megaSprite = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)this).GetParent<Node2D>()));
		_megaSprite.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_deathParticles = ((Node)this).GetParent().GetNode<GpuParticles2D>(NodePath.op_Implicit("MouthSlot/DeathParticles"));
		_buffParticles = ((Node)this).GetParent().GetNode<GpuParticles2D>(NodePath.op_Implicit("MouthSlot/BuffParticles"));
		_chompParticles = ((Node)this).GetParent().GetNode<GpuParticles2D>(NodePath.op_Implicit("MouthSlot/ChompParticles"));
		_scaleNode = ((Node)this).GetParent().GetNode<Node2D>(NodePath.op_Implicit("CoralScaleBone"));
		_deathParticles.Emitting = false;
		_deathParticles.OneShot = true;
		_buffParticles.Emitting = false;
		_chompParticles.OneShot = true;
		_chompParticles.Emitting = false;
		_scaleNode.Scale = new Vector2(0.1f, 0.1f);
	}

	private void ScaleCoralTo(float targetScale)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(targetScale - 0.2f, targetScale - 0.2f);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(targetScale, targetScale);
		Tween val3 = ((Node)this).CreateTween();
		val3.SetEase((EaseType)1).SetTrans((TransitionType)6);
		val3.TweenProperty((GodotObject)(object)_scaleNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(val2), 0.5).From(Variant.op_Implicit(val)).SetDelay(0.5);
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		switch (new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName())
		{
		case "death_explode":
			OnDeathStart();
			break;
		case "darkness_start":
			OnDarknessStart();
			break;
		case "darkness_end":
			OnDarknessEnd();
			break;
		case "chomp":
			OnChomp();
			break;
		case "grow":
			OnGrow();
			break;
		}
	}

	private void OnDeathStart()
	{
		_deathParticles.Restart();
	}

	private void OnDeathEnd()
	{
		_deathParticles.Emitting = false;
	}

	private void OnDarknessStart()
	{
		_buffParticles.Restart();
	}

	private void OnDarknessEnd()
	{
		_buffParticles.Emitting = false;
	}

	private void OnChomp()
	{
		_chompParticles.Restart();
	}

	private void OnGrow()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		float num = _scaleNode.Scale.X + 0.2f;
		if (num >= 1.5f)
		{
			num = 1.5f;
		}
		ScaleCoralTo(num);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ScaleCoralTo, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("targetScale"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDeathStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDeathEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDarknessStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDarknessEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnChomp, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnGrow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ScaleCoralTo && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ScaleCoralTo(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAnimationEvent && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			OnAnimationEvent(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDeathStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDeathStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDeathEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDeathEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDarknessStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDarknessStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDarknessEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDarknessEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnChomp && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnChomp();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnGrow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnGrow();
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
		if ((ref method) == MethodName.ScaleCoralTo)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAnimationEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDeathStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDeathEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDarknessStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDarknessEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnChomp)
		{
			return true;
		}
		if ((ref method) == MethodName.OnGrow)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._deathParticles)
		{
			_deathParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._buffParticles)
		{
			_buffParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._chompParticles)
		{
			_chompParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scaleNode)
		{
			_scaleNode = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._keyDown)
		{
			_keyDown = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._onState)
		{
			_onState = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._deathParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _deathParticles);
			return true;
		}
		if ((ref name) == PropertyName._buffParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _buffParticles);
			return true;
		}
		if ((ref name) == PropertyName._chompParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _chompParticles);
			return true;
		}
		if ((ref name) == PropertyName._scaleNode)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _scaleNode);
			return true;
		}
		if ((ref name) == PropertyName._keyDown)
		{
			value = VariantUtils.CreateFrom<bool>(ref _keyDown);
			return true;
		}
		if ((ref name) == PropertyName._onState)
		{
			value = VariantUtils.CreateFrom<bool>(ref _onState);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._deathParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._buffParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._chompParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scaleNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._keyDown, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._onState, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._deathParticles, Variant.From<GpuParticles2D>(ref _deathParticles));
		info.AddProperty(PropertyName._buffParticles, Variant.From<GpuParticles2D>(ref _buffParticles));
		info.AddProperty(PropertyName._chompParticles, Variant.From<GpuParticles2D>(ref _chompParticles));
		info.AddProperty(PropertyName._scaleNode, Variant.From<Node2D>(ref _scaleNode));
		info.AddProperty(PropertyName._keyDown, Variant.From<bool>(ref _keyDown));
		info.AddProperty(PropertyName._onState, Variant.From<bool>(ref _onState));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._deathParticles, ref val))
		{
			_deathParticles = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._buffParticles, ref val2))
		{
			_buffParticles = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._chompParticles, ref val3))
		{
			_chompParticles = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._scaleNode, ref val4))
		{
			_scaleNode = ((Variant)(ref val4)).As<Node2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._keyDown, ref val5))
		{
			_keyDown = ((Variant)(ref val5)).As<bool>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._onState, ref val6))
		{
			_onState = ((Variant)(ref val6)).As<bool>();
		}
	}
}
