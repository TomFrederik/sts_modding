using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Quality;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NMultiplayerTimeoutOverlay.cs")]
public class NMultiplayerTimeoutOverlay : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Relocalize = StringName.op_Implicit("Relocalize");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName IsShown = StringName.op_Implicit("IsShown");

		public static readonly StringName _gameLevel = StringName.op_Implicit("_gameLevel");

		public static readonly StringName _icon = StringName.op_Implicit("_icon");
	}

	public class SignalName : SignalName
	{
	}

	private const int _noResponseMsec = 3000;

	private const int _loadingNoResponseMsec = 8000;

	private bool _gameLevel;

	private TextureRect _icon;

	private NetClientGameService? _netService;

	public bool IsShown { get; private set; }

	public override void _Ready()
	{
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		Relocalize();
	}

	public void Relocalize()
	{
		MegaLabel node = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Title"));
		MegaRichTextLabel node2 = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Description"));
		node.SetTextAutoSize(new LocString("main_menu_ui", "TIMEOUT_OVERLAY.title").GetFormattedText());
		node2.SetTextAutoSize(new LocString("main_menu_ui", "TIMEOUT_OVERLAY.description").GetFormattedText());
		node.RefreshFont();
		node2.RefreshFont();
	}

	public void Initialize(INetGameService netService, bool isGameLevel)
	{
		if (netService is NetClientGameService netService2)
		{
			_netService = netService2;
			_gameLevel = isGameLevel;
			TaskHelper.RunSafely(UpdateLoop());
		}
	}

	private async Task UpdateLoop()
	{
		while ((_netService?.IsConnected ?? false) && ((Node?)(object)this).IsValid())
		{
			ConnectionStats statsForPeer = _netService.GetStatsForPeer(_netService.HostNetId);
			if (statsForPeer == null)
			{
				return;
			}
			int num = (int)(statsForPeer.LastReceivedTime.HasValue ? (Time.GetTicksMsec() - statsForPeer.LastReceivedTime.Value) : 0);
			bool flag = _gameLevel == (_netService.IsGameLoading || !RunManager.Instance.IsInProgress);
			int num2 = (statsForPeer.RemoteIsLoading ? 8000 : 3000);
			bool flag2 = flag && num >= num2;
			if (!IsShown && flag2)
			{
				((CanvasItem)this).Visible = true;
			}
			else if (IsShown && !flag2)
			{
				((CanvasItem)this).Visible = false;
			}
			IsShown = flag2;
			await Task.Delay(200);
		}
		if (((Node?)(object)this).IsValid())
		{
			((CanvasItem)this).Visible = false;
		}
		_netService = null;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Relocalize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Relocalize && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Relocalize();
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
		if ((ref method) == MethodName.Relocalize)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.IsShown)
		{
			IsShown = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._gameLevel)
		{
			_gameLevel = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsShown)
		{
			bool isShown = IsShown;
			value = VariantUtils.CreateFrom<bool>(ref isShown);
			return true;
		}
		if ((ref name) == PropertyName._gameLevel)
		{
			value = VariantUtils.CreateFrom<bool>(ref _gameLevel);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._gameLevel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsShown, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName isShown = PropertyName.IsShown;
		bool isShown2 = IsShown;
		info.AddProperty(isShown, Variant.From<bool>(ref isShown2));
		info.AddProperty(PropertyName._gameLevel, Variant.From<bool>(ref _gameLevel));
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsShown, ref val))
		{
			IsShown = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._gameLevel, ref val2))
		{
			_gameLevel = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val3))
		{
			_icon = ((Variant)(ref val3)).As<TextureRect>();
		}
	}
}
