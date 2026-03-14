using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Nodes;

[ScriptPath("res://src/Core/Nodes/NBackgroundModeHandler.cs")]
public class NBackgroundModeHandler : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Notification = StringName.op_Implicit("_Notification");

		public static readonly StringName EnterBackgroundMode = StringName.op_Implicit("EnterBackgroundMode");

		public static readonly StringName ExitBackgroundMode = StringName.op_Implicit("ExitBackgroundMode");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _savedMaxFps = StringName.op_Implicit("_savedMaxFps");

		public static readonly StringName _isBackgrounded = StringName.op_Implicit("_isBackgrounded");
	}

	public class SignalName : SignalName
	{
	}

	private const int _backgroundFps = 30;

	private int _savedMaxFps;

	private bool _isBackgrounded;

	private static bool IsHeadless => DisplayServer.GetName().Equals("headless", StringComparison.OrdinalIgnoreCase);

	private static bool IsEditor => OS.HasFeature("editor");

	public override void _Notification(int what)
	{
		if (!IsHeadless && !IsEditor && !NonInteractiveMode.IsActive)
		{
			if ((long)what == 1005)
			{
				EnterBackgroundMode();
			}
			else if ((long)what == 1004)
			{
				ExitBackgroundMode();
			}
		}
	}

	private void EnterBackgroundMode()
	{
		if (!_isBackgrounded && SaveManager.Instance.SettingsSave.LimitFpsInBackground)
		{
			INetGameService netService = RunManager.Instance.NetService;
			if (netService == null || !netService.Type.IsMultiplayer())
			{
				_isBackgrounded = true;
				_savedMaxFps = Engine.MaxFps;
				Engine.MaxFps = 30;
				Log.Info($"Limiting background FPS to {30}");
			}
		}
	}

	private void ExitBackgroundMode()
	{
		if (_isBackgrounded)
		{
			_isBackgrounded = false;
			Engine.MaxFps = _savedMaxFps;
			Log.Info("Restored foreground FPS");
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Notification, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("what"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnterBackgroundMode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ExitBackgroundMode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Notification && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((GodotObject)this)._Notification(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnterBackgroundMode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnterBackgroundMode();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ExitBackgroundMode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ExitBackgroundMode();
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Notification)
		{
			return true;
		}
		if ((ref method) == MethodName.EnterBackgroundMode)
		{
			return true;
		}
		if ((ref method) == MethodName.ExitBackgroundMode)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._savedMaxFps)
		{
			_savedMaxFps = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isBackgrounded)
		{
			_isBackgrounded = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._savedMaxFps)
		{
			value = VariantUtils.CreateFrom<int>(ref _savedMaxFps);
			return true;
		}
		if ((ref name) == PropertyName._isBackgrounded)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isBackgrounded);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName._savedMaxFps, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isBackgrounded, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._savedMaxFps, Variant.From<int>(ref _savedMaxFps));
		info.AddProperty(PropertyName._isBackgrounded, Variant.From<bool>(ref _isBackgrounded));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._savedMaxFps, ref val))
		{
			_savedMaxFps = ((Variant)(ref val)).As<int>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._isBackgrounded, ref val2))
		{
			_isBackgrounded = ((Variant)(ref val2)).As<bool>();
		}
	}
}
