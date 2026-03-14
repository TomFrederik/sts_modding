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
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NGroundFireVfx.cs")]
public class NGroundFireVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ApplyColor = StringName.op_Implicit("ApplyColor");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _mainFire = StringName.op_Implicit("_mainFire");

		public static readonly StringName _ember = StringName.op_Implicit("_ember");

		public static readonly StringName _flameSprites = StringName.op_Implicit("_flameSprites");

		public static readonly StringName _vfxColor = StringName.op_Implicit("_vfxColor");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _outerColor = new StringName("OuterColor");

	private static readonly StringName _innerColor = new StringName("InnerColor");

	private Tween? _tween;

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/fires/vfx_ground_fire");

	private Node2D _mainFire;

	private GpuParticles2D _ember;

	private GpuParticles2D _flameSprites;

	private VfxColor _vfxColor;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NGroundFireVfx? Create(Creature target, VfxColor color = VfxColor.Red)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(target);
		if (creatureNode == null)
		{
			return null;
		}
		NGroundFireVfx nGroundFireVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NGroundFireVfx>((GenEditState)0);
		nGroundFireVfx._vfxColor = color;
		((Node2D)nGroundFireVfx).GlobalPosition = creatureNode.GetBottomOfHitbox();
		return nGroundFireVfx;
	}

	public override void _Ready()
	{
		_mainFire = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("MainFire"));
		_ember = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("Ember"));
		_flameSprites = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("FlameSprites"));
		ApplyColor();
		TaskHelper.RunSafely(AnimateIn());
	}

	private void ApplyColor()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		if (_vfxColor != VfxColor.Red)
		{
			Color white = Colors.White;
			Color white2 = Colors.White;
			Color black = default(Color);
			((Color)(ref black))._002Ector("541b00");
			switch (_vfxColor)
			{
			case VfxColor.Green:
				((Color)(ref white))._002Ector("2fa800");
				((Color)(ref white2))._002Ector("06a000");
				((Color)(ref black))._002Ector("541b00");
				break;
			case VfxColor.Blue:
				((Color)(ref white))._002Ector("0099cd");
				((Color)(ref white2))._002Ector("00a3bf");
				black = Colors.Black;
				break;
			case VfxColor.Purple:
				((Color)(ref white))._002Ector("7821ff");
				((Color)(ref white2))._002Ector("3f21ff");
				((Color)(ref black))._002Ector("541b00");
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case VfxColor.Black:
			case VfxColor.White:
				break;
			}
			Node node = ((Node)_mainFire).GetNode(NodePath.op_Implicit("VfxAdditiveStepFire"));
			ShaderMaterial val = (ShaderMaterial)((CanvasItem)node.GetNode<Node2D>(NodePath.op_Implicit("SteppedFireMix"))).Material;
			val.SetShaderParameter(_outerColor, Variant.op_Implicit(white));
			val.SetShaderParameter(_innerColor, Variant.op_Implicit(white2));
			val = (ShaderMaterial)((CanvasItem)node.GetNode<Node2D>(NodePath.op_Implicit("SteppedFireAdd"))).Material;
			val.SetShaderParameter(_outerColor, Variant.op_Implicit(black));
		}
	}

	public override void _ExitTree()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
	}

	private async Task AnimateIn()
	{
		((CanvasItem)_mainFire).Modulate = Colors.Transparent;
		_mainFire.Scale = Vector2.Zero;
		_ember.Emitting = true;
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_mainFire, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 4f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_tween.TweenProperty((GodotObject)(object)_mainFire, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.9f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		_flameSprites.Emitting = true;
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_mainFire, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
		_tween.TweenProperty((GodotObject)(object)_flameSprites, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
		_tween.TweenProperty((GodotObject)(object)_mainFire, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 2f), 2.0).SetEase((EaseType)2).SetTrans((TransitionType)7);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		_flameSprites.Emitting = false;
		await ((GodotObject)this).ToSignal((GodotObject)(object)_ember, SignalName.Finished);
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
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ApplyColor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ApplyColor && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ApplyColor();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
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
		if ((ref method) == MethodName.ApplyColor)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mainFire)
		{
			_mainFire = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ember)
		{
			_ember = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flameSprites)
		{
			_flameSprites = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
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
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._mainFire)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _mainFire);
			return true;
		}
		if ((ref name) == PropertyName._ember)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _ember);
			return true;
		}
		if ((ref name) == PropertyName._flameSprites)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _flameSprites);
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
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mainFire, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ember, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._flameSprites, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._mainFire, Variant.From<Node2D>(ref _mainFire));
		info.AddProperty(PropertyName._ember, Variant.From<GpuParticles2D>(ref _ember));
		info.AddProperty(PropertyName._flameSprites, Variant.From<GpuParticles2D>(ref _flameSprites));
		info.AddProperty(PropertyName._vfxColor, Variant.From<VfxColor>(ref _vfxColor));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val))
		{
			_tween = ((Variant)(ref val)).As<Tween>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._mainFire, ref val2))
		{
			_mainFire = ((Variant)(ref val2)).As<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._ember, ref val3))
		{
			_ember = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._flameSprites, ref val4))
		{
			_flameSprites = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._vfxColor, ref val5))
		{
			_vfxColor = ((Variant)(ref val5)).As<VfxColor>();
		}
	}
}
