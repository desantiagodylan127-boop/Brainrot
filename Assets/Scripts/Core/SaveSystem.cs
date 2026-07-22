using System;
using UnityEngine;

namespace BrainrotRush
{
    /// <summary>
    /// Lightweight JSON save using PlayerPrefs. Fast to ship, easy to swap later.
    /// </summary>
    public static class SaveSystem
    {
        const string SaveKey = "BrainrotRush_PlayerData_v1";

        public static PlayerData Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
                return new PlayerData();

            try
            {
                var json = PlayerPrefs.GetString(SaveKey);
                var data = JsonUtility.FromJson<PlayerData>(json);
                return data ?? new PlayerData();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Save load failed, using defaults: {e.Message}");
                return new PlayerData();
            }
        }

        public static void Save(PlayerData data)
        {
            if (data == null) return;
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public static void Delete()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }
    }
}
