using Godot;
using System;

public partial class GameSceneController : Node3D
{
    [Export]
    public PackedScene UnitScene { get; set; }

    private Label _turnLabel;
    private Label _phaseLabel;
    private Label _playerLabel;
    private Button _nextPhaseButton;
    private Panel _unitInfo;
    private Label _unitNameLabel;
    private Label _statsLabel;
    private Label _combatLabel;
    private Button _moveButton;
    private Button _chargeButton;
    private Button _attackButton;
    private Panel _diceResults;
    private Label _diceResultsLabel;
    private CommandPointTrackerController _commandPointTracker;
    private Button _commandPointsButton;
    private UnitAbilityUIController _unitAbilityUI;
    private Button _unitAbilitiesButton;

    public override void _Ready()
    {
        SetupUI();
        ConnectSignals();
        InitializeGame();
    }

    private void SetupUI()
    {
        _turnLabel = GetNode<Label>("UI/TurnInfo/VBoxContainer/TurnLabel");
        _phaseLabel = GetNode<Label>("UI/TurnInfo/VBoxContainer/PhaseLabel");
        _playerLabel = GetNode<Label>("UI/TurnInfo/VBoxContainer/PlayerLabel");
        _nextPhaseButton = GetNode<Button>("UI/TurnInfo/VBoxContainer/NextPhaseButton");
        
        _unitInfo = GetNode<Panel>("UI/UnitInfo");
        _unitNameLabel = GetNode<Label>("UI/UnitInfo/VBoxContainer/UnitNameLabel");
        _statsLabel = GetNode<Label>("UI/UnitInfo/VBoxContainer/StatsLabel");
        _combatLabel = GetNode<Label>("UI/UnitInfo/VBoxContainer/CombatLabel");
        _moveButton = GetNode<Button>("UI/UnitInfo/VBoxContainer/ActionButtons/MoveButton");
        _chargeButton = GetNode<Button>("UI/UnitInfo/VBoxContainer/ActionButtons/ChargeButton");
        _attackButton = GetNode<Button>("UI/UnitInfo/VBoxContainer/ActionButtons/AttackButton");
        
        _diceResults = GetNode<Panel>("UI/DiceResults");
        _diceResultsLabel = GetNode<Label>("UI/DiceResults/VBoxContainer/DiceResultsLabel");
        
        var closeButton = GetNode<Button>("UI/DiceResults/VBoxContainer/CloseButton");
        closeButton.Pressed += () => _diceResults.Visible = false;

        _commandPointTracker = GetNode<CommandPointTrackerController>("UI/CommandPointTracker");
        _commandPointsButton = GetNode<Button>("UI/ActionPanel/VBoxContainer/CommandPointsButton");
        _unitAbilityUI = GetNode<UnitAbilityUIController>("UI/UnitAbilityUI");
        _unitAbilitiesButton = GetNode<Button>("UI/ActionPanel/VBoxContainer/UnitAbilitiesButton");
    }

    private void ConnectSignals()
    {
        _nextPhaseButton.Pressed += OnNextPhasePressed;
        _moveButton.Pressed += OnMovePressed;
        _chargeButton.Pressed += OnChargePressed;
        _attackButton.Pressed += OnAttackPressed;
        _commandPointsButton.Pressed += OnCommandPointsPressed;
        _unitAbilitiesButton.Pressed += OnUnitAbilitiesPressed;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameStateChanged += OnGameStateChanged;
            GameManager.Instance.TurnPhaseChanged += OnTurnPhaseChanged;
            GameManager.Instance.PlayerTurnChanged += OnPlayerTurnChanged;
        }

