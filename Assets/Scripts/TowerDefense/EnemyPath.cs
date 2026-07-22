using UnityEngine;

namespace BrainrotRush
{
    public class EnemyPath : MonoBehaviour
    {
        public Vector3[] Waypoints { get; private set; }

        public void SetupDefault()
        {
            Waypoints = new[]
            {
                new Vector3(-8f, 0.5f, -4f),
                new Vector3(-8f, 0.5f, 4f),
                new Vector3(-2f, 0.5f, 4f),
                new Vector3(-2f, 0.5f, -2f),
                new Vector3(4f, 0.5f, -2f),
                new Vector3(4f, 0.5f, 5f),
                new Vector3(9f, 0.5f, 5f),
            };

            // Visual path ribbon
            for (int i = 0; i < Waypoints.Length - 1; i++)
            {
                var a = Waypoints[i];
                var b = Waypoints[i + 1];
                var mid = (a + b) * 0.5f;
                var length = Vector3.Distance(a, b);
                var road = GameObject.CreatePrimitive(PrimitiveType.Cube);
                road.name = "PathSegment";
                road.transform.SetParent(transform);
                road.transform.position = mid + Vector3.down * 0.4f;
                road.transform.localScale = new Vector3(1.6f, 0.15f, length + 0.2f);
                road.transform.rotation = Quaternion.LookRotation(b - a);
                road.GetComponent<Renderer>().material.color = new Color(0.35f, 0.32f, 0.28f);
                Destroy(road.GetComponent<Collider>());
            }
        }

        public Vector3 GetPoint(int index) =>
            Waypoints[Mathf.Clamp(index, 0, Waypoints.Length - 1)];
    }
}
