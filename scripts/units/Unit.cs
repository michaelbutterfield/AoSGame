using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Unit : Node3D
{
    [Signal]
    public delegate void UnitSelectedEventHandler(Unit unit);

    [Signal]
    public delegate void UnitMovedEventHandler(Unit unit, Vector3 oldPosition, Vector3 newPosition);

    [Signal]
    public delegate void UnitDamagedEventHandler(Unit unit, int damage);

    [Signal]
    public delegate void UnitDestroyedEventHandler(Unit unit);

    [Export]
    public int Id { get; set; }

    [Export]
    public string UnitName { get; set; } = "";

    [Export]
    public int PlayerId { get; set; }

    [Export]
    public string ArmyName { get; set; } = "";

    // Unit Stats (from Warscroll)
    [Export]
    public int Move { get; set; } = 6; // Movement in inches

    [Export]
    public int Wounds { get; set; } = 1; // Current wounds

    [Export]
    public int MaxWounds { get; set; } = 1; // Maximum wounds

    [Export]
    public int Bravery { get; set; } = 6; // Bravery characteristic

    [Export]
    public int Save { get; set; } = 4; // Save characteristic

    // Combat Stats
    [Export]
    public int Attacks { get; set; } = 1; // Number of attacks

    [Export]
    public int ToHit { get; set; } = 4; // Hit roll needed

    [Export]
    public int ToWound { get; set; } = 4; // Wound roll needed

    [Export]
    public int Rend { get; set; } = 0; // Rend characteristic

    [Export]
    public int Damage { get; set; } = 1; // Damage characteristic

    // Unit Properties
    [Export]
    public int ModelCount { get; set; } = 1; // Number of models in unit

    [Export]
    public float BaseSize { get; set; } = 32.0f; // Base size in millimeters

    [Export]
    public bool IsHero { get; set; } = false;

    [Export]
    public bool IsGeneral { get; set; } = false;

    [Export]
    public bool IsWizard { get; set; } = false;

    [Export]
    public bool IsPriest { get; set; } = false;

    // Game State
    [Export]
    public bool HasMoved { get; set; } = false;

    [Export]
    public bool HasShot { get; set; } = false;

    [Export]
    public bool HasCharged { get; set; } = false;

    [Export]
    public bool HasFought { get; set; } = false;

    [Export]
    public bool IsSelected { get; set; } = false;

    [Export]
    public bool IsEngaged { get; set; } = false;

    [Export]
    public Vector3 Position { get; set; }

    // Abilities and Spells
    public List<Ability> Abilities { get; set; } = new List<Ability>();
    public List<Spell> Spells { get; set; } = new List<Spell>();
    public List<CommandAbility> CommandAbilities { get; set; } = new List<CommandAbility>();
    
    // Enhanced Unit and Model Abilities
    public List<UnitAbility> UnitAbilities { get; set; } = new List<UnitAbility>();
    public List<ModelAbility> ModelAbilities { get; set; } = new List<ModelAbility>();
    
    // Model-specific properties
    public bool HasChampion { get; set; } = false;
    public bool HasMusician { get; set; } = false;
    public bool HasBannerBearer { get; set; } = false;
    public bool HasSpecialWeapon { get; set; } = false;
    
    // Enhanced unit properties
    public bool CanFly { get; set; } = false;
    public bool CanRunAndCharge { get; set; } = false;
    public bool CanRunAndShoot { get; set; } = false;
    public bool FightsFirst { get; set; } = false;
    public bool FightsLast { get; set; } = false;
    public bool IgnoreCover { get; set; } = false;
    public bool IgnoreBattleshock { get; set; } = false;
    public int WardSave { get; set; } = 0; // 0 = no ward, >0 = ward save value
    public bool Regeneration { get; set; } = false;
    public bool IsEthereal { get; set; } = false;
    
    // Base size and deployment properties
    public float BaseSizeInMillimeters { get; set; } = 32.0f; // Default 32mm base
    public bool IsDeployed { get; set; } = false;
    public Vector3 DeploymentPosition { get; set; } = Vector3.Zero;

    // Equipment and Enhancements
    public List<Weapon> Weapons { get; set; } = new List<Weapon>();
    public List<Armour> Armour { get; set; } = new List<Armour>();
    public List<Artefact> Artefacts { get; set; } = new List<Artefact>();

    // Temporary Effects (from Command Abilities, Spells, etc.)
    public List<TemporaryEffect> TemporaryEffects { get; set; } = new List<TemporaryEffect>();

    // Visual Components
    private MeshInstance3D _model;
    private CollisionShape3D _collision;
    private Node3D _selectionIndicator;
    private Label3D _woundLabel;

    public override void _Ready()
    {
        SetupVisualComponents();
        UpdateWoundDisplay();
    }

    private void SetupVisualComponents()
    {
        // Create basic visual representation
        _model = new MeshInstance3D();
        var boxMesh = new BoxMesh();
        var baseSizeInUnits = GetBaseSizeInUnits();
        boxMesh.Size = new Vector3(baseSizeInUnits, 0.5f, baseSizeInUnits);
        _model.Mesh = boxMesh;
        AddChild(_model);

        // Create collision
        _collision = new CollisionShape3D();
        var boxShape = new BoxShape3D();
        boxShape.Size = new Vector3(baseSizeInUnits, 0.5f, baseSizeInUnits);
        _collision.Shape = boxShape;
        AddChild(_collision);

        // Create selection indicator
        _selectionIndicator = new Node3D();
        var indicatorMesh = new MeshInstance3D();
        var cylinderMesh = new CylinderMesh();
        cylinderMesh.TopRadius = baseSizeInUnits + 0.2f;
        cylinderMesh.BottomRadius = baseSizeInUnits + 0.2f;
        cylinderMesh.Height = 0.1f;
        indicatorMesh.Mesh = cylinderMesh;
        indicatorMesh.Visible = false;
        _selectionIndicator.AddChild(indicatorMesh);
        AddChild(_selectionIndicator);

        // Create wound display
        _woundLabel = new Label3D();
        _woundLabel.Text = $"{Wounds}/{MaxWounds}";
        _woundLabel.Position = new Vector3(0, 1.0f, 0);
        AddChild(_woundLabel);
    }

    /// <summary>
    /// Get the base size in millimeters
    /// </summary>
    public float GetBaseSizeInMillimeters()
    {
        return BaseSizeInMillimeters > 0 ? BaseSizeInMillimeters : BaseSize;
    }

    /// <summary>
    /// Get the base size in inches
    /// </summary>
    public float GetBaseSizeInInches()
    {
        return GetBaseSizeInMillimeters() / 25.4f; // Convert mm to inches
    }

    /// <summary>
    /// Get the base size as a formatted string
    /// </summary>
    public string GetBaseSizeString()
    {
        var inches = GetBaseSizeInInches();
        if (inches >= 1.0f)
        {
            return $"{inches:F1}\" ({GetBaseSizeInMillimeters()}mm)";
        }
        else
        {
            return $"{GetBaseSizeInMillimeters()}mm";
        }
    }

    /// <summary>
    /// Get movement as a formatted string in inches
    /// </summary>
    public string GetMovementString()
    {
        return $"{Move}\"";
    }

    /// <summary>
    /// Get range as a formatted string in inches
    /// </summary>
    public string GetRangeString(float range)
    {
        return $"{range:F1}\"";
    }

    /// <summary>
    /// Check if unit can fly over terrain and other units
    /// </summary>
    public bool CanFlyOverObstacles()
    {
        return CanFly || IsEthereal;
    }

    /// <summary>
    /// Get the unit's footprint radius in inches (for movement and positioning)
    /// </summary>
    public float GetFootprintRadiusInInches()
    {
        return GetBaseSizeInInches() / 2.0f;
    }

    /// <summary>
    /// Check if a position is within the unit's base area
    /// </summary>
    public bool IsPositionWithinBase(Vector3 position)
    {
        var distance = Position.DistanceTo(position);
        var radius = GetFootprintRadiusInInches();
        return distance <= radius;
    }

    /// <summary>
    /// Get the unit's engagement range in inches (base size + 1" for melee)
    /// </summary>
    public float GetEngagementRangeInInches()
    {
        return GetBaseSizeInInches() + 1.0f;
    }

    /// <summary>
    /// Check if this unit is in engagement range of another unit
    /// </summary>
    public bool IsInEngagementRange(Unit otherUnit)
    {
        var distance = Position.DistanceTo(otherUnit.Position);
        var myRange = GetEngagementRangeInInches();
        var theirRange = otherUnit.GetEngagementRangeInInches();
        var totalRange = myRange + theirRange;
        return distance <= totalRange;
    }

    /// <summary>
    /// Get the unit's charge range in inches (movement + 1D6")
    /// </summary>
    public int GetChargeRangeInInches()
    {
        return Move + 6; // Maximum charge range (Move + 6")
    }

    /// <summary>
    /// Get the unit's run range in inches (movement + 1D6")
    /// </summary>
    public int GetRunRangeInInches()
    {
        return Move + 6; // Maximum run range (Move + 6")
    }

    /// <summary>
    /// Get the unit's shooting range in inches (if applicable)
    /// </summary>
    public float GetShootingRangeInInches()
    {
        // This would be weapon-specific, but for now return a default
        return 18.0f; // 18" is a common shooting range
    }

    /// <summary>
    /// Check if a target is within shooting range
    /// </summary>
    public bool IsTargetInShootingRange(Unit target)
    {
        var distance = Position.DistanceTo(target.Position);
        return distance <= GetShootingRangeInInches();
    }

    /// <summary>
    /// Get the unit's aura range in inches (if applicable)
    /// </summary>
    public float GetAuraRangeInInches()
    {
        // Check for aura abilities
        foreach (var ability in UnitAbilities)
        {
            if (ability.Range > 0)
            {
                return ability.Range;
            }
        }
        return 0.0f; // No aura abilities
    }

    /// <summary>
    /// Check if a target is within aura range
    /// </summary>
    public bool IsTargetInAuraRange(Unit target)
    {
        var auraRange = GetAuraRangeInInches();
        if (auraRange <= 0) return false;
        
        var distance = Position.DistanceTo(target.Position);
        return distance <= auraRange;
    }

    /// <summary>
    /// Get a formatted string showing all the unit's key measurements
    /// </summary>
    public string GetMeasurementsString()
    {
        var measurements = $"Move: {GetMovementString()}\n";
        measurements += $"Base: {GetBaseSizeString()}\n";
        measurements += $"Engagement: {GetEngagementRangeInInches():F1}\"\n";
        measurements += $"Charge: {GetChargeRangeInInches()}\"\n";
        
        var shootingRange = GetShootingRangeInInches();
        if (shootingRange > 0)
        {
            measurements += $"Shooting: {shootingRange:F1}\"\n";
        }
        
        var auraRange = GetAuraRangeInInches();
        if (auraRange > 0)
        {
            measurements += $"Aura: {auraRange:F1}\"\n";
        }
        
        return measurements;
    }

    /// <summary>
    /// Convert base size to game units (assuming 1 unit = 1 inch)
    /// </summary>
    private float GetBaseSizeInUnits()
    {
        return GetBaseSizeInInches();
    }

    public void Select()
    {
        IsSelected = true;
        _selectionIndicator.GetChild<MeshInstance3D>(0).Visible = true;
        EmitSignal(SignalName.UnitSelected, this);
    }

    public void Deselect()
    {
        IsSelected = false;
        _selectionIndicator.GetChild<MeshInstance3D>(0).Visible = false;
    }

    public void SetPosition(Vector3 newPosition)
    {
        Vector3 oldPosition = Position;
        Position = newPosition;
        GlobalPosition = newPosition;
        EmitSignal(SignalName.UnitMoved, this, oldPosition, newPosition);
    }

    public bool CanMove()
    {
        return !HasMoved && GameManager.Instance.CurrentTurnPhase == GameManager.TurnPhase.Movement;
    }

    public bool CanShoot()
    {
        return !HasShot && GameManager.Instance.CurrentTurnPhase == GameManager.TurnPhase.Shooting && !IsEngaged;
    }

    public bool CanCharge()
    {
        return !HasCharged && GameManager.Instance.CurrentTurnPhase == GameManager.TurnPhase.Charge;
    }

    public bool CanFight()
    {
        return !HasFought && GameManager.Instance.CurrentTurnPhase == GameManager.TurnPhase.Combat;
    }

    // Temporary Effects System
    public void AddTemporaryEffect(string name, string stat, int value, GameManager.TurnPhase expiresAtPhase)
    {
        var effect = new TemporaryEffect
        {
            Name = name,
            Stat = stat,
            Value = value,
            ExpiresAtPhase = expiresAtPhase,
            AppliedAtPhase = GameManager.Instance.CurrentTurnPhase
        };
        
        TemporaryEffects.Add(effect);
        GD.Print($"Unit {UnitName}: Added temporary effect '{name}' (stat: {stat}, value: {value}, expires: {expiresAtPhase})");
    }

    public void RemoveTemporaryEffect(string name)
    {
        TemporaryEffects.RemoveAll(e => e.Name == name);
        GD.Print($"Unit {UnitName}: Removed temporary effect '{name}'");
    }

    public void ClearExpiredEffects()
    {
        var currentPhase = GameManager.Instance.CurrentTurnPhase;
        var expiredEffects = TemporaryEffects.Where(e => e.ExpiresAtPhase == currentPhase).ToList();
        
        foreach (var effect in expiredEffects)
        {
            GD.Print($"Unit {UnitName}: Temporary effect '{effect.Name}' expired");
        }
        
        TemporaryEffects.RemoveAll(e => e.ExpiresAtPhase == currentPhase);
    }

    public int GetTemporaryEffectValue(string effectType)
    {
        return TemporaryEffects
            .Where(e => e.Stat.Contains(effectType))
            .Sum(e => e.Value);
    }

    public bool HasTemporaryEffect(string name)
    {
        return TemporaryEffects.Any(e => e.Name == name);
    }

    public List<TemporaryEffect> GetActiveEffects()
    {
        return new List<TemporaryEffect>(TemporaryEffects);
    }

    // Enhanced combat methods that consider abilities
    public int GetModifiedToHit()
    {
        int modifier = GetTemporaryEffectValue("ToHit");
        
        // Check for unit abilities that modify hit rolls
        foreach (var ability in UnitAbilities.Where(a => a.Type == AbilityType.Passive))
        {
            foreach (var effect in ability.Effects.Where(e => e.Stat.ToLower() == "tohit" && e.IsPermanent))
            {
                modifier += effect.Value;
            }
        }
        
        return Math.Max(2, Math.Min(6, ToHit - modifier)); // Ensure result is between 2-6
    }
    
    public int GetModifiedSave()
    {
        int modifier = GetTemporaryEffectValue("Save");
        
        // Check for unit abilities that modify save rolls
        foreach (var ability in UnitAbilities.Where(a => a.Type == AbilityType.Passive))
        {
            foreach (var effect in ability.Effects.Where(e => e.Stat.ToLower() == "save" && e.IsPermanent))
            {
                modifier += effect.Value;
            }
        }
        
        return Math.Max(2, Math.Min(6, Save - modifier)); // Ensure result is between 2-6
    }
    
    // Ward save handling
    public bool RollWardSave(int damage)
    {
        if (WardSave <= 0) return false;
        
        int roll = DiceManager.RollD6();
        if (roll >= WardSave)
        {
            GD.Print($"Unit {UnitName}: Ward save successful ({roll} >= {WardSave})");
            return true;
        }
        
        GD.Print($"Unit {UnitName}: Ward save failed ({roll} < {WardSave})");
        return false;
    }
    
    // Enhanced damage handling
    public void TakeDamage(int damage)
    {
        // Check for ward saves first
        if (WardSave > 0)
        {
            int wardRolls = damage;
            int savedDamage = 0;
            
            for (int i = 0; i < wardRolls; i++)
            {
                if (RollWardSave(1))
                {
                    savedDamage++;
                }
            }
            
            damage -= savedDamage;
            GD.Print($"Unit {UnitName}: Ward save prevented {savedDamage} damage");
        }
        
        // Apply remaining damage
        if (damage > 0)
        {
            Wounds = Math.Max(0, Wounds - damage);
            UpdateWoundDisplay();
            EmitSignal(SignalName.UnitDamaged, this, damage);
            
            if (Wounds <= 0)
            {
                Destroy();
            }
        }
    }

    public void Heal(int amount)
    {
        Wounds = Math.Min(MaxWounds, Wounds + amount);
        UpdateWoundDisplay();
    }

    private void UpdateWoundDisplay()
    {
        if (_woundLabel != null)
        {
            _woundLabel.Text = $"{Wounds}/{MaxWounds}";
        }
    }

    private void Destroy()
    {
        EmitSignal(SignalName.UnitDestroyed, this);
        GameManager.Instance.RemoveUnit(this);
        QueueFree();
    }

    public void ResetTurnActions()
    {
        HasMoved = false;
        HasShot = false;
        HasCharged = false;
        HasFought = false;
        
        // Reset unit abilities for the new turn
        foreach (var ability in UnitAbilities)
        {
            ability.ResetTurn();
        }
    }
    
    // Unit Ability Management
    public void AddUnitAbility(UnitAbility ability)
    {
        if (!UnitAbilities.Contains(ability))
        {
            UnitAbilities.Add(ability);
            GD.Print($"Unit {UnitName}: Added unit ability '{ability.Name}'");
        }
    }
    
    public void RemoveUnitAbility(string abilityName)
    {
        var ability = UnitAbilities.FirstOrDefault(a => a.Name == abilityName);
        if (ability != null)
        {
            UnitAbilities.Remove(ability);
            GD.Print($"Unit {UnitName}: Removed unit ability '{abilityName}'");
        }
    }
    
    public bool HasUnitAbility(string abilityName)
    {
        return UnitAbilities.Any(a => a.Name == abilityName);
    }
    
    public UnitAbility GetUnitAbility(string abilityName)
    {
        return UnitAbilities.FirstOrDefault(a => a.Name == abilityName);
    }
    
    public List<UnitAbility> GetAvailableAbilities()
    {
        return UnitAbilities.Where(a => a.CanActivate(this)).ToList();
    }
    
    public bool ActivateUnitAbility(string abilityName, Unit target = null)
    {
        var ability = GetUnitAbility(abilityName);
        if (ability != null && ability.CanActivate(this, target))
        {
            ability.Activate(this, target);
            return true;
        }
        return false;
    }
    
    // Model Ability Management
    public void AddModelAbility(ModelAbility ability)
    {
        if (!ModelAbilities.Contains(ability))
        {
            ModelAbilities.Add(ability);
            GD.Print($"Unit {UnitName}: Added model ability '{ability.Name}'");
        }
    }
    
    public void RemoveModelAbility(string abilityName)
    {
        var ability = ModelAbilities.FirstOrDefault(a => a.Name == abilityName);
        if (ability != null)
        {
            ModelAbilities.Remove(ability);
            GD.Print($"Unit {UnitName}: Removed model ability '{abilityName}'");
        }
    }
    
    public bool HasModelAbility(string abilityName)
    {
        return ModelAbilities.Any(a => a.Name == abilityName);
    }
    
    public ModelAbility GetModelAbility(string abilityName)
    {
        return ModelAbilities.FirstOrDefault(a => a.Name == abilityName);
    }
    
    public List<ModelAbility> GetAvailableModelAbilities()
    {
        return ModelAbilities.Where(a => a.CanActivate(this)).ToList();
    }
    
    public bool ActivateModelAbility(string abilityName, Unit target = null)
    {
        var ability = GetModelAbility(abilityName);
        if (ability != null && ability.CanActivate(this, target))
        {
            ability.Activate(this, target);
            return true;
        }
        return false;
    }

    // Combat Methods
    public CombatResult PerformAttack(Unit target, Weapon weapon = null)
    {
        if (weapon == null && Weapons.Count > 0)
        {
            weapon = Weapons[0]; // Use first weapon
        }

        var result = new CombatResult();

        // Roll to hit
        int hitRolls = weapon?.Attacks ?? Attacks;
        for (int i = 0; i < hitRolls; i++)
        {
            int roll = DiceManager.RollD6();
            if (roll >= (weapon?.ToHit ?? GetModifiedToHit()))
            {
                result.Hits++;
            }
        }

        // Roll to wound
        for (int i = 0; i < result.Hits; i++)
        {
            int roll = DiceManager.RollD6();
            if (roll >= (weapon?.ToWound ?? ToWound))
            {
                result.Wounds++;
            }
        }

        // Target rolls saves
        for (int i = 0; i < result.Wounds; i++)
        {
            int saveRoll = DiceManager.RollD6();
            int modifiedSave = target.GetModifiedSave() - (weapon?.Rend ?? Rend);
            if (saveRoll >= modifiedSave)
            {
                result.SavedWounds++;
            }
        }

        result.UnsavedWounds = result.Wounds - result.SavedWounds;
        result.Damage = result.UnsavedWounds * (weapon?.Damage ?? Damage);

        return result;
    }

    public void ApplyCombatResult(CombatResult result)
    {
        TakeDamage(result.Damage);
    }

    // Movement Methods
    public bool MoveTo(Vector3 targetPosition)
    {
        if (!CanMove()) return false;

        float distance = Position.DistanceTo(targetPosition);
        float maxMove = Move; // Movement is already in inches

        if (distance <= maxMove && GameManager.Instance.IsValidBoardPosition(targetPosition))
        {
            SetPosition(targetPosition);
            HasMoved = true;
            return true;
        }

        return false;
    }

    // Charge Methods
    public bool Charge(Unit target)
    {
        if (!CanCharge()) return false;

        float distance = Position.DistanceTo(target.Position);
        float chargeDistance = GetChargeRangeInInches();

        if (distance <= chargeDistance)
        {
            // Move to engagement range
            Vector3 chargePosition = CalculateChargePosition(target);
            SetPosition(chargePosition);
            HasCharged = true;
            IsEngaged = true;
            target.IsEngaged = true;
            return true;
        }

        return false;
    }

    private Vector3 CalculateChargePosition(Unit target)
    {
        Vector3 direction = (target.Position - Position).Normalized();
        float engagementDistance = GetEngagementRangeInInches();
        return target.Position - (direction * engagementDistance);
    }

    // AoS 4th Edition - Rally Phase (replaces Battleshock)
    public bool CanRally()
    {
        return Wounds < MaxWounds && !IsEngaged;
    }

    public int Rally()
    {
        if (!CanRally()) return 0;

        int roll = DiceManager.RollD6();
        if (roll >= Bravery)
        {
            Heal(1);
            return 1;
        }
        return 0;
    }

    // AoS 4th Edition - Command Abilities
    public bool UseCommandAbility(CommandAbility ability, Unit target = null)
    {
        if (ability.IsUsed) return false;

        // Check if unit is within range
        if (target != null)
        {
            float distance = Position.DistanceTo(target.Position);
            if (distance > ability.Range) return false;
        }

        ability.IsUsed = true;
        return true;
    }

    // AoS 4th Edition - Spell Casting
    public SpellCastResult CastSpell(Spell spell, Unit target = null)
    {
        if (!IsWizard) return new SpellCastResult { Success = false };

        float distance = 0;
        if (target != null)
        {
            distance = Position.DistanceTo(target.Position);
            if (distance > spell.Range) return new SpellCastResult { Success = false };
        }

        return DiceManager.RollSpellCast(spell.CastingValue);
    }

    // AoS 4th Edition - Prayer
    public bool Pray(Spell prayer)
    {
        if (!IsPriest) return false;

        return DiceManager.RollPrayer(prayer.CastingValue);
    }

    // Check if unit is in combat
    public bool IsInCombat()
    {
        return IsEngaged;
    }
}

