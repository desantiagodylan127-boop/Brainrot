#!/usr/bin/env python3
"""Generate low-poly OBJ stand-ins for Brainrot Rush characters/enemies/towers.

These are stylized silhouettes meant as Blender starting points and Unity
Resources drop-ins until official Italian Brainrot packs are imported.
"""

from __future__ import annotations

import math
import os
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT_DIRS = [
    ROOT / "Assets" / "Art",
    ROOT / "Assets" / "Resources" / "Art" / "Models",
]


def write_obj(path: Path, name: str, verts: list[tuple], faces: list[tuple]):
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8") as f:
        f.write(f"# Brainrot Rush placeholder: {name}\n")
        f.write(f"o {name}\n")
        for x, y, z in verts:
            f.write(f"v {x:.5f} {y:.5f} {z:.5f}\n")
        for face in faces:
            f.write("f " + " ".join(str(i) for i in face) + "\n")


def box(cx, cy, cz, sx, sy, sz):
    hx, hy, hz = sx * 0.5, sy * 0.5, sz * 0.5
    verts = [
        (cx - hx, cy - hy, cz - hz),
        (cx + hx, cy - hy, cz - hz),
        (cx + hx, cy + hy, cz - hz),
        (cx - hx, cy + hy, cz - hz),
        (cx - hx, cy - hy, cz + hz),
        (cx + hx, cy - hy, cz + hz),
        (cx + hx, cy + hy, cz + hz),
        (cx - hx, cy + hy, cz + hz),
    ]
    faces = [
        (1, 2, 3, 4),
        (5, 8, 7, 6),
        (1, 5, 6, 2),
        (4, 3, 7, 8),
        (1, 4, 8, 5),
        (2, 6, 7, 3),
    ]
    return verts, faces


def cylinder(cx, cy, cz, radius, height, segments=10):
    verts = []
    faces = []
    hy = height * 0.5
    # top + bottom centers
    verts.append((cx, cy + hy, cz))  # 1
    verts.append((cx, cy - hy, cz))  # 2
    for i in range(segments):
        a = (2 * math.pi * i) / segments
        x = cx + math.cos(a) * radius
        z = cz + math.sin(a) * radius
        verts.append((x, cy + hy, z))
        verts.append((x, cy - hy, z))
    # ring indices start at 3
    for i in range(segments):
        t0 = 3 + i * 2
        t1 = 3 + ((i + 1) % segments) * 2
        b0 = t0 + 1
        b1 = t1 + 1
        faces.append((1, t0, t1))
        faces.append((2, b1, b0))
        faces.append((t0, b0, b1, t1))
    return verts, faces


def merge(parts: list[tuple[list, list]]):
    verts = []
    faces = []
    offset = 0
    for v, f in parts:
        verts.extend(v)
        for face in f:
            faces.append(tuple(i + offset for i in face))
        offset += len(v)
    return verts, faces


def save_all(category: str, key: str, verts, faces):
    for base in OUT_DIRS:
        write_obj(base / category / f"{key}.obj", key, verts, faces)


def make_tung_sahur():
    # Tall wooden bat / stick figure with drumstick vibe
    parts = [
        box(0, 1.1, 0, 0.28, 2.0, 0.28),
        box(0, 2.25, 0, 0.55, 0.45, 0.55),  # head
        box(0.55, 1.5, 0, 0.9, 0.18, 0.18),  # bat
        box(-0.35, 0.15, 0.15, 0.2, 0.35, 0.2),
        box(0.35, 0.15, -0.1, 0.2, 0.35, 0.2),
    ]
    return merge(parts)


