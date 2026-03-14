using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace MegaCrit.Sts2.Core.Nodes.Cards.Holders;

[ScriptPath("res://src/Core/Nodes/Cards/Holders/NCardHolder.cs")]
public abstract class NCardHolder : Control
{
	[Signal]
	public delegate void PressedEventHandler(NCardHolder cardHolder);

	[Signal]
	public delegate void AltPressedEventHandler(NCardHolder cardHolder);

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetClickable = StringName.op_Implicit("SetClickable");

		public static readonly StringName ConnectSignals = StringName.op_Implicit("ConnectSignals");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName SetCard = StringName.op_Implicit("SetCard");

		public static readonly StringName OnCardReassigned = StringName.op_Implicit("OnCardReassigned");

		public static readonly StringName OnMousePressed = StringName.op_Implicit("OnMousePressed");

		public static readonly StringName OnMouseReleased = StringName.op_Implicit("OnMouseReleased");

		public static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName CreateHoverTips = StringName.op_Implicit("CreateHoverTips");

		public static readonly StringName ClearHoverTips = StringName.op_Implicit("ClearHoverTips");

		public static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName RefreshFocusState = StringName.op_Implicit("RefreshFocusState");

		public static readonly StringName DoCardHoverEffects = StringName.op_Implicit("DoCardHoverEffects");

		public static readonly StringName OnChildExitingTree = StringName.op_Implicit("OnChildExitingTree");

		public static readonly StringName Clear = StringName.op_Implicit("Clear");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName HoverScale = StringName.op_Implicit("HoverScale");

		public static readonly StringName SmallScale = StringName.op_Implicit("SmallScale");

		public static readonly StringName Hitbox = StringName.op_Implicit("Hitbox");

		public static readonly StringName CardNode = StringName.op_Implicit("CardNode");

		public static readonly StringName IsShowingUpgradedCard = StringName.op_Implicit("IsShowingUpgradedCard");

		public static readonly StringName CanBeFocused = StringName.op_Implicit("CanBeFocused");

		public static readonly StringName _hitbox = StringName.op_Implicit("_hitbox");

		public static readonly StringName _isHovered = StringName.op_Implicit("_isHovered");

		public static readonly StringName _isFocused = StringName.op_Implicit("_isFocused");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _currentPressedAction = StringName.op_Implicit("_currentPressedAction");

		public static readonly StringName _isClickable = StringName.op_Implicit("_isClickable");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName Pressed = StringName.op_Implicit("Pressed");

