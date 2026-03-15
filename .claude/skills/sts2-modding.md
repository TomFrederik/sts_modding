# Slay the Spire 2 Modding Guide

This skill contains comprehensive knowledge for creating STS2 mods.

## Project Structure

A mod consists of:
```
ModName/
├── ModName.json          # Required manifest
├── ModName.dll           # Compiled C# code
└── ModName.pck           # Optional Godot resource pack
```

### Manifest (ModName.json)
```json
{
  "id": "ModName",
  "name": "Display Name",
  "author": "AuthorName",
  "description": "Mod description",
  "version": "1.0.0",
  "has_pck": false,
  "has_dll": true,
  "affects_gameplay": true
}
```

### Project File (ModName.csproj)
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
      <Private>false</Private>  <!-- IMPORTANT: Don't copy sts2.dll -->
    </Reference>
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Lib.Harmony" Version="2.3.3" />
  </ItemGroup>
</Project>
```

### Entry Point
```csharp
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;

namespace ModName;

[ModInitializer("ModLoaded")]
public static class ModInit
{
    public static void ModLoaded()
    {
        Log.Info("[ModName] Initializing...");
        // Setup code here
    }
}
```

## Key Namespaces

| Namespace | Purpose |
|-----------|---------|
| `MegaCrit.Sts2.Core.Models` | Base model classes (CardModel, RelicModel, PowerModel), ModelDb |
| `MegaCrit.Sts2.Core.Models.Cards` | All card implementations |
| `MegaCrit.Sts2.Core.Models.Relics` | All relic implementations |
| `MegaCrit.Sts2.Core.Models.Powers` | All power/status effect implementations |
| `MegaCrit.Sts2.Core.Entities.Players` | Player runtime state |
| `MegaCrit.Sts2.Core.Entities.Creatures` | Creature (player/enemy) runtime |
| `MegaCrit.Sts2.Core.Entities.Cards` | CardPile, CardPlay, etc. |
| `MegaCrit.Sts2.Core.Commands` | Game commands (PowerCmd, CreatureCmd, etc.) |
| `MegaCrit.Sts2.Core.Combat` | CombatManager, CombatState |
| `MegaCrit.Sts2.Core.Nodes.Combat` | UI nodes (NCreature, NPower, etc.) |
| `MegaCrit.Sts2.Core.Nodes.Rooms` | NCombatRoom |
| `MegaCrit.Sts2.Core.Nodes.HoverTips` | NHoverTipSet for showing tooltips |
| `MegaCrit.Sts2.Core.HoverTips` | HoverTip, IHoverTip, HoverTipAlignment |
| `MegaCrit.Sts2.Core.Hooks` | Hook system for game events |
| `MegaCrit.Sts2.Core.Rooms` | CombatRoom and room types |

## Accessing Game State

### Combat Manager (Singleton)
```csharp
CombatManager.Instance.IsInProgress  // Is combat active?
CombatManager.Instance.CombatSetUp += OnCombatSetUp;  // Combat start event
CombatManager.Instance.CombatEnded += OnCombatEnded;  // Combat end event
CombatManager.Instance.TurnStarted += OnTurnStarted;  // Turn start event
```

### Player's Hand
```csharp
Player player = ...;
CardPile hand = player.PlayerCombatState?.Hand;

// Iterate cards
foreach (CardModel card in hand.Cards) { ... }

// Subscribe to changes
hand.CardAdded += card => { ... };
hand.CardRemoved += card => { ... };
hand.ContentsChanged += () => { ... };
```

### Card Properties
```csharp
card.Type              // CardType.Attack, Skill, Power, Status, Curse
card.Owner             // The Player that owns it
card.IsUpgraded        // Whether upgraded
card.UpgradeLevel      // Upgrade level (0, 1, etc.)
card.Pile              // Current PileType (Hand, Draw, Discard, etc.)
card.DynamicVars       // Access to damage, block, power amounts
card.DynamicVars.TryGetValue("VulnerablePower", out var vulnVar)
card.DynamicVars["Damage"].BaseValue
card.DynamicVars.ContainsKey("VulnerablePower")
```

### Powers/Status Effects
```csharp
Creature target = ...;

// Check for power
bool hasVuln = target.HasPower<VulnerablePower>();

// Get power instance
VulnerablePower? vuln = target.GetPower<VulnerablePower>();

// Get amount
int amount = target.GetPowerAmount<VulnerablePower>();

// Iterate all powers
foreach (PowerModel power in target.Powers) { ... }
```

## UI/Nodes

### Get Creature UI Node
```csharp
NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);
Vector2 topPos = creatureNode.GetTopOfHitbox();
Vector2 bottomPos = creatureNode.GetBottomOfHitbox();
Vector2 vfxPos = creatureNode.VfxSpawnPosition;
```

### Adding Custom UI
```csharp
// Create a TextureRect with proper scaling
var indicator = new TextureRect();
indicator.Texture = ModelDb.Power<VulnerablePower>().Icon;  // Use ModelDb for icons
indicator.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;  // Required to scale
indicator.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
indicator.CustomMinimumSize = new Vector2(32, 32);
indicator.Size = new Vector2(32, 32);
indicator.ZIndex = 100;
indicator.MouseFilter = Control.MouseFilterEnum.Stop;  // Enable mouse events

