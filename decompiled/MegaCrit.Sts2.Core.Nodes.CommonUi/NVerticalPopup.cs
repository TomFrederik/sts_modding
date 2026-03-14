using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NVerticalPopup.cs")]
public class NVerticalPopup : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetText = StringName.op_Implicit("SetText");

		public static readonly StringName Close = StringName.op_Implicit("Close");

		public static readonly StringName HideNoButton = StringName.op_Implicit("HideNoButton");

		public static readonly StringName DisconnectSignals = StringName.op_Implicit("DisconnectSignals");

		public static readonly StringName DisconnectHotkeys = StringName.op_Implicit("DisconnectHotkeys");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName TitleLabel = StringName.op_Implicit("TitleLabel");

		public static readonly StringName BodyLabel = StringName.op_Implicit("BodyLabel");

		public static readonly StringName YesButton = StringName.op_Implicit("YesButton");

		public static readonly StringName NoButton = StringName.op_Implicit("NoButton");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("ui/vertical_popup");

	private Callable? _yesCallable;

	private Callable? _noCallable;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public MegaLabel TitleLabel { get; private set; }

	public MegaRichTextLabel BodyLabel { get; private set; }

	public NPopupYesNoButton YesButton { get; private set; }

	public NPopupYesNoButton NoButton { get; private set; }

	public override void _Ready()
	{
		TitleLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("Header"));
		BodyLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("Description"));
		YesButton = ((Node)this).GetNode<NPopupYesNoButton>(NodePath.op_Implicit("YesButton"));
		NoButton = ((Node)this).GetNode<NPopupYesNoButton>(NodePath.op_Implicit("NoButton"));
	}

	public void SetText(LocString title, LocString body)
	{
		TitleLabel.SetTextAutoSize(title.GetFormattedText());
		BodyLabel.Text = "[center]" + body.GetFormattedText() + "[/center]";
	}

	public void SetText(string title, string body)
	{
		TitleLabel.SetTextAutoSize(title);
		BodyLabel.Text = "[center]" + body + "[/center]";
	}

	public void InitYesButton(LocString yesButton, Action<NButton> onPressed)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		_yesCallable = Callable.From<NButton>(onPressed);
		YesButton.IsYes = true;
		YesButton.SetText(yesButton.GetFormattedText());
		((GodotObject)YesButton).Connect(NClickableControl.SignalName.Released, _yesCallable.Value, 0u);
		((GodotObject)YesButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)Close), 0u);
	}

	public void InitNoButton(LocString noButton, Action<NButton> onPressed)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		_noCallable = Callable.From<NButton>(onPressed);
		((CanvasItem)NoButton).Visible = true;
		NoButton.IsYes = false;
		NoButton.SetText(noButton.GetFormattedText());
		((GodotObject)NoButton).Connect(NClickableControl.SignalName.Released, _noCallable.Value, 0u);
		((GodotObject)NoButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)Close), 0u);
	}

	private void Close(NButton _)
	{
		NModalContainer.Instance.Clear();
	}

	public void HideNoButton()
	{
		((CanvasItem)NoButton).Visible = false;
	}

	public void DisconnectSignals()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (_yesCallable.HasValue)
		{
			((GodotObject)YesButton).Disconnect(NClickableControl.SignalName.Released, _yesCallable.Value);
			((GodotObject)YesButton).Disconnect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)Close));
		}
		if (_noCallable.HasValue)
		{
			((GodotObject)NoButton).Disconnect(NClickableControl.SignalName.Released, _noCallable.Value);
			((GodotObject)NoButton).Disconnect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)Close));
		}
	}

	public void DisconnectHotkeys()
	{
		if (_yesCallable.HasValue)
		{
			YesButton.DisconnectHotkeys();
		}
		if (_noCallable.HasValue)
		{
			NoButton.DisconnectHotkeys();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("title"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("body"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Close, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideNoButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisconnectSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisconnectHotkeys, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetText && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			SetText(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Close && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Close(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideNoButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideNoButton();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisconnectSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisconnectSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisconnectHotkeys && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisconnectHotkeys();
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
		if ((ref method) == MethodName.SetText)
		{
			return true;
		}
		if ((ref method) == MethodName.Close)
		{
			return true;
		}
		if ((ref method) == MethodName.HideNoButton)
		{
			return true;
		}
		if ((ref method) == MethodName.DisconnectSignals)
		{
			return true;
		}
		if ((ref method) == MethodName.DisconnectHotkeys)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.TitleLabel)
		{
			TitleLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.BodyLabel)
		{
			BodyLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.YesButton)
		{
			YesButton = VariantUtils.ConvertTo<NPopupYesNoButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.NoButton)
		{
			NoButton = VariantUtils.ConvertTo<NPopupYesNoButton>(ref value);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.TitleLabel)
		{
			MegaLabel titleLabel = TitleLabel;
			value = VariantUtils.CreateFrom<MegaLabel>(ref titleLabel);
			return true;
		}
		if ((ref name) == PropertyName.BodyLabel)
		{
			MegaRichTextLabel bodyLabel = BodyLabel;
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref bodyLabel);
			return true;
		}
		if ((ref name) == PropertyName.YesButton)
		{
			NPopupYesNoButton yesButton = YesButton;
			value = VariantUtils.CreateFrom<NPopupYesNoButton>(ref yesButton);
			return true;
		}
		if ((ref name) == PropertyName.NoButton)
		{
			NPopupYesNoButton yesButton = NoButton;
			value = VariantUtils.CreateFrom<NPopupYesNoButton>(ref yesButton);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.TitleLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.BodyLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.YesButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.NoButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName titleLabel = PropertyName.TitleLabel;
		MegaLabel titleLabel2 = TitleLabel;
		info.AddProperty(titleLabel, Variant.From<MegaLabel>(ref titleLabel2));
		StringName bodyLabel = PropertyName.BodyLabel;
		MegaRichTextLabel bodyLabel2 = BodyLabel;
		info.AddProperty(bodyLabel, Variant.From<MegaRichTextLabel>(ref bodyLabel2));
		StringName yesButton = PropertyName.YesButton;
		NPopupYesNoButton yesButton2 = YesButton;
		info.AddProperty(yesButton, Variant.From<NPopupYesNoButton>(ref yesButton2));
		StringName noButton = PropertyName.NoButton;
		yesButton2 = NoButton;
		info.AddProperty(noButton, Variant.From<NPopupYesNoButton>(ref yesButton2));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.TitleLabel, ref val))
		{
			TitleLabel = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.BodyLabel, ref val2))
		{
			BodyLabel = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.YesButton, ref val3))
		{
			YesButton = ((Variant)(ref val3)).As<NPopupYesNoButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.NoButton, ref val4))
		{
			NoButton = ((Variant)(ref val4)).As<NPopupYesNoButton>();
		}
	}
}
