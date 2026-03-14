using System;
using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

public class ScreenPunchInstance : ShakeInstance
{
	public ScreenPunchInstance(float strength, double duration, float degAngle)
	{
		_strength = strength;
		_startDuration = duration;
		_duration = duration;
		_angle = Mathf.DegToRad(degAngle);
	}

	public override Vector2 Update(double delta)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (base.IsDone)
		{
			return Vector2.Zero;
		}
		_duration -= delta;
		float num = (float)Math.Cos(_duration * (double)ShakeInstance.WiggleSpeed);
		_ease = Ease.CubicOut((float)(_duration / _startDuration));
		Vector2 val = new Vector2(num, 0f);
		Vector2 result = ((Vector2)(ref val)).Rotated(_angle) * _strength * _ease;
		if (_duration < 0.0)
		{
			base.IsDone = true;
		}
		return result;
	}

	public void Cancel()
	{
		_duration = 0.0;
		base.IsDone = true;
	}
}
