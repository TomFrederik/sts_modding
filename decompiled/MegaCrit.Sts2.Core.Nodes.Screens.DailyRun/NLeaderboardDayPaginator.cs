using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;

[ScriptPath("res://src/Core/Nodes/Screens/DailyRun/NLeaderboardDayPaginator.cs")]
public class NLeaderboardDayPaginator : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName PageLeft = StringName.op_Implicit("PageLeft");

		public static readonly StringName PageRight = StringName.op_Implicit("PageRight");

		public static readonly StringName DayChangeHelper = StringName.op_Implicit("DayChangeHelper");

		public static readonly StringName OnDayChanged = StringName.op_Implicit("OnDayChanged");

		public static readonly StringName Disable = StringName.op_Implicit("Disable");

		public static readonly StringName Enable = StringName.op_Implicit("Enable");

		public static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _vfxLabel = StringName.op_Implicit("_vfxLabel");

		public static readonly StringName _leftArrow = StringName.op_Implicit("_leftArrow");

		public static readonly StringName _rightArrow = StringName.op_Implicit("_rightArrow");

		public static readonly StringName _selectionReticle = StringName.op_Implicit("_selectionReticle");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _leaderboard = StringName.op_Implicit("_leaderboard");
	}

	public class SignalName : SignalName
	{
	}

	protected MegaLabel _label;

	private MegaLabel _vfxLabel;

	private NLeaderboardPageArrow _leftArrow;

	private NLeaderboardPageArrow _rightArrow;

	private NSelectionReticle _selectionReticle;

	private Tween? _tween;

	private const double _animDuration = 0.25;

	private const float _animDistance = 90f;

	private DateTimeOffset _currentDay;

	private NDailyRunLeaderboard? _leaderboard;

	public override void _Ready()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		_label = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("LabelContainer/Mask/Label"));
		_vfxLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("LabelContainer/Mask/VfxLabel"));
		_selectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("SelectionReticle"));
		_leftArrow = ((Node)this).GetNode<NLeaderboardPageArrow>(NodePath.op_Implicit("LeftArrow"));
		_rightArrow = ((Node)this).GetNode<NLeaderboardPageArrow>(NodePath.op_Implicit("RightArrow"));
		((GodotObject)this).Connect(SignalName.FocusEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)this).Connect(SignalName.FocusExited, Callable.From((Action)OnUnfocus), 0u);
		_leftArrow.Connect(PageLeft);
		_rightArrow.Connect(PageRight);
	}

	public void Initialize(NDailyRunLeaderboard leaderboard, DateTimeOffset dateTime, bool showArrows)
	{
		_currentDay = dateTime;
		_leaderboard = leaderboard;
		OnDayChanged(changeLeaderboardDay: false);
		((CanvasItem)_leftArrow).Visible = showArrows;
		((CanvasItem)_rightArrow).Visible = showArrows;
	}

	public override void _GuiInput(InputEvent input)
	{
		((Control)this)._GuiInput(input);
		if (input.IsActionPressed(MegaInput.left, false, false))
		{
			PageLeft();
		}
		if (input.IsActionPressed(MegaInput.right, false, false))
		{
			PageRight();
		}
	}

	private void PageLeft()
	{
		_currentDay -= TimeSpan.FromDays(1);
		DayChangeHelper(pagedLeft: true);
	}

	private void PageRight()
	{
		_currentDay += TimeSpan.FromDays(1);
		DayChangeHelper(pagedLeft: false);
	}

	private void DayChangeHelper(bool pagedLeft)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		_vfxLabel.SetTextAutoSize(((Label)_label).Text);
		((CanvasItem)_vfxLabel).Modulate = ((CanvasItem)_label).Modulate;
		OnDayChanged(changeLeaderboardDay: true);
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("position:x"), Variant.op_Implicit(0f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.From(Variant.op_Implicit(pagedLeft ? (-90f) : 90f));
		_tween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25).From(Variant.op_Implicit(0.75f));
		_tween.TweenProperty((GodotObject)(object)_vfxLabel, NodePath.op_Implicit("position:x"), Variant.op_Implicit(pagedLeft ? 90f : (-90f)), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.From(Variant.op_Implicit(0f));
		_tween.TweenProperty((GodotObject)(object)_vfxLabel, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentBlack), 0.25);
	}

	private void OnDayChanged(bool changeLeaderboardDay)
	{
		_label.SetTextAutoSize(_currentDay.ToString(NDailyRunScreen.dateFormat));
		if (changeLeaderboardDay)
		{
			_leaderboard.SetDay(_currentDay);
		}
	}

	public void Disable()
	{
		_leftArrow.Disable();
		_rightArrow.Disable();
	}

	public void Enable(bool leftArrowEnabled, bool rightArrowEnabled)
	{
		if (leftArrowEnabled)
		{
			_leftArrow.Enable();
		}
		if (rightArrowEnabled)
		{
			_rightArrow.Enable();
		}
	}

	private void OnFocus()
	{
		if (NControllerManager.Instance.IsUsingController)
		{
			_selectionReticle.OnSelect();
		}
	}

	private void OnUnfocus()
	{
		_selectionReticle.OnDeselect();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("input"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PageLeft, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PageRight, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DayChangeHelper, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("pagedLeft"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDayChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("changeLeaderboardDay"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Disable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Enable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("leftArrowEnabled"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("rightArrowEnabled"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PageLeft && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PageLeft();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PageRight && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PageRight();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DayChangeHelper && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DayChangeHelper(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDayChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnDayChanged(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Disable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Disable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Enable && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			Enable(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
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
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName.PageLeft)
		{
			return true;
		}
		if ((ref method) == MethodName.PageRight)
		{
			return true;
		}
		if ((ref method) == MethodName.DayChangeHelper)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDayChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.Disable)
		{
			return true;
		}
		if ((ref method) == MethodName.Enable)
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._vfxLabel)
		{
			_vfxLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftArrow)
		{
			_leftArrow = VariantUtils.ConvertTo<NLeaderboardPageArrow>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightArrow)
		{
			_rightArrow = VariantUtils.ConvertTo<NLeaderboardPageArrow>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leaderboard)
		{
			_leaderboard = VariantUtils.ConvertTo<NDailyRunLeaderboard>(ref value);
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
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._vfxLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _vfxLabel);
			return true;
		}
		if ((ref name) == PropertyName._leftArrow)
		{
			value = VariantUtils.CreateFrom<NLeaderboardPageArrow>(ref _leftArrow);
			return true;
		}
		if ((ref name) == PropertyName._rightArrow)
		{
			value = VariantUtils.CreateFrom<NLeaderboardPageArrow>(ref _rightArrow);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _selectionReticle);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._leaderboard)
		{
			value = VariantUtils.CreateFrom<NDailyRunLeaderboard>(ref _leaderboard);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._vfxLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leftArrow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rightArrow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leaderboard, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._label, Variant.From<MegaLabel>(ref _label));
		info.AddProperty(PropertyName._vfxLabel, Variant.From<MegaLabel>(ref _vfxLabel));
		info.AddProperty(PropertyName._leftArrow, Variant.From<NLeaderboardPageArrow>(ref _leftArrow));
		info.AddProperty(PropertyName._rightArrow, Variant.From<NLeaderboardPageArrow>(ref _rightArrow));
		info.AddProperty(PropertyName._selectionReticle, Variant.From<NSelectionReticle>(ref _selectionReticle));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._leaderboard, Variant.From<NDailyRunLeaderboard>(ref _leaderboard));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val))
		{
			_label = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._vfxLabel, ref val2))
		{
			_vfxLabel = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftArrow, ref val3))
		{
			_leftArrow = ((Variant)(ref val3)).As<NLeaderboardPageArrow>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightArrow, ref val4))
		{
			_rightArrow = ((Variant)(ref val4)).As<NLeaderboardPageArrow>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionReticle, ref val5))
		{
			_selectionReticle = ((Variant)(ref val5)).As<NSelectionReticle>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val6))
		{
			_tween = ((Variant)(ref val6)).As<Tween>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._leaderboard, ref val7))
		{
			_leaderboard = ((Variant)(ref val7)).As<NDailyRunLeaderboard>();
		}
	}
}
