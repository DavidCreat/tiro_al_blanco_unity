using UnityEngine;
using TiroAlBlanco.Core;

namespace TiroAlBlanco.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private float destroyDelay = 4f;

        private Rigidbody rb;
        private bool hasHit;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Launch(Vector3 velocity)
        {
            rb.linearVelocity = velocity;
        }

        private void Update()
        {
            if (hasHit || rb.linearVelocity.sqrMagnitude < 0.05f) return;
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasHit) return;
            hasHit = true;

            Target target = other.GetComponentInParent<Target>();
            if (target != null && target.IsActive)
            {
                bool isCenter = other.GetComponent<CenterZone>() != null;
                if (isCenter)
                {
                    target.OnHit();
                    ScoreManager.Instance?.AddPoints(10, transform.position);
                    AudioManager.Instance?.PlayHit();
                }
                else
                {
                    ScoreManager.Instance?.MissShot();
                    AudioManager.Instance?.PlayMiss();
                }
            }

            if (rb != null) rb.isKinematic = true;
            transform.SetParent(other.transform, true);
            Destroy(gameObject, destroyDelay);
        }
    }
}
