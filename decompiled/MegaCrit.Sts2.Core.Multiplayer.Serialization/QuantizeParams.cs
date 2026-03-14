namespace MegaCrit.Sts2.Core.Multiplayer.Serialization;

public struct QuantizeParams(float min, float max, int bits)
{
	public float min = min;

	public float max = max;

	public int bits = bits;
}
