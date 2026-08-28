# Copilot instructions

## Project overview

This is a Unity 6.0 project (`6000.5.9f1`) for a small tower-defense game. The runtime gameplay code is in `Assets/script/` and is compiled into the generated `Assembly-CSharp` project. Most of the rest of `Assets/` is scene, prefab, environment, skybox, and package content.

`Assets/Scenes/etapa1.unity` is the only scene currently enabled in `ProjectSettings/EditorBuildSettings.asset`, so it is the primary playable scene. `etapa2.unity` and the `Menu*.unity` scenes exist in the repository but are not currently part of the build scene list.

## Build, run, test, and lint

- Use Unity Hub/Unity Editor version `6000.5.9f1` to open the repository.
- Press Play with `Assets/Scenes/etapa1.unity` open to run the playable level.
- Use Unity's **File > Build Profiles** window to configure and build a player; there is no repository build script or CLI wrapper.
- There is no tracked test assembly or test suite. Use Unity Test Framework's Test Runner only if tests are added; there is currently no single-test command.
- There is no configured C# linter or formatter command in this repository.
- A fresh checkout needs Git LFS: run `git lfs install` once, then `git lfs pull` to retrieve large assets such as the Cold Sunset HDRI texture.

## Architecture and gameplay flow

- `EnemySpawner` is a scene component on the `EnemySpawner` GameObject in `etapa1`. It owns the serialized enemy-type/probability table, waypoint list, wave timing, and progressive speed settings. On each timer expiry it selects a type, forces boss types from wave 4 onward, instantiates the selected prefab at the first waypoint, and passes the shared waypoint list and calculated speed to `EnemyMovement`.
- Enemy prefabs in `Assets/enemys/` carry `EnemyMovement`. The component places an enemy at waypoint zero, advances through the ordered `Transform` list with `Vector3.MoveTowards`, rotates toward the current target, and destroys the object after the final waypoint when `destruirAlLlegar` is enabled. The waypoint order in the scene is part of the gameplay configuration.
- `TowerPlacer` is a scene component on the `TowerPlacer` GameObject in `etapa1`. It creates a `Tower-cube` preview, raycasts from the configured camera to position it, checks `Physics.OverlapSphere` against `capasBloqueadas`, and instantiates a real tower on a valid left-click. Right-click or Escape cancels placement. Preview materials are optional Inspector references.
- Gameplay is configured primarily through Unity scene and prefab serialization, not through code constructors. Changes to prefab assignments, probabilities, waypoint references, layers, masks, or camera references must be made and saved in the Unity Inspector.
- The project uses the Input System package, but the gameplay scripts currently use Unity's legacy `Input` API (`Input.GetMouseButtonDown`, `Input.GetKeyDown`, and the standard axis/button APIs in the included demo helper scripts). Preserve that behavior unless deliberately migrating all relevant input code and project settings.
- URP is enabled through the Unity project settings. Navigation/package assets and the Polytope/AllSky/NaaszArts content are supporting environment assets; do not treat package-cache or generated `Library/` content as source.

## Repository conventions

- Keep game-specific scripts under `Assets/script/`; use one public MonoBehaviour/enum responsibility per file following the existing class names (`EnemySpawner`, `EnemyMovement`, `TowerPlacer`).
- Public gameplay fields are intentionally serialized and exposed in the Inspector, with Spanish-facing `Header` and `Tooltip` text. Preserve those Inspector-facing names and serialized field names when possible; renaming them can break scene/prefab data.
- Scene and prefab references are GUID-based. Prefer editing them in Unity so `.meta` files and serialized references remain valid. When adding or moving assets, include the corresponding `.meta` file.
- Preserve the existing Spanish terminology in user-facing Inspector labels and logs unless the surrounding feature is being consistently localized.
- Keep Unity-generated directories (`Library/`, `Temp/`, `Logs/`, `UserSettings/`, build output, and generated `.csproj`/solution files) out of commits; the root `.gitignore` already covers them.
- Large binary assets are managed with Git LFS. Follow the existing `.gitattributes` rules instead of committing duplicate or untracked copies of LFS content.
