using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Events.Custom.CrystalSphereEvent;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Events.Custom.CrystalSphere;

[ScriptPath("res://src/Core/Nodes/Events/Custom/CrystalSphere/NCrystalSphereScreen.cs")]
public class NCrystalSphereScreen : Control, IOverlayScreen, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetBigDivination = StringName.op_Implicit("SetBigDivination");

		public static readonly StringName SetSmallDivination = StringName.op_Implicit("SetSmallDivination");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnHoverCell = StringName.op_Implicit("OnHoverCell");

		public static readonly StringName OnUnhoverCell = StringName.op_Implicit("OnUnhoverCell");

		public static readonly StringName UpdateDivinationsLeft = StringName.op_Implicit("UpdateDivinationsLeft");

		public static readonly StringName OnMinigameFinished = StringName.op_Implicit("OnMinigameFinished");

		public static readonly StringName OnProceedButtonPressed = StringName.op_Implicit("OnProceedButtonPressed");

		public static readonly StringName AfterOverlayOpened = StringName.op_Implicit("AfterOverlayOpened");

		public static readonly StringName AfterOverlayClosed = StringName.op_Implicit("AfterOverlayClosed");

		public static readonly StringName AfterOverlayShown = StringName.op_Implicit("AfterOverlayShown");

		public static readonly StringName AfterOverlayHidden = StringName.op_Implicit("AfterOverlayHidden");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public static readonly StringName UseSharedBackstop = StringName.op_Implicit("UseSharedBackstop");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _itemsContainer = StringName.op_Implicit("_itemsContainer");

		public static readonly StringName _cellContainer = StringName.op_Implicit("_cellContainer");

		public static readonly StringName _bigDivinationButton = StringName.op_Implicit("_bigDivinationButton");

		public static readonly StringName _smallDivinationButton = StringName.op_Implicit("_smallDivinationButton");

		public static readonly StringName _divinationsLeftLabel = StringName.op_Implicit("_divinationsLeftLabel");

		public static readonly StringName _mask = StringName.op_Implicit("_mask");

		public static readonly StringName _proceedButton = StringName.op_Implicit("_proceedButton");

		public static readonly StringName _instructionsTitleLabel = StringName.op_Implicit("_instructionsTitleLabel");

		public static readonly StringName _instructionsDescriptionLabel = StringName.op_Implicit("_instructionsDescriptionLabel");

		public static readonly StringName _instructionsContainer = StringName.op_Implicit("_instructionsContainer");

		public static readonly StringName _dialogue = StringName.op_Implicit("_dialogue");

		public static readonly StringName _fadeTween = StringName.op_Implicit("_fadeTween");
	}

	public class SignalName : SignalName
	{
	}

	private readonly LocString _instructionsTitleLoc = new LocString("events", "CRYSTAL_SPHERE.minigame.instructions.title");

	private readonly LocString _instructionsDescriptionLoc = new LocString("events", "CRYSTAL_SPHERE.minigame.instructions.description");

	private readonly LocString _divinationsRemainLoc = new LocString("events", "CRYSTAL_SPHERE.minigame.divinationsRemain");

	private const string _scenePath = "res://scenes/events/custom/crystal_sphere/crystal_sphere_screen.tscn";

	private CrystalSphereMinigame _entity;

	private Control _itemsContainer;

	private Control _cellContainer;

	private NDivinationButton _bigDivinationButton;

	private NDivinationButton _smallDivinationButton;

	private MegaRichTextLabel _divinationsLeftLabel;

	private NCrystalSphereMask _mask;

	private NProceedButton _proceedButton;

	private MegaRichTextLabel _instructionsTitleLabel;

	private MegaRichTextLabel _instructionsDescriptionLabel;

	private Control _instructionsContainer;

	private NCrystalSphereDialogue _dialogue;

	private Tween? _fadeTween;

	public NetScreenType ScreenType => NetScreenType.None;

	public bool UseSharedBackstop => false;

	public Control DefaultFocusedControl
	{
		get
		{
			List<NCrystalSphereCell> list = (from c in ((IEnumerable)((Node)_cellContainer).GetChildren(false)).OfType<NCrystalSphereCell>()
				where c.Entity.IsHidden
				select c).ToList();
			return (Control)(object)list[list.Count / 2];
		}
	}

	public static NCrystalSphereScreen ShowScreen(CrystalSphereMinigame grid)
	{
		NCrystalSphereScreen nCrystalSphereScreen = PreloadManager.Cache.GetScene("res://scenes/events/custom/crystal_sphere/crystal_sphere_screen.tscn").Instantiate<NCrystalSphereScreen>((GenEditState)0);
		nCrystalSphereScreen._entity = grid;
		NOverlayStack.Instance.Push(nCrystalSphereScreen);
		return nCrystalSphereScreen;
	}

	public override void _Ready()
	{
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		_itemsContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Items"));
		_cellContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Cells"));
		_bigDivinationButton = ((Node)this).GetNode<NDivinationButton>(NodePath.op_Implicit("%BigDivinationButton"));
		_smallDivinationButton = ((Node)this).GetNode<NDivinationButton>(NodePath.op_Implicit("%SmallDivinationButton"));
		_bigDivinationButton.SetLabel(new LocString("events", "CRYSTAL_SPHERE.button.DIVINATION_LABEL_BIG"));
		_smallDivinationButton.SetLabel(new LocString("events", "CRYSTAL_SPHERE.button.DIVINATION_LABEL_SMALL"));
		_divinationsLeftLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%DivinationsLeft"));
		_mask = ((Node)this).GetNode<NCrystalSphereMask>(NodePath.op_Implicit("%ScryMask"));
		_proceedButton = ((Node)this).GetNode<NProceedButton>(NodePath.op_Implicit("%ProceedButton"));
		_instructionsTitleLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%InstructionsTitle"));
		_instructionsDescriptionLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%InstructionsDescription"));
		_instructionsContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Instructions"));
		_instructionsTitleLabel.SetTextAutoSize(_instructionsTitleLoc.GetFormattedText());
		_instructionsDescriptionLabel.SetTextAutoSize(_instructionsDescriptionLoc.GetFormattedText());
		_dialogue = ((Node)this).GetNode<NCrystalSphereDialogue>(NodePath.op_Implicit("%Dialogue"));
		Vector2 val = Vector2.One * (float)(-(57 * _entity.GridSize.X)) * 0.5f;
		NCrystalSphereCell[,] array = new NCrystalSphereCell[_entity.GridSize.X, _entity.GridSize.Y];
		for (int i = 0; i < _entity.GridSize.X; i++)
		{
			for (int j = 0; j < _entity.GridSize.Y; j++)
			{
				NCrystalSphereCell cell = NCrystalSphereCell.Create(_entity.cells[i, j], _mask);
				((Node)(object)_cellContainer).AddChildSafely((Node?)(object)cell);
				array[i, j] = cell;
				((Control)cell).Position = val + 57f * new Vector2((float)i, (float)j);
				((GodotObject)cell).Connect(SignalName.MouseEntered, Callable.From((Action)delegate
				{
					OnHoverCell(cell);
				}), 0u);
				((GodotObject)cell).Connect(SignalName.MouseExited, Callable.From((Action)delegate
				{
					OnUnhoverCell(cell);
				}), 0u);
				((GodotObject)cell).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
				{
					OnHoverCell(cell);
				}), 0u);
				((GodotObject)cell).Connect(SignalName.FocusExited, Callable.From((Action)delegate
				{
					OnUnhoverCell(cell);
				}), 0u);
				((GodotObject)cell).Connect(NClickableControl.SignalName.MouseReleased, Callable.From<InputEvent>((Action<InputEvent>)delegate
				{
					TaskHelper.RunSafely(OnCellClicked(cell));
				}), 0u);
				((GodotObject)cell).Connect(NClickableControl.SignalName.Released, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
				{
					TaskHelper.RunSafely(OnCellClicked(cell));
				}), 0u);
			}
		}
		foreach (CrystalSphereItem item in _entity.Items)
		{
			NCrystalSphereItem nCrystalSphereItem = NCrystalSphereItem.Create(item);
			((Control)nCrystalSphereItem).Size = Vector2I.op_Implicit(item.Size * 57);
			((Node)(object)_itemsContainer).AddChildSafely((Node?)(object)nCrystalSphereItem);
			((Control)nCrystalSphereItem).Position = val + 57f * new Vector2((float)item.Position.X, (float)item.Position.Y);
			item.Revealed += OnItemRevealed;
		}
		((GodotObject)_bigDivinationButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)SetBigDivination), 0u);
		((GodotObject)_smallDivinationButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)SetSmallDivination), 0u);
		((GodotObject)_proceedButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnProceedButtonPressed), 0u);
		_smallDivinationButton.SetHotkeys(new string[1] { StringName.op_Implicit(MegaInput.viewExhaustPileAndTabRight) });
		_bigDivinationButton.SetHotkeys(new string[1] { StringName.op_Implicit(MegaInput.viewDeckAndTabLeft) });
		UpdateDivinationsLeft();
		_proceedButton.Disable();
		_proceedButton.UpdateText(NProceedButton.ProceedLoc);
		for (int num = 0; num < array.GetLength(0); num++)
		{
			for (int num2 = 0; num2 < array.GetLength(1); num2++)
			{
				Control val2 = (Control)(object)array[num, num2];
				val2.FocusNeighborTop = ((num2 > 0) ? ((Node)array[num, num2 - 1]).GetPath() : ((Node)array[num, num2]).GetPath());
				val2.FocusNeighborBottom = ((num2 < array.GetLength(1) - 1) ? ((Node)array[num, num2 + 1]).GetPath() : ((Node)array[num, num2]).GetPath());
				val2.FocusNeighborLeft = ((num > 0) ? ((Node)array[num - 1, num2]).GetPath() : ((Node)array[num, num2]).GetPath());
				val2.FocusNeighborRight = ((num < array.GetLength(0) - 1) ? ((Node)array[num + 1, num2]).GetPath() : ((Node)array[num, num2]).GetPath());
			}
		}
	}

	private void SetBigDivination(NButton obj)
	{
		_bigDivinationButton.SetActive(isActive: true);
		_smallDivinationButton.SetActive(isActive: false);
		_entity.SetTool(CrystalSphereMinigame.CrystalSphereToolType.Big);
	}

	private void SetSmallDivination(NButton obj)
	{
		_smallDivinationButton.SetActive(isActive: true);
		_bigDivinationButton.SetActive(isActive: false);
		_entity.SetTool(CrystalSphereMinigame.CrystalSphereToolType.Small);
	}

	public override void _EnterTree()
	{
		_entity.DivinationCountChanged += UpdateDivinationsLeft;
		_entity.Finished += OnMinigameFinished;
	}

	public override void _ExitTree()
	{
		_entity.DivinationCountChanged -= UpdateDivinationsLeft;
		_entity.Finished -= OnMinigameFinished;
		Tween? fadeTween = _fadeTween;
		if (fadeTween != null)
		{
			fadeTween.Kill();
		}
		foreach (CrystalSphereItem item in _entity.Items)
		{
			item.Revealed -= OnItemRevealed;
		}
		_entity.ForceMinigameEnd();
	}

	private void OnItemRevealed(CrystalSphereItem item)
	{
		if (item.IsGood)
		{
			_dialogue.PlayGood();
		}
		else
		{
			_dialogue.PlayBad();
		}
	}

	private async Task OnCellClicked(NCrystalSphereCell cell)
	{
		if (_entity.DivinationCount > 0)
		{
			UpdateDivinationsLeft();
			await _entity.CellClicked(cell.Entity);
			List<NCrystalSphereCell> source = (from c in ((IEnumerable)((Node)_cellContainer).GetChildren(false)).OfType<NCrystalSphereCell>()
				where c.Entity.IsHidden
				select c).ToList();
			((Control)(object)source.OrderBy(delegate(NCrystalSphereCell c1)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				Vector2I val = new Vector2I(cell.Entity.X, cell.Entity.Y);
				return ((Vector2I)(ref val)).DistanceTo(new Vector2I(c1.Entity.X, c1.Entity.Y));
			}).First()).TryGrabFocus();
		}
	}

	private void OnHoverCell(NCrystalSphereCell cell)
	{
		if (!_entity.IsFinished)
		{
			_entity.SetHoveredCell(cell.Entity);
		}
	}

	private void OnUnhoverCell(NCrystalSphereCell cell)
	{
		_entity.UnsetHoveredCell();
	}

	private void UpdateDivinationsLeft()
	{
		_divinationsRemainLoc.Add("Count", _entity.DivinationCount);
		_divinationsLeftLabel.Text = _divinationsRemainLoc.GetFormattedText() ?? "";
	}

	private void OnMinigameFinished()
	{
		((CanvasItem)_bigDivinationButton).Visible = false;
		((CanvasItem)_smallDivinationButton).Visible = false;
		((CanvasItem)_divinationsLeftLabel).Visible = false;
		((CanvasItem)_instructionsContainer).Visible = false;
		((CanvasItem)_proceedButton).Visible = true;
		_dialogue.PlayEnd();
		_proceedButton.Enable();
		NMapScreen.Instance.SetTravelEnabled(enabled: true);
	}

	private void OnProceedButtonPressed(NButton _)
	{
		TaskHelper.RunSafely(RunManager.Instance.ProceedFromTerminalRewardsScreen());
	}

	public void AfterOverlayOpened()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_itemsContainer).Visible = false;
		_fadeTween?.FastForwardToCompletion();
		_fadeTween = ((Node)this).CreateTween();
		_fadeTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1.0), 0.5).From(Variant.op_Implicit(0f));
		_fadeTween.Chain().TweenCallback(Callable.From((Action)delegate
		{
			_dialogue.PlayStart();
			((CanvasItem)_itemsContainer).Visible = true;
		}));
	}

	public void AfterOverlayClosed()
	{
		_fadeTween?.FastForwardToCompletion();
		((Node)(object)this).QueueFreeSafely();
	}

	public void AfterOverlayShown()
	{
		if (_entity.IsFinished)
		{
			_proceedButton.Enable();
		}
	}

	public void AfterOverlayHidden()
	{
		_proceedButton.Disable();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Expected O, but got Unknown
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(14);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetBigDivination, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("obj"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetSmallDivination, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("obj"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHoverCell, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cell"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnhoverCell, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cell"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateDivinationsLeft, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMinigameFinished, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnProceedButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetBigDivination && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetBigDivination(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetSmallDivination && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetSmallDivination(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.OnHoverCell && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnHoverCell(VariantUtils.ConvertTo<NCrystalSphereCell>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnhoverCell && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnUnhoverCell(VariantUtils.ConvertTo<NCrystalSphereCell>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateDivinationsLeft && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateDivinationsLeft();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMinigameFinished && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnMinigameFinished();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnProceedButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnProceedButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayShown();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayHidden && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayHidden();
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
		if ((ref method) == MethodName.SetBigDivination)
		{
			return true;
		}
		if ((ref method) == MethodName.SetSmallDivination)
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
		if ((ref method) == MethodName.OnHoverCell)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnhoverCell)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateDivinationsLeft)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMinigameFinished)
		{
			return true;
		}
		if ((ref method) == MethodName.OnProceedButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayShown)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayHidden)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._itemsContainer)
		{
			_itemsContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cellContainer)
		{
			_cellContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bigDivinationButton)
		{
			_bigDivinationButton = VariantUtils.ConvertTo<NDivinationButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._smallDivinationButton)
		{
			_smallDivinationButton = VariantUtils.ConvertTo<NDivinationButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._divinationsLeftLabel)
		{
			_divinationsLeftLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mask)
		{
			_mask = VariantUtils.ConvertTo<NCrystalSphereMask>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			_proceedButton = VariantUtils.ConvertTo<NProceedButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._instructionsTitleLabel)
		{
			_instructionsTitleLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._instructionsDescriptionLabel)
		{
			_instructionsDescriptionLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._instructionsContainer)
		{
			_instructionsContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dialogue)
		{
			_dialogue = VariantUtils.ConvertTo<NCrystalSphereDialogue>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			_fadeTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ScreenType)
		{
			NetScreenType screenType = ScreenType;
			value = VariantUtils.CreateFrom<NetScreenType>(ref screenType);
			return true;
		}
		if ((ref name) == PropertyName.UseSharedBackstop)
		{
			bool useSharedBackstop = UseSharedBackstop;
			value = VariantUtils.CreateFrom<bool>(ref useSharedBackstop);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._itemsContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _itemsContainer);
			return true;
		}
		if ((ref name) == PropertyName._cellContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cellContainer);
			return true;
		}
		if ((ref name) == PropertyName._bigDivinationButton)
		{
			value = VariantUtils.CreateFrom<NDivinationButton>(ref _bigDivinationButton);
			return true;
		}
		if ((ref name) == PropertyName._smallDivinationButton)
		{
			value = VariantUtils.CreateFrom<NDivinationButton>(ref _smallDivinationButton);
			return true;
		}
		if ((ref name) == PropertyName._divinationsLeftLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _divinationsLeftLabel);
			return true;
		}
		if ((ref name) == PropertyName._mask)
		{
			value = VariantUtils.CreateFrom<NCrystalSphereMask>(ref _mask);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			value = VariantUtils.CreateFrom<NProceedButton>(ref _proceedButton);
			return true;
		}
		if ((ref name) == PropertyName._instructionsTitleLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _instructionsTitleLabel);
			return true;
		}
		if ((ref name) == PropertyName._instructionsDescriptionLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _instructionsDescriptionLabel);
			return true;
		}
		if ((ref name) == PropertyName._instructionsContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _instructionsContainer);
			return true;
		}
		if ((ref name) == PropertyName._dialogue)
		{
			value = VariantUtils.CreateFrom<NCrystalSphereDialogue>(ref _dialogue);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _fadeTween);
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
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._itemsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cellContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bigDivinationButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._smallDivinationButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._divinationsLeftLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mask, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._proceedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._instructionsTitleLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._instructionsDescriptionLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._instructionsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dialogue, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fadeTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.ScreenType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.UseSharedBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._itemsContainer, Variant.From<Control>(ref _itemsContainer));
		info.AddProperty(PropertyName._cellContainer, Variant.From<Control>(ref _cellContainer));
		info.AddProperty(PropertyName._bigDivinationButton, Variant.From<NDivinationButton>(ref _bigDivinationButton));
		info.AddProperty(PropertyName._smallDivinationButton, Variant.From<NDivinationButton>(ref _smallDivinationButton));
		info.AddProperty(PropertyName._divinationsLeftLabel, Variant.From<MegaRichTextLabel>(ref _divinationsLeftLabel));
		info.AddProperty(PropertyName._mask, Variant.From<NCrystalSphereMask>(ref _mask));
		info.AddProperty(PropertyName._proceedButton, Variant.From<NProceedButton>(ref _proceedButton));
		info.AddProperty(PropertyName._instructionsTitleLabel, Variant.From<MegaRichTextLabel>(ref _instructionsTitleLabel));
		info.AddProperty(PropertyName._instructionsDescriptionLabel, Variant.From<MegaRichTextLabel>(ref _instructionsDescriptionLabel));
		info.AddProperty(PropertyName._instructionsContainer, Variant.From<Control>(ref _instructionsContainer));
		info.AddProperty(PropertyName._dialogue, Variant.From<NCrystalSphereDialogue>(ref _dialogue));
		info.AddProperty(PropertyName._fadeTween, Variant.From<Tween>(ref _fadeTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._itemsContainer, ref val))
		{
			_itemsContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cellContainer, ref val2))
		{
			_cellContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._bigDivinationButton, ref val3))
		{
			_bigDivinationButton = ((Variant)(ref val3)).As<NDivinationButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._smallDivinationButton, ref val4))
		{
			_smallDivinationButton = ((Variant)(ref val4)).As<NDivinationButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._divinationsLeftLabel, ref val5))
		{
			_divinationsLeftLabel = ((Variant)(ref val5)).As<MegaRichTextLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._mask, ref val6))
		{
			_mask = ((Variant)(ref val6)).As<NCrystalSphereMask>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._proceedButton, ref val7))
		{
			_proceedButton = ((Variant)(ref val7)).As<NProceedButton>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._instructionsTitleLabel, ref val8))
		{
			_instructionsTitleLabel = ((Variant)(ref val8)).As<MegaRichTextLabel>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._instructionsDescriptionLabel, ref val9))
		{
			_instructionsDescriptionLabel = ((Variant)(ref val9)).As<MegaRichTextLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._instructionsContainer, ref val10))
		{
			_instructionsContainer = ((Variant)(ref val10)).As<Control>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._dialogue, ref val11))
		{
			_dialogue = ((Variant)(ref val11)).As<NCrystalSphereDialogue>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._fadeTween, ref val12))
		{
			_fadeTween = ((Variant)(ref val12)).As<Tween>();
		}
	}
}
