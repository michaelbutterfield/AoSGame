using Godot;
using System.Collections.Generic;

public partial class CommandPointTrackerController : Control
{
    // UI References
    private Label _currentPointsValue;
    private Label _maxPointsValue;
    private Label _generatedThisTurn;
    private VBoxContainer _availableAbilitiesList;
    private VBoxContainer _usedAbilitiesList;
    private Label _selectedAbilityName;
    private Label _selectedAbilityDescription;
    private Label _selectedAbilityCost;
    private Button _useAbilityButton;

    // State
    private string _selectedAbility = "";
    private int _currentPlayerId = 1;

    public override void _Ready()
    {
        GetNodeReferences();
        ConnectSignals();
        UpdateDisplay();
    }

    private void GetNodeReferences()
    {
        _currentPointsValue = GetNode<Label>("VBoxContainer/CommandPointsDisplay/CurrentPointsValue");
        _maxPointsValue = GetNode<Label>("VBoxContainer/CommandPointsDisplay/MaxPointsValue");
        _generatedThisTurn = GetNode<Label>("VBoxContainer/GeneratedThisTurn");
        _availableAbilitiesList = GetNode<VBoxContainer>("VBoxContainer/AvailableAbilitiesList");
        _usedAbilitiesList = GetNode<VBoxContainer>("VBoxContainer/UsedAbilitiesList");
        _selectedAbilityName = GetNode<Label>("VBoxContainer/CommandAbilityDetails/SelectedAbilityName");
        _selectedAbilityDescription = GetNode<Label>("VBoxContainer/CommandAbilityDetails/SelectedAbilityDescription");
        _selectedAbilityCost = GetNode<Label>("VBoxContainer/CommandAbilityDetails/SelectedAbilityCost");
        _useAbilityButton = GetNode<Button>("VBoxContainer/CommandAbilityDetails/UseAbilityButton");
    }

    private void ConnectSignals()
    {
        if (CommandPointManager.Instance != null)
        {
            CommandPointManager.Instance.CommandPointsChanged += OnCommandPointsChanged;
            CommandPointManager.Instance.CommandAbilityUsed += OnCommandAbilityUsed;
            CommandPointManager.Instance.CommandPointsGenerated += OnCommandPointsGenerated;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerTurnChanged += OnPlayerTurnChanged;
            GameManager.Instance.TurnPhaseChanged += OnTurnPhaseChanged;
        }

        _useAbilityButton.Pressed += OnUseAbilityPressed;
    }

    private void OnCommandPointsChanged(int playerId, int currentPoints, int maxPoints)
    {
        if (playerId == _currentPlayerId)
        {
            UpdateCommandPointsDisplay(currentPoints, maxPoints);
        }
    }

    private void OnCommandAbilityUsed(int playerId, string abilityName, int cost)
    {
        if (playerId == _currentPlayerId)
        {
            UpdateAbilityLists();
            ClearSelectedAbility();
        }
    }

    private void OnCommandPointsGenerated(int playerId, int pointsGenerated, string source)
    {
        if (playerId == _currentPlayerId)
        {
            UpdateGeneratedThisTurn(pointsGenerated);
        }
    }

    private void OnPlayerTurnChanged(int playerId)
    {
        _currentPlayerId = playerId;
        UpdateDisplay();
    }

    private void OnTurnPhaseChanged(GameManager.TurnPhase newPhase)
    {
        UpdateAbilityLists();
    }

    private void OnUseAbilityPressed()
    {
        if (string.IsNullOrEmpty(_selectedAbility)) return;

        // For now, we'll use the ability on the first available friendly unit
        // In a full implementation, this would open a target selection UI
        var playerUnits = GameManager.Instance.GetPlayerUnits(_currentPlayerId);
        if (playerUnits.Count > 0)
        {
            var targetUnit = playerUnits[0]; // Default to first unit
            bool success = CommandPointManager.Instance.UseCommandAbility(_currentPlayerId, _selectedAbility, targetUnit);
            
            if (success)
            {
                GD.Print($"Used command ability '{_selectedAbility}' on {targetUnit.UnitName}");
            }
            else
            {
                GD.PrintErr($"Failed to use command ability '{_selectedAbility}'");
            }
        }
    }

