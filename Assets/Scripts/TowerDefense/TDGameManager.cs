using System.Collections.Generic;
using UnityEngine;

namespace BrainrotRush
{
    public class TDGameManager : MonoBehaviour
    {
        public const int MaxLives = 20;
        public const int MaxWaves = 15;
        public const int StartingCurrency = 120;

        public int Lives { get; private set; }
        public int TdCurrency { get; private set; }
        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }
        public TowerType? SelectedTowerType { get; private set; }
        public BuildSpot SelectedSpot { get; private set; }

        WaveSpawner _spawner;
        EnemyPath _path;
        TDUI _ui;
        readonly List<BuildSpot> _spots = new List<BuildSpot>();
        int _enemiesKilled;
        bool _waitingForNextWave;
        bool _victory;

        public void Begin()
        {
            GameServices.Instance.CurrentMode = GameMode.TowerDefense;
            Lives = MaxLives;
            TdCurrency = StartingCurrency;
            IsPlaying = true;
            IsPaused = false;
            _enemiesKilled = 0;
            _waitingForNextWave = true;
            _victory = false;
            Time.timeScale = 1f;
            BuildArena();
            GameEvents.TdCurrencyChanged(TdCurrency);
        }

        void BuildArena()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
                camGo.AddComponent<AudioListener>();
            }
            cam.transform.position = new Vector3(1f, 16f, -10f);
            cam.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
            cam.backgroundColor = new Color(0.2f, 0.55f, 0.35f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.orthographic = false;

            if (FindObjectOfType<Light>() == null)
            {
                var lightGo = new GameObject("Directional Light");
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            // Ground
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "TDGround";
            ground.transform.position = new Vector3(0.5f, -0.5f, 0.5f);
            ground.transform.localScale = new Vector3(22f, 1f, 16f);
            ground.GetComponent<Renderer>().material.color = new Color(0.22f, 0.45f, 0.28f);

            var pathGo = new GameObject("EnemyPath");
            _path = pathGo.AddComponent<EnemyPath>();
            _path.SetupDefault();

            // Build spots along the path sides
            Vector3[] spotPositions =
            {
                new Vector3(-5.5f, 0.15f, 2f),
                new Vector3(-5.5f, 0.15f, -1.5f),
                new Vector3(-0.2f, 0.15f, 1.5f),
                new Vector3(-0.2f, 0.15f, -4.2f),
                new Vector3(1.8f, 0.15f, -4.2f),
                new Vector3(6.2f, 0.15f, -0.5f),
                new Vector3(6.2f, 0.15f, 2.5f),
                new Vector3(1.5f, 0.15f, 3.2f),
            };

            foreach (var pos in spotPositions)
            {
                var spotGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                spotGo.name = "BuildSpot";
                spotGo.transform.position = pos;
                spotGo.transform.localScale = new Vector3(1.4f, 0.15f, 1.4f);
                var spot = spotGo.AddComponent<BuildSpot>();
                spot.Init(this);
                _spots.Add(spot);
            }

            // Base marker at path end
            var baseGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseGo.name = "Base";
            baseGo.transform.position = _path.GetPoint(_path.Waypoints.Length - 1) + Vector3.up * 0.5f;
            baseGo.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            baseGo.GetComponent<Renderer>().material.color = new Color(0.95f, 0.85f, 0.3f);

            _spawner = gameObject.AddComponent<WaveSpawner>();
            _spawner.Init(this, _path);

            _ui = gameObject.AddComponent<TDUI>();
            _ui.Bind(this);
        }

        void Update()
        {
            if (!IsPlaying || IsPaused) return;

            HandleBuildTap();

            if (!_waitingForNextWave && _spawner.WaveCleared)
                OnWaveCleared();
        }

        void HandleBuildTap()
        {
            bool tapped = false;
            Vector3 screenPos = Vector3.zero;

            if (Input.GetMouseButtonDown(0))
            {
                tapped = true;
                screenPos = Input.mousePosition;
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                tapped = true;
                screenPos = Input.GetTouch(0).position;
            }

            if (!tapped || Camera.main == null) return;

            // Ignore taps on UI
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;
            if (Input.touchCount > 0 &&
                UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            var ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit, 200f))
            {
                var spot = hit.collider.GetComponent<BuildSpot>();
                if (spot != null) OnBuildSpotClicked(spot);
            }
        }

        void OnWaveCleared()
        {
            _waitingForNextWave = true;
            GameEvents.WaveCompleted(_spawner.CurrentWave);
            GameServices.Instance.Data.totalWavesCleared++;
            GameServices.Instance.Missions.Report(MissionType.CompleteWaves);
            GameServices.Instance.Persist();

            if (_spawner.CurrentWave >= MaxWaves)
            {
                Victory();
                return;
            }

            _ui.ShowNextWave(true);
        }

        public void StartWave()
        {
            if (!IsPlaying || !_waitingForNextWave) return;
            _waitingForNextWave = false;
            _ui.ShowNextWave(false);
            _spawner.StartNextWave();
        }

        public void SelectTowerType(TowerType type)
        {
            SelectedTowerType = type;
            SelectedSpot = null;
            _ui.ShowTowerActions(false);
        }

        public void OnBuildSpotClicked(BuildSpot spot)
        {
            if (!IsPlaying || IsPaused) return;

            if (spot.IsOccupied)
            {
                SelectedSpot = spot;
                SelectedTowerType = null;
                _ui.ShowTowerActions(true, spot.OccupyingTower);
                return;
            }

            if (SelectedTowerType == null) return;

            int cost = Tower.BaseCost(SelectedTowerType.Value);
            if (TdCurrency < cost)
            {
                _ui.FlashMessage("Not enough scrap!");
                return;
            }

            PlaceTower(spot, SelectedTowerType.Value, cost);
        }

        void PlaceTower(BuildSpot spot, TowerType type, int cost)
        {
            TdCurrency -= cost;
            GameEvents.TdCurrencyChanged(TdCurrency);

            var skin = GameServices.Instance.Unlocks.Get(
                GameServices.Instance.Unlocks.GetEquipped(CosmeticType.TowerSkin));
            Color tint = skin != null ? skin.tint : Color.white;

            var towerGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            towerGo.name = type + "Tower";
            towerGo.transform.position = spot.transform.position + Vector3.up * 0.6f;
            towerGo.transform.localScale = new Vector3(1f, 0.7f, 1f);
            Destroy(towerGo.GetComponent<Collider>());
            var tower = towerGo.AddComponent<Tower>();
            tower.Setup(type, tint);
            spot.Place(tower);
            SelectedTowerType = null;
            _ui.RefreshSelection();
        }

        public void UpgradeSelected()
        {
            if (SelectedSpot == null || !SelectedSpot.IsOccupied) return;
            var tower = SelectedSpot.OccupyingTower;
            if (TdCurrency < tower.UpgradeCost)
            {
                _ui.FlashMessage("Not enough scrap!");
                return;
            }
            TdCurrency -= tower.UpgradeCost;
            tower.Upgrade();
            GameEvents.TdCurrencyChanged(TdCurrency);
            _ui.ShowTowerActions(true, tower);
        }

        public void SellSelected()
        {
            if (SelectedSpot == null || !SelectedSpot.IsOccupied) return;
            var tower = SelectedSpot.OccupyingTower;
            TdCurrency += tower.SellValue;
            Destroy(tower.gameObject);
            SelectedSpot.Clear();
            SelectedSpot = null;
            GameEvents.TdCurrencyChanged(TdCurrency);
            _ui.ShowTowerActions(false);
        }

        public void OnEnemyKilled(Enemy enemy)
        {
            _spawner.NotifyEnemyRemoved();
            TdCurrency += enemy.Reward;
            _enemiesKilled++;
            GameServices.Instance.Data.totalEnemiesDefeated++;
            GameServices.Instance.Missions.Report(MissionType.DefeatEnemies);
            GameEvents.TdCurrencyChanged(TdCurrency);

            if (enemy.IsBoss)
                GameServices.Instance.Achievements.NotifyBossKilled();
        }

        public void OnEnemyReachedEnd(Enemy enemy)
        {
            _spawner.NotifyEnemyRemoved();
            Lives -= enemy.DamageToBase;
            _ui.RefreshHud();
            if (Lives <= 0) Defeat();
        }

        void Victory()
        {
            if (_victory) return;
            _victory = true;
            IsPlaying = false;
            Time.timeScale = 0f;
            GrantRewards(won: true);
            GameEvents.TdVictory();
            _ui.ShowEnd(true);
        }

        void Defeat()
        {
            IsPlaying = false;
            Time.timeScale = 0f;
            GrantRewards(won: false);
            GameEvents.TdDefeat();
            _ui.ShowEnd(false);
        }

        void GrantRewards(bool won)
        {
            var s = GameServices.Instance;
            s.Data.gamesPlayed++;
            int coins = 40 + _enemiesKilled * 2 + _spawner.CurrentWave * 15;
            if (won) coins += 200;
            s.Currency.Add(CurrencyType.Coins, coins, persist: false);
            s.XP.AddXp(20 + _spawner.CurrentWave * 10 + (won ? 50 : 0));
            s.Missions.Report(MissionType.PlayGames);
            s.Achievements.Evaluate();
            s.Persist();
            _ui.SetRewardSummary(coins, won);
        }

        public void Pause()
        {
            if (!IsPlaying) return;
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

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            GameServices.Instance.CurrentMode = GameMode.None;
            Bootstrap.AppBootstrap.LoadMenu();
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            Bootstrap.AppBootstrap.LoadTowerDefense();
        }

        public int CurrentWave => _spawner != null ? _spawner.CurrentWave : 0;
    }
}
