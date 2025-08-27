using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AoSGame.Tests
{
    [TestFixture]
    public class TestSuite
    {
        private GameManager _gameManager;
        private ArmyBuilder _armyBuilder;
        private DiceManager _diceManager;
        private AIOpponent _aiOpponent;
        private TerrainPlacer _terrainPlacer;
        private GameSettings _gameSettings;

        [SetUp]
        public void Setup()
        {
            // Initialize test environment
            _gameManager = new GameManager();
            _armyBuilder = new ArmyBuilder();
            _diceManager = new DiceManager();
            _aiOpponent = new AIOpponent();
            _terrainPlacer = new TerrainPlacer();
            _gameSettings = new GameSettings();
        }

        [TearDown]
        public void Teardown()
        {
            // Clean up test environment
            _gameManager?.QueueFree();
            _armyBuilder?.QueueFree();
            _diceManager?.QueueFree();
            _aiOpponent?.QueueFree();
            _terrainPlacer?.QueueFree();
            _gameSettings?.QueueFree();
        }

        #region GameManager Tests

        [Test]
        public void TestGameManagerInitialization()
        {
            Assert.IsNotNull(_gameManager);
            Assert.AreEqual(GameManager.GameState.MainMenu, _gameManager.CurrentGameState);
            Assert.AreEqual(GameManager.TurnPhase.Hero, _gameManager.CurrentTurnPhase);
            Assert.AreEqual(1, _gameManager.CurrentPlayerTurn);
            Assert.AreEqual(2, _gameManager.MaxPlayers);
        }

        [Test]
        public void TestGameStateTransitions()
        {
            _gameManager.StartSinglePlayerGame();
            Assert.AreEqual(GameManager.GameState.Setup, _gameManager.CurrentGameState);

            _gameManager.StartGame();
            Assert.AreEqual(GameManager.GameState.Playing, _gameManager.CurrentGameState);
        }

        [Test]
        public void TestTurnPhaseProgression()
        {
            _gameManager.StartSinglePlayerGame();
            _gameManager.StartGame();

            var initialPhase = _gameManager.CurrentTurnPhase;
            _gameManager.NextTurnPhase();
            Assert.AreNotEqual(initialPhase, _gameManager.CurrentTurnPhase);

            // Test full phase cycle
            for (int i = 0; i < 5; i++)
            {
                var currentPhase = _gameManager.CurrentTurnPhase;
                _gameManager.NextTurnPhase();
                Assert.AreNotEqual(currentPhase, _gameManager.CurrentTurnPhase);
            }
        }

        [Test]
        public void TestPlayerTurnChanges()
        {
            _gameManager.StartSinglePlayerGame();
            _gameManager.StartGame();

            var initialPlayer = _gameManager.CurrentPlayerTurn;
            
            // Complete a full turn cycle
            for (int i = 0; i < 5; i++)
            {
                _gameManager.NextTurnPhase();
            }

            Assert.AreNotEqual(initialPlayer, _gameManager.CurrentPlayerTurn);
        }

        [Test]
        public void TestBoardPositionValidation()
        {
            var validPosition = new Vector3(0, 0, 0);
            var invalidPosition = new Vector3(100, 0, 100);

            Assert.IsTrue(_gameManager.IsValidBoardPosition(validPosition));
            Assert.IsFalse(_gameManager.IsValidBoardPosition(invalidPosition));
        }

        [Test]
        public void TestUnitConversion()
        {
            float inches = 12.0f;
            float units = _gameManager.ConvertInchesToUnits(inches);
            float convertedBack = _gameManager.ConvertUnitsToInches(units);

            Assert.AreEqual(inches, convertedBack, 0.001f);
        }

        #endregion

        #region ArmyBuilder Tests

        [Test]
        public void TestArmyBuilderInitialization()
        {
            Assert.IsNotNull(_armyBuilder);
            Assert.AreEqual(2000, _armyBuilder.MaxPoints);
            Assert.IsNotNull(_armyBuilder.GetCurrentArmy());
        }

        [Test]
        public void TestUnitDatabase()
        {
            var unitDatabase = _armyBuilder.GetUnitDatabase();
            Assert.IsNotNull(unitDatabase);
            Assert.Greater(unitDatabase.Count, 0);

            // Test specific factions exist
            var factions = _armyBuilder.GetAvailableFactions();
            Assert.Contains("Stormcast Eternals", factions);
            Assert.Contains("Orruk Warclans", factions);
        }

        [Test]
        public void TestAddingUnits()
        {
            var initialCount = _armyBuilder.GetCurrentArmy().Units.Count;
            
            bool success = _armyBuilder.AddUnit("Liberators");
            Assert.IsTrue(success);
            
            var newCount = _armyBuilder.GetCurrentArmy().Units.Count;
            Assert.Greater(newCount, initialCount);
        }

        [Test]
        public void TestRemovingUnits()
        {
            _armyBuilder.AddUnit("Liberators");
            var initialCount = _armyBuilder.GetCurrentArmy().Units.Count;
            
            bool success = _armyBuilder.RemoveUnit("Liberators");
            Assert.IsTrue(success);
            
            var newCount = _armyBuilder.GetCurrentArmy().Units.Count;
            Assert.Less(newCount, initialCount);
        }

        [Test]
        public void TestArmyValidation()
        {
            // Empty army should be invalid
            Assert.IsFalse(_armyBuilder.ValidateArmy());

            // Add minimum required units (only Leader required in AoS 4th Edition)
            _armyBuilder.AddUnit("Lord-Celestant"); // Leader

            Assert.IsTrue(_armyBuilder.ValidateArmy());
        }

        [Test]
        public void TestPointLimitEnforcement()
        {
            // Add units until we exceed point limit
            int unitsAdded = 0;
            while (_armyBuilder.GetCurrentArmy().TotalPoints <= _armyBuilder.MaxPoints && unitsAdded < 100)
            {
                bool success = _armyBuilder.AddUnit("Liberators");
                if (!success) break;
                unitsAdded++;
            }

            Assert.LessOrEqual(_armyBuilder.GetCurrentArmy().TotalPoints, _armyBuilder.MaxPoints);
        }

        #endregion

        #region DiceManager Tests

        [Test]
        public void TestDiceRolling()
        {
            var result = _diceManager.RollDice(6, 1);
            Assert.GreaterOrEqual(result, 1);
            Assert.LessOrEqual(result, 6);
        }

        [Test]
        public void TestMultipleDiceRolling()
        {
            var result = _diceManager.RollDice(6, 3);
            Assert.GreaterOrEqual(result, 3);
            Assert.LessOrEqual(result, 18);
        }

        [Test]
        public void TestHitRoll()
        {
            var result = _diceManager.RollHit(3); // 3+ to hit
            Assert.IsTrue(result >= 0); // Should return number of hits
        }

        [Test]
        public void TestWoundRoll()
        {
            var result = _diceManager.RollWound(4, 3); // 4+ to wound, 3 hits
            Assert.IsTrue(result >= 0);
            Assert.LessOrEqual(result, 3);
        }

        [Test]
        public void TestSaveRoll()
        {
            var result = _diceManager.RollSave(4, 2); // 4+ save, 2 wounds
            Assert.IsTrue(result >= 0);
            Assert.LessOrEqual(result, 2);
        }

        [Test]
        public void TestRendCalculation()
        {
            var result = _diceManager.RollSave(4, 2, 1); // 4+ save, 2 wounds, rend -1
            Assert.IsTrue(result >= 0);
            Assert.LessOrEqual(result, 2);
        }

        #endregion

        #region AI Tests

        [Test]
        public void TestAIOpponentInitialization()
        {
            Assert.IsNotNull(_aiOpponent);
            Assert.AreEqual(2, _aiOpponent.PlayerId);
            Assert.IsTrue(_aiOpponent.IsEnabled);
        }

        [Test]
        public void TestAIDecisionMaking()
        {
            // Test AI responds to turn changes
            _aiOpponent.OnTurnStarted();
            // Should not throw exceptions
            Assert.Pass();
        }

        [Test]
        public void TestAIPhaseHandling()
        {
            // Test AI handles all phases
            var phases = Enum.GetValues(typeof(GameManager.TurnPhase));
            foreach (GameManager.TurnPhase phase in phases)
            {
                _aiOpponent.OnPhaseChanged(phase);
                // Should not throw exceptions
            }
            Assert.Pass();
        }

        #endregion

        #region Terrain Tests

        [Test]
        public void TestTerrainPlacerInitialization()
        {
            Assert.IsNotNull(_terrainPlacer);
            Assert.IsFalse(_terrainPlacer.IsPlacementMode);
        }

        [Test]
        public void TestTerrainPlacementMode()
        {
            _terrainPlacer.SetPlacementMode(true);
            Assert.IsTrue(_terrainPlacer.IsPlacementMode);

            _terrainPlacer.SetPlacementMode(false);
            Assert.IsFalse(_terrainPlacer.IsPlacementMode);
        }

        [Test]
        public void TestTerrainTypeSelection()
        {
            _terrainPlacer.SetTerrainType(TerrainType.Forest);
            // Should not throw exceptions
            Assert.Pass();
        }

        [Test]
        public void TestTerrainRandomization()
        {
            _terrainPlacer.RandomizeTerrain(3);
            var placedTerrain = _terrainPlacer.GetPlacedTerrain();
            Assert.AreEqual(3, placedTerrain.Count);
        }

        #endregion

        #region BattleplanManager Tests

        [Test]
        public void TestBattleplanManagerInitialization()
        {
            var battleplanManager = new BattleplanManager();
            Assert.IsNotNull(battleplanManager);
            Assert.IsNotNull(battleplanManager.CurrentBattleplan);
        }

        [Test]
        public void TestBattleplanCreation()
        {
            var battleplanManager = new BattleplanManager();
            
            // Test different battleplan types
            var battleplanTypes = System.Enum.GetValues(typeof(BattleplanType)).Cast<BattleplanType>();
            
            foreach (var battleplanType in battleplanTypes)
            {
                battleplanManager.SetBattleplan(battleplanType);
                Assert.IsNotNull(battleplanManager.CurrentBattleplan);
                Assert.IsNotNull(battleplanManager.CurrentBattleplan.Name);
                Assert.IsNotNull(battleplanManager.CurrentBattleplan.Description);
                Assert.Greater(battleplanManager.CurrentBattleplan.MaxTurns, 0);
            }
        }

        [Test]
        public void TestBattleplanObjectives()
        {
            var battleplanManager = new BattleplanManager();
            battleplanManager.SetBattleplan(BattleplanType.FocalPoints);
            
            var objectives = battleplanManager.GetObjectives();
            Assert.AreEqual(5, objectives.Count); // Focal Points has 5 objectives
            
            battleplanManager.SetBattleplan(BattleplanType.BloodAndGlory);
            objectives = battleplanManager.GetObjectives();
            Assert.AreEqual(2, objectives.Count); // Blood and Glory has 2 objectives
        }

        [Test]
        public void TestDeploymentZones()
        {
            var battleplanManager = new BattleplanManager();
            battleplanManager.SetBattleplan(BattleplanType.ThePitchedBattle);
            
            var deploymentZones = battleplanManager.GetDeploymentZones();
            Assert.AreEqual(2, deploymentZones.Count); // Should have 2 deployment zones
            
            // Check that each player has a deployment zone
            var player1Zone = deploymentZones.FirstOrDefault(z => z.PlayerId == 1);
            var player2Zone = deploymentZones.FirstOrDefault(z => z.PlayerId == 2);
            
            Assert.IsNotNull(player1Zone);
            Assert.IsNotNull(player2Zone);
        }

        [Test]
        public void TestVictoryPoints()
        {
            var battleplanManager = new BattleplanManager();
            battleplanManager.SetBattleplan(BattleplanType.ThePitchedBattle);
            
            // Test awarding victory points
            battleplanManager.AwardVictoryPoints(1, 5);
            Assert.AreEqual(5, battleplanManager.GetPlayerVictoryPoints(1));
            
            battleplanManager.AwardVictoryPoints(1, 3);
            Assert.AreEqual(8, battleplanManager.GetPlayerVictoryPoints(1));
            
            // Test different player
            battleplanManager.AwardVictoryPoints(2, 10);
            Assert.AreEqual(10, battleplanManager.GetPlayerVictoryPoints(2));
        }

        [Test]
        public void TestObjectiveCompletion()
        {
            var battleplanManager = new BattleplanManager();
            battleplanManager.SetBattleplan(BattleplanType.ThePitchedBattle);
            
            var initialVP = battleplanManager.GetPlayerVictoryPoints(1);
            battleplanManager.CompleteObjective(1, 1);
            var newVP = battleplanManager.GetPlayerVictoryPoints(1);
            
            Assert.Greater(newVP, initialVP);
        }

        [Test]
        public void TestVictoryConditions()
        {
            var battleplanManager = new BattleplanManager();
            battleplanManager.SetBattleplan(BattleplanType.ThePitchedBattle);
            
            // Test victory points condition
            battleplanManager.AwardVictoryPoints(1, 15); // More than required 12
            Assert.IsTrue(battleplanManager.CheckVictoryConditions(1));
            
            // Test with insufficient points
            battleplanManager.SetBattleplan(BattleplanType.ThePitchedBattle); // Reset
            battleplanManager.AwardVictoryPoints(1, 5); // Less than required 12
            Assert.IsFalse(battleplanManager.CheckVictoryConditions(1));
        }

        #endregion

        #region GameSettings Tests

        [Test]
        public void TestGameSettingsInitialization()
        {
            Assert.IsNotNull(_gameSettings);
            Assert.AreEqual(1920, _gameSettings.ResolutionWidth);
            Assert.AreEqual(1080, _gameSettings.ResolutionHeight);
        }

        [Test]
        public void TestSettingsPersistence()
        {
            // Test save and load
            _gameSettings.ResolutionWidth = 1280;
            _gameSettings.ResolutionHeight = 720;
            _gameSettings.SaveSettings();

            // Reset to default
            _gameSettings.ResolutionWidth = 1920;
            _gameSettings.ResolutionHeight = 1080;

            // Load settings
            _gameSettings.LoadSettings();
            Assert.AreEqual(1280, _gameSettings.ResolutionWidth);
            Assert.AreEqual(720, _gameSettings.ResolutionHeight);
        }

        [Test]
        public void TestSettingsReset()
        {
            // Change some settings
            _gameSettings.ResolutionWidth = 800;
            _gameSettings.MasterVolume = 0.5f;

            // Reset to defaults
            _gameSettings.ResetToDefaults();

            Assert.AreEqual(1920, _gameSettings.ResolutionWidth);
            Assert.AreEqual(1.0f, _gameSettings.MasterVolume);
        }

        #endregion

        #region Integration Tests

        [Test]
        public void TestFullGameFlow()
        {
            // Test complete game setup and play
            _gameManager.StartSinglePlayerGame();
            Assert.AreEqual(GameManager.GameState.Setup, _gameManager.CurrentGameState);

            // Build an army
            _armyBuilder.AddUnit("Lord-Celestant");
            _armyBuilder.AddUnit("Liberators");
            _armyBuilder.AddUnit("Liberators");
            Assert.IsTrue(_armyBuilder.ValidateArmy());

            // Start the game
            _gameManager.StartGame();
            Assert.AreEqual(GameManager.GameState.Playing, _gameManager.CurrentGameState);

            // Play a few phases
            for (int i = 0; i < 3; i++)
            {
                _gameManager.NextTurnPhase();
            }

            Assert.Pass();
        }

        [Test]
        public void TestArmyAndGameIntegration()
        {
            // Test army integration with game manager
            var army = _armyBuilder.GetCurrentArmy();
            _gameManager.SetPlayerArmy(army);
            // Should not throw exceptions
            Assert.Pass();
        }

        #endregion

        #region Performance Tests

        [Test]
        public void TestDiceRollingPerformance()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < 10000; i++)
            {
                _diceManager.RollDice(6, 1);
            }
            
            stopwatch.Stop();
            Assert.Less(stopwatch.ElapsedMilliseconds, 1000); // Should complete in under 1 second
        }

        [Test]
        public void TestArmyValidationPerformance()
        {
            // Add many units
            for (int i = 0; i < 50; i++)
            {
                _armyBuilder.AddUnit("Liberators");
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _armyBuilder.ValidateArmy();
            stopwatch.Stop();
            
            Assert.Less(stopwatch.ElapsedMilliseconds, 100); // Should complete in under 100ms
        }

        #endregion

        #region Edge Case Tests

        [Test]
        public void TestInvalidDiceRolls()
        {
            // Test edge cases
            Assert.Throws<ArgumentException>(() => _diceManager.RollDice(0, 1));
            Assert.Throws<ArgumentException>(() => _diceManager.RollDice(6, 0));
            Assert.Throws<ArgumentException>(() => _diceManager.RollDice(-1, 1));
        }

        [Test]
        public void TestInvalidArmyOperations()
        {
            // Test removing non-existent unit
            bool result = _armyBuilder.RemoveUnit("NonExistentUnit");
            Assert.IsFalse(result);

            // Test adding invalid unit
            result = _armyBuilder.AddUnit("InvalidUnit");
            Assert.IsFalse(result);
        }

        [Test]
        public void TestGameStateEdgeCases()
        {
            // Test phase progression without starting game
            var initialPhase = _gameManager.CurrentTurnPhase;
            _gameManager.NextTurnPhase();
            Assert.AreEqual(initialPhase, _gameManager.CurrentTurnPhase);
        }

        #endregion

        #region CommandPointManager Tests

        [Test]
        public void TestCommandPointManagerInitialization()
        {
            var commandPointManager = new CommandPointManager();
            Assert.IsNotNull(commandPointManager);
        }

        [Test]
        public void TestPlayerInitialization()
        {
            var commandPointManager = new CommandPointManager();
            commandPointManager.InitializePlayer(1);
            
            var playerCP = commandPointManager.GetPlayerCommandPoints(1);
            Assert.IsNotNull(playerCP);
            Assert.AreEqual(1, playerCP.CurrentPoints); // Starting command points
            Assert.AreEqual(3, playerCP.MaxPoints); // Max command points
        }

        [Test]
        public void TestTurnGeneration()
        {
            var commandPointManager = new CommandPointManager();
            commandPointManager.InitializePlayer(1);
            
            commandPointManager.OnTurnStarted(1);
            var playerCP = commandPointManager.GetPlayerCommandPoints(1);
            Assert.Greater(playerCP.CurrentPoints, 1); // Should have generated additional points
        }

        [Test]
        public void TestAbilityUsage()
        {
            var commandPointManager = new CommandPointManager();
            commandPointManager.InitializePlayer(1);
            commandPointManager.OnTurnStarted(1);
            
            // Create a mock unit for testing
            var unit = new Unit();
            unit.UnitName = "Test Unit";
            unit.PlayerId = 1;
            
            bool canUse = commandPointManager.CanUseCommandAbility(1, "All-out Attack", unit);
            Assert.IsTrue(canUse);
        }

        [Test]
        public void TestPointIncrease()
        {
            var commandPointManager = new CommandPointManager();
            commandPointManager.InitializePlayer(1);
            
            commandPointManager.IncreaseCommandPoints(1, 2, "Test Bonus");
            var playerCP = commandPointManager.GetPlayerCommandPoints(1);
            Assert.AreEqual(3, playerCP.CurrentPoints); // 1 starting + 2 bonus
        }

        [Test]
        public void TestPointDecrease()
        {
            var commandPointManager = new CommandPointManager();
            commandPointManager.InitializePlayer(1);
            commandPointManager.IncreaseCommandPoints(1, 2, "Test Bonus");
            
            commandPointManager.DecreaseCommandPoints(1, 1, "Test Penalty");
            var playerCP = commandPointManager.GetPlayerCommandPoints(1);
            Assert.AreEqual(2, playerCP.CurrentPoints); // 3 - 1 penalty
        }

        [Test]
        public void TestMaxPointsLimit()
        {
            var commandPointManager = new CommandPointManager();
            commandPointManager.InitializePlayer(1);
            
            commandPointManager.IncreaseCommandPoints(1, 5, "Test Bonus");
            var playerCP = commandPointManager.GetPlayerCommandPoints(1);
            Assert.AreEqual(3, playerCP.CurrentPoints); // Should be capped at max
        }

        [Test]
        public void TestAvailableAbilities()
        {
            var commandPointManager = new CommandPointManager();
            commandPointManager.InitializePlayer(1);
            commandPointManager.OnTurnStarted(1);
            
            var abilities = commandPointManager.GetAvailableCommandAbilities(1);
            Assert.IsNotNull(abilities);
            Assert.Greater(abilities.Count, 0);
        }

        [Test]
        public void TestUsedAbilitiesTracking()
        {
            var commandPointManager = new CommandPointManager();
            commandPointManager.InitializePlayer(1);
            commandPointManager.OnTurnStarted(1);
            
            var unit = new Unit();
            unit.UnitName = "Test Unit";
            unit.PlayerId = 1;
            
            commandPointManager.UseCommandAbility(1, "All-out Attack", unit);
            var usedAbilities = commandPointManager.GetUsedAbilitiesThisTurn(1);
            Assert.Contains("All-out Attack", usedAbilities);
        }

        [Test]
        public void TestAbilityResetOnNewTurn()
        {
            var commandPointManager = new CommandPointManager();
            commandPointManager.InitializePlayer(1);
            commandPointManager.OnTurnStarted(1);
            
            var unit = new Unit();
            unit.UnitName = "Test Unit";
            unit.PlayerId = 1;
            
            commandPointManager.UseCommandAbility(1, "All-out Attack", unit);
            commandPointManager.OnTurnStarted(1); // New turn
            
            var usedAbilities = commandPointManager.GetUsedAbilitiesThisTurn(1);
            Assert.AreEqual(0, usedAbilities.Count); // Should be reset
        }

            #endregion
    
    #region Unit Abilities Tests
    [Test]
    public void TestUnitAbilityCreation()
    {
        var ability = new UnitAbility
        {
            Name = "Test Ability",
            Description = "A test ability",
            Type = AbilityType.Passive,
            ActivationPhases = new List<GameManager.TurnPhase> { GameManager.TurnPhase.Combat }
        };
        
        Assert.That(ability.Name, Is.EqualTo("Test Ability"));
        Assert.That(ability.Type, Is.EqualTo(AbilityType.Passive));
        Assert.That(ability.ActivationPhases.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void TestModelAbilityCreation()
    {
        var ability = new ModelAbility
        {
            Name = "Test Model Ability",
            Description = "A test model ability",
            Type = ModelAbilityType.Passive,
            RequiresChampion = true
        };
        
        Assert.That(ability.Name, Is.EqualTo("Test Model Ability"));
        Assert.That(ability.RequiresChampion, Is.True);
    }
    
    [Test]
    public void TestAbilityEffectApplication()
    {
        var effect = new AbilityEffect
        {
            Type = EffectType.StatModifier,
            Target = "Self",
            Stat = "ToHit",
            Value = -1,
            Duration = EffectDuration.Instant
        };
        
        var unit = new Unit();
        unit.ToHit = 4;
        
        effect.Apply(unit, null);
        
        Assert.That(unit.ToHit, Is.EqualTo(3));
    }
    
    [Test]
    public void TestAbilityConditionEvaluation()
    {
        var condition = new AbilityCondition
        {
            Type = ConditionType.Always,
            Stat = "ModelCount",
            Value = 5,
            Operator = ComparisonOperator.GreaterThanOrEqual,
            Target = "Self"
        };
        
        var unit = new Unit();
        unit.ModelCount = 10;
        
        bool result = condition.Evaluate(unit, null);
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void TestUnitDatabaseInitialization()
    {
        UnitDatabase.Initialize();
        
        var liberators = UnitDatabase.GetUnit("Liberators");
        Assert.That(liberators, Is.Not.Null);
        Assert.That(liberators.Faction, Is.EqualTo("Stormcast Eternals"));
        
        var abilities = UnitDatabase.GetUnitAbilities("Liberators");
        Assert.That(abilities.Count, Is.GreaterThan(0));
    }
    
    [Test]
    public void TestUnitWithAbilitiesCreation()
    {
        UnitDatabase.Initialize();
        
        var armyBuilder = new ArmyBuilder();
        var unit = armyBuilder.CreateUnitInstance("Liberators", 1, Vector3.Zero);
        
        Assert.That(unit, Is.Not.Null);
        Assert.That(unit.UnitAbilities.Count, Is.GreaterThan(0));
        Assert.That(unit.HasUnitAbility("Shield of Civilisation"), Is.True);
    }
    
    [Test]
    public void TestAbilityActivation()
    {
        var ability = new UnitAbility
        {
            Name = "Test Ability",
            Type = AbilityType.Active,
            ActivationPhases = new List<GameManager.TurnPhase> { GameManager.TurnPhase.Hero }
        };
        
        var unit = new Unit();
        unit.AddUnitAbility(ability);
        
        Assert.That(unit.HasUnitAbility("Test Ability"), Is.True);
        Assert.That(unit.GetUnitAbility("Test Ability"), Is.EqualTo(ability));
    }
    
    [Test]
    public void TestModelSpecificProperties()
    {
        var ability = new ModelAbility
        {
            Name = "Champion Ability",
            RequiresChampion = true
        };
        
        var unit = new Unit();
        unit.AddModelAbility(ability);
        
        Assert.That(unit.HasChampion, Is.True);
    }
    
    [Test]
    public void TestWardSaveSystem()
    {
        var unit = new Unit();
        unit.WardSave = 4;
        
        // Simulate damage
        int originalWounds = unit.Wounds;
        unit.TakeDamage(2);
        
        // Ward save should prevent some damage
        Assert.That(unit.Wounds, Is.GreaterThan(originalWounds - 2));
    }
    
    [Test]
    public void TestAbilityCooldown()
    {
        var ability = new UnitAbility
        {
            Name = "Cooldown Ability",
            Type = AbilityType.Active,
            Cooldown = 1,
            ActivationPhases = new List<GameManager.TurnPhase> { GameManager.TurnPhase.Hero }
        };
        
        var unit = new Unit();
        unit.AddUnitAbility(ability);
        
        // First activation should work
        bool firstActivation = unit.ActivateUnitAbility("Cooldown Ability");
        Assert.That(firstActivation, Is.True);
        
        // Second activation in same turn should fail
        bool secondActivation = unit.ActivateUnitAbility("Cooldown Ability");
        Assert.That(secondActivation, Is.False);
    }
    
    [Test]
    public void TestFactionSpecificUnits()
    {
        UnitDatabase.Initialize();
        
        var stormcastUnits = UnitDatabase.GetUnitsByFaction("Stormcast Eternals");
        Assert.That(stormcastUnits.Count, Is.GreaterThan(0));
        
        var orrukUnits = UnitDatabase.GetUnitsByFaction("Orruk Warclans");
        Assert.That(orrukUnits.Count, Is.GreaterThan(0));
        
        // Factions should be different
        Assert.That(stormcastUnits, Is.Not.EquivalentTo(orrukUnits));
    }
    #endregion
}
}
