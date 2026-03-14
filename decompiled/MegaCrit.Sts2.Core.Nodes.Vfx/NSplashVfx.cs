using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NSplashVfx.cs")]
public class NSplashVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _particles = StringName.op_Implicit("_particles");

		public static readonly StringName _tint = StringName.op_Implicit("_tint");
	}

	public class SignalName : SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("vfx/vfx_splash");

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<GpuParticles2D> _particles = new Array<GpuParticles2D>();

	private Color _tint = Colors.White;

	private CancellationTokenSource? _cts;

	public static NSplashVfx? Create(Vector2 targetPosition, Color tint)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NSplashVfx nSplashVfx = PreloadManager.Cache.GetScene(scenePath).Instantiate<NSplashVfx>((GenEditState)0);
		((Node2D)nSplashVfx).GlobalPosition = targetPosition;
		nSplashVfx._tint = tint;
		return nSplashVfx;
	}

	public override void _Ready()
	{
		TaskHelper.RunSafely(PlayVfx());
	}

	public override void _ExitTree()
	{
		_cts?.Cancel();
		_cts?.Dispose();
	}

	private async Task PlayVfx()
	{
		_cts = new CancellationTokenSource();
		foreach (GpuParticles2D particle in _particles)
		{
			((CanvasItem)particle).SelfModulate = _tint;
			particle.Restart();
		}
		await Cmd.Wait(2f, _cts.Token);
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("targetPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)20, StringName.op_Implicit("tint"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NSplashVfx nSplashVfx = Create(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NSplashVfx>(ref nSplashVfx);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NSplashVfx nSplashVfx = Create(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NSplashVfx>(ref nSplashVfx);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._particles)
		{
			_particles = VariantUtils.ConvertToArray<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tint)
		{
			_tint = VariantUtils.ConvertTo<Color>(ref value);
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
		if ((ref name) == PropertyName._particles)
		{
			value = VariantUtils.CreateFromArray<GpuParticles2D>(_particles);
			return true;
		}
		if ((ref name) == PropertyName._tint)
		{
			value = VariantUtils.CreateFrom<Color>(ref _tint);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)28, PropertyName._particles, (PropertyHint)23, "24/34:GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)20, PropertyName._tint, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._particles, Variant.CreateFrom<GpuParticles2D>(_particles));
		info.AddProperty(PropertyName._tint, Variant.From<Color>(ref _tint));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._particles, ref val))
		{
			_particles = ((Variant)(ref val)).AsGodotArray<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._tint, ref val2))
		{
			_tint = ((Variant)(ref val2)).As<Color>();
		}
	}
}
