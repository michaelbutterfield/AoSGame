using Godot;
using System;
using System.Collections.Generic;

public partial class DiceManager : Node
{
    [Signal]
    public delegate void DiceRolledEventHandler(int[] results, string rollType);

    public static DiceManager Instance { get; private set; }

    private Random _random = new Random();

    public override void _Ready()
    {
        Instance = this;
    }

    // Basic dice rolling
    public static int RollD6()
    {
        if (Instance != null)
        {
            return Instance._random.Next(1, 7);
        }
        return new Random().Next(1, 7);
    }

    public static int RollD3()
    {
        return RollD6() / 2 + 1;
    }

    public static int RollDice(int sides)
    {
        if (Instance != null)
        {
            return Instance._random.Next(1, sides + 1);
        }
        return new Random().Next(1, sides + 1);
    }

    public static int[] RollMultipleDice(int count, int sides = 6)
    {
        int[] results = new int[count];
        for (int i = 0; i < count; i++)
        {
            results[i] = RollDice(sides);
        }
        return results;
    }

    // Age of Sigmar specific dice mechanics
    public static int[] RollToHit(int attacks, int toHitValue)
    {
        int[] rolls = RollMultipleDice(attacks);
        List<int> hits = new List<int>();

        foreach (int roll in rolls)
        {
            if (roll >= toHitValue)
            {
                hits.Add(roll);
            }
        }

        return hits.ToArray();
    }

    public static int[] RollToWound(int hits, int toWoundValue)
    {
        int[] rolls = RollMultipleDice(hits);
        List<int> wounds = new List<int>();

        foreach (int roll in rolls)
        {
            if (roll >= toWoundValue)
            {
                wounds.Add(roll);
            }
        }

        return wounds.ToArray();
    }

    public static int[] RollSaves(int wounds, int saveValue, int rend = 0)
    {
        int modifiedSave = Math.Max(2, saveValue - rend); // Saves can't be worse than 2+
        int[] rolls = RollMultipleDice(wounds);
        List<int> saves = new List<int>();

        foreach (int roll in rolls)
        {
            if (roll >= modifiedSave)
            {
                saves.Add(roll);
            }
        }

        return saves.ToArray();
    }

    public static int RollCharge(int chargeDistance = 12)
    {
        return RollD6() + RollD6();
    }

    public static int RollBattleshock(int bravery, int casualties)
    {
        int roll = RollD6();
        return Math.Max(0, roll + casualties - bravery);
    }

    public static int RollCasting(int castingValue)
    {
        return RollD6() + RollD6();
    }

    public static int RollDispelling(int dispelValue)
    {
        return RollD6() + RollD6();
    }

    // Physical dice rolling with 3D dice
    public void RollPhysicalDice(int count, Vector3 position)
    {
        for (int i = 0; i < count; i++)
        {
            CreatePhysicalDie(position + new Vector3(i * 2, 0, 0));
        }
    }

    private void CreatePhysicalDie(Vector3 position)
    {
        var die = new RigidBody3D();
        var mesh = new MeshInstance3D();
        var boxMesh = new BoxMesh();
        boxMesh.Size = new Vector3(1, 1, 1);
        mesh.Mesh = boxMesh;
        die.AddChild(mesh);

        // Add collision
        var collision = new CollisionShape3D();
        var boxShape = new BoxShape3D();
        boxShape.Size = new Vector3(1, 1, 1);
        collision.Shape = boxShape;
        die.AddChild(collision);

        // Position and add random rotation
        die.GlobalPosition = position;
        die.Rotation = new Vector3(
            (float)(_random.NextDouble() * Math.PI * 2),
            (float)(_random.NextDouble() * Math.PI * 2),
            (float)(_random.NextDouble() * Math.PI * 2)
        );

        // Add to scene
        GetTree().CurrentScene.AddChild(die);

        // Auto-destroy after 5 seconds
        var timer = new Timer();
        timer.WaitTime = 5.0f;
        timer.OneShot = true;
        timer.Timeout += () => die.QueueFree();
        die.AddChild(timer);
        timer.Start();
    }

