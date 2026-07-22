using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace BrainrotRush.EditorTools
{
#if UNITY_EDITOR
    public static class BrainrotSetupMenu
    {
        [MenuItem("Brainrot Rush/Create Bootstrap Scene")]
        public static void CreateBootstrapScene()
        {
            var scene = EditorSceneManagerCompat.NewScene();
            var go = new GameObject("BootstrapNote");
            // Marker only — AppBootstrap auto-runs on play.
            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManagerCompat.SaveScene(scene, "Assets/Scenes/Bootstrap.unity");
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/Bootstrap.unity", true)
            };
            Debug.Log("Created Assets/Scenes/Bootstrap.unity and added it to Build Settings. Press Play.");
        }

        [MenuItem("Brainrot Rush/Reset Player Save")]
        public static void ResetSave()
        {
            SaveSystem.Delete();
            Debug.Log("Player save wiped.");
        }
    }

    /// <summary>Tiny wrapper so we don't need asmdef references beyond UnityEditor.</summary>
    static class EditorSceneManagerCompat
    {
        public static UnityEngine.SceneManagement.Scene NewScene()
        {
            return UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects,
                UnityEditor.SceneManagement.NewSceneMode.Single);
        }

        public static void SaveScene(UnityEngine.SceneManagement.Scene scene, string path)
        {
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, path);
        }
    }
#endif
}
