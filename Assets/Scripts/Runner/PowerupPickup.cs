using UnityEngine;

namespace BrainrotRush
{
    public class PowerupPickup : MonoBehaviour
    {
        public PowerupType Type;
        public float Duration = 6f;

        public void Setup(PowerupType type)
        {
            Type = type;
            var rend = GetComponent<Renderer>();
            if (rend == null) return;
            rend.material.color = type switch
            {
                PowerupType.Magnet => new Color(1f, 0.85f, 0.2f),
                PowerupType.Shield => new Color(0.3f, 0.85f, 1f),
                PowerupType.SpeedBoost => new Color(1f, 0.35f, 0.2f),
                _ => Color.white
            };
        }

        void Update() => transform.Rotate(0f, 120f * Time.deltaTime, 40f * Time.deltaTime);

        void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<RunnerPlayer>();
            if (player == null) return;
            player.ActivatePowerup(Type, Duration);
            Destroy(gameObject);
        }
    }
}
