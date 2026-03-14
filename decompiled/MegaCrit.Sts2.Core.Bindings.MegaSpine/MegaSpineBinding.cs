using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public abstract class MegaSpineBinding
{
	public GodotObject BoundObject { get; private set; }

	protected abstract string SpineClassName { get; }

	protected virtual IEnumerable<string> SpineMethods => Array.Empty<string>();

	protected virtual IEnumerable<string> SpineSignals => Array.Empty<string>();

	protected MegaSpineBinding(Variant native)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I8
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if ((long)((Variant)(ref native)).VariantType != 24)
		{
			throw new InvalidOperationException($"Expected a GodotObject but was {((Variant)(ref native)).VariantType}!");
		}
		BoundObject = ((Variant)(ref native)).AsGodotObject();
		ValidateBoundObject();
	}

	protected Error Connect(string signalName, Callable callable)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return BoundObject.Connect(StringName.op_Implicit(signalName), callable, 0u);
	}

	protected void Disconnect(string signalName, Callable callable)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		BoundObject.Disconnect(StringName.op_Implicit(signalName), callable);
	}

	protected Variant Call(string methodName, params Variant[] args)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (!SpineMethods.Contains(methodName))
		{
			throw new InvalidOperationException($"You must add {methodName} to {GetType().Name}.SpineMethods before calling it!");
		}
		return BoundObject.Call(StringName.op_Implicit(methodName), args);
	}

	protected Variant? CallNullable(string methodName, params Variant[] args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Variant val = Call(methodName, args);
		return (Variant)(((int)((Variant)(ref val)).VariantType == 0) ? default(Variant) : val);
	}

	private void ValidateBoundObject()
	{
		if (BoundObject == null)
		{
			return;
		}
		if (BoundObject.GetClass() != SpineClassName)
		{
			throw new InvalidOperationException($"Expected {"BoundObject"} to be a {SpineClassName}, but it is a {BoundObject.GetClass()}!");
		}
		foreach (string spineMethod in SpineMethods)
		{
			if (!BoundObject.HasMethod(StringName.op_Implicit(spineMethod)))
			{
				throw new InvalidOperationException(SpineClassName + " does not have method " + spineMethod + "!");
			}
		}
		foreach (string spineSignal in SpineSignals)
		{
			if (!BoundObject.HasSignal(StringName.op_Implicit(spineSignal)))
			{
				throw new InvalidOperationException(SpineClassName + " does not have signal " + spineSignal + "!");
			}
		}
	}
}
