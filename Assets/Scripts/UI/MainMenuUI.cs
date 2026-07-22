using UnityEngine;
using UnityEngine.UI;

namespace BrainrotRush
{
    public class MainMenuUI : MonoBehaviour
    {
        Text _currencyText;
        Text _levelText;
        GameObject _missionsPanel;
        GameObject _achievementsPanel;
        GameObject _shopPanel;
        GameObject _cosmeticsPanel;
        GameObject _battlePassPanel;
        GameObject _dailyPanel;

        public void Show()
        {
            BuildBackdrop();
            BuildUI();
            RefreshHeader();
            GameEvents.OnPlayerDataChanged += RefreshHeader;
            GameEvents.OnCurrencyChanged += OnCurrencyChanged;
        }

        void OnDestroy()
        {
            GameEvents.OnPlayerDataChanged -= RefreshHeader;
            GameEvents.OnCurrencyChanged -= OnCurrencyChanged;
        }

        void OnCurrencyChanged(CurrencyType type, int amount, int total) => RefreshHeader();

        void BuildBackdrop()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
                camGo.AddComponent<AudioListener>();
            }
            cam.transform.position = new Vector3(0f, 1f, -10f);
            cam.backgroundColor = new Color(0.08f, 0.12f, 0.18f);
            cam.clearFlags = CameraClearFlags.SolidColor;

            // Decorative floating cubes
            for (int i = 0; i < 12; i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Decor";
                float x = Random.Range(-8f, 8f);
                float y = Random.Range(-4f, 4f);
                cube.transform.position = new Vector3(x, y, Random.Range(2f, 8f));
                cube.transform.localScale = Vector3.one * Random.Range(0.3f, 1.2f);
                cube.transform.rotation = Random.rotation;
                cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(Random.value, 0.55f, 0.7f);
                Destroy(cube.GetComponent<Collider>());
                cube.AddComponent<MenuFloater>();
            }
        }

        void BuildUI()
        {
            var canvas = UIFactory.CreateCanvas("MainMenu");

            // Soft gradient-ish overlay
            var bg = UIFactory.CreatePanel(canvas.transform, "Dim", new Color(0.05f, 0.07f, 0.12f, 0.55f));
            bg.transform.SetAsFirstSibling();

            UIFactory.CreateText(canvas.transform, "Title", "BRAINROT RUSH", 64, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -160f), new Vector2(900f, 90f));

            UIFactory.CreateText(canvas.transform, "Subtitle", "Run wild. Defend harder.", 28, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -230f), new Vector2(700f, 40f))
                .color = new Color(0.7f, 0.85f, 1f, 0.85f);

            _currencyText = UIFactory.CreateText(canvas.transform, "Currency", "", 26, TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -30f), new Vector2(500f, 70f));

            _levelText = UIFactory.CreateText(canvas.transform, "Level", "", 24, TextAnchor.UpperRight,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -30f), new Vector2(280f, 50f));

            UIFactory.CreateButton(canvas.transform, "Runner", "ENDLESS RUNNER", () => Bootstrap.AppBootstrap.LoadRunner(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 80f), new Vector2(420f, 90f),
                new Color(0.15f, 0.75f, 0.55f));

            UIFactory.CreateButton(canvas.transform, "TD", "TOWER DEFENSE", () => Bootstrap.AppBootstrap.LoadTowerDefense(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -30f), new Vector2(420f, 90f),
                new Color(0.9f, 0.5f, 0.2f));

            // Meta row
            float metaY = -180f;
            UIFactory.CreateButton(canvas.transform, "Daily", "Daily", () => Toggle(_dailyPanel),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-320f, metaY), new Vector2(140f, 60f),
                new Color(0.35f, 0.45f, 0.7f));
            UIFactory.CreateButton(canvas.transform, "Missions", "Missions", () => Toggle(_missionsPanel),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-160f, metaY), new Vector2(140f, 60f),
                new Color(0.35f, 0.45f, 0.7f));
            UIFactory.CreateButton(canvas.transform, "Shop", "Shop", () => Toggle(_shopPanel),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, metaY), new Vector2(140f, 60f),
                new Color(0.35f, 0.45f, 0.7f));
            UIFactory.CreateButton(canvas.transform, "Looks", "Looks", () => Toggle(_cosmeticsPanel),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(160f, metaY), new Vector2(140f, 60f),
                new Color(0.35f, 0.45f, 0.7f));
            UIFactory.CreateButton(canvas.transform, "Pass", "Pass", () => Toggle(_battlePassPanel),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(320f, metaY), new Vector2(140f, 60f),
                new Color(0.35f, 0.45f, 0.7f));

            UIFactory.CreateButton(canvas.transform, "Achievements", "Achievements", () => Toggle(_achievementsPanel),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, metaY - 80f), new Vector2(220f, 55f),
                new Color(0.3f, 0.35f, 0.5f));

            UIFactory.CreateButton(canvas.transform, "Chest", "Bonus Chest (Ad)", () =>
            {
                GameServices.Instance.Ads.BonusChest(RefreshHeader);
            }, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 60f), new Vector2(320f, 60f),
                new Color(0.7f, 0.45f, 0.15f));

            _dailyPanel = MetaPanels.BuildDaily(canvas.transform, CloseAll);
            _missionsPanel = MetaPanels.BuildMissions(canvas.transform, CloseAll);
            _shopPanel = MetaPanels.BuildShop(canvas.transform, CloseAll, RefreshHeader);
            _cosmeticsPanel = MetaPanels.BuildCosmetics(canvas.transform, CloseAll, RefreshHeader);
            _battlePassPanel = MetaPanels.BuildBattlePass(canvas.transform, CloseAll, RefreshHeader);
            _achievementsPanel = MetaPanels.BuildAchievements(canvas.transform, CloseAll);
            CloseAll();
        }

        void Toggle(GameObject panel)
        {
            bool was = panel.activeSelf;
            CloseAll();
            panel.SetActive(!was);
            if (panel.activeSelf)
            {
                // rebuild dynamic content by enabling — panels refresh on enable via components
                var refreshable = panel.GetComponent<IMetaPanelRefresh>();
                refreshable?.Refresh();
            }
        }

        void CloseAll()
        {
            _dailyPanel.SetActive(false);
            _missionsPanel.SetActive(false);
            _shopPanel.SetActive(false);
            _cosmeticsPanel.SetActive(false);
            _battlePassPanel.SetActive(false);
            _achievementsPanel.SetActive(false);
        }

        void RefreshHeader()
        {
            if (_currencyText == null) return;
            var s = GameServices.Instance;
            _currencyText.text = $"Coins: {s.Currency.Coins}\nGems: {s.Currency.Gems}   Tokens: {s.Currency.UnlockTokens}";
            _levelText.text = $"Lv {s.XP.Level}\nBest Run: {s.Data.runnerHighScore}";
            _levelText.alignment = TextAnchor.UpperRight;
        }
    }

    public interface IMetaPanelRefresh
    {
        void Refresh();
    }

    public class MenuFloater : MonoBehaviour
    {
        Vector3 _spin;
        float _bob;
        Vector3 _origin;

        void Start()
        {
            _spin = Random.insideUnitSphere * 40f;
            _bob = Random.Range(0.5f, 1.5f);
            _origin = transform.position;
        }

        void Update()
        {
            transform.Rotate(_spin * Time.deltaTime);
            transform.position = _origin + Vector3.up * Mathf.Sin(Time.time * _bob) * 0.35f;
        }
    }
}
