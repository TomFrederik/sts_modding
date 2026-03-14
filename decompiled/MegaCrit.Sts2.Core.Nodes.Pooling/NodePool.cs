using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Nodes.Pooling;

public class NodePool
{
	private static Dictionary<Type, INodePool> _pools = new Dictionary<Type, INodePool>();

	public static NodePool<T> Init<T>(string scenePath, int prewarmCount) where T : Node, IPoolable
	{
		Type typeFromHandle = typeof(T);
		if (_pools.TryGetValue(typeFromHandle, out INodePool _))
		{
			throw new InvalidOperationException($"Tried to init NodePool for type {typeof(T)} but it's already initialized!");
		}
		NodePool<T> nodePool = new NodePool<T>(scenePath, prewarmCount);
		_pools[typeFromHandle] = nodePool;
		return nodePool;
	}

	public static IPoolable Get(Type type)
	{
		if (!_pools.TryGetValue(type, out INodePool value))
		{
			throw new InvalidOperationException($"Tried to get pool for type {type} before it was initialized!");
		}
		return value.Get();
	}

	public static void Free(IPoolable poolable)
	{
		Type type = poolable.GetType();
		if (!_pools.TryGetValue(type, out INodePool value))
		{
			throw new InvalidOperationException($"Tried to get pool for type {type} before it was initialized!");
		}
		value.Free(poolable);
	}

	public static T Get<T>() where T : Node, IPoolable
	{
		return (T)Get(typeof(T));
	}

	public static void Free<T>(T obj) where T : Node, IPoolable
	{
		Free((IPoolable)obj);
	}
}
public class NodePool<T> : INodePool where T : Node, IPoolable
{
	private static Variant _nameStr = Variant.CreateFrom("name");

	private static Variant _callableStr = Variant.CreateFrom("callable");

	private static Variant _signalStr = Variant.CreateFrom("signal");

	private string _scenePath;

	private readonly List<T> _freeObjects = new List<T>();

	private readonly HashSet<T> _usedObjects = new HashSet<T>();

	public IReadOnlyList<T> DebugFreeObjects => _freeObjects;

	public NodePool(string scenePath, int prewarmCount = 0)
	{
		_scenePath = scenePath;
		for (int i = 0; i < prewarmCount; i++)
		{
			_freeObjects.Add(Instantiate());
		}
	}

	IPoolable INodePool.Get()
	{
		return Get();
	}

	void INodePool.Free(IPoolable poolable)
	{
		Free((T)poolable);
	}

	public T Get()
	{
		T val;
		if (_freeObjects.Count > 0)
		{
			List<T> freeObjects = _freeObjects;
			val = freeObjects[freeObjects.Count - 1];
			_freeObjects.RemoveAt(_freeObjects.Count - 1);
		}
		else
		{
			val = Instantiate();
		}
		_usedObjects.Add(val);
		val.OnReturnedFromPool();
		return val;
	}

	public void Free(T obj)
	{
		if (!_usedObjects.Contains(obj))
		{
			if (_freeObjects.Contains(obj))
			{
				Log.Error($"Tried to free object {obj} ({obj.GetType()}) back to pool {typeof(NodePool<T>)} but it's already been freed!");
			}
			else
			{
				Log.Error($"Tried to free object {obj} ({obj.GetType()}) back to pool {typeof(NodePool<T>)} but it's not part of the pool!");
			}
			((Node)(object)obj).QueueFreeSafelyNoPool();
		}
		else
		{
			DisconnectIncomingAndOutgoingSignals((Node)(object)obj);
			_usedObjects.Remove(obj);
			_freeObjects.Add(obj);
			obj.OnFreedToPool();
		}
	}

	private T Instantiate()
	{
		T val = PreloadManager.Cache.GetScene(_scenePath).Instantiate<T>((GenEditState)0);
		val.OnInstantiated();
		return val;
	}

	private void DisconnectIncomingAndOutgoingSignals(Node obj)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		Variant val;
		foreach (Dictionary signal3 in ((GodotObject)obj).GetSignalList())
		{
			val = signal3[_nameStr];
			StringName val2 = ((Variant)(ref val)).AsStringName();
			foreach (Dictionary signalConnection in ((GodotObject)obj).GetSignalConnectionList(val2))
			{
				val = signalConnection[_callableStr];
				Callable callable = ((Variant)(ref val)).AsCallable();
				val = signalConnection[_signalStr];
				Signal signal = ((Variant)(ref val)).AsSignal();
				DisconnectSignal(callable, signal);
			}
		}
		foreach (Dictionary incomingConnection in ((GodotObject)obj).GetIncomingConnections())
		{
			val = incomingConnection[_callableStr];
			Callable callable2 = ((Variant)(ref val)).AsCallable();
			val = incomingConnection[_signalStr];
			Signal signal2 = ((Variant)(ref val)).AsSignal();
			DisconnectSignal(callable2, signal2);
		}
		for (int i = 0; i < obj.GetChildCount(false); i++)
		{
			DisconnectIncomingAndOutgoingSignals(obj.GetChild(i, false));
		}
	}

	private void DisconnectSignal(Callable callable, Signal signal)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		GodotObject target = ((Callable)(ref callable)).Target;
		if (target == null && ((Callable)(ref callable)).Method == (StringName)null)
		{
			return;
		}
		StringName name = ((Signal)(ref signal)).Name;
		Node val = (Node)(object)((target is Node) ? target : null);
		if (val == null || val.IsInsideTree())
		{
			GodotObject owner = ((Signal)(ref signal)).Owner;
			Node val2 = (Node)(object)((owner is Node) ? owner : null);
			if (val != null && ((GodotObject)val).HasSignal(name) && ((GodotObject)val).IsConnected(name, callable))
			{
				((GodotObject)val).Disconnect(name, callable);
			}
			else if (val2 != null && ((GodotObject)val2).HasSignal(name) && ((GodotObject)val2).IsConnected(name, callable))
			{
				((GodotObject)val2).Disconnect(name, callable);
			}
		}
	}
}
