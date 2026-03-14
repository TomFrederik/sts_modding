using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Ftue;

[ScriptPath("res://src/Core/Nodes/Ftue/NRestSiteFtue.cs")]
public class NRestSiteFtue : NFtue
{
	public new class MethodName : NFtue.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName CloseFtue = StringName.op_Implicit("CloseFtue");
	}

	public new class PropertyName : NFtue.PropertyName
	{
		public static readonly StringName _confirmButton = StringName.op_Implicit("_confirmButton");

		public static readonly StringName _header = StringName.op_Implicit("_header");

		public static readonly StringName _description = StringName.op_Implicit("_description");

		public static readonly StringName _arrow = StringName.op_Implicit("_arrow");

		public static readonly StringName _defaultZIndex = StringName.op_Implicit("_defaultZIndex");

		public static readonly StringName _restSiteOptionContainer = StringName.op_Implicit("_restSiteOptionContainer");
	}

	public new class SignalName : NFtue.SignalName
	{
	}

	public const string id = "rest_site_ftue";

	private static readonly string _scenePath = SceneHelper.GetScenePath("ftue/rest_site_ftue");

	private NButton _confirmButton;

	private MegaLabel _header;

	private MegaRichTextLabel _description;

	private Control _arrow;

	private int _defaultZIndex;

	private Control _restSiteOptionContainer;

	public override void _Ready()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		_header = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("FtuePopup/Header"));
		_header.SetTextAutoSize(new LocString("ftues", "REST_SITE_FTUE_TITLE").GetFormattedText());
		_description = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("FtuePopup/DescriptionContainer/Description"));
		_description.Text = new LocString("ftues", "REST_SITE_FTUE_DESCRIPTION").GetFormattedText();
		_confirmButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("FtuePopup/FtueConfirmButton"));
		((GodotObject)_confirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CloseFtue), 0u);
		_defaultZIndex = ((CanvasItem)_restSiteOptionContainer).ZIndex;
		Control restSiteOptionContainer = _restSiteOptionContainer;
		int zIndex = ((CanvasItem)restSiteOptionContainer).ZIndex;
		((CanvasItem)restSiteOptionContainer).ZIndex = zIndex + 1;
	}

	public static NRestSiteFtue? Create(Control restSiteOptionContainer)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NRestSiteFtue nRestSiteFtue = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NRestSiteFtue>((GenEditState)0);
		nRestSiteFtue._restSiteOptionContainer = restSiteOptionContainer;
		return nRestSiteFtue;
	}

	private void CloseFtue(NButton _)
	{
		((CanvasItem)_restSiteOptionContainer).ZIndex = _defaultZIndex;
		CloseFtue();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("restSiteOptionContainer"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CloseFtue, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NRestSiteFtue nRestSiteFtue = Create(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NRestSiteFtue>(ref nRestSiteFtue);
			return true;
		}
		if ((ref method) == MethodName.CloseFtue && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CloseFtue(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
			NRestSiteFtue nRestSiteFtue = Create(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NRestSiteFtue>(ref nRestSiteFtue);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName.CloseFtue)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._confirmButton)
		{
			_confirmButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._header)
		{
			_header = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			_description = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._arrow)
		{
			_arrow = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultZIndex)
		{
			_defaultZIndex = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._restSiteOptionContainer)
		{
			_restSiteOptionContainer = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName._confirmButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _confirmButton);
			return true;
		}
		if ((ref name) == PropertyName._header)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _header);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _description);
			return true;
		}
		if ((ref name) == PropertyName._arrow)
		{
			value = VariantUtils.CreateFrom<Control>(ref _arrow);
			return true;
		}
		if ((ref name) == PropertyName._defaultZIndex)
		{
			value = VariantUtils.CreateFrom<int>(ref _defaultZIndex);
			return true;
		}
		if ((ref name) == PropertyName._restSiteOptionContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _restSiteOptionContainer);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._confirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._header, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._description, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._arrow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._defaultZIndex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._restSiteOptionContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._confirmButton, Variant.From<NButton>(ref _confirmButton));
		info.AddProperty(PropertyName._header, Variant.From<MegaLabel>(ref _header));
		info.AddProperty(PropertyName._description, Variant.From<MegaRichTextLabel>(ref _description));
		info.AddProperty(PropertyName._arrow, Variant.From<Control>(ref _arrow));
		info.AddProperty(PropertyName._defaultZIndex, Variant.From<int>(ref _defaultZIndex));
		info.AddProperty(PropertyName._restSiteOptionContainer, Variant.From<Control>(ref _restSiteOptionContainer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._confirmButton, ref val))
		{
			_confirmButton = ((Variant)(ref val)).As<NButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._header, ref val2))
		{
			_header = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._description, ref val3))
		{
			_description = ((Variant)(ref val3)).As<MegaRichTextLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._arrow, ref val4))
		{
			_arrow = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultZIndex, ref val5))
		{
			_defaultZIndex = ((Variant)(ref val5)).As<int>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._restSiteOptionContainer, ref val6))
		{
			_restSiteOptionContainer = ((Variant)(ref val6)).As<Control>();
		}
	}
}
