using UnityEngine;
using TMPro;
using System.Collections;

namespace TiroAlBlanco.UI
{
    // Floating score text that rises and fades out. Attach to a Canvas-space or world-space Text prefab.
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private float duration = 1f;
        [SerializeField] private float riseSpeed = 80f;
        [SerializeField] private AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        private TextMeshProUGUI label;
        private RectTransform rectTransform;

        private void Awake()
        {
            label = GetComponent<TextMeshProUGUI>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void Show(string text, Color color)
        {
            label.text = text;
            label.color = color;
            StartCoroutine(AnimateRoutine());
        }

        private IEnumerator AnimateRoutine()
        {
            float elapsed = 0f;
            Vector2 startPos = rectTransform.anchoredPosition;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                rectTransform.anchoredPosition = startPos + Vector2.up * riseSpeed * t;

                Color c = label.color;
                c.a = alphaCurve.Evaluate(t);
                label.color = c;

                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
