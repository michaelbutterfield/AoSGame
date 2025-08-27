using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class HeroicTraitsManager : Node
{
    public static HeroicTraitsManager Instance { get; private set; }

    private Dictionary<string, List<HeroicTrait>> _availableTraits = new Dictionary<string, List<HeroicTrait>>();
    private Dictionary<int, List<AppliedHeroicTrait>> _playerTraits = new Dictionary<int, List<AppliedHeroicTrait>>();

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeTraits();
        }
        else
        {
            QueueFree();
        }
    }

    private void InitializeTraits()
    {
        // Stormcast Eternals Heroic Traits
        AddHeroicTrait("Stormcast Eternals", new HeroicTrait
        {
            Name = "Lightning Strike",
            Description = "This HERO can make a charge move of up to 12\" instead of rolling.",
            Faction = "Stormcast Eternals",
            Effect = "Once per battle, charge move of 12\"",
            PointCost = 0,
            StatModifiers = new Dictionary<string, int>
            {
                { "ChargeRange", 12 }
            },
            SpecialRules = new List<string> { "Lightning Strike" }
        });

        AddHeroicTrait("Stormcast Eternals", new HeroicTrait
        {
            Name = "Unbreakable",
            Description = "This HERO cannot be affected by battleshock tests.",
            Faction = "Stormcast Eternals",
            Effect = "Immune to battleshock",
            PointCost = 0,
            SpecialRules = new List<string> { "Unbreakable" }
        });

        AddHeroicTrait("Stormcast Eternals", new HeroicTrait
        {
            Name = "Sigmar's Chosen",
            Description = "Add 1 to save rolls for attacks that target this HERO.",
            Faction = "Stormcast Eternals",
            Effect = "+1 to save rolls",
            PointCost = 0,
            StatModifiers = new Dictionary<string, int>
            {
                { "Save", 1 }
            }
        });

        // Orruk Warclans Heroic Traits
        AddHeroicTrait("Orruk Warclans", new HeroicTrait
        {
            Name = "Brutal Cunning",
            Description = "This HERO can use the 'All-out Attack' command ability for free once per turn.",
            Faction = "Orruk Warclans",
            Effect = "Free All-out Attack once per turn",
            PointCost = 0,
            SpecialRules = new List<string> { "Brutal Cunning" }
        });

        AddHeroicTrait("Orruk Warclans", new HeroicTrait
        {
            Name = "Waaagh! Leader",
            Description = "Add 1 to the Damage characteristic of melee weapons used by this HERO.",
            Faction = "Orruk Warclans",
            Effect = "+1 Damage to melee weapons",
            PointCost = 0,
            StatModifiers = new Dictionary<string, int>
            {
                { "Damage", 1 }
            }
        });

        // Cities of Sigmar Heroic Traits
        AddHeroicTrait("Cities of Sigmar", new HeroicTrait
        {
            Name = "Disciplined Commander",
            Description = "This HERO can use the 'Inspiring Presence' command ability for free once per turn.",
            Faction = "Cities of Sigmar",
            Effect = "Free Inspiring Presence once per turn",
            PointCost = 0,
            SpecialRules = new List<string> { "Disciplined Commander" }
        });

        // Nighthaunt Heroic Traits
        AddHeroicTrait("Nighthaunt", new HeroicTrait
        {
            Name = "Spectral Terror",
            Description = "Subtract 1 from the Bravery characteristic of enemy units within 3\" of this HERO.",
            Faction = "Nighthaunt",
            Effect = "-1 Bravery to nearby enemies",
            PointCost = 0,
            SpecialRules = new List<string> { "Spectral Terror" },
            AuraRange = 3.0f
        });

        // Sylvaneth Heroic Traits
        AddHeroicTrait("Sylvaneth", new HeroicTrait
        {
            Name = "Forest Spirit",
            Description = "Add 1 to save rolls for attacks that target this HERO while it is wholly within 6\" of any terrain features.",
            Faction = "Sylvaneth",
            Effect = "+1 Save near terrain",
            PointCost = 0,
            StatModifiers = new Dictionary<string, int>
            {
                { "Save", 1 }
            },
            SpecialRules = new List<string> { "Forest Spirit" },
            TerrainDependent = true
        });
    }

    private void AddHeroicTrait(string faction, HeroicTrait trait)
    {
        if (!_availableTraits.ContainsKey(faction))
            _availableTraits[faction] = new List<HeroicTrait>();
        _availableTraits[faction].Add(trait);
    }

    /// <summary>
    /// Get all available heroic traits for a faction
    /// </summary>
    public List<HeroicTrait> GetTraitsForFaction(string faction)
    {
        return _availableTraits.ContainsKey(faction) ? _availableTraits[faction] : new List<HeroicTrait>();
    }

    /// <summary>
    /// Get all available heroic traits
    /// </summary>
    public List<HeroicTrait> GetAllTraits()
    {
        return _availableTraits.Values.SelectMany(t => t).ToList();
    }

    /// <summary>
    /// Apply a heroic trait to a unit
    /// </summary>
    public bool ApplyHeroicTrait(int playerId, Unit unit, string traitName)
    {
        var trait = GetAllTraits().FirstOrDefault(t => t.Name == traitName);
        if (trait == null)
        {
            GD.PrintErr($"Heroic trait {traitName} not found");
            return false;
        }

        // Check if unit can have this trait
        if (!CanUnitHaveTrait(unit, trait))
        {
            GD.PrintErr($"Unit {unit.UnitName} cannot have heroic trait {traitName}");
            return false;
        }

        // Check if unit already has a heroic trait
        if (HasHeroicTrait(unit))
        {
            GD.PrintErr($"Unit {unit.UnitName} already has a heroic trait");
            return false;
        }

        var appliedTrait = new AppliedHeroicTrait
        {
            Trait = trait,
            Unit = unit,
            PlayerId = playerId,
            AppliedAt = DateTime.Now
        };

        // Add to player's traits
        if (!_playerTraits.ContainsKey(playerId))
            _playerTraits[playerId] = new List<AppliedHeroicTrait>();
        
        _playerTraits[playerId].Add(appliedTrait);

        // Apply trait effects to unit
        ApplyTraitEffects(unit, trait);

        GD.Print($"Player {playerId} applied heroic trait {traitName} to {unit.UnitName}");
        return true;
    }

    /// <summary>
    /// Check if a unit can have a specific heroic trait
    /// </summary>
    public bool CanUnitHaveTrait(Unit unit, HeroicTrait trait)
    {
        // Check faction compatibility
        if (trait.Faction != unit.ArmyName)
            return false;

        // Check if unit is a hero
        if (!unit.IsHero)
            return false;

        // Check if unit is a general (some traits require this)
        if (trait.RequiresGeneral && !unit.IsGeneral)
            return false;

        return true;
    }

    /// <summary>
    /// Check if a unit already has a heroic trait
    /// </summary>
    public bool HasHeroicTrait(Unit unit)
    {
        foreach (var playerTraits in _playerTraits.Values)
        {
            if (playerTraits.Any(t => t.Unit == unit))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Get the heroic trait applied to a unit
    /// </summary>
    public AppliedHeroicTrait GetUnitTrait(Unit unit)
    {
        foreach (var playerTraits in _playerTraits.Values)
        {
            var trait = playerTraits.FirstOrDefault(t => t.Unit == unit);
            if (trait != null)
                return trait;
        }
        return null;
    }

    /// <summary>
    /// Remove a heroic trait from a unit
    /// </summary>
    public void RemoveHeroicTrait(Unit unit)
    {
        var appliedTrait = GetUnitTrait(unit);
        if (appliedTrait != null)
        {
            // Remove trait effects from unit
            RemoveTraitEffects(unit, appliedTrait.Trait);

            // Remove from player's traits
            var playerTraits = _playerTraits[appliedTrait.PlayerId];
            playerTraits.Remove(appliedTrait);

            GD.Print($"Removed heroic trait {appliedTrait.Trait.Name} from {unit.UnitName}");
        }
    }

    /// <summary>
    /// Apply trait effects to a unit
    /// </summary>
    private void ApplyTraitEffects(Unit unit, HeroicTrait trait)
    {
        // Apply stat modifiers
        if (trait.StatModifiers != null)
        {
            foreach (var modifier in trait.StatModifiers)
            {
                switch (modifier.Key.ToLower())
                {
                    case "save":
                        unit.AddTemporaryEffect($"Heroic Trait: {trait.Name}", "Save", modifier.Value, GameManager.TurnPhase.End);
                        break;
                    case "damage":
                        unit.AddTemporaryEffect($"Heroic Trait: {trait.Name}", "Damage", modifier.Value, GameManager.TurnPhase.End);
                        break;
                    case "chargerange":
                        // Store as custom property
                        unit.SetMeta("HeroicTrait_ChargeRange", modifier.Value);
                        break;
                }
            }
        }

        // Apply special rules
        if (trait.SpecialRules != null)
        {
            foreach (var rule in trait.SpecialRules)
            {
                unit.SetMeta($"HeroicTrait_{rule}", true);
            }
        }
    }

    /// <summary>
    /// Remove trait effects from a unit
    /// </summary>
    private void RemoveTraitEffects(Unit unit, HeroicTrait trait)
    {
        // Remove stat modifiers
        if (trait.StatModifiers != null)
        {
            foreach (var modifier in trait.StatModifiers)
            {
                unit.RemoveTemporaryEffect($"Heroic Trait: {trait.Name}");
            }
        }

        // Remove special rules
        if (trait.SpecialRules != null)
        {
            foreach (var rule in trait.SpecialRules)
            {
                unit.RemoveMeta($"HeroicTrait_{rule}");
            }
        }

        // Remove custom properties
        unit.RemoveMeta("HeroicTrait_ChargeRange");
    }

    /// <summary>
    /// Get all heroic traits for a specific player
    /// </summary>
    public List<AppliedHeroicTrait> GetPlayerTraits(int playerId)
    {
        return _playerTraits.ContainsKey(playerId) ? _playerTraits[playerId] : new List<AppliedHeroicTrait>();
    }

    /// <summary>
    /// Get heroic trait information for display
    /// </summary>
    public string GetTraitInfo(string traitName)
    {
        var trait = GetAllTraits().FirstOrDefault(t => t.Name == traitName);
        if (trait == null)
            return "Heroic trait not found";

        var info = $"Heroic Trait: {trait.Name}\n";
        info += $"Faction: {trait.Faction}\n";
        info += $"Description: {trait.Description}\n";
        info += $"Effect: {trait.Effect}\n";
        info += $"Point Cost: {trait.PointCost}\n";

        if (trait.StatModifiers != null && trait.StatModifiers.Count > 0)
        {
            info += $"Stat Modifiers:\n";
            foreach (var modifier in trait.StatModifiers)
            {
                info += $"  {modifier.Key}: {modifier.Value:+0;-0}\n";
            }
        }

        if (trait.SpecialRules != null && trait.SpecialRules.Count > 0)
        {
            info += $"Special Rules: {string.Join(", ", trait.SpecialRules)}\n";
        }

        if (trait.AuraRange > 0)
        {
            info += $"Aura Range: {trait.AuraRange}\"\n";
        }

        if (trait.TerrainDependent)
        {
            info += $"Terrain Dependent: Yes\n";
        }

        return info;
    }

    /// <summary>
    /// Check if a unit can use a specific heroic trait ability
    /// </summary>
    public bool CanUseTraitAbility(Unit unit, string abilityName)
    {
        var appliedTrait = GetUnitTrait(unit);
        if (appliedTrait == null) return false;

        return appliedTrait.Trait.SpecialRules.Contains(abilityName);
    }

    /// <summary>
    /// Use a heroic trait ability
    /// </summary>
    public bool UseTraitAbility(Unit unit, string abilityName)
    {
        if (!CanUseTraitAbility(unit, abilityName)) return false;

        // Handle specific abilities
        switch (abilityName)
        {
            case "Lightning Strike":
                // This would be handled in the movement/charge system
                GD.Print($"Unit {unit.UnitName} used Lightning Strike heroic trait");
                return true;

            case "Brutal Cunning":
                // This would grant a free command ability use
                GD.Print($"Unit {unit.UnitName} used Brutal Cunning heroic trait");
                return true;

            case "Disciplined Commander":
                // This would grant a free command ability use
                GD.Print($"Unit {unit.UnitName} used Disciplined Commander heroic trait");
                return true;

            default:
                GD.Print($"Unit {unit.UnitName} used {abilityName} heroic trait");
                return true;
        }
    }
}

// Enhanced HeroicTrait class
public class HeroicTrait
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Faction { get; set; }
    public string Effect { get; set; }
    public int PointCost { get; set; }
    public Dictionary<string, int> StatModifiers { get; set; } = new Dictionary<string, int>();
    public List<string> SpecialRules { get; set; } = new List<string>();
    public float AuraRange { get; set; } = 0.0f;
    public bool TerrainDependent { get; set; } = false;
    public bool RequiresGeneral { get; set; } = false;
}

// Applied heroic trait for a specific unit
public class AppliedHeroicTrait
{
    public HeroicTrait Trait { get; set; }
    public Unit Unit { get; set; }
    public int PlayerId { get; set; }
    public DateTime AppliedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
