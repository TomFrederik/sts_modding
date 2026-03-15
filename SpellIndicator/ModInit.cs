using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace SpellIndicator;

/// <summary>
/// Defines a spell effect that can be indicated above a player.
/// </summary>
public record SpellEffectDefinition(
    string DynamicVarKey,          // Key in card.DynamicVars (e.g., "WeakPower", "VulnerablePower")
    int DisplayOrder,              // Lower = displayed first (leftmost)
    string TooltipText,            // Text shown on hover
    Func<PowerModel> GetPower      // Function to get the power model (provides icon, title, etc.)
)
{
    public Texture2D? GetIcon() => GetPower().Icon;
}

[ModInitializer("ModLoaded")]
public static class ModInit
{
    private const int IconSize = 32;
    private const int IconSpacing = 4;
    private const int VerticalOffset = -320;

    // All supported spell effects, in display order
    private static readonly List<SpellEffectDefinition> _spellEffects = new()
    {
        new SpellEffectDefinition(
            DynamicVarKey: "WeakPower",
            DisplayOrder: 0,
            TooltipText: "This player can apply Weak.",
            GetPower: () => ModelDb.Power<WeakPower>()
        ),
        new SpellEffectDefinition(
            DynamicVarKey: "VulnerablePower",
            DisplayOrder: 1,
            TooltipText: "This player can apply Vulnerable.",
            GetPower: () => ModelDb.Power<VulnerablePower>()
        ),
    };

    // Track indicator container per player
    private static readonly Dictionary<Player, HBoxContainer> _indicatorContainers = new();

    // Track individual effect indicators per player
    private static readonly Dictionary<Player, Dictionary<string, TextureRect>> _effectIndicators = new();

    public static void ModLoaded()
    {
        Log.Info("[SpellIndicator] Initializing...");

        CombatManager.Instance.CombatSetUp += OnCombatSetUp;
        CombatManager.Instance.CombatEnded += OnCombatEnded;

        Log.Info("[SpellIndicator] Initialization complete!");
    }

    private static void OnCombatSetUp(CombatState state)
    {
        Log.Info("[SpellIndicator] Combat started, setting up hand watchers...");

        foreach (var player in state.Players)
        {
            var hand = player.PlayerCombatState?.Hand;
            if (hand == null) continue;

            // Subscribe to hand changes
            hand.CardAdded += _ => OnHandChanged(player);
            hand.CardRemoved += _ => OnHandChanged(player);

            // Initial check
            OnHandChanged(player);
        }
    }

    private static void OnCombatEnded(CombatRoom room)
    {
        Log.Info("[SpellIndicator] Combat ended, cleaning up...");

        foreach (var container in _indicatorContainers.Values)
        {
            if (GodotObject.IsInstanceValid(container))
            {
                container.QueueFree();
            }
        }
        _indicatorContainers.Clear();
        _effectIndicators.Clear();
    }

    private static void OnHandChanged(Player player)
    {
        var hand = player.PlayerCombatState?.Hand;
        if (hand == null) return;

        // Check which effects are available in hand
        var activeEffects = GetActiveEffects(hand);

        // Update indicators
        UpdateIndicators(player, activeEffects);
    }

    private static HashSet<string> GetActiveEffects(CardPile hand)
    {
        var activeEffects = new HashSet<string>();

        foreach (var card in hand.Cards)
        {
            foreach (var effect in _spellEffects)
            {
                if (card.DynamicVars.TryGetValue(effect.DynamicVarKey, out var dynVar) &&
                    dynVar.BaseValue > 0)
                {
                    activeEffects.Add(effect.DynamicVarKey);
                }
            }
        }

        return activeEffects;
    }

    private static void UpdateIndicators(Player player, HashSet<string> activeEffects)
    {
        // Ensure container exists
        var container = GetOrCreateContainer(player);
        if (container == null) return;

        // Ensure we have a dictionary for this player's indicators
        if (!_effectIndicators.TryGetValue(player, out var indicators))
        {
            indicators = new Dictionary<string, TextureRect>();
            _effectIndicators[player] = indicators;
        }

        // Update each effect indicator
        foreach (var effect in _spellEffects.OrderBy(e => e.DisplayOrder))
        {
            bool shouldShow = activeEffects.Contains(effect.DynamicVarKey);

            if (!indicators.TryGetValue(effect.DynamicVarKey, out var indicator))
            {
                // Create indicator if it doesn't exist
                indicator = CreateIndicator(effect);
                indicators[effect.DynamicVarKey] = indicator;
                container.AddChild(indicator);
            }

            indicator.Visible = shouldShow;
        }
    }

    private static HBoxContainer? GetOrCreateContainer(Player player)
    {
        if (_indicatorContainers.TryGetValue(player, out var existing) &&
            GodotObject.IsInstanceValid(existing))
        {
            return existing;
        }

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        if (creatureNode == null)
        {
            Log.Warn("[SpellIndicator] Could not find creature node for player");
            return null;
        }

        var container = new HBoxContainer();
        container.AddThemeConstantOverride("separation", IconSpacing);
        container.MouseFilter = Control.MouseFilterEnum.Pass; // Allow mouse events to pass through to children

        // Position above the creature
        // Calculate X to center the container (will adjust based on number of visible icons)
        container.Position = new Vector2(-IconSize, VerticalOffset);

        creatureNode.AddChild(container);
        _indicatorContainers[player] = container;

        return container;
    }

    private static TextureRect CreateIndicator(SpellEffectDefinition effect)
    {
        var indicator = new TextureRect();
        indicator.Texture = effect.GetIcon();
        indicator.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        indicator.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        indicator.CustomMinimumSize = new Vector2(IconSize, IconSize);
        indicator.Size = new Vector2(IconSize, IconSize);
        indicator.ZIndex = 100;
        indicator.MouseFilter = Control.MouseFilterEnum.Stop; // Enable mouse events

        // Set up hover tooltip
        indicator.MouseEntered += () => OnIndicatorHovered(indicator, effect);
        indicator.MouseExited += () => OnIndicatorUnhovered(indicator);

        return indicator;
    }

    private static void OnIndicatorHovered(TextureRect indicator, SpellEffectDefinition effect)
    {
        Log.Info($"[SpellIndicator] Mouse entered indicator for {effect.DynamicVarKey}");
        try
        {
            // Use the PowerModel constructor which handles title/icon/debuff status automatically
            var power = effect.GetPower();
            var hoverTip = new HoverTip(power, effect.TooltipText, isSmart: false);
            Log.Info($"[SpellIndicator] Created HoverTip, calling CreateAndShow...");

            // Get optimal alignment based on screen position
            var alignment = HoverTip.GetHoverTipAlignment(indicator);
            NHoverTipSet.CreateAndShow(indicator, hoverTip, alignment);
            Log.Info($"[SpellIndicator] CreateAndShow completed with alignment {alignment}");
        }
        catch (Exception e)
        {
            Log.Warn($"[SpellIndicator] Failed to show hover tip: {e.Message}\n{e.StackTrace}");
        }
    }

    private static void OnIndicatorUnhovered(TextureRect indicator)
    {
        Log.Info($"[SpellIndicator] Mouse exited indicator");
        try
        {
            NHoverTipSet.Remove(indicator);
        }
        catch (Exception e)
        {
            Log.Warn($"[SpellIndicator] Failed to hide hover tip: {e.Message}");
        }
    }
}
