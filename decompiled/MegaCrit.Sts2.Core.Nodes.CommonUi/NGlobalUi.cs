using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NGlobalUi.cs")]
public class NGlobalUi : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnWindowChange = StringName.op_Implicit("OnWindowChange");

		public static readonly StringName ReparentCard = StringName.op_Implicit("ReparentCard");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName TopBar = StringName.op_Implicit("TopBar");

		public static readonly StringName Overlays = StringName.op_Implicit("Overlays");

		public static readonly StringName CapstoneContainer = StringName.op_Implicit("CapstoneContainer");

		public static readonly StringName RelicInventory = StringName.op_Implicit("RelicInventory");

		public static readonly StringName EventCardPreviewContainer = StringName.op_Implicit("EventCardPreviewContainer");

		public static readonly StringName CardPreviewContainer = StringName.op_Implicit("CardPreviewContainer");

		public static readonly StringName MessyCardPreviewContainer = StringName.op_Implicit("MessyCardPreviewContainer");

		public static readonly StringName GridCardPreviewContainer = StringName.op_Implicit("GridCardPreviewContainer");

		public static readonly StringName AboveTopBarVfxContainer = StringName.op_Implicit("AboveTopBarVfxContainer");

		public static readonly StringName MapScreen = StringName.op_Implicit("MapScreen");

		public static readonly StringName MultiplayerPlayerContainer = StringName.op_Implicit("MultiplayerPlayerContainer");

		public static readonly StringName TimeoutOverlay = StringName.op_Implicit("TimeoutOverlay");

		public static readonly StringName SubmenuStack = StringName.op_Implicit("SubmenuStack");

		public static readonly StringName TargetManager = StringName.op_Implicit("TargetManager");

		public static readonly StringName _window = StringName.op_Implicit("_window");
	}

	public class SignalName : SignalName
	{
	}

	private const float _maxNarrowRatio = 1.3333334f;

	private const float _maxWideRatio = 2.3888888f;

	private Window _window;

	public NTopBar TopBar { get; private set; }

	public NOverlayStack Overlays { get; private set; }

	public NCapstoneContainer CapstoneContainer { get; private set; }

	public NRelicInventory RelicInventory { get; private set; }

	public Control EventCardPreviewContainer { get; private set; }

	public Control CardPreviewContainer { get; private set; }

	public NMessyCardPreviewContainer MessyCardPreviewContainer { get; private set; }

	public NGridCardPreviewContainer GridCardPreviewContainer { get; private set; }

	public Control AboveTopBarVfxContainer { get; private set; }

	public NMapScreen MapScreen { get; private set; }

	public NMultiplayerPlayerStateContainer MultiplayerPlayerContainer { get; private set; }

	public NMultiplayerTimeoutOverlay TimeoutOverlay { get; private set; }

	public NCapstoneSubmenuStack SubmenuStack { get; private set; }

	public NTargetManager TargetManager { get; private set; }

	public override void _Ready()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		_window = ((Node)this).GetTree().Root;
		((GodotObject)_window).Connect(SignalName.SizeChanged, Callable.From((Action)OnWindowChange), 0u);
		EventCardPreviewContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%EventCardPreviewContainer"));
		CardPreviewContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CardPreviewContainer"));
		MessyCardPreviewContainer = ((Node)this).GetNode<NMessyCardPreviewContainer>(NodePath.op_Implicit("%MessyCardPreviewContainer"));
		GridCardPreviewContainer = ((Node)this).GetNode<NGridCardPreviewContainer>(NodePath.op_Implicit("%GridCardPreviewContainer"));
		TopBar = ((Node)this).GetNode<NTopBar>(NodePath.op_Implicit("%TopBar"));
		Overlays = ((Node)this).GetNode<NOverlayStack>(NodePath.op_Implicit("%OverlayScreensContainer"));
		CapstoneContainer = ((Node)this).GetNode<NCapstoneContainer>(NodePath.op_Implicit("%CapstoneScreenContainer"));
		MapScreen = ((Node)this).GetNode<NMapScreen>(NodePath.op_Implicit("%MapScreen"));
		SubmenuStack = ((Node)this).GetNode<NCapstoneSubmenuStack>(NodePath.op_Implicit("%CapstoneSubmenuStack"));
		RelicInventory = ((Node)this).GetNode<NRelicInventory>(NodePath.op_Implicit("%RelicInventory"));
		MultiplayerPlayerContainer = ((Node)this).GetNode<NMultiplayerPlayerStateContainer>(NodePath.op_Implicit("%MultiplayerPlayerContainer"));
		TargetManager = ((Node)this).GetNode<NTargetManager>(NodePath.op_Implicit("TargetManager"));
		TimeoutOverlay = ((Node)this).GetNode<NMultiplayerTimeoutOverlay>(NodePath.op_Implicit("%MultiplayerTimeoutOverlay"));
		AboveTopBarVfxContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%AboveTopBarVfxContainer"));
	}

	private void OnWindowChange()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (SaveManager.Instance.SettingsSave.AspectRatioSetting == AspectRatioSetting.Auto)
		{
			float num = (float)_window.Size.X / (float)_window.Size.Y;
			if (num > 2.3888888f)
			{
				_window.ContentScaleAspect = (ContentScaleAspectEnum)2;
				_window.ContentScaleSize = new Vector2I(2580, 1080);
			}
			else if (num < 1.3333334f)
			{
				_window.ContentScaleAspect = (ContentScaleAspectEnum)3;
				_window.ContentScaleSize = new Vector2I(1680, 1260);
			}
			else
			{
				_window.ContentScaleAspect = (ContentScaleAspectEnum)4;
				_window.ContentScaleSize = new Vector2I(1680, 1080);
			}
		}
	}

	public void ReparentCard(NCard card)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 globalPosition = ((Control)card).GlobalPosition;
		((Node)card).GetParent()?.RemoveChildSafely((Node?)(object)card);
		TopBar.TrailContainer.AddChildSafely((Node?)(object)card);
		((Control)card).GlobalPosition = globalPosition;
	}

	public void Initialize(RunState runState)
	{
		TopBar.Initialize(runState);
		MultiplayerPlayerContainer.Initialize(runState);
		RelicInventory.Initialize(runState);
		MapScreen.Initialize(runState);
		TimeoutOverlay.Initialize(RunManager.Instance.NetService, isGameLevel: false);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReparentCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnWindowChange();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ReparentCard && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ReparentCard(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.OnWindowChange)
		{
			return true;
		}
		if ((ref method) == MethodName.ReparentCard)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.TopBar)
		{
			TopBar = VariantUtils.ConvertTo<NTopBar>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Overlays)
		{
			Overlays = VariantUtils.ConvertTo<NOverlayStack>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.CapstoneContainer)
		{
			CapstoneContainer = VariantUtils.ConvertTo<NCapstoneContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.RelicInventory)
		{
			RelicInventory = VariantUtils.ConvertTo<NRelicInventory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.EventCardPreviewContainer)
		{
			EventCardPreviewContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.CardPreviewContainer)
		{
			CardPreviewContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.MessyCardPreviewContainer)
		{
			MessyCardPreviewContainer = VariantUtils.ConvertTo<NMessyCardPreviewContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.GridCardPreviewContainer)
		{
			GridCardPreviewContainer = VariantUtils.ConvertTo<NGridCardPreviewContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.AboveTopBarVfxContainer)
		{
			AboveTopBarVfxContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.MapScreen)
		{
			MapScreen = VariantUtils.ConvertTo<NMapScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.MultiplayerPlayerContainer)
		{
			MultiplayerPlayerContainer = VariantUtils.ConvertTo<NMultiplayerPlayerStateContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.TimeoutOverlay)
		{
			TimeoutOverlay = VariantUtils.ConvertTo<NMultiplayerTimeoutOverlay>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.SubmenuStack)
		{
			SubmenuStack = VariantUtils.ConvertTo<NCapstoneSubmenuStack>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.TargetManager)
		{
			TargetManager = VariantUtils.ConvertTo<NTargetManager>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._window)
		{
			_window = VariantUtils.ConvertTo<Window>(ref value);
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
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.TopBar)
		{
			NTopBar topBar = TopBar;
			value = VariantUtils.CreateFrom<NTopBar>(ref topBar);
			return true;
		}
		if ((ref name) == PropertyName.Overlays)
		{
			NOverlayStack overlays = Overlays;
			value = VariantUtils.CreateFrom<NOverlayStack>(ref overlays);
			return true;
		}
		if ((ref name) == PropertyName.CapstoneContainer)
		{
			NCapstoneContainer capstoneContainer = CapstoneContainer;
			value = VariantUtils.CreateFrom<NCapstoneContainer>(ref capstoneContainer);
			return true;
		}
		if ((ref name) == PropertyName.RelicInventory)
		{
			NRelicInventory relicInventory = RelicInventory;
			value = VariantUtils.CreateFrom<NRelicInventory>(ref relicInventory);
			return true;
		}
		if ((ref name) == PropertyName.EventCardPreviewContainer)
		{
			Control eventCardPreviewContainer = EventCardPreviewContainer;
			value = VariantUtils.CreateFrom<Control>(ref eventCardPreviewContainer);
			return true;
		}
		if ((ref name) == PropertyName.CardPreviewContainer)
		{
			Control eventCardPreviewContainer = CardPreviewContainer;
			value = VariantUtils.CreateFrom<Control>(ref eventCardPreviewContainer);
			return true;
		}
		if ((ref name) == PropertyName.MessyCardPreviewContainer)
		{
			NMessyCardPreviewContainer messyCardPreviewContainer = MessyCardPreviewContainer;
			value = VariantUtils.CreateFrom<NMessyCardPreviewContainer>(ref messyCardPreviewContainer);
			return true;
		}
		if ((ref name) == PropertyName.GridCardPreviewContainer)
		{
			NGridCardPreviewContainer gridCardPreviewContainer = GridCardPreviewContainer;
			value = VariantUtils.CreateFrom<NGridCardPreviewContainer>(ref gridCardPreviewContainer);
			return true;
		}
		if ((ref name) == PropertyName.AboveTopBarVfxContainer)
		{
			Control eventCardPreviewContainer = AboveTopBarVfxContainer;
			value = VariantUtils.CreateFrom<Control>(ref eventCardPreviewContainer);
			return true;
		}
		if ((ref name) == PropertyName.MapScreen)
		{
			NMapScreen mapScreen = MapScreen;
			value = VariantUtils.CreateFrom<NMapScreen>(ref mapScreen);
			return true;
		}
		if ((ref name) == PropertyName.MultiplayerPlayerContainer)
		{
			NMultiplayerPlayerStateContainer multiplayerPlayerContainer = MultiplayerPlayerContainer;
			value = VariantUtils.CreateFrom<NMultiplayerPlayerStateContainer>(ref multiplayerPlayerContainer);
			return true;
		}
		if ((ref name) == PropertyName.TimeoutOverlay)
		{
			NMultiplayerTimeoutOverlay timeoutOverlay = TimeoutOverlay;
			value = VariantUtils.CreateFrom<NMultiplayerTimeoutOverlay>(ref timeoutOverlay);
			return true;
		}
		if ((ref name) == PropertyName.SubmenuStack)
		{
			NCapstoneSubmenuStack submenuStack = SubmenuStack;
			value = VariantUtils.CreateFrom<NCapstoneSubmenuStack>(ref submenuStack);
			return true;
		}
		if ((ref name) == PropertyName.TargetManager)
		{
			NTargetManager targetManager = TargetManager;
			value = VariantUtils.CreateFrom<NTargetManager>(ref targetManager);
			return true;
		}
		if ((ref name) == PropertyName._window)
		{
			value = VariantUtils.CreateFrom<Window>(ref _window);
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
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._window, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.TopBar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Overlays, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CapstoneContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.RelicInventory, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.EventCardPreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CardPreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.MessyCardPreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.GridCardPreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.AboveTopBarVfxContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.MapScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.MultiplayerPlayerContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.TimeoutOverlay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.SubmenuStack, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.TargetManager, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName topBar = PropertyName.TopBar;
		NTopBar topBar2 = TopBar;
		info.AddProperty(topBar, Variant.From<NTopBar>(ref topBar2));
		StringName overlays = PropertyName.Overlays;
		NOverlayStack overlays2 = Overlays;
		info.AddProperty(overlays, Variant.From<NOverlayStack>(ref overlays2));
		StringName capstoneContainer = PropertyName.CapstoneContainer;
		NCapstoneContainer capstoneContainer2 = CapstoneContainer;
		info.AddProperty(capstoneContainer, Variant.From<NCapstoneContainer>(ref capstoneContainer2));
		StringName relicInventory = PropertyName.RelicInventory;
		NRelicInventory relicInventory2 = RelicInventory;
		info.AddProperty(relicInventory, Variant.From<NRelicInventory>(ref relicInventory2));
		StringName eventCardPreviewContainer = PropertyName.EventCardPreviewContainer;
		Control eventCardPreviewContainer2 = EventCardPreviewContainer;
		info.AddProperty(eventCardPreviewContainer, Variant.From<Control>(ref eventCardPreviewContainer2));
		StringName cardPreviewContainer = PropertyName.CardPreviewContainer;
		eventCardPreviewContainer2 = CardPreviewContainer;
		info.AddProperty(cardPreviewContainer, Variant.From<Control>(ref eventCardPreviewContainer2));
		StringName messyCardPreviewContainer = PropertyName.MessyCardPreviewContainer;
		NMessyCardPreviewContainer messyCardPreviewContainer2 = MessyCardPreviewContainer;
		info.AddProperty(messyCardPreviewContainer, Variant.From<NMessyCardPreviewContainer>(ref messyCardPreviewContainer2));
		StringName gridCardPreviewContainer = PropertyName.GridCardPreviewContainer;
		NGridCardPreviewContainer gridCardPreviewContainer2 = GridCardPreviewContainer;
		info.AddProperty(gridCardPreviewContainer, Variant.From<NGridCardPreviewContainer>(ref gridCardPreviewContainer2));
		StringName aboveTopBarVfxContainer = PropertyName.AboveTopBarVfxContainer;
		eventCardPreviewContainer2 = AboveTopBarVfxContainer;
		info.AddProperty(aboveTopBarVfxContainer, Variant.From<Control>(ref eventCardPreviewContainer2));
		StringName mapScreen = PropertyName.MapScreen;
		NMapScreen mapScreen2 = MapScreen;
		info.AddProperty(mapScreen, Variant.From<NMapScreen>(ref mapScreen2));
		StringName multiplayerPlayerContainer = PropertyName.MultiplayerPlayerContainer;
		NMultiplayerPlayerStateContainer multiplayerPlayerContainer2 = MultiplayerPlayerContainer;
		info.AddProperty(multiplayerPlayerContainer, Variant.From<NMultiplayerPlayerStateContainer>(ref multiplayerPlayerContainer2));
		StringName timeoutOverlay = PropertyName.TimeoutOverlay;
		NMultiplayerTimeoutOverlay timeoutOverlay2 = TimeoutOverlay;
		info.AddProperty(timeoutOverlay, Variant.From<NMultiplayerTimeoutOverlay>(ref timeoutOverlay2));
		StringName submenuStack = PropertyName.SubmenuStack;
		NCapstoneSubmenuStack submenuStack2 = SubmenuStack;
		info.AddProperty(submenuStack, Variant.From<NCapstoneSubmenuStack>(ref submenuStack2));
		StringName targetManager = PropertyName.TargetManager;
		NTargetManager targetManager2 = TargetManager;
		info.AddProperty(targetManager, Variant.From<NTargetManager>(ref targetManager2));
		info.AddProperty(PropertyName._window, Variant.From<Window>(ref _window));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.TopBar, ref val))
		{
			TopBar = ((Variant)(ref val)).As<NTopBar>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.Overlays, ref val2))
		{
			Overlays = ((Variant)(ref val2)).As<NOverlayStack>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.CapstoneContainer, ref val3))
		{
			CapstoneContainer = ((Variant)(ref val3)).As<NCapstoneContainer>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.RelicInventory, ref val4))
		{
			RelicInventory = ((Variant)(ref val4)).As<NRelicInventory>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName.EventCardPreviewContainer, ref val5))
		{
			EventCardPreviewContainer = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName.CardPreviewContainer, ref val6))
		{
			CardPreviewContainer = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName.MessyCardPreviewContainer, ref val7))
		{
			MessyCardPreviewContainer = ((Variant)(ref val7)).As<NMessyCardPreviewContainer>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName.GridCardPreviewContainer, ref val8))
		{
			GridCardPreviewContainer = ((Variant)(ref val8)).As<NGridCardPreviewContainer>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName.AboveTopBarVfxContainer, ref val9))
		{
			AboveTopBarVfxContainer = ((Variant)(ref val9)).As<Control>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName.MapScreen, ref val10))
		{
			MapScreen = ((Variant)(ref val10)).As<NMapScreen>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName.MultiplayerPlayerContainer, ref val11))
		{
			MultiplayerPlayerContainer = ((Variant)(ref val11)).As<NMultiplayerPlayerStateContainer>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName.TimeoutOverlay, ref val12))
		{
			TimeoutOverlay = ((Variant)(ref val12)).As<NMultiplayerTimeoutOverlay>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName.SubmenuStack, ref val13))
		{
			SubmenuStack = ((Variant)(ref val13)).As<NCapstoneSubmenuStack>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName.TargetManager, ref val14))
		{
			TargetManager = ((Variant)(ref val14)).As<NTargetManager>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._window, ref val15))
		{
			_window = ((Variant)(ref val15)).As<Window>();
		}
	}
}
