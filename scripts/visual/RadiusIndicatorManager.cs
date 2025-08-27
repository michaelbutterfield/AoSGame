using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RadiusIndicatorManager : Node
{
    public static RadiusIndicatorManager Instance { get; private set; }

    private Dictionary<Unit, List<RadiusIndicator>> _unitIndicators = new Dictionary<Unit, List<RadiusIndicator>>();
    private Dictionary<string, RadiusIndicatorTemplate> _indicatorTemplates = new Dictionary<string, RadiusIndicatorTemplate>();
    private bool _showAllIndicators = false;
    private bool _showBuffIndicators = true;
    private bool _showDebuffIndicators = true;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeIndicatorTemplates();
        }
        else
        {
            QueueFree();
        }
    }

    private void InitializeIndicatorTemplates()
    {
        // Buff radius templates
        AddIndicatorTemplate("ChargeBuff", new RadiusIndicatorTemplate
        {
            Name = "Charge Buff",
            Color = new Color(0, 1, 0, 0.3f), // Green with transparency
            BorderColor = new Color(0, 1, 0, 0.8f),
            BorderWidth = 2.0f,
            PulseEffect = true,
            PulseSpeed = 2.0f,
            ShowRangeText = true,
            RangeTextColor = new Color(0, 1, 0, 1.0f)
        });

        AddIndicatorTemplate("SaveBuff", new RadiusIndicatorTemplate
        {
            Name = "Save Buff",
            Color = new Color(0, 0, 1, 0.3f), // Blue with transparency
            BorderColor = new Color(0, 0, 1, 0.8f),
            BorderWidth = 2.0f,
            PulseEffect = false,
            ShowRangeText = true,
            RangeTextColor = new Color(0, 0, 1, 1.0f)
        });

        AddIndicatorTemplate("BraveryBuff", new RadiusIndicatorTemplate
        {
            Name = "Bravery Buff",
            Color = new Color(1, 1, 0, 0.3f), // Yellow with transparency
            BorderColor = new Color(1, 1, 0, 0.8f),
            BorderWidth = 2.0f,
            PulseEffect = true,
            PulseSpeed = 1.5f,
            ShowRangeText = true,
            RangeTextColor = new Color(1, 1, 0, 1.0f)
        });

        // Debuff radius templates
        AddIndicatorTemplate("BraveryDebuff", new RadiusIndicatorTemplate
        {
            Name = "Bravery Debuff",
            Color = new Color(1, 0, 0, 0.3f), // Red with transparency
            BorderColor = new Color(1, 0, 0, 0.8f),
            BorderWidth = 2.0f,
            PulseEffect = true,
            PulseSpeed = 3.0f,
            ShowRangeText = true,
            RangeTextColor = new Color(1, 0, 0, 1.0f)
        });

        AddIndicatorTemplate("MovementDebuff", new RadiusIndicatorTemplate
        {
            Name = "Movement Debuff",
            Color = new Color(0.5f, 0, 0.5f, 0.3f), // Purple with transparency
            BorderColor = new Color(0.5f, 0, 0.5f, 0.8f),
            BorderWidth = 2.0f,
            PulseEffect = false,
            ShowRangeText = true,
            RangeTextColor = new Color(0.5f, 0, 0.5f, 1.0f)
        });

        // Special ability templates
        AddIndicatorTemplate("Aura", new RadiusIndicatorTemplate
        {
            Name = "Aura",
            Color = new Color(1, 0.5f, 0, 0.3f), // Orange with transparency
            BorderColor = new Color(1, 0.5f, 0, 0.8f),
            BorderWidth = 3.0f,
            PulseEffect = true,
            PulseSpeed = 1.0f,
            ShowRangeText = true,
            RangeTextColor = new Color(1, 0.5f, 0, 1.0f)
        });

        AddIndicatorTemplate("TerrainDependent", new RadiusIndicatorTemplate
        {
            Name = "Terrain Dependent",
            Color = new Color(0, 1, 1, 0.3f), // Cyan with transparency
            BorderColor = new Color(0, 1, 1, 0.8f),
            BorderWidth = 2.0f,
            PulseEffect = true,
            PulseSpeed = 2.5f,
            ShowRangeText = true,
            RangeTextColor = new Color(0, 1, 1, 1.0f)
        });
    }

    private void AddIndicatorTemplate(string name, RadiusIndicatorTemplate template)
    {
        _indicatorTemplates[name] = template;
    }

    /// <summary>
    /// Create a radius indicator for a unit's ability
    /// </summary>
    public RadiusIndicator CreateRadiusIndicator(Unit unit, string templateName, float radiusInInches, string description = "")
    {
        if (!_indicatorTemplates.ContainsKey(templateName))
        {
            GD.PrintErr($"Radius indicator template {templateName} not found");
            return null;
        }

        var template = _indicatorTemplates[templateName];
        var indicator = new RadiusIndicator(unit, template, radiusInInches, description);
        
        // Add to unit's indicators
        if (!_unitIndicators.ContainsKey(unit))
            _unitIndicators[unit] = new List<RadiusIndicator>();
        
        _unitIndicators[unit].Add(indicator);
        
        // Add to scene
        AddChild(indicator);
        
        return indicator;
    }

    /// <summary>
    /// Create a charge buff radius indicator (e.g., Megaboss +1 to charge rolls within 12")
    /// </summary>
    public RadiusIndicator CreateChargeBuffIndicator(Unit unit, float radiusInInches)
    {
        return CreateRadiusIndicator(unit, "ChargeBuff", radiusInInches, $"+1 to charge rolls within {radiusInInches}\"");
    }

    /// <summary>
    /// Create a save buff radius indicator
    /// </summary>
    public RadiusIndicator CreateSaveBuffIndicator(Unit unit, float radiusInInches)
    {
        return CreateRadiusIndicator(unit, "SaveBuff", radiusInInches, $"+1 to save rolls within {radiusInInches}\"");
    }

    /// <summary>
    /// Create a bravery buff radius indicator
    /// </summary>
    public RadiusIndicator CreateBraveryBuffIndicator(Unit unit, float radiusInInches)
    {
        return CreateRadiusIndicator(unit, "BraveryBuff", radiusInInches, $"+1 to bravery within {radiusInInches}\"");
    }

    /// <summary>
    /// Create a bravery debuff radius indicator (e.g., Nighthaunt -1 bravery within 3")
    /// </summary>
    public RadiusIndicator CreateBraveryDebuffIndicator(Unit unit, float radiusInInches)
    {
        return CreateRadiusIndicator(unit, "BraveryDebuff", radiusInInches, $"-1 bravery within {radiusInInches}\"");
    }

    /// <summary>
    /// Create an aura radius indicator
    /// </summary>
    public RadiusIndicator CreateAuraIndicator(Unit unit, float radiusInInches, string effect)
    {
        return CreateRadiusIndicator(unit, "Aura", radiusInInches, effect);
    }

    /// <summary>
    /// Create a terrain-dependent radius indicator
    /// </summary>
    public RadiusIndicator CreateTerrainDependentIndicator(Unit unit, float radiusInInches, string effect)
    {
        return CreateRadiusIndicator(unit, "TerrainDependent", radiusInInches, effect);
    }

    /// <summary>
    /// Remove all radius indicators for a unit
    /// </summary>
    public void RemoveUnitIndicators(Unit unit)
    {
        if (_unitIndicators.ContainsKey(unit))
        {
            foreach (var indicator in _unitIndicators[unit])
            {
                indicator.QueueFree();
            }
            _unitIndicators.Remove(unit);
        }
    }

    /// <summary>
    /// Remove a specific radius indicator
    /// </summary>
    public void RemoveRadiusIndicator(RadiusIndicator indicator)
    {
        foreach (var unitIndicators in _unitIndicators.Values)
        {
            if (unitIndicators.Contains(indicator))
            {
                unitIndicators.Remove(indicator);
                indicator.QueueFree();
                break;
            }
        }
    }

    /// <summary>
    /// Show/hide all radius indicators
    /// </summary>
    public void SetShowAllIndicators(bool show)
    {
        _showAllIndicators = show;
        foreach (var unitIndicators in _unitIndicators.Values)
        {
            foreach (var indicator in unitIndicators)
            {
                indicator.Visible = show;
            }
        }
    }

    /// <summary>
    /// Show/hide buff radius indicators
    /// </summary>
    public void SetShowBuffIndicators(bool show)
    {
        _showBuffIndicators = show;
        foreach (var unitIndicators in _unitIndicators.Values)
        {
            foreach (var indicator in unitIndicators)
            {
                if (indicator.Template.Name.Contains("Buff"))
                {
                    indicator.Visible = show && _showAllIndicators;
                }
            }
        }
    }

    /// <summary>
    /// Show/hide debuff radius indicators
    /// </summary>
    public void SetShowDebuffIndicators(bool show)
    {
        _showDebuffIndicators = show;
        foreach (var unitIndicators in _unitIndicators.Values)
        {
            foreach (var indicator in unitIndicators)
            {
                if (indicator.Template.Name.Contains("Debuff"))
                {
                    indicator.Visible = show && _showAllIndicators;
                }
            }
        }
    }

    /// <summary>
    /// Get all units affected by a specific unit's buffs
    /// </summary>
    public List<Unit> GetUnitsInBuffRange(Unit sourceUnit, string buffType)
    {
        var affectedUnits = new List<Unit>();
        
        if (!_unitIndicators.ContainsKey(sourceUnit)) return affectedUnits;

        foreach (var indicator in _unitIndicators[sourceUnit])
        {
            if (indicator.Template.Name.Contains(buffType))
            {
                var unitsInRange = GetUnitsInRange(sourceUnit.Position, indicator.RadiusInInches);
                affectedUnits.AddRange(unitsInRange);
            }
        }

        return affectedUnits.Distinct().ToList();
    }

    /// <summary>
    /// Get all units within a specific range of a position
    /// </summary>
    private List<Unit> GetUnitsInRange(Vector3 center, float rangeInInches)
    {
        var unitsInRange = new List<Unit>();
        var gameManager = GameManager.Instance;
        
        if (gameManager == null) return unitsInRange;

        foreach (var unit in gameManager.GetAllUnits())
        {
            var distance = center.DistanceTo(unit.Position);
            var distanceInInches = gameManager.ConvertUnitsToInches(distance);
            
            if (distanceInInches <= rangeInInches && distanceInInches > 0)
            {
                unitsInRange.Add(unit);
            }
        }

        return unitsInRange;
    }

    /// <summary>
    /// Update all radius indicators (called each frame)
    /// </summary>
    public override void _Process(double delta)
    {
        foreach (var unitIndicators in _unitIndicators.Values)
        {
            foreach (var indicator in unitIndicators)
            {
                indicator.UpdateIndicator(delta);
            }
        }
    }

    /// <summary>
    /// Toggle radius indicators for a specific unit
    /// </summary>
    public void ToggleUnitIndicators(Unit unit)
    {
        if (_unitIndicators.ContainsKey(unit))
        {
            var visible = _unitIndicators[unit].FirstOrDefault()?.Visible ?? false;
            foreach (var indicator in _unitIndicators[unit])
            {
                indicator.Visible = !visible;
            }
        }
    }

    /// <summary>
    /// Get radius indicator information for display
    /// </summary>
    public string GetIndicatorInfo(Unit unit)
    {
        if (!_unitIndicators.ContainsKey(unit) || _unitIndicators[unit].Count == 0)
            return "No radius indicators";

        var info = $"Radius Indicators for {unit.UnitName}:\n";
        foreach (var indicator in _unitIndicators[unit])
        {
            info += $"• {indicator.Template.Name}: {indicator.RadiusInInches}\" - {indicator.Description}\n";
        }
        return info;
    }
}