        if (InputManager.Instance != null)
        {
            InputManager.Instance.UnitSelected += OnUnitSelected;
            InputManager.Instance.UnitDeselected += OnUnitDeselected;
            InputManager.Instance.DiceRollRequested += OnDiceRollRequested;
            InputManager.Instance.CommandPointsToggleRequested += OnCommandPointsPressed;
            InputManager.Instance.UnitAbilitiesToggleRequested += OnUnitAbilitiesPressed;
        }
    }

    private void InitializeGame()
    {
        if (GameManager.Instance != null)
        {
            UpdateTurnDisplay();
            CreateSampleUnits();
        }
    }

    private void CreateSampleUnits()
    {
        // Create sample units for testing
        CreateUnit("Liberator Prime", 1, new Vector3(-10, 0, 0), 6, 2, 6, 4, 2, 4, 4, 0, 1);
        CreateUnit("Stormcast Warrior", 1, new Vector3(-8, 0, 0), 6, 1, 6, 4, 1, 4, 4, 0, 1);
        CreateUnit("Orruk Brute", 2, new Vector3(10, 0, 0), 5, 3, 5, 5, 2, 4, 3, 1, 2);
        CreateUnit("Orruk Warrior", 2, new Vector3(8, 0, 0), 5, 1, 5, 5, 1, 4, 4, 0, 1);
    }

    private void CreateUnit(string name, int playerId, Vector3 position, int move, int wounds, int bravery, int save, int attacks, int toHit, int toWound, int rend, int damage)
    {
        var unit = new Unit();
        unit.UnitName = name;
        unit.PlayerId = playerId;
        unit.Move = move;
        unit.Wounds = wounds;
        unit.MaxWounds = wounds;
        unit.Bravery = bravery;
        unit.Save = save;
        unit.Attacks = attacks;
        unit.ToHit = toHit;
        unit.ToWound = toWound;
        unit.Rend = rend;
        unit.Damage = damage;
        unit.BaseSize = 1.0f;
        
        unit.SetPosition(position);
        AddChild(unit);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddUnit(unit);
        }
    }

    private void UpdateTurnDisplay()
    {
        if (GameManager.Instance != null)
        {
            _turnLabel.Text = $"Turn: {GameManager.Instance.CurrentPlayerTurn}";
            _phaseLabel.Text = $"Phase: {GameManager.Instance.CurrentTurnPhase}";
            _playerLabel.Text = $"Player: {GameManager.Instance.CurrentPlayerTurn}";
        }
    }

    private void OnNextPhasePressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextTurnPhase();
        }
    }

    private void OnMovePressed()
    {
        var selectedUnit = InputManager.Instance?.GetSelectedUnit();
        if (selectedUnit != null)
        {
            GD.Print($"Move action for {selectedUnit.UnitName}");
            // Movement will be handled by clicking on the board
        }
    }

    private void OnChargePressed()
    {
        var selectedUnit = InputManager.Instance?.GetSelectedUnit();
        if (selectedUnit != null)
        {
            GD.Print($"Charge action for {selectedUnit.UnitName}");
            // Charge will be handled by selecting target
        }
    }

    private void OnAttackPressed()
    {
        var selectedUnit = InputManager.Instance?.GetSelectedUnit();
        if (selectedUnit != null)
        {
            GD.Print($"Attack action for {selectedUnit.UnitName}");
            // Attack will be handled by selecting target
        }
    }

    private void OnGameStateChanged(GameManager.GameState newState)
    {
        GD.Print($"GameScene: Game state changed to {newState}");
        UpdateTurnDisplay();
    }

    private void OnTurnPhaseChanged(GameManager.TurnPhase newPhase)
    {
        GD.Print($"GameScene: Turn phase changed to {newPhase}");
        UpdateTurnDisplay();
        UpdateActionButtons(newPhase);
    }

    private void OnPlayerTurnChanged(int playerId)
    {
        GD.Print($"GameScene: Player turn changed to {playerId}");
        UpdateTurnDisplay();
    }

    private void OnUnitSelected(Unit unit)
    {
        GD.Print($"GameScene: Unit selected: {unit.UnitName}");
        ShowUnitInfo(unit);
    }

    private void OnUnitDeselected()
    {
        GD.Print("GameScene: Unit deselected");
        HideUnitInfo();
    }

    private void OnDiceRollRequested(int count, int sides)
    {
        ShowDiceResults(count, sides);
    }

    private void ShowUnitInfo(Unit unit)
    {
        _unitNameLabel.Text = unit.UnitName;
        _statsLabel.Text = $"Move: {unit.Move}\" | Wounds: {unit.Wounds}/{unit.MaxWounds} | Save: {unit.Save}+";
        _combatLabel.Text = $"Attacks: {unit.Attacks} | To Hit: {unit.ToHit}+ | To Wound: {unit.ToWound}+ | Rend: {unit.Rend} | Damage: {unit.Damage}";
        
        _unitInfo.Visible = true;
        UpdateActionButtons(GameManager.Instance.CurrentTurnPhase);
    }

    private void HideUnitInfo()
    {
        _unitInfo.Visible = false;
    }

    private void UpdateActionButtons(GameManager.TurnPhase phase)
    {
        var selectedUnit = InputManager.Instance?.GetSelectedUnit();
        if (selectedUnit == null)
        {
            _moveButton.Disabled = true;
            _chargeButton.Disabled = true;
            _attackButton.Disabled = true;
            return;
        }

        _moveButton.Disabled = !selectedUnit.CanMove();
        _chargeButton.Disabled = !selectedUnit.CanCharge();
        _attackButton.Disabled = !selectedUnit.CanFight();
    }

    private void ShowDiceResults(int count, int sides)
    {
        var results = DiceManager.RollMultipleDice(count, sides);
        var resultText = $"Rolled {count}D{sides}: {string.Join(", ", results)}";
        
        _diceResultsLabel.Text = resultText;
        _diceResults.Visible = true;
        
        GD.Print($"Dice Results: {resultText}");
    }

    private void OnCommandPointsPressed()
    {
        _commandPointTracker.Toggle();
        GD.Print("Command Points panel toggled");
    }
    
    private void OnUnitAbilitiesPressed()
    {
        var selectedUnit = InputManager.Instance?.GetSelectedUnit();
        if (selectedUnit != null)
        {
            _unitAbilityUI.ShowForUnit(selectedUnit);
        }
        else
        {
            GD.Print("No unit selected. Please select a unit first.");
        }
    }
}