    private void UpdateDisplay()
    {
        if (CommandPointManager.Instance == null) return;

        var playerCP = CommandPointManager.Instance.GetPlayerCommandPoints(_currentPlayerId);
        if (playerCP != null)
        {
            UpdateCommandPointsDisplay(playerCP.CurrentPoints, playerCP.MaxPoints);
            UpdateGeneratedThisTurn(playerCP.PointsGeneratedThisTurn);
        }

        UpdateAbilityLists();
        ClearSelectedAbility();
    }

    private void UpdateCommandPointsDisplay(int currentPoints, int maxPoints)
    {
        _currentPointsValue.Text = currentPoints.ToString();
        _maxPointsValue.Text = maxPoints.ToString();
    }

    private void UpdateGeneratedThisTurn(int pointsGenerated)
    {
        _generatedThisTurn.Text = $"Generated this turn: {pointsGenerated}";
    }

    private void UpdateAbilityLists()
    {
        if (CommandPointManager.Instance == null) return;

        // Clear existing lists
        foreach (var child in _availableAbilitiesList.GetChildren())
        {
            child.QueueFree();
        }
        foreach (var child in _usedAbilitiesList.GetChildren())
        {
            child.QueueFree();
        }

        // Populate available abilities
        var availableAbilities = CommandPointManager.Instance.GetAvailableCommandAbilities(_currentPlayerId);
        foreach (var ability in availableAbilities)
        {
            var button = new Button();
            button.Text = $"{ability.Name} ({ability.Cost} CP)";
            button.Pressed += () => OnAbilitySelected(ability.Name);
            _availableAbilitiesList.AddChild(button);
        }

        // Populate used abilities
        var usedAbilities = CommandPointManager.Instance.GetUsedAbilitiesThisTurn(_currentPlayerId);
        foreach (var abilityName in usedAbilities)
        {
            var label = new Label();
            label.Text = abilityName;
            label.Modulate = new Color(0.7f, 0.7f, 0.7f); // Grayed out
            _usedAbilitiesList.AddChild(label);
        }
    }

    private void OnAbilitySelected(string abilityName)
    {
        _selectedAbility = abilityName;
        UpdateSelectedAbilityDetails();
    }

    private void UpdateSelectedAbilityDetails()
    {
        if (string.IsNullOrEmpty(_selectedAbility))
        {
            ClearSelectedAbility();
            return;
        }

        var allAbilities = CommandPointManager.Instance.GetAllCommandAbilities();
        if (allAbilities.ContainsKey(_selectedAbility))
        {
            var ability = allAbilities[_selectedAbility];
            _selectedAbilityName.Text = ability.Name;
            _selectedAbilityDescription.Text = ability.Description;
            _selectedAbilityCost.Text = $"Cost: {ability.Cost} Command Point{(ability.Cost > 1 ? "s" : "")}";

            // Check if we can use this ability
            var playerCP = CommandPointManager.Instance.GetPlayerCommandPoints(_currentPlayerId);
            bool canUse = playerCP != null && 
                         playerCP.CurrentPoints >= ability.Cost &&
                         ability.Phase == GameManager.Instance.CurrentTurnPhase;

            _useAbilityButton.Disabled = !canUse;
        }
    }

    private void ClearSelectedAbility()
    {
        _selectedAbility = "";
        _selectedAbilityName.Text = "Select an ability for details";
        _selectedAbilityDescription.Text = "";
        _selectedAbilityCost.Text = "";
        _useAbilityButton.Disabled = true;
    }

    public void SetCurrentPlayer(int playerId)
    {
        _currentPlayerId = playerId;
        UpdateDisplay();
    }

    public void Show()
    {
        Visible = true;
        UpdateDisplay();
    }

    public void Hide()
    {
        Visible = false;
    }

    public void Toggle()
    {
        if (Visible)
            Hide();
        else
            Show();
    }
}
