using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NCardTrailVfx.cs")]
public class NCardTrailVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName StopParticles = StringName.op_Implicit("StopParticles");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _nodeToFollow = StringName.op_Implicit("_nodeToFollow");

		public static readonly StringName _sprites = StringName.op_Implicit("_sprites");

		public static readonly StringName _updateSprites = StringName.op_Implicit("_updateSprites");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private Control _nodeToFollow;

	private Node2D _sprites;

	private bool _updateSprites = true;

	private Tween? _tween;

	public static NCardTrailVfx? Create(Control card, string characterTrailPath)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NCardTrailVfx nCardTrailVfx = PreloadManager.Cache.GetScene(characterTrailPath).Instantiate<NCardTrailVfx>((GenEditState)0);
		nCardTrailVfx._nodeToFollow = card;
		return nCardTrailVfx;
	}

	public override void _Ready()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (NCombatUi.IsDebugHidingPlayContainer)
		{
			((CanvasItem)this).Visible = false;
		}
		_sprites = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("Sprites"));
		((CanvasItem)_sprites).Modulate = StsColors.transparentWhite;
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_sprites, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.5f), 0.5).SetEase((EaseType)0).SetTrans((TransitionType)7)
			.SetDelay(0.25);
		_tween.TweenProperty((GodotObject)(object)_nodeToFollow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.75f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)_sprites, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	public override void _Process(double delta)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (_updateSprites)
		{
			((Node2D)this).GlobalPosition = _nodeToFollow.GlobalPosition;
			((Node2D)this).Rotation = _nodeToFollow.Rotation;
		}
	}

	public async Task FadeOut()
	{
		_updateSprites = false;
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
		StopParticles(_tween);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	private void StopParticles(Tween tween)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		foreach (Node child in ((Node)_sprites).GetChildren(false))
		{
			CpuParticles2D val = (CpuParticles2D)(object)((child is CpuParticles2D) ? child : null);
			if (val != null)
			{
				tween.TweenProperty((GodotObject)(object)val, NodePath.op_Implicit("amount"), Variant.op_Implicit(1), 0.5);
			}
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
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)4, StringName.op_Implicit("characterTrailPath"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopParticles, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tween"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Tween"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NCardTrailVfx nCardTrailVfx = Create(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NCardTrailVfx>(ref nCardTrailVfx);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopParticles && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			StopParticles(VariantUtils.ConvertTo<Tween>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NCardTrailVfx nCardTrailVfx = Create(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NCardTrailVfx>(ref nCardTrailVfx);
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
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.StopParticles)
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
		if ((ref name) == PropertyName._nodeToFollow)
		{
			_nodeToFollow = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sprites)
		{
			_sprites = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._updateSprites)
		{
			_updateSprites = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._nodeToFollow)
		{
			value = VariantUtils.CreateFrom<Control>(ref _nodeToFollow);
			return true;
		}
		if ((ref name) == PropertyName._sprites)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _sprites);
			return true;
		}
		if ((ref name) == PropertyName._updateSprites)
		{
			value = VariantUtils.CreateFrom<bool>(ref _updateSprites);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._nodeToFollow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sprites, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._updateSprites, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._nodeToFollow, Variant.From<Control>(ref _nodeToFollow));
		info.AddProperty(PropertyName._sprites, Variant.From<Node2D>(ref _sprites));
		info.AddProperty(PropertyName._updateSprites, Variant.From<bool>(ref _updateSprites));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._nodeToFollow, ref val))
		{
			_nodeToFollow = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._sprites, ref val2))
		{
			_sprites = ((Variant)(ref val2)).As<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._updateSprites, ref val3))
		{
			_updateSprites = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val4))
		{
			_tween = ((Variant)(ref val4)).As<Tween>();
		}
	}
}
