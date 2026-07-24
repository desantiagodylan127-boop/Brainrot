using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrainrotRush
{
    [Serializable]
    public class PlayerData
    {
        public int coins = 500;
        public int gems = 20;
        public int unlockTokens = 0;
        public int xp;
        public int level = 1;

        public int runnerHighScore;
        public int totalDistanceRun;
        public int totalCoinsCollected;
        public int totalEnemiesDefeated;
        public int totalWavesCleared;
        public int gamesPlayed;

        public string equippedCharacterId = "char_default";
        public string equippedTrailId = "trail_none";
        public string equippedHatId = "hat_none";
        public string equippedEmoteId = "emote_wave";
        public string equippedTowerSkinId = "skin_default";

        public List<string> unlockedCosmetics = new List<string>
        {
            "char_default", "trail_none", "hat_none", "emote_wave", "skin_default"
        };

        public List<string> claimedAchievements = new List<string>();
        public List<MissionProgress> dailyMissions = new List<MissionProgress>();
        public string lastDailyResetDate = "";
        public int dailyRewardStreak;
        public string lastDailyRewardDate = "";
        public bool claimedDailyRewardToday;

        public int battlePassXp;
        public int battlePassTier;
        public bool battlePassPremium;
        public List<int> claimedFreeTiers = new List<int>();
        public List<int> claimedPremiumTiers = new List<int>();

        public bool adsRemoved;
        public bool hasExtraLifePending;

        public PlayerData Clone()
        {
            return JsonUtility.FromJson<PlayerData>(JsonUtility.ToJson(this));
        }
    }

    [Serializable]
    public class MissionProgress
    {
        public MissionType type;
        public int target;
        public int current;
        public int rewardCoins;
        public int rewardXp;
        public bool claimed;

        public bool IsComplete => current >= target;
    }
}
