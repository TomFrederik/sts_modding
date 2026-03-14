using Godot;

namespace MegaCrit.Sts2.Core.Helpers;

public static class MathHelper
{
	public const float degToRad = 0.0174533f;

	public static float Remap(float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	public static Vector2 BezierCurve(Vector2 v0, Vector2 v1, Vector2 c0, float t)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Pow(1f - t, 2f) * v0 + 2f * (1f - t) * t * c0 + Mathf.Pow(t, 2f) * v1;
	}

	public static float GetAngle(Vector2 vector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Atan2(vector.Y, vector.X);
	}

	public static Vector2 Clamp(Vector2 input, float min, float max)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Clamp(input.X, min, max);
		return new Vector2(num, num);
	}

	public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float deltaTime, float maxSpeed = float.PositiveInfinity)
	{
		smoothTime = Mathf.Max(0.0001f, smoothTime);
		float num = 2f / smoothTime;
		float num2 = num * deltaTime;
		float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
		float num4 = current - target;
		float num5 = target;
		float num6 = maxSpeed * smoothTime;
		num4 = Mathf.Clamp(num4, 0f - num6, num6);
		target = current - num4;
		float num7 = (currentVelocity + num * num4) * deltaTime;
		currentVelocity = (currentVelocity - num * num7) * num3;
		float num8 = target + (num4 + num7) * num3;
		if (num5 - current > 0f == num8 > num5)
		{
			num8 = num5;
			currentVelocity = (num8 - num5) / deltaTime;
		}
		return num8;
	}

	public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float deltaTime, float maxSpeed = float.PositiveInfinity)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		smoothTime = Mathf.Max(0.0001f, smoothTime);
		float num = 2f / smoothTime;
		float num2 = num * deltaTime;
		float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
		float num4 = current.X - target.X;
		float num5 = current.Y - target.Y;
		Vector2 val = target;
		float num6 = maxSpeed * smoothTime;
		float num7 = num6 * num6;
		float num8 = num4 * num4 + num5 * num5;
		if (num8 > num7)
		{
			float num9 = Mathf.Sqrt(num8);
			num4 = num4 / num9 * num6;
			num5 = num5 / num9 * num6;
		}
		target.X = current.X - num4;
		target.Y = current.Y - num5;
		float num10 = (currentVelocity.X + num * num4) * deltaTime;
		float num11 = (currentVelocity.Y + num * num5) * deltaTime;
		currentVelocity.X = (currentVelocity.X - num * num10) * num3;
		currentVelocity.Y = (currentVelocity.Y - num * num11) * num3;
		float num12 = target.X + (num4 + num10) * num3;
		float num13 = target.Y + (num5 + num11) * num3;
		float num14 = val.X - current.X;
		float num15 = val.Y - current.Y;
		float num16 = num12 - val.X;
		float num17 = num13 - val.Y;
		if (num14 * num16 + num15 * num17 > 0f)
		{
			num12 = val.X;
			num13 = val.Y;
			currentVelocity.X = (num12 - val.X) / deltaTime;
			currentVelocity.Y = (num13 - val.Y) / deltaTime;
		}
		return new Vector2(num12, num13);
	}
}
