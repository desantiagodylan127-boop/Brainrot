using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrainrotRush
{
    public class MissionSystem
    {
        readonly GameServices _services;

        public MissionSystem(GameServices services) => _services = services;

        public IReadOnlyList<MissionProgress> Missions => _services.Data.dailyMissions;

        public void EnsureDailyMissions(bool force = false)
        {
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            if (!force && _services.Data.lastDailyResetDate == today && _services.Data.dailyMissions.Count > 0)
                return;

            _services.Data.lastDailyResetDate = today;
            _services.Data.dailyMissions = GenerateMissions();
            GameEvents.DailyMissionsUpdated();
            _services.Persist();
        }

        public void Report(MissionType type, int amount = 1)
        {
            bool changed = false;
            foreach (var m in _services.Data.dailyMissions)
            {
                if (m.type != type || m.claimed) continue;
                int before = m.current;
                m.current = Mathf.Min(m.target, m.current + amount);
                if (m.current != before) changed = true;
            }

            if (changed)
            {
                GameEvents.DailyMissionsUpdated();
                _services.Persist();
            }
        }

        public bool Claim(int index)
        {
            if (index < 0 || index >= _services.Data.dailyMissions.Count) return false;
            var m = _services.Data.dailyMissions[index];
            if (!m.IsComplete || m.claimed) return false;

            m.claimed = true;
            _services.Currency.Add(CurrencyType.Coins, m.rewardCoins, persist: false);
            _services.XP.AddXp(m.rewardXp);
            GameEvents.DailyMissionsUpdated();
            return true;
        }

        static List<MissionProgress> GenerateMissions()
        {
            return new List<MissionProgress>
            {
                new MissionProgress { type = MissionType.CollectCoins, target = 50, rewardCoins = 100, rewardXp = 30 },
                new MissionProgress { type = MissionType.RunDistance, target = 500, rewardCoins = 120, rewardXp = 40 },
                new MissionProgress { type = MissionType.DefeatEnemies, target = 20, rewardCoins = 150, rewardXp = 50 },
                new MissionProgress { type = MissionType.CompleteWaves, target = 3, rewardCoins = 180, rewardXp = 60 },
                new MissionProgress { type = MissionType.PlayGames, target = 2, rewardCoins = 80, rewardXp = 25 },
            };
        }
    }
}
