using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NExhaustPileButton.cs")]
public class NExhaustPileButton : NCombatCardPile
{
	public new class MethodName : NCombatCardPile.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName AddCard = StringName.op_Implicit("AddCard");

		public new static readonly StringName SetAnimInOutPositions = StringName.op_Implicit("SetAnimInOutPositions");

		public new static readonly StringName AnimIn = StringName.op_Implicit("AnimIn");
	}

	public new class PropertyName : NCombatCardPile.PropertyName
	{
		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");

		public new static readonly StringName Pile = StringName.op_Implicit("Pile");

		public static readonly StringName _viewport = StringName.op_Implicit("_viewport");

		public static readonly StringName _posOffset = StringName.op_Implicit("_posOffset");
	}

	public new class SignalName : NCombatCardPile.SignalName
	{
	}

	private Viewport _viewport;

	private Vector2 _posOffset;

	private static readonly Vector2 _hideOffset = new Vector2(150f, 0f);

	protected override string[] Hotkeys => new string[1] { StringName.op_Implicit(MegaInput.viewExhaustPileAndTabRight) };

	protected override PileType Pile => PileType.Exhaust;

	public override void _Ready()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		((CanvasItem)this).Visible = false;
		_viewport = ((Node)this).GetViewport();
		_posOffset = new Vector2(((Control)this).OffsetRight + 100f, 0f - ((Control)this).OffsetBottom + 90f);
		((GodotObject)((Node)this).GetTree().Root).Connect(SignalName.SizeChanged, Callable.From((Action)SetAnimInOutPositions), 0u);
		SetAnimInOutPositions();
		Disable();
	}

	public override void Initialize(Player player)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(player);
		if (Pile.GetPile(player).Cards.Count > 0)
		{
			((CanvasItem)this).Visible = true;
			((Control)this).Position = _showPosition;
			Enable();
		}
	}

	protected override void AddCard()
	{
		base.AddCard();
		if (!((CanvasItem)this).Visible)
		{
			AnimIn();
		}
		Enable();
	}

	protected override void SetAnimInOutPositions()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		_showPosition = ((Control)NGame.Instance).Size - _posOffset;
		_hidePosition = _showPosition + _hideOffset;
	}

	public override void AnimIn()
	{
		base.AnimIn();
		((CanvasItem)this).Visible = true;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
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
		list.Add(new MethodInfo(MethodName.AddCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAnimInOutPositions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddCard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AddCard();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAnimInOutPositions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetAnimInOutPositions();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimIn();
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
		if ((ref method) == MethodName.AddCard)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAnimInOutPositions)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimIn)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._viewport)
		{
			_viewport = VariantUtils.ConvertTo<Viewport>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._posOffset)
		{
			_posOffset = VariantUtils.ConvertTo<Vector2>(ref value);
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
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Hotkeys)
		{
			string[] hotkeys = Hotkeys;
			value = VariantUtils.CreateFrom<string[]>(ref hotkeys);
			return true;
		}
		if ((ref name) == PropertyName.Pile)
		{
			PileType pile = Pile;
			value = VariantUtils.CreateFrom<PileType>(ref pile);
			return true;
		}
		if ((ref name) == PropertyName._viewport)
		{
			value = VariantUtils.CreateFrom<Viewport>(ref _viewport);
			return true;
		}
		if ((ref name) == PropertyName._posOffset)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _posOffset);
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
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewport, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._posOffset, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.Pile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._viewport, Variant.From<Viewport>(ref _viewport));
		info.AddProperty(PropertyName._posOffset, Variant.From<Vector2>(ref _posOffset));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._viewport, ref val))
		{
			_viewport = ((Variant)(ref val)).As<Viewport>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._posOffset, ref val2))
		{
			_posOffset = ((Variant)(ref val2)).As<Vector2>();
		}
	}
}
