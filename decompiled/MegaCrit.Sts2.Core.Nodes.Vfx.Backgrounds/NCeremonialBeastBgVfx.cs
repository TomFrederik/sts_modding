using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Audio;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Backgrounds;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/Backgrounds/NCeremonialBeastBgVfx.cs")]
public class NCeremonialBeastBgVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName PlayGlow = StringName.op_Implicit("PlayGlow");

		public static readonly StringName PlaySkulls = StringName.op_Implicit("PlaySkulls");

		public static readonly StringName PlayFlowers = StringName.op_Implicit("PlayFlowers");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _isGlowOn = StringName.op_Implicit("_isGlowOn");

		public static readonly StringName _areSkullsOn = StringName.op_Implicit("_areSkullsOn");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private bool _isGlowOn;

	private bool _areSkullsOn;

	private Node2D _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		((CanvasItem)_parent).Visible = false;
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
	}

	public override void _EnterTree()
	{
		CombatManager.Instance.StateTracker.CombatStateChanged += UpdateState;
	}

	public override void _ExitTree()
	{
		CombatManager.Instance.StateTracker.CombatStateChanged -= UpdateState;
	}

	private void UpdateState(CombatState combatState)
	{
		UpdateRingingSfx(combatState);
		UpdateVfxAndMusic(combatState);
	}

	private void UpdateRingingSfx(CombatState combatState)
	{
		bool flag = LocalContext.GetMe(combatState).Creature.HasPower<RingingPower>();
		NRunMusicController.Instance?.UpdateMusicParameter("ringing", flag ? 1 : 0);
	}

	private void UpdateVfxAndMusic(CombatState combatState)
	{
		Creature creature = combatState.Creatures.FirstOrDefault((Creature c) => c.Monster is CeremonialBeast);
		if (creature == null)
		{
			NRunMusicController.Instance?.UpdateMusicParameter("ceremonial_beast_progress", 5f);
			PlayFlowers();
			return;
		}
		if ((float)creature.CurrentHp > (float)creature.MaxHp * 0.66f)
		{
			((CanvasItem)_parent).Visible = false;
			return;
		}
		((CanvasItem)_parent).Visible = true;
		if ((float)creature.CurrentHp > (float)creature.MaxHp * 0.33f)
		{
			NRunMusicController.Instance?.UpdateMusicParameter("ceremonial_beast_progress", 1f);
			PlayGlow();
		}
		else if (creature.IsAlive)
		{
			NRunMusicController.Instance?.UpdateMusicParameter("ceremonial_beast_progress", 1f);
			PlaySkulls();
		}
		else
		{
			NRunMusicController.Instance?.UpdateMusicParameter("ceremonial_beast_progress", 5f);
			PlayFlowers();
		}
	}

	private void PlayGlow()
	{
		if (!_isGlowOn)
		{
			_isGlowOn = true;
			MegaAnimationState animationState = _animController.GetAnimationState();
			animationState.SetAnimation("glow_spawn");
			animationState.AddAnimation("glow_idle");
		}
	}

	private void PlaySkulls()
	{
		if (!_areSkullsOn)
		{
			_areSkullsOn = true;
			MegaAnimationState animationState = _animController.GetAnimationState();
			animationState.SetAnimation("skulls_spawn");
			animationState.AddAnimation("glow_and_skulls_idle");
		}
	}

	private void PlayFlowers()
	{
		MegaAnimationState animationState = _animController.GetAnimationState();
		animationState.SetAnimation("glow_and_skulls_idle");
		animationState.AddAnimation("plants_spawn", 4.5f, loop: false);
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
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayGlow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlaySkulls, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayFlowers, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.PlayGlow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayGlow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlaySkulls && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlaySkulls();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayFlowers && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayFlowers();
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
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
		if ((ref method) == MethodName.PlayGlow)
		{
			return true;
		}
		if ((ref method) == MethodName.PlaySkulls)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayFlowers)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._isGlowOn)
		{
			_isGlowOn = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._areSkullsOn)
		{
			_areSkullsOn = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			_parent = VariantUtils.ConvertTo<Node2D>(ref value);
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
		if ((ref name) == PropertyName._isGlowOn)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isGlowOn);
			return true;
		}
		if ((ref name) == PropertyName._areSkullsOn)
		{
			value = VariantUtils.CreateFrom<bool>(ref _areSkullsOn);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _parent);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._isGlowOn, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._areSkullsOn, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._isGlowOn, Variant.From<bool>(ref _isGlowOn));
		info.AddProperty(PropertyName._areSkullsOn, Variant.From<bool>(ref _areSkullsOn));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._isGlowOn, ref val))
		{
			_isGlowOn = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._areSkullsOn, ref val2))
		{
			_areSkullsOn = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val3))
		{
			_parent = ((Variant)(ref val3)).As<Node2D>();
		}
	}
}
