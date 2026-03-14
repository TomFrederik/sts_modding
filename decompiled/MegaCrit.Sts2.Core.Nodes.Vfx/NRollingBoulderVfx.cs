using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NRollingBoulderVfx.cs")]
public class NRollingBoulderVfx : Node2D
{
	[Signal]
	public delegate void HitCreatureEventHandler(NCreature creature);

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName CleanUpBeforeEarlyExit = StringName.op_Implicit("CleanUpBeforeEarlyExit");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _boulder = StringName.op_Implicit("_boulder");

		public static readonly StringName _shadow = StringName.op_Implicit("_shadow");

		public static readonly StringName _slamBehind = StringName.op_Implicit("_slamBehind");

		public static readonly StringName _slamFront = StringName.op_Implicit("_slamFront");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName HitCreature = StringName.op_Implicit("HitCreature");
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/vfx_rolling_boulder");

	private const float _maxScale = 1.5f;

	private const decimal _halfScaleDamage = 40m;

	private const float _maxTimeToImpact = 0.5f;

	private const float _minTimeToImpact = 0.25f;

	private const float _minRotationSpeed = 600f;

	private const float _maxRotationSpeed = 1000f;

	private const float _minXOffset = 600f;

	private const float _maxXOffset = 1000f;

	private Sprite2D _boulder;

	private Sprite2D _shadow;

	private GpuParticles2D _slamBehind;

	private GpuParticles2D _slamFront;

	private List<Creature> _creatures;

	private Vector2? _debugFinalPosition;

	private decimal _damage;

	private HitCreatureEventHandler backing_HitCreature;

	public static string[] AssetPaths => new string[1] { _scenePath };

	public event HitCreatureEventHandler HitCreature
	{
		add
		{
			backing_HitCreature = (HitCreatureEventHandler)Delegate.Combine(backing_HitCreature, value);
		}
		remove
		{
			backing_HitCreature = (HitCreatureEventHandler)Delegate.Remove(backing_HitCreature, value);
		}
	}

