using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Bestiary;

[ScriptPath("res://src/Core/Nodes/Screens/Bestiary/NBestiaryEntry.cs")]
public class NBestiaryEntry : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Select = StringName.op_Implicit("Select");

		public static readonly StringName Deselect = StringName.op_Implicit("Deselect");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName IsLocked = StringName.op_Implicit("IsLocked");

		public static readonly StringName _nameLabel = StringName.op_Implicit("_nameLabel");

		public static readonly StringName _highlight = StringName.op_Implicit("_highlight");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private MegaRichTextLabel _nameLabel;

	private Control _highlight;

	private static string ScenePath => SceneHelper.GetScenePath("screens/bestiary/bestiary_entry");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public MonsterModel Monster { get; private set; }

	public bool IsLocked { get; private set; }

	public static NBestiaryEntry Create(MonsterModel monster, bool isLocked)
	{
		NBestiaryEntry nBestiaryEntry = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NBestiaryEntry>((GenEditState)0);
		nBestiaryEntry.Monster = monster;
		nBestiaryEntry.IsLocked = isLocked;
		return nBestiaryEntry;
	}

	public override void _Ready()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_nameLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("Label"));
		_highlight = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Highlight"));
		if (IsLocked)
		{
			_nameLabel.Text = "[Locked]";
			((CanvasItem)_nameLabel).Modulate = StsColors.gray;
		}
		else
		{
			_nameLabel.Text = Monster.Title.GetFormattedText();
			((CanvasItem)_nameLabel).Modulate = StsColors.cream;
		}
	}

	public void Select()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_nameLabel).Modulate = StsColors.gold;
	}

	public void Deselect()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_nameLabel).Modulate = (IsLocked ? StsColors.gray : StsColors.cream);
	}

	protected override void OnFocus()
	{
		base.OnFocus();
		if (NControllerManager.Instance.IsUsingController)
		{
			((CanvasItem)_highlight).Visible = true;
		}
	}

	protected override void OnUnfocus()
	{
		base.OnUnfocus();
		((CanvasItem)_highlight).Visible = false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Select, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Deselect, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Select && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Select();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Deselect && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Deselect();
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
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.Select)
		{
			return true;
		}
		if ((ref method) == MethodName.Deselect)
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
		if ((ref name) == PropertyName.IsLocked)
		{
			IsLocked = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nameLabel)
		{
			_nameLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._highlight)
		{
			_highlight = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		if ((ref name) == PropertyName.IsLocked)
		{
			bool isLocked = IsLocked;
			value = VariantUtils.CreateFrom<bool>(ref isLocked);
			return true;
		}
		if ((ref name) == PropertyName._nameLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _nameLabel);
			return true;
		}
		if ((ref name) == PropertyName._highlight)
		{
			value = VariantUtils.CreateFrom<Control>(ref _highlight);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._nameLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._highlight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsLocked, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName isLocked = PropertyName.IsLocked;
		bool isLocked2 = IsLocked;
		info.AddProperty(isLocked, Variant.From<bool>(ref isLocked2));
		info.AddProperty(PropertyName._nameLabel, Variant.From<MegaRichTextLabel>(ref _nameLabel));
		info.AddProperty(PropertyName._highlight, Variant.From<Control>(ref _highlight));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsLocked, ref val))
		{
			IsLocked = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._nameLabel, ref val2))
		{
			_nameLabel = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._highlight, ref val3))
		{
			_highlight = ((Variant)(ref val3)).As<Control>();
		}
	}
}
