using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NItemThrowVfx.cs")]
public class NItemThrowVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _itemSprite = StringName.op_Implicit("_itemSprite");

		public static readonly StringName _flightTime = StringName.op_Implicit("_flightTime");

		public static readonly StringName _heightMultiplier = StringName.op_Implicit("_heightMultiplier");

		public static readonly StringName _horizontalCurve = StringName.op_Implicit("_horizontalCurve");

		public static readonly StringName _verticalCurve = StringName.op_Implicit("_verticalCurve");

		public static readonly StringName _rotationMultiplier = StringName.op_Implicit("_rotationMultiplier");

		public static readonly StringName _rotationInfluenceCurve = StringName.op_Implicit("_rotationInfluenceCurve");

		public static readonly StringName _sourcePosition = StringName.op_Implicit("_sourcePosition");

		public static readonly StringName _targetPosition = StringName.op_Implicit("_targetPosition");
	}

	public class SignalName : SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("vfx/vfx_item_throw");

	private const float _baseItemSize = 80f;

	[Export(/*Could not decode attribute arguments.*/)]
	private Sprite2D? _itemSprite;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _flightTime;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _heightMultiplier;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _horizontalCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _verticalCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _rotationMultiplier;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _rotationInfluenceCurve;

	private Vector2 _sourcePosition;

	private Vector2 _targetPosition;

	public static NItemThrowVfx? Create(Vector2 sourcePosition, Vector2 targetPosition, Texture2D? itemTexture, Vector2? scale = null)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NItemThrowVfx nItemThrowVfx = PreloadManager.Cache.GetScene(scenePath).Instantiate<NItemThrowVfx>((GenEditState)0);
		nItemThrowVfx._sourcePosition = sourcePosition;
		nItemThrowVfx._targetPosition = targetPosition;
		if (nItemThrowVfx._itemSprite != null)
		{
			((CanvasItem)nItemThrowVfx._itemSprite).Visible = false;
			((Node2D)nItemThrowVfx._itemSprite).Scale = (Vector2)(((_003F?)scale) ?? Vector2.One);
			if (itemTexture != null)
			{
				nItemThrowVfx._itemSprite.Texture = itemTexture;
				Sprite2D? itemSprite = nItemThrowVfx._itemSprite;
				((Node2D)itemSprite).Scale = ((Node2D)itemSprite).Scale * (80f / (float)itemTexture.GetWidth());
			}
		}
		return nItemThrowVfx;
	}

	public override void _Ready()
	{
		TaskHelper.RunSafely(ThrowItem());
	}

	private async Task ThrowItem()
	{
		((CanvasItem)_itemSprite).Visible = true;
		((Node2D)_itemSprite).GlobalPosition = _sourcePosition;
		((Node2D)_itemSprite).RotationDegrees = Rng.Chaotic.NextFloat(360f);
		double timer = 0.0;
		while (timer < (double)_flightTime)
		{
			double processDeltaTime = ((Node)this).GetProcessDeltaTime();
			float num = (float)(timer / (double)_flightTime);
			float num2 = _horizontalCurve.Sample(num);
			float num3 = _verticalCurve.Sample(num);
			float num4 = _rotationInfluenceCurve.Sample(num);
			((Node2D)_itemSprite).Rotate((float)Mathf.DegToRad((double)(num4 * _rotationMultiplier) * processDeltaTime));
			Vector2 globalPosition = ((Vector2)(ref _sourcePosition)).Lerp(_targetPosition, num2) + Vector2.Up * num3 * _heightMultiplier;
			((Node2D)_itemSprite).GlobalPosition = globalPosition;
			timer += processDeltaTime;
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		((CanvasItem)_itemSprite).Visible = false;
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._itemSprite)
		{
			_itemSprite = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flightTime)
		{
			_flightTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._heightMultiplier)
		{
			_heightMultiplier = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._horizontalCurve)
		{
			_horizontalCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._verticalCurve)
		{
			_verticalCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rotationMultiplier)
		{
			_rotationMultiplier = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rotationInfluenceCurve)
		{
			_rotationInfluenceCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sourcePosition)
		{
			_sourcePosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetPosition)
		{
			_targetPosition = VariantUtils.ConvertTo<Vector2>(ref value);
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
		if ((ref name) == PropertyName._itemSprite)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _itemSprite);
			return true;
		}
		if ((ref name) == PropertyName._flightTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _flightTime);
			return true;
		}
		if ((ref name) == PropertyName._heightMultiplier)
		{
			value = VariantUtils.CreateFrom<float>(ref _heightMultiplier);
			return true;
		}
		if ((ref name) == PropertyName._horizontalCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _horizontalCurve);
			return true;
		}
		if ((ref name) == PropertyName._verticalCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _verticalCurve);
			return true;
		}
		if ((ref name) == PropertyName._rotationMultiplier)
		{
			value = VariantUtils.CreateFrom<float>(ref _rotationMultiplier);
			return true;
		}
		if ((ref name) == PropertyName._rotationInfluenceCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _rotationInfluenceCurve);
			return true;
		}
		if ((ref name) == PropertyName._sourcePosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _sourcePosition);
			return true;
		}
		if ((ref name) == PropertyName._targetPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetPosition);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._itemSprite, (PropertyHint)34, "Sprite2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._flightTime, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._heightMultiplier, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._horizontalCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._verticalCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._rotationMultiplier, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._rotationInfluenceCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)5, PropertyName._sourcePosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._itemSprite, Variant.From<Sprite2D>(ref _itemSprite));
		info.AddProperty(PropertyName._flightTime, Variant.From<float>(ref _flightTime));
		info.AddProperty(PropertyName._heightMultiplier, Variant.From<float>(ref _heightMultiplier));
		info.AddProperty(PropertyName._horizontalCurve, Variant.From<Curve>(ref _horizontalCurve));
		info.AddProperty(PropertyName._verticalCurve, Variant.From<Curve>(ref _verticalCurve));
		info.AddProperty(PropertyName._rotationMultiplier, Variant.From<float>(ref _rotationMultiplier));
		info.AddProperty(PropertyName._rotationInfluenceCurve, Variant.From<Curve>(ref _rotationInfluenceCurve));
		info.AddProperty(PropertyName._sourcePosition, Variant.From<Vector2>(ref _sourcePosition));
		info.AddProperty(PropertyName._targetPosition, Variant.From<Vector2>(ref _targetPosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._itemSprite, ref val))
		{
			_itemSprite = ((Variant)(ref val)).As<Sprite2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._flightTime, ref val2))
		{
			_flightTime = ((Variant)(ref val2)).As<float>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._heightMultiplier, ref val3))
		{
			_heightMultiplier = ((Variant)(ref val3)).As<float>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._horizontalCurve, ref val4))
		{
			_horizontalCurve = ((Variant)(ref val4)).As<Curve>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._verticalCurve, ref val5))
		{
			_verticalCurve = ((Variant)(ref val5)).As<Curve>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._rotationMultiplier, ref val6))
		{
			_rotationMultiplier = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._rotationInfluenceCurve, ref val7))
		{
			_rotationInfluenceCurve = ((Variant)(ref val7)).As<Curve>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._sourcePosition, ref val8))
		{
			_sourcePosition = ((Variant)(ref val8)).As<Vector2>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetPosition, ref val9))
		{
			_targetPosition = ((Variant)(ref val9)).As<Vector2>();
		}
	}
}
