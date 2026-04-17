using UnityEngine;
using System.Collections;

namespace TiroAlBlanco.Effects
{
    public class CameraShake : MonoBehaviour
    {
        private Vector3 originalPosition;
        private Coroutine shakeCoroutine;

        private void Awake()
        {
            originalPosition = transform.localPosition;
        }

        public void Shake(float magnitude, float duration)
        {
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(ShakeRoutine(magnitude, duration));
        }

        private IEnumerator ShakeRoutine(float magnitude, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float damping = 1f - t; // shake fades out
                Vector3 offset = Random.insideUnitSphere * magnitude * damping;
                transform.localPosition = originalPosition + offset;
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
            shakeCoroutine = null;
        }
    }
}
