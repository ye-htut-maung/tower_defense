# Tower Defense

Unity project using Unity **6000.6.0f1** and the Universal Render Pipeline (URP).
The current map is **Hidden Leaf Forest Road** in `Assets/Scenes/Main.unity`.
It contains a primitive road, six enemy waypoints, a spawn marker, a textured
Hidden Leaf Gate, and six tower pads. Regular enemies are slim dark-navy ninjas
with **3 HP**; tough enemies are broader armored ninjas with **6 HP**, rounded
burnt-orange shoulder plates, a charcoal outfit and a charcoal helmet with an orange crest.
The ninja is built from Unity primitives under the enemy prefab's `Ninja Visual`
child, with a headband, mask, scarf and boots. Only this model turns to face travel;
the gameplay root, collider, health and movement retain their original behavior.
Both types follow the same waypoints: regular enemies move at **4 units/second**
and tough enemies at **3 units/second**. Both award **20 Ryo** when defeated.
The tough prefab overrides the existing Enemy Movement **Movement Speed** setting;
regular enemies retain the base prefab's speed. `Assets/Prefabs/Enemy.prefab`
is the regular enemy; `Assets/Prefabs/ToughEnemy.prefab` is its tough variant.
`EnemyHealth.TakeDamage` subtracts health and removes the enemy at zero.

