using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NMonsterDeathVfx.cs")]
public class NMonsterDeathVfx : Node2D
{
	public class MethodName : MethodName
	{
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	private const string _deathSfx = "event:/sfx/enemy/enemy_fade";

	private const float _refLength = 0.1f;

	private const float _refTweenDuration = 2.5f;

	private const float _minTweenDuration = 2.5f;

	private const float _tweenStartValue = 0f;

	private const string _shaderParamThreshold = "shader_parameter/threshold";

	private List<NCreature> _creatureNodes;

	private List<Control> _hitboxes;

	private CancellationToken _cancelToken;

	private static string ScenePath => SceneHelper.GetScenePath("vfx/vfx_monster_death");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public static NMonsterDeathVfx? Create(NCreature creatureNode, CancellationToken cancelToken)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant)
		{
			return null;
		}
		if (cancelToken.IsCancellationRequested)
		{
			return null;
		}
		NMonsterDeathVfx nMonsterDeathVfx = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NMonsterDeathVfx>((GenEditState)0);
		int num = 1;
		List<NCreature> list = new List<NCreature>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<NCreature> span = CollectionsMarshal.AsSpan(list);
		int index = 0;
		span[index] = creatureNode;
		nMonsterDeathVfx._creatureNodes = list;
		nMonsterDeathVfx._cancelToken = cancelToken;
		index = 1;
		List<Control> list2 = new List<Control>(index);
		CollectionsMarshal.SetCount(list2, index);
		Span<Control> span2 = CollectionsMarshal.AsSpan(list2);
		num = 0;
		span2[num] = creatureNode.Hitbox;
		nMonsterDeathVfx._hitboxes = list2;
		return nMonsterDeathVfx;
	}

	public static NMonsterDeathVfx? Create(List<NCreature> creatureNodes)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NMonsterDeathVfx nMonsterDeathVfx = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NMonsterDeathVfx>((GenEditState)0);
		nMonsterDeathVfx._creatureNodes = creatureNodes;
		nMonsterDeathVfx._cancelToken = default(CancellationToken);
		nMonsterDeathVfx._hitboxes = creatureNodes.Select((NCreature c) => c.Hitbox).ToList();
		return nMonsterDeathVfx;
	}

	public async Task PlayVfx()
	{
		if (_cancelToken.IsCancellationRequested)
		{
			return;
		}
		NCombatRoom instance = NCombatRoom.Instance;
		if (instance == null)
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		SubViewport node = ((Node)this).GetNode<SubViewport>(NodePath.op_Implicit("Viewport"));
		Rect2? val = null;
		Rect2 val3 = default(Rect2);
		Vector2 val4 = default(Vector2);
		Vector2 val5 = default(Vector2);
		Rect2 val9 = default(Rect2);
		Rect2 value;
		Rect2 val14 = default(Rect2);
		for (int i = 0; i < _creatureNodes.Count; i++)
		{
			NCreature nCreature = _creatureNodes[i];
			NCreatureVisuals visuals = nCreature.Visuals;
			MegaSprite spineBody = visuals.SpineBody;
			Vector2 scale = visuals.Body.Scale;
			Vector2 val2 = nCreature.Entity.Monster?.ExtraDeathVfxPadding ?? MonsterModel.defaultDeathVfxPadding;
			if (visuals.HasSpineAnimation)
			{
				Vector2 scale2 = instance.SceneContainer.Scale;
				Rect2 bounds = spineBody.GetSkeleton().GetBounds();
				((Rect2)(ref val3))._002Ector(((Rect2)(ref bounds)).Position * scale * scale2, ((Rect2)(ref bounds)).Size * scale * scale2);
				((Vector2)(ref val4))._002Ector(Math.Min(((Rect2)(ref val3)).Position.X, ((Rect2)(ref val3)).End.X), Math.Min(((Rect2)(ref val3)).Position.Y, ((Rect2)(ref val3)).End.Y));
				((Vector2)(ref val5))._002Ector(Math.Max(((Rect2)(ref val3)).Position.X, ((Rect2)(ref val3)).End.X), Math.Max(((Rect2)(ref val3)).Position.Y, ((Rect2)(ref val3)).End.Y));
				Vector2 val6 = val5 - val4;
				Vector2 val7 = val6 * val2;
				Vector2 val8 = (val7 - val6) * 0.5f;
				((Rect2)(ref val9))._002Ector(visuals.Body.GlobalPosition + val4 - val8, val7);
				if (!val.HasValue)
				{
					val = val9;
					continue;
				}
				value = val.Value;
				val = ((Rect2)(ref value)).Merge(val9);
			}
			else
			{
				Control val10 = _hitboxes[i];
				Vector2 val11 = val10.Size * val10.Scale;
				Vector2 val12 = val11 * val2;
				Vector2 val13 = (val12 - val11) * 0.5f;
				((Rect2)(ref val14))._002Ector(val10.GlobalPosition - val13, val12);
				if (!val.HasValue)
				{
					val = val14;
					continue;
				}
				value = val.Value;
				val = ((Rect2)(ref value)).Merge(val14);
			}
		}
		NMonsterDeathVfx nMonsterDeathVfx = this;
		value = val.Value;
		Vector2 position = ((Rect2)(ref value)).Position;
		value = val.Value;
		((Node2D)nMonsterDeathVfx).GlobalPosition = position + ((Rect2)(ref value)).Size * 0.5f;
		value = val.Value;
		Vector2 size = ((Rect2)(ref value)).Size;
		int num = Mathf.RoundToInt(Mathf.Max(size.X, size.Y));
		((Vector2)(ref size))._002Ector((float)num, (float)num);
		node.Size = new Vector2I(num, num) * 2;
		node.Size2DOverride = new Vector2I(num, num);
		Sprite2D node2 = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("Visual"));
		((Node2D)node2).Scale = ((Node2D)node2).Scale / (2f * instance.SceneContainer.Scale);
		Vector2 val15 = ((Node2D)this).GlobalPosition - size * 0.5f;
		foreach (NCreature creatureNode in _creatureNodes)
		{
			if (GodotObject.IsInstanceValid((GodotObject)(object)creatureNode.Visuals.Body))
			{
				Vector2 globalPosition = creatureNode.Visuals.Body.GlobalPosition;
				((Node)creatureNode.Visuals.Body).Reparent((Node)(object)node, true);
				if (GodotObject.IsInstanceValid((GodotObject)(object)creatureNode.Visuals.Body))
				{
					creatureNode.Visuals.Body.Position = globalPosition - val15;
				}
			}
		}
		node.RenderTargetUpdateMode = (UpdateMode)1;
		await PlayVfxInternal();
	}

	private async Task PlayVfxInternal()
	{
		SfxCmd.Play("event:/sfx/enemy/enemy_fade");
		SubViewport node = ((Node)this).GetNode<SubViewport>(NodePath.op_Implicit("Viewport"));
		Sprite2D node2 = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("Visual"));
		CpuParticles2D node3 = ((Node)this).GetNode<CpuParticles2D>(NodePath.op_Implicit("%Particles"));
		node3.EmissionSphereRadius = (float)node.Size.Y / 4f;
		node3.Emitting = true;
		Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
		float num = 0.1f * ((Rect2)(ref viewportRect)).Size.X;
		float num2 = Math.Min((float)node.Size.X / num * 2.5f, 2.5f);
		Tween tween = ((Node)this).CreateTween();
		try
		{
			tween.TweenProperty((GodotObject)(object)((CanvasItem)node2).Material, NodePath.op_Implicit("shader_parameter/threshold"), Variant.op_Implicit(0f), (double)num2).SetEase((EaseType)1).SetTrans((TransitionType)1);
			await ((GodotObject)this).ToSignal((GodotObject)(object)tween, SignalName.Finished);
			((Node)(object)this).QueueFreeSafely();
		}
		finally
		{
			((IDisposable)tween)?.Dispose();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
	}
}
