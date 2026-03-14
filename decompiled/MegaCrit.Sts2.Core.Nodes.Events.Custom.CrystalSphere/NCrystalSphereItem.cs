using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Events.Custom.CrystalSphereEvent;
using MegaCrit.Sts2.Core.Events.Custom.CrystalSphereEvent.CrystalSphereItems;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Events.Custom.CrystalSphere;

[ScriptPath("res://src/Core/Nodes/Events/Custom/CrystalSphere/NCrystalSphereItem.cs")]
public class NCrystalSphereItem : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _material = StringName.op_Implicit("_material");

		public static readonly StringName _card = StringName.op_Implicit("_card");

		public static readonly StringName _cardFrame = StringName.op_Implicit("_cardFrame");

		public static readonly StringName _cardBanner = StringName.op_Implicit("_cardBanner");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	public const string scenePath = "res://scenes/events/custom/crystal_sphere/crystal_sphere_item.tscn";

	private CrystalSphereItem _item;

	private TextureRect _icon;

	private Material _material;

	private Control _card;

	private TextureRect _cardFrame;

	private TextureRect _cardBanner;

	private Tween? _tween;

	public static NCrystalSphereItem? Create(CrystalSphereItem item)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NCrystalSphereItem nCrystalSphereItem = PreloadManager.Cache.GetScene("res://scenes/events/custom/crystal_sphere/crystal_sphere_item.tscn").Instantiate<NCrystalSphereItem>((GenEditState)0);
		nCrystalSphereItem._item = item;
		return nCrystalSphereItem;
	}

	public override void _Ready()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Icon"));
		_card = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Card"));
		_cardFrame = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%CardFrame"));
		_cardBanner = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%CardBanner"));
		((Control)this).PivotOffset = ((Control)this).Size / 2f;
		((Control)_icon).PivotOffset = ((Control)this).Size / 2f;
		_card.PivotOffset = ((Control)this).Size / 2f;
		_material = ((CanvasItem)_icon).Material;
		if (_item is CrystalSphereCardReward crystalSphereCardReward)
		{
			((CanvasItem)_card).Visible = true;
			((CanvasItem)_icon).Visible = false;
			((CanvasItem)_cardBanner).Material = crystalSphereCardReward.BannerMaterial;
			((CanvasItem)_cardFrame).Material = crystalSphereCardReward.FrameMaterial;
		}
		else
		{
			((CanvasItem)_card).Visible = false;
			((CanvasItem)_icon).Visible = true;
			_icon.Texture = _item.Texture;
		}
	}

	public override void _EnterTree()
	{
		_item.Revealed += OnRevealed;
	}

	public override void _ExitTree()
	{
		_item.Revealed -= OnRevealed;
		if (_tween != null && _tween.IsRunning())
		{
			_tween.Kill();
		}
	}

	private void OnRevealed(CrystalSphereItem item)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenInterval(0.25);
		_tween.Chain().TweenProperty((GodotObject)(object)_material, NodePath.op_Implicit("shader_parameter/val"), Variant.op_Implicit(1f), 0.5).From(Variant.op_Implicit(0));
		_tween.Parallel().TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.2f), 0.15000000596046448);
		_tween.Parallel().TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetDelay(0.15000000596046448);
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
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
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
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._material)
		{
			_material = VariantUtils.ConvertTo<Material>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._card)
		{
			_card = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardFrame)
		{
			_cardFrame = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardBanner)
		{
			_cardBanner = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._material)
		{
			value = VariantUtils.CreateFrom<Material>(ref _material);
			return true;
		}
		if ((ref name) == PropertyName._card)
		{
			value = VariantUtils.CreateFrom<Control>(ref _card);
			return true;
		}
		if ((ref name) == PropertyName._cardFrame)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _cardFrame);
			return true;
		}
		if ((ref name) == PropertyName._cardBanner)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _cardBanner);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._material, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._card, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardFrame, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardBanner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._material, Variant.From<Material>(ref _material));
		info.AddProperty(PropertyName._card, Variant.From<Control>(ref _card));
		info.AddProperty(PropertyName._cardFrame, Variant.From<TextureRect>(ref _cardFrame));
		info.AddProperty(PropertyName._cardBanner, Variant.From<TextureRect>(ref _cardBanner));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val))
		{
			_icon = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._material, ref val2))
		{
			_material = ((Variant)(ref val2)).As<Material>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._card, ref val3))
		{
			_card = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardFrame, ref val4))
		{
			_cardFrame = ((Variant)(ref val4)).As<TextureRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardBanner, ref val5))
		{
			_cardBanner = ((Variant)(ref val5)).As<TextureRect>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val6))
		{
			_tween = ((Variant)(ref val6)).As<Tween>();
		}
	}
}
