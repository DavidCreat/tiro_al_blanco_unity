using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace TiroAlBlanco.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private Image[] ammoSlots;
        [SerializeField] private Sprite ammoFull;
        [SerializeField] private Sprite ammoEmpty;
        [SerializeField] private GameObject reloadingIndicator;

        [Header("Floating Score")]
        [SerializeField] private FloatingText floatingTextPrefab;
        [SerializeField] private Canvas worldCanvas;
        [SerializeField] private Camera mainCamera;

        [Header("Screens")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Game Over")]
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI newHighScoreLabel;

        [Header("Power Bar")]
        [SerializeField] private GameObject powerBarContainer;
        [SerializeField] private Image powerBarFill;

        [Header("Timer Colors")]
        [SerializeField] private Color normalTimeColor = Color.white;
        [SerializeField] private Color warningTimeColor = Color.yellow;
        [SerializeField] private Color criticalTimeColor = Color.red;
        [SerializeField] private float warningThreshold = 15f;
        [SerializeField] private float criticalThreshold = 5f;

        private void OnEnable()
        {
            Core.GameManager.OnGameStart += OnGameStart;
            Core.GameManager.OnGameOver += OnGameOver;
            Core.GameManager.OnGamePause += OnPause;
            Core.GameManager.OnGameResume += OnResume;
            Core.GameManager.OnTimerUpdate += UpdateTimer;

            Core.ScoreManager.OnScoreChanged += UpdateScore;
            Core.ScoreManager.OnComboChanged += UpdateCombo;
            Core.ScoreManager.OnPointsEarned += ShowFloatingPoints;
            Core.ScoreManager.OnNewHighScore += OnNewHighScore;

            Gameplay.ShootingController.OnAmmoChanged += UpdateAmmo;
            Gameplay.ShootingController.OnReloadStart += ShowReloading;
            Gameplay.ShootingController.OnReloadEnd += HideReloading;
            Gameplay.ShootingController.OnPowerChanged += UpdatePowerBar;
        }

        private void OnDisable()
        {
            Core.GameManager.OnGameStart -= OnGameStart;
            Core.GameManager.OnGameOver -= OnGameOver;
            Core.GameManager.OnGamePause -= OnPause;
            Core.GameManager.OnGameResume -= OnResume;
            Core.GameManager.OnTimerUpdate -= UpdateTimer;

            Core.ScoreManager.OnScoreChanged -= UpdateScore;
            Core.ScoreManager.OnComboChanged -= UpdateCombo;
            Core.ScoreManager.OnPointsEarned -= ShowFloatingPoints;
            Core.ScoreManager.OnNewHighScore -= OnNewHighScore;

            Gameplay.ShootingController.OnAmmoChanged -= UpdateAmmo;
            Gameplay.ShootingController.OnReloadStart -= ShowReloading;
            Gameplay.ShootingController.OnReloadEnd -= HideReloading;
            Gameplay.ShootingController.OnPowerChanged -= UpdatePowerBar;
        }

        private void Start()
        {
            Debug.Log("UIManager.Start() - Mostrando menu principal");
            ShowMainMenu();
        }

        // --- Screen Management ---

        private void ShowMainMenu()
        {
            mainMenuPanel?.SetActive(true);
            hudPanel?.SetActive(false);
            pauseMenuPanel?.SetActive(false);
            gameOverPanel?.SetActive(false);
        }

        private void OnGameStart()
        {
            Debug.Log("UIManager.OnGameStart() - Ocultando menu y mostrando HUD");
            mainMenuPanel?.SetActive(false);
            pauseMenuPanel?.SetActive(false);
            gameOverPanel?.SetActive(false);
            hudPanel?.SetActive(true);
            reloadingIndicator?.SetActive(false);
            UpdateScore(0);
            UpdateCombo(0, 1);
        }

        private void OnGameOver()
        {
            hudPanel?.SetActive(false);
            gameOverPanel?.SetActive(true);

            int score = Core.ScoreManager.Instance?.CurrentScore ?? 0;
            int high = Core.ScoreManager.Instance?.HighScore ?? 0;

            if (finalScoreText) finalScoreText.text = $"Score: {score:N0}";
            if (highScoreText) highScoreText.text = $"Best: {high:N0}";
            if (newHighScoreLabel) newHighScoreLabel.gameObject.SetActive(false);
        }

        private void OnPause()
        {
            pauseMenuPanel?.SetActive(true);
        }

        private void OnResume()
        {
            pauseMenuPanel?.SetActive(false);
        }

        // --- HUD Updates ---

        private void UpdateScore(int score)
        {
            if (scoreText) scoreText.text = score.ToString("N0");
        }

        private void UpdateTimer(float time)
        {
            int seconds = Mathf.CeilToInt(time);
            if (timerText)
            {
                timerText.text = seconds.ToString();
                timerText.color = seconds <= criticalThreshold ? criticalTimeColor
                                : seconds <= warningThreshold ? warningTimeColor
                                : normalTimeColor;
            }
        }

        private void UpdateCombo(int comboCount, int multiplier)
        {
            if (comboText == null) return;

            if (comboCount < 3)
            {
                comboText.gameObject.SetActive(false);
                return;
            }

            comboText.gameObject.SetActive(true);
            comboText.text = multiplier > 1 ? $"x{multiplier} COMBO!" : $"{comboCount} HIT";
        }

        private void UpdateAmmo(int current, int max)
        {
            if (ammoSlots == null) return;
            for (int i = 0; i < ammoSlots.Length; i++)
            {
                if (ammoSlots[i] == null) continue;
                ammoSlots[i].sprite = i < current ? ammoFull : ammoEmpty;
            }
        }

        private void ShowReloading()
        {
            reloadingIndicator?.SetActive(true);
        }

        private void HideReloading()
        {
            reloadingIndicator?.SetActive(false);
        }

        private void UpdatePowerBar(float power)
        {
            if (powerBarContainer != null)
                powerBarContainer.SetActive(power > 0f);
            if (powerBarFill != null)
                powerBarFill.fillAmount = power;
        }

        private void ShowFloatingPoints(int points, Vector3 worldPos)
        {
            if (floatingTextPrefab == null || worldCanvas == null || mainCamera == null) return;

            Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPos);
            FloatingText instance = Instantiate(floatingTextPrefab, worldCanvas.transform);
            instance.GetComponent<RectTransform>().position = screenPos;

            bool isBonus = points >= 300;
            Color color = isBonus ? Color.yellow : Color.white;
            instance.Show($"+{points}", color);
        }

        private void OnNewHighScore(int score)
        {
            if (newHighScoreLabel) newHighScoreLabel.gameObject.SetActive(true);
        }

        // --- Button Callbacks (wire up in Inspector) ---

        public void OnPlayButtonPressed()
        {
            if (Core.GameManager.Instance == null)
            {
                Debug.LogError("GameManager.Instance es null! Asegurate de que GameManager este en la escena.");
                return;
            }
            
            Debug.Log("Boton de jugar presionado - Iniciando juego");
            Core.GameManager.Instance.StartGame();
        }

        public void OnPauseButtonPressed()
        {
            Core.GameManager.Instance?.PauseGame();
        }

        public void OnResumeButtonPressed()
        {
            Core.GameManager.Instance?.ResumeGame();
        }

        public void OnRestartButtonPressed()
        {
            Core.GameManager.Instance?.StartGame();
        }

        public void OnMainMenuButtonPressed()
        {
            Core.GameManager.Instance?.ReturnToMenu();
            ShowMainMenu();
        }

        public void OnQuitButtonPressed()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
