using Godot;
using System.Collections.Generic;

public partial class TerrainPlacer : Node
{
    [Export] public PackedScene TerrainScene;
    [Export] public float PlacementHeight = 0.1f;
    [Export] public bool IsPlacementMode = false;
    
    private Camera3D _camera;
    private List<Terrain> _placedTerrain = new List<Terrain>();
    private Terrain _previewTerrain;
    private TerrainType _selectedTerrainType = TerrainType.Forest;
    private Vector3 _placementPosition;
    
    // Terrain templates
    private Dictionary<TerrainType, TerrainTemplate> _terrainTemplates;
    
    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("../Camera3D");
        InitializeTerrainTemplates();
    }
    
    private void InitializeTerrainTemplates()
    {
        _terrainTemplates = new Dictionary<TerrainType, TerrainTemplate>
        {
            { TerrainType.Forest, new TerrainTemplate
            {
                Name = "Forest",
                Width = 3.0f,
                Length = 3.0f,
                Height = 2.0f,
                BlocksLineOfSight = true,
                DifficultTerrain = true,
                CoverBonus = 1,
                MovementPenalty = 1
            }},
            { TerrainType.Hill, new TerrainTemplate
            {
                Name = "Hill",
                Width = 4.0f,
                Length = 4.0f,
                Height = 1.5f,
                BlocksLineOfSight = false,
                DifficultTerrain = false,
                CoverBonus = 0,
                MovementPenalty = 0
            }},
            { TerrainType.Ruins, new TerrainTemplate
            {
                Name = "Ruins",
                Width = 2.0f,
                Length = 2.0f,
                Height = 3.0f,
                BlocksLineOfSight = true,
                DifficultTerrain = false,
                CoverBonus = 1,
                MovementPenalty = 0
            }},
            { TerrainType.Water, new TerrainTemplate
            {
                Name = "Water",
                Width = 5.0f,
                Length = 3.0f,
                Height = 0.2f,
                BlocksLineOfSight = false,
                DifficultTerrain = true,
                CoverBonus = 0,
                MovementPenalty = 2
            }},
            { TerrainType.Objective, new TerrainTemplate
            {
                Name = "Objective",
                Width = 1.0f,
                Length = 1.0f,
                Height = 0.5f,
                BlocksLineOfSight = false,
                DifficultTerrain = false,
                CoverBonus = 0,
                MovementPenalty = 0
            }},
            { TerrainType.Dangerous, new TerrainTemplate
            {
                Name = "Dangerous Terrain",
                Width = 3.0f,
                Length = 3.0f,
                Height = 0.5f,
                BlocksLineOfSight = false,
                DifficultTerrain = true,
                DangerousTerrain = true,
                CoverBonus = 0,
                MovementPenalty = 1
            }},
            { TerrainType.Deadly, new TerrainTemplate
            {
                Name = "Deadly Terrain",
                Width = 2.0f,
                Length = 2.0f,
                Height = 0.3f,
                BlocksLineOfSight = false,
                DifficultTerrain = false,
                DeadlyTerrain = true,
                CoverBonus = 0,
                MovementPenalty = 0
            }}
        };
    }
    
    public override void _Input(InputEvent @event)
    {
        if (!IsPlacementMode) return;
        
        if (@event is InputEventMouseMotion mouseMotion)
        {
            UpdatePlacementPreview();
        }
        else if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
            {
                PlaceTerrain();
            }
            else if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed)
            {
                RemoveTerrainAtPosition();
            }
        }
    }
    
    public void SetPlacementMode(bool enabled)
    {
        IsPlacementMode = enabled;
        
        if (enabled)
        {
            CreatePlacementPreview();
            GD.Print("Terrain placement mode enabled");
        }
        else
        {
            RemovePlacementPreview();
            GD.Print("Terrain placement mode disabled");
        }
    }
    
    public void SetTerrainType(TerrainType terrainType)
    {
        _selectedTerrainType = terrainType;
        
        if (IsPlacementMode)
        {
            CreatePlacementPreview();
        }
        
        GD.Print($"Selected terrain type: {terrainType}");
    }
    
    private void UpdatePlacementPreview()
    {
        if (_previewTerrain == null) return;
        
        var mousePos = GetViewport().GetMousePosition();
        var from = _camera.ProjectRayOrigin(mousePos);
        var to = from + _camera.ProjectRayNormal(mousePos) * 1000.0f;
        
        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        query.CollisionMask = 1; // Board layer
        
        var result = spaceState.IntersectRay(query);
        
        if (result.Count > 0)
        {
            _placementPosition = (Vector3)result["position"];
            _placementPosition.Y = PlacementHeight;
            _previewTerrain.Position = _placementPosition;
        }
    }
    
    private void CreatePlacementPreview()
    {
        RemovePlacementPreview();
        
        if (!_terrainTemplates.ContainsKey(_selectedTerrainType))
            return;
        
        var template = _terrainTemplates[_selectedTerrainType];
        _previewTerrain = Terrain.CreateTerrain(_selectedTerrainType, template.Name);
        
        if (_previewTerrain != null)
        {
            _previewTerrain.SetupVisualComponents();
            _previewTerrain.Modulate = new Color(1, 1, 1, 0.5f); // Semi-transparent
            AddChild(_previewTerrain);
        }
    }
    
    private void RemovePlacementPreview()
    {
        if (_previewTerrain != null)
        {
            _previewTerrain.QueueFree();
            _previewTerrain = null;
        }
    }
    
    private void PlaceTerrain()
    {
        if (_previewTerrain == null) return;
        
        var template = _terrainTemplates[_selectedTerrainType];
        var terrain = Terrain.CreateTerrain(_selectedTerrainType, template.Name);
        
        if (terrain != null)
        {
            terrain.SetPosition(_placementPosition);
            terrain.SetupVisualComponents();
            terrain.SetupCollision();
            
            AddChild(terrain);
            _placedTerrain.Add(terrain);
            
            GD.Print($"Placed {template.Name} at {_placementPosition}");
        }
    }
    
    private void RemoveTerrainAtPosition()
    {
        var mousePos = GetViewport().GetMousePosition();
        var from = _camera.ProjectRayOrigin(mousePos);
        var to = from + _camera.ProjectRayNormal(mousePos) * 1000.0f;
        
        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        query.CollisionMask = 4; // Terrain layer
        
        var result = spaceState.IntersectRay(query);
        
        if (result.Count > 0)
        {
            var hitPosition = (Vector3)result["position"];
            var hitObject = result["collider"];
            
            if (hitObject is CollisionShape3D collisionShape)
            {
                var terrain = collisionShape.GetParent() as Terrain;
                if (terrain != null && _placedTerrain.Contains(terrain))
                {
                    _placedTerrain.Remove(terrain);
                    terrain.QueueFree();
                    GD.Print($"Removed terrain at {hitPosition}");
                }
            }
        }
    }
    
    public void ClearAllTerrain()
    {
        foreach (var terrain in _placedTerrain)
        {
            terrain.QueueFree();
        }
        _placedTerrain.Clear();
        GD.Print("Cleared all terrain");
    }
    
    public void SaveTerrainLayout(string filename)
    {
        // TODO: Implement terrain layout saving
        GD.Print($"Saving terrain layout to {filename}");
    }
    
    public void LoadTerrainLayout(string filename)
    {
        // TODO: Implement terrain layout loading
        GD.Print($"Loading terrain layout from {filename}");
    }
    
    public List<Terrain> GetPlacedTerrain()
    {
        return _placedTerrain;
    }
    
    public void RandomizeTerrain(int count = 5)
    {
        ClearAllTerrain();
        
        var boardCenter = GameManager.Instance.GetBoardCenter();
        var boardWidth = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardWidthInches);
        var boardHeight = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardHeightInches);
        
        for (int i = 0; i < count; i++)
        {
            var terrainType = (TerrainType)(i % System.Enum.GetValues(typeof(TerrainType)).Length);
            var template = _terrainTemplates[terrainType];
            
            var x = (float)GD.RandRange(-boardWidth/3, boardWidth/3);
            var z = (float)GD.RandRange(-boardHeight/3, boardHeight/3);
            var position = new Vector3(x, PlacementHeight, z);
            
            var terrain = Terrain.CreateTerrain(terrainType, template.Name);
            if (terrain != null)
            {
                terrain.SetPosition(position);
                terrain.SetupVisualComponents();
                terrain.SetupCollision();
                
                AddChild(terrain);
                _placedTerrain.Add(terrain);
            }
        }
        
        GD.Print($"Randomized {count} terrain pieces");
    }
    
    public void PlaceTerrainAt(TerrainType terrainType, Vector3 position)
    {
        if (!_terrainTemplates.ContainsKey(terrainType))
            return;
            
        var template = _terrainTemplates[terrainType];
        var terrain = Terrain.CreateTerrain(terrainType, template.Name);
        
        if (terrain != null)
        {
            terrain.SetPosition(position);
            terrain.SetupVisualComponents();
            terrain.SetupCollision();
            
            AddChild(terrain);
            _placedTerrain.Add(terrain);
            
            GD.Print($"Placed {template.Name} at {position}");
        }
    }
}

public class TerrainTemplate
{
    public string Name { get; set; }
    public float Width { get; set; }
    public float Length { get; set; }
    public float Height { get; set; }
    public bool BlocksLineOfSight { get; set; }
    public bool DifficultTerrain { get; set; }
    public bool DangerousTerrain { get; set; }
    public bool DeadlyTerrain { get; set; }
    public int CoverBonus { get; set; }
    public int MovementPenalty { get; set; }
}
