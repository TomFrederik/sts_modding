using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NCombatCardPile.cs")]
public abstract class NCombatCardPile : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName ConnectSignals = StringName.op_Implicit("ConnectSignals");

		public new static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName SetAnimInOutPositions = StringName.op_Implicit("SetAnimInOutPositions");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public static readonly StringName AddCard = StringName.op_Implicit("AddCard");

		public static readonly StringName RemoveCard = StringName.op_Implicit("RemoveCard");

		public static readonly StringName AnimIn = StringName.op_Implicit("AnimIn");

		public static readonly StringName AnimOut = StringName.op_Implicit("AnimOut");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName Pile = StringName.op_Implicit("Pile");

		public static readonly StringName _countLabel = StringName.op_Implicit("_countLabel");

		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _bumpTween = StringName.op_Implicit("_bumpTween");

		public static readonly StringName _currentCount = StringName.op_Implicit("_currentCount");

		public static readonly StringName _positionTween = StringName.op_Implicit("_positionTween");

		public static readonly StringName _showPosition = StringName.op_Implicit("_showPosition");

		public static readonly StringName _hidePosition = StringName.op_Implicit("_hidePosition");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private CardPile? _pile;

	private Player? _localPlayer;

	private MegaLabel _countLabel;

	private Control _icon;

	private HoverTip _hoverTip;

	protected LocString _emptyPileMessage;

	private Tween? _bumpTween;

	private int _currentCount;

	private static readonly Vector2 _hoverScale = Vector2.One * 1.25f;

	private const double _unhoverAnimDur = 0.5;

	private const double _pressDownDur = 0.25;

	private static readonly Color _downColor = Colors.DarkGray;

	private Tween? _positionTween;

	private const double _animDuration = 0.5;

	protected Vector2 _showPosition = new Vector2(100f, 828f);

	protected Vector2 _hidePosition = new Vector2(-160f, 860f);

	protected abstract PileType Pile { get; }

	public override void _Ready()
	{
		if (((object)this).GetType() != typeof(NCombatCardPile))
		{
			Log.Error($"{((object)this).GetType()}");
			throw new InvalidOperationException("Don't call base._Ready()! Call ConnectSignals() instead.");
		}
		ConnectSignals();
	}

	protected override void ConnectSignals()
	{
		base.ConnectSignals();
		_icon = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Icon"));
		_countLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("CountContainer/Count"));
		SetAnimInOutPositions();
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		if (_pile != null)
		{
			_pile.CardAddFinished -= AddCard;
			_pile.CardAddFinished += AddCard;
			_pile.CardRemoveFinished -= RemoveCard;
			_pile.CardRemoveFinished += RemoveCard;
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (_pile != null)
		{
			_pile.CardAddFinished -= AddCard;
			_pile.CardRemoveFinished -= RemoveCard;
		}
		Tween? positionTween = _positionTween;
		if (positionTween != null)
		{
			positionTween.Kill();
		}
		Tween? bumpTween = _bumpTween;
		if (bumpTween != null)
		{
			bumpTween.Kill();
		}
	}

	public virtual void Initialize(Player player)
	{
		_localPlayer = player;
		_pile = Pile.GetPile(_localPlayer);
		_pile.CardAddFinished += AddCard;
		_pile.CardRemoveFinished += RemoveCard;
		_currentCount = _pile.Cards.Count;
		_countLabel.SetTextAutoSize(_currentCount.ToString());
		_hoverTip = _pile.Type switch
		{
			PileType.Draw => new HoverTip(new LocString("static_hover_tips", "DRAW_PILE.title"), new LocString("static_hover_tips", "DRAW_PILE.description")), 
			PileType.Discard => new HoverTip(new LocString("static_hover_tips", "DISCARD_PILE.title"), new LocString("static_hover_tips", "DISCARD_PILE.description")), 
			PileType.Exhaust => new HoverTip(new LocString("static_hover_tips", "EXHAUST_PILE.title"), new LocString("static_hover_tips", "EXHAUST_PILE.description")), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	protected virtual void SetAnimInOutPositions()
	{
	}

	protected override void OnRelease()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Tween? bumpTween = _bumpTween;
		if (bumpTween != null)
		{
			bumpTween.Kill();
		}
		_bumpTween = ((Node)this).CreateTween();
		_bumpTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(base.IsFocused ? _hoverScale : Vector2.One), 0.05);
		_bumpTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		if (_pile == null || _localPlayer == null || !CombatManager.Instance.IsInProgress)
		{
			return;
		}
		if (_pile.IsEmpty)
		{
			NCapstoneContainer? instance = NCapstoneContainer.Instance;
			if (instance != null && instance.InUse)
			{
				NCapstoneContainer.Instance.Close();
			}
			NThoughtBubbleVfx child = NThoughtBubbleVfx.Create(_emptyPileMessage.GetFormattedText(), _localPlayer.Creature, 2.0);
			((Node)(object)NCombatRoom.Instance?.CombatVfxContainer).AddChildSafely((Node?)(object)child);
		}
		else if (NCapstoneContainer.Instance?.CurrentCapstoneScreen is NCardPileScreen nCardPileScreen && nCardPileScreen.Pile == _pile)
		{
			NCapstoneContainer.Instance.Close();
		}
		else
		{
			NCardPileScreen.ShowScreen(_pile, Hotkeys);
		}
	}

	protected override void OnFocus()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		if (_pile != null)
		{
			NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _hoverTip);
			((Control)nHoverTipSet).GlobalPosition = (Vector2)(_pile.Type switch
			{
				PileType.Draw => ((Control)this).GlobalPosition + new Vector2(14f, -375f), 
				PileType.Discard => ((Control)this).GlobalPosition + new Vector2(-320f, -370f), 
				PileType.Exhaust => ((Control)this).GlobalPosition + new Vector2(-320f, -125f), 
				_ => ((Control)NHoverTipSet.CreateAndShow((Control)(object)this, _hoverTip)).GlobalPosition, 
			});
			Tween? bumpTween = _bumpTween;
			if (bumpTween != null)
			{
				bumpTween.Kill();
			}
			_bumpTween = ((Node)this).CreateTween();
			_bumpTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(_hoverScale), 0.05);
		}
	}

	protected override void OnUnfocus()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		NHoverTipSet.Remove((Control)(object)this);
		Tween? bumpTween = _bumpTween;
		if (bumpTween != null)
		{
			bumpTween.Kill();
		}
		_bumpTween = ((Node)this).CreateTween().SetParallel(true);
		_bumpTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_bumpTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	protected override void OnPress()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		Tween? bumpTween = _bumpTween;
		if (bumpTween != null)
		{
			bumpTween.Kill();
		}
		_bumpTween = ((Node)this).CreateTween().SetParallel(true);
		_bumpTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_bumpTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("modulate"), Variant.op_Implicit(_downColor), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	protected virtual void AddCard()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (_pile != null)
		{
			_currentCount = Math.Min(_currentCount + 1, _pile.Cards.Count);
			_countLabel.SetTextAutoSize(_currentCount.ToString());
			((Control)_countLabel).PivotOffset = ((Control)_countLabel).Size * 0.5f;
			Tween? bumpTween = _bumpTween;
			if (bumpTween != null)
			{
				bumpTween.Kill();
			}
			_bumpTween = ((Node)this).CreateTween().SetParallel(true);
			_icon.Scale = _hoverScale;
			_bumpTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			((Control)_countLabel).Scale = _hoverScale;
			_bumpTween.TweenProperty((GodotObject)(object)_countLabel, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		}
	}

	private void RemoveCard()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (_pile != null)
		{
			_currentCount = Math.Max(_currentCount - 1, _pile.Cards.Count);
			_countLabel.SetTextAutoSize(_currentCount.ToString());
			((Control)_countLabel).PivotOffset = ((Control)_countLabel).Size * 0.5f;
		}
	}

	public virtual void AnimIn()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Position = _hidePosition;
		Tween? positionTween = _positionTween;
		if (positionTween != null)
		{
			positionTween.Kill();
		}
		_positionTween = ((Node)this).CreateTween();
		_positionTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_showPosition), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	public void AnimOut()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Position = _showPosition;
		Tween? positionTween = _positionTween;
		if (positionTween != null)
		{
			positionTween.Kill();
		}
		_positionTween = ((Node)this).CreateTween();
		_positionTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_hidePosition), 0.5).SetEase((EaseType)0).SetTrans((TransitionType)10);
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
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAnimInOutPositions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimOut, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAnimInOutPositions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetAnimInOutPositions();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
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
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddCard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AddCard();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemoveCard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RemoveCard();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimOut && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimOut();
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
		if ((ref method) == MethodName.ConnectSignals)
		{
			return true;
		}
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAnimInOutPositions)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
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
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName.AddCard)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveCard)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimIn)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimOut)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._countLabel)
		{
			_countLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bumpTween)
		{
			_bumpTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentCount)
		{
			_currentCount = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._positionTween)
		{
			_positionTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._showPosition)
		{
			_showPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hidePosition)
		{
			_hidePosition = VariantUtils.ConvertTo<Vector2>(ref value);
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
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Pile)
		{
			PileType pile = Pile;
			value = VariantUtils.CreateFrom<PileType>(ref pile);
			return true;
		}
		if ((ref name) == PropertyName._countLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _countLabel);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<Control>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._bumpTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _bumpTween);
			return true;
		}
		if ((ref name) == PropertyName._currentCount)
		{
			value = VariantUtils.CreateFrom<int>(ref _currentCount);
			return true;
		}
		if ((ref name) == PropertyName._positionTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _positionTween);
			return true;
		}
		if ((ref name) == PropertyName._showPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _showPosition);
			return true;
		}
		if ((ref name) == PropertyName._hidePosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _hidePosition);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName.Pile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._countLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bumpTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._currentCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._positionTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._showPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._hidePosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._countLabel, Variant.From<MegaLabel>(ref _countLabel));
		info.AddProperty(PropertyName._icon, Variant.From<Control>(ref _icon));
		info.AddProperty(PropertyName._bumpTween, Variant.From<Tween>(ref _bumpTween));
		info.AddProperty(PropertyName._currentCount, Variant.From<int>(ref _currentCount));
		info.AddProperty(PropertyName._positionTween, Variant.From<Tween>(ref _positionTween));
		info.AddProperty(PropertyName._showPosition, Variant.From<Vector2>(ref _showPosition));
		info.AddProperty(PropertyName._hidePosition, Variant.From<Vector2>(ref _hidePosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._countLabel, ref val))
		{
			_countLabel = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val2))
		{
			_icon = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._bumpTween, ref val3))
		{
			_bumpTween = ((Variant)(ref val3)).As<Tween>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentCount, ref val4))
		{
			_currentCount = ((Variant)(ref val4)).As<int>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._positionTween, ref val5))
		{
			_positionTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._showPosition, ref val6))
		{
			_showPosition = ((Variant)(ref val6)).As<Vector2>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._hidePosition, ref val7))
		{
			_hidePosition = ((Variant)(ref val7)).As<Vector2>();
		}
	}
}
