using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

[ScriptPath("res://src/Core/Nodes/Vfx/Utilities/NHitStop.cs")]
public class NHitStop : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName DoHitStop = StringName.op_Implicit("DoHitStop");

		public static readonly StringName SetTimeScale = StringName.op_Implicit("SetTimeScale");

		public static readonly StringName EaseForStrength = StringName.op_Implicit("EaseForStrength");

		public static readonly StringName SecondsForDuration = StringName.op_Implicit("SecondsForDuration");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	private const float _minTimeScale = 0.1f;

	private CancellationTokenSource? _cancelToken;

	public void DoHitStop(ShakeStrength strength, ShakeDuration duration)
	{
		_cancelToken?.Cancel();
		_cancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(HitStopTask(EaseForStrength(strength), SecondsForDuration(duration)));
	}

	private async Task HitStopTask(Ease.Functions easing, float seconds)
	{
		SetTimeScale(0.1f);
		ulong lastTicks = Time.GetTicksMsec();
		float timer = 0f;
		while (timer <= seconds)
		{
			await ((GodotObject)((Node)this).GetTree()).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			if (_cancelToken?.IsCancellationRequested ?? false)
			{
				return;
			}
			timer += (float)(Time.GetTicksMsec() - lastTicks) / 1000f;
			float num = Ease.Interpolate(timer / seconds, easing);
			float timeScale = Mathf.Min(0.1f + num * 0.9f, 1f);
			SetTimeScale(timeScale);
			lastTicks = Time.GetTicksMsec();
		}
		SetTimeScale(1f);
	}

	private void SetTimeScale(float timeScale)
	{
		Engine.SetTimeScale((double)timeScale);
	}

	private Ease.Functions EaseForStrength(ShakeStrength strength)
	{
		return strength switch
		{
			ShakeStrength.VeryWeak => Ease.Functions.CircIn, 
			ShakeStrength.Weak => Ease.Functions.SineIn, 
			ShakeStrength.Medium => Ease.Functions.QuadIn, 
			ShakeStrength.Strong => Ease.Functions.QuartIn, 
			ShakeStrength.TooMuch => Ease.Functions.ExpoIn, 
			_ => throw new ArgumentOutOfRangeException("strength", strength, null), 
		};
	}

	private float SecondsForDuration(ShakeDuration duration)
	{
		return duration switch
		{
			ShakeDuration.Short => 0.15f, 
			ShakeDuration.Normal => 0.3f, 
			ShakeDuration.Long => 0.6f, 
			ShakeDuration.Forever => 2f, 
			_ => throw new ArgumentOutOfRangeException("duration", duration, null), 
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName.DoHitStop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("strength"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTimeScale, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("timeScale"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EaseForStrength, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("strength"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SecondsForDuration, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.DoHitStop && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			DoHitStop(VariantUtils.ConvertTo<ShakeStrength>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<ShakeDuration>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetTimeScale && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTimeScale(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EaseForStrength && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Ease.Functions functions = EaseForStrength(VariantUtils.ConvertTo<ShakeStrength>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Ease.Functions>(ref functions);
			return true;
		}
		if ((ref method) == MethodName.SecondsForDuration && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			float num = SecondsForDuration(VariantUtils.ConvertTo<ShakeDuration>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<float>(ref num);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.DoHitStop)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTimeScale)
		{
			return true;
		}
		if ((ref method) == MethodName.EaseForStrength)
		{
			return true;
		}
		if ((ref method) == MethodName.SecondsForDuration)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
	}
}
