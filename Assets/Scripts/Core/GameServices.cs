using UnityEngine;

namespace BrainrotRush
{
    /// <summary>
    /// Persistent service hub for shared progression across both game modes.
    /// </summary>
    public class GameServices : MonoBehaviour
    {
        public static GameServices Instance { get; private set; }

        public PlayerData Data { get; private set; }
        public CurrencyManager Currency { get; private set; }
        public XPManager XP { get; private set; }
        public UnlockManager Unlocks { get; private set; }
        public DailyRewardSystem DailyRewards { get; private set; }
        public MissionSystem Missions { get; private set; }
        public AchievementSystem Achievements { get; private set; }
        public BattlePassSystem BattlePass { get; private set; }
        public AdManager Ads { get; private set; }
        public IAPManager IAP { get; private set; }

        public GameMode CurrentMode { get; set; } = GameMode.None;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            if (Instance != null) return;
            var go = new GameObject("GameServices");
            DontDestroyOnLoad(go);
            go.AddComponent<GameServices>();
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Data = SaveSystem.Load();
            Currency = new CurrencyManager(this);
            XP = new XPManager(this);
            Unlocks = new UnlockManager(this);
            DailyRewards = new DailyRewardSystem(this);
            Missions = new MissionSystem(this);
            Achievements = new AchievementSystem(this);
            BattlePass = new BattlePassSystem(this);
            Ads = gameObject.AddComponent<AdManager>();
            IAP = gameObject.AddComponent<IAPManager>();

            DailyRewards.CheckReset();
            Missions.EnsureDailyMissions();
            Persist();
        }

        public void Persist()
        {
            SaveSystem.Save(Data);
            GameEvents.PlayerDataChanged();
        }

        public void ResetProgress()
        {
            Data = new PlayerData();
            Missions.EnsureDailyMissions(force: true);
            Persist();
        }

        void OnApplicationPause(bool pause)
        {
            if (pause) Persist();
        }

        void OnApplicationQuit() => Persist();
    }
}