		public static readonly StringName AltPressed = StringName.op_Implicit("AltPressed");
	}

	public static readonly Vector2 smallScale = Vector2.One * 0.8f;

	protected NClickableControl _hitbox;

	protected bool _isHovered;

	protected bool _isFocused;

	protected Tween? _hoverTween;

	private InputEventMouseButton? _currentPressedAction;

	protected bool _isClickable = true;

	private PressedEventHandler backing_Pressed;

	private AltPressedEventHandler backing_AltPressed;

	protected virtual Vector2 HoverScale => Vector2.One;

	public virtual Vector2 SmallScale => smallScale;

	public NClickableControl Hitbox => _hitbox;

	public NCard? CardNode { get; protected set; }

	public virtual CardModel? CardModel => CardNode?.Model;

	public virtual bool IsShowingUpgradedCard => CardModel?.IsUpgraded ?? false;

	protected bool CanBeFocused => _isHovered;

	public event PressedEventHandler Pressed
	{
		add
		{
			backing_Pressed = (PressedEventHandler)Delegate.Combine(backing_Pressed, value);
		}
		remove
		{
			backing_Pressed = (PressedEventHandler)Delegate.Remove(backing_Pressed, value);
		}
	}

	public event AltPressedEventHandler AltPressed
	{
		add
		{
			backing_AltPressed = (AltPressedEventHandler)Delegate.Combine(backing_AltPressed, value);
		}
		remove
		{
			backing_AltPressed = (AltPressedEventHandler)Delegate.Remove(backing_AltPressed, value);
		}
	}

	public override void _Ready()
	{
		if (((object)this).GetType() != typeof(NCardHolder))
		{
			Log.Error($"{((object)this).GetType()}");
			throw new InvalidOperationException("Don't call base._Ready()! Call ConnectSignals() instead.");
		}
		ConnectSignals();
	}

	public void SetClickable(bool isClickable)
	{
		_isClickable = isClickable;
	}

	protected void ConnectSignals()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (CardNode != null)
		{
			((Control)CardNode).Position = Vector2.Zero;
		}
		((GodotObject)this).Connect(SignalName.FocusEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)this).Connect(SignalName.FocusExited, Callable.From((Action)OnUnfocus), 0u);
		_hitbox = ((Node)this).GetNode<NClickableControl>(NodePath.op_Implicit("%Hitbox"));
		((GodotObject)_hitbox).Connect(NClickableControl.SignalName.Focused, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
		{
			OnFocus();
		}), 0u);
		((GodotObject)_hitbox).Connect(NClickableControl.SignalName.Unfocused, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
		{
			OnUnfocus();
		}), 0u);
		((GodotObject)_hitbox).Connect(NClickableControl.SignalName.MousePressed, Callable.From<InputEvent>((Action<InputEvent>)OnMousePressed), 0u);
		((GodotObject)_hitbox).Connect(NClickableControl.SignalName.MouseReleased, Callable.From<InputEvent>((Action<InputEvent>)OnMouseReleased), 0u);
		((GodotObject)this).Connect(SignalName.ChildExitingTree, Callable.From<Node>((Action<Node>)OnChildExitingTree), 0u);
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		((Control)this)._GuiInput(inputEvent);
		if (_isClickable && CardNode != null)
		{
			if (inputEvent.IsActionPressed(MegaInput.select, false, false))
			{
				SfxCmd.Play("event:/sfx/ui/clicks/ui_click");
				((GodotObject)this).EmitSignal(SignalName.Pressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
			}
			else if (inputEvent.IsActionPressed(MegaInput.accept, false, false))
			{
				SfxCmd.Play("event:/sfx/ui/clicks/ui_click");
				((GodotObject)this).EmitSignal(SignalName.AltPressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
			}
		}
	}

	protected virtual void SetCard(NCard node)
	{
		if (CardNode != null)
		{
			throw new InvalidOperationException("Cannot set a card node on a holder that already has one.");
		}
		CardNode = node;
		if (((Node)CardNode).GetParent() == null)
		{
			((Node)(object)this).AddChildSafely((Node?)(object)node);
		}
		else
		{
			((Node)node).Reparent((Node)(object)this, true);
		}
	}

	public void ReassignToCard(CardModel cardModel, PileType pileType, Creature? target, ModelVisibility visibility)
	{
		CardNode.Visibility = visibility;
		CardNode.Model = cardModel;
		CardNode.SetPreviewTarget(target);
		CardNode.UpdateVisuals(pileType, CardPreviewMode.Normal);
		OnCardReassigned();
	}

	protected virtual void OnCardReassigned()
	{
	}

	protected virtual void OnMousePressed(InputEvent inputEvent)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I8
		if (_currentPressedAction != null)
		{
			return;
		}
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null && _isClickable)
		{
			MouseButton buttonIndex = val.ButtonIndex;
			if (((long)(buttonIndex - 1) <= 1L) ? true : false)
			{
				SfxCmd.Play("event:/sfx/ui/clicks/ui_click");
			}
			_currentPressedAction = val;
		}
	}

	protected virtual void OnMouseReleased(InputEvent inputEvent)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Invalid comparison between Unknown and I8
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (CardNode == null || !_isHovered || _currentPressedAction == null)
		{
			return;
		}
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null && _isClickable)
		{
			if (val.ButtonIndex != _currentPressedAction.ButtonIndex)
			{
				return;
			}
			if ((long)val.ButtonIndex == 1)
			{
				((GodotObject)this).EmitSignal(SignalName.Pressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
			}
			else
			{
				((GodotObject)this).EmitSignal(SignalName.AltPressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
			}
		}
		_currentPressedAction = null;
	}

	protected virtual void OnFocus()
	{
		_isHovered = true;
		RefreshFocusState();
	}

	protected virtual void CreateHoverTips()
	{
		if (CardNode != null)
		{
			NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, CardNode.Model.HoverTips);
			nHoverTipSet.SetAlignmentForCardHolder(this);
		}
	}

	protected void ClearHoverTips()
	{
		NHoverTipSet.Remove((Control)(object)this);
	}

	protected virtual void OnUnfocus()
	{
		_isHovered = false;
		_currentPressedAction = null;
		RefreshFocusState();
	}

	protected void RefreshFocusState()
	{
		if (_isFocused != CanBeFocused)
		{
			_isFocused = CanBeFocused;
			DoCardHoverEffects(_isFocused);
		}
	}

	protected virtual void DoCardHoverEffects(bool isHovered)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (isHovered)
		{
			Tween? hoverTween = _hoverTween;
			if (hoverTween != null)
			{
				hoverTween.Kill();
			}
			((Control)this).Scale = HoverScale;
			if (CardNode.Visibility == ModelVisibility.Visible)
			{
				CreateHoverTips();
			}
		}
		else if (!isHovered)
		{
			Tween? hoverTween2 = _hoverTween;
			if (hoverTween2 != null)
			{
				hoverTween2.Kill();
			}
			_hoverTween = ((Node)this).CreateTween();
			_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(SmallScale), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			ClearHoverTips();
		}
	}

	private void OnChildExitingTree(Node node)
	{
		if ((object)node == CardNode && (object)node.GetParent() != this)
		{
			ClearHoverTips();
			CardNode = null;
		}
	}

	public virtual void Clear()
	{
		if (CardNode != null)
		{
			if ((object)((Node)CardNode).GetParent() == this)
			{
				((Node)(object)this).RemoveChildSafely((Node?)(object)CardNode);
			}
			CardNode = null;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Expected O, but got Unknown
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Expected O, but got Unknown
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Expected O, but got Unknown
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(16);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetClickable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isClickable"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCardReassigned, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMousePressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMouseReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateHoverTips, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearHoverTips, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshFocusState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DoCardHoverEffects, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isHovered"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnChildExitingTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Clear, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetClickable && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetClickable(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCard && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCard(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCardReassigned && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCardReassigned();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMousePressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMousePressed(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMouseReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMouseReleased(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CreateHoverTips && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CreateHoverTips();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearHoverTips && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearHoverTips();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshFocusState && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshFocusState();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DoCardHoverEffects && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DoCardHoverEffects(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnChildExitingTree && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnChildExitingTree(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Clear && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Clear();
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
		if ((ref method) == MethodName.SetClickable)
		{
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName.SetCard)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCardReassigned)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMousePressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMouseReleased)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.CreateHoverTips)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearHoverTips)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshFocusState)
		{
			return true;
		}
		if ((ref method) == MethodName.DoCardHoverEffects)
		{
			return true;
		}
		if ((ref method) == MethodName.OnChildExitingTree)
		{
			return true;
		}
		if ((ref method) == MethodName.Clear)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.CardNode)
		{
			CardNode = VariantUtils.ConvertTo<NCard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hitbox)
		{
			_hitbox = VariantUtils.ConvertTo<NClickableControl>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isHovered)
		{
			_isHovered = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isFocused)
		{
			_isFocused = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentPressedAction)
		{
			_currentPressedAction = VariantUtils.ConvertTo<InputEventMouseButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isClickable)
		{
			_isClickable = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.HoverScale)
		{
			Vector2 hoverScale = HoverScale;
			value = VariantUtils.CreateFrom<Vector2>(ref hoverScale);
			return true;
		}
		if ((ref name) == PropertyName.SmallScale)
		{
			Vector2 hoverScale = SmallScale;
			value = VariantUtils.CreateFrom<Vector2>(ref hoverScale);
			return true;
		}
		if ((ref name) == PropertyName.Hitbox)
		{
			NClickableControl hitbox = Hitbox;
			value = VariantUtils.CreateFrom<NClickableControl>(ref hitbox);
			return true;
		}
		if ((ref name) == PropertyName.CardNode)
		{
			NCard cardNode = CardNode;
			value = VariantUtils.CreateFrom<NCard>(ref cardNode);
			return true;
		}
		if ((ref name) == PropertyName.IsShowingUpgradedCard)
		{
			bool isShowingUpgradedCard = IsShowingUpgradedCard;
			value = VariantUtils.CreateFrom<bool>(ref isShowingUpgradedCard);
			return true;
		}
		if ((ref name) == PropertyName.CanBeFocused)
		{
			bool isShowingUpgradedCard = CanBeFocused;
			value = VariantUtils.CreateFrom<bool>(ref isShowingUpgradedCard);
			return true;
		}
		if ((ref name) == PropertyName._hitbox)
		{
			value = VariantUtils.CreateFrom<NClickableControl>(ref _hitbox);
			return true;
		}
		if ((ref name) == PropertyName._isHovered)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isHovered);
			return true;
		}
		if ((ref name) == PropertyName._isFocused)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isFocused);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._currentPressedAction)
		{
			value = VariantUtils.CreateFrom<InputEventMouseButton>(ref _currentPressedAction);
			return true;
		}
		if ((ref name) == PropertyName._isClickable)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isClickable);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)5, PropertyName.HoverScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.SmallScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Hitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CardNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsShowingUpgradedCard, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.CanBeFocused, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isHovered, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isFocused, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentPressedAction, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isClickable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName cardNode = PropertyName.CardNode;
		NCard cardNode2 = CardNode;
		info.AddProperty(cardNode, Variant.From<NCard>(ref cardNode2));
		info.AddProperty(PropertyName._hitbox, Variant.From<NClickableControl>(ref _hitbox));
		info.AddProperty(PropertyName._isHovered, Variant.From<bool>(ref _isHovered));
		info.AddProperty(PropertyName._isFocused, Variant.From<bool>(ref _isFocused));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._currentPressedAction, Variant.From<InputEventMouseButton>(ref _currentPressedAction));
		info.AddProperty(PropertyName._isClickable, Variant.From<bool>(ref _isClickable));
		info.AddSignalEventDelegate(SignalName.Pressed, (Delegate)backing_Pressed);
		info.AddSignalEventDelegate(SignalName.AltPressed, (Delegate)backing_AltPressed);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.CardNode, ref val))
		{
			CardNode = ((Variant)(ref val)).As<NCard>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._hitbox, ref val2))
		{
			_hitbox = ((Variant)(ref val2)).As<NClickableControl>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._isHovered, ref val3))
		{
			_isHovered = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._isFocused, ref val4))
		{
			_isFocused = ((Variant)(ref val4)).As<bool>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val5))
		{
			_hoverTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentPressedAction, ref val6))
		{
			_currentPressedAction = ((Variant)(ref val6)).As<InputEventMouseButton>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isClickable, ref val7))
		{
			_isClickable = ((Variant)(ref val7)).As<bool>();
		}
		PressedEventHandler pressedEventHandler = default(PressedEventHandler);
		if (info.TryGetSignalEventDelegate<PressedEventHandler>(SignalName.Pressed, ref pressedEventHandler))
		{
			backing_Pressed = pressedEventHandler;
		}
		AltPressedEventHandler altPressedEventHandler = default(AltPressedEventHandler);
		if (info.TryGetSignalEventDelegate<AltPressedEventHandler>(SignalName.AltPressed, ref altPressedEventHandler))
		{
			backing_AltPressed = altPressedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(SignalName.Pressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.AltPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalPressed(NCardHolder cardHolder)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Pressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)cardHolder) });
	}

	protected void EmitSignalAltPressed(NCardHolder cardHolder)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.AltPressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)cardHolder) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Pressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Pressed?.Invoke(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.AltPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_AltPressed?.Invoke(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Pressed)
		{
			return true;
		}
		if ((ref signal) == SignalName.AltPressed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
