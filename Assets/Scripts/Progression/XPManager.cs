using UnityEngine;

namespace BrainrotRush
{
    public class XPManager
    {
        readonly GameServices _services;
        public const int BaseXpPerLevel = 100;

        public XPManager(GameServices services) => _services = services;

        public int Level => _services.Data.level;
        public int Xp => _services.Data.xp;

        public int XpToNextLevel(int level) => BaseXpPerLevel + (level - 1) * 50;

        public void AddXp(int amount)
        {
            if (amount <= 0) return;
            _services.Data.xp += amount;
            _services.BattlePass.AddXp(amount / 2);

            bool leveled = false;
            while (_services.Data.xp >= XpToNextLevel(_services.Data.level))
            {
                _services.Data.xp -= XpToNextLevel(_services.Data.level);
                _services.Data.level++;
                leveled = true;
                _services.Currency.Add(CurrencyType.UnlockTokens, 1, persist: false);
                _services.Currency.Add(CurrencyType.Coins, 50 * _services.Data.level, persist: false);
                GameEvents.LevelUp(_services.Data.level);
            }

            GameEvents.XpChanged(_services.Data.xp, _services.Data.level);
            _services.Persist();
            if (leveled) Debug.Log($"Level up! Now level {_services.Data.level}");
        }
    }
}
