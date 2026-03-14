using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.Orbs;

[ScriptPath("res://src/Core/Nodes/Orbs/NOrbManager.cs")]
public class NOrbManager : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName RemoveSlotAnim = StringName.op_Implicit("RemoveSlotAnim");

		public static readonly StringName AddSlotAnim = StringName.op_Implicit("AddSlotAnim");

		public static readonly StringName AddOrbAnim = StringName.op_Implicit("AddOrbAnim");

		public static readonly StringName UpdateControllerNavigation = StringName.op_Implicit("UpdateControllerNavigation");

		public static readonly StringName TweenLayout = StringName.op_Implicit("TweenLayout");

		public static readonly StringName UpdateVisuals = StringName.op_Implicit("UpdateVisuals");

		public static readonly StringName ClearOrbs = StringName.op_Implicit("ClearOrbs");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName IsLocal = StringName.op_Implicit("IsLocal");

		public static readonly StringName DefaultFocusOwner = StringName.op_Implicit("DefaultFocusOwner");

		public static readonly StringName _orbContainer = StringName.op_Implicit("_orbContainer");

		public static readonly StringName _creatureNode = StringName.op_Implicit("_creatureNode");

		public static readonly StringName _curTween = StringName.op_Implicit("_curTween");
	}

	public class SignalName : SignalName
	{
	}

	private Control _orbContainer;

	private readonly List<NOrb> _orbs = new List<NOrb>();

	private NCreature _creatureNode;

	private const float _minRadius = 225f;

	private const float _maxRadius = 300f;

	private const float _range = 150f;

	private const float _angleOffset = -25f;

	private const float _tweenSpeed = 0.45f;

	private Tween? _curTween;

	private static string ScenePath => SceneHelper.GetScenePath("/orbs/orb_manager");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public bool IsLocal { get; private set; }

	private Player Player => _creatureNode.Entity.Player;

	public Control DefaultFocusOwner
	{
		get
		{
			if (_orbs.Count <= 0)
			{
				return _creatureNode.Hitbox;
			}
			return (Control)(object)_orbs.First();
		}
	}

	public override void _Ready()
	{
		_orbContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Orbs"));
	}

	public override void _EnterTree()
	{
		((Node)this)._EnterTree();
		CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
		CombatManager.Instance.CombatSetUp += OnCombatSetup;
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
		CombatManager.Instance.CombatSetUp -= OnCombatSetup;
	}

	public static NOrbManager Create(NCreature creature, bool isLocal)
	{
		if (creature.Entity.Player == null)
		{
			throw new InvalidOperationException("NOrbManager can only be applied to player creatures");
		}
		NOrbManager nOrbManager = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NOrbManager>((GenEditState)0);
		nOrbManager._creatureNode = creature;
		nOrbManager.IsLocal = isLocal;
		return nOrbManager;
	}

	private void OnCombatSetup(CombatState _)
	{
		if (Player.Creature.IsAlive && Player.PlayerCombatState != null)
		{
			AddSlotAnim(Player.PlayerCombatState.OrbQueue.Capacity);
		}
	}

	public void RemoveSlotAnim(int amount)
	{
		if (amount > _orbs.Count)
		{
			throw new InvalidOperationException("There are not enough slots to remove.");
		}
		for (int i = 0; i < amount; i++)
		{
			NOrb nOrb = _orbs.Last();
			((Node)(object)nOrb).QueueFreeSafely();
			_orbs.Remove(nOrb);
			if (((Control)nOrb).HasFocus())
			{
				_creatureNode.Hitbox.TryGrabFocus();
			}
		}
		TweenLayout();
		UpdateControllerNavigation();
	}

	public void AddSlotAnim(int amount)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < amount; i++)
		{
			NOrb nOrb = NOrb.Create(LocalContext.IsMe(Player));
			((Node)(object)_orbContainer).AddChildSafely((Node?)(object)nOrb);
			_orbs.Add(nOrb);
			((Control)nOrb).Position = Vector2.Zero;
		}
		TweenLayout();
		UpdateControllerNavigation();
	}

	public void ReplaceOrb(OrbModel oldOrb, OrbModel newOrb)
	{
		for (int i = 0; i < _orbs.Count; i++)
		{
			if (_orbs[i].Model == oldOrb)
			{
				_orbs[i].ReplaceOrb(newOrb);
			}
		}
		UpdateControllerNavigation();
	}

	public void AddOrbAnim()
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		OrbModel model = Player.PlayerCombatState.OrbQueue.Orbs.LastOrDefault();
		NOrb nOrb = _orbs.FirstOrDefault((NOrb node) => node.Model == null);
		if (nOrb == null)
		{
			EvokeOrbAnim(_orbs.First((NOrb node) => node.Model != null).Model);
			nOrb = (NOrb)(object)((IEnumerable<Node>)((Node)_orbContainer).GetChildren(false)).First((Node node) => ((NOrb)(object)node).Model == null);
		}
		NOrb nOrb2 = NOrb.Create(LocalContext.IsMe(Player), model);
		((Node)nOrb).AddSibling((Node)(object)nOrb2, false);
		_orbs.Insert(_orbs.IndexOf(nOrb), nOrb2);
		((Control)nOrb2).Position = ((Control)nOrb).Position;
		((Node)(object)_orbContainer).RemoveChildSafely((Node?)(object)nOrb);
		_orbs.Remove(nOrb);
		((Node)(object)nOrb).QueueFreeSafely();
		TweenLayout();
		UpdateControllerNavigation();
	}

	public void EvokeOrbAnim(OrbModel orb)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		NOrb nOrb = _orbs.Last((NOrb node) => node.Model == orb);
		Tween val = ((Node)this).CreateTween();
		_orbs.Remove(nOrb);
		val.TweenProperty((GodotObject)(object)nOrb, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0), 0.25);
		val.Chain().TweenCallback(Callable.From((Action)nOrb.QueueFreeSafely));
		NOrb nOrb2 = NOrb.Create(LocalContext.IsMe(Player));
		((Node)(object)_orbContainer).AddChildSafely((Node?)(object)nOrb2);
		_orbs.Add(nOrb2);
		((Control)nOrb2).Position = Vector2.Zero;
		if (((Control)nOrb).HasFocus())
		{
			_creatureNode.Hitbox.TryGrabFocus();
		}
		TweenLayout();
		UpdateControllerNavigation();
	}

	private void UpdateControllerNavigation()
	{
		for (int i = 0; i < _orbs.Count; i++)
		{
			NOrb nOrb = _orbs[i];
			NodePath path;
			if (i <= 0)
			{
				List<NOrb> orbs = _orbs;
				path = ((Node)orbs[orbs.Count - 1]).GetPath();
			}
			else
			{
				path = ((Node)_orbs[i - 1]).GetPath();
			}
			((Control)nOrb).FocusNeighborRight = path;
			((Control)_orbs[i]).FocusNeighborLeft = ((i < _orbs.Count - 1) ? ((Node)_orbs[i + 1]).GetPath() : ((Node)_orbs[0]).GetPath());
			((Control)_orbs[i]).FocusNeighborTop = ((Node)_orbs[i]).GetPath();
			((Control)_orbs[i]).FocusNeighborBottom = ((Node)_creatureNode.Hitbox).GetPath();
		}
		_creatureNode.UpdateNavigation();
	}

	private void TweenLayout()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		int capacity = Player.PlayerCombatState.OrbQueue.Capacity;
		if (capacity != 0)
		{
			float num = 125f;
			float num2 = num / (float)(capacity - 1);
			float num3 = Mathf.Lerp(225f, 300f, ((float)capacity - 3f) / 7f);
			if (!IsLocal)
			{
				num3 *= 0.75f;
			}
			Tween? curTween = _curTween;
			if (curTween != null)
			{
				curTween.Kill();
			}
			_curTween = ((Node)this).CreateTween().SetParallel(true);
			for (int i = 0; i < capacity; i++)
			{
				float num4 = float.DegreesToRadians(-25f - num);
				Vector2 val = new Vector2(0f - Mathf.Cos(num4), Mathf.Sin(num4)) * num3;
				_curTween.TweenProperty((GodotObject)(object)_orbs[i], NodePath.op_Implicit("position"), Variant.op_Implicit(val), 0.44999998807907104).SetEase((EaseType)2).SetTrans((TransitionType)1);
				num -= num2;
			}
		}
	}

	private void OnCombatStateChanged(CombatState _)
	{
		UpdateVisuals(OrbEvokeType.None);
	}

	public void UpdateVisuals(OrbEvokeType evokeType)
	{
		foreach (NOrb orb in _orbs)
		{
			orb.UpdateVisuals(isEvoking: false);
		}
		switch (evokeType)
		{
		case OrbEvokeType.Front:
			_orbs.FirstOrDefault()?.UpdateVisuals(isEvoking: true);
			break;
		case OrbEvokeType.All:
		{
			foreach (NOrb orb2 in _orbs)
			{
				orb2.UpdateVisuals(isEvoking: true);
			}
			break;
		}
		case OrbEvokeType.None:
			break;
		}
	}

	public void ClearOrbs()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		Tween? curTween = _curTween;
		if (curTween != null)
		{
			curTween.Kill();
		}
		if (_orbs.Count == 0)
		{
			return;
		}
		_curTween = ((Node)this).CreateTween();
		foreach (NOrb orb in _orbs)
		{
			_curTween.Parallel().TweenProperty((GodotObject)(object)orb, NodePath.op_Implicit("position"), Variant.op_Implicit(Vector2.Zero), 1.0).SetEase((EaseType)2)
				.SetTrans((TransitionType)1);
			_curTween.Parallel().TweenProperty((GodotObject)(object)orb, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0), 0.25);
		}
		foreach (NOrb orb2 in _orbs)
		{
			_curTween.Chain().TweenCallback(Callable.From((Action)orb2.QueueFreeSafely));
		}
		_orbs.Clear();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("creature"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)1, StringName.op_Implicit("isLocal"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveSlotAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("amount"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddSlotAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("amount"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddOrbAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateControllerNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TweenLayout, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("evokeType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearOrbs, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NOrbManager nOrbManager = Create(VariantUtils.ConvertTo<NCreature>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NOrbManager>(ref nOrbManager);
			return true;
		}
		if ((ref method) == MethodName.RemoveSlotAnim && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RemoveSlotAnim(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddSlotAnim && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AddSlotAnim(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddOrbAnim && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AddOrbAnim();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateControllerNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateControllerNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TweenLayout && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TweenLayout();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateVisuals(VariantUtils.ConvertTo<OrbEvokeType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearOrbs && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearOrbs();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NOrbManager nOrbManager = Create(VariantUtils.ConvertTo<NCreature>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NOrbManager>(ref nOrbManager);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveSlotAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.AddSlotAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.AddOrbAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateControllerNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.TweenLayout)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearOrbs)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.IsLocal)
		{
			IsLocal = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._orbContainer)
		{
			_orbContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._creatureNode)
		{
			_creatureNode = VariantUtils.ConvertTo<NCreature>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._curTween)
		{
			_curTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsLocal)
		{
			bool isLocal = IsLocal;
			value = VariantUtils.CreateFrom<bool>(ref isLocal);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusOwner)
		{
			Control defaultFocusOwner = DefaultFocusOwner;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusOwner);
			return true;
		}
		if ((ref name) == PropertyName._orbContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _orbContainer);
			return true;
		}
		if ((ref name) == PropertyName._creatureNode)
		{
			value = VariantUtils.CreateFrom<NCreature>(ref _creatureNode);
			return true;
		}
		if ((ref name) == PropertyName._curTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _curTween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.IsLocal, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._orbContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._creatureNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._curTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusOwner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName isLocal = PropertyName.IsLocal;
		bool isLocal2 = IsLocal;
		info.AddProperty(isLocal, Variant.From<bool>(ref isLocal2));
		info.AddProperty(PropertyName._orbContainer, Variant.From<Control>(ref _orbContainer));
		info.AddProperty(PropertyName._creatureNode, Variant.From<NCreature>(ref _creatureNode));
		info.AddProperty(PropertyName._curTween, Variant.From<Tween>(ref _curTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsLocal, ref val))
		{
			IsLocal = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._orbContainer, ref val2))
		{
			_orbContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._creatureNode, ref val3))
		{
			_creatureNode = ((Variant)(ref val3)).As<NCreature>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._curTween, ref val4))
		{
			_curTween = ((Variant)(ref val4)).As<Tween>();
		}
	}
}
