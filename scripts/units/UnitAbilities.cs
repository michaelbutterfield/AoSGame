using Godot;
using System;
using System.Collections.Generic;

namespace AoSGame.Units
{
    /// <summary>
    /// Represents a unit ability that can be activated during specific phases
    /// </summary>
    public class UnitAbility
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string FlavorText { get; set; } = "";
        public AbilityType Type { get; set; } = AbilityType.Passive;
        public List<GameManager.TurnPhase> ActivationPhases { get; set; } = new List<GameManager.TurnPhase>();
        public float Range { get; set; } = 0.0f; // 0 = self, >0 = range in inches
        public TargetType TargetType { get; set; } = TargetType.Self;
        public int Cooldown { get; set; } = 0; // 0 = no cooldown, >0 = turns before reuse
        public bool IsUsedThisTurn { get; set; } = false;
        public bool IsUsedThisGame { get; set; } = false;
        
        // Effect properties
        public List<AbilityEffect> Effects { get; set; } = new List<AbilityEffect>();
        public List<AbilityCondition> Conditions { get; set; } = new List<AbilityCondition>();
        
        // Visual and audio
        public string IconPath { get; set; } = "";
        public string SoundEffect { get; set; } = "";
        public string ParticleEffect { get; set; } = "";
        
        public bool CanActivate(Unit unit, Unit target = null)
        {
            if (IsUsedThisTurn && Cooldown > 0) return false;
            if (!ActivationPhases.Contains(GameManager.Instance.CurrentTurnPhase)) return false;
            
            // Check conditions
            foreach (var condition in Conditions)
            {
                if (!condition.Evaluate(unit, target)) return false;
            }
            
            // Check range
            if (target != null && Range > 0)
            {
                float distance = unit.Position.DistanceTo(target.Position);
                float rangeInUnits = GameManager.Instance.ConvertInchesToUnits(Range);
                if (distance > rangeInUnits) return false;
            }
            
            return true;
        }
        
        public void Activate(Unit unit, Unit target = null)
        {
            if (!CanActivate(unit, target)) return;
            
            // Apply effects
            foreach (var effect in Effects)
            {
                effect.Apply(unit, target);
            }
            
            // Mark as used
            IsUsedThisTurn = true;
            if (Cooldown > 0)
            {
                IsUsedThisGame = true;
            }
            
            GD.Print($"Unit {unit.UnitName} activated ability: {Name}");
        }
        
        public void ResetTurn()
        {
            IsUsedThisTurn = false;
        }
        
