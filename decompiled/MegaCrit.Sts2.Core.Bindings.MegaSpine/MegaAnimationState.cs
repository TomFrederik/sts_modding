using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaAnimationState : MegaSpineBinding
{
	protected override string SpineClassName => "SpineAnimationState";

	protected override IEnumerable<string> SpineMethods => new global::_003C_003Ez__ReadOnlyArray<string>(new string[6] { "add_animation", "apply", "get_current", "set_animation", "set_time_scale", "update" });

	public MegaAnimationState(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public MegaTrackEntry AddAnimation(string animationName, float delay = 0f, bool loop = true, int trackId = 0)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		return new MegaTrackEntry(Call("add_animation", Variant.op_Implicit(animationName), Variant.op_Implicit(delay), Variant.op_Implicit(loop), Variant.op_Implicit(trackId)));
	}

	public void Apply(MegaSkeleton skeleton)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Call("apply", Variant.op_Implicit(skeleton.BoundObject));
	}

	public MegaTrackEntry GetCurrent(int trackIndex)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return new MegaTrackEntry(Call("get_current", Variant.op_Implicit(trackIndex)));
	}

	public MegaTrackEntry? SetAnimation(string animationName, bool loop = true, int trackId = 0)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Variant native = Call("set_animation", Variant.op_Implicit(animationName), Variant.op_Implicit(loop), Variant.op_Implicit(trackId));
		if (((Variant)(ref native)).AsGodotObject() == null)
		{
			return null;
		}
		return new MegaTrackEntry(native);
	}

	public void SetTimeScale(float scale)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("set_time_scale", Variant.op_Implicit(scale));
	}

	public void Update(float delta)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("update", Variant.op_Implicit(delta));
	}
}
