# Art & Blender Assets — Brainrot Rush

This project ships **low-poly placeholder meshes** plus a drop-in pipeline for real Blender / marketplace models.

## Folder layout

```
Assets/Art/
  Characters/     # Source OBJ stand-ins (open in Blender, iterate)
  Enemies/
  Towers/
  Hats/
  Props/
  DropIn/         # Drop finished FBX/GLB here, then move into Resources
  Blender/        # Optional .blend work files (gitignored if huge)

Assets/Resources/Art/Models/
  Characters/     # Runtime Resources.Load targets (same keys as ArtCatalog)
  Enemies/
  Towers/
  Hats/

Assets/StreamingAssets/Models/   # Optional future runtime GLB loader path
```

## Character / enemy roster

| Gameplay ID | Display | Mesh key |
|-------------|---------|----------|
| `char_default` | Tung Tung Sahur | `Characters/char_tung_sahur` |
| `char_sigma` | Cappuccino Assassino | `Characters/char_assassino` |
| `char_rizz` | Ballerina Cappuccina | `Characters/char_ballerina` |
| `char_ohio` | Tralalero Tralala | `Characters/char_tralalero` |
| `enemy_bombardiro` | Bombardiro Crocodilo | `Enemies/enemy_bombardiro` |
| `enemy_patapim` | Brr Brr Patapim | `Enemies/enemy_patapim` |
| `enemy_lirili` | Lirili Larila | `Enemies/enemy_lirili` |
| `enemy_boneca` | Boneca Ambalabu | `Enemies/enemy_boneca` |
| `enemy_tralalero` | Tralalero (enemy) | `Enemies/enemy_tralalero` |
| `boss_bombardiro` | Boss Bombardiro | `Enemies/boss_bombardiro` |

Towers: `tower_rapid`, `tower_cannon`, `tower_freeze`, `tower_laser`.

## How visuals load

1. `BrainrotArtFactory` tries `Resources.Load` for `Art/Models/{meshKey}`.
2. If no imported mesh is found, it builds a **procedural primitive silhouette** so Play Mode always works.
3. Gameplay colliders stay on the invisible root capsule/cube.

## Importing Blender / Sketchfab / BlenderKit models

1. In Blender, open a stand-in from `Assets/Art/.../*.obj` **or** import your downloaded GLB/FBX.
2. Scale so the character is roughly **2 units tall**, origin at feet, facing **+Z**.
3. Apply transforms (`Ctrl+A`), then export:
   - **FBX** or **OBJ** → `Assets/Resources/Art/Models/{Characters|Enemies|Towers|Hats}/`
   - Name the file exactly like the mesh key (e.g. `char_ballerina.fbx`).
4. In Unity, confirm the import, then Press Play — cosmetics / waves pick it up automatically.

### Suggested free / CC sources

- Sketchfab CC packs: [Italian Brainrot game-ready pack](https://sketchfab.com/alex.cgwarrior/collections/italian-brainrot-game-ready-3d-model-pack-bf199a37d5ff42ccb54efaf04556b26a)
- BlenderKit (in Blender): search `Ballerina Cappuccina`, `Bombardini Gusini`
- Always check license (CC-BY vs NonCommercial) before shipping.

## Regenerate placeholders

```bash
python3 tools/generate_brainrot_meshes.py
```

Writes matching OBJs into both `Assets/Art/` and `Assets/Resources/Art/Models/`.
