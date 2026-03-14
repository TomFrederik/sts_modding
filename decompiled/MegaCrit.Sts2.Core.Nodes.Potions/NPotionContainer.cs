using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Nodes.Potions;

[ScriptPath("res://src/Core/Nodes/Potions/NPotionContainer.cs")]
public class NPotionContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName ConnectPlayerEvents = StringName.op_Implicit("ConnectPlayerEvents");

		public static readonly StringName DisconnectPlayerEvents = StringName.op_Implicit("DisconnectPlayerEvents");

		public static readonly StringName GrowPotionHolders = StringName.op_Implicit("GrowPotionHolders");

		public static readonly StringName UpdateNavigation = StringName.op_Implicit("UpdateNavigation");

		public static readonly StringName PotionFtueCheck = StringName.op_Implicit("PotionFtueCheck");

		public static readonly StringName PlayAddFailedAnim = StringName.op_Implicit("PlayAddFailedAnim");

		public static readonly StringName OnPotionHolderFocused = StringName.op_Implicit("OnPotionHolderFocused");

		public static readonly StringName OnPotionHolderUnfocused = StringName.op_Implicit("OnPotionHolderUnfocused");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName FirstPotionControl = StringName.op_Implicit("FirstPotionControl");

		public static readonly StringName LastPotionControl = StringName.op_Implicit("LastPotionControl");

		public static readonly StringName _potionHolders = StringName.op_Implicit("_potionHolders");

		public static readonly StringName _potionErrorBg = StringName.op_Implicit("_potionErrorBg");

		public static readonly StringName _potionShortcutButton = StringName.op_Implicit("_potionShortcutButton");

		public static readonly StringName _potionsFullTween = StringName.op_Implicit("_potionsFullTween");

		public static readonly StringName _potionHolderInitPos = StringName.op_Implicit("_potionHolderInitPos");

		public static readonly StringName _focusedHolder = StringName.op_Implicit("_focusedHolder");
	}

	public class SignalName : SignalName
	{
	}

	private Player? _player;

	private readonly List<NPotionHolder> _holders = new List<NPotionHolder>();

	private Control _potionHolders;

	private Control _potionErrorBg;

	private NButton _potionShortcutButton;

	private Tween? _potionsFullTween;

	private Vector2 _potionHolderInitPos;

	private NPotionHolder? _focusedHolder;

	public Control? FirstPotionControl => (Control?)(object)_holders.FirstOrDefault();

	public Control? LastPotionControl => (Control?)(object)_holders.LastOrDefault();

	public override void _Ready()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Callable val = Callable.From((Action)UpdateNavigation);
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
	}

	public override void _EnterTree()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		_potionHolders = ((Node)this).GetNode<Control>(NodePath.op_Implicit("MarginContainer/PotionHolders"));
		_potionErrorBg = ((Node)this).GetNode<Control>(NodePath.op_Implicit("PotionErrorBg"));
		_potionShortcutButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("PotionShortcutButton"));
		((CanvasItem)_potionErrorBg).Modulate = Colors.Transparent;
		((GodotObject)_potionShortcutButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			((Node)_potionHolders).GetChild<Control>(0, false).TryGrabFocus();
		}), 0u);
		CombatManager.Instance.CombatSetUp += OnCombatSetUp;
		ConnectPlayerEvents();
	}

	public override void _ExitTree()
	{
		DisconnectPlayerEvents();
		_player = null;
		CombatManager.Instance.CombatSetUp -= OnCombatSetUp;
	}

	public void Initialize(IRunState runState)
	{
		DisconnectPlayerEvents();
		_player = LocalContext.GetMe(runState);
		ConnectPlayerEvents();
		GrowPotionHolders(_player.MaxPotionCount);
		foreach (PotionModel potion in _player.Potions)
		{
			Add(potion, isInitialization: true);
		}
	}

	private void ConnectPlayerEvents()
	{
		if (_player != null)
		{
			_player.AddPotionFailed += PlayAddFailedAnim;
			_player.PotionProcured += OnPotionProcured;
			_player.UsedPotionRemoved += OnUsedPotionRemoved;
			_player.PotionDiscarded += Discard;
			_player.MaxPotionCountChanged += GrowPotionHolders;
			_player.RelicObtained += OnRelicsUpdated;
			_player.RelicRemoved += OnRelicsUpdated;
		}
	}

	private void DisconnectPlayerEvents()
	{
		if (_player != null)
		{
			_player.AddPotionFailed -= PlayAddFailedAnim;
			_player.PotionProcured -= OnPotionProcured;
			_player.UsedPotionRemoved -= OnUsedPotionRemoved;
			_player.PotionDiscarded -= Discard;
			_player.MaxPotionCountChanged -= GrowPotionHolders;
			_player.RelicObtained -= OnRelicsUpdated;
			_player.RelicRemoved -= OnRelicsUpdated;
		}
	}

	private void GrowPotionHolders(int newMaxPotionSlots)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		for (int i = _holders.Count; i < newMaxPotionSlots; i++)
		{
			NPotionHolder node = NPotionHolder.Create(isUsable: true);
			_holders.Add(node);
			((Node)(object)_potionHolders).AddChildSafely((Node?)(object)node);
			((GodotObject)node).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
			{
				OnPotionHolderFocused(node);
			}), 0u);
			((GodotObject)node).Connect(SignalName.FocusExited, Callable.From((Action)delegate
			{
				OnPotionHolderUnfocused(node);
			}), 0u);
			((GodotObject)node).Connect(SignalName.MouseEntered, Callable.From((Action)delegate
			{
				OnPotionHolderFocused(node);
			}), 0u);
			((GodotObject)node).Connect(SignalName.MouseExited, Callable.From((Action)delegate
			{
				OnPotionHolderUnfocused(node);
			}), 0u);
		}
		UpdateNavigation();
	}

	private void OnRelicsUpdated(RelicModel _)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Callable val = Callable.From((Action)UpdateNavigation);
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
	}

	private void UpdateNavigation()
	{
		Control val = (Control)(object)NRun.Instance.GlobalUi.RelicInventory.RelicNodes.FirstOrDefault();
		if (val != null)
		{
			for (int i = 0; i < _holders.Count; i++)
			{
				((Control)_holders[i]).FocusNeighborLeft = ((i > 0) ? ((Node)_holders[i - 1]).GetPath() : ((Node)NRun.Instance.GlobalUi.TopBar.Gold).GetPath());
				((Control)_holders[i]).FocusNeighborRight = ((i < _holders.Count - 1) ? ((Node)_holders[i + 1]).GetPath() : ((Node)NRun.Instance.GlobalUi.TopBar.RoomIcon).GetPath());
				((Control)_holders[i]).FocusNeighborBottom = ((Node)val).GetPath();
				((Control)_holders[i]).FocusNeighborTop = ((Node)_holders[i]).GetPath();
			}
		}
	}

	private void Add(PotionModel potion, bool isInitialization)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!_holders.All((NPotionHolder h) => h.HasPotion))
		{
			if (!isInitialization)
			{
				PotionFtueCheck();
			}
			NPotion nPotion = NPotion.Create(potion);
			((Control)nPotion).Position = new Vector2(-30f, -30f);
			NPotionHolder nPotionHolder = _holders[potion.Owner.PotionSlots.IndexOf<PotionModel>(potion)];
			nPotionHolder.AddPotion(nPotion);
		}
	}

	public void AnimatePotion(PotionModel potion, Vector2? startPosition = null)
	{
		if (LocalContext.IsMine(potion))
		{
			NPotionHolder nPotionHolder = _holders.First((NPotionHolder n) => n.Potion != null && n.Potion.Model == potion);
			TaskHelper.RunSafely(nPotionHolder.Potion.PlayNewlyAcquiredAnimation(startPosition));
		}
	}

	public void OnPotionUseCanceled(PotionModel potion)
	{
		NPotionHolder nPotionHolder = _holders.FirstOrDefault((NPotionHolder n) => n.Potion.Model == potion);
		if (nPotionHolder != null)
		{
			nPotionHolder.CancelPotionUse();
			return;
		}
		Log.Error($"Tried to cancel potion use for potion {potion} but a holder for it does not exist in the player's belt!");
	}

	private void PotionFtueCheck()
	{
		if (!SaveManager.Instance.SeenFtue("obtain_potion_ftue"))
		{
			NModalContainer.Instance.Add((Node)(object)NObtainPotionFtue.Create());
			SaveManager.Instance.MarkFtueAsComplete("obtain_potion_ftue");
		}
	}

	private void PlayAddFailedAnim()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (_potionsFullTween != null && _potionsFullTween.IsRunning())
		{
			Tween? potionsFullTween = _potionsFullTween;
			if (potionsFullTween != null)
			{
				potionsFullTween.Kill();
			}
			_potionHolders.Position = _potionHolderInitPos;
		}
		_potionsFullTween = ((Node)this).CreateTween().SetParallel(true);
		_potionHolderInitPos = _potionHolders.Position;
		_potionsFullTween.TweenMethod(Callable.From<float>((Action<float>)delegate(float t)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			_potionHolders.Position = _potionHolderInitPos + Vector2.Right * 3f * Mathf.Sin(t * 5f) * Mathf.Sin(t * 0.5f);
		}), Variant.op_Implicit(0f), Variant.op_Implicit((float)Math.PI * 2f), 0.5);
		_potionsFullTween.TweenProperty((GodotObject)(object)_potionErrorBg, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.15);
		_potionsFullTween.TweenProperty((GodotObject)(object)_potionErrorBg, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.Transparent), 0.5).SetDelay(0.35);
	}

	private void Discard(PotionModel potion)
	{
		NPotionHolder nPotionHolder = _holders.First((NPotionHolder n) => n.Potion != null && n.Potion.Model == potion);
		OnPotionHolderUnfocused(nPotionHolder);
		nPotionHolder.DiscardPotion();
	}

	private void RemoveUsed(PotionModel potion)
	{
		NPotionHolder nPotionHolder = _holders.First((NPotionHolder n) => n.Potion != null && n.Potion.Model == potion);
		OnPotionHolderUnfocused(nPotionHolder);
		nPotionHolder.RemoveUsedPotion();
	}

	private void OnPotionProcured(PotionModel potion)
	{
		Add(potion, isInitialization: false);
	}

	private void OnUsedPotionRemoved(PotionModel potion)
	{
		RemoveUsed(potion);
	}

	private void OnPotionHolderFocused(NPotionHolder holder)
	{
		if (_focusedHolder != holder && holder.Potion != null)
		{
			RunManager.Instance.HoveredModelTracker.OnLocalPotionHovered(holder.Potion.Model);
			_focusedHolder = holder;
		}
	}

	private void OnPotionHolderUnfocused(NPotionHolder holder)
	{
		if (_focusedHolder == holder)
		{
			RunManager.Instance.HoveredModelTracker.OnLocalPotionUnhovered();
			_focusedHolder = null;
		}
	}

	private void OnCombatSetUp(CombatState _)
	{
		TaskHelper.RunSafely(ShinePotions());
	}

	private async Task ShinePotions()
	{
		await Cmd.Wait(1f);
		foreach (NPotionHolder holder in _holders)
		{
			await TaskHelper.RunSafely(holder.ShineOnStartOfCombat());
		}
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
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Expected O, but got Unknown
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Expected O, but got Unknown
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectPlayerEvents, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisconnectPlayerEvents, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GrowPotionHolders, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("newMaxPotionSlots"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PotionFtueCheck, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayAddFailedAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPotionHolderFocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPotionHolderUnfocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
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
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.ConnectPlayerEvents && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectPlayerEvents();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisconnectPlayerEvents && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisconnectPlayerEvents();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GrowPotionHolders && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			GrowPotionHolders(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PotionFtueCheck && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PotionFtueCheck();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayAddFailedAnim && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayAddFailedAnim();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPotionHolderFocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPotionHolderFocused(VariantUtils.ConvertTo<NPotionHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPotionHolderUnfocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPotionHolderUnfocused(VariantUtils.ConvertTo<NPotionHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.ConnectPlayerEvents)
		{
			return true;
		}
		if ((ref method) == MethodName.DisconnectPlayerEvents)
		{
			return true;
		}
		if ((ref method) == MethodName.GrowPotionHolders)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.PotionFtueCheck)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayAddFailedAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPotionHolderFocused)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPotionHolderUnfocused)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._potionHolders)
		{
			_potionHolders = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionErrorBg)
		{
			_potionErrorBg = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionShortcutButton)
		{
			_potionShortcutButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionsFullTween)
		{
			_potionsFullTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionHolderInitPos)
		{
			_potionHolderInitPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._focusedHolder)
		{
			_focusedHolder = VariantUtils.ConvertTo<NPotionHolder>(ref value);
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
		if ((ref name) == PropertyName.FirstPotionControl)
		{
			Control firstPotionControl = FirstPotionControl;
			value = VariantUtils.CreateFrom<Control>(ref firstPotionControl);
			return true;
		}
		if ((ref name) == PropertyName.LastPotionControl)
		{
			Control firstPotionControl = LastPotionControl;
			value = VariantUtils.CreateFrom<Control>(ref firstPotionControl);
			return true;
		}
		if ((ref name) == PropertyName._potionHolders)
		{
			value = VariantUtils.CreateFrom<Control>(ref _potionHolders);
			return true;
		}
		if ((ref name) == PropertyName._potionErrorBg)
		{
			value = VariantUtils.CreateFrom<Control>(ref _potionErrorBg);
			return true;
		}
		if ((ref name) == PropertyName._potionShortcutButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _potionShortcutButton);
			return true;
		}
		if ((ref name) == PropertyName._potionsFullTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _potionsFullTween);
			return true;
		}
		if ((ref name) == PropertyName._potionHolderInitPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _potionHolderInitPos);
			return true;
		}
		if ((ref name) == PropertyName._focusedHolder)
		{
			value = VariantUtils.CreateFrom<NPotionHolder>(ref _focusedHolder);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._potionHolders, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionErrorBg, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionShortcutButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionsFullTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._potionHolderInitPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._focusedHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.FirstPotionControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.LastPotionControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._potionHolders, Variant.From<Control>(ref _potionHolders));
		info.AddProperty(PropertyName._potionErrorBg, Variant.From<Control>(ref _potionErrorBg));
		info.AddProperty(PropertyName._potionShortcutButton, Variant.From<NButton>(ref _potionShortcutButton));
		info.AddProperty(PropertyName._potionsFullTween, Variant.From<Tween>(ref _potionsFullTween));
		info.AddProperty(PropertyName._potionHolderInitPos, Variant.From<Vector2>(ref _potionHolderInitPos));
		info.AddProperty(PropertyName._focusedHolder, Variant.From<NPotionHolder>(ref _focusedHolder));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._potionHolders, ref val))
		{
			_potionHolders = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionErrorBg, ref val2))
		{
			_potionErrorBg = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionShortcutButton, ref val3))
		{
			_potionShortcutButton = ((Variant)(ref val3)).As<NButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionsFullTween, ref val4))
		{
			_potionsFullTween = ((Variant)(ref val4)).As<Tween>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionHolderInitPos, ref val5))
		{
			_potionHolderInitPos = ((Variant)(ref val5)).As<Vector2>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._focusedHolder, ref val6))
		{
			_focusedHolder = ((Variant)(ref val6)).As<NPotionHolder>();
		}
	}
}
