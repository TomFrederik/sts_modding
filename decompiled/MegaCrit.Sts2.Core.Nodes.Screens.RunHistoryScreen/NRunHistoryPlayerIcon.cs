using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;

[ScriptPath("res://src/Core/Nodes/Screens/RunHistoryScreen/NRunHistoryPlayerIcon.cs")]
public class NRunHistoryPlayerIcon : NClickableControl
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Select = StringName.op_Implicit("Select");

		public static readonly StringName Deselect = StringName.op_Implicit("Deselect");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName _ascensionIcon = StringName.op_Implicit("_ascensionIcon");

		public static readonly StringName _ascensionLabel = StringName.op_Implicit("_ascensionLabel");

		public static readonly StringName _selectionReticle = StringName.op_Implicit("_selectionReticle");

		public static readonly StringName _currentIcon = StringName.op_Implicit("_currentIcon");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("screens/run_history_screen/run_history_player_icon");

	private readonly List<IHoverTip> _hoverTips = new List<IHoverTip>();

	private Control _ascensionIcon;

	private MegaLabel _ascensionLabel;

	private NSelectionReticle _selectionReticle;

	private Control? _currentIcon;

	public RunHistoryPlayer Player { get; private set; }

	public override void _Ready()
	{
		_ascensionIcon = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%AscensionIcon"));
		_ascensionLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%AscensionLabel"));
		_selectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("%SelectionReticle"));
		ConnectSignals();
	}

	public void LoadRun(RunHistoryPlayer player, RunHistory history)
	{
		Player = player;
		CharacterModel byId = ModelDb.GetById<CharacterModel>(player.Character);
		((Node)(object)_currentIcon)?.QueueFreeSafely();
		_currentIcon = byId.Icon;
		((Node)(object)this).AddChildSafely((Node?)(object)_currentIcon);
		((Node)this).MoveChild((Node)(object)_currentIcon, 0);
		LocString locString = new LocString("ascension", "PORTRAIT_TITLE");
		locString.Add("character", byId.Title);
		locString.Add("ascension", history.Ascension);
		LocString locString2 = new LocString("ascension", "PORTRAIT_DESCRIPTION");
		List<string> list = new List<string>();
		for (int i = 1; i <= history.Ascension; i++)
		{
			list.Add(AscensionHelper.GetTitle(i).GetFormattedText());
		}
		locString2.Add("ascensions", list);
		((CanvasItem)_selectionReticle).Visible = history.Players.Count > 1;
		((CanvasItem)_ascensionIcon).Visible = false;
		_ascensionLabel.SetTextAutoSize((history.Ascension > 0) ? history.Ascension.ToString() : string.Empty);
		LocString locString3 = new LocString("run_history", "PLAYER_HOVER");
		if (history.Players.Count > 1)
		{
			locString3.Add("PlayerName", PlatformUtil.GetPlayerName(history.PlatformType, player.Id));
			locString3.Add("CharacterName", byId.Title.GetFormattedText());
		}
		else
		{
			locString3.Add("PlayerName", byId.Title.GetFormattedText());
			locString3.Add("CharacterName", string.Empty);
		}
		_hoverTips.Add(new HoverTip(locString3));
		if (history.Ascension > 0)
		{
			_hoverTips.Add(AscensionHelper.GetHoverTip(byId, history.Ascension));
		}
	}

	public void Select()
	{
		((CanvasItem)_ascensionIcon).Visible = ((Label)_ascensionLabel).Text != string.Empty;
		_selectionReticle.OnSelect();
	}

	public void Deselect()
	{
		((CanvasItem)_ascensionIcon).Visible = false;
		_selectionReticle.OnDeselect();
	}

	protected override void OnFocus()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _hoverTips);
		((Control)nHoverTipSet).GlobalPosition = ((Control)this).GlobalPosition + new Vector2(0f, ((Control)this).Size.Y + 20f);
	}

	protected override void OnUnfocus()
	{
		NHoverTipSet.Remove((Control)(object)this);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Select, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Deselect, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Select && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Select();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Deselect && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Deselect();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.Select)
		{
			return true;
		}
		if ((ref method) == MethodName.Deselect)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._ascensionIcon)
		{
			_ascensionIcon = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ascensionLabel)
		{
			_ascensionLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentIcon)
		{
			_currentIcon = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		if ((ref name) == PropertyName._ascensionIcon)
		{
			value = VariantUtils.CreateFrom<Control>(ref _ascensionIcon);
			return true;
		}
		if ((ref name) == PropertyName._ascensionLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _ascensionLabel);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _selectionReticle);
			return true;
		}
		if ((ref name) == PropertyName._currentIcon)
		{
			value = VariantUtils.CreateFrom<Control>(ref _currentIcon);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._ascensionIcon, Variant.From<Control>(ref _ascensionIcon));
		info.AddProperty(PropertyName._ascensionLabel, Variant.From<MegaLabel>(ref _ascensionLabel));
		info.AddProperty(PropertyName._selectionReticle, Variant.From<NSelectionReticle>(ref _selectionReticle));
		info.AddProperty(PropertyName._currentIcon, Variant.From<Control>(ref _currentIcon));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionIcon, ref val))
		{
			_ascensionIcon = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionLabel, ref val2))
		{
			_ascensionLabel = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionReticle, ref val3))
		{
			_selectionReticle = ((Variant)(ref val3)).As<NSelectionReticle>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentIcon, ref val4))
		{
			_currentIcon = ((Variant)(ref val4)).As<Control>();
		}
	}
}
