using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RegimentOfRenownManager : Node
{
    public static RegimentOfRenownManager Instance { get; private set; }

    private Dictionary<string, RegimentOfRenown> _availableRegiments = new Dictionary<string, RegimentOfRenown>();
    private Dictionary<int, List<RegimentOfRenown>> _playerRegiments = new Dictionary<int, List<RegimentOfRenown>>();

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeRegiments();
        }
        else
        {
            QueueFree();
        }
    }

    private void InitializeRegiments()
    {
        // Stormcast Eternals Regiments
        AddRegiment(new RegimentOfRenown
        {
            Name = "Hammer of Sigmar",
            Description = "A legendary regiment of Stormcast Eternals known for their unbreakable resolve and divine protection.",
            Faction = "Stormcast Eternals",
            Units = new List<string> { "Lord-Celestant", "Liberators", "Vindictors" },
            SpecialRules = new List<string> { "Unbreakable", "Sigmar's Blessing", "Thunderstrike" },
            PointBonus = 0,
            MinUnitCount = 3,
            MaxUnitCount = 5,
            RequiredUnits = new List<string> { "Lord-Celestant" },
            OptionalUnits = new List<string> { "Liberators", "Vindictors" }
        });

        AddRegiment(new RegimentOfRenown
        {
            Name = "Celestial Vindicators",
            Description = "Swift and deadly, these Stormcast strike with lightning speed and precision.",
            Faction = "Stormcast Eternals",
            Units = new List<string> { "Knight-Incantor", "Vindictors", "Liberators" },
            SpecialRules = new List<string> { "Lightning Strike", "Celestial Speed", "Vindicator's Fury" },
            PointBonus = 0,
            MinUnitCount = 3,
            MaxUnitCount = 4,
            RequiredUnits = new List<string> { "Knight-Incantor" },
            OptionalUnits = new List<string> { "Vindictors", "Liberators" }
        });

        // Orruk Warclans Regiments
        AddRegiment(new RegimentOfRenown
        {
            Name = "Ironjawz Waaagh!",
            Description = "A mighty waaagh! led by the most powerful Orruk warlords, crushing all in their path.",
            Faction = "Orruk Warclans",
            Units = new List<string> { "Megaboss", "Orruk Brute", "Warchanter" },
            SpecialRules = new List<string> { "Waaagh!", "Ironjawz Might", "Brutal Power" },
            PointBonus = 0,
            MinUnitCount = 3,
            MaxUnitCount = 5,
            RequiredUnits = new List<string> { "Megaboss" },
            OptionalUnits = new List<string> { "Orruk Brute", "Warchanter" }
        });

        AddRegiment(new RegimentOfRenown
        {
            Name = "Bloodtoofs",
            Description = "The most savage and bloodthirsty of the Ironjawz, they revel in close combat.",
            Faction = "Orruk Warclans",
            Units = new List<string> { "Megaboss", "Orruk Brute" },
            SpecialRules = new List<string> { "Bloodtoof Rage", "Savage Fury", "Blood for the Blood God" },
            PointBonus = 0,
            MinUnitCount = 2,
            MaxUnitCount = 4,
            RequiredUnits = new List<string> { "Megaboss" },
            OptionalUnits = new List<string> { "Orruk Brute" }
        });

        // Cities of Sigmar Regiments
        AddRegiment(new RegimentOfRenown
        {
            Name = "Freeguild Defenders",
            Description = "The stalwart defenders of the cities, trained in shield wall tactics and disciplined combat.",
            Faction = "Cities of Sigmar",
            Units = new List<string> { "Freeguild General", "Freeguild Guard", "Battlemage" },
            SpecialRules = new List<string> { "Shield Wall", "Disciplined", "City Defender" },
            PointBonus = 0,
            MinUnitCount = 3,
            MaxUnitCount = 6,
            RequiredUnits = new List<string> { "Freeguild General" },
            OptionalUnits = new List<string> { "Freeguild Guard", "Battlemage" }
        });

        // Nighthaunt Regiments
        AddRegiment(new RegimentOfRenown
        {
            Name = "Chainrasp Legion",
            Description = "A terrifying legion of the dead, bound by chains and driven by eternal hatred.",
            Faction = "Nighthaunt",
            Units = new List<string> { "Knight of Shrouds", "Chainrasp Horde" },
            SpecialRules = new List<string> { "Ethereal", "Chain Bound", "Spectral Terror" },
            PointBonus = 0,
            MinUnitCount = 2,
            MaxUnitCount = 4,
            RequiredUnits = new List<string> { "Knight of Shrouds" },
            OptionalUnits = new List<string> { "Chainrasp Horde" }
        });
    }

    private void AddRegiment(RegimentOfRenown regiment)
    {
        _availableRegiments[regiment.Name] = regiment;
    }

    /// <summary>
    /// Check if a unit can be part of a specific regiment
    /// </summary>
    public bool CanUnitJoinRegiment(string unitName, string regimentName)
    {
        if (!_availableRegiments.ContainsKey(regimentName))
            return false;

        var regiment = _availableRegiments[regimentName];
        return regiment.Units.Contains(unitName);
    }

    /// <summary>
    /// Get all available regiments for a faction
    /// </summary>
    public List<RegimentOfRenown> GetRegimentsForFaction(string faction)
    {
        return _availableRegiments.Values.Where(r => r.Faction == faction).ToList();
    }

    /// <summary>
    /// Get all available regiments
    /// </summary>
    public List<RegimentOfRenown> GetAllRegiments()
    {
        return _availableRegiments.Values.ToList();
    }

    /// <summary>
    /// Create a regiment instance for a player
    /// </summary>
    public RegimentInstance CreateRegimentInstance(int playerId, string regimentName, List<Unit> units)
    {
        if (!_availableRegiments.ContainsKey(regimentName))
        {
            GD.PrintErr($"Regiment {regimentName} not found");
            return null;
        }

        var regiment = _availableRegiments[regimentName];
        
        // Validate units
        if (!ValidateRegimentComposition(regiment, units))
        {
            GD.PrintErr($"Invalid regiment composition for {regimentName}");
            return null;
        }

        var regimentInstance = new RegimentInstance
        {
            Regiment = regiment,
            Units = units,
            PlayerId = playerId,
            IsActive = true
        };

        // Add to player's regiments
        if (!_playerRegiments.ContainsKey(playerId))
            _playerRegiments[playerId] = new List<RegimentInstance>();
        
        _playerRegiments[playerId].Add(regimentInstance);

        GD.Print($"Player {playerId} created regiment {regimentName} with {units.Count} units");
        return regimentInstance;
    }

    /// <summary>
    /// Validate that a regiment composition meets the requirements
    /// </summary>
    private bool ValidateRegimentComposition(RegimentOfRenown regiment, List<Unit> units)
    {
        // Check minimum and maximum unit counts
        if (units.Count < regiment.MinUnitCount || units.Count > regiment.MaxUnitCount)
        {
            GD.PrintErr($"Regiment {regiment.Name} requires {regiment.MinUnitCount}-{regiment.MaxUnitCount} units, got {units.Count}");
            return false;
        }

        // Check required units are present
        foreach (var requiredUnit in regiment.RequiredUnits)
        {
            if (!units.Any(u => u.UnitName == requiredUnit))
            {
                GD.PrintErr($"Regiment {regiment.Name} requires {requiredUnit}");
                return false;
            }
        }

        // Check all units are valid for this regiment
        foreach (var unit in units)
        {
            if (!regiment.Units.Contains(unit.UnitName))
            {
                GD.PrintErr($"Unit {unit.UnitName} cannot be part of regiment {regiment.Name}");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Get all regiments for a specific player
    /// </summary>
    public List<RegimentInstance> GetPlayerRegiments(int playerId)
    {
        return _playerRegiments.ContainsKey(playerId) ? _playerRegiments[playerId] : new List<RegimentInstance>();
    }

    /// <summary>
    /// Activate a regiment's special rules
    /// </summary>
    public void ActivateRegimentRules(RegimentInstance regimentInstance)
    {
        if (!regimentInstance.IsActive)
            return;

        var regiment = regimentInstance.Regiment;
        
        foreach (var unit in regimentInstance.Units)
        {
            // Apply regiment-specific bonuses
            ApplyRegimentBonuses(unit, regiment);
        }

        GD.Print($"Activated regiment rules for {regiment.Name}");
    }

    /// <summary>
    /// Apply regiment-specific bonuses to a unit
    /// </summary>
    private void ApplyRegimentBonuses(Unit unit, RegimentOfRenown regiment)
    {
        switch (regiment.Name)
        {
            case "Hammer of Sigmar":
                // Add +1 to Bravery for all units in regiment
                unit.AddTemporaryEffect("Sigmar's Blessing", "Bravery", -1, GameManager.Instance.CurrentTurnPhase);
                break;
                
            case "Ironjawz Waaagh!":
                // Add +1 to Damage for all units in regiment
                unit.AddTemporaryEffect("Waaagh! Fury", "Damage", 1, GameManager.Instance.CurrentTurnPhase);
                break;
                
            case "Freeguild Defenders":
                // Add +1 to Save for all units in regiment
                unit.AddTemporaryEffect("Shield Wall", "Save", -1, GameManager.Instance.CurrentTurnPhase);
                break;
                
            case "Chainrasp Legion":
                // Add Ethereal ability
                unit.IsEthereal = true;
                break;
        }
    }

    /// <summary>
    /// Check if a unit is part of an active regiment
    /// </summary>
    public RegimentInstance GetUnitRegiment(Unit unit)
    {
        foreach (var playerRegiments in _playerRegiments.Values)
        {
            foreach (var regiment in playerRegiments)
            {
                if (regiment.Units.Contains(unit) && regiment.IsActive)
                    return regiment;
            }
        }
        return null;
    }

    /// <summary>
    /// Deactivate a regiment
    /// </summary>
    public void DeactivateRegiment(RegimentInstance regimentInstance)
    {
        if (regimentInstance.IsActive)
        {
            regimentInstance.IsActive = false;
            
            // Remove regiment bonuses
            foreach (var unit in regimentInstance.Units)
            {
                RemoveRegimentBonuses(unit, regimentInstance.Regiment);
            }
            
            GD.Print($"Deactivated regiment {regimentInstance.Regiment.Name}");
        }
    }

    /// <summary>
    /// Remove regiment-specific bonuses from a unit
    /// </summary>
    private void RemoveRegimentBonuses(Unit unit, RegimentOfRenown regiment)
    {
        switch (regiment.Name)
        {
            case "Hammer of Sigmar":
                unit.RemoveTemporaryEffect("Sigmar's Blessing");
                break;
                
            case "Ironjawz Waaagh!":
                unit.RemoveTemporaryEffect("Waaagh! Fury");
                break;
                
            case "Freeguild Defenders":
                unit.RemoveTemporaryEffect("Shield Wall");
                break;
                
            case "Chainrasp Legion":
                unit.IsEthereal = false;
                break;
        }
    }

    /// <summary>
    /// Get regiment information for display
    /// </summary>
    public string GetRegimentInfo(string regimentName)
    {
        if (!_availableRegiments.ContainsKey(regimentName))
            return "Regiment not found";

        var regiment = _availableRegiments[regimentName];
        var info = $"Regiment: {regiment.Name}\n";
        info += $"Faction: {regiment.Faction}\n";
        info += $"Description: {regiment.Description}\n";
        info += $"Units: {string.Join(", ", regiment.Units)}\n";
        info += $"Special Rules: {string.Join(", ", regiment.SpecialRules)}\n";
        info += $"Min Units: {regiment.MinUnitCount}, Max Units: {regiment.MaxUnitCount}\n";
        info += $"Required: {string.Join(", ", regiment.RequiredUnits)}\n";
        info += $"Optional: {string.Join(", ", regiment.OptionalUnits)}";

        return info;
    }
}

// Enhanced RegimentOfRenown class
public class RegimentOfRenown
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Faction { get; set; }
    public List<string> Units { get; set; } = new List<string>();
    public List<string> SpecialRules { get; set; } = new List<string>();
    public int PointBonus { get; set; }
    public int MinUnitCount { get; set; }
    public int MaxUnitCount { get; set; }
    public List<string> RequiredUnits { get; set; } = new List<string>();
    public List<string> OptionalUnits { get; set; } = new List<string>();
}

// Regiment instance for a specific player
public class RegimentInstance
{
    public RegimentOfRenown Regiment { get; set; }
    public List<Unit> Units { get; set; } = new List<Unit>();
    public int PlayerId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
