using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaSkeleton : MegaSpineBinding
{
	protected override string SpineClassName => "SpineSkeleton";

	protected override IEnumerable<string> SpineMethods => new global::_003C_003Ez__ReadOnlyArray<string>(new string[6] { "find_bone", "get_bounds", "get_data", "set_skin", "set_skin_by_name", "set_slots_to_setup_pose" });

	public MegaSkeleton(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public MegaBone? FindBone(string boneName)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Variant native = Call("find_bone", Variant.op_Implicit(boneName));
		if (((Variant)(ref native)).AsGodotObject() == null)
		{
			return null;
		}
		return new MegaBone(native);
	}

	public Rect2 GetBounds()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Variant val = Call("get_bounds");
		return ((Variant)(ref val)).As<Rect2>();
	}

	public MegaSkeletonDataResource GetData()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return new MegaSkeletonDataResource(Call("get_data"));
	}

	public void SetSkin(MegaSkin? skin)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (skin != null)
		{
			Call("set_skin", Variant.op_Implicit(skin.BoundObject));
		}
	}

	public void SetSkinByName(string skinName)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("set_skin_by_name", Variant.op_Implicit(skinName));
	}

	public void SetSlotsToSetupPose()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Call("set_slots_to_setup_pose");
	}
}
