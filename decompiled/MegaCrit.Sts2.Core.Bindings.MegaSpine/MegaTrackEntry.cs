using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaTrackEntry : MegaSpineBinding
{
	protected override string SpineClassName => "SpineTrackEntry";

	protected override IEnumerable<string> SpineMethods => new global::_003C_003Ez__ReadOnlyArray<string>(new string[9] { "get_animation", "get_animation_end", "get_track_complete", "get_track_time", "is_complete", "set_loop", "set_time_scale", "set_track_time", "set_mix_duration" });

	public MegaTrackEntry(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public MegaAnimation GetAnimation()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return new MegaAnimation(Call("get_animation"));
	}

	public float GetAnimationEnd()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Variant val = Call("get_animation_end");
		return ((Variant)(ref val)).AsSingle();
	}

	public float GetTrackComplete()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Variant val = Call("get_track_complete");
		return ((Variant)(ref val)).AsSingle();
	}

	public float GetTrackTime()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Variant val = Call("get_track_time");
		return ((Variant)(ref val)).AsSingle();
	}

	public bool IsComplete()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Variant val = Call("is_complete");
		return ((Variant)(ref val)).AsBool();
	}

	public void SetLoop(bool loop)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("set_loop", Variant.op_Implicit(loop));
	}

	public void SetTimeScale(float scale)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("set_time_scale", Variant.op_Implicit(scale));
	}

	public void SetTrackTime(float time)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("set_track_time", Variant.op_Implicit(time));
	}

	public void SetMixDuration(float time)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("set_mix_duration", Variant.op_Implicit(time));
	}
}
