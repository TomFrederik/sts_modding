using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaSlotNode : MegaSpineBinding
{
	protected override string SpineClassName => "SpineSlotNode";

	protected override IEnumerable<string> SpineMethods => new _003C_003Ez__ReadOnlySingleElementList<string>("get_normal_material");

	public MegaSlotNode(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


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
}
