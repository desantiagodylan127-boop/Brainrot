# Architecture

## Goals
Ship a fun dual-mode prototype fast. Prefer plain C# classes and MonoBehaviours over frameworks.

## Runtime flow
1. `GameServices` boots via `RuntimeInitializeOnLoadMethod` (BeforeSceneLoad).
2. `AppBootstrap` boots after scene load and opens the main menu.
3. Mode loaders destroy scene roots (except `GameServices`) and spawn mode managers.
4. Mode managers build world + UI from code (`UIFactory`, primitives).

## Shared state
`PlayerData` holds currencies, XP, cosmetics, missions, battle pass, ads flag.
Persisted through `SaveSystem` (JSON in PlayerPrefs).

## Modes
- **Runner**: world scrolls backward; player stays near z=0. Lane / jump / slide via `SwipeInput`.
- **TD**: enemies follow `EnemyPath` waypoints. Towers acquire nearest target in range.

## Extending
- Art: `BrainrotArtFactory` + `ArtCatalog` attach visuals by mesh key. Prefer Resources meshes under `Assets/Resources/Art/Models/`; procedural silhouettes are the fallback. See `Docs/ASSETS.md`.
- Ads/IAP: replace stub bodies in `AdManager` / `IAPManager`.
- Data: optional ScriptableObjects under `Assets/Scripts/Data` for designer-friendly tuning.
