# Age of Sigmar Virtual Tabletop

A digital recreation of the Warhammer Age of Sigmar tabletop wargame, built with Godot 4.

## 🎯 Features

### Core Game Systems

- **Game Management**: Turn-based gameplay with 5 phases (Hero, Movement, Shooting, Charge, Combat), Rally phase, player management, unit tracking
- **Multiplayer Networking**: Built-in Godot networking with lobby system, player synchronization, and real-time updates
- **Unit System**: Detailed Age of Sigmar 4th Edition unit stats, actions, combat logic, temporary effects, and base sizes in millimeters
- **Dice Management**: Comprehensive dice rolling system with AoS-specific rolls and physical dice simulation
- **Input Management**: Camera control, unit selection, movement, measurement, dice rolling, phase progression, command point tracker toggle, unit abilities toggle
- **Army Building**: Unit database, point system (2000 points), army composition rules, save/load functionality, regiments of renown support
- **Terrain System**: Various terrain types with effects (cover, difficult, dangerous, deadly), collision detection, and interactive placement
- **AI Opponent**: AI logic for all game phases, targeting, movement, and strategic decision making
- **Battleplan Management**: Battle scenarios, objectives, deployment zones, terrain layouts, victory conditions, and victory points
- **Command Point System**: Command point generation, spending, tracking, and comprehensive command ability database
- **Unit Abilities System**: Comprehensive unit and model abilities with effects, conditions, and activation phases
- **Radius Indicator System**: Visual radius indicators for buffs, debuffs, and auras with customizable templates and effects

### Radius Indicator System

The Radius Indicator System provides visual feedback for unit abilities that affect other units within specific ranges. This is perfect for abilities like the Megaboss's "+1 to charge rolls within 12\"" or the Nighthaunt's "-1 bravery within 3\"".

**Key Features:**

- **Visual Radius Indicators**: Transparent colored circles showing the range of unit abilities
- **Customizable Templates**: Different visual styles for buffs, debuffs, auras, and terrain-dependent effects
- **Dynamic Effects**: Pulsing animations, emission effects, and range text labels
- **Smart Categorization**: Automatic classification of indicators by type (buff, debuff, aura, terrain)
- **Interactive Management**: Toggle visibility, filter by type, and manage individual unit indicators
- **Real-time Updates**: Indicators automatically follow units and update positions
- **Range Validation**: Built-in methods to check which units are affected by specific abilities

**Indicator Types:**

- **Charge Buff** (Green): +1 to charge rolls, e.g., Megaboss Waaagh! aura
- **Save Buff** (Blue): +1 to save rolls, e.g., Freeguild General discipline
- **Bravery Buff** (Yellow): +1 to bravery, e.g., General leadership auras
- **Bravery Debuff** (Red): -1 to bravery, e.g., Nighthaunt spectral terror
- **Aura** (Orange): General magical effects, e.g., spell casting bonuses
- **Terrain Dependent** (Cyan): Effects that require terrain proximity

**Controls:**

- **R**: Toggle all radius indicators
- **B**: Toggle buff radius indicators
- **V**: Toggle debuff radius indicators
- **T**: Toggle terrain-dependent indicators
- **Y**: Toggle aura indicators

**Usage Examples:**

```csharp
// Create a charge buff indicator (e.g., Megaboss +1 to charge rolls within 12")
var chargeIndicator = radiusManager.CreateChargeBuffIndicator(megaboss, 12.0f);

// Create a bravery debuff indicator (e.g., Nighthaunt -1 bravery within 3")
var braveryIndicator = radiusManager.CreateBraveryDebuffIndicator(knightOfShrouds, 3.0f);

// Create a custom aura indicator
var customIndicator = radiusManager.CreateAuraIndicator(wizard, 18.0f, "+1 to spell casting");

// Check which units are affected by a buff
var affectedUnits = radiusManager.GetUnitsInBuffRange(sourceUnit, "Charge");
```

## Tech Stack

- **Engine**: Godot 4.2+
- **Language**: C# (with GDScript for scripting)
- **Version Control**: Git with GitHub
- **CI/CD**: GitHub Actions for testing and builds
- **3D Models**: GLTF/GLB format
- **Networking**: Godot's built-in networking

## Project Structure

```
AoSGame/
├── .github/                 # GitHub Actions workflows
├── assets/                  # Game assets
│   ├── models/             # 3D miniatures and terrain
│   ├── textures/           # Textures and materials
│   ├── sounds/             # Audio files
│   ├── ui/                 # UI elements and icons
│   └── data/               # Game data (armies, rules, etc.)
├── scenes/                 # Godot scene files
│   ├── main/               # Main game scenes
│   ├── ui/                 # User interface scenes
│   └── multiplayer/        # Multiplayer-specific scenes
├── scripts/                # C# and GDScript files
│   ├── core/               # Core game systems
│   ├── units/              # Unit behavior and stats
│   ├── ui/                 # UI controllers
│   └── networking/         # Multiplayer code
├── tests/                  # Unit and integration tests
├── docs/                   # Documentation
└── export/                 # Build outputs
```

## Getting Started

### Prerequisites

- Godot 4.2 or later
- .NET 6.0 SDK (for C# support)
- Git

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/AoSGame.git
   cd AoSGame
   ```

2. Open the project in Godot:

   - Launch Godot
   - Click "Import" or "Open"
   - Select the `project.godot` file

3. Install dependencies:
   ```bash
   # If using C# (optional)
   dotnet restore
   ```

### Development

1. **Running the Game**:

   - Press F5 in Godot or use the "Play" button
   - The game will launch in the editor

2. **Building**:

   - Use Godot's export system
   - Supported platforms: Windows, macOS, Linux, Web

3. **Testing**:
   ```bash
   # Run tests via GitHub Actions or locally
   godot --headless --script tests/run_tests.gd
   ```

## Game Mechanics

### Board System

- 44" x 60" virtual battlefield
- Grid-based movement with inch precision
- Terrain placement and effects
- Objective markers

### Unit System

- Individual model tracking
- Unit coherency rules
- Wound tracking
- Equipment and abilities

### Combat System

- Melee and ranged combat
- Dice rolling with physics
- Hit/wound/save sequence
- Damage calculation

### Turn Structure

- Hero Phase
- Movement Phase
- Shooting Phase
- Charge Phase
- Combat Phase
- Battleshock Phase

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/new-feature`
3. Commit your changes: `git commit -am 'Add new feature'`
4. Push to the branch: `git push origin feature/new-feature`
5. Submit a pull request

## License

This project is for educational and personal use. Warhammer Age of Sigmar is a trademark of Games Workshop Limited.

## Roadmap

- [ ] Basic board and camera system
- [ ] Unit placement and movement
- [ ] Dice rolling mechanics
- [ ] Combat system
- [ ] Army builder
- [ ] Multiplayer networking
- [ ] Rule validation engine
- [ ] Victory condition tracking
- [ ] Save/load game states
- [ ] AI opponent (basic)

## Support

For questions or issues, please open an issue on GitHub or contact the development team.
