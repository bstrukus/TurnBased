# Tactics Game – Scene Setup

## 1. Create a new 3D scene
File → New Scene → Basic (Built-in). Save it as `TacticsGame/Scenes/Battle.unity`.

## 2. Create the manager objects
Create five empty GameObjects (right-click Hierarchy → Create Empty):

| GameObject name  | Add Component(s)                          |
|------------------|-------------------------------------------|
| `GameManager`    | `Tactics/GameManager`                     |
| `GridSystem`     | `Tactics/GridSystem`                      |
| `TurnManager`    | `Tactics/TurnManager`                     |
| `ActionMenuUI`   | `Tactics/ActionMenuUI` (also add Canvas)  |
| `CameraRig`      | *(leave empty – managed by CameraController)* |

## 3. Set up the Camera
- Select the **Main Camera**.
- Add component `Tactics/CameraController`.
- Position doesn't matter – the script will place it automatically.

## 4. Wire references on GameManager
Select `GameManager` and fill the Inspector fields:
- **Grid System** → drag `GridSystem`
- **Turn Manager** → drag `TurnManager`
- **Action Menu** → drag `ActionMenuUI`
- **Camera Controller** → drag `Main Camera`
- **Unit Prefab** → leave empty (script creates capsules automatically)

## 5. Define your units
Still on `GameManager`, expand **Team 0 Units** and **Team 1 Units**.

Each entry has:
| Field    | Meaning                      | Suggested defaults      |
|----------|------------------------------|-------------------------|
| unitName | Display name                 | "Knight", "Mage", …     |
| maxHP    | Hit points                   | 100                     |
| speed    | CT gained per tick           | 5–8                     |
| move     | Tiles per turn               | 3–5                     |
| jump     | Max height diff per step     | 2                       |
| attack   | Base damage                  | 20                      |
| defense  | Damage reduction             | 10                      |
| attackRange | 1 = melee, 2–3 = ranged | 1                       |
| itemCount | How many items the unit has | 2                       |
| startX / startZ | Grid coordinates to spawn on | spread them out |

Example – two player units, two enemies (4 entries total):
- Team 0: startX=2, startZ=2 and startX=3, startZ=3
- Team 1: startX=16, startZ=16 and startX=17, startZ=15

## 6. Physics layer (important for clicking)
The script uses `OnMouseDown` which requires a Physics Raycaster:
- Select **Main Camera** → Add Component → **Physics Raycaster**.

## 7. Controls summary

| Key / Click      | Action                                  |
|------------------|-----------------------------------------|
| Click cell       | Confirm move / attack / item target     |
| Escape           | Cancel target selection, back to menu   |
| Q / E            | Rotate camera 90° (Orbit mode)          |
| Tab              | Toggle Orbit ↔ Free camera              |
| WASD             | Pan (Free mode only)                    |
| Right-mouse drag | Look around (Free mode only)            |
| Scroll wheel     | Zoom / dolly                            |

## 8. Play
Hit Play. Blue capsules = Team 0 (player), red = Team 1 (AI).
- Team 0 units show the action menu at the bottom of the screen.
- Team 1 units run a simple AI (attacks if in range, otherwise advances).
- Console logs show damage numbers and turn events.
