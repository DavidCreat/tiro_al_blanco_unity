using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;
using TiroAlBlanco.Core;
using TiroAlBlanco.Data;

namespace TiroAlBlanco.Gameplay
{
    public class TargetSpawner : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private GameConfig config;
        [SerializeField] private Target targetPrefab;
        [SerializeField] private Transform[] spawnPoints;

        private ObjectPool<Target> pool;
        private List<Target> activeTargets = new();
        private float spawnInterval;
        private float spawnTimer;
        private bool isSpawning;

        private void Awake()
        {
            if (targetPrefab == null)
            {
                Debug.LogError("TargetSpawner: targetPrefab no esta asignado!");
                return;
            }
            
            if (config == null)
            {
                Debug.LogError("TargetSpawner: config no esta asignado!");
                return;
            }
            
            pool = new ObjectPool<Target>(
                createFunc: () => Instantiate(targetPrefab),
                actionOnGet: t => { if (t != null) t.gameObject.SetActive(true); },
                actionOnRelease: t => { if (t != null) t.gameObject.SetActive(false); },
                actionOnDestroy: t => { if (t != null) Destroy(t.gameObject); },
                defaultCapacity: 10,
                maxSize: 20
            );
        }

        private void OnEnable()
        {
            GameManager.OnGameStart += StartSpawning;
            GameManager.OnGameOver += StopSpawning;
            GameManager.OnGamePause += StopSpawning;
            GameManager.OnGameResume += StartSpawning;
            Target.OnTargetHit += ReturnToPool;
            Target.OnTargetExpired += ReturnToPool;
        }

        private void OnDisable()
        {
            GameManager.OnGameStart -= StartSpawning;
            GameManager.OnGameOver -= StopSpawning;
            GameManager.OnGamePause -= StopSpawning;
            GameManager.OnGameResume -= StartSpawning;
            Target.OnTargetHit -= ReturnToPool;
            Target.OnTargetExpired -= ReturnToPool;
        }

        private void Update()
        {
            if (!isSpawning) return;

            // Increase difficulty over time by shrinking spawn interval
            spawnInterval = Mathf.Max(
                config.minimumSpawnInterval,
                spawnInterval - config.spawnIntervalDecreaseRate * Time.deltaTime
            );

            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                TrySpawn();
                spawnTimer = spawnInterval;
            }
        }

        private void TrySpawn()
        {
            if (pool == null || config == null) return;
            
            activeTargets.RemoveAll(t => t == null || !t.gameObject.activeSelf);

            if (activeTargets.Count >= config.maxConcurrentTargets) return;

            Transform spawnPoint = GetFreeSpawnPoint();
            if (spawnPoint == null) return;

            TargetData data = SelectRandomTargetType();
            if (data == null) return;

            Target target = pool.Get();
            if (target == null)
            {
                Debug.LogError("TargetSpawner: pool.Get() devolvio null!");
                return;
            }
            
            target.transform.position = spawnPoint.position;
            target.transform.rotation = spawnPoint.rotation;
            target.Initialize(data);
            activeTargets.Add(target);
        }

        private Transform GetFreeSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0) return null;

            // Shuffle to avoid always using same point
            int startIndex = Random.Range(0, spawnPoints.Length);
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                Transform point = spawnPoints[(startIndex + i) % spawnPoints.Length];
                if (!IsPointOccupied(point)) return point;
            }
            return null;
        }

        private bool IsPointOccupied(Transform point)
        {
            foreach (Target t in activeTargets)
            {
                if (t == null || !t.gameObject.activeSelf) continue;
                if (Vector3.Distance(t.transform.position, point.position) < 1f) return true;
            }
            return false;
        }

        private TargetData SelectRandomTargetType()
        {
            if (config.targetTypes == null || config.targetTypes.Length == 0) return null;

            float totalWeight = 0f;
            foreach (var data in config.targetTypes)
                totalWeight += data.spawnWeight;

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var data in config.targetTypes)
            {
                cumulative += data.spawnWeight;
                if (roll <= cumulative) return data;
            }

            return config.targetTypes[0];
        }

        private void ReturnToPool(Target target)
        {
            activeTargets.Remove(target);
            StartCoroutine(ReleaseWhenInactive(target));
        }

        private System.Collections.IEnumerator ReleaseWhenInactive(Target target)
        {
            yield return new WaitUntil(() => target == null || !target.gameObject.activeSelf);
            if (target != null && pool != null && target.gameObject.activeSelf == false)
                pool.Release(target);
        }

        private void StartSpawning()
        {
            isSpawning = true;
            spawnInterval = config.initialSpawnInterval;
            spawnTimer = 0.5f; // first spawn after brief delay
        }

        private void StopSpawning()
        {
            isSpawning = false;
            foreach (var t in activeTargets)
            {
                if (t != null && t.gameObject.activeSelf)
                    pool.Release(t);
            }
            activeTargets.Clear();
        }
    }
}