	public static NRollingBoulderVfx? Create(IEnumerable<Creature> creatures, decimal damage, Vector2? debugFinalPosition = null)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NRollingBoulderVfx nRollingBoulderVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NRollingBoulderVfx>((GenEditState)0);
		nRollingBoulderVfx._creatures = creatures.ToList();
		nRollingBoulderVfx._damage = damage;
		nRollingBoulderVfx._debugFinalPosition = debugFinalPosition;
		return nRollingBoulderVfx;
	}

	public override void _Ready()
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		_boulder = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("Boulder"));
		_shadow = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("Shadow"));
		_slamBehind = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("SlamBehind"));
		_slamFront = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("SlamFront"));
		float num = (float)(_damage / (_damage + 40m));
		float num2 = num * 1.5f;
		float timeToImpact = Mathf.Lerp(0.5f, 0.25f, num);
		float rotationSpeed = Mathf.Lerp(600f, 1000f, num);
		float xOffset = Mathf.Lerp(600f, 1000f, num);
		((Node2D)this).Scale = Vector2.One * num2;
		foreach (GpuParticles2D item in ((IEnumerable)((Node)this).GetChildren(false)).OfType<GpuParticles2D>())
		{
			if (!item.LocalCoords)
			{
				Material processMaterial = item.ProcessMaterial;
				ParticleProcessMaterial val = (ParticleProcessMaterial)(object)((processMaterial is ParticleProcessMaterial) ? processMaterial : null);
				if (val != null)
				{
					val.Scale = Vector2.One * num2;
				}
			}
		}
		TaskHelper.RunSafely(PlayAnim(timeToImpact, xOffset, rotationSpeed));
	}

	private async Task PlayAnim(float timeToImpact, float xOffset, float rotationSpeed)
	{
		if (NCombatRoom.Instance == null || _creatures.Count == 0)
		{
			CleanUpBeforeEarlyExit();
			return;
		}
		Vector2 initialBoulderPosition = ((Node2D)this).GlobalPosition;
		Vector2 initialShadowOffset = ((Node2D)_shadow).Position;
		Vector2 initialShadowScale = ((Node2D)_shadow).Scale;
		Vector2 val = Vector2.Zero;
		List<Creature> creaturesHit = new List<Creature>();
		int num = 0;
		foreach (Creature creature in _creatures)
		{
			NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(creature);
			if (creatureNode != null)
			{
				val += ((Node2D)creatureNode.Visuals).GlobalPosition;
				num++;
			}
		}
		val /= (float)num;
		((Node)_shadow).Reparent(NCombatRoom.Instance.BackCombatVfxContainer, true);
		if (val == Vector2.Zero)
		{
			if (!_debugFinalPosition.HasValue)
			{
				CleanUpBeforeEarlyExit();
				return;
			}
			val = _debugFinalPosition.Value;
		}
		Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
		Vector2 impactPoint = new Vector2(((Rect2)(ref viewportRect)).Size.X * 0.5f, val.Y);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(impactPoint.X - xOffset, -150f);
		float num2 = (impactPoint.X - val2.X) / timeToImpact;
		float yAccel = 2f * (impactPoint.Y - val2.Y) / (timeToImpact * timeToImpact);
		((Node2D)_shadow).Scale = Vector2.Zero;
		((Node2D)this).GlobalPosition = val2;
		Vector2 velocity = new Vector2(num2, 0f);
		float timer = 0f;
		bool firstImpact = false;
		while (timer <= 1f)
		{
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			float num3 = (float)((Node)this).GetProcessDeltaTime();
			velocity.Y += yAccel * num3;
			((Node2D)this).GlobalPosition = ((Node2D)this).GlobalPosition + velocity * num3;
			Sprite2D boulder = _boulder;
			((Node2D)boulder).Rotation = ((Node2D)boulder).Rotation + Mathf.DegToRad(rotationSpeed) * num3;
			((Node2D)_shadow).Scale = initialShadowScale * ((Node2D)this).Scale * Mathf.InverseLerp(initialBoulderPosition.Y, impactPoint.Y, ((Node2D)this).GlobalPosition.Y);
			((Node2D)_shadow).GlobalPosition = new Vector2(((Node2D)this).GlobalPosition.X, impactPoint.Y) + initialShadowOffset * ((Node2D)this).Scale;
			foreach (Creature creature2 in _creatures)
			{
				if (creaturesHit.Contains(creature2))
				{
					continue;
				}
				NCreature creatureNode2 = NCombatRoom.Instance.GetCreatureNode(creature2);
				if (creatureNode2 == null)
				{
					creaturesHit.Add(creature2);
					continue;
				}
				float num4 = ((Node2D)creatureNode2.Visuals).GlobalPosition.X - creatureNode2.Visuals.Bounds.Size.X * 0.5f;
				if (((Node2D)this).GlobalPosition.X >= num4)
				{
					EmitSignalHitCreature(creatureNode2);
					creaturesHit.Add(creature2);
				}
			}
			if (_creatures.Count == creaturesHit.Count)
			{
				timer += num3;
			}
			if (((Node2D)this).GlobalPosition.Y >= impactPoint.Y)
			{
				if (!firstImpact)
				{
					((Node2D)_slamBehind).GlobalPosition = impactPoint;
					((Node2D)_slamFront).GlobalPosition = impactPoint;
					_slamBehind.Emitting = true;
					_slamFront.Emitting = true;
					firstImpact = true;
				}
				velocity.Y = (0f - velocity.Y) * 0.33f;
				if (Mathf.Abs(velocity.Y) < 1f)
				{
					velocity.Y = 0f;
					yAccel = 0f;
				}
				NRollingBoulderVfx nRollingBoulderVfx = this;
				Vector2 globalPosition = ((Node2D)this).GlobalPosition;
				globalPosition.Y = impactPoint.Y;
				((Node2D)nRollingBoulderVfx).GlobalPosition = globalPosition;
			}
		}
		((Node)(object)_shadow).QueueFreeSafely();
		((Node)(object)this).QueueFreeSafely();
	}

	private void CleanUpBeforeEarlyExit()
	{
		Log.Warn("Rolling boulder VFX spawned with no targets, disabling");
		((Node)(object)_shadow).QueueFreeSafely();
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CleanUpBeforeEarlyExit, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CleanUpBeforeEarlyExit && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CleanUpBeforeEarlyExit();
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
		if ((ref method) == MethodName.CleanUpBeforeEarlyExit)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._boulder)
		{
			_boulder = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shadow)
		{
			_shadow = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._slamBehind)
		{
			_slamBehind = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._slamFront)
		{
			_slamFront = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
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
		if ((ref name) == PropertyName._boulder)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _boulder);
			return true;
		}
		if ((ref name) == PropertyName._shadow)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _shadow);
			return true;
		}
		if ((ref name) == PropertyName._slamBehind)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _slamBehind);
			return true;
		}
		if ((ref name) == PropertyName._slamFront)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _slamFront);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._boulder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shadow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._slamBehind, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._slamFront, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._boulder, Variant.From<Sprite2D>(ref _boulder));
		info.AddProperty(PropertyName._shadow, Variant.From<Sprite2D>(ref _shadow));
		info.AddProperty(PropertyName._slamBehind, Variant.From<GpuParticles2D>(ref _slamBehind));
		info.AddProperty(PropertyName._slamFront, Variant.From<GpuParticles2D>(ref _slamFront));
		info.AddSignalEventDelegate(SignalName.HitCreature, (Delegate)backing_HitCreature);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._boulder, ref val))
		{
			_boulder = ((Variant)(ref val)).As<Sprite2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._shadow, ref val2))
		{
			_shadow = ((Variant)(ref val2)).As<Sprite2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._slamBehind, ref val3))
		{
			_slamBehind = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._slamFront, ref val4))
		{
			_slamFront = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		HitCreatureEventHandler hitCreatureEventHandler = default(HitCreatureEventHandler);
		if (info.TryGetSignalEventDelegate<HitCreatureEventHandler>(SignalName.HitCreature, ref hitCreatureEventHandler))
		{
			backing_HitCreature = hitCreatureEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.HitCreature, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("creature"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalHitCreature(NCreature creature)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.HitCreature, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)creature) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.HitCreature && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_HitCreature?.Invoke(VariantUtils.ConvertTo<NCreature>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.HitCreature)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassSignal(ref signal);
	}
}
