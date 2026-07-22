using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BrainrotRush
{
    public static class MetaPanels
    {
        public static GameObject BuildDaily(Transform parent, System.Action onClose)
        {
            var panel = CreateOverlay(parent, "DailyPanel", onClose);
            var body = panel.transform.Find("Body");
            var text = UIFactory.CreateText(body, "Info", "", 28, TextAnchor.UpperLeft,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 40f), new Vector2(700f, 200f));

            var refresh = panel.AddComponent<SimpleRefresh>();
            refresh.OnRefresh = () =>
            {
                var d = GameServices.Instance.DailyRewards;
                text.text = $"Daily Reward\n\nStreak: {d.Streak}\nToday: {d.TodayReward} coins" +
                            (d.CanClaim ? "\n\nReady to claim!" : "\n\nAlready claimed today.");
            };

            UIFactory.CreateButton(body, "Claim", "Claim", () =>
            {
                if (GameServices.Instance.DailyRewards.Claim())
                    refresh.Refresh();
            }, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -120f), new Vector2(260f, 70f));

            return panel;
        }

        public static GameObject BuildMissions(Transform parent, System.Action onClose)
        {
            var panel = CreateOverlay(parent, "MissionsPanel", onClose);
            var body = panel.transform.Find("Body");
            var text = UIFactory.CreateText(body, "List", "", 24, TextAnchor.UpperLeft,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 20f), new Vector2(750f, 420f));

            var refresh = panel.AddComponent<SimpleRefresh>();
            refresh.OnRefresh = () =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("Daily Missions\n");
                var missions = GameServices.Instance.Missions.Missions;
                for (int i = 0; i < missions.Count; i++)
                {
                    var m = missions[i];
                    string status = m.claimed ? "CLAIMED" : (m.IsComplete ? "READY" : $"{m.current}/{m.target}");
                    sb.AppendLine($"{i + 1}. {m.type}: {status}  (+{m.rewardCoins}c / {m.rewardXp}xp)");
                }
                text.text = sb.ToString();
            };

            for (int i = 0; i < 5; i++)
            {
                int index = i;
                UIFactory.CreateButton(body, $"Claim{i}", $"Claim {i + 1}", () =>
                {
                    GameServices.Instance.Missions.Claim(index);
                    refresh.Refresh();
                }, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-300f + i * 150f, 50f), new Vector2(130f, 50f),
                    new Color(0.25f, 0.55f, 0.4f));
            }

            return panel;
        }

        public static GameObject BuildShop(Transform parent, System.Action onClose, System.Action onChanged)
        {
            var panel = CreateOverlay(parent, "ShopPanel", onClose);
            var body = panel.transform.Find("Body");
            UIFactory.CreateText(body, "Title", "Shop / IAP", 36, TextAnchor.UpperCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(400f, 50f));

            float y = 120f;
            foreach (var product in IAPManager.Catalog)
            {
                var p = product;
                UIFactory.CreateButton(body, p.id, $"{p.displayName}\n{p.priceLabel}", () =>
                {
                    GameServices.Instance.IAP.Purchase(p.id, ok =>
                    {
                        if (ok) onChanged?.Invoke();
                    });
                }, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, y), new Vector2(420f, 70f),
                    new Color(0.4f, 0.35f, 0.65f));
                y -= 85f;
            }

            return panel;
        }

        public static GameObject BuildCosmetics(Transform parent, System.Action onClose, System.Action onChanged)
        {
            var panel = CreateOverlay(parent, "CosmeticsPanel", onClose);
            var body = panel.transform.Find("Body");
            var text = UIFactory.CreateText(body, "List", "", 22, TextAnchor.UpperLeft,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 40f), new Vector2(780f, 360f));

            var refresh = panel.AddComponent<SimpleRefresh>();
            refresh.OnRefresh = () =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("Cosmetics (no gameplay advantage)\n");
                foreach (var c in UnlockManager.Catalog)
                {
                    var u = GameServices.Instance.Unlocks;
                    bool unlocked = u.IsUnlocked(c.id);
                    bool eq = u.GetEquipped(c.type) == c.id;
                    string cost = c.tokenCost > 0 ? $"{c.tokenCost} tok"
                        : c.gemCost > 0 ? $"{c.gemCost} gems"
                        : c.coinCost > 0 ? $"{c.coinCost} coins" : "free";
                    sb.AppendLine($"{c.displayName} [{c.type}] {(eq ? "EQUIPPED" : unlocked ? "owned" : cost)}");
                }
                text.text = sb.ToString();
            };

            // Quick actions for a few featured items
            string[] featured = { "char_sigma", "trail_fire", "hat_cap", "emote_dance", "skin_neon" };
            for (int i = 0; i < featured.Length; i++)
            {
                string id = featured[i];
                UIFactory.CreateButton(body, id, "Buy/Equip", () =>
                {
                    var u = GameServices.Instance.Unlocks;
                    if (!u.IsUnlocked(id))
                    {
                        if (u.TryPurchase(id))
                            GameServices.Instance.Achievements.NotifyCosmeticUnlocked();
                    }
                    else u.Equip(id);
                    refresh.Refresh();
                    onChanged?.Invoke();
                }, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-300f + i * 150f, 45f), new Vector2(140f, 50f));
            }

            return panel;
        }

        public static GameObject BuildBattlePass(Transform parent, System.Action onClose, System.Action onChanged)
        {
            var panel = CreateOverlay(parent, "BattlePassPanel", onClose);
            var body = panel.transform.Find("Body");
            var text = UIFactory.CreateText(body, "Info", "", 24, TextAnchor.UpperLeft,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), new Vector2(750f, 300f));

            var refresh = panel.AddComponent<SimpleRefresh>();
            refresh.OnRefresh = () =>
            {
                var bp = GameServices.Instance.BattlePass;
                var sb = new StringBuilder();
                sb.AppendLine($"Battle Pass  |  Tier {bp.CurrentTier}/{BattlePassSystem.MaxTiers}");
                sb.AppendLine($"XP: {bp.Xp}  |  Premium: {(bp.HasPremium ? "YES" : "NO")}\n");
                for (int t = 1; t <= Mathf.Min(10, BattlePassSystem.MaxTiers); t++)
                {
                    string free = bp.IsClaimed(t, false) ? "claimed" : (t <= bp.CurrentTier ? "claimable" : "locked");
                    string prem = bp.IsClaimed(t, true) ? "claimed" : (t <= bp.CurrentTier && bp.HasPremium ? "claimable" : "locked");
                    sb.AppendLine($"T{t}: Free[{free}]  Premium[{prem}]");
                }
                sb.AppendLine("...");
                text.text = sb.ToString();
            };

            UIFactory.CreateButton(body, "BuyPremium", "Buy Premium (Gems/IAP)", () =>
            {
                if (!GameServices.Instance.BattlePass.BuyPremium())
                    GameServices.Instance.IAP.Purchase("battle_pass", _ => refresh.Refresh());
                refresh.Refresh();
                onChanged?.Invoke();
            }, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -140f), new Vector2(360f, 60f));

            UIFactory.CreateButton(body, "ClaimFree", "Claim Free Tier", () =>
            {
                var bp = GameServices.Instance.BattlePass;
                for (int t = 1; t <= bp.CurrentTier; t++)
                    if (!bp.IsClaimed(t, false) && bp.Claim(t, false)) break;
                refresh.Refresh();
                onChanged?.Invoke();
            }, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-180f, -220f), new Vector2(280f, 55f));

            UIFactory.CreateButton(body, "ClaimPrem", "Claim Premium Tier", () =>
            {
                var bp = GameServices.Instance.BattlePass;
                for (int t = 1; t <= bp.CurrentTier; t++)
                    if (!bp.IsClaimed(t, true) && bp.Claim(t, true)) break;
                refresh.Refresh();
                onChanged?.Invoke();
            }, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(180f, -220f), new Vector2(280f, 55f));

            return panel;
        }

        public static GameObject BuildAchievements(Transform parent, System.Action onClose)
        {
            var panel = CreateOverlay(parent, "AchievementsPanel", onClose);
            var body = panel.transform.Find("Body");
            var text = UIFactory.CreateText(body, "List", "", 24, TextAnchor.UpperLeft,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 20f), new Vector2(750f, 450f));

            var refresh = panel.AddComponent<SimpleRefresh>();
            refresh.OnRefresh = () =>
            {
                GameServices.Instance.Achievements.Evaluate();
                var sb = new StringBuilder();
                sb.AppendLine("Achievements\n");
                foreach (var a in GameServices.Instance.Achievements.All())
                    sb.AppendLine($"{(a.claimed ? "[x]" : "[ ]")} {a.name} — {a.desc}");
                text.text = sb.ToString();
            };

            return panel;
        }

        static GameObject CreateOverlay(Transform parent, string name, System.Action onClose)
        {
            var panel = UIFactory.CreatePanel(parent, name, new Color(0f, 0f, 0f, 0.8f));
            var body = new GameObject("Body");
            body.transform.SetParent(panel.transform, false);
            var rt = body.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(860f, 1100f);
            var img = body.AddComponent<Image>();
            img.color = new Color(0.12f, 0.15f, 0.22f, 0.98f);

            UIFactory.CreateButton(body.transform, "Close", "X", () => onClose?.Invoke(),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -40f), new Vector2(70f, 70f),
                new Color(0.7f, 0.25f, 0.25f));

            return panel;
        }
    }

    public class SimpleRefresh : MonoBehaviour, IMetaPanelRefresh
    {
        public System.Action OnRefresh;
        public void Refresh() => OnRefresh?.Invoke();
        void OnEnable() => Refresh();
    }
}
