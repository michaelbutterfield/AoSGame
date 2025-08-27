using Godot;
using System;
using System.Collections.Generic;

public partial class Terrain : Node3D
{
    [Signal]
    public delegate void TerrainPlacedEventHandler(Terrain terrain, Vector3 position);

    [Export]
    public string TerrainName { get; set; } = "";

    [Export]
    public TerrainType Type { get; set; } = TerrainType.Forest;

    [Export]
    public float Width { get; set; } = 3.0f; // in inches

    [Export]
    public float Length { get; set; } = 3.0f; // in inches

    [Export]
    public float Height { get; set; } = 2.0f; // in inches

    [Export]
    public bool BlocksLineOfSight { get; set; } = true;

    [Export]
    public bool DifficultTerrain { get; set; } = false;

    [Export]
    public bool DangerousTerrain { get; set; } = false;

    [Export]
    public bool MysticalTerrain { get; set; } = false;

    [Export]
    public bool DeadlyTerrain { get; set; } = false;

    [Export]
    public int CoverBonus { get; set; } = 0; // +1 to save rolls

    [Export]
    public int MovementPenalty { get; set; } = 0; // inches of movement lost

    [Export]
    public string TerrainEffect { get; set; } = "";

    // Visual components
    private MeshInstance3D _mesh;
    private CollisionShape3D _collision;
    private Area3D _area;

    public override void _Ready()
    {
        SetupVisualComponents();
        SetupCollision();
    }

    private void SetupVisualComponents()
    {
        // Create mesh based on terrain type
        _mesh = new MeshInstance3D();
        
        switch (Type)
        {
            case TerrainType.Forest:
                CreateForestMesh();
                break;
            case TerrainType.Hill:
                CreateHillMesh();
                break;
            case TerrainType.Ruins:
                CreateRuinsMesh();
                break;
            case TerrainType.Water:
                CreateWaterMesh();
                break;
            case TerrainType.Objective:
                CreateObjectiveMesh();
                break;
            default:
                CreateGenericMesh();
                break;
        }
        
        AddChild(_mesh);
    }

    private void CreateForestMesh()
    {
        var boxMesh = new BoxMesh();
        boxMesh.Size = new Vector3(
            GameManager.Instance.ConvertInchesToUnits(Width),
            GameManager.Instance.ConvertInchesToUnits(Height),
            GameManager.Instance.ConvertInchesToUnits(Length)
        );
        _mesh.Mesh = boxMesh;
        
        // Add green material
        var material = new StandardMaterial3D();
        material.AlbedoColor = new Color(0.2f, 0.6f, 0.2f);
        _mesh.MaterialOverride = material;
    }

    private void CreateHillMesh()
    {
        var cylinderMesh = new CylinderMesh();
        cylinderMesh.TopRadius = GameManager.Instance.ConvertInchesToUnits(Width) / 2.0f;
        cylinderMesh.BottomRadius = GameManager.Instance.ConvertInchesToUnits(Width) / 2.0f;
        cylinderMesh.Height = GameManager.Instance.ConvertInchesToUnits(Height);
        _mesh.Mesh = cylinderMesh;
        
        // Add brown material
        var material = new StandardMaterial3D();
        material.AlbedoColor = new Color(0.6f, 0.4f, 0.2f);
        _mesh.MaterialOverride = material;
    }

    private void CreateRuinsMesh()
    {
        var boxMesh = new BoxMesh();
        boxMesh.Size = new Vector3(
            GameManager.Instance.ConvertInchesToUnits(Width),
            GameManager.Instance.ConvertInchesToUnits(Height),
            GameManager.Instance.ConvertInchesToUnits(Length)
        );
        _mesh.Mesh = boxMesh;
        
        // Add stone material
        var material = new StandardMaterial3D();
        material.AlbedoColor = new Color(0.5f, 0.5f, 0.5f);
        _mesh.MaterialOverride = material;
    }

    private void CreateWaterMesh()
    {
        var planeMesh = new PlaneMesh();
        planeMesh.Size = new Vector2(
            GameManager.Instance.ConvertInchesToUnits(Width),
            GameManager.Instance.ConvertInchesToUnits(Length)
        );
        _mesh.Mesh = planeMesh;
        
        // Add blue material
        var material = new StandardMaterial3D();
        material.AlbedoColor = new Color(0.2f, 0.4f, 0.8f);
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        material.AlbedoColor.A = 0.7f;
        _mesh.MaterialOverride = material;
    }

    private void CreateObjectiveMesh()
    {
        var cylinderMesh = new CylinderMesh();
        cylinderMesh.TopRadius = GameManager.Instance.ConvertInchesToUnits(Width) / 2.0f;
        cylinderMesh.BottomRadius = GameManager.Instance.ConvertInchesToUnits(Width) / 2.0f;
        cylinderMesh.Height = GameManager.Instance.ConvertInchesToUnits(Height);
        _mesh.Mesh = cylinderMesh;
        
        // Add gold material
        var material = new StandardMaterial3D();
        material.AlbedoColor = new Color(1.0f, 0.8f, 0.0f);
        _mesh.MaterialOverride = material;
    }

    private void CreateGenericMesh()
    {
        var boxMesh = new BoxMesh();
        boxMesh.Size = new Vector3(
            GameManager.Instance.ConvertInchesToUnits(Width),
            GameManager.Instance.ConvertInchesToUnits(Height),
            GameManager.Instance.ConvertInchesToUnits(Length)
        );
        _mesh.Mesh = boxMesh;
    }

