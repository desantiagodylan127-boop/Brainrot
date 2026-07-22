using UnityEngine;

namespace BrainrotRush
{
    public class RunnerObstacle : MonoBehaviour
    {
        public enum ObstacleKind { Low, High, Full }

        public ObstacleKind Kind = ObstacleKind.Full;
        public int Lane;

        public void Setup(ObstacleKind kind, int lane, Color color)
        {
            Kind = kind;
            Lane = lane;
            var rend = GetComponent<Renderer>();
            if (rend) rend.material.color = color;

            switch (kind)
            {
                case ObstacleKind.Low:
                    transform.localScale = new Vector3(1.4f, 0.7f, 1f);
                    transform.position = new Vector3(transform.position.x, 0.35f, transform.position.z);
                    break;
                case ObstacleKind.High:
                    transform.localScale = new Vector3(1.4f, 1.2f, 1f);
                    transform.position = new Vector3(transform.position.x, 1.6f, transform.position.z);
                    break;
                default:
                    transform.localScale = new Vector3(1.4f, 1.6f, 1f);
                    transform.position = new Vector3(transform.position.x, 0.8f, transform.position.z);
                    break;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<RunnerPlayer>();
            if (player == null) return;

            bool dodged = false;
            if (Kind == ObstacleKind.Low && player.IsJumping) dodged = true;
            if (Kind == ObstacleKind.High && player.IsSliding) dodged = true;

            if (dodged) return;

            var mgr = FindObjectOfType<RunnerGameManager>();
            if (mgr != null) mgr.OnPlayerHit();
        }
    }
}
