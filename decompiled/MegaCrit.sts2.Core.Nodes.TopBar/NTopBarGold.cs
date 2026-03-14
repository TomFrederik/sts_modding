using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.sts2.Core.Nodes.TopBar;

[ScriptPath("res://src/Core/Nodes/TopBar/NTopBarGold.cs")]
public class NTopBarGold : NClickableControl
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName UpdateGold = StringName.op_Implicit("UpdateGold");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName _goldLabel = StringName.op_Implicit("_goldLabel");

		public static readonly StringName _goldPopupLabel = StringName.op_Implicit("_goldPopupLabel");

		public static readonly StringName _currentGold = StringName.op_Implicit("_currentGold");

		public static readonly StringName _additionalGold = StringName.op_Implicit("_additionalGold");

		public static readonly StringName _alreadyRunning = StringName.op_Implicit("_alreadyRunning");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private static readonly HoverTip _hoverTip = new HoverTip(new LocString("static_hover_tips", "MONEY_POUCH.title"), new LocString("static_hover_tips", "MONEY_POUCH.description"));

	private Player? _player;

	private MegaLabel _goldLabel;

	private MegaLabel _goldPopupLabel;

	private int _currentGold;

	private int _additionalGold;

	private bool _alreadyRunning;

	public override void _Ready()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		_goldLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%GoldLabel"));
		_goldPopupLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%GoldPopup"));
		((CanvasItem)_goldPopupLabel).Modulate = Colors.Transparent;
		ConnectSignals();
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		if (_player != null)
		{
			_player.GoldChanged -= UpdateGold;
		}
	}

	public void Initialize(Player player)
	{
		_player = player;
		_currentGold = _player.Gold;
		_goldLabel.SetTextAutoSize($"{_currentGold}");
		_player.GoldChanged += UpdateGold;
	}

	private void UpdateGold()
	{
		TaskHelper.RunSafely(UpdateGoldAnim());
	}

	private async Task UpdateGoldAnim()
	{
		if (_player == null)
		{
			return;
		}
		int currentGold = _player.Gold - _currentGold;
		_additionalGold = (_currentGold = currentGold);
		_currentGold = _player.Gold;
		_goldPopupLabel.SetTextAutoSize(((_additionalGold > 0) ? "+" : "") + _additionalGold);
		if (_alreadyRunning)
		{
			return;
		}
		_alreadyRunning = true;
		Tween val = ((Node)this).CreateTween().SetParallel(true);
		val.TweenProperty((GodotObject)(object)_goldPopupLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.15000000596046448);
		val.TweenProperty((GodotObject)(object)_goldPopupLabel, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)_goldPopupLabel).Position.Y + 30f), 0.25);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
		await Task.Delay(150);
		while (_additionalGold != 0)
		{
			int num = 1;
			if (Mathf.Abs(_additionalGold) > 100)
			{
				num = 75;
			}
			else if (Mathf.Abs(_additionalGold) > 50)
			{
				num = 10;
			}
			_additionalGold = ((_additionalGold > 0) ? (_additionalGold - num) : (_additionalGold + num));
			_goldPopupLabel.SetTextAutoSize(((_additionalGold >= 0) ? "+" : "") + _additionalGold);
			_goldLabel.SetTextAutoSize($"{_player.Gold - _additionalGold}");
			await Task.Delay((int)Mathf.Lerp(10f, 20f, (float)Mathf.Max(0, 10 - Mathf.Abs(_additionalGold))));
		}
		await Task.Delay(250);
		Tween val2 = ((Node)this).CreateTween().SetParallel(true);
		val2.TweenProperty((GodotObject)(object)_goldPopupLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.10000000149011612);
		val2.TweenProperty((GodotObject)(object)_goldPopupLabel, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)_goldPopupLabel).Position.Y - 30f), 0.25).FromCurrent();
		_goldLabel.SetTextAutoSize($"{_player.Gold}");
		_alreadyRunning = false;
	}

	protected override void OnFocus()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _hoverTip);
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
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateGold, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateGold && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateGold();
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateGold)
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
		if ((ref name) == PropertyName._goldLabel)
		{
			_goldLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._goldPopupLabel)
		{
			_goldPopupLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentGold)
		{
			_currentGold = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._additionalGold)
		{
			_additionalGold = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._alreadyRunning)
		{
			_alreadyRunning = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._goldLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _goldLabel);
			return true;
		}
		if ((ref name) == PropertyName._goldPopupLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _goldPopupLabel);
			return true;
		}
		if ((ref name) == PropertyName._currentGold)
		{
			value = VariantUtils.CreateFrom<int>(ref _currentGold);
			return true;
		}
		if ((ref name) == PropertyName._additionalGold)
		{
			value = VariantUtils.CreateFrom<int>(ref _additionalGold);
			return true;
		}
		if ((ref name) == PropertyName._alreadyRunning)
		{
			value = VariantUtils.CreateFrom<bool>(ref _alreadyRunning);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._goldLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._goldPopupLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._currentGold, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._additionalGold, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._alreadyRunning, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._goldLabel, Variant.From<MegaLabel>(ref _goldLabel));
		info.AddProperty(PropertyName._goldPopupLabel, Variant.From<MegaLabel>(ref _goldPopupLabel));
		info.AddProperty(PropertyName._currentGold, Variant.From<int>(ref _currentGold));
		info.AddProperty(PropertyName._additionalGold, Variant.From<int>(ref _additionalGold));
		info.AddProperty(PropertyName._alreadyRunning, Variant.From<bool>(ref _alreadyRunning));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._goldLabel, ref val))
		{
			_goldLabel = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._goldPopupLabel, ref val2))
		{
			_goldPopupLabel = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentGold, ref val3))
		{
			_currentGold = ((Variant)(ref val3)).As<int>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._additionalGold, ref val4))
		{
			_additionalGold = ((Variant)(ref val4)).As<int>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._alreadyRunning, ref val5))
		{
			_alreadyRunning = ((Variant)(ref val5)).As<bool>();
		}
	}
}
