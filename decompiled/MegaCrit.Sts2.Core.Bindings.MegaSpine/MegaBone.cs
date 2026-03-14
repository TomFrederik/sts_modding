using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaBone : MegaSpineBinding
{
	protected override string SpineClassName => "SpineBone";

	protected override IEnumerable<string> SpineMethods => new global::_003C_003Ez__ReadOnlyArray<string>(new string[4] { "get_data", "set_rotation", "set_scale_x", "set_scale_y" });

	public MegaBone(Variant native)
		: base(native)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public MegaBoneData GetData()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return new MegaBoneData(Call("get_data"));
	}

	public void SetRotation(float rotation)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("set_rotation", Variant.op_Implicit(rotation));
	}

	public void SetScaleX(float scaleX)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("set_scale_x", Variant.op_Implicit(scaleX));
	}

	public void SetScaleY(float scaleY)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Call("set_scale_y", Variant.op_Implicit(scaleY));
	}

	public void Hide()
	{
		SetScaleX(0f);
		SetScaleY(0f);
	}
}
