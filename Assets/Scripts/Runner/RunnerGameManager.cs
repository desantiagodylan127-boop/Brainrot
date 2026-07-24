using UnityEngine;

namespace BrainrotRush
{
    public class RunnerGameManager : MonoBehaviour
    {
        public float BaseSpeed = 8f;
        public float MaxSpeed = 22f;
        public float SpeedRampPerSecond = 0.12f;

        public float Distance { get; private set; }
        public int CoinsThisRun { get; private set; }
        public int Score => Mathf.FloorToInt(Distance) + CoinsThisRun * 5;
        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsGameOver { get; private set; }

        RunnerPlayer _player;
        LevelGenerator _generator;
        Transform _worldRoot;
        RunnerUI _ui;
        float _runTime;
        bool _extraLifeUsed;

        public void Begin()
        {
            GameServices.Instance.CurrentMode = GameMode.Runner;
            BuildWorld();
            IsPlaying = true;
            IsPaused = false;
            IsGameOver = false;
            Distance = 0f;
            CoinsThisRun = 0;
            _runTime = 0f;
            _extraLifeUsed = false;
            Time.timeScale = 1f;
        }

        void BuildWorld()
        {
            // Camera
            var cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
                camGo.AddComponent<AudioListener>();
            }
            cam.transform.position = new Vector3(0f, 6f, -8f);
            cam.transform.rotation = Quaternion.Euler(25f, 0f, 0f);
            cam.backgroundColor = new Color(0.45f, 0.7f, 0.95f);
            cam.clearFlags = CameraClearFlags.SolidColor;

            // Light
            if (FindObjectOfType<Light>() == null)
            {
                var lightGo = new GameObject("Directional Light");
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            _worldRoot = new GameObject("RunnerWorld").transform;

            var playerGo = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerGo.name = "RunnerPlayer";
            playerGo.transform.position = new Vector3(0f, 0.5f, 0f);
            var rb = playerGo.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            playerGo.GetComponent<Collider>().isTrigger = false;
            // Make player collider trigger-friendly for pickups: use a child trigger? 
            // Obstacles/coins use OnTriggerEnter on themselves checking player collider - non-trigger player works with trigger others.
            _player = playerGo.AddComponent<RunnerPlayer>();

            var input = gameObject.AddComponent<SwipeInput>();
            _player.Init(input);

            _generator = gameObject.AddComponent<LevelGenerator>();
            _generator.Init(_worldRoot);

            _ui = gameObject.AddComponent<RunnerUI>();
            _ui.Bind(this);
        }

        void Update()
        {
            if (!IsPlaying || IsPaused || IsGameOver) return;

            _runTime += Time.deltaTime;
            float difficulty = Mathf.Clamp01(_runTime / 90f);
            float speed = Mathf.Lerp(BaseSpeed, MaxSpeed, difficulty) * _player.SpeedMultiplier;

            _generator.Tick(speed, difficulty);
            Distance += speed * Time.deltaTime;
            GameEvents.RunnerScoreChanged(Score);
        }

        public void CollectCoin(int value)
        {
            CoinsThisRun += value;
            GameServices.Instance.Missions.Report(MissionType.CollectCoins, value);
        }

        public void OnPlayerHit()
        {
            if (IsGameOver || !IsPlaying) return;
            if (!_player.TryHit()) return;

            if (!_extraLifeUsed && GameServices.Instance.Data.hasExtraLifePending)
            {
                GameServices.Instance.Data.hasExtraLifePending = false;
                GameServices.Instance.Persist();
                _extraLifeUsed = true;
                _player.ActivatePowerup(PowerupType.Shield, 2f);
                return;
            }

            GameOver();
        }

        public void Pause()
        {
            if (!IsPlaying || IsGameOver) return;
            IsPaused = true;
            Time.timeScale = 0f;
            _ui.ShowPause(true);
        }

        public void Resume()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            _ui.ShowPause(false);
        }

        public void OfferExtraLife()
        {
            if (_extraLifeUsed) return;
            GameServices.Instance.Ads.ExtraLife(() =>
            {
                _extraLifeUsed = true;
                IsGameOver = false;
                IsPlaying = true;
                Time.timeScale = 1f;
                _player.ActivatePowerup(PowerupType.Shield, 3f);
                _ui.ShowGameOver(false);
            });
        }

        void GameOver()
        {
            IsGameOver = true;
            IsPlaying = false;
            Time.timeScale = 0f;

            var s = GameServices.Instance;
            s.Data.gamesPlayed++;
            s.Data.totalDistanceRun += Mathf.FloorToInt(Distance);
            if (Score > s.Data.runnerHighScore)
                s.Data.runnerHighScore = Score;

            s.Currency.Add(CurrencyType.Coins, CoinsThisRun, persist: false);
            int xp = 10 + Mathf.FloorToInt(Distance / 20f) + CoinsThisRun;
            s.XP.AddXp(xp);
            s.Missions.Report(MissionType.RunDistance, Mathf.FloorToInt(Distance));
            s.Missions.Report(MissionType.PlayGames);
            s.Achievements.Evaluate();
            s.Persist();

            GameEvents.RunnerGameOver();
            _ui.ShowGameOver(true);
        }

        public void DoubleCoinsReward()
        {
            GameServices.Instance.Ads.DoubleCoins(CoinsThisRun, _ =>
            {
                CoinsThisRun *= 2;
                _ui.RefreshGameOver();
            });
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            GameServices.Instance.CurrentMode = GameMode.None;
            Bootstrap.AppBootstrap.LoadMenu();
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            Bootstrap.AppBootstrap.LoadRunner();
        }
    }
}
