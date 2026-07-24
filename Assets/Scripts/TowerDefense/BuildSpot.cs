using UnityEngine;

namespace BrainrotRush
{
    public class BuildSpot : MonoBehaviour
    {
        public Tower OccupyingTower { get; private set; }
        public bool IsOccupied => OccupyingTower != null;

        Renderer _renderer;
        Color _idle = new Color(0.25f, 0.55f, 0.35f, 0.85f);
        Color _hover = new Color(0.4f, 0.85f, 0.5f, 0.95f);

        public void Init(TDGameManager mgr)
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer) _renderer.material.color = _idle;
            var col = GetComponent<Collider>();
            if (col) col.isTrigger = false;
        }

        public void SetHovered(bool hovered)
        {
            if (_renderer == null || IsOccupied) return;
            _renderer.material.color = hovered ? _hover : _idle;
        }

        public void Place(Tower tower)
        {
            OccupyingTower = tower;
            if (_renderer) _renderer.enabled = false;
        }

        public void Clear()
        {
            OccupyingTower = null;
            if (_renderer)
            {
                _renderer.enabled = true;
                _renderer.material.color = _idle;
            }
        }
    }
}
