using System.Collections.Generic;
using UnityEngine;

namespace BrainrotRush
{
    /// <summary>
    /// Spawns repeating ground segments with random obstacles/coins/powerups.
    /// World scrolls toward the player (player Z stays near 0).
    /// </summary>
    public class LevelGenerator : MonoBehaviour
    {
        public float SegmentLength = 12f;
        public int SegmentsAhead = 8;
        public float LaneWidth = 2.2f;
        public int LaneCount = 3;

        readonly List<GameObject> _segments = new List<GameObject>();
        float _nextSpawnZ;
        float _scrollSpeed;
        Transform _worldRoot;
        float _difficulty;

        public void Init(Transform worldRoot)
        {
            _worldRoot = worldRoot;
            _nextSpawnZ = 0f;
            for (int i = 0; i < SegmentsAhead; i++)
                SpawnSegment();
        }

        public void Tick(float speed, float difficulty)
        {
            _scrollSpeed = speed;
            _difficulty = difficulty;
            _worldRoot.position += Vector3.back * speed * Time.deltaTime;

            // Recycle segments behind camera
            for (int i = _segments.Count - 1; i >= 0; i--)
            {
                var seg = _segments[i];
                if (seg == null)
                {
                    _segments.RemoveAt(i);
                    continue;
                }

                float worldZ = seg.transform.position.z;
                if (worldZ < -SegmentLength * 2f)
                {
                    Destroy(seg);
                    _segments.RemoveAt(i);
                    SpawnSegment();
                }
            }
        }

        void SpawnSegment()
        {
            var seg = new GameObject($"Segment_{_nextSpawnZ}");
            seg.transform.SetParent(_worldRoot);
            seg.transform.localPosition = new Vector3(0f, 0f, _nextSpawnZ);

            // Ground
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.transform.SetParent(seg.transform);
            ground.transform.localPosition = new Vector3(0f, -0.25f, SegmentLength * 0.5f);
            ground.transform.localScale = new Vector3(LaneCount * LaneWidth + 1.5f, 0.5f, SegmentLength);
            ground.GetComponent<Renderer>().material.color = new Color(0.18f, 0.22f, 0.28f);

            // Lane markers
            for (int lane = 0; lane < LaneCount; lane++)
            {
                float x = LaneX(lane);
                var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                line.name = "LaneLine";
                line.transform.SetParent(seg.transform);
                line.transform.localPosition = new Vector3(x, 0.01f, SegmentLength * 0.5f);
                line.transform.localScale = new Vector3(0.08f, 0.02f, SegmentLength);
                line.GetComponent<Renderer>().material.color = new Color(0.35f, 0.4f, 0.5f);
                Destroy(line.GetComponent<Collider>());
            }

            // Don't clutter the first two segments
            if (_nextSpawnZ > SegmentLength * 1.5f)
                Populate(seg.transform);

            _segments.Add(seg);
            _nextSpawnZ += SegmentLength;
        }

        void Populate(Transform seg)
        {
            float dens = Mathf.Lerp(0.35f, 0.85f, _difficulty);

            // Obstacles
            if (Random.value < dens)
            {
                int lane = Random.Range(0, LaneCount);
                var kind = PickObstacleKind();
                var obs = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obs.name = "Obstacle";
                obs.transform.SetParent(seg);
                obs.transform.localPosition = new Vector3(LaneX(lane), 0.8f, Random.Range(3f, SegmentLength - 2f));
                var col = obs.GetComponent<Collider>();
                col.isTrigger = true;
                var ro = obs.AddComponent<RunnerObstacle>();
                Color c = kind == RunnerObstacle.ObstacleKind.Low ? new Color(0.9f, 0.3f, 0.3f)
                    : kind == RunnerObstacle.ObstacleKind.High ? new Color(0.9f, 0.6f, 0.2f)
                    : new Color(0.7f, 0.2f, 0.7f);
                ro.Setup(kind, lane, c);
            }

            // Coins
            if (Random.value < 0.7f)
            {
                int lane = Random.Range(0, LaneCount);
                int count = Random.Range(3, 7);
                float startZ = Random.Range(2f, SegmentLength - count - 1f);
                for (int i = 0; i < count; i++)
                {
                    var coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    coin.name = "Coin";
                    coin.transform.SetParent(seg);
                    coin.transform.localPosition = new Vector3(LaneX(lane), 1f, startZ + i * 1.1f);
                    coin.transform.localScale = new Vector3(0.5f, 0.08f, 0.5f);
                    coin.transform.Rotate(90f, 0f, 0f);
                    coin.GetComponent<Renderer>().material.color = new Color(1f, 0.85f, 0.2f);
                    coin.GetComponent<Collider>().isTrigger = true;
                    coin.AddComponent<CoinPickup>();
                }
            }

            // Powerup
            if (Random.value < 0.12f + _difficulty * 0.08f)
            {
                int lane = Random.Range(0, LaneCount);
                var pu = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pu.name = "Powerup";
                pu.transform.SetParent(seg);
                pu.transform.localPosition = new Vector3(LaneX(lane), 1.2f, Random.Range(4f, SegmentLength - 2f));
                pu.transform.localScale = Vector3.one * 0.7f;
                pu.GetComponent<Collider>().isTrigger = true;
                var types = new[] { PowerupType.Magnet, PowerupType.Shield, PowerupType.SpeedBoost };
                var pickup = pu.AddComponent<PowerupPickup>();
                pickup.Setup(types[Random.Range(0, types.Length)]);
            }
        }

        RunnerObstacle.ObstacleKind PickObstacleKind()
        {
            float r = Random.value;
            if (r < 0.4f) return RunnerObstacle.ObstacleKind.Low;
            if (r < 0.7f) return RunnerObstacle.ObstacleKind.High;
            return RunnerObstacle.ObstacleKind.Full;
        }

        float LaneX(int lane) => (lane - (LaneCount - 1) * 0.5f) * LaneWidth;
    }
}
