using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.StatsScreen;

[ScriptPath("res://src/Core/Nodes/Screens/StatsScreen/NStatEntry.cs")]
public class NStatEntry : NClickableControl
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName SetTopText = StringName.op_Implicit("SetTopText");

		public static readonly StringName SetBottomText = StringName.op_Implicit("SetBottomText");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _imgUrl = StringName.op_Implicit("_imgUrl");

		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _topLabel = StringName.op_Implicit("_topLabel");

		public static readonly StringName _bottomLabel = StringName.op_Implicit("_bottomLabel");

		public static readonly StringName _controllerFocusReticle = StringName.op_Implicit("_controllerFocusReticle");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private HoverTip? _hoverTip;

	private Tween? _tween;

	private string? _imgUrl;

	private TextureRect _icon;

	private MegaRichTextLabel _topLabel;

	private MegaRichTextLabel _bottomLabel;

	private NSelectionReticle _controllerFocusReticle;

	private static string ScenePath => SceneHelper.GetScenePath("screens/stats_screen/stats_screen_section");

	public static NStatEntry Create(string imgUrl)
	{
		NStatEntry nStatEntry = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NStatEntry>((GenEditState)0);
		nStatEntry._imgUrl = imgUrl;
		return nStatEntry;
	}

	public void SetTopText(string text)
	{
		((CanvasItem)_topLabel).Visible = true;
		_topLabel.SetTextAutoSize(text);
	}

	public void SetBottomText(string text)
	{
		((CanvasItem)_bottomLabel).Visible = true;
		_bottomLabel.SetTextAutoSize(text);
	}

	public override void _Ready()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).SetPivotOffset(new Vector2(50f, ((Control)this).Size.Y * 0.5f));
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		if (_imgUrl != null)
		{
			_icon.Texture = PreloadManager.Cache.GetTexture2D(_imgUrl);
		}
		_topLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%TopLabel"));
		_bottomLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%BottomLabel"));
		_controllerFocusReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("%SelectionReticle"));
		ConnectSignals();
	}

	public void SetHoverTip(HoverTip hoverTip)
	{
		_hoverTip = hoverTip;
	}

	protected override void OnFocus()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.05f), 0.05);
		if (_hoverTip.HasValue)
		{
			float x = ((Control)this).GlobalPosition.X;
			Rect2 visibleRect = ((Node)this).GetViewport().GetVisibleRect();
			if (x < ((Rect2)(ref visibleRect)).Size.X * 0.4f)
			{
				NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _hoverTip);
				((Control)nHoverTipSet).GlobalPosition = new Vector2(((Control)this).GlobalPosition.X - 392f, ((Control)this).GlobalPosition.Y);
			}
			else
			{
				NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _hoverTip);
				((Control)nHoverTipSet).GlobalPosition = new Vector2(((Control)this).GlobalPosition.X + 532f, ((Control)this).GlobalPosition.Y);
			}
		}
		if (NControllerManager.Instance.IsUsingController)
		{
			_controllerFocusReticle.OnSelect();
		}
	}

	protected override void OnUnfocus()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		if (_hoverTip.HasValue)
		{
			NHoverTipSet.Remove((Control)(object)this);
		}
		_controllerFocusReticle.OnDeselect();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("imgUrl"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTopText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetBottomText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NStatEntry nStatEntry = Create(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NStatEntry>(ref nStatEntry);
			return true;
		}
		if ((ref method) == MethodName.SetTopText && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTopText(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetBottomText && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetBottomText(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
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
			NStatEntry nStatEntry = Create(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NStatEntry>(ref nStatEntry);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTopText)
		{
			return true;
		}
		if ((ref method) == MethodName.SetBottomText)
		{
			return true;
		}
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
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._imgUrl)
		{
			_imgUrl = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._topLabel)
		{
			_topLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bottomLabel)
		{
			_bottomLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._controllerFocusReticle)
		{
			_controllerFocusReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
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
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._imgUrl)
		{
			value = VariantUtils.CreateFrom<string>(ref _imgUrl);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._topLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _topLabel);
			return true;
		}
		if ((ref name) == PropertyName._bottomLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _bottomLabel);
			return true;
		}
		if ((ref name) == PropertyName._controllerFocusReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _controllerFocusReticle);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._imgUrl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._topLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bottomLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._controllerFocusReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._imgUrl, Variant.From<string>(ref _imgUrl));
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._topLabel, Variant.From<MegaRichTextLabel>(ref _topLabel));
		info.AddProperty(PropertyName._bottomLabel, Variant.From<MegaRichTextLabel>(ref _bottomLabel));
		info.AddProperty(PropertyName._controllerFocusReticle, Variant.From<NSelectionReticle>(ref _controllerFocusReticle));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val))
		{
			_tween = ((Variant)(ref val)).As<Tween>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._imgUrl, ref val2))
		{
			_imgUrl = ((Variant)(ref val2)).As<string>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val3))
		{
			_icon = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._topLabel, ref val4))
		{
			_topLabel = ((Variant)(ref val4)).As<MegaRichTextLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._bottomLabel, ref val5))
		{
			_bottomLabel = ((Variant)(ref val5)).As<MegaRichTextLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._controllerFocusReticle, ref val6))
		{
			_controllerFocusReticle = ((Variant)(ref val6)).As<NSelectionReticle>();
		}
	}
}
