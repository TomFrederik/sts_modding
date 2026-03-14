using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NDropdownScrollbar.cs")]
public class NDropdownScrollbar : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshTrainBounds = StringName.op_Implicit("RefreshTrainBounds");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName OnShow = StringName.op_Implicit("OnShow");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName _Input = StringName.op_Implicit("_Input");

		public new static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName ClampTrain = StringName.op_Implicit("ClampTrain");

		public static readonly StringName SetTrainPositionFromPercentage = StringName.op_Implicit("SetTrainPositionFromPercentage");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName _dropdownContainer = StringName.op_Implicit("_dropdownContainer");

		public static readonly StringName _train = StringName.op_Implicit("_train");

		public static readonly StringName hasControl = StringName.op_Implicit("hasControl");

		public static readonly StringName _startDragPos = StringName.op_Implicit("_startDragPos");

		public static readonly StringName _targetDragPos = StringName.op_Implicit("_targetDragPos");

		public static readonly StringName _scrollLimitTop = StringName.op_Implicit("_scrollLimitTop");

		public static readonly StringName _scrollLimitBottom = StringName.op_Implicit("_scrollLimitBottom");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private NDropdownContainer _dropdownContainer;

	private Control _train;

	public bool hasControl;

	private Vector2 _startDragPos;

	private Vector2 _targetDragPos;

	private float _scrollLimitTop;

	private float _scrollLimitBottom;

	public override void _Ready()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_dropdownContainer = ((Node)this).GetParent<NDropdownContainer>();
		_train = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Train"));
		((GodotObject)this).Connect(SignalName.VisibilityChanged, Callable.From((Action)OnShow), 0u);
	}

	public void RefreshTrainBounds()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_scrollLimitTop = 600f - _train.Size.Y - 9f;
		_scrollLimitBottom = 9f;
	}

	protected override void OnFocus()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_train).Modulate = StsColors.gold;
	}

	protected override void OnUnfocus()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!hasControl)
		{
			((CanvasItem)_train).Modulate = StsColors.quarterTransparentWhite;
		}
	}

	private void OnShow()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_train).Modulate = StsColors.quarterTransparentWhite;
	}

	protected override void OnPress()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		hasControl = true;
		((CanvasItem)_train).Modulate = StsColors.gold;
		Input.MouseMode = (MouseModeEnum)1;
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I8
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base._Input(inputEvent);
		if (hasControl)
		{
			InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
			if (val != null && (long)val.ButtonIndex == 1 && ((InputEvent)val).IsReleased())
			{
				hasControl = false;
				Input.MouseMode = (MouseModeEnum)0;
				((CanvasItem)_train).Modulate = StsColors.quarterTransparentWhite;
			}
		}
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		base._GuiInput(inputEvent);
		if (hasControl)
		{
			InputEventMouseMotion val = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
			if (val != null)
			{
				Control train = _train;
				train.Position += new Vector2(0f, val.Relative.Y);
				ClampTrain();
				_dropdownContainer.UpdatePositionBasedOnTrain(1f - (_train.Position.Y - _scrollLimitBottom) / (_scrollLimitTop - _scrollLimitBottom));
			}
		}
	}

	private void ClampTrain()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		_train.Position = new Vector2(_train.Position.X, Mathf.Clamp(_train.Position.Y, _scrollLimitBottom, _scrollLimitTop));
	}

	public void SetTrainPositionFromPercentage(float percentage)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		_train.Position = new Vector2(_train.Position.X, _scrollLimitBottom + percentage * (_scrollLimitTop - _scrollLimitBottom));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected O, but got Unknown
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshTrainBounds, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnShow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClampTrain, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTrainPositionFromPercentage, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("percentage"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshTrainBounds && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshTrainBounds();
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
		if ((ref method) == MethodName.OnShow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnShow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClampTrain && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClampTrain();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetTrainPositionFromPercentage && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTrainPositionFromPercentage(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.RefreshTrainBounds)
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
		if ((ref method) == MethodName.OnShow)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName.ClampTrain)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTrainPositionFromPercentage)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._dropdownContainer)
		{
			_dropdownContainer = VariantUtils.ConvertTo<NDropdownContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._train)
		{
			_train = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.hasControl)
		{
			hasControl = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._startDragPos)
		{
			_startDragPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetDragPos)
		{
			_targetDragPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollLimitTop)
		{
			_scrollLimitTop = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollLimitBottom)
		{
			_scrollLimitBottom = VariantUtils.ConvertTo<float>(ref value);
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
		if ((ref name) == PropertyName._dropdownContainer)
		{
			value = VariantUtils.CreateFrom<NDropdownContainer>(ref _dropdownContainer);
			return true;
		}
		if ((ref name) == PropertyName._train)
		{
			value = VariantUtils.CreateFrom<Control>(ref _train);
			return true;
		}
		if ((ref name) == PropertyName.hasControl)
		{
			value = VariantUtils.CreateFrom<bool>(ref hasControl);
			return true;
		}
		if ((ref name) == PropertyName._startDragPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _startDragPos);
			return true;
		}
		if ((ref name) == PropertyName._targetDragPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetDragPos);
			return true;
		}
		if ((ref name) == PropertyName._scrollLimitTop)
		{
			value = VariantUtils.CreateFrom<float>(ref _scrollLimitTop);
			return true;
		}
		if ((ref name) == PropertyName._scrollLimitBottom)
		{
			value = VariantUtils.CreateFrom<float>(ref _scrollLimitBottom);
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
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._dropdownContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._train, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.hasControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._startDragPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetDragPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._scrollLimitTop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._scrollLimitBottom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._dropdownContainer, Variant.From<NDropdownContainer>(ref _dropdownContainer));
		info.AddProperty(PropertyName._train, Variant.From<Control>(ref _train));
		info.AddProperty(PropertyName.hasControl, Variant.From<bool>(ref hasControl));
		info.AddProperty(PropertyName._startDragPos, Variant.From<Vector2>(ref _startDragPos));
		info.AddProperty(PropertyName._targetDragPos, Variant.From<Vector2>(ref _targetDragPos));
		info.AddProperty(PropertyName._scrollLimitTop, Variant.From<float>(ref _scrollLimitTop));
		info.AddProperty(PropertyName._scrollLimitBottom, Variant.From<float>(ref _scrollLimitBottom));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._dropdownContainer, ref val))
		{
			_dropdownContainer = ((Variant)(ref val)).As<NDropdownContainer>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._train, ref val2))
		{
			_train = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.hasControl, ref val3))
		{
			hasControl = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._startDragPos, ref val4))
		{
			_startDragPos = ((Variant)(ref val4)).As<Vector2>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetDragPos, ref val5))
		{
			_targetDragPos = ((Variant)(ref val5)).As<Vector2>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollLimitTop, ref val6))
		{
			_scrollLimitTop = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollLimitBottom, ref val7))
		{
			_scrollLimitBottom = ((Variant)(ref val7)).As<float>();
		}
	}
}
