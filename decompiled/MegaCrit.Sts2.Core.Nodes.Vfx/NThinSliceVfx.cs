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

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NThinSliceVfx.cs")]
public class NThinSliceVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetColor = StringName.op_Implicit("SetColor");

		public static readonly StringName GenerateSpawnPosition = StringName.op_Implicit("GenerateSpawnPosition");

		public static readonly StringName GetAngle = StringName.op_Implicit("GetAngle");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _slash = StringName.op_Implicit("_slash");

		public static readonly StringName _sparkle = StringName.op_Implicit("_sparkle");

		public static readonly StringName _creatureCenter = StringName.op_Implicit("_creatureCenter");

		public static readonly StringName _vfxColor = StringName.op_Implicit("_vfxColor");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/vfx/thin_slice_vfx.tscn";

	private GpuParticles2D _slash;

	private GpuParticles2D _sparkle;

	private Vector2 _creatureCenter;

	private VfxColor _vfxColor;

	public static NThinSliceVfx? Create(Creature? target, VfxColor vfxColor = VfxColor.Cyan)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		Vector2 vfxSpawnPosition = NCombatRoom.Instance.GetCreatureNode(target).VfxSpawnPosition;
		NThinSliceVfx nThinSliceVfx = PreloadManager.Cache.GetScene("res://scenes/vfx/thin_slice_vfx.tscn").Instantiate<NThinSliceVfx>((GenEditState)0);
		nThinSliceVfx._vfxColor = vfxColor;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(Rng.Chaotic.NextFloat(-50f, 50f), Rng.Chaotic.NextFloat(-50f, 50f));
		nThinSliceVfx._creatureCenter = vfxSpawnPosition + val;
		return nThinSliceVfx;
	}

	public override void _Ready()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		_slash = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("Slash"));
		((Node2D)_slash).GlobalPosition = GenerateSpawnPosition();
		((Node2D)_slash).Rotation = GetAngle();
		_slash.Emitting = true;
		_sparkle = ((Node)_slash).GetNode<GpuParticles2D>(NodePath.op_Implicit("Sparkle"));
		((Node2D)_sparkle).GlobalPosition = _creatureCenter;
		_sparkle.Emitting = true;
		SetColor();
		TaskHelper.RunSafely(SelfDestruct());
	}

	private void SetColor()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		ParticleProcessMaterial val = (ParticleProcessMaterial)_slash.GetProcessMaterial();
		switch (_vfxColor)
		{
		case VfxColor.Red:
			val.Color = new Color("FF9900");
			break;
		case VfxColor.White:
			val.Color = Colors.White;
			break;
		case VfxColor.Cyan:
			val.Color = new Color("C4FFE6");
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case VfxColor.Green:
		case VfxColor.Blue:
		case VfxColor.Purple:
		case VfxColor.Black:
			break;
		}
	}

	private Vector2 GenerateSpawnPosition()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float num = Rng.Chaotic.NextFloat(0f, (float)Math.PI * 2f);
		float num2 = Rng.Chaotic.NextFloat(400f, 500f);
		return new Vector2(_creatureCenter.X + num2 * Mathf.Cos(num), _creatureCenter.Y + num2 * Mathf.Sin(num));
	}

	private float GetAngle()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = _creatureCenter - ((Node2D)_slash).GlobalPosition;
		return Mathf.Atan2(val.Y, val.X);
	}

	private async Task SelfDestruct()
	{
		await Task.Delay(1000);
		((Node)(object)this).QueueFreeSafely();
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
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetColor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GenerateSpawnPosition, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetAngle, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.GenerateSpawnPosition && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Vector2 val = GenerateSpawnPosition();
			ret = VariantUtils.CreateFrom<Vector2>(ref val);
			return true;
		}
		if ((ref method) == MethodName.GetAngle && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			float angle = GetAngle();
			ret = VariantUtils.CreateFrom<float>(ref angle);
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
		if ((ref method) == MethodName.SetColor)
		{
			return true;
		}
		if ((ref method) == MethodName.GenerateSpawnPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.GetAngle)
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
		if ((ref name) == PropertyName._slash)
		{
			_slash = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sparkle)
		{
			_sparkle = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._creatureCenter)
		{
			_creatureCenter = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._vfxColor)
		{
			_vfxColor = VariantUtils.ConvertTo<VfxColor>(ref value);
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
		if ((ref name) == PropertyName._slash)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _slash);
			return true;
		}
		if ((ref name) == PropertyName._sparkle)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _sparkle);
			return true;
		}
		if ((ref name) == PropertyName._creatureCenter)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _creatureCenter);
			return true;
		}
		if ((ref name) == PropertyName._vfxColor)
		{
			value = VariantUtils.CreateFrom<VfxColor>(ref _vfxColor);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._slash, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sparkle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._creatureCenter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._vfxColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._slash, Variant.From<GpuParticles2D>(ref _slash));
		info.AddProperty(PropertyName._sparkle, Variant.From<GpuParticles2D>(ref _sparkle));
		info.AddProperty(PropertyName._creatureCenter, Variant.From<Vector2>(ref _creatureCenter));
		info.AddProperty(PropertyName._vfxColor, Variant.From<VfxColor>(ref _vfxColor));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._slash, ref val))
		{
			_slash = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._sparkle, ref val2))
		{
			_sparkle = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._creatureCenter, ref val3))
		{
			_creatureCenter = ((Variant)(ref val3)).As<Vector2>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._vfxColor, ref val4))
		{
			_vfxColor = ((Variant)(ref val4)).As<VfxColor>();
		}
	}
}
