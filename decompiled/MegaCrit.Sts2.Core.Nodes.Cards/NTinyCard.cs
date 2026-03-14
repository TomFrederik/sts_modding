using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.Cards;

[ScriptPath("res://src/Core/Nodes/Cards/NTinyCard.cs")]
public class NTinyCard : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetCardPortraitShape = StringName.op_Implicit("SetCardPortraitShape");

		public static readonly StringName SetBannerColor = StringName.op_Implicit("SetBannerColor");

		public static readonly StringName GetBannerColor = StringName.op_Implicit("GetBannerColor");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName _cardBack = StringName.op_Implicit("_cardBack");

		public static readonly StringName _cardPortrait = StringName.op_Implicit("_cardPortrait");

		public static readonly StringName _cardPortraitShadow = StringName.op_Implicit("_cardPortraitShadow");

		public static readonly StringName _cardBanner = StringName.op_Implicit("_cardBanner");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private TextureRect _cardBack;

	private TextureRect _cardPortrait;

	private TextureRect _cardPortraitShadow;

	private Control _cardBanner;

	public override void _Ready()
	{
		ConnectSignals();
		_cardBack = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%CardBack"));
		_cardPortrait = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Portrait"));
		_cardPortraitShadow = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%PortraitShadow"));
		_cardBanner = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Banner"));
	}

	public void SetCard(CardModel card)
	{
		SetCardBackColor(card.Pool);
		SetCardPortraitShape(card.Type);
		SetBannerColor(card.Rarity);
		((CanvasItem)_cardBack).Material = card.FrameMaterial;
	}

	public void Set(CardPoolModel cardPool, CardType type, CardRarity rarity)
	{
		SetCardBackColor(cardPool);
		SetCardPortraitShape(type);
		SetBannerColor(rarity);
		((CanvasItem)_cardBack).Material = cardPool.AllCards.First().FrameMaterial;
	}

	private void SetCardBackColor(CardPoolModel cardPool)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_cardBack).Modulate = cardPool.DeckEntryCardColor;
	}

	private void SetCardPortraitShape(CardType type)
	{
		switch (type)
		{
		case CardType.Attack:
			_cardPortrait.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D("res://images/packed/run_history/attack_portrait.png");
			_cardPortraitShadow.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D("res://images/packed/run_history/attack_portrait_shadow.png");
			break;
		case CardType.Power:
			_cardPortrait.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D("res://images/packed/run_history/power_portrait.png");
			_cardPortraitShadow.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D("res://images/packed/run_history/power_portrait_shadow.png");
			break;
		default:
			_cardPortrait.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D("res://images/packed/run_history/skill_portrait.png");
			_cardPortraitShadow.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D("res://images/packed/run_history/skill_portrait_shadow.png");
			break;
		}
	}

	private void SetBannerColor(CardRarity rarity)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_cardBanner).Modulate = GetBannerColor(rarity);
	}

	private Color GetBannerColor(CardRarity rarity)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		switch (rarity)
		{
		case CardRarity.Basic:
		case CardRarity.Common:
			return new Color("9C9C9CFF");
		case CardRarity.Uncommon:
			return new Color("64FFFFFF");
		case CardRarity.Rare:
			return new Color("FFDA36FF");
		case CardRarity.Curse:
			return new Color("E669FFFF");
		case CardRarity.Event:
			return new Color("13BE1AFF");
		case CardRarity.Quest:
			return new Color("F46836FF");
		default:
			Log.Warn($"Unspecified Rarity: {rarity}");
			return Colors.White;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCardPortraitShape, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("type"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetBannerColor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("rarity"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetBannerColor, new PropertyInfo((Type)20, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("rarity"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCardPortraitShape && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCardPortraitShape(VariantUtils.ConvertTo<CardType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetBannerColor && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetBannerColor(VariantUtils.ConvertTo<CardRarity>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetBannerColor && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Color bannerColor = GetBannerColor(VariantUtils.ConvertTo<CardRarity>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Color>(ref bannerColor);
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
		if ((ref method) == MethodName.SetCardPortraitShape)
		{
			return true;
		}
		if ((ref method) == MethodName.SetBannerColor)
		{
			return true;
		}
		if ((ref method) == MethodName.GetBannerColor)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._cardBack)
		{
			_cardBack = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardPortrait)
		{
			_cardPortrait = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardPortraitShadow)
		{
			_cardPortraitShadow = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardBanner)
		{
			_cardBanner = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		if ((ref name) == PropertyName._cardBack)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _cardBack);
			return true;
		}
		if ((ref name) == PropertyName._cardPortrait)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _cardPortrait);
			return true;
		}
		if ((ref name) == PropertyName._cardPortraitShadow)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _cardPortraitShadow);
			return true;
		}
		if ((ref name) == PropertyName._cardBanner)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cardBanner);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._cardBack, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardPortrait, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardPortraitShadow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardBanner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._cardBack, Variant.From<TextureRect>(ref _cardBack));
		info.AddProperty(PropertyName._cardPortrait, Variant.From<TextureRect>(ref _cardPortrait));
		info.AddProperty(PropertyName._cardPortraitShadow, Variant.From<TextureRect>(ref _cardPortraitShadow));
		info.AddProperty(PropertyName._cardBanner, Variant.From<Control>(ref _cardBanner));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._cardBack, ref val))
		{
			_cardBack = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardPortrait, ref val2))
		{
			_cardPortrait = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardPortraitShadow, ref val3))
		{
			_cardPortraitShadow = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardBanner, ref val4))
		{
			_cardBanner = ((Variant)(ref val4)).As<Control>();
		}
	}
}
