using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EnhancedArmyBuilderUI : Control
{
    [Export] private ItemList _unitList;
    [Export] private ItemList _armyList;
    [Export] private Label _pointsLabel;
    [Export] private Label _validationLabel;
    [Export] private Button _addUnitButton;
    [Export] private Button _removeUnitButton;
    [Export] private Button _createRegimentButton;
    [Export] private Button _saveArmyButton;
    [Export] private Button _loadArmyButton;
    [Export] private OptionButton _factionSelector;
    [Export] private ItemList _regimentList;
    [Export] private Label _unitDetailsLabel;
    [Export] private Label _measurementsLabel;
    [Export] private Label _abilitiesLabel;

    private ArmyBuilder _armyBuilder;
    private RegimentOfRenownManager _regimentManager;
    private List<string> _selectedUnits = new List<string>();
    private List<RegimentOfRenown> _availableRegiments = new List<RegimentOfRenown>();

    public override void _Ready()
    {
        _armyBuilder = ArmyBuilder.Instance;
        _regimentManager = RegimentOfRenownManager.Instance;
        
        SetupUI();
        ConnectSignals();
        InitializeFactionSelector();
        RefreshUnitList();
    }

    private void SetupUI()
    {
        if (_unitList != null)
        {
            _unitList.AllowReselect = true;
            _unitList.AllowRmbSelect = true;
        }

        if (_armyList != null)
        {
            _armyList.AllowReselect = true;
            _armyList.AllowRmbSelect = true;
        }

        if (_regimentList != null)
        {
            _regimentList.AllowReselect = true;
        }
    }

    private void ConnectSignals()
    {
        if (_addUnitButton != null)
            _addUnitButton.Pressed += OnAddUnitPressed;
        
        if (_removeUnitButton != null)
            _removeUnitButton.Pressed += OnRemoveUnitPressed;
        
        if (_createRegimentButton != null)
            _createRegimentButton.Pressed += OnCreateRegimentPressed;
        
        if (_saveArmyButton != null)
            _saveArmyButton.Pressed += OnSaveArmyPressed;
        
        if (_loadArmyButton != null)
            _loadArmyButton.Pressed += OnLoadArmyPressed;
        
        if (_factionSelector != null)
            _factionSelector.ItemSelected += OnFactionSelected;
        
        if (_unitList != null)
            _unitList.ItemSelected += OnUnitSelected;
        
        if (_armyList != null)
            _armyList.ItemSelected += OnArmyUnitSelected;
        
        if (_regimentList != null)
            _regimentList.ItemSelected += OnRegimentSelected;
    }

    private void InitializeFactionSelector()
    {
        if (_factionSelector == null) return;

        _factionSelector.Clear();
        _factionSelector.AddItem("All Factions", -1);
        
        var factions = UnitDatabase.GetAvailableFactions();
        foreach (var faction in factions)
        {
            _factionSelector.AddItem(faction);
        }
        
        _factionSelector.Selected = 0;
    }

    private void RefreshUnitList()
    {
        if (_unitList == null) return;

        _unitList.Clear();
        var selectedFaction = _factionSelector?.GetItemText(_factionSelector.Selected) ?? "All Factions";
        
        List<string> units;
        if (selectedFaction == "All Factions")
        {
            units = UnitDatabase.GetAllUnitNames();
        }
        else
        {
            units = UnitDatabase.GetUnitsByFaction(selectedFaction);
        }

        foreach (var unitName in units.OrderBy(u => u))
        {
            var unitData = UnitDatabase.GetUnit(unitName);
            if (unitData != null)
            {
                var itemText = $"{unitName} ({unitData.Points}pts)";
                var itemIndex = _unitList.AddItem(itemText);
                
                // Store unit data in item metadata
                _unitList.SetItemMetadata(itemIndex, unitData);
            }
        }
    }

    private void RefreshArmyList()
    {
        if (_armyList == null) return;

        _armyList.Clear();
        var army = _armyBuilder.GetCurrentArmy();
        
        foreach (var unitEntry in army.Units)
        {
            var unitData = unitEntry.UnitData;
            var itemText = $"{unitData.Name} x{unitEntry.Count} ({unitData.Points * unitEntry.Count}pts)";
            var itemIndex = _armyList.AddItem(itemText);
            _armyList.SetItemMetadata(itemIndex, unitEntry);
        }
        
        UpdatePointsDisplay();
        UpdateValidationDisplay();
    }

    private void RefreshRegimentList()
    {
        if (_regimentList == null) return;

        _regimentList.Clear();
        var selectedFaction = _factionSelector?.GetItemText(_factionSelector.Selected) ?? "All Factions";
        
        if (selectedFaction != "All Factions")
        {
            _availableRegiments = _regimentManager.GetRegimentsForFaction(selectedFaction);
            foreach (var regiment in _availableRegiments)
            {
                var itemText = $"{regiment.Name} ({regiment.MinUnitCount}-{regiment.MaxUnitCount} units)";
                var itemIndex = _regimentList.AddItem(itemText);
                _regimentList.SetItemMetadata(itemIndex, regiment);
            }
        }
    }

    private void UpdatePointsDisplay()
    {
        if (_pointsLabel == null) return;

        var army = _armyBuilder.GetCurrentArmy();
        var totalPoints = army.GetTotalPoints();
        var maxPoints = army.MaxPoints;
        
        _pointsLabel.Text = $"Points: {totalPoints}/{maxPoints}";
        
        // Color coding for points
        if (totalPoints > maxPoints)
        {
            _pointsLabel.Modulate = new Color(1, 0, 0); // Red for over points
        }
        else if (totalPoints == maxPoints)
        {
            _pointsLabel.Modulate = new Color(0, 1, 0); // Green for exact points
        }
        else
        {
            _pointsLabel.Modulate = new Color(1, 1, 1); // White for under points
        }
    }

    private void UpdateValidationDisplay()
    {
        if (_validationLabel == null) return;

        var army = _armyBuilder.GetCurrentArmy();
        var isValid = _armyBuilder.ValidateArmy();
        
        if (isValid)
        {
            _validationLabel.Text = "✅ Army is valid!";
            _validationLabel.Modulate = new Color(0, 1, 0);
        }
        else
        {
            _validationLabel.Text = "❌ Army is invalid!";
            _validationLabel.Modulate = new Color(1, 0, 0);
        }
    }

    private void UpdateUnitDetails(string unitName)
    {
        if (_unitDetailsLabel == null) return;

        var unitData = UnitDatabase.GetUnit(unitName);
        if (unitData == null) return;

        var details = $"Unit: {unitData.Name}\n";
        details += $"Faction: {unitData.Faction}\n";
        details += $"Type: {unitData.Type}\n";
        details += $"Points: {unitData.Points}\n";
        details += $"Models: {unitData.ModelCount}\n";
        details += $"Move: {unitData.Move}\"\n";
        details += $"Wounds: {unitData.Wounds}\n";
        details += $"Bravery: {unitData.Bravery}\n";
        details += $"Save: {unitData.Save}+\n";
        details += $"Attacks: {unitData.Attacks}\n";
        details += $"To Hit: {unitData.ToHit}+\n";
        details += $"To Wound: {unitData.ToWound}+\n";
        details += $"Rend: {unitData.Rend}\n";
        details += $"Damage: {unitData.Damage}";

        _unitDetailsLabel.Text = details;
    }

    private void UpdateMeasurements(string unitName)
    {
        if (_measurementsLabel == null) return;

        var unitData = UnitDatabase.GetUnit(unitName);
        if (unitData == null) return;

        var measurements = $"Base Size: {unitData.BaseSize}mm ({unitData.BaseSize / 25.4f:F1}\")\n";
        measurements += $"Footprint Radius: {unitData.BaseSize / 25.4f / 2.0f:F1}\"\n";
        measurements += $"Engagement Range: {unitData.BaseSize / 25.4f + 1.0f:F1}\"\n";
        measurements += $"Charge Range: {unitData.Move + 6}\"\n";
        measurements += $"Run Range: {unitData.Move + 6}\"";

        if (unitData.IsWizard)
        {
            measurements += $"\nSpell Range: 18\"";
        }

        _measurementsLabel.Text = measurements;
    }

    private void UpdateAbilities(string unitName)
    {
        if (_abilitiesLabel == null) return;

        var unitAbilities = UnitDatabase.GetUnitAbilities(unitName);
        var modelAbilities = UnitDatabase.GetModelAbilities(unitName);
        
        var abilities = "";
        
        if (unitAbilities.Count > 0)
        {
            abilities += "Unit Abilities:\n";
            foreach (var ability in unitAbilities)
            {
                abilities += $"• {ability.Name}: {ability.Description}\n";
            }
        }
        
        if (modelAbilities.Count > 0)
        {
            abilities += "\nModel Abilities:\n";
            foreach (var ability in modelAbilities)
            {
                abilities += $"• {ability.Name}: {ability.Description}\n";
            }
        }
        
        if (abilities == "")
        {
            abilities = "No special abilities";
        }
        
        _abilitiesLabel.Text = abilities;
    }

    // Signal handlers
    private void OnFactionSelected(int index)
    {
        RefreshUnitList();
        RefreshRegimentList();
    }

    private void OnUnitSelected(int index)
    {
        if (_unitList == null || index < 0) return;

        var unitData = _unitList.GetItemMetadata(index) as UnitData;
        if (unitData != null)
        {
            UpdateUnitDetails(unitData.Name);
            UpdateMeasurements(unitData.Name);
            UpdateAbilities(unitData.Name);
        }
    }

    private void OnArmyUnitSelected(int index)
    {
        if (_armyList == null || index < 0) return;

        var unitEntry = _armyList.GetItemMetadata(index) as ArmyUnitEntry;
        if (unitEntry != null)
        {
            UpdateUnitDetails(unitEntry.UnitData.Name);
            UpdateMeasurements(unitEntry.UnitData.Name);
            UpdateAbilities(unitEntry.UnitData.Name);
        }
    }

    private void OnRegimentSelected(int index)
    {
        if (_regimentList == null || index < 0) return;

        var regiment = _regimentList.GetItemMetadata(index) as RegimentOfRenown;
        if (regiment != null)
        {
            // Show regiment details
            var details = $"Regiment: {regiment.Name}\n";
            details += $"Faction: {regiment.Faction}\n";
            details += $"Description: {regiment.Description}\n";
            details += $"Units: {string.Join(", ", regiment.Units)}\n";
            details += $"Special Rules: {string.Join(", ", regiment.SpecialRules)}\n";
            details += $"Min Units: {regiment.MinUnitCount}, Max Units: {regiment.MaxUnitCount}\n";
            details += $"Required: {string.Join(", ", regiment.RequiredUnits)}\n";
            details += $"Optional: {string.Join(", ", regiment.OptionalUnits)}";
            
            if (_unitDetailsLabel != null)
                _unitDetailsLabel.Text = details;
        }
    }

    private void OnAddUnitPressed()
    {
        if (_unitList == null) return;

        var selectedIndex = _unitList.GetSelectedItems().FirstOrDefault();
        if (selectedIndex < 0) return;

        var unitData = _unitList.GetItemMetadata(selectedIndex) as UnitData;
        if (unitData != null)
        {
            _armyBuilder.AddUnit(unitData.Name);
            RefreshArmyList();
        }
    }

    private void OnRemoveUnitPressed()
    {
        if (_armyList == null) return;

        var selectedIndex = _armyList.GetSelectedItems().FirstOrDefault();
        if (selectedIndex < 0) return;

        var unitEntry = _armyList.GetItemMetadata(selectedIndex) as ArmyUnitEntry;
        if (unitEntry != null)
        {
            _armyBuilder.RemoveUnit(unitEntry.UnitData.Name);
            RefreshArmyList();
        }
    }

    private void OnCreateRegimentPressed()
    {
        if (_regimentList == null) return;

        var selectedIndex = _regimentList.GetSelectedItems().FirstOrDefault();
        if (selectedIndex < 0) return;

        var regiment = _regimentList.GetItemMetadata(selectedIndex) as RegimentOfRenown;
        if (regiment != null)
        {
            // Show regiment creation dialog
            ShowRegimentCreationDialog(regiment);
        }
    }

    private void OnSaveArmyPressed()
    {
        var army = _armyBuilder.GetCurrentArmy();
        var filename = $"army_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        
        if (_armyBuilder.SaveArmy(filename))
        {
            GD.Print($"Army saved to {filename}");
        }
        else
        {
            GD.PrintErr("Failed to save army");
        }
    }

    private void OnLoadArmyPressed()
    {
        // Show file dialog for loading
        ShowLoadArmyDialog();
    }

    private void ShowRegimentCreationDialog(RegimentOfRenown regiment)
    {
        // This would show a dialog to select units for the regiment
        // For now, just print the regiment info
        GD.Print($"Creating regiment: {regiment.Name}");
        GD.Print($"Required units: {string.Join(", ", regiment.RequiredUnits)}");
        GD.Print($"Optional units: {string.Join(", ", regiment.OptionalUnits)}");
    }

    private void ShowLoadArmyDialog()
    {
        // This would show a file dialog for loading armies
        GD.Print("Load army dialog would appear here");
    }

    // Public methods for external access
    public void RefreshAll()
    {
        RefreshUnitList();
        RefreshArmyList();
        RefreshRegimentList();
    }

    public void SetFaction(string faction)
    {
        if (_factionSelector == null) return;

        for (int i = 0; i < _factionSelector.ItemCount; i++)
        {
            if (_factionSelector.GetItemText(i) == faction)
            {
                _factionSelector.Selected = i;
                OnFactionSelected(i);
                break;
            }
        }
    }
}
