using Godot;
using System.Collections.Generic;

public partial class RadiusIndicatorExample : Node
{
    private RadiusIndicatorManager _radiusManager;
    private GameManager _gameManager;

    public override void _Ready()
    {
        _radiusManager = RadiusIndicatorManager.Instance;
        _gameManager = GameManager.Instance;
        
        // Wait a bit for other systems to initialize
        CallDeferred(nameof(SetupExampleIndicators));
    }

    private void SetupExampleIndicators()
    {
        if (_radiusManager == null || _gameManager == null) return;

        GD.Print("Setting up example radius indicators...");

        // Create example units with different abilities
        CreateExampleUnits();
        
        // Add radius indicators for each unit type
        AddRadiusIndicators();
        
        GD.Print("Example radius indicators setup complete!");
    }

    private void CreateExampleUnits()
    {
        // This would normally be done through the Army Builder
        // For demonstration, we'll create some example units
        GD.Print("Creating example units...");
    }

    private void AddRadiusIndicators()
    {
        var units = _gameManager.GetAllUnits();
        
        foreach (var unit in units)
        {
            AddUnitRadiusIndicators(unit);
        }
    }

    private void AddUnitRadiusIndicators(Unit unit)
    {
        if (unit == null) return;

        GD.Print($"Adding radius indicators for {unit.UnitName}");

        // Stormcast Eternals - Lord-Celestant
        if (unit.UnitName == "Lord-Celestant")
        {
            // General aura: +1 Bravery within 18"
            _radiusManager.CreateBraveryBuffIndicator(unit, 18.0f);
            
            // Hero aura: +1 to charge rolls within 12"
            _radiusManager.CreateChargeBuffIndicator(unit, 12.0f);
            
            GD.Print($"  - Added Bravery Buff (18\") and Charge Buff (12\") for Lord-Celestant");
        }

        // Orruk Warclans - Megaboss
        else if (unit.UnitName == "Megaboss")
        {
            // Waaagh! aura: +1 to charge rolls within 12"
            _radiusManager.CreateChargeBuffIndicator(unit, 12.0f);
            
            // General aura: +1 Bravery within 18"
            _radiusManager.CreateBraveryBuffIndicator(unit, 18.0f);
            
            GD.Print($"  - Added Charge Buff (12\") and Bravery Buff (18\") for Megaboss");
        }

        // Cities of Sigmar - Freeguild General
        else if (unit.UnitName == "Freeguild General")
        {
            // Disciplined aura: +1 Save within 12"
            _radiusManager.CreateSaveBuffIndicator(unit, 12.0f);
            
            // General aura: +1 Bravery within 18"
            _radiusManager.CreateBraveryBuffIndicator(unit, 18.0f);
            
            GD.Print($"  - Added Save Buff (12\") and Bravery Buff (18\") for Freeguild General");
        }

        // Nighthaunt - Knight of Shrouds
        else if (unit.UnitName == "Knight of Shrouds")
        {
            // Spectral terror: -1 Bravery within 3"
            _radiusManager.CreateBraveryDebuffIndicator(unit, 3.0f);
            
            // General aura: +1 Bravery within 18"
            _radiusManager.CreateBraveryBuffIndicator(unit, 18.0f);
            
            GD.Print($"  - Added Bravery Debuff (3\") and Bravery Buff (18\") for Knight of Shrouds");
        }

        // Sylvaneth - Branchwych
        else if (unit.UnitName == "Branchwych")
        {
            // Forest spirit: +1 Save near terrain within 6"
            _radiusManager.CreateTerrainDependentIndicator(unit, 6.0f, "+1 Save near terrain");
            
            // Wizard aura: +1 to spell casting within 12"
            _radiusManager.CreateAuraIndicator(unit, 12.0f, "+1 to spell casting");
            
            GD.Print($"  - Added Terrain Dependent (6\") and Aura (12\") for Branchwych");
        }

        // Khorne - Bloodthirster
        else if (unit.UnitName == "Bloodthirster")
        {
            // Blood rage: +1 Damage within 8"
            _radiusManager.CreateRadiusIndicator(unit, "Aura", 8.0f, "+1 Damage within 8\"");
            
            // Terror: -1 Bravery within 6"
            _radiusManager.CreateBraveryDebuffIndicator(unit, 6.0f);
            
            GD.Print($"  - Added Damage Aura (8\") and Bravery Debuff (6\") for Bloodthirster");
        }

        // Tzeentch - Lord of Change
        else if (unit.UnitName == "Lord of Change")
        {
            // Arcane mastery: +1 to spell casting within 12"
            _radiusManager.CreateAuraIndicator(unit, 12.0f, "+1 to spell casting");
            
            // Reality distortion: -1 Save within 6"
            _radiusManager.CreateRadiusIndicator(unit, "MovementDebuff", 6.0f, "-1 Save within 6\"");
            
            GD.Print($"  - Added Spell Aura (12\") and Save Debuff (6\") for Lord of Change");
        }

        // Lumineth Realm-Lords - Vanari Dawnriders
        else if (unit.UnitName == "Vanari Dawnriders")
        {
            // Swift movement: +1 Move within 6"
            _radiusManager.CreateRadiusIndicator(unit, "Aura", 6.0f, "+1 Move within 6\"");
            
            GD.Print($"  - Added Movement Aura (6\") for Vanari Dawnriders");
        }

        // Idoneth Deepkin - Akhelian King
        else if (unit.UnitName == "Akhelian King")
        {
            // Sea lord: +1 to hit within 9"
            _radiusManager.CreateRadiusIndicator(unit, "SaveBuff", 9.0f, "+1 to hit within 9\"");
            
            // General aura: +1 Bravery within 18"
            _radiusManager.CreateBraveryBuffIndicator(unit, 18.0f);
            
            GD.Print($"  - Added Hit Buff (9\") and Bravery Buff (18\") for Akhelian King");
        }
    }

