using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NCursorManager.cs")]
public class NCursorManager : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName StopOverridingCursor = StringName.op_Implicit("StopOverridingCursor");

		public static readonly StringName OverrideCursor = StringName.op_Implicit("OverrideCursor");

		public static readonly StringName UpdateCursor = StringName.op_Implicit("UpdateCursor");

		public static readonly StringName SetIsUsingController = StringName.op_Implicit("SetIsUsingController");

		public static readonly StringName SetCursorShown = StringName.op_Implicit("SetCursorShown");

		public static readonly StringName RefreshCursorShown = StringName.op_Implicit("RefreshCursorShown");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName CursorTilted = StringName.op_Implicit("CursorTilted");

		public static readonly StringName CursorNotTilted = StringName.op_Implicit("CursorNotTilted");

		public static readonly StringName HotSpot = StringName.op_Implicit("HotSpot");

		public static readonly StringName _cursorTilted = StringName.op_Implicit("_cursorTilted");

		public static readonly StringName _cursorNotTilted = StringName.op_Implicit("_cursorNotTilted");

		public static readonly StringName _cursorInspect = StringName.op_Implicit("_cursorInspect");

		public static readonly StringName _overriddenCursorTilted = StringName.op_Implicit("_overriddenCursorTilted");

		public static readonly StringName _overriddenCursorNotTilted = StringName.op_Implicit("_overriddenCursorNotTilted");

		public static readonly StringName _lastSetCursor = StringName.op_Implicit("_lastSetCursor");

		public static readonly StringName _isDown = StringName.op_Implicit("_isDown");

		public static readonly StringName _isUsingController = StringName.op_Implicit("_isUsingController");

		public static readonly StringName _shouldShowCursor = StringName.op_Implicit("_shouldShowCursor");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly Vector2 _defaultHotSpot = new Vector2(14f, 5f);

	private static readonly Vector2 _inspectHotSpot = new Vector2(12f, 12f);

	[Export(/*Could not decode attribute arguments.*/)]
	private Image _cursorTilted;

	[Export(/*Could not decode attribute arguments.*/)]
	private Image _cursorNotTilted;

	[Export(/*Could not decode attribute arguments.*/)]
	private Image _cursorInspect;

	private Image? _overriddenCursorTilted;

	private Image? _overriddenCursorNotTilted;

	private Vector2? _overriddenHotSpot;

	private Image? _lastSetCursor;

	private bool _isDown;

	private bool _isUsingController;

	private bool _shouldShowCursor = true;

	private Image CursorTilted => _overriddenCursorTilted ?? _cursorTilted;

	private Image CursorNotTilted => _overriddenCursorNotTilted ?? _cursorNotTilted;

	private Vector2 HotSpot => (Vector2)(((_003F?)_overriddenHotSpot) ?? _defaultHotSpot);

	public override void _EnterTree()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Input.SetCustomMouseCursor((Resource)(object)_cursorInspect, (CursorShape)16, (Vector2?)_inspectHotSpot);
		UpdateCursor();
	}

	public override void _Ready()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)delegate
		{
			SetIsUsingController(isUsingController: true);
		}), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)delegate
		{
			SetIsUsingController(isUsingController: false);
		}), 0u);
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I8
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I8
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I8
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null && ((long)val.ButtonIndex == 1 || (long)val.ButtonIndex == 2 || (long)val.ButtonIndex == 3))
		{
			if (((InputEvent)val).IsPressed() && !_isDown)
			{
				_isDown = true;
				UpdateCursor();
			}
			else if (((InputEvent)val).IsReleased() && _isDown)
			{
				_isDown = false;
				UpdateCursor();
			}
		}
	}

	public void StopOverridingCursor()
	{
		_overriddenCursorTilted = null;
		_overriddenCursorNotTilted = null;
		_overriddenHotSpot = null;
		UpdateCursor();
	}

	public void OverrideCursor(Image cursorTilted, Image cursorNotTilted, Vector2 hotspot)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		_overriddenCursorTilted = cursorTilted;
		_overriddenCursorNotTilted = cursorNotTilted;
		_overriddenHotSpot = hotspot;
		UpdateCursor();
	}

	private void UpdateCursor()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I8
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if ((long)Input.MouseMode != 1)
		{
			Image val = (_isDown ? CursorTilted : CursorNotTilted);
			if (val != _lastSetCursor)
			{
				Input.SetCustomMouseCursor((Resource)(object)val, (CursorShape)0, (Vector2?)HotSpot);
				_lastSetCursor = val;
			}
		}
	}

	private void SetIsUsingController(bool isUsingController)
	{
		_isUsingController = isUsingController;
		RefreshCursorShown();
	}

	public void SetCursorShown(bool show)
	{
		_shouldShowCursor = show;
		RefreshCursorShown();
	}

	private void RefreshCursorShown()
	{
		bool flag = !_isUsingController && _shouldShowCursor;
		Input.MouseMode = (MouseModeEnum)(flag ? 0 : 1);
		if (!flag)
		{
			_lastSetCursor = null;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopOverridingCursor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OverrideCursor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cursorTilted"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Image"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("cursorNotTilted"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Image"), false),
			new PropertyInfo((Type)5, StringName.op_Implicit("hotspot"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateCursor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetIsUsingController, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isUsingController"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCursorShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("show"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshCursorShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopOverridingCursor && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopOverridingCursor();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OverrideCursor && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			OverrideCursor(VariantUtils.ConvertTo<Image>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Image>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateCursor && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateCursor();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetIsUsingController && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetIsUsingController(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCursorShown && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCursorShown(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshCursorShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshCursorShown();
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.StopOverridingCursor)
		{
			return true;
		}
		if ((ref method) == MethodName.OverrideCursor)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateCursor)
		{
			return true;
		}
		if ((ref method) == MethodName.SetIsUsingController)
		{
			return true;
		}
		if ((ref method) == MethodName.SetCursorShown)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshCursorShown)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._cursorTilted)
		{
			_cursorTilted = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cursorNotTilted)
		{
			_cursorNotTilted = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cursorInspect)
		{
			_cursorInspect = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._overriddenCursorTilted)
		{
			_overriddenCursorTilted = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._overriddenCursorNotTilted)
		{
			_overriddenCursorNotTilted = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastSetCursor)
		{
			_lastSetCursor = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isDown)
		{
			_isDown = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isUsingController)
		{
			_isUsingController = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shouldShowCursor)
		{
			_shouldShowCursor = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CursorTilted)
		{
			Image cursorTilted = CursorTilted;
			value = VariantUtils.CreateFrom<Image>(ref cursorTilted);
			return true;
		}
		if ((ref name) == PropertyName.CursorNotTilted)
		{
			Image cursorTilted = CursorNotTilted;
			value = VariantUtils.CreateFrom<Image>(ref cursorTilted);
			return true;
		}
		if ((ref name) == PropertyName.HotSpot)
		{
			Vector2 hotSpot = HotSpot;
			value = VariantUtils.CreateFrom<Vector2>(ref hotSpot);
			return true;
		}
		if ((ref name) == PropertyName._cursorTilted)
		{
			value = VariantUtils.CreateFrom<Image>(ref _cursorTilted);
			return true;
		}
		if ((ref name) == PropertyName._cursorNotTilted)
		{
			value = VariantUtils.CreateFrom<Image>(ref _cursorNotTilted);
			return true;
		}
		if ((ref name) == PropertyName._cursorInspect)
		{
			value = VariantUtils.CreateFrom<Image>(ref _cursorInspect);
			return true;
		}
		if ((ref name) == PropertyName._overriddenCursorTilted)
		{
			value = VariantUtils.CreateFrom<Image>(ref _overriddenCursorTilted);
			return true;
		}
		if ((ref name) == PropertyName._overriddenCursorNotTilted)
		{
			value = VariantUtils.CreateFrom<Image>(ref _overriddenCursorNotTilted);
			return true;
		}
		if ((ref name) == PropertyName._lastSetCursor)
		{
			value = VariantUtils.CreateFrom<Image>(ref _lastSetCursor);
			return true;
		}
		if ((ref name) == PropertyName._isDown)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isDown);
			return true;
		}
		if ((ref name) == PropertyName._isUsingController)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isUsingController);
			return true;
		}
		if ((ref name) == PropertyName._shouldShowCursor)
		{
			value = VariantUtils.CreateFrom<bool>(ref _shouldShowCursor);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._cursorTilted, (PropertyHint)17, "Image", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._cursorNotTilted, (PropertyHint)17, "Image", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._cursorInspect, (PropertyHint)17, "Image", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._overriddenCursorTilted, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._overriddenCursorNotTilted, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CursorTilted, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CursorNotTilted, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.HotSpot, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lastSetCursor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isDown, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isUsingController, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._shouldShowCursor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._cursorTilted, Variant.From<Image>(ref _cursorTilted));
		info.AddProperty(PropertyName._cursorNotTilted, Variant.From<Image>(ref _cursorNotTilted));
		info.AddProperty(PropertyName._cursorInspect, Variant.From<Image>(ref _cursorInspect));
		info.AddProperty(PropertyName._overriddenCursorTilted, Variant.From<Image>(ref _overriddenCursorTilted));
		info.AddProperty(PropertyName._overriddenCursorNotTilted, Variant.From<Image>(ref _overriddenCursorNotTilted));
		info.AddProperty(PropertyName._lastSetCursor, Variant.From<Image>(ref _lastSetCursor));
		info.AddProperty(PropertyName._isDown, Variant.From<bool>(ref _isDown));
		info.AddProperty(PropertyName._isUsingController, Variant.From<bool>(ref _isUsingController));
		info.AddProperty(PropertyName._shouldShowCursor, Variant.From<bool>(ref _shouldShowCursor));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._cursorTilted, ref val))
		{
			_cursorTilted = ((Variant)(ref val)).As<Image>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cursorNotTilted, ref val2))
		{
			_cursorNotTilted = ((Variant)(ref val2)).As<Image>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._cursorInspect, ref val3))
		{
			_cursorInspect = ((Variant)(ref val3)).As<Image>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._overriddenCursorTilted, ref val4))
		{
			_overriddenCursorTilted = ((Variant)(ref val4)).As<Image>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._overriddenCursorNotTilted, ref val5))
		{
			_overriddenCursorNotTilted = ((Variant)(ref val5)).As<Image>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastSetCursor, ref val6))
		{
			_lastSetCursor = ((Variant)(ref val6)).As<Image>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isDown, ref val7))
		{
			_isDown = ((Variant)(ref val7)).As<bool>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._isUsingController, ref val8))
		{
			_isUsingController = ((Variant)(ref val8)).As<bool>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._shouldShowCursor, ref val9))
		{
			_shouldShowCursor = ((Variant)(ref val9)).As<bool>();
		}
	}
}
