using UnityEngine;
using UnityEngine.UI;

namespace BrainrotRush
{
    public class RunnerUI : MonoBehaviour
    {
        RunnerGameManager _mgr;
        Text _scoreText;
        Text _coinText;
        Text _speedHint;
        GameObject _pausePanel;
        GameObject _gameOverPanel;
        Text _gameOverStats;

        public void Bind(RunnerGameManager mgr)
        {
            _mgr = mgr;
            Build();
            GameEvents.OnRunnerScoreChanged += OnScore;
        }

        void OnDestroy()
        {
            GameEvents.OnRunnerScoreChanged -= OnScore;
        }

        void Build()
        {
            var canvas = UIFactory.CreateCanvas("RunnerUI");

            _scoreText = UIFactory.CreateText(canvas.transform, "Score", "0", 42, TextAnchor.UpperCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(400f, 60f));

            _coinText = UIFactory.CreateText(canvas.transform, "Coins", "Coins: 0", 28, TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -40f), new Vector2(300f, 40f));

            _speedHint = UIFactory.CreateText(canvas.transform, "Hint", "Swipe to dodge!", 22, TextAnchor.LowerCenter,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(500f, 40f));
            _speedHint.color = new Color(1f, 1f, 1f, 0.7f);

            UIFactory.CreateButton(canvas.transform, "PauseBtn", "II", () => _mgr.Pause(),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-30f, -40f), new Vector2(70f, 70f));

            _pausePanel = UIFactory.CreatePanel(canvas.transform, "PausePanel", new Color(0f, 0f, 0f, 0.7f));
            UIFactory.CreateText(_pausePanel.transform, "PauseTitle", "PAUSED", 48, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 120f), new Vector2(400f, 60f));
            UIFactory.CreateButton(_pausePanel.transform, "Resume", "Resume", () => _mgr.Resume(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 20f), new Vector2(280f, 70f));
            UIFactory.CreateButton(_pausePanel.transform, "Quit", "Quit", () => _mgr.ReturnToMenu(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -70f), new Vector2(280f, 70f));
            _pausePanel.SetActive(false);

            _gameOverPanel = UIFactory.CreatePanel(canvas.transform, "GameOverPanel", new Color(0.05f, 0.05f, 0.1f, 0.85f));
            UIFactory.CreateText(_gameOverPanel.transform, "GOTitle", "GAME OVER", 52, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 180f), new Vector2(500f, 70f));
            _gameOverStats = UIFactory.CreateText(_gameOverPanel.transform, "GOStats", "", 28, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 90f), new Vector2(500f, 100f));
            UIFactory.CreateButton(_gameOverPanel.transform, "Retry", "Retry", () => _mgr.Restart(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(280f, 70f));
            UIFactory.CreateButton(_gameOverPanel.transform, "Double", "2x Coins (Ad)", () => _mgr.DoubleCoinsReward(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -80f), new Vector2(280f, 70f));
            UIFactory.CreateButton(_gameOverPanel.transform, "ExtraLife", "Extra Life (Ad)", () => _mgr.OfferExtraLife(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -160f), new Vector2(280f, 70f));
            UIFactory.CreateButton(_gameOverPanel.transform, "Menu", "Menu", () => _mgr.ReturnToMenu(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -240f), new Vector2(280f, 70f));
            _gameOverPanel.SetActive(false);
        }

        void OnScore(int score)
        {
            if (_scoreText) _scoreText.text = score.ToString();
            if (_coinText) _coinText.text = $"Coins: {_mgr.CoinsThisRun}";
        }

        public void ShowPause(bool show) => _pausePanel.SetActive(show);

        public void ShowGameOver(bool show)
        {
            _gameOverPanel.SetActive(show);
            if (show) RefreshGameOver();
        }

        public void RefreshGameOver()
        {
            var high = GameServices.Instance.Data.runnerHighScore;
            _gameOverStats.text = $"Score: {_mgr.Score}\nDistance: {Mathf.FloorToInt(_mgr.Distance)}\nCoins: {_mgr.CoinsThisRun}\nBest: {high}";
        }
    }
}
