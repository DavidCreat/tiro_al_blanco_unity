using UnityEngine;
using UnityEngine.UI;

namespace TiroAlBlanco.UI
{
    // Dynamic crosshair that expands on shoot and shrinks back.
    public class CrosshairController : MonoBehaviour
    {
        [Header("Crosshair Images")]
        [SerializeField] private RectTransform top;
        [SerializeField] private RectTransform bottom;
        [SerializeField] private RectTransform left;
        [SerializeField] private RectTransform right;
        [SerializeField] private Image centerDot;

        [Header("Settings")]
        [SerializeField] private float restSpread = 10f;
        [SerializeField] private float shootSpread = 40f;
        [SerializeField] private float shrinkSpeed = 8f;

        private float currentSpread;

        private void Awake()
        {
            currentSpread = restSpread;
        }

        private void OnEnable()
        {
            Gameplay.ShootingController.OnAmmoChanged += OnShoot;
            Core.GameManager.OnGameStart += Show;
            Core.GameManager.OnGameOver += Hide;
        }

        private void OnDisable()
        {
            Gameplay.ShootingController.OnAmmoChanged -= OnShoot;
            Core.GameManager.OnGameStart -= Show;
            Core.GameManager.OnGameOver -= Hide;
        }

        private void Update()
        {
            currentSpread = Mathf.Lerp(currentSpread, restSpread, Time.deltaTime * shrinkSpeed);
            ApplySpread(currentSpread);
        }

        private void OnShoot(int current, int max)
        {
            currentSpread = shootSpread;
        }

        private void ApplySpread(float spread)
        {
            if (top) top.anchoredPosition = new Vector2(0, spread);
            if (bottom) bottom.anchoredPosition = new Vector2(0, -spread);
            if (left) left.anchoredPosition = new Vector2(-spread, 0);
            if (right) right.anchoredPosition = new Vector2(spread, 0);
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }
}
