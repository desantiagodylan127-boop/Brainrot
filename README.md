# Brainrot Rush

Mobile hybrid-casual prototype: **Endless Runner** + **Tower Defense**, with shared progression, cosmetics, economy, and monetization stubs.

Built for **Unity 2022.3 LTS** (portrait mobile). Gameplay and UI are generated at runtime — open the project, press Play.

## Quick Start

1. Install **Unity 2022.3 LTS**
2. Open this folder as a Unity project
3. Open `Assets/Scenes/Bootstrap.unity`
4. Press **Play**

Optional menu: **Brainrot Rush → Create Bootstrap Scene** / **Reset Player Save**

### Controls

| Mode | Input |
|------|--------|
| Runner | Swipe L/R lanes, Up jump, Down slide (or WASD / arrows) |
| Tower Defense | Tap tower buttons, tap green build spots, Upgrade / Sell |

## What's Included

### Mode 1 — Endless Runner
- 3-lane auto-run with swipe jump / slide / lane change
- Procedural segments with obstacles, coins, powerups
- Magnet, Shield, Speed Boost
- Speed ramps over time, distance score, high score save
- Pause + game over (2x coins ad, extra life ad)

### Mode 2 — Tower Defense
- Fixed path, funny Brainrot enemies, health bars
- Build spots, 4 towers: Rapid Fire, Cannon, Freeze, Laser
- Upgrade / sell, scrap currency from kills
- Waves + boss every 5 waves (15 waves to victory)
- Victory / defeat screens with shared rewards

### Shared Progression
- Coins, Gems, Unlock Tokens, XP / levels
- Cosmetics: characters, trails, hats, emotes, tower skins (**no P2W**)
- Daily rewards, daily missions, achievements
- Battle Pass (free + premium tracks)

### Monetization (stubs)
- Rewarded ads: double coins, extra life, bonus chest
- IAP: remove ads, gem packs, battle pass, cosmetic bundle
- Swap `AdManager` / `IAPManager` for real SDKs later

## Architecture

```
Assets/Scripts/
  Core/           GameServices, save, events, enums
  Economy/        currency, daily, missions, achievements, battle pass
  Progression/    XP, unlocks / cosmetics catalog
  Runner/         endless runner gameplay + UI
  TowerDefense/   TD gameplay + UI
  Monetization/   ad + IAP stubs
  UI/             main menu, meta panels, UIFactory
  Bootstrap/      AppBootstrap runtime entry
  Data/           optional ScriptableObject defs
```

- `GameServices` — DontDestroyOnLoad singleton (save + shared systems)
- `GameEvents` — lightweight static events
- `SaveSystem` — PlayerPrefs JSON
- `AppBootstrap` — builds menu / runner / TD at runtime (no prefab dependency)

## Design Notes

- Speed over perfection: primitives + runtime UI, modular readable scripts
- Cosmetics are visual only
- No pay-to-win (IAP is cosmetics / convenience / gems for cosmetics & battle pass)

## Next Steps (when polishing)

- Replace primitives with Brainrot character / tower art
- Wire AdMob / Unity Ads and Unity IAP
- Add audio, juice, and particle VFX
- Split modes into dedicated baked scenes if needed for shipping
