using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Potions;

[ScriptPath("res://src/Core/Nodes/Potions/NPotionPopup.cs")]
public class NPotionPopup : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnUseButtonPressed = StringName.op_Implicit("OnUseButtonPressed");

		public static readonly StringName OnDiscardButtonPressed = StringName.op_Implicit("OnDiscardButtonPressed");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName Remove = StringName.op_Implicit("Remove");

		public static readonly StringName RefreshUseButton = StringName.op_Implicit("RefreshUseButton");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName IsUsable = StringName.op_Implicit("IsUsable");

		public static readonly StringName InACardSelectScreen = StringName.op_Implicit("InACardSelectScreen");

		public static readonly StringName _holder = StringName.op_Implicit("_holder");

		public static readonly StringName _popupContainer = StringName.op_Implicit("_popupContainer");

		public static readonly StringName _useButton = StringName.op_Implicit("_useButton");

		public static readonly StringName _discardButton = StringName.op_Implicit("_discardButton");

		public static readonly StringName _hoverTipBounds = StringName.op_Implicit("_hoverTipBounds");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private NPotionHolder _holder;

	private Control _popupContainer;

	private NPotionPopupButton _useButton;

	private NPotionPopupButton _discardButton;

	private Control _hoverTipBounds;

	private Tween? _tween;

	private PotionModel? Potion => _holder.Potion?.Model;

	public bool IsUsable => _useButton.IsEnabled;

	private static string ScenePath => SceneHelper.GetScenePath("/potions/potion_popup");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	private bool InACardSelectScreen
	{
		get
		{
			if (!NPlayerHand.Instance.IsInCardSelection)
			{
				return NOverlayStack.Instance.Peek() is ICardSelector;
			}
			return true;
		}
	}

	public static NPotionPopup Create(NPotionHolder holder)
	{
		NPotionPopup nPotionPopup = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NPotionPopup>((GenEditState)0);
		nPotionPopup._holder = holder;
		return nPotionPopup;
	}

	public override void _Ready()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).GlobalPosition = ((Control)_holder).GlobalPosition + Vector2.Down * ((Control)_holder).Size.Y * 1.5f + Vector2.Right * ((Control)_holder).Size * 0.5f + Vector2.Left * ((Control)this).Size * 0.5f;
		_hoverTipBounds = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%HoverTipBounds"));
		NHoverTipSet.CreateAndShow(_hoverTipBounds, _holder.Potion.Model.HoverTips, HoverTipAlignment.Right);
		NHoverTipSet.shouldBlockHoverTips = true;
		_popupContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Container"));
		_useButton = ((Node)this).GetNode<NPotionPopupButton>(NodePath.op_Implicit("%UseButton"));
		((GodotObject)_useButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnUseButtonPressed), 0u);
		_discardButton = ((Node)this).GetNode<NPotionPopupButton>(NodePath.op_Implicit("%DiscardButton"));
		_discardButton.SetLocKey("POTION_POPUP.discard");
		((GodotObject)_discardButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnDiscardButtonPressed), 0u);
		((Control)_useButton).FocusNeighborLeft = ((Node)_useButton).GetPath();
		((Control)_useButton).FocusNeighborRight = ((Node)_useButton).GetPath();
		((Control)_useButton).FocusNeighborTop = ((Node)_useButton).GetPath();
		((Control)_useButton).FocusNeighborBottom = ((Node)_discardButton).GetPath();
		((Control)_discardButton).FocusNeighborLeft = ((Node)_discardButton).GetPath();
		((Control)_discardButton).FocusNeighborRight = ((Node)_discardButton).GetPath();
		((Control)_discardButton).FocusNeighborTop = ((Node)_useButton).GetPath();
		((Control)_discardButton).FocusNeighborBottom = ((Node)_discardButton).GetPath();
		if (Potion == null || Potion.IsQueued || Potion.Owner.Creature.IsDead)
		{
			_useButton.Disable();
			_discardButton.Disable();
		}
		else
		{
			switch (Potion.Usage)
			{
			case PotionUsage.None:
				throw new InvalidOperationException("No potions should have 'None' usage.");
			case PotionUsage.CombatOnly:
				CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
				CombatManager.Instance.TurnStarted += OnTurnStarted;
				CombatManager.Instance.PlayerEndedTurn += OnPlayerEndTurnStatusChanged;
				CombatManager.Instance.PlayerUnendedTurn += OnPlayerEndTurnStatusChanged;
				NOverlayStack.Instance.Changed += Remove;
				NCapstoneContainer.Instance.Changed += Remove;
				RefreshUseButton();
				break;
			case PotionUsage.AnyTime:
				_useButton.Enable();
				break;
			case PotionUsage.Automatic:
				_useButton.Disable();
				break;
			default:
				throw new ArgumentOutOfRangeException("Usage");
			}
			if (!Potion.Owner.CanRemovePotions)
			{
				_useButton.Disable();
				_discardButton.Disable();
			}
			if (!Potion.PassesCustomUsabilityCheck)
			{
				_useButton.Disable();
			}
			if (_useButton.IsEnabled)
			{
				((Control)(object)_useButton).TryGrabFocus();
			}
			else if (_discardButton.IsEnabled)
			{
				((Control)(object)_discardButton).TryGrabFocus();
			}
			else
			{
				((Control)(object)this).TryGrabFocus();
			}
		}
		if (Potion == null)
		{
			_useButton.SetLocKey("POTION_POPUP.drink");
		}
		else
		{
			TargetType targetType = Potion.TargetType;
			bool flag = ((targetType == TargetType.AnyEnemy || targetType == TargetType.TargetedNoCreature) ? true : false);
			if (flag || Potion.CanThrowAtAlly())
			{
				_useButton.SetLocKey("POTION_POPUP.throw");
			}
			else
			{
				_useButton.SetLocKey("POTION_POPUP.drink");
			}
		}
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		((CanvasItem)this).Modulate = Colors.Transparent;
		Control popupContainer = _popupContainer;
		popupContainer.Position += Vector2.Up * 25f;
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.10000000149011612).SetTrans((TransitionType)1);
		_tween.TweenProperty((GodotObject)(object)_popupContainer, NodePath.op_Implicit("position:y"), Variant.op_Implicit(_popupContainer.Position.Y + 25f), 0.15000000596046448).SetEase((EaseType)0).SetTrans((TransitionType)1);
	}

	private void OnUseButtonPressed(NButton _)
	{
		TaskHelper.RunSafely(UsePotion());
		Remove();
	}

	private async Task UsePotion()
	{
		if (Potion == null)
		{
			return;
		}
		PotionModel potion = Potion;
		potion.BeforeUse += DisableHolder;
		try
		{
			await _holder.UsePotion();
		}
		finally
		{
			potion.BeforeUse -= DisableHolder;
		}
		void DisableHolder()
		{
			_holder.DisableUntilPotionRemoved();
		}
	}

	private void OnDiscardButtonPressed(NButton _)
	{
		if (Potion != null)
		{
			Player owner = Potion.Owner;
			int num = owner.PotionSlots.IndexOf<PotionModel>(_holder.Potion.Model);
			if (num < 0)
			{
				throw new InvalidOperationException($"Tried to discard potion {_holder.Potion.Model} but it's not in the player's belt!");
			}
			_holder.DisableUntilPotionRemoved();
			DiscardPotionGameAction action = new DiscardPotionGameAction(owner, (uint)num);
			RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(action);
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I8
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null)
		{
			MouseButton buttonIndex = val.ButtonIndex;
			bool flag = (((long)(buttonIndex - 1) <= 1L) ? true : false);
			if (flag && ((InputEvent)val).IsReleased())
			{
				Remove();
			}
		}
		else if (inputEvent.IsActionPressed(MegaInput.cancel, false, false))
		{
			Remove();
			((Control)(object)_holder).TryGrabFocus();
		}
	}

	public void Remove()
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		NHoverTipSet.shouldBlockHoverTips = false;
		NHoverTipSet.Remove(_hoverTipBounds);
		CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
		CombatManager.Instance.TurnStarted -= OnTurnStarted;
		CombatManager.Instance.PlayerEndedTurn -= OnPlayerEndTurnStatusChanged;
		CombatManager.Instance.PlayerUnendedTurn -= OnPlayerEndTurnStatusChanged;
		NOverlayStack.Instance.Changed -= Remove;
		NCapstoneContainer.Instance.Changed -= Remove;
		Callable val = Callable.From((Action)delegate
		{
			_useButton.Disable();
			_discardButton.Disable();
		});
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.Transparent), 0.10000000149011612).SetTrans((TransitionType)1);
		_tween.TweenProperty((GodotObject)(object)_popupContainer, NodePath.op_Implicit("position:y"), Variant.op_Implicit(-25f), 0.20000000298023224).SetEase((EaseType)1).SetTrans((TransitionType)1);
		_tween.Chain().TweenCallback(Callable.From((Action)this.QueueFreeSafely));
	}

	private void OnTurnStarted(CombatState _)
	{
		RefreshUseButton();
	}

	private void OnPlayerEndTurnStatusChanged(Player _, bool __)
	{
		RefreshUseButton();
	}

	private void OnPlayerEndTurnStatusChanged(Player _)
	{
		RefreshUseButton();
	}

	private void OnCombatStateChanged(CombatState _)
	{
		RefreshUseButton();
	}

	private void RefreshUseButton()
	{
		Creature creature = Potion?.Owner.Creature;
		if (creature != null && CombatManager.Instance.IsInProgress && creature.CombatState.CurrentSide == creature.Side && !InACardSelectScreen && !CombatManager.Instance.PlayerActionsDisabled)
		{
			_useButton.Enable();
		}
		else
		{
			_useButton.Disable();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUseButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDiscardButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Remove, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshUseButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NPotionPopup nPotionPopup = Create(VariantUtils.ConvertTo<NPotionHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NPotionPopup>(ref nPotionPopup);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUseButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnUseButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDiscardButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnDiscardButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Remove && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Remove();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshUseButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshUseButton();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NPotionPopup nPotionPopup = Create(VariantUtils.ConvertTo<NPotionHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NPotionPopup>(ref nPotionPopup);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUseButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDiscardButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.Remove)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshUseButton)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._holder)
		{
			_holder = VariantUtils.ConvertTo<NPotionHolder>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._popupContainer)
		{
			_popupContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._useButton)
		{
			_useButton = VariantUtils.ConvertTo<NPotionPopupButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._discardButton)
		{
			_discardButton = VariantUtils.ConvertTo<NPotionPopupButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTipBounds)
		{
			_hoverTipBounds = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName.IsUsable)
		{
			bool isUsable = IsUsable;
			value = VariantUtils.CreateFrom<bool>(ref isUsable);
			return true;
		}
		if ((ref name) == PropertyName.InACardSelectScreen)
		{
			bool isUsable = InACardSelectScreen;
			value = VariantUtils.CreateFrom<bool>(ref isUsable);
			return true;
		}
		if ((ref name) == PropertyName._holder)
		{
			value = VariantUtils.CreateFrom<NPotionHolder>(ref _holder);
			return true;
		}
		if ((ref name) == PropertyName._popupContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _popupContainer);
			return true;
		}
		if ((ref name) == PropertyName._useButton)
		{
			value = VariantUtils.CreateFrom<NPotionPopupButton>(ref _useButton);
			return true;
		}
		if ((ref name) == PropertyName._discardButton)
		{
			value = VariantUtils.CreateFrom<NPotionPopupButton>(ref _discardButton);
			return true;
		}
		if ((ref name) == PropertyName._hoverTipBounds)
		{
			value = VariantUtils.CreateFrom<Control>(ref _hoverTipBounds);
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
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._holder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._popupContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._useButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._discardButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTipBounds, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsUsable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.InACardSelectScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._holder, Variant.From<NPotionHolder>(ref _holder));
		info.AddProperty(PropertyName._popupContainer, Variant.From<Control>(ref _popupContainer));
		info.AddProperty(PropertyName._useButton, Variant.From<NPotionPopupButton>(ref _useButton));
		info.AddProperty(PropertyName._discardButton, Variant.From<NPotionPopupButton>(ref _discardButton));
		info.AddProperty(PropertyName._hoverTipBounds, Variant.From<Control>(ref _hoverTipBounds));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._holder, ref val))
		{
			_holder = ((Variant)(ref val)).As<NPotionHolder>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._popupContainer, ref val2))
		{
			_popupContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._useButton, ref val3))
		{
			_useButton = ((Variant)(ref val3)).As<NPotionPopupButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._discardButton, ref val4))
		{
			_discardButton = ((Variant)(ref val4)).As<NPotionPopupButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTipBounds, ref val5))
		{
			_hoverTipBounds = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val6))
		{
			_tween = ((Variant)(ref val6)).As<Tween>();
		}
	}
}
