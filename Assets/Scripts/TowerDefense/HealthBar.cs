using UnityEngine;
using UnityEngine.UI;

namespace BrainrotRush
{
    public class HealthBar : MonoBehaviour
    {
        Enemy _enemy;
        Image _fill;
        Canvas _canvas;

        public void Init(Enemy enemy)
        {
            _enemy = enemy;

            var canvasGo = new GameObject("HealthCanvas");
            canvasGo.transform.SetParent(transform);
            canvasGo.transform.localPosition = Vector3.up * (enemy.IsBoss ? 2.2f : 1.4f);
            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            var rt = canvasGo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1.2f, 0.2f);
            canvasGo.transform.localScale = Vector3.one * 0.02f;
            canvasGo.AddComponent<CanvasScaler>();

            var bg = new GameObject("BG");
            bg.transform.SetParent(canvasGo.transform, false);
            var bgRt = bg.AddComponent<RectTransform>();
            UIFactory.StretchFull(bgRt);
            bg.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

            var fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(canvasGo.transform, false);
            var fillRt = fillGo.AddComponent<RectTransform>();
            UIFactory.StretchFull(fillRt);
            _fill = fillGo.AddComponent<Image>();
            _fill.color = new Color(0.2f, 0.9f, 0.35f);
            _fill.type = Image.Type.Filled;
            _fill.fillMethod = Image.FillMethod.Horizontal;
        }

        void LateUpdate()
        {
            if (_enemy == null || _fill == null) return;
            _fill.fillAmount = _enemy.HealthNormalized;
            if (_canvas != null && Camera.main != null)
                _canvas.transform.rotation = Quaternion.LookRotation(_canvas.transform.position - Camera.main.transform.position);
        }
    }
}
