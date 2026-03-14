using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Potions;

namespace MegaCrit.Sts2.Core.Nodes.Screens.PotionLab;

[ScriptPath("res://src/Core/Nodes/Screens/PotionLab/NLabPotionHolder.cs")]
public class NLabPotionHolder : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _potionNode = StringName.op_Implicit("_potionNode");

		public static readonly StringName _potionHolder = StringName.op_Implicit("_potionHolder");

		public static readonly StringName _visibility = StringName.op_Implicit("_visibility");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");
	}

	public class SignalName : SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("screens/potion_lab/lab_potion_holder");

	public static readonly string lockedIconPath = ImageHelper.GetImagePath("packed/common_ui/locked_model.png");

	private PotionModel _model;

	private NPotion _potionNode;

	private Control _potionHolder;

	private ModelVisibility _visibility;

	private Tween? _hoverTween;

	private LocString UnknownHoverTipTitle => new LocString("main_menu_ui", "POTION_LAB_COLLECTION.unknown.title");

	private LocString UnknownHoverTipDescription => new LocString("main_menu_ui", "POTION_LAB_COLLECTION.unknown.description");

	private HoverTip UnknownHoverTip => new HoverTip(UnknownHoverTipTitle, UnknownHoverTipDescription);

	private static LocString LockedHoverTipTitle => new LocString("main_menu_ui", "POTION_LAB_COLLECTION.locked.title");

	private static LocString LockedHoverTipDescription => new LocString("main_menu_ui", "POTION_LAB_COLLECTION.locked.description");

	private static HoverTip LockedHoverTip => new HoverTip(LockedHoverTipTitle, LockedHoverTipDescription);

	public static NLabPotionHolder Create(PotionModel potion, ModelVisibility visibility)
	{
		NLabPotionHolder nLabPotionHolder = PreloadManager.Cache.GetScene(scenePath).Instantiate<NLabPotionHolder>((GenEditState)0);
		nLabPotionHolder._model = potion;
		nLabPotionHolder._visibility = visibility;
		return nLabPotionHolder;
	}

	public override void _Ready()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		_potionHolder = ((Node)this).GetNode<Control>(NodePath.op_Implicit("PotionHolder"));
		_potionNode = NPotion.Create(_model);
		((Node)(object)_potionHolder).AddChildSafely((Node?)(object)_potionNode);
		if (_visibility == ModelVisibility.Locked)
		{
			_potionNode.Image.Texture = PreloadManager.Cache.GetTexture2D(lockedIconPath);
			((CanvasItem)_potionNode.Outline).Visible = false;
			((CanvasItem)_potionNode).Modulate = StsColors.gray;
		}
		else if (_visibility == ModelVisibility.NotSeen)
		{
			((CanvasItem)_potionNode.Image).SelfModulate = StsColors.ninetyPercentBlack;
			((CanvasItem)_potionNode.Outline).Modulate = StsColors.halfTransparentWhite;
		}
		else
		{
			foreach (PotionPoolModel allCharacterPotionPool in ModelDb.AllCharacterPotionPools)
			{
				PotionModel potionModel = allCharacterPotionPool.AllPotions.FirstOrDefault((PotionModel p) => p.Id == _model.Id);
				if (potionModel != null)
				{
					TextureRect outline = _potionNode.Outline;
					Color labOutlineColor = allCharacterPotionPool.LabOutlineColor;
					labOutlineColor.A = 0.66f;
					((CanvasItem)outline).Modulate = labOutlineColor;
					break;
				}
			}
		}
		((Control)_potionNode).MouseFilter = (MouseFilterEnum)2;
		((Control)_potionNode).PivotOffset = ((Control)_potionNode).Size * 0.5f;
		((Control)_potionNode).Position = Vector2.Zero;
		((GodotObject)this).Connect(SignalName.FocusEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)this).Connect(SignalName.FocusExited, Callable.From((Action)OnUnfocus), 0u);
		((GodotObject)this).Connect(SignalName.MouseEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)this).Connect(SignalName.MouseExited, Callable.From((Action)OnUnfocus), 0u);
	}

	private void OnFocus()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		NHoverTipSet.Remove((Control)(object)this);
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)_potionNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.2f), 0.05);
		ModelVisibility visibility = _visibility;
		IEnumerable<IHoverTip> enumerable = default(IEnumerable<IHoverTip>);
		switch (visibility)
		{
		case ModelVisibility.None:
			throw new ArgumentOutOfRangeException();
		case ModelVisibility.Visible:
			enumerable = _potionNode.Model.HoverTips;
			break;
		case ModelVisibility.NotSeen:
			enumerable = new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(UnknownHoverTip);
			break;
		case ModelVisibility.Locked:
			enumerable = new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(LockedHoverTip);
			break;
		default:
			global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(visibility);
			break;
		}
		IEnumerable<IHoverTip> hoverTips = enumerable;
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, hoverTips, HoverTip.GetHoverTipAlignment((Control)(object)this));
		nHoverTipSet.SetFollowOwner();
		nHoverTipSet.SetExtraFollowOffset(new Vector2(32f, 0f));
	}

	private void OnUnfocus()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)_potionNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		NHoverTipSet.Remove((Control)(object)this);
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
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._potionNode)
		{
			_potionNode = VariantUtils.ConvertTo<NPotion>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionHolder)
		{
			_potionHolder = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._visibility)
		{
			_visibility = VariantUtils.ConvertTo<ModelVisibility>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._potionNode)
		{
			value = VariantUtils.CreateFrom<NPotion>(ref _potionNode);
			return true;
		}
		if ((ref name) == PropertyName._potionHolder)
		{
			value = VariantUtils.CreateFrom<Control>(ref _potionHolder);
			return true;
		}
		if ((ref name) == PropertyName._visibility)
		{
			value = VariantUtils.CreateFrom<ModelVisibility>(ref _visibility);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._potionNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._visibility, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._potionNode, Variant.From<NPotion>(ref _potionNode));
		info.AddProperty(PropertyName._potionHolder, Variant.From<Control>(ref _potionHolder));
		info.AddProperty(PropertyName._visibility, Variant.From<ModelVisibility>(ref _visibility));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._potionNode, ref val))
		{
			_potionNode = ((Variant)(ref val)).As<NPotion>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionHolder, ref val2))
		{
			_potionHolder = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._visibility, ref val3))
		{
			_visibility = ((Variant)(ref val3)).As<ModelVisibility>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val4))
		{
			_hoverTween = ((Variant)(ref val4)).As<Tween>();
		}
	}
}
