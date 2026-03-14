using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;

[ScriptPath("res://src/Core/Nodes/Screens/DailyRun/NDailyRunCharacterContainer.cs")]
public class NDailyRunCharacterContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetIsReady = StringName.op_Implicit("SetIsReady");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _characterIconContainer = StringName.op_Implicit("_characterIconContainer");

		public static readonly StringName _playerNameLabel = StringName.op_Implicit("_playerNameLabel");

		public static readonly StringName _characterNameLabel = StringName.op_Implicit("_characterNameLabel");

		public static readonly StringName _ascensionLabel = StringName.op_Implicit("_ascensionLabel");

		public static readonly StringName _ascensionNumberLabel = StringName.op_Implicit("_ascensionNumberLabel");

		public static readonly StringName _readyIndicator = StringName.op_Implicit("_readyIndicator");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly LocString _ascensionLoc = new LocString("main_menu_ui", "DAILY_RUN_MENU.ASCENSION");

	private Control _characterIconContainer;

	private MegaLabel _playerNameLabel;

	private MegaLabel _characterNameLabel;

	private MegaLabel _ascensionLabel;

	private MegaLabel _ascensionNumberLabel;

	private Control _readyIndicator;

	public override void _Ready()
	{
		_characterIconContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CharacterIconContainer"));
		_playerNameLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%PlayerNameLabel"));
		_characterNameLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%CharacterNameLabel"));
		_ascensionLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%AscensionLabel"));
		_ascensionNumberLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%AscensionNumberLabel"));
		_readyIndicator = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ReadyIndicator"));
	}

	public void Fill(CharacterModel character, ulong playerId, int ascension, INetGameService netService)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		_ascensionLoc.Add("ascension", ascension);
		bool flag = netService.Type.IsMultiplayer();
		Control icon = character.Icon;
		foreach (Node child in ((Node)_characterIconContainer).GetChildren(false))
		{
			((Node)(object)_characterIconContainer).RemoveChildSafely(child);
		}
		((CanvasItem)_playerNameLabel).Visible = flag;
		((CanvasItem)_characterNameLabel).Modulate = (flag ? StsColors.cream : StsColors.gold);
		((Node)(object)_characterIconContainer).AddChildSafely((Node?)(object)icon);
		_characterNameLabel.SetTextAutoSize(character.Title.GetFormattedText());
		_playerNameLabel.SetTextAutoSize(PlatformUtil.GetPlayerName(netService.Platform, playerId));
		_ascensionLabel.SetTextAutoSize(_ascensionLoc.GetFormattedText());
		_ascensionNumberLabel.SetTextAutoSize(ascension.ToString());
	}

	public void SetIsReady(bool isReady)
	{
		((CanvasItem)_readyIndicator).Visible = isReady;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetIsReady, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isReady"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetIsReady && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetIsReady(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.SetIsReady)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._characterIconContainer)
		{
			_characterIconContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playerNameLabel)
		{
			_playerNameLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterNameLabel)
		{
			_characterNameLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ascensionLabel)
		{
			_ascensionLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ascensionNumberLabel)
		{
			_ascensionNumberLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._readyIndicator)
		{
			_readyIndicator = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._characterIconContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _characterIconContainer);
			return true;
		}
		if ((ref name) == PropertyName._playerNameLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _playerNameLabel);
			return true;
		}
		if ((ref name) == PropertyName._characterNameLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _characterNameLabel);
			return true;
		}
		if ((ref name) == PropertyName._ascensionLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _ascensionLabel);
			return true;
		}
		if ((ref name) == PropertyName._ascensionNumberLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _ascensionNumberLabel);
			return true;
		}
		if ((ref name) == PropertyName._readyIndicator)
		{
			value = VariantUtils.CreateFrom<Control>(ref _readyIndicator);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._characterIconContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._playerNameLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterNameLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionNumberLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._readyIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._characterIconContainer, Variant.From<Control>(ref _characterIconContainer));
		info.AddProperty(PropertyName._playerNameLabel, Variant.From<MegaLabel>(ref _playerNameLabel));
		info.AddProperty(PropertyName._characterNameLabel, Variant.From<MegaLabel>(ref _characterNameLabel));
		info.AddProperty(PropertyName._ascensionLabel, Variant.From<MegaLabel>(ref _ascensionLabel));
		info.AddProperty(PropertyName._ascensionNumberLabel, Variant.From<MegaLabel>(ref _ascensionNumberLabel));
		info.AddProperty(PropertyName._readyIndicator, Variant.From<Control>(ref _readyIndicator));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._characterIconContainer, ref val))
		{
			_characterIconContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._playerNameLabel, ref val2))
		{
			_playerNameLabel = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterNameLabel, ref val3))
		{
			_characterNameLabel = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionLabel, ref val4))
		{
			_ascensionLabel = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionNumberLabel, ref val5))
		{
			_ascensionNumberLabel = ((Variant)(ref val5)).As<MegaLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._readyIndicator, ref val6))
		{
			_readyIndicator = ((Variant)(ref val6)).As<Control>();
		}
	}
}