// Template for radius indicators
public class RadiusIndicatorTemplate
{
    public string Name { get; set; }
    public Color Color { get; set; }
    public Color BorderColor { get; set; }
    public float BorderWidth { get; set; }
    public bool PulseEffect { get; set; }
    public float PulseSpeed { get; set; }
    public bool ShowRangeText { get; set; }
    public Color RangeTextColor { get; set; }
}

// Individual radius indicator instance
public partial class RadiusIndicator : Node3D
{
    public Unit SourceUnit { get; private set; }
    public RadiusIndicatorTemplate Template { get; private set; }
    public float RadiusInInches { get; private set; }
    public string Description { get; private set; }

    private MeshInstance3D _indicatorMesh;
    private Label3D _rangeLabel;
    private float _pulseTime = 0.0f;
    private bool _isVisible = true;

    public bool Visible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            if (_indicatorMesh != null)
                _indicatorMesh.Visible = value;
            if (_rangeLabel != null)
                _rangeLabel.Visible = value;
        }
    }

    public RadiusIndicator(Unit unit, RadiusIndicatorTemplate template, float radiusInInches, string description)
    {
        SourceUnit = unit;
        Template = template;
        RadiusInInches = radiusInInches;
        Description = description;
    }

    public override void _Ready()
    {
        CreateVisualIndicator();
        UpdatePosition();
    }

    private void CreateVisualIndicator()
    {
        // Create the main indicator mesh (cylinder)
        _indicatorMesh = new MeshInstance3D();
        var cylinderMesh = new CylinderMesh();
        
        // Convert inches to game units
        var radiusInUnits = GameManager.Instance.ConvertInchesToUnits(RadiusInInches);
        cylinderMesh.TopRadius = radiusInUnits;
        cylinderMesh.BottomRadius = radiusInUnits;
        cylinderMesh.Height = 0.05f; // Very thin
        
        _indicatorMesh.Mesh = cylinderMesh;
        _indicatorMesh.MaterialOverride = CreateMaterial();
        AddChild(_indicatorMesh);

        // Create range label
        if (Template.ShowRangeText)
        {
            _rangeLabel = new Label3D();
            _rangeLabel.Text = $"{RadiusInInches}\"";
            _rangeLabel.FontSize = 24;
            _rangeLabel.Modulate = Template.RangeTextColor;
            _rangeLabel.Position = new Vector3(0, 0.1f, 0);
            _rangeLabel.Billboard = true; // Always face camera
            AddChild(_rangeLabel);
        }
    }

    private StandardMaterial3D CreateMaterial()
    {
        var material = new StandardMaterial3D();
        material.AlbedoColor = Template.Color;
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        material.AlbedoMode = BaseMaterial3D.AlbedoModeEnum.Constant;
        material.EmissionEnabled = true;
        material.EmissionColor = Template.BorderColor;
        material.EmissionEnergy = 0.2f;
        
        return material;
    }

    private void UpdatePosition()
    {
        if (SourceUnit != null)
        {
            GlobalPosition = SourceUnit.GlobalPosition;
            GlobalPosition = new Vector3(GlobalPosition.X, 0.01f, GlobalPosition.Z); // Slightly above ground
        }
    }

    public void UpdateIndicator(double delta)
    {
        if (!_isVisible) return;

        UpdatePosition();

        // Handle pulse effect
        if (Template.PulseEffect)
        {
            _pulseTime += (float)delta * Template.PulseSpeed;
            var pulseScale = 1.0f + 0.1f * Mathf.Sin(_pulseTime);
            
            if (_indicatorMesh != null)
            {
                _indicatorMesh.Scale = new Vector3(pulseScale, 1.0f, pulseScale);
            }
        }

        // Update material properties for dynamic effects
        if (_indicatorMesh?.MaterialOverride is StandardMaterial3D material)
        {
            // Add some subtle animation to the emission
            var emissionIntensity = 0.2f + 0.1f * Mathf.Sin(_pulseTime * 0.5f);
            material.EmissionEnergy = emissionIntensity;
        }
    }

    /// <summary>
    /// Check if a position is within this indicator's range
    /// </summary>
    public bool IsPositionInRange(Vector3 position)
    {
        var distance = GlobalPosition.DistanceTo(position);
        var rangeInUnits = GameManager.Instance.ConvertInchesToUnits(RadiusInInches);
        return distance <= rangeInUnits;
    }

    /// <summary>
    /// Get the buff effect for units within range
    /// </summary>
    public string GetBuffEffect()
    {
        return Description;
    }

    /// <summary>
    /// Highlight the indicator (e.g., when hovering over it)
    /// </summary>
    public void Highlight()
    {
        if (_indicatorMesh?.MaterialOverride is StandardMaterial3D material)
        {
            material.EmissionEnergy = 0.5f;
        }
    }

    /// <summary>
    /// Remove highlight
    /// </summary>
    public void RemoveHighlight()
    {
        if (_indicatorMesh?.MaterialOverride is StandardMaterial3D material)
        {
            material.EmissionEnergy = 0.2f;
        }
    }
}