        public void ResetGame()
        {
            IsUsedThisTurn = false;
            IsUsedThisGame = false;
        }
    }
    
    /// <summary>
    /// Represents a model ability that affects individual models within a unit
    /// </summary>
    public class ModelAbility
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string FlavorText { get; set; } = "";
        public ModelAbilityType Type { get; set; } = ModelAbilityType.Passive;
        public int ModelCount { get; set; } = 1; // How many models can use this ability
        public List<GameManager.TurnPhase> ActivationPhases { get; set; } = new List<GameManager.TurnPhase>();
        public float Range { get; set; } = 0.0f;
        public TargetType TargetType { get; set; } = TargetType.Self;
        
        // Effect properties
        public List<AbilityEffect> Effects { get; set; } = new List<AbilityEffect>();
        public List<AbilityCondition> Conditions { get; set; } = new List<AbilityCondition>();
        
        // Model-specific properties
        public bool RequiresChampion { get; set; } = false;
        public bool RequiresMusician { get; set; } = false;
        public bool RequiresBannerBearer { get; set; } = false;
        public bool RequiresSpecialWeapon { get; set; } = false;
        
        public bool CanActivate(Unit unit, Unit target = null)
        {
            if (!ActivationPhases.Contains(GameManager.Instance.CurrentTurnPhase)) return false;
            
            // Check if unit has required model types
            if (RequiresChampion && !unit.HasChampion) return false;
            if (RequiresMusician && !unit.HasMusician) return false;
            if (RequiresBannerBearer && !unit.HasBannerBearer) return false;
            if (RequiresSpecialWeapon && !unit.HasSpecialWeapon) return false;
            
            // Check conditions
            foreach (var condition in Conditions)
            {
                if (!condition.Evaluate(unit, target)) return false;
            }
            
            // Check range
            if (target != null && Range > 0)
            {
                float distance = unit.Position.DistanceTo(target.Position);
                float rangeInUnits = GameManager.Instance.ConvertInchesToUnits(Range);
                if (distance > rangeInUnits) return false;
            }
            
            return true;
        }
        
        public void Activate(Unit unit, Unit target = null)
        {
            if (!CanActivate(unit, target)) return;
            
            // Apply effects
            foreach (var effect in Effects)
            {
                effect.Apply(unit, target);
            }
            
            GD.Print($"Unit {unit.UnitName} activated model ability: {Name}");
        }
    }
    
    /// <summary>
    /// Represents an effect that an ability can apply
    /// </summary>
    public class AbilityEffect
    {
        public EffectType Type { get; set; } = EffectType.StatModifier;
        public string Target { get; set; } = "Self"; // Self, Target, Friendly, Enemy, All
        public string Stat { get; set; } = ""; // Move, Wounds, Bravery, Save, Attacks, ToHit, ToWound, Rend, Damage
        public int Value { get; set; } = 0;
        public EffectDuration Duration { get; set; } = EffectDuration.Instant;
        public GameManager.TurnPhase ExpiresAtPhase { get; set; } = GameManager.TurnPhase.Hero;
        public bool IsPermanent { get; set; } = false;
        
        public void Apply(Unit source, Unit target)
        {
            Unit effectTarget = GetEffectTarget(source, target);
            if (effectTarget == null) return;
            
            switch (Type)
            {
                case EffectType.StatModifier:
                    ApplyStatModifier(effectTarget);
                    break;
                case EffectType.Heal:
                    ApplyHeal(effectTarget);
                    break;
                case EffectType.Damage:
                    ApplyDamage(effectTarget);
                    break;
                case EffectType.Movement:
                    ApplyMovement(effectTarget);
                    break;
                case EffectType.Combat:
                    ApplyCombat(effectTarget);
                    break;
                case EffectType.Morale:
                    ApplyMorale(effectTarget);
                    break;
                case EffectType.Special:
                    ApplySpecial(effectTarget);
                    break;
            }
        }
        
        private Unit GetEffectTarget(Unit source, Unit target)
        {
            switch (Target)
            {
                case "Self": return source;
                case "Target": return target;
                case "Friendly": return source.PlayerId == target?.PlayerId ? target : source;
                case "Enemy": return source.PlayerId != target?.PlayerId ? target : null;
                case "All": return source; // TODO: Implement area effects
                default: return source;
            }
        }
        
        private void ApplyStatModifier(Unit unit)
        {
            if (Duration == EffectDuration.Instant)
            {
                // Apply immediate stat change
                ApplyImmediateStatChange(unit);
            }
            else
            {
                // Apply temporary effect
                var effect = new TemporaryEffect
                {
                    Name = $"Ability Effect: {Stat}",
                    Description = $"{Stat} modified by {Value}",
                    Value = Value,
                    ExpiresAtPhase = ExpiresAtPhase,
                    AppliedAtPhase = GameManager.Instance.CurrentTurnPhase
                };
                unit.AddTemporaryEffect(effect);
            }
        }
        
        private void ApplyImmediateStatChange(Unit unit)
        {
            switch (Stat.ToLower())
            {
                case "move":
                    unit.Move = Math.Max(1, unit.Move + Value);
                    break;
                case "wounds":
                    unit.Wounds = Math.Max(1, unit.Wounds + Value);
                    break;
                case "bravery":
                    unit.Bravery = Math.Max(1, unit.Bravery + Value);
                    break;
                case "save":
                    unit.Save = Math.Max(2, Math.Min(6, unit.Save - Value)); // Lower is better
                    break;
                case "attacks":
                    unit.Attacks = Math.Max(1, unit.Attacks + Value);
                    break;
                case "tohit":
                    unit.ToHit = Math.Max(2, Math.Min(6, unit.ToHit - Value)); // Lower is better
                    break;
                case "towound":
                    unit.ToWound = Math.Max(2, Math.Min(6, unit.ToWound - Value)); // Lower is better
                    break;
                case "rend":
                    unit.Rend = Math.Max(0, unit.Rend + Value);
                    break;
                case "damage":
                    unit.Damage = Math.Max(1, unit.Damage + Value);
                    break;
            }
        }
        
        private void ApplyHeal(Unit unit)
        {
            if (Stat.ToLower() == "wounds")
            {
                unit.Heal(Value);
            }
        }
        
        private void ApplyDamage(Unit unit)
        {
            if (Stat.ToLower() == "wounds")
            {
                unit.TakeDamage(Value);
            }
        }
        
        private void ApplyMovement(Unit unit)
        {
            // Special movement effects
            switch (Stat.ToLower())
            {
                case "fly":
                    unit.CanFly = Value > 0;
                    break;
                case "runandcharge":
                    unit.CanRunAndCharge = Value > 0;
                    break;
                case "runandshoot":
                    unit.CanRunAndShoot = Value > 0;
                    break;
            }
        }
        
        private void ApplyCombat(Unit unit)
        {
            // Special combat effects
            switch (Stat.ToLower())
            {
                case "fightfirst":
                    unit.FightsFirst = Value > 0;
                    break;
                case "fightlast":
                    unit.FightsLast = Value > 0;
                    break;
                case "ignorecover":
                    unit.IgnoreCover = Value > 0;
                    break;
            }
        }
        
        private void ApplyMorale(Unit unit)
        {
            // Special morale effects
            switch (Stat.ToLower())
            {
                case "ignorebattleshock":
                    unit.IgnoreBattleshock = Value > 0;
                    break;
                case "braverybonus":
                    unit.Bravery += Value;
                    break;
            }
        }
        
        private void ApplySpecial(Unit unit)
        {
            // Special effects that don't fit other categories
            switch (Stat.ToLower())
            {
                case "ward":
                    unit.WardSave = Value;
                    break;
                case "regeneration":
                    unit.Regeneration = Value > 0;
                    break;
                case "ethereal":
                    unit.IsEthereal = Value > 0;
                    break;
            }
        }
    }
    
    /// <summary>
    /// Represents a condition that must be met for an ability to activate
    /// </summary>
    public class AbilityCondition
    {
        public ConditionType Type { get; set; } = ConditionType.Always;
        public string Stat { get; set; } = "";
        public int Value { get; set; } = 0;
        public ComparisonOperator Operator { get; set; } = ComparisonOperator.Equal;
        public string Target { get; set; } = "Self"; // Self, Target, Friendly, Enemy
        
        public bool Evaluate(Unit source, Unit target)
        {
            Unit conditionTarget = GetConditionTarget(source, target);
            if (conditionTarget == null) return false;
            
            int statValue = GetStatValue(conditionTarget);
            
            switch (Operator)
            {
                case ComparisonOperator.Equal: return statValue == Value;
                case ComparisonOperator.NotEqual: return statValue != Value;
                case ComparisonOperator.GreaterThan: return statValue > Value;
                case ComparisonOperator.GreaterThanOrEqual: return statValue >= Value;
                case ComparisonOperator.LessThan: return statValue < Value;
                case ComparisonOperator.LessThanOrEqual: return statValue <= Value;
                default: return true;
            }
        }
        
        private Unit GetConditionTarget(Unit source, Unit target)
        {
            switch (Target)
            {
                case "Self": return source;
                case "Target": return target;
                case "Friendly": return source.PlayerId == target?.PlayerId ? target : source;
                case "Enemy": return source.PlayerId != target?.PlayerId ? target : null;
                default: return source;
            }
        }
        
        private int GetStatValue(Unit unit)
        {
            switch (Stat.ToLower())
            {
                case "move": return unit.Move;
                case "wounds": return unit.Wounds;
                case "maxwounds": return unit.MaxWounds;
                case "bravery": return unit.Bravery;
                case "save": return unit.Save;
                case "attacks": return unit.Attacks;
                case "tohit": return unit.ToHit;
                case "towound": return unit.ToWound;
                case "rend": return unit.Rend;
                case "damage": return unit.Damage;
                case "modelcount": return unit.ModelCount;
                case "hasmoved": return unit.HasMoved ? 1 : 0;
                case "hasshot": return unit.HasShot ? 1 : 0;
                case "hascharged": return unit.HasCharged ? 1 : 0;
                case "hasfought": return unit.HasFought ? 1 : 0;
                case "isengaged": return unit.IsEngaged ? 1 : 0;
                case "ishero": return unit.IsHero ? 1 : 0;
                case "isgeneral": return unit.IsGeneral ? 1 : 0;
                case "iswizard": return unit.IsWizard ? 1 : 0;
                case "ispriest": return unit.IsPriest ? 1 : 0;
                default: return 0;
            }
        }
    }
    
    // Enums
    public enum AbilityType
    {
        Passive,    // Always active
        Active,     // Must be activated
        Reactive,   // Triggers on specific events
        Command     // Command ability variant
    }
    
    public enum ModelAbilityType
    {
        Passive,    // Always active
        Active,     // Must be activated
        Reactive    // Triggers on specific events
    }
    
    public enum EffectType
    {
        StatModifier,   // Modifies unit stats
        Heal,          // Heals wounds
        Damage,        // Deals damage
        Movement,      // Affects movement
        Combat,        // Affects combat
        Morale,        // Affects bravery/battleshock
        Special        // Special effects
    }
    
    public enum EffectDuration
    {
        Instant,        // Immediate effect
        Turn,          // Lasts until end of turn
        Phase,         // Lasts until end of phase
        Game           // Lasts entire game
    }
    
    public enum TargetType
    {
        Self,           // Affects the unit itself
        Single,         // Affects a single target
        Area,           // Affects area around unit
        AllFriendly,    // Affects all friendly units
        AllEnemy,       // Affects all enemy units
        All             // Affects all units
    }
    
    public enum ComparisonOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }
}
