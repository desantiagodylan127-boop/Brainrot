using System.Collections.Generic;
using UnityEngine;

namespace BrainrotRush.Art
{
    /// <summary>
    /// Builds stylized Brainrot visuals at runtime.
    /// Prefers Resources meshes (OBJ/FBX imported by Unity), else procedural primitives.
    /// </summary>
    public static class BrainrotArtFactory
    {
        static readonly Dictionary<string, Mesh> MeshCache = new Dictionary<string, Mesh>();
        static Material _sharedMat;

        public static Material SharedMaterial
        {
            get
            {
                if (_sharedMat == null)
                {
                    var shader = Shader.Find("Standard") ?? Shader.Find("Sprites/Default") ?? Shader.Find("Diffuse");
                    _sharedMat = new Material(shader);
                }
                return _sharedMat;
            }
        }

        public static GameObject AttachVisual(Transform parent, string meshKey, Color tint, float scale = 1f, string visualName = "ArtVisual")
        {
            ClearVisual(parent, visualName);

            var root = new GameObject(visualName);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one * scale;

            if (TryAttachResourceMesh(root.transform, meshKey, tint))
                return root;

            BuildProcedural(root.transform, meshKey, tint);
            return root;
        }

        public static void ClearVisual(Transform parent, string visualName = "ArtVisual")
        {
            var existing = parent.Find(visualName);
            if (existing != null)
                Object.Destroy(existing.gameObject);
        }

        public static void HideRootPrimitive(GameObject host)
        {
            var rend = host.GetComponent<Renderer>();
            if (rend != null) rend.enabled = false;
            // Keep collider on host for gameplay.
        }

        public static void TintRenderers(Transform visualRoot, Color tint)
        {
            if (visualRoot == null) return;
            foreach (var r in visualRoot.GetComponentsInChildren<Renderer>())
            {
                if (r.material != null)
                    r.material.color = tint;
            }
        }

        static bool TryAttachResourceMesh(Transform parent, string meshKey, Color tint)
        {
            var mesh = LoadMesh(meshKey);
            if (mesh == null) return false;

            var go = new GameObject(meshKey.Replace('/', '_'));
            go.transform.SetParent(parent, false);
            var filter = go.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            var rend = go.AddComponent<MeshRenderer>();
            rend.material = new Material(SharedMaterial) { color = tint };
            return true;
        }

        public static Mesh LoadMesh(string meshKey)
        {
            if (string.IsNullOrEmpty(meshKey)) return null;
            if (MeshCache.TryGetValue(meshKey, out var cached) && cached != null)
                return cached;

            // Unity imports OBJ as a Mesh (or Model with MeshFilter). Try common Resources paths.
            var mesh = Resources.Load<Mesh>($"{ArtCatalog.ResourcesRoot}/{meshKey}");
            if (mesh == null)
            {
                var go = Resources.Load<GameObject>($"{ArtCatalog.ResourcesRoot}/{meshKey}");
                if (go != null)
                {
                    var filter = go.GetComponentInChildren<MeshFilter>();
                    if (filter != null) mesh = filter.sharedMesh;
                }
            }

            // Some Unity versions expose OBJ meshes as "meshKey" without folder nesting quirks.
            if (mesh == null)
                mesh = Resources.Load<Mesh>(meshKey);

            if (mesh != null)
                MeshCache[meshKey] = mesh;
            return mesh;
        }

        static void BuildProcedural(Transform parent, string meshKey, Color tint)
        {
            string id = meshKey.Contains("/") ? meshKey.Substring(meshKey.LastIndexOf('/') + 1) : meshKey;
            switch (id)
            {
                case "char_tung_sahur":
                    Part(parent, PrimitiveType.Cylinder, new Vector3(0f, 1.0f, 0f), new Vector3(0.28f, 1.0f, 0.28f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 2.15f, 0f), new Vector3(0.55f, 0.45f, 0.55f), tint * 1.05f);
                    Part(parent, PrimitiveType.Cube, new Vector3(0.55f, 1.45f, 0f), new Vector3(0.9f, 0.16f, 0.16f), new Color(0.45f, 0.28f, 0.12f));
                    Part(parent, PrimitiveType.Cube, new Vector3(-0.2f, 0.15f, 0.1f), new Vector3(0.18f, 0.3f, 0.18f), tint * 0.85f);
                    Part(parent, PrimitiveType.Cube, new Vector3(0.2f, 0.15f, -0.1f), new Vector3(0.18f, 0.3f, 0.18f), tint * 0.85f);
                    break;

                case "char_ballerina":
                    Part(parent, PrimitiveType.Cylinder, new Vector3(0f, 1.85f, 0f), new Vector3(0.75f, 0.28f, 0.75f), new Color(0.72f, 0.52f, 0.32f));
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 2.15f, 0f), new Vector3(0.55f, 0.1f, 0.55f), new Color(0.9f, 0.85f, 0.75f));
                    Part(parent, PrimitiveType.Capsule, new Vector3(0f, 1.05f, 0f), new Vector3(0.35f, 0.55f, 0.35f), new Color(1f, 0.7f, 0.85f));
                    Part(parent, PrimitiveType.Cylinder, new Vector3(0f, 0.75f, 0f), new Vector3(1.2f, 0.06f, 1.2f), new Color(1f, 0.55f, 0.75f));
                    Part(parent, PrimitiveType.Cube, new Vector3(-0.15f, 0.25f, 0f), new Vector3(0.12f, 0.45f, 0.12f), Color.white);
                    Part(parent, PrimitiveType.Cube, new Vector3(0.15f, 0.25f, 0f), new Vector3(0.12f, 0.45f, 0.12f), Color.white);
                    break;