// Add to creature node (positions are local to parent)
creatureNode.AddChild(indicator);
indicator.Position = new Vector2(x, y);  // Local coordinates, e.g., (0, -320) for above

// For containers, set MouseFilter to Pass to allow events to reach children
var container = new HBoxContainer();
container.MouseFilter = Control.MouseFilterEnum.Pass;
```

### Power Icons (Use ModelDb, Not Resource Paths)
```csharp
// CORRECT: Get icons via ModelDb (handles atlas loading automatically)
Texture2D icon = ModelDb.Power<VulnerablePower>().Icon;
Texture2D bigIcon = ModelDb.Power<VulnerablePower>().BigIcon;

// DON'T use resource paths directly - they may not load correctly
// string iconPath = "res://images/powers/...";  // Avoid this
```

### HoverTips (Tooltips)
```csharp
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

// Create a HoverTip for a power (recommended - handles title/icon/debuff automatically)
var power = ModelDb.Power<VulnerablePower>();
var hoverTip = new HoverTip(power, "Custom description text", isSmart: false);

// Show the tooltip next to a control
var alignment = HoverTip.GetHoverTipAlignment(myControl);  // Determines Left/Right
NHoverTipSet.CreateAndShow(myControl, hoverTip, alignment);

// Remove the tooltip
NHoverTipSet.Remove(myControl);

// HoverTip constructor overloads:
// - HoverTip(PowerModel power, string description, bool isSmart)  // Best for powers
// - HoverTip(LocString title, string description, Texture2D? icon)
// - HoverTip(LocString title, LocString description, Texture2D? icon)
// - HoverTip(LocString description, Texture2D? icon)

// IMPORTANT: Always pass an alignment (Left/Right), not HoverTipAlignment.None
// Otherwise the tooltip appears at (0,0) in the top-left corner
```

### Mouse Events on Custom UI
```csharp
// Subscribe to hover events
indicator.MouseEntered += () => OnHovered(indicator);
indicator.MouseExited += () => OnUnhovered(indicator);

private static void OnHovered(TextureRect indicator)
{
    var hoverTip = new HoverTip(power, "Description", isSmart: false);
    var alignment = HoverTip.GetHoverTipAlignment(indicator);
    NHoverTipSet.CreateAndShow(indicator, hoverTip, alignment);
}

private static void OnUnhovered(TextureRect indicator)
{
    NHoverTipSet.Remove(indicator);
}
```

## Commands

### Apply Powers
```csharp
await PowerCmd.Apply<VulnerablePower>(target, amount, dealer, cardSource);
await PowerCmd.Apply<WeakPower>(enemy, 2, player.Creature, card);
await PowerCmd.Remove<VulnerablePower>(creature);
await PowerCmd.Increase<StrengthPower>(creature, 2);
```

### Creature Actions
```csharp
await CreatureCmd.Damage(ctx, target, 10, ValueProp.None, dealer, cardSource);
await CreatureCmd.GainBlock(creature, 5, ValueProp.None, cardPlay);
await CreatureCmd.Heal(creature, 12);
```

### Card Pile Operations
```csharp
await CardPileCmd.Draw(ctx, 3, player);
await CardPileCmd.Add(card, PileType.Exhaust);
await CardPileCmd.Shuffle(ctx, player);
```

### Fluent Attack Builder
```csharp
await new AttackCommand(10)
    .FromCard(this)
    .Targeting(target)
    .WithHitFx("vfx/vfx_slash")
    .Execute(ctx);
```

## Harmony Patching

**IMPORTANT:** Never use `[HarmonyPatch]` attributes. Always use manual patching.

```csharp
using HarmonyLib;
using System.Reflection;

private static Harmony? _harmony;

public static void ModLoaded()
{
    _harmony = new Harmony("mymod.uniqueid");

    var targetMethod = typeof(TargetClass).GetMethod(
        "MethodName",
        BindingFlags.Public | BindingFlags.Instance,
        null,
        new[] { typeof(ParamType) },
        null
    );

    var prefixMethod = typeof(ModInit).GetMethod(
        nameof(MyPrefix),
        BindingFlags.NonPublic | BindingFlags.Static
    );

    _harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefixMethod));
}

// Prefix: runs before original, return false to skip original
private static bool MyPrefix(TargetClass __instance, ref ReturnType __result)
{
    __result = customValue;
    return false; // Skip original
}

