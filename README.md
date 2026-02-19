# Jamison Gaming's Jump Quest!

A 3D obby-style platformer for iPad. Kids (ages 4-10) navigate obstacle courses across themed worlds, collecting coins, unlocking power-ups, and progressing through a skill tree.

## What's Built (MVP)

### Core Gameplay Loop
- 3D platformer with joystick + Run + Jump controls
- Levels loaded from JSON data files
- Coins, checkpoints, hazards, moving platforms, rotating obstacles, jump pads
- Finish goal triggers level completion
- Fall reset to last checkpoint

### Screens
- **Main Menu** - Play, Cosmetics, Skills, Dev: Level Wizard
- **World Select** - 5 worlds (Mountains unlocked, others level-gated), 3 levels per world
- **Gameplay** - Full HUD with coin count, timer, pause menu
- **Results** - Time, coins, time bonus, XP, currency breakdown
- **Skill Tree** - 8 nodes across 3 power-up tracks + utility
- **Cosmetics** - Skins, hats, trails (placeholder system with buy/equip)
- **Level Draft Wizard** - Dev tool: answer 5 questions, generates a playable level

### Progression
- XP per level completion (scaled by difficulty and performance)
- Leveling unlocks skill tree nodes and worlds
- Currency = coins collected + time bonus
- Skill tree unlocks: Double Jump, Speed Burst, Shield

### Power-ups (unlocked via skill tree)
1. **Feather Step** (Double Jump) - second jump in air
2. **Swift Current** (Speed Burst) - sprint boost with cooldown
3. **Deepguard** (Shield) - absorbs one hazard hit

### Level Data
- JSON-based level definitions in `StreamingAssets/Levels/`
- 3 sample Mountains levels with increasing difficulty
- Supports: static platforms, moving platforms, rotating obstacles, jump pads, coins, checkpoints, hazards, finish goals

## How to Set Up

### Prerequisites
- Unity 2022.3 LTS (or any recent LTS)
- Xcode 15+ (for iOS build)
- macOS (for iOS build) or Windows (for editor testing)

### Steps

1. **Open Unity Hub**, click "Open" and select the `JumpQuest` folder.

2. **Wait for import** - Unity will import all scripts and set up the project.

3. **Run project setup**: In Unity's top menu bar, click:
   ```
   JumpQuest > Setup Project (Run Once)
   ```
   This creates all 7 scenes, configures build settings, and sets up tags.

4. **Open the MainMenu scene**:
   ```
   File > Open Scene > Assets/Scenes/MainMenu.unity
   ```

5. **Press Play** - You should see the main menu. Click "PLAY" to enter World Select, then pick a level.

### Controls (Editor Testing)
- **WASD / Arrow Keys** - Move
- **Space** - Jump
- **Left Shift** - Run/Sprint
- **Escape** - Pause

### Controls (iPad)
- **Left joystick** - Move direction
- **JUMP button** (right side) - Jump
- **RUN button** (right side) - Hold to sprint
- **BOOST / SHIELD** (right side, if unlocked) - Activate power-ups

## How to Build for iOS

1. In Unity: `File > Build Settings`
2. Select **iOS** platform, click "Switch Platform"
3. Set these Player Settings (`Edit > Project Settings > Player`):
   - Company Name: Your name
   - Product Name: Jump Quest
   - Bundle Identifier: `com.yourname.jumpquest`
   - Target minimum iOS version: 16.0
   - Target SDK: Device SDK
   - Supported orientations: Landscape Left + Landscape Right
4. Click **Build** and choose an output folder
5. Open the generated `.xcodeproj` in Xcode
6. Select your signing team
7. Connect your iPad and click Run

## Project Structure

