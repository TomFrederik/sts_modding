using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NControllerCardPlay.cs")]
public class NControllerCardPlay : NCardPlay
{
	[Signal]
	public delegate void ConfirmedEventHandler();

	[Signal]
	public delegate void CanceledEventHandler();

	public new class MethodName : NCardPlay.MethodName
	{
		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName Start = StringName.op_Implicit("Start");

		public static readonly StringName MultiCreatureTargeting = StringName.op_Implicit("MultiCreatureTargeting");

		public new static readonly StringName OnCancelPlayCard = StringName.op_Implicit("OnCancelPlayCard");

		public new static readonly StringName Cleanup = StringName.op_Implicit("Cleanup");
	}

	public new class PropertyName : NCardPlay.PropertyName
	{
	}

	public new class SignalName : NCardPlay.SignalName
	{
		public static readonly StringName Confirmed = StringName.op_Implicit("Confirmed");

		public static readonly StringName Canceled = StringName.op_Implicit("Canceled");
	}

	private ConfirmedEventHandler backing_Confirmed;

	private CanceledEventHandler backing_Canceled;

	public event ConfirmedEventHandler Confirmed
	{
		add
		{
			backing_Confirmed = (ConfirmedEventHandler)Delegate.Combine(backing_Confirmed, value);
		}
		remove
		{
			backing_Confirmed = (ConfirmedEventHandler)Delegate.Remove(backing_Confirmed, value);
		}
	}

	public event CanceledEventHandler Canceled
	{
		add
		{
			backing_Canceled = (CanceledEventHandler)Delegate.Combine(backing_Canceled, value);
		}
		remove
		{
			backing_Canceled = (CanceledEventHandler)Delegate.Remove(backing_Canceled, value);
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		InputEventAction val = (InputEventAction)(object)((inputEvent is InputEventAction) ? inputEvent : null);
		if (val != null)
		{
			if (((InputEvent)val).IsActionPressed(MegaInput.select, false, false))
			{
				((GodotObject)this).EmitSignal(SignalName.Confirmed, Array.Empty<Variant>());
			}
			if (((InputEvent)val).IsActionPressed(MegaInput.cancel, false, false))
			{
				((GodotObject)this).EmitSignal(SignalName.Canceled, Array.Empty<Variant>());
			}
		}
	}

	public static NControllerCardPlay Create(NHandCardHolder holder)
	{
		NControllerCardPlay nControllerCardPlay = new NControllerCardPlay();
		nControllerCardPlay.Holder = holder;
		nControllerCardPlay.Player = holder.CardModel.Owner;
		return nControllerCardPlay;
	}

	public override void Start()
	{
		if (base.Card == null || base.CardNode == null)
		{
			return;
		}
		NDebugAudioManager.Instance?.Play("card_select.mp3");
		NHoverTipSet.Remove((Control)(object)base.Holder);
		if (!base.Card.CanPlay(out UnplayableReason reason, out AbstractModel preventer))
		{
			CannotPlayThisCardFtueCheck(base.Card);
			CancelPlayCard();
			LocString playerDialogueLine = reason.GetPlayerDialogueLine(preventer);
			if (playerDialogueLine != null)
			{
				((Node)(object)NCombatRoom.Instance.CombatVfxContainer).AddChildSafely((Node?)(object)NThoughtBubbleVfx.Create(playerDialogueLine.GetFormattedText(), base.Card.Owner.Creature, 1.0));
			}
			return;
		}
		TryShowEvokingOrbs();
		base.CardNode.CardHighlight.AnimFlash();
		CenterCard();
		TargetType targetType = base.Card.TargetType;
		if ((targetType == TargetType.AnyEnemy || targetType == TargetType.AnyAlly) ? true : false)
		{
			TaskHelper.RunSafely(SingleCreatureTargeting(base.Card.TargetType));
		}
		else
		{
			MultiCreatureTargeting();
		}
	}

	private async Task SingleCreatureTargeting(TargetType targetType)
	{
		NTargetManager targetManager = NTargetManager.Instance;
		((GodotObject)targetManager).Connect(NTargetManager.SignalName.CreatureHovered, Callable.From<NCreature>((Action<NCreature>)base.OnCreatureHover), 0u);
		((GodotObject)targetManager).Connect(NTargetManager.SignalName.CreatureUnhovered, Callable.From<NCreature>((Action<NCreature>)base.OnCreatureUnhover), 0u);
		targetManager.StartTargeting(targetType, (Control)(object)base.CardNode, TargetMode.Controller, () => !GodotObject.IsInstanceValid((GodotObject)(object)this) || !NControllerManager.Instance.IsUsingController, null);
		Creature owner = base.Card.Owner.Creature;
		List<Creature> list = new List<Creature>();
		switch (targetType)
		{
		case TargetType.AnyEnemy:
			list = (from c in owner.CombatState.GetOpponentsOf(owner)
				where c.IsHittable
				select c).ToList();
			break;
		case TargetType.AnyAlly:
			list = base.Card.CombatState.PlayerCreatures.Where((Creature c) => c.IsHittable && c != owner).ToList();
			break;
		}
		if (list.Count == 0)
		{
			CancelPlayCard();
			return;
		}
		NCombatRoom.Instance.RestrictControllerNavigation(list.Select((Creature c) => NCombatRoom.Instance.GetCreatureNode(c).Hitbox));
		NCombatRoom.Instance.GetCreatureNode(list.First()).Hitbox.TryGrabFocus();
		NCreature nCreature = (NCreature)(object)(await targetManager.SelectionFinished());
		if (GodotObject.IsInstanceValid((GodotObject)(object)this))
		{
			((GodotObject)targetManager).Disconnect(NTargetManager.SignalName.CreatureHovered, Callable.From<NCreature>((Action<NCreature>)base.OnCreatureHover));
			((GodotObject)targetManager).Disconnect(NTargetManager.SignalName.CreatureUnhovered, Callable.From<NCreature>((Action<NCreature>)base.OnCreatureUnhover));
			if (nCreature != null)
			{
				TryPlayCard(nCreature.Entity);
			}
			else
			{
				CancelPlayCard();
			}
		}
	}

	private void MultiCreatureTargeting()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		NCombatRoom.Instance.RestrictControllerNavigation(Array.Empty<Control>());
		ShowMultiCreatureTargetingVisuals();
		((GodotObject)this).Connect(SignalName.Confirmed, Callable.From((Action)delegate
		{
			TryPlayCard(null);
		}), 0u);
		((GodotObject)this).Connect(SignalName.Canceled, Callable.From((Action)base.CancelPlayCard), 0u);
	}

