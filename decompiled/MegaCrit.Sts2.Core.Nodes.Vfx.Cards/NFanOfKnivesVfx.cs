using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Cards;

[ScriptPath("res://src/Core/Nodes/Vfx/Cards/NFanOfKnivesVfx.cs")]
public class NFanOfKnivesVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _shiv1 = StringName.op_Implicit("_shiv1");

		public static readonly StringName _shiv2 = StringName.op_Implicit("_shiv2");

		public static readonly StringName _shiv3 = StringName.op_Implicit("_shiv3");

		public static readonly StringName _shiv4 = StringName.op_Implicit("_shiv4");

		public static readonly StringName _shiv5 = StringName.op_Implicit("_shiv5");

		public static readonly StringName _shiv6 = StringName.op_Implicit("_shiv6");

		public static readonly StringName _shiv7 = StringName.op_Implicit("_shiv7");

		public static readonly StringName _shiv8 = StringName.op_Implicit("_shiv8");

		public static readonly StringName _shiv9 = StringName.op_Implicit("_shiv9");

		public static readonly StringName _spawnPosition = StringName.op_Implicit("_spawnPosition");

		public static readonly StringName _spawnTween = StringName.op_Implicit("_spawnTween");

		public static readonly StringName _fanTween = StringName.op_Implicit("_fanTween");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/fan_of_knives_vfx");

	private const string _fanOfKnivesSfx = "event:/sfx/characters/silent/silent_fan_of_knives";

	private readonly List<Node2D> _shivs = new List<Node2D>();

	private Node2D _shiv1;

	private Node2D _shiv2;

	private Node2D _shiv3;

	private Node2D _shiv4;

	private Node2D _shiv5;

	private Node2D _shiv6;

	private Node2D _shiv7;

	private Node2D _shiv8;

	private Node2D _shiv9;

	private Vector2 _spawnPosition;

	private const double _fanDuration = 0.8;

	private Tween? _spawnTween;

	private Tween? _fanTween;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NFanOfKnivesVfx? Create(Creature target)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NFanOfKnivesVfx nFanOfKnivesVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NFanOfKnivesVfx>((GenEditState)0);
		nFanOfKnivesVfx._spawnPosition = NCombatRoom.Instance.GetCreatureNode(target).VfxSpawnPosition;
		return nFanOfKnivesVfx;
	}

	public override void _Ready()
	{
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		_shiv1 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("ShivFanParticle1"));
		_shiv2 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("ShivFanParticle2"));
		_shiv3 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("ShivFanParticle3"));
		_shiv4 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("ShivFanParticle4"));
		_shiv5 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("ShivFanParticle5"));
		_shiv6 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("ShivFanParticle6"));
		_shiv7 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("ShivFanParticle7"));
		_shiv8 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("ShivFanParticle8"));
		_shiv9 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("ShivFanParticle9"));
		_shivs.Add(_shiv1);
		_shivs.Add(_shiv2);
		_shivs.Add(_shiv3);
		_shivs.Add(_shiv4);
		_shivs.Add(_shiv5);
		_shivs.Add(_shiv6);
		_shivs.Add(_shiv7);
		_shivs.Add(_shiv8);
		_shivs.Add(_shiv9);
		foreach (Node2D shiv in _shivs)
		{
			shiv.Scale = Vector2.One * Rng.Chaotic.NextFloat(0.98f, 1.02f);
			shiv.GlobalPosition = _spawnPosition;
		}
		TaskHelper.RunSafely(Animate());
	}

	public override void _ExitTree()
	{
		Tween? fanTween = _fanTween;
		if (fanTween != null)
		{
			fanTween.Kill();
		}
		Tween? spawnTween = _spawnTween;
		if (spawnTween != null)
		{
			spawnTween.Kill();
		}
	}

	private async Task Animate()
	{
		SfxCmd.Play("event:/sfx/characters/silent/silent_fan_of_knives");
		_spawnTween = ((Node)this).CreateTween().SetParallel(true);
		foreach (Node2D shiv in _shivs)
		{
			float num = Rng.Chaotic.NextFloat(0.4f, 0.8f);
			_spawnTween.TweenProperty((GodotObject)(object)shiv, NodePath.op_Implicit("offset:y"), Variant.op_Implicit(-180f), (double)num).From(Variant.op_Implicit(0f)).SetEase((EaseType)1)
				.SetTrans((TransitionType)10);
			_spawnTween.TweenProperty((GodotObject)(object)shiv, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), (double)num).From(Variant.op_Implicit(StsColors.transparentBlack));
			_spawnTween.TweenProperty((GodotObject)(object)((Node)shiv).GetNode<Node2D>(NodePath.op_Implicit("Shadow")), NodePath.op_Implicit("offset:y"), Variant.op_Implicit(-180f), (double)num).From(Variant.op_Implicit(0f)).SetEase((EaseType)1)
				.SetTrans((TransitionType)10);
		}
		_spawnTween.Chain();
		foreach (Node2D shiv2 in _shivs)
		{
			_spawnTween.TweenProperty((GodotObject)(object)shiv2, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentWhite), 0.4).SetDelay(Rng.Chaotic.NextDouble(0.25, 0.5));
		}
		_fanTween = ((Node)this).CreateTween().SetParallel(true);
		_fanTween.TweenInterval(0.4000000059604645);
		_fanTween.Chain();
		_fanTween.TweenProperty((GodotObject)(object)_shiv1, NodePath.op_Implicit("rotation"), Variant.op_Implicit(-1.74533f), 0.8).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_fanTween.TweenProperty((GodotObject)(object)_shiv2, NodePath.op_Implicit("rotation"), Variant.op_Implicit(-1.3089975f), 0.8).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_fanTween.TweenProperty((GodotObject)(object)_shiv3, NodePath.op_Implicit("rotation"), Variant.op_Implicit(-0.872665f), 0.8).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_fanTween.TweenProperty((GodotObject)(object)_shiv4, NodePath.op_Implicit("rotation"), Variant.op_Implicit(-0.4363325f), 0.8).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_fanTween.TweenProperty((GodotObject)(object)_shiv6, NodePath.op_Implicit("rotation"), Variant.op_Implicit(0.4363325f), 0.8).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_fanTween.TweenProperty((GodotObject)(object)_shiv7, NodePath.op_Implicit("rotation"), Variant.op_Implicit(0.872665f), 0.8).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_fanTween.TweenProperty((GodotObject)(object)_shiv8, NodePath.op_Implicit("rotation"), Variant.op_Implicit(1.3089975f), 0.8).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_fanTween.TweenProperty((GodotObject)(object)_shiv9, NodePath.op_Implicit("rotation"), Variant.op_Implicit(1.74533f), 0.8).SetEase((EaseType)1).SetTrans((TransitionType)10);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_spawnTween, SignalName.Finished);
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
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._shiv1)
		{
			_shiv1 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shiv2)
		{
			_shiv2 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shiv3)
		{
			_shiv3 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shiv4)
		{
			_shiv4 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shiv5)
		{
			_shiv5 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shiv6)
		{
			_shiv6 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shiv7)
		{
			_shiv7 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shiv8)
		{
			_shiv8 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shiv9)
		{
			_shiv9 = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spawnPosition)
		{
			_spawnPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spawnTween)
		{
			_spawnTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fanTween)
		{
			_fanTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._shiv1)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _shiv1);
			return true;
		}
		if ((ref name) == PropertyName._shiv2)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _shiv2);
			return true;
		}
		if ((ref name) == PropertyName._shiv3)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _shiv3);
			return true;
		}
		if ((ref name) == PropertyName._shiv4)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _shiv4);
			return true;
		}
		if ((ref name) == PropertyName._shiv5)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _shiv5);
			return true;
		}
		if ((ref name) == PropertyName._shiv6)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _shiv6);
			return true;
		}
		if ((ref name) == PropertyName._shiv7)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _shiv7);
			return true;
		}
		if ((ref name) == PropertyName._shiv8)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _shiv8);
			return true;
		}
		if ((ref name) == PropertyName._shiv9)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _shiv9);
			return true;
		}
		if ((ref name) == PropertyName._spawnPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _spawnPosition);
			return true;
		}
		if ((ref name) == PropertyName._spawnTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _spawnTween);
			return true;
		}
		if ((ref name) == PropertyName._fanTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _fanTween);
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
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._shiv1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shiv2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shiv3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shiv4, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shiv5, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shiv6, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shiv7, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shiv8, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shiv9, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._spawnPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spawnTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fanTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._shiv1, Variant.From<Node2D>(ref _shiv1));
		info.AddProperty(PropertyName._shiv2, Variant.From<Node2D>(ref _shiv2));
		info.AddProperty(PropertyName._shiv3, Variant.From<Node2D>(ref _shiv3));
		info.AddProperty(PropertyName._shiv4, Variant.From<Node2D>(ref _shiv4));
		info.AddProperty(PropertyName._shiv5, Variant.From<Node2D>(ref _shiv5));
		info.AddProperty(PropertyName._shiv6, Variant.From<Node2D>(ref _shiv6));
		info.AddProperty(PropertyName._shiv7, Variant.From<Node2D>(ref _shiv7));
		info.AddProperty(PropertyName._shiv8, Variant.From<Node2D>(ref _shiv8));
		info.AddProperty(PropertyName._shiv9, Variant.From<Node2D>(ref _shiv9));
		info.AddProperty(PropertyName._spawnPosition, Variant.From<Vector2>(ref _spawnPosition));
		info.AddProperty(PropertyName._spawnTween, Variant.From<Tween>(ref _spawnTween));
		info.AddProperty(PropertyName._fanTween, Variant.From<Tween>(ref _fanTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._shiv1, ref val))
		{
			_shiv1 = ((Variant)(ref val)).As<Node2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._shiv2, ref val2))
		{
			_shiv2 = ((Variant)(ref val2)).As<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._shiv3, ref val3))
		{
			_shiv3 = ((Variant)(ref val3)).As<Node2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._shiv4, ref val4))
		{
			_shiv4 = ((Variant)(ref val4)).As<Node2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._shiv5, ref val5))
		{
			_shiv5 = ((Variant)(ref val5)).As<Node2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._shiv6, ref val6))
		{
			_shiv6 = ((Variant)(ref val6)).As<Node2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._shiv7, ref val7))
		{
			_shiv7 = ((Variant)(ref val7)).As<Node2D>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._shiv8, ref val8))
		{
			_shiv8 = ((Variant)(ref val8)).As<Node2D>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._shiv9, ref val9))
		{
			_shiv9 = ((Variant)(ref val9)).As<Node2D>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._spawnPosition, ref val10))
		{
			_spawnPosition = ((Variant)(ref val10)).As<Vector2>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._spawnTween, ref val11))
		{
			_spawnTween = ((Variant)(ref val11)).As<Tween>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._fanTween, ref val12))
		{
			_fanTween = ((Variant)(ref val12)).As<Tween>();
		}
	}
}
