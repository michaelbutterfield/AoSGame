using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class AIOpponent : Node
{
    [Export] public int PlayerId = 2;
    [Export] public float DecisionDelay = 1.0f;
    [Export] public bool IsEnabled = true;
    
    private Timer _decisionTimer;
    private List<Unit> _aiUnits = new List<Unit>();
    private List<Unit> _enemyUnits = new List<Unit>();
    private Vector3 _aiDeploymentZone;
    private Vector3 _enemyDeploymentZone;
    
    public override void _Ready()
    {
        _decisionTimer = new Timer();
        _decisionTimer.WaitTime = DecisionDelay;
        _decisionTimer.OneShot = true;
        _decisionTimer.Timeout += OnDecisionTimerTimeout;
        AddChild(_decisionTimer);
        
        SetupDeploymentZones();
    }
    
    private void SetupDeploymentZones()
    {
        var boardCenter = GameManager.Instance.GetBoardCenter();
        var boardHeight = GameManager.Instance.ConvertInchesToUnits(GameManager.Instance.BoardHeightInches);
        
        // AI deploys in the top half, enemy in bottom half
        _aiDeploymentZone = boardCenter + new Vector3(0, 0, boardHeight / 4);
        _enemyDeploymentZone = boardCenter - new Vector3(0, 0, boardHeight / 4);
    }
    
    public void SetAIUnits(List<Unit> units)
    {
        _aiUnits = units;
        GD.Print($"AI: Set {units.Count} units");
    }
    
    public void SetEnemyUnits(List<Unit> units)
    {
        _enemyUnits = units;
        GD.Print($"AI: Set {units.Count} enemy units");
    }
    
    public void OnTurnStarted()
    {
        if (!IsEnabled || GameManager.Instance.CurrentPlayerTurn != PlayerId)
            return;
            
        GD.Print("AI: Turn started, making decisions...");
        _decisionTimer.Start();
    }
    
    public void OnPhaseChanged(TurnPhase phase)
    {
        if (!IsEnabled || GameManager.Instance.CurrentPlayerTurn != PlayerId)
            return;
            
        GD.Print($"AI: Phase changed to {phase}");
        
        switch (phase)
        {
            case TurnPhase.Hero:
                HandleHeroPhase();
                break;
            case TurnPhase.Movement:
                HandleMovementPhase();
                break;
            case TurnPhase.Shooting:
                HandleShootingPhase();
                break;
            case TurnPhase.Charge:
                HandleChargePhase();
                break;
            case TurnPhase.Combat:
                HandleCombatPhase();
                break;
        }
    }
    
    private void OnDecisionTimerTimeout()
    {
        // Auto-advance to next phase after decision delay
        GameManager.Instance.NextTurnPhase();
    }
    
    private void HandleHeroPhase()
    {
        GD.Print("AI: Handling Hero Phase");
        
        // Use command abilities on units that need them
        foreach (var unit in _aiUnits)
        {
            if (unit.IsHero && unit.CanUseCommandAbility())
            {
                // Find a nearby unit to buff
                var targetUnit = FindNearestFriendlyUnit(unit.Position, 12.0f);
                if (targetUnit != null)
                {
                    unit.UseCommandAbility();
                    GD.Print($"AI: Used command ability on {targetUnit.UnitName}");
                }
            }
        }
        
        // Cast spells if available
        foreach (var unit in _aiUnits)
        {
            if (unit.IsWizard && unit.CanCastSpell())
            {
                var target = FindBestSpellTarget(unit);
                if (target != null)
                {
                    unit.CastSpell();
                    GD.Print($"AI: Cast spell targeting {target.UnitName}");
                }
            }
        }
        
        // Pray if available
        foreach (var unit in _aiUnits)
        {
            if (unit.IsPriest && unit.CanPray())
            {
                unit.Pray();
                GD.Print($"AI: Unit {unit.UnitName} prayed");
            }
        }
    }
    
    private void HandleMovementPhase()
    {
        GD.Print("AI: Handling Movement Phase");
        
        foreach (var unit in _aiUnits)
        {
            if (unit.CanMove())
            {
                var targetPosition = CalculateOptimalPosition(unit);
                if (targetPosition != unit.Position)
                {
                    unit.MoveTo(targetPosition);
                    GD.Print($"AI: Moved {unit.UnitName} to {targetPosition}");
                }
            }
        }
    }
    
    private void HandleShootingPhase()
    {
        GD.Print("AI: Handling Shooting Phase");
        
        foreach (var unit in _aiUnits)
        {
            if (unit.CanShoot())
            {
                var target = FindBestShootingTarget(unit);
                if (target != null)
                {
                    // Perform shooting attack
                    unit.HasShot = true;
                    GD.Print($"AI: {unit.UnitName} shot at {target.UnitName}");
                }
            }
        }
    }
    
    private void HandleChargePhase()
    {
        GD.Print("AI: Handling Charge Phase");
        
        foreach (var unit in _aiUnits)
        {
            if (unit.CanCharge())
            {
                var target = FindBestChargeTarget(unit);
                if (target != null)
                {
                    var chargePosition = unit.CalculateChargePosition(target.Position);
                    unit.Charge(chargePosition);
                    GD.Print($"AI: {unit.UnitName} charged {target.UnitName}");
                }
            }
        }
    }
    
    private void HandleCombatPhase()
    {
        GD.Print("AI: Handling Combat Phase");
        
        foreach (var unit in _aiUnits)
        {
            if (unit.CanFight())
            {
                var target = FindBestCombatTarget(unit);
                if (target != null)
                {
                    // Perform combat attack
                    unit.HasFought = true;
                    GD.Print($"AI: {unit.UnitName} fought {target.UnitName}");
                }
            }
        }
    }
    
    private Vector3 CalculateOptimalPosition(Unit unit)
    {
        // Simple AI: move towards nearest enemy
        var nearestEnemy = FindNearestEnemyUnit(unit.Position);
        if (nearestEnemy == null)
            return unit.Position;
        
        var direction = (nearestEnemy.Position - unit.Position).Normalized();
        var targetPosition = unit.Position + direction * unit.Move;
        
        // Ensure position is within board bounds
        if (!GameManager.Instance.IsValidBoardPosition(targetPosition))
        {
            targetPosition = GameManager.Instance.GetBoardCenter();
        }
        
        return targetPosition;
    }
    
    private Unit FindNearestEnemyUnit(Vector3 position)
    {
        Unit nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach (var enemy in _enemyUnits)
        {
            var distance = position.DistanceTo(enemy.Position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy;
            }
        }
        
        return nearest;
    }
    
    private Unit FindNearestFriendlyUnit(Vector3 position, float maxRange)
    {
        Unit nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach (var friendly in _aiUnits)
        {
            if (friendly == null) continue;
            
            var distance = position.DistanceTo(friendly.Position);
            if (distance <= maxRange && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = friendly;
            }
        }
        
        return nearest;
    }
    
    private Unit FindBestShootingTarget(Unit shooter)
    {
        // Prioritize low health enemies within range
        Unit bestTarget = null;
        float bestScore = float.MinValue;
        
        foreach (var enemy in _enemyUnits)
        {
            var distance = shooter.Position.DistanceTo(enemy.Position);
            if (distance <= 18.0f) // Assume 18" shooting range
            {
                var score = CalculateTargetPriority(enemy, distance);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = enemy;
                }
            }
        }
        
        return bestTarget;
    }
    
    private Unit FindBestChargeTarget(Unit charger)
    {
        // Prioritize nearby enemies that can be charged
        Unit bestTarget = null;
        float bestScore = float.MinValue;
        
        foreach (var enemy in _enemyUnits)
        {
            var distance = charger.Position.DistanceTo(enemy.Position);
            if (distance <= 12.0f) // Charge range
            {
                var score = CalculateTargetPriority(enemy, distance);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = enemy;
                }
            }
        }
        
        return bestTarget;
    }
    
    private Unit FindBestCombatTarget(Unit fighter)
    {
        // Find engaged enemies
        foreach (var enemy in _enemyUnits)
        {
            var distance = fighter.Position.DistanceTo(enemy.Position);
            if (distance <= 1.0f) // Combat range
            {
                return enemy;
            }
        }
        
        return null;
    }
    
    private Unit FindBestSpellTarget(Unit caster)
    {
        // Target the most dangerous enemy
        Unit bestTarget = null;
        float bestScore = float.MinValue;
        
        foreach (var enemy in _enemyUnits)
        {
            var distance = caster.Position.DistanceTo(enemy.Position);
            if (distance <= 18.0f) // Spell range
            {
                var score = CalculateTargetPriority(enemy, distance);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = enemy;
                }
            }
        }
        
        return bestTarget;
    }
    
    private float CalculateTargetPriority(Unit target, float distance)
    {
        // Higher priority for:
        // - Low health units (easier to kill)
        // - Hero units (more valuable)
        // - Closer units (easier to reach)
        
        var healthFactor = 1.0f - (float)target.Wounds / target.MaxWounds;
        var heroBonus = target.IsHero ? 2.0f : 1.0f;
        var distanceFactor = 1.0f / (distance + 1.0f);
        
        return healthFactor * heroBonus * distanceFactor;
    }
    
    public void DeployUnits(List<Unit> units)
    {
        GD.Print($"AI: Deploying {units.Count} units");
        
        var deploymentSpacing = 2.0f;
        var currentX = -10.0f;
        var currentZ = _aiDeploymentZone.Z;
        
        foreach (var unit in units)
        {
            var position = new Vector3(currentX, 0, currentZ);
            unit.SetPosition(position);
            currentX += deploymentSpacing;
            
            if (currentX > 10.0f)
            {
                currentX = -10.0f;
                currentZ += deploymentSpacing;
            }
        }
    }
}
