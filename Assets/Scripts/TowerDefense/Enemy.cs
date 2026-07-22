using UnityEngine;
using UnityEngine.UI;

namespace BrainrotRush
{
    public class Enemy : MonoBehaviour
    {
        public float MaxHealth = 30f;
        public float Speed = 2.2f;
        public int Reward = 10;
        public int DamageToBase = 1;
        public bool IsBoss;
        public float SlowMultiplier { get; private set; } = 1f;

        float _health;
        float _slowTimer;
        int _waypointIndex;
        EnemyPath _path;
        HealthBar _healthBar;
        TDGameManager _mgr;
        Renderer _renderer;
        Color _baseColor;

        public float HealthNormalized => _health / MaxHealth;
        public bool IsAlive => _health > 0f;

        public void Init(EnemyPath path, TDGameManager mgr, float hp, float speed, int reward, bool boss, Color color)
        {
            _path = path;
            _mgr = mgr;
            MaxHealth = hp;
            _health = hp;
            Speed = speed;
            Reward = reward;
            IsBoss = boss;
            DamageToBase = boss ? 5 : 1;
            _waypointIndex = 0;
            transform.position = path.GetPoint(0);
            _renderer = GetComponent<Renderer>();
            _baseColor = color;
            if (_renderer) _renderer.material.color = color;
            if (boss) transform.localScale = Vector3.one * 1.8f;

            _healthBar = gameObject.AddComponent<HealthBar>();
            _healthBar.Init(this);
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;
            _health -= amount;
            if (_health <= 0f) Die();
        }

        public void ApplySlow(float multiplier, float duration)
        {
            SlowMultiplier = Mathf.Min(SlowMultiplier, multiplier);
            _slowTimer = Mathf.Max(_slowTimer, duration);
            if (_renderer) _renderer.material.color = Color.Lerp(_baseColor, Color.cyan, 0.5f);
        }

        void Die()
        {
            _mgr.OnEnemyKilled(this);
            GameEvents.EnemyDefeated(this);
            Destroy(gameObject);
        }

        void Update()
        {
            if (!IsAlive || _path == null) return;

            if (_slowTimer > 0f)
            {
                _slowTimer -= Time.deltaTime;
                if (_slowTimer <= 0f)
                {
                    SlowMultiplier = 1f;
                    if (_renderer) _renderer.material.color = _baseColor;
                }
            }

            var target = _path.GetPoint(_waypointIndex);
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                Speed * SlowMultiplier * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.08f)
            {
                _waypointIndex++;
                if (_waypointIndex >= _path.Waypoints.Length)
                {
                    _mgr.OnEnemyReachedEnd(this);
                    Destroy(gameObject);
                }
            }
        }
    }
}
