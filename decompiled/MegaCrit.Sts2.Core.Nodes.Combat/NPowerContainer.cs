using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NPowerContainer.cs")]
public class NPowerContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName ConnectCreatureSignals = StringName.op_Implicit("ConnectCreatureSignals");

		public static readonly StringName SetCreatureBounds = StringName.op_Implicit("SetCreatureBounds");

		public static readonly StringName UpdatePositions = StringName.op_Implicit("UpdatePositions");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	private Creature? _creature;

	private Vector2? _originalPosition;

	private readonly List<NPower> _powerNodes = new List<NPower>();

	public override void _EnterTree()
	{
		((Node)this)._EnterTree();
		ConnectCreatureSignals();
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		if (_creature != null)
		{
			_creature.PowerApplied -= OnPowerApplied;
			_creature.PowerRemoved -= OnPowerRemoved;
		}
	}

	private void ConnectCreatureSignals()
	{
		if (_creature != null)
		{
			_creature.PowerApplied -= OnPowerApplied;
			_creature.PowerRemoved -= OnPowerRemoved;
			_creature.PowerApplied += OnPowerApplied;
			_creature.PowerRemoved += OnPowerRemoved;
		}
	}

	public void SetCreatureBounds(Control bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).GlobalPosition = new Vector2(bounds.GlobalPosition.X, ((Control)this).GlobalPosition.Y);
		((Control)this).Size = new Vector2(bounds.Size.X * bounds.Scale.X + 25f, ((Control)this).Size.Y);
		_originalPosition = ((Control)this).Position;
		UpdatePositions();
	}

	private void Add(PowerModel power)
	{
		if (power.IsVisible)
		{
			NPower nPower = NPower.Create(power);
			nPower.Container = this;
			_powerNodes.Add(nPower);
			((Node)(object)this).AddChildSafely((Node?)(object)nPower);
			UpdatePositions();
		}
	}

	private void Remove(PowerModel power)
	{
		if (CombatManager.Instance.IsInProgress)
		{
			NPower nPower = _powerNodes.FirstOrDefault((NPower n) => n.Model == power);
			if (nPower != null)
			{
				_powerNodes.Remove(nPower);
				UpdatePositions();
				((Node)(object)nPower).QueueFreeSafely();
			}
		}
	}

	private void UpdatePositions()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		if (_powerNodes.Count != 0)
		{
			float x = ((Control)_powerNodes[0]).Size.X;
			float num = Mathf.CeilToInt(((Control)this).Size.X / x);
			num = Mathf.Max((float)Mathf.CeilToInt((float)_powerNodes.Count / 2f), num);
			for (int i = 0; i < _powerNodes.Count; i++)
			{
				((Control)_powerNodes[i]).Position = new Vector2(x * ((float)i % num), Mathf.Floor((float)i / num) * x);
			}
			float num2 = x * Mathf.Min(num, (float)_powerNodes.Count);
			((Control)this).Position = (Vector2)(((_003F?)_originalPosition) ?? ((Control)this).Position) + Vector2.Left * Mathf.Max(0f, num2 - ((Control)this).Size.X) / 2f;
		}
	}

	public void SetCreature(Creature creature)
	{
		if (_creature != null)
		{
			throw new InvalidOperationException("Creature was already set.");
		}
		_creature = creature;
		ConnectCreatureSignals();
		foreach (PowerModel power in _creature.Powers)
		{
			Add(power);
		}
	}

	private void OnPowerApplied(PowerModel power)
	{
		Add(power);
	}

	private void OnPowerRemoved(PowerModel power)
	{
		Remove(power);
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
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectCreatureSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCreatureBounds, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("bounds"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdatePositions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.ConnectCreatureSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectCreatureSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCreatureBounds && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCreatureBounds(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdatePositions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdatePositions();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.ConnectCreatureSignals)
		{
			return true;
		}
		if ((ref method) == MethodName.SetCreatureBounds)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdatePositions)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
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
