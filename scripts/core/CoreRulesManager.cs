using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CoreRulesManager : Node
{
    public static CoreRulesManager Instance { get; private set; }

    // Core abilities from AoS 4th Edition
    public enum CoreAbilityType
    {
        Rally,
        CoveringFire,
        DeployUnit,
        DeployFactionTerrain,
        DeployRegiment,
        TacticalGambit,
        ActivatePlaceOfPower,
        BanishManifestation,
        Fly
    }

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }

    /// <summary>
    /// RALLY: Make 6 rally rolls of D6. For each 4+, receive 1 rally point.
    /// Rally points can be spent to heal or return slain models.
    /// </summary>
    public RallyResult Rally(Unit unit)
    {
        if (unit.IsInCombat())
        {
            GD.PrintErr($"Unit {unit.UnitName} cannot rally while in combat");
            return new RallyResult { Success = false, RallyPoints = 0 };
        }

        var rallyPoints = 0;
        var rolls = new List<int>();

        // Make 6 rally rolls
        for (int i = 0; i < 6; i++)
        {
            var roll = DiceManager.RollD6();
            rolls.Add(roll);
            if (roll >= 4)
            {
                rallyPoints++;
            }
        }

        GD.Print($"Unit {unit.UnitName} rallied: {rallyPoints} points from rolls {string.Join(", ", rolls)}");
        
        return new RallyResult 
        { 
            Success = true, 
            RallyPoints = rallyPoints,
            Rolls = rolls
        };
    }

    /// <summary>
    /// COVERING FIRE: Unit can shoot at closest enemy unit before it charges.
    /// Must subtract 1 from hit rolls.
    /// </summary>
    public bool CoveringFire(Unit unit, Unit target)
    {
        if (unit.HasRunThisTurn || unit.IsInCombat())
        {
            GD.PrintErr($"Unit {unit.UnitName} cannot use Covering Fire (has run or is in combat)");
            return false;
        }

        if (target == null)
        {
            GD.PrintErr("No valid target for Covering Fire");
            return false;
        }

        // Apply -1 to hit penalty
        var originalToHit = unit.GetModifiedToHit();
        unit.AddTemporaryEffect("Covering Fire Penalty", "ToHit", 1, GameManager.Instance.CurrentTurnPhase);

        // Resolve shooting attacks
        var shootingResult = ResolveShooting(unit, target);
        
        // Remove temporary effect
        unit.RemoveTemporaryEffect("Covering Fire Penalty");

        GD.Print($"Unit {unit.UnitName} used Covering Fire against {target.UnitName}");
        return shootingResult;
    }

    /// <summary>
    /// DEPLOY UNIT: Set up unit wholly within friendly territory and more than 9" from enemy territory
    /// </summary>
    public bool DeployUnit(Unit unit, Vector3 position, Vector3 friendlyTerritory, Vector3 enemyTerritory)
    {
        // Check if position is within friendly territory
        if (!IsPositionInTerritory(position, friendlyTerritory))
        {
            GD.PrintErr($"Position is not within friendly territory");
            return false;
        }

        // Check if position is more than 9" from enemy territory
        var distanceToEnemy = position.DistanceTo(enemyTerritory);
        if (distanceToEnemy <= 9.0f)
        {
            GD.PrintErr($"Position too close to enemy territory: {distanceToEnemy:F1}\"");
            return false;
        }

        unit.Position = position;
        unit.IsDeployed = true;
        
        GD.Print($"Unit {unit.UnitName} deployed at {position}");
        return true;
    }

    /// <summary>
    /// DEPLOY FACTION TERRAIN: Set up terrain wholly within friendly territory, more than 3" from objectives and other terrain
    /// </summary>
    public bool DeployFactionTerrain(Terrain terrain, Vector3 position, List<Vector3> objectives, List<Terrain> otherTerrain)
    {
        // Check distance from objectives
        foreach (var objective in objectives)
        {
            var distance = position.DistanceTo(objective);
            if (distance <= 3.0f)
            {
                GD.PrintErr($"Position too close to objective: {distance:F1}\"");
                return false;
            }
        }

        // Check distance from other terrain
        foreach (var other in otherTerrain)
        {
            var distance = position.DistanceTo(other.Position);
            if (distance <= 3.0f)
            {
                GD.PrintErr($"Position too close to other terrain: {distance:F1}\"");
                return false;
            }
        }

        terrain.Position = position;
        terrain.IsDeployed = true;
        
        GD.Print($"Faction terrain {terrain.Name} deployed at {position}");
        return true;
    }

    /// <summary>
    /// DEPLOY REGIMENT: Deploy all units in a regiment together
    /// </summary>
    public bool DeployRegiment(List<Unit> regimentUnits, List<Vector3> positions, Vector3 friendlyTerritory, Vector3 enemyTerritory)
    {
        if (regimentUnits.Count != positions.Count)
        {
            GD.PrintErr("Number of units and positions must match");
            return false;
        }

        // Check all positions are valid
        for (int i = 0; i < regimentUnits.Count; i++)
        {
            if (!IsPositionInTerritory(positions[i], friendlyTerritory))
            {
                GD.PrintErr($"Position {i} is not within friendly territory");
                return false;
            }

            var distanceToEnemy = positions[i].DistanceTo(enemyTerritory);
            if (distanceToEnemy <= 9.0f)
            {
                GD.PrintErr($"Position {i} too close to enemy territory: {distanceToEnemy:F1}\"");
                return false;
            }
        }

        // Deploy all units
        for (int i = 0; i < regimentUnits.Count; i++)
        {
            regimentUnits[i].Position = positions[i];
            regimentUnits[i].IsDeployed = true;
        }

        GD.Print($"Regiment deployed with {regimentUnits.Count} units");
        return true;
    }

    /// <summary>
    /// TACTICAL GAMBIT: Pick 1 battle tactic that hasn't been attempted yet
    /// </summary>
    public bool TacticalGambit(Player player, string battleTactic)
    {
        if (player.HasAttemptedTactic(battleTactic))
        {
            GD.PrintErr($"Battle tactic {battleTactic} has already been attempted");
            return false;
        }

        if (player.WentSecondLastTurn && player.GoingFirstThisTurn)
        {
            GD.PrintErr("Cannot use Tactical Gambit when going first after going second");
            return false;
        }

        player.AddAvailableTactic(battleTactic);
        GD.Print($"Player {player.PlayerId} used Tactical Gambit for {battleTactic}");
        return true;
    }

    /// <summary>
    /// ACTIVATE PLACE OF POWER: Hero draws power from nearby mythical landmark
    /// </summary>
    public bool ActivatePlaceOfPower(Unit hero, Terrain placeOfPower, string effect)
    {
        if (!hero.IsHero)
        {
            GD.PrintErr($"Unit {hero.UnitName} is not a hero");
            return false;
        }

        var distance = hero.Position.DistanceTo(placeOfPower.Position);
        if (distance > 3.0f)
        {
            GD.PrintErr($"Hero too far from Place of Power: {distance:F1}\"");
            return false;
        }

        switch (effect.ToLower())
        {
            case "cauterising_pollen":
                return ApplyCauterisingPollen(hero, placeOfPower);
            case "rapid_sprouting":
                return ApplyRapidSprouting(hero, placeOfPower);
            case "tap_the_ley_lines":
                return ApplyTapTheLeyLines(hero, placeOfPower);
            default:
                GD.PrintErr($"Unknown Place of Power effect: {effect}");
                return false;
        }
    }

    /// <summary>
    /// BANISH MANIFESTATION: Wizard or priest disrupts arcane forces
    /// </summary>
    public bool BanishManifestation(Unit caster, Terrain manifestation, int banishmentValue, List<Terrain> otherManifestations)
    {
        if (!caster.IsWizard && !caster.IsPriest)
        {
            GD.PrintErr($"Unit {caster.UnitName} is not a wizard or priest");
            return false;
        }

        var distance = caster.Position.DistanceTo(manifestation.Position);
        if (distance > 30.0f)
        {
            GD.PrintErr($"Manifestation too far: {distance:F1}\"");
            return false;
        }

        // Calculate banishment roll
        var banishmentRoll = DiceManager.RollDice(6, 2);
        var additionalManifestations = otherManifestations.Count - 1; // -1 because we're not counting the target
        banishmentRoll += additionalManifestations;

        if (banishmentRoll >= banishmentValue)
        {
            // Banish the manifestation
            manifestation.Destroy();
            GD.Print($"Manifestation {manifestation.Name} banished (roll: {banishmentRoll} >= {banishmentValue})");
            return true;
        }
        else
        {
            GD.Print($"Failed to banish manifestation (roll: {banishmentRoll} < {banishmentValue})");
            return false;
        }
    }

    /// <summary>
    /// FLY: Unit ignores other models, terrain, and combat ranges during movement
    /// </summary>
    public bool CanFly(Unit unit)
    {
        return unit.CanFly;
    }

    // Helper methods
    private bool IsPositionInTerritory(Vector3 position, Vector3 territory)
    {
        // Simplified territory check - in real implementation this would check against actual territory boundaries
        return true; // Placeholder
    }

    private bool ResolveShooting(Unit attacker, Unit target)
    {
        // Simplified shooting resolution - in real implementation this would use the full shooting system
        GD.Print($"Resolving shooting from {attacker.UnitName} to {target.UnitName}");
        return true;
    }

    private bool ApplyCauterisingPollen(Unit hero, Terrain placeOfPower)
    {
        var roll = DiceManager.RollD6();
        if (roll == 1)
        {
            // Inflict 1 mortal damage on all units within 6"
            var unitsInRange = GetUnitsInRange(placeOfPower.Position, 6.0f);
            foreach (var unit in unitsInRange)
            {
                unit.TakeDamage(1);
            }
            GD.Print("Cauterising Pollen: 1 mortal damage to all units within 6\"");
        }
        else if (roll >= 3)
        {
            // Heal 2 wounds to all units wholly within 6"
            var unitsInRange = GetUnitsInRange(placeOfPower.Position, 6.0f);
            foreach (var unit in unitsInRange)
            {
                unit.Heal(2);
            }
            GD.Print("Cauterising Pollen: Healed 2 wounds to all units within 6\"");
        }
        return true;
    }

    private bool ApplyRapidSprouting(Unit hero, Terrain placeOfPower)
    {
        // Pick a Ghyranite objective or terrain feature within 12"
        var target = GetGhyraniteTargetInRange(hero.Position, 12.0f);
        if (target != null)
        {
            var roll = DiceManager.RollD6();
            if (roll >= 3)
            {
                target.AddAbility("Obscuring");
                GD.Print("Rapid Sprouting: Target gained Obscuring ability");
                return true;
            }
        }
        return false;
    }

    private bool ApplyTapTheLeyLines(Unit hero, Terrain placeOfPower)
    {
        if (!hero.IsWizard && !hero.IsPriest)
        {
            // Grant WIZARD(1) ability for the rest of the turn
            hero.AddTemporaryEffect("Tap the Ley Lines", "Wizard", 1, GameManager.Instance.CurrentTurnPhase);
            GD.Print("Tap the Ley Lines: Hero gained WIZARD(1) ability");
            return true;
        }
        return false;
    }

    private List<Unit> GetUnitsInRange(Vector3 center, float range)
    {
        // Simplified - in real implementation this would query the actual unit list
        return new List<Unit>();
    }

    private Terrain GetGhyraniteTargetInRange(Vector3 center, float range)
    {
        // Simplified - in real implementation this would query the actual terrain list
        return null;
    }
}

// Data classes for core rules
public class RallyResult
{
    public bool Success { get; set; }
    public int RallyPoints { get; set; }
    public List<int> Rolls { get; set; } = new List<int>();
}

public class DeployResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = "";
    public Vector3 Position { get; set; }
}
