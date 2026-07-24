using UnityEngine;
using BrainrotRush.Art;

namespace BrainrotRush
{
    public class RunnerPlayer : MonoBehaviour
    {
        public int LaneCount = 3;
        public float LaneWidth = 2.2f;
        public float LaneChangeSpeed = 12f;
        public float JumpHeight = 2.2f;
        public float JumpDuration = 0.55f;
        public float SlideDuration = 0.55f;

        public int CurrentLane { get; private set; } = 1;
        public bool IsJumping { get; private set; }
        public bool IsSliding { get; private set; }
        public bool HasShield { get; private set; }
        public bool MagnetActive { get; private set; }
        public float SpeedMultiplier { get; private set; } = 1f;

        float _jumpTimer;
        float _slideTimer;
        float _shieldTimer;
        float _magnetTimer;
        float _speedTimer;
        float _baseY;
        Vector3 _targetPos;
        Vector3 _restScale = Vector3.one;
        Color _baseColor = Color.white;
        Transform _visualRoot;
        Transform _hat;

        public void Init(SwipeInput input)
        {
            CurrentLane = 1;
            _baseY = 0.5f;
            transform.position = LanePosition(CurrentLane);
            _targetPos = transform.position;

            input.OnSwipeLeft += () => ChangeLane(-1);
            input.OnSwipeRight += () => ChangeLane(1);
            input.OnSwipeUp += Jump;
            input.OnSwipeDown += Slide;

            ApplyCosmetics();
        }

        void ApplyCosmetics()
        {
            var unlocks = GameServices.Instance.Unlocks;
            var character = unlocks.Get(unlocks.GetEquipped(CosmeticType.Character));
            _baseColor = character != null ? character.tint : new Color(0.72f, 0.55f, 0.35f);

            BrainrotArtFactory.HideRootPrimitive(gameObject);
            var meshKey = ArtCatalog.CharacterMeshKey(character != null ? character.id : "char_default");
            _visualRoot = BrainrotArtFactory.AttachVisual(transform, meshKey, _baseColor, 1f).transform;

            // Clear old trail if re-init ever happens
            var oldTrail = GetComponent<TrailRenderer>();
            if (oldTrail != null) Destroy(oldTrail);

            var trailDef = unlocks.Get(unlocks.GetEquipped(CosmeticType.Trail));
            if (trailDef != null && trailDef.id != "trail_none")
            {
                var trail = gameObject.AddComponent<TrailRenderer>();
                trail.time = 0.35f;
                trail.startWidth = 0.4f;
                trail.endWidth = 0.05f;
                trail.material = new Material(Shader.Find("Sprites/Default"));
                trail.startColor = trailDef.tint;
                trail.endColor = new Color(trailDef.tint.r, trailDef.tint.g, trailDef.tint.b, 0f);
            }

            var hatDef = unlocks.Get(unlocks.GetEquipped(CosmeticType.Hat));
            if (hatDef != null && hatDef.id != "hat_none")
            {
                var hatKey = ArtCatalog.HatMeshKey(hatDef.id);
                if (hatKey != null)
                {
                    var hatGo = BrainrotArtFactory.AttachVisual(transform, hatKey, hatDef.tint, 1f, "HatVisual");
                    hatGo.transform.localPosition = new Vector3(0f, 2.15f, 0f);
                    _hat = hatGo.transform;
                }
            }
        }

        void ChangeLane(int dir)
        {
            int next = Mathf.Clamp(CurrentLane + dir, 0, LaneCount - 1);
            if (next == CurrentLane) return;
            CurrentLane = next;
            _targetPos = LanePosition(CurrentLane);
            _targetPos.y = transform.position.y;
        }

        Vector3 LanePosition(int lane)
        {
            float x = (lane - (LaneCount - 1) * 0.5f) * LaneWidth;
            return new Vector3(x, _baseY, transform.position.z);
        }

        void Jump()
        {
            if (IsJumping || IsSliding) return;
            IsJumping = true;
            _jumpTimer = 0f;
        }

        void Slide()
        {
            if (IsJumping || IsSliding) return;
            IsSliding = true;
            _slideTimer = 0f;
            transform.localScale = new Vector3(_restScale.x, _restScale.y * 0.5f, _restScale.z * 1.2f);
        }

        public void ActivatePowerup(PowerupType type, float duration)
        {
            switch (type)
            {
                case PowerupType.Magnet:
                    MagnetActive = true;
                    _magnetTimer = duration;
                    break;
                case PowerupType.Shield:
                    HasShield = true;
                    _shieldTimer = duration;
                    BrainrotArtFactory.TintRenderers(_visualRoot, Color.cyan);
                    break;
                case PowerupType.SpeedBoost:
                    SpeedMultiplier = 1.45f;
                    _speedTimer = duration;
                    break;
            }
            GameEvents.PowerupCollected(type);
        }

        public bool TryHit()
        {
            if (HasShield)
            {
                HasShield = false;
                _shieldTimer = 0f;
                BrainrotArtFactory.TintRenderers(_visualRoot, _baseColor);
                return false;
            }
            return true;
        }

        void Update()
        {
            var pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, _targetPos.x, Time.deltaTime * LaneChangeSpeed);

            if (IsJumping)
            {
                _jumpTimer += Time.deltaTime;
                float t = Mathf.Clamp01(_jumpTimer / JumpDuration);
                pos.y = _baseY + Mathf.Sin(t * Mathf.PI) * JumpHeight;
                if (t >= 1f)
                {
                    IsJumping = false;
                    pos.y = _baseY;
                }
            }
            else if (IsSliding)
            {
                _slideTimer += Time.deltaTime;
                pos.y = _baseY * 0.5f;
                if (_slideTimer >= SlideDuration)
                {
                    IsSliding = false;
                    transform.localScale = _restScale;
                    pos.y = _baseY;
                }
            }
            else
            {
                pos.y = _baseY;
            }

            transform.position = pos;
            TickPowerups();
        }

        void TickPowerups()
        {
            if (MagnetActive)
            {
                _magnetTimer -= Time.deltaTime;
                if (_magnetTimer <= 0f) MagnetActive = false;
            }
            if (HasShield)
            {
                _shieldTimer -= Time.deltaTime;
                if (_shieldTimer <= 0f)
                {
                    HasShield = false;
                    BrainrotArtFactory.TintRenderers(_visualRoot, _baseColor);
                }
            }
            if (SpeedMultiplier > 1f)
            {
                _speedTimer -= Time.deltaTime;
                if (_speedTimer <= 0f) SpeedMultiplier = 1f;
            }
        }
    }
}
