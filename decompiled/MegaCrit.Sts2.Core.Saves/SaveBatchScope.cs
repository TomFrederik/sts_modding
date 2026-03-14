using System;

namespace MegaCrit.Sts2.Core.Saves;

public readonly struct SaveBatchScope(SaveManager saveManager) : IDisposable
{
	public void Dispose()
	{
		saveManager.EndSaveBatch();
	}
}
