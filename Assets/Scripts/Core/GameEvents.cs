using System;

namespace BrainrotRush
{
    /// <summary>
    /// Simple static event hub. No DI framework needed for a prototype.
    /// </summary>
    public static class GameEvents
    {
        public static event Action<CurrencyType, int, int> OnCurrencyChanged;
        public static event Action<int, int> OnXpChanged;
        public static event Action<int> OnLevelUp;
        public static event Action<string> OnCosmeticUnlocked;
        public static event Action OnPlayerDataChanged;
        public static event Action<AchievementId> OnAchievementUnlocked;
        public static event Action OnDailyMissionsUpdated;
        public static event Action OnBattlePassUpdated;

        public static event Action<int> OnRunnerScoreChanged;
        public static event Action OnRunnerGameOver;
        public static event Action<PowerupType> OnPowerupCollected;

        public static event Action<int> OnWaveStarted;
        public static event Action<int> OnWaveCompleted;
        public static event Action OnTdVictory;
        public static event Action OnTdDefeat;
        public static event Action<int> OnTdCurrencyChanged;
        public static event Action<Enemy> OnEnemyDefeated;

        public static void CurrencyChanged(CurrencyType type, int amount, int total) =>
            OnCurrencyChanged?.Invoke(type, amount, total);

        public static void XpChanged(int xp, int level) => OnXpChanged?.Invoke(xp, level);
        public static void LevelUp(int level) => OnLevelUp?.Invoke(level);
        public static void CosmeticUnlocked(string id) => OnCosmeticUnlocked?.Invoke(id);
        public static void PlayerDataChanged() => OnPlayerDataChanged?.Invoke();
        public static void AchievementUnlocked(AchievementId id) => OnAchievementUnlocked?.Invoke(id);
        public static void DailyMissionsUpdated() => OnDailyMissionsUpdated?.Invoke();
        public static void BattlePassUpdated() => OnBattlePassUpdated?.Invoke();

        public static void RunnerScoreChanged(int score) => OnRunnerScoreChanged?.Invoke(score);
        public static void RunnerGameOver() => OnRunnerGameOver?.Invoke();
        public static void PowerupCollected(PowerupType type) => OnPowerupCollected?.Invoke(type);

        public static void WaveStarted(int wave) => OnWaveStarted?.Invoke(wave);
        public static void WaveCompleted(int wave) => OnWaveCompleted?.Invoke(wave);
        public static void TdVictory() => OnTdVictory?.Invoke();
        public static void TdDefeat() => OnTdDefeat?.Invoke();
        public static void TdCurrencyChanged(int amount) => OnTdCurrencyChanged?.Invoke(amount);
        public static void EnemyDefeated(Enemy enemy) => OnEnemyDefeated?.Invoke(enemy);

        public static void ClearModeEvents()
        {
            OnRunnerScoreChanged = null;
            OnRunnerGameOver = null;
            OnPowerupCollected = null;
            OnWaveStarted = null;
            OnWaveCompleted = null;
            OnTdVictory = null;
            OnTdDefeat = null;
            OnTdCurrencyChanged = null;
            OnEnemyDefeated = null;
        }

        public static void ClearAll()
        {
            OnCurrencyChanged = null;
            OnXpChanged = null;
            OnLevelUp = null;
            OnCosmeticUnlocked = null;
            OnPlayerDataChanged = null;
            OnAchievementUnlocked = null;
            OnDailyMissionsUpdated = null;
            OnBattlePassUpdated = null;
            ClearModeEvents();
        }
    }
}