// Postfix: runs after original, can modify result
private static void MyPostfix(ref ReturnType __result)
{
    __result = modifiedValue;
}
```

## Relic/Power Hooks

Override these virtual methods in your RelicModel or PowerModel:

### Lifecycle
```csharp
public override async Task AfterObtained() { }
public override async Task AfterRemoved() { }
```

### Combat Flow
```csharp
public override async Task BeforeCombatStart() { }
public override async Task AfterCombatVictory(CombatRoom room) { }
public override async Task AfterCombatEnd(CombatRoom room) { }
```

### Turn Management
```csharp
public override async Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side, CombatState state) { }
public override async Task AfterTurnEnd(PlayerChoiceContext ctx, CombatSide side) { }
public override async Task AfterEnergyReset(Player player) { }
```

### Card Events
```csharp
public override async Task BeforeCardPlayed(CardPlay cardPlay) { }
public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay) { }
public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw) { }
public override async Task AfterCardDiscarded(PlayerChoiceContext ctx, CardModel card) { }
```

### Damage Events
```csharp
public override async Task AfterDamageGiven(...) { }
public override async Task AfterDamageReceived(...) { }
public override async Task AfterBlockGained(...) { }
```

### Modifiers (return modified values)
```csharp
public override decimal ModifyDamageAdditive(...) { return baseAmount + 2; }
public override decimal ModifyDamageMultiplicative(...) { return currentAmount * 1.5m; }
public override int ModifyEnergyCostInCombat(CardModel card, int currentCost) { ... }
```

## Cards That Apply Specific Effects

To detect if a card applies a status effect, check its DynamicVars:

```csharp
// Check if card applies vulnerability (with amount > 0)
if (card.DynamicVars.TryGetValue("VulnerablePower", out var vulnVar))
{
    if (vulnVar.BaseValue > 0)
    {
        // Card applies vulnerability
    }
}

// Common DynamicVar keys for effects:
// "VulnerablePower" - Vulnerability
// "WeakPower" - Weak
// "StrengthPower" - Strength
// "DexterityPower" - Dexterity
// "PoisonPower" - Poison
// "Block" - Block amount
// "Damage" - Damage amount
```

## Logging

```csharp
using MegaCrit.Sts2.Core.Logging;

Log.Info("Message");
Log.Debug("Debug message");
Log.Warn("Warning");
Log.Error("Error");
```

## Common Patterns

### Always-Active Mod (No Relic Required)
Subscribe to CombatManager events in ModLoaded():
```csharp
public static void ModLoaded()
{
    CombatManager.Instance.CombatSetUp += OnCombatSetUp;
    CombatManager.Instance.CombatEnded += OnCombatEnded;
}
```

### Tracking State Across Combat
```csharp
private static readonly Dictionary<Player, MyState> _playerStates = new();

private static void OnCombatSetUp(CombatState state)
{
    foreach (var player in state.Players)
    {
        _playerStates[player] = new MyState();
        // Subscribe to events...
    }
}

private static void OnCombatEnded(CombatRoom room)
{
    _playerStates.Clear();
}
```

## Deployment

### Mods Directory Location
- **macOS**: `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods/`
- **Windows**: `<SteamLibrary>/steamapps/common/Slay the Spire 2/mods/`

### Build Script (build.sh)
Create a build script in your mod folder:
```bash
#!/bin/bash
set -e

# Configuration
MOD_NAME="YourModName"
MODS_DIR="${MODS:-$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods}"
MOD_DIR="$MODS_DIR/$MOD_NAME"
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

echo "=== Building $MOD_NAME ==="

# Build C# project
cd "$SCRIPT_DIR"
dotnet build -c Release

# Find the built DLL
DLL_PATH="$SCRIPT_DIR/.godot/mono/temp/bin/Release/$MOD_NAME.dll"
if [ ! -f "$DLL_PATH" ]; then
    echo "Error: DLL not found at $DLL_PATH"
    exit 1
fi

# Create mod directory if needed
mkdir -p "$MOD_DIR"

# Copy DLL and manifest
cp "$DLL_PATH" "$MOD_DIR/"
cp "$SCRIPT_DIR/$MOD_NAME.json" "$MOD_DIR/"

echo "=== Build complete ==="
```

### Deploy Steps
1. Run `./build.sh` from your mod folder
2. Restart STS2 (or reload mods if available)
3. Enable the mod in the in-game mods menu

### Deployed Mod Structure
```
mods/
└── YourModName/
    ├── YourModName.dll      # Compiled code
    └── YourModName.json     # Manifest
```

## File Locations

- Decompiled source: `/Users/tom/code/sts2_modding/decompiled/`
- Example mod: `/Users/tom/code/sts2_modding/FirstMod/`
- API Reference: `/Users/tom/code/sts2_modding/API_REFERENCE.md`
- Modding Guide: `/Users/tom/code/sts2_modding/MODDING_GUIDE.md`
