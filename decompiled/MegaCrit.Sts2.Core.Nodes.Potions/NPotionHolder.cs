using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Potions;

[ScriptPath("res://src/Core/Nodes/Potions/NPotionHolder.cs")]
public class NPotionHolder : NClickableControl
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public static readonly StringName OpenPotionPopup = StringName.op_Implicit("OpenPotionPopup");

		public static readonly StringName AddPotion = StringName.op_Implicit("AddPotion");

		public static readonly StringName DisableUntilPotionRemoved = StringName.op_Implicit("DisableUntilPotionRemoved");

		public static readonly StringName CancelPotionUse = StringName.op_Implicit("CancelPotionUse");

		public static readonly StringName RemoveUsedPotion = StringName.op_Implicit("RemoveUsedPotion");

		public static readonly StringName DiscardPotion = StringName.op_Implicit("DiscardPotion");

		public static readonly StringName ShouldCancelTargeting = StringName.op_Implicit("ShouldCancelTargeting");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName Potion = StringName.op_Implicit("Potion");

		public static readonly StringName HasPotion = StringName.op_Implicit("HasPotion");

		public static readonly StringName IsPotionUsable = StringName.op_Implicit("IsPotionUsable");

		public static readonly StringName _potionScale = StringName.op_Implicit("_potionScale");

		public static readonly StringName _emptyIcon = StringName.op_Implicit("_emptyIcon");

		public static readonly StringName _popup = StringName.op_Implicit("_popup");

		public static readonly StringName _potionTargeting = StringName.op_Implicit("_potionTargeting");

		public static readonly StringName _isUsable = StringName.op_Implicit("_isUsable");

		public static readonly StringName _emptyPotionTween = StringName.op_Implicit("_emptyPotionTween");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _disabledUntilPotionRemoved = StringName.op_Implicit("_disabledUntilPotionRemoved");

		public static readonly StringName _isFocused = StringName.op_Implicit("_isFocused");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private Vector2 _potionScale = 0.9f * Vector2.One;

	private static readonly HoverTip _emptyHoverTip = new HoverTip(new LocString("static_hover_tips", "POTION_SLOT.title"), new LocString("static_hover_tips", "POTION_SLOT.description"));

	private TextureRect _emptyIcon;

	private NPotionPopup? _popup;

	private bool _potionTargeting;

	private bool _isUsable;

	private Tween? _emptyPotionTween;

	private Tween? _hoverTween;

	private bool _disabledUntilPotionRemoved;

	private bool _isFocused;

	private CancellationTokenSource? _cancelGrayOutPotionSource;

	public NPotion? Potion { get; private set; }

	public bool HasPotion => Potion != null;

	public bool IsPotionUsable => _popup.IsUsable;

	private static string ScenePath => SceneHelper.GetScenePath("/potions/potion_holder");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public static NPotionHolder Create(bool isUsable)
	{
		NPotionHolder nPotionHolder = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NPotionHolder>((GenEditState)0);
		nPotionHolder._isUsable = isUsable;
		return nPotionHolder;
	}

	public override void _Ready()
	{
		_emptyIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%EmptyIcon"));
		ConnectSignals();
	}

	protected override void OnFocus()
	{
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		if (_isFocused)
		{
			return;
		}
		_isFocused = true;
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		if (Potion != null)
		{
			Potion.DoBounce();
			_hoverTween.TweenProperty((GodotObject)(object)Potion, NodePath.op_Implicit("scale"), Variant.op_Implicit(_potionScale * 1.15f), 0.05);
			NDebugAudioManager.Instance?.Play(Rng.Chaotic.NextItem(TmpSfx.PotionSlosh), 0.5f, PitchVariance.Large);
			if (!GodotObject.IsInstanceValid((GodotObject)(object)_popup))
			{
				NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, Potion.Model.HoverTips, HoverTipAlignment.Center);
				((Control)nHoverTipSet).GlobalPosition = ((Control)this).GlobalPosition + Vector2.Down * ((Control)this).Size.Y * Mathf.Max(1.5f, ((Control)this).Scale.Y);
			}
		}
		else
		{
			_hoverTween.TweenProperty((GodotObject)(object)_emptyIcon, NodePath.op_Implicit("scale"), Variant.op_Implicit(_potionScale * 1.15f), 0.05);
			NHoverTipSet nHoverTipSet2 = NHoverTipSet.CreateAndShow((Control)(object)this, _emptyHoverTip);
			((Control)nHoverTipSet2).GlobalPosition = ((Control)this).GlobalPosition + Vector2.Down * ((Control)this).Size.Y * Mathf.Max(1.5f, ((Control)this).Scale.Y);
			nHoverTipSet2.SetAlignment((Control)(object)this, HoverTipAlignment.Center);
		}
	}

	protected override void OnUnfocus()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		_isFocused = false;
		NHoverTipSet.Remove((Control)(object)this);
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		if (Potion != null)
		{
			if (!_disabledUntilPotionRemoved)
			{
				_hoverTween.TweenProperty((GodotObject)(object)Potion, NodePath.op_Implicit("scale"), Variant.op_Implicit(_potionScale), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			}
		}
		else
		{
			_hoverTween.TweenProperty((GodotObject)(object)_emptyIcon, NodePath.op_Implicit("scale"), Variant.op_Implicit(_potionScale), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		}
	}

	protected override void OnPress()
	{
		if (Potion != null)
		{
			((Node)this).GetViewport().SetInputAsHandled();
		}
	}

	protected override void OnRelease()
	{
		if (_isUsable)
		{
			OpenPotionPopup();
		}
		if (Potion != null)
		{
			((Node)this).GetViewport().SetInputAsHandled();
		}
	}

	private void OpenPotionPopup()
	{
		if (HasPotion && !Potion.Model.Owner.RunState.IsGameOver && !_disabledUntilPotionRemoved)
		{
			NHoverTipSet.Remove((Control)(object)this);
			_popup = NPotionPopup.Create(this);
			((Node)(object)this).AddChildSafely((Node?)(object)_popup);
		}
	}

	public void AddPotion(NPotion potion)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (Potion != null)
		{
			throw new InvalidOperationException("Slot already contains a potion");
		}
		Potion = potion;
		Tween? emptyPotionTween = _emptyPotionTween;
		if (emptyPotionTween != null)
		{
			emptyPotionTween.Kill();
		}
		((CanvasItem)_emptyIcon).Modulate = Colors.Transparent;
		((Node)(object)this).AddChildSafely((Node?)(object)Potion);
		((Control)Potion).Scale = _potionScale;
		((Control)Potion).PivotOffset = ((Control)Potion).Size * 0.5f;
	}

	public void DisableUntilPotionRemoved()
	{
		if (_popup != null && GodotObject.IsInstanceValid((GodotObject)(object)_popup))
		{
			_popup.Remove();
		}
		_disabledUntilPotionRemoved = true;
		TaskHelper.RunSafely(GrayPotionHolderUntilPlayedAfterDelay());
		((Control)(object)this).TryGrabFocus();
	}

	private async Task GrayPotionHolderUntilPlayedAfterDelay()
	{
		_cancelGrayOutPotionSource = new CancellationTokenSource();
		await Task.Delay(100, _cancelGrayOutPotionSource.Token);
		if (!_cancelGrayOutPotionSource.IsCancellationRequested)
		{
			((CanvasItem)this).Modulate = StsColors.gray;
		}
	}

	public void CancelPotionUse()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		_cancelGrayOutPotionSource?.Cancel();
		_disabledUntilPotionRemoved = false;
		((CanvasItem)this).Modulate = Colors.White;
	}

	public void RemoveUsedPotion()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		if (Potion == null)
		{
			throw new InvalidOperationException("This slot doesn't contain a potion");
		}
		if (_popup != null && GodotObject.IsInstanceValid((GodotObject)(object)_popup))
		{
			_popup.Remove();
		}
		NHoverTipSet.Remove((Control)(object)this);
		_disabledUntilPotionRemoved = false;
		_cancelGrayOutPotionSource?.Cancel();
		((CanvasItem)this).Modulate = Colors.White;
		NPotion potionToRemove = Potion;
		Potion = null;
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)potionToRemove, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.Zero), 0.20000000298023224).SetEase((EaseType)0).SetTrans((TransitionType)10)
			.FromCurrent();
		val.TweenCallback(Callable.From((Action)delegate
		{
			((Node)(object)this).RemoveChildSafely((Node?)(object)potionToRemove);
			((Node)(object)potionToRemove).QueueFreeSafely();
		}));
		if (base.IsFocused)
		{
			NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _emptyHoverTip);
			((Control)nHoverTipSet).GlobalPosition = ((Control)this).GlobalPosition + Vector2.Down * ((Control)this).Size.Y * 1.5f;
			nHoverTipSet.SetAlignment((Control)(object)this, HoverTipAlignment.Center);
		}
		Tween? emptyPotionTween = _emptyPotionTween;
		if (emptyPotionTween != null)
		{
			emptyPotionTween.Kill();
		}
		_emptyPotionTween = ((Node)this).CreateTween();
		_emptyPotionTween.TweenProperty((GodotObject)(object)_emptyIcon, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.20000000298023224).SetDelay(0.20000000298023224);
	}

	public void DiscardPotion()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if (Potion == null)
		{
			throw new InvalidOperationException("This slot doesn't contain a potion");
		}
		if (_popup != null && GodotObject.IsInstanceValid((GodotObject)(object)_popup))
		{
			_popup.Remove();
		}
		_disabledUntilPotionRemoved = false;
		_cancelGrayOutPotionSource?.Cancel();
		((CanvasItem)this).Modulate = Colors.White;
		NPotion potionToRemove = Potion;
		Potion = null;
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)potionToRemove, NodePath.op_Implicit("position:y"), Variant.op_Implicit(-100f), 0.4000000059604645).SetEase((EaseType)0).SetTrans((TransitionType)10);
		val.TweenCallback(Callable.From((Action)delegate
		{
			((Node)(object)this).RemoveChildSafely((Node?)(object)potionToRemove);
			((Node)(object)potionToRemove).QueueFreeSafely();
		}));
		Tween? emptyPotionTween = _emptyPotionTween;
		if (emptyPotionTween != null)
		{
			emptyPotionTween.Kill();
		}
		_emptyPotionTween = ((Node)this).CreateTween();
		_emptyPotionTween.TweenProperty((GodotObject)(object)_emptyIcon, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.20000000298023224).FromCurrent().SetDelay(0.20000000298023224);
	}

	public async Task UsePotion()
	{
		if (Potion == null)
		{
			Log.Warn("Tried to use potion in holder, but potion node is null!");
			return;
		}
		TargetType targetType = Potion.Model.TargetType;
		bool flag = ((targetType == TargetType.AnyEnemy || targetType == TargetType.TargetedNoCreature) ? true : false);
		if (flag || Potion.Model.CanThrowAtAlly())
		{
			RunManager.Instance.HoveredModelTracker.OnLocalPotionSelected(Potion.Model);
			await TargetNode(Potion.Model.TargetType);
			RunManager.Instance.HoveredModelTracker.OnLocalPotionDeselected();
		}
		else
		{
			Potion.Model.EnqueueManualUse(Potion.Model.Owner.Creature);
			((Control)(object)this).TryGrabFocus();
		}
	}

	private async Task TargetNode(TargetType targetType)
	{
		Vector2 startPosition = ((Control)this).GlobalPosition + Vector2.Right * ((Control)this).Size.X * 0.5f + Vector2.Down * 50f;
		NTargetManager instance = NTargetManager.Instance;
		bool isUsingController = NControllerManager.Instance.IsUsingController;
		instance.StartTargeting(targetType, startPosition, isUsingController ? TargetMode.Controller : TargetMode.ClickMouseToTarget, ShouldCancelTargeting, null);
		Creature creature = Potion.Model.Owner.Creature;
		if (isUsingController && CombatManager.Instance.IsInProgress)
		{
			CombatState combatState = creature.CombatState;
			List<Creature> source = (targetType switch
			{
				TargetType.AnyEnemy => combatState.GetOpponentsOf(creature), 
				TargetType.AnyPlayer => combatState.GetTeammatesOf(creature), 
				_ => throw new ArgumentOutOfRangeException("targetType", targetType, null), 
			}).Where((Creature c) => c.IsAlive).ToList();
			NCombatRoom.Instance.RestrictControllerNavigation(source.Select((Creature c) => NCombatRoom.Instance.GetCreatureNode(c).Hitbox));
			NCombatRoom.Instance.GetCreatureNode(source.First()).Hitbox.TryGrabFocus();
		}
		else if (isUsingController && targetType == TargetType.AnyPlayer)
		{
			NMultiplayerPlayerStateContainer multiplayerPlayerContainer = NRun.Instance.GlobalUi.MultiplayerPlayerContainer;
			((Control)(object)multiplayerPlayerContainer.FirstPlayerState?.Hitbox).TryGrabFocus();
			multiplayerPlayerContainer.LockNavigation();
		}
		bool flag = Potion.Model is FoulPotion;
		bool flag2 = creature.Player.RunState.CurrentRoom.RoomType == RoomType.Shop;
		bool isFoulPotionInShop = isUsingController && flag && flag2;
		if (isFoulPotionInShop)
		{
			NMerchantButton merchantButton = NMerchantRoom.Instance.MerchantButton;
			((Control)merchantButton).SetFocusMode((FocusModeEnum)2);
			((Control)(object)merchantButton).TryGrabFocus();
		}
		try
		{
			Node val = await instance.SelectionFinished();
			NCombatRoom.Instance?.EnableControllerNavigation();
			NRun.Instance.GlobalUi.MultiplayerPlayerContainer.UnlockNavigation();
			if (val != null)
			{
				Creature creature2;
				if (!(val is NCreature nCreature))
				{
					if (!(val is NMultiplayerPlayerState nMultiplayerPlayerState))
					{
						if (!(val is NMerchantButton))
						{
							throw new ArgumentOutOfRangeException("targetNode", val, null);
						}
						creature2 = null;
					}
					else
					{
						creature2 = nMultiplayerPlayerState.Player.Creature;
					}
				}
				else
				{
					creature2 = nCreature.Entity;
				}
				Creature target = creature2;
				Potion.Model.EnqueueManualUse(target);
			}
		}
		finally
		{
			if (isFoulPotionInShop)
			{
				NMerchantButton merchantButton2 = NMerchantRoom.Instance.MerchantButton;
				((Control)merchantButton2).SetFocusMode((FocusModeEnum)0);
			}
		}
		((Control)(object)this).TryGrabFocus();
	}

	private bool ShouldCancelTargeting()
	{
		if (Potion != null)
		{
			if (CombatManager.Instance.IsInProgress)
			{
				if (NOverlayStack.Instance.ScreenCount <= 0)
				{
					return NCapstoneContainer.Instance.InUse;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public async Task ShineOnStartOfCombat()
	{
		if (HasPotion)
		{
			Potion.DoBounce();
			await Cmd.Wait(0.25f);
			NDebugAudioManager.Instance?.Play(Rng.Chaotic.NextItem(TmpSfx.PotionSlosh), 0.3f, PitchVariance.Large);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isUsable"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenPotionPopup, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddPotion, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("potion"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableUntilPotionRemoved, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelPotionUse, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveUsedPotion, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DiscardPotion, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShouldCancelTargeting, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NPotionHolder nPotionHolder = Create(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NPotionHolder>(ref nPotionHolder);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenPotionPopup && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenPotionPopup();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddPotion && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AddPotion(VariantUtils.ConvertTo<NPotion>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableUntilPotionRemoved && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableUntilPotionRemoved();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CancelPotionUse && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CancelPotionUse();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemoveUsedPotion && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RemoveUsedPotion();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DiscardPotion && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DiscardPotion();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShouldCancelTargeting && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = ShouldCancelTargeting();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NPotionHolder nPotionHolder = Create(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NPotionHolder>(ref nPotionHolder);
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
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenPotionPopup)
		{
			return true;
		}
		if ((ref method) == MethodName.AddPotion)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableUntilPotionRemoved)
		{
			return true;
		}
		if ((ref method) == MethodName.CancelPotionUse)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveUsedPotion)
		{
			return true;
		}
		if ((ref method) == MethodName.DiscardPotion)
		{
			return true;
		}
		if ((ref method) == MethodName.ShouldCancelTargeting)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Potion)
		{
			Potion = VariantUtils.ConvertTo<NPotion>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionScale)
		{
			_potionScale = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._emptyIcon)
		{
			_emptyIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._popup)
		{
			_popup = VariantUtils.ConvertTo<NPotionPopup>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionTargeting)
		{
			_potionTargeting = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isUsable)
		{
			_isUsable = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._emptyPotionTween)
		{
			_emptyPotionTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._disabledUntilPotionRemoved)
		{
			_disabledUntilPotionRemoved = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isFocused)
		{
			_isFocused = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName.Potion)
		{
			NPotion potion = Potion;
			value = VariantUtils.CreateFrom<NPotion>(ref potion);
			return true;
		}
		if ((ref name) == PropertyName.HasPotion)
		{
			bool hasPotion = HasPotion;
			value = VariantUtils.CreateFrom<bool>(ref hasPotion);
			return true;
		}
		if ((ref name) == PropertyName.IsPotionUsable)
		{
			bool hasPotion = IsPotionUsable;
			value = VariantUtils.CreateFrom<bool>(ref hasPotion);
			return true;
		}
		if ((ref name) == PropertyName._potionScale)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _potionScale);
			return true;
		}
		if ((ref name) == PropertyName._emptyIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _emptyIcon);
			return true;
		}
		if ((ref name) == PropertyName._popup)
		{
			value = VariantUtils.CreateFrom<NPotionPopup>(ref _popup);
			return true;
		}
		if ((ref name) == PropertyName._potionTargeting)
		{
			value = VariantUtils.CreateFrom<bool>(ref _potionTargeting);
			return true;
		}
		if ((ref name) == PropertyName._isUsable)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isUsable);
			return true;
		}
		if ((ref name) == PropertyName._emptyPotionTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _emptyPotionTween);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._disabledUntilPotionRemoved)
		{
			value = VariantUtils.CreateFrom<bool>(ref _disabledUntilPotionRemoved);
			return true;
		}
		if ((ref name) == PropertyName._isFocused)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isFocused);
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
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)5, PropertyName._potionScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Potion, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._emptyIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.HasPotion, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._popup, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._potionTargeting, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isUsable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._emptyPotionTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._disabledUntilPotionRemoved, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsPotionUsable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isFocused, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName potion = PropertyName.Potion;
		NPotion potion2 = Potion;
		info.AddProperty(potion, Variant.From<NPotion>(ref potion2));
		info.AddProperty(PropertyName._potionScale, Variant.From<Vector2>(ref _potionScale));
		info.AddProperty(PropertyName._emptyIcon, Variant.From<TextureRect>(ref _emptyIcon));
		info.AddProperty(PropertyName._popup, Variant.From<NPotionPopup>(ref _popup));
		info.AddProperty(PropertyName._potionTargeting, Variant.From<bool>(ref _potionTargeting));
		info.AddProperty(PropertyName._isUsable, Variant.From<bool>(ref _isUsable));
		info.AddProperty(PropertyName._emptyPotionTween, Variant.From<Tween>(ref _emptyPotionTween));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._disabledUntilPotionRemoved, Variant.From<bool>(ref _disabledUntilPotionRemoved));
		info.AddProperty(PropertyName._isFocused, Variant.From<bool>(ref _isFocused));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Potion, ref val))
		{
			Potion = ((Variant)(ref val)).As<NPotion>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionScale, ref val2))
		{
			_potionScale = ((Variant)(ref val2)).As<Vector2>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._emptyIcon, ref val3))
		{
			_emptyIcon = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._popup, ref val4))
		{
			_popup = ((Variant)(ref val4)).As<NPotionPopup>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionTargeting, ref val5))
		{
			_potionTargeting = ((Variant)(ref val5)).As<bool>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._isUsable, ref val6))
		{
			_isUsable = ((Variant)(ref val6)).As<bool>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._emptyPotionTween, ref val7))
		{
			_emptyPotionTween = ((Variant)(ref val7)).As<Tween>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val8))
		{
			_hoverTween = ((Variant)(ref val8)).As<Tween>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._disabledUntilPotionRemoved, ref val9))
		{
			_disabledUntilPotionRemoved = ((Variant)(ref val9)).As<bool>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._isFocused, ref val10))
		{
			_isFocused = ((Variant)(ref val10)).As<bool>();
		}
	}
}
