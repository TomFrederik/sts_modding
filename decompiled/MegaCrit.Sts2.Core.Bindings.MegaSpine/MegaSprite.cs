using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaSprite : MegaSpineBinding
{
	public const string spineClassName = "SpineSprite";

	protected override string SpineClassName => "SpineSprite";

	protected override IEnumerable<string> SpineMethods => new global::_003C_003Ez__ReadOnlyArray<string>(new string[7] { "get_animation_state", "get_additive_material", "get_normal_material", "get_skeleton", "new_skin", "set_normal_material", "set_skeleton_data_res" });

	protected override IEnumerable<string> SpineSignals => new global::_003C_003Ez__ReadOnlyArray<string>(new string[10] { "animation_started", "animation_interrupted", "animation_ended", "animation_completed", "animation_disposed", "animation_event", "before_animation_state_update", "before_animation_state_apply", "before_world_transforms_change", "world_transforms_changed" });

	public MegaSprite(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public Error ConnectAnimationStarted(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Connect("animation_started", callable);
	}

	public Error ConnectAnimationInterrupted(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Connect("animation_interrupted", callable);
	}

	public Error ConnectAnimationEnded(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Connect("animation_ended", callable);
	}

	public Error ConnectAnimationCompleted(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Connect("animation_completed", callable);
	}

	public Error ConnectAnimationDisposed(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Connect("animation_disposed", callable);
	}

	public Error ConnectAnimationEvent(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Connect("animation_event", callable);
	}

	public Error ConnectBeforeAnimationStateUpdate(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Connect("before_animation_state_update", callable);
	}

	public Error ConnectBeforeAnimationStateApply(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Connect("before_animation_state_apply", callable);
	}

	public Error ConnectBeforeWorldTransformsChange(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Connect("before_world_transforms_change", callable);
	}

	public Error ConnectWorldTransformsChanged(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Connect("world_transforms_changed", callable);
	}

	public void DisconnectAnimationStarted(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Disconnect("animation_started", callable);
	}

	public void DisconnectAnimationInterrupted(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Disconnect("animation_interrupted", callable);
	}

	public void DisconnectAnimationEnded(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Disconnect("animation_ended", callable);
	}

	public void DisconnectAnimationCompleted(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Disconnect("animation_completed", callable);
	}

	public void DisconnectAnimationDisposed(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Disconnect("animation_disposed", callable);
	}

	public void DisconnectAnimationEvent(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Disconnect("animation_event", callable);
	}

	public void DisconnectBeforeAnimationStateUpdate(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Disconnect("before_animation_state_update", callable);
	}

	public void DisconnectBeforeAnimationStateApply(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Disconnect("before_animation_state_apply", callable);
	}

	public void DisconnectBeforeWorldTransformsChange(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Disconnect("before_world_transforms_change", callable);
	}

	public void DisconnectWorldTransformsChanged(Callable callable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Disconnect("world_transforms_changed", callable);
	}

	public bool HasAnimation(string animId)
	{
		return GetSkeleton().GetData().FindAnimation(animId) != null;
	}

	public MegaAnimationState GetAnimationState()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return new MegaAnimationState(Call("get_animation_state"));
	}

	public MegaSkeleton GetSkeleton()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return new MegaSkeleton(Call("get_skeleton"));
	}

	public Material? GetAdditiveMaterial()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Variant? val = CallNullable("get_additive_material");
		if (!val.HasValue)
		{
			return null;
		}
		Variant valueOrDefault = val.GetValueOrDefault();
		return ((Variant)(ref valueOrDefault)).As<Material>();
	}

	public Material? GetNormalMaterial()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Variant? val = CallNullable("get_normal_material");
		if (!val.HasValue)
		{
			return null;
		}
		Variant valueOrDefault = val.GetValueOrDefault();
		return ((Variant)(ref valueOrDefault)).As<Material>();
	}

	public MegaSkin NewSkin(string name)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return new MegaSkin(Call("new_skin", Variant.op_Implicit(name)));
	}

	public void SetNormalMaterial(Material material)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("set_normal_material", Variant.op_Implicit((GodotObject)(object)material));
	}

	public void SetSkeletonDataRes(MegaSkeletonDataResource skeletonData)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Call("set_skeleton_data_res", Variant.op_Implicit(skeletonData.BoundObject));
	}
}
