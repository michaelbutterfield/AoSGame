using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class UnitDatabase
{
    private static Dictionary<string, UnitData> _unitDatabase = new Dictionary<string, UnitData>();
    private static Dictionary<string, List<UnitAbility>> _unitAbilities = new Dictionary<string, List<UnitAbility>>();
    private static Dictionary<string, List<ModelAbility>> _modelAbilities = new Dictionary<string, List<ModelAbility>>();
    private static Dictionary<string, List<RegimentOfRenown>> _regimentsOfRenown = new Dictionary<string, List<RegimentOfRenown>>();
    private static Dictionary<string, List<HeroicTrait>> _heroicTraits = new Dictionary<string, List<HeroicTrait>>();
    private static Dictionary<string, List<ArtefactOfPower>> _artefactsOfPower = new Dictionary<string, List<ArtefactOfPower>>();
    private static Dictionary<string, List<BattleFormation>> _battleFormations = new Dictionary<string, List<BattleFormation>>();

    public static void Initialize()
    {
        InitializeUnits();
        InitializeUnitAbilities();
        InitializeModelAbilities();
        InitializeRegimentsOfRenown();
        InitializeHeroicTraits();
        InitializeArtefactsOfPower();
        InitializeBattleFormations();
    }

    private static void InitializeUnits()
    {
        // Stormcast Eternals - AoS 4th Edition Points with correct base sizes (mm) and measurements (inches)
        AddUnit("Liberators", new UnitData
        {
            Name = "Liberators",
            Faction = "Stormcast Eternals",
            Points = 120,
            Type = UnitType.Battleline,
            Move = 5, // 5 inches
            Wounds = 2,
            Bravery = 7,
            Save = 4,
            Attacks = 2,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 5,
            BaseSize = 32.0f, // 32mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Lord-Celestant", new UnitData
        {
            Name = "Lord-Celestant",
            Faction = "Stormcast Eternals",
            Points = 160,
            Type = UnitType.Leader,
            Move = 5, // 5 inches
            Wounds = 5,
            Bravery = 8,
            Save = 3,
            Attacks = 4,
            ToHit = 3,
            ToWound = 3,
            Rend = -1,
            Damage = 2,
            ModelCount = 1,
            BaseSize = 40.0f, // 40mm round base
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Knight-Incantor", new UnitData
        {
            Name = "Knight-Incantor",
            Faction = "Stormcast Eternals",
            Points = 140,
            Type = UnitType.Leader,
            Move = 5, // 5 inches
            Wounds = 4,
            Bravery = 7,
            Save = 4,
            Attacks = 2,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 1,
            BaseSize = 32.0f, // 32mm round base
            IsHero = true,
            IsGeneral = false,
            IsWizard = true,
            IsPriest = false
        });

        AddUnit("Vindictors", new UnitData
        {
            Name = "Vindictors",
            Faction = "Stormcast Eternals",
            Points = 130,
            Type = UnitType.Battleline,
            Move = 5, // 5 inches
            Wounds = 2,
            Bravery = 7,
            Save = 4,
            Attacks = 2,
            ToHit = 4,
            ToWound = 3,
            Rend = -1,
            Damage = 1,
            ModelCount = 5,
            BaseSize = 32.0f, // 32mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Retributors", new UnitData
        {
            Name = "Retributors",
            Faction = "Stormcast Eternals",
            Points = 150,
            Type = UnitType.Other,
            Move = 5, // 5 inches
            Wounds = 3,
            Bravery = 7,
            Save = 4,
            Attacks = 3,
            ToHit = 4,
            ToWound = 3,
            Rend = -1,
            Damage = 2,
            ModelCount = 5,
            BaseSize = 40.0f, // 40mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Prosecutors", new UnitData
        {
            Name = "Prosecutors",
            Faction = "Stormcast Eternals",
            Points = 140,
            Type = UnitType.Other,
            Move = 12, // 12 inches (flying)
            Wounds = 2,
            Bravery = 7,
            Save = 4,
            Attacks = 2,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 3,
            BaseSize = 40.0f, // 40mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        // Orruk Warclans - AoS 4th Edition Points with correct base sizes
        AddUnit("Orruk Brute", new UnitData
        {
            Name = "Orruk Brute",
            Faction = "Orruk Warclans",
            Points = 140,
            Type = UnitType.Battleline,
            Move = 5, // 5 inches
            Wounds = 3,
            Bravery = 6,
            Save = 5,
            Attacks = 3,
            ToHit = 4,
            ToWound = 3,
            Rend = -1,
            Damage = 1,
            ModelCount = 5,
            BaseSize = 40.0f, // 40mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Megaboss", new UnitData
        {
            Name = "Megaboss",
            Faction = "Orruk Warclans",
            Points = 180,
            Type = UnitType.Leader,
            Move = 5, // 5 inches
            Wounds = 6,
            Bravery = 8,
            Save = 4,
            Attacks = 5,
            ToHit = 3,
            ToWound = 3,
            Rend = -2,
            Damage = 2,
            ModelCount = 1,
            BaseSize = 50.0f, // 50mm round base
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Warchanter", new UnitData
        {
            Name = "Warchanter",
            Faction = "Orruk Warclans",
            Points = 120,
            Type = UnitType.Leader,
            Move = 5, // 5 inches
            Wounds = 4,
            Bravery = 7,
            Save = 5,
            Attacks = 3,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 1,
            BaseSize = 32.0f, // 32mm round base
            IsHero = true,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = true
        });

        AddUnit("Gore-Gruntas", new UnitData
        {
            Name = "Gore-Gruntas",
            Faction = "Orruk Warclans",
            Points = 170,
            Type = UnitType.Other,
            Move = 8, // 8 inches
            Wounds = 4,
            Bravery = 7,
            Save = 4,
            Attacks = 4,
            ToHit = 4,
            ToWound = 3,
            Rend = -1,
            Damage = 2,
            ModelCount = 3,
            BaseSize = 60.0f, // 60mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        // Cities of Sigmar - AoS 4th Edition Points with correct base sizes
        AddUnit("Freeguild Guard", new UnitData
        {
            Name = "Freeguild Guard",
            Faction = "Cities of Sigmar",
            Points = 90,
            Type = UnitType.Battleline,
            Move = 5, // 5 inches
            Wounds = 1,
            Bravery = 6,
            Save = 5,
            Attacks = 1,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 10,
            BaseSize = 25.0f, // 25mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Freeguild General", new UnitData
        {
            Name = "Freeguild General",
            Faction = "Cities of Sigmar",
            Points = 130,
            Type = UnitType.Leader,
            Move = 5, // 5 inches
            Wounds = 4,
            Bravery = 7,
            Save = 4,
            Attacks = 4,
            ToHit = 3,
            ToWound = 3,
            Rend = -1,
            Damage = 2,
            ModelCount = 1,
            BaseSize = 32.0f, // 32mm round base
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Freeguild Handgunners", new UnitData
        {
            Name = "Freeguild Handgunners",
            Faction = "Cities of Sigmar",
            Points = 110,
            Type = UnitType.Battleline,
            Move = 5, // 5 inches
            Wounds = 1,
            Bravery = 6,
            Save = 5,
            Attacks = 1,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 10,
            BaseSize = 25.0f, // 25mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        // Nighthaunt - AoS 4th Edition Points with correct base sizes
        AddUnit("Chainrasp Horde", new UnitData
        {
            Name = "Chainrasp Horde",
            Faction = "Nighthaunt",
            Points = 110,
            Type = UnitType.Battleline,
            Move = 6, // 6 inches
            Wounds = 1,
            Bravery = 10,
            Save = 6,
            Attacks = 2,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 10,
            BaseSize = 25.0f, // 25mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Knight of Shrouds", new UnitData
        {
            Name = "Knight of Shrouds",
            Faction = "Nighthaunt",
            Points = 120,
            Type = UnitType.Leader,
            Move = 6, // 6 inches
            Wounds = 4,
            Bravery = 10,
            Save = 6,
            Attacks = 4,
            ToHit = 3,
            ToWound = 3,
            Rend = -1,
            Damage = 2,
            ModelCount = 1,
            BaseSize = 40.0f, // 40mm round base
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });

        // Sylvaneth - AoS 4th Edition Points with correct base sizes
        AddUnit("Dryads", new UnitData
        {
            Name = "Dryads",
            Faction = "Sylvaneth",
            Points = 100,
            Type = UnitType.Battleline,
            Move = 6, // 6 inches
            Wounds = 1,
            Bravery = 7,
            Save = 5,
            Attacks = 2,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 10,
            BaseSize = 25.0f, // 25mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Branchwych", new UnitData
        {
            Name = "Branchwych",
            Faction = "Sylvaneth",
            Points = 130,
            Type = UnitType.Leader,
            Move = 6, // 6 inches
            Wounds = 4,
            Bravery = 7,
            Save = 5,
            Attacks = 3,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 1,
            BaseSize = 32.0f, // 32mm round base
            IsHero = true,
            IsGeneral = false,
            IsWizard = true,
            IsPriest = false
        });

        // Khorne - AoS 4th Edition Points with correct base sizes
        AddUnit("Bloodreavers", new UnitData
        {
            Name = "Bloodreavers",
            Faction = "Khorne",
            Points = 90,
            Type = UnitType.Battleline,
            Move = 6, // 6 inches
            Wounds = 1,
            Bravery = 6,
            Save = 6,
            Attacks = 2,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 10,
            BaseSize = 25.0f, // 25mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Bloodthirster", new UnitData
        {
            Name = "Bloodthirster",
            Faction = "Khorne",
            Points = 350,
            Type = UnitType.Leader,
            Move = 12, // 12 inches (flying)
            Wounds = 14,
            Bravery = 10,
            Save = 4,
            Attacks = 6,
            ToHit = 3,
            ToWound = 3,
            Rend = -2,
            Damage = 3,
            ModelCount = 1,
            BaseSize = 100.0f, // 100mm round base
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });

        // Tzeentch - AoS 4th Edition Points with correct base sizes
        AddUnit("Pink Horrors", new UnitData
        {
            Name = "Pink Horrors",
            Faction = "Tzeentch",
            Points = 130,
            Type = UnitType.Battleline,
            Move = 5, // 5 inches
            Wounds = 1,
            Bravery = 7,
            Save = 6,
            Attacks = 2,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 10,
            BaseSize = 25.0f, // 25mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = true,
            IsPriest = false
        });

        AddUnit("Lord of Change", new UnitData
        {
            Name = "Lord of Change",
            Faction = "Tzeentch",
            Points = 400,
            Type = UnitType.Leader,
            Move = 12, // 12 inches (flying)
            Wounds = 16,
            Bravery = 10,
            Save = 4,
            Attacks = 6,
            ToHit = 3,
            ToWound = 3,
            Rend = -2,
            Damage = 3,
            ModelCount = 1,
            BaseSize = 100.0f, // 100mm round base
            IsHero = true,
            IsGeneral = true,
            IsWizard = true,
            IsPriest = false
        });

        // Lumineth Realm-Lords - AoS 4th Edition Points with correct base sizes
        AddUnit("Vanari Aetherwings", new UnitData
        {
            Name = "Vanari Aetherwings",
            Faction = "Lumineth Realm-Lords",
            Points = 70,
            Type = UnitType.Battleline,
            Move = 12, // 12 inches (flying)
            Wounds = 1,
            Bravery = 8,
            Save = 5,
            Attacks = 2,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 3,
            BaseSize = 32.0f, // 32mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Vanari Dawnriders", new UnitData
        {
            Name = "Vanari Dawnriders",
            Faction = "Lumineth Realm-Lords",
            Points = 140,
            Type = UnitType.Other,
            Move = 10, // 10 inches
            Wounds = 2,
            Bravery = 8,
            Save = 4,
            Attacks = 3,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 5,
            BaseSize = 60.0f, // 60mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        // Idoneth Deepkin - AoS 4th Edition Points with correct base sizes
        AddUnit("Namarti Thralls", new UnitData
        {
            Name = "Namarti Thralls",
            Faction = "Idoneth Deepkin",
            Points = 120,
            Type = UnitType.Battleline,
            Move = 6, // 6 inches
            Wounds = 1,
            Bravery = 6,
            Save = 5,
            Attacks = 2,
            ToHit = 4,
            ToWound = 4,
            Rend = 0,
            Damage = 1,
            ModelCount = 10,
            BaseSize = 25.0f, // 25mm round base
            IsHero = false,
            IsGeneral = false,
            IsWizard = false,
            IsPriest = false
        });

        AddUnit("Akhelian King", new UnitData
        {
            Name = "Akhelian King",
            Faction = "Idoneth Deepkin",
            Points = 200,
            Type = UnitType.Leader,
            Move = 12, // 12 inches (flying)
            Wounds = 6,
            Bravery = 8,
            Save = 4,
            Attacks = 5,
            ToHit = 3,
            ToWound = 3,
            Rend = -2,
            Damage = 2,
            ModelCount = 1,
            BaseSize = 60.0f, // 60mm round base
            IsHero = true,
            IsGeneral = true,
            IsWizard = false,
            IsPriest = false
        });
    }

    private static void InitializeUnitAbilities()
    {
        // Stormcast Eternals Abilities
        AddUnitAbility("Liberators", new UnitAbility
        {
            Name = "Shield of Civilisation",
            Description = "Add 1 to save rolls for attacks that target this unit.",
            FlavorText = "The Liberators' shields are blessed with the power of Sigmar himself.",
            Type = AbilityType.Passive,
            ActivationPhases = new List<GameManager.TurnPhase> { GameManager.TurnPhase.Combat },
            Range = 0,
            TargetType = TargetType.Self,
            Cooldown = 0,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.StatModifier,
                    Target = "Self",
                    Stat = "Save",
                    Value = 1,
                    Duration = EffectDuration.Permanent,
                    IsPermanent = true
                }
            }
        });

        AddUnitAbility("Lord-Celestant", new UnitAbility
        {
            Name = "Lightning Strike",
            Description = "Once per battle, this unit can make a charge move of up to 12\" instead of rolling.",
            FlavorText = "The Lord-Celestant strikes with the speed of lightning.",
            Type = AbilityType.Active,
            ActivationPhases = new List<GameManager.TurnPhase> { GameManager.TurnPhase.Charge },
            Range = 0,
            TargetType = TargetType.Self,
            Cooldown = 1,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.Special,
                    Target = "Self",
                    Stat = "Charge",
                    Value = 12,
                    Duration = EffectDuration.Instant
                }
            }
        });

        AddUnitAbility("Prosecutors", new UnitAbility
        {
            Name = "Fly",
            Description = "This unit can fly. When it makes a move, it can pass across other models and terrain as if they were not there.",
            FlavorText = "The Prosecutors soar above the battlefield on wings of lightning.",
            Type = AbilityType.Passive,
            ActivationPhases = new List<GameManager.TurnPhase> { GameManager.TurnPhase.Movement },
            Range = 0,
            TargetType = TargetType.Self,
            Cooldown = 0,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.Special,
                    Target = "Self",
                    Stat = "Fly",
                    Value = 1,
                    Duration = EffectDuration.Permanent,
                    IsPermanent = true
                }
            }
        });

        // Orruk Warclans Abilities
        AddUnitAbility("Orruk Brute", new UnitAbility
        {
            Name = "Brute Force",
            Description = "Add 1 to the Damage characteristic of melee weapons used by this unit.",
            FlavorText = "Orruk Brutes hit with devastating force.",
            Type = AbilityType.Passive,
            ActivationPhases = new List<GameManager.TurnPhase> { GameManager.TurnPhase.Combat },
            Range = 0,
            TargetType = TargetType.Self,
            Cooldown = 0,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.StatModifier,
                    Target = "Self",
                    Stat = "Damage",
                    Value = 1,
                    Duration = EffectDuration.Permanent,
                    IsPermanent = true
                }
            }
        });

        AddUnitAbility("Gore-Gruntas", new UnitAbility
        {
            Name = "Boar Charge",
            Description = "Add 1 to hit rolls for attacks made by this unit if it made a charge move in the same turn.",
            FlavorText = "The Gore-Gruntas' momentum from charging makes their attacks more accurate.",
            Type = AbilityType.Passive,
            ActivationPhases = new List<GameManager.TurnPhase> { GameManager.TurnPhase.Combat },
            Range = 0,
            TargetType = TargetType.Self,
            Cooldown = 0,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.StatModifier,
                    Target = "Self",
                    Stat = "ToHit",
                    Value = -1,
                    Duration = EffectDuration.Turn
                }
            }
        });

        // Cities of Sigmar Abilities
        AddUnitAbility("Freeguild Handgunners", new UnitAbility
        {
            Name = "Volley Fire",
            Description = "Add 1 to hit rolls for attacks made by this unit if it has 20 or more models.",
            FlavorText = "The disciplined volley fire of the Handgunners is devastating in large numbers.",
            Type = AbilityType.Passive,
            ActivationPhases = new List<GameManager.TurnPhase> { GameManager.TurnPhase.Shooting },
            Range = 0,
            TargetType = TargetType.Self,
            Cooldown = 0,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.StatModifier,
                    Target = "Self",
                    Stat = "ToHit",
                    Value = -1,
                    Duration = EffectDuration.Permanent,
                    IsPermanent = true
                }
            }
        });

        // Nighthaunt Abilities
        AddUnitAbility("Chainrasp Horde", new UnitAbility
        {
            Name = "Ethereal",
            Description = "This unit has a 6+ ward save.",
            FlavorText = "The Chainrasps are spectral beings, partially existing in the mortal realm.",
            Type = AbilityType.Passive,
            ActivationPhases = new List<GameManager.TurnPhase> { GameManager.TurnPhase.Combat },
            Range = 0,
            TargetType = TargetType.Self,
            Cooldown = 0,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.Special,
                    Target = "Self",
                    Stat = "Ward",
                    Value = 6,
                    Duration = EffectDuration.Permanent,
                    IsPermanent = true
                }
            }
        });

        // Sylvaneth Abilities
        AddUnitAbility("Dryads", new UnitAbility
        {
            Name = "Forest Spirits",
            Description = "Add 1 to save rolls for attacks that target this unit while it is wholly within 6\" of any terrain features.",
            FlavorText = "The Dryads draw strength from the natural world around them.",
            Type = AbilityType.Passive,
            ActivationPhases = new List<GameManager.TurnPhase> { GameManager.TurnPhase.Combat },
            Range = 6.0f,
            TargetType = TargetType.Self,
            Cooldown = 0,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.StatModifier,
                    Target = "Self",
                    Stat = "Save",
                    Value = 1,
                    Duration = EffectDuration.Permanent,
                    IsPermanent = true
                }
            }
        });
    }

    private static void InitializeModelAbilities()
    {
        // Liberators Model Abilities
        AddModelAbility("Liberators", new ModelAbility
        {
            Name = "Liberator-Prime",
            Description = "The Liberator-Prime can make 1 extra attack with its melee weapon.",
            FlavorText = "The Prime leads by example, striking with greater skill.",
            Type = ModelAbilityType.Passive,
            RequiresChampion = true,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.StatModifier,
                    Target = "Self",
                    Stat = "Attacks",
                    Value = 1,
                    Duration = EffectDuration.Permanent,
                    IsPermanent = true
                }
            }
        });

        AddModelAbility("Liberators", new ModelAbility
        {
            Name = "Standard Bearer",
            Description = "Add 1 to the Bravery characteristic of this unit.",
            FlavorText = "The unit's banner inspires courage in all who fight alongside it.",
            Type = ModelAbilityType.Passive,
            RequiresBannerBearer = true,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.StatModifier,
                    Target = "Self",
                    Stat = "Bravery",
                    Value = 1,
                    Duration = EffectDuration.Permanent,
                    IsPermanent = true
                }
            }
        });

        // Orruk Brute Model Abilities
        AddModelAbility("Orruk Brute", new ModelAbility
        {
            Name = "Brute Boss",
            Description = "The Brute Boss can make 1 extra attack with its melee weapon.",
            FlavorText = "The Boss leads the charge with brutal efficiency.",
            Type = ModelAbilityType.Passive,
            RequiresChampion = true,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.StatModifier,
                    Target = "Self",
                    Stat = "Attacks",
                    Value = 1,
                    Duration = EffectDuration.Permanent,
                    IsPermanent = true
                }
            }
        });

        // Freeguild Guard Model Abilities
        AddModelAbility("Freeguild Guard", new ModelAbility
        {
            Name = "Sergeant",
            Description = "The Sergeant can make 1 extra attack with its melee weapon.",
            FlavorText = "The Sergeant's training and experience make them more effective in combat.",
            Type = ModelAbilityType.Passive,
            RequiresChampion = true,
            Effects = new List<AbilityEffect>
            {
                new AbilityEffect
                {
                    Type = EffectType.StatModifier,
                    Target = "Self",
                    Stat = "Attacks",
                    Value = 1,
                    Duration = EffectDuration.Permanent,
                    IsPermanent = true
                }
            }
        });
    }

    private static void InitializeRegimentsOfRenown()
    {
        // Stormcast Eternals Regiments
        AddRegimentOfRenown("Stormcast Eternals", new RegimentOfRenown
        {
            Name = "Hammer of Sigmar",
            Description = "A legendary regiment of Stormcast Eternals known for their unbreakable resolve.",
            Faction = "Stormcast Eternals",
            Units = new List<string> { "Lord-Celestant", "Liberators", "Vindictors" },
            SpecialRules = new List<string> { "Unbreakable", "Sigmar's Blessing" },
            PointBonus = 0
        });

        // Orruk Warclans Regiments
        AddRegimentOfRenown("Orruk Warclans", new RegimentOfRenown
        {
            Name = "Ironjawz Waaagh!",
            Description = "A mighty waaagh! led by the most powerful Orruk warlords.",
            Faction = "Orruk Warclans",
            Units = new List<string> { "Megaboss", "Orruk Brute", "Warchanter" },
            SpecialRules = new List<string> { "Waaagh!", "Ironjawz Might" },
            PointBonus = 0
        });
    }

    private static void InitializeHeroicTraits()
    {
        // Stormcast Eternals Heroic Traits
        AddHeroicTrait("Stormcast Eternals", new HeroicTrait
        {
            Name = "Lightning Strike",
            Description = "This HERO can make a charge move of up to 12\" instead of rolling.",
            Faction = "Stormcast Eternals",
            Effect = "Once per battle, charge move of 12\"",
            PointCost = 0
        });

        AddHeroicTrait("Stormcast Eternals", new HeroicTrait
        {
            Name = "Unbreakable",
            Description = "This HERO cannot be affected by battleshock tests.",
            Faction = "Stormcast Eternals",
            Effect = "Immune to battleshock",
            PointCost = 0
        });

        // Orruk Warclans Heroic Traits
        AddHeroicTrait("Orruk Warclans", new HeroicTrait
        {
            Name = "Brutal Cunning",
            Description = "This HERO can use the 'All-out Attack' command ability for free once per turn.",
            Faction = "Orruk Warclans",
            Effect = "Free All-out Attack once per turn",
            PointCost = 0
        });
    }

    private static void InitializeArtefactsOfPower()
    {
        // Stormcast Eternals Artefacts
        AddArtefactOfPower("Stormcast Eternals", new ArtefactOfPower
        {
            Name = "Godforged Blade",
            Description = "Add 1 to the Damage characteristic of melee weapons used by the bearer.",
            Faction = "Stormcast Eternals",
            Effect = "+1 Damage to melee weapons",
            PointCost = 0
        });

        AddArtefactOfPower("Stormcast Eternals", new ArtefactOfPower
        {
            Name = "Mirrorshield",
            Description = "Add 1 to save rolls for attacks that target the bearer.",
            Faction = "Stormcast Eternals",
            Effect = "+1 to save rolls",
            PointCost = 0
        });

        // Orruk Warclans Artefacts
        AddArtefactOfPower("Orruk Warclans", new ArtefactOfPower
        {
            Name = "Destroyer",
            Description = "Add 1 to the Rend characteristic of melee weapons used by the bearer.",
            Faction = "Orruk Warclans",
            Effect = "+1 Rend to melee weapons",
            PointCost = 0
        });
    }

    private static void InitializeBattleFormations()
    {
        // Stormcast Eternals Formations
        AddBattleFormation("Stormcast Eternals", new BattleFormation
        {
            Name = "Thunderstrike Brotherhood",
            Description = "A formation that enhances the combat abilities of Stormcast units.",
            Faction = "Stormcast Eternals",
            RequiredUnits = new List<string> { "Lord-Celestant", "Liberators" },
            MinUnitCount = 2,
            SpecialRules = new List<string> { "Thunderstrike", "Brotherhood" },
            PointBonus = 0
        });

        // Orruk Warclans Formations
        AddBattleFormation("Orruk Warclans", new BattleFormation
        {
            Name = "Ironjawz Waaagh!",
            Description = "A formation that increases the ferocity of Orruk units.",
            Faction = "Orruk Warclans",
            RequiredUnits = new List<string> { "Megaboss", "Orruk Brute" },
            MinUnitCount = 2,
            SpecialRules = new List<string> { "Waaagh!", "Ironjawz" },
            PointBonus = 0
        });
    }

    // Helper methods
    private static void AddUnit(string name, UnitData unitData)
    {
        _unitDatabase[name] = unitData;
    }

    private static void AddUnitAbility(string unitName, UnitAbility ability)
    {
        if (!_unitAbilities.ContainsKey(unitName))
            _unitAbilities[unitName] = new List<UnitAbility>();
        _unitAbilities[unitName].Add(ability);
    }

    private static void AddModelAbility(string unitName, ModelAbility ability)
    {
        if (!_modelAbilities.ContainsKey(unitName))
            _modelAbilities[unitName] = new List<ModelAbility>();
        _modelAbilities[unitName].Add(ability);
    }

    private static void AddRegimentOfRenown(string faction, RegimentOfRenown regiment)
    {
        if (!_regimentsOfRenown.ContainsKey(faction))
            _regimentsOfRenown[faction] = new List<RegimentOfRenown>();
        _regimentsOfRenown[faction].Add(regiment);
    }

    private static void AddHeroicTrait(string faction, HeroicTrait trait)
    {
        if (!_heroicTraits.ContainsKey(faction))
            _heroicTraits[faction] = new List<HeroicTrait>();
        _heroicTraits[faction].Add(trait);
    }

    private static void AddArtefactOfPower(string faction, ArtefactOfPower artefact)
    {
        if (!_artefactsOfPower.ContainsKey(faction))
            _artefactsOfPower[faction] = new List<ArtefactOfPower>();
        _artefactsOfPower[faction].Add(artefact);
    }

    private static void AddBattleFormation(string faction, BattleFormation formation)
    {
        if (!_battleFormations.ContainsKey(faction))
            _battleFormations[faction] = new List<BattleFormation>();
        _battleFormations[faction].Add(formation);
    }

    // Public access methods
    public static UnitData GetUnit(string unitName)
    {
        return _unitDatabase.ContainsKey(unitName) ? _unitDatabase[unitName] : null;
    }

    public static List<UnitAbility> GetUnitAbilities(string unitName)
    {
        return _unitAbilities.ContainsKey(unitName) ? _unitAbilities[unitName] : new List<UnitAbility>();
    }

    public static List<ModelAbility> GetModelAbilities(string unitName)
    {
        return _modelAbilities.ContainsKey(unitName) ? _modelAbilities[unitName] : new List<ModelAbility>();
    }

    public static List<RegimentOfRenown> GetRegimentsOfRenown(string faction)
    {
        return _regimentsOfRenown.ContainsKey(faction) ? _regimentsOfRenown[faction] : new List<RegimentOfRenown>();
    }

    public static List<HeroicTrait> GetHeroicTraits(string faction)
    {
        return _heroicTraits.ContainsKey(faction) ? _heroicTraits[faction] : new List<HeroicTrait>();
    }

    public static List<ArtefactOfPower> GetArtefactsOfPower(string faction)
    {
        return _artefactsOfPower.ContainsKey(faction) ? _artefactsOfPower[faction] : new List<ArtefactOfPower>();
    }

    public static List<BattleFormation> GetBattleFormations(string faction)
    {
        return _battleFormations.ContainsKey(faction) ? _battleFormations[faction] : new List<BattleFormation>();
    }

    public static List<string> GetAllUnitNames()
    {
        return _unitDatabase.Keys.ToList();
    }

    public static List<string> GetUnitsByFaction(string faction)
    {
        return _unitDatabase.Values.Where(u => u.Faction == faction).Select(u => u.Name).ToList();
    }

    public static List<string> GetAvailableFactions()
    {
        return _unitDatabase.Values.Select(u => u.Faction).Distinct().ToList();
    }

    /// <summary>
    /// Get base size in millimeters for a unit
    /// </summary>
    public static float GetBaseSizeInMillimeters(string unitName)
    {
        var unit = GetUnit(unitName);
        return unit?.BaseSize ?? 0.0f;
    }

    /// <summary>
    /// Get base size in inches for a unit
    /// </summary>
    public static float GetBaseSizeInInches(string unitName)
    {
        var mm = GetBaseSizeInMillimeters(unitName);
        return mm / 25.4f; // Convert mm to inches
    }

    /// <summary>
    /// Get movement in inches for a unit
    /// </summary>
    public static int GetMovementInInches(string unitName)
    {
        var unit = GetUnit(unitName);
        return unit?.Move ?? 0;
    }
}

// New data classes for enhanced features
public class RegimentOfRenown
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Faction { get; set; }
    public List<string> Units { get; set; } = new List<string>();
    public List<string> SpecialRules { get; set; } = new List<string>();
    public int PointBonus { get; set; }
}

public class HeroicTrait
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Faction { get; set; }
    public string Effect { get; set; }
    public int PointCost { get; set; }
}

public class ArtefactOfPower
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Faction { get; set; }
    public string Effect { get; set; }
    public int PointCost { get; set; }
}

public class BattleFormation
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Faction { get; set; }
    public List<string> RequiredUnits { get; set; } = new List<string>();
    public int MinUnitCount { get; set; }
    public List<string> SpecialRules { get; set; } = new List<string>();
    public int PointBonus { get; set; }
}
