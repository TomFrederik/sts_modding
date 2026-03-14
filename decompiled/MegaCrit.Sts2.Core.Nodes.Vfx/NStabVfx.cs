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
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NStabVfx.cs")]
public class NStabVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetColor = StringName.op_Implicit("SetColor");

		public static readonly StringName GenerateSpawnPosition = StringName.op_Implicit("GenerateSpawnPosition");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _primaryVfx = StringName.op_Implicit("_primaryVfx");

		public static readonly StringName _secondaryVfx = StringName.op_Implicit("_secondaryVfx");

		public static readonly StringName _creatureCenter = StringName.op_Implicit("_creatureCenter");

		public static readonly StringName _vfxColor = StringName.op_Implicit("_vfxColor");

		public static readonly StringName _facingEnemies = StringName.op_Implicit("_facingEnemies");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/vfx/stab_vfx.tscn";

	private Node2D _primaryVfx;

	private Node2D _secondaryVfx;

	private Vector2 _creatureCenter;

	private VfxColor _vfxColor;

	private bool _facingEnemies;

	private Tween? _tween;

	public static NStabVfx? Create(Creature? target, bool facingEnemies = false, VfxColor vfxColor = VfxColor.Red)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(target);
		if (creatureNode == null)
		{
			return null;
		}
		Vector2 vfxSpawnPosition = creatureNode.VfxSpawnPosition;
		NStabVfx nStabVfx = PreloadManager.Cache.GetScene("res://scenes/vfx/stab_vfx.tscn").Instantiate<NStabVfx>((GenEditState)0);
		nStabVfx._vfxColor = vfxColor;
		nStabVfx._facingEnemies = facingEnemies;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(facingEnemies ? Rng.Chaotic.NextFloat(0f, 48f) : Rng.Chaotic.NextFloat(-48f, 0f), Rng.Chaotic.NextFloat(-50f, 50f));
		nStabVfx._creatureCenter = vfxSpawnPosition + val;
		return nStabVfx;
	}

	public override void _Ready()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		_primaryVfx = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("Primary"));
		_secondaryVfx = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%Secondary"));
		_primaryVfx.GlobalPosition = GenerateSpawnPosition();
		_primaryVfx.Rotation = MathHelper.GetAngle(_primaryVfx.GlobalPosition - _creatureCenter) + (float)Math.PI / 2f;
		SetColor();
		TaskHelper.RunSafely(Animate());
	}

	private void SetColor()
	{
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		switch (_vfxColor)
		{
		case VfxColor.Green:
			((CanvasItem)_primaryVfx).SelfModulate = new Color("00A52F");
			((CanvasItem)_secondaryVfx).SelfModulate = new Color("FFCB2D");
			break;
		case VfxColor.Blue:
			((CanvasItem)_primaryVfx).SelfModulate = new Color("007BDD");
			((CanvasItem)_secondaryVfx).SelfModulate = new Color("00EFF6");
			break;
		case VfxColor.Purple:
			((CanvasItem)_primaryVfx).SelfModulate = new Color("A803FF");
			((CanvasItem)_secondaryVfx).SelfModulate = new Color("00EFF3");
			break;
		case VfxColor.White:
			((CanvasItem)_primaryVfx).SelfModulate = new Color("808080");
			((CanvasItem)_secondaryVfx).SelfModulate = new Color("FFFFFF");
			break;
		case VfxColor.Cyan:
			((CanvasItem)_primaryVfx).SelfModulate = new Color("009599");
			((CanvasItem)_secondaryVfx).SelfModulate = new Color("5CDCFF");
			break;
		case VfxColor.Gold:
			((CanvasItem)_primaryVfx).SelfModulate = new Color("EBA800");
			((CanvasItem)_secondaryVfx).SelfModulate = new Color("FFE39C");
			break;
		default:
			((CanvasItem)_primaryVfx).SelfModulate = new Color("FF0000");
			((CanvasItem)_secondaryVfx).SelfModulate = new Color("FFCB2D");
			break;
		case VfxColor.Black:
			break;
		}
	}

	private Vector2 GenerateSpawnPosition()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(Rng.Chaotic.NextFloat(-12f, 12f), Rng.Chaotic.NextFloat(-64f, 64f));
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(_facingEnemies ? (-200f) : 200f, 0f);
		return _creatureCenter + val + val2;
	}

	public override void _ExitTree()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
	}

	private async Task Animate()
	{
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25);
		_tween.TweenProperty((GodotObject)(object)_primaryVfx, NodePath.op_Implicit("position"), Variant.op_Implicit(_creatureCenter), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)6);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25).SetDelay(0.25);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
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
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.SetColor)
		{
			return true;
		}
		if ((ref method) == MethodName.GenerateSpawnPosition)
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
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._primaryVfx)
		{
			_primaryVfx = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._secondaryVfx)
		{
			_secondaryVfx = VariantUtils.ConvertTo<Node2D>(ref value);
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
		if ((ref name) == PropertyName._facingEnemies)
		{
			_facingEnemies = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._primaryVfx)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _primaryVfx);
			return true;
		}
		if ((ref name) == PropertyName._secondaryVfx)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _secondaryVfx);
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
		if ((ref name) == PropertyName._facingEnemies)
		{
			value = VariantUtils.CreateFrom<bool>(ref _facingEnemies);
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
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._primaryVfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._secondaryVfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._creatureCenter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._vfxColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._facingEnemies, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._primaryVfx, Variant.From<Node2D>(ref _primaryVfx));
		info.AddProperty(PropertyName._secondaryVfx, Variant.From<Node2D>(ref _secondaryVfx));
		info.AddProperty(PropertyName._creatureCenter, Variant.From<Vector2>(ref _creatureCenter));
		info.AddProperty(PropertyName._vfxColor, Variant.From<VfxColor>(ref _vfxColor));
		info.AddProperty(PropertyName._facingEnemies, Variant.From<bool>(ref _facingEnemies));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._primaryVfx, ref val))
		{
			_primaryVfx = ((Variant)(ref val)).As<Node2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._secondaryVfx, ref val2))
		{
			_secondaryVfx = ((Variant)(ref val2)).As<Node2D>();
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
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._facingEnemies, ref val5))
		{
			_facingEnemies = ((Variant)(ref val5)).As<bool>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val6))
		{
			_tween = ((Variant)(ref val6)).As<Tween>();
		}
	}
}