public class CombatResult
{
    public int Hits { get; set; } = 0;
    public int Wounds { get; set; } = 0;
    public int SavedWounds { get; set; } = 0;
    public int UnsavedWounds { get; set; } = 0;
    public int Damage { get; set; } = 0;
}

public class Weapon
{
    public string Name { get; set; } = "";
    public int Attacks { get; set; } = 1;
    public int ToHit { get; set; } = 4;
    public int ToWound { get; set; } = 4;
    public int Rend { get; set; } = 0;
    public int Damage { get; set; } = 1;
    public float Range { get; set; } = 0; // 0 for melee, >0 for ranged
}

public class Armour
{
    public string Name { get; set; } = "";
    public int Save { get; set; } = 6;
    public int Wounds { get; set; } = 0;
}

public class Artefact
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<Ability> Abilities { get; set; } = new List<Ability>();
}

public class Ability
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int BraveryModifier { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class Spell
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int CastingValue { get; set; } = 7;
    public float Range { get; set; } = 18.0f; // in inches
}

public class CommandAbility
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public float Range { get; set; } = 12.0f; // in inches
    public bool IsUsed { get; set; } = false;
}

public class TemporaryEffect
{
    public string Name { get; set; } = "";
    public string Stat { get; set; } = "";
    public int Value { get; set; } = 0;
    public GameManager.TurnPhase ExpiresAtPhase { get; set; }
    public GameManager.TurnPhase AppliedAtPhase { get; set; }
}

public class SpellCastResult
{
    public bool Success { get; set; } = false;
    public int CastingRoll { get; set; } = 0;
    public string Message { get; set; } = "";
}
