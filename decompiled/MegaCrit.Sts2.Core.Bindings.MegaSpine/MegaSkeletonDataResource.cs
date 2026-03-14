using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaSkeletonDataResource : MegaSpineBinding
{
	protected override string SpineClassName => "SpineSkeletonDataResource";

	protected override IEnumerable<string> SpineMethods => new global::_003C_003Ez__ReadOnlyArray<string>(new string[4] { "find_animation", "find_skin", "get_animations", "get_skins" });

	public MegaSkeletonDataResource(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public MegaSkin? FindSkin(string skinName)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Variant native = Call("find_skin", Variant.op_Implicit(skinName));
		if (((Variant)(ref native)).AsGodotObject() == null)
		{
			return null;
		}
		return new MegaSkin(native);
	}

	public MegaAnimation? FindAnimation(string animName)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Variant native = Call("find_animation", Variant.op_Implicit(animName));
		if (((Variant)(ref native)).AsGodotObject() == null)
		{
			return null;
		}
		return new MegaAnimation(native);
	}

	public Array<GodotObject> GetAnimations()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (Array<GodotObject>)Call("get_animations");
	}

	public Array<GodotObject> GetSkins()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (Array<GodotObject>)Call("get_skins");
	}
}
