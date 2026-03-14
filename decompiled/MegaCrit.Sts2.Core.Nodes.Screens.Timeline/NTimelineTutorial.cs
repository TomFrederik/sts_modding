using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/NTimelineTutorial.cs")]
public class NTimelineTutorial : Control, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName Init = StringName.op_Implicit("Init");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName CloseTutorial = StringName.op_Implicit("CloseTutorial");

		public static readonly StringName AnimateTutorial = StringName.op_Implicit("AnimateTutorial");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _text = StringName.op_Implicit("_text");

		public static readonly StringName _acknowledgeButton = StringName.op_Implicit("_acknowledgeButton");

		public static readonly StringName _timeline = StringName.op_Implicit("_timeline");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private MegaRichTextLabel _text;

	private NAcknowledgeButton _acknowledgeButton;

	private NTimelineScreen _timeline;

	private Tween? _tween;

	public Control? DefaultFocusedControl => null;

	public void Init(NTimelineScreen screen)
	{
		_timeline = screen;
		screen.HideBackButtonImmediately();
	}

	public override void _Ready()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		SfxCmd.Play("event:/sfx/ui/timeline/ui_timeline_unlock");
		_text = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%TutorialText"));
		_text.Text = "[center]" + new LocString("timeline", "TUTORIAL_TEXT").GetRawText() + "[/center]";
		_acknowledgeButton = ((Node)this).GetNode<NAcknowledgeButton>(NodePath.op_Implicit("%AcknowledgeButton"));
		((GodotObject)_acknowledgeButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)CloseTutorial), 0u);
		AnimateTutorial();
	}

	private void CloseTutorial(NButton _)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		_acknowledgeButton.Disable();
		_tween?.FastForwardToCompletion();
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
		_tween.Chain().TweenCallback(Callable.From((Action)delegate
		{
			TaskHelper.RunSafely(_timeline.SpawnFirstTimeTimeline());
			((Node)(object)this).QueueFreeSafely();
		}));
	}

	private void AnimateTutorial()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		_acknowledgeButton.Disable();
		((RichTextLabel)_text).VisibleRatio = 0f;
		MegaRichTextLabel text = _text;
		Color modulate = ((CanvasItem)_text).Modulate;
		modulate.A = 0f;
		((CanvasItem)text).Modulate = modulate;
		_tween?.FastForwardToCompletion();
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)_text, NodePath.op_Implicit("visible_ratio"), Variant.op_Implicit(1f), 2.0).SetEase((EaseType)1).SetTrans((TransitionType)4);
		_tween.Parallel().TweenProperty((GodotObject)(object)_text, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.0);
		_tween.Chain().TweenCallback(Callable.From((Action)delegate
		{
			_acknowledgeButton.Enable();
		}));
		_tween.Parallel().TweenProperty((GodotObject)(object)_acknowledgeButton, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.3).SetEase((EaseType)1)
			.SetTrans((TransitionType)7)
			.SetDelay(1.0);
		_tween.Parallel().TweenProperty((GodotObject)(object)_acknowledgeButton, NodePath.op_Implicit("position:y"), Variant.op_Implicit(920f), 0.3).SetEase((EaseType)1)
			.SetTrans((TransitionType)10)
			.SetDelay(1.0);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName.Init, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("screen"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CloseTutorial, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateTutorial, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Init && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Init(VariantUtils.ConvertTo<NTimelineScreen>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CloseTutorial && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CloseTutorial(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateTutorial && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateTutorial();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Init)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.CloseTutorial)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateTutorial)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._text)
		{
			_text = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._acknowledgeButton)
		{
			_acknowledgeButton = VariantUtils.ConvertTo<NAcknowledgeButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._timeline)
		{
			_timeline = VariantUtils.ConvertTo<NTimelineScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
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
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._text)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _text);
			return true;
		}
		if ((ref name) == PropertyName._acknowledgeButton)
		{
			value = VariantUtils.CreateFrom<NAcknowledgeButton>(ref _acknowledgeButton);
			return true;
		}
		if ((ref name) == PropertyName._timeline)
		{
			value = VariantUtils.CreateFrom<NTimelineScreen>(ref _timeline);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._text, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._acknowledgeButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._timeline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._text, Variant.From<MegaRichTextLabel>(ref _text));
		info.AddProperty(PropertyName._acknowledgeButton, Variant.From<NAcknowledgeButton>(ref _acknowledgeButton));
		info.AddProperty(PropertyName._timeline, Variant.From<NTimelineScreen>(ref _timeline));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._text, ref val))
		{
			_text = ((Variant)(ref val)).As<MegaRichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._acknowledgeButton, ref val2))
		{
			_acknowledgeButton = ((Variant)(ref val2)).As<NAcknowledgeButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._timeline, ref val3))
		{
			_timeline = ((Variant)(ref val3)).As<NTimelineScreen>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val4))
		{
			_tween = ((Variant)(ref val4)).As<Tween>();
		}
	}
}
