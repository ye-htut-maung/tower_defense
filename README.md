# Hidden Leaf Defense

Defend the Hidden Leaf Gate from seven waves of invading ninjas in this 3D tower defense game for CSCI 381. Spend Ryo on Tenten and Sasuke towers, and use placement and upgrades to stop every enemy before it reaches the gate.

## How to Play

- **Place towers:** Left-click one of the six empty stone pads, choose Tenten or Sasuke, check the range preview, then press **Deploy**. Each pad holds one tower.
- **Manage Ryo:** Start with **100 Ryo**. Tenten costs **50 Ryo**, Sasuke costs **60 Ryo**, and each defeated enemy earns **15 Ryo**.
- **Upgrade or sell:** Select an occupied pad to upgrade Genin Tenten to Chunin for **60 Ryo**, or sell either tower for a partial refund. Confirm the action or cancel without spending. Tower management remains available during waves.
- **Start waves:** Press **Start Wave** when ready. Each of the seven waves starts manually, giving you time to adjust your defenses between waves.
- **Win or restart:** Defeat every enemy in all seven waves to win. Any enemy reaching the gate causes Defeat. **Restart** returns to Wave 1 with empty pads and 100 Ryo.

## Features

- **Two ninja towers:** Tenten throws physical-damage kunai; Sasuke uses Fire Style: Fireball Jutsu.
- **Tower upgrades:** Chunin Tenten doubles her kunai damage and gains a blue visual accent.
- **Distinct enemies:** Slim regular ninjas and slower, tougher armored ninjas have visible damage-responsive health bars.
- **Strategic waves:** Increasing enemy counts, tighter spacing, and armored groups reward route coverage and well-timed purchases.
- **Presentation:** A forest road, textured Hidden Leaf Gate, background music, and distinct attack and round cues.

## Run in Unity

1. Clone or download this repository and add its project folder through Unity Hub.
2. Open with **Unity 6000.6.0f1**, as specified in `ProjectSettings/ProjectVersion.txt`. Allow package restoration and asset importing to finish.
3. Open `Assets/Scenes/Main.unity` and press **Play**.

This repository contains the Unity source project, not a standalone executable.

## Credits and Licenses

- **Character and gate art:** User-supplied Meshy AI exports for Tenten, Sasuke (the supplied “Young Shadow Ninja” model), and the Hidden Leaf Gate, with their accompanying textures. Tenten’s kunai is a supplied FBX model. No individual creator credits or model license files accompanied these exports.
- **Music:** “Calm Track (loopable version)” by **pmiller**, from [OpenGameArt](https://opengameart.org/content/calm-track), released under [CC0 1.0 Universal](https://creativecommons.org/publicdomain/zero/1.0/).
- **Sound effects:** Original synthesized kunai, fireball, wave-start, Victory, and Defeat sounds created for this project and dedicated under CC0.

See the [full audio credits](Assets/Audio/CREDITS.md) and [included CC0 license](Assets/Audio/LICENSE-CC0.txt).
