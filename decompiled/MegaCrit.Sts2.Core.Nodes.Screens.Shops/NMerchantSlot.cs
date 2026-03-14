using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Shops;

[ScriptPath("res://src/Core/Nodes/Screens/Shops/NMerchantSlot.cs")]
public abstract class NMerchantSlot : Control
{
	[Signal]
	public delegate void HoveredEventHandler(NMerchantSlot slot);

	[Signal]
	public delegate void UnhoveredEventHandler(NMerchantSlot slot);

	public class MethodName : MethodName
	{
		public static readonly StringName Initialize = StringName.op_Implicit("Initialize");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ConnectSignals = StringName.op_Implicit("ConnectSignals");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName OnMousePressed = StringName.op_Implicit("OnMousePressed");

		public static readonly StringName OnMouseReleased = StringName.op_Implicit("OnMouseReleased");

		public static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName TriggerMerchantHandToPointHere = StringName.op_Implicit("TriggerMerchantHandToPointHere");

		public static readonly StringName OnPreview = StringName.op_Implicit("OnPreview");

		public static readonly StringName CreateHoverTip = StringName.op_Implicit("CreateHoverTip");

		public static readonly StringName ClearHoverTip = StringName.op_Implicit("ClearHoverTip");

		public static readonly StringName OnMerchantHandHovered = StringName.op_Implicit("OnMerchantHandHovered");

		public static readonly StringName OnMerchantHandUnhovered = StringName.op_Implicit("OnMerchantHandUnhovered");

		public static readonly StringName OnPurchaseFailed = StringName.op_Implicit("OnPurchaseFailed");

		public static readonly StringName UpdateVisual = StringName.op_Implicit("UpdateVisual");

		public static readonly StringName WiggleAnimation = StringName.op_Implicit("WiggleAnimation");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Hitbox = StringName.op_Implicit("Hitbox");

		public static readonly StringName Visual = StringName.op_Implicit("Visual");

		public static readonly StringName _isHovered = StringName.op_Implicit("_isHovered");

		public static readonly StringName _hitbox = StringName.op_Implicit("_hitbox");

		public static readonly StringName _costLabel = StringName.op_Implicit("_costLabel");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _purchaseFailedTween = StringName.op_Implicit("_purchaseFailedTween");

		public static readonly StringName _merchantRug = StringName.op_Implicit("_merchantRug");

		public static readonly StringName _ignoreMouseRelease = StringName.op_Implicit("_ignoreMouseRelease");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName Hovered = StringName.op_Implicit("Hovered");

		public static readonly StringName Unhovered = StringName.op_Implicit("Unhovered");
	}

	private bool _isHovered;

	private static readonly Vector2 _hoverScale = Vector2.One * 0.8f;

	private static readonly Vector2 _smallScale = Vector2.One * 0.65f;

	protected NClickableControl _hitbox;

	protected MegaLabel _costLabel;

	private Tween? _hoverTween;

	private Tween? _purchaseFailedTween;

	private NMerchantInventory? _merchantRug;

	private bool _ignoreMouseRelease;

	private float? _originalVisualPosition;

	private HoveredEventHandler backing_Hovered;

	private UnhoveredEventHandler backing_Unhovered;

	public NClickableControl Hitbox => _hitbox;

	public abstract MerchantEntry Entry { get; }

	protected abstract CanvasItem Visual { get; }

	protected Player? Player => _merchantRug?.Inventory?.Player;

	public event HoveredEventHandler Hovered
	{
		add
		{
			backing_Hovered = (HoveredEventHandler)Delegate.Combine(backing_Hovered, value);
		}
		remove
		{
			backing_Hovered = (HoveredEventHandler)Delegate.Remove(backing_Hovered, value);
		}
	}

	public event UnhoveredEventHandler Unhovered
	{
		add
		{
			backing_Unhovered = (UnhoveredEventHandler)Delegate.Combine(backing_Unhovered, value);
		}
		remove
		{
			backing_Unhovered = (UnhoveredEventHandler)Delegate.Remove(backing_Unhovered, value);
		}
	}

