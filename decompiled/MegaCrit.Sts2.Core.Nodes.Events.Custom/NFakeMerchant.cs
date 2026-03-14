using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Events.Custom;

[ScriptPath("res://src/Core/Nodes/Events/Custom/NFakeMerchant.cs")]
public class NFakeMerchant : Control, ICustomEventNode, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName ToggleMerchantTrack = StringName.op_Implicit("ToggleMerchantTrack");

		public static readonly StringName AfterRoomIsLoaded = StringName.op_Implicit("AfterRoomIsLoaded");

		public static readonly StringName StartCharacterAnimation = StringName.op_Implicit("StartCharacterAnimation");

		public static readonly StringName HideScreen = StringName.op_Implicit("HideScreen");

		public static readonly StringName OnMerchantOpened = StringName.op_Implicit("OnMerchantOpened");

		public static readonly StringName OpenInventory = StringName.op_Implicit("OpenInventory");

		public static readonly StringName ShowProceedButton = StringName.op_Implicit("ShowProceedButton");

		public static readonly StringName OnActiveScreenUpdated = StringName.op_Implicit("OnActiveScreenUpdated");

		public static readonly StringName BlockInput = StringName.op_Implicit("BlockInput");

		public static readonly StringName UnblockInput = StringName.op_Implicit("UnblockInput");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName MerchantButton = StringName.op_Implicit("MerchantButton");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _proceedButton = StringName.op_Implicit("_proceedButton");

		public static readonly StringName _characterContainer = StringName.op_Implicit("_characterContainer");

		public static readonly StringName _inputBlocker = StringName.op_Implicit("_inputBlocker");

		public static readonly StringName _inventory = StringName.op_Implicit("_inventory");
	}

	public class SignalName : SignalName
	{
	}

	private const float _animVariance = 0.5f;

	private readonly List<Player> _players = new List<Player>();

	private FakeMerchant _event;

	private MerchantDialogueSet _dialogue;

	private NProceedButton _proceedButton;

	private Control _characterContainer;

	private Control _inputBlocker;

	private NMerchantInventory _inventory;

	public NMerchantButton MerchantButton { get; private set; }

	public IScreenContext CurrentScreenContext
	{
		get
		{
			if (!_inventory.IsOpen)
			{
				return this;
			}
			return _inventory;
		}
	}

	public Control? DefaultFocusedControl => null;

	public void Initialize(EventModel eventModel)
	{
		_event = (FakeMerchant)eventModel;
		_dialogue = FakeMerchant.Dialogue;
		_players.AddRange(_event.Owner.RunState.Players);
	}

	public override void _Ready()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		_proceedButton = ((Node)this).GetNode<NProceedButton>(NodePath.op_Implicit("%ProceedButton"));
		((GodotObject)_proceedButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)HideScreen), 0u);
		_proceedButton.UpdateText(NProceedButton.ProceedLoc);
		_proceedButton.SetPulseState(isPulsing: false);
		_proceedButton.Enable();
		MerchantButton = ((Node)this).GetNode<NMerchantButton>(NodePath.op_Implicit("%MerchantButton"));
		if (_event.StartedFight)
		{
			((CanvasItem)MerchantButton).Hide();
			Player me = LocalContext.GetMe(_players);
			if (me.GetRelic<FakeMerchantsRug>() != null)
			{
				MegaSprite megaSprite = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%FakeMerchantBackground"))));
				megaSprite.GetSkeleton().FindBone("rug").Hide();
			}
		}
		else
		{
			MerchantButton.IsLocalPlayerDead = LocalContext.GetMe(_players).Creature.IsDead;
			MerchantButton.PlayerDeadLines = _dialogue.PlayerDeadLines;
			((GodotObject)MerchantButton).Connect(NMerchantButton.SignalName.MerchantOpened, Callable.From<NMerchantButton>((Action<NMerchantButton>)OnMerchantOpened), 0u);
		}
		_inventory = ((Node)this).GetNode<NMerchantInventory>(NodePath.op_Implicit("%Inventory"));
		((Control)_inventory).MouseFilter = (MouseFilterEnum)2;
		_inventory.Initialize(_event.Inventory, _dialogue);
		_characterContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CharacterContainer"));
		_inputBlocker = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%InputBlocker"));
		NMapScreen.Instance.SetTravelEnabled(enabled: true);
		NGame.Instance.SetScreenShakeTarget((Control)(object)this);
		AfterRoomIsLoaded();
	}

	public override void _EnterTree()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		ActiveScreenContext.Instance.Updated += OnActiveScreenUpdated;
		((GodotObject)NMapScreen.Instance).Connect(NMapScreen.SignalName.Opened, Callable.From((Action)ToggleMerchantTrack), 0u);
		((GodotObject)NMapScreen.Instance).Connect(NMapScreen.SignalName.Closed, Callable.From((Action)ToggleMerchantTrack), 0u);
	}

	public override void _ExitTree()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		NGame.Instance.ClearScreenShakeTarget();
		ActiveScreenContext.Instance.Updated -= OnActiveScreenUpdated;
		((GodotObject)NMapScreen.Instance).Disconnect(NMapScreen.SignalName.Opened, Callable.From((Action)ToggleMerchantTrack));
		((GodotObject)NMapScreen.Instance).Disconnect(NMapScreen.SignalName.Closed, Callable.From((Action)ToggleMerchantTrack));
	}

	public async Task FoulPotionThrown(FoulPotion potion)
	{
		LocString locString = Rng.Chaotic.NextItem(_dialogue.FoulPotionLines);
		if (locString != null)
		{
			NSpeechBubbleVfx nSpeechBubbleVfx = MerchantButton.PlayDialogue(locString);
			if (nSpeechBubbleVfx != null)
			{
				await Cmd.Wait((float)nSpeechBubbleVfx.SecondsToDisplay - 1f);
			}
		}
	}

	private void ToggleMerchantTrack()
	{
	}

	private void AfterRoomIsLoaded()
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		Player me = LocalContext.GetMe(_players);
		_players.Remove(me);
		_players.Insert(0, me);
		int num = Mathf.CeilToInt(Mathf.Sqrt((float)_players.Count));
		for (int i = 0; i < num; i++)
		{
			float num2 = -75f * (float)i;
			for (int j = 0; j < num; j++)
			{
				int num3 = i * num + j;
				if (num3 >= _players.Count)
				{
					break;
				}
				NCreatureVisuals nCreatureVisuals = _players[num3].Character.CreateVisuals();
				((Node)(object)_characterContainer).AddChildSafely((Node?)(object)nCreatureVisuals);
				StartCharacterAnimation(nCreatureVisuals);
				((Node)_characterContainer).MoveChild((Node)(object)nCreatureVisuals, 0);
				((Node2D)nCreatureVisuals).Position = new Vector2(num2, -50f * (float)i);
				if (i > 0)
				{
					((CanvasItem)nCreatureVisuals).Modulate = new Color(0.5f, 0.5f, 0.5f, 1f);
				}
				num2 -= nCreatureVisuals.Bounds.Size.X * 0.5f + 25f;
			}
		}
		if (!_event.StartedFight)
		{
			TaskHelper.RunSafely(ShowWelcomeDialogue());
		}
	}

	private async Task ShowWelcomeDialogue()
	{
		LocString line = Rng.Chaotic.NextItem(_dialogue.WelcomeLines);
		if (line != null)
		{
			await Cmd.Wait(0.75f);
			SfxCmd.Play("event:/sfx/npcs/reverse_merchant/reverse_merchant_laugh");
			MerchantButton.PlayDialogue(line, 4.0);
		}
	}

	private void StartCharacterAnimation(NCreatureVisuals visuals)
	{
		MegaTrackEntry megaTrackEntry = visuals.SpineBody.GetAnimationState().SetAnimation("relaxed_loop");
		if (megaTrackEntry != null)
		{
			megaTrackEntry.SetLoop(loop: true);
			megaTrackEntry.SetTimeScale(Rng.Chaotic.NextFloat(0.9f, 1.1f));
			float animationEnd = megaTrackEntry.GetAnimationEnd();
			megaTrackEntry.SetTrackTime((animationEnd + Rng.Chaotic.NextFloat(-0.5f, 0.5f)) % animationEnd);
		}
	}

	private void HideScreen(NButton _)
	{
		NMapScreen.Instance.Open();
	}

	private void OnMerchantOpened(NMerchantButton _)
	{
		OpenInventory();
	}

	private void OpenInventory()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!_inventory.IsOpen)
		{
			_proceedButton.Disable();
			_inventory.Open();
			MerchantButton.Disable();
			((GodotObject)_inventory).Connect(NMerchantInventory.SignalName.InventoryClosed, Callable.From((Action)delegate
			{
				MerchantButton.Enable();
				ShowProceedButton();
			}), 4u);
		}
	}

	private void ShowProceedButton()
	{
		_proceedButton.Enable();
		_proceedButton.SetPulseState(isPulsing: true);
	}

	private void OnActiveScreenUpdated()
	{
		this.UpdateControllerNavEnabled();
		if (ActiveScreenContext.Instance.IsCurrent(this))
		{
			MerchantButton.Enable();
			if (!_proceedButton.IsEnabled)
			{
				_proceedButton.Enable();
			}
		}
		else
		{
			MerchantButton.Disable();
			_proceedButton.Disable();
		}
	}

	public void BlockInput()
	{
		_inputBlocker.MouseFilter = (MouseFilterEnum)0;
		NHotkeyManager.Instance.AddBlockingScreen((Node)(object)_inputBlocker);
	}

	public void UnblockInput()
	{
		_inputBlocker.MouseFilter = (MouseFilterEnum)2;
		NHotkeyManager.Instance.RemoveBlockingScreen((Node)(object)_inputBlocker);
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
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleMerchantTrack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterRoomIsLoaded, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartCharacterAnimation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("visuals"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMerchantOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenInventory, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowProceedButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnActiveScreenUpdated, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.BlockInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UnblockInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.ToggleMerchantTrack && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ToggleMerchantTrack();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterRoomIsLoaded && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterRoomIsLoaded();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartCharacterAnimation && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			StartCharacterAnimation(VariantUtils.ConvertTo<NCreatureVisuals>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			HideScreen(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMerchantOpened && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMerchantOpened(VariantUtils.ConvertTo<NMerchantButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenInventory && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenInventory();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowProceedButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowProceedButton();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenUpdated && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnActiveScreenUpdated();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.BlockInput && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			BlockInput();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UnblockInput && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UnblockInput();
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
		if ((ref method) == MethodName.ToggleMerchantTrack)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterRoomIsLoaded)
		{
			return true;
		}
		if ((ref method) == MethodName.StartCharacterAnimation)
		{
			return true;
		}
		if ((ref method) == MethodName.HideScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMerchantOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenInventory)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowProceedButton)
		{
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenUpdated)
		{
			return true;
		}
		if ((ref method) == MethodName.BlockInput)
		{
			return true;
		}
		if ((ref method) == MethodName.UnblockInput)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.MerchantButton)
		{
			MerchantButton = VariantUtils.ConvertTo<NMerchantButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			_proceedButton = VariantUtils.ConvertTo<NProceedButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterContainer)
		{
			_characterContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._inputBlocker)
		{
			_inputBlocker = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._inventory)
		{
			_inventory = VariantUtils.ConvertTo<NMerchantInventory>(ref value);
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
		if ((ref name) == PropertyName.MerchantButton)
		{
			NMerchantButton merchantButton = MerchantButton;
			value = VariantUtils.CreateFrom<NMerchantButton>(ref merchantButton);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			value = VariantUtils.CreateFrom<NProceedButton>(ref _proceedButton);
			return true;
		}
		if ((ref name) == PropertyName._characterContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _characterContainer);
			return true;
		}
		if ((ref name) == PropertyName._inputBlocker)
		{
			value = VariantUtils.CreateFrom<Control>(ref _inputBlocker);
			return true;
		}
		if ((ref name) == PropertyName._inventory)
		{
			value = VariantUtils.CreateFrom<NMerchantInventory>(ref _inventory);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._proceedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._inputBlocker, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._inventory, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.MerchantButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		StringName merchantButton = PropertyName.MerchantButton;
		NMerchantButton merchantButton2 = MerchantButton;
		info.AddProperty(merchantButton, Variant.From<NMerchantButton>(ref merchantButton2));
		info.AddProperty(PropertyName._proceedButton, Variant.From<NProceedButton>(ref _proceedButton));
		info.AddProperty(PropertyName._characterContainer, Variant.From<Control>(ref _characterContainer));
		info.AddProperty(PropertyName._inputBlocker, Variant.From<Control>(ref _inputBlocker));
		info.AddProperty(PropertyName._inventory, Variant.From<NMerchantInventory>(ref _inventory));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.MerchantButton, ref val))
		{
			MerchantButton = ((Variant)(ref val)).As<NMerchantButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._proceedButton, ref val2))
		{
			_proceedButton = ((Variant)(ref val2)).As<NProceedButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterContainer, ref val3))
		{
			_characterContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._inputBlocker, ref val4))
		{
			_inputBlocker = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._inventory, ref val5))
		{
			_inventory = ((Variant)(ref val5)).As<NMerchantInventory>();
		}
	}
}
