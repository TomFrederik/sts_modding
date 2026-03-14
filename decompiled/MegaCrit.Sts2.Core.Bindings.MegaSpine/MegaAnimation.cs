using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaAnimation : MegaSpineBinding
{
	protected override string SpineClassName => "SpineAnimation";

	protected override IEnumerable<string> SpineMethods => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { "get_name", "get_duration" });

	public MegaAnimation(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public string GetName()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Variant val = Call("get_name");
		return ((Variant)(ref val)).AsString();
	}

	public float GetDuration()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Variant val = Call("get_duration");
		return ((Variant)(ref val)).AsSingle();
	}
}
