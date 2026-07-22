using UnityEngine;

namespace BrainrotRush
{
    public class Tower : MonoBehaviour
    {
        public TowerType Type;
        public int Level = 1;
        public float Range = 4.5f;
        public float FireRate = 1f;
        public float Damage = 8f;
        public int SellValue = 30;

        float _cooldown;
        Transform _muzzle;
        Renderer _renderer;
        Color _skinTint = Color.white;

        public static int BaseCost(TowerType type) => type switch
        {
            TowerType.RapidFire => 50,
            TowerType.Cannon => 80,
            TowerType.Freeze => 70,
            TowerType.Laser => 120,
            _ => 50
        };

        public int UpgradeCost => Mathf.RoundToInt(BaseCost(Type) * 0.75f * Level);

        public void Setup(TowerType type, Color skinTint)
        {
            Type = type;
            _skinTint = skinTint;
            Level = 1;
            ApplyStats();
            BuildVisual();
        }

        void ApplyStats()
        {
            switch (Type)
            {
                case TowerType.RapidFire:
                    Range = 4f; FireRate = 3.5f; Damage = 4f + Level * 2f; break;
                case TowerType.Cannon:
                    Range = 5f; FireRate = 0.7f; Damage = 20f + Level * 10f; break;
                case TowerType.Freeze:
                    Range = 4.2f; FireRate = 1.2f; Damage = 6f + Level * 3f; break;
                case TowerType.Laser:
                    Range = 6f; FireRate = 8f; Damage = 2.5f + Level * 1.5f; break;
            }
            SellValue = Mathf.RoundToInt(BaseCost(Type) * 0.6f * Level);
        }

        void BuildVisual()
        {
            _renderer = GetComponent<Renderer>();
            Color baseCol = Type switch
            {
                TowerType.RapidFire => new Color(0.3f, 0.85f, 0.4f),
                TowerType.Cannon => new Color(0.85f, 0.45f, 0.2f),
                TowerType.Freeze => new Color(0.4f, 0.75f, 1f),
                TowerType.Laser => new Color(1f, 0.3f, 0.55f),
                _ => Color.gray
            };
            baseCol = Color.Lerp(baseCol, _skinTint, 0.35f);
            if (_renderer) _renderer.material.color = baseCol;

            var muzzleGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            muzzleGo.name = "Muzzle";
            muzzleGo.transform.SetParent(transform);
            muzzleGo.transform.localPosition = new Vector3(0f, 0.6f, 0.55f);
            muzzleGo.transform.localScale = new Vector3(0.25f, 0.25f, 0.6f);
            Destroy(muzzleGo.GetComponent<Collider>());
            muzzleGo.GetComponent<Renderer>().material.color = Color.Lerp(baseCol, Color.white, 0.3f);
            _muzzle = muzzleGo.transform;
        }

        public void Upgrade()
        {
            Level++;
            ApplyStats();
            transform.localScale = Vector3.one * (1f + (Level - 1) * 0.12f);
        }

        void Update()
        {
            _cooldown -= Time.deltaTime;
            if (_cooldown > 0f) return;

            var target = FindTarget();
            if (target == null) return;

            Fire(target);
            _cooldown = 1f / FireRate;
        }

        Enemy FindTarget()
        {
            Enemy best = null;
            float bestDist = float.MaxValue;
            foreach (var e in FindObjectsOfType<Enemy>())
            {
                if (!e.IsAlive) continue;
                float d = Vector3.Distance(transform.position, e.transform.position);
                if (d <= Range && d < bestDist)
                {
                    bestDist = d;
                    best = e;
                }
            }
            return best;
        }

        void Fire(Enemy target)
        {
            var dir = target.transform.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(dir);

            bool isLaser = Type == TowerType.Laser;
            var projGo = GameObject.CreatePrimitive(isLaser ? PrimitiveType.Cube : PrimitiveType.Sphere);
            projGo.name = "Projectile";
            projGo.transform.position = _muzzle != null ? _muzzle.position : transform.position + Vector3.up;
            projGo.transform.localScale = isLaser ? new Vector3(0.15f, 0.15f, 0.5f) : Vector3.one * 0.3f;
            projGo.GetComponent<Collider>().isTrigger = true;
            Destroy(projGo.GetComponent<Collider>());

            var proj = projGo.AddComponent<Projectile>();
            Color c = Type switch
            {
                TowerType.Freeze => Color.cyan,
                TowerType.Laser => Color.magenta,
                TowerType.Cannon => new Color(1f, 0.5f, 0.1f),
                _ => Color.yellow
            };
            proj.Fire(target, Damage, c, Type == TowerType.Freeze, 0.45f);
            if (Type == TowerType.Cannon) proj.Speed = 12f;
            if (isLaser) proj.Speed = 30f;
        }
    }
}
