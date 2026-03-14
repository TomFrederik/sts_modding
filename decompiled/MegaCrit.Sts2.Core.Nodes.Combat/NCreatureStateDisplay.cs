using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NCreatureStateDisplay.cs")]
public class NCreatureStateDisplay : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName SubscribeToCreatureEvents = StringName.op_Implicit("SubscribeToCreatureEvents");

		public static readonly StringName DebugToggleVisibility = StringName.op_Implicit("DebugToggleVisibility");

		public static readonly StringName SetCreatureBounds = StringName.op_Implicit("SetCreatureBounds");

		public static readonly StringName RefreshValues = StringName.op_Implicit("RefreshValues");

		public static readonly StringName OnHovered = StringName.op_Implicit("OnHovered");

		public static readonly StringName OnUnhovered = StringName.op_Implicit("OnUnhovered");

		public static readonly StringName ShowNameplate = StringName.op_Implicit("ShowNameplate");

		public static readonly StringName HideNameplate = StringName.op_Implicit("HideNameplate");

		public static readonly StringName HideImmediately = StringName.op_Implicit("HideImmediately");

		public static readonly StringName AnimateIn = StringName.op_Implicit("AnimateIn");

		public static readonly StringName AnimateInBlock = StringName.op_Implicit("AnimateInBlock");

		public static readonly StringName AnimateOut = StringName.op_Implicit("AnimateOut");

		public static readonly StringName OnBlockTrackingCreatureBlockChanged = StringName.op_Implicit("OnBlockTrackingCreatureBlockChanged");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _powerContainer = StringName.op_Implicit("_powerContainer");

		public static readonly StringName _nameplateContainer = StringName.op_Implicit("_nameplateContainer");

		public static readonly StringName _nameplateLabel = StringName.op_Implicit("_nameplateLabel");

		public static readonly StringName _healthBar = StringName.op_Implicit("_healthBar");

		public static readonly StringName _hpBarHitbox = StringName.op_Implicit("_hpBarHitbox");

		public static readonly StringName _creatureSize = StringName.op_Implicit("_creatureSize");

		public static readonly StringName _showHideTween = StringName.op_Implicit("_showHideTween");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _originalPosition = StringName.op_Implicit("_originalPosition");
	}

	public class SignalName : SignalName
	{
	}

	private NPowerContainer _powerContainer;

	private Control _nameplateContainer;

	private MegaLabel _nameplateLabel;

	private NHealthBar _healthBar;

	private Control _hpBarHitbox;

	private Creature? _creature;

	private Vector2 _creatureSize;

	private Creature? _blockTrackingCreature;

	private Tween? _showHideTween;

	private Tween? _hoverTween;

	private static readonly Vector2 _healthBarAnimOffset = new Vector2(0f, 20f);

	private Vector2 _originalPosition;

	public override void _Ready()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		_powerContainer = ((Node)this).GetNode<NPowerContainer>(NodePath.op_Implicit("%PowerContainer"));
		_nameplateContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%NameplateContainer"));
		_nameplateLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%NameplateLabel"));
		_healthBar = ((Node)this).GetNode<NHealthBar>(NodePath.op_Implicit("%HealthBar"));
		_hpBarHitbox = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%HpBarHitbox"));
		((CanvasItem)_nameplateContainer).Modulate = StsColors.transparentWhite;
		_originalPosition = ((Control)this).Position;
		((GodotObject)_hpBarHitbox).Connect(SignalName.MouseEntered, Callable.From((Action)OnHovered), 0u);
		((GodotObject)_hpBarHitbox).Connect(SignalName.MouseExited, Callable.From((Action)OnUnhovered), 0u);
	}

	public override void _EnterTree()
	{
		((Node)this)._EnterTree();
		CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
		SubscribeToCreatureEvents();
		if (NCombatRoom.Instance != null)
		{
			NCombatRoom.Instance.Ui.DebugToggleHpBar += DebugToggleVisibility;
		}
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
		if (_creature != null)
		{
			_creature.BlockChanged -= AnimateInBlock;
			_creature.Died -= OnCreatureDied;
			_creature.Revived -= OnCreatureRevived;
		}
		if (_blockTrackingCreature != null)
		{
			_blockTrackingCreature.BlockChanged -= OnBlockTrackingCreatureBlockChanged;
		}
		if (NCombatRoom.Instance != null)
		{
			NCombatRoom.Instance.Ui.DebugToggleHpBar -= DebugToggleVisibility;
		}
	}

	public void SetCreature(Creature creature)
	{
		if (_creature != null)
		{
			throw new InvalidOperationException("Creature was already set.");
		}
		_creature = creature;
		SubscribeToCreatureEvents();
		_nameplateLabel.SetTextAutoSize(creature.Name);
		_powerContainer.SetCreature(_creature);
		_healthBar.SetCreature(_creature);
		RefreshValues();
	}

	private void SubscribeToCreatureEvents()
	{
		if (_creature != null)
		{
			_creature.BlockChanged += AnimateInBlock;
			_creature.Died += OnCreatureDied;
			_creature.Revived += OnCreatureRevived;
		}
	}

	private void DebugToggleVisibility()
	{
		((CanvasItem)this).Visible = !NCombatUi.IsDebugHidingHpBar;
	}

	public void SetCreatureBounds(Control bounds)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		_healthBar.UpdateLayoutForCreatureBounds(bounds);
		_nameplateContainer.GlobalPosition = new Vector2(bounds.GlobalPosition.X, _nameplateContainer.GlobalPosition.Y);
		_nameplateContainer.Size = new Vector2(bounds.Size.X * bounds.Scale.X, _nameplateContainer.Size.Y);
		_powerContainer.SetCreatureBounds(bounds);
		RefreshValues();
	}

	private void RefreshValues()
	{
		if (_creature != null)
		{
			_nameplateLabel.SetTextAutoSize(_creature.Name);
			_healthBar.RefreshValues();
		}
	}

	private void OnCombatStateChanged(CombatState _)
	{
		RefreshValues();
	}

	private void OnHovered()
	{
		_healthBar.FadeOutHpLabel(0.5f, 0.1f);
		ShowNameplate();
		if (!NTargetManager.Instance.IsInSelection)
		{
			NCombatRoom.Instance?.GetCreatureNode(_creature)?.ShowHoverTips(_creature.HoverTips);
		}
	}

	private void OnUnhovered()
	{
		_healthBar.FadeInHpLabel(0.5f);
		HideNameplate();
		NCombatRoom.Instance.GetCreatureNode(_creature)?.HideHoverTips();
	}

	public void ShowNameplate()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)_powerContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.5f), 0.15000000596046448);
		_hoverTween.TweenProperty((GodotObject)(object)_nameplateContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1), 0.15000000596046448);
	}

	public void HideNameplate()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)_powerContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.20000000298023224);
		_hoverTween.TweenProperty((GodotObject)(object)_nameplateContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0), 0.20000000298023224);
	}

	public void HideImmediately()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Color modulate = ((CanvasItem)this).Modulate;
		modulate.A = 0f;
		((CanvasItem)this).Modulate = modulate;
	}

	public void AnimateIn(HealthBarAnimMode mode)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant)
		{
			Color modulate = ((CanvasItem)this).Modulate;
			modulate.A = 1f;
			((CanvasItem)this).Modulate = modulate;
			((CanvasItem)this).Visible = true;
			return;
		}
		float num = 0f;
		((CanvasItem)this).Visible = true;
		((CanvasItem)this).Modulate = StsColors.transparentWhite;
		((Control)this).Position = ((Control)this).Position - _healthBarAnimOffset;
		if (mode == HealthBarAnimMode.SpawnedAtCombatStart)
		{
			num = Rng.Chaotic.NextFloat(1.3f, 1.7f);
		}
		Tween? showHideTween = _showHideTween;
		if (showHideTween != null)
		{
			showHideTween.Kill();
		}
		_showHideTween = ((Node)this).CreateTween().SetParallel(true);
		_showHideTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), (double)((mode == HealthBarAnimMode.FromHidden) ? 0.15f : 1f)).SetEase((EaseType)1).SetTrans((TransitionType)1)
			.SetDelay((double)num);
		_showHideTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_originalPosition), (double)((mode == HealthBarAnimMode.FromHidden) ? 0.15f : 0.5f)).SetEase((EaseType)1).SetTrans((TransitionType)4)
			.SetDelay((double)num);
	}

	private void AnimateInBlock(int oldBlock, int blockGain)
	{
		if (oldBlock == 0 && blockGain != 0)
		{
			_healthBar.AnimateInBlock(oldBlock, blockGain);
		}
	}

	public void AnimateOut()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant)
		{
			Color modulate = ((CanvasItem)this).Modulate;
			modulate.A = 0f;
			((CanvasItem)this).Modulate = modulate;
			((CanvasItem)this).Visible = false;
			return;
		}
		Tween? showHideTween = _showHideTween;
		if (showHideTween != null)
		{
			showHideTween.Kill();
		}
		_showHideTween = ((Node)this).CreateTween().SetParallel(true);
		_showHideTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)1);
		_showHideTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_healthBarAnimOffset), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)4);
		_showHideTween.Chain().TweenCallback(Callable.From<bool>((Func<bool>)(() => ((CanvasItem)this).Visible = false)));
	}

	private void OnCreatureDied(Creature _)
	{
		_hpBarHitbox.MouseFilter = (MouseFilterEnum)2;
	}

	private void OnCreatureRevived(Creature _)
	{
		_hpBarHitbox.MouseFilter = (MouseFilterEnum)0;
	}

	public void TrackBlockStatus(Creature creature)
	{
		_blockTrackingCreature = creature;
		_blockTrackingCreature.BlockChanged += OnBlockTrackingCreatureBlockChanged;
		_healthBar.TrackBlockStatus(creature);
	}

	private void OnBlockTrackingCreatureBlockChanged(int oldBlock, int blockGain)
	{
		_healthBar.RefreshValues();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
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
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(16);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SubscribeToCreatureEvents, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugToggleVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCreatureBounds, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("bounds"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshValues, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowNameplate, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideNameplate, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideImmediately, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("mode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateInBlock, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("oldBlock"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("blockGain"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateOut, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnBlockTrackingCreatureBlockChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("oldBlock"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("blockGain"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
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
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		if ((ref method) == MethodName.SubscribeToCreatureEvents && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SubscribeToCreatureEvents();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugToggleVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DebugToggleVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCreatureBounds && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCreatureBounds(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshValues && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshValues();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHovered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHovered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnhovered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnhovered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowNameplate && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowNameplate();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideNameplate && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideNameplate();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideImmediately && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideImmediately();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateIn && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AnimateIn(VariantUtils.ConvertTo<HealthBarAnimMode>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateInBlock && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			AnimateInBlock(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateOut && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateOut();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnBlockTrackingCreatureBlockChanged && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			OnBlockTrackingCreatureBlockChanged(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.SubscribeToCreatureEvents)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugToggleVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName.SetCreatureBounds)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshValues)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHovered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnhovered)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowNameplate)
		{
			return true;
		}
		if ((ref method) == MethodName.HideNameplate)
		{
			return true;
		}
		if ((ref method) == MethodName.HideImmediately)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateIn)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateInBlock)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateOut)
		{
			return true;
		}
		if ((ref method) == MethodName.OnBlockTrackingCreatureBlockChanged)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._powerContainer)
		{
			_powerContainer = VariantUtils.ConvertTo<NPowerContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nameplateContainer)
		{
			_nameplateContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nameplateLabel)
		{
			_nameplateLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._healthBar)
		{
			_healthBar = VariantUtils.ConvertTo<NHealthBar>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hpBarHitbox)
		{
			_hpBarHitbox = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._creatureSize)
		{
			_creatureSize = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._showHideTween)
		{
			_showHideTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalPosition)
		{
			_originalPosition = VariantUtils.ConvertTo<Vector2>(ref value);
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
		if ((ref name) == PropertyName._powerContainer)
		{
			value = VariantUtils.CreateFrom<NPowerContainer>(ref _powerContainer);
			return true;
		}
		if ((ref name) == PropertyName._nameplateContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _nameplateContainer);
			return true;
		}
		if ((ref name) == PropertyName._nameplateLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _nameplateLabel);
			return true;
		}
		if ((ref name) == PropertyName._healthBar)
		{
			value = VariantUtils.CreateFrom<NHealthBar>(ref _healthBar);
			return true;
		}
		if ((ref name) == PropertyName._hpBarHitbox)
		{
			value = VariantUtils.CreateFrom<Control>(ref _hpBarHitbox);
			return true;
		}
		if ((ref name) == PropertyName._creatureSize)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _creatureSize);
			return true;
		}
		if ((ref name) == PropertyName._showHideTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _showHideTween);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._originalPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _originalPosition);
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
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._powerContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._nameplateContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._nameplateLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._healthBar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hpBarHitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._creatureSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._showHideTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._originalPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._powerContainer, Variant.From<NPowerContainer>(ref _powerContainer));
		info.AddProperty(PropertyName._nameplateContainer, Variant.From<Control>(ref _nameplateContainer));
		info.AddProperty(PropertyName._nameplateLabel, Variant.From<MegaLabel>(ref _nameplateLabel));
		info.AddProperty(PropertyName._healthBar, Variant.From<NHealthBar>(ref _healthBar));
		info.AddProperty(PropertyName._hpBarHitbox, Variant.From<Control>(ref _hpBarHitbox));
		info.AddProperty(PropertyName._creatureSize, Variant.From<Vector2>(ref _creatureSize));
		info.AddProperty(PropertyName._showHideTween, Variant.From<Tween>(ref _showHideTween));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._originalPosition, Variant.From<Vector2>(ref _originalPosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._powerContainer, ref val))
		{
			_powerContainer = ((Variant)(ref val)).As<NPowerContainer>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._nameplateContainer, ref val2))
		{
			_nameplateContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._nameplateLabel, ref val3))
		{
			_nameplateLabel = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._healthBar, ref val4))
		{
			_healthBar = ((Variant)(ref val4)).As<NHealthBar>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._hpBarHitbox, ref val5))
		{
			_hpBarHitbox = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._creatureSize, ref val6))
		{
			_creatureSize = ((Variant)(ref val6)).As<Vector2>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._showHideTween, ref val7))
		{
			_showHideTween = ((Variant)(ref val7)).As<Tween>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val8))
		{
			_hoverTween = ((Variant)(ref val8)).As<Tween>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalPosition, ref val9))
		{
			_originalPosition = ((Variant)(ref val9)).As<Vector2>();
		}
	}
}
