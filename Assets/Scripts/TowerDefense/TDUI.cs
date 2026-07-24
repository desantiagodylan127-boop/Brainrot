using UnityEngine;
using UnityEngine.UI;

namespace BrainrotRush
{
    public class TDUI : MonoBehaviour
    {
        TDGameManager _mgr;
        Text _hud;
        Text _message;
        GameObject _nextWavePanel;
        GameObject _towerActions;
        GameObject _pausePanel;
        GameObject _endPanel;
        Text _endTitle;
        Text _endStats;
        Text _towerInfo;
        string _rewardSummary;

        public void Bind(TDGameManager mgr)
        {
            _mgr = mgr;
            Build();
            GameEvents.OnTdCurrencyChanged += OnCurrency;
            GameEvents.OnWaveStarted += OnWave;
            RefreshHud();
        }

        void OnDestroy()
        {
            GameEvents.OnTdCurrencyChanged -= OnCurrency;
            GameEvents.OnWaveStarted -= OnWave;
        }

        void OnCurrency(int _) => RefreshHud();
        void OnWave(int _) => RefreshHud();

        void Build()
        {
            var canvas = UIFactory.CreateCanvas("TDUI");

            _hud = UIFactory.CreateText(canvas.transform, "HUD", "", 26, TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -30f), new Vector2(700f, 120f));

            _message = UIFactory.CreateText(canvas.transform, "Msg", "", 30, TextAnchor.UpperCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -140f), new Vector2(600f, 50f));
            _message.color = new Color(1f, 0.9f, 0.4f);

            // Tower build buttons
            float y = -220f;
            CreateTowerButton(canvas.transform, "Rapid", TowerType.RapidFire, new Vector2(-360f, y));
            CreateTowerButton(canvas.transform, "Cannon", TowerType.Cannon, new Vector2(-120f, y));
            CreateTowerButton(canvas.transform, "Freeze", TowerType.Freeze, new Vector2(120f, y));
            CreateTowerButton(canvas.transform, "Laser", TowerType.Laser, new Vector2(360f, y));

            UIFactory.CreateButton(canvas.transform, "Pause", "II", () => _mgr.Pause(),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -40f), new Vector2(70f, 70f));

            _nextWavePanel = new GameObject("NextWave");
            _nextWavePanel.transform.SetParent(canvas.transform, false);
            var nrt = _nextWavePanel.AddComponent<RectTransform>();
            nrt.anchorMin = nrt.anchorMax = new Vector2(0.5f, 0.15f);
            nrt.sizeDelta = new Vector2(320f, 80f);
            UIFactory.CreateButton(_nextWavePanel.transform, "StartWave", "Start Wave", () => _mgr.StartWave(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(300f, 70f),
                new Color(0.9f, 0.55f, 0.15f));

            _towerActions = new GameObject("TowerActions");
            _towerActions.transform.SetParent(canvas.transform, false);
            var tart = _towerActions.AddComponent<RectTransform>();
            tart.anchorMin = tart.anchorMax = new Vector2(0.5f, 0.28f);
            tart.sizeDelta = new Vector2(500f, 140f);
            _towerInfo = UIFactory.CreateText(_towerActions.transform, "Info", "", 22, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 40f), new Vector2(480f, 40f));
            UIFactory.CreateButton(_towerActions.transform, "Upgrade", "Upgrade", () => _mgr.UpgradeSelected(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-120f, -30f), new Vector2(200f, 60f));
            UIFactory.CreateButton(_towerActions.transform, "Sell", "Sell", () => _mgr.SellSelected(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(120f, -30f), new Vector2(200f, 60f),
                new Color(0.8f, 0.3f, 0.3f));
            _towerActions.SetActive(false);

            _pausePanel = UIFactory.CreatePanel(canvas.transform, "Pause", new Color(0f, 0f, 0f, 0.7f));
            UIFactory.CreateText(_pausePanel.transform, "T", "PAUSED", 48, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 80f), new Vector2(400f, 60f));
            UIFactory.CreateButton(_pausePanel.transform, "Resume", "Resume", () => _mgr.Resume(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(260f, 70f));
            UIFactory.CreateButton(_pausePanel.transform, "Quit", "Quit", () => _mgr.ReturnToMenu(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -90f), new Vector2(260f, 70f));
            _pausePanel.SetActive(false);

            _endPanel = UIFactory.CreatePanel(canvas.transform, "End", new Color(0.05f, 0.08f, 0.12f, 0.9f));
            _endTitle = UIFactory.CreateText(_endPanel.transform, "EndTitle", "", 52, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 140f), new Vector2(600f, 70f));
            _endStats = UIFactory.CreateText(_endPanel.transform, "EndStats", "", 28, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 50f), new Vector2(500f, 100f));
            UIFactory.CreateButton(_endPanel.transform, "Retry", "Retry", () => _mgr.Restart(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -40f), new Vector2(260f, 70f));
            UIFactory.CreateButton(_endPanel.transform, "Menu", "Menu", () => _mgr.ReturnToMenu(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -130f), new Vector2(260f, 70f));
            _endPanel.SetActive(false);
        }

        void CreateTowerButton(Transform parent, string label, TowerType type, Vector2 pos)
        {
            int cost = Tower.BaseCost(type);
            UIFactory.CreateButton(parent, label, $"{label}\n${cost}", () =>
            {
                _mgr.SelectTowerType(type);
                FlashMessage($"Selected {label}");
                RefreshSelection();
            }, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), pos, new Vector2(200f, 80f));
        }

        public void RefreshHud()
        {
            if (_hud == null || _mgr == null) return;
            _hud.text = $"Wave {_mgr.CurrentWave}/{TDGameManager.MaxWaves}\nLives: {_mgr.Lives}\nScrap: {_mgr.TdCurrency}";
        }

        public void RefreshSelection() { }

        public void ShowNextWave(bool show) => _nextWavePanel.SetActive(show);

        public void ShowTowerActions(bool show, Tower tower = null)
        {
            _towerActions.SetActive(show);
            if (show && tower != null)
                _towerInfo.text = $"{tower.Type} Lv{tower.Level}  |  Upgrade ${tower.UpgradeCost}  |  Sell ${tower.SellValue}";
        }

        public void ShowPause(bool show) => _pausePanel.SetActive(show);

        public void ShowEnd(bool victory)
        {
            _endPanel.SetActive(true);
            _endTitle.text = victory ? "VICTORY!" : "DEFEAT";
            _endTitle.color = victory ? new Color(0.4f, 1f, 0.5f) : new Color(1f, 0.4f, 0.4f);
            _endStats.text = $"Waves: {_mgr.CurrentWave}\n{_rewardSummary}";
        }

        public void SetRewardSummary(int coins, bool won) =>
            _rewardSummary = $"Coins earned: {coins}" + (won ? "\nBoss rush cleared!" : "");

        public void FlashMessage(string msg)
        {
            _message.text = msg;
            CancelInvoke(nameof(ClearMessage));
            Invoke(nameof(ClearMessage), 1.5f);
        }

        void ClearMessage() => _message.text = "";
    }
}
