using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Events;

[ScriptPath("res://src/Core/Nodes/Events/NCombatEventLayout.cs")]
public class NCombatEventLayout : NEventLayout
{
	public new class MethodName : NEventLayout.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetCombatRoomNode = StringName.op_Implicit("SetCombatRoomNode");

		public new static readonly StringName InitializeVisuals = StringName.op_Implicit("InitializeVisuals");

		public static readonly StringName HideEventVisuals = StringName.op_Implicit("HideEventVisuals");
	}

	public new class PropertyName : NEventLayout.PropertyName
	{
		public static readonly StringName EmbeddedCombatRoom = StringName.op_Implicit("EmbeddedCombatRoom");

		public static readonly StringName HasCombatStarted = StringName.op_Implicit("HasCombatStarted");

		public new static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _combatRoomContainer = StringName.op_Implicit("_combatRoomContainer");
	}

	public new class SignalName : NEventLayout.SignalName
	{
	}

	public const string combatScenePath = "res://scenes/events/combat_event_layout.tscn";

	private Control _combatRoomContainer;

	public NCombatRoom? EmbeddedCombatRoom { get; private set; }

	public bool HasCombatStarted { get; private set; }

	public override Control? DefaultFocusedControl
	{
		get
		{
			if (!HasCombatStarted)
			{
				return base.DefaultFocusedControl;
			}
			return EmbeddedCombatRoom?.DefaultFocusedControl;
		}
	}

	public override void _Ready()
	{
		base._Ready();
		_combatRoomContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CombatRoomContainer"));
	}

	public void SetCombatRoomNode(NCombatRoom? combatRoomNode)
	{
		if (combatRoomNode != null)
		{
			if (EmbeddedCombatRoom != null)
			{
				throw new InvalidOperationException("Combat room node was already set.");
			}
			EmbeddedCombatRoom = combatRoomNode;
			((Node)(object)_combatRoomContainer).AddChildSafely((Node?)(object)combatRoomNode);
		}
	}

	public override void SetEvent(EventModel eventModel)
	{
		IRunState runState = eventModel.Owner.RunState;
		ICombatRoomVisuals visuals = eventModel.CreateCombatRoomVisuals(runState.Players, runState.Act);
		NCombatRoom nCombatRoom = NCombatRoom.Create(visuals, CombatRoomMode.VisualOnly);
		SetCombatRoomNode(nCombatRoom);
		nCombatRoom?.SetUpBackground(runState);
		base.SetEvent(eventModel);
	}

	protected override void InitializeVisuals()
	{
	}

	public void HideEventVisuals()
	{
		if (_description != null)
		{
			((CanvasItem)_description).Visible = false;
		}
		if (_sharedEventLabel != null)
		{
			((CanvasItem)_sharedEventLabel).Visible = false;
		}
		((CanvasItem)_optionsContainer).Visible = false;
		HasCombatStarted = true;
		DefaultFocusedControl?.TryGrabFocus();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCombatRoomNode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("combatRoomNode"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideEventVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCombatRoomNode && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCombatRoomNode(VariantUtils.ConvertTo<NCombatRoom>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitializeVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitializeVisuals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideEventVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideEventVisuals();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.SetCombatRoomNode)
		{
			return true;
		}
		if ((ref method) == MethodName.InitializeVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.HideEventVisuals)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.EmbeddedCombatRoom)
		{
			EmbeddedCombatRoom = VariantUtils.ConvertTo<NCombatRoom>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.HasCombatStarted)
		{
			HasCombatStarted = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._combatRoomContainer)
		{
			_combatRoomContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.EmbeddedCombatRoom)
		{
			NCombatRoom embeddedCombatRoom = EmbeddedCombatRoom;
			value = VariantUtils.CreateFrom<NCombatRoom>(ref embeddedCombatRoom);
			return true;
		}
		if ((ref name) == PropertyName.HasCombatStarted)
		{
			bool hasCombatStarted = HasCombatStarted;
			value = VariantUtils.CreateFrom<bool>(ref hasCombatStarted);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._combatRoomContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _combatRoomContainer);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._combatRoomContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.EmbeddedCombatRoom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.HasCombatStarted, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName embeddedCombatRoom = PropertyName.EmbeddedCombatRoom;
		NCombatRoom embeddedCombatRoom2 = EmbeddedCombatRoom;
		info.AddProperty(embeddedCombatRoom, Variant.From<NCombatRoom>(ref embeddedCombatRoom2));
		StringName hasCombatStarted = PropertyName.HasCombatStarted;
		bool hasCombatStarted2 = HasCombatStarted;
		info.AddProperty(hasCombatStarted, Variant.From<bool>(ref hasCombatStarted2));
		info.AddProperty(PropertyName._combatRoomContainer, Variant.From<Control>(ref _combatRoomContainer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.EmbeddedCombatRoom, ref val))
		{
			EmbeddedCombatRoom = ((Variant)(ref val)).As<NCombatRoom>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.HasCombatStarted, ref val2))
		{
			HasCombatStarted = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._combatRoomContainer, ref val3))
		{
			_combatRoomContainer = ((Variant)(ref val3)).As<Control>();
		}
	}
}
