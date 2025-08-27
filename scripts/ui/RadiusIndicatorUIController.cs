using Godot;
using System.Collections.Generic;

public partial class RadiusIndicatorUIController : Control
{
    [Export] private Button _toggleAllButton;
    [Export] private Button _toggleBuffsButton;
    [Export] private Button _toggleDebuffsButton;
    [Export] private Button _toggleAurasButton;
    [Export] private Button _toggleTerrainButton;
    [Export] private Label _statusLabel;
    [Export] private ItemList _indicatorList;
    [Export] private Label _infoLabel;

    private RadiusIndicatorManager _radiusManager;
    private bool _showAllIndicators = false;
    private bool _showBuffIndicators = true;
    private bool _showDebuffIndicators = true;
    private bool _showAuraIndicators = true;
    private bool _showTerrainIndicators = true;

    public override void _Ready()
    {
        _radiusManager = RadiusIndicatorManager.Instance;
        
        SetupUI();
        ConnectSignals();
        UpdateStatusDisplay();
    }

    private void SetupUI()
    {
        if (_toggleAllButton != null)
        {
            _toggleAllButton.Text = "Show All Indicators";
        }

        if (_toggleBuffsButton != null)
        {
            _toggleBuffsButton.Text = "Show Buffs";
        }

        if (_toggleDebuffsButton != null)
        {
            _toggleDebuffsButton.Text = "Show Debuffs";
        }

        if (_toggleAurasButton != null)
        {
            _toggleAurasButton.Text = "Show Auras";
        }

        if (_toggleTerrainButton != null)
        {
            _toggleTerrainButton.Text = "Show Terrain";
        }
    }

    private void ConnectSignals()
    {
        if (_toggleAllButton != null)
            _toggleAllButton.Pressed += OnToggleAllPressed;
        
        if (_toggleBuffsButton != null)
            _toggleBuffsButton.Pressed += OnToggleBuffsPressed;
        
        if (_toggleDebuffsButton != null)
            _toggleDebuffsButton.Pressed += OnToggleDebuffsPressed;
        
        if (_toggleAurasButton != null)
            _toggleAurasButton.Pressed += OnToggleAurasPressed;
        
        if (_toggleTerrainButton != null)
            _toggleTerrainButton.Pressed += OnToggleTerrainPressed;
        
        if (_indicatorList != null)
            _indicatorList.ItemSelected += OnIndicatorSelected;
    }

    private void OnToggleAllPressed()
    {
        _showAllIndicators = !_showAllIndicators;
        _radiusManager.SetShowAllIndicators(_showAllIndicators);
        
        UpdateButtonText(_toggleAllButton, "Show All Indicators", "Hide All Indicators", _showAllIndicators);
        UpdateStatusDisplay();
    }

    private void OnToggleBuffsPressed()
    {
        _showBuffIndicators = !_showBuffIndicators;
        _radiusManager.SetShowBuffIndicators(_showBuffIndicators);
        
        UpdateButtonText(_toggleBuffsButton, "Show Buffs", "Hide Buffs", _showBuffIndicators);
        UpdateStatusDisplay();
    }

    private void OnToggleDebuffsPressed()
    {
        _showDebuffIndicators = !_showDebuffIndicators;
        _radiusManager.SetShowDebuffIndicators(_showDebuffIndicators);
        
        UpdateButtonText(_toggleDebuffsButton, "Show Debuffs", "Hide Debuffs", _showDebuffIndicators);
        UpdateStatusDisplay();
    }

    private void OnToggleAurasPressed()
    {
        _showAuraIndicators = !_showAuraIndicators;
        // This would control aura-specific indicators
        UpdateButtonText(_toggleAurasButton, "Show Auras", "Hide Auras", _showAuraIndicators);
        UpdateStatusDisplay();
    }

    private void OnToggleTerrainPressed()
    {
        _showTerrainIndicators = !_showTerrainIndicators;
        // This would control terrain-dependent indicators
        UpdateButtonText(_toggleTerrainButton, "Show Terrain", "Hide Terrain", _showTerrainIndicators);
        UpdateStatusDisplay();
    }

    private void OnIndicatorSelected(int index)
    {
        if (_indicatorList == null || _infoLabel == null) return;

        var selectedText = _indicatorList.GetItemText(index);
        var metadata = _indicatorList.GetItemMetadata(index);
        
        if (metadata is Unit unit)
        {
            var info = _radiusManager.GetIndicatorInfo(unit);
            _infoLabel.Text = info;
        }
    }

    private void UpdateButtonText(Button button, string showText, string hideText, bool isVisible)
    {
        if (button != null)
        {
            button.Text = isVisible ? hideText : showText;
        }
    }

    private void UpdateStatusDisplay()
    {
        if (_statusLabel == null) return;

        var status = "Radius Indicators:\n";
        status += $"All: {(_showAllIndicators ? "ON" : "OFF")}\n";
        status += $"Buffs: {(_showBuffIndicators ? "ON" : "OFF")}\n";
        status += $"Debuffs: {(_showDebuffIndicators ? "ON" : "OFF")}\n";
        status += $"Auras: {(_showAuraIndicators ? "ON" : "OFF")}\n";
        status += $"Terrain: {(_showTerrainIndicators ? "ON" : "OFF")}";

        _statusLabel.Text = status;
    }

