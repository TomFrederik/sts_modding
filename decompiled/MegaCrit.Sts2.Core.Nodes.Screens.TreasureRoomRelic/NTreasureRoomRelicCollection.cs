using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.TreasureRelicPicking;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.TreasureRoomRelic;

[ScriptPath("res://src/Core/Nodes/Screens/TreasureRoomRelic/NTreasureRoomRelicCollection.cs")]
public class NTreasureRoomRelicCollection : Control, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName InitializeRelics = StringName.op_Implicit("InitializeRelics");

		public static readonly StringName SetSelectionEnabled = StringName.op_Implicit("SetSelectionEnabled");

		public static readonly StringName AnimIn = StringName.op_Implicit("AnimIn");

		public static readonly StringName AnimOut = StringName.op_Implicit("AnimOut");

		public static readonly StringName PickRelic = StringName.op_Implicit("PickRelic");

		public static readonly StringName RefreshVotes = StringName.op_Implicit("RefreshVotes");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName SingleplayerRelicHolder = StringName.op_Implicit("SingleplayerRelicHolder");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _fightBackstop = StringName.op_Implicit("_fightBackstop");

		public static readonly StringName _hands = StringName.op_Implicit("_hands");

		public static readonly StringName _openedTicks = StringName.op_Implicit("_openedTicks");

		public static readonly StringName _emptyLabel = StringName.op_Implicit("_emptyLabel");

		public static readonly StringName _isEmptyChest = StringName.op_Implicit("_isEmptyChest");
	}

	public class SignalName : SignalName
	{
	}

	private const ulong _noSelectionTimeMsec = 200uL;

	private Control _fightBackstop;

	private NHandImageCollection _hands;

	private readonly List<NTreasureRoomRelicHolder> _multiplayerHolders = new List<NTreasureRoomRelicHolder>();

	private List<NTreasureRoomRelicHolder> _holdersInUse = new List<NTreasureRoomRelicHolder>();

	private TaskCompletionSource? _relicPickingTaskCompletionSource;

	private ulong _openedTicks;

	private IRunState _runState;

	private Label? _emptyLabel;

	private bool _isEmptyChest;

	private static string ScenePath => SceneHelper.GetScenePath("screens/shared_relic_picking_screen");

	public static IEnumerable<string> AssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.Add(ScenePath);
			list.AddRange(NCardRewardAlternativeButton.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	public NTreasureRoomRelicHolder SingleplayerRelicHolder { get; private set; }

	public Control? DefaultFocusedControl
	{
		get
		{
			if (_holdersInUse.Count <= 0)
			{
				return null;
			}
			return (Control?)(object)_holdersInUse[_runState.GetPlayerSlotIndex(LocalContext.GetMe(_runState.Players))];
		}
	}

	public override void _Ready()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		_fightBackstop = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%FightBackstop"));
		_hands = ((Node)this).GetNode<NHandImageCollection>(NodePath.op_Implicit("%HandsContainer"));
		Control node = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Container"));
		SingleplayerRelicHolder = ((Node)node).GetNode<NTreasureRoomRelicHolder>(NodePath.op_Implicit("%SingleplayerRelicHolder"));
		foreach (NTreasureRoomRelicHolder item in ((IEnumerable)((Node)node).GetChildren(false)).OfType<NTreasureRoomRelicHolder>())
		{
			if (item != SingleplayerRelicHolder)
			{
				_multiplayerHolders.Add(item);
			}
		}
		Control fightBackstop = _fightBackstop;
		Color modulate = ((CanvasItem)_fightBackstop).Modulate;
		modulate.A = 0f;
		((CanvasItem)fightBackstop).Modulate = modulate;
		((CanvasItem)_fightBackstop).Visible = false;
		RunManager.Instance.TreasureRoomRelicSynchronizer.VotesChanged += RefreshVotes;
		RunManager.Instance.TreasureRoomRelicSynchronizer.RelicsAwarded += OnRelicsAwarded;
	}

	public override void _ExitTree()
	{
		RunManager.Instance.TreasureRoomRelicSynchronizer.VotesChanged -= RefreshVotes;
		RunManager.Instance.TreasureRoomRelicSynchronizer.RelicsAwarded -= OnRelicsAwarded;
	}

	public void Initialize(IRunState runState)
	{
		_runState = runState;
		_hands.Initialize(runState);
	}

	public void InitializeRelics()
	{
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<RelicModel> currentRelics = RunManager.Instance.TreasureRoomRelicSynchronizer.CurrentRelics;
		if (currentRelics == null || currentRelics.Count == 0)
		{
			_isEmptyChest = true;
			((CanvasItem)SingleplayerRelicHolder).Visible = false;
			foreach (NTreasureRoomRelicHolder multiplayerHolder in _multiplayerHolders)
			{
				((CanvasItem)multiplayerHolder).Visible = false;
			}
			MegaLabel megaLabel = new MegaLabel();
			((Label)megaLabel).Text = new LocString("gameplay_ui", "TREASURE_EMPTY").GetFormattedText();
			((Label)megaLabel).HorizontalAlignment = (HorizontalAlignment)1;
			((Label)megaLabel).VerticalAlignment = (VerticalAlignment)1;
			((Control)megaLabel).CustomMinimumSize = new Vector2(400f, 100f);
			((Control)megaLabel).LayoutMode = 1;
			((Control)megaLabel).AnchorsPreset = 8;
			_emptyLabel = (Label?)(object)megaLabel;
			((Control)_emptyLabel).AddThemeFontSizeOverride(ThemeConstants.Label.fontSize, 48);
			((Node)(object)this).AddChildSafely((Node?)(object)_emptyLabel);
			return;
		}
		if (currentRelics.Count == 1)
		{
			SingleplayerRelicHolder.Initialize(currentRelics[0], _runState);
			((CanvasItem)SingleplayerRelicHolder).Visible = true;
			SingleplayerRelicHolder.Index = 0;
			((GodotObject)SingleplayerRelicHolder).Connect(NClickableControl.SignalName.Released, Callable.From<NTreasureRoomRelicHolder>((Action<NTreasureRoomRelicHolder>)delegate
			{
				PickRelic(SingleplayerRelicHolder);
			}), 0u);
			int num = 1;
			List<NTreasureRoomRelicHolder> list = new List<NTreasureRoomRelicHolder>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<NTreasureRoomRelicHolder> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = SingleplayerRelicHolder;
			_holdersInUse = list;
			{
				foreach (NTreasureRoomRelicHolder multiplayerHolder2 in _multiplayerHolders)
				{
					((CanvasItem)multiplayerHolder2).Visible = false;
				}
				return;
			}
		}
		((CanvasItem)SingleplayerRelicHolder).Visible = false;
		for (int num2 = 0; num2 < _multiplayerHolders.Count; num2++)
		{
			NTreasureRoomRelicHolder holder = _multiplayerHolders[num2];
			if (num2 < currentRelics.Count)
			{
				((CanvasItem)holder).Visible = true;
				holder.Relic.Model = currentRelics[num2];
				holder.Initialize(currentRelics[num2], _runState);
			}
			else
			{
				((CanvasItem)holder).Visible = false;
			}
			holder.Index = num2;
			((GodotObject)holder).Connect(NClickableControl.SignalName.Released, Callable.From<NTreasureRoomRelicHolder>((Action<NTreasureRoomRelicHolder>)delegate
			{
				PickRelic(holder);
			}), 0u);
			_holdersInUse.Add(holder);
			holder.VoteContainer.RefreshPlayerVotes();
		}
		for (int num3 = 0; num3 < _holdersInUse.Count; num3++)
		{
			((Control)_holdersInUse[num3]).SetFocusMode((FocusModeEnum)2);
			((Control)_holdersInUse[num3]).FocusNeighborTop = ((Node)_holdersInUse[num3]).GetPath();
			((Control)_holdersInUse[num3]).FocusNeighborBottom = ((Node)_holdersInUse[num3]).GetPath();
			NTreasureRoomRelicHolder nTreasureRoomRelicHolder = _holdersInUse[num3];
			NodePath path;
			if (num3 <= 0)
			{
				List<NTreasureRoomRelicHolder> holdersInUse = _holdersInUse;
				path = ((Node)holdersInUse[holdersInUse.Count - 1]).GetPath();
			}
			else
			{
				path = ((Node)_holdersInUse[num3 - 1]).GetPath();
			}
			((Control)nTreasureRoomRelicHolder).FocusNeighborLeft = path;
			((Control)_holdersInUse[num3]).FocusNeighborRight = ((num3 < _holdersInUse.Count - 1) ? ((Node)_holdersInUse[num3 + 1]).GetPath() : ((Node)_holdersInUse[0]).GetPath());
		}
		if (currentRelics.Count == 2)
		{
			((Control)_multiplayerHolders[1]).Position = ((Control)_multiplayerHolders[3]).Position;
		}
	}

	public void SetSelectionEnabled(bool isEnabled)
	{
		if (isEnabled)
		{
			SingleplayerRelicHolder.Enable();
			{
				foreach (NTreasureRoomRelicHolder multiplayerHolder in _multiplayerHolders)
				{
					multiplayerHolder.Enable();
				}
				return;
			}
		}
		SingleplayerRelicHolder.Disable();
		foreach (NTreasureRoomRelicHolder multiplayerHolder2 in _multiplayerHolders)
		{
			multiplayerHolder2.Disable();
		}
	}

	public Task RelicPickingFinished()
	{
		_relicPickingTaskCompletionSource = new TaskCompletionSource();
		return _relicPickingTaskCompletionSource.Task;
	}

	public void AnimIn(Node chestVisual)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Visible = true;
		((CanvasItem)this).Modulate = Colors.Transparent;
		Tween val = ((Node)this).CreateTween().SetParallel(true);
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.4);
		val.TweenProperty((GodotObject)(object)chestVisual, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.halfTransparentWhite), 0.4);
		if (_isEmptyChest)
		{
			LocalContext.GetMe(_runState)?.Relics.OfType<SilverCrucible>().FirstOrDefault()?.Flash();
			val.TweenCallback(Callable.From((Action)delegate
			{
				RunManager.Instance.TreasureRoomRelicSynchronizer.CompleteWithNoRelics();
			})).SetDelay(1.0);
			return;
		}
		foreach (NTreasureRoomRelicHolder holder in _holdersInUse)
		{
			((Control)holder).MouseFilter = (MouseFilterEnum)2;
			float num = ((_holdersInUse.Count == 1) ? 150f : 50f);
			float num2 = 0.2f + 0.2f * Rng.Chaotic.NextFloat();
			((CanvasItem)holder).Modulate = Colors.Black;
			NTreasureRoomRelicHolder nTreasureRoomRelicHolder = holder;
			Vector2 position = ((Control)holder).Position;
			position.Y = ((Control)holder).Position.Y + num;
			((Control)nTreasureRoomRelicHolder).Position = position;
			Tween val2 = ((Node)this).CreateTween().SetParallel(true);
			val2.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.2).SetDelay((double)num2);
			val2.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)holder).Position.Y - num), 0.6).SetDelay((double)num2).SetEase((EaseType)1)
				.SetTrans((TransitionType)10);
			val2.TweenCallback(Callable.From<MouseFilterEnum>((Func<MouseFilterEnum>)delegate
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				NTreasureRoomRelicHolder nTreasureRoomRelicHolder2 = holder;
				long num3 = 0L;
				MouseFilterEnum result = (MouseFilterEnum)num3;
				((Control)nTreasureRoomRelicHolder2).MouseFilter = (MouseFilterEnum)num3;
				return result;
			})).SetDelay((double)(num2 + 0.6f));
		}
		NRun.Instance.ScreenStateTracker.SetIsInSharedRelicPickingScreen(isInSharedRelicPicking: true);
		_hands.AnimateHandsIn();
	}

	public void AnimOut(Node chestVisual)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Modulate = Colors.White;
		Tween val = ((Node)this).CreateTween().Parallel();
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentWhite), 0.3);
		val.TweenProperty((GodotObject)(object)chestVisual, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.3);
		val.TweenCallback(Callable.From<bool>((Func<bool>)(() => ((CanvasItem)this).Visible = false)));
		NRun.Instance.ScreenStateTracker.SetIsInSharedRelicPickingScreen(isInSharedRelicPicking: false);
	}

	private void PickRelic(NTreasureRoomRelicHolder holder)
	{
		if (Time.GetTicksMsec() - _openedTicks > 200)
		{
			RunManager.Instance.TreasureRoomRelicSynchronizer.PickRelicLocally(holder.Index);
		}
	}

	private void OnRelicsAwarded(List<RelicPickingResult> results)
	{
		TaskHelper.RunSafely(AnimateRelicAwards(results));
	}

	private async Task AnimateRelicAwards(List<RelicPickingResult> results)
	{
		for (int i = 0; i < _holdersInUse.Count; i++)
		{
			((Control)_holdersInUse[i]).SetFocusMode((FocusModeEnum)0);
		}
		_hands.BeforeRelicsAwarded();
		List<Task> tasksToWait = new List<Task>();
		RelicPickingResultType? relicPickingResultType = null;
		results.Sort((RelicPickingResult r1, RelicPickingResult r2) => r1.type.CompareTo(r2.type));
		foreach (RelicPickingResult result in results)
		{
			NTreasureRoomRelicHolder holder = _holdersInUse.First((NTreasureRoomRelicHolder h) => h.Relic.Model == result.relic);
			holder.AnimateAwayVotes();
			if (relicPickingResultType.HasValue && result.type != relicPickingResultType)
			{
				await Cmd.Wait(0.5f);
			}
			if (result.type == RelicPickingResultType.FoughtOver)
			{
				((CanvasItem)holder).ZIndex = 1;
				((CanvasItem)_fightBackstop).Visible = true;
				Tween val = ((Node)this).CreateTween();
				val.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("global_position"), Variant.op_Implicit((_fightBackstop.Size - ((Control)holder).Size) * 0.5f), 0.25).SetTrans((TransitionType)10).SetEase((EaseType)0);
				val.TweenProperty((GodotObject)(object)_fightBackstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25);
				_hands.BeforeFightStarted(result.fight.playersInvolved);
				await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
				await Cmd.Wait(1f);
				await _hands.DoFight(result, holder);
				val = ((Node)this).CreateTween();
				val.TweenProperty((GodotObject)(object)_fightBackstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
				await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
				((CanvasItem)_fightBackstop).Visible = false;
				((CanvasItem)holder).ZIndex = 0;
			}
			else
			{
				NHandImage hand = _hands.GetHand(result.player.NetId);
				if (hand != null)
				{
					tasksToWait.Add(TaskHelper.RunSafely(hand.GrabRelic(holder)));
					await Cmd.Wait(0.25f);
				}
			}
			relicPickingResultType = result.type;
		}
		await Task.WhenAll(tasksToWait);
		_hands.AnimateHandsAway();
		foreach (RelicPickingResult result2 in results)
		{
			NTreasureRoomRelicHolder nTreasureRoomRelicHolder = _holdersInUse.First((NTreasureRoomRelicHolder h) => h.Relic.Model == result2.relic);
			RelicModel relic = result2.relic.ToMutable();
			TaskHelper.RunSafely(RelicCmd.Obtain(relic, result2.player));
			if (LocalContext.IsMe(result2.player))
			{
				NRun.Instance.GlobalUi.RelicInventory.AnimateRelic(relic, ((Control)nTreasureRoomRelicHolder).GlobalPosition, ((Control)nTreasureRoomRelicHolder).Scale);
			}
			if (_runState.Players.Count == 1)
			{
				((CanvasItem)nTreasureRoomRelicHolder).Visible = false;
			}
			foreach (Player player in result2.player.RunState.Players)
			{
				if (player != result2.player)
				{
					player.RelicGrabBag.MoveToFallback(result2.relic);
				}
			}
		}
		_relicPickingTaskCompletionSource.SetResult();
	}

	private void RefreshVotes()
	{
		foreach (NTreasureRoomRelicHolder item in _holdersInUse)
		{
			item.VoteContainer.RefreshPlayerVotes();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected O, but got Unknown
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeRelics, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetSelectionEnabled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isEnabled"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("chestVisual"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimOut, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("chestVisual"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PickRelic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshVotes, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitializeRelics && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitializeRelics();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetSelectionEnabled && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetSelectionEnabled(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimIn && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AnimIn(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimOut && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AnimOut(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PickRelic && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PickRelic(VariantUtils.ConvertTo<NTreasureRoomRelicHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshVotes && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshVotes();
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.InitializeRelics)
		{
			return true;
		}
		if ((ref method) == MethodName.SetSelectionEnabled)
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
		if ((ref method) == MethodName.PickRelic)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshVotes)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.SingleplayerRelicHolder)
		{
			SingleplayerRelicHolder = VariantUtils.ConvertTo<NTreasureRoomRelicHolder>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fightBackstop)
		{
			_fightBackstop = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hands)
		{
			_hands = VariantUtils.ConvertTo<NHandImageCollection>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._openedTicks)
		{
			_openedTicks = VariantUtils.ConvertTo<ulong>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._emptyLabel)
		{
			_emptyLabel = VariantUtils.ConvertTo<Label>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isEmptyChest)
		{
			_isEmptyChest = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName.SingleplayerRelicHolder)
		{
			NTreasureRoomRelicHolder singleplayerRelicHolder = SingleplayerRelicHolder;
			value = VariantUtils.CreateFrom<NTreasureRoomRelicHolder>(ref singleplayerRelicHolder);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._fightBackstop)
		{
			value = VariantUtils.CreateFrom<Control>(ref _fightBackstop);
			return true;
		}
		if ((ref name) == PropertyName._hands)
		{
			value = VariantUtils.CreateFrom<NHandImageCollection>(ref _hands);
			return true;
		}
		if ((ref name) == PropertyName._openedTicks)
		{
			value = VariantUtils.CreateFrom<ulong>(ref _openedTicks);
			return true;
		}
		if ((ref name) == PropertyName._emptyLabel)
		{
			value = VariantUtils.CreateFrom<Label>(ref _emptyLabel);
			return true;
		}
		if ((ref name) == PropertyName._isEmptyChest)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isEmptyChest);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._fightBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hands, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._openedTicks, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._emptyLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isEmptyChest, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.SingleplayerRelicHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		StringName singleplayerRelicHolder = PropertyName.SingleplayerRelicHolder;
		NTreasureRoomRelicHolder singleplayerRelicHolder2 = SingleplayerRelicHolder;
		info.AddProperty(singleplayerRelicHolder, Variant.From<NTreasureRoomRelicHolder>(ref singleplayerRelicHolder2));
		info.AddProperty(PropertyName._fightBackstop, Variant.From<Control>(ref _fightBackstop));
		info.AddProperty(PropertyName._hands, Variant.From<NHandImageCollection>(ref _hands));
		info.AddProperty(PropertyName._openedTicks, Variant.From<ulong>(ref _openedTicks));
		info.AddProperty(PropertyName._emptyLabel, Variant.From<Label>(ref _emptyLabel));
		info.AddProperty(PropertyName._isEmptyChest, Variant.From<bool>(ref _isEmptyChest));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.SingleplayerRelicHolder, ref val))
		{
			SingleplayerRelicHolder = ((Variant)(ref val)).As<NTreasureRoomRelicHolder>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._fightBackstop, ref val2))
		{
			_fightBackstop = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._hands, ref val3))
		{
			_hands = ((Variant)(ref val3)).As<NHandImageCollection>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._openedTicks, ref val4))
		{
			_openedTicks = ((Variant)(ref val4)).As<ulong>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._emptyLabel, ref val5))
		{
			_emptyLabel = ((Variant)(ref val5)).As<Label>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._isEmptyChest, ref val6))
		{
			_isEmptyChest = ((Variant)(ref val6)).As<bool>();
		}
	}
}
