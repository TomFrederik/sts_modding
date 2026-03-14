using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.FeedbackScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.PauseMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;

namespace MegaCrit.Sts2.Core.Nodes.Screens;

[ScriptPath("res://src/Core/Nodes/Screens/NCapstoneSubmenuStack.cs")]
public class NCapstoneSubmenuStack : Control, ICapstoneScreen, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName ShowScreen = StringName.op_Implicit("ShowScreen");

		public static readonly StringName GetCapstoneSubmenuType = StringName.op_Implicit("GetCapstoneSubmenuType");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnSubmenuStackChanged = StringName.op_Implicit("OnSubmenuStackChanged");

		public static readonly StringName AfterCapstoneOpened = StringName.op_Implicit("AfterCapstoneOpened");

		public static readonly StringName AfterCapstoneClosed = StringName.op_Implicit("AfterCapstoneClosed");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Type = StringName.op_Implicit("Type");

		public static readonly StringName Stack = StringName.op_Implicit("Stack");

		public static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public static readonly StringName UseSharedBackstop = StringName.op_Implicit("UseSharedBackstop");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");
	}

	public class SignalName : SignalName
	{
	}

	private static string ScenePath => SceneHelper.GetScenePath("screens/capstone_submenu_stack");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public CapstoneSubmenuType Type { get; private set; }

	public NRunSubmenuStack Stack { get; private set; }

	public NetScreenType ScreenType => GetCapstoneSubmenuType();

	public bool UseSharedBackstop => true;

	public Control? DefaultFocusedControl => Stack.Peek()?.DefaultFocusedControl;

	public NSubmenu ShowScreen(CapstoneSubmenuType type)
	{
		while (Stack.Peek() != null)
		{
			Stack.Pop();
		}
		Type type2 = type switch
		{
			CapstoneSubmenuType.Compendium => typeof(NCompendiumSubmenu), 
			CapstoneSubmenuType.Feedback => typeof(NSendFeedbackScreen), 
			CapstoneSubmenuType.PauseMenu => typeof(NPauseMenu), 
			CapstoneSubmenuType.Settings => typeof(NSettingsScreen), 
			_ => throw new ArgumentOutOfRangeException("type", type, null), 
		};
		NSubmenu result = Stack.PushSubmenuType(type2);
		Type = type;
		NCapstoneContainer.Instance.Open(this);
		return result;
	}

	private NetScreenType GetCapstoneSubmenuType()
	{
		return Type switch
		{
			CapstoneSubmenuType.Compendium => NetScreenType.Compendium, 
			CapstoneSubmenuType.Feedback => NetScreenType.Feedback, 
			CapstoneSubmenuType.PauseMenu => NetScreenType.PauseMenu, 
			CapstoneSubmenuType.Settings => NetScreenType.Settings, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public override void _Ready()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Stack = ((Node)this).GetNode<NRunSubmenuStack>(NodePath.op_Implicit("%Submenus"));
		((GodotObject)Stack).Connect(NSubmenuStack.SignalName.StackModified, Callable.From((Action)OnSubmenuStackChanged), 0u);
	}

	private void OnSubmenuStackChanged()
	{
		if (Stack.Peek() == null && NCapstoneContainer.Instance.CurrentCapstoneScreen == this)
		{
			NCapstoneContainer.Instance.Close();
		}
	}

	public void AfterCapstoneOpened()
	{
		NGlobalUi globalUi = NRun.Instance.GlobalUi;
		globalUi.TopBar.AnimHide();
		globalUi.RelicInventory.AnimHide();
		globalUi.MultiplayerPlayerContainer.AnimHide();
		SfxCmd.Play("event:/sfx/ui/pause_open");
		((Node)globalUi).MoveChild((Node)(object)globalUi.AboveTopBarVfxContainer, ((Node)globalUi.CapstoneContainer).GetIndex(false));
		((Node)globalUi).MoveChild((Node)(object)globalUi.CardPreviewContainer, ((Node)globalUi.CapstoneContainer).GetIndex(false));
		((Node)globalUi).MoveChild((Node)(object)globalUi.MessyCardPreviewContainer, ((Node)globalUi.CapstoneContainer).GetIndex(false));
		((CanvasItem)this).Visible = true;
	}

	public void AfterCapstoneClosed()
	{
		while (Stack.Peek() != null)
		{
			Stack.Pop();
		}
		SfxCmd.Play("event:/sfx/ui/pause_close");
		NGlobalUi globalUi = NRun.Instance.GlobalUi;
		globalUi.TopBar.AnimShow();
		globalUi.RelicInventory.AnimShow();
		globalUi.MultiplayerPlayerContainer.AnimShow();
		((Node)globalUi).MoveChild((Node)(object)globalUi.AboveTopBarVfxContainer, ((Node)globalUi.TopBar).GetIndex(false) + 1);
		((Node)globalUi).MoveChild((Node)(object)globalUi.CardPreviewContainer, ((Node)globalUi.TopBar).GetIndex(false) + 1);
		((Node)globalUi).MoveChild((Node)(object)globalUi.MessyCardPreviewContainer, ((Node)globalUi.TopBar).GetIndex(false) + 1);
		((CanvasItem)this).Visible = false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName.ShowScreen, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("type"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetCapstoneSubmenuType, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuStackChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterCapstoneOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterCapstoneClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.ShowScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NSubmenu nSubmenu = ShowScreen(VariantUtils.ConvertTo<CapstoneSubmenuType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NSubmenu>(ref nSubmenu);
			return true;
		}
		if ((ref method) == MethodName.GetCapstoneSubmenuType && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NetScreenType capstoneSubmenuType = GetCapstoneSubmenuType();
			ret = VariantUtils.CreateFrom<NetScreenType>(ref capstoneSubmenuType);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuStackChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuStackChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterCapstoneOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterCapstoneOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterCapstoneClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterCapstoneClosed();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.ShowScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.GetCapstoneSubmenuType)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuStackChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterCapstoneOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterCapstoneClosed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Type)
		{
			Type = VariantUtils.ConvertTo<CapstoneSubmenuType>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Stack)
		{
			Stack = VariantUtils.ConvertTo<NRunSubmenuStack>(ref value);
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
		if ((ref name) == PropertyName.Type)
		{
			CapstoneSubmenuType type = Type;
			value = VariantUtils.CreateFrom<CapstoneSubmenuType>(ref type);
			return true;
		}
		if ((ref name) == PropertyName.Stack)
		{
			NRunSubmenuStack stack = Stack;
			value = VariantUtils.CreateFrom<NRunSubmenuStack>(ref stack);
			return true;
		}
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
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName.Type, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Stack, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.ScreenType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.UseSharedBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName type = PropertyName.Type;
		CapstoneSubmenuType type2 = Type;
		info.AddProperty(type, Variant.From<CapstoneSubmenuType>(ref type2));
		StringName stack = PropertyName.Stack;
		NRunSubmenuStack stack2 = Stack;
		info.AddProperty(stack, Variant.From<NRunSubmenuStack>(ref stack2));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Type, ref val))
		{
			Type = ((Variant)(ref val)).As<CapstoneSubmenuType>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.Stack, ref val2))
		{
			Stack = ((Variant)(ref val2)).As<NRunSubmenuStack>();
		}
	}
}
