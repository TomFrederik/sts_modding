using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NDoomVfx.cs")]
public class NDoomVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ShowOrHideParticles = StringName.op_Implicit("ShowOrHideParticles");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _back = StringName.op_Implicit("_back");

		public static readonly StringName _front = StringName.op_Implicit("_front");

		public static readonly StringName _creatureVisuals = StringName.op_Implicit("_creatureVisuals");

		public static readonly StringName _position = StringName.op_Implicit("_position");

		public static readonly StringName _size = StringName.op_Implicit("_size");

		public static readonly StringName _shouldDie = StringName.op_Implicit("_shouldDie");
	}

	public class SignalName : SignalName
	{
	}

	private Tween? _tween;

	private NDoomSubEmitterVfx _back;

	private NDoomSubEmitterVfx _front;

	private NCreatureVisuals _creatureVisuals;

	private Vector2 _position;

	private Vector2 _size;

	private bool _shouldDie;

	private CancellationToken _cancelToken;

	private const float _doomVfxSize = 260f;

	private CancellationTokenSource VfxCancellationToken { get; } = new CancellationTokenSource();

	private static string ScenePath => SceneHelper.GetScenePath("vfx/vfx_doom");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public Task? VfxTask { get; private set; }

	public static NDoomVfx? Create(NCreatureVisuals creatureVisuals, Vector2 position, Vector2 size, bool shouldDie)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NDoomVfx nDoomVfx = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NDoomVfx>((GenEditState)0);
		nDoomVfx._creatureVisuals = creatureVisuals;
		nDoomVfx._position = position;
		nDoomVfx._size = size;
		nDoomVfx._shouldDie = shouldDie;
		return nDoomVfx;
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		VfxCancellationToken.Cancel();
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
	}

	public override void _Ready()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		_back = ((Node)this).GetNode<NDoomSubEmitterVfx>(NodePath.op_Implicit("DoomVfxBack"));
		_front = ((Node)this).GetNode<NDoomSubEmitterVfx>(NodePath.op_Implicit("DoomVfxFront"));
		_cancelToken = VfxCancellationToken.Token;
		VfxTask = TaskHelper.RunSafely(PlayVfx(_creatureVisuals, _position, _size, _shouldDie));
	}

	private async Task PlayVfx(NCreatureVisuals creatureVisuals, Vector2 position, Vector2 size, bool shouldDie)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!_cancelToken.IsCancellationRequested)
		{
			SfxCmd.Play("event:/sfx/characters/necrobinder/necrobinder_doom_kill");
			((Node2D)this).GlobalPosition = position + new Vector2(size.X * 0.5f, size.Y) * NCombatRoom.Instance.SceneContainer.Scale;
			((Node2D)this).Scale = NCombatRoom.Instance.SceneContainer.Scale;
			SubViewport node = ((Node)this).GetNode<SubViewport>(NodePath.op_Implicit("Viewport"));
			Vector2 val = size;
			val.X *= 1.5f;
			val.Y *= 1.5f;
			node.Size = (Vector2I)val;
			if (shouldDie)
			{
				Vector2 creatureOffset = new Vector2(val.X / 2f, (float)node.Size.Y) + creatureVisuals.Body.Position;
				Vector2 originalGlobalScale = creatureVisuals.Body.GlobalScale;
				await Reparent((Node)(object)creatureVisuals.Body, (Node)(object)node);
				creatureVisuals.Body.Position = creatureOffset;
				creatureVisuals.Body.Scale = originalGlobalScale;
			}
			if (!_cancelToken.IsCancellationRequested)
			{
				await PlayVfxInternal();
			}
		}
	}

	private async Task PlayVfxInternal()
	{
		_ = 1;
		try
		{
			SubViewport node = ((Node)this).GetNode<SubViewport>(NodePath.op_Implicit("Viewport"));
			Sprite2D node2 = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("%Visual"));
			((Node2D)node2).Position = ((Node2D)node2).Position + Vector2.Up * (float)node.Size.Y * 0.5f;
			NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short, 180f + Rng.Chaotic.NextFloat(-10f, 10f));
			ShowOrHideParticles((float)node.Size.X / 260f, 0.5f);
			_tween = ((Node)this).CreateTween();
			_tween.TweenProperty((GodotObject)(object)node2, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Node2D)node2).Position.Y + (float)node.Size.Y), 0.75).SetEase((EaseType)0).SetDelay(0.75)
				.SetTrans((TransitionType)5);
			await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
			ShowOrHideParticles(0f, 0.25f);
			await Task.Delay(2000, _cancelToken);
		}
		finally
		{
			if (GodotObject.IsInstanceValid((GodotObject)(object)this))
			{
				((Node)(object)this).QueueFreeSafely();
			}
		}
	}

	private void ShowOrHideParticles(float widthScale, float tweenTime)
	{
		_back.ShowOrHide(widthScale, tweenTime);
		_front.ShowOrHide(widthScale, tweenTime);
	}

	private async Task Reparent(Node creatureNode, Node newParent)
	{
		Node parent = creatureNode.GetParent();
		bool removeCompleted = false;
		Callable reparent = Callable.From<bool>((Func<bool>)(() => removeCompleted = true));
		((GodotObject)creatureNode).Connect(SignalName.TreeExited, reparent, 0u);
		parent.RemoveChildSafely(creatureNode);
		while (!removeCompleted)
		{
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		newParent.AddChildSafely(creatureNode);
		((GodotObject)creatureNode).Disconnect(SignalName.TreeExited, reparent);
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
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("creatureVisuals"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false),
			new PropertyInfo((Type)5, StringName.op_Implicit("position"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)5, StringName.op_Implicit("size"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("shouldDie"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowOrHideParticles, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("widthScale"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("tweenTime"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			NDoomVfx nDoomVfx = Create(VariantUtils.ConvertTo<NCreatureVisuals>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = VariantUtils.CreateFrom<NDoomVfx>(ref nDoomVfx);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowOrHideParticles && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			ShowOrHideParticles(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			NDoomVfx nDoomVfx = Create(VariantUtils.ConvertTo<NCreatureVisuals>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = VariantUtils.CreateFrom<NDoomVfx>(ref nDoomVfx);
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowOrHideParticles)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._back)
		{
			_back = VariantUtils.ConvertTo<NDoomSubEmitterVfx>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._front)
		{
			_front = VariantUtils.ConvertTo<NDoomSubEmitterVfx>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._creatureVisuals)
		{
			_creatureVisuals = VariantUtils.ConvertTo<NCreatureVisuals>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._position)
		{
			_position = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._size)
		{
			_size = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shouldDie)
		{
			_shouldDie = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._back)
		{
			value = VariantUtils.CreateFrom<NDoomSubEmitterVfx>(ref _back);
			return true;
		}
		if ((ref name) == PropertyName._front)
		{
			value = VariantUtils.CreateFrom<NDoomSubEmitterVfx>(ref _front);
			return true;
		}
		if ((ref name) == PropertyName._creatureVisuals)
		{
			value = VariantUtils.CreateFrom<NCreatureVisuals>(ref _creatureVisuals);
			return true;
		}
		if ((ref name) == PropertyName._position)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _position);
			return true;
		}
		if ((ref name) == PropertyName._size)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _size);
			return true;
		}
		if ((ref name) == PropertyName._shouldDie)
		{
			value = VariantUtils.CreateFrom<bool>(ref _shouldDie);
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
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._back, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._front, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._creatureVisuals, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._position, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._size, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._shouldDie, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._back, Variant.From<NDoomSubEmitterVfx>(ref _back));
		info.AddProperty(PropertyName._front, Variant.From<NDoomSubEmitterVfx>(ref _front));
		info.AddProperty(PropertyName._creatureVisuals, Variant.From<NCreatureVisuals>(ref _creatureVisuals));
		info.AddProperty(PropertyName._position, Variant.From<Vector2>(ref _position));
		info.AddProperty(PropertyName._size, Variant.From<Vector2>(ref _size));
		info.AddProperty(PropertyName._shouldDie, Variant.From<bool>(ref _shouldDie));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val))
		{
			_tween = ((Variant)(ref val)).As<Tween>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._back, ref val2))
		{
			_back = ((Variant)(ref val2)).As<NDoomSubEmitterVfx>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._front, ref val3))
		{
			_front = ((Variant)(ref val3)).As<NDoomSubEmitterVfx>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._creatureVisuals, ref val4))
		{
			_creatureVisuals = ((Variant)(ref val4)).As<NCreatureVisuals>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._position, ref val5))
		{
			_position = ((Variant)(ref val5)).As<Vector2>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._size, ref val6))
		{
			_size = ((Variant)(ref val6)).As<Vector2>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._shouldDie, ref val7))
		{
			_shouldDie = ((Variant)(ref val7)).As<bool>();
		}
	}
}
