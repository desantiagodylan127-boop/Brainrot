using UnityEngine;

namespace BrainrotRush
{
    public class CoinPickup : MonoBehaviour
    {
        public int Value = 1;
        RunnerPlayer _player;

        void Update()
        {
            transform.Rotate(0f, 180f * Time.deltaTime, 0f);

            if (_player == null) _player = FindObjectOfType<RunnerPlayer>();
            if (_player != null && _player.MagnetActive)
            {
                float dist = Vector3.Distance(transform.position, _player.transform.position);
                if (dist < 6f)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        _player.transform.position,
                        20f * Time.deltaTime);
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<RunnerPlayer>() == null) return;
            var mgr = FindObjectOfType<RunnerGameManager>();
            if (mgr != null) mgr.CollectCoin(Value);
            Destroy(gameObject);
        }
    }
}
