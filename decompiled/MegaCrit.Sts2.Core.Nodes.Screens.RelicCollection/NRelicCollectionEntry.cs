using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Relics;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;

[ScriptPath("res://src/Core/Nodes/Screens/RelicCollection/NRelicCollectionEntry.cs")]
public class NRelicCollectionEntry : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName ModelVisibility = StringName.op_Implicit("ModelVisibility");

		public static readonly StringName _relicHolder = StringName.op_Implicit("_relicHolder");

		public static readonly StringName _relicNode = StringName.op_Implicit("_relicNode");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("screens/relic_collection/relic_collection_entry");

	public static readonly string lockedIconPath = ImageHelper.GetImagePath("packed/common_ui/locked_model.png");

	public RelicModel relic;

	private Control _relicHolder;

	private Control _relicNode;

	private Tween? _hoverTween;

	private static LocString UnknownHoverTipTitle => new LocString("main_menu_ui", "COMPENDIUM_RELIC_COLLECTION.unknown.title");

	private static LocString UnknownHoverTipDescription => new LocString("main_menu_ui", "COMPENDIUM_RELIC_COLLECTION.unknown.description");

	private static HoverTip UnknownHoverTip => new HoverTip(UnknownHoverTipTitle, UnknownHoverTipDescription);

	private static LocString LockedHoverTipTitle => new LocString("main_menu_ui", "COMPENDIUM_RELIC_COLLECTION.locked.title");

	private static LocString LockedHoverTipDescription => new LocString("main_menu_ui", "COMPENDIUM_RELIC_COLLECTION.locked.description");

	private static HoverTip LockedHoverTip => new HoverTip(LockedHoverTipTitle, LockedHoverTipDescription);

	public ModelVisibility ModelVisibility { get; set; }

	public static NRelicCollectionEntry Create(RelicModel relic, ModelVisibility visibility)
	{
		NRelicCollectionEntry nRelicCollectionEntry = PreloadManager.Cache.GetScene(scenePath).Instantiate<NRelicCollectionEntry>((GenEditState)0);
		nRelicCollectionEntry.relic = relic;
		nRelicCollectionEntry.ModelVisibility = visibility;
		return nRelicCollectionEntry;
	}

	public override void _Ready()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_relicHolder = ((Node)this).GetNode<Control>(NodePath.op_Implicit("RelicHolder"));
		if (ModelVisibility == ModelVisibility.Locked)
		{
			TextureRect val = new TextureRect();
			val.ExpandMode = (ExpandModeEnum)1;
			val.StretchMode = (StretchModeEnum)4;
			val.Texture = PreloadManager.Cache.GetTexture2D(lockedIconPath);
			((Control)val).Size = Vector2.One * 68f;
			((Control)val).PivotOffset = ((Control)val).Size * 0.5f;
			((CanvasItem)val).Modulate = StsColors.gray;
			((Node)(object)_relicHolder).AddChildSafely((Node?)(object)val);
			_relicNode = (Control)(object)val;
		}
		else
		{
			NRelic nRelic = NRelic.Create(relic.ToMutable(), NRelic.IconSize.Small);
			((Node)(object)_relicHolder).AddChildSafely((Node?)(object)nRelic);
			if (ModelVisibility == ModelVisibility.NotSeen)
			{
				((CanvasItem)nRelic.Icon).SelfModulate = StsColors.ninetyPercentBlack;
				((CanvasItem)nRelic.Outline).SelfModulate = StsColors.halfTransparentWhite;
			}
			else
			{
				foreach (RelicPoolModel allCharacterRelicPool in ModelDb.AllCharacterRelicPools)
				{
					if (allCharacterRelicPool.AllRelicIds.Contains(relic.Id))
					{
						TextureRect outline = nRelic.Outline;
						Color labOutlineColor = allCharacterRelicPool.LabOutlineColor;
						labOutlineColor.A = 0.66f;
						((CanvasItem)outline).SelfModulate = labOutlineColor;
						break;
					}
				}
			}
			_relicNode = (Control)(object)nRelic;
		}
		_relicNode.MouseFilter = (MouseFilterEnum)2;
		_relicNode.FocusMode = (FocusModeEnum)0;
	}

	protected override void OnRelease()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)_relicNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		NHoverTipSet.Remove((Control)(object)this);
	}

	protected override void OnFocus()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_relicNode.Scale = Vector2.One * 1.25f;
		ModelVisibility modelVisibility = ModelVisibility;
		IEnumerable<IHoverTip> enumerable = default(IEnumerable<IHoverTip>);
		switch (modelVisibility)
		{
		case ModelVisibility.None:
			throw new ArgumentOutOfRangeException();
		case ModelVisibility.Visible:
			enumerable = relic.HoverTips;
			break;
		case ModelVisibility.NotSeen:
			enumerable = new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(UnknownHoverTip);
			break;
		case ModelVisibility.Locked:
			enumerable = new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(LockedHoverTip);
			break;
		default:
			global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(modelVisibility);
			break;
		}
		IEnumerable<IHoverTip> hoverTips = enumerable;
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, hoverTips, HoverTip.GetHoverTipAlignment((Control)(object)this));
		nHoverTipSet.SetFollowOwner();
	}

	protected override void OnUnfocus()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)_relicNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		NHoverTipSet.Remove((Control)(object)this);
	}

	protected override void OnPress()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.OnPress();
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)_relicNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
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
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
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
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.ModelVisibility)
		{
			ModelVisibility = VariantUtils.ConvertTo<ModelVisibility>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicHolder)
		{
			_relicHolder = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicNode)
		{
			_relicNode = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ModelVisibility)
		{
			ModelVisibility modelVisibility = ModelVisibility;
			value = VariantUtils.CreateFrom<ModelVisibility>(ref modelVisibility);
			return true;
		}
		if ((ref name) == PropertyName._relicHolder)
		{
			value = VariantUtils.CreateFrom<Control>(ref _relicHolder);
			return true;
		}
		if ((ref name) == PropertyName._relicNode)
		{
			value = VariantUtils.CreateFrom<Control>(ref _relicNode);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName.ModelVisibility, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName modelVisibility = PropertyName.ModelVisibility;
		ModelVisibility modelVisibility2 = ModelVisibility;
		info.AddProperty(modelVisibility, Variant.From<ModelVisibility>(ref modelVisibility2));
		info.AddProperty(PropertyName._relicHolder, Variant.From<Control>(ref _relicHolder));
		info.AddProperty(PropertyName._relicNode, Variant.From<Control>(ref _relicNode));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.ModelVisibility, ref val))
		{
			ModelVisibility = ((Variant)(ref val)).As<ModelVisibility>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicHolder, ref val2))
		{
			_relicHolder = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicNode, ref val3))
		{
			_relicNode = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val4))
		{
			_hoverTween = ((Variant)(ref val4)).As<Tween>();
		}
	}
}
