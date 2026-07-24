using System;
using UnityEngine;

namespace BrainrotRush
{
    /// <summary>
    /// Stub rewarded ads. Swap with AdMob/Unity Ads later.
    /// </summary>
    public class AdManager : MonoBehaviour
    {
        public bool AdsEnabled => GameServices.Instance == null || !GameServices.Instance.Data.adsRemoved;

        public void ShowRewarded(string placement, Action onSuccess, Action onFail = null)
        {
            if (!AdsEnabled)
            {
                onSuccess?.Invoke();
                return;
            }

            Debug.Log($"[Ads] Showing rewarded: {placement}");
            // Prototype: always succeed after a short delay.
            StartCoroutine(FakeAd(onSuccess));
        }

        public void DoubleCoins(int baseCoins, Action<int> onResult)
        {
            ShowRewarded("double_coins", () =>
            {
                GameServices.Instance.Currency.Add(CurrencyType.Coins, baseCoins);
                onResult?.Invoke(baseCoins * 2);
            }, () => onResult?.Invoke(baseCoins));
        }

        public void ExtraLife(Action onGranted)
        {
            ShowRewarded("extra_life", () =>
            {
                GameServices.Instance.Data.hasExtraLifePending = true;
                GameServices.Instance.Persist();
                onGranted?.Invoke();
            });
        }

        public void BonusChest(Action onOpened)
        {
            ShowRewarded("bonus_chest", () =>
            {
                int coins = UnityEngine.Random.Range(50, 151);
                int gems = UnityEngine.Random.value > 0.7f ? 2 : 0;
                GameServices.Instance.Currency.Add(CurrencyType.Coins, coins, persist: false);
                if (gems > 0) GameServices.Instance.Currency.Add(CurrencyType.Gems, gems, persist: false);
                GameServices.Instance.Persist();
                onOpened?.Invoke();
            });
        }

        System.Collections.IEnumerator FakeAd(Action onSuccess)
        {
            yield return new WaitForSecondsRealtime(0.35f);
            onSuccess?.Invoke();
        }
    }
}
