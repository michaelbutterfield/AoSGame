using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    public enum GameState
    {
        MainMenu,
        Lobby,
        Setup,
        Playing,
        GameOver
    }

    public enum TurnPhase
    {
        Hero,
        Movement,
        Shooting,
        Charge,
        Combat
    }

    [Signal]
    public delegate void GameStateChangedEventHandler(GameState newState);

    [Signal]
    public delegate void TurnPhaseChangedEventHandler(TurnPhase newPhase);

    [Signal]
    public delegate void PlayerTurnChangedEventHandler(int playerId);

    public static GameManager Instance { get; private set; }

    [Export]
    public GameState CurrentGameState { get; private set; } = GameState.MainMenu;

    [Export]
    public TurnPhase CurrentTurnPhase { get; private set; } = TurnPhase.Hero;

    [Export]
    public int CurrentPlayerTurn { get; private set; } = 1;

    [Export]
    public int MaxPlayers { get; set; } = 2;

    [Export]
    public float BoardWidthInches { get; set; } = 44.0f;

    [Export]
    public float BoardHeightInches { get; set; } = 60.0f;

    [Export]
    public float InchesPerUnit { get; set; } = 1.0f; // 1 Godot unit = 1 inch

    public Dictionary<int, PlayerData> Players { get; private set; } = new Dictionary<int, PlayerData>();
    public List<Unit> AllUnits { get; private set; } = new List<Unit>();
    public List<Terrain> TerrainPieces { get; private set; } = new List<Terrain>();

    private int _turnNumber = 1;
    private bool _isMultiplayer = false;

    public override void _Ready()
    {
        Instance = this;
        InitializeGame();
    }

    public void InitializeGame()
    {
        GD.Print("GameManager: Initializing game...");
        CurrentGameState = GameState.MainMenu;
        EmitSignal(SignalName.GameStateChanged, CurrentGameState);
    }

    public void StartMultiplayerGame()
    {
        _isMultiplayer = true;
        CurrentGameState = GameState.Setup;
        EmitSignal(SignalName.GameStateChanged, CurrentGameState);
        GD.Print("GameManager: Starting multiplayer game");
    }

    public void StartSinglePlayerGame()
    {
        _isMultiplayer = false;
        CurrentGameState = GameState.Setup;
        EmitSignal(SignalName.GameStateChanged, CurrentGameState);
        GD.Print("GameManager: Starting single player game");
    }

    public void AddPlayer(int playerId, string playerName, string armyName)
    {
        if (Players.Count >= MaxPlayers)
        {
            GD.PrintErr($"GameManager: Cannot add player {playerId}, max players reached");
            return;
        }

        var playerData = new PlayerData
        {
            Id = playerId,
            Name = playerName,
            ArmyName = armyName,
            VictoryPoints = 0
        };

        Players[playerId] = playerData;
        GD.Print($"GameManager: Added player {playerId} ({playerName}) with army {armyName}");
    }

    public void StartGame()
    {
        if (Players.Count != MaxPlayers)
        {
            GD.PrintErr($"GameManager: Cannot start game, need {MaxPlayers} players, have {Players.Count}");
            return;
        }

        CurrentGameState = GameState.Playing;
        CurrentPlayerTurn = 1;
        CurrentTurnPhase = TurnPhase.Hero;
        _turnNumber = 1;

        // Initialize command points for both players
        if (CommandPointManager.Instance != null)
        {
            CommandPointManager.Instance.InitializePlayer(1);
            CommandPointManager.Instance.InitializePlayer(2);
            CommandPointManager.Instance.OnTurnStarted(1); // Generate CP for first player
        }

        EmitSignal(SignalName.GameStateChanged, CurrentGameState);
        EmitSignal(SignalName.PlayerTurnChanged, CurrentPlayerTurn);
        EmitSignal(SignalName.TurnPhaseChanged, CurrentTurnPhase);

        GD.Print($"GameManager: Game started! Turn {_turnNumber}, Player {CurrentPlayerTurn}, Phase {CurrentTurnPhase}");
    }

    public void NextTurnPhase()
    {
        switch (CurrentTurnPhase)
        {
            case TurnPhase.Hero:
                CurrentTurnPhase = TurnPhase.Movement;
                break;
            case TurnPhase.Movement:
                CurrentTurnPhase = TurnPhase.Shooting;
                break;
            case TurnPhase.Shooting:
                CurrentTurnPhase = TurnPhase.Charge;
                break;
            case TurnPhase.Charge:
                CurrentTurnPhase = TurnPhase.Combat;
                break;
            case TurnPhase.Combat:
                NextPlayerTurn();
                return;
        }

        EmitSignal(SignalName.TurnPhaseChanged, CurrentTurnPhase);
        GD.Print($"GameManager: Phase changed to {CurrentTurnPhase}");
    }

    public void NextPlayerTurn()
    {
        CurrentPlayerTurn = CurrentPlayerTurn == 1 ? 2 : 1;
        
        if (CurrentPlayerTurn == 1)
        {
            _turnNumber++;
        }

        CurrentTurnPhase = TurnPhase.Hero;
        
        // Generate command points for the new player's turn
        if (CommandPointManager.Instance != null)
        {
            CommandPointManager.Instance.OnTurnStarted(CurrentPlayerTurn);
        }
        
        // Clear expired temporary effects for all units
        foreach (var unit in AllUnits)
        {
            unit.ClearExpiredEffects();
        }
        
        EmitSignal(SignalName.PlayerTurnChanged, CurrentPlayerTurn);
        EmitSignal(SignalName.TurnPhaseChanged, CurrentTurnPhase);
        
        GD.Print($"GameManager: Turn {_turnNumber}, Player {CurrentPlayerTurn}, Phase {CurrentTurnPhase}");
    }

    public void AddUnit(Unit unit)
    {
        AllUnits.Add(unit);
        GD.Print($"GameManager: Added unit {unit.UnitName} to player {unit.PlayerId}");
    }

    public void RemoveUnit(Unit unit)
    {
        AllUnits.Remove(unit);
        GD.Print($"GameManager: Removed unit {unit.UnitName}");
    }

    public List<Unit> GetPlayerUnits(int playerId)
    {
        return AllUnits.FindAll(unit => unit.PlayerId == playerId);
    }

    public float ConvertInchesToUnits(float inches)
    {
        return inches * InchesPerUnit;
    }

    public float ConvertUnitsToInches(float units)
    {
        return units / InchesPerUnit;
    }

    public Vector3 GetBoardCenter()
    {
        return new Vector3(
            ConvertInchesToUnits(BoardWidthInches) / 2.0f,
            0,
            ConvertInchesToUnits(BoardHeightInches) / 2.0f
        );
    }

    public bool IsValidBoardPosition(Vector3 position)
    {
        float halfWidth = ConvertInchesToUnits(BoardWidthInches) / 2.0f;
        float halfHeight = ConvertInchesToUnits(BoardHeightInches) / 2.0f;

        return position.X >= -halfWidth && position.X <= halfWidth &&
               position.Z >= -halfHeight && position.Z <= halfHeight;
    }

    public void EndGame(int winnerPlayerId)
    {
        CurrentGameState = GameState.GameOver;
        EmitSignal(SignalName.GameStateChanged, CurrentGameState);
        
        if (Players.ContainsKey(winnerPlayerId))
        {
            GD.Print($"GameManager: Game Over! Player {winnerPlayerId} ({Players[winnerPlayerId].Name}) wins!");
        }
        else
        {
            GD.Print("GameManager: Game Over!");
        }
    }
    
    public void SetPlayerArmy(Army army)
    {
        // TODO: Implement army setup for the current player
        GD.Print($"Setting army for player {CurrentPlayerTurn}: {army.Name}");
        // This would create units on the board based on the army composition
    }
}

public class PlayerData
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string ArmyName { get; set; } = "";
    public int VictoryPoints { get; set; }
    public bool IsReady { get; set; } = false;
}