	protected override void OnCancelPlayCard()
	{
		((Control)(object)base.Holder).TryGrabFocus();
	}

	protected override void Cleanup()
	{
		base.Cleanup();
		NCombatRoom.Instance.EnableControllerNavigation();
		NCombatRoom.Instance.Ui.Hand.DefaultFocusedControl.TryGrabFocus();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Start, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MultiCreatureTargeting, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCancelPlayCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Cleanup, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NControllerCardPlay nControllerCardPlay = Create(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NControllerCardPlay>(ref nControllerCardPlay);
			return true;
		}
		if ((ref method) == MethodName.Start && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Start();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MultiCreatureTargeting && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			MultiCreatureTargeting();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCancelPlayCard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCancelPlayCard();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Cleanup && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Cleanup();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NControllerCardPlay nControllerCardPlay = Create(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NControllerCardPlay>(ref nControllerCardPlay);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName.Start)
		{
			return true;
		}
		if ((ref method) == MethodName.MultiCreatureTargeting)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCancelPlayCard)
		{
			return true;
		}
		if ((ref method) == MethodName.Cleanup)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddSignalEventDelegate(SignalName.Confirmed, (Delegate)backing_Confirmed);
		info.AddSignalEventDelegate(SignalName.Canceled, (Delegate)backing_Canceled);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		ConfirmedEventHandler confirmedEventHandler = default(ConfirmedEventHandler);
		if (info.TryGetSignalEventDelegate<ConfirmedEventHandler>(SignalName.Confirmed, ref confirmedEventHandler))
		{
			backing_Confirmed = confirmedEventHandler;
		}
		CanceledEventHandler canceledEventHandler = default(CanceledEventHandler);
		if (info.TryGetSignalEventDelegate<CanceledEventHandler>(SignalName.Canceled, ref canceledEventHandler))
		{
			backing_Canceled = canceledEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(SignalName.Confirmed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.Canceled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalConfirmed()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Confirmed, Array.Empty<Variant>());
	}

	protected void EmitSignalCanceled()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Canceled, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Confirmed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_Confirmed?.Invoke();
		}
		else if ((ref signal) == SignalName.Canceled && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_Canceled?.Invoke();
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Confirmed)
		{
			return true;
		}
		if ((ref signal) == SignalName.Canceled)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
