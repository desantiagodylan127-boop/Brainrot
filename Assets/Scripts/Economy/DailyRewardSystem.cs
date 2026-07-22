using System;
using UnityEngine;

namespace BrainrotRush
{
    public class DailyRewardSystem
    {
        readonly GameServices _services;

        static readonly int[] Rewards = { 100, 150, 200, 250, 300, 400, 500 };

        public DailyRewardSystem(GameServices services) => _services = services;

        public int Streak => _services.Data.dailyRewardStreak;
        public bool CanClaim => !_services.Data.claimedDailyRewardToday;
        public int TodayReward => Rewards[Mathf.Clamp(Streak, 0, Rewards.Length - 1)];

        public void CheckReset()
        {
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            if (_services.Data.lastDailyRewardDate == today) return;

            if (!string.IsNullOrEmpty(_services.Data.lastDailyRewardDate))
            {
                if (DateTime.TryParse(_services.Data.lastDailyRewardDate, out var last))
                {
                    var gap = (DateTime.UtcNow.Date - last.Date).Days;
                    if (gap > 1)
                        _services.Data.dailyRewardStreak = 0;
                }
            }

            _services.Data.claimedDailyRewardToday = false;
        }

        public bool Claim()
        {
            if (!CanClaim) return false;

            int reward = TodayReward;
            int streakBonusGems = Streak >= 6 ? 5 : 0;

            _services.Currency.Add(CurrencyType.Coins, reward, persist: false);
            if (streakBonusGems > 0)
                _services.Currency.Add(CurrencyType.Gems, streakBonusGems, persist: false);

            _services.Data.dailyRewardStreak = Mathf.Min(Streak + 1, Rewards.Length);
            _services.Data.lastDailyRewardDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            _services.Data.claimedDailyRewardToday = true;
            _services.Achievements.NotifyDailyClaim();
            _services.Persist();
            return true;
        }
    }
}
