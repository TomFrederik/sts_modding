using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Rewards;

namespace MegaCrit.Sts2.Core.Nodes.Rewards;

[ScriptPath("res://src/Core/Nodes/Rewards/NLinkedRewardSet.cs")]
public class NLinkedRewardSet : Control
{
	[Signal]
	public delegate void RewardClaimedEventHandler(NLinkedRewardSet linkedRewardSet);

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Reload = StringName.op_Implicit("Reload");

		public static readonly StringName GetReward = StringName.op_Implicit("GetReward");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _rewardsScreen = StringName.op_Implicit("_rewardsScreen");

		public static readonly StringName _rewardContainer = StringName.op_Implicit("_rewardContainer");

		public static readonly StringName _chainsContainer = StringName.op_Implicit("_chainsContainer");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName RewardClaimed = StringName.op_Implicit("RewardClaimed");
	}

	private NRewardsScreen _rewardsScreen;

	private Control _rewardContainer;

	private Control _chainsContainer;

	private RewardClaimedEventHandler backing_RewardClaimed;

	public LinkedRewardSet LinkedRewardSet { get; private set; }

	private static string ScenePath => SceneHelper.GetScenePath("/rewards/linked_reward_set");

	private static string ChainImagePath => ImageHelper.GetImagePath("/ui/reward_screen/reward_chain.png");

	public static IEnumerable<string> AssetPaths => new string[2] { ScenePath, ChainImagePath };

	public event RewardClaimedEventHandler RewardClaimed
	{
		add
		{
			backing_RewardClaimed = (RewardClaimedEventHandler)Delegate.Combine(backing_RewardClaimed, value);
		}
		remove
		{
			backing_RewardClaimed = (RewardClaimedEventHandler)Delegate.Remove(backing_RewardClaimed, value);
		}
	}

	public override void _Ready()
	{
		_rewardContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%RewardContainer"));
		_chainsContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ChainContainer"));
		Reload();
	}

	public static NLinkedRewardSet Create(LinkedRewardSet linkedReward, NRewardsScreen screen)
	{
		NLinkedRewardSet nLinkedRewardSet = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NLinkedRewardSet>((GenEditState)0);
		nLinkedRewardSet._rewardsScreen = screen;
		nLinkedRewardSet.SetReward(linkedReward);
		return nLinkedRewardSet;
	}

	private void SetReward(LinkedRewardSet linkedReward)
	{
		LinkedRewardSet = linkedReward;
		if (((Node)this).IsNodeReady())
		{
			Reload();
		}
	}

	private void Reload()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if (!((Node)this).IsNodeReady())
		{
			return;
		}
		for (int i = 0; i < LinkedRewardSet.Rewards.Count; i++)
		{
			Reward reward = LinkedRewardSet.Rewards[i];
			NRewardButton nRewardButton = NRewardButton.Create(reward, _rewardsScreen);
			((Control)nRewardButton).CustomMinimumSize = ((Control)nRewardButton).CustomMinimumSize - Vector2.Right * 20f;
			((Node)(object)_rewardContainer).AddChildSafely((Node?)(object)nRewardButton);
			((GodotObject)nRewardButton).Connect(NRewardButton.SignalName.RewardClaimed, Callable.From((Action)GetReward), 0u);
			if (i < LinkedRewardSet.Rewards.Count - 1)
			{
				TextureRect val = new TextureRect();
				((Control)val).MouseFilter = (MouseFilterEnum)2;
				val.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D(ChainImagePath);
				((Control)val).Size = Vector2.One * 50f;
				((Node)(object)_chainsContainer).AddChildSafely((Node?)(object)val);
				((Control)val).GlobalPosition = _chainsContainer.GlobalPosition + Vector2.Down * (float)i * (3f + ((Control)nRewardButton).CustomMinimumSize.Y);
			}
		}
	}

	private void GetReward()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		_rewardsScreen.RewardCollectedFrom((Control)(object)this);
		LinkedRewardSet.OnSkipped();
		((GodotObject)this).EmitSignal(SignalName.RewardClaimed, Array.Empty<Variant>());
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Reload, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetReward, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Reload && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Reload();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetReward && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			GetReward();
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
		if ((ref method) == MethodName.Reload)
		{
			return true;
		}
		if ((ref method) == MethodName.GetReward)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._rewardsScreen)
		{
			_rewardsScreen = VariantUtils.ConvertTo<NRewardsScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rewardContainer)
		{
			_rewardContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._chainsContainer)
		{
			_chainsContainer = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName._rewardsScreen)
		{
			value = VariantUtils.CreateFrom<NRewardsScreen>(ref _rewardsScreen);
			return true;
		}
		if ((ref name) == PropertyName._rewardContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _rewardContainer);
			return true;
		}
		if ((ref name) == PropertyName._chainsContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _chainsContainer);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._rewardsScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rewardContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._chainsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._rewardsScreen, Variant.From<NRewardsScreen>(ref _rewardsScreen));
		info.AddProperty(PropertyName._rewardContainer, Variant.From<Control>(ref _rewardContainer));
		info.AddProperty(PropertyName._chainsContainer, Variant.From<Control>(ref _chainsContainer));
		info.AddSignalEventDelegate(SignalName.RewardClaimed, (Delegate)backing_RewardClaimed);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._rewardsScreen, ref val))
		{
			_rewardsScreen = ((Variant)(ref val)).As<NRewardsScreen>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._rewardContainer, ref val2))
		{
			_rewardContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._chainsContainer, ref val3))
		{
			_chainsContainer = ((Variant)(ref val3)).As<Control>();
		}
		RewardClaimedEventHandler rewardClaimedEventHandler = default(RewardClaimedEventHandler);
		if (info.TryGetSignalEventDelegate<RewardClaimedEventHandler>(SignalName.RewardClaimed, ref rewardClaimedEventHandler))
		{
			backing_RewardClaimed = rewardClaimedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.RewardClaimed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("linkedRewardSet"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalRewardClaimed(NLinkedRewardSet linkedRewardSet)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.RewardClaimed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)linkedRewardSet) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.RewardClaimed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_RewardClaimed?.Invoke(VariantUtils.ConvertTo<NLinkedRewardSet>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.RewardClaimed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
