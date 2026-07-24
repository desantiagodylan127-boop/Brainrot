using UnityEngine;

namespace BrainrotRush
{
    public class Projectile : MonoBehaviour
    {
        public float Speed = 18f;
        public float Damage = 10f;
        public bool Slow;
        public float SlowAmount = 0.5f;
        public float SlowDuration = 2f;
        public bool IsLaser;

        Enemy _target;
        Vector3 _lastDir = Vector3.forward;

        public void Fire(Enemy target, float damage, Color color, bool slow = false, float slowAmount = 0.5f)
        {
            _target = target;
            Damage = damage;
            Slow = slow;
            SlowAmount = slowAmount;
            var rend = GetComponent<Renderer>();
            if (rend) rend.material.color = color;
        }

        void Update()
        {
            if (_target == null || !_target.IsAlive)
            {
                // Keep flying briefly then die
                transform.position += _lastDir * Speed * Time.deltaTime;
                Destroy(gameObject, 0.4f);
                return;
            }

            var dir = (_target.transform.position - transform.position).normalized;
            _lastDir = dir;
            transform.position += dir * Speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);

            if (Vector3.Distance(transform.position, _target.transform.position) < 0.45f)
            {
                _target.TakeDamage(Damage);
                if (Slow) _target.ApplySlow(SlowAmount, SlowDuration);
                Destroy(gameObject);
            }
        }
    }
}