    /// <summary>
    /// Demonstrate how to create custom radius indicators
    /// </summary>
    public void CreateCustomRadiusIndicator(Unit unit, string effect, float rangeInInches)
    {
        if (_radiusManager == null) return;

        // Create a custom indicator based on the effect type
        RadiusIndicator indicator = null;
        
        if (effect.Contains("charge") || effect.Contains("Charge"))
        {
            indicator = _radiusManager.CreateChargeBuffIndicator(unit, rangeInInches);
        }
        else if (effect.Contains("save") || effect.Contains("Save"))
        {
            indicator = _radiusManager.CreateSaveBuffIndicator(unit, rangeInInches);
        }
        else if (effect.Contains("bravery") || effect.Contains("Bravery"))
        {
            if (effect.Contains("-") || effect.Contains("debuff"))
            {
                indicator = _radiusManager.CreateBraveryDebuffIndicator(unit, rangeInInches);
            }
            else
            {
                indicator = _radiusManager.CreateBraveryBuffIndicator(unit, rangeInInches);
            }
        }
        else if (effect.Contains("aura") || effect.Contains("Aura"))
        {
            indicator = _radiusManager.CreateAuraIndicator(unit, rangeInInches, effect);
        }
        else
        {
            // Generic indicator
            indicator = _radiusManager.CreateRadiusIndicator(unit, "Aura", rangeInInches, effect);
        }

        if (indicator != null)
        {
            GD.Print($"Created custom radius indicator: {effect} within {rangeInInches}\" for {unit.UnitName}");
        }
    }

    /// <summary>
    /// Show all radius indicators
    /// </summary>
    public void ShowAllIndicators()
    {
        if (_radiusManager != null)
        {
            _radiusManager.SetShowAllIndicators(true);
            GD.Print("All radius indicators are now visible");
        }
    }

    /// <summary>
    /// Hide all radius indicators
    /// </summary>
    public void HideAllIndicators()
    {
        if (_radiusManager != null)
        {
            _radiusManager.SetShowAllIndicators(false);
            GD.Print("All radius indicators are now hidden");
        }
    }

    /// <summary>
    /// Toggle radius indicators for a specific unit
    /// </summary>
    public void ToggleUnitIndicators(Unit unit)
    {
        if (_radiusManager != null)
        {
            _radiusManager.ToggleUnitIndicators(unit);
            GD.Print($"Toggled radius indicators for {unit.UnitName}");
        }
    }

    /// <summary>
    /// Get information about all radius indicators
    /// </summary>
    public void PrintAllIndicatorsInfo()
    {
        if (_radiusManager == null) return;

        var units = _gameManager.GetAllUnits();
        GD.Print("\n=== RADIUS INDICATORS SUMMARY ===");
        
        foreach (var unit in units)
        {
            var info = _radiusManager.GetIndicatorInfo(unit);
            if (info != "No radius indicators")
            {
                GD.Print($"\n{unit.UnitName}:");
                GD.Print(info);
            }
        }
        
        GD.Print("\n=== END SUMMARY ===");
    }

    /// <summary>
    /// Demonstrate radius indicator interactions
    /// </summary>
    public void DemonstrateRadiusInteractions()
    {
        if (_radiusManager == null) return;

        var units = _gameManager.GetAllUnits();
        
        GD.Print("\n=== RADIUS INTERACTION DEMONSTRATION ===");
        
        foreach (var unit in units)
        {
            // Check what units are affected by this unit's buffs
            var unitsInChargeRange = _radiusManager.GetUnitsInBuffRange(unit, "Charge");
            var unitsInBraveryRange = _radiusManager.GetUnitsInBuffRange(unit, "Bravery");
            var unitsInSaveRange = _radiusManager.GetUnitsInBuffRange(unit, "Save");
            
            if (unitsInChargeRange.Count > 0 || unitsInBraveryRange.Count > 0 || unitsInSaveRange.Count > 0)
            {
                GD.Print($"\n{unit.UnitName} affects:");
                
                if (unitsInChargeRange.Count > 0)
                {
                    GD.Print($"  Charge buff: {string.Join(", ", unitsInChargeRange.Select(u => u.UnitName))}");
                }
                
                if (unitsInBraveryRange.Count > 0)
                {
                    GD.Print($"  Bravery buff: {string.Join(", ", unitsInBraveryRange.Select(u => u.UnitName))}");
                }
                
                if (unitsInSaveRange.Count > 0)
                {
                    GD.Print($"  Save buff: {string.Join(", ", unitsInSaveRange.Select(u => u.UnitName))}");
                }
            }
        }
        
        GD.Print("\n=== END DEMONSTRATION ===");
    }

    /// <summary>
    /// Create a comprehensive example army with radius indicators
    /// </summary>
    public void CreateComprehensiveExample()
    {
        GD.Print("\n=== CREATING COMPREHENSIVE EXAMPLE ===");
        
        // This would normally be done through the Army Builder
        // For now, we'll just demonstrate the radius indicator system
        
        ShowAllIndicators();
        PrintAllIndicatorsInfo();
        DemonstrateRadiusInteractions();
        
        GD.Print("Comprehensive example complete!");
    }
}
