# Tower Defense

Unity project using Unity **6000.6.0f1** and the Universal Render Pipeline (URP).
The current project contains the starter `SampleScene`; tower-defense gameplay
has not been implemented yet.

## Open and run

1. Install Unity Hub and Unity Editor **6000.6.0f1**. The exact version is also
   recorded in `ProjectSettings/ProjectVersion.txt`.
2. Clone this repository, or download its ZIP from GitHub and extract it.
3. In Unity Hub, choose **Add project from disk** and select the folder containing
   `Assets`, `Packages`, and `ProjectSettings`.
4. Open the project. Allow Unity to download packages and finish importing assets
   and compiling scripts. Internet access is needed for the first package restore.
5. Open `Assets/Scenes/SampleScene.unity` and press **Play** in the Editor.

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
