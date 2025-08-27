using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using AoSGame.Units;

namespace AoSGame.UI
{
    /// <summary>
    /// Controller for the Unit Ability UI, displays and manages unit abilities
    /// </summary>
    public partial class UnitAbilityUIController : Control
    {
        // UI References
        private Label _unitNameLabel;
        private Label _unitStatsLabel;
        private VBoxContainer _unitAbilitiesContainer;
        private VBoxContainer _modelAbilitiesContainer;
        private Label _selectedAbilityLabel;
        private Label _abilityDescriptionLabel;
        private Label _abilityFlavorLabel;
        private Label _abilityActivationLabel;
        private Label _abilityRangeLabel;
        private Button _activateButton;
        private Button _closeButton;
        
        // Current state
        private Unit _selectedUnit;
        private UnitAbility _selectedUnitAbility;
        private ModelAbility _selectedModelAbility;
        private bool _isUnitAbilitySelected = false;
        
        public override void _Ready()
        {
            SetupUI();
            ConnectSignals();
            Hide(); // Start hidden
        }
        
        private void SetupUI()
        {
            _unitNameLabel = GetNode<Label>("VBoxContainer/UnitInfoContainer/UnitNameLabel");
            _unitStatsLabel = GetNode<Label>("VBoxContainer/UnitInfoContainer/UnitStatsLabel");
            _unitAbilitiesContainer = GetNode<VBoxContainer>("VBoxContainer/AbilitiesScrollContainer/AbilitiesVBox/UnitAbilitiesContainer");
            _modelAbilitiesContainer = GetNode<VBoxContainer>("VBoxContainer/AbilitiesScrollContainer/AbilitiesVBox/ModelAbilitiesContainer");
            _selectedAbilityLabel = GetNode<Label>("VBoxContainer/AbilityDetailsContainer/SelectedAbilityLabel");
            _abilityDescriptionLabel = GetNode<Label>("VBoxContainer/AbilityDetailsContainer/AbilityDescriptionLabel");
            _abilityFlavorLabel = GetNode<Label>("VBoxContainer/AbilityDetailsContainer/AbilityFlavorLabel");
            _abilityActivationLabel = GetNode<Label>("VBoxContainer/AbilityDetailsContainer/AbilityActivationLabel");
            _abilityRangeLabel = GetNode<Label>("VBoxContainer/AbilityDetailsContainer/AbilityRangeLabel");
            _activateButton = GetNode<Button>("VBoxContainer/ActionContainer/ActivateButton");
            _closeButton = GetNode<Button>("VBoxContainer/ActionContainer/CloseButton");
        }
        
        private void ConnectSignals()
        {
            _activateButton.Pressed += OnActivateButtonPressed;
            _closeButton.Pressed += OnCloseButtonPressed;
        }
        
        /// <summary>
        /// Display the UI for a specific unit
        /// </summary>
        public void ShowForUnit(Unit unit)
        {
            if (unit == null) return;
            
            _selectedUnit = unit;
            UpdateUnitInfo();
            UpdateAbilitiesDisplay();
            ClearAbilityDetails();
            Show();
        }
        
        /// <summary>
        /// Update the unit information display
        /// </summary>
        private void UpdateUnitInfo()
        {
            if (_selectedUnit == null) return;
            
            _unitNameLabel.Text = $"Unit: {_selectedUnit.UnitName}";
            _unitStatsLabel.Text = $"Stats: M: {_selectedUnit.Move}\" | W: {_selectedUnit.Wounds}/{_selectedUnit.MaxWounds} | B: {_selectedUnit.Bravery} | Sv: {_selectedUnit.Save}+";
        }
        
        /// <summary>
        /// Update the abilities display for the selected unit
        /// </summary>
        private void UpdateAbilitiesDisplay()
        {
            if (_selectedUnit == null) return;
            
            ClearAbilitiesContainers();
            
            // Display unit abilities
            foreach (var ability in _selectedUnit.UnitAbilities)
            {
                CreateAbilityButton(ability, _unitAbilitiesContainer, true);
            }
            
            // Display model abilities
            foreach (var ability in _selectedUnit.ModelAbilities)
            {
                CreateAbilityButton(ability, _modelAbilitiesContainer, false);
            }
        }
        
        /// <summary>
        /// Create a button for an ability
        /// </summary>
        private void CreateAbilityButton(dynamic ability, VBoxContainer container, bool isUnitAbility)
        {
            var button = new Button();
            button.Text = ability.Name;
            button.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            
            // Style based on ability type
            if (isUnitAbility)
            {
                var unitAbility = ability as UnitAbility;
                if (unitAbility.Type == AbilityType.Passive)
                {
                    button.Modulate = new Color(0.7f, 0.9f, 0.7f); // Green for passive
                }
                else if (unitAbility.Type == AbilityType.Active)
                {
                    button.Modulate = new Color(0.9f, 0.7f, 0.7f); // Red for active
                }
                else if (unitAbility.Type == AbilityType.Reactive)
                {
                    button.Modulate = new Color(0.7f, 0.7f, 0.9f); // Blue for reactive
                }
                
                button.Pressed += () => OnUnitAbilitySelected(unitAbility);
            }
            else
            {
                var modelAbility = ability as ModelAbility;
                button.Modulate = new Color(0.9f, 0.9f, 0.7f); // Yellow for model abilities
                button.Pressed += () => OnModelAbilitySelected(modelAbility);
            }
            
            container.AddChild(button);
        }
        
