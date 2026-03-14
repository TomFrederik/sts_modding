namespace MegaCrit.Sts2.Core.Runs.Metrics;

public struct ActWinMetric(string actId, bool win)
{
	public readonly string act = actId;

	public readonly bool win = win;
}
