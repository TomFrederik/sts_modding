using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Shops;

[ScriptPath("res://src/Core/Nodes/Screens/Shops/NMerchantDialogue.cs")]
public class NMerchantDialogue : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ShowOnInventoryOpen = StringName.op_Implicit("ShowOnInventoryOpen");

		public static readonly StringName ShowForPurchaseAttempt = StringName.op_Implicit("ShowForPurchaseAttempt");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _dialogueBox = StringName.op_Implicit("_dialogueBox");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _bubble = StringName.op_Implicit("_bubble");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private static readonly StringName _h = new StringName("h");

	private static readonly Vector2 _xRange = new Vector2(450f, 1450f);

	private MegaRichTextLabel _label;

	private Node2D _dialogueBox;

	private Tween? _tween;

	private Sprite2D _bubble;

	private ShaderMaterial _hsv;

	private MerchantDialogueSet _dialogueSet;

	public override void _Ready()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		_label = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Text"));
		_dialogueBox = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%DialogueBox"));
		_bubble = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("%Bubble"));
		((CanvasItem)this).Modulate = Colors.Transparent;
		_hsv = (ShaderMaterial)((CanvasItem)_bubble).Material;
		_hsv.SetShaderParameter(_h, Variant.op_Implicit(1f));
		_hsv.SetShaderParameter(_s, Variant.op_Implicit(1.2f));
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(0.4f));
	}

	public void Initialize(MerchantDialogueSet dialogueSet)
	{
		_dialogueSet = dialogueSet;
	}

	public void ShowOnInventoryOpen()
	{
		ShowRandom(_dialogueSet.OpenInventoryLines);
	}

	public void ShowForPurchaseAttempt(PurchaseStatus status)
	{
		ShowRandom(_dialogueSet.GetPurchaseSuccessLines(status));
	}

	private void ShowRandom(IEnumerable<LocString> lines)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		LocString locString = Rng.Chaotic.NextItem(lines);
		if (locString != null)
		{
			_label.Text = "[fly_in]" + locString.GetFormattedText() + "[/fly_in]";
			((CanvasItem)this).Modulate = StsColors.transparentWhite;
			((Node2D)this).Position = new Vector2(Rng.Chaotic.NextFloat(_xRange.X, _xRange.Y), ((Node2D)this).Position.Y);
			Tween? tween = _tween;
			if (tween != null)
			{
				tween.Kill();
			}
			_tween = ((Node)this).CreateTween().SetParallel(true);
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25);
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10);
			_tween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("visible_ratio"), Variant.op_Implicit(1f), 0.4).From(Variant.op_Implicit(0f));
			_tween.TweenProperty((GodotObject)(object)_bubble, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(0.75f, 0.75f)), 0.5).From(Variant.op_Implicit(new Vector2(0.25f, 0.25f))).SetEase((EaseType)1)
				.SetTrans((TransitionType)5);
			_tween.TweenProperty((GodotObject)(object)_dialogueBox, NodePath.op_Implicit("position:y"), Variant.op_Implicit(0f), 0.5).From(Variant.op_Implicit(-80f)).SetEase((EaseType)1)
				.SetTrans((TransitionType)10);
			_tween.Chain();
			_tween.TweenInterval(1.0);
			_tween.Chain();
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)1);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowOnInventoryOpen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowForPurchaseAttempt, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("status"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowOnInventoryOpen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowOnInventoryOpen();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowForPurchaseAttempt && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ShowForPurchaseAttempt(VariantUtils.ConvertTo<PurchaseStatus>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.ShowOnInventoryOpen)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowForPurchaseAttempt)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dialogueBox)
		{
			_dialogueBox = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bubble)
		{
			_bubble = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
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
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._dialogueBox)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _dialogueBox);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._bubble)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _bubble);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dialogueBox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bubble, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._label, Variant.From<MegaRichTextLabel>(ref _label));
		info.AddProperty(PropertyName._dialogueBox, Variant.From<Node2D>(ref _dialogueBox));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._bubble, Variant.From<Sprite2D>(ref _bubble));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val))
		{
			_label = ((Variant)(ref val)).As<MegaRichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._dialogueBox, ref val2))
		{
			_dialogueBox = ((Variant)(ref val2)).As<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val3))
		{
			_tween = ((Variant)(ref val3)).As<Tween>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._bubble, ref val4))
		{
			_bubble = ((Variant)(ref val4)).As<Sprite2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val5))
		{
			_hsv = ((Variant)(ref val5)).As<ShaderMaterial>();
		}
	}
}
