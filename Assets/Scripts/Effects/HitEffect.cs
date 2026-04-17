using UnityEngine;

namespace TiroAlBlanco.Effects
{
    // Attach this to a ParticleSystem prefab. It auto-destroys after the particles finish.
    [RequireComponent(typeof(ParticleSystem))]
    public class HitEffect : MonoBehaviour
    {
        private ParticleSystem ps;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            ps.Play();
        }

        private void Update()
        {
            if (!ps.IsAlive())
                Destroy(gameObject);
        }
    }
}
