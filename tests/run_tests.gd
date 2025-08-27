extends Node

func _ready():
	print("Running Age of Sigmar Virtual Tabletop tests...")
	
	# Test GameManager
	test_game_manager()
	
	# Test DiceManager
	test_dice_manager()
	
	# Test Unit creation
	test_unit_creation()
	
	print("All tests completed!")
	get_tree().quit()

func test_game_manager():
	print("Testing GameManager...")
	
	# Test board dimensions
	var board_width = GameManager.Instance.ConvertInchesToUnits(44.0)
	var board_height = GameManager.Instance.ConvertInchesToUnits(60.0)
	
	assert(board_width == 44.0, "Board width conversion failed")
	assert(board_height == 60.0, "Board height conversion failed")
	
	print("GameManager tests passed!")

func test_dice_manager():
	print("Testing DiceManager...")
	
	# Test basic dice rolling
	var roll = DiceManager.RollD6()
	assert(roll >= 1 and roll <= 6, "D6 roll out of range")
	
	# Test multiple dice
	var rolls = DiceManager.RollMultipleDice(5)
	assert(rolls.size() == 5, "Multiple dice roll count incorrect")
	
	print("DiceManager tests passed!")

func test_unit_creation():
	print("Testing Unit creation...")
	
	# Create a test unit
	var unit = Unit.new()
	unit.UnitName = "Test Warrior"
	unit.PlayerId = 1
	unit.Move = 6
	unit.Wounds = 1
	unit.MaxWounds = 1
	unit.Bravery = 6
	unit.Save = 4
	unit.Attacks = 1
	unit.ToHit = 4
	unit.ToWound = 4
	unit.Rend = 0
	unit.Damage = 1
	
	assert(unit.UnitName == "Test Warrior", "Unit name not set correctly")
	assert(unit.PlayerId == 1, "Unit player ID not set correctly")
	assert(unit.CanMove(), "Unit should be able to move initially")
	
	# Test unit movement
	unit.HasMoved = true
	assert(!unit.CanMove(), "Unit should not be able to move after moving")
	
	unit.QueueFree()
	print("Unit creation tests passed!")
