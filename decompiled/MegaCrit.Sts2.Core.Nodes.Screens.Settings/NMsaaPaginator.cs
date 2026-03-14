using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NMsaaPaginator.cs")]
public class NMsaaPaginator : NPaginator, IResettableSettingNode
{
	public new class MethodName : NPaginator.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetFromSettings = StringName.op_Implicit("SetFromSettings");

		public new static readonly StringName OnIndexChanged = StringName.op_Implicit("OnIndexChanged");

		public static readonly StringName GetMsaaLabel = StringName.op_Implicit("GetMsaaLabel");

		public static readonly StringName GetMsaa = StringName.op_Implicit("GetMsaa");
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
		_options.Add("0");
		_options.Add("2");
		_options.Add("4");
		_options.Add("8");
		SetFromSettings();
	}

	public void SetFromSettings()
	{
		int num = _options.IndexOf(SaveManager.Instance.SettingsSave.Msaa.ToString());
		_currentIndex = ((num != -1) ? num : 3);
		_label.SetTextAutoSize(GetMsaaLabel(int.Parse(_options[_currentIndex])));
	}

	protected override void OnIndexChanged(int index)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		_currentIndex = index;
		_label.SetTextAutoSize(GetMsaaLabel(int.Parse(_options[index])));
		SaveManager.Instance.SettingsSave.Msaa = int.Parse(_options[index]);
		Log.Info("MSAA: " + ((Label)_label).Text);
		RenderingServer.ViewportSetMsaa2D(((Node)this).GetViewport().GetViewportRid(), GetMsaa(SaveManager.Instance.SettingsSave.Msaa));
	}

	private string GetMsaaLabel(int msaaAmount)
	{
		return msaaAmount switch
		{
			2 => "2x", 
			4 => "4x", 
			8 => "8x", 
			_ => new LocString("settings_ui", "MSAA_NONE").GetFormattedText(), 
		};
	}

	private ViewportMsaa GetMsaa(int index)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return (ViewportMsaa)(index switch
		{
			2 => 1, 
			4 => 2, 
			8 => 3, 
			_ => 0, 
		});
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
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetFromSettings, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnIndexChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetMsaaLabel, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("msaaAmount"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetMsaa, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
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
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.GetMsaaLabel && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string msaaLabel = GetMsaaLabel(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref msaaLabel);
			return true;
		}
		if ((ref method) == MethodName.GetMsaa && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ViewportMsaa msaa = GetMsaa(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<ViewportMsaa>(ref msaa);
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
		if ((ref method) == MethodName.SetFromSettings)
		{
			return true;
		}
		if ((ref method) == MethodName.OnIndexChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.GetMsaaLabel)
		{
			return true;
		}
		if ((ref method) == MethodName.GetMsaa)
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