def make_ballerina():
    # Cup head + tutu body
    parts = [
        cylinder(0, 1.85, 0, 0.42, 0.55, 12),  # cup
        box(0, 2.2, 0, 0.55, 0.12, 0.55),  # lid
        box(0, 2.4, 0, 0.12, 0.25, 0.12),  # handle nub
        box(0, 1.1, 0, 0.28, 0.9, 0.28),  # torso
        cylinder(0, 0.75, 0, 0.7, 0.12, 14),  # tutu
        box(-0.18, 0.25, 0, 0.14, 0.5, 0.14),
        box(0.18, 0.25, 0, 0.14, 0.5, 0.14),
        box(-0.55, 1.25, 0, 0.55, 0.1, 0.1),
        box(0.55, 1.25, 0, 0.55, 0.1, 0.1),
    ]
    return merge(parts)


def make_tralalero():
    # Shark body + three legs + sneakers vibe
    parts = [
        box(0, 0.85, 0.1, 0.7, 0.55, 1.4),  # body
        box(0, 1.0, 0.95, 0.45, 0.35, 0.55),  # snout
        box(0, 1.35, -0.2, 0.15, 0.55, 0.55),  # fin
        box(-0.35, 0.25, -0.15, 0.2, 0.5, 0.25),
        box(0.0, 0.25, 0.25, 0.2, 0.5, 0.25),
        box(0.35, 0.25, -0.15, 0.2, 0.5, 0.25),
        box(-0.35, 0.05, -0.15, 0.28, 0.12, 0.4),
        box(0.0, 0.05, 0.25, 0.28, 0.12, 0.4),
        box(0.35, 0.05, -0.15, 0.28, 0.12, 0.4),
    ]
    return merge(parts)


def make_cappuccino_assassino():
    parts = [
        cylinder(0, 1.55, 0, 0.38, 0.7, 12),
        box(0, 2.0, 0, 0.5, 0.12, 0.5),
        box(0, 1.0, 0, 0.45, 0.55, 0.3),  # coat
        box(-0.45, 0.95, 0.05, 0.35, 0.12, 0.12),
        box(0.55, 0.95, 0.05, 0.55, 0.1, 0.1),  # blade arm
        box(0.9, 0.95, 0.05, 0.35, 0.08, 0.18),  # blade
        box(-0.15, 0.3, 0, 0.14, 0.55, 0.14),
        box(0.15, 0.3, 0, 0.14, 0.55, 0.14),
    ]
    return merge(parts)


def make_bombardiro():
    # Croc + plane hybrid
    parts = [
        box(0, 0.7, 0, 0.7, 0.45, 1.5),
        box(0, 0.7, 0.95, 0.45, 0.3, 0.5),
        box(-0.95, 0.75, 0, 1.1, 0.1, 0.45),  # wing
        box(0.95, 0.75, 0, 1.1, 0.1, 0.45),
        box(0, 1.1, -0.55, 0.12, 0.45, 0.35),  # tail fin
        box(-0.25, 0.25, 0.35, 0.18, 0.4, 0.25),
        box(0.25, 0.25, 0.35, 0.18, 0.4, 0.25),
    ]
    return merge(parts)


def make_patapim():
    # Tree / cactus creature
    parts = [
        cylinder(0, 1.0, 0, 0.28, 1.8, 10),
        box(-0.55, 1.3, 0, 0.7, 0.2, 0.2),
        box(0.55, 1.55, 0, 0.7, 0.2, 0.2),
        box(0, 2.05, 0, 0.45, 0.35, 0.45),
        box(-0.2, 0.15, 0.1, 0.2, 0.3, 0.2),
        box(0.2, 0.15, -0.1, 0.2, 0.3, 0.2),
    ]
    return merge(parts)


def make_lirili():
    # Elephant-cactus hybrid silhouette
    parts = [
        box(0, 0.85, 0, 0.7, 0.7, 0.9),
        box(0, 1.35, 0.55, 0.35, 0.25, 0.55),  # trunk-ish
        cylinder(-0.2, 1.55, -0.1, 0.12, 0.55, 8),
        cylinder(0.2, 1.55, -0.1, 0.12, 0.55, 8),
        box(-0.25, 0.25, 0.2, 0.2, 0.45, 0.2),
        box(0.25, 0.25, 0.2, 0.2, 0.45, 0.2),
        box(-0.25, 0.25, -0.25, 0.2, 0.45, 0.2),
        box(0.25, 0.25, -0.25, 0.2, 0.45, 0.2),
    ]
    return merge(parts)


