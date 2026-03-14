using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Cards;

[ScriptPath("res://src/Core/Nodes/Vfx/Cards/NBolasVfx.cs")]
public class NBolasVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName FollowCurve = StringName.op_Implicit("FollowCurve");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _bola2 = StringName.op_Implicit("_bola2");

		public static readonly StringName _bola3 = StringName.op_Implicit("_bola3");

		public static readonly StringName _startPosition = StringName.op_Implicit("_startPosition");

		public static readonly StringName _controlPosition = StringName.op_Implicit("_controlPosition");

		public static readonly StringName _endPosition = StringName.op_Implicit("_endPosition");

		public static readonly StringName _rotationSpeed = StringName.op_Implicit("_rotationSpeed");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/cards/bolas_vfx");

	private Node2D _bola2;

	private Node2D _bola3;

	private Vector2 _startPosition;

	private Vector2 _controlPosition;

	private Vector2 _endPosition;

	private float _rotationSpeed = 30f;

	private Tween? _tween;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NBolasVfx? Create(Creature owner, Creature target)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		if (target.IsDead)
		{
			return null;
		}
		NBolasVfx nBolasVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NBolasVfx>((GenEditState)0);
		((Node2D)nBolasVfx).GlobalPosition = NCombatRoom.Instance.GetCreatureNode(owner).VfxSpawnPosition;
		nBolasVfx._startPosition = ((Node2D)nBolasVfx).GlobalPosition;
		nBolasVfx._endPosition = NCombatRoom.Instance.GetCreatureNode(target).VfxSpawnPosition;
		float num = Mathf.Min(nBolasVfx._startPosition.Y, nBolasVfx._endPosition.Y) - Rng.Chaotic.NextFloat(400f, 500f);
		nBolasVfx._controlPosition = new Vector2((nBolasVfx._startPosition.X + nBolasVfx._endPosition.X) * 0.5f, num);
		return nBolasVfx;
	}

	public override void _Ready()
	{
		TaskHelper.RunSafely(FlyBolasFly());
		_bola2 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("Bola2"));
		_bola3 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("Bola3"));
	}

	public override void _ExitTree()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
	}

	private async Task FlyBolasFly()
	{
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25).From(Variant.op_Implicit(0f));
		_tween.TweenMethod(Callable.From<float>((Action<float>)FollowCurve), Variant.op_Implicit(0f), Variant.op_Implicit(1f), 0.6000000238418579).SetEase((EaseType)1).SetTrans((TransitionType)1);
		_tween.Chain();
		_tween.TweenInterval(0.15000000596046448);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.15000000596046448);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	private void FollowCurve(float progressPercent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		((Node2D)this).GlobalPosition = MathHelper.BezierCurve(_startPosition, _endPosition, _controlPosition, progressPercent);
	}

	public override void _Process(double delta)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)delta;
		((Node2D)this).Rotation = ((Node2D)this).Rotation + num * (0f - _rotationSpeed);
		_rotationSpeed -= num * 12f;
		Node2D bola = _bola2;
		bola.Position -= new Vector2(150f * num, 0f);
		Node2D bola2 = _bola3;
		bola2.Position -= new Vector2(-150f * num, 0f);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FollowCurve, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("progressPercent"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FollowCurve && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			FollowCurve(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.FollowCurve)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._bola2)
		{
			_bola2 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bola3)
		{
			_bola3 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._startPosition)
		{
			_startPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._controlPosition)
		{
			_controlPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._endPosition)
		{
			_endPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rotationSpeed)
		{
			_rotationSpeed = VariantUtils.ConvertTo<float>(ref value);
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
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._bola2)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _bola2);
			return true;
		}
		if ((ref name) == PropertyName._bola3)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _bola3);
			return true;
		}
		if ((ref name) == PropertyName._startPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _startPosition);
			return true;
		}
		if ((ref name) == PropertyName._controlPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _controlPosition);
			return true;
		}
		if ((ref name) == PropertyName._endPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _endPosition);
			return true;
		}
		if ((ref name) == PropertyName._rotationSpeed)
		{
			value = VariantUtils.CreateFrom<float>(ref _rotationSpeed);
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
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._bola2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bola3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._startPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._controlPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._endPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._rotationSpeed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._bola2, Variant.From<Node2D>(ref _bola2));
		info.AddProperty(PropertyName._bola3, Variant.From<Node2D>(ref _bola3));
		info.AddProperty(PropertyName._startPosition, Variant.From<Vector2>(ref _startPosition));
		info.AddProperty(PropertyName._controlPosition, Variant.From<Vector2>(ref _controlPosition));
		info.AddProperty(PropertyName._endPosition, Variant.From<Vector2>(ref _endPosition));
		info.AddProperty(PropertyName._rotationSpeed, Variant.From<float>(ref _rotationSpeed));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._bola2, ref val))
		{
			_bola2 = ((Variant)(ref val)).As<Node2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._bola3, ref val2))
		{
			_bola3 = ((Variant)(ref val2)).As<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._startPosition, ref val3))
		{
			_startPosition = ((Variant)(ref val3)).As<Vector2>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._controlPosition, ref val4))
		{
			_controlPosition = ((Variant)(ref val4)).As<Vector2>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._endPosition, ref val5))
		{
			_endPosition = ((Variant)(ref val5)).As<Vector2>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._rotationSpeed, ref val6))
		{
			_rotationSpeed = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val7))
		{
			_tween = ((Variant)(ref val7)).As<Tween>();
		}
	}
}
