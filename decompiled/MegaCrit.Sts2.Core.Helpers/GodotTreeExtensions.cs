using System;
using Godot;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Pooling;

namespace MegaCrit.Sts2.Core.Helpers;

public static class GodotTreeExtensions
{
	public static void AddChildSafely(this Node parent, Node? child)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (child != null)
		{
			if (NGame.IsMainThread())
			{
				parent.AddChild(child, false, (InternalMode)0);
				return;
			}
			((GodotObject)parent).CallDeferred(MethodName.AddChild, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)child) });
		}
	}

	public static void RemoveChildSafely(this Node parent, Node? child)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (child != null)
		{
			if (NGame.IsMainThread())
			{
				parent.RemoveChild(child);
				return;
			}
			((GodotObject)parent).CallDeferred(MethodName.RemoveChild, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)child) });
		}
	}

	public static void QueueFreeSafely(this Node node)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!GodotObject.IsInstanceValid((GodotObject)(object)node))
		{
			return;
		}
		IPoolable poolable = node as IPoolable;
		if (poolable != null)
		{
			node.GetParent()?.RemoveChildSafely(node);
			Callable val = Callable.From((Action)delegate
			{
				NodePool.Free(poolable);
			});
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		}
		else
		{
			node.QueueFreeSafelyNoPool();
		}
	}

	public static void QueueFreeSafelyNoPool(this Node node)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (NGame.IsMainThread())
		{
			node.QueueFree();
		}
		else
		{
			((GodotObject)node).CallDeferred(MethodName.QueueFree, Array.Empty<Variant>());
		}
	}

	public static void FreeChildren(this Node node)
	{
		foreach (Node child in node.GetChildren(false))
		{
			child.QueueFreeSafely();
		}
	}
}
