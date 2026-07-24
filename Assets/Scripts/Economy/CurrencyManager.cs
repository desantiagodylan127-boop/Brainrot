using UnityEngine;

namespace BrainrotRush
{
    public class CurrencyManager
    {
        readonly GameServices _services;

        public CurrencyManager(GameServices services) => _services = services;

        public int Coins => _services.Data.coins;
        public int Gems => _services.Data.gems;
        public int UnlockTokens => _services.Data.unlockTokens;

        public bool CanAfford(CurrencyType type, int amount) => Get(type) >= amount;

        public int Get(CurrencyType type) => type switch
        {
            CurrencyType.Coins => _services.Data.coins,
            CurrencyType.Gems => _services.Data.gems,
            CurrencyType.UnlockTokens => _services.Data.unlockTokens,
            _ => 0
        };

        public void Add(CurrencyType type, int amount, bool persist = true)
        {
            if (amount == 0) return;
            switch (type)
            {
                case CurrencyType.Coins:
                    _services.Data.coins = Mathf.Max(0, _services.Data.coins + amount);
                    if (amount > 0) _services.Data.totalCoinsCollected += amount;
                    break;
                case CurrencyType.Gems:
                    _services.Data.gems = Mathf.Max(0, _services.Data.gems + amount);
                    break;
                case CurrencyType.UnlockTokens:
                    _services.Data.unlockTokens = Mathf.Max(0, _services.Data.unlockTokens + amount);
                    break;
            }

            GameEvents.CurrencyChanged(type, amount, Get(type));
            if (persist) _services.Persist();
        }

        public bool Spend(CurrencyType type, int amount)
        {
            if (!CanAfford(type, amount)) return false;
            Add(type, -amount);
            return true;
        }
    }
}
