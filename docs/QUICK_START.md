tio# Age of Sigmar 4th Edition Virtual Tabletop - Quick Start Guide

## Getting Started

### Prerequisites

- **Godot 4.2+** with .NET 6.0 SDK
- **Git** for version control
- **GitHub account** for source control

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/AoSGame.git
   cd AoSGame
   ```

2. Open the project in Godot:

   - Launch Godot 4.2+
   - Click "Import" and select the `project.godot` file
   - Click "Import & Edit"

3. Run the game:
   - Press F5 or click the "Play" button
   - The game will start with the main menu

## Basic Controls

### Camera Controls

- **WASD**: Move camera
- **Mouse Wheel**: Zoom in/out
- **Middle Mouse**: Pan camera
- **C**: Center camera on selected unit

### Game Controls

- **Left Click**: Select unit, move unit
- **Right Click**: Measure distance
- **Space**: Next turn phase
- **R**: Roll dice
- **M**: Toggle measurement mode
- **Escape**: Deselect unit

### UI Controls

- **Next Phase Button**: Advance to next turn phase
- **Unit Info Panel**: View unit statistics and actions
- **Dice Results**: View dice roll results

## Game Flow

### 1. Main Menu

- **Single Player**: Start a game against AI (coming soon)
- **Host Multiplayer**: Create a multiplayer game
- **Join Multiplayer**: Connect to an existing game

### 2. Game Setup

- **Army Building**: Select units and build your army
- **Terrain Placement**: Place terrain pieces on the board
- **Unit Deployment**: Position your units in deployment zones

### 3. Turn Structure (AoS 4th Edition)

1. **Hero Phase**

   - Cast spells
   - Use command abilities
   - Prayers
   - Rally attempts

2. **Movement Phase**

   - Move units up to their Move characteristic
   - Retreat from combat
   - Run (extra D6 movement)

3. **Shooting Phase**

   - Ranged attacks
   - Cannot shoot if engaged in combat

4. **Charge Phase**

   - Declare charges
   - Roll 2D6 for charge distance
   - Move into engagement range

5. **Combat Phase**
   - Melee attacks
   - Hit/Wound/Save sequence
   - Damage allocation

## Sample Game

### Creating Units

The game comes with sample units for testing:

- **Liberator Prime** (Stormcast Eternals): Hero unit with 2 wounds
- **Stormcast Warrior** (Stormcast Eternals): Basic infantry
- **Orruk Brute** (Orruk Warclans): Heavy infantry with rend
- **Orruk Warrior** (Orruk Warclans): Basic infantry

### Basic Combat

1. **Select a unit** by clicking on it
2. **Move to enemy** during Movement Phase
3. **Charge** during Charge Phase (if in range)
4. **Attack** during Combat Phase
5. **Roll dice** for hit, wound, and save rolls

### Terrain Effects

- **Forests**: Provide cover (+1 to save), difficult terrain
- **Hills**: Elevated positions
- **Ruins**: Provide cover, difficult terrain
- **Water**: Dangerous terrain, movement penalty
- **Objectives**: Victory point locations

## Army Building

### Point System

- **1000 points** standard game size
- **Minimum 80%** of point limit required
- **Maximum 100%** of point limit allowed

### Army Composition Rules

- **At least 2 Battleline units** required
- **Leaders cannot exceed 1/4** of total units
- **Faction restrictions** apply

### Available Factions

- **Stormcast Eternals**: Order faction
- **Orruk Warclans**: Destruction faction
- More factions coming soon

## Multiplayer

### Hosting a Game

1. Click "Host Multiplayer Game" from main menu
2. Wait for opponent to connect
3. Both players build armies
4. Start the game when ready

### Joining a Game

1. Click "Join Multiplayer Game" from main menu
2. Enter the host's IP address
3. Build your army
4. Wait for host to start the game

### Network Features

- **Real-time synchronization** of unit positions
- **Dice roll synchronization** across all players
- **Turn phase management** with host authority
- **Chat system** (coming soon)

## Tips and Tricks

### Movement

- **Measure distances** before moving units
- **Plan charges** carefully - failed charges prevent movement
- **Use terrain** for cover and strategic positioning

### Combat

- **Position units** to maximize attacks
- **Consider rend** when choosing targets
- **Use command abilities** to enhance combat

### Strategy

- **Control objectives** for victory points
- **Protect your heroes** - they provide command abilities
- **Use terrain** to your advantage
- **Plan your turn** before starting

## Troubleshooting

### Common Issues

- **Game won't start**: Check Godot version (4.2+ required)
- **Units not moving**: Ensure you're in Movement Phase
- **Cannot attack**: Check if unit is engaged and in Combat Phase
- **Network issues**: Check firewall settings and IP address

### Performance

- **Low FPS**: Reduce graphics settings in Godot
- **Network lag**: Check internet connection
- **Memory issues**: Close other applications

## Next Steps

### Planned Features

- **AI opponents** for single player
- **More factions** and units
- **Campaign system** with persistent armies
- **Tournament support** with Swiss pairings
- **3D model import** for custom units
- **Modding support** for custom rules

### Contributing

- **Report bugs** on GitHub Issues
- **Suggest features** in Discussions
- **Submit pull requests** for improvements
- **Help with documentation**

## Support

### Resources

- **GitHub Repository**: [Link to repo]
- **Discord Server**: [Link to Discord]
- **Documentation**: [Link to docs]
- **Bug Reports**: [Link to issues]

### Community

- **Join discussions** on GitHub
- **Share strategies** with other players
- **Help new players** learn the game
- **Contribute to development**

---

**Happy gaming! May Sigmar guide your armies to victory!**
