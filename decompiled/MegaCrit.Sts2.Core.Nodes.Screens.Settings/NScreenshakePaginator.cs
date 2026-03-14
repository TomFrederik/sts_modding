using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NScreenshakePaginator.cs")]
public class NScreenshakePaginator : NPaginator, IResettableSettingNode
{
	public new class MethodName : NPaginator.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetFromSettings = StringName.op_Implicit("SetFromSettings");

		public new static readonly StringName OnIndexChanged = StringName.op_Implicit("OnIndexChanged");

		public static readonly StringName GetShakeMultiplier = StringName.op_Implicit("GetShakeMultiplier");
	}

	public new class PropertyName : NPaginator.PropertyName
	{
	}

	public new class SignalName : NPaginator.SignalName
	{
	}

	public override void _Ready()
	{
		ConnectSignals();
		_options.Add(new LocString("settings_ui", "SCREENSHAKE_NONE").GetFormattedText());
		_options.Add(new LocString("settings_ui", "SCREENSHAKE_SOME").GetFormattedText());
		_options.Add(new LocString("settings_ui", "SCREENSHAKE_NORMAL").GetFormattedText());
		_options.Add(new LocString("settings_ui", "SCREENSHAKE_LOTS").GetFormattedText());
		_options.Add(new LocString("settings_ui", "SCREENSHAKE_CAAAW").GetFormattedText());
		SetFromSettings();
	}

	public void SetFromSettings()
	{
		_currentIndex = SaveManager.Instance.PrefsSave.ScreenShakeOptionIndex;
		if (_currentIndex >= _options.Count)
		{
			_label.SetTextAutoSize(">:P");
		}
		else
		{
			_label.SetTextAutoSize(_options[_currentIndex]);
		}
		NGame.Instance.SetScreenshakeMultiplier(GetShakeMultiplier(_currentIndex));
	}

	protected override void OnIndexChanged(int index)
	{
		if (_currentIndex >= _options.Count)
		{
			_currentIndex = 2;
		}
		_currentIndex = index;
		_label.SetTextAutoSize(_options[index]);
		SaveManager.Instance.PrefsSave.ScreenShakeOptionIndex = _currentIndex;
		Log.Info($"Screenshake set to: {_currentIndex}");
		NGame.Instance.SetScreenshakeMultiplier(GetShakeMultiplier(_currentIndex));
		NGame.Instance.ScreenShakeTrauma(ShakeStrength.Medium);
	}

	public static float GetShakeMultiplier(int index)
	{
		return index switch
		{
			0 => 0f, 
			1 => 0.5f, 
			2 => 1f, 
			3 => 2f, 
			4 => 4f, 
			_ => index, 
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetFromSettings, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnIndexChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetShakeMultiplier, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetFromSettings && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetFromSettings();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnIndexChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnIndexChanged(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetShakeMultiplier && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			float shakeMultiplier = GetShakeMultiplier(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<float>(ref shakeMultiplier);
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
		if ((ref method) == MethodName.GetShakeMultiplier && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			float shakeMultiplier = GetShakeMultiplier(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<float>(ref shakeMultiplier);
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
		if ((ref method) == MethodName.SetFromSettings)
		{
			return true;
		}
		if ((ref method) == MethodName.OnIndexChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.GetShakeMultiplier)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
	}
}
