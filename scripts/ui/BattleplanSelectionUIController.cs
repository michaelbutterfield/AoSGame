using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class BattleplanSelectionUIController : Control
{
    [Export] public PackedScene MainMenuScene;
    [Export] public PackedScene GameScene;
    
    // UI References
    private OptionButton _battleplanOptionButton;
    private Label _battleplanName;
    private Label _battleplanDescription;
    private ItemList _victoryList;
    private Label _maxTurnsValue;
    private Label _deploymentValue;
    private Label _objectivesValue;
    private Label _terrainValue;
    private Button _backButton;
    private Button _selectBattleplanButton;
    
    // State
    private List<BattleplanType> _availableBattleplans = new List<BattleplanType>();
    private BattleplanType _selectedBattleplan = BattleplanType.BattleForThePass;
    
    public override void _Ready()
    {
        GetNodeReferences();
        ConnectSignals();
        InitializeUI();
    }
    
    private void GetNodeReferences()
    {
        _battleplanOptionButton = GetNode<OptionButton>("MainContainer/ContentContainer/BattleplanList/BattleplanOptionButton");
        _battleplanName = GetNode<Label>("MainContainer/ContentContainer/BattleplanDetails/BattleplanName");
        _battleplanDescription = GetNode<Label>("MainContainer/ContentContainer/BattleplanDetails/BattleplanDescription");
        _victoryList = GetNode<ItemList>("MainContainer/ContentContainer/BattleplanDetails/VictoryConditions/VictoryList");
        _maxTurnsValue = GetNode<Label>("MainContainer/ContentContainer/BattleplanDetails/BattleplanInfo/MaxTurnsValue");
        _deploymentValue = GetNode<Label>("MainContainer/ContentContainer/BattleplanDetails/BattleplanInfo/DeploymentValue");
        _objectivesValue = GetNode<Label>("MainContainer/ContentContainer/BattleplanDetails/BattleplanInfo/ObjectivesValue");
        _terrainValue = GetNode<Label>("MainContainer/ContentContainer/BattleplanDetails/BattleplanInfo/TerrainValue");
        _backButton = GetNode<Button>("MainContainer/ActionButtons/BackButton");
        _selectBattleplanButton = GetNode<Button>("MainContainer/ActionButtons/SelectBattleplanButton");
    }
    
    private void ConnectSignals()
    {
        _battleplanOptionButton.ItemSelected += OnBattleplanSelected;
        _backButton.Pressed += OnBackPressed;
        _selectBattleplanButton.Pressed += OnSelectBattleplanPressed;
    }
    
    private void InitializeUI()
    {
        PopulateBattleplans();
        UpdateBattleplanDetails();
    }
    
    private void PopulateBattleplans()
    {
        _battleplanOptionButton.Clear();
        _availableBattleplans.Clear();
        
        if (BattleplanManager.Instance != null)
        {
            _availableBattleplans = BattleplanManager.Instance.GetAvailableBattleplans();
        }
        else
        {
            // Fallback to enum values if BattleplanManager not available
            _availableBattleplans = System.Enum.GetValues(typeof(BattleplanType)).Cast<BattleplanType>().ToList();
        }
        
        foreach (var battleplan in _availableBattleplans)
        {
            var displayName = GetBattleplanDisplayName(battleplan);
            _battleplanOptionButton.AddItem(displayName);
        }
        
        _battleplanOptionButton.Selected = 0;
        _selectedBattleplan = _availableBattleplans[0];
    }
    
    private string GetBattleplanDisplayName(BattleplanType battleplan)
    {
        return battleplan switch
        {
            BattleplanType.BattleForThePass => "Battle for the Pass",
            BattleplanType.ThePitchedBattle => "The Pitched Battle",
            BattleplanType.SurgeOfConquest => "Surge of Conquest",
            BattleplanType.BloodAndGlory => "Blood and Glory",
            BattleplanType.FocalPoints => "Focal Points",
            BattleplanType.TotalCommitment => "Total Commitment",
            _ => battleplan.ToString()
        };
    }
    
    private void OnBattleplanSelected(int index)
    {
        if (index >= 0 && index < _availableBattleplans.Count)
        {
            _selectedBattleplan = _availableBattleplans[index];
            UpdateBattleplanDetails();
        }
    }
    
    private void UpdateBattleplanDetails()
    {
        var battleplan = CreateBattleplan(_selectedBattleplan);
        
        _battleplanName.Text = battleplan.Name;
        _battleplanDescription.Text = battleplan.Description;
        _maxTurnsValue.Text = battleplan.MaxTurns.ToString();
        _deploymentValue.Text = GetDeploymentDisplayName(battleplan.DeploymentType);
        _objectivesValue.Text = battleplan.Objectives.Count.ToString();
        _terrainValue.Text = GetTerrainDisplayName(battleplan.TerrainLayout);
        
        UpdateVictoryConditions(battleplan);
    }
    
    private Battleplan CreateBattleplan(BattleplanType type)
    {
        return type switch
        {
            BattleplanType.BattleForThePass => new Battleplan
            {
                Name = "Battle for the Pass",
                Description = "A narrow pass through the mountains becomes the site of a desperate battle. Control the strategic objectives to secure victory.",
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
                Description = "A classic battle on an open field with strategic objectives. The most traditional form of warfare in the Mortal Realms.",
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
                Description = "A dynamic battle with shifting objectives and aggressive tactics. Speed and mobility are key to victory.",
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
                Description = "A brutal battle where heroes and leaders are the primary targets. Eliminate enemy commanders to claim victory.",
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
                Description = "A battle focused on controlling key strategic locations. Multiple objectives create complex tactical decisions.",
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
                Description = "A battle where every unit must be committed to the fight. No reserves, no retreat - only total victory or defeat.",
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
            
            _ => new Battleplan
            {
                Name = "Unknown Battleplan",
                Description = "Battleplan details not available.",
                MaxTurns = 5,
                VictoryConditions = new List<VictoryCondition>(),
                DeploymentType = DeploymentType.LongEdge,
                TerrainLayout = TerrainLayout.OpenField,
                Objectives = new List<ObjectiveType>()
            }
        };
    }
    
    private string GetDeploymentDisplayName(DeploymentType deploymentType)
    {
        return deploymentType switch
        {
            DeploymentType.LongEdge => "Long Edge",
            DeploymentType.ShortEdge => "Short Edge",
            DeploymentType.Diagonal => "Diagonal",
            _ => deploymentType.ToString()
        };
    }
    
    private string GetTerrainDisplayName(TerrainLayout terrainLayout)
    {
        return terrainLayout switch
        {
            TerrainLayout.MountainPass => "Mountain Pass",
            TerrainLayout.OpenField => "Open Field",
            TerrainLayout.Strategic => "Strategic",
            TerrainLayout.Heroic => "Heroic",
            TerrainLayout.Focal => "Focal",
            TerrainLayout.Committed => "Committed",
            _ => terrainLayout.ToString()
        };
    }
    
    private void UpdateVictoryConditions(Battleplan battleplan)
    {
        _victoryList.Clear();
        
        foreach (var condition in battleplan.VictoryConditions)
        {
            var displayText = condition.Description;
            if (condition.Type == VictoryType.VictoryPoints)
            {
                displayText = $"Score {condition.TargetPoints}+ Victory Points";
            }
            else if (condition.Type == VictoryType.HeroKills)
            {
                displayText = $"Kill {condition.TargetPoints}+ Enemy Heroes";
            }
            
            _victoryList.AddItem(displayText);
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
    
    private void OnSelectBattleplanPressed()
    {
        // Set the selected battleplan
        if (BattleplanManager.Instance != null)
        {
            BattleplanManager.Instance.SetBattleplan(_selectedBattleplan);
            GD.Print($"Selected battleplan: {_selectedBattleplan}");
        }
        
        // Navigate to game scene or army builder
        if (GameScene != null)
        {
            GetTree().ChangeSceneToPacked(GameScene);
        }
        else
        {
            GD.PrintErr("GameScene not assigned");
        }
    }
}