    // Combat sequence helper
    public static CombatRollResult PerformCombatRoll(int attacks, int toHit, int toWound, int save, int rend = 0)
    {
        var result = new CombatRollResult();

        // Roll to hit
        int[] hitRolls = RollToHit(attacks, toHit);
        result.Hits = hitRolls.Length;
        result.HitRolls = hitRolls;

        // Roll to wound
        int[] woundRolls = RollToWound(result.Hits, toWound);
        result.Wounds = woundRolls.Length;
        result.WoundRolls = woundRolls;

        // Roll saves
        int[] saveRolls = RollSaves(result.Wounds, save, rend);
        result.SavedWounds = saveRolls.Length;
        result.SaveRolls = saveRolls;

        result.UnsavedWounds = result.Wounds - result.SavedWounds;

        return result;
    }

    // Command ability rolls
    public static bool RollCommandAbility(int commandAbilityValue)
    {
        int roll = RollD6();
        return roll >= commandAbilityValue;
    }

    // Spell casting rolls
    public static SpellCastResult RollSpellCast(int castingValue, int dispelRoll = 0)
    {
        int roll = RollCasting(castingValue);
        bool success = roll >= castingValue;
        bool dispelled = dispelRoll > 0 && dispelRoll >= roll;

        return new SpellCastResult
        {
            Roll = roll,
            Success = success && !dispelled,
            Dispelled = dispelled
        };
    }

    // Prayer rolls
    public static bool RollPrayer(int prayerValue)
    {
        int roll = RollD6();
        return roll >= prayerValue;
    }

    // Rally rolls
    public static int RollRally(int bravery)
    {
        int roll = RollD6();
        return roll >= bravery ? 1 : 0;
    }

    // Random movement rolls
    public static int RollRandomMovement(int diceCount = 1)
    {
        int total = 0;
        for (int i = 0; i < diceCount; i++)
        {
            total += RollD6();
        }
        return total;
    }

    // Exploding dice (6s generate extra hits)
    public static int[] RollExplodingDice(int count, int sides = 6)
    {
        List<int> results = new List<int>();
        int extraRolls = 0;

        for (int i = 0; i < count + extraRolls; i++)
        {
            int roll = RollDice(sides);
            results.Add(roll);

            if (roll == sides) // Exploding on max value
            {
                extraRolls++;
            }
        }

        return results.ToArray();
    }

    // Reroll mechanics
    public static int[] RerollDice(int[] originalRolls, Func<int, bool> rerollCondition)
    {
        int[] newRolls = new int[originalRolls.Length];
        
        for (int i = 0; i < originalRolls.Length; i++)
        {
            if (rerollCondition(originalRolls[i]))
            {
                newRolls[i] = RollD6();
            }
            else
            {
                newRolls[i] = originalRolls[i];
            }
        }

        return newRolls;
    }

    // Modifier application
    public static int ApplyModifier(int roll, int modifier)
    {
        return Math.Max(1, roll + modifier); // Dice rolls can't go below 1
    }

    // Multiple modifier application
    public static int ApplyModifiers(int roll, params int[] modifiers)
    {
        int result = roll;
        foreach (int modifier in modifiers)
        {
            result = ApplyModifier(result, modifier);
        }
        return result;
    }
}

public class CombatRollResult
{
    public int Hits { get; set; } = 0;
    public int Wounds { get; set; } = 0;
    public int SavedWounds { get; set; } = 0;
    public int UnsavedWounds { get; set; } = 0;
    public int[] HitRolls { get; set; } = new int[0];
    public int[] WoundRolls { get; set; } = new int[0];
    public int[] SaveRolls { get; set; } = new int[0];
}

public class SpellCastResult
{
    public int Roll { get; set; }
    public bool Success { get; set; }
    public bool Dispelled { get; set; }
}
