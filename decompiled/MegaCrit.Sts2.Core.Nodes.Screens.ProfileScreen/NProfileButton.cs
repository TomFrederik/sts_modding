using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.ProfileScreen;

[ScriptPath("res://src/Core/Nodes/Screens/ProfileScreen/NProfileButton.cs")]
public class NProfileButton : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Initialize = StringName.op_Implicit("Initialize");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName UpdateShaderV = StringName.op_Implicit("UpdateShaderV");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName _title = StringName.op_Implicit("_title");

		public static readonly StringName _description = StringName.op_Implicit("_description");

		public static readonly StringName _currentProfileIndicator = StringName.op_Implicit("_currentProfileIndicator");

		public static readonly StringName _profileIcon = StringName.op_Implicit("_profileIcon");

		public static readonly StringName _deleteButton = StringName.op_Implicit("_deleteButton");

		public static readonly StringName _background = StringName.op_Implicit("_background");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _profileScreen = StringName.op_Implicit("_profileScreen");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _profileId = StringName.op_Implicit("_profileId");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private MegaRichTextLabel _title;

	private MegaRichTextLabel _description;

	private Control _currentProfileIndicator;

	private NProfileIcon _profileIcon;

	private NDeleteProfileButton _deleteButton;

	private TextureRect _background;

	private ShaderMaterial _hsv;

	private NProfileScreen? _profileScreen;

	private Tween? _tween;

	private int _profileId;

	public static IEnumerable<string> AssetPaths => NProfileIcon.AssetPaths;

	public override void _Ready()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		ConnectSignals();
		_background = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Background"));
		_hsv = (ShaderMaterial)((CanvasItem)_background).Material;
		_title = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Title"));
		_description = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Info"));
		_profileIcon = ((Node)this).GetNode<NProfileIcon>(NodePath.op_Implicit("%ProfileIcon"));
		_currentProfileIndicator = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CurrentProfileIndicator"));
	}

	public void Initialize(NProfileScreen profileScreen, int profileId)
	{
		_profileScreen = profileScreen;
		_profileId = profileId;
		LocString locString = new LocString("main_menu_ui", "PROFILE_SCREEN.BUTTON.title");
		locString.Add("Id", profileId);
		_title.Text = locString.GetFormattedText();
		GodotFileIo godotFileIo = new GodotFileIo(UserDataPathProvider.GetProfileScopedPath(profileId, "saves"));
		_profileIcon.SetProfileId(profileId);
		((CanvasItem)_currentProfileIndicator).Visible = SaveManager.Instance.CurrentProfileId == profileId;
		string path = "progress.save";
		if (NProfileScreen.forceShowProfileAsDeleted == profileId || !godotFileIo.FileExists(path))
		{
			LocString locString2 = new LocString("main_menu_ui", "PROFILE_SCREEN.BUTTON.empty");
			_description.Text = locString2.GetFormattedText();
			return;
		}
		LocString locString3 = new LocString("main_menu_ui", "PROFILE_SCREEN.BUTTON.description");
		if (SaveManager.Instance.CurrentProfileId == profileId)
		{
			locString3.Add("Playtime", TimeFormatting.Format(SaveManager.Instance.Progress.TotalPlaytime));
		}
		else
		{
			string json = godotFileIo.ReadFile(path);
			JsonObject jsonObject = JsonSerializer.Deserialize(json, JsonSerializationUtility.GetTypeInfo<JsonObject>());
			if (jsonObject.TryGetPropertyValue("total_playtime", out JsonNode jsonNode) && jsonNode is JsonValue jsonValue && jsonValue.TryGetValue<long>(out var value))
			{
				locString3.Add("Playtime", TimeFormatting.Format(value));
			}
			else
			{
				locString3.Add("Playtime", "???");
			}
		}
		DateTimeOffset dateTimeOffset = godotFileIo.GetLastModifiedTime(path);
		string path2 = "current_run.save";
		if (godotFileIo.FileExists(path2))
		{
			DateTimeOffset lastModifiedTime = godotFileIo.GetLastModifiedTime(path2);
			dateTimeOffset = ((dateTimeOffset > lastModifiedTime) ? dateTimeOffset : lastModifiedTime);
		}
		string path3 = "current_run_mp.save";
		if (godotFileIo.FileExists(path3))
		{
			DateTimeOffset lastModifiedTime2 = godotFileIo.GetLastModifiedTime(path3);
			dateTimeOffset = ((dateTimeOffset > lastModifiedTime2) ? dateTimeOffset : lastModifiedTime2);
		}
		DateTimeFormatInfo dateTimeFormat = LocManager.Instance.CultureInfo.DateTimeFormat;
		DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeOffset.UtcDateTime, TimeZoneInfo.Local);
		LocString locString4 = new LocString("main_menu_ui", "PROFILE_SCREEN.BUTTON.dateFormat");
		string variable = dateTime.ToString(locString4.GetFormattedText(), dateTimeFormat);
		locString3.Add("LastUpdatedTime", variable);
		_description.Text = locString3.GetFormattedText();
	}

	protected override void OnRelease()
	{
		if (SaveManager.Instance.CurrentProfileId == _profileId)
		{
			NGame.Instance.MainMenu.SubmenuStack.Pop();
		}
		else
		{
			TaskHelper.RunSafely(SwitchToThisProfile());
		}
	}

	private async Task SwitchToThisProfile()
	{
		_profileScreen?.ShowLoading();
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		SaveManager.Instance.SwitchProfileId(_profileId);
		ReadSaveResult<PrefsSave> prefsReadResult = SaveManager.Instance.InitPrefsData();
		ReadSaveResult<SerializableProgress> progressReadResult = SaveManager.Instance.InitProgressData();
		NGame.Instance.ReloadMainMenu();
		NGame.Instance.CheckShowSaveFileError(progressReadResult, prefsReadResult, new ReadSaveResult<SettingsSave>(new SettingsSave()));
	}

	protected override void OnFocus()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.03f), 0.05);
		_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1.3f), 0.05);
	}

	protected override void OnUnfocus()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		base.OnUnfocus();
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	private void UpdateShaderV(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(value));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Initialize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("profileScreen"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)2, StringName.op_Implicit("profileId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderV, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Initialize && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			Initialize(VariantUtils.ConvertTo<NProfileScreen>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
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
		if ((ref method) == MethodName.UpdateShaderV && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderV(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.Initialize)
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
		if ((ref method) == MethodName.UpdateShaderV)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._title)
		{
			_title = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			_description = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentProfileIndicator)
		{
			_currentProfileIndicator = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._profileIcon)
		{
			_profileIcon = VariantUtils.ConvertTo<NProfileIcon>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deleteButton)
		{
			_deleteButton = VariantUtils.ConvertTo<NDeleteProfileButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._background)
		{
			_background = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._profileScreen)
		{
			_profileScreen = VariantUtils.ConvertTo<NProfileScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._profileId)
		{
			_profileId = VariantUtils.ConvertTo<int>(ref value);
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._title)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _title);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _description);
			return true;
		}
		if ((ref name) == PropertyName._currentProfileIndicator)
		{
			value = VariantUtils.CreateFrom<Control>(ref _currentProfileIndicator);
			return true;
		}
		if ((ref name) == PropertyName._profileIcon)
		{
			value = VariantUtils.CreateFrom<NProfileIcon>(ref _profileIcon);
			return true;
		}
		if ((ref name) == PropertyName._deleteButton)
		{
			value = VariantUtils.CreateFrom<NDeleteProfileButton>(ref _deleteButton);
			return true;
		}
		if ((ref name) == PropertyName._background)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _background);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._profileScreen)
		{
			value = VariantUtils.CreateFrom<NProfileScreen>(ref _profileScreen);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._profileId)
		{
			value = VariantUtils.CreateFrom<int>(ref _profileId);
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
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._title, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._description, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentProfileIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._profileIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._deleteButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._background, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._profileScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._profileId, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._title, Variant.From<MegaRichTextLabel>(ref _title));
		info.AddProperty(PropertyName._description, Variant.From<MegaRichTextLabel>(ref _description));
		info.AddProperty(PropertyName._currentProfileIndicator, Variant.From<Control>(ref _currentProfileIndicator));
		info.AddProperty(PropertyName._profileIcon, Variant.From<NProfileIcon>(ref _profileIcon));
		info.AddProperty(PropertyName._deleteButton, Variant.From<NDeleteProfileButton>(ref _deleteButton));
		info.AddProperty(PropertyName._background, Variant.From<TextureRect>(ref _background));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._profileScreen, Variant.From<NProfileScreen>(ref _profileScreen));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._profileId, Variant.From<int>(ref _profileId));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._title, ref val))
		{
			_title = ((Variant)(ref val)).As<MegaRichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._description, ref val2))
		{
			_description = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentProfileIndicator, ref val3))
		{
			_currentProfileIndicator = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._profileIcon, ref val4))
		{
			_profileIcon = ((Variant)(ref val4)).As<NProfileIcon>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._deleteButton, ref val5))
		{
			_deleteButton = ((Variant)(ref val5)).As<NDeleteProfileButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._background, ref val6))
		{
			_background = ((Variant)(ref val6)).As<TextureRect>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val7))
		{
			_hsv = ((Variant)(ref val7)).As<ShaderMaterial>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._profileScreen, ref val8))
		{
			_profileScreen = ((Variant)(ref val8)).As<NProfileScreen>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val9))
		{
			_tween = ((Variant)(ref val9)).As<Tween>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._profileId, ref val10))
		{
			_profileId = ((Variant)(ref val10)).As<int>();
		}
	}
}
