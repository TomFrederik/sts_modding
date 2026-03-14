using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;

[ScriptPath("res://src/Core/Nodes/Screens/RunHistoryScreen/NRunHistoryArrowButton.cs")]
public class NRunHistoryArrowButton : NGoldArrowButton
{
	public new class MethodName : NGoldArrowButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public new class PropertyName : NGoldArrowButton.PropertyName
	{
		public static readonly StringName IsLeft = StringName.op_Implicit("IsLeft");

		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");

		public static readonly StringName _isLeft = StringName.op_Implicit("_isLeft");
	}

	public new class SignalName : NGoldArrowButton.SignalName
	{
	}

	private bool _isLeft;

	public bool IsLeft
	{
		get
		{
			return _isLeft;
		}
		set
		{
			if (_isLeft != value)
			{
				UnregisterHotkeys();
				_isLeft = value;
				RegisterHotkeys();
				_icon.FlipH = !IsLeft;
				UpdateControllerButton();
			}
		}
	}

	protected override string[] Hotkeys => new string[1] { StringName.op_Implicit(_isLeft ? MegaInput.viewDeckAndTabLeft : MegaInput.viewExhaustPileAndTabRight) };

	public override void _Ready()
	{
		base._Ready();
		_icon.FlipH = !IsLeft;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.IsLeft)
		{
			IsLeft = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isLeft)
		{
			_isLeft = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsLeft)
		{
			bool isLeft = IsLeft;
			value = VariantUtils.CreateFrom<bool>(ref isLeft);
			return true;
		}
		if ((ref name) == PropertyName.Hotkeys)
		{
			string[] hotkeys = Hotkeys;
			value = VariantUtils.CreateFrom<string[]>(ref hotkeys);
			return true;
		}
		if ((ref name) == PropertyName._isLeft)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isLeft);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._isLeft, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsLeft, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName isLeft = PropertyName.IsLeft;
		bool isLeft2 = IsLeft;
		info.AddProperty(isLeft, Variant.From<bool>(ref isLeft2));
		info.AddProperty(PropertyName._isLeft, Variant.From<bool>(ref _isLeft));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsLeft, ref val))
		{
			IsLeft = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._isLeft, ref val2))
		{
			_isLeft = ((Variant)(ref val2)).As<bool>();
		}
	}
}
