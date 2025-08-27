using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CommandPointManager : Node
{
    [Signal]
    public delegate void CommandPointsChangedEventHandler(int playerId, int currentPoints, int maxPoints);
    
    [Signal]
    public delegate void CommandAbilityUsedEventHandler(int playerId, string abilityName, int cost);
    
    [Signal]
    public delegate void CommandPointsGeneratedEventHandler(int playerId, int pointsGenerated, string source);

    public static CommandPointManager Instance { get; private set; }

    // Command point limits and generation
    [Export] public int StartingCommandPoints = 1;
    [Export] public int MaxCommandPoints = 3;
    [Export] public int CommandPointsPerTurn = 1;
    [Export] public int CommandPointsPerHero = 1;

    // Player command point tracking
    private Dictionary<int, PlayerCommandPoints> _playerCommandPoints = new Dictionary<int, PlayerCommandPoints>();
    
    // Command abilities database
    private Dictionary<string, CommandAbilityData> _commandAbilities = new Dictionary<string, CommandAbilityData>();
    
    // Used abilities tracking (resets each turn)
    private Dictionary<int, List<string>> _usedAbilitiesThisTurn = new Dictionary<int, List<string>>();

    public override void _Ready()
    {
        Instance = this;
        InitializeCommandAbilities();
    }

    private void InitializeCommandAbilities()
    {
        // Core command abilities from AoS 4th Edition
        AddCommandAbility("All-out Attack", new CommandAbilityData
        {
            Name = "All-out Attack",
            Description = "Add 1 to hit rolls for attacks made by this unit until the end of the phase.",
            Cost = 1,
            Range = 12.0f, // 12 inches
            Phase = GameManager.TurnPhase.Combat,
            TargetType = CommandTargetType.Self,
            Effect = CommandEffect.HitBonus,
            EffectValue = 1
        });

        AddCommandAbility("All-out Defence", new CommandAbilityData
        {
            Name = "All-out Defence",
            Description = "Add 1 to save rolls for attacks that target this unit until the end of the phase.",
            Cost = 1,
            Range = 12.0f,
            Phase = GameManager.TurnPhase.Combat,
            TargetType = CommandTargetType.Self,
            Effect = CommandEffect.SaveBonus,
            EffectValue = 1
        });

        AddCommandAbility("Rally", new CommandAbilityData
        {
            Name = "Rally",
            Description = "This unit can attempt to rally. Roll a dice for each slain model from this unit. For each 6, you can return 1 slain model to this unit.",
            Cost = 1,
            Range = 12.0f,
            Phase = GameManager.TurnPhase.Hero,
            TargetType = CommandTargetType.Self,
            Effect = CommandEffect.Rally,
            EffectValue = 0
        });

        AddCommandAbility("At the Double", new CommandAbilityData
        {
            Name = "At the Double",
            Description = "This unit can run and still charge later in the same turn.",
            Cost = 1,
            Range = 12.0f,
            Phase = GameManager.TurnPhase.Movement,
            TargetType = CommandTargetType.Self,
            Effect = CommandEffect.RunAndCharge,
            EffectValue = 0
        });

        AddCommandAbility("Forward to Victory", new CommandAbilityData
        {
            Name = "Forward to Victory",
            Description = "This unit can charge even if it ran earlier in the same turn.",
            Cost = 1,
            Range = 12.0f,
            Phase = GameManager.TurnPhase.Charge,
            TargetType = CommandTargetType.Self,
            Effect = CommandEffect.ChargeAfterRun,
            EffectValue = 0
        });

        AddCommandAbility("Inspiring Presence", new CommandAbilityData
        {
            Name = "Inspiring Presence",
            Description = "This unit does not have to take battleshock tests.",
            Cost = 1,
            Range = 12.0f,
            Phase = GameManager.TurnPhase.Hero,
            TargetType = CommandTargetType.Self,
            Effect = CommandEffect.IgnoreBattleshock,
            EffectValue = 0
        });

        AddCommandAbility("Unleash Hell", new CommandAbilityData
        {
            Name = "Unleash Hell",
            Description = "This unit can shoot even if it ran earlier in the same turn.",
            Cost = 1,
            Range = 12.0f,
            Phase = GameManager.TurnPhase.Shooting,
            TargetType = CommandTargetType.Self,
            Effect = CommandEffect.ShootAfterRun,
            EffectValue = 0
        });

        AddCommandAbility("Redeploy", new CommandAbilityData
        {
            Name = "Redeploy",
            Description = "This unit can make a normal move of up to D6 inches.",
            Cost = 1,
            Range = 12.0f,
            Phase = GameManager.TurnPhase.Movement,
            TargetType = CommandTargetType.Self,
            Effect = CommandEffect.ExtraMove,
            EffectValue = 0
        });

        AddCommandAbility("Counter-charge", new CommandAbilityData
        {
            Name = "Counter-charge",
            Description = "This unit can make a charge move of up to D6 inches.",
            Cost = 1,
            Range = 12.0f,
            Phase = GameManager.TurnPhase.Charge,
            TargetType = CommandTargetType.Self,
            Effect = CommandEffect.ExtraCharge,
            EffectValue = 0
        });
    }

    private void AddCommandAbility(string name, CommandAbilityData ability)
    {
        _commandAbilities[name] = ability;
    }

    public void InitializePlayer(int playerId)
    {
        _playerCommandPoints[playerId] = new PlayerCommandPoints
        {
            PlayerId = playerId,
            CurrentPoints = StartingCommandPoints,
            MaxPoints = MaxCommandPoints,
            PointsGeneratedThisTurn = 0
        };
        
        _usedAbilitiesThisTurn[playerId] = new List<string>();
        
        EmitSignal(SignalName.CommandPointsChanged, playerId, StartingCommandPoints, MaxCommandPoints);
        GD.Print($"CommandPointManager: Initialized player {playerId} with {StartingCommandPoints} command points");
    }

    public void OnTurnStarted(int playerId)
    {
        if (!_playerCommandPoints.ContainsKey(playerId))
        {
            InitializePlayer(playerId);
        }

        var playerCP = _playerCommandPoints[playerId];
        
        // Generate command points for the turn
        int pointsToGenerate = CommandPointsPerTurn;
        
        // Add points for each hero unit
        var heroUnits = GameManager.Instance.GetPlayerUnits(playerId).Where(u => u.IsHero).ToList();
        pointsToGenerate += heroUnits.Count * CommandPointsPerHero;
        
        // Generate points (up to max)
        int actualGenerated = Math.Min(pointsToGenerate, playerCP.MaxPoints - playerCP.CurrentPoints);
        playerCP.CurrentPoints += actualGenerated;
        playerCP.PointsGeneratedThisTurn = actualGenerated;
        
        // Clear used abilities for the new turn
        _usedAbilitiesThisTurn[playerId].Clear();
        
        EmitSignal(SignalName.CommandPointsChanged, playerId, playerCP.CurrentPoints, playerCP.MaxPoints);
        EmitSignal(SignalName.CommandPointsGenerated, playerId, actualGenerated, $"Turn start + {heroUnits.Count} heroes");
        
        GD.Print($"CommandPointManager: Player {playerId} generated {actualGenerated} command points (turn: {CommandPointsPerTurn}, heroes: {heroUnits.Count})");
    }

    public bool CanUseCommandAbility(int playerId, string abilityName, Unit targetUnit = null)
    {
        if (!_playerCommandPoints.ContainsKey(playerId))
            return false;

        if (!_commandAbilities.ContainsKey(abilityName))
            return false;

        var playerCP = _playerCommandPoints[playerId];
        var ability = _commandAbilities[abilityName];

        // Check if player has enough command points
        if (playerCP.CurrentPoints < ability.Cost)
            return false;

        // Check if ability was already used this turn
        if (_usedAbilitiesThisTurn[playerId].Contains(abilityName))
            return false;

        // Check if we're in the correct phase
        if (GameManager.Instance.CurrentTurnPhase != ability.Phase)
            return false;

        // Check range if target is specified
        if (targetUnit != null)
        {
            var playerUnits = GameManager.Instance.GetPlayerUnits(playerId);
            bool inRange = false;
            
            foreach (var unit in playerUnits)
            {
                if (unit.IsHero) // Only heroes can use command abilities
                {
                    float distance = unit.Position.DistanceTo(targetUnit.Position);
                    float distanceInches = GameManager.Instance.ConvertUnitsToInches(distance);
                    if (distanceInches <= ability.Range)
                    {
                        inRange = true;
                        break;
                    }
                }
            }
            
            if (!inRange)
                return false;
        }

        return true;
    }

    public bool UseCommandAbility(int playerId, string abilityName, Unit targetUnit = null)
    {
        if (!CanUseCommandAbility(playerId, abilityName, targetUnit))
            return false;

        var playerCP = _playerCommandPoints[playerId];
        var ability = _commandAbilities[abilityName];

        // Spend command points
        playerCP.CurrentPoints -= ability.Cost;
        
        // Mark ability as used this turn
        _usedAbilitiesThisTurn[playerId].Add(abilityName);

        // Apply the command ability effect
        ApplyCommandAbilityEffect(ability, targetUnit);

        EmitSignal(SignalName.CommandPointsChanged, playerId, playerCP.CurrentPoints, playerCP.MaxPoints);
        EmitSignal(SignalName.CommandAbilityUsed, playerId, abilityName, ability.Cost);

        GD.Print($"CommandPointManager: Player {playerId} used {abilityName} for {ability.Cost} CP. Remaining: {playerCP.CurrentPoints}");
        return true;
    }

    private void ApplyCommandAbilityEffect(CommandAbilityData ability, Unit targetUnit)
    {
        if (targetUnit == null) return;

        switch (ability.Effect)
        {
            case CommandEffect.HitBonus:
                targetUnit.AddTemporaryEffect("All-out Attack", "Hit Bonus", ability.EffectValue, GameManager.Instance.CurrentTurnPhase);
                break;
                
            case CommandEffect.SaveBonus:
                targetUnit.AddTemporaryEffect("All-out Defence", "Save Bonus", ability.EffectValue, GameManager.Instance.CurrentTurnPhase);
                break;
                
            case CommandEffect.Rally:
                // Rally is handled in the Unit class
                break;
                
            case CommandEffect.RunAndCharge:
                targetUnit.AddTemporaryEffect("At the Double", "Can Charge After Run", 1, GameManager.TurnPhase.Charge);
                break;
                
            case CommandEffect.ChargeAfterRun:
                targetUnit.AddTemporaryEffect("Forward to Victory", "Can Charge After Run", 1, GameManager.TurnPhase.Charge);
                break;
                
            case CommandEffect.IgnoreBattleshock:
                targetUnit.AddTemporaryEffect("Inspiring Presence", "Ignore Battleshock", 1, GameManager.TurnPhase.Hero);
                break;
                
            case CommandEffect.ShootAfterRun:
                targetUnit.AddTemporaryEffect("Unleash Hell", "Can Shoot After Run", 1, GameManager.TurnPhase.Shooting);
                break;
                
            case CommandEffect.ExtraMove:
                targetUnit.AddTemporaryEffect("Redeploy", "Extra Move D6", 1, GameManager.TurnPhase.Movement);
                break;
                
            case CommandEffect.ExtraCharge:
                targetUnit.AddTemporaryEffect("Counter-charge", "Extra Charge D6", 1, GameManager.TurnPhase.Charge);
                break;
        }
    }

    public void IncreaseCommandPoints(int playerId, int amount, string source = "Bonus")
    {
        if (!_playerCommandPoints.ContainsKey(playerId))
            return;

        var playerCP = _playerCommandPoints[playerId];
        int actualIncrease = Math.Min(amount, playerCP.MaxPoints - playerCP.CurrentPoints);
        
        if (actualIncrease > 0)
        {
            playerCP.CurrentPoints += actualIncrease;
            EmitSignal(SignalName.CommandPointsChanged, playerId, playerCP.CurrentPoints, playerCP.MaxPoints);
            EmitSignal(SignalName.CommandPointsGenerated, playerId, actualIncrease, source);
            
            GD.Print($"CommandPointManager: Player {playerId} gained {actualIncrease} command points from {source}");
        }
    }

    public void DecreaseCommandPoints(int playerId, int amount, string source = "Penalty")
    {
        if (!_playerCommandPoints.ContainsKey(playerId))
            return;

        var playerCP = _playerCommandPoints[playerId];
        int actualDecrease = Math.Min(amount, playerCP.CurrentPoints);
        
        if (actualDecrease > 0)
        {
            playerCP.CurrentPoints -= actualDecrease;
            EmitSignal(SignalName.CommandPointsChanged, playerId, playerCP.CurrentPoints, playerCP.MaxPoints);
            
            GD.Print($"CommandPointManager: Player {playerId} lost {actualDecrease} command points from {source}");
        }
    }

    public void SetMaxCommandPoints(int playerId, int maxPoints)
    {
        if (!_playerCommandPoints.ContainsKey(playerId))
            return;

        var playerCP = _playerCommandPoints[playerId];
        playerCP.MaxPoints = maxPoints;
        
        // Adjust current points if they exceed the new max
        if (playerCP.CurrentPoints > maxPoints)
        {
            playerCP.CurrentPoints = maxPoints;
        }
        
        EmitSignal(SignalName.CommandPointsChanged, playerId, playerCP.CurrentPoints, playerCP.MaxPoints);
        GD.Print($"CommandPointManager: Player {playerId} max command points set to {maxPoints}");
    }

    public PlayerCommandPoints GetPlayerCommandPoints(int playerId)
    {
        return _playerCommandPoints.ContainsKey(playerId) ? _playerCommandPoints[playerId] : null;
    }

    public List<CommandAbilityData> GetAvailableCommandAbilities(int playerId)
    {
        var availableAbilities = new List<CommandAbilityData>();
        var playerCP = GetPlayerCommandPoints(playerId);
        
        if (playerCP == null) return availableAbilities;

        foreach (var ability in _commandAbilities.Values)
        {
            if (playerCP.CurrentPoints >= ability.Cost && 
                !_usedAbilitiesThisTurn[playerId].Contains(ability.Name) &&
                ability.Phase == GameManager.Instance.CurrentTurnPhase)
            {
                availableAbilities.Add(ability);
            }
        }

        return availableAbilities;
    }

    public List<string> GetUsedAbilitiesThisTurn(int playerId)
    {
        return _usedAbilitiesThisTurn.ContainsKey(playerId) ? _usedAbilitiesThisTurn[playerId] : new List<string>();
    }

    public Dictionary<string, CommandAbilityData> GetAllCommandAbilities()
    {
        return new Dictionary<string, CommandAbilityData>(_commandAbilities);
    }

    public void ResetForNewGame()
    {
        _playerCommandPoints.Clear();
        _usedAbilitiesThisTurn.Clear();
        GD.Print("CommandPointManager: Reset for new game");
    }
}

public class PlayerCommandPoints
{
    public int PlayerId { get; set; }
    public int CurrentPoints { get; set; }
    public int MaxPoints { get; set; }
    public int PointsGeneratedThisTurn { get; set; }
}

public class CommandAbilityData
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Cost { get; set; } = 1;
    public float Range { get; set; } = 12.0f; // in inches
    public GameManager.TurnPhase Phase { get; set; }
    public CommandTargetType TargetType { get; set; }
    public CommandEffect Effect { get; set; }
    public int EffectValue { get; set; }
}

public enum CommandTargetType
{
    Self,
    Friendly,
    Enemy,
    Any
}

public enum CommandEffect
{
    HitBonus,
    SaveBonus,
    Rally,
    RunAndCharge,
    ChargeAfterRun,
    IgnoreBattleshock,
    ShootAfterRun,
    ExtraMove,
    ExtraCharge
}
