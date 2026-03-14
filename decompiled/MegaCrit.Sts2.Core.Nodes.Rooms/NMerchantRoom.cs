using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Rooms;

[ScriptPath("res://src/Core/Nodes/Rooms/NMerchantRoom.cs")]
public class NMerchantRoom : Control, IScreenContext, IRoomWithProceedButton
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName ToggleMerchantTrack = StringName.op_Implicit("ToggleMerchantTrack");

		public static readonly StringName AfterRoomIsLoaded = StringName.op_Implicit("AfterRoomIsLoaded");

		public static readonly StringName HideScreen = StringName.op_Implicit("HideScreen");

		public static readonly StringName MerchantFtueCheck = StringName.op_Implicit("MerchantFtueCheck");

		public static readonly StringName OnMerchantOpened = StringName.op_Implicit("OnMerchantOpened");

		public static readonly StringName OpenInventory = StringName.op_Implicit("OpenInventory");

		public static readonly StringName OnActiveScreenUpdated = StringName.op_Implicit("OnActiveScreenUpdated");

		public static readonly StringName BlockInput = StringName.op_Implicit("BlockInput");

		public static readonly StringName UnblockInput = StringName.op_Implicit("UnblockInput");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ProceedButton = StringName.op_Implicit("ProceedButton");

		public static readonly StringName Inventory = StringName.op_Implicit("Inventory");

		public static readonly StringName MerchantButton = StringName.op_Implicit("MerchantButton");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _proceedButton = StringName.op_Implicit("_proceedButton");

		public static readonly StringName _characterContainer = StringName.op_Implicit("_characterContainer");

		public static readonly StringName _inputBlocker = StringName.op_Implicit("_inputBlocker");
	}

	public class SignalName : SignalName
	{
	}

	private const float _animVariance = 0.5f;

	private static readonly string _scenePath = SceneHelper.GetScenePath("rooms/merchant_room");

	private readonly List<Player> _players = new List<Player>();

	private MerchantDialogueSet _dialogue;

	private NProceedButton _proceedButton;

	private Control _characterContainer;

	private Control _inputBlocker;

	private readonly List<NMerchantCharacter> _playerVisuals = new List<NMerchantCharacter>();

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NMerchantRoom? Instance => NRun.Instance?.MerchantRoom;

	public NProceedButton ProceedButton => _proceedButton;

	public MerchantRoom Room { get; private set; }

	public NMerchantInventory Inventory { get; private set; }

	public NMerchantButton MerchantButton { get; private set; }

	public IReadOnlyList<NMerchantCharacter> PlayerVisuals => _playerVisuals;

	public Control? DefaultFocusedControl => null;

	public static NMerchantRoom? Create(MerchantRoom room, IReadOnlyList<Player> players)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NMerchantRoom nMerchantRoom = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NMerchantRoom>((GenEditState)0);
		nMerchantRoom.Room = room;
		nMerchantRoom._players.AddRange(players);
		nMerchantRoom._dialogue = MerchantRoom.Dialogue;
		return nMerchantRoom;
	}

	public override void _Ready()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		_proceedButton = ((Node)this).GetNode<NProceedButton>(NodePath.op_Implicit("%ProceedButton"));
		((GodotObject)_proceedButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)HideScreen), 0u);
		_proceedButton.UpdateText(NProceedButton.ProceedLoc);
		_proceedButton.SetPulseState(isPulsing: false);
		_proceedButton.Enable();
		MerchantButton = ((Node)this).GetNode<NMerchantButton>(NodePath.op_Implicit("%MerchantButton"));
		MerchantButton.IsLocalPlayerDead = LocalContext.GetMe(_players).Creature.IsDead;
		MerchantButton.PlayerDeadLines = _dialogue.PlayerDeadLines;
		((GodotObject)MerchantButton).Connect(NMerchantButton.SignalName.MerchantOpened, Callable.From<NMerchantButton>((Action<NMerchantButton>)OnMerchantOpened), 0u);
		Inventory = ((Node)this).GetNode<NMerchantInventory>(NodePath.op_Implicit("%Inventory"));
		((Control)Inventory).MouseFilter = (MouseFilterEnum)2;
		Inventory.Initialize(Room.Inventory, _dialogue);
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

	public void FoulPotionThrown(FoulPotion potion)
	{
		SfxCmd.Play("event:/sfx/npcs/merchant/merchant_thank_yous");
		LocString locString = Rng.Chaotic.NextItem(_dialogue.FoulPotionLines);
		if (locString != null)
		{
			NSpeechBubbleVfx nSpeechBubbleVfx = MerchantButton.PlayDialogue(locString);
			if (nSpeechBubbleVfx != null)
			{
				NGame.Instance?.ScreenRumble(ShakeStrength.Medium, ShakeDuration.Short, RumbleStyle.Rumble);
			}
		}
	}

	private void ToggleMerchantTrack()
	{
		NRunMusicController.Instance?.ToggleMerchantTrack();
	}

	private void AfterRoomIsLoaded()
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		Player me = LocalContext.GetMe(_players);
		_players.Remove(me);
		_players.Insert(0, me);
		int num = Mathf.CeilToInt(Mathf.Sqrt((float)_players.Count));
		for (int i = 0; i < num; i++)
		{
			float num2 = -140f * (float)i;
			for (int j = 0; j < num; j++)
			{
				int num3 = i * num + j;
				if (num3 >= _players.Count)
				{
					break;
				}
				NMerchantCharacter nMerchantCharacter = PreloadManager.Cache.GetScene(_players[num3].Character.MerchantAnimPath).Instantiate<NMerchantCharacter>((GenEditState)0);
				((Node)(object)_characterContainer).AddChildSafely((Node?)(object)nMerchantCharacter);
				((Node)_characterContainer).MoveChild((Node)(object)nMerchantCharacter, 0);
				((Node2D)nMerchantCharacter).Position = new Vector2(num2, -50f * (float)i);
				if (i > 0)
				{
					((CanvasItem)nMerchantCharacter).Modulate = new Color(0.5f, 0.5f, 0.5f, 1f);
				}
				num2 -= 275f;
				_playerVisuals.Add(nMerchantCharacter);
			}
		}
	}

	private void HideScreen(NButton _)
	{
		if (!MerchantFtueCheck())
		{
			NMapScreen.Instance.Open();
		}
	}

	private bool MerchantFtueCheck()
	{
		if (!SaveManager.Instance.SeenFtue("merchant_ftue"))
		{
			NModalContainer.Instance.Add((Node)(object)NMerchantFtue.Create(this));
			SaveManager.Instance.MarkFtueAsComplete("merchant_ftue");
			return true;
		}
		return false;
	}

	private void OnMerchantOpened(NMerchantButton _)
	{
		OpenInventory();
	}

	public void OpenInventory()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!Inventory.IsOpen)
		{
			_proceedButton.Disable();
			Inventory.Open();
			MerchantButton.Disable();
			((GodotObject)Inventory).Connect(NMerchantInventory.SignalName.InventoryClosed, Callable.From((Action)delegate
			{
				MerchantButton.Enable();
				_proceedButton.Enable();
				_proceedButton.SetPulseState(isPulsing: true);
			}), 4u);
		}
	}

	private void OnActiveScreenUpdated()
	{
		this.UpdateControllerNavEnabled();
		if (ActiveScreenContext.Instance.IsCurrent(this))
		{
			MerchantButton.Enable();
			_proceedButton.Enable();
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
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected O, but got Unknown
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(12);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleMerchantTrack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterRoomIsLoaded, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MerchantFtueCheck, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMerchantOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenInventory, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.HideScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			HideScreen(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MerchantFtueCheck && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = MerchantFtueCheck();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
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
		if ((ref method) == MethodName.HideScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.MerchantFtueCheck)
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
		if ((ref name) == PropertyName.Inventory)
		{
			Inventory = VariantUtils.ConvertTo<NMerchantInventory>(ref value);
			return true;
		}
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ProceedButton)
		{
			NProceedButton proceedButton = ProceedButton;
			value = VariantUtils.CreateFrom<NProceedButton>(ref proceedButton);
			return true;
		}
		if ((ref name) == PropertyName.Inventory)
		{
			NMerchantInventory inventory = Inventory;
			value = VariantUtils.CreateFrom<NMerchantInventory>(ref inventory);
			return true;
		}
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._proceedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.ProceedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._inputBlocker, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Inventory, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.MerchantButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName inventory = PropertyName.Inventory;
		NMerchantInventory inventory2 = Inventory;
		info.AddProperty(inventory, Variant.From<NMerchantInventory>(ref inventory2));
		StringName merchantButton = PropertyName.MerchantButton;
		NMerchantButton merchantButton2 = MerchantButton;
		info.AddProperty(merchantButton, Variant.From<NMerchantButton>(ref merchantButton2));
		info.AddProperty(PropertyName._proceedButton, Variant.From<NProceedButton>(ref _proceedButton));
		info.AddProperty(PropertyName._characterContainer, Variant.From<Control>(ref _characterContainer));
		info.AddProperty(PropertyName._inputBlocker, Variant.From<Control>(ref _inputBlocker));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Inventory, ref val))
		{
			Inventory = ((Variant)(ref val)).As<NMerchantInventory>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.MerchantButton, ref val2))
		{
			MerchantButton = ((Variant)(ref val2)).As<NMerchantButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._proceedButton, ref val3))
		{
			_proceedButton = ((Variant)(ref val3)).As<NProceedButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterContainer, ref val4))
		{
			_characterContainer = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._inputBlocker, ref val5))
		{
			_inputBlocker = ((Variant)(ref val5)).As<Control>();
		}
	}
}
