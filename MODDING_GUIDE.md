# Slay the Spire 2 Modding Guide

Based on decompilation of `sts2.dll` (game version built with Godot 4.5 + C#/.NET 9.0)

## Table of Contents
- [Mod Structure](#mod-structure)
- [Creating a New Relic](#creating-a-new-relic)
- [Hook System](#hook-system)
- [Custom UI/Icons](#custom-uiicons)
- [Key Namespaces](#key-namespaces)

---

## Mod Structure

A mod consists of:
```
YourMod/
├── YourMod.dll          # Compiled C# code
├── YourMod.pck          # Godot resource pack (scenes, textures, etc.)
└── mod_manifest.json    # Required metadata
```

### mod_manifest.json
```json
{
  "pck_name": "YourMod",
  "name": "Your Mod Display Name",
  "author": "YourName",
  "description": "What your mod does",
  "version": "1.0.0"
}
```

### Entry Point
```csharp
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;

namespace YourMod;

[ModInitializer("Initialize")]
public static class YourMod
{
    public static void Initialize()
    {
        Log.Info("Your mod loaded!");
        // Register custom relics, cards, etc. here
    }
}
```

### Building
- Project must reference `sts2.dll` (the game's API)
- Target: .NET 9.0
- Uses HarmonyLib for patching (included in game)
- Output: `.dll` + `.pck` placed in `<GameDir>/MacOS/mods/YourMod/`

---

## Creating a New Relic

Relics extend `RelicModel` and must be registered via `ModelDb.Inject()`.

### Relic Lifecycle Hooks

| Method | When Called |
|--------|-------------|
| `AfterObtained()` | When player picks up the relic |
| `AfterRemoved()` | When relic is removed |
| `BeforeCombatStart()` | Before combat begins |
| `AfterCombatEnd(CombatRoom)` | After combat ends |
| `BeforeCardPlayed(CardPlay)` | Before a card is played |
| `AfterCardPlayed(PlayerChoiceContext, CardPlay)` | After a card is played |
| `AfterCardPlayedLate(PlayerChoiceContext, CardPlay)` | Late-phase after card played |
| `BeforeSideTurnStart(PlayerChoiceContext, CombatSide, CombatState)` | Before turn starts |
| `AfterTurnEnd(PlayerChoiceContext, CombatSide)` | After turn ends |
| `AfterShuffle(PlayerChoiceContext, Player)` | After deck shuffle |
| `AfterDamageGiven(...)` | After dealing damage |
| `AfterDamageReceived(...)` | After taking damage |
| `BeforeDeath(Creature)` | Before creature dies |

### Example: Custom Relic
```csharp
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;

namespace YourMod.Relics;

public sealed class MyCustomRelic : RelicModel
{
    private int _counter;

    // Required: Set rarity
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    // Optional: Show counter on relic
    public override bool ShowCounter => true;
    public override int DisplayAmount => _counter;

    // Hook: After a card is played
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // Only trigger for owner's attack cards
        if (cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Attack)
            return;

        _counter++;

        if (_counter >= 3)
        {
            _counter = 0;
            Flash(); // Visual feedback
            // Apply effect here using PowerCmd, DamageCmd, etc.
        }
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side, CombatState state)
    {
        if (side == Owner.Creature.Side)
            _counter = 0; // Reset at turn start
        return Task.CompletedTask;
    }
}
```

### Registering the Relic
```csharp
[ModInitializer("Initialize")]
public static class YourMod
{
    public static void Initialize()
    {
        // Register your relic type
        ModelDb.Inject(typeof(MyCustomRelic));

        // To add to a relic pool, you may need Harmony patches
    }
}
```

### Relic Properties

| Property | Type | Description |
|----------|------|-------------|
| `Owner` | `Player` | The player who owns this relic |
| `Status` | `RelicStatus` | Normal, Active, or Disabled |
| `DynamicVars` | `DynamicVarSet` | Variable values for descriptions |
| `IsWax` | `bool` | Wax variant (from ToyBox) |
| `IsMelted` | `bool` | Whether melted |
| `IsUsedUp` | `bool` | Whether consumed |
| `StackCount` | `int` | For stackable relics |

### Relic Visual Methods
- `Flash()` - Flash the relic icon
- `Flash(IEnumerable<Creature> targets)` - Flash with targets
- `UpdateDisplay()` - Refresh counter/status display
- `InvokeDisplayAmountChanged()` - Notify UI of counter change

---

## Hook System

The game uses `MegaCrit.Sts2.Core.Hooks.Hook` to dispatch events to all models (relics, powers, cards) that implement `ShouldReceiveCombatHooks => true`.

### Modifier Hooks (return modified values)

| Method | Purpose |
|--------|---------|
| `ModifyDamageAdditive(...)` | Add flat damage |
| `ModifyDamageMultiplicative(...)` | Multiply damage |
| `ModifyBlockAdditive(...)` | Add flat block |
| `ModifyBlockMultiplicative(...)` | Multiply block |
| `ModifyHandDraw(Player, decimal)` | Modify cards drawn |
| `ModifyHealAmount(Creature, decimal)` | Modify healing |
| `ModifyEnergyCostInCombat(...)` | Modify card energy cost |

### Conditional Hooks (return bool)

| Method | Purpose |
|--------|---------|
| `ShouldDie(Creature)` | Return `false` to prevent death |
| `ShouldPlay(CardModel, AutoPlayType)` | Return `false` to prevent card play |
| `ShouldDraw(Player, bool)` | Return `false` to prevent drawing |
| `ShouldClearBlock(Creature)` | Return `false` to prevent block clear |
| `ShouldGainGold(decimal, Player)` | Return `false` to prevent gold gain |

---

## Custom UI/Icons

### Adding Icons Above Creatures

The creature display system uses:
- `NCreatureStateDisplay` - Main container above creatures
- `NPowerContainer` - Shows power icons
- `NHealthBar` - Health/block display

To add custom icons, you can:

1. **Use the Power system** - Create a custom `PowerModel` that displays as an icon
2. **Harmony patch** - Patch `NCreatureStateDisplay` or `NPowerContainer` to add custom elements
3. **Create overlay nodes** - Add Godot Control nodes as children

### Example: Using Powers for Icons
```csharp
// Powers automatically display icons above creatures
public sealed class MyStatusPower : PowerModel
{
    public override string IconPath => "res://YourMod/icons/my_status.png";
    public override bool IsDebuff => false;

    // Power logic here...
}
```

### HoverTips (Tooltips)
```csharp
using MegaCrit.Sts2.Core.HoverTips;

// Create a tooltip
var tip = new HoverTip(title: "Effect Name", description: "What it does");

// Display tooltip on hover
NHoverTipSet.CreateAndShow(control, tip);
NHoverTipSet.Remove(control);
```

---

## Key Namespaces

| Namespace | Contents |
|-----------|----------|
| `MegaCrit.Sts2.Core.Modding` | `ModManager`, `ModInitializerAttribute` |
| `MegaCrit.Sts2.Core.Models` | `AbstractModel`, `ModelDb`, `RelicModel`, `CardModel`, `PowerModel` |
| `MegaCrit.Sts2.Core.Models.Relics` | All built-in relics (Kunai, PenNib, etc.) |
| `MegaCrit.Sts2.Core.Entities.Relics` | `RelicRarity`, `RelicStatus` |
| `MegaCrit.Sts2.Core.Entities.Cards` | `CardPlay`, `CardType`, `CardModel` |
| `MegaCrit.Sts2.Core.Entities.Creatures` | `Creature` |
| `MegaCrit.Sts2.Core.Entities.Players` | `Player` |
| `MegaCrit.Sts2.Core.Combat` | `CombatManager`, `CombatState`, `CombatSide` |
| `MegaCrit.Sts2.Core.Commands` | `PowerCmd`, `DamageCmd`, `BlockCmd`, `Cmd` |
| `MegaCrit.Sts2.Core.Hooks` | `Hook` (static class for event dispatch) |
| `MegaCrit.Sts2.Core.Logging` | `Log` (Info, Warn, Error) |
| `MegaCrit.Sts2.Core.Nodes.Combat` | UI nodes (NCreature, NPowerContainer, etc.) |
| `MegaCrit.Sts2.Core.HoverTips` | Tooltip system |

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

// Draw cards
await DrawCmd.Draw(player, count, context);

// Wait (for animations)
await Cmd.Wait(seconds);
```

---

## Localization

Place localization files at:
```
res://YourMod/localization/{language}/relics.json
```

Format:
```json
{
  "my_custom_relic.title": "My Relic",
  "my_custom_relic.description": "Every 3 Attacks, gain {Strength} Strength.",
  "my_custom_relic.flavor": "A mysterious artifact..."
}
```

---

## Mod Loading System

The game's `ModManager` handles mod loading automatically.

### Mod Sources
1. **Local Directory**: `<GameDir>/MacOS/mods/YourMod/`
2. **Steam Workshop**: Subscribed items are loaded automatically

### Loading Process
1. Game scans `mods/` directory for `.pck` files
2. Loads accompanying `YourMod.dll` if present
3. Reads `mod_manifest.json` from inside the PCK
4. **If `[ModInitializer]` attribute is found**: Calls that method
5. **If NO `[ModInitializer]` found**: Automatically calls `Harmony.PatchAll()` on your assembly

### Auto-Discovery
The game automatically discovers your mod's types via `ReflectionHelper.GetSubtypesInMods<T>()`:
- Any class extending `RelicModel`, `CardModel`, `PowerModel`, etc. is found
- `ModelDb.Inject()` registers them with the game database
- No manual listing required!

---

## Harmony Patching

Harmony is built into the game! You can patch any game method.

### Option 1: Automatic (No ModInitializer)
If you don't use `[ModInitializer]`, the game calls `Harmony.PatchAll()` automatically:

```csharp
using HarmonyLib;

namespace YourMod;

[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.Flash))]
public static class RelicFlashPatch
{
    public static void Prefix(RelicModel __instance)
    {
        Log.Info($"Relic flashing: {__instance.Id}");
    }
}
```

### Option 2: Manual (With ModInitializer)
If you use `[ModInitializer]`, you must call Harmony yourself:

```csharp
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace YourMod;

[ModInitializer("Initialize")]
public static class YourMod
{
    private static Harmony? _harmony;

    public static void Initialize()
    {
        _harmony = new Harmony("yourname.yourmod");
        _harmony.PatchAll();

        // Register models, etc.
        ModelDb.Inject(typeof(MyCustomRelic));
    }
}
```

### Harmony Patch Types
- `[HarmonyPrefix]` - Run before original method
- `[HarmonyPostfix]` - Run after original method
- `[HarmonyTranspiler]` - Modify IL code directly

---

## Creating Custom Powers (Status Icons)

Powers display as icons above creatures and are the easiest way to show custom icons.

### PowerModel Base Class

```csharp
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;

namespace YourMod.Powers;

public sealed class MyCustomPower : PowerModel
{
    // Required: Buff or Debuff
    public override PowerType Type => PowerType.Buff;

    // Required: How stacks work (Counter, Duration, or Intensity)
    public override PowerStackType StackType => PowerStackType.Counter;

    // Optional: Allow negative values (like Strength can go negative)
    public override bool AllowNegative => false;

    // Icons are loaded from: res://images/powers/{id}.png
    // Or from atlas: res://images/atlases/power_atlas.sprites/{id}.tres

    // Example: Modify damage dealt
    public override decimal ModifyDamageAdditive(
        Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer) return 0m;
        if (!props.IsPoweredAttack()) return 0m;
        return Amount; // Add power's amount to damage
    }
}
```

### PowerStackType Values
- `Counter` - Stacks add up (like Strength: 3 + 2 = 5)
- `Duration` - Decrements each turn (like Vulnerable)
- `Intensity` - Special stacking behavior

### Applying Powers
```csharp
// Apply power to a creature
await PowerCmd.Apply<MyCustomPower>(creature, amount: 3, applier, cardSource);

// Set exact amount (instead of adding)
await PowerCmd.SetAmount<MyCustomPower>(creature, amount: 5, applier, cardSource);

// Remove power
await PowerCmd.Remove<MyCustomPower>(creature);

// Decrement by 1
await PowerCmd.Decrement(power);
```

---

## Attack Command Builder

Damage is dealt through a fluent builder pattern:

```csharp
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;

// Basic attack from a card
await DamageCmd.Attack(damagePerHit: 6)
    .FromCard(card)
    .Targeting(target)
    .WithHitCount(3)
    .WithHitFx(vfx: "res://vfx/slash.tscn", sfx: "event:/sfx/hit")
    .Execute(choiceContext);

// Attack all enemies
await DamageCmd.Attack(10)
    .FromCard(card)
    .TargetingAllOpponents(combatState)
    .Execute(choiceContext);

// Random target attacks
await DamageCmd.Attack(5)
    .FromCard(card)
    .TargetingRandomOpponents(combatState, allowDuplicates: true)
    .WithHitCount(5)
    .Execute(choiceContext);
```

### AttackCommand Methods
| Method | Description |
|--------|-------------|
| `.FromCard(card)` | Set card as damage source |
| `.FromMonster(monster)` | Set monster as attacker |
| `.Targeting(creature)` | Single target |
| `.TargetingAllOpponents(state)` | Hit all enemies |
| `.TargetingRandomOpponents(state)` | Random targeting |
| `.WithHitCount(n)` | Multi-hit attack |
| `.Unpowered()` | Ignore Strength/damage modifiers |
| `.WithAttackerAnim(name, delay)` | Custom attack animation |
| `.WithHitFx(vfx, sfx)` | Hit visual/sound effects |
| `.Execute(context)` | Execute the attack |

---

## Decompiled Source Location

Full decompiled source is at:
```
~/code/sts2_modding/decompiled/
```

Key files to reference:
- `MegaCrit.Sts2.Core.Models/RelicModel.cs` - Base relic class
- `MegaCrit.Sts2.Core.Models/AbstractModel.cs` - All hook methods (~100 virtual methods!)
- `MegaCrit.Sts2.Core.Models/PowerModel.cs` - Power/status effect base class
- `MegaCrit.Sts2.Core.Hooks/Hook.cs` - Hook dispatch system
- `MegaCrit.Sts2.Core.Commands/*.cs` - All game commands
- `MegaCrit.Sts2.Core.Commands.Builders/AttackCommand.cs` - Attack builder
- `MegaCrit.Sts2.Core.Modding/ModManager.cs` - How mods are loaded
- `MegaCrit.Sts2.Core.Models.Relics/*.cs` - Example relic implementations
- `MegaCrit.Sts2.Core.Models.Powers/*.cs` - Example power implementations

---

## Worked Example: Adding a Custom Icon Above a Creature

This example shows how to add a custom graphical element (like the Vulnerable icon) positioned **above** a creature's body. This is separate from the existing power system which displays icons **below** creatures.

### Understanding the UI Hierarchy

```
NCombatRoom (the combat scene)
├── AllyContainer / EnemyContainer
│   └── NCreature (one per creature)
│       ├── Visuals (NCreatureVisuals - the sprite/animation)
│       │   ├── Body (the actual sprite)
│       │   ├── Bounds (Control - hitbox sizing)
│       │   └── VfxSpawnPosition (Marker2D - center point)
│       ├── Hitbox (Control - clickable area)
│       └── _stateDisplay (NCreatureStateDisplay - powers shown BELOW)
└── CombatVfxContainer (where custom overlays should go)
```

### Getting a Creature's Screen Position

```csharp
using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

// Get the NCreature UI node for a creature
NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);

if (creatureNode != null)
{
    // Key position methods:
    Vector2 topOfCreature = creatureNode.GetTopOfHitbox();     // Above the creature
    Vector2 bottomOfCreature = creatureNode.GetBottomOfHitbox(); // Base of creature
    Vector2 center = creatureNode.VfxSpawnPosition;             // Center point
    Vector2 globalPos = ((Control)creatureNode).GlobalPosition; // Bottom-center
}
```

### Creating a Custom Icon Overlay

```csharp
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

public class CustomIconOverlay
{
    private TextureRect? _iconNode;
    private Creature _target;

    public void ShowIconAboveCreature(Creature creature, string iconPath)
    {
        _target = creature;

        // Get the creature's UI node
        NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (creatureNode == null) return;

        // Load the texture (from your mod's PCK or game resources)
        // For game resources: "res://images/powers/vulnerable.png"
        // For mod resources: "res://YourMod/icons/my_icon.png"
        Texture2D texture = ResourceLoader.Load<Texture2D>(iconPath);

        // Create a TextureRect to display the icon
        _iconNode = new TextureRect();
        _iconNode.Texture = texture;
        _iconNode.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        _iconNode.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        _iconNode.CustomMinimumSize = new Vector2(48, 48); // Icon size

        // Position above the creature
        Vector2 topPos = creatureNode.GetTopOfHitbox();
        _iconNode.GlobalPosition = topPos + new Vector2(-24, -60); // Centered, offset up

        // Add to the combat VFX container (overlays on top of creatures)
        NCombatRoom.Instance.CombatVfxContainer.AddChild(_iconNode);
    }

    public void UpdatePosition()
    {
        // Call this each frame if the creature can move
        NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(_target);
        if (creatureNode != null && _iconNode != null)
        {
            Vector2 topPos = creatureNode.GetTopOfHitbox();
            _iconNode.GlobalPosition = topPos + new Vector2(-24, -60);
        }
    }

    public void Hide()
    {
        _iconNode?.QueueFree();
        _iconNode = null;
    }
}
```

### Using the Overlay from a Relic

```csharp
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Entities.Relics;
using System.Threading.Tasks;

public sealed class IconDisplayRelic : RelicModel
{
    private CustomIconOverlay? _overlay;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override Task BeforeCombatStart()
    {
        // Show vulnerable icon above our creature at combat start
        _overlay = new CustomIconOverlay();
        _overlay.ShowIconAboveCreature(
            Owner.Creature,
            "res://images/powers/vulnerable.png"  // Use existing game icon
        );
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _overlay?.Hide();
        _overlay = null;
        return Task.CompletedTask;
    }
}
```

### Alternative: Using a Scene from Your PCK

For more complex UI (animations, multiple elements), create a Godot scene:

```csharp
// In your mod's PCK, create: res://YourMod/scenes/custom_indicator.tscn
// Then load and instantiate it:

PackedScene scene = ResourceLoader.Load<PackedScene>("res://YourMod/scenes/custom_indicator.tscn");
Node2D indicator = scene.Instantiate<Node2D>();

NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);
if (creatureNode != null)
{
    indicator.GlobalPosition = creatureNode.GetTopOfHitbox() + new Vector2(0, -50);
    NCombatRoom.Instance.CombatVfxContainer.AddChild(indicator);
}
```

### Using VfxCmd for One-Shot Effects

For temporary visual effects (not persistent icons), use the built-in VFX system:

```csharp
using MegaCrit.Sts2.Core.Commands;

// Play a VFX on a creature's center
VfxCmd.PlayOnCreatureCenter(creature, "vfx/vfx_heal");

// Play at creature's base
VfxCmd.PlayOnCreature(creature, "vfx/vfx_block");

// Play at a specific position
VfxCmd.PlayVfx(position, "vfx/vfx_attack_slash");
```

---

## Multiplayer Data Access

STS2 is a multiplayer game. Here's what data you can access about other players.

### Identifying the Local Player

```csharp
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

// Check if a player is the local player
bool isMe = LocalContext.IsMe(player);

// Check if a card belongs to the local player
bool isMine = LocalContext.IsMine(card);

// Get the local player's network ID
ulong? myNetId = LocalContext.NetId;
```

### Accessing All Players in Combat

```csharp
using MegaCrit.Sts2.Core.Combat;

// Get combat state (available from most hooks)
CombatState combatState = creature.CombatState;

// All players in combat
IReadOnlyList<Player> allPlayers = combatState.Players;

// Get a specific player by network ID
Player? otherPlayer = combatState.GetPlayer(playerId);

// All player creatures
IReadOnlyList<Creature> playerCreatures = combatState.PlayerCreatures;
```

### Can You Access Other Players' Card Hands?

**Yes, the data is accessible in memory:**

```csharp
foreach (Player player in combatState.Players)
{
    PlayerCombatState pcs = player.PlayerCombatState;

    // Cards in hand
    IEnumerable<CardModel> handCards = pcs.Hand.Cards;

    // Draw pile
    IEnumerable<CardModel> drawPile = pcs.DrawPile.Cards;

    // Discard pile
    IEnumerable<CardModel> discardPile = pcs.DiscardPile.Cards;

    // Exhaust pile
    IEnumerable<CardModel> exhaustPile = pcs.ExhaustPile.Cards;

    // All cards owned by this player
    IEnumerable<CardModel> allCards = pcs.AllCards;
}
```

### Important: UI Visibility vs Data Access

**The data exists and is accessible, but the UI hides certain information from other players:**

```csharp
// From PowerModel.cs - powers on other players are hidden in the UI
public bool IsVisible
{
    get
    {
        // Visible if: no target, OR target is me, OR target is an enemy
        if (Target == null || LocalContext.IsMe(Target) || Target.IsEnemy)
        {
            return IsVisibleInternal;
        }
        return false;  // Hidden: power on another player
    }
}
```

**What this means for modding:**
- You CAN read other players' hands, powers, and game state in code
- The default UI only shows your own cards and powers
- Powers/debuffs on OTHER players' creatures are hidden by default
- If you want to display custom info about other players, you'll need to create custom UI

### Example: Relic that Reacts to Ally's Cards

```csharp
public sealed class TeamworkRelic : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // Check if ANY player (not just owner) played an Attack
        if (cardPlay.Card.Type == CardType.Attack)
        {
            // Get the player who played the card
            Player cardPlayer = cardPlay.Card.Owner;

            // React even if it wasn't our card
            if (!LocalContext.IsMe(cardPlayer))
            {
                // Ally played an Attack - give ourselves 1 Strength
                await PowerCmd.Apply<StrengthPower>(
                    Owner.Creature,
                    amount: 1,
                    applier: Owner.Creature,
                    cardSource: null
                );
                Flash();
            }
        }
    }
}
```

### Network Synchronization

The game handles synchronization automatically through:
- `PlayerChoiceContext` - wraps player decisions for network sync
- Commands (`PowerCmd`, `DamageCmd`, etc.) - execute on all clients
- `RunState.Rng` - seeded random for deterministic results across clients

When writing mods, use the provided command system rather than directly modifying state to ensure multiplayer compatibility.
