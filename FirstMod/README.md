# FirstMod - STS2 Modding Project

A test mod that makes Ironclad's **Burning Blood** relic heal **12 HP** instead of 6 after combat.

## Quick Start

```bash
# Build
dotnet build -c Release

# Deploy
cp .godot/mono/temp/bin/Release/FirstMod.dll "$MODS/FirstMod/"
```

## Project Structure

```
FirstMod/
├── FirstMod.csproj      # C# project
├── ModInit.cs           # Entry point + Harmony patches
├── project.godot        # Godot project file
└── README.md
```

Deployed mod structure:
```
$MODS/FirstMod/
├── FirstMod.json        # Manifest (required)
└── FirstMod.dll         # Compiled code
```

## Environment Setup

### Required
- .NET 9.0 SDK
- STS2 Public Beta (for macOS ARM64 Harmony support)

### Environment Variables
```bash
export MODS="$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods"
```

## How It Works

Uses Harmony to patch `BurningBlood`:

1. **AfterCombatVictory** (prefix) - Replaces healing logic to use 12 instead of 6
2. **CanonicalVars** (postfix) - Updates tooltip to show "12" instead of "6"

### Key Pattern: Manual Harmony Patching

```csharp
[ModInitializer("ModLoaded")]
public static class ModInit
{
    private static Harmony? _harmony;

    public static void ModLoaded()
    {
        _harmony = new Harmony("firstmod.burningblood");

        // Get method via reflection
        var method = typeof(BurningBlood).GetMethod(
            "AfterCombatVictory",
            BindingFlags.Public | BindingFlags.Instance,
            null,
            new[] { typeof(CombatRoom) },
            null
        );

        // Apply patch manually
        var patch = typeof(ModInit).GetMethod(
            nameof(Prefix_AfterCombatVictory),
            BindingFlags.NonPublic | BindingFlags.Static
        );
        _harmony.Patch(method, prefix: new HarmonyMethod(patch));
    }

    private static bool Prefix_AfterCombatVictory(BurningBlood __instance, ref Task __result)
    {
        __result = DoHeal(__instance);
        return false; // Skip original
    }
}
```

## Manifest Format (FirstMod.json)

```json
{
  "id": "FirstMod",
  "name": "FirstMod",
  "author": "TomFrederik",
  "description": "Test mod - BurningBlood heals 12 instead of 6",
  "version": "1.0.0",
  "has_pck": false,
  "has_dll": true,
  "affects_gameplay": true
}
```

**Note:** PCK is not required for code-only mods. Set `has_pck: false`.

## References

- `../MODDING_GUIDE.md` - Full modding documentation
- `../decompiled/` - Decompiled game source
- [Harmony Wiki](https://harmony.pardeike.net/articles/intro.html)
