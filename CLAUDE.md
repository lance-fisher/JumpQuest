# Jamison Gaming's Jump Quest!

3D obby platformer for kids ages 4-10. Built for Jamison (Lance's son, age 6).

## Quick Ref
- **Unity**: Open in Unity 2022.3 LTS, run `JumpQuest > Setup Project` menu first
- **Web**: Open `web/index.html` in browser (standalone Three.js version)
- **Web launcher**: `web/launch-jumpquest.bat`
- **Desktop shortcut**: "Jamison Gaming's Jump Quest!" in Jamison's Games folder

## Architecture
- All UI built programmatically (no prefab dependencies)
- JSON level data in `Assets/StreamingAssets/Levels/`
- Singleton GameManager pattern
- Web version: Single HTML file with Three.js, same JSON levels, Xbox controller support

## Game Systems
- 5 themed worlds: Mountains, Jungle, Space, Candy, Pirate
- XP leveling, skill tree (8 nodes), cosmetics shop
- Level Draft Wizard for procedural level generation
- Checkpoints, coin collection, obstacle courses

## Controls
- **Xbox controller**: A=Jump, RT=Run, X=Boost, Y=Shield
- **Keyboard**: WASD + Space
- **Touch**: Virtual joystick (mobile/tablet)

## Key Files
- `Assets/Scripts/` — All C# game code (25+ scripts)
- `Assets/StreamingAssets/Levels/` — JSON level definitions
- `web/index.html` — Playable web version (~70KB)
- `README.md` — Full setup and gameplay guide
