using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx.Ui;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

public static class PlayerHurtVignetteHelper
{
	private static NLowHpBorderVfx? _currentVfx;

	public static void Play()
	{
		if (_currentVfx != null && GodotObject.IsInstanceValid((GodotObject)(object)_currentVfx))
		{
			_currentVfx.Play();
			return;
		}
		_currentVfx = NLowHpBorderVfx.Create();
		((Node)(object)NRun.Instance?.GlobalUi).AddChildSafely((Node?)(object)_currentVfx);
		_currentVfx?.Play();
	}
}
