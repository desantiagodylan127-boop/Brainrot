using System;
using UnityEngine;

namespace BrainrotRush
{
    public class SwipeInput : MonoBehaviour
    {
        public float MinSwipeDistance = 60f;
        public float MaxSwipeTime = 0.6f;

        public event Action OnSwipeLeft;
        public event Action OnSwipeRight;
        public event Action OnSwipeUp;
        public event Action OnSwipeDown;

        Vector2 _startPos;
        float _startTime;
        bool _tracking;

        void Update()
        {
            // Touch
            if (Input.touchCount > 0)
            {
                var t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                {
                    _tracking = true;
                    _startPos = t.position;
                    _startTime = Time.unscaledTime;
                }
                else if (t.phase == TouchPhase.Ended && _tracking)
                {
                    _tracking = false;
                    Process(_startPos, t.position, Time.unscaledTime - _startTime);
                }
            }

            // Mouse / editor
            if (Input.GetMouseButtonDown(0))
            {
                _tracking = true;
                _startPos = Input.mousePosition;
                _startTime = Time.unscaledTime;
            }
            else if (Input.GetMouseButtonUp(0) && _tracking)
            {
                _tracking = false;
                Process(_startPos, Input.mousePosition, Time.unscaledTime - _startTime);
            }

            // Keyboard fallback for quick testing
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) OnSwipeLeft?.Invoke();
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) OnSwipeRight?.Invoke();
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) OnSwipeUp?.Invoke();
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) OnSwipeDown?.Invoke();
        }

        void Process(Vector2 start, Vector2 end, float duration)
        {
            if (duration > MaxSwipeTime) return;
            var delta = end - start;
            if (delta.magnitude < MinSwipeDistance) return;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0) OnSwipeRight?.Invoke();
                else OnSwipeLeft?.Invoke();
            }
            else
            {
                if (delta.y > 0) OnSwipeUp?.Invoke();
                else OnSwipeDown?.Invoke();
            }
        }
    }
}