    /// <summary>
    /// Refresh the indicator list with current units
    /// </summary>
    public void RefreshIndicatorList()
    {
        if (_indicatorList == null) return;

        _indicatorList.Clear();
        var gameManager = GameManager.Instance;
        
        if (gameManager != null)
        {
            var units = gameManager.GetAllUnits();
            foreach (var unit in units)
            {
                var itemText = $"{unit.UnitName} ({unit.ArmyName})";
                var itemIndex = _indicatorList.AddItem(itemText);
                _indicatorList.SetItemMetadata(itemIndex, unit);
            }
        }
    }

    /// <summary>
    /// Create example radius indicators for demonstration
    /// </summary>
    public void CreateExampleIndicators()
    {
        if (_radiusManager == null) return;

        var gameManager = GameManager.Instance;
        if (gameManager == null) return;

        var units = gameManager.GetAllUnits();
        foreach (var unit in units)
        {
            // Create appropriate indicators based on unit type
            if (unit.IsHero)
            {
                // Hero units get charge buff indicators
                _radiusManager.CreateChargeBuffIndicator(unit, 12.0f);
            }

            if (unit.IsGeneral)
            {
                // General units get bravery buff indicators
                _radiusManager.CreateBraveryBuffIndicator(unit, 18.0f);
            }

            if (unit.ArmyName == "Nighthaunt")
            {
                // Nighthaunt units get bravery debuff indicators
                _radiusManager.CreateBraveryDebuffIndicator(unit, 3.0f);
            }

            if (unit.ArmyName == "Sylvaneth")
            {
                // Sylvaneth units get terrain-dependent indicators
                _radiusManager.CreateTerrainDependentIndicator(unit, 6.0f, "+1 Save near terrain");
            }
        }

        // Show all indicators by default
        _showAllIndicators = true;
        _radiusManager.SetShowAllIndicators(true);
        UpdateStatusDisplay();
    }

    /// <summary>
    /// Clear all radius indicators
    /// </summary>
    public void ClearAllIndicators()
    {
        if (_radiusManager == null) return;

        var gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            var units = gameManager.GetAllUnits();
            foreach (var unit in units)
            {
                _radiusManager.RemoveUnitIndicators(unit);
            }
        }

        _showAllIndicators = false;
        _radiusManager.SetShowAllIndicators(false);
        UpdateStatusDisplay();
    }

    /// <summary>
    /// Get current indicator settings
    /// </summary>
    public Dictionary<string, bool> GetIndicatorSettings()
    {
        return new Dictionary<string, bool>
        {
            { "ShowAll", _showAllIndicators },
            { "ShowBuffs", _showBuffIndicators },
            { "ShowDebuffs", _showDebuffIndicators },
            { "ShowAuras", _showAuraIndicators },
            { "ShowTerrain", _showTerrainIndicators }
        };
    }

    /// <summary>
    /// Apply indicator settings
    /// </summary>
    public void ApplyIndicatorSettings(Dictionary<string, bool> settings)
    {
        if (settings.ContainsKey("ShowAll"))
        {
            _showAllIndicators = settings["ShowAll"];
            _radiusManager.SetShowAllIndicators(_showAllIndicators);
        }

        if (settings.ContainsKey("ShowBuffs"))
        {
            _showBuffIndicators = settings["ShowBuffs"];
            _radiusManager.SetShowBuffIndicators(_showBuffIndicators);
        }

        if (settings.ContainsKey("ShowDebuffs"))
        {
            _showDebuffIndicators = settings["ShowDebuffs"];
            _radiusManager.SetShowDebuffIndicators(_showDebuffIndicators);
        }

        if (settings.ContainsKey("ShowAuras"))
        {
            _showAuraIndicators = settings["ShowAuras"];
        }

        if (settings.ContainsKey("ShowTerrain"))
        {
            _showTerrainIndicators = settings["ShowTerrain"];
        }

        UpdateStatusDisplay();
    }

    /// <summary>
    /// Toggle indicators for a specific unit
    /// </summary>
    public void ToggleUnitIndicators(Unit unit)
    {
        if (_radiusManager != null)
        {
            _radiusManager.ToggleUnitIndicators(unit);
        }
    }

    /// <summary>
    /// Show only indicators for a specific unit
    /// </summary>
    public void ShowOnlyUnitIndicators(Unit unit)
    {
        if (_radiusManager == null) return;

        // Hide all indicators first
        _radiusManager.SetShowAllIndicators(false);

        // Show only the specified unit's indicators
        _radiusManager.ToggleUnitIndicators(unit);
    }

    /// <summary>
    /// Get information about all active indicators
    /// </summary>
    public string GetAllIndicatorsInfo()
    {
        if (_radiusManager == null) return "No radius manager available";

        var info = "Active Radius Indicators:\n\n";
        var gameManager = GameManager.Instance;
        
        if (gameManager != null)
        {
            var units = gameManager.GetAllUnits();
            foreach (var unit in units)
            {
                var unitInfo = _radiusManager.GetIndicatorInfo(unit);
                if (unitInfo != "No radius indicators")
                {
                    info += $"{unitInfo}\n\n";
                }
            }
        }

        if (info == "Active Radius Indicators:\n\n")
        {
            info += "No active radius indicators";
        }

        return info;
    }
}
