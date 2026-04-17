using UnityEngine;
using System;
using System.Collections;
using TiroAlBlanco.Data;

namespace TiroAlBlanco.Gameplay
{
    public class Target : MonoBehaviour
    {
        public static event Action<Target> OnTargetHit;
        public static event Action<Target> OnTargetExpired;

        [Header("References")]
        [SerializeField] private ParticleSystem hitParticlesPrefab;
        [SerializeField] private Animator animator;
        [SerializeField] private TargetBuilder targetBuilder;

        private static readonly int AnimPopUp = Animator.StringToHash("PopUp");
        private static readonly int AnimPopDown = Animator.StringToHash("PopDown");

        public TargetData Data { get; private set; }
        public bool IsActive { get; private set; }

        private Vector3 startPosition;
        private Coroutine lifeCoroutine;

        public void Initialize(TargetData data)
        {
            Data = data;
            IsActive = true;
            startPosition = transform.position;

            if (targetBuilder == null) targetBuilder = GetComponent<TargetBuilder>();
            targetBuilder?.Build(data);
            transform.localScale = Vector3.one * data.scale;

            float lifeTime = UnityEngine.Random.Range(data.minActiveTime, data.maxActiveTime);
            lifeCoroutine = StartCoroutine(LifetimeRoutine(lifeTime));

            if (animator != null) animator.SetTrigger(AnimPopUp);
            Core.AudioManager.Instance?.PlayTargetPopUp();
        }

        private void Update()
        {
            if (!IsActive || Data == null || !Data.moves) return;

            float offset = Mathf.Sin(Time.time * Data.moveSpeed) * Data.moveDistance;
            transform.position = startPosition + Vector3.right * offset;
        }

        public void OnHit()
        {
            if (!IsActive) return;
            IsActive = false;

            if (lifeCoroutine != null) StopCoroutine(lifeCoroutine);

            SpawnHitParticles();
            OnTargetHit?.Invoke(this);
            StartCoroutine(PopDownAndDeactivate());
        }

        private IEnumerator LifetimeRoutine(float lifetime)
        {
            yield return new WaitForSeconds(lifetime);
            if (!IsActive) yield break;

            IsActive = false;
            OnTargetExpired?.Invoke(this);
            yield return PopDownAndDeactivate();
        }

        private IEnumerator PopDownAndDeactivate()
        {
            if (animator != null)
            {
                animator.SetTrigger(AnimPopDown);
                yield return new WaitForSeconds(0.35f);
            }
            gameObject.SetActive(false);
        }

        private void SpawnHitParticles()
        {
            if (hitParticlesPrefab == null) return;
            ParticleSystem ps = Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
            var main = ps.main;
            main.startColor = Data != null ? Data.targetColor : Color.red;
            Destroy(ps.gameObject, 3f);
        }

        private void OnDisable()
        {
            IsActive = false;
            if (lifeCoroutine != null)
            {
                StopCoroutine(lifeCoroutine);
                lifeCoroutine = null;
            }
        }
    }
}
