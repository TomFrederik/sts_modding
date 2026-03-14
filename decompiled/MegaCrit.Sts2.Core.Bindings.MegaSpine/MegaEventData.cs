using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaEventData : MegaSpineBinding
{
	protected override string SpineClassName => "SpineEventData";

	protected override IEnumerable<string> SpineMethods => new _003C_003Ez__ReadOnlySingleElementList<string>("get_event_name");

	public MegaEventData(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public string GetEventName()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Variant val = Call("get_event_name");
		return ((Variant)(ref val)).AsString();
	}
}
