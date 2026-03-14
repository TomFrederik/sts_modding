using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.AutoSlay.Helpers;

public static class UiHelper
{
	public static async Task Click(NClickableControl button, int delayMs = 100)
	{
		button.ForceClick();
		await Task.Delay(delayMs);
	}

	public static List<T> FindAll<T>(Node start) where T : Node
	{
		List<T> list = new List<T>();
		if (GodotObject.IsInstanceValid((GodotObject)(object)start))
		{
			FindAllRecursive(start, list);
		}
		return list;
	}

	private static void FindAllRecursive<T>(Node node, List<T> found) where T : Node
	{
		if (!GodotObject.IsInstanceValid((GodotObject)(object)node))
		{
			return;
		}
		T val = (T)(object)((node is T) ? node : null);
		if (val != null)
		{
			found.Add(val);
		}
		foreach (Node child in node.GetChildren(false))
		{
			FindAllRecursive(child, found);
		}
	}

	public static T? FindFirst<T>(Node start) where T : Node
	{
		if (!GodotObject.IsInstanceValid((GodotObject)(object)start))
		{
			return default(T);
		}
		T val = (T)(object)((start is T) ? start : null);
		if (val != null)
		{
			return val;
		}
		foreach (Node child in start.GetChildren(false))
		{
			T val2 = FindFirst<T>(child);
			if (val2 != null)
			{
				return val2;
			}
		}
		return default(T);
	}
}
