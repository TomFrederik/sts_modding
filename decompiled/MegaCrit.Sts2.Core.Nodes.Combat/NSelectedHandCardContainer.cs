using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NSelectedHandCardContainer.cs")]
public class NSelectedHandCardContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Add = StringName.op_Implicit("Add");

		public static readonly StringName RefreshHolderPositions = StringName.op_Implicit("RefreshHolderPositions");

		public static readonly StringName DeselectHolder = StringName.op_Implicit("DeselectHolder");

		public static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Hand = StringName.op_Implicit("Hand");
	}

	public class SignalName : SignalName
	{
	}

	public NPlayerHand? Hand { get; set; }

	public List<NSelectedHandCardHolder> Holders => ((IEnumerable)((Node)this).GetChildren(false)).OfType<NSelectedHandCardHolder>().ToList();

	public override void _Ready()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).Connect(SignalName.FocusEntered, Callable.From((Action)OnFocus), 0u);
	}

	public NSelectedHandCardHolder Add(NHandCardHolder originalHolder)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		NCard cardNode = originalHolder.CardNode;
		Vector2 globalPosition = ((Control)cardNode).GlobalPosition;
		NSelectedHandCardHolder nSelectedHandCardHolder = NSelectedHandCardHolder.Create(originalHolder);
		((GodotObject)nSelectedHandCardHolder).Connect(NCardHolder.SignalName.Pressed, Callable.From<NCardHolder>((Action<NCardHolder>)DeselectHolder), 4u);
		((Node)(object)this).AddChildSafely((Node?)(object)nSelectedHandCardHolder);
		RefreshHolderPositions();
		((Control)cardNode).GlobalPosition = globalPosition;
		return nSelectedHandCardHolder;
	}

	private void RefreshHolderPositions()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		int count = Holders.Count;
		((Control)this).FocusMode = (FocusModeEnum)((count > 0) ? 2 : 0);
		if (count != 0)
		{
			float x = ((Control)Holders.First()).Size.X;
			float num = (0f - x) * (float)(count - 1) / 2f;
			for (int i = 0; i < count; i++)
			{
				((Control)Holders[i]).Position = new Vector2(num, 0f);
				num += x;
				((Control)Holders[i]).FocusNeighborLeft = ((i > 0) ? ((Node)Holders[i - 1]).GetPath() : ((Node)Holders[Holders.Count - 1]).GetPath());
				((Control)Holders[i]).FocusNeighborRight = ((i < Holders.Count - 1) ? ((Node)Holders[i + 1]).GetPath() : ((Node)Holders[0]).GetPath());
			}
		}
	}

	private void DeselectHolder(NCardHolder holder)
	{
		NSelectedHandCardHolder nSelectedHandCardHolder = (NSelectedHandCardHolder)holder;
		NCard cardNode = nSelectedHandCardHolder.CardNode;
		Hand.DeselectCard(cardNode);
		((Node)(object)this).RemoveChildSafely((Node?)(object)nSelectedHandCardHolder);
		((Node)(object)nSelectedHandCardHolder).QueueFreeSafely();
		RefreshHolderPositions();
	}

	public void DeselectCard(CardModel card)
	{
		NSelectedHandCardHolder holder = Holders.First((NSelectedHandCardHolder child) => child.CardNode.Model == card);
		DeselectHolder(holder);
	}

	private void OnFocus()
	{
		((Control)(object)Holders.FirstOrDefault())?.TryGrabFocus();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Add, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("originalHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshHolderPositions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DeselectHolder, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Add && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NSelectedHandCardHolder nSelectedHandCardHolder = Add(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NSelectedHandCardHolder>(ref nSelectedHandCardHolder);
			return true;
		}
		if ((ref method) == MethodName.RefreshHolderPositions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshHolderPositions();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DeselectHolder && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DeselectHolder(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
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
		if ((ref method) == MethodName.Add)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshHolderPositions)
		{
			return true;
		}
		if ((ref method) == MethodName.DeselectHolder)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Hand)
		{
			Hand = VariantUtils.ConvertTo<NPlayerHand>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Hand)
		{
			NPlayerHand hand = Hand;
			value = VariantUtils.CreateFrom<NPlayerHand>(ref hand);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.Hand, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName hand = PropertyName.Hand;
		NPlayerHand hand2 = Hand;
		info.AddProperty(hand, Variant.From<NPlayerHand>(ref hand2));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Hand, ref val))
		{
			Hand = ((Variant)(ref val)).As<NPlayerHand>();
		}
	}
}
