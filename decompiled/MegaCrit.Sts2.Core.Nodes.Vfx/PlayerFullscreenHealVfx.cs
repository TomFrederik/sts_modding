using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

public static class PlayerFullscreenHealVfx
{
	public static void Play(Player player, decimal healAmount, Control? vfxContainer)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (!TestMode.IsOn && !(healAmount < 1m) && vfxContainer != null)
		{
			float num = Ease.QuadOut((float)(healAmount / (decimal)player.Creature.MaxHp));
			Color green = StsColors.green;
			green.A = Mathf.Max(num * 0.8f, 0.4f);
			NSmokyVignetteVfx nSmokyVignetteVfx = NSmokyVignetteVfx.Create(green, new Color(0f, 1f, 0f, 0.33f));
			if (nSmokyVignetteVfx != null)
			{
				((Node)(object)vfxContainer).AddChildSafely((Node?)(object)nSmokyVignetteVfx);
			}
			string scenePath = SceneHelper.GetScenePath("vfx/vfx_cross_heal_fullscreen");
			NVfxParticleSystem nVfxParticleSystem = PreloadManager.Cache.GetScene(scenePath).Instantiate<NVfxParticleSystem>((GenEditState)0);
			Rect2 viewportRect = ((CanvasItem)NGame.Instance).GetViewportRect();
			((Node2D)nVfxParticleSystem).GlobalPosition = ((Rect2)(ref viewportRect)).Size * 0.5f;
			GpuParticles2D node = ((Node)nVfxParticleSystem).GetNode<GpuParticles2D>(NodePath.op_Implicit("beam"));
			ParticleProcessMaterial val = (ParticleProcessMaterial)node.ProcessMaterial;
			val.EmissionBoxExtents = new Vector3(((Rect2)(ref viewportRect)).Size.X / 2f, ((Rect2)(ref viewportRect)).Size.Y / 2f, 1f);
			int amount = Mathf.RoundToInt(num * 40f + 10f);
			node.Amount = amount;
			((Node)(object)vfxContainer).AddChildSafely((Node?)(object)nVfxParticleSystem);
		}
	}
}
