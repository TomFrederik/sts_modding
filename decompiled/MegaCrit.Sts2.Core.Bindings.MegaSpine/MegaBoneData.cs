using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaBoneData : MegaSpineBinding
{
	protected override string SpineClassName => "SpineBoneData";

	protected override IEnumerable<string> SpineMethods => new _003C_003Ez__ReadOnlySingleElementList<string>("set_color");

	public MegaBoneData(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public void SetColor(Color color)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("set_color", Variant.op_Implicit(color));
	}
}
