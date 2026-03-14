using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NKinPriestGrenadeVfx.cs")]
public class NKinPriestGrenadeVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _cryptoParticles = StringName.op_Implicit("_cryptoParticles");

		public static readonly StringName _noiseParticles = StringName.op_Implicit("_noiseParticles");

		public static readonly StringName _explosionBase = StringName.op_Implicit("_explosionBase");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/monsters/kin_priest_grenade_vfx");

	private GpuParticles2D _cryptoParticles;

	private GpuParticles2D _noiseParticles;

	private GpuParticles2D _explosionBase;

	private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

	public static NKinPriestGrenadeVfx? Create(Creature target)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(target);
		if (creatureNode == null)
		{
			return null;
		}
		NKinPriestGrenadeVfx nKinPriestGrenadeVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NKinPriestGrenadeVfx>((GenEditState)0);
		((Node2D)nKinPriestGrenadeVfx).GlobalPosition = creatureNode.GetBottomOfHitbox();
		return nKinPriestGrenadeVfx;
	}

	public override void _Ready()
	{
		_cryptoParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("CryptoParticles"));
		_noiseParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("NoiseParticles"));
		_explosionBase = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("ExplosionBaseParticle"));
		_cryptoParticles.Emitting = false;
		_cryptoParticles.OneShot = true;
		_noiseParticles.Emitting = false;
		_noiseParticles.OneShot = true;
		_explosionBase.Emitting = false;
		_explosionBase.OneShot = true;
		TaskHelper.RunSafely(Play());
	}

	private async Task Play()
	{
		NDebugAudioManager.Instance?.Play("blunt_attack.mp3");
		_noiseParticles.SetEmitting(true);
		_explosionBase.SetEmitting(true);
		await Task.Delay(100, _cancelToken.Token);
		_cryptoParticles.SetEmitting(true);
		await Task.Delay(5000, _cancelToken.Token);
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
		if ((ref name) == PropertyName._cryptoParticles)
		{
			_cryptoParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noiseParticles)
		{
			_noiseParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._explosionBase)
		{
			_explosionBase = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
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
		if ((ref name) == PropertyName._cryptoParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _cryptoParticles);
			return true;
		}
		if ((ref name) == PropertyName._noiseParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _noiseParticles);
			return true;
		}
		if ((ref name) == PropertyName._explosionBase)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _explosionBase);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._cryptoParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._noiseParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._explosionBase, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._cryptoParticles, Variant.From<GpuParticles2D>(ref _cryptoParticles));
		info.AddProperty(PropertyName._noiseParticles, Variant.From<GpuParticles2D>(ref _noiseParticles));
		info.AddProperty(PropertyName._explosionBase, Variant.From<GpuParticles2D>(ref _explosionBase));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._cryptoParticles, ref val))
		{
			_cryptoParticles = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._noiseParticles, ref val2))
		{
			_noiseParticles = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._explosionBase, ref val3))
		{
			_explosionBase = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
	}
}