Enemies have small camera-facing health bars. Bars are hidden at full health;
Regular enemies show a two-thirds bar at 2 HP and a one-third bar at 1 HP. Tough
enemies use the same bar divided by their 6-HP maximum: three Chunin hits take
them through **6 -> 4 -> 2 -> defeated**, showing 4/6 then 2/6. Each bar
follows its enemy and is removed on defeat or Restart. Bars do not block clicks.
To fit a future model, adjust **Vertical Offset** on the **Enemy Health Bar**
component in `Assets/Prefabs/Enemy.prefab` (default: **1.4 world units** above the
ninja's origin; the armored tough variant uses **1.55** to clear its helmet). To prototype a future model,
replace primitives inside `Ninja Visual` while retaining its `EnemyVisualFacing`
component and gameplay-root reference. Keep gameplay collision on the root.

All six pads start empty. The on-screen placement hint reads:
**Click an empty pad, choose Tenten or Sasuke, then press Deploy.**
Before, during, or between waves, left-click a pad to
select it. The highlighted pad shows a shared tower panel and a map preview of
the selected tower's actual attack range. Use the **Tenten** and **Sasuke** buttons
on an empty pad to choose a tower before deploying.

The panel shows the chosen tower's name, attack, and deployment price.
**Deploy** places one tower and charges once; **Cancel** closes the
panel without spending anything. Deploy is disabled with **Not enough Ryo.**
when the balance is below the selected tower's price. Each pad accepts one tower.

| Genin tower | Deploy / sell | Attack | Shot interval | Projectile speed | Range |
| --- | --- | --- | --- | --- | --- |
| Tenten | 50 / 35 Ryo | Kunai: 1 physical damage | 0.9 seconds | 10 units/s | 5.5 |
| Sasuke | 60 / 42 Ryo | Fire Style: Fireball Jutsu, 2 fire damage | 1.6 seconds | 14 units/s | 5.5 |

Sasuke's textured Genin model is **0.5 world units taller** than
Genin Tenten (2.29 versus 1.79 units). His glowing fireball sphere has a short
trail and deals damage to one enemy once on 3D impact.

Both Tenten and Sasuke use **predictive aiming**: the tower solves for the earliest
reachable intercept using the enemy's current position, measured movement velocity,
the actual launch point, and its projectile speed. Kunai and fireballs then fly
**straight without homing**. A target that keeps its current velocity can be hit
on a straight stretch; a turn after launch can still make a shot miss. Towers wait
if no positive intercept within the projectile's lifetime is reachable. Both
projectile types keep their existing damage/speeds and expire after **3 seconds**
if they miss; physical impact still applies damage once.

On an occupied **Genin Tenten** pad, **Upgrade - 60 Ryo** opens a confirmation showing
the cost and the change from **1 to 2 physical damage per hit**. **Confirm Upgrade**
charges 60 Ryo once and upgrades that tower on its existing pad. Cancel spends
nothing; the upgrade is disabled with **Not enough Ryo.** below 60 Ryo.
Chunin Tenten has a blue rank disc beneath her feet and is marked **Highest tier** in the panel.
Her range and fire rate stay the same. Only her newly fired kunai deal 2 damage;
other Genin towers and already airborne kunai retain their original damage.
Chunin's first hit leaves a fresh 3-HP enemy at 1 HP (one-third bar); the second
defeats it. Upgrading is available before, during, and between waves when funds permit.
Sasuke has no upgrade action in this step.

On any occupied pad, choose **Sell for 35 Ryo** for Tenten or **Sell for 42 Ryo**
for Sasuke, then **Confirm Sale** to remove the tower and receive the refund once
(70% of deployment cost). Canceling keeps the tower and balance.
The freed pad can be used again when you can afford deployment. UI clicks do not
select pads behind the panel. Waves keep running while the panel is open.
Enemy rewards immediately refresh Deploy/Upgrade affordability without requiring
reselection or dismissing a pending confirmation. Selling refunds the shown amount once
and frees the pad immediately, including during combat. Victory and Defeat close
the panel, clear the range preview, and block all tower changes. Restart also
clears selection, highlighting, and pending sale/upgrade confirmations.

Place Tenten before clicking **Start Wave**. Forest Road has **three waves**.
Here, **R** means regular (3 HP) and **T** means tough (6 HP):

| Wave | Spawn order | Interval |
| --- | --- | --- |
| 1 | R, R, R | 2.0 seconds |
| 2 | R, R, T, R, R | 1.6 seconds |
| 3 | R, R, T, R, R, T, R | 1.2 seconds |

Select `Round Controller` to edit each wave's **Spawn Order** dropdown list and
**Spawn Interval** in the Inspector. List length determines enemy count. The HUD
shows **Wave 1 of 3** and advances as waves clear.
Each wave waits for a manual Start Wave click; repeated clicks cannot overlap
waves. Place, sell, or upgrade towers during and between waves using your Ryo.
Each round starts with **100 Ryo**. The HUD shows the balance and both deployment
prices. Try Tenten towers on Pads
2 and 3 to win with the starting budget. Each defeated enemy awards **20 Ryo**
once during the wave. Gate arrivals award nothing. Selecting pads, clicking road
or ground, and canceling panel actions spend nothing.
Each wave clears only after all its enemies have spawned and been defeated.
**Victory** appears only after Wave 3 clears;
any enemy reaching the gate immediately causes **Defeat**. The result stays fixed
and the round stops. **Restart** removes towers, enemies, and projectiles, clears the
pads, clears all upgrades, restores 100 Ryo, and returns to Wave 1 so you can try again.
Select `Assets/Prefabs/TentenTower.prefab` to tune attack range (5.5 units), time between
shots (0.9 seconds), and projectile speed (10 units/second). Each physics hit deals
1 physical damage for Genin or 2 for Chunin. Sasuke's settings and fireball reference
are on `Assets/Prefabs/SasukeTower.prefab`; the projectile is `Assets/Prefabs/Fireball.prefab`.
The original starter `SampleScene` is also included.

Verified Tenten-only winning sequence using the starting budget and normal rewards:

1. Before Wave 1, deploy Genin on **Pads 2 and 3** (100 -> 0 Ryo), then Start Wave.
   Clearing it leaves **60 Ryo**.
2. Before Wave 2, upgrade **Pad 3** to Chunin and confirm (60 -> 0), then Start
   Wave. Clearing it leaves **100 Ryo**.
3. Before Wave 3, upgrade **Pad 2** to Chunin and confirm (100 -> 40), then Start
   Wave. Leave both Chunin in place; this run won with **180 Ryo** remaining.

Step 17 replayed this complete strategy in Unity Play Mode on predictive-aiming
commit `cc2e09c1`, before changing the hint. All three waves cleared, ending in
**Victory with 180 Ryo** and zero Console errors or warnings. Gameplay stats and
aiming were unchanged. After updating the hint, Game view at 1280x720 showed the
complete sentence without clipping. Mouse-input checks deployed Tenten on Pad 3
for 50 Ryo, then, after Restart, Sasuke on Pad 3 for 60 Ryo. The final Console
had zero errors and one Unity AI Toolkit Account API timeout warning. Play Mode
was exited and Main saved with empty pads.

Step 18 verified the distinct speeds in Main using the actual enemy prefabs and
six waypoints: both completed all four turns and reached the gate. The 66-unit
route took approximately **16.50 seconds** for regular enemies and **22.00 seconds**
for tough enemies. Separate straight-stretch physics checks confirmed Tenten's
kunai and Sasuke's fireball each intercepted both moving types, dealing 1 physical
and 2 fire damage respectively, once per projectile. The same Pads 2/3 strategy
above cleared all three waves again: **Victory with 180 Ryo**. Wave composition,
intervals, tower stats, HP, and predictive aiming were unchanged. Console: zero
errors and two internal Unity JobTempAlloc warnings before the winning run
(allocation lifespan exceeded four frames, followed by a diagnostic-launch
suggestion); no new messages appeared during the winning run. Play Mode was
exited, temporary test observers removed, and Main saved with empty pads.

Verified mixed Wave 1: deploy Tenten on **Pad 3** (100 -> 50 Ryo), select empty
**Pad 2**, choose **Sasuke**, then Start Wave with that panel open. After Tenten's
first defeat reward raises the balance to 70, Deploy becomes enabled. Deploy
Sasuke for 60 (10 left). The remaining enemies were defeated with both towers;
Wave 1 cleared with **50 Ryo**. This also checks deployment during an active wave.

## Imported tower and kunai models

User-supplied Meshy AI character exports now replace both primitive tower visuals.
The Tenten export is organized as `Assets/Art/TentenGenin/Source/Tenten_Genin.fbx`;
the supplied Young Shadow Ninja export is Sasuke at
`Assets/Art/SasukeGenin/Source/Sasuke_Genin.fbx`. Each has matching `_BaseColor`,
`_Normal`, `_Metallic`, `_Roughness`, and `_Emission` PNGs. URP Lit materials live
beside the Source folders and reference their own character's base color, normal,
metallic and emission maps. Normal maps use Normal Map import; metallic/roughness
use linear color space. Roughness sources are retained; smoothness is 0.25.
No individual creator or license file accompanied these supplied exports.

`Assets/Art/TentenKunai/Source/Tenten_Kunai.fbx` replaces the projectile's visual
children. No separate kunai textures were supplied, so it uses a URP steel material.
The source blade's +Y tip is rotated to projectile +Z, with a total length of 0.85
units. Existing Rigidbody, trigger collider, impact damage and three-second
lifetime are unchanged. Imported triangle counts: Tenten **84,084**, Sasuke
**121,766**, kunai **640**. No mesh reduction or source-file changes were made.

The original unimported exports were also renamed locally to `TentenGenin/`,
`SasukeGenin/`, and `Tenten_Kunai.fbx` at the project root, with matching clear
texture filenames; these local copies are not required by Unity. The imported
sources under `Assets/Art/` preserve all source bytes. Unity metadata accompanies
all imported files. The tower prefab roots retain their firing settings and
launch points; only visual children and Tenten's upgrade-accent reference changed.
Tenten is 1.79 units tall; Sasuke is 2.29. Their feet sit on the pad surface.
Tenten's original upgrade logic changes a red rank disc to blue while preserving
the textured model. Neither imported character adds colliders.

Play Mode checks passed with both towers deployed through the normal pad/panel
controls and normal Ryo. Tenten's kunai and Sasuke's fireball each hit moving
regular and tough enemies once for 1 physical / 2 fire damage. The mixed Wave 1
cleared with 50 Ryo; an earned Wave 2 reward enabled the 60-Ryo Chunin upgrade,
which left 10 Ryo and changed the rank disc to blue. Wave 2 then cleared with
90 Ryo. Inspected Game view for scale, facing, materials and the upgrade accent.
Final Console: **0 errors, 0 warnings**. Main was saved with empty pads and Play
Mode exited.

Follow-up full three-wave Play Mode test also passed using only Tenten on Pad 3
and Sasuke on Pad 2: **Victory, 230 Ryo**. Deploy Tenten before Wave 1, deploy
Sasuke after the first kill enables her 60-Ryo cost, then upgrade Tenten during
Wave 2 after its first kill enables the 60-Ryo upgrade. Wave 1 cleared with 50
Ryo, Wave 2 with 90, and all seven Wave 3 enemies were defeated. The blue rank
accent was readable from the gameplay camera; the kunai blade axis matched its
straight flight direction, and both physical and fire impacts were observed.
Console during this run: **0 errors, 0 warnings**. FBXs, textures, materials,
prefab references and matching metadata were checked for submission.

![Imported Genin Tenten and Sasuke deployed](Documentation/Screenshots/imported-genin-towers.png)
![Chunin Tenten's blue rank accent and Sasuke](Documentation/Screenshots/imported-chunin-tenten-sasuke.png)

## Sasuke fireball polish (Step 22)

Fireballs now have a bright yellow-white core, an orange halo, a short hot trail
and drifting ember particles. A confirmed enemy impact creates a brief flash
with 16 sparks; the burst fades and removes itself automatically. Effects add
no colliders or damage. The existing 0.35-radius trigger, 14-unit/second speed,
2 fire damage, firing interval, range and straight predictive aiming are unchanged.

Play Mode checks from the overhead camera confirmed visible flight and impact
effects. Single shots took a moving tough enemy from 6 to 4 HP and a moving
regular enemy with overlapping colliders from 3 to 1 HP. Each shot caused one
burst and one damage application; health stayed unchanged afterward, and each
burst was gone within 0.55 seconds. A normal mixed Tenten/Sasuke Wave 1 cleared
with 50 Ryo, with both physical and fire impacts observed. Console during these
checks: **0 errors, 0 warnings**. Full three-wave play was not repeated for this
visual polish.

![Fireball core and short ember trail](Documentation/Screenshots/fireball-flight.png)
![Quick burst at a confirmed enemy impact](Documentation/Screenshots/fireball-impact.png)

## Shared enemy styling (Step 23)

Both enemy prefabs now share rounded garment shapes, steel headbands, warm skin
details, linen forearm wraps and sandals, using URP Lit materials in
`Assets/Art/EnemyNinjas/`. This palette and their compact proportions sit alongside
the imported Tenten and Sasuke models. The regular ninja is slim and dark navy
with a muted burgundy sash. The tough ninja has a wider charcoal torso, rounded
burnt-orange shoulder armor with small steel rivets, and a helmet with an orange
crest. These changes reuse the existing `Ninja Visual` children and facing logic.

Only visual children changed. Root colliders, transforms, movement, health,
20-Ryo rewards and wave configuration are preserved: regular 3 HP / 4 units per
second, tough 6 HP / 3 units per second. Health-bar offsets remain 1.4 and 1.55;
visual children have no colliders.

In Play Mode, Wave 2 showed both enemy types together and clearly distinguishable
from the gameplay camera, with a readable damaged armored health bar. Natural
Tenten and Sasuke impacts updated regular bars to 2/3 and 1/3 and tough bars to
4/6 and 2/6, with camera-facing orientation maintained. The normal-budget mixed
three-wave strategy finished **Victory with 230 Ryo**. Console during verification:
**0 errors, 0 warnings**. Main saved with empty pads and Play Mode exited. Final Editor Console: **0 errors, 1 warning** from Unity AI Toolkit
(Account API unavailable within 30 seconds); no gameplay warnings were observed.


## Final Play Mode verification (Step 20)

The tough variant reuses the regular `Ninja Visual` child, adding broad
burnt-orange shoulder plates and a steel helmet over a charcoal outfit. Its
original capsule renderer is disabled; the gameplay capsule collider is retained.
The visual primitives have no colliders. The original Step 20 materials remain in
`Assets/Materials/ArmoredNinja/`; Step 23 uses the shared palette described above.
Geometry and material overrides are saved in
`Assets/Prefabs/ToughEnemy.prefab`. Movement, HP, rewards, wave placement and
predictive aiming are unchanged.

The documented Pads 2/3 Tenten strategy cleared all three waves with the armored
ninja and textured gate: **Victory, 180 Ryo**. Wave 1 left 60 Ryo; upgrading Pad 3
before Wave 2 spent 60, Wave 2 left 100, and upgrading Pad 2 before Wave 3 left 40.
Real kunai impacts updated armored health bars, including 4/6 and 2/6; the raised
bar remained readable above the helmet. Violet and armored enemies are distinct
from the unchanged gameplay camera.

Empty pads caused **Defeat** when an enemy reached Point5, with no Ryo reward.
Restart after both Victory and Defeat restored Wave 1, 100 Ryo, six empty pads,
no enemies/projectiles and no pending selection; the textured gate remained.
The complete placement hint was readable at 1280x720. Both tower choices deployed;
active-wave checks passed deployment, canceling/confirming sales, 35/42-Ryo refunds,
immediate pad reuse, and canceling/confirming the 60-Ryo Chunin upgrade. Confirmed
transactions occurred once. Waves continued with the panel open and kill rewards
updated affordability. Mouse-input deployment was also checked using temporary
in-memory Editor focus routing, then the original default input settings restored.
The active-wave transactions and empty-pad Defeat/Restart checks were rerun after
restoration. Final Console: **0 errors, 0 warnings**. Main was saved with empty
pads and Play Mode exited.

Final checks run in Unity 6000.6.0f1 Play Mode; no standalone player build was made.
Gate FBX/textures/material and armor assets have their `.meta` files. The renamed
gate sources retain their GUIDs and the original source files. The source renames,
armor assets and their metadata are included together.

![Regular and armored enemies with a damaged armored health bar](Documentation/Screenshots/step20-armored-ninja.png)

## Hidden Leaf Gate asset (Step 19a)

The gate uses the user-supplied Meshy AI export named
`HiddenLeafGate.fbx` and its accompanying
textures named `HiddenLeafGate_BaseColor`, `HiddenLeafGate_Normal`,
`HiddenLeafGate_Metallic`, and `HiddenLeafGate_Roughness`, imported under
`Assets/Art/HiddenLeafGate/Source/`. Attribution is to
the supplied Meshy AI export; no individual creator or license file was provided
with the source folder. The original FBX and all four texture files are preserved
byte-for-byte; no reduced mesh was used. Unity imports **200,748 triangles**
(112,996 vertices).

`Hidden Leaf Gate.mat` uses the project's **URP Lit** shader with the supplied
base-color and normal textures. The normal texture is imported as a Normal Map.
Metallic and roughness source images are retained; this material uses constant
metallic 0 and smoothness 0.25. The visual is centered over Point5, rotated
55 degrees around Y and scaled 1.9 on its parent so the arch spans the final road
and the roof is readable from the unchanged gameplay camera. The primitive gate
was removed; the new visual has no colliders and does not change waypoints or
defeat logic.

In Play Mode, Wave 1 with six empty pads reached **Defeat** when the first enemy
arrived at `(18, 0.9, -12)`. Restart restored Wave 1, 100 Ryo, six empty pads and
zero enemies/projectiles; the textured gate stayed in place. At 1280x720, both
the roof and approaching enemies were visible, with no missing textures.
Final Console: **0 errors, 0 warnings**. Main was saved and Play Mode exited.

![Gate and final approach in Game view](Documentation/Screenshots/step19a-gate-final-approach.png)

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
