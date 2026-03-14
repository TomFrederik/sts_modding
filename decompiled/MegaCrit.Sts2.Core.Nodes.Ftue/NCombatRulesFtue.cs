using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Ftue;

[ScriptPath("res://src/Core/Nodes/Ftue/NCombatRulesFtue.cs")]
public class NCombatRulesFtue : NFtue
{
	public new class MethodName : NFtue.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Start = StringName.op_Implicit("Start");

		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName ToggleLeft = StringName.op_Implicit("ToggleLeft");

		public static readonly StringName ToggleRight = StringName.op_Implicit("ToggleRight");
	}

	public new class PropertyName : NFtue.PropertyName
	{
		public static readonly StringName _image1 = StringName.op_Implicit("_image1");

		public static readonly StringName _image2 = StringName.op_Implicit("_image2");

		public static readonly StringName _image3 = StringName.op_Implicit("_image3");

		public static readonly StringName _prevButton = StringName.op_Implicit("_prevButton");

		public static readonly StringName _nextButton = StringName.op_Implicit("_nextButton");

		public static readonly StringName _pageCount = StringName.op_Implicit("_pageCount");

		public static readonly StringName _image = StringName.op_Implicit("_image");

		public static readonly StringName _bodyText = StringName.op_Implicit("_bodyText");

		public static readonly StringName _header = StringName.op_Implicit("_header");

		public static readonly StringName _currentPage = StringName.op_Implicit("_currentPage");

		public static readonly StringName _imagePosition = StringName.op_Implicit("_imagePosition");

		public static readonly StringName _textPosition = StringName.op_Implicit("_textPosition");

		public static readonly StringName _pageTurnTween = StringName.op_Implicit("_pageTurnTween");
	}

	public new class SignalName : NFtue.SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private Texture2D _image1;

	[Export(/*Could not decode attribute arguments.*/)]
	private Texture2D _image2;

	[Export(/*Could not decode attribute arguments.*/)]
	private Texture2D _image3;

	public const string id = "combat_rules_ftue";

	private static readonly string _scenePath = SceneHelper.GetScenePath("ftue/combat_rules_ftue");

	private NButton _prevButton;

	private NButton _nextButton;

	private MegaLabel _pageCount;

	private TextureRect _image;

	private MegaRichTextLabel _bodyText;

	private MegaLabel _header;

	private int _currentPage = 1;

	private const int _totalPages = 3;

	private Vector2 _imagePosition;

	private Vector2 _textPosition;

	private Tween? _pageTurnTween;

	private const double _textTweenSpeed = 0.6;

	private static readonly Vector2 _imageAnimOffset = new Vector2(200f, 0f);

	public override void _Ready()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		_image = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Image"));
		_bodyText = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Description"));
		_pageCount = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("PageCount"));
		_header = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("Header"));
		_prevButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("LeftArrow"));
		_nextButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("RightArrow"));
		((CanvasItem)_image).Modulate = Colors.Transparent;
		((CanvasItem)_bodyText).Modulate = Colors.Transparent;
		((GodotObject)_prevButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ToggleLeft), 0u);
		((GodotObject)_nextButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ToggleRight), 0u);
		((CanvasItem)_prevButton).Visible = false;
		_prevButton.Disable();
		((CanvasItem)_nextButton).Visible = false;
		_nextButton.Disable();
		((CanvasItem)_pageCount).Visible = false;
		((CanvasItem)_header).Visible = false;
	}

	public void Start()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		NModalContainer.Instance?.ShowBackstop();
		_currentPage = 1;
		_imagePosition = ((Control)_image).Position;
		_bodyText.Text = new LocString("ftues", "TUTORIAL_FTUE_BODY_1").GetFormattedText();
		((CanvasItem)_bodyText).Modulate = StsColors.transparentWhite;
		_textPosition = ((Control)_bodyText).Position;
		LocString locString = new LocString("ftues", "COMBAT_BASICS_FTUE_PAGE_COUNT");
		locString.Add("totalPages", 3m);
		locString.Add("currentPage", _currentPage);
		_pageCount.SetTextAutoSize(locString.GetFormattedText());
		_header.SetTextAutoSize(new LocString("ftues", "COMBAT_BASICS_FTUE_HEADER").GetFormattedText());
		((CanvasItem)_nextButton).Visible = true;
		_nextButton.Enable();
		((CanvasItem)_pageCount).Visible = true;
		((CanvasItem)_header).Visible = true;
		_pageTurnTween = ((Node)this).CreateTween().SetParallel(true);
		_pageTurnTween.TweenProperty((GodotObject)(object)_image, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.From(Variant.op_Implicit(0f));
		_pageTurnTween.TweenProperty((GodotObject)(object)_bodyText, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)0);
		_pageTurnTween.TweenProperty((GodotObject)(object)_bodyText, NodePath.op_Implicit("visible_ratio"), Variant.op_Implicit(1f), 0.6).SetEase((EaseType)1).SetTrans((TransitionType)1)
			.From(Variant.op_Implicit(0f));
	}

	public static NCombatRulesFtue? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NCombatRulesFtue>((GenEditState)0);
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (!((CanvasItem)this).IsVisibleInTree() || ((CanvasItem)NDevConsole.Instance).Visible)
		{
			return;
		}
		Control val = ((Node)this).GetViewport().GuiGetFocusOwner();
		if ((!(val is TextEdit) && !(val is LineEdit)) || 1 == 0)
		{
			if (inputEvent.IsActionPressed(MegaInput.left, false, false) && _prevButton.IsEnabled)
			{
				ToggleLeft(_prevButton);
			}
			if (inputEvent.IsActionPressed(MegaInput.right, false, false) && _nextButton.IsEnabled)
			{
				ToggleRight(_nextButton);
			}
		}
	}

	private void ToggleLeft(NButton _)
	{
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		_currentPage--;
		switch (_currentPage)
		{
		case 1:
			((CanvasItem)_prevButton).Visible = false;
			_prevButton.Disable();
			_bodyText.SetTextAutoSize(new LocString("ftues", "TUTORIAL_FTUE_BODY_1").GetFormattedText());
			_image.Texture = _image1;
			break;
		case 2:
			_bodyText.SetTextAutoSize(new LocString("ftues", "TUTORIAL_FTUE_BODY_2").GetFormattedText());
			_image.Texture = _image2;
			break;
		}
		LocString locString = new LocString("ftues", "COMBAT_BASICS_FTUE_PAGE_COUNT");
		locString.Add("totalPages", 3m);
		locString.Add("currentPage", _currentPage);
		_pageCount.SetTextAutoSize(locString.GetFormattedText());
		Tween? pageTurnTween = _pageTurnTween;
		if (pageTurnTween != null)
		{
			pageTurnTween.Kill();
		}
		_pageTurnTween = ((Node)this).CreateTween().SetParallel(true);
		_pageTurnTween.TweenProperty((GodotObject)(object)_image, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.From(Variant.op_Implicit(0.5f));
		_pageTurnTween.TweenProperty((GodotObject)(object)_image, NodePath.op_Implicit("position"), Variant.op_Implicit(_imagePosition), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(_imagePosition - _imageAnimOffset));
		_pageTurnTween.TweenProperty((GodotObject)(object)_bodyText, NodePath.op_Implicit("position"), Variant.op_Implicit(_textPosition), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(_textPosition - _imageAnimOffset));
		_pageTurnTween.TweenProperty((GodotObject)(object)_bodyText, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.6).SetEase((EaseType)1).SetTrans((TransitionType)0)
			.From(Variant.op_Implicit(0f));
		_pageTurnTween.TweenProperty((GodotObject)(object)_bodyText, NodePath.op_Implicit("visible_ratio"), Variant.op_Implicit(1f), 0.6).SetEase((EaseType)1).SetTrans((TransitionType)1)
			.From(Variant.op_Implicit(0f));
	}

	private void ToggleRight(NButton _)
	{
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		if (_currentPage == 3)
		{
			Tween? pageTurnTween = _pageTurnTween;
			if (pageTurnTween != null)
			{
				pageTurnTween.Kill();
			}
			SaveManager.Instance.MarkFtueAsComplete("combat_rules_ftue");
			((Node)(object)NCombatRoom.Instance).AddChildSafely((Node?)(object)NCombatStartBanner.Create());
			CloseFtue();
			return;
		}
		_currentPage++;
		switch (_currentPage)
		{
		case 2:
			((CanvasItem)_prevButton).Visible = true;
			_prevButton.Enable();
			_bodyText.SetTextAutoSize(new LocString("ftues", "TUTORIAL_FTUE_BODY_2").GetFormattedText());
			_image.Texture = _image2;
			break;
		case 3:
			_bodyText.SetTextAutoSize(new LocString("ftues", "TUTORIAL_FTUE_BODY_3").GetFormattedText());
			_image.Texture = _image3;
			break;
		}
		LocString locString = new LocString("ftues", "COMBAT_BASICS_FTUE_PAGE_COUNT");
		locString.Add("totalPages", 3m);
		locString.Add("currentPage", _currentPage);
		_pageCount.SetTextAutoSize(locString.GetFormattedText());
		Tween? pageTurnTween2 = _pageTurnTween;
		if (pageTurnTween2 != null)
		{
			pageTurnTween2.Kill();
		}
		_pageTurnTween = ((Node)this).CreateTween().SetParallel(true);
		_pageTurnTween.TweenProperty((GodotObject)(object)_image, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.From(Variant.op_Implicit(0.5f));
		_pageTurnTween.TweenProperty((GodotObject)(object)_image, NodePath.op_Implicit("position"), Variant.op_Implicit(_imagePosition), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(_imagePosition + _imageAnimOffset));
		_pageTurnTween.TweenProperty((GodotObject)(object)_bodyText, NodePath.op_Implicit("position"), Variant.op_Implicit(_textPosition), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(_textPosition + _imageAnimOffset));
		_pageTurnTween.TweenProperty((GodotObject)(object)_bodyText, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.6).SetEase((EaseType)1).SetTrans((TransitionType)0)
			.From(Variant.op_Implicit(0f));
		_pageTurnTween.TweenProperty((GodotObject)(object)_bodyText, NodePath.op_Implicit("visible_ratio"), Variant.op_Implicit(1f), 0.6).SetEase((EaseType)1).SetTrans((TransitionType)1)
			.From(Variant.op_Implicit(0f));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Start, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleLeft, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleRight, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Start && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Start();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCombatRulesFtue nCombatRulesFtue = Create();
			ret = VariantUtils.CreateFrom<NCombatRulesFtue>(ref nCombatRulesFtue);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleLeft && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ToggleLeft(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleRight && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ToggleRight(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCombatRulesFtue nCombatRulesFtue = Create();
			ret = VariantUtils.CreateFrom<NCombatRulesFtue>(ref nCombatRulesFtue);
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
		if ((ref method) == MethodName.Start)
		{
			return true;
		}
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleLeft)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleRight)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._image1)
		{
			_image1 = VariantUtils.ConvertTo<Texture2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._image2)
		{
			_image2 = VariantUtils.ConvertTo<Texture2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._image3)
		{
			_image3 = VariantUtils.ConvertTo<Texture2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._prevButton)
		{
			_prevButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nextButton)
		{
			_nextButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pageCount)
		{
			_pageCount = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._image)
		{
			_image = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bodyText)
		{
			_bodyText = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._header)
		{
			_header = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentPage)
		{
			_currentPage = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._imagePosition)
		{
			_imagePosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._textPosition)
		{
			_textPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pageTurnTween)
		{
			_pageTurnTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._image1)
		{
			value = VariantUtils.CreateFrom<Texture2D>(ref _image1);
			return true;
		}
		if ((ref name) == PropertyName._image2)
		{
			value = VariantUtils.CreateFrom<Texture2D>(ref _image2);
			return true;
		}
		if ((ref name) == PropertyName._image3)
		{
			value = VariantUtils.CreateFrom<Texture2D>(ref _image3);
			return true;
		}
		if ((ref name) == PropertyName._prevButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _prevButton);
			return true;
		}
		if ((ref name) == PropertyName._nextButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _nextButton);
			return true;
		}
		if ((ref name) == PropertyName._pageCount)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _pageCount);
			return true;
		}
		if ((ref name) == PropertyName._image)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _image);
			return true;
		}
		if ((ref name) == PropertyName._bodyText)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _bodyText);
			return true;
		}
		if ((ref name) == PropertyName._header)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _header);
			return true;
		}
		if ((ref name) == PropertyName._currentPage)
		{
			value = VariantUtils.CreateFrom<int>(ref _currentPage);
			return true;
		}
		if ((ref name) == PropertyName._imagePosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _imagePosition);
			return true;
		}
		if ((ref name) == PropertyName._textPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _textPosition);
			return true;
		}
		if ((ref name) == PropertyName._pageTurnTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _pageTurnTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._image1, (PropertyHint)17, "Texture2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._image2, (PropertyHint)17, "Texture2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._image3, (PropertyHint)17, "Texture2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._prevButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._nextButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._pageCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._image, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bodyText, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._header, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._currentPage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._imagePosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._textPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._pageTurnTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._image1, Variant.From<Texture2D>(ref _image1));
		info.AddProperty(PropertyName._image2, Variant.From<Texture2D>(ref _image2));
		info.AddProperty(PropertyName._image3, Variant.From<Texture2D>(ref _image3));
		info.AddProperty(PropertyName._prevButton, Variant.From<NButton>(ref _prevButton));
		info.AddProperty(PropertyName._nextButton, Variant.From<NButton>(ref _nextButton));
		info.AddProperty(PropertyName._pageCount, Variant.From<MegaLabel>(ref _pageCount));
		info.AddProperty(PropertyName._image, Variant.From<TextureRect>(ref _image));
		info.AddProperty(PropertyName._bodyText, Variant.From<MegaRichTextLabel>(ref _bodyText));
		info.AddProperty(PropertyName._header, Variant.From<MegaLabel>(ref _header));
		info.AddProperty(PropertyName._currentPage, Variant.From<int>(ref _currentPage));
		info.AddProperty(PropertyName._imagePosition, Variant.From<Vector2>(ref _imagePosition));
		info.AddProperty(PropertyName._textPosition, Variant.From<Vector2>(ref _textPosition));
		info.AddProperty(PropertyName._pageTurnTween, Variant.From<Tween>(ref _pageTurnTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._image1, ref val))
		{
			_image1 = ((Variant)(ref val)).As<Texture2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._image2, ref val2))
		{
			_image2 = ((Variant)(ref val2)).As<Texture2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._image3, ref val3))
		{
			_image3 = ((Variant)(ref val3)).As<Texture2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._prevButton, ref val4))
		{
			_prevButton = ((Variant)(ref val4)).As<NButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._nextButton, ref val5))
		{
			_nextButton = ((Variant)(ref val5)).As<NButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._pageCount, ref val6))
		{
			_pageCount = ((Variant)(ref val6)).As<MegaLabel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._image, ref val7))
		{
			_image = ((Variant)(ref val7)).As<TextureRect>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._bodyText, ref val8))
		{
			_bodyText = ((Variant)(ref val8)).As<MegaRichTextLabel>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._header, ref val9))
		{
			_header = ((Variant)(ref val9)).As<MegaLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentPage, ref val10))
		{
			_currentPage = ((Variant)(ref val10)).As<int>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._imagePosition, ref val11))
		{
			_imagePosition = ((Variant)(ref val11)).As<Vector2>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._textPosition, ref val12))
		{
			_textPosition = ((Variant)(ref val12)).As<Vector2>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._pageTurnTween, ref val13))
		{
			_pageTurnTween = ((Variant)(ref val13)).As<Tween>();
		}
	}
}
