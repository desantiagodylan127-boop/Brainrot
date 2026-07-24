using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BrainrotRush
{
    [Serializable]
    public class CosmeticDef
    {
        public string id;
        public string displayName;
        public CosmeticType type;
        public int coinCost;
        public int gemCost;
        public int tokenCost;
        public Color tint = Color.white;
        public string description;
    }

    public class UnlockManager
    {
        readonly GameServices _services;
        public static readonly List<CosmeticDef> Catalog = BuildCatalog();

        public UnlockManager(GameServices services) => _services = services;

        public IEnumerable<CosmeticDef> GetByType(CosmeticType type) =>
            Catalog.Where(c => c.type == type);

        public bool IsUnlocked(string id) =>
            _services.Data.unlockedCosmetics.Contains(id);

        public string GetEquipped(CosmeticType type) => type switch
        {
            CosmeticType.Character => _services.Data.equippedCharacterId,
            CosmeticType.Trail => _services.Data.equippedTrailId,
            CosmeticType.Hat => _services.Data.equippedHatId,
            CosmeticType.Emote => _services.Data.equippedEmoteId,
            CosmeticType.TowerSkin => _services.Data.equippedTowerSkinId,
            _ => ""
        };

        public bool Equip(string id)
        {
            var def = Catalog.FirstOrDefault(c => c.id == id);
            if (def == null || !IsUnlocked(id)) return false;

            switch (def.type)
            {
                case CosmeticType.Character: _services.Data.equippedCharacterId = id; break;
                case CosmeticType.Trail: _services.Data.equippedTrailId = id; break;
                case CosmeticType.Hat: _services.Data.equippedHatId = id; break;
                case CosmeticType.Emote: _services.Data.equippedEmoteId = id; break;
                case CosmeticType.TowerSkin: _services.Data.equippedTowerSkinId = id; break;
            }

            _services.Persist();
            return true;
        }

        public bool TryPurchase(string id)
        {
            var def = Catalog.FirstOrDefault(c => c.id == id);
            if (def == null || IsUnlocked(id)) return false;

            if (def.tokenCost > 0)
            {
                if (!_services.Currency.Spend(CurrencyType.UnlockTokens, def.tokenCost)) return false;
            }
            else if (def.gemCost > 0)
            {
                if (!_services.Currency.Spend(CurrencyType.Gems, def.gemCost)) return false;
            }
            else if (def.coinCost > 0)
            {
                if (!_services.Currency.Spend(CurrencyType.Coins, def.coinCost)) return false;
            }

            Unlock(id);
            return true;
        }

        public void Unlock(string id)
        {
            if (IsUnlocked(id)) return;
            _services.Data.unlockedCosmetics.Add(id);
            GameEvents.CosmeticUnlocked(id);
            _services.Persist();
        }

        public CosmeticDef Get(string id) => Catalog.FirstOrDefault(c => c.id == id);

        static List<CosmeticDef> BuildCatalog()
        {
            return new List<CosmeticDef>
            {
                // Character IDs map to ArtCatalog / Assets/Art meshes. Legacy ids kept as aliases in ArtCatalog.
                new CosmeticDef { id = "char_default", displayName = "Tung Tung Sahur", type = CosmeticType.Character, tint = new Color(0.72f, 0.55f, 0.35f), description = "Starter bat brainrot" },
                new CosmeticDef { id = "char_sigma", displayName = "Cappuccino Assassino", type = CosmeticType.Character, tokenCost = 2, tint = new Color(0.35f, 0.25f, 0.2f), description = "Espresso stealth drip" },
                new CosmeticDef { id = "char_rizz", displayName = "Ballerina Cappuccina", type = CosmeticType.Character, gemCost = 80, tint = new Color(1f, 0.65f, 0.8f), description = "Tutu chaos energy" },
                new CosmeticDef { id = "char_ohio", displayName = "Tralalero Tralala", type = CosmeticType.Character, coinCost = 2500, tint = new Color(0.35f, 0.55f, 0.9f), description = "Three-legged shark runner" },

                new CosmeticDef { id = "trail_none", displayName = "No Trail", type = CosmeticType.Trail },
                new CosmeticDef { id = "trail_fire", displayName = "Fire Trail", type = CosmeticType.Trail, coinCost = 800, tint = new Color(1f, 0.4f, 0.1f) },
                new CosmeticDef { id = "trail_sparkle", displayName = "Sparkle Trail", type = CosmeticType.Trail, gemCost = 40, tint = new Color(1f, 0.9f, 0.3f) },

                new CosmeticDef { id = "hat_none", displayName = "No Hat", type = CosmeticType.Hat },
                new CosmeticDef { id = "hat_cap", displayName = "Brainrot Cap", type = CosmeticType.Hat, coinCost = 500, tint = Color.red },
                new CosmeticDef { id = "hat_crown", displayName = "Sigma Crown", type = CosmeticType.Hat, gemCost = 60, tint = new Color(1f, 0.85f, 0.2f) },

                new CosmeticDef { id = "emote_wave", displayName = "Wave", type = CosmeticType.Emote },
                new CosmeticDef { id = "emote_dance", displayName = "Brainrot Dance", type = CosmeticType.Emote, coinCost = 400 },
                new CosmeticDef { id = "emote_flex", displayName = "Sigma Flex", type = CosmeticType.Emote, tokenCost = 1 },

                new CosmeticDef { id = "skin_default", displayName = "Classic Towers", type = CosmeticType.TowerSkin },
                new CosmeticDef { id = "skin_neon", displayName = "Neon Towers", type = CosmeticType.TowerSkin, gemCost = 100, tint = new Color(0.2f, 1f, 0.8f) },
                new CosmeticDef { id = "skin_gold", displayName = "Gold Towers", type = CosmeticType.TowerSkin, coinCost = 5000, tint = new Color(1f, 0.8f, 0.2f) },
            };
        }
    }
}
