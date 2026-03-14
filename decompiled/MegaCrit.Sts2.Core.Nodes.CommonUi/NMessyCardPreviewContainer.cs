using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NMessyCardPreviewContainer.cs")]
public class NMessyCardPreviewContainer : Control
{
	public class PoissonDiscSampler
	{
		private struct GridPos(Vector2 sample, float cellSize)
		{
			public readonly int x = (int)(sample.X / cellSize);

			public readonly int y = (int)(sample.Y / cellSize);
		}

		private const int _maxAttempts = 30;

		private readonly Rect2 _rect;

		private readonly float _radius2;

		private readonly float _cellSize;

		private readonly Vector2[,] _grid;

		private readonly List<Vector2> _activeSamples = new List<Vector2>();

		public PoissonDiscSampler(float width, float height, float radius)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			_rect = new Rect2(0f, 0f, width, height);
			_radius2 = radius * radius;
			_cellSize = radius / Mathf.Sqrt(2f);
			_grid = new Vector2[Mathf.CeilToInt(width / _cellSize), Mathf.CeilToInt(height / _cellSize)];
		}

		public IEnumerator<Vector2> Samples()
		{
			yield return AddSample(((Rect2)(ref _rect)).Size / 2f);
			while (_activeSamples.Count > 0)
			{
				int i = (int)(Rng.Chaotic.NextFloat() * (float)_activeSamples.Count);
				Vector2 val = _activeSamples[i];
				bool found = false;
				for (int j = 0; j < 30; j++)
				{
					float num = (float)Math.PI * 2f * Rng.Chaotic.NextFloat();
					float num2 = Mathf.Sqrt(Rng.Chaotic.NextFloat() * 3f * _radius2 + _radius2);
					Vector2 val2 = val + num2 * new Vector2(Mathf.Cos(num), Mathf.Sin(num));
					if (((Rect2)(ref _rect)).HasPoint(val2) && IsFarEnough(val2))
					{
						found = true;
						yield return AddSample(val2);
						break;
					}
				}
				if (!found)
				{
					_activeSamples[i] = _activeSamples[_activeSamples.Count - 1];
					_activeSamples.RemoveAt(_activeSamples.Count - 1);
				}
			}
		}

		private bool IsFarEnough(Vector2 sample)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			GridPos gridPos = new GridPos(sample, _cellSize);
			int num = Mathf.Max(gridPos.x - 2, 0);
			int num2 = Mathf.Max(gridPos.y - 2, 0);
			int num3 = Mathf.Min(gridPos.x + 2, _grid.GetLength(0) - 1);
			int num4 = Mathf.Min(gridPos.y + 2, _grid.GetLength(1) - 1);
			for (int i = num2; i <= num4; i++)
			{
				for (int j = num; j <= num3; j++)
				{
					Vector2 val = _grid[j, i];
					if (val != Vector2.Zero)
					{
						Vector2 val2 = val - sample;
						if (val2.X * val2.X + val2.Y * val2.Y < _radius2)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private Vector2 AddSample(Vector2 sample)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			_activeSamples.Add(sample);
			GridPos gridPos = new GridPos(sample, _cellSize);
			_grid[gridPos.x, gridPos.y] = sample;
			return sample;
		}
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName PositionNewChild = StringName.op_Implicit("PositionNewChild");

		public static readonly StringName ResetSamples = StringName.op_Implicit("ResetSamples");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _resetNewCardTimer = StringName.op_Implicit("_resetNewCardTimer");

		public static readonly StringName _currentMaxPosition = StringName.op_Implicit("_currentMaxPosition");
	}

	public class SignalName : SignalName
	{
	}

	private const float _spacing = 150f;

	private const int _resetNewCardMsec = 2000;

	private ulong _resetNewCardTimer;

	private float _currentMaxPosition;

	private IEnumerator<Vector2>? _samples;

	public override void _Ready()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).Connect(SignalName.ChildEnteredTree, Callable.From<Node>((Action<Node>)PositionNewChild), 0u);
	}

	private void PositionNewChild(Node node)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (Time.GetTicksMsec() > _resetNewCardTimer)
		{
			_currentMaxPosition = 0f;
			ResetSamples();
		}
		_resetNewCardTimer = Time.GetTicksMsec() + 2000;
		if (!_samples.MoveNext())
		{
			ResetSamples();
		}
		Vector2 current = _samples.Current;
		Control val = (Control)(object)((node is Control) ? node : null);
		if (val != null)
		{
			val.Position = current;
			return;
		}
		Node2D val2 = (Node2D)(object)((node is Node2D) ? node : null);
		if (val2 != null)
		{
			val2.Position = current;
		}
	}

	private void ResetSamples()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		PoissonDiscSampler poissonDiscSampler = new PoissonDiscSampler(((Control)this).Size.X, ((Control)this).Size.Y, 150f);
		_samples = poissonDiscSampler.Samples();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PositionNewChild, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ResetSamples, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PositionNewChild && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PositionNewChild(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ResetSamples && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ResetSamples();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.PositionNewChild)
		{
			return true;
		}
		if ((ref method) == MethodName.ResetSamples)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._resetNewCardTimer)
		{
			_resetNewCardTimer = VariantUtils.ConvertTo<ulong>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentMaxPosition)
		{
			_currentMaxPosition = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._resetNewCardTimer)
		{
			value = VariantUtils.CreateFrom<ulong>(ref _resetNewCardTimer);
			return true;
		}
		if ((ref name) == PropertyName._currentMaxPosition)
		{
			value = VariantUtils.CreateFrom<float>(ref _currentMaxPosition);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName._resetNewCardTimer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._currentMaxPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._resetNewCardTimer, Variant.From<ulong>(ref _resetNewCardTimer));
		info.AddProperty(PropertyName._currentMaxPosition, Variant.From<float>(ref _currentMaxPosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._resetNewCardTimer, ref val))
		{
			_resetNewCardTimer = ((Variant)(ref val)).As<ulong>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentMaxPosition, ref val2))
		{
			_currentMaxPosition = ((Variant)(ref val2)).As<float>();
		}
	}
}