```
JumpQuest/
  Assets/
    Editor/
      ProjectSetup.cs          # One-click scene/build setup + level validator
    Scripts/
      Core/
        GameManager.cs          # Singleton, owns game state and scene flow
        PlayerProgressData.cs   # XP, level, currency, unlocks (serializable)
        SaveManager.cs          # JSON save/load to persistent data
        ScoringConfig.cs        # ScriptableObject for tuning rewards
        BootstrapScene.cs       # Per-scene initializer
      Gameplay/
        PlayerController.cs     # Movement, jump, coyote time, power-ups
        CameraController.cs     # Third-person follow cam
        GameplaySceneController.cs # Level load + session timer
        Coin.cs                 # Collectible coin
        Checkpoint.cs           # Respawn point
        FinishGoal.cs           # Level end trigger
        MovingPlatform.cs       # Platform with waypoint path
        RotatingObstacle.cs     # Spinning hazard
        JumpPad.cs              # Launch pad
      Data/
        LevelData.cs            # JSON schema classes
        LevelLoader.cs          # Reads JSON, spawns level geometry
      UI/
        Controls/
          VirtualJoystick.cs    # Touch joystick
          TouchInputBridge.cs   # Connects touch UI to PlayerController
        HUD/
          GameHUD.cs            # Coin count, timer, pause
          HUDBuilder.cs         # Runtime UI construction
        Menus/
          MainMenuUI.cs         # Main menu
          WorldSelectUI.cs      # World + level selection
          ResultsUI.cs          # End-of-level results
      Progression/
        SkillTreeUI.cs          # Skill tree with buy/unlock
        CosmeticsUI.cs          # Cosmetics shop
      PowerUps/                 # (Integrated into PlayerController)
      LevelBuilder/
        LevelDraftWizard.cs     # Dev tool: question wizard -> JSON -> play
      Audio/
        AudioManager.cs         # SFX/music singleton (placeholder clips)
    StreamingAssets/
      Levels/
        Mountains/
          mountains_level_0.json  # Foothill Trail (Easy)
          mountains_level_1.json  # Rocky Ridge (Medium)
          mountains_level_2.json  # Summit Storm (Hard)
    Scenes/                     # Created by ProjectSetup
    Resources/
      Configs/                  # ScoringConfig ScriptableObject
    Prefabs/                    # Empty (primitives built at runtime)
    Materials/                  # Empty (materials built at runtime)
```

## Level JSON Schema

```json
{
    "worldId": "mountains",
    "levelName": "Level Name",
    "difficulty": 1-10,
    "parTime": 60,
    "setting": "foothills",
    "finishType": "flag|portal|chest",
    "playerSpawn": { "x": 0, "y": 2, "z": 0 },
    "platforms": [{ "x": 0, "y": 0, "z": 0, "sx": 4, "sy": 0.5, "sz": 4, "color": "stone" }],
    "movingPlatforms": [{ "x": 0, "y": 0, "z": 0, "sx": 3, "sy": 0.5, "sz": 3, "waypoints": [{"x":0,"y":0,"z":0},{"x":5,"y":0,"z":0}], "speed": 3, "loop": true }],
    "rotatingObstacles": [{ "x": 0, "y": 5, "z": 10, "sx": 6, "sy": 0.4, "sz": 0.4, "ax": 0, "ay": 1, "az": 0, "speed": 90 }],
    "jumpPads": [{ "x": 0, "y": 0.3, "z": 5, "force": 18 }],
    "coins": [{ "x": 0, "y": 2, "z": 5, "value": 1 }],
    "checkpoints": [{ "x": 0, "y": 1, "z": 20, "index": 0 }],
    "hazards": [{ "x": 0, "y": 0, "z": 15, "sx": 3, "sy": 0.3, "sz": 1, "type": "spike|lava" }],
    "finish": { "x": 0, "y": 1, "z": 50 }
}
```

## Validate Level Data

In Unity menu: `JumpQuest > Validate Level Data`

Checks all JSON files in StreamingAssets/Levels for schema compliance.

## Adding New Levels

### Option 1: Manual JSON
Create a new file following the schema above. Place it in `StreamingAssets/Levels/[WorldName]/[worldid]_level_[index].json`.

### Option 2: Level Draft Wizard
From the main menu, click "DEV: LEVEL WIZARD". Answer 5 questions and the wizard generates a procedural level JSON and can immediately play-test it. Generated files are saved to the app's persistent data path.

## Assumptions Made
- Unity 2022.3 LTS, Built-in Render Pipeline (lightest weight for iPad)
- Landscape orientation only for gameplay
- All UI built programmatically at runtime (no prefab dependencies)
- Placeholder art using Unity primitives (capsule player, cube platforms, cylinder coins)
- Local save only (no cloud/accounts)
- No real purchases implemented; store is cosmetic-only with in-game currency
- Single character model (capsule) with color variants for cosmetics
- Audio clips are null placeholders (assign real clips to AudioManager fields)

## Next Expansions
1. **Real 3D models** - Replace primitives with low-poly meshes
2. **Particle effects** - Coin pickup sparkle, speed burst trail, shield bubble
3. **Sound design** - Add jump, coin, finish, and ambient music clips
4. **More worlds** - Jungle, Space, Candy, Pirate (add JSON levels + world colors)
5. **Parent gate** - Settings screen with math-problem gate for store access
6. **Microtransaction stubs** - Premium currency, cosmetic bundles (IAP hooks)
7. **Auto-run toggle** - Accessibility option for younger kids
8. **Leaderboards** - Local best times, optional Game Center
9. **Map editor v2** - In-game drag-and-drop level builder
10. **Boss hazards** - Chase boulder, avalanche timer, collapsing bridge sequences
