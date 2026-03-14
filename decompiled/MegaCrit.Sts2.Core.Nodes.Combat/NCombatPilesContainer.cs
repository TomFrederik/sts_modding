using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NCombatPilesContainer.cs")]
public class NCombatPilesContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName AnimIn = StringName.op_Implicit("AnimIn");

		public static readonly StringName AnimOut = StringName.op_Implicit("AnimOut");

		public static readonly StringName Enable = StringName.op_Implicit("Enable");

		public static readonly StringName Disable = StringName.op_Implicit("Disable");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DrawPile = StringName.op_Implicit("DrawPile");

		public static readonly StringName DiscardPile = StringName.op_Implicit("DiscardPile");

		public static readonly StringName ExhaustPile = StringName.op_Implicit("ExhaustPile");

		public static readonly StringName _drawPile = StringName.op_Implicit("_drawPile");

		public static readonly StringName _discardPile = StringName.op_Implicit("_discardPile");

		public static readonly StringName _exhaustPile = StringName.op_Implicit("_exhaustPile");
	}

	public class SignalName : SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("combat/combat_piles_container");

	private NDrawPileButton _drawPile;

	private NDiscardPileButton _discardPile;

	private NExhaustPileButton _exhaustPile;

	public NDrawPileButton DrawPile => _drawPile;

	public NDiscardPileButton DiscardPile => _discardPile;

	public NExhaustPileButton ExhaustPile => _exhaustPile;

	public override void _Ready()
	{
		_drawPile = ((Node)this).GetNode<NDrawPileButton>(NodePath.op_Implicit("%DrawPile"));
		_discardPile = ((Node)this).GetNode<NDiscardPileButton>(NodePath.op_Implicit("%DiscardPile"));
		_exhaustPile = ((Node)this).GetNode<NExhaustPileButton>(NodePath.op_Implicit("%ExhaustPile"));
	}

	public void Initialize(Player player)
	{
		_drawPile.Initialize(player);
		_discardPile.Initialize(player);
		_exhaustPile.Initialize(player);
	}

	public void AnimIn()
	{
		_drawPile.AnimIn();
		_discardPile.AnimIn();
	}

	public void AnimOut()
	{
		_drawPile.AnimOut();
		_discardPile.AnimOut();
		_exhaustPile.AnimOut();
	}

	public void Enable()
	{
		_drawPile.Enable();
		_discardPile.Enable();
		_exhaustPile.Enable();
	}

	public void Disable()
	{
		_drawPile.Disable();
		_discardPile.Disable();
		_exhaustPile.Disable();
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
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimOut, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Enable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Disable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimOut && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimOut();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Enable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Enable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Disable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Disable();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimIn)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimOut)
		{
			return true;
		}
		if ((ref method) == MethodName.Enable)
		{
			return true;
		}
		if ((ref method) == MethodName.Disable)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._drawPile)
		{
			_drawPile = VariantUtils.ConvertTo<NDrawPileButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._discardPile)
		{
			_discardPile = VariantUtils.ConvertTo<NDiscardPileButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._exhaustPile)
		{
			_exhaustPile = VariantUtils.ConvertTo<NExhaustPileButton>(ref value);
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
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.DrawPile)
		{
			NDrawPileButton drawPile = DrawPile;
			value = VariantUtils.CreateFrom<NDrawPileButton>(ref drawPile);
			return true;
		}
		if ((ref name) == PropertyName.DiscardPile)
		{
			NDiscardPileButton discardPile = DiscardPile;
			value = VariantUtils.CreateFrom<NDiscardPileButton>(ref discardPile);
			return true;
		}
		if ((ref name) == PropertyName.ExhaustPile)
		{
			NExhaustPileButton exhaustPile = ExhaustPile;
			value = VariantUtils.CreateFrom<NExhaustPileButton>(ref exhaustPile);
			return true;
		}
		if ((ref name) == PropertyName._drawPile)
		{
			value = VariantUtils.CreateFrom<NDrawPileButton>(ref _drawPile);
			return true;
		}
		if ((ref name) == PropertyName._discardPile)
		{
			value = VariantUtils.CreateFrom<NDiscardPileButton>(ref _discardPile);
			return true;
		}
		if ((ref name) == PropertyName._exhaustPile)
		{
			value = VariantUtils.CreateFrom<NExhaustPileButton>(ref _exhaustPile);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._drawPile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._discardPile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._exhaustPile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DrawPile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DiscardPile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.ExhaustPile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._drawPile, Variant.From<NDrawPileButton>(ref _drawPile));
		info.AddProperty(PropertyName._discardPile, Variant.From<NDiscardPileButton>(ref _discardPile));
		info.AddProperty(PropertyName._exhaustPile, Variant.From<NExhaustPileButton>(ref _exhaustPile));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._drawPile, ref val))
		{
			_drawPile = ((Variant)(ref val)).As<NDrawPileButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._discardPile, ref val2))
		{
			_discardPile = ((Variant)(ref val2)).As<NDiscardPileButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._exhaustPile, ref val3))
		{
			_exhaustPile = ((Variant)(ref val3)).As<NExhaustPileButton>();
		}
	}
}