        /// <summary>
        /// Clear the abilities containers
        /// </summary>
        private void ClearAbilitiesContainers()
        {
            // Clear unit abilities
            foreach (var child in _unitAbilitiesContainer.GetChildren())
            {
                child.QueueFree();
            }
            
            // Clear model abilities
            foreach (var child in _modelAbilitiesContainer.GetChildren())
            {
                child.QueueFree();
            }
        }
        
        /// <summary>
        /// Handle unit ability selection
        /// </summary>
        private void OnUnitAbilitySelected(UnitAbility ability)
        {
            _selectedUnitAbility = ability;
            _selectedModelAbility = null;
            _isUnitAbilitySelected = true;
            
            UpdateAbilityDetails();
            UpdateActivateButton();
        }
        
        /// <summary>
        /// Handle model ability selection
        /// </summary>
        private void OnModelAbilitySelected(ModelAbility ability)
        {
            _selectedModelAbility = ability;
            _selectedUnitAbility = null;
            _isUnitAbilitySelected = false;
            
            UpdateAbilityDetails();
            UpdateActivateButton();
        }
        
        /// <summary>
        /// Update the ability details display
        /// </summary>
        private void UpdateAbilityDetails()
        {
            if (_isUnitAbilitySelected && _selectedUnitAbility != null)
            {
                var ability = _selectedUnitAbility;
                _selectedAbilityLabel.Text = $"Selected Ability: {ability.Name}";
                _abilityDescriptionLabel.Text = $"Description: {ability.Description}";
                _abilityFlavorLabel.Text = $"Flavor: {ability.FlavorText}";
                _abilityActivationLabel.Text = $"Activation: {GetActivationPhasesText(ability.ActivationPhases)}";
                _abilityRangeLabel.Text = $"Range: {(ability.Range > 0 ? $"{ability.Range}\"" : "Self")}";
            }
            else if (!_isUnitAbilitySelected && _selectedModelAbility != null)
            {
                var ability = _selectedModelAbility;
                _selectedAbilityLabel.Text = $"Selected Model Ability: {ability.Name}";
                _abilityDescriptionLabel.Text = $"Description: {ability.Description}";
                _abilityFlavorLabel.Text = $"Flavor: {ability.FlavorText}";
                _abilityActivationLabel.Text = $"Activation: {GetActivationPhasesText(ability.ActivationPhases)}";
                _abilityRangeLabel.Text = $"Range: {(ability.Range > 0 ? $"{ability.Range}\"" : "Self")}";
            }
        }
        
        /// <summary>
        /// Get a formatted string for activation phases
        /// </summary>
        private string GetActivationPhasesText(List<GameManager.TurnPhase> phases)
        {
            if (phases == null || phases.Count == 0) return "Always";
            
            var phaseNames = phases.Select(p => p.ToString()).ToArray();
            return string.Join(", ", phaseNames);
        }
        
        /// <summary>
        /// Update the activate button state
        /// </summary>
        private void UpdateActivateButton()
        {
            if (_selectedUnit == null)
            {
                _activateButton.Disabled = true;
                return;
            }
            
            bool canActivate = false;
            
            if (_isUnitAbilitySelected && _selectedUnitAbility != null)
            {
                canActivate = _selectedUnitAbility.CanActivate(_selectedUnit);
            }
            else if (!_isUnitAbilitySelected && _selectedModelAbility != null)
            {
                canActivate = _selectedModelAbility.CanActivate(_selectedUnit);
            }
            
            _activateButton.Disabled = !canActivate;
        }
        
        /// <summary>
        /// Clear the ability details display
        /// </summary>
        private void ClearAbilityDetails()
        {
            _selectedAbilityLabel.Text = "Selected Ability: None";
            _abilityDescriptionLabel.Text = "Description: Select an ability to see details";
            _abilityFlavorLabel.Text = "Flavor: ";
            _abilityActivationLabel.Text = "Activation: ";
            _abilityRangeLabel.Text = "Range: ";
            _activateButton.Disabled = true;
        }
        
        /// <summary>
        /// Handle activate button press
        /// </summary>
        private void OnActivateButtonPressed()
        {
            if (_selectedUnit == null) return;
            
            bool success = false;
            
            if (_isUnitAbilitySelected && _selectedUnitAbility != null)
            {
                success = _selectedUnit.ActivateUnitAbility(_selectedUnitAbility.Name);
                if (success)
                {
                    GD.Print($"Successfully activated unit ability: {_selectedUnitAbility.Name}");
                    UpdateAbilitiesDisplay(); // Refresh display
                }
            }
            else if (!_isUnitAbilitySelected && _selectedModelAbility != null)
            {
                success = _selectedUnit.ActivateModelAbility(_selectedModelAbility.Name);
                if (success)
                {
                    GD.Print($"Successfully activated model ability: {_selectedModelAbility.Name}");
                    UpdateAbilitiesDisplay(); // Refresh display
                }
            }
            
            if (success)
            {
                UpdateActivateButton();
            }
        }
        
        /// <summary>
        /// Handle close button press
        /// </summary>
        private void OnCloseButtonPressed()
        {
            Hide();
        }
        
        /// <summary>
        /// Show the UI
        /// </summary>
        public new void Show()
        {
            Visible = true;
            ProcessMode = ProcessModeEnum.Inherit;
        }
        
        /// <summary>
        /// Hide the UI
        /// </summary>
        public new void Hide()
        {
            Visible = false;
            ProcessMode = ProcessModeEnum.Disabled;
        }
        
        /// <summary>
        /// Toggle the UI visibility
        /// </summary>
        public void Toggle()
        {
            if (Visible)
                Hide();
            else
                Show();
        }
    }
}
