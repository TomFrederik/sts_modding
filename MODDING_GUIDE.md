# Slay the Spire 2 Modding Guide

Based on decompilation of `sts2.dll` (game version built with Godot 4.5 + C#/.NET 9.0)

## Table of Contents
- [Mod Structure](#mod-structure)
- [Harmony Patching](#harmony-patching)
- [Creating a New Relic](#creating-a-new-relic)
- [Hook System](#hook-system)
- [Custom UI/Icons](#custom-uiicons)
- [Key Namespaces](#key-namespaces)

---

## Mod Structure

### New Structure (Public Beta+)

A mod requires a JSON manifest file. PCK and DLL are optional:

```
YourMod/
├── YourMod.json         # Required: Mod manifest (outside PCK)
├── YourMod.dll          # Optional: Compiled C# code
└── YourMod.pck          # Optional: Godot resource pack (scenes, textures, etc.)
```

### YourMod.json (Manifest)

```json
{
  "id": "YourMod",
  "name": "Your Mod Display Name",
  "author": "YourName",
  "description": "What your mod does",
  "version": "1.0.0",
  "has_pck": false,
  "has_dll": true,
  "affects_gameplay": true
}
```

**Fields:**
- `id`: Unique identifier (should match filename)
- `has_pck`: Set `false` for code-only mods (no custom resources)
- `has_dll`: Set `true` if mod includes compiled C# code
- `affects_gameplay`: Set `false` for cosmetic-only mods (not checked in multiplayer)

### Project Setup (.csproj)

```xml
<Project Sdk="Godot.NET.Sdk/4.5.1">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <EnableDynamicLoading>true</EnableDynamicLoading>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="sts2">
      <HintPath>../sts2.dll</HintPath>
      <Private>false</Private>
    </Reference>
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Lib.Harmony" Version="2.3.3" />
  </ItemGroup>
</Project>
```

**Important:** Set `Private=false` for sts2.dll so it's not copied to output.

### Entry Point

```csharp
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;

namespace YourMod;

[ModInitializer("ModLoaded")]
public static class YourMod
{
    public static void ModLoaded()
    {
        Log.Info("Your mod loaded!");
        // Apply Harmony patches, register models, etc.
    }
}
```

### Mods Directory Location

- **macOS**: `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods/`
- **Windows**: `<SteamLibrary>/steamapps/common/Slay the Spire 2/mods/`

---

## Harmony Patching

Harmony is included in the game and works on all platforms (including macOS ARM64 as of the public beta).

### IMPORTANT: Use Manual Patching

**DO NOT use `[HarmonyPatch]` attributes.** Use manual patching with `[ModInitializer]`:

```csharp
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;

namespace YourMod;

[ModInitializer("ModLoaded")]
public static class YourMod
{
    private static Harmony? _harmony;

    public static void ModLoaded()
    {
        _harmony = new Harmony("yourname.yourmod");

        // Get the method to patch
        var targetMethod = typeof(SomeClass).GetMethod(
            "SomeMethod",
            BindingFlags.Public | BindingFlags.Instance,
            null,
            new[] { typeof(ParamType) },
            null
        );

        if (targetMethod != null)
        {
            var patchMethod = typeof(YourMod).GetMethod(
                nameof(Patch_SomeMethod),
                BindingFlags.NonPublic | BindingFlags.Static
            );
            _harmony.Patch(targetMethod, prefix: new HarmonyMethod(patchMethod));
            Log.Info("Patched SomeClass.SomeMethod");
        }
    }

    // Prefix: Runs before original. Return false to skip original.
    private static bool Patch_SomeMethod(SomeClass __instance, ref ReturnType __result)
    {
        // Your code here
        return true; // true = continue to original, false = skip original
    }
}
```

### Patching Properties

```csharp
// Get a property getter (including protected/private)
var propertyGetter = typeof(SomeClass).GetProperty(
    "SomeProperty",
    BindingFlags.NonPublic | BindingFlags.Instance
)?.GetGetMethod(true);

if (propertyGetter != null)
{
    _harmony.Patch(propertyGetter, postfix: new HarmonyMethod(postfixMethod));
}
```

### Patch Method Signatures

**Prefix** - runs before original:
```csharp
// Return false to skip original method
private static bool Prefix_Method(TargetClass __instance, ref ParamType param)
{
    param = newValue; // Modify parameter
    return true;      // Continue to original
}

// For async methods, set __result to skip original
private static bool Prefix_AsyncMethod(TargetClass __instance, ref Task __result)
{
    __result = YourReplacementTask();
    return false; // Skip original
}
```

**Postfix** - runs after original:
```csharp
// Modify the return value
private static void Postfix_Method(ref ReturnType __result)
{
    __result = newValue;
}
```

### Special Parameters
- `__instance` - The object instance (for instance methods)
- `__result` - Return value (use `ref` to modify)
- `___fieldName` - Access private field `fieldName` (triple underscore)

---

## Creating a New Relic

Relics extend `RelicModel` and are auto-discovered via `ReflectionHelper.GetSubtypesInMods<T>()`.

### Relic Lifecycle Hooks

| Method | When Called |
|--------|-------------|
| `AfterObtained()` | When player picks up the relic |
| `AfterRemoved()` | When relic is removed |
| `BeforeCombatStart()` | Before combat begins |
| `AfterCombatVictory(CombatRoom)` | After winning combat |
| `AfterCombatEnd(CombatRoom)` | After combat ends |
| `BeforeCardPlayed(CardPlay)` | Before a card is played |
| `AfterCardPlayed(PlayerChoiceContext, CardPlay)` | After a card is played |
| `BeforeSideTurnStart(PlayerChoiceContext, CombatSide, CombatState)` | Before turn starts |
| `AfterTurnEnd(PlayerChoiceContext, CombatSide)` | After turn ends |
| `AfterDamageGiven(...)` | After dealing damage |
| `AfterDamageReceived(...)` | After taking damage |
| `BeforeDeath(Creature)` | Before creature dies |

### Example: Custom Relic

```csharp
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;

namespace YourMod.Relics;

public sealed class MyCustomRelic : RelicModel
{
    private int _counter;

    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override bool ShowCounter => true;
    public override int DisplayAmount => _counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Attack)
            return;

        _counter++;
        if (_counter >= 3)
        {
            _counter = 0;
            Flash();
            // Apply effect using PowerCmd, DamageCmd, etc.
        }
    }
}
```

### Registering the Relic

```csharp
[ModInitializer("ModLoaded")]
public static class YourMod
{
    public static void ModLoaded()
    {
        ModelDb.Inject(typeof(MyCustomRelic));
    }
}
```

---

## Hook System

The game uses `MegaCrit.Sts2.Core.Hooks.Hook` to dispatch events to all models that implement `ShouldReceiveCombatHooks => true`.

### Modifier Hooks (return modified values)

| Method | Purpose |
|--------|---------|
| `ModifyDamageAdditive(...)` | Add flat damage |
| `ModifyDamageMultiplicative(...)` | Multiply damage |
| `ModifyBlockAdditive(...)` | Add flat block |
| `ModifyHealAmount(Creature, decimal)` | Modify healing |
| `ModifyEnergyCostInCombat(...)` | Modify card energy cost |

### Conditional Hooks (return bool)

| Method | Purpose |
|--------|---------|
| `ShouldDie(Creature)` | Return `false` to prevent death |
| `ShouldPlay(CardModel, AutoPlayType)` | Return `false` to prevent card play |
| `ShouldClearBlock(Creature)` | Return `false` to prevent block clear |

---

## Commands (Game Actions)

Use commands to apply game effects:

```csharp
using MegaCrit.Sts2.Core.Commands;

// Apply power to creature
await PowerCmd.Apply<StrengthPower>(creature, amount, applier, cardSource);

// Deal damage
await DamageCmd.Deal(target, amount, dealer, props, cardSource);

// Gain block
await BlockCmd.Gain(creature, amount, cardSource);

// Heal
await CreatureCmd.Heal(creature, amount);

// Draw cards
await DrawCmd.Draw(player, count, context);
```

---

## Key Namespaces

| Namespace | Contents |
|-----------|----------|
| `MegaCrit.Sts2.Core.Modding` | `ModManager`, `ModInitializerAttribute` |
| `MegaCrit.Sts2.Core.Models` | `AbstractModel`, `ModelDb`, `RelicModel`, `CardModel`, `PowerModel` |
| `MegaCrit.Sts2.Core.Models.Relics` | All built-in relics |
| `MegaCrit.Sts2.Core.Entities.Relics` | `RelicRarity`, `RelicStatus` |
| `MegaCrit.Sts2.Core.Commands` | `PowerCmd`, `DamageCmd`, `BlockCmd`, `CreatureCmd` |
| `MegaCrit.Sts2.Core.Logging` | `Log` (Info, Warn, Error) |
| `MegaCrit.Sts2.Core.Localization.DynamicVars` | `HealVar`, `DamageVar`, etc. for tooltips |

---

## Decompiled Source

Full decompiled source is at: `~/code/sts2_modding/decompiled/`

Key files:
- `MegaCrit.Sts2.Core.Models/RelicModel.cs` - Base relic class
- `MegaCrit.Sts2.Core.Models/AbstractModel.cs` - All hook methods
- `MegaCrit.Sts2.Core.Modding/ModManager.cs` - How mods are loaded
- `MegaCrit.Sts2.Core.Models.Relics/*.cs` - Example relic implementations
