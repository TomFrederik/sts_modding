using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.PauseMenu;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.TopBar;

[ScriptPath("res://src/Core/Nodes/TopBar/NTopBarPauseButton.cs")]
public class NTopBarPauseButton : NTopBarButton
{
	public new class MethodName : NTopBarButton.MethodName
	{
		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName IsOpen = StringName.op_Implicit("IsOpen");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName ToggleAnimState = StringName.op_Implicit("ToggleAnimState");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public new class PropertyName : NTopBarButton.PropertyName
	{
		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");
	}

	public new class SignalName : NTopBarButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly HoverTip _hoverTip = new HoverTip(new LocString("static_hover_tips", "SETTINGS.title"), new LocString("static_hover_tips", "SETTINGS.description"));

	private const float _hoverAngle = -(float)Math.PI;

	private const float _hoverShaderV = 1.1f;

	private const float _defaultV = 0.9f;

	private const float _pressDownV = 0.4f;

	private IRunState _runState;

	protected override string[] Hotkeys => new string[1] { StringName.op_Implicit(MegaInput.pauseAndBack) };

	protected override void OnRelease()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		base.OnRelease();
		if (IsOpen())
		{
			NCapstoneContainer.Instance.Close();
		}
		else
		{
			NPauseMenu nPauseMenu = (NPauseMenu)NRun.Instance.GlobalUi.SubmenuStack.ShowScreen(CapstoneSubmenuType.PauseMenu);
			nPauseMenu.Initialize(_runState);
		}
		UpdateScreenOpen();
		ShaderMaterial? hsv = _hsv;
		if (hsv != null)
		{
			hsv.SetShaderParameter(_v, Variant.op_Implicit(0.9f));
		}
	}

	protected override bool IsOpen()
	{
		if (NCapstoneContainer.Instance.CurrentCapstoneScreen is NCapstoneSubmenuStack nCapstoneSubmenuStack)
		{
			return nCapstoneSubmenuStack.ScreenType == NetScreenType.PauseMenu;
		}
		return false;
	}

	public override void _Process(double delta)
	{
		if (base.IsScreenOpen)
		{
			Control icon = _icon;
			icon.Rotation += (float)delta;
		}
	}

	public void Initialize(IRunState runState)
	{
		_runState = runState;
	}

	protected override async Task AnimPressDown(CancellationTokenSource cancelToken)
	{
		float timer = 0f;
		float startAngle = _icon.Rotation;
		float targetAngle = startAngle + (float)Math.PI / 4f;
		for (; timer < 0.25f; timer += (float)((Node)this).GetProcessDeltaTime())
		{
			if (cancelToken.IsCancellationRequested)
			{
				return;
			}
			_icon.Rotation = Mathf.LerpAngle(startAngle, targetAngle, Ease.CubicOut(timer / 0.25f));
			ShaderMaterial? hsv = _hsv;
			if (hsv != null)
			{
				hsv.SetShaderParameter(_v, Variant.op_Implicit(Mathf.Lerp(1.1f, 0.4f, Ease.CubicOut(timer / 0.25f))));
			}
			if (!((Node?)(object)this).IsValid() || !((Node)this).IsInsideTree())
			{
				return;
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		_icon.Rotation = targetAngle;
		ShaderMaterial? hsv2 = _hsv;
		if (hsv2 != null)
		{
			hsv2.SetShaderParameter(_v, Variant.op_Implicit(0.4f));
		}
	}

	protected override async Task AnimHover(CancellationTokenSource cancelToken)
	{
		float timer = 0f;
		float startAngle = _icon.Rotation;
		for (; timer < 0.5f; timer += (float)((Node)this).GetProcessDeltaTime())
		{
			if (cancelToken.IsCancellationRequested)
			{
				return;
			}
			_icon.Rotation = Mathf.LerpAngle(startAngle, -(float)Math.PI, Ease.BackOut(timer / 0.5f));
			if (!((Node?)(object)this).IsValid() || !((Node)this).IsInsideTree())
			{
				return;
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		_icon.Rotation = -(float)Math.PI;
	}

	protected override async Task AnimUnhover(CancellationTokenSource cancelToken)
	{
		float timer = 0f;
		float startAngle = _icon.Rotation;
		for (; timer < 1f; timer += (float)((Node)this).GetProcessDeltaTime())
		{
			if (cancelToken.IsCancellationRequested)
			{
				return;
			}
			_icon.Rotation = Mathf.LerpAngle(startAngle, 0f, Ease.ElasticOut(timer / 1f));
			ShaderMaterial? hsv = _hsv;
			if (hsv != null)
			{
				hsv.SetShaderParameter(_v, Variant.op_Implicit(Mathf.Lerp(1.1f, 0.9f, Ease.ExpoOut(timer / 1f))));
			}
			_icon.Scale = ((Vector2)(ref NTopBarButton._hoverScale)).Lerp(Vector2.One, Ease.ExpoOut(timer / 1f));
			if (!((Node?)(object)this).IsValid() || !((Node)this).IsInsideTree())
			{
				return;
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		ShaderMaterial? hsv2 = _hsv;
		if (hsv2 != null)
		{
			hsv2.SetShaderParameter(_v, Variant.op_Implicit(0.9f));
		}
		_icon.Rotation = 0f;
		_icon.Scale = Vector2.One;
	}

	public void ToggleAnimState()
	{
		UpdateScreenOpen();
	}

	protected override void OnFocus()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _hoverTip);
		((Control)nHoverTipSet).GlobalPosition = ((Control)this).GlobalPosition + new Vector2(((Control)this).Size.X - ((Control)nHoverTipSet).Size.X, ((Control)this).Size.Y + 20f);
	}

	protected override void OnUnfocus()
	{
		base.OnUnfocus();
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
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsOpen, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleAnimState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsOpen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = IsOpen();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleAnimState && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ToggleAnimState();
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
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.IsOpen)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleAnimState)
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
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Hotkeys)
		{
			string[] hotkeys = Hotkeys;
			value = VariantUtils.CreateFrom<string[]>(ref hotkeys);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
	}
}
