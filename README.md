# Tower Defense

Unity project using Unity **6000.6.0f1** and the Universal Render Pipeline (URP).
The current map is **Hidden Leaf Forest Road** in `Assets/Scenes/Main.unity`.
It contains a primitive road, six enemy waypoints, a spawn marker, a gate
placeholder, and six tower pads. Magenta enemies follow the waypoints at
4 units per second toward the gate. Select `Assets/Prefabs/Enemy.prefab` to change their
movement speed in the Inspector. Each starts with 3 health; `EnemyHealth.TakeDamage`
subtracts health and removes the enemy at zero.

All six pads start empty. In Play Mode, left-click an empty pad to place Tenten.
Each pad accepts one tower; clicking occupied pads, road, or ground does nothing.
Place Tenten before clicking **Start Wave**. Forest Road has **three waves** of
**3, 5, and 7 enemies**, each spawning two seconds apart by default. Select
`Round Controller` in the scene to configure each wave's enemy count and spawn
interval in the Inspector. The HUD shows **Wave 1 of 3** and advances as waves clear.
Each wave waits for a manual Start Wave click; repeated clicks cannot overlap
waves. Place more towers between waves using the preserved towers and Ryo.
Each round starts with **100 Ryo**; deploying Tenten costs **50 Ryo**.
The HUD shows both the current balance and deployment cost. Try towers on Pads
2 and 3 to win with the starting budget. Each defeated enemy awards **20 Ryo**
once during the wave. Gate arrivals award nothing. Unaffordable placements leave
the pad empty and show **Not enough Ryo.** Occupied-pad, road, and ground clicks
spend nothing.
Each wave clears only after all its enemies have spawned and been defeated.
**Victory** appears only after Wave 3 clears;
any enemy reaching the gate immediately causes **Defeat**. The result stays fixed
and the round stops. **Restart** removes towers, enemies, and kunai, clears the
pads, restores 100 Ryo, and returns to Wave 1 so you can try again.
Select `Assets/Prefabs/TentenTower.prefab` to tune attack range (5.5 units), time between
shots (0.9 seconds), and projectile speed (10 units/second). Each physics hit deals
1 physical damage; three hits remove the enemy. Missed kunai expire after 3 seconds.
Tower selection is not implemented yet. The original
starter `SampleScene` is also included.

## Open and run

1. Install Unity Hub and Unity Editor **6000.6.0f1**. The exact version is also
   recorded in `ProjectSettings/ProjectVersion.txt`.
2. Clone this repository, or download its ZIP from GitHub and extract it.
3. In Unity Hub, choose **Add project from disk** and select the folder containing
   `Assets`, `Packages`, and `ProjectSettings`.
4. Open the project. Allow Unity to download packages and finish importing assets
   and compiling scripts. Internet access is needed for the first package restore.
5. Open `Assets/Scenes/Main.unity` and press **Play** in the Editor.

Unity Hub may ask you to sign in and activate an appropriate Unity license.
Codex, Unity MCP, and the Unity CLI are not required to open or run the scene.
This repository contains the Unity source project, not a standalone executable.

## Files included in Git

Keep `Assets/` and its `.meta` files, `Packages/manifest.json`,
`Packages/packages-lock.json`, and `ProjectSettings/` together. They preserve asset
references, package versions, scene configuration, and the Editor version.

The standard Unity `.gitignore` excludes generated folders such as `Library/`,
`Temp/`, `Logs/`, and `UserSettings/`, plus generated IDE project files. Unity
recreates these locally when the project opens; they are not needed in a download.

Before committing, save changes in Unity and check the Console for compilation
errors. Review the files being committed so generated files stay out of Git.
