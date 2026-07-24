using UnityEngine;

namespace BrainrotRush.Art
{
    /// <summary>
    /// Catalog entry mapping gameplay IDs to mesh keys, tints, and scale.
    /// Drop matching FBX/OBJ/GLB into Resources/Art/Models or Art/DropIn to replace procedural stand-ins.
    /// </summary>
    public static class ArtCatalog
    {
        public const string ResourcesRoot = "Art/Models";

        public static string CharacterMeshKey(string cosmeticId)
        {
            switch (cosmeticId)
            {
                case "char_sigma":
                case "char_assassino":
                    return "Characters/char_assassino";
                case "char_rizz":
                case "char_ballerina":
                    return "Characters/char_ballerina";
                case "char_ohio":
                case "char_tralalero":
                    return "Characters/char_tralalero";
                case "char_default":
                case "char_tung_sahur":
                default:
                    return "Characters/char_tung_sahur";
            }
        }

        public static string HatMeshKey(string cosmeticId)
        {
            switch (cosmeticId)
            {
                case "hat_cap": return "Hats/hat_cap";
                case "hat_crown": return "Hats/hat_crown";
                default: return null;
            }
        }

        public static string TowerMeshKey(TowerType type)
        {
            switch (type)
            {
                case TowerType.Cannon: return "Towers/tower_cannon";
                case TowerType.Freeze: return "Towers/tower_freeze";
                case TowerType.Laser: return "Towers/tower_laser";
                case TowerType.RapidFire:
                default: return "Towers/tower_rapid";
            }
        }

        public static string EnemyMeshKey(string enemyId, bool boss)
        {
            if (boss) return "Enemies/boss_bombardiro";
            switch (enemyId)
            {
                case "enemy_bombardiro": return "Enemies/enemy_bombardiro";
                case "enemy_lirili": return "Enemies/enemy_lirili";
                case "enemy_boneca": return "Enemies/enemy_boneca";
                case "enemy_tralalero": return "Enemies/enemy_tralalero";
                case "enemy_patapim":
                default: return "Enemies/enemy_patapim";
            }
        }

        public static readonly string[] EnemyRoster =
        {
            "enemy_bombardiro",
            "enemy_patapim",
            "enemy_lirili",
            "enemy_boneca",
            "enemy_tralalero"
        };

        public static Color EnemyTint(string enemyId)
        {
            switch (enemyId)
            {
                case "enemy_bombardiro": return new Color(0.25f, 0.55f, 0.3f);
                case "enemy_patapim": return new Color(0.35f, 0.7f, 0.35f);
                case "enemy_lirili": return new Color(0.85f, 0.7f, 0.35f);
                case "enemy_boneca": return new Color(0.4f, 0.75f, 0.45f);
                case "enemy_tralalero": return new Color(0.35f, 0.55f, 0.85f);
                default: return Color.white;
            }
        }
    }
}
