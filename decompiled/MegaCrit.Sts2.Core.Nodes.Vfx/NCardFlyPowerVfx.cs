using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NCardFlyPowerVfx.cs")]
public class NCardFlyPowerVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName GetDuration = StringName.op_Implicit("GetDuration");

		public static readonly StringName GetDurationInternal = StringName.op_Implicit("GetDurationInternal");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName CardNode = StringName.op_Implicit("CardNode");

		public static readonly StringName _cardOwnerNode = StringName.op_Implicit("_cardOwnerNode");

		public static readonly StringName _vfx = StringName.op_Implicit("_vfx");

		public static readonly StringName _swooshPath = StringName.op_Implicit("_swooshPath");
	}

	public class SignalName : SignalName
	{
	}

	private const float _speed = 3000f;

	private const float _scaleOutProportion = 0.9f;

	private const float _initialRotationSpeed = (float)Math.PI;

	private const float _maxRotationSpeed = (float)Math.PI * 50f;

	private NCreature _cardOwnerNode;

	private NCardTrailVfx? _vfx;

	private Path2D _swooshPath;

	private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/vfx_card_power_fly");

	public NCard CardNode { get; private set; }

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NCardFlyPowerVfx? Create(NCard card)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NCardFlyPowerVfx nCardFlyPowerVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NCardFlyPowerVfx>((GenEditState)0);
		nCardFlyPowerVfx.CardNode = card;
		return nCardFlyPowerVfx;
	}

	public override void _Ready()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		((Node2D)this).GlobalPosition = ((Control)CardNode).GlobalPosition;
		Player owner = CardNode.Model.Owner;
		_cardOwnerNode = NCombatRoom.Instance.GetCreatureNode(owner.Creature);
		_vfx = NCardTrailVfx.Create((Control)(object)CardNode, owner.Character.TrailPath);
		if (_vfx != null)
		{
			((Node)(object)this).AddChildSafely((Node?)(object)_vfx);
		}
		Vector2 vfxSpawnPosition = _cardOwnerNode.VfxSpawnPosition;
		Vector2 val = vfxSpawnPosition - ((Node2D)this).GlobalPosition;
		_swooshPath = ((Node)this).GetNode<Path2D>(NodePath.op_Implicit("SwooshPath"));
		_swooshPath.Curve.SetPointPosition(_swooshPath.Curve.PointCount - 1, val);
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		_cancelToken.Cancel();
		_cancelToken.Dispose();
	}

	public float GetDuration()
	{
		return GetDurationInternal() + 0.05f;
	}

	private float GetDurationInternal()
	{
		return _swooshPath.Curve.GetBakedLength() / 3000f;
	}

	public async Task PlayAnim()
	{
		((Node)this).CreateTween().TweenProperty((GodotObject)(object)CardNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.1f), 0.30000001192092896);
		float length = _swooshPath.Curve.GetBakedLength();
		double timeAccumulator = 0.0;
		float duration = GetDurationInternal();
		while (timeAccumulator < (double)duration)
		{
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			if (_cancelToken.IsCancellationRequested)
			{
				break;
			}
			double processDeltaTime = ((Node)this).GetProcessDeltaTime();
			timeAccumulator += processDeltaTime;
			float num = (float)(timeAccumulator / (double)duration);
			float num2 = Ease.QuadIn(num);
			Transform2D val = _swooshPath.Curve.SampleBakedWithRotation(num2 * length, false);
			((Control)CardNode).GlobalPosition = ((Node2D)this).GlobalPosition + val.Origin;
			float num3 = ((Transform2D)(ref val)).Rotation - ((Control)CardNode).Rotation;
			float num4 = Mathf.Lerp((float)Math.PI, (float)Math.PI * 50f, num);
			NCard cardNode = CardNode;
			((Control)cardNode).Rotation = ((Control)cardNode).Rotation + (float)Mathf.Sign(num3) * Mathf.Min(Mathf.Abs(num3), (float)((double)num4 * processDeltaTime));
			if (num >= 0.9f)
			{
				((Node)this).CreateTween().TweenProperty((GodotObject)(object)CardNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.Zero), (double)duration - timeAccumulator);
			}
		}
		NGame.Instance.ScreenShake(ShakeStrength.Medium, ShakeDuration.Short);
		if (_vfx != null)
		{
			await _vfx.FadeOut();
		}
		((Node)(object)CardNode).QueueFreeSafely();
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetDuration, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetDurationInternal, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NCardFlyPowerVfx nCardFlyPowerVfx = Create(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NCardFlyPowerVfx>(ref nCardFlyPowerVfx);
			return true;
		}
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
		if ((ref method) == MethodName.GetDuration && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			float duration = GetDuration();
			ret = VariantUtils.CreateFrom<float>(ref duration);
			return true;
		}
		if ((ref method) == MethodName.GetDurationInternal && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			float durationInternal = GetDurationInternal();
			ret = VariantUtils.CreateFrom<float>(ref durationInternal);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NCardFlyPowerVfx nCardFlyPowerVfx = Create(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NCardFlyPowerVfx>(ref nCardFlyPowerVfx);
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.GetDuration)
		{
			return true;
		}
		if ((ref method) == MethodName.GetDurationInternal)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.CardNode)
		{
			CardNode = VariantUtils.ConvertTo<NCard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardOwnerNode)
		{
			_cardOwnerNode = VariantUtils.ConvertTo<NCreature>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._vfx)
		{
			_vfx = VariantUtils.ConvertTo<NCardTrailVfx>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._swooshPath)
		{
			_swooshPath = VariantUtils.ConvertTo<Path2D>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CardNode)
		{
			NCard cardNode = CardNode;
			value = VariantUtils.CreateFrom<NCard>(ref cardNode);
			return true;
		}
		if ((ref name) == PropertyName._cardOwnerNode)
		{
			value = VariantUtils.CreateFrom<NCreature>(ref _cardOwnerNode);
			return true;
		}
		if ((ref name) == PropertyName._vfx)
		{
			value = VariantUtils.CreateFrom<NCardTrailVfx>(ref _vfx);
			return true;
		}
		if ((ref name) == PropertyName._swooshPath)
		{
			value = VariantUtils.CreateFrom<Path2D>(ref _swooshPath);
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
		list.Add(new PropertyInfo((Type)24, PropertyName.CardNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardOwnerNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._vfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._swooshPath, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName cardNode = PropertyName.CardNode;
		NCard cardNode2 = CardNode;
		info.AddProperty(cardNode, Variant.From<NCard>(ref cardNode2));
		info.AddProperty(PropertyName._cardOwnerNode, Variant.From<NCreature>(ref _cardOwnerNode));
		info.AddProperty(PropertyName._vfx, Variant.From<NCardTrailVfx>(ref _vfx));
		info.AddProperty(PropertyName._swooshPath, Variant.From<Path2D>(ref _swooshPath));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.CardNode, ref val))
		{
			CardNode = ((Variant)(ref val)).As<NCard>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardOwnerNode, ref val2))
		{
			_cardOwnerNode = ((Variant)(ref val2)).As<NCreature>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._vfx, ref val3))
		{
			_vfx = ((Variant)(ref val3)).As<NCardTrailVfx>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._swooshPath, ref val4))
		{
			_swooshPath = ((Variant)(ref val4)).As<Path2D>();
		}
	}
}
