using UnityEngine;

namespace BrainrotRush
{
    /// <summary>
    /// Optional ScriptableObject definitions for designers.
    /// Runtime catalog currently lives in UnlockManager / IAPManager for speed.
    /// </summary>
    [CreateAssetMenu(menuName = "Brainrot Rush/Cosmetic Definition", fileName = "Cosmetic")]
    public class CosmeticDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public CosmeticType type;
        public int coinCost;
        public int gemCost;
        public int tokenCost;
        public Color tint = Color.white;
        [TextArea] public string description;
    }

    [CreateAssetMenu(menuName = "Brainrot Rush/Enemy Definition", fileName = "Enemy")]
    public class EnemyDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public float health = 30f;
        public float speed = 2f;
        public int reward = 10;
        public bool isBoss;
        public Color tint = Color.white;
    }

    [CreateAssetMenu(menuName = "Brainrot Rush/Tower Definition", fileName = "Tower")]
    public class TowerDefinition : ScriptableObject
    {
        public TowerType type;
        public int cost = 50;
        public float range = 4.5f;
        public float fireRate = 1f;
        public float damage = 8f;
        public Color tint = Color.white;
    }
}
