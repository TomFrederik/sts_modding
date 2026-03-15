using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Rooms;

namespace FirstMod;

[ModInitializer("ModLoaded")]
public static class ModInit
{
    private const decimal NEW_HEAL_AMOUNT = 12m;
    private static Harmony? _harmony;

    public static void ModLoaded()
    {
        Log.Info("[FirstMod] Initializing...");
        ApplyHarmonyPatches();
        Log.Info("[FirstMod] Initialization complete!");
    }

    private static void ApplyHarmonyPatches()
    {
        try
        {
            _harmony = new Harmony("firstmod.burningblood");

            // Patch AfterCombatVictory - the actual healing
            var afterCombatMethod = typeof(BurningBlood).GetMethod(
                "AfterCombatVictory",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(CombatRoom) },
                null
            );

            if (afterCombatMethod != null)
            {
                var prefixMethod = typeof(ModInit).GetMethod(
                    nameof(Prefix_AfterCombatVictory),
                    BindingFlags.NonPublic | BindingFlags.Static
                );
                _harmony.Patch(afterCombatMethod, prefix: new HarmonyMethod(prefixMethod));
                Log.Info("[FirstMod] Patched BurningBlood.AfterCombatVictory");
            }

            // Patch CanonicalVars getter - the tooltip value
            var canonicalVarsGetter = typeof(BurningBlood).GetProperty(
                "CanonicalVars",
                BindingFlags.NonPublic | BindingFlags.Instance
            )?.GetGetMethod(true);

            if (canonicalVarsGetter != null)
            {
                var postfixMethod = typeof(ModInit).GetMethod(
                    nameof(Postfix_CanonicalVars),
                    BindingFlags.NonPublic | BindingFlags.Static
                );
                _harmony.Patch(canonicalVarsGetter, postfix: new HarmonyMethod(postfixMethod));
                Log.Info("[FirstMod] Patched BurningBlood.CanonicalVars (tooltip shows 12)");
            }

            Log.Info("[FirstMod] BurningBlood now heals 12 HP instead of 6!");
        }
        catch (Exception e)
        {
            Log.Error($"[FirstMod] Failed to apply Harmony patches: {e.Message}\n{e.StackTrace}");
        }
    }

    // Prefix: Replace healing behavior
    private static bool Prefix_AfterCombatVictory(BurningBlood __instance, ref Task __result)
    {
        __result = DoHeal(__instance);
        return false;
    }

    private static async Task DoHeal(BurningBlood relic)
    {
        if (!relic.Owner.Creature.IsDead)
        {
            relic.Flash();
            await CreatureCmd.Heal(relic.Owner.Creature, NEW_HEAL_AMOUNT);
        }
    }

    // Postfix: Replace tooltip value
    private static void Postfix_CanonicalVars(ref IEnumerable<DynamicVar> __result)
    {
        __result = new[] { new HealVar(NEW_HEAL_AMOUNT) };
    }
}
