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
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Cards;

[ScriptPath("res://src/Core/Nodes/Vfx/Cards/NSpikeSplashVfx.cs")]
public class NSpikeSplashVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _duration = StringName.op_Implicit("_duration");

		public static readonly StringName _spikeAmount = StringName.op_Implicit("_spikeAmount");

		public static readonly StringName _spawnPosition = StringName.op_Implicit("_spawnPosition");

		public static readonly StringName _vfxColor = StringName.op_Implicit("_vfxColor");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/spike_splash_vfx");

	private float _duration = 1f;

	private int _spikeAmount = 6;

	private Vector2 _spawnPosition;

	private VfxColor _vfxColor;

	public static NSpikeSplashVfx? Create(Creature target, VfxColor vfxColor = VfxColor.Red)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NSpikeSplashVfx nSpikeSplashVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NSpikeSplashVfx>((GenEditState)0);
		nSpikeSplashVfx._spawnPosition = NCombatRoom.Instance.GetCreatureNode(target).GetBottomOfHitbox();
		nSpikeSplashVfx._vfxColor = vfxColor;
		return nSpikeSplashVfx;
	}

	public override void _Ready()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _spikeAmount; i++)
		{
			NFgGroundSpikeVfx child = NFgGroundSpikeVfx.Create(_spawnPosition, movingRight: true, _vfxColor);
			((Node)(object)NCombatRoom.Instance.CombatVfxContainer).AddChildSafely((Node?)(object)child);
			child = NFgGroundSpikeVfx.Create(_spawnPosition, movingRight: false, _vfxColor);
			((Node)(object)NCombatRoom.Instance.CombatVfxContainer).AddChildSafely((Node?)(object)child);
		}
		for (int j = 0; j < _spikeAmount; j++)
		{
			NBgGroundSpikeVfx child2 = NBgGroundSpikeVfx.Create(_spawnPosition, movingRight: true, _vfxColor);
			NCombatRoom.Instance.BackCombatVfxContainer.AddChildSafely((Node?)(object)child2);
			child2 = NBgGroundSpikeVfx.Create(_spawnPosition, movingRight: false, _vfxColor);
			NCombatRoom.Instance.BackCombatVfxContainer.AddChildSafely((Node?)(object)child2);
		}
		TaskHelper.RunSafely(SelfDestruct());
	}

	private async Task SelfDestruct()
	{
		await Task.Delay(2000);
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
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._duration)
		{
			_duration = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spikeAmount)
		{
			_spikeAmount = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spawnPosition)
		{
			_spawnPosition = VariantUtils.ConvertTo<Vector2>(ref value);
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
		if ((ref name) == PropertyName._duration)
		{
			value = VariantUtils.CreateFrom<float>(ref _duration);
			return true;
		}
		if ((ref name) == PropertyName._spikeAmount)
		{
			value = VariantUtils.CreateFrom<int>(ref _spikeAmount);
			return true;
		}
		if ((ref name) == PropertyName._spawnPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _spawnPosition);
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._duration, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._spikeAmount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._spawnPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._duration, Variant.From<float>(ref _duration));
		info.AddProperty(PropertyName._spikeAmount, Variant.From<int>(ref _spikeAmount));
		info.AddProperty(PropertyName._spawnPosition, Variant.From<Vector2>(ref _spawnPosition));
		info.AddProperty(PropertyName._vfxColor, Variant.From<VfxColor>(ref _vfxColor));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._duration, ref val))
		{
			_duration = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._spikeAmount, ref val2))
		{
			_spikeAmount = ((Variant)(ref val2)).As<int>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._spawnPosition, ref val3))
		{
			_spawnPosition = ((Variant)(ref val3)).As<Vector2>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._vfxColor, ref val4))
		{
			_vfxColor = ((Variant)(ref val4)).As<VfxColor>();
		}
	}
}
