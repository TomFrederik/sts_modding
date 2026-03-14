using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaSkin : MegaSpineBinding
{
	protected override string SpineClassName => "SpineSkin";

	protected override IEnumerable<string> SpineMethods => new _003C_003Ez__ReadOnlySingleElementList<string>("add_skin");

	public MegaSkin(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public void AddSkin(MegaSkin? skin)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (skin != null)
		{
			Call("add_skin", Variant.op_Implicit(skin.BoundObject));
		}
	}
}