                case "char_tralalero":
                case "enemy_tralalero":
                    Part(parent, PrimitiveType.Capsule, new Vector3(0f, 0.85f, 0.1f), new Vector3(0.7f, 0.45f, 1.1f), new Color(0.35f, 0.55f, 0.9f));
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 1.0f, 0.85f), new Vector3(0.4f, 0.3f, 0.5f), new Color(0.3f, 0.5f, 0.85f));
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 1.35f, -0.2f), new Vector3(0.12f, 0.5f, 0.45f), new Color(0.25f, 0.4f, 0.75f));
                    Part(parent, PrimitiveType.Cube, new Vector3(-0.3f, 0.25f, -0.1f), new Vector3(0.18f, 0.45f, 0.22f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 0.25f, 0.2f), new Vector3(0.18f, 0.45f, 0.22f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(0.3f, 0.25f, -0.1f), new Vector3(0.18f, 0.45f, 0.22f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(-0.3f, 0.04f, -0.1f), new Vector3(0.28f, 0.1f, 0.38f), Color.white);
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 0.04f, 0.2f), new Vector3(0.28f, 0.1f, 0.38f), Color.white);
                    Part(parent, PrimitiveType.Cube, new Vector3(0.3f, 0.04f, -0.1f), new Vector3(0.28f, 0.1f, 0.38f), Color.white);
                    break;

                case "char_assassino":
                    Part(parent, PrimitiveType.Cylinder, new Vector3(0f, 1.55f, 0f), new Vector3(0.7f, 0.35f, 0.7f), new Color(0.55f, 0.35f, 0.2f));
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 1.0f, 0f), new Vector3(0.5f, 0.55f, 0.3f), new Color(0.12f, 0.12f, 0.14f));
                    Part(parent, PrimitiveType.Cube, new Vector3(0.7f, 0.95f, 0.05f), new Vector3(0.7f, 0.08f, 0.12f), new Color(0.75f, 0.75f, 0.8f));
                    Part(parent, PrimitiveType.Cube, new Vector3(-0.15f, 0.3f, 0f), new Vector3(0.14f, 0.5f, 0.14f), new Color(0.1f, 0.1f, 0.1f));
                    Part(parent, PrimitiveType.Cube, new Vector3(0.15f, 0.3f, 0f), new Vector3(0.14f, 0.5f, 0.14f), new Color(0.1f, 0.1f, 0.1f));
                    break;

                case "enemy_bombardiro":
                case "boss_bombardiro":
                    Part(parent, PrimitiveType.Capsule, new Vector3(0f, 0.7f, 0f), new Vector3(0.7f, 0.4f, 1.2f), new Color(0.2f, 0.5f, 0.28f));
                    Part(parent, PrimitiveType.Cube, new Vector3(-0.95f, 0.75f, 0f), new Vector3(1.1f, 0.08f, 0.4f), new Color(0.55f, 0.55f, 0.6f));
                    Part(parent, PrimitiveType.Cube, new Vector3(0.95f, 0.75f, 0f), new Vector3(1.1f, 0.08f, 0.4f), new Color(0.55f, 0.55f, 0.6f));
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 1.1f, -0.5f), new Vector3(0.1f, 0.4f, 0.3f), new Color(0.5f, 0.5f, 0.55f));
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 0.7f, 0.85f), new Vector3(0.4f, 0.28f, 0.45f), new Color(0.15f, 0.4f, 0.22f));
                    break;

                case "enemy_patapim":
                    Part(parent, PrimitiveType.Cylinder, new Vector3(0f, 1.0f, 0f), new Vector3(0.45f, 0.95f, 0.45f), new Color(0.35f, 0.65f, 0.3f));
                    Part(parent, PrimitiveType.Cube, new Vector3(-0.55f, 1.25f, 0f), new Vector3(0.7f, 0.18f, 0.18f), new Color(0.3f, 0.55f, 0.25f));
                    Part(parent, PrimitiveType.Cube, new Vector3(0.55f, 1.5f, 0f), new Vector3(0.7f, 0.18f, 0.18f), new Color(0.3f, 0.55f, 0.25f));
                    Part(parent, PrimitiveType.Sphere, new Vector3(0f, 2.0f, 0f), Vector3.one * 0.45f, new Color(0.85f, 0.7f, 0.45f));
                    break;

                case "enemy_lirili":
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 0.85f, 0f), new Vector3(0.7f, 0.7f, 0.85f), new Color(0.85f, 0.72f, 0.4f));
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 1.2f, 0.55f), new Vector3(0.28f, 0.22f, 0.55f), new Color(0.8f, 0.65f, 0.35f));
                    Part(parent, PrimitiveType.Cylinder, new Vector3(-0.2f, 1.55f, -0.1f), new Vector3(0.18f, 0.28f, 0.18f), new Color(0.3f, 0.65f, 0.3f));
                    Part(parent, PrimitiveType.Cylinder, new Vector3(0.2f, 1.55f, -0.1f), new Vector3(0.18f, 0.28f, 0.18f), new Color(0.3f, 0.65f, 0.3f));
                    break;

                case "enemy_boneca":
                    Part(parent, PrimitiveType.Sphere, new Vector3(0f, 0.85f, 0f), Vector3.one * 0.7f, new Color(0.35f, 0.75f, 0.4f));
                    Part(parent, PrimitiveType.Cylinder, new Vector3(0f, 0.3f, 0f), new Vector3(0.95f, 0.12f, 0.95f), new Color(0.15f, 0.15f, 0.15f));
                    Part(parent, PrimitiveType.Sphere, new Vector3(-0.25f, 1.15f, 0.25f), Vector3.one * 0.22f, Color.white);
                    Part(parent, PrimitiveType.Sphere, new Vector3(0.25f, 1.15f, 0.25f), Vector3.one * 0.22f, Color.white);
                    break;

                case "tower_rapid":
                    Part(parent, PrimitiveType.Cylinder, new Vector3(0f, 0.35f, 0f), new Vector3(0.7f, 0.35f, 0.7f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 0.85f, 0.35f), new Vector3(0.25f, 0.25f, 0.55f), Color.Lerp(tint, Color.white, 0.3f));
                    break;

                case "tower_cannon":
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 0.35f, 0f), new Vector3(0.7f, 0.7f, 0.7f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 0.7f, 0.7f), new Vector3(0.35f, 0.35f, 1.0f), Color.Lerp(tint, Color.gray, 0.25f));
                    break;

                case "tower_freeze":
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 0.45f, 0f), new Vector3(0.6f, 0.9f, 0.6f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 1.05f, 0f), new Vector3(0.7f, 0.12f, 0.7f), Color.white);
                    break;

                case "tower_laser":
                    Part(parent, PrimitiveType.Cylinder, new Vector3(0f, 0.5f, 0f), new Vector3(0.45f, 0.5f, 0.45f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 1.15f, 0.25f), new Vector3(0.18f, 0.18f, 0.7f), Color.magenta);
                    break;

                case "hat_cap":
                    Part(parent, PrimitiveType.Cylinder, new Vector3(0f, 0.1f, 0f), new Vector3(0.7f, 0.1f, 0.7f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 0.06f, 0.32f), new Vector3(0.5f, 0.05f, 0.3f), tint * 0.9f);
                    break;

                case "hat_crown":
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 0.12f, 0f), new Vector3(0.55f, 0.22f, 0.55f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(-0.18f, 0.35f, 0f), new Vector3(0.1f, 0.22f, 0.1f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(0f, 0.4f, 0f), new Vector3(0.1f, 0.3f, 0.1f), tint);
                    Part(parent, PrimitiveType.Cube, new Vector3(0.18f, 0.35f, 0f), new Vector3(0.1f, 0.22f, 0.1f), tint);
                    break;

                default:
                    Part(parent, PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.7f, 0.9f, 0.7f), tint);
                    break;
            }
        }

        static void Part(Transform parent, PrimitiveType type, Vector3 localPos, Vector3 localScale, Color color)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = type.ToString();
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            Object.Destroy(go.GetComponent<Collider>());
            var rend = go.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = color;
        }
    }
}
