using System.Collections.Generic;
using UnityEngine;

namespace BrainrotRush
{
    public class BattlePassReward
    {
        public int tier;
        public CurrencyType currency;
        public int amount;
        public string cosmeticId;
        public bool isPremium;
    }

    public class BattlePassSystem
    {
        public const int MaxTiers = 20;
        public const int XpPerTier = 100;
        public const int PremiumGemCost = 500;

        readonly GameServices _services;
        readonly List<BattlePassReward> _rewards;

        public BattlePassSystem(GameServices services)
        {
            _services = services;
            _rewards = BuildRewards();
        }

        public int Xp => _services.Data.battlePassXp;
        public int CurrentTier => Mathf.Min(MaxTiers, _services.Data.battlePassXp / XpPerTier);
        public bool HasPremium => _services.Data.battlePassPremium;
        public IReadOnlyList<BattlePassReward> Rewards => _rewards;

        public void AddXp(int amount)
        {
            if (amount <= 0) return;
            _services.Data.battlePassXp += amount;
            GameEvents.BattlePassUpdated();
        }

        public bool BuyPremium()
        {
            if (HasPremium) return false;
            if (!_services.Currency.Spend(CurrencyType.Gems, PremiumGemCost)) return false;
            _services.Data.battlePassPremium = true;
            GameEvents.BattlePassUpdated();
            _services.Persist();
            return true;
        }

        public bool Claim(int tier, bool premium)
        {
            if (tier < 1 || tier > CurrentTier) return false;
            if (premium && !HasPremium) return false;

            var claimed = premium ? _services.Data.claimedPremiumTiers : _services.Data.claimedFreeTiers;
            if (claimed.Contains(tier)) return false;

            var reward = _rewards.Find(r => r.tier == tier && r.isPremium == premium);
            if (reward == null) return false;

            claimed.Add(tier);
            if (!string.IsNullOrEmpty(reward.cosmeticId))
                _services.Unlocks.Unlock(reward.cosmeticId);
            else
                _services.Currency.Add(reward.currency, reward.amount, persist: false);

            GameEvents.BattlePassUpdated();
            _services.Persist();
            return true;
        }

        public bool IsClaimed(int tier, bool premium)
        {
            var claimed = premium ? _services.Data.claimedPremiumTiers : _services.Data.claimedFreeTiers;
            return claimed.Contains(tier);
        }

        static List<BattlePassReward> BuildRewards()
        {
            var list = new List<BattlePassReward>();
            for (int t = 1; t <= MaxTiers; t++)
            {
                list.Add(new BattlePassReward
                {
                    tier = t,
                    isPremium = false,
                    currency = t % 5 == 0 ? CurrencyType.Gems : CurrencyType.Coins,
                    amount = t % 5 == 0 ? 10 : 50 + t * 10
                });

                list.Add(new BattlePassReward
                {
                    tier = t,
                    isPremium = true,
                    currency = t % 4 == 0 ? CurrencyType.UnlockTokens : CurrencyType.Coins,
                    amount = t % 4 == 0 ? 1 : 100 + t * 20,
                    cosmeticId = t == 10 ? "trail_sparkle" : (t == 20 ? "hat_crown" : null)
                });
            }
            return list;
        }
    }
}