    private void SetupCollision()
    {
        // Create collision area
        _area = new Area3D();
        var collisionShape = new CollisionShape3D();
        
        var boxShape = new BoxShape3D();
        boxShape.Size = new Vector3(
            GameManager.Instance.ConvertInchesToUnits(Width),
            GameManager.Instance.ConvertInchesToUnits(Height),
            GameManager.Instance.ConvertInchesToUnits(Length)
        );
        collisionShape.Shape = boxShape;
        
        _area.AddChild(collisionShape);
        AddChild(_area);
        
        // Connect area signals
        _area.BodyEntered += OnBodyEntered;
        _area.BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is Unit unit)
        {
            ApplyTerrainEffects(unit, true);
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (body is Unit unit)
        {
            ApplyTerrainEffects(unit, false);
        }
    }

    private void ApplyTerrainEffects(Unit unit, bool entering)
    {
        if (entering)
        {
            // Apply terrain effects when entering
            if (CoverBonus > 0)
            {
                unit.Save += CoverBonus;
                GD.Print($"Terrain: {unit.UnitName} gains +{CoverBonus} to save from {TerrainName}");
            }
            
            if (DangerousTerrain)
            {
                GD.Print($"Terrain: {unit.UnitName} enters dangerous terrain {TerrainName}");
                // Roll for dangerous terrain test
                int roll = DiceManager.RollD6();
                if (roll == 1)
                {
                    unit.TakeDamage(1);
                    GD.Print($"Terrain: {unit.UnitName} takes 1 mortal wound from dangerous terrain");
                }
            }
            
            if (DeadlyTerrain)
            {
                GD.Print($"Terrain: {unit.UnitName} enters deadly terrain {TerrainName}");
                // Roll for deadly terrain test
                int roll = DiceManager.RollD6();
                if (roll <= 2)
                {
                    unit.TakeDamage(1);
                    GD.Print($"Terrain: {unit.UnitName} takes 1 mortal wound from deadly terrain");
                }
            }
        }
        else
        {
            // Remove terrain effects when exiting
            if (CoverBonus > 0)
            {
                unit.Save -= CoverBonus;
                GD.Print($"Terrain: {unit.UnitName} loses +{CoverBonus} to save from {TerrainName}");
            }
        }
    }

    public bool IsUnitInTerrain(Unit unit)
    {
        // Check if unit is within terrain bounds
        Vector3 terrainCenter = GlobalPosition;
        Vector3 unitPos = unit.GlobalPosition;
        
        float halfWidth = GameManager.Instance.ConvertInchesToUnits(Width) / 2.0f;
        float halfLength = GameManager.Instance.ConvertInchesToUnits(Length) / 2.0f;
        
        return Math.Abs(unitPos.X - terrainCenter.X) <= halfWidth &&
               Math.Abs(unitPos.Z - terrainCenter.Z) <= halfLength;
    }

    public bool BlocksLineOfSightBetween(Vector3 from, Vector3 to)
    {
        if (!BlocksLineOfSight) return false;
        
        // Simple line-of-sight check
        // In a full implementation, you'd do proper raycasting
        Vector3 terrainCenter = GlobalPosition;
        float halfWidth = GameManager.Instance.ConvertInchesToUnits(Width) / 2.0f;
        float halfLength = GameManager.Instance.ConvertInchesToUnits(Length) / 2.0f;
        float height = GameManager.Instance.ConvertInchesToUnits(Height);
        
        // Check if line intersects with terrain bounds
        // This is a simplified check - full implementation would be more complex
        return true; // Placeholder
    }

    public float GetMovementCost()
    {
        if (DifficultTerrain)
            return GameManager.Instance.ConvertInchesToUnits(MovementPenalty);
        return 0;
    }

    public void SetPosition(Vector3 position)
    {
        GlobalPosition = position;
        EmitSignal(SignalName.TerrainPlaced, this, position);
    }

    public static Terrain CreateTerrain(TerrainType type, Vector3 position)
    {
        var terrain = new Terrain();
        
        switch (type)
        {
            case TerrainType.Forest:
                terrain.TerrainName = "Forest";
                terrain.BlocksLineOfSight = true;
                terrain.DifficultTerrain = true;
                terrain.CoverBonus = 1;
                terrain.MovementPenalty = 2;
                terrain.Width = 6.0f;
                terrain.Length = 6.0f;
                terrain.Height = 4.0f;
                break;
                
            case TerrainType.Hill:
                terrain.TerrainName = "Hill";
                terrain.BlocksLineOfSight = false;
                terrain.DifficultTerrain = false;
                terrain.Width = 8.0f;
                terrain.Length = 8.0f;
                terrain.Height = 3.0f;
                break;
                
            case TerrainType.Ruins:
                terrain.TerrainName = "Ruins";
                terrain.BlocksLineOfSight = true;
                terrain.DifficultTerrain = true;
                terrain.CoverBonus = 1;
                terrain.MovementPenalty = 1;
                terrain.Width = 4.0f;
                terrain.Length = 4.0f;
                terrain.Height = 3.0f;
                break;
                
            case TerrainType.Water:
                terrain.TerrainName = "Water";
                terrain.BlocksLineOfSight = false;
                terrain.DifficultTerrain = true;
                terrain.DangerousTerrain = true;
                terrain.MovementPenalty = 3;
                terrain.Width = 6.0f;
                terrain.Length = 6.0f;
                terrain.Height = 0.5f;
                break;
                
            case TerrainType.Objective:
                terrain.TerrainName = "Objective";
                terrain.BlocksLineOfSight = false;
                terrain.DifficultTerrain = false;
                terrain.Width = 2.0f;
                terrain.Length = 2.0f;
                terrain.Height = 1.0f;
                break;
        }
        
        terrain.Type = type;
        terrain.SetPosition(position);
        
        return terrain;
    }
}

public enum TerrainType
{
    Forest,
    Hill,
    Ruins,
    Water,
    Objective,
    Custom
}