	public void Initialize(NMerchantInventory rug)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		_merchantRug = rug;
		Player.GoldChanged += UpdateVisual;
		((GodotObject)this).Connect(SignalName.Hovered, Callable.From<NMerchantSlot>((Action<NMerchantSlot>)OnMerchantHandHovered), 0u);
		((GodotObject)this).Connect(SignalName.Unhovered, Callable.From<NMerchantSlot>((Action<NMerchantSlot>)OnMerchantHandUnhovered), 0u);
	}

	public override void _Ready()
	{
		if (((object)this).GetType() != typeof(NMerchantSlot))
		{
			Log.Error($"{((object)this).GetType()}");
			throw new InvalidOperationException("Don't call base._Ready()! Call ConnectSignals() instead.");
		}
		ConnectSignals();
	}

	protected virtual void ConnectSignals()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		_hitbox = ((Node)this).GetNode<NClickableControl>(NodePath.op_Implicit("%Hitbox"));
		((GodotObject)this).Connect(SignalName.FocusEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)this).Connect(SignalName.FocusExited, Callable.From((Action)OnUnfocus), 0u);
		((GodotObject)_hitbox).Connect(SignalName.MouseEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)_hitbox).Connect(SignalName.MouseExited, Callable.From((Action)OnUnfocus), 0u);
		((GodotObject)_hitbox).Connect(NClickableControl.SignalName.MousePressed, Callable.From<InputEvent>((Action<InputEvent>)OnMousePressed), 0u);
		((GodotObject)_hitbox).Connect(NClickableControl.SignalName.MouseReleased, Callable.From<InputEvent>((Action<InputEvent>)OnMouseReleased), 0u);
		_costLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%CostLabel"));
	}

	public override void _ExitTree()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).Disconnect(SignalName.Unhovered, Callable.From<NMerchantSlot>((Action<NMerchantSlot>)OnMerchantHandUnhovered));
		((GodotObject)_hitbox).Disconnect(SignalName.MouseExited, Callable.From((Action)OnUnfocus));
		((GodotObject)this).Disconnect(SignalName.FocusExited, Callable.From((Action)OnUnfocus));
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		if (Player != null)
		{
			Player.GoldChanged -= UpdateVisual;
		}
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(MegaInput.select, false))
		{
			TaskHelper.RunSafely(OnReleased());
		}
		else if (inputEvent.IsActionReleased(MegaInput.accept, false))
		{
			OnPreview();
			((Node)this).GetViewport().SetInputAsHandled();
		}
	}

	private void OnMousePressed(InputEvent inputEvent)
	{
		_ignoreMouseRelease = false;
	}

	private void OnMouseReleased(InputEvent inputEvent)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I8
		if (!_isHovered || _ignoreMouseRelease)
		{
			return;
		}
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null)
		{
			if ((long)val.ButtonIndex == 1)
			{
				TaskHelper.RunSafely(OnReleased());
			}
			else
			{
				OnPreview();
			}
		}
	}

	private void OnFocus()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		_isHovered = true;
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		((Control)this).Scale = _hoverScale;
		CreateHoverTip();
		((GodotObject)this).EmitSignal(SignalName.Hovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
	}

	private void OnUnfocus()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		_isHovered = false;
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(_smallScale), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		ClearHoverTip();
		((GodotObject)this).EmitSignal(SignalName.Unhovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
	}

	private async Task OnReleased()
	{
		ClearHoverTip();
		await OnTryPurchase(_merchantRug?.Inventory);
		MerchantEntry entry = Entry;
		if (entry != null && entry.IsStocked && _isHovered)
		{
			CreateHoverTip();
		}
	}

	protected abstract Task OnTryPurchase(MerchantInventory? inventory);

	protected void TriggerMerchantHandToPointHere()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_merchantRug?.MerchantHand.PointAtTarget(((Control)this).GlobalPosition);
		_merchantRug?.MerchantHand.StopPointing(2f);
	}

	protected virtual void OnPreview()
	{
	}

	protected abstract void CreateHoverTip();

	protected void ClearHoverTip()
	{
		NHoverTipSet.Remove((Control)(object)this);
	}

	private void OnMerchantHandHovered(NMerchantSlot _)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_merchantRug?.MerchantHand.PointAtTarget(((Control)this).GlobalPosition);
	}

	private void OnMerchantHandUnhovered(NMerchantSlot _)
	{
		_merchantRug?.MerchantHand.StopPointing(2f);
	}

	protected void OnPurchaseFailed(PurchaseStatus status)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (status == PurchaseStatus.Success)
		{
			return;
		}
		if (!_originalVisualPosition.HasValue)
		{
			CanvasItem visual = Visual;
			Node2D val = (Node2D)(object)((visual is Node2D) ? visual : null);
			if (val != null)
			{
				_originalVisualPosition = val.Position.X;
			}
			else
			{
				CanvasItem visual2 = Visual;
				Control val2 = (Control)(object)((visual2 is Control) ? visual2 : null);
				if (val2 != null)
				{
					_originalVisualPosition = val2.Position.X;
				}
			}
		}
		Tween? purchaseFailedTween = _purchaseFailedTween;
		if (purchaseFailedTween != null)
		{
			purchaseFailedTween.Kill();
		}
		_purchaseFailedTween = ((Node)this).CreateTween();
		_purchaseFailedTween.TweenMethod(Callable.From<float>((Action<float>)WiggleAnimation), Variant.op_Implicit(0f), Variant.op_Implicit(2f), 0.4000000059604645).SetEase((EaseType)1).SetTrans((TransitionType)4);
		SfxCmd.Play("event:/sfx/npcs/merchant/merchant_dissapointment");
	}

	protected virtual void UpdateVisual()
	{
		if (Entry.IsStocked)
		{
			_costLabel.SetTextAutoSize(Entry.Cost.ToString());
		}
	}

	private void WiggleAnimation(float progress)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		CanvasItem visual = Visual;
		Node2D val = (Node2D)(object)((visual is Node2D) ? visual : null);
		if (val != null)
		{
			Vector2 position = val.Position;
			position.X = _originalVisualPosition.Value + (float)Math.Sin(progress * (float)Math.PI * 2f) * 10f;
			val.Position = position;
			return;
		}
		CanvasItem visual2 = Visual;
		Control val2 = (Control)(object)((visual2 is Control) ? visual2 : null);
		if (val2 != null)
		{
			Vector2 position = val2.Position;
			position.X = _originalVisualPosition.Value + (float)Math.Sin(progress * (float)Math.PI * 2f) * 10f;
			val2.Position = position;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Expected O, but got Unknown
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Expected O, but got Unknown
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(18);
		list.Add(new MethodInfo(MethodName.Initialize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("rug"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMousePressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMouseReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TriggerMerchantHandToPointHere, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPreview, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateHoverTip, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearHoverTip, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMerchantHandHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMerchantHandUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPurchaseFailed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("status"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateVisual, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.WiggleAnimation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("progress"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Initialize && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Initialize(VariantUtils.ConvertTo<NMerchantInventory>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
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
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TriggerMerchantHandToPointHere && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TriggerMerchantHandToPointHere();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPreview && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPreview();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CreateHoverTip && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CreateHoverTip();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearHoverTip && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearHoverTip();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMerchantHandHovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMerchantHandHovered(VariantUtils.ConvertTo<NMerchantSlot>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMerchantHandUnhovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMerchantHandUnhovered(VariantUtils.ConvertTo<NMerchantSlot>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPurchaseFailed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPurchaseFailed(VariantUtils.ConvertTo<PurchaseStatus>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateVisual && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateVisual();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.WiggleAnimation && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			WiggleAnimation(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Initialize)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
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
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.TriggerMerchantHandToPointHere)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPreview)
		{
			return true;
		}
		if ((ref method) == MethodName.CreateHoverTip)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearHoverTip)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMerchantHandHovered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMerchantHandUnhovered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPurchaseFailed)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateVisual)
		{
			return true;
		}
		if ((ref method) == MethodName.WiggleAnimation)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._isHovered)
		{
			_isHovered = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hitbox)
		{
			_hitbox = VariantUtils.ConvertTo<NClickableControl>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._costLabel)
		{
			_costLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._purchaseFailedTween)
		{
			_purchaseFailedTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._merchantRug)
		{
			_merchantRug = VariantUtils.ConvertTo<NMerchantInventory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ignoreMouseRelease)
		{
			_ignoreMouseRelease = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Hitbox)
		{
			NClickableControl hitbox = Hitbox;
			value = VariantUtils.CreateFrom<NClickableControl>(ref hitbox);
			return true;
		}
		if ((ref name) == PropertyName.Visual)
		{
			CanvasItem visual = Visual;
			value = VariantUtils.CreateFrom<CanvasItem>(ref visual);
			return true;
		}
		if ((ref name) == PropertyName._isHovered)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isHovered);
			return true;
		}
		if ((ref name) == PropertyName._hitbox)
		{
			value = VariantUtils.CreateFrom<NClickableControl>(ref _hitbox);
			return true;
		}
		if ((ref name) == PropertyName._costLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _costLabel);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._purchaseFailedTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _purchaseFailedTween);
			return true;
		}
		if ((ref name) == PropertyName._merchantRug)
		{
			value = VariantUtils.CreateFrom<NMerchantInventory>(ref _merchantRug);
			return true;
		}
		if ((ref name) == PropertyName._ignoreMouseRelease)
		{
			value = VariantUtils.CreateFrom<bool>(ref _ignoreMouseRelease);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._isHovered, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Hitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._costLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._purchaseFailedTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._merchantRug, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._ignoreMouseRelease, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Visual, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._isHovered, Variant.From<bool>(ref _isHovered));
		info.AddProperty(PropertyName._hitbox, Variant.From<NClickableControl>(ref _hitbox));
		info.AddProperty(PropertyName._costLabel, Variant.From<MegaLabel>(ref _costLabel));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._purchaseFailedTween, Variant.From<Tween>(ref _purchaseFailedTween));
		info.AddProperty(PropertyName._merchantRug, Variant.From<NMerchantInventory>(ref _merchantRug));
		info.AddProperty(PropertyName._ignoreMouseRelease, Variant.From<bool>(ref _ignoreMouseRelease));
		info.AddSignalEventDelegate(SignalName.Hovered, (Delegate)backing_Hovered);
		info.AddSignalEventDelegate(SignalName.Unhovered, (Delegate)backing_Unhovered);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._isHovered, ref val))
		{
			_isHovered = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._hitbox, ref val2))
		{
			_hitbox = ((Variant)(ref val2)).As<NClickableControl>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._costLabel, ref val3))
		{
			_costLabel = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val4))
		{
			_hoverTween = ((Variant)(ref val4)).As<Tween>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._purchaseFailedTween, ref val5))
		{
			_purchaseFailedTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._merchantRug, ref val6))
		{
			_merchantRug = ((Variant)(ref val6)).As<NMerchantInventory>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._ignoreMouseRelease, ref val7))
		{
			_ignoreMouseRelease = ((Variant)(ref val7)).As<bool>();
		}
		HoveredEventHandler hoveredEventHandler = default(HoveredEventHandler);
		if (info.TryGetSignalEventDelegate<HoveredEventHandler>(SignalName.Hovered, ref hoveredEventHandler))
		{
			backing_Hovered = hoveredEventHandler;
		}
		UnhoveredEventHandler unhoveredEventHandler = default(UnhoveredEventHandler);
		if (info.TryGetSignalEventDelegate<UnhoveredEventHandler>(SignalName.Unhovered, ref unhoveredEventHandler))
		{
			backing_Unhovered = unhoveredEventHandler;
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
		list.Add(new MethodInfo(SignalName.Hovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("slot"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.Unhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("slot"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalHovered(NMerchantSlot slot)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Hovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)slot) });
	}

	protected void EmitSignalUnhovered(NMerchantSlot slot)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Unhovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)slot) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Hovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Hovered?.Invoke(VariantUtils.ConvertTo<NMerchantSlot>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.Unhovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Unhovered?.Invoke(VariantUtils.ConvertTo<NMerchantSlot>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Hovered)
		{
			return true;
		}
		if ((ref signal) == SignalName.Unhovered)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
