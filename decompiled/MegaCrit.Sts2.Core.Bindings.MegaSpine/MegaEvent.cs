using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaEvent : MegaSpineBinding
{
	protected override string SpineClassName => "SpineEvent";

	protected override IEnumerable<string> SpineMethods => new _003C_003Ez__ReadOnlySingleElementList<string>("get_data");

	public MegaEvent(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public MegaEventData GetData()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return new MegaEventData(Call("get_data"));
	}
}
