# Age of Sigmar Virtual Tabletop - Game Mechanics

## Overview

This document describes the implementation of Age of Sigmar tabletop game mechanics in the virtual environment. The game aims to be a 1:1 recreation of the official tabletop rules.

## Core Game Systems

### 1. Turn Structure

The game follows the official Age of Sigmar turn structure:

1. **Hero Phase**

   - Cast spells
   - Use command abilities
   - Prayers
   - Heroic actions

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

6. **Combat Phase**
   - Melee attacks
   - Hit/Wound/Save sequence
   - Damage allocation

**Note: Age of Sigmar 4th Edition removed the Battleshock phase. Units no longer flee due to low bravery.**

### 2. Unit System

#### Unit Characteristics

- **Move**: Movement in inches per turn
- **Wounds**: Current/Maximum wounds
- **Bravery**: Battleshock resistance
- **Save**: Armor save characteristic
- **Attacks**: Number of attacks
- **To Hit**: Hit roll needed (usually 4+)
- **To Wound**: Wound roll needed (usually 4+)
- **Rend**: Armor penetration
- **Damage**: Damage per unsaved wound

#### Unit Types

- **Hero**: Can use command abilities
- **General**: Army leader
- **Wizard**: Can cast spells
- **Priest**: Can pray

### 3. Combat System

#### Attack Sequence

1. **Hit Roll**: Roll dice equal to attacks, compare to To Hit
2. **Wound Roll**: For each hit, roll to wound, compare to To Wound
3. **Save Roll**: Target rolls saves, modified by Rend
4. **Damage**: Apply damage for unsaved wounds

#### Combat Modifiers

- **Rend**: Reduces save characteristic
- **Rerolls**: Various abilities allow rerolls
- **Exploding Dice**: 6s generate extra hits
- **Mortal Wounds**: Bypass save rolls

### 4. Movement System

#### Movement Rules

- Units move up to their Move characteristic
- Must maintain unit coherency
- Cannot move through other units
- Retreat: Move away from enemy units
- Run: Add D6 to movement, cannot charge

#### Charge Rules

- Declare charge target
- Roll 2D6 for charge distance
- Must end within 0.5" of enemy
- Failed charges cannot move

### 5. Magic System

#### Spell Casting

- Wizards can cast spells in Hero Phase
- Roll 2D6, compare to casting value
- Opponent can attempt to dispel
- Spells have various effects and ranges

#### Prayer System

- Priests can pray in Hero Phase
- Roll D6, compare to prayer value
- Prayers provide buffs or debuffs

### 6. Command Abilities

#### Command Point System

- Generate command points each turn
- Spend on command abilities
- Abilities affect nearby units
- Examples: All-out Attack, All-out Defense

### 7. Rally Phase (AoS 4th Edition)

#### Rally Mechanics

- Units can attempt to rally in Hero Phase
- Roll D6, if result equals or exceeds Bravery, heal 1 wound
- Cannot rally if engaged in combat
- Replaces the old Battleshock phase

## Technical Implementation

### 1. Board System

#### Dimensions

- **Width**: 44 inches
- **Height**: 60 inches
- **Scale**: 1 Godot unit = 1 inch
- **Grid**: Optional grid overlay

#### Terrain

- Terrain pieces affect movement and combat
- Line of sight calculations
- Cover bonuses
- Difficult terrain penalties

### 2. Multiplayer System

#### Network Architecture

- Host-Client model
- ENet for reliable networking
- State synchronization
- Turn-based synchronization

#### Lobby System

- Player connection management
- Army selection
- Game setup
- Ready state tracking

### 3. Input System

#### Controls

- **Left Click**: Select unit, move unit
- **Right Click**: Measure distance
- **Middle Mouse**: Pan camera
- **Mouse Wheel**: Zoom camera
- **WASD**: Camera movement
- **Space**: Next turn phase
- **R**: Roll dice
- **M**: Toggle measurement mode

#### Unit Interaction

- Click to select
- Drag to move
- Right-click for context menu
- Hover for unit information

### 4. Dice System

#### Physical Dice

- 3D dice with physics
- Realistic rolling mechanics
- Automatic result detection
- Visual feedback

#### Digital Dice

- Instant rolling for speed
- Result logging
- Statistics tracking
- Custom dice types

## Army Building

### 1. Point System

- Units have point costs
- Army size limits
- Faction restrictions
- Enhancement limits

### 2. Unit Selection

- Core units
- Battleline units
- Leaders
- Artillery
- Behemoths

### 3. Enhancements

- Artefacts
- Command traits
- Spell lores
- Prayers

## Victory Conditions

### 1. Battleplans

- Various mission types
- Objective markers
- Victory point scoring
- Special rules

### 2. Win Conditions

- Destroy enemy army
- Control objectives
- Achieve mission goals
- Time limits

## Future Features

### 1. AI Opponent

- Basic AI for single player
- Difficulty levels
- Strategy implementation
- Learning algorithms

### 2. Campaign System

- Persistent armies
- Experience tracking
- Equipment upgrades
- Story progression

### 3. Tournament Support

- Swiss pairings
- Time controls
- Result tracking
- Rankings

### 4. Modding Support

- Custom units
- Custom rules
- Custom scenarios
- Asset import/export

## Performance Considerations

### 1. Optimization

- LOD system for models
- Occlusion culling
- Efficient networking
- Memory management

### 2. Scalability

- Large unit counts
- Multiple players
- Complex scenarios
- Cross-platform support

## Legal Considerations

### 1. Intellectual Property

- Games Workshop trademarks
- Fair use guidelines
- Educational purposes
- Non-commercial use

### 2. Licensing

- Godot license
- Asset licenses
- Third-party libraries
- Distribution rights
