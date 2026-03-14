using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NContinueRunInfo.cs")]
public class NContinueRunInfo : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName AnimShow = StringName.op_Implicit("AnimShow");

		public static readonly StringName AnimHide = StringName.op_Implicit("AnimHide");

		public static readonly StringName ShowError = StringName.op_Implicit("ShowError");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName HasResult = StringName.op_Implicit("HasResult");

		public static readonly StringName _visTween = StringName.op_Implicit("_visTween");

		public static readonly StringName _initPosition = StringName.op_Implicit("_initPosition");

		public static readonly StringName _runInfoContainer = StringName.op_Implicit("_runInfoContainer");

		public static readonly StringName _errorContainer = StringName.op_Implicit("_errorContainer");

		public static readonly StringName _dateLabel = StringName.op_Implicit("_dateLabel");

		public static readonly StringName _goldLabel = StringName.op_Implicit("_goldLabel");

		public static readonly StringName _healthLabel = StringName.op_Implicit("_healthLabel");

		public static readonly StringName _progressLabel = StringName.op_Implicit("_progressLabel");

		public static readonly StringName _ascensionLabel = StringName.op_Implicit("_ascensionLabel");

		public static readonly StringName _charIcon = StringName.op_Implicit("_charIcon");

		public static readonly StringName _isShown = StringName.op_Implicit("_isShown");
	}

	public class SignalName : SignalName
	{
	}

	private Tween? _visTween;

	private Vector2 _initPosition;

	private Control _runInfoContainer;

	private Control _errorContainer;

	private MegaRichTextLabel _dateLabel;

	private MegaRichTextLabel _goldLabel;

	private MegaRichTextLabel _healthLabel;

	private MegaRichTextLabel _progressLabel;

	private MegaRichTextLabel _ascensionLabel;

	private TextureRect _charIcon;

	private bool _isShown;

	public bool HasResult { get; private set; }

	public override void _Ready()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_initPosition = ((Control)this).Position;
		((CanvasItem)this).Modulate = StsColors.transparentWhite;
		_runInfoContainer = (Control)(object)((Node)this).GetNode<VBoxContainer>(NodePath.op_Implicit("%RunInfoContainer"));
		_errorContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ErrorContainer"));
		_dateLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%DateLabel"));
		_goldLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%GoldLabel"));
		_healthLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%HealthLabel"));
		_progressLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%ProgressLabel"));
		_charIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%CharacterIcon"));
		_ascensionLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%AscensionLabel"));
	}

	public void AnimShow()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Tween? visTween = _visTween;
		if (visTween != null)
		{
			visTween.Kill();
		}
		_visTween = ((Node)this).CreateTween().SetParallel(true);
		_visTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_initPosition + new Vector2(0f, -20f)), 0.20000000298023224).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_visTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.20000000298023224);
		_isShown = true;
	}

	public void AnimHide()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (_isShown)
		{
			Tween? visTween = _visTween;
			if (visTween != null)
			{
				visTween.Kill();
			}
			_visTween = ((Node)this).CreateTween().SetParallel(true);
			_visTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_initPosition), 0.20000000298023224);
			_visTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.20000000298023224);
			_isShown = false;
		}
	}

	public void SetResult(ReadSaveResult<SerializableRun>? result)
	{
		if (result != null && result.Success && result.SaveData != null)
		{
			ShowInfo(result.SaveData);
		}
		else if (result != null)
		{
			ShowError();
		}
		HasResult = result != null;
	}

	private void ShowInfo(SerializableRun save)
	{
		((CanvasItem)_errorContainer).Visible = false;
		((CanvasItem)_runInfoContainer).Visible = true;
		DateTimeFormatInfo dateTimeFormat = LocManager.Instance.CultureInfo.DateTimeFormat;
		DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTimeOffset.FromUnixTimeSeconds(save.SaveTime).UtcDateTime, TimeZoneInfo.Local);
		string rawText = new LocString("main_menu_ui", "CONTINUE_RUN_INFO.savedTimeFormat").GetRawText();
		string variable = dateTime.ToString(rawText, dateTimeFormat);
		LocString locString = new LocString("main_menu_ui", "CONTINUE_RUN_INFO.saved");
		locString.Add("LastSavedTime", variable);
		_dateLabel.Text = locString.GetFormattedText();
		if (save.Ascension > 0)
		{
			LocString locString2 = new LocString("main_menu_ui", "CONTINUE_RUN_INFO.ascension");
			_ascensionLabel.Text = $"{locString2.GetFormattedText()} {save.Ascension}";
		}
		else
		{
			((CanvasItem)_ascensionLabel).Visible = false;
		}
		ActModel byId = ModelDb.GetById<ActModel>(save.Acts[save.CurrentActIndex].Id);
		SerializablePlayer serializablePlayer = save.Players[0];
		_charIcon.Texture = ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId).IconTexture;
		string formattedText = byId.Title.GetFormattedText();
		string formattedText2 = new LocString("main_menu_ui", "CONTINUE_RUN_INFO.floor").GetFormattedText();
		int num = save.VisitedMapCoords.Count;
		for (int i = 0; i < save.CurrentActIndex; i++)
		{
			num += ModelDb.GetById<ActModel>(save.Acts[i].Id).GetNumberOfFloors(save.Players.Count > 1);
		}
		_progressLabel.Text = $"{formattedText} [blue]- {formattedText2} {num}[/blue]";
		_healthLabel.Text = $"[red]{serializablePlayer.CurrentHp}/{serializablePlayer.MaxHp}[/red]";
		_goldLabel.Text = $"[gold]{serializablePlayer.Gold}[/gold]";
	}

	private void ShowError()
	{
		((CanvasItem)_runInfoContainer).Visible = false;
		((CanvasItem)_errorContainer).Visible = true;
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
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimShow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimHide, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowError, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimShow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimShow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimHide && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimHide();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowError && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowError();
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
		if ((ref method) == MethodName.AnimShow)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimHide)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowError)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.HasResult)
		{
			HasResult = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._visTween)
		{
			_visTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._initPosition)
		{
			_initPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._runInfoContainer)
		{
			_runInfoContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._errorContainer)
		{
			_errorContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dateLabel)
		{
			_dateLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._goldLabel)
		{
			_goldLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._healthLabel)
		{
			_healthLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._progressLabel)
		{
			_progressLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ascensionLabel)
		{
			_ascensionLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._charIcon)
		{
			_charIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isShown)
		{
			_isShown = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
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
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.HasResult)
		{
			bool hasResult = HasResult;
			value = VariantUtils.CreateFrom<bool>(ref hasResult);
			return true;
		}
		if ((ref name) == PropertyName._visTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _visTween);
			return true;
		}
		if ((ref name) == PropertyName._initPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _initPosition);
			return true;
		}
		if ((ref name) == PropertyName._runInfoContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _runInfoContainer);
			return true;
		}
		if ((ref name) == PropertyName._errorContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _errorContainer);
			return true;
		}
		if ((ref name) == PropertyName._dateLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _dateLabel);
			return true;
		}
		if ((ref name) == PropertyName._goldLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _goldLabel);
			return true;
		}
		if ((ref name) == PropertyName._healthLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _healthLabel);
			return true;
		}
		if ((ref name) == PropertyName._progressLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _progressLabel);
			return true;
		}
		if ((ref name) == PropertyName._ascensionLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _ascensionLabel);
			return true;
		}
		if ((ref name) == PropertyName._charIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _charIcon);
			return true;
		}
		if ((ref name) == PropertyName._isShown)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isShown);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._visTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._initPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._runInfoContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._errorContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dateLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._goldLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._healthLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._progressLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._charIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isShown, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.HasResult, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName hasResult = PropertyName.HasResult;
		bool hasResult2 = HasResult;
		info.AddProperty(hasResult, Variant.From<bool>(ref hasResult2));
		info.AddProperty(PropertyName._visTween, Variant.From<Tween>(ref _visTween));
		info.AddProperty(PropertyName._initPosition, Variant.From<Vector2>(ref _initPosition));
		info.AddProperty(PropertyName._runInfoContainer, Variant.From<Control>(ref _runInfoContainer));
		info.AddProperty(PropertyName._errorContainer, Variant.From<Control>(ref _errorContainer));
		info.AddProperty(PropertyName._dateLabel, Variant.From<MegaRichTextLabel>(ref _dateLabel));
		info.AddProperty(PropertyName._goldLabel, Variant.From<MegaRichTextLabel>(ref _goldLabel));
		info.AddProperty(PropertyName._healthLabel, Variant.From<MegaRichTextLabel>(ref _healthLabel));
		info.AddProperty(PropertyName._progressLabel, Variant.From<MegaRichTextLabel>(ref _progressLabel));
		info.AddProperty(PropertyName._ascensionLabel, Variant.From<MegaRichTextLabel>(ref _ascensionLabel));
		info.AddProperty(PropertyName._charIcon, Variant.From<TextureRect>(ref _charIcon));
		info.AddProperty(PropertyName._isShown, Variant.From<bool>(ref _isShown));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.HasResult, ref val))
		{
			HasResult = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._visTween, ref val2))
		{
			_visTween = ((Variant)(ref val2)).As<Tween>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._initPosition, ref val3))
		{
			_initPosition = ((Variant)(ref val3)).As<Vector2>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._runInfoContainer, ref val4))
		{
			_runInfoContainer = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._errorContainer, ref val5))
		{
			_errorContainer = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._dateLabel, ref val6))
		{
			_dateLabel = ((Variant)(ref val6)).As<MegaRichTextLabel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._goldLabel, ref val7))
		{
			_goldLabel = ((Variant)(ref val7)).As<MegaRichTextLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._healthLabel, ref val8))
		{
			_healthLabel = ((Variant)(ref val8)).As<MegaRichTextLabel>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._progressLabel, ref val9))
		{
			_progressLabel = ((Variant)(ref val9)).As<MegaRichTextLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionLabel, ref val10))
		{
			_ascensionLabel = ((Variant)(ref val10)).As<MegaRichTextLabel>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._charIcon, ref val11))
		{
			_charIcon = ((Variant)(ref val11)).As<TextureRect>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._isShown, ref val12))
		{
			_isShown = ((Variant)(ref val12)).As<bool>();
		}
	}
}
