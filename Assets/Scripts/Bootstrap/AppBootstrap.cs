using UnityEngine;
using UnityEngine.SceneManagement;

namespace BrainrotRush.Bootstrap
{
    /// <summary>
    /// Runtime entry point. Creates playable modes without hand-authored scene content.
    /// Open any empty scene, enter Play, and the menu boots automatically.
    /// </summary>
    public static class AppBootstrap
    {
        static bool _booted;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoBoot()
        {
            if (_booted) return;
            _booted = true;
            LoadMenu();
        }

        public static void LoadMenu()
        {
            TearDownGameplay();
            EnsureCleanScene("Menu");
            var go = new GameObject("MainMenu");
            go.AddComponent<MainMenuUI>().Show();
        }

        public static void LoadRunner()
        {
            TearDownGameplay();
            EnsureCleanScene("Runner");
            var go = new GameObject("RunnerGame");
            go.AddComponent<RunnerGameManager>().Begin();
        }

        public static void LoadTowerDefense()
        {
            TearDownGameplay();
            EnsureCleanScene("TowerDefense");
            var go = new GameObject("TDGame");
            go.AddComponent<TDGameManager>().Begin();
        }

        static void EnsureCleanScene(string label)
        {
            // Destroy leftover root objects except services / event system / bootstrap leftovers we own next frame
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.GetComponent<GameServices>() != null) continue;
                if (root.name == "GameServices") continue;
                Object.Destroy(root);
            }

            // Ensure camera exists for next mode builder
            if (Camera.main == null)
            {
                var camGo = new GameObject("Main Camera");
                camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
                camGo.AddComponent<AudioListener>();
            }

            Debug.Log($"[BrainrotRush] Loaded mode: {label}");
        }

        static void TearDownGameplay()
        {
            Time.timeScale = 1f;
            GameEvents.ClearModeEvents();
        }
    }
}
