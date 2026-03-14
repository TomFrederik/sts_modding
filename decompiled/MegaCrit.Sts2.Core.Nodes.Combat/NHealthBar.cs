using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NHealthBar.cs")]
public class NHealthBar : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName DebugToggleVisibility = StringName.op_Implicit("DebugToggleVisibility");

		public static readonly StringName UpdateLayoutForCreatureBounds = StringName.op_Implicit("UpdateLayoutForCreatureBounds");

		public static readonly StringName UpdateWidthRelativeToReferenceValue = StringName.op_Implicit("UpdateWidthRelativeToReferenceValue");

		public static readonly StringName SetHpBarContainerSizeWithOffsets = StringName.op_Implicit("SetHpBarContainerSizeWithOffsets");

		public static readonly StringName SetHpBarContainerSizeWithOffsetsImmediately = StringName.op_Implicit("SetHpBarContainerSizeWithOffsetsImmediately");

		public static readonly StringName RefreshValues = StringName.op_Implicit("RefreshValues");

		public static readonly StringName RefreshMiddleground = StringName.op_Implicit("RefreshMiddleground");

		public static readonly StringName RefreshForeground = StringName.op_Implicit("RefreshForeground");

		public static readonly StringName RefreshBlockUi = StringName.op_Implicit("RefreshBlockUi");

		public static readonly StringName RefreshText = StringName.op_Implicit("RefreshText");

		public static readonly StringName IsPoisonLethal = StringName.op_Implicit("IsPoisonLethal");

		public static readonly StringName IsDoomLethal = StringName.op_Implicit("IsDoomLethal");

		public static readonly StringName GetFgWidth = StringName.op_Implicit("GetFgWidth");

		public static readonly StringName FadeOutHpLabel = StringName.op_Implicit("FadeOutHpLabel");

		public static readonly StringName FadeInHpLabel = StringName.op_Implicit("FadeInHpLabel");

		public static readonly StringName AnimateInBlock = StringName.op_Implicit("AnimateInBlock");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName MaxFgWidth = StringName.op_Implicit("MaxFgWidth");

		public static readonly StringName HpBarContainer = StringName.op_Implicit("HpBarContainer");

		public static readonly StringName _hpForegroundContainer = StringName.op_Implicit("_hpForegroundContainer");

		public static readonly StringName _hpForeground = StringName.op_Implicit("_hpForeground");

		public static readonly StringName _poisonForeground = StringName.op_Implicit("_poisonForeground");

		public static readonly StringName _doomForeground = StringName.op_Implicit("_doomForeground");

		public static readonly StringName _hpMiddleground = StringName.op_Implicit("_hpMiddleground");

		public static readonly StringName _hpLabel = StringName.op_Implicit("_hpLabel");

		public static readonly StringName _blockContainer = StringName.op_Implicit("_blockContainer");

		public static readonly StringName _blockLabel = StringName.op_Implicit("_blockLabel");

		public static readonly StringName _blockOutline = StringName.op_Implicit("_blockOutline");

		public static readonly StringName _infinityTex = StringName.op_Implicit("_infinityTex");

		public static readonly StringName _blockTween = StringName.op_Implicit("_blockTween");

		public static readonly StringName _hpLabelFadeTween = StringName.op_Implicit("_hpLabelFadeTween");

		public static readonly StringName _middlegroundTween = StringName.op_Implicit("_middlegroundTween");

		public static readonly StringName _originalBlockPosition = StringName.op_Implicit("_originalBlockPosition");

		public static readonly StringName _currentHpOnLastRefresh = StringName.op_Implicit("_currentHpOnLastRefresh");

		public static readonly StringName _maxHpOnLastRefresh = StringName.op_Implicit("_maxHpOnLastRefresh");

		public static readonly StringName _expectedMaxFgWidth = StringName.op_Implicit("_expectedMaxFgWidth");
	}

	public class SignalName : SignalName
	{
	}

	private Control _hpForegroundContainer;

	private Control _hpForeground;

	private Control _poisonForeground;

	private Control _doomForeground;

	private Control _hpMiddleground;

	private MegaLabel _hpLabel;

	private Control _blockContainer;

	private MegaLabel _blockLabel;

	private Control _blockOutline;

	private Creature _creature;

	private Creature? _blockTrackingCreature;

	private readonly LocString _healthBarDead = new LocString("gameplay_ui", "HEALTH_BAR.DEAD");

	private TextureRect _infinityTex;

	private Tween? _blockTween;

	private Tween? _hpLabelFadeTween;

	private Tween? _middlegroundTween;

	private Vector2 _originalBlockPosition;

	private int _currentHpOnLastRefresh = -1;

	private int _maxHpOnLastRefresh = -1;

	private float _expectedMaxFgWidth = -1f;

	private const float _minSize = 12f;

	private static readonly Vector2 _blockAnimOffset = new Vector2(0f, 20f);

	private static readonly Color _defaultFontColor = StsColors.cream;

	private static readonly Color _defaultFontOutlineColor = new Color("900000");

	private static readonly Color _blockOutlineColor = new Color("1B3045");

	private static readonly Color _redForegroundColor = new Color("F1373E");

	private static readonly Color _blockHpForegroundColor = new Color("3B6FA3");

	private static readonly Color _invincibleForegroundColor = new Color("C5BBED");

	private const float _foregroundContainerInset = 10f;

	private float MaxFgWidth
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (!(_expectedMaxFgWidth > 0f))
			{
				return _hpForegroundContainer.Size.X;
			}
			return _expectedMaxFgWidth;
		}
	}

	public Control HpBarContainer { get; private set; }

	public void SetCreature(Creature creature)
	{
		if (_creature != null)
		{
			throw new InvalidOperationException("Creature was already set.");
		}
		_creature = creature;
		_hpForeground.OffsetRight = GetFgWidth(_creature.CurrentHp) - MaxFgWidth;
		_hpMiddleground.OffsetRight = _hpForeground.OffsetRight - 2f;
	}

	public override void _Ready()
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		HpBarContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%HpBarContainer"));
		_hpForegroundContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%HpForegroundContainer"));
		_hpMiddleground = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%HpMiddleground"));
		_hpForeground = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%HpForeground"));
		_poisonForeground = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%PoisonForeground"));
		_doomForeground = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%DoomForeground"));
		_hpLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%HpLabel"));
		_blockContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BlockContainer"));
		_blockLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%BlockLabel"));
		_blockOutline = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BlockOutline"));
		_infinityTex = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%InfinityTex"));
		_originalBlockPosition = _blockContainer.Position;
	}

	private void DebugToggleVisibility()
	{
		((CanvasItem)this).Visible = !NCombatUi.IsDebugHidingHpBar;
	}

	public void UpdateLayoutForCreatureBounds(Control bounds)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		float valueOrDefault = (24f - _creature.Monster?.HpBarSizeReduction).GetValueOrDefault();
		HpBarContainer.GlobalPosition = new Vector2(bounds.GlobalPosition.X - valueOrDefault * 0.5f, HpBarContainer.GlobalPosition.Y);
		float num = bounds.Size.X + valueOrDefault;
		SetHpBarContainerSizeWithOffsets(new Vector2(num, HpBarContainer.Size.Y));
		float num2 = _blockContainer.Size.X * 0.5f;
		_blockContainer.GlobalPosition = new Vector2(bounds.GlobalPosition.X - num2, _blockContainer.GlobalPosition.Y);
		_originalBlockPosition = _blockContainer.Position;
	}

	public void UpdateWidthRelativeToReferenceValue(float refMaxHp, float refWidth)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Vector2 size = HpBarContainer.Size;
		size.X = (float)_creature.MaxHp / refMaxHp * refWidth;
		SetHpBarContainerSizeWithOffsetsImmediately(size);
	}

	private void SetHpBarContainerSizeWithOffsets(Vector2 size)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Callable val = Callable.From((Action)delegate
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			SetHpBarContainerSizeWithOffsetsImmediately(size);
		});
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
	}

	private void SetHpBarContainerSizeWithOffsetsImmediately(Vector2 size)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Vector2 size2 = HpBarContainer.Size;
		if (!((Vector2)(ref size2)).IsEqualApprox(size))
		{
			Tween? middlegroundTween = _middlegroundTween;
			if (middlegroundTween != null)
			{
				middlegroundTween.Kill();
			}
			HpBarContainer.Size = size;
			_expectedMaxFgWidth = size.X - 10f;
			_hpForeground.OffsetRight = GetFgWidth(_creature.CurrentHp, _expectedMaxFgWidth) - _expectedMaxFgWidth;
			_hpMiddleground.OffsetRight = _hpForeground.OffsetRight - 2f;
		}
	}

	public void RefreshValues()
	{
		RefreshBlockUi();
		RefreshForeground();
		RefreshMiddleground();
		RefreshText();
	}

	private void RefreshMiddleground()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (_creature.CurrentHp <= 0)
		{
			((CanvasItem)_hpMiddleground).Visible = false;
			return;
		}
		((CanvasItem)_hpMiddleground).Visible = true;
		Control hpMiddleground = _hpMiddleground;
		Vector2 position = _hpMiddleground.Position;
		position.X = 1f;
		hpMiddleground.Position = position;
		int currentHp = _creature.CurrentHp;
		int maxHp = _creature.MaxHp;
		if (currentHp != _currentHpOnLastRefresh || maxHp != _maxHpOnLastRefresh)
		{
			_currentHpOnLastRefresh = currentHp;
			_maxHpOnLastRefresh = maxHp;
			float num = (_creature.HasPower<PoisonPower>() ? _poisonForeground.OffsetRight : _hpForeground.OffsetRight);
			bool flag = num >= _hpMiddleground.OffsetRight;
			Control hpMiddleground2 = _hpMiddleground;
			hpMiddleground2.OffsetRight += 1f;
			Tween? middlegroundTween = _middlegroundTween;
			if (middlegroundTween != null)
			{
				middlegroundTween.Kill();
			}
			_middlegroundTween = ((Node)this).CreateTween();
			_middlegroundTween.TweenProperty((GodotObject)(object)_hpMiddleground, NodePath.op_Implicit("offset_right"), Variant.op_Implicit(num - 2f), 1.0).SetDelay(flag ? 0.0 : 1.0).SetEase((EaseType)1)
				.SetTrans((TransitionType)5);
		}
	}

	private void RefreshForeground()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		if (_creature.CurrentHp <= 0)
		{
			((CanvasItem)_poisonForeground).Visible = false;
			((CanvasItem)_doomForeground).Visible = false;
			((CanvasItem)_hpForeground).Visible = false;
			return;
		}
		((CanvasItem)_hpForeground).Visible = true;
		float offsetRight = GetFgWidth(_creature.CurrentHp) - MaxFgWidth;
		_hpForeground.OffsetRight = offsetRight;
		if (_creature.ShowsInfiniteHp)
		{
			((CanvasItem)_hpForeground).SelfModulate = _invincibleForegroundColor;
			return;
		}
		int powerAmount = _creature.GetPowerAmount<DoomPower>();
		int num = _creature.GetPower<PoisonPower>()?.CalculateTotalDamageNextTurn() ?? 0;
		if (_creature.HasPower<PoisonPower>())
		{
			if (num > 0)
			{
				((CanvasItem)_poisonForeground).Visible = true;
				if (IsPoisonLethal(num))
				{
					_poisonForeground.OffsetLeft = 0f;
					_poisonForeground.OffsetRight = offsetRight;
					((CanvasItem)_hpForeground).Visible = false;
				}
				else
				{
					float fgWidth = GetFgWidth(_creature.CurrentHp - num);
					_hpForeground.OffsetRight = fgWidth - MaxFgWidth;
					((CanvasItem)_hpForeground).Visible = true;
					int patchMarginLeft = ((NinePatchRect)_poisonForeground).PatchMarginLeft;
					_poisonForeground.OffsetLeft = Math.Max(0f, fgWidth - (float)patchMarginLeft);
					_poisonForeground.OffsetRight = offsetRight;
				}
			}
			else
			{
				((CanvasItem)_poisonForeground).Visible = false;
			}
		}
		else
		{
			((CanvasItem)_poisonForeground).Visible = false;
			_poisonForeground.OffsetLeft = 0f;
		}
		if (_creature.HasPower<DoomPower>())
		{
			if (powerAmount > 0)
			{
				((CanvasItem)_doomForeground).Visible = true;
				float num2 = GetFgWidth(powerAmount) - MaxFgWidth;
				if (IsDoomLethal(powerAmount, num))
				{
					if (!IsPoisonLethal(num))
					{
						_doomForeground.OffsetRight = _hpForeground.OffsetRight;
						((CanvasItem)_hpForeground).Visible = false;
					}
					else
					{
						((CanvasItem)_hpForeground).Visible = false;
						((CanvasItem)_doomForeground).Visible = false;
					}
				}
				else
				{
					int patchMarginRight = ((NinePatchRect)_doomForeground).PatchMarginRight;
					_doomForeground.OffsetRight = Math.Min(0f, num2 + (float)patchMarginRight);
					((CanvasItem)_hpForeground).Visible = true;
				}
			}
			else
			{
				((CanvasItem)_doomForeground).Visible = false;
			}
		}
		else
		{
			((CanvasItem)_doomForeground).Visible = false;
		}
	}

	private void RefreshBlockUi()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (_creature.Block <= 0)
		{
			Creature blockTrackingCreature = _blockTrackingCreature;
			if (blockTrackingCreature == null || blockTrackingCreature.Block <= 0)
			{
				if (((CanvasItem)_blockContainer).Visible)
				{
					NBlockBrokenVfx nBlockBrokenVfx = NBlockBrokenVfx.Create();
					if (nBlockBrokenVfx != null)
					{
						((Node)(object)this).AddChildSafely((Node?)(object)nBlockBrokenVfx);
						((Node2D)nBlockBrokenVfx).GlobalPosition = _blockContainer.GlobalPosition + _blockContainer.Size * 0.5f;
					}
				}
				((CanvasItem)_blockContainer).Visible = false;
				((CanvasItem)_blockOutline).Visible = false;
				((CanvasItem)_hpForeground).SelfModulate = _redForegroundColor;
				return;
			}
		}
		((CanvasItem)_blockOutline).Visible = true;
		((CanvasItem)_hpForeground).SelfModulate = _blockHpForegroundColor;
		if (_creature.Block > 0)
		{
			((CanvasItem)_blockContainer).Visible = true;
			_blockLabel.SetTextAutoSize(_creature.Block.ToString());
		}
	}

	private void RefreshText()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		if (_creature.CurrentHp <= 0)
		{
			((Control)_hpLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, _defaultFontColor);
			((Control)_hpLabel).AddThemeColorOverride(ThemeConstants.Label.fontOutlineColor, _defaultFontOutlineColor);
			_hpLabel.SetTextAutoSize(_healthBarDead.GetRawText());
			return;
		}
		if (_creature.ShowsInfiniteHp)
		{
			((CanvasItem)_infinityTex).Visible = _creature.IsAlive;
			((CanvasItem)_doomForeground).Modulate = Colors.Transparent;
			((CanvasItem)_hpLabel).Visible = !((CanvasItem)_infinityTex).Visible;
			return;
		}
		((CanvasItem)_hpLabel).Visible = true;
		int poisonDamage = _creature.GetPower<PoisonPower>()?.CalculateTotalDamageNextTurn() ?? 0;
		int powerAmount = _creature.GetPowerAmount<DoomPower>();
		Color defaultFontColor = default(Color);
		Color val = default(Color);
		if (IsPoisonLethal(poisonDamage))
		{
			((Color)(ref defaultFontColor))._002Ector("76FF40");
			((Color)(ref val))._002Ector("074700");
		}
		else if (IsDoomLethal(powerAmount, poisonDamage))
		{
			((Color)(ref defaultFontColor))._002Ector("FB8DFF");
			((Color)(ref val))._002Ector("2D1263");
		}
		else
		{
			if (_creature.Block <= 0)
			{
				Creature blockTrackingCreature = _blockTrackingCreature;
				if (blockTrackingCreature == null || blockTrackingCreature.Block <= 0)
				{
					defaultFontColor = _defaultFontColor;
					val = _defaultFontOutlineColor;
					goto IL_0151;
				}
			}
			defaultFontColor = _defaultFontColor;
			val = _blockOutlineColor;
		}
		goto IL_0151;
		IL_0151:
		((Control)_hpLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, defaultFontColor);
		((Control)_hpLabel).AddThemeColorOverride(ThemeConstants.Label.fontOutlineColor, val);
		_hpLabel.SetTextAutoSize($"{_creature.CurrentHp}/{_creature.MaxHp}");
	}

	private bool IsPoisonLethal(int poisonDamage)
	{
		if (poisonDamage <= 0 || !_creature.HasPower<PoisonPower>())
		{
			return false;
		}
		return poisonDamage >= _creature.CurrentHp;
	}

	private bool IsDoomLethal(int doomAmount, int poisonDamage)
	{
		if (doomAmount <= 0 || !_creature.HasPower<DoomPower>())
		{
			return false;
		}
		return doomAmount >= _creature.CurrentHp - poisonDamage;
	}

	private float GetFgWidth(int amount)
	{
		return GetFgWidth(amount, MaxFgWidth);
	}

	private float GetFgWidth(int amount, float maxFgWidth)
	{
		if (_creature.MaxHp <= 0)
		{
			return 0f;
		}
		float val = (float)amount / (float)_creature.MaxHp * maxFgWidth;
		return Math.Max(val, (_creature.CurrentHp > 0) ? 12f : 0f);
	}

	public void FadeOutHpLabel(float duration, float finalAlpha)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Tween? hpLabelFadeTween = _hpLabelFadeTween;
		if (hpLabelFadeTween != null)
		{
			hpLabelFadeTween.Kill();
		}
		_hpLabelFadeTween = ((Node)this).CreateTween();
		_hpLabelFadeTween.TweenProperty((GodotObject)(object)_hpLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(finalAlpha), (double)duration).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	public void FadeInHpLabel(float duration)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Tween? hpLabelFadeTween = _hpLabelFadeTween;
		if (hpLabelFadeTween != null)
		{
			hpLabelFadeTween.Kill();
		}
		_hpLabelFadeTween = ((Node)this).CreateTween();
		_hpLabelFadeTween.TweenProperty((GodotObject)(object)_hpLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), (double)duration);
	}

	public void AnimateInBlock(int oldBlock, int blockGain)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (oldBlock != 0 || blockGain == 0)
		{
			return;
		}
		((CanvasItem)_blockContainer).Visible = true;
		if (SaveManager.Instance.PrefsSave.FastMode != FastModeType.Instant)
		{
			((CanvasItem)_blockContainer).Modulate = StsColors.transparentWhite;
			_blockContainer.Position = _originalBlockPosition - _blockAnimOffset;
			Tween? blockTween = _blockTween;
			if (blockTween != null)
			{
				blockTween.Kill();
			}
			_blockTween = ((Node)this).CreateTween().SetParallel(true);
			_blockTween.TweenProperty((GodotObject)(object)_blockContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)1);
			_blockTween.TweenProperty((GodotObject)(object)_blockContainer, NodePath.op_Implicit("position"), Variant.op_Implicit(_originalBlockPosition), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)10);
		}
	}

	public void TrackBlockStatus(Creature creature)
	{
		_blockTrackingCreature = creature;
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
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(18);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugToggleVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateLayoutForCreatureBounds, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("bounds"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateWidthRelativeToReferenceValue, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("refMaxHp"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("refWidth"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetHpBarContainerSizeWithOffsets, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("size"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetHpBarContainerSizeWithOffsetsImmediately, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("size"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshValues, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshMiddleground, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshForeground, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshBlockUi, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsPoisonLethal, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("poisonDamage"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsDoomLethal, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("doomAmount"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("poisonDamage"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetFgWidth, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("amount"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetFgWidth, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("amount"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("maxFgWidth"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FadeOutHpLabel, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("finalAlpha"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FadeInHpLabel, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateInBlock, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
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
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugToggleVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DebugToggleVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateLayoutForCreatureBounds && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateLayoutForCreatureBounds(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateWidthRelativeToReferenceValue && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			UpdateWidthRelativeToReferenceValue(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetHpBarContainerSizeWithOffsets && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetHpBarContainerSizeWithOffsets(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetHpBarContainerSizeWithOffsetsImmediately && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetHpBarContainerSizeWithOffsetsImmediately(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshValues && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshValues();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshMiddleground && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshMiddleground();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshForeground && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshForeground();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshBlockUi && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshBlockUi();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshText && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshText();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsPoisonLethal && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			bool flag = IsPoisonLethal(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.IsDoomLethal && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			bool flag2 = IsDoomLethal(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<bool>(ref flag2);
			return true;
		}
		if ((ref method) == MethodName.GetFgWidth && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			float fgWidth = GetFgWidth(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<float>(ref fgWidth);
			return true;
		}
		if ((ref method) == MethodName.GetFgWidth && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			float fgWidth2 = GetFgWidth(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<float>(ref fgWidth2);
			return true;
		}
		if ((ref method) == MethodName.FadeOutHpLabel && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			FadeOutHpLabel(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FadeInHpLabel && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			FadeInHpLabel(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateInBlock && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			AnimateInBlock(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
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
		if ((ref method) == MethodName.DebugToggleVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateLayoutForCreatureBounds)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateWidthRelativeToReferenceValue)
		{
			return true;
		}
		if ((ref method) == MethodName.SetHpBarContainerSizeWithOffsets)
		{
			return true;
		}
		if ((ref method) == MethodName.SetHpBarContainerSizeWithOffsetsImmediately)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshValues)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshMiddleground)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshForeground)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshBlockUi)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshText)
		{
			return true;
		}
		if ((ref method) == MethodName.IsPoisonLethal)
		{
			return true;
		}
		if ((ref method) == MethodName.IsDoomLethal)
		{
			return true;
		}
		if ((ref method) == MethodName.GetFgWidth)
		{
			return true;
		}
		if ((ref method) == MethodName.FadeOutHpLabel)
		{
			return true;
		}
		if ((ref method) == MethodName.FadeInHpLabel)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateInBlock)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.HpBarContainer)
		{
			HpBarContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hpForegroundContainer)
		{
			_hpForegroundContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hpForeground)
		{
			_hpForeground = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._poisonForeground)
		{
			_poisonForeground = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._doomForeground)
		{
			_doomForeground = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hpMiddleground)
		{
			_hpMiddleground = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hpLabel)
		{
			_hpLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._blockContainer)
		{
			_blockContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._blockLabel)
		{
			_blockLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._blockOutline)
		{
			_blockOutline = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._infinityTex)
		{
			_infinityTex = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._blockTween)
		{
			_blockTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hpLabelFadeTween)
		{
			_hpLabelFadeTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._middlegroundTween)
		{
			_middlegroundTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalBlockPosition)
		{
			_originalBlockPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentHpOnLastRefresh)
		{
			_currentHpOnLastRefresh = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maxHpOnLastRefresh)
		{
			_maxHpOnLastRefresh = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._expectedMaxFgWidth)
		{
			_expectedMaxFgWidth = VariantUtils.ConvertTo<float>(ref value);
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
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.MaxFgWidth)
		{
			float maxFgWidth = MaxFgWidth;
			value = VariantUtils.CreateFrom<float>(ref maxFgWidth);
			return true;
		}
		if ((ref name) == PropertyName.HpBarContainer)
		{
			Control hpBarContainer = HpBarContainer;
			value = VariantUtils.CreateFrom<Control>(ref hpBarContainer);
			return true;
		}
		if ((ref name) == PropertyName._hpForegroundContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _hpForegroundContainer);
			return true;
		}
		if ((ref name) == PropertyName._hpForeground)
		{
			value = VariantUtils.CreateFrom<Control>(ref _hpForeground);
			return true;
		}
		if ((ref name) == PropertyName._poisonForeground)
		{
			value = VariantUtils.CreateFrom<Control>(ref _poisonForeground);
			return true;
		}
		if ((ref name) == PropertyName._doomForeground)
		{
			value = VariantUtils.CreateFrom<Control>(ref _doomForeground);
			return true;
		}
		if ((ref name) == PropertyName._hpMiddleground)
		{
			value = VariantUtils.CreateFrom<Control>(ref _hpMiddleground);
			return true;
		}
		if ((ref name) == PropertyName._hpLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _hpLabel);
			return true;
		}
		if ((ref name) == PropertyName._blockContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _blockContainer);
			return true;
		}
		if ((ref name) == PropertyName._blockLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _blockLabel);
			return true;
		}
		if ((ref name) == PropertyName._blockOutline)
		{
			value = VariantUtils.CreateFrom<Control>(ref _blockOutline);
			return true;
		}
		if ((ref name) == PropertyName._infinityTex)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _infinityTex);
			return true;
		}
		if ((ref name) == PropertyName._blockTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _blockTween);
			return true;
		}
		if ((ref name) == PropertyName._hpLabelFadeTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hpLabelFadeTween);
			return true;
		}
		if ((ref name) == PropertyName._middlegroundTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _middlegroundTween);
			return true;
		}
		if ((ref name) == PropertyName._originalBlockPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _originalBlockPosition);
			return true;
		}
		if ((ref name) == PropertyName._currentHpOnLastRefresh)
		{
			value = VariantUtils.CreateFrom<int>(ref _currentHpOnLastRefresh);
			return true;
		}
		if ((ref name) == PropertyName._maxHpOnLastRefresh)
		{
			value = VariantUtils.CreateFrom<int>(ref _maxHpOnLastRefresh);
			return true;
		}
		if ((ref name) == PropertyName._expectedMaxFgWidth)
		{
			value = VariantUtils.CreateFrom<float>(ref _expectedMaxFgWidth);
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
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._hpForegroundContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hpForeground, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._poisonForeground, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._doomForeground, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hpMiddleground, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hpLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._blockContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._blockLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._blockOutline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infinityTex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._blockTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hpLabelFadeTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._middlegroundTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._originalBlockPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._currentHpOnLastRefresh, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._maxHpOnLastRefresh, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._expectedMaxFgWidth, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.MaxFgWidth, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.HpBarContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName hpBarContainer = PropertyName.HpBarContainer;
		Control hpBarContainer2 = HpBarContainer;
		info.AddProperty(hpBarContainer, Variant.From<Control>(ref hpBarContainer2));
		info.AddProperty(PropertyName._hpForegroundContainer, Variant.From<Control>(ref _hpForegroundContainer));
		info.AddProperty(PropertyName._hpForeground, Variant.From<Control>(ref _hpForeground));
		info.AddProperty(PropertyName._poisonForeground, Variant.From<Control>(ref _poisonForeground));
		info.AddProperty(PropertyName._doomForeground, Variant.From<Control>(ref _doomForeground));
		info.AddProperty(PropertyName._hpMiddleground, Variant.From<Control>(ref _hpMiddleground));
		info.AddProperty(PropertyName._hpLabel, Variant.From<MegaLabel>(ref _hpLabel));
		info.AddProperty(PropertyName._blockContainer, Variant.From<Control>(ref _blockContainer));
		info.AddProperty(PropertyName._blockLabel, Variant.From<MegaLabel>(ref _blockLabel));
		info.AddProperty(PropertyName._blockOutline, Variant.From<Control>(ref _blockOutline));
		info.AddProperty(PropertyName._infinityTex, Variant.From<TextureRect>(ref _infinityTex));
		info.AddProperty(PropertyName._blockTween, Variant.From<Tween>(ref _blockTween));
		info.AddProperty(PropertyName._hpLabelFadeTween, Variant.From<Tween>(ref _hpLabelFadeTween));
		info.AddProperty(PropertyName._middlegroundTween, Variant.From<Tween>(ref _middlegroundTween));
		info.AddProperty(PropertyName._originalBlockPosition, Variant.From<Vector2>(ref _originalBlockPosition));
		info.AddProperty(PropertyName._currentHpOnLastRefresh, Variant.From<int>(ref _currentHpOnLastRefresh));
		info.AddProperty(PropertyName._maxHpOnLastRefresh, Variant.From<int>(ref _maxHpOnLastRefresh));
		info.AddProperty(PropertyName._expectedMaxFgWidth, Variant.From<float>(ref _expectedMaxFgWidth));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.HpBarContainer, ref val))
		{
			HpBarContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._hpForegroundContainer, ref val2))
		{
			_hpForegroundContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._hpForeground, ref val3))
		{
			_hpForeground = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._poisonForeground, ref val4))
		{
			_poisonForeground = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._doomForeground, ref val5))
		{
			_doomForeground = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._hpMiddleground, ref val6))
		{
			_hpMiddleground = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._hpLabel, ref val7))
		{
			_hpLabel = ((Variant)(ref val7)).As<MegaLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._blockContainer, ref val8))
		{
			_blockContainer = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._blockLabel, ref val9))
		{
			_blockLabel = ((Variant)(ref val9)).As<MegaLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._blockOutline, ref val10))
		{
			_blockOutline = ((Variant)(ref val10)).As<Control>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._infinityTex, ref val11))
		{
			_infinityTex = ((Variant)(ref val11)).As<TextureRect>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._blockTween, ref val12))
		{
			_blockTween = ((Variant)(ref val12)).As<Tween>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._hpLabelFadeTween, ref val13))
		{
			_hpLabelFadeTween = ((Variant)(ref val13)).As<Tween>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._middlegroundTween, ref val14))
		{
			_middlegroundTween = ((Variant)(ref val14)).As<Tween>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalBlockPosition, ref val15))
		{
			_originalBlockPosition = ((Variant)(ref val15)).As<Vector2>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentHpOnLastRefresh, ref val16))
		{
			_currentHpOnLastRefresh = ((Variant)(ref val16)).As<int>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxHpOnLastRefresh, ref val17))
		{
			_maxHpOnLastRefresh = ((Variant)(ref val17)).As<int>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._expectedMaxFgWidth, ref val18))
		{
			_expectedMaxFgWidth = ((Variant)(ref val18)).As<float>();
		}
	}
}
