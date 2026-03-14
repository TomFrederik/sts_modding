using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Bestiary;

[ScriptPath("res://src/Core/Nodes/Screens/Bestiary/NBestiary.cs")]
public class NBestiary : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public static readonly StringName CreateEntries = StringName.op_Implicit("CreateEntries");

		public static readonly StringName OnMonsterClicked = StringName.op_Implicit("OnMonsterClicked");

		public static readonly StringName SelectMonster = StringName.op_Implicit("SelectMonster");

		public static readonly StringName OnMoveButtonFocused = StringName.op_Implicit("OnMoveButtonFocused");

		public static readonly StringName OnMoveButtonUnfocused = StringName.op_Implicit("OnMoveButtonUnfocused");

		public static readonly StringName OnMoveButtonClicked = StringName.op_Implicit("OnMoveButtonClicked");

		public static readonly StringName PlayIdleAnim = StringName.op_Implicit("PlayIdleAnim");

		public static readonly StringName PlayMoveAnim = StringName.op_Implicit("PlayMoveAnim");

		public static readonly StringName OnMoveAnimCompleted = StringName.op_Implicit("OnMoveAnimCompleted");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _monsterNameLabel = StringName.op_Implicit("_monsterNameLabel");

		public static readonly StringName _epithet = StringName.op_Implicit("_epithet");

		public static readonly StringName _sidebar = StringName.op_Implicit("_sidebar");

		public static readonly StringName _bestiaryList = StringName.op_Implicit("_bestiaryList");

		public static readonly StringName _monsterVisualsContainer = StringName.op_Implicit("_monsterVisualsContainer");

		public static readonly StringName _descriptionLabel = StringName.op_Implicit("_descriptionLabel");

		public static readonly StringName _selectionArrow = StringName.op_Implicit("_selectionArrow");

		public static readonly StringName _moveList = StringName.op_Implicit("_moveList");

		public static readonly StringName _moveContainer = StringName.op_Implicit("_moveContainer");

		public static readonly StringName _arrowTween = StringName.op_Implicit("_arrowTween");

		public static readonly StringName _arrowPosReset = StringName.op_Implicit("_arrowPosReset");

		public static readonly StringName _monsterVisuals = StringName.op_Implicit("_monsterVisuals");

		public static readonly StringName _selectedEntry = StringName.op_Implicit("_selectedEntry");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _isPlayingMoveAnim = StringName.op_Implicit("_isPlayingMoveAnim");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/bestiary/bestiary");

	private MegaRichTextLabel _monsterNameLabel;

	private MegaLabel _epithet;

	private NScrollableContainer _sidebar;

	private VBoxContainer _bestiaryList;

	private static readonly LocString _locked = new LocString("bestiary", "LOCKED.monsterTitle");

	private Control _monsterVisualsContainer;

	private static readonly LocString _placeholderDesc = new LocString("bestiary", "DESCRIPTION.placeholder");

	private MegaRichTextLabel _descriptionLabel;

	private Control _selectionArrow;

	private Control _moveList;

	private Control _moveContainer;

	private Tween? _arrowTween;

	private static readonly Vector2 _arrowOffset = new Vector2(-34f, 4f);

	private bool _arrowPosReset = true;

	private NCreatureVisuals? _monsterVisuals;

	private MegaSprite? _animController;

	private MegaAnimationState? _animState;

	private NBestiaryEntry? _selectedEntry;

	private Tween? _tween;

	private bool _isPlayingMoveAnim;

	protected override Control? InitialFocusedControl => (Control?)(object)((IEnumerable)((Node)_bestiaryList).GetChildren(false)).OfType<NBestiaryEntry>().FirstOrDefault();

	public static string[] AssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.Add(_scenePath);
			list.AddRange(NBestiaryEntry.AssetPaths);
			return list.ToArray();
		}
	}

	public static NBestiary? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NBestiary>((GenEditState)0);
	}

	public override void _Ready()
	{
		ConnectSignals();
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%MoveHeader")).SetTextAutoSize(new LocString("bestiary", "ACTIONS.header").GetFormattedText());
		_sidebar = ((Node)this).GetNode<NScrollableContainer>(NodePath.op_Implicit("%Sidebar"));
		_bestiaryList = ((Node)this).GetNode<VBoxContainer>(NodePath.op_Implicit("%BestiaryList"));
		_monsterNameLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%MonsterName"));
		_epithet = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Epithet"));
		_descriptionLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Description"));
		_moveContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%MoveContainer"));
		_selectionArrow = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%SelectionArrow"));
		_monsterVisualsContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%MonsterVisualsContainer"));
		_moveList = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%MoveList"));
	}

	public override void OnSubmenuOpened()
	{
		CreateEntries();
	}

	public override void OnSubmenuClosed()
	{
		_selectedEntry = null;
		((Node)(object)_monsterVisuals)?.QueueFreeSafely();
		_monsterVisuals = null;
		foreach (Node child in ((Node)_bestiaryList).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
	}

	private void CreateEntries()
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		HashSet<ModelId> hashSet = (from e in SaveManager.Instance.Progress.EnemyStats.Values
			where e.TotalWins > 0
			select e.Id).ToHashSet();
		foreach (MonsterModel item in ModelDb.Monsters.OrderBy((MonsterModel m) => m.Id.Entry))
		{
			bool flag = hashSet.Contains(item.Id);
			NBestiaryEntry nBestiaryEntry = NBestiaryEntry.Create(item, !flag);
			((Node)(object)_bestiaryList).AddChildSafely((Node?)(object)nBestiaryEntry);
			((GodotObject)nBestiaryEntry).Connect(NClickableControl.SignalName.Released, Callable.From<NBestiaryEntry>((Action<NBestiaryEntry>)OnMonsterClicked), 0u);
		}
		_sidebar.InstantlyScrollToTop();
		SelectMonster(((Node)_bestiaryList).GetChild<NBestiaryEntry>(0, false));
	}

	private void OnMonsterClicked(NBestiaryEntry entry)
	{
		SelectMonster(entry);
	}

	private void SelectMonster(NBestiaryEntry entry)
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		if (entry == _selectedEntry)
		{
			return;
		}
		((Node)(object)_moveList).FreeChildren();
		_arrowPosReset = true;
		_selectedEntry?.Deselect();
		_selectedEntry = entry;
		_selectedEntry.Select();
		MonsterModel monster = _selectedEntry.Monster;
		if (entry.IsLocked)
		{
			_monsterNameLabel.Text = _locked.GetFormattedText();
			_descriptionLabel.Text = _placeholderDesc.GetFormattedText();
			((Node)(object)_monsterVisuals)?.QueueFreeSafely();
			_monsterVisuals = null;
			((CanvasItem)_moveContainer).Visible = false;
			return;
		}
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_descriptionLabel.Text = _placeholderDesc.GetFormattedText();
		((CanvasItem)_descriptionLabel).Modulate = StsColors.transparentWhite;
		_monsterNameLabel.Text = monster.Title.GetFormattedText();
		((CanvasItem)_monsterNameLabel).SelfModulate = StsColors.transparentWhite;
		((CanvasItem)_epithet).Modulate = StsColors.transparentWhite;
		((CanvasItem)_moveContainer).Modulate = StsColors.transparentWhite;
		_tween.TweenProperty((GodotObject)(object)_monsterNameLabel, NodePath.op_Implicit("position:y"), Variant.op_Implicit(88f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(24f));
		_tween.TweenProperty((GodotObject)(object)_monsterNameLabel, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(1f), 0.5);
		_tween.TweenProperty((GodotObject)(object)_epithet, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetDelay(0.2);
		_tween.TweenProperty((GodotObject)(object)_descriptionLabel, NodePath.op_Implicit("position:y"), Variant.op_Implicit(894f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(958f));
		_tween.TweenProperty((GodotObject)(object)_descriptionLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5);
		_tween.TweenProperty((GodotObject)(object)_moveContainer, NodePath.op_Implicit("position:x"), Variant.op_Implicit(242f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(210f))
			.SetDelay(0.2);
		_tween.TweenProperty((GodotObject)(object)_moveContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetDelay(0.2);
		((Node)(object)_monsterVisuals)?.QueueFreeSafely();
		_monsterVisuals = monster.CreateVisuals();
		((Node)(object)_monsterVisualsContainer).AddChildSafely((Node?)(object)_monsterVisuals);
		((Node2D)_monsterVisuals).Position = new Vector2(0f, _monsterVisuals.Bounds.Size.Y * 0.5f);
		((CanvasItem)_monsterVisuals).Modulate = StsColors.transparentBlack;
		_tween.TweenProperty((GodotObject)(object)_monsterVisuals, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25);
		_isPlayingMoveAnim = false;
		if (_monsterVisuals.HasSpineAnimation)
		{
			((CanvasItem)_moveContainer).Visible = true;
			_animController = _monsterVisuals.SpineBody;
			_animState = _animController.GetAnimationState();
			monster.GenerateAnimator(_animController);
			_monsterVisuals.SetUpSkin(monster);
			PlayIdleAnim();
			{
				foreach (BestiaryMonsterMove item in monster.MonsterMoveList(_monsterVisuals))
				{
					NBestiaryMoveButton nBestiaryMoveButton = NBestiaryMoveButton.Create(item);
					((Node)(object)_moveList).AddChildSafely((Node?)(object)nBestiaryMoveButton);
					((GodotObject)nBestiaryMoveButton).Connect(NClickableControl.SignalName.Focused, Callable.From<NBestiaryMoveButton>((Action<NBestiaryMoveButton>)OnMoveButtonFocused), 0u);
					((GodotObject)nBestiaryMoveButton).Connect(NClickableControl.SignalName.Unfocused, Callable.From<NBestiaryMoveButton>((Action<NBestiaryMoveButton>)OnMoveButtonUnfocused), 0u);
					((GodotObject)nBestiaryMoveButton).Connect(NClickableControl.SignalName.Released, Callable.From<NBestiaryMoveButton>((Action<NBestiaryMoveButton>)OnMoveButtonClicked), 0u);
				}
				return;
			}
		}
		((CanvasItem)_moveContainer).Visible = false;
	}

	private void OnMoveButtonFocused(NBestiaryMoveButton button)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (_arrowPosReset)
		{
			_selectionArrow.GlobalPosition = ((Control)button).GlobalPosition + _arrowOffset;
			_arrowPosReset = false;
		}
		Tween? arrowTween = _arrowTween;
		if (arrowTween != null)
		{
			arrowTween.Kill();
		}
		_arrowTween = ((Node)this).CreateTween().SetParallel(true);
		_arrowTween.TweenProperty((GodotObject)(object)_selectionArrow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.05);
		_arrowTween.TweenProperty((GodotObject)(object)_selectionArrow, NodePath.op_Implicit("global_position"), Variant.op_Implicit(((Control)button).GlobalPosition + _arrowOffset), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	private void OnMoveButtonUnfocused(NBestiaryMoveButton button)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Tween? arrowTween = _arrowTween;
		if (arrowTween != null)
		{
			arrowTween.Kill();
		}
		_arrowTween = ((Node)this).CreateTween().SetParallel(true);
		_arrowTween.TweenProperty((GodotObject)(object)_selectionArrow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
	}

	private void OnMoveButtonClicked(NButton button)
	{
		NBestiaryMoveButton nBestiaryMoveButton = (NBestiaryMoveButton)button;
		PlayMoveAnim(nBestiaryMoveButton.Move.animId);
		nBestiaryMoveButton.PlaySfx();
	}

	private void PlayIdleAnim()
	{
		if (_monsterVisuals != null && _monsterVisuals.HasSpineAnimation)
		{
			_isPlayingMoveAnim = false;
			_animState.SetAnimation("idle_loop");
		}
	}

	private void PlayMoveAnim(string animId)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (_monsterVisuals != null && _monsterVisuals.HasSpineAnimation)
		{
			_animState.SetAnimation(animId, loop: false);
			if (!_isPlayingMoveAnim)
			{
				_animController.ConnectAnimationCompleted(Callable.From<GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject>)OnMoveAnimCompleted));
			}
			_isPlayingMoveAnim = true;
		}
	}

	private void OnMoveAnimCompleted(GodotObject _, GodotObject __, GodotObject ___)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (_isPlayingMoveAnim)
		{
			_isPlayingMoveAnim = false;
			_animController.DisconnectAnimationCompleted(Callable.From<GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject>)OnMoveAnimCompleted));
			PlayIdleAnim();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Expected O, but got Unknown
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Expected O, but got Unknown
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Expected O, but got Unknown
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Expected O, but got Unknown
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Expected O, but got Unknown
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateEntries, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMonsterClicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("entry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SelectMonster, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("entry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMoveButtonFocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMoveButtonUnfocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMoveButtonClicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayIdleAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayMoveAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("animId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMoveAnimCompleted, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NBestiary nBestiary = Create();
			ret = VariantUtils.CreateFrom<NBestiary>(ref nBestiary);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CreateEntries && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CreateEntries();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMonsterClicked && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMonsterClicked(VariantUtils.ConvertTo<NBestiaryEntry>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SelectMonster && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SelectMonster(VariantUtils.ConvertTo<NBestiaryEntry>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMoveButtonFocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMoveButtonFocused(VariantUtils.ConvertTo<NBestiaryMoveButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMoveButtonUnfocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMoveButtonUnfocused(VariantUtils.ConvertTo<NBestiaryMoveButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMoveButtonClicked && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMoveButtonClicked(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayIdleAnim && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayIdleAnim();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayMoveAnim && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PlayMoveAnim(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMoveAnimCompleted && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			OnMoveAnimCompleted(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]));
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
			NBestiary nBestiary = Create();
			ret = VariantUtils.CreateFrom<NBestiary>(ref nBestiary);
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
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.CreateEntries)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMonsterClicked)
		{
			return true;
		}
		if ((ref method) == MethodName.SelectMonster)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMoveButtonFocused)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMoveButtonUnfocused)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMoveButtonClicked)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayIdleAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayMoveAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMoveAnimCompleted)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._monsterNameLabel)
		{
			_monsterNameLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._epithet)
		{
			_epithet = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sidebar)
		{
			_sidebar = VariantUtils.ConvertTo<NScrollableContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bestiaryList)
		{
			_bestiaryList = VariantUtils.ConvertTo<VBoxContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._monsterVisualsContainer)
		{
			_monsterVisualsContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._descriptionLabel)
		{
			_descriptionLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectionArrow)
		{
			_selectionArrow = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._moveList)
		{
			_moveList = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._moveContainer)
		{
			_moveContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._arrowTween)
		{
			_arrowTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._arrowPosReset)
		{
			_arrowPosReset = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._monsterVisuals)
		{
			_monsterVisuals = VariantUtils.ConvertTo<NCreatureVisuals>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectedEntry)
		{
			_selectedEntry = VariantUtils.ConvertTo<NBestiaryEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isPlayingMoveAnim)
		{
			_isPlayingMoveAnim = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._monsterNameLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _monsterNameLabel);
			return true;
		}
		if ((ref name) == PropertyName._epithet)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _epithet);
			return true;
		}
		if ((ref name) == PropertyName._sidebar)
		{
			value = VariantUtils.CreateFrom<NScrollableContainer>(ref _sidebar);
			return true;
		}
		if ((ref name) == PropertyName._bestiaryList)
		{
			value = VariantUtils.CreateFrom<VBoxContainer>(ref _bestiaryList);
			return true;
		}
		if ((ref name) == PropertyName._monsterVisualsContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _monsterVisualsContainer);
			return true;
		}
		if ((ref name) == PropertyName._descriptionLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _descriptionLabel);
			return true;
		}
		if ((ref name) == PropertyName._selectionArrow)
		{
			value = VariantUtils.CreateFrom<Control>(ref _selectionArrow);
			return true;
		}
		if ((ref name) == PropertyName._moveList)
		{
			value = VariantUtils.CreateFrom<Control>(ref _moveList);
			return true;
		}
		if ((ref name) == PropertyName._moveContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _moveContainer);
			return true;
		}
		if ((ref name) == PropertyName._arrowTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _arrowTween);
			return true;
		}
		if ((ref name) == PropertyName._arrowPosReset)
		{
			value = VariantUtils.CreateFrom<bool>(ref _arrowPosReset);
			return true;
		}
		if ((ref name) == PropertyName._monsterVisuals)
		{
			value = VariantUtils.CreateFrom<NCreatureVisuals>(ref _monsterVisuals);
			return true;
		}
		if ((ref name) == PropertyName._selectedEntry)
		{
			value = VariantUtils.CreateFrom<NBestiaryEntry>(ref _selectedEntry);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._isPlayingMoveAnim)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isPlayingMoveAnim);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
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
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._monsterNameLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._epithet, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sidebar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bestiaryList, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._monsterVisualsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._descriptionLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionArrow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._moveList, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._moveContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._arrowTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._arrowPosReset, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._monsterVisuals, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectedEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isPlayingMoveAnim, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._monsterNameLabel, Variant.From<MegaRichTextLabel>(ref _monsterNameLabel));
		info.AddProperty(PropertyName._epithet, Variant.From<MegaLabel>(ref _epithet));
		info.AddProperty(PropertyName._sidebar, Variant.From<NScrollableContainer>(ref _sidebar));
		info.AddProperty(PropertyName._bestiaryList, Variant.From<VBoxContainer>(ref _bestiaryList));
		info.AddProperty(PropertyName._monsterVisualsContainer, Variant.From<Control>(ref _monsterVisualsContainer));
		info.AddProperty(PropertyName._descriptionLabel, Variant.From<MegaRichTextLabel>(ref _descriptionLabel));
		info.AddProperty(PropertyName._selectionArrow, Variant.From<Control>(ref _selectionArrow));
		info.AddProperty(PropertyName._moveList, Variant.From<Control>(ref _moveList));
		info.AddProperty(PropertyName._moveContainer, Variant.From<Control>(ref _moveContainer));
		info.AddProperty(PropertyName._arrowTween, Variant.From<Tween>(ref _arrowTween));
		info.AddProperty(PropertyName._arrowPosReset, Variant.From<bool>(ref _arrowPosReset));
		info.AddProperty(PropertyName._monsterVisuals, Variant.From<NCreatureVisuals>(ref _monsterVisuals));
		info.AddProperty(PropertyName._selectedEntry, Variant.From<NBestiaryEntry>(ref _selectedEntry));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._isPlayingMoveAnim, Variant.From<bool>(ref _isPlayingMoveAnim));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._monsterNameLabel, ref val))
		{
			_monsterNameLabel = ((Variant)(ref val)).As<MegaRichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._epithet, ref val2))
		{
			_epithet = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._sidebar, ref val3))
		{
			_sidebar = ((Variant)(ref val3)).As<NScrollableContainer>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._bestiaryList, ref val4))
		{
			_bestiaryList = ((Variant)(ref val4)).As<VBoxContainer>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._monsterVisualsContainer, ref val5))
		{
			_monsterVisualsContainer = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._descriptionLabel, ref val6))
		{
			_descriptionLabel = ((Variant)(ref val6)).As<MegaRichTextLabel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionArrow, ref val7))
		{
			_selectionArrow = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._moveList, ref val8))
		{
			_moveList = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._moveContainer, ref val9))
		{
			_moveContainer = ((Variant)(ref val9)).As<Control>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._arrowTween, ref val10))
		{
			_arrowTween = ((Variant)(ref val10)).As<Tween>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._arrowPosReset, ref val11))
		{
			_arrowPosReset = ((Variant)(ref val11)).As<bool>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._monsterVisuals, ref val12))
		{
			_monsterVisuals = ((Variant)(ref val12)).As<NCreatureVisuals>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectedEntry, ref val13))
		{
			_selectedEntry = ((Variant)(ref val13)).As<NBestiaryEntry>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val14))
		{
			_tween = ((Variant)(ref val14)).As<Tween>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._isPlayingMoveAnim, ref val15))
		{
			_isPlayingMoveAnim = ((Variant)(ref val15)).As<bool>();
		}
	}
}
