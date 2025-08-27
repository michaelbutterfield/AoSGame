using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BattleplanManager : Node
{
    public static BattleplanManager Instance { get; private set; }
    
    [Signal]
    public delegate void BattleplanChangedEventHandler(Battleplan newBattleplan);
    
    [Signal]
    public delegate void ObjectiveCompletedEventHandler(int playerId, int objectiveId);
    
    [Export] public Battleplan CurrentBattleplan { get; private set; }
    [Export] public int CurrentTurn { get; private set; } = 1;
    [Export] public int MaxTurns { get; set; } = 5;
    
    private Dictionary<int, int> _playerVictoryPoints = new Dictionary<int, int>();
    private Dictionary<int, List<int>> _playerCompletedObjectives = new Dictionary<int, List<int>>();
    private List<Objective> _objectives = new List<Objective>();
    private List<DeploymentZone> _deploymentZones = new List<DeploymentZone>();
    
    public override void _Ready()
    {
        Instance = this;
        InitializeBattleplans();
    }
    
    private void InitializeBattleplans()
    {
        // Initialize default battleplan
        SetBattleplan(BattleplanType.BattleForThePass);
    }
    
    public void SetBattleplan(BattleplanType battleplanType)
    {
        CurrentBattleplan = CreateBattleplan(battleplanType);
        EmitSignal(SignalName.BattleplanChanged, CurrentBattleplan);
        
        // Reset game state
        CurrentTurn = 1;
        _playerVictoryPoints.Clear();
        _playerCompletedObjectives.Clear();
        _objectives.Clear();
        _deploymentZones.Clear();
        
        // Setup battleplan
        SetupBattleplan();
        
        GD.Print($"Battleplan set to: {CurrentBattleplan.Name}");
    }
    
    private Battleplan CreateBattleplan(BattleplanType type)
    {
        return type switch
        {
            BattleplanType.BattleForThePass => new Battleplan
            {
                Name = "Battle for the Pass",
                Description = "A narrow pass through the mountains becomes the site of a desperate battle.",
                MaxTurns = 5,
                VictoryConditions = new List<VictoryCondition>
                {
                    new VictoryCondition { Type = VictoryType.VictoryPoints, TargetPoints = 15, Description = "Score 15+ Victory Points" },
                    new VictoryCondition { Type = VictoryType.DestroyEnemy, Description = "Destroy the enemy army" }
                },
                DeploymentType = DeploymentType.Diagonal,
                TerrainLayout = TerrainLayout.MountainPass,
                Objectives = new List<ObjectiveType> { ObjectiveType.Control, ObjectiveType.Control, ObjectiveType.Control }
            },
            
            BattleplanType.ThePitchedBattle => new Battleplan
            {
                Name = "The Pitched Battle",
                Description = "A classic battle on an open field with strategic objectives.",
                MaxTurns = 5,
                VictoryConditions = new List<VictoryCondition>
                {
                    new VictoryCondition { Type = VictoryType.VictoryPoints, TargetPoints = 12, Description = "Score 12+ Victory Points" },
                    new VictoryCondition { Type = VictoryType.DestroyEnemy, Description = "Destroy the enemy army" }
                },
                DeploymentType = DeploymentType.LongEdge,
                TerrainLayout = TerrainLayout.OpenField,
                Objectives = new List<ObjectiveType> { ObjectiveType.Control, ObjectiveType.Control, ObjectiveType.Control }
            },
            
            BattleplanType.SurgeOfConquest => new Battleplan
            {
                Name = "Surge of Conquest",
                Description = "A dynamic battle with shifting objectives and aggressive tactics.",
                MaxTurns = 5,
                VictoryConditions = new List<VictoryCondition>
                {
                    new VictoryCondition { Type = VictoryType.VictoryPoints, TargetPoints = 18, Description = "Score 18+ Victory Points" },
                    new VictoryCondition { Type = VictoryType.DestroyEnemy, Description = "Destroy the enemy army" }
                },
                DeploymentType = DeploymentType.ShortEdge,
                TerrainLayout = TerrainLayout.Strategic,
                Objectives = new List<ObjectiveType> { ObjectiveType.Control, ObjectiveType.Control, ObjectiveType.Control, ObjectiveType.Control }
            },
            
            BattleplanType.BloodAndGlory => new Battleplan
            {
                Name = "Blood and Glory",
                Description = "A brutal battle where heroes and leaders are the primary targets.",
                MaxTurns = 5,
                VictoryConditions = new List<VictoryCondition>
                {
                    new VictoryCondition { Type = VictoryType.HeroKills, TargetPoints = 3, Description = "Kill 3+ enemy Heroes" },
                    new VictoryCondition { Type = VictoryType.DestroyEnemy, Description = "Destroy the enemy army" }
                },
                DeploymentType = DeploymentType.Diagonal,
                TerrainLayout = TerrainLayout.Heroic,
                Objectives = new List<ObjectiveType> { ObjectiveType.Control, ObjectiveType.Control }
            },
            
            BattleplanType.FocalPoints => new Battleplan
            {
                Name = "Focal Points",
                Description = "A battle focused on controlling key strategic locations.",
                MaxTurns = 5,
                VictoryConditions = new List<VictoryCondition>
                {
                    new VictoryCondition { Type = VictoryType.VictoryPoints, TargetPoints = 20, Description = "Score 20+ Victory Points" },
                    new VictoryCondition { Type = VictoryType.DestroyEnemy, Description = "Destroy the enemy army" }
                },
                DeploymentType = DeploymentType.LongEdge,
                TerrainLayout = TerrainLayout.Focal,
                Objectives = new List<ObjectiveType> { ObjectiveType.Control, ObjectiveType.Control, ObjectiveType.Control, ObjectiveType.Control, ObjectiveType.Control }
            },
            
            BattleplanType.TotalCommitment => new Battleplan
            {
                Name = "Total Commitment",
                Description = "A battle where every unit must be committed to the fight.",
                MaxTurns = 5,
                VictoryConditions = new List<VictoryCondition>
                {
                    new VictoryCondition { Type = VictoryType.VictoryPoints, TargetPoints = 16, Description = "Score 16+ Victory Points" },
                    new VictoryCondition { Type = VictoryType.DestroyEnemy, Description = "Destroy the enemy army" }
                },
                DeploymentType = DeploymentType.ShortEdge,
                TerrainLayout = TerrainLayout.Committed,
                Objectives = new List<ObjectiveType> { ObjectiveType.Control, ObjectiveType.Control, ObjectiveType.Control }
            },
            
            _ => throw new ArgumentException($"Unknown battleplan type: {type}")
        };
    }
    
    private void SetupBattleplan()
    {
        SetupDeploymentZones();
        SetupObjectives();
        SetupTerrainLayout();
        SetupVictoryPoints();
    }
    
    private void SetupDeploymentZones()
    {
        var boardCenter = GameManager.Instance.GetBoardCenter();
        var boardWidth = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardWidthInches);
        var boardHeight = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardHeightInches);
        
        _deploymentZones.Clear();
        
        switch (CurrentBattleplan.DeploymentType)
        {
            case DeploymentType.LongEdge:
                // Players deploy along the long edges
                _deploymentZones.Add(new DeploymentZone
                {
                    PlayerId = 1,
                    Center = boardCenter + new Vector3(0, 0, -boardHeight / 4),
                    Width = boardWidth * 0.8f,
                    Height = boardHeight * 0.3f,
                    Name = "Player 1 Deployment"
                });
                
                _deploymentZones.Add(new DeploymentZone
                {
                    PlayerId = 2,
                    Center = boardCenter + new Vector3(0, 0, boardHeight / 4),
                    Width = boardWidth * 0.8f,
                    Height = boardHeight * 0.3f,
                    Name = "Player 2 Deployment"
                });
                break;
                
            case DeploymentType.ShortEdge:
                // Players deploy along the short edges
                _deploymentZones.Add(new DeploymentZone
                {
                    PlayerId = 1,
                    Center = boardCenter + new Vector3(-boardWidth / 4, 0, 0),
                    Width = boardWidth * 0.3f,
                    Height = boardHeight * 0.8f,
                    Name = "Player 1 Deployment"
                });
                
                _deploymentZones.Add(new DeploymentZone
                {
                    PlayerId = 2,
                    Center = boardCenter + new Vector3(boardWidth / 4, 0, 0),
                    Width = boardWidth * 0.3f,
                    Height = boardHeight * 0.8f,
                    Name = "Player 2 Deployment"
                });
                break;
                
            case DeploymentType.Diagonal:
                // Players deploy in diagonal corners
                _deploymentZones.Add(new DeploymentZone
                {
                    PlayerId = 1,
                    Center = boardCenter + new Vector3(-boardWidth / 3, 0, -boardHeight / 3),
                    Width = boardWidth * 0.4f,
                    Height = boardHeight * 0.4f,
                    Name = "Player 1 Deployment"
                });
                
                _deploymentZones.Add(new DeploymentZone
                {
                    PlayerId = 2,
                    Center = boardCenter + new Vector3(boardWidth / 3, 0, boardHeight / 3),
                    Width = boardWidth * 0.4f,
                    Height = boardHeight * 0.4f,
                    Name = "Player 2 Deployment"
                });
                break;
        }
    }
    
    private void SetupObjectives()
    {
        _objectives.Clear();
        var boardCenter = GameManager.Instance.GetBoardCenter();
        var boardWidth = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardWidthInches);
        var boardHeight = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardHeightInches);
        
        for (int i = 0; i < CurrentBattleplan.Objectives.Count; i++)
        {
            var objectiveType = CurrentBattleplan.Objectives[i];
            Vector3 position;
            
            switch (CurrentBattleplan.TerrainLayout)
            {
                case TerrainLayout.MountainPass:
                    position = GetMountainPassObjectivePosition(i, boardCenter, boardWidth, boardHeight);
                    break;
                    
                case TerrainLayout.OpenField:
                    position = GetOpenFieldObjectivePosition(i, boardCenter, boardWidth, boardHeight);
                    break;
                    
                case TerrainLayout.Strategic:
                    position = GetStrategicObjectivePosition(i, boardCenter, boardWidth, boardHeight);
                    break;
                    
                case TerrainLayout.Heroic:
                    position = GetHeroicObjectivePosition(i, boardCenter, boardWidth, boardHeight);
                    break;
                    
                case TerrainLayout.Focal:
                    position = GetFocalObjectivePosition(i, boardCenter, boardWidth, boardHeight);
                    break;
                    
                case TerrainLayout.Committed:
                    position = GetCommittedObjectivePosition(i, boardCenter, boardWidth, boardHeight);
                    break;
                    
                default:
                    position = boardCenter;
                    break;
            }
            
            _objectives.Add(new Objective
            {
                Id = i + 1,
                Type = objectiveType,
                Position = position,
                Name = $"Objective {i + 1}",
                VictoryPoints = GetObjectiveVictoryPoints(objectiveType, i)
            });
        }
    }
    
    private Vector3 GetMountainPassObjectivePosition(int index, Vector3 center, float width, float height)
    {
        return index switch
        {
            0 => center + new Vector3(0, 0, -height / 6), // Near player 1
            1 => center, // Center
            2 => center + new Vector3(0, 0, height / 6), // Near player 2
            _ => center
        };
    }
    
    private Vector3 GetOpenFieldObjectivePosition(int index, Vector3 center, float width, float height)
    {
        return index switch
        {
            0 => center + new Vector3(-width / 4, 0, -height / 4), // Top left
            1 => center + new Vector3(0, 0, 0), // Center
            2 => center + new Vector3(width / 4, 0, height / 4), // Bottom right
            _ => center
        };
    }
    
    private Vector3 GetStrategicObjectivePosition(int index, Vector3 center, float width, float height)
    {
        return index switch
        {
            0 => center + new Vector3(-width / 3, 0, -height / 3), // Corner
            1 => center + new Vector3(width / 3, 0, -height / 3), // Corner
            2 => center + new Vector3(-width / 3, 0, height / 3), // Corner
            3 => center + new Vector3(width / 3, 0, height / 3), // Corner
            _ => center
        };
    }
    
    private Vector3 GetHeroicObjectivePosition(int index, Vector3 center, float width, float height)
    {
        return index switch
        {
            0 => center + new Vector3(-width / 4, 0, 0), // Left center
            1 => center + new Vector3(width / 4, 0, 0), // Right center
            _ => center
        };
    }
    
    private Vector3 GetFocalObjectivePosition(int index, Vector3 center, float width, float height)
    {
        return index switch
        {
            0 => center + new Vector3(-width / 3, 0, -height / 3), // Corner
            1 => center + new Vector3(width / 3, 0, -height / 3), // Corner
            2 => center, // Center
            3 => center + new Vector3(-width / 3, 0, height / 3), // Corner
            4 => center + new Vector3(width / 3, 0, height / 3), // Corner
            _ => center
        };
    }
    
    private Vector3 GetCommittedObjectivePosition(int index, Vector3 center, float width, float height)
    {
        return index switch
        {
            0 => center + new Vector3(-width / 4, 0, -height / 4), // Near player 1
            1 => center, // Center
            2 => center + new Vector3(width / 4, 0, height / 4), // Near player 2
            _ => center
        };
    }
    
    private int GetObjectiveVictoryPoints(ObjectiveType type, int index)
    {
        return type switch
        {
            ObjectiveType.Control => 3,
            ObjectiveType.Destroy => 5,
            ObjectiveType.Reach => 2,
            _ => 1
        };
    }
    
    private void SetupTerrainLayout()
    {
        if (TerrainPlacer.Instance != null)
        {
            TerrainPlacer.Instance.ClearAllTerrain();
            
            switch (CurrentBattleplan.TerrainLayout)
            {
                case TerrainLayout.MountainPass:
                    SetupMountainPassTerrain();
                    break;
                    
                case TerrainLayout.OpenField:
                    SetupOpenFieldTerrain();
                    break;
                    
                case TerrainLayout.Strategic:
                    SetupStrategicTerrain();
                    break;
                    
                case TerrainLayout.Heroic:
                    SetupHeroicTerrain();
                    break;
                    
                case TerrainLayout.Focal:
                    SetupFocalTerrain();
                    break;
                    
                case TerrainLayout.Committed:
                    SetupCommittedTerrain();
                    break;
            }
        }
    }
    
    private void SetupMountainPassTerrain()
    {
        // Mountain pass has hills and forests along the sides
        var boardCenter = GameManager.Instance.GetBoardCenter();
        var boardWidth = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardWidthInches);
        var boardHeight = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardHeightInches);
        
        // Hills on the sides
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Hill, boardCenter + new Vector3(-boardWidth / 3, 0, 0));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Hill, boardCenter + new Vector3(boardWidth / 3, 0, 0));
        
        // Forests near the objectives
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Forest, boardCenter + new Vector3(-boardWidth / 4, 0, -boardHeight / 6));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Forest, boardCenter + new Vector3(boardWidth / 4, 0, boardHeight / 6));
    }
    
    private void SetupOpenFieldTerrain()
    {
        // Open field has scattered terrain
        var boardCenter = GameManager.Instance.GetBoardCenter();
        var boardWidth = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardWidthInches);
        var boardHeight = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardHeightInches);
        
        // Scattered hills and forests
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Hill, boardCenter + new Vector3(-boardWidth / 4, 0, -boardHeight / 4));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Forest, boardCenter + new Vector3(boardWidth / 4, 0, boardHeight / 4));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Ruins, boardCenter + new Vector3(0, 0, boardHeight / 6));
    }
    
    private void SetupStrategicTerrain()
    {
        // Strategic layout has terrain around objectives
        var boardCenter = GameManager.Instance.GetBoardCenter();
        var boardWidth = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardWidthInches);
        var boardHeight = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardHeightInches);
        
        // Terrain near each corner objective
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Forest, boardCenter + new Vector3(-boardWidth / 3, 0, -boardHeight / 3));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Hill, boardCenter + new Vector3(boardWidth / 3, 0, -boardHeight / 3));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Ruins, boardCenter + new Vector3(-boardWidth / 3, 0, boardHeight / 3));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Forest, boardCenter + new Vector3(boardWidth / 3, 0, boardHeight / 3));
    }
    
    private void SetupHeroicTerrain()
    {
        // Heroic layout has dramatic terrain
        var boardCenter = GameManager.Instance.GetBoardCenter();
        var boardWidth = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardWidthInches);
        var boardHeight = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardHeightInches);
        
        // Dramatic terrain features
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Ruins, boardCenter + new Vector3(-boardWidth / 4, 0, 0));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Ruins, boardCenter + new Vector3(boardWidth / 4, 0, 0));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Dangerous, boardCenter);
    }
    
    private void SetupFocalTerrain()
    {
        // Focal layout has terrain around all objectives
        var boardCenter = GameManager.Instance.GetBoardCenter();
        var boardWidth = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardWidthInches);
        var boardHeight = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardHeightInches);
        
        // Terrain near each objective
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Forest, boardCenter + new Vector3(-boardWidth / 3, 0, -boardHeight / 3));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Hill, boardCenter + new Vector3(boardWidth / 3, 0, -boardHeight / 3));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Ruins, boardCenter);
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Forest, boardCenter + new Vector3(-boardWidth / 3, 0, boardHeight / 3));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Hill, boardCenter + new Vector3(boardWidth / 3, 0, boardHeight / 3));
    }
    
    private void SetupCommittedTerrain()
    {
        // Committed layout has blocking terrain
        var boardCenter = GameManager.Instance.GetBoardCenter();
        var boardWidth = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardWidthInches);
        var boardHeight = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardHeightInches);
        
        // Blocking terrain in the center
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Forest, boardCenter + new Vector3(-boardWidth / 6, 0, 0));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Forest, boardCenter + new Vector3(boardWidth / 6, 0, 0));
        TerrainPlacer.Instance.PlaceTerrainAt(TerrainType.Ruins, boardCenter);
    }
    
    private void SetupVictoryPoints()
    {
        _playerVictoryPoints[1] = 0;
        _playerVictoryPoints[2] = 0;
        _playerCompletedObjectives[1] = new List<int>();
        _playerCompletedObjectives[2] = new List<int>();
    }
    
    public void AwardVictoryPoints(int playerId, int points)
    {
        if (!_playerVictoryPoints.ContainsKey(playerId))
            _playerVictoryPoints[playerId] = 0;
            
        _playerVictoryPoints[playerId] += points;
        GD.Print($"Player {playerId} awarded {points} Victory Points. Total: {_playerVictoryPoints[playerId]}");
    }
    
    public void CompleteObjective(int playerId, int objectiveId)
    {
        if (!_playerCompletedObjectives.ContainsKey(playerId))
            _playerCompletedObjectives[playerId] = new List<int>();
            
        if (!_playerCompletedObjectives[playerId].Contains(objectiveId))
        {
            _playerCompletedObjectives[playerId].Add(objectiveId);
            
            var objective = _objectives.FirstOrDefault(o => o.Id == objectiveId);
            if (objective != null)
            {
                AwardVictoryPoints(playerId, objective.VictoryPoints);
                EmitSignal(SignalName.ObjectiveCompleted, playerId, objectiveId);
                GD.Print($"Player {playerId} completed Objective {objectiveId} for {objective.VictoryPoints} VP");
            }
        }
    }
    
    public bool CheckVictoryConditions(int playerId)
    {
        foreach (var condition in CurrentBattleplan.VictoryConditions)
        {
            switch (condition.Type)
            {
                case VictoryType.VictoryPoints:
                    if (_playerVictoryPoints.ContainsKey(playerId) && 
                        _playerVictoryPoints[playerId] >= condition.TargetPoints)
                        return true;
                    break;
                    
                case VictoryType.DestroyEnemy:
                    // Check if enemy army is destroyed
                    var enemyId = playerId == 1 ? 2 : 1;
                    var enemyUnits = GameManager.Instance.GetPlayerUnits(enemyId);
                    if (enemyUnits.Count == 0)
                        return true;
                    break;
                    
                case VictoryType.HeroKills:
                    // Check hero kills (would need to track this separately)
                    break;
            }
        }
        
        return false;
    }
    
    public void NextTurn()
    {
        CurrentTurn++;
        if (CurrentTurn > MaxTurns)
        {
            EndGame();
        }
    }
    
    private void EndGame()
    {
        // Determine winner based on victory points
        var player1VP = _playerVictoryPoints.ContainsKey(1) ? _playerVictoryPoints[1] : 0;
        var player2VP = _playerVictoryPoints.ContainsKey(2) ? _playerVictoryPoints[2] : 0;
        
        int winner = 0;
        if (player1VP > player2VP)
            winner = 1;
        else if (player2VP > player1VP)
            winner = 2;
        else
            winner = 0; // Draw
            
        GameManager.Instance.EndGame(winner);
        GD.Print($"Game ended! Player 1: {player1VP} VP, Player 2: {player2VP} VP. Winner: {(winner == 0 ? "Draw" : $"Player {winner}")}");
    }
    
    public List<Objective> GetObjectives() => _objectives;
    public List<DeploymentZone> GetDeploymentZones() => _deploymentZones;
    public int GetPlayerVictoryPoints(int playerId) => _playerVictoryPoints.ContainsKey(playerId) ? _playerVictoryPoints[playerId] : 0;
    public List<BattleplanType> GetAvailableBattleplans() => Enum.GetValues(typeof(BattleplanType)).Cast<BattleplanType>().ToList();
}

