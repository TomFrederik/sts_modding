# Slay the Spire 2 API Reference

A comprehensive technical reference for modding Slay the Spire 2. This document provides detailed method signatures, class hierarchies, and usage patterns.

Based on decompilation of `sts2.dll` (Godot 4.5 + C#/.NET 9.0)

---

## Table of Contents

1. [Namespace Overview](#1-namespace-overview)
2. [Core Classes](#2-core-classes)
3. [Hook Methods](#3-hook-methods)
4. [Command System](#4-command-system)
5. [UI System](#5-ui-system)
6. [Entity System](#6-entity-system)
7. [Useful Utilities](#7-useful-utilities)

---

## 1. Namespace Overview

### Core Model System
| Namespace | Description |
|-----------|-------------|
| `MegaCrit.Sts2.Core.Models` | Base model classes: `AbstractModel`, `ModelDb`, `ModelId` |
| `MegaCrit.Sts2.Core.Models.Relics` | All built-in relic implementations |
| `MegaCrit.Sts2.Core.Models.Powers` | All built-in power implementations |
| `MegaCrit.Sts2.Core.Models.Cards` | Character-specific card implementations |
| `MegaCrit.Sts2.Core.Models.Monsters` | All monster/enemy implementations |
| `MegaCrit.Sts2.Core.Models.Characters` | Character class definitions (Ironclad, Silent, etc.) |
| `MegaCrit.Sts2.Core.Models.Acts` | Act definitions (Overgrowth, Hive, Glory, Underdocks) |
| `MegaCrit.Sts2.Core.Models.Events` | Event encounter definitions |
| `MegaCrit.Sts2.Core.Models.Orbs` | Orb implementations (Lightning, Frost, Dark, Plasma) |
| `MegaCrit.Sts2.Core.Models.Modifiers` | Run modifier implementations |

### Entity System
| Namespace | Description |
|-----------|-------------|
| `MegaCrit.Sts2.Core.Entities.Creatures` | `Creature` - runtime creature instances |
| `MegaCrit.Sts2.Core.Entities.Players` | `Player` - runtime player instances |
| `MegaCrit.Sts2.Core.Entities.Cards` | `CardPlay`, `CardPile`, `PileType` |
| `MegaCrit.Sts2.Core.Entities.Relics` | `RelicRarity`, `RelicStatus` enums |
| `MegaCrit.Sts2.Core.Entities.Potions` | Potion management |
| `MegaCrit.Sts2.Core.Entities.Powers` | Power management utilities |

### Commands (Game Actions)
| Namespace | Description |
|-----------|-------------|
| `MegaCrit.Sts2.Core.Commands` | Core commands: `Cmd`, `PowerCmd`, `CardCmd`, `CardPileCmd`, `CreatureCmd`, `SfxCmd`, `VfxCmd`, `ThinkCmd` |
| `MegaCrit.Sts2.Core.Commands.Builders` | `AttackCommand` - fluent attack builder |

### Combat System
| Namespace | Description |
|-----------|-------------|
| `MegaCrit.Sts2.Core.Combat` | `CombatManager`, `CombatState`, `CombatSide`, `DamageResult` |
| `MegaCrit.Sts2.Core.Hooks` | `Hook` - static hook dispatcher |
| `MegaCrit.Sts2.Core.Context` | `PlayerChoiceContext` for async player interactions |

### UI/Nodes
| Namespace | Description |
|-----------|-------------|
| `MegaCrit.Sts2.Core.Nodes` | Base node classes, `NGame`, `NRun` |
| `MegaCrit.Sts2.Core.Nodes.Combat` | `NCombatRoom`, `NCreature`, `NPowerContainer`, `NHealthBar` |
| `MegaCrit.Sts2.Core.Nodes.Cards` | `NCard`, `NPlayerHand`, `NCardPlayQueue` |
| `MegaCrit.Sts2.Core.Nodes.Rooms` | Room-specific UI nodes |
| `MegaCrit.Sts2.Core.Nodes.Vfx` | Visual effect nodes |

### Utilities
| Namespace | Description |
|-----------|-------------|
| `MegaCrit.Sts2.Core.Logging` | `Log` - logging utilities |
| `MegaCrit.Sts2.Core.Helpers` | Helper utilities |
| `MegaCrit.Sts2.Core.Random` | `Rng` - deterministic random |
| `MegaCrit.Sts2.Core.Localization` | `LocString` - localization |
| `MegaCrit.Sts2.Core.HoverTips` | Tooltip system |
| `MegaCrit.Sts2.Core.ValueProps` | `ValueProp` flags for damage/block |
| `MegaCrit.Sts2.Core.Modding` | `ModManager`, `ModInitializerAttribute` |

---

## 2. Core Classes

### AbstractModel

The base class for all game content models. Located in `MegaCrit.Sts2.Core.Models`.

```csharp
public abstract class AbstractModel
{
    // Identity
    public ModelId Id { get; }
    public bool IsMutable { get; }

    // Localization
    public virtual LocString Title { get; }
    public virtual LocString Description { get; }
    public virtual LocString Flavor { get; }

    // Dynamic Variables (for description templating)
    public DynamicVarSet DynamicVars { get; }
    protected virtual IEnumerable<DynamicVar> CanonicalVars { get; }

    // Hooks enabled
    public virtual bool ShouldReceiveCombatHooks => false;
    public virtual bool ShouldReceiveRunHooks => false;

    // Create mutable copy
    public T ToMutable<T>() where T : AbstractModel;
    public AbstractModel ToMutable();

    // Validation
    public void AssertMutable();
    public void AssertImmutable();
}
```

### RelicModel

Base class for all relics. Extends `AbstractModel`.

```csharp
public class RelicModel : AbstractModel
{
    // Owner
    public Player Owner { get; internal set; }

    // Rarity (MUST override)
    public abstract RelicRarity Rarity { get; }

    // Counter display
    public virtual bool ShowCounter => false;
    public virtual int DisplayAmount => 0;

    // Status
    public RelicStatus Status { get; set; }  // Normal, Active, Disabled
    public bool IsWax { get; set; }          // From ToyBox
    public bool IsMelted { get; set; }       // Melted wax relic
    public bool IsUsedUp { get; set; }       // Consumed relic
    public int StackCount { get; set; }      // For stackable relics

    // Visual paths
    public virtual string IconPath { get; }
    public virtual string FlashSfx => "event:/sfx/relic_proc";
    public virtual bool ShouldFlashOnPlayer => true;

    // Add to pools
    public virtual bool AddsPet => false;

    // Lifecycle
    public virtual Task AfterObtained();
    public virtual Task AfterRemoved();

    // Visual feedback
    public void Flash();
    public void Flash(IEnumerable<Creature> targets);
    public void UpdateDisplay();
    public void InvokeDisplayAmountChanged();
}
```

**RelicRarity enum:**
```csharp
public enum RelicRarity { Starter, Common, Uncommon, Rare, Boss, Shop, Event }
```

**RelicStatus enum:**
```csharp
public enum RelicStatus { Normal, Active, Disabled }
```

### CardModel

Base class for all cards.

```csharp
public class CardModel : AbstractModel
{
    // Owner
    public Player Owner { get; internal set; }

    // Type and properties
    public virtual CardType Type { get; }        // Attack, Skill, Power, Status, Curse
    public virtual CardRarity Rarity { get; }    // Common, Uncommon, Rare, Basic, Special
    public virtual int BaseCost { get; }
    public virtual int EnergyCost { get; }       // Runtime cost (may be modified)

    // Targeting
    public virtual CardTarget TargetType { get; }  // SingleEnemy, AllEnemies, Self, None

    // Keywords
    public virtual bool Exhaust { get; }
    public virtual bool Ethereal { get; }
    public virtual bool Innate { get; }
    public virtual bool Retain { get; }
    public virtual bool Unplayable { get; }

    // Runtime state
    public CardPile Pile { get; }
    public CombatState CombatState { get; }
    public bool ExhaustOnNextPlay { get; set; }
    public CardModel DeckVersion { get; set; }   // Link to deck copy

    // Upgrades
    public int UpgradeLevel { get; }
    public bool IsUpgraded => UpgradeLevel > 0;
    public virtual bool CanUpgrade { get; }

    // Core play method (OVERRIDE THIS)
    public virtual Task Play(PlayerChoiceContext ctx, Creature? target, CardPlay play);

    // Upgrade effect
    protected virtual void OnUpgrade();

    // Visual
    public virtual string PortraitPath { get; }
    public virtual IEnumerable<string> AllPortraitPaths { get; }
}
```

**CardType enum:**
```csharp
public enum CardType { Attack, Skill, Power, Status, Curse }
```

### PowerModel

Base class for powers/buffs/debuffs.

```csharp
public class PowerModel : AbstractModel
{
    // Owner creature
    public Creature Owner { get; internal set; }

    // Stack amount
    public int Amount { get; internal set; }
    public int AmountOnTurnStart { get; internal set; }

    // Stack behavior
    public virtual PowerStackType StackType { get; }  // Intensity, Duration, Counter
    public virtual bool AllowNegative => false;
    public virtual bool IsInstanced => false;

    // Display
    public virtual bool IsDebuff { get; }
    public virtual string IconPath { get; }
    public virtual string BigIconPath { get; }
    public virtual int Priority => 0;           // Sort order in power bar

    // Special flags
    public virtual bool OwnerIsSecondaryEnemy => false;  // Minion marker

    // Lifecycle
    public virtual Task AfterApplied(Creature target, Creature? applier, CardModel? source);
    public virtual Task AfterIncreased(int change, Creature? applier, CardModel? source);
    public virtual Task AfterDecreased(Creature? applier, CardModel? source);
    public virtual Task AfterRemoved(Creature creature);

    // Override to modify when this power should be removed on death
    public virtual bool ShouldPowerBeRemovedAfterOwnerDeath() => true;
}
```

**PowerStackType enum:**
```csharp
public enum PowerStackType { Intensity, Duration, Counter }
```

### ModelDb

Static database for all game content. Used for registration and lookup.

```csharp
public static class ModelDb
{
    // Initialization
    public static void Init();
    public static void Preload();

    // Mod injection (USE THIS TO REGISTER CUSTOM CONTENT)
    public static void Inject(Type type);
    public static void Remove(Type type);

    // Lookup by type
    public static T Card<T>() where T : CardModel;
    public static T Relic<T>() where T : RelicModel;
    public static T Power<T>() where T : PowerModel;
    public static T Monster<T>() where T : MonsterModel;
    public static T Character<T>() where T : CharacterModel;
    public static T Event<T>() where T : EventModel;
    public static T Potion<T>() where T : PotionModel;
    public static T Act<T>() where T : ActModel;
    public static T Orb<T>() where T : OrbModel;
    public static T Modifier<T>() where T : ModifierModel;

    // Lookup by ID
    public static T GetById<T>(ModelId id) where T : AbstractModel;
    public static T? GetByIdOrNull<T>(ModelId id) where T : AbstractModel;
    public static bool Contains(Type type);

    // ID generation
    public static ModelId GetId<T>() where T : AbstractModel;
    public static ModelId GetId(Type type);

    // Collections
    public static IEnumerable<CardModel> AllCards { get; }
    public static IEnumerable<RelicModel> AllRelics { get; }
    public static IEnumerable<PotionModel> AllPotions { get; }
    public static IEnumerable<PowerModel> AllPowers { get; }
    public static IEnumerable<CharacterModel> AllCharacters { get; }
    public static IEnumerable<EncounterModel> AllEncounters { get; }
    public static IEnumerable<EventModel> AllEvents { get; }
    public static IEnumerable<ActModel> Acts { get; }
}
```

---

## 3. Hook Methods

All hooks are virtual methods on `AbstractModel`. Override them to respond to game events.

### Combat Flow Hooks

```csharp
// Combat lifecycle
public virtual Task BeforeCombatStart();
public virtual Task BeforeCombatStartLate();
public virtual Task AfterCombatEnd(CombatRoom room);
public virtual Task AfterCombatVictoryEarly(CombatRoom room);
public virtual Task AfterCombatVictory(CombatRoom room);

// Turn lifecycle
public virtual Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side, CombatState state);
public virtual Task AfterSideTurnStart(CombatSide side, CombatState combatState);
public virtual Task BeforeTurnEndVeryEarly(PlayerChoiceContext ctx, CombatSide side);
public virtual Task BeforeTurnEndEarly(PlayerChoiceContext ctx, CombatSide side);
public virtual Task BeforeTurnEnd(PlayerChoiceContext ctx, CombatSide side);
public virtual Task BeforeTurnEndLate(PlayerChoiceContext ctx, CombatSide side);
public virtual Task AfterTurnEnd(PlayerChoiceContext ctx, CombatSide side);
public virtual Task AfterTurnEndLate(PlayerChoiceContext ctx, CombatSide side);

// Player turn specific
public virtual Task AfterPlayerTurnStartEarly(PlayerChoiceContext ctx, Player player);
public virtual Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player);
public virtual Task AfterPlayerTurnStartLate(PlayerChoiceContext ctx, Player player);
public virtual Task BeforePlayPhaseStart(PlayerChoiceContext ctx, Player player);
```

### Card Hooks

```csharp
// Card play
public virtual Task BeforeCardPlayed(CardPlay cardPlay);
public virtual Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay);
public virtual Task AfterCardPlayedLate(PlayerChoiceContext ctx, CardPlay cardPlay);
public virtual Task BeforeCardAutoPlayed(CardModel card, Creature? target, AutoPlayType type);

// Card movement
public virtual Task AfterCardDrawnEarly(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw);
public virtual Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw);
public virtual Task AfterCardDiscarded(PlayerChoiceContext ctx, CardModel card);
public virtual Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal);
public virtual Task AfterCardRetained(CardModel card);
public virtual Task AfterCardChangedPiles(CardModel card, PileType oldPile, AbstractModel? source);
public virtual Task AfterCardChangedPilesLate(CardModel card, PileType oldPile, AbstractModel? source);

// Card generation and removal
public virtual Task AfterCardEnteredCombat(CardModel card);
public virtual Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer);
public virtual Task BeforeCardRemoved(CardModel card);

// Shuffle
public virtual Task AfterShuffle(PlayerChoiceContext ctx, Player shuffler);
public virtual Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, CombatState state);
public virtual Task BeforeHandDrawLate(Player player, PlayerChoiceContext ctx, CombatState state);
public virtual Task AfterHandEmptied(PlayerChoiceContext ctx, Player player);
public virtual Task BeforeFlush(PlayerChoiceContext ctx, Player player);
public virtual Task BeforeFlushLate(PlayerChoiceContext ctx, Player player);
```

### Damage and Block Hooks

```csharp
// Damage dealing
public virtual Task BeforeAttack(AttackCommand command);
public virtual Task AfterAttack(AttackCommand command);
public virtual Task BeforeDamageReceived(PlayerChoiceContext ctx, Creature target, decimal amount,
    ValueProp props, Creature? dealer, CardModel? cardSource);
public virtual Task AfterDamageGiven(PlayerChoiceContext ctx, Creature? dealer, DamageResult results,
    ValueProp props, Creature target, CardModel? cardSource);
public virtual Task AfterDamageReceived(PlayerChoiceContext ctx, Creature target, DamageResult result,
    ValueProp props, Creature? dealer, CardModel? cardSource);
public virtual Task AfterDamageReceivedLate(PlayerChoiceContext ctx, Creature target, DamageResult result,
    ValueProp props, Creature? dealer, CardModel? cardSource);

// Block
public virtual Task BeforeBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource);
public virtual Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource);
public virtual Task AfterBlockBroken(Creature creature);
public virtual Task AfterBlockCleared(Creature creature);
```

### Power Hooks

```csharp
// Power changes
public virtual Task BeforePowerAmountChanged(PowerModel power, decimal amount,
    Creature target, Creature? applier, CardModel? cardSource);
public virtual Task AfterPowerAmountChanged(PowerModel power, decimal amount,
    Creature? applier, CardModel? cardSource);
```

### Creature Lifecycle Hooks

```csharp
// Death
public virtual Task BeforeDeath(Creature creature);
public virtual Task AfterDeath(PlayerChoiceContext ctx, Creature creature,
    bool wasRemovalPrevented, float deathAnimLength);
public virtual Task AfterDiedToDoom(PlayerChoiceContext ctx, IReadOnlyList<Creature> creatures);
public virtual Task AfterPreventingDeath(Creature creature);

// HP changes
public virtual Task AfterCurrentHpChanged(Creature creature, decimal delta);

// Creature management
public virtual Task AfterCreatureAddedToCombat(Creature creature);
public virtual Task AfterOstyRevived(Creature osty);
```

### Energy and Resource Hooks

```csharp
public virtual Task AfterEnergyReset(Player player);
public virtual Task AfterEnergyResetLate(Player player);
public virtual Task AfterEnergySpent(CardModel card, int amount);
public virtual Task AfterForge(decimal amount, Player forger, AbstractModel? source);
public virtual Task AfterStarsGained(int amount, Player gainer);
public virtual Task AfterStarsSpent(int amount, Player spender);
public virtual Task AfterSummon(PlayerChoiceContext ctx, Player summoner, decimal amount);
```

### Orb Hooks

```csharp
public virtual Task AfterOrbChanneled(PlayerChoiceContext ctx, Player player, OrbModel orb);
public virtual Task AfterOrbEvoked(PlayerChoiceContext ctx, OrbModel orb, IEnumerable<Creature> targets);
```

### Potion Hooks

```csharp
public virtual Task BeforePotionUsed(PotionModel potion, Creature? target);
public virtual Task AfterPotionUsed(PotionModel potion, Creature? target);
public virtual Task AfterPotionProcured(PotionModel potion);
public virtual Task AfterPotionDiscarded(PotionModel potion);
```

### Room and Map Hooks

```csharp
public virtual Task BeforeRoomEntered(AbstractRoom room);
public virtual Task AfterRoomEntered(AbstractRoom room);
public virtual Task AfterMapGenerated(ActMap map, int actIndex);
public virtual Task AfterActEntered();
```

### Reward Hooks

```csharp
public virtual Task BeforeRewardsOffered(Player player, IReadOnlyList<Reward> rewards);
public virtual Task AfterRewardTaken(Player player, Reward reward);
public virtual Task AfterItemPurchased(Player player, MerchantEntry itemPurchased, int goldSpent);
public virtual Task AfterGoldGained(Player player);
```

### Rest Site Hooks

```csharp
public virtual Task AfterRestSiteHeal(Player player, bool isMimicked);
public virtual Task AfterRestSiteSmith(Player player);
```

### Modifier Hooks (Return Modified Values)

These hooks modify game values. Return the modified value.

```csharp
// Damage modification
public virtual decimal ModifyDamageAdditive(Creature target, Creature? dealer,
    decimal baseAmount, ValueProp props, CardModel? cardSource, CardPreviewMode previewMode);
public virtual decimal ModifyDamageMultiplicative(Creature target, Creature? dealer,
    decimal currentAmount, ValueProp props, CardModel? cardSource, CardPreviewMode previewMode);

// Block modification
public virtual decimal ModifyBlockAdditive(Creature creature, decimal baseAmount,
    ValueProp props, CardModel? cardSource, CardPlay? cardPlay);
public virtual decimal ModifyBlockMultiplicative(Creature creature, decimal currentAmount,
    ValueProp props, CardModel? cardSource, CardPlay? cardPlay);

// HP modification
public virtual decimal ModifyHpLostBeforeOsty(Creature target, decimal amount,
    ValueProp props, Creature? dealer, CardModel? cardSource);
public virtual decimal ModifyHpLostAfterOsty(Creature target, decimal amount,
    ValueProp props, Creature? dealer, CardModel? cardSource);
public virtual decimal ModifyHealAmount(Creature creature, decimal amount);

// Card modification
public virtual int ModifyEnergyCostInCombat(CardModel card, int currentCost);
public virtual decimal ModifyHandDraw(Player player, decimal currentDraw);
public virtual int ModifyCardPlayCount(CardModel card, int currentCount);

// Power modification
public virtual decimal ModifyPowerAmountGiven(PowerModel power, decimal amount,
    Creature target, Creature? applier, CardModel? cardSource);
public virtual decimal ModifyPowerAmountReceived(PowerModel power, decimal amount,
    Creature? applier, CardModel? cardSource);

// Attack modification
public virtual decimal ModifyAttackHitCount(AttackCommand command, decimal currentHitCount);
```

### Conditional Hooks (Return Boolean)

Return `false` to prevent the action.

```csharp
public virtual bool ShouldDie(Creature creature);
public virtual bool ShouldPlay(CardModel card, AutoPlayType type);
public virtual bool ShouldDraw(Player player, bool fromHandDraw);
public virtual bool ShouldClearBlock(Creature creature);
public virtual bool ShouldGainGold(decimal amount, Player player);
public virtual bool ShouldAddToDeck(CardModel card);
public virtual bool ShouldAllowHitting(Creature creature);
public virtual bool ShouldPowerBeRemovedOnDeath(PowerModel power);
```

---

## 4. Command System

Commands are static classes that perform game actions. Always `await` them.

### Cmd (Core Utilities)

```csharp
public static class Cmd
{
    // Timing
    public static Task Wait(float seconds);
    public static Task CustomScaledWait(float fastSeconds, float standardSeconds);

    // Frame waiting
    public static Task WaitForNextFrame();
}
```

### PowerCmd

```csharp
public static class PowerCmd
{
    // Apply power to creature
    public static Task Apply<T>(Creature target, int amount, Creature? applier, CardModel? cardSource)
        where T : PowerModel;
    public static Task Apply(PowerModel power, Creature target, int amount,
        Creature? applier, CardModel? cardSource);

    // Remove power
    public static Task Remove<T>(Creature creature) where T : PowerModel;
    public static Task Remove(Creature creature, PowerModel power);

    // Modify power amount
    public static Task Increase<T>(Creature creature, int amount) where T : PowerModel;
    public static Task Decrease<T>(Creature creature, int amount) where T : PowerModel;
}
```

### CreatureCmd

```csharp
public static class CreatureCmd
{
    // Damage (use AttackCommand for attack cards)
    public static Task<IEnumerable<DamageResult>> Damage(PlayerChoiceContext ctx,
        Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource);
    public static Task<IEnumerable<DamageResult>> Damage(PlayerChoiceContext ctx,
        IEnumerable<Creature> targets, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource);

    // Block
    public static Task<decimal> GainBlock(Creature creature, decimal amount,
        ValueProp props, CardPlay? cardPlay, bool fast = false);
    public static Task LoseBlock(Creature creature, decimal amount);

    // HP
    public static Task Heal(Creature creature, decimal amount, bool playAnim = true);
    public static Task SetCurrentHp(Creature creature, decimal amount);
    public static Task GainMaxHp(Creature creature, decimal amount);
    public static Task LoseMaxHp(PlayerChoiceContext ctx, Creature creature,
        decimal amount, bool isFromCard);
    public static Task SetMaxHp(Creature creature, decimal amount);

    // Combat
    public static Task Kill(Creature creature, bool force = false);
    public static Task Kill(IReadOnlyCollection<Creature> creatures, bool force = false);
    public static Task Escape(Creature creature, bool removeCreatureNode = true);
    public static Task Stun(Creature creature, string? nextMoveId = null);

    // Animation
    public static Task TriggerAnim(Creature creature, string triggerName, float waitTime);

    // Creature spawning
    public static Task<Creature> Add<T>(CombatState combatState, string? slotName = null)
        where T : MonsterModel;
    public static Task<Creature> Add(MonsterModel monster, CombatState combatState,
        CombatSide side = CombatSide.Enemy, string? slotName = null);
}
```

### CardPileCmd

```csharp
public static class CardPileCmd
{
    // Draw cards
    public static Task<CardModel?> Draw(PlayerChoiceContext ctx, Player player);
    public static Task<IEnumerable<CardModel>> Draw(PlayerChoiceContext ctx,
        decimal count, Player player, bool fromHandDraw = false);

    // Move cards between piles
    public static Task<CardPileAddResult> Add(CardModel card, PileType newPileType,
        CardPilePosition position = CardPilePosition.Bottom,
        AbstractModel? source = null, bool skipVisuals = false);
    public static Task<IReadOnlyList<CardPileAddResult>> Add(IEnumerable<CardModel> cards,
        PileType newPileType, CardPilePosition position = CardPilePosition.Bottom,
        AbstractModel? source = null, bool skipVisuals = false);

    // Generate cards for combat
    public static Task<CardPileAddResult> AddGeneratedCardToCombat(CardModel card,
        PileType newPileType, bool addedByPlayer,
        CardPilePosition position = CardPilePosition.Bottom);
    public static Task<IReadOnlyList<CardPileAddResult>> AddGeneratedCardsToCombat(
        IEnumerable<CardModel> cards, PileType newPileType, bool addedByPlayer,
        CardPilePosition position = CardPilePosition.Bottom);

    // Remove cards
    public static Task RemoveFromDeck(CardModel card, bool showPreview = true);
    public static Task RemoveFromCombat(CardModel card, bool isBeingPlayed, bool skipVisuals = false);

    // Add status/curse cards
    public static Task AddToCombatAndPreview<T>(Creature target, PileType pileType,
        int count, bool addedByPlayer, CardPilePosition position = CardPilePosition.Bottom)
        where T : CardModel;
    public static Task AddCurseToDeck<T>(Player owner) where T : CardModel;

    // Shuffle
    public static Task Shuffle(PlayerChoiceContext ctx, Player player);
    public static Task ShuffleIfNecessary(PlayerChoiceContext ctx, Player player);

    // Auto-play from draw pile
    public static Task AutoPlayFromDrawPile(PlayerChoiceContext ctx, Player player,
        int count, CardPilePosition position, bool forceExhaust);
}
```

**PileType enum:**
```csharp
public enum PileType { None, Hand, Draw, Discard, Exhaust, Play, Deck }
```

**CardPilePosition enum:**
```csharp
public enum CardPilePosition { Top, Bottom, Random }
```

### CardCmd

```csharp
public static class CardCmd
{
    // Auto-play a card
    public static Task AutoPlay(PlayerChoiceContext ctx, CardModel card, Creature? target);

    // Exhaust cards
    public static Task Exhaust(PlayerChoiceContext ctx, CardModel card);
    public static Task ExhaustHand(PlayerChoiceContext ctx, Player player);

    // Discard cards
    public static Task Discard(PlayerChoiceContext ctx, CardModel card);

    // Upgrade
    public static Task Upgrade(CardModel card);

    // Preview cards being added
    public static void PreviewCardPileAdd(CardPileAddResult[] results, float duration,
        CardPreviewStyle style = CardPreviewStyle.HorizontalLayout);
}
```

### AttackCommand (Fluent Builder)

For attack cards, use the fluent `AttackCommand` builder:

```csharp
public class AttackCommand
{
    // Constructor
    public AttackCommand(decimal damagePerHit);
    public AttackCommand(CalculatedDamageVar calculatedDamageVar);

    // Source (required)
    public AttackCommand FromCard(CardModel card);
    public AttackCommand FromMonster(MonsterModel monster);
    public AttackCommand FromOsty(Creature osty, CardModel card);

    // Targeting (required)
    public AttackCommand Targeting(Creature target);
    public AttackCommand TargetingAllOpponents(CombatState combatState);
    public AttackCommand TargetingRandomOpponents(CombatState combatState, bool allowDuplicates = true);

    // Modifiers
    public AttackCommand Unpowered();
    public AttackCommand WithHitCount(int hitCount);
    public AttackCommand OnlyPlayAnimOnce();

    // Animation
    public AttackCommand WithAttackerAnim(string? animName, float delay, Creature? visualAttacker = null);
    public AttackCommand WithNoAttackerAnim();
    public AttackCommand AfterAttackerAnim(Func<Task> afterAttackerAnim);

    // Effects
    public AttackCommand WithAttackerFx(string? vfx = null, string? sfx = null, string? tmpSfx = null);
    public AttackCommand WithAttackerFx(Func<Node2D?> createAttackerVfx);
    public AttackCommand WithHitFx(string? vfx = null, string? sfx = null, string? tmpSfx = null);
    public AttackCommand WithHitVfxNode(Func<Creature, Node2D?> createHitVfxNode);
    public AttackCommand SpawningHitVfxOnEachCreature();
    public AttackCommand WithHitVfxSpawnedAtBase();

    // Timing
    public AttackCommand WithWaitBeforeHit(float fastSeconds, float standardSeconds);
    public AttackCommand BeforeDamage(Func<Task> beforeDamage);

    // Execute
    public Task<AttackCommand> Execute(PlayerChoiceContext? choiceContext);

    // Results
    public IEnumerable<DamageResult> Results { get; }
}
```

**Example usage:**
```csharp
public override async Task Play(PlayerChoiceContext ctx, Creature? target, CardPlay play)
{
    var attack = await new AttackCommand(DynamicVars["D"].DecimalValue)
        .FromCard(this)
        .Targeting(target!)
        .WithHitFx(vfx: "vfx/vfx_slash")
        .Execute(ctx);
}
```

### SfxCmd

```csharp
public static class SfxCmd
{
    public static void Play(string eventPath);
    public static void PlayDamage(MonsterModel monster, int damage);
}
```

### VfxCmd

```csharp
public static class VfxCmd
{
    public static void PlayOnCreature(Creature creature, string scenePath);
    public static void PlayOnCreatureCenter(Creature creature, string scenePath);
    public static void PlayOnCreatures(IEnumerable<Creature> creatures, string scenePath);
    public static void PlayOnCreatureCenters(IEnumerable<Creature> creatures, string scenePath);
    public static void PlayOnSide(CombatSide side, string scenePath, CombatState combatState);
}
```

### ThinkCmd

Display thought bubbles above creatures:

```csharp
public static class ThinkCmd
{
    public static void Play(LocString message, Creature creature, double duration);
}
```

### ValueProp Flags

Used for damage/block properties:

```csharp
[Flags]
public enum ValueProp
{
    None = 0,
    Move = 1,           // From monster move
    Unpowered = 2,      // Not affected by Strength/Dexterity
    Unblockable = 4,    // Ignores block
    SkipHurtAnim = 8,   // Don't play hurt animation
    // ... additional flags
}
```

---

## 5. UI System

### Key UI Nodes

| Class | Description |
|-------|-------------|
| `NGame` | Root game node, singleton via `NGame.Instance` |
| `NRun` | Run manager node, singleton via `NRun.Instance` |
| `NCombatRoom` | Combat room, singleton via `NCombatRoom.Instance` |
| `NCreature` | Visual representation of a creature |
| `NPowerContainer` | Power icon display |
| `NHealthBar` | HP/Block display |
| `NCard` | Card visual |
| `NPlayerHand` | Hand card layout |
| `NCardPlayQueue` | Queue for cards being played |

### Accessing UI Elements

```csharp
// Get creature's visual node
NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);

// Get combat UI
NCombatUi? ui = NCombatRoom.Instance?.Ui;
NPlayerHand hand = ui?.Hand;
NCardPlayQueue playQueue = ui?.PlayQueue;

// Add VFX to combat
Node vfxContainer = NCombatRoom.Instance?.CombatVfxContainer;
vfxContainer?.AddChildSafely(myVfxNode);
```

### Creating Card Nodes

```csharp
NCard nCard = NCard.Create(cardModel);
nCard.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
```

### Screen Effects

```csharp
// Screen shake
NGame.Instance?.ScreenShake(ShakeStrength.Medium, ShakeDuration.Short);

// Screen shake strengths: Weak, Medium, Strong
// Screen shake durations: Short, Medium, Long
```

---

## 6. Entity System

### Creature

Runtime instance of a creature (player or monster).

```csharp
public class Creature
{
    // Identity
    public MonsterModel? Monster { get; }
    public Player? Player { get; }
    public bool IsMonster => Monster != null;
    public bool IsPlayer => Player != null;
    public ModelId ModelId { get; }
    public string Name { get; }

    // Combat state
    public CombatState? CombatState { get; set; }
    public CombatSide Side { get; }
    public uint? CombatId { get; set; }
    public string? SlotName { get; set; }

    // Stats
    public int CurrentHp { get; }
    public int MaxHp { get; }
    public int Block { get; }

    // Status
    public bool IsAlive => CurrentHp > 0;
    public bool IsDead => !IsAlive;
    public bool IsHittable { get; }
    public bool CanReceivePowers { get; }
    public bool IsStunned { get; }

    // Side classification
    public bool IsEnemy => Side == CombatSide.Enemy;
    public bool IsPrimaryEnemy { get; }
    public bool IsSecondaryEnemy { get; }  // Minion

    // Pet system
    public Player? PetOwner { get; set; }
    public bool IsPet => PetOwner != null;
    public IReadOnlyList<Creature> Pets { get; }

    // Powers
    public IReadOnlyList<PowerModel> Powers { get; }
    public bool HasPower<T>() where T : PowerModel;
    public T? GetPower<T>() where T : PowerModel;
    public int GetPowerAmount<T>() where T : PowerModel;

    // Events
    public event Action<int, int>? BlockChanged;
    public event Action<int, int>? CurrentHpChanged;
    public event Action<int, int>? MaxHpChanged;
    public event Action<PowerModel>? PowerApplied;
    public event Action<PowerModel>? PowerRemoved;
    public event Action<Creature>? Died;
    public event Action<Creature>? Revived;

    // Utility
    public double GetHpPercentRemaining();
}
```

### Player

Runtime instance of a player.

```csharp
public class Player
{
    // Identity
    public CharacterModel Character { get; }
    public Creature Creature { get; }
    public ulong NetId { get; }

    // Run state
    public IRunState RunState { get; set; }
    public PlayerCombatState? PlayerCombatState { get; }

    // Resources
    public int Gold { get; set; }
    public int MaxEnergy { get; set; }

    // Collections
    public CardPile Deck { get; }
    public IReadOnlyList<RelicModel> Relics { get; }
    public IReadOnlyList<PotionModel?> PotionSlots { get; }
    public IEnumerable<PotionModel> Potions { get; }
    public IEnumerable<CardPile> Piles { get; }  // All card piles

    // Orbs (Defect)
    public int BaseOrbSlotCount { get; set; }

    // Potions
    public int MaxPotionCount { get; }
    public bool HasOpenPotionSlots { get; }
    public bool CanRemovePotions { get; set; }

    // Pet (Osty)
    public Creature? Osty { get; }
    public bool IsOstyAlive { get; }
    public bool IsOstyMissing { get; }

    // Relic/Potion lookup
    public T? GetRelic<T>() where T : RelicModel;
    public RelicModel? GetRelicById(ModelId id);
    public PotionModel? GetPotionAtSlotIndex(int index);

    // Hooks
    public bool IsActiveForHooks { get; }
    public void DeactivateHooks();
    public void ActivateHooks();

    // Events
    public event Action<RelicModel>? RelicObtained;
    public event Action<RelicModel>? RelicRemoved;
    public event Action<PotionModel>? PotionProcured;
    public event Action<PotionModel>? PotionDiscarded;
    public event Action? GoldChanged;

    // Combat state management
    public void ResetCombatState();
    public void PopulateCombatState(Rng rng, CombatState state);
    public void AfterCombatEnd();
}
```

### CombatState

Combat session state.

```csharp
public class CombatState
{
    // Players and creatures
    public IReadOnlyList<Player> Players { get; }
    public IReadOnlyList<Creature> PlayerCreatures { get; }
    public IReadOnlyList<Creature> Allies { get; }  // Players + pets
    public IReadOnlyList<Creature> Enemies { get; }
    public IReadOnlyList<Creature> AllCreatures { get; }

    // Turn info
    public int RoundNumber { get; }
    public CombatSide CurrentSide { get; }

    // Run state
    public IRunState RunState { get; }

    // Card operations
    public CardModel CreateCard<T>(Player owner) where T : CardModel;
    public CardModel CloneCard(CardModel card);
    public bool ContainsCard(CardModel card);

    // Creature operations
    public Creature CreateCreature(MonsterModel monster, CombatSide side, string? slotName);
    public IReadOnlyList<Creature> GetOpponentsOf(Creature creature);
    public IReadOnlyList<Creature> GetTeammatesOf(Creature creature);

    // Hook iteration
    public IEnumerable<AbstractModel> IterateHookListeners();
}
```

### CardPlay

Information about a card being played.

```csharp
public class CardPlay
{
    public CardModel Card { get; }
    public Creature? Target { get; }
    public Player Player { get; }
    public bool WasAutoPlayed { get; }
    public int TimesPlayed { get; }
    public int EnergyCost { get; }
}
```

### DamageResult

Result of dealing damage.

```csharp
public class DamageResult
{
    public Creature Receiver { get; }
    public int UnblockedDamage { get; set; }
    public int BlockedDamage { get; set; }
    public int OverkillDamage { get; set; }
    public bool WasTargetKilled { get; set; }
    public bool WasBlockBroken { get; set; }
    public bool WasFullyBlocked { get; set; }
}
```

---

## 7. Useful Utilities

### Log

Static logging class for debugging.

```csharp
public static class Log
{
    public static string Timestamp { get; }  // "HH:mm:ss"

    // Log levels
    public static void Debug(string text, int skipFrames = 2);
    public static void VeryDebug(string text, int skipFrames = 2);
    public static void Info(string text, int skipFrames = 2);
    public static void Warn(string text, int skipFrames = 2);
    public static void Error(string text, int skipFrames = 2);
    public static void Load(string text, int skipFrames = 2);

    // Generic log
    public static void LogMessage(LogLevel level, LogType type, string text, int skipFrames = 1);

    // Event
    public static event Action<LogLevel, string, int>? LogCallback;
}
```

**Usage:**
```csharp
Log.Info("My mod initialized!");
Log.Debug($"Card played: {card.Id}");
Log.Error("Something went wrong!");
```

### LocString

Localized string wrapper.

```csharp
public readonly struct LocString
{
    public LocString(string category, string key);
    public string GetFormattedText();
    public string GetFormattedText(DynamicVarSet vars);
}
```

### DynamicVar System

For templating description strings with values.

```csharp
// Define vars in your model
protected override IEnumerable<DynamicVar> CanonicalVars => new[]
{
    new DamageVar("D", 6),          // {D} in description
    new BlockVar("B", 5),           // {B} in description
    new PowerVar<StrengthPower>(2)  // {Strength} in description
};

// Access at runtime
int damageValue = DynamicVars["D"].IntValue;
decimal decimalValue = DynamicVars["D"].DecimalValue;
```

### HoverTips (Tooltips)

```csharp
// Create simple tooltip
var tip = new HoverTip("Title", "Description text");

// Create tooltip from power
var powerTip = HoverTipFactory.FromPower<StrengthPower>();

// For models, override ExtraHoverTips
protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    new[] { HoverTipFactory.FromPower<VigorPower>() };
```

### CombatManager

Singleton for combat management.

```csharp
public class CombatManager
{
    public static CombatManager Instance { get; }

    public bool IsInProgress { get; }
    public bool IsOverOrEnding { get; }
    public bool IsEnding { get; }

    public void LoseCombat();
    public void WinCombat();
}
```

### SaveManager

Access to save data.

```csharp
public class SaveManager
{
    public static SaveManager Instance { get; }

    public PrefsSave PrefsSave { get; }  // Player preferences
    public ProgressSave Progress { get; }  // Unlocks, stats

    public bool SeenFtue(string ftueId);
    public void MarkFtueAsComplete(string ftueId);
}
```

### Rng (Random Number Generator)

Deterministic random for seeded runs.

```csharp
// Access through player or run state
Rng rng = player.PlayerRng.Combat;

// Generate numbers
int randomInt = rng.NextInt(min, max);  // inclusive min, exclusive max
float randomFloat = rng.NextFloat(min, max);
T randomItem = rng.NextItem(list);

// Shuffle
list.StableShuffle(rng);
```

---

## Appendix: Common Patterns

### Relic That Triggers on Attack

```csharp
public sealed class MyRelic : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Attack)
            return;

        Flash();
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, null);
    }
}
```

### Power That Modifies Damage

```csharp
public sealed class MyPower : PowerModel
{
    public override bool IsDebuff => false;
    public override PowerStackType StackType => PowerStackType.Intensity;

    public override decimal ModifyDamageAdditive(Creature target, Creature? dealer,
        decimal baseAmount, ValueProp props, CardModel? cardSource, CardPreviewMode previewMode)
    {
        if (dealer != Owner) return 0;
        return Amount;  // Add power amount to damage
    }
}
```

### Card That Deals Damage and Applies Debuff

```csharp
public sealed class MyAttackCard : CardModel
{
    public override CardType Type => CardType.Attack;
    public override int BaseCost => 2;
    public override CardTarget TargetType => CardTarget.SingleEnemy;

    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new DamageVar("D", 8),
        new PowerVar<WeakPower>(2)
    };

    public override async Task Play(PlayerChoiceContext ctx, Creature? target, CardPlay play)
    {
        await new AttackCommand(DynamicVars["D"].DecimalValue)
            .FromCard(this)
            .Targeting(target!)
            .Execute(ctx);

        await PowerCmd.Apply<WeakPower>(target!, DynamicVars["WeakPower"].IntValue,
            Owner.Creature, this);
    }
}
```

---

*Last updated: Generated from decompiled source*