def make_boneca():
    # Amphibian + tire vibe
    parts = [
        box(0, 0.7, 0, 0.7, 0.55, 0.7),
        box(0, 1.15, 0.15, 0.55, 0.35, 0.45),
        cylinder(0, 0.35, 0, 0.55, 0.25, 12),  # tire
        box(-0.45, 0.85, 0.25, 0.25, 0.15, 0.35),
        box(0.45, 0.85, 0.25, 0.25, 0.15, 0.35),
    ]
    return merge(parts)


def make_tower_rapid():
    parts = [
        cylinder(0, 0.35, 0, 0.45, 0.7, 10),
        box(0, 0.85, 0.35, 0.25, 0.25, 0.55),
        box(0, 1.05, 0, 0.35, 0.2, 0.35),
    ]
    return merge(parts)


def make_tower_cannon():
    parts = [
        box(0, 0.35, 0, 0.7, 0.7, 0.7),
        cylinder(0, 0.7, 0.55, 0.22, 0.9, 10),
    ]
    # rotate cannon barrel along Z by rebuilding as box
    parts = [
        box(0, 0.35, 0, 0.7, 0.7, 0.7),
        box(0, 0.7, 0.7, 0.35, 0.35, 1.0),
    ]
    return merge(parts)


def make_tower_freeze():
    parts = [
        box(0, 0.45, 0, 0.6, 0.9, 0.6),
        box(0, 1.05, 0, 0.7, 0.15, 0.7),
        box(0, 0.7, 0.4, 0.2, 0.2, 0.35),
    ]
    return merge(parts)


def make_tower_laser():
    parts = [
        cylinder(0, 0.5, 0, 0.28, 1.0, 10),
        box(0, 1.15, 0.2, 0.2, 0.2, 0.7),
        box(0, 0.2, 0, 0.55, 0.15, 0.55),
    ]
    return merge(parts)


def make_hat_cap():
    parts = [
        cylinder(0, 0.12, 0, 0.45, 0.2, 12),
        box(0, 0.08, 0.35, 0.55, 0.06, 0.35),
    ]
    return merge(parts)


def make_hat_crown():
    parts = [
        box(0, 0.15, 0, 0.55, 0.25, 0.55),
        box(-0.2, 0.4, 0, 0.12, 0.25, 0.12),
        box(0.0, 0.45, 0, 0.12, 0.35, 0.12),
        box(0.2, 0.4, 0, 0.12, 0.25, 0.12),
    ]
    return merge(parts)


MODELS = {
    ("Characters", "char_tung_sahur"): make_tung_sahur,
    ("Characters", "char_ballerina"): make_ballerina,
    ("Characters", "char_tralalero"): make_tralalero,
    ("Characters", "char_assassino"): make_cappuccino_assassino,
    ("Enemies", "enemy_bombardiro"): make_bombardiro,
    ("Enemies", "enemy_patapim"): make_patapim,
    ("Enemies", "enemy_lirili"): make_lirili,
    ("Enemies", "enemy_boneca"): make_boneca,
    ("Enemies", "enemy_tralalero"): make_tralalero,
    ("Enemies", "boss_bombardiro"): make_bombardiro,
    ("Towers", "tower_rapid"): make_tower_rapid,
    ("Towers", "tower_cannon"): make_tower_cannon,
    ("Towers", "tower_freeze"): make_tower_freeze,
    ("Towers", "tower_laser"): make_tower_laser,
    ("Hats", "hat_cap"): make_hat_cap,
    ("Hats", "hat_crown"): make_hat_crown,
}


def main():
    for (category, key), builder in MODELS.items():
        verts, faces = builder()
        save_all(category, key, verts, faces)
        print(f"wrote {category}/{key}.obj ({len(verts)} verts)")
    print("done")


if __name__ == "__main__":
    main()
