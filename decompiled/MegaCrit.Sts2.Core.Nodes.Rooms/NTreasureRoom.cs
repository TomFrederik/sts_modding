using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Screens.TreasureRoomRelic;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Rooms;

[ScriptPath("res://src/Core/Nodes/Rooms/NTreasureRoom.cs")]
public class NTreasureRoom : Control, IScreenContext, IRoomWithProceedButton
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnProceedButtonPressed = StringName.op_Implicit("OnProceedButtonPressed");

		public static readonly StringName OnProceedButtonReleased = StringName.op_Implicit("OnProceedButtonReleased");

		public static readonly StringName OnChestButtonReleased = StringName.op_Implicit("OnChestButtonReleased");

		public static readonly StringName OnMouseEntered = StringName.op_Implicit("OnMouseEntered");

		public static readonly StringName OnMouseExited = StringName.op_Implicit("OnMouseExited");

		public static readonly StringName UpdateChestSkin = StringName.op_Implicit("UpdateChestSkin");

		public static readonly StringName OnActiveScreenChanged = StringName.op_Implicit("OnActiveScreenChanged");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ProceedButton = StringName.op_Implicit("ProceedButton");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _banner = StringName.op_Implicit("_banner");

		public static readonly StringName _chestButton = StringName.op_Implicit("_chestButton");

		public static readonly StringName _chestNode = StringName.op_Implicit("_chestNode");

		public static readonly StringName _proceedButton = StringName.op_Implicit("_proceedButton");

		public static readonly StringName _goldParticles = StringName.op_Implicit("_goldParticles");

		public static readonly StringName _relicCollection = StringName.op_Implicit("_relicCollection");

		public static readonly StringName _isRelicCollectionOpen = StringName.op_Implicit("_isRelicCollectionOpen");

		public static readonly StringName _hasRelicBeenClaimed = StringName.op_Implicit("_hasRelicBeenClaimed");
	}

	public class SignalName : SignalName
	{
	}

	private TreasureRoom _room;

	private IRunState _runState;

	private NCommonBanner _banner;

	private NButton _chestButton;

	private Node2D _chestNode;

	private MegaSprite _chestAnimController;

	private NProceedButton _proceedButton;

	private MegaSkin _regularChestSkin;

	private MegaSkin _outlineChestSkin;

	private GpuParticles2D _goldParticles;

	private NTreasureRoomRelicCollection _relicCollection;

	private static readonly string _scenePath = SceneHelper.GetScenePath("rooms/treasure_room");

	private bool _isRelicCollectionOpen;

	private bool _hasRelicBeenClaimed;

	public NProceedButton ProceedButton => _proceedButton;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public Control? DefaultFocusedControl
	{
		get
		{
			if (!_isRelicCollectionOpen)
			{
				return null;
			}
			return _relicCollection.DefaultFocusedControl;
		}
	}

	public static NTreasureRoom? Create(TreasureRoom room, IRunState runState)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NTreasureRoom nTreasureRoom = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NTreasureRoom>((GenEditState)0);
		nTreasureRoom._room = room;
		nTreasureRoom._runState = runState;
		return nTreasureRoom;
	}

	public override void _Ready()
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		_banner = ((Node)this).GetNode<NCommonBanner>(NodePath.op_Implicit("%Banner"));
		if (_runState.Players.Count == 1)
		{
			_banner.label.SetTextAutoSize(new LocString("gameplay_ui", "TREASURE_BANNER").GetFormattedText());
		}
		else
		{
			_banner.label.SetTextAutoSize(new LocString("gameplay_ui", "CHOOSE_SHARED_RELIC_HEADER").GetFormattedText());
		}
		_proceedButton = ((Node)this).GetNode<NProceedButton>(NodePath.op_Implicit("%ProceedButton"));
		_chestNode = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%ChestVisual"));
		_chestAnimController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_chestNode));
		_goldParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("%GoldExplosion"));
		_relicCollection = ((Node)this).GetNode<NTreasureRoomRelicCollection>(NodePath.op_Implicit("%RelicCollection"));
		_relicCollection.Initialize(_runState);
		((CanvasItem)_relicCollection).Visible = false;
		_chestAnimController.SetSkeletonDataRes(_runState.Act.ChestSpineResource);
		MegaSkeleton skeleton = _chestAnimController.GetSkeleton();
		MegaSkeletonDataResource data = skeleton.GetData();
		_regularChestSkin = data.FindSkin(_runState.Act.ChestSpineSkinNameNormal);
		_outlineChestSkin = data.FindSkin(_runState.Act.ChestSpineSkinNameStroke);
		skeleton.SetSlotsToSetupPose();
		_chestAnimController.GetAnimationState().Apply(skeleton);
		MegaAnimationState animationState = _chestAnimController.GetAnimationState();
		animationState.SetAnimation("animation", loop: false);
		_chestAnimController.GetAnimationState().AddAnimation("shine_fade", 0f, loop: false);
		((GodotObject)_proceedButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnProceedButtonPressed), 0u);
		_proceedButton.UpdateText(NProceedButton.ProceedLoc);
		animationState.SetTimeScale(0f);
		UpdateChestSkin(showOutline: false);
		_chestButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("%Chest"));
		((GodotObject)_chestButton).Connect(SignalName.MouseEntered, Callable.From((Action)OnMouseEntered), 0u);
		((GodotObject)_chestButton).Connect(SignalName.MouseExited, Callable.From((Action)OnMouseExited), 0u);
		((GodotObject)_chestButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnChestButtonReleased), 0u);
	}

	public override void _EnterTree()
	{
		ActiveScreenContext.Instance.Updated += OnActiveScreenChanged;
	}

	public override void _ExitTree()
	{
		ActiveScreenContext.Instance.Updated -= OnActiveScreenChanged;
	}

	private void OnProceedButtonPressed(NButton _)
	{
		TaskHelper.RunSafely(RunManager.Instance.ProceedFromTerminalRewardsScreen());
	}

	private void OnProceedButtonReleased(NButton _)
	{
		NMapScreen.Instance.Open();
	}

	private void OnChestButtonReleased(NButton _)
	{
		TaskHelper.RunSafely(OpenChest());
		_chestButton.Disable();
	}

	private void OnMouseEntered()
	{
		UpdateChestSkin(showOutline: true);
	}

	private void OnMouseExited()
	{
		UpdateChestSkin(showOutline: false);
	}

	private async Task OpenChest()
	{
		_banner.AnimateIn();
		_proceedButton.Disable();
		UpdateChestSkin(showOutline: false);
		SfxCmd.Play(_runState.Act.ChestOpenSfx);
		_chestAnimController.GetAnimationState().SetTimeScale(1f);
		((Control)_chestButton).MouseFilter = (MouseFilterEnum)2;
		int num = await _room.DoNormalRewards();
		if (num > 0)
		{
			_goldParticles.Amount = num;
			_goldParticles.Emitting = true;
		}
		await _room.DoExtraRewardsIfNeeded();
		_relicCollection.InitializeRelics();
		_relicCollection.AnimIn((Node)(object)_chestNode);
		_isRelicCollectionOpen = true;
		ActiveScreenContext.Instance.Update();
		TaskHelper.RunSafely(RelicFtueCheck());
		await _relicCollection.RelicPickingFinished();
		_isRelicCollectionOpen = false;
		_banner.AnimateOut();
		NMapScreen.Instance.SetTravelEnabled(enabled: true);
		_proceedButton.Enable();
		_hasRelicBeenClaimed = true;
		_relicCollection.AnimOut((Node)(object)_chestNode);
	}

	private async Task RelicFtueCheck()
	{
		if (!SaveManager.Instance.SeenFtue("obtain_relic_ftue"))
		{
			_relicCollection.SetSelectionEnabled(isEnabled: false);
			await Cmd.Wait(1f);
			Control relicReward = (Control)((!((CanvasItem)_relicCollection.SingleplayerRelicHolder).Visible) ? ((object)_relicCollection) : ((object)_relicCollection.SingleplayerRelicHolder));
			_relicCollection.SetSelectionEnabled(isEnabled: true);
			NModalContainer.Instance.Add((Node)(object)NRelicRewardFtue.Create(relicReward));
			SaveManager.Instance.MarkFtueAsComplete("obtain_relic_ftue");
		}
	}

	private void UpdateChestSkin(bool showOutline)
	{
		MegaSkeleton skeleton = _chestAnimController.GetSkeleton();
		skeleton.SetSkin(showOutline ? _outlineChestSkin : _regularChestSkin);
		skeleton.SetSlotsToSetupPose();
		_chestAnimController.GetAnimationState().Apply(skeleton);
	}

	private void OnActiveScreenChanged()
	{
		this.UpdateControllerNavEnabled();
		if (ActiveScreenContext.Instance.IsCurrent(this) && _hasRelicBeenClaimed)
		{
			_proceedButton.Enable();
		}
		else
		{
			_proceedButton.Disable();
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
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnProceedButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnProceedButtonReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnChestButtonReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMouseEntered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMouseExited, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateChestSkin, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("showOutline"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnActiveScreenChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnProceedButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnProceedButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnProceedButtonReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnProceedButtonReleased(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnChestButtonReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnChestButtonReleased(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMouseEntered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnMouseEntered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMouseExited && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnMouseExited();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateChestSkin && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateChestSkin(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnActiveScreenChanged();
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
		if ((ref method) == MethodName.OnProceedButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnProceedButtonReleased)
		{
			return true;
		}
		if ((ref method) == MethodName.OnChestButtonReleased)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMouseEntered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMouseExited)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateChestSkin)
		{
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenChanged)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._banner)
		{
			_banner = VariantUtils.ConvertTo<NCommonBanner>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._chestButton)
		{
			_chestButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._chestNode)
		{
			_chestNode = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			_proceedButton = VariantUtils.ConvertTo<NProceedButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._goldParticles)
		{
			_goldParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicCollection)
		{
			_relicCollection = VariantUtils.ConvertTo<NTreasureRoomRelicCollection>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isRelicCollectionOpen)
		{
			_isRelicCollectionOpen = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hasRelicBeenClaimed)
		{
			_hasRelicBeenClaimed = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName.ProceedButton)
		{
			NProceedButton proceedButton = ProceedButton;
			value = VariantUtils.CreateFrom<NProceedButton>(ref proceedButton);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			value = VariantUtils.CreateFrom<NCommonBanner>(ref _banner);
			return true;
		}
		if ((ref name) == PropertyName._chestButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _chestButton);
			return true;
		}
		if ((ref name) == PropertyName._chestNode)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _chestNode);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			value = VariantUtils.CreateFrom<NProceedButton>(ref _proceedButton);
			return true;
		}
		if ((ref name) == PropertyName._goldParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _goldParticles);
			return true;
		}
		if ((ref name) == PropertyName._relicCollection)
		{
			value = VariantUtils.CreateFrom<NTreasureRoomRelicCollection>(ref _relicCollection);
			return true;
		}
		if ((ref name) == PropertyName._isRelicCollectionOpen)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isRelicCollectionOpen);
			return true;
		}
		if ((ref name) == PropertyName._hasRelicBeenClaimed)
		{
			value = VariantUtils.CreateFrom<bool>(ref _hasRelicBeenClaimed);
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
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._banner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._chestButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._chestNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._proceedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.ProceedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._goldParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicCollection, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isRelicCollectionOpen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._hasRelicBeenClaimed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._banner, Variant.From<NCommonBanner>(ref _banner));
		info.AddProperty(PropertyName._chestButton, Variant.From<NButton>(ref _chestButton));
		info.AddProperty(PropertyName._chestNode, Variant.From<Node2D>(ref _chestNode));
		info.AddProperty(PropertyName._proceedButton, Variant.From<NProceedButton>(ref _proceedButton));
		info.AddProperty(PropertyName._goldParticles, Variant.From<GpuParticles2D>(ref _goldParticles));
		info.AddProperty(PropertyName._relicCollection, Variant.From<NTreasureRoomRelicCollection>(ref _relicCollection));
		info.AddProperty(PropertyName._isRelicCollectionOpen, Variant.From<bool>(ref _isRelicCollectionOpen));
		info.AddProperty(PropertyName._hasRelicBeenClaimed, Variant.From<bool>(ref _hasRelicBeenClaimed));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._banner, ref val))
		{
			_banner = ((Variant)(ref val)).As<NCommonBanner>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._chestButton, ref val2))
		{
			_chestButton = ((Variant)(ref val2)).As<NButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._chestNode, ref val3))
		{
			_chestNode = ((Variant)(ref val3)).As<Node2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._proceedButton, ref val4))
		{
			_proceedButton = ((Variant)(ref val4)).As<NProceedButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._goldParticles, ref val5))
		{
			_goldParticles = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicCollection, ref val6))
		{
			_relicCollection = ((Variant)(ref val6)).As<NTreasureRoomRelicCollection>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isRelicCollectionOpen, ref val7))
		{
			_isRelicCollectionOpen = ((Variant)(ref val7)).As<bool>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._hasRelicBeenClaimed, ref val8))
		{
			_hasRelicBeenClaimed = ((Variant)(ref val8)).As<bool>();
		}
	}
}
