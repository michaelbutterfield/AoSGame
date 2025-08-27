using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ArmyBuilder : Node
{
    [Export] public int MaxPoints = 2000;
    [Export] public string Faction = "";
    
    private Army _currentArmy;
    private Dictionary<string, UnitData> _unitDatabase;
    
    public override void _Ready()
    {
        // Initialize the comprehensive unit database
        UnitDatabase.Initialize();
        
        // Initialize legacy unit database for backward compatibility
        InitializeUnitDatabase();
        _currentArmy = new Army();
    }
    
    private void InitializeUnitDatabase()
    {
        _unitDatabase = new Dictionary<string, UnitData>();
        
        // Stormcast Eternals
        AddUnitToDatabase("Liberators", new UnitData
        {
            Name = "Liberators",
            Faction = "Stormcast Eternals",
            Points = 120,
            Type = UnitType.Battleline,
            Move = 5,
            Wounds = 2,
            Bravery = 7,
            Save = 4,
            ModelCount = 5,
            BaseSize = 1.0f,
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Liberator-Prime", new UnitData
        {
            Name = "Liberator-Prime",
            Faction = "Stormcast Eternals",
            Points = 120,
            Type = UnitType.Battleline,
            Move = 5,
            Wounds = 2,
            Bravery = 7,
            Save = 4,
            ModelCount = 1,
            BaseSize = 1.0f,
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Stormcast Warrior", new UnitData
        {
            Name = "Stormcast Warrior",
            Faction = "Stormcast Eternals",
            Points = 110,
            Type = UnitType.Battleline,
            Move = 5,
            Wounds = 2,
            Bravery = 7,
            Save = 4,
            ModelCount = 5,
            BaseSize = 1.0f,
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Lord-Celestant", new UnitData
        {
            Name = "Lord-Celestant",
            Faction = "Stormcast Eternals",
            Points = 160,
            Type = UnitType.Leader,
            Move = 5,
            Wounds = 5,
            Bravery = 8,
            Save = 3,
            ModelCount = 1,
            BaseSize = 1.0f,
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Knight-Incantor", new UnitData
        {
            Name = "Knight-Incantor",
            Faction = "Stormcast Eternals",
            Points = 140,
            Type = UnitType.Leader,
            Move = 5,
            Wounds = 4,
            Bravery = 7,
            Save = 4,
            ModelCount = 1,
            BaseSize = 1.0f,
            IsHero = true,
            IsGeneral = false,
            IsWizard = true,
            IsPriest = false
        });
        
        // Orruk Warclans
        AddUnitToDatabase("Orruk Brute", new UnitData
        {
            Name = "Orruk Brute",
            Faction = "Orruk Warclans",
            Points = 140,
            Type = UnitType.Battleline,
            Move = 5,
            Wounds = 3,
            Bravery = 6,
            Save = 5,
            ModelCount = 5,
            BaseSize = 1.0f,
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Orruk Warrior", new UnitData
        {
            Name = "Orruk Warrior",
            Faction = "Orruk Warclans",
            Points = 100,
            Type = UnitType.Battleline,
            Move = 5,
            Wounds = 2,
            Bravery = 6,
            Save = 5,
            ModelCount = 10,
            BaseSize = 1.0f,
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Megaboss", new UnitData
        {
            Name = "Megaboss",
            Faction = "Orruk Warclans",
            Points = 180,
            Type = UnitType.Leader,
            Move = 5,
            Wounds = 6,
            Bravery = 8,
            Save = 4,
            ModelCount = 1,
            BaseSize = 1.0f,
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Warchanter", new UnitData
        {
            Name = "Warchanter",
            Faction = "Orruk Warclans",
            Points = 120,
            Type = UnitType.Leader,
            Move = 5,
            Wounds = 4,
            Bravery = 7,
            Save = 5,
            ModelCount = 1,
            BaseSize = 1.0f,
            IsHero = true,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = true
        });
        
        // Cities of Sigmar
        AddUnitToDatabase("Freeguild Guard", new UnitData
        {
            Name = "Freeguild Guard",
            Faction = "Cities of Sigmar",
            Points = 90,
            Type = UnitType.Battleline,
            Move = 5,
            Wounds = 1,
            Bravery = 6,
            Save = 5,
            ModelCount = 10,
            BaseSize = 1.0f,
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Freeguild General", new UnitData
        {
            Name = "Freeguild General",
            Faction = "Cities of Sigmar",
            Points = 130,
            Type = UnitType.Leader,
            Move = 5,
            Wounds = 4,
            Bravery = 7,
            Save = 4,
            ModelCount = 1,
            BaseSize = 1.0f,
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Battlemage", new UnitData
        {
            Name = "Battlemage",
            Faction = "Cities of Sigmar",
            Points = 110,
            Type = UnitType.Leader,
            Move = 5,
            Wounds = 3,
            Bravery = 6,
            Save = 5,
            ModelCount = 1,
            BaseSize = 1.0f,
            IsHero = true,
            IsGeneral = false,
            IsWizard = true,
            IsPriest = false
        });
        
        // Nighthaunt
        AddUnitToDatabase("Chainrasp Horde", new UnitData
        {
            Name = "Chainrasp Horde",
            Faction = "Nighthaunt",
            Points = 110,
            Type = UnitType.Battleline,
            Move = 6,
            Wounds = 1,
            Bravery = 10,
            Save = 6,
            ModelCount = 10,
            BaseSize = 1.0f,
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Knight of Shrouds", new UnitData
        {
            Name = "Knight of Shrouds",
            Faction = "Nighthaunt",
            Points = 150,
            Type = UnitType.Leader,
            Move = 6,
            Wounds = 5,
            Bravery = 10,
            Save = 5,
            ModelCount = 1,
            BaseSize = 1.0f,
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Guardian of Souls", new UnitData
        {
            Name = "Guardian of Souls",
            Faction = "Nighthaunt",
            Points = 140,
            Type = UnitType.Leader,
            Move = 6,
            Wounds = 4,
            Bravery = 10,
            Save = 5,
            ModelCount = 1,
            BaseSize = 1.0f,
            IsHero = true,
            IsGeneral = false,
            IsWizard = true,
            IsPriest = false
        });
        
        // Sylvaneth
        AddUnitToDatabase("Dryads", new UnitData
        {
            Name = "Dryads",
            Faction = "Sylvaneth",
            Points = 100,
            Type = UnitType.Battleline,
            Move = 6,
            Wounds = 1,
            Bravery = 7,
            Save = 5,
            ModelCount = 10,
            BaseSize = 1.0f,
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Branchwych", new UnitData
        {
            Name = "Branchwych",
            Faction = "Sylvaneth",
            Points = 130,
            Type = UnitType.Leader,
            Move = 6,
            Wounds = 4,
            Bravery = 7,
            Save = 5,
            ModelCount = 1,
            BaseSize = 1.0f,
            IsHero = true,
            IsGeneral = false,
            IsWizard = true,
            IsPriest = false
        });
        
        // Khorne
        AddUnitToDatabase("Bloodreavers", new UnitData
        {
            Name = "Bloodreavers",
            Faction = "Khorne",
            Points = 90,
            Type = UnitType.Battleline,
            Move = 6,
            Wounds = 1,
            Bravery = 6,
            Save = 6,
            ModelCount = 10,
            BaseSize = 1.0f,
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });
        
        AddUnitToDatabase("Bloodsecrator", new UnitData
        {
            Name = "Bloodsecrator",
            Faction = "Khorne",
            Points = 140,
            Type = UnitType.Leader,
            Move = 5,
            Wounds = 5,
            Bravery = 8,
            Save = 4,
            ModelCount = 1,
            BaseSize = 1.0f,
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });
        
        // Tzeentch
        AddUnitToDatabase("Pink Horrors", new UnitData
        {
            Name = "Pink Horrors",
            Faction = "Tzeentch",
            Points = 130,
            Type = UnitType.Battleline,
            Move = 5,
            Wounds = 1,
            Bravery = 7,
            Save = 6,
            ModelCount = 10,
            BaseSize = 1.0f,
            IsHero = false,
            IsGeneral = false,
            IsWizard = true,
            IsPriest = false
        });
        
        AddUnitToDatabase("Lord of Change", new UnitData
        {
            Name = "Lord of Change",
            Faction = "Tzeentch",
            Points = 400,
            Type = UnitType.Leader,
            Move = 12,
            Wounds = 16,
            Bravery = 10,
            Save = 4,
            ModelCount = 1,
            BaseSize = 2.0f,
            IsHero = true,
            IsGeneral = true,
            IsWizard = true,
            IsPriest = false
        });
    }
    
    private void AddUnitToDatabase(string key, UnitData unitData)
    {
        _unitDatabase[key] = unitData;
    }
    
    public bool AddUnit(string unitName, int count = 1)
    {
        if (!_unitDatabase.ContainsKey(unitName))
        {
            GD.PrintErr($"Unit {unitName} not found in database");
            return false;
        }
        
        var unitData = _unitDatabase[unitName];
        var totalCost = unitData.Points * count;
        
        if (_currentArmy.TotalPoints + totalCost > MaxPoints)
        {
            GD.PrintErr($"Adding {unitName} would exceed point limit");
            return false;
        }
        
        var existingUnit = _currentArmy.Units.FirstOrDefault(u => u.UnitName == unitName);
        if (existingUnit != null)
        {
            existingUnit.Count += count;
        }
        else
        {
            _currentArmy.Units.Add(new ArmyUnit
            {
                UnitName = unitName,
                Count = count,
                UnitData = unitData
            });
        }
        
        _currentArmy.TotalPoints += totalCost;
        return true;
    }
    
    public bool RemoveUnit(string unitName, int count = 1)
    {
        var existingUnit = _currentArmy.Units.FirstOrDefault(u => u.UnitName == unitName);
        if (existingUnit == null)
        {
            GD.PrintErr($"Unit {unitName} not in army");
            return false;
        }
        
        if (existingUnit.Count <= count)
        {
            _currentArmy.TotalPoints -= existingUnit.UnitData.Points * existingUnit.Count;
            _currentArmy.Units.Remove(existingUnit);
        }
        else
        {
            existingUnit.Count -= count;
            _currentArmy.TotalPoints -= existingUnit.UnitData.Points * count;
        }
        
        return true;
    }
    
    public bool ValidateArmyComposition()
    {
        if (_currentArmy.TotalPoints > MaxPoints)
        {
            GD.PrintErr("Army exceeds point limit");
            return false;
        }
        
        var leaders = _currentArmy.Units.Where(u => u.UnitData.IsHero).Sum(u => u.Count);
        var battleline = _currentArmy.Units.Where(u => u.UnitData.Type == UnitType.Battleline).Sum(u => u.Count);
        
        // Basic AoS 4th Edition composition rules
        if (leaders < 1)
        {
            GD.PrintErr("Army must have at least 1 Leader");
            return false;
        }
        
        // Note: AoS 4th Edition no longer requires minimum battleline units
        // Battleline units are now optional but provide benefits
        
        return true;
    }
    
    public bool ValidateArmy()
    {
        return ValidateArmyComposition();
    }
    
    public Army GetCurrentArmy()
    {
        return _currentArmy;
    }
    
    public List<UnitData> GetUnitsByFaction(string faction)
    {
        return _unitDatabase.Values.Where(u => u.Faction == faction).ToList();
    }
    
    public List<string> GetAvailableFactions()
    {
        return _unitDatabase.Values.Select(u => u.Faction).Distinct().ToList();
    }
    
    public void ClearArmy()
    {
        _currentArmy = new Army();
    }
    
    public void SaveArmy(string filename)
    {
        // TODO: Implement army saving to file
        GD.Print($"Saving army to {filename}");
    }
    
    public void LoadArmy(string filename)
    {
        // TODO: Implement army loading from file
        GD.Print($"Loading army from {filename}");
    }
    
    public Dictionary<string, UnitData> GetUnitDatabase()
    {
        return _unitDatabase;
    }
    
    /// <summary>
    /// Get a unit with all its abilities from the comprehensive database
    /// </summary>
    public UnitData GetUnitWithAbilities(string unitName)
    {
        return UnitDatabase.GetUnit(unitName);
    }
    
    /// <summary>
    /// Get all available unit names from the comprehensive database
    /// </summary>
    public List<string> GetAllUnitNames()
    {
        return UnitDatabase.GetAllUnitNames();
    }
    
    /// <summary>
    /// Get units by faction from the comprehensive database
    /// </summary>
    public List<string> GetUnitsByFaction(string faction)
    {
        return UnitDatabase.GetUnitsByFaction(faction);
    }
    
    /// <summary>
    /// Create a unit instance with abilities from the database
    /// </summary>
    public Unit CreateUnitInstance(string unitName, int playerId, Vector3 position)
    {
        var unitData = UnitDatabase.GetUnit(unitName);
        if (unitData == null) return null;
        
        var unit = new Unit();
        unit.UnitName = unitData.Name;
        unit.PlayerId = playerId;
        unit.ArmyName = unitData.Faction;
        unit.Move = unitData.Move;
        unit.Wounds = unitData.Wounds;
        unit.MaxWounds = unitData.Wounds;
        unit.Bravery = unitData.Bravery;
        unit.Save = unitData.Save;
        unit.Attacks = unitData.Attacks;
        unit.ToHit = unitData.ToHit;
        unit.ToWound = unitData.ToWound;
        unit.Rend = unitData.Rend;
        unit.Damage = unitData.Damage;
        unit.ModelCount = unitData.ModelCount;
        unit.BaseSize = unitData.BaseSize;
        unit.IsHero = unitData.IsHero;
        unit.IsGeneral = unitData.IsGeneral;
        unit.IsWizard = unitData.IsWizard;
        unit.IsPriest = unitData.IsPriest;
        unit.Position = position;
        
        // Add abilities from the database
        var unitAbilities = UnitDatabase.GetUnitAbilities(unitName);
        foreach (var ability in unitAbilities)
        {
            unit.AddUnitAbility(ability);
        }
        
        var modelAbilities = UnitDatabase.GetModelAbilities(unitName);
        foreach (var ability in modelAbilities)
        {
            unit.AddModelAbility(ability);
        }
        
        // Set model-specific properties based on abilities
        foreach (var ability in modelAbilities)
        {
            if (ability.RequiresChampion) unit.HasChampion = true;
            if (ability.RequiresMusician) unit.HasMusician = true;
            if (ability.RequiresBannerBearer) unit.HasBannerBearer = true;
            if (ability.RequiresSpecialWeapon) unit.HasSpecialWeapon = true;
        }
        
        return unit;
    }
}

public class Army
{
    public List<ArmyUnit> Units { get; set; } = new List<ArmyUnit>();
    public int TotalPoints { get; set; } = 0;
    public string Name { get; set; } = "My Army";
    public string Faction { get; set; } = "";
}

public class ArmyUnit
{
    public string UnitName { get; set; }
    public int Count { get; set; }
    public UnitData UnitData { get; set; }
}

public class UnitData
{
    public string Name { get; set; }
    public string Faction { get; set; }
    public int Points { get; set; }
    public UnitType Type { get; set; }
    public int Move { get; set; }
    public int Wounds { get; set; }
    public int Bravery { get; set; }
    public int Save { get; set; }
    public int ModelCount { get; set; }
    public float BaseSize { get; set; }
    public bool IsHero { get; set; }
    public bool IsGeneral { get; set; }
    public bool IsWizard { get; set; }
    public bool IsPriest { get; set; }
}

public enum UnitType
{
    Leader,
    Battleline,
    Other
}