public enum BattleplanType
{
    BattleForThePass,
    ThePitchedBattle,
    SurgeOfConquest,
    BloodAndGlory,
    FocalPoints,
    TotalCommitment
}

public enum DeploymentType
{
    LongEdge,
    ShortEdge,
    Diagonal
}

public enum TerrainLayout
{
    MountainPass,
    OpenField,
    Strategic,
    Heroic,
    Focal,
    Committed
}

public enum ObjectiveType
{
    Control,
    Destroy,
    Reach
}

public enum VictoryType
{
    VictoryPoints,
    DestroyEnemy,
    HeroKills
}

public class Battleplan
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int MaxTurns { get; set; } = 5;
    public List<VictoryCondition> VictoryConditions { get; set; } = new List<VictoryCondition>();
    public DeploymentType DeploymentType { get; set; }
    public TerrainLayout TerrainLayout { get; set; }
    public List<ObjectiveType> Objectives { get; set; } = new List<ObjectiveType>();
}

public class VictoryCondition
{
    public VictoryType Type { get; set; }
    public int TargetPoints { get; set; }
    public string Description { get; set; } = "";
}

public class Objective
{
    public int Id { get; set; }
    public ObjectiveType Type { get; set; }
    public Vector3 Position { get; set; }
    public string Name { get; set; } = "";
    public int VictoryPoints { get; set; }
    public bool IsControlled { get; set; } = false;
    public int ControllingPlayer { get; set; } = 0;
}

public class DeploymentZone
{
    public int PlayerId { get; set; }
    public Vector3 Center { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public string Name { get; set; } = "";
}
