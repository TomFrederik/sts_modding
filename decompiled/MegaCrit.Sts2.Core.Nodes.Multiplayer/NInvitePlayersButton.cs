using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NInvitePlayersButton.cs")]
public class NInvitePlayersButton : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName UpdateVisibility = StringName.op_Implicit("UpdateVisibility");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName _shaderMaterial = StringName.op_Implicit("_shaderMaterial");

		public static readonly StringName _container = StringName.op_Implicit("_container");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private ShaderMaterial _shaderMaterial;

	private Control _container;

	private StartRunLobby? _startRunLobby;

	public override void _Ready()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		ConnectSignals();
		_container = ((Node)this).GetParent<Control>();
		Control node = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Background"));
		MegaRichTextLabel node2 = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("Label"));
		_shaderMaterial = (ShaderMaterial)((CanvasItem)node).Material;
		node2.SetTextAutoSize(new LocString("main_menu_ui", "INVITE").GetFormattedText());
		UpdateVisibility();
	}

	public void Initialize(StartRunLobby lobby)
	{
		_startRunLobby = lobby;
		_startRunLobby.PlayerConnected += OnPlayerConnected;
		_startRunLobby.PlayerDisconnected += OnPlayerDisconnected;
		UpdateVisibility();
	}

	public override void _ExitTree()
	{
		if (_startRunLobby != null)
		{
			_startRunLobby.PlayerConnected -= OnPlayerConnected;
			_startRunLobby.PlayerDisconnected -= OnPlayerDisconnected;
		}
	}

	private void OnPlayerConnected(LobbyPlayer player)
	{
		UpdateVisibility();
	}

	private void OnPlayerDisconnected(LobbyPlayer player)
	{
		UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		((CanvasItem)_container).Visible = _startRunLobby != null && PlatformUtil.SupportsInviteDialog(_startRunLobby.NetService.Platform) && _startRunLobby.Players.Count < _startRunLobby.MaxPlayers;
	}

	protected override void OnRelease()
	{
		if (_startRunLobby != null)
		{
			PlatformUtil.OpenInviteDialog(_startRunLobby.NetService);
		}
	}

	protected override void OnFocus()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		_shaderMaterial.SetShaderParameter(_s, Variant.op_Implicit(1.1f));
		_shaderMaterial.SetShaderParameter(_v, Variant.op_Implicit(1.1f));
	}

	protected override void OnUnfocus()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		_shaderMaterial.SetShaderParameter(_s, Variant.op_Implicit(1f));
		_shaderMaterial.SetShaderParameter(_v, Variant.op_Implicit(1f));
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
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.UpdateVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
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
		if ((ref method) == MethodName.UpdateVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
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
		if ((ref name) == PropertyName._shaderMaterial)
		{
			_shaderMaterial = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._container)
		{
			_container = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName._shaderMaterial)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _shaderMaterial);
			return true;
		}
		if ((ref name) == PropertyName._container)
		{
			value = VariantUtils.CreateFrom<Control>(ref _container);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._shaderMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._container, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._shaderMaterial, Variant.From<ShaderMaterial>(ref _shaderMaterial));
		info.AddProperty(PropertyName._container, Variant.From<Control>(ref _container));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._shaderMaterial, ref val))
		{
			_shaderMaterial = ((Variant)(ref val)).As<ShaderMaterial>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._container, ref val2))
		{
			_container = ((Variant)(ref val2)).As<Control>();
		}
	}
}
