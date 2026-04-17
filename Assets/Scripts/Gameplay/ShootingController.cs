using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TiroAlBlanco.Core;
using TiroAlBlanco.Effects;

namespace TiroAlBlanco.Gameplay
{
    [RequireComponent(typeof(Camera))]
    public class ShootingController : MonoBehaviour
    {
        public static ShootingController Instance { get; private set; }

        [Header("Arrow Settings")]
        [SerializeField] private Arrow arrowPrefab;
        [SerializeField] private Transform arrowSpawnPoint;
        [SerializeField] private float minLaunchForce = 8f;
        [SerializeField] private float maxLaunchForce = 35f;
        [SerializeField] private float maxChargeTime = 2f;

        [Header("Effects")]
        [SerializeField] private CameraShake cameraShake;
        [SerializeField] private float shootShakeMagnitude = 0.04f;
        [SerializeField] private float shootShakeDuration = 0.1f;

        public static event Action<float> OnPowerChanged;   // 0..1
        public static event Action OnArrowFired;

        // Keep these for backward-compat with UIManager ammo slots (ignored now)
        public static event Action<int, int> OnAmmoChanged;
        public static event Action OnReloadStart;
        public static event Action OnReloadEnd;

        public float CurrentPower { get; private set; }
        public bool IsCharging { get; private set; }

        private Camera cam;
        private bool canShoot;
        private float chargeStartTime;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
            cam = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            GameManager.OnGameStart += EnableShooting;
            GameManager.OnGameOver += DisableShooting;
            GameManager.OnGamePause += DisableShooting;
            GameManager.OnGameResume += EnableShooting;
        }

        private void OnDisable()
        {
            GameManager.OnGameStart -= EnableShooting;
            GameManager.OnGameOver -= DisableShooting;
            GameManager.OnGamePause -= DisableShooting;
            GameManager.OnGameResume -= EnableShooting;
        }

        private void Update()
        {
            if (!canShoot || Keyboard.current == null) return;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                BeginCharge();

            if (IsCharging)
                UpdateCharge();

            if (Keyboard.current.spaceKey.wasReleasedThisFrame && IsCharging)
                Fire();
        }

        private void BeginCharge()
        {
            IsCharging = true;
            chargeStartTime = Time.time;
            CurrentPower = 0f;
            OnPowerChanged?.Invoke(0f);
        }

        private void UpdateCharge()
        {
            CurrentPower = Mathf.Clamp01((Time.time - chargeStartTime) / maxChargeTime);
            OnPowerChanged?.Invoke(CurrentPower);
        }

        private void Fire()
        {
            IsCharging = false;

            if (arrowPrefab == null)
            {
                Debug.LogWarning("ShootingController: arrowPrefab no asignado.");
                CurrentPower = 0f;
                OnPowerChanged?.Invoke(0f);
                return;
            }

            Transform spawnTF = arrowSpawnPoint != null ? arrowSpawnPoint : transform;
            Arrow arrow = Instantiate(arrowPrefab, spawnTF.position, spawnTF.rotation);

            float force = Mathf.Lerp(minLaunchForce, maxLaunchForce, CurrentPower);
            arrow.Launch(spawnTF.forward * force);

            if (cameraShake != null)
                cameraShake.Shake(shootShakeMagnitude * CurrentPower, shootShakeDuration);

            AudioManager.Instance?.PlayShoot();
            OnArrowFired?.Invoke();

            CurrentPower = 0f;
            OnPowerChanged?.Invoke(0f);
        }

        private void EnableShooting() => canShoot = true;

        private void DisableShooting()
        {
            canShoot = false;
            IsCharging = false;
            CurrentPower = 0f;
            OnPowerChanged?.Invoke(0f);
        }
    }
}
