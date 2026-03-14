using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NBgGroundSpikeVfx.cs")]
public class NBgGroundSpikeVfx : Sprite2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetColor = StringName.op_Implicit("SetColor");

		public static readonly StringName AdjustStartPosition = StringName.op_Implicit("AdjustStartPosition");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _startPosition = StringName.op_Implicit("_startPosition");

		public static readonly StringName _movingRight = StringName.op_Implicit("_movingRight");

		public static readonly StringName _vfxColor = StringName.op_Implicit("_vfxColor");

		public static readonly StringName _velocity = StringName.op_Implicit("_velocity");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/vfx/bg_ground_spike_vfx.tscn";

	protected Vector2 _startPosition;

	protected bool _movingRight = true;

	protected VfxColor _vfxColor;

	private Vector2 _velocity;

	private Tween? _tween;

	public static NBgGroundSpikeVfx? Create(Vector2 position, bool movingRight = true, VfxColor vfxColor = VfxColor.Red)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NBgGroundSpikeVfx nBgGroundSpikeVfx = PreloadManager.Cache.GetScene("res://scenes/vfx/bg_ground_spike_vfx.tscn").Instantiate<NBgGroundSpikeVfx>((GenEditState)0);
		nBgGroundSpikeVfx._startPosition = position;
		nBgGroundSpikeVfx._movingRight = movingRight;
		nBgGroundSpikeVfx._vfxColor = vfxColor;
		return nBgGroundSpikeVfx;
	}

	public override void _Ready()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		((Node2D)this).Skew = (_movingRight ? (Rng.Chaotic.NextFloat(15f, 30f) * 0.0174533f) : (Rng.Chaotic.NextFloat(-30f, -15f) * 0.0174533f));
		float num = Rng.Chaotic.NextFloat(0.5f, 1.5f);
		((Node2D)this).Scale = new Vector2(Rng.Chaotic.NextFloat(0.8f, 1.2f), Rng.Chaotic.NextFloat(0.8f, 2f)) * num;
		AdjustStartPosition();
		((Node2D)this).GlobalPosition = _startPosition;
		_velocity = new Vector2(_movingRight ? Rng.Chaotic.NextFloat(50f, 250f) : Rng.Chaotic.NextFloat(-250f, -50f), Rng.Chaotic.NextFloat(-5f, 5f)) / num;
		SetColor();
		TaskHelper.RunSafely(Animate());
	}

	private void SetColor()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		switch (_vfxColor)
		{
		case VfxColor.Red:
			((CanvasItem)this).Modulate = new Color(1f, Rng.Chaotic.NextFloat(0.2f, 0.8f), Rng.Chaotic.NextFloat(0f, 0.2f), 0.5f);
			break;
		case VfxColor.Purple:
			((CanvasItem)this).Modulate = new Color(Rng.Chaotic.NextFloat(0f, 0.2f), Rng.Chaotic.NextFloat(0.2f, 0.8f), 1f, 0.5f);
			break;
		case VfxColor.White:
		{
			float num3 = Rng.Chaotic.NextFloat(0.2f, 0.8f);
			((CanvasItem)this).Modulate = new Color(num3, num3, num3, 0.5f);
			break;
		}
		case VfxColor.Cyan:
		{
			float num2 = Rng.Chaotic.NextFloat(0.6f, 1f);
			((CanvasItem)this).Modulate = new Color(0.2f, num2, num2, 0.5f);
			break;
		}
		case VfxColor.Gold:
		{
			float num = Rng.Chaotic.NextFloat(0.6f, 1f);
			((CanvasItem)this).Modulate = new Color(num, num, 0.2f, 1f);
			break;
		}
		default:
			Log.Error("Color: " + _vfxColor.ToString() + " not implemented");
			throw new ArgumentOutOfRangeException();
		}
	}

	protected virtual void AdjustStartPosition()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		_startPosition += new Vector2(_movingRight ? Rng.Chaotic.NextFloat(40f, 160f) : Rng.Chaotic.NextFloat(-160f, -40f), Rng.Chaotic.NextFloat(-96f, -10f));
	}

	public override void _ExitTree()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
	}

	private async Task Animate()
	{
		float num = Rng.Chaotic.NextFloat(0.25f, 1f);
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenInterval((double)Rng.Chaotic.NextFloat(0.01f, 0.2f));
		_tween.Chain();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("skew"), Variant.op_Implicit(_movingRight ? (Rng.Chaotic.NextFloat(30f, 60f) * 0.0174533f) : (Rng.Chaotic.NextFloat(-60f, -30f) * 0.0174533f)), (double)num).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(((Node2D)this).Scale * Rng.Chaotic.NextFloat(0.1f, 0.5f)), (double)num).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), (double)num).SetEase((EaseType)1).SetTrans((TransitionType)5);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	public override void _Process(double delta)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		((Node2D)this).Position = ((Node2D)this).Position + _velocity * (float)delta;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Sprite2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("position"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("movingRight"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("vfxColor"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetColor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AdjustStartPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			NBgGroundSpikeVfx nBgGroundSpikeVfx = Create(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<VfxColor>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<NBgGroundSpikeVfx>(ref nBgGroundSpikeVfx);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetColor && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetColor();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AdjustStartPosition && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AdjustStartPosition();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Sprite2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			NBgGroundSpikeVfx nBgGroundSpikeVfx = Create(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<VfxColor>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<NBgGroundSpikeVfx>(ref nBgGroundSpikeVfx);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.SetColor)
		{
			return true;
		}
		if ((ref method) == MethodName.AdjustStartPosition)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		return ((Sprite2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._startPosition)
		{
			_startPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._movingRight)
		{
			_movingRight = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._vfxColor)
		{
			_vfxColor = VariantUtils.ConvertTo<VfxColor>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._velocity)
		{
			_velocity = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._startPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _startPosition);
			return true;
		}
		if ((ref name) == PropertyName._movingRight)
		{
			value = VariantUtils.CreateFrom<bool>(ref _movingRight);
			return true;
		}
		if ((ref name) == PropertyName._vfxColor)
		{
			value = VariantUtils.CreateFrom<VfxColor>(ref _vfxColor);
			return true;
		}
		if ((ref name) == PropertyName._velocity)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _velocity);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)5, PropertyName._startPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._movingRight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._vfxColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._velocity, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._startPosition, Variant.From<Vector2>(ref _startPosition));
		info.AddProperty(PropertyName._movingRight, Variant.From<bool>(ref _movingRight));
		info.AddProperty(PropertyName._vfxColor, Variant.From<VfxColor>(ref _vfxColor));
		info.AddProperty(PropertyName._velocity, Variant.From<Vector2>(ref _velocity));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._startPosition, ref val))
		{
			_startPosition = ((Variant)(ref val)).As<Vector2>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._movingRight, ref val2))
		{
			_movingRight = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._vfxColor, ref val3))
		{
			_vfxColor = ((Variant)(ref val3)).As<VfxColor>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._velocity, ref val4))
		{
			_velocity = ((Variant)(ref val4)).As<Vector2>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val5))
		{
			_tween = ((Variant)(ref val5)).As<Tween>();
		}
	}
}
