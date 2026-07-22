using System.Collections;
using UnityEngine;

namespace BrainrotRush
{
    public class WaveSpawner : MonoBehaviour
    {
        TDGameManager _mgr;
        EnemyPath _path;
        int _wave;
        bool _spawning;

        public int CurrentWave => _wave;
        public bool IsSpawning => _spawning;
        public int AliveEnemies { get; private set; }

        public void Init(TDGameManager mgr, EnemyPath path)
        {
            _mgr = mgr;
            _path = path;
            _wave = 0;
        }

        public void StartNextWave()
        {
            if (_spawning) return;
            _wave++;
            StartCoroutine(SpawnWave(_wave));
        }

        IEnumerator SpawnWave(int wave)
        {
            _spawning = true;
            GameEvents.WaveStarted(wave);

            bool bossWave = wave % 5 == 0;

            if (bossWave)
            {
                float bossHp = 120f + wave * 40f;
                SpawnOne(bossHp, 1.35f, 50 + wave * 5, true, new Color(0.9f, 0.15f, 0.2f));
                yield return new WaitForSeconds(1.4f);

                int adds = 3 + wave / 5;
                for (int i = 0; i < adds; i++)
                {
                    SpawnOne(20f + wave * 6f, 2f, 8 + wave, false, new Color(0.8f, 0.4f, 0.4f));
                    yield return new WaitForSeconds(0.55f);
                }
            }
            else
            {
                int count = 5 + wave * 2;
                for (int i = 0; i < count; i++)
                {
                    float hp = 20f + wave * 8f;
                    float speed = 1.8f + wave * 0.05f;
                    int reward = 8 + wave;
                    Color color = Color.HSVToRGB((wave * 0.13f) % 1f, 0.7f, 0.95f);
                    SpawnOne(hp, speed, reward, false, color);
                    yield return new WaitForSeconds(Mathf.Max(0.35f, 1.1f - wave * 0.04f));
                }
            }

            _spawning = false;
        }

        void SpawnOne(float hp, float speed, int reward, bool boss, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = boss ? "BossBrainrot" : "BrainrotEnemy";
            go.transform.localScale = boss ? Vector3.one * 1.6f : Vector3.one * 0.9f;
            var enemy = go.AddComponent<Enemy>();
            enemy.Init(_path, _mgr, hp, speed, reward, boss, color);
            AliveEnemies++;
        }

        public void NotifyEnemyRemoved() => AliveEnemies = Mathf.Max(0, AliveEnemies - 1);

        public bool WaveCleared => !_spawning && AliveEnemies <= 0 && _wave > 0;
    }
}
