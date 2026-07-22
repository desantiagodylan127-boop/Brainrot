using System.Collections.Generic;
using UnityEngine;

namespace BrainrotRush
{
    public class AchievementSystem
    {
        readonly GameServices _services;

        static readonly Dictionary<AchievementId, (string name, string desc, int coins, int xp)> Defs =
            new Dictionary<AchievementId, (string, string, int, int)>
            {
                { AchievementId.FirstRun, ("First Rush", "Play your first runner game", 50, 20) },
                { AchievementId.CoinCollector, ("Coin Goblin", "Collect 500 coins total", 200, 50) },
                { AchievementId.DistanceRunner, ("Long Legs", "Run 5000 distance total", 250, 60) },
                { AchievementId.WaveClearer, ("Wave Rider", "Clear 25 waves", 300, 80) },
                { AchievementId.BossSlayer, ("Boss Buster", "Defeat a boss", 400, 100) },
                { AchievementId.TowerMaster, ("Tower Enjoyer", "Defeat 100 enemies", 350, 90) },
                { AchievementId.DailyStreak3, ("Habitual", "Claim 3 daily rewards in a streak", 150, 40) },
                { AchievementId.Spender, ("Shopaholic", "Unlock any cosmetic", 100, 30) },
            };

        public AchievementSystem(GameServices services) => _services = services;

        public bool IsClaimed(AchievementId id) =>
            _services.Data.claimedAchievements.Contains(id.ToString());

        public void Evaluate()
        {
            TryUnlock(AchievementId.FirstRun, _services.Data.gamesPlayed >= 1);
            TryUnlock(AchievementId.CoinCollector, _services.Data.totalCoinsCollected >= 500);
            TryUnlock(AchievementId.DistanceRunner, _services.Data.totalDistanceRun >= 5000);
            TryUnlock(AchievementId.WaveClearer, _services.Data.totalWavesCleared >= 25);
            TryUnlock(AchievementId.TowerMaster, _services.Data.totalEnemiesDefeated >= 100);
            TryUnlock(AchievementId.DailyStreak3, _services.Data.dailyRewardStreak >= 3);
        }

        public void NotifyBossKilled() => TryUnlock(AchievementId.BossSlayer, true);
        public void NotifyDailyClaim() => Evaluate();
        public void NotifyCosmeticUnlocked() => TryUnlock(AchievementId.Spender, true);

        void TryUnlock(AchievementId id, bool condition)
        {
            if (!condition || IsClaimed(id)) return;
            _services.Data.claimedAchievements.Add(id.ToString());
            var def = Defs[id];
            _services.Currency.Add(CurrencyType.Coins, def.coins, persist: false);
            _services.XP.AddXp(def.xp);
            GameEvents.AchievementUnlocked(id);
            Debug.Log($"Achievement unlocked: {def.name}");
        }

        public IEnumerable<(AchievementId id, string name, string desc, bool claimed)> All()
        {
            foreach (var kv in Defs)
                yield return (kv.Key, kv.Value.name, kv.Value.desc, IsClaimed(kv.Key));
        }
    }
}
