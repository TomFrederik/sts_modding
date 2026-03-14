using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Quality;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NMultiplayerNetworkProblemIndicator.cs")]
public class NMultiplayerNetworkProblemIndicator : TextureRect
{
	public class MethodName : MethodName
	{
		public static readonly StringName Initialize = StringName.op_Implicit("Initialize");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName IsShown = StringName.op_Implicit("IsShown");

		public static readonly StringName _peerId = StringName.op_Implicit("_peerId");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private const float _qualityScoreToShowAt = 350f;

	private ulong _peerId;

	private Tween? _tween;

	public bool IsShown { get; private set; }

	public void Initialize(ulong peerId)
	{
		if (RunManager.Instance.NetService.Type.IsMultiplayer())
		{
			_peerId = peerId;
			TaskHelper.RunSafely(UpdateLoop());
		}
	}

	private async Task UpdateLoop()
	{
		while (RunManager.Instance.NetService.IsConnected && ((Node?)(object)this).IsValid())
		{
			ConnectionStats statsForPeer = RunManager.Instance.NetService.GetStatsForPeer(_peerId);
			if (statsForPeer == null)
			{
				break;
			}
			float num = statsForPeer.PingMsec / (1f - statsForPeer.PacketLoss);
			bool flag = num >= 350f;
			if (!IsShown && flag)
			{
				Tween? tween = _tween;
				if (tween != null)
				{
					tween.Kill();
				}
				((CanvasItem)this).Visible = true;
				((CanvasItem)this).Modulate = Colors.White;
			}
			else if (IsShown && flag)
			{
				NUiFlashVfx nUiFlashVfx = NUiFlashVfx.Create(((TextureRect)this).Texture, Colors.White);
				((Node)(object)this).AddChildSafely((Node?)(object)nUiFlashVfx);
				TaskHelper.RunSafely(nUiFlashVfx.StartVfx());
			}
			else if (IsShown && !flag)
			{
				Tween? tween2 = _tween;
				if (tween2 != null)
				{
					tween2.Kill();
				}
				_tween = ((Node)this).CreateTween();
				_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
			}
			IsShown = flag;
			await Task.Delay(2000);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName.Initialize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("peerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Initialize && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Initialize(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((TextureRect)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Initialize)
		{
			return true;
		}
		return ((TextureRect)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.IsShown)
		{
			IsShown = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._peerId)
		{
			_peerId = VariantUtils.ConvertTo<ulong>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._peerId)
		{
			value = VariantUtils.CreateFrom<ulong>(ref _peerId);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
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
		list.Add(new PropertyInfo((Type)2, PropertyName._peerId, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._peerId, Variant.From<ulong>(ref _peerId));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
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
		if (info.TryGetProperty(PropertyName._peerId, ref val2))
		{
			_peerId = ((Variant)(ref val2)).As<ulong>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val3))
		{
			_tween = ((Variant)(ref val3)).As<Tween>();
		}
	}
}
