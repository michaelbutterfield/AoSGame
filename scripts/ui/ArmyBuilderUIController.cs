using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class ArmyBuilderUIController : Control
{
    [Export] public PackedScene GameScene;
    [Export] public PackedScene MainMenuScene;
    
    // UI References
    private OptionButton _factionOptionButton;
    private ItemList _unitList;
    private ItemList _armyList;
    private ItemList _validationList;
    
    // Unit Details
    private Label _unitNameLabel;
    private Label _pointsValue;
    private Label _moveValue;
    private Label _woundsValue;
    private Label _saveValue;
    private Label _braveryValue;
    
    // Army Info
    private Label _totalPointsValue;
    private Label _validationLabel;
    private Label _leadersLabel;
    private Label _battlelineLabel;
    private Label _otherLabel;
    
    // Buttons
    private Button _addUnitButton;
    private Button _removeUnitButton;
    private Button _clearArmyButton;
    private Button _saveArmyButton;
    private Button _loadArmyButton;
    private Button _validateButton;
    private Button _startGameButton;
    private Button _backButton;
    
    // State
    private string _selectedFaction = "";
    private string _selectedUnit = "";
    private int _selectedArmyIndex = -1;
    private List<string> _availableUnits = new List<string>();
    
    public override void _Ready()
    {
        GetNodeReferences();
        ConnectSignals();
        InitializeUI();
    }
    
    private void GetNodeReferences()
    {
        _factionOptionButton = GetNode<OptionButton>("MainContainer/LeftPanel/FactionSelection/FactionOptionButton");
        _unitList = GetNode<ItemList>("MainContainer/LeftPanel/UnitSelection/UnitList");
        _armyList = GetNode<ItemList>("MainContainer/CenterPanel/ArmyList");
        _validationList = GetNode<ItemList>("MainContainer/RightPanel/ValidationPanel/ValidationList");
        
        _unitNameLabel = GetNode<Label>("MainContainer/LeftPanel/UnitDetails/UnitNameLabel");
        _pointsValue = GetNode<Label>("MainContainer/LeftPanel/UnitDetails/UnitStats/PointsValue");
        _moveValue = GetNode<Label>("MainContainer/LeftPanel/UnitDetails/UnitStats/MoveValue");
        _woundsValue = GetNode<Label>("MainContainer/LeftPanel/UnitDetails/UnitStats/WoundsValue");
        _saveValue = GetNode<Label>("MainContainer/LeftPanel/UnitDetails/UnitStats/SaveValue");
        _braveryValue = GetNode<Label>("MainContainer/LeftPanel/UnitDetails/UnitStats/BraveryValue");
        
        _totalPointsValue = GetNode<Label>("MainContainer/CenterPanel/ArmyInfo/PointsValue");
        _validationLabel = GetNode<Label>("MainContainer/CenterPanel/ArmyInfo/ValidationLabel");
        _leadersLabel = GetNode<Label>("MainContainer/RightPanel/ArmyComposition/LeadersLabel");
        _battlelineLabel = GetNode<Label>("MainContainer/RightPanel/ArmyComposition/BattlelineLabel");
        _otherLabel = GetNode<Label>("MainContainer/RightPanel/ArmyComposition/OtherLabel");
        
        _addUnitButton = GetNode<Button>("MainContainer/LeftPanel/AddUnitButton");
        _removeUnitButton = GetNode<Button>("MainContainer/CenterPanel/ArmyActions/RemoveUnitButton");
        _clearArmyButton = GetNode<Button>("MainContainer/CenterPanel/ArmyActions/ClearArmyButton");
        _saveArmyButton = GetNode<Button>("MainContainer/CenterPanel/ArmyActions/SaveArmyButton");
        _loadArmyButton = GetNode<Button>("MainContainer/CenterPanel/ArmyActions/LoadArmyButton");
        _validateButton = GetNode<Button>("MainContainer/RightPanel/ActionButtons/ValidateButton");
        _startGameButton = GetNode<Button>("MainContainer/RightPanel/ActionButtons/StartGameButton");
        _backButton = GetNode<Button>("MainContainer/RightPanel/ActionButtons/BackButton");
    }
    
    private void ConnectSignals()
    {
        _factionOptionButton.ItemSelected += OnFactionSelected;
        _unitList.ItemSelected += OnUnitSelected;
        _armyList.ItemSelected += OnArmyItemSelected;
        
        _addUnitButton.Pressed += OnAddUnitPressed;
        _removeUnitButton.Pressed += OnRemoveUnitPressed;
        _clearArmyButton.Pressed += OnClearArmyPressed;
        _saveArmyButton.Pressed += OnSaveArmyPressed;
        _loadArmyButton.Pressed += OnLoadArmyPressed;
        _validateButton.Pressed += OnValidatePressed;
        _startGameButton.Pressed += OnStartGamePressed;
        _backButton.Pressed += OnBackPressed;
    }
    
    private void InitializeUI()
    {
        PopulateFactions();
        UpdateArmyDisplay();
        UpdateValidation();
    }
    
    private void PopulateFactions()
    {
        _factionOptionButton.Clear();
        _factionOptionButton.AddItem("All Factions", -1);
        
        var factions = ArmyBuilder.Instance.GetAvailableFactions();
        foreach (var faction in factions)
        {
            _factionOptionButton.AddItem(faction);
        }
        
        _factionOptionButton.Selected = 0;
        OnFactionSelected(0);
    }
    
    private void OnFactionSelected(int index)
    {
        if (index == 0) // All Factions
        {
            _selectedFaction = "";
        }
        else
        {
            _selectedFaction = _factionOptionButton.GetItemText(index);
        }
        
        PopulateUnitList();
    }
    
    private void PopulateUnitList()
    {
        _unitList.Clear();
        _availableUnits.Clear();
        
        var unitDatabase = ArmyBuilder.Instance.GetUnitDatabase();
        var units = unitDatabase.Values
            .Where(u => string.IsNullOrEmpty(_selectedFaction) || u.Faction == _selectedFaction)
            .OrderBy(u => u.Faction)
            .ThenBy(u => u.Name)
            .ToList();
        
        foreach (var unit in units)
        {
            var displayText = $"{unit.Name} ({unit.Faction}) - {unit.Points}pts";
            _unitList.AddItem(displayText);
            _availableUnits.Add(unit.Name);
        }
    }
    
    private void OnUnitSelected(int index)
    {
        if (index >= 0 && index < _availableUnits.Count)
        {
            _selectedUnit = _availableUnits[index];
            DisplayUnitDetails(_selectedUnit);
        }
    }
    
    private void DisplayUnitDetails(string unitName)
    {
        var unitDatabase = ArmyBuilder.Instance.GetUnitDatabase();
        if (!unitDatabase.ContainsKey(unitName))
            return;
        
        var unit = unitDatabase[unitName];
        
        _unitNameLabel.Text = unit.Name;
        _pointsValue.Text = unit.Points.ToString();
        _moveValue.Text = unit.Move.ToString();
        _woundsValue.Text = unit.Wounds.ToString();
        _saveValue.Text = unit.Save.ToString();
        _braveryValue.Text = unit.Bravery.ToString();
    }
    
    private void OnAddUnitPressed()
    {
        if (string.IsNullOrEmpty(_selectedUnit))
        {
            GD.Print("No unit selected");
            return;
        }
        
        if (ArmyBuilder.Instance.AddUnit(_selectedUnit))
        {
            UpdateArmyDisplay();
            UpdateValidation();
            GD.Print($"Added {_selectedUnit} to army");
        }
        else
        {
            GD.PrintErr($"Failed to add {_selectedUnit} to army");
        }
    }
    
    private void OnArmyItemSelected(int index)
    {
        _selectedArmyIndex = index;
    }
    
    private void OnRemoveUnitPressed()
    {
        if (_selectedArmyIndex < 0)
        {
            GD.Print("No army unit selected");
            return;
        }
        
        var army = ArmyBuilder.Instance.GetCurrentArmy();
        if (_selectedArmyIndex < army.Units.Count)
        {
            var unit = army.Units[_selectedArmyIndex];
            if (ArmyBuilder.Instance.RemoveUnit(unit.UnitName))
            {
                UpdateArmyDisplay();
                UpdateValidation();
                _selectedArmyIndex = -1;
                GD.Print($"Removed {unit.UnitName} from army");
            }
        }
    }
    
    private void OnClearArmyPressed()
    {
        ArmyBuilder.Instance.ClearArmy();
        UpdateArmyDisplay();
        UpdateValidation();
        _selectedArmyIndex = -1;
        GD.Print("Army cleared");
    }
    
    private void UpdateArmyDisplay()
    {
        var army = ArmyBuilder.Instance.GetCurrentArmy();
        
        _armyList.Clear();
        foreach (var unit in army.Units)
        {
            var displayText = $"{unit.UnitName} x{unit.Count} ({unit.UnitData.Points * unit.Count}pts)";
            _armyList.AddItem(displayText);
        }
        
        _totalPointsValue.Text = $"{army.TotalPoints}/{ArmyBuilder.Instance.MaxPoints}";
        
        UpdateCompositionDisplay();
    }
    
    private void UpdateCompositionDisplay()
    {
        var army = ArmyBuilder.Instance.GetCurrentArmy();
        
        var leaders = army.Units.Where(u => u.UnitData.IsHero).Sum(u => u.Count);
        var battleline = army.Units.Where(u => u.UnitData.Type == UnitType.Battleline).Sum(u => u.Count);
        var other = army.Units.Where(u => !u.UnitData.IsHero && u.UnitData.Type != UnitType.Battleline).Sum(u => u.Count);
        
        _leadersLabel.Text = $"Leaders: {leaders}";
        _battlelineLabel.Text = $"Battleline: {battleline}";
        _otherLabel.Text = $"Other: {other}";
    }
    
    private void UpdateValidation()
    {
        var isValid = ArmyBuilder.Instance.ValidateArmy();
        _validationLabel.Text = isValid ? "Valid" : "Invalid";
        _validationLabel.Modulate = isValid ? Colors.Green : Colors.Red;
        
        _startGameButton.Disabled = !isValid;
        
        UpdateValidationList();
    }
    
    private void UpdateValidationList()
    {
        _validationList.Clear();
        
        var army = ArmyBuilder.Instance.GetCurrentArmy();
        
        // Check point limit
        if (army.TotalPoints > ArmyBuilder.Instance.MaxPoints)
        {
            _validationList.AddItem("❌ Army exceeds point limit", null, false);
        }
        else if (army.TotalPoints < ArmyBuilder.Instance.MaxPoints * 0.8f)
        {
            _validationList.AddItem("⚠️ Army is under minimum points", null, false);
        }
        else
        {
            _validationList.AddItem("✅ Points within limit", null, false);
        }
        
        // Check leaders
        var leaders = army.Units.Where(u => u.UnitData.IsHero).Sum(u => u.Count);
        if (leaders < 1)
        {
            _validationList.AddItem("❌ Need at least 1 Leader", null, false);
        }
        else
        {
            _validationList.AddItem($"✅ Leaders: {leaders}", null, false);
        }
        
        // Check battleline (optional in AoS 4th Edition)
        var battleline = army.Units.Where(u => u.UnitData.Type == UnitType.Battleline).Sum(u => u.Count);
        if (battleline > 0)
        {
            _validationList.AddItem($"✅ Battleline: {battleline} (provides benefits)", null, false);
        }
        else
        {
            _validationList.AddItem("ℹ️ No battleline units (optional in AoS 4th Edition)", null, false);
        }
    }
    
    private void OnValidatePressed()
    {
        UpdateValidation();
        GD.Print("Army validation updated");
    }
    
    private void OnSaveArmyPressed()
    {
        // TODO: Implement save dialog
        ArmyBuilder.Instance.SaveArmy("my_army");
        GD.Print("Army saved");
    }
    
    private void OnLoadArmyPressed()
    {
        // TODO: Implement load dialog
        ArmyBuilder.Instance.LoadArmy("my_army");
        UpdateArmyDisplay();
        UpdateValidation();
        GD.Print("Army loaded");
    }
    
    private void OnStartGamePressed()
    {
        if (!ArmyBuilder.Instance.ValidateArmy())
        {
            GD.PrintErr("Cannot start game with invalid army");
            return;
        }
        
        // Set up the game with the current army
        GameManager.Instance.SetPlayerArmy(ArmyBuilder.Instance.GetCurrentArmy());
        
        if (GameScene != null)
        {
            GetTree().ChangeSceneToPacked(GameScene);
        }
        else
        {
            GD.PrintErr("GameScene not assigned");
        }
    }
    
    private void OnBackPressed()
    {
        if (MainMenuScene != null)
        {
            GetTree().ChangeSceneToPacked(MainMenuScene);
        }
        else
        {
            GetTree().Quit();
        }
    }
}
