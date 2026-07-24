using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrainrotRush
{
    [Serializable]
    public class IAPProduct
    {
        public string id;
        public string displayName;
        public string description;
        public string priceLabel;
        public int gemAmount;
        public bool removeAds;
        public bool battlePass;
        public string bundleCosmeticId;
    }

    /// <summary>
    /// Stub IAP. Wire to Unity IAP / store SDKs later. No pay-to-win.
    /// </summary>
    public class IAPManager : MonoBehaviour
    {
        public static readonly List<IAPProduct> Catalog = new List<IAPProduct>
        {
            new IAPProduct { id = "remove_ads", displayName = "Remove Ads", description = "No more interstitial ads", priceLabel = "$2.99", removeAds = true },
            new IAPProduct { id = "gems_small", displayName = "Gem Pouch", description = "80 Gems", priceLabel = "$0.99", gemAmount = 80 },
            new IAPProduct { id = "gems_medium", displayName = "Gem Bag", description = "500 Gems", priceLabel = "$4.99", gemAmount = 500 },
            new IAPProduct { id = "gems_large", displayName = "Gem Vault", description = "1200 Gems", priceLabel = "$9.99", gemAmount = 1200 },
            new IAPProduct { id = "battle_pass", displayName = "Premium Battle Pass", description = "Unlock premium track", priceLabel = "$4.99", battlePass = true },
            new IAPProduct { id = "bundle_rizz", displayName = "Rizz Bundle", description = "Rizz Runner + Fire Trail", priceLabel = "$3.99", bundleCosmeticId = "char_rizz", gemAmount = 50 },
        };

        public void Purchase(string productId, Action<bool> onComplete = null)
        {
            var product = Catalog.Find(p => p.id == productId);
            if (product == null)
            {
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log($"[IAP] Simulated purchase: {product.displayName}");
            Grant(product);
            onComplete?.Invoke(true);
        }

        void Grant(IAPProduct product)
        {
            var s = GameServices.Instance;
            if (product.removeAds)
            {
                s.Data.adsRemoved = true;
            }

            if (product.gemAmount > 0)
                s.Currency.Add(CurrencyType.Gems, product.gemAmount, persist: false);

            if (product.battlePass)
            {
                s.Data.battlePassPremium = true;
                GameEvents.BattlePassUpdated();
            }

            if (!string.IsNullOrEmpty(product.bundleCosmeticId))
            {
                s.Unlocks.Unlock(product.bundleCosmeticId);
                s.Unlocks.Unlock("trail_fire");
                s.Achievements.NotifyCosmeticUnlocked();
            }

            s.Persist();
        }
    }
}
