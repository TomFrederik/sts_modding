using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Debug;

[ScriptPath("res://src/Core/Nodes/Debug/NDebugInfoLabelManager.cs")]
public class NDebugInfoLabelManager : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName UpdateText = StringName.op_Implicit("UpdateText");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName isMainMenu = StringName.op_Implicit("isMainMenu");

		public static readonly StringName _releaseInfo = StringName.op_Implicit("_releaseInfo");

		public static readonly StringName _moddedWarning = StringName.op_Implicit("_moddedWarning");

		public static readonly StringName _seed = StringName.op_Implicit("_seed");

		public static readonly StringName _runningModded = StringName.op_Implicit("_runningModded");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	public bool isMainMenu;

	private MegaLabel _releaseInfo;

	private MegaLabel _moddedWarning;

	private MegaLabel? _seed;

	private bool _runningModded;

	public override void _Ready()
	{
		_releaseInfo = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ReleaseInfo"));
		_moddedWarning = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ModdedWarning"));
		_seed = ((Node)this).GetNodeOrNull<MegaLabel>(NodePath.op_Implicit("%DebugSeed"));
		_runningModded = ModManager.LoadedMods.Count > 0;
		UpdateText(null);
		if (ReleaseInfoManager.Instance.ReleaseInfo == null)
		{
			TaskHelper.RunSafely(SetCommitIdInEditor());
		}
	}

	private async Task SetCommitIdInEditor()
	{
		if (GitHelper.ShortCommitIdTask != null)
		{
			UpdateText(await GitHelper.ShortCommitIdTask);
		}
	}

	private void UpdateText(string? commitId)
	{
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		ReleaseInfo releaseInfo = ReleaseInfoManager.Instance.ReleaseInfo;
		string text = DateTime.Now.ToString("yyyy-MM-dd");
		string text2 = releaseInfo?.Version ?? commitId ?? "NONE";
		if (isMainMenu)
		{
			((Label)_releaseInfo).Text = text2 + "\n" + text;
		}
		else
		{
			((Label)_releaseInfo).Text = $"[{text2}] ({text})";
		}
		((CanvasItem)_moddedWarning).Visible = _runningModded;
		if (_runningModded)
		{
			bool flag = ModManager.LoadedMods.Any((Mod m) => !(m.assemblyLoadedSuccessfully ?? true));
			if (isMainMenu)
			{
				LocString locString = new LocString("main_menu_ui", "MODDED_WARNING");
				locString.Add("count", ModManager.LoadedMods.Count);
				locString.Add("hasError", flag);
				_moddedWarning.SetTextAutoSize(locString.GetFormattedText());
			}
			else
			{
				_moddedWarning.SetTextAutoSize($"MODDED ({ModManager.LoadedMods.Count})");
			}
			if (flag)
			{
				((CanvasItem)_moddedWarning).Modulate = StsColors.redGlow;
			}
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(DebugHotkey.hideVersionInfo, false))
		{
			((CanvasItem)_releaseInfo).Visible = !((CanvasItem)_releaseInfo).Visible;
			((CanvasItem)_moddedWarning).Visible = _runningModded && !((CanvasItem)_moddedWarning).Visible;
			MegaLabel? seed = _seed;
			if (seed != null)
			{
				((CanvasItem)seed).SetVisible(!((CanvasItem)_seed).Visible);
			}
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(((CanvasItem)_releaseInfo).Visible ? "Show Version Info" : "Hide Version Info"));
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("commitId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateText && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateText(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateText)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.isMainMenu)
		{
			isMainMenu = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._releaseInfo)
		{
			_releaseInfo = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._moddedWarning)
		{
			_moddedWarning = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._seed)
		{
			_seed = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._runningModded)
		{
			_runningModded = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName.isMainMenu)
		{
			value = VariantUtils.CreateFrom<bool>(ref isMainMenu);
			return true;
		}
		if ((ref name) == PropertyName._releaseInfo)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _releaseInfo);
			return true;
		}
		if ((ref name) == PropertyName._moddedWarning)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _moddedWarning);
			return true;
		}
		if ((ref name) == PropertyName._seed)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _seed);
			return true;
		}
		if ((ref name) == PropertyName._runningModded)
		{
			value = VariantUtils.CreateFrom<bool>(ref _runningModded);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.isMainMenu, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._releaseInfo, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._moddedWarning, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._seed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._runningModded, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName.isMainMenu, Variant.From<bool>(ref isMainMenu));
		info.AddProperty(PropertyName._releaseInfo, Variant.From<MegaLabel>(ref _releaseInfo));
		info.AddProperty(PropertyName._moddedWarning, Variant.From<MegaLabel>(ref _moddedWarning));
		info.AddProperty(PropertyName._seed, Variant.From<MegaLabel>(ref _seed));
		info.AddProperty(PropertyName._runningModded, Variant.From<bool>(ref _runningModded));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.isMainMenu, ref val))
		{
			isMainMenu = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._releaseInfo, ref val2))
		{
			_releaseInfo = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._moddedWarning, ref val3))
		{
			_moddedWarning = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._seed, ref val4))
		{
			_seed = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._runningModded, ref val5))
		{
			_runningModded = ((Variant)(ref val5)).As<bool>();
		}
	}
}
