using UnityEngine;
using System;

namespace TiroAlBlanco.Core
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        private const string HighScoreKey = "HighScore";

        public static event Action<int> OnScoreChanged;
        public static event Action<int, int> OnComboChanged;  // comboCount, multiplier
        public static event Action<int, Vector3> OnPointsEarned; // points, worldPos
        public static event Action<int> OnNewHighScore;

        [Header("Combo Settings")]
        [SerializeField] private float comboResetTime = 2f;
        [SerializeField] private int comboTierSize = 3; // hits per multiplier tier

        public int CurrentScore { get; private set; }
        public int HighScore { get; private set; }
        public int ComboCount { get; private set; }
        public int Multiplier => Mathf.Clamp(1 + (ComboCount / comboTierSize), 1, 8);

        private float comboTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        private void OnEnable()
        {
            GameManager.OnGameStart += ResetScore;
            GameManager.OnGameOver += CheckHighScore;
        }

        private void OnDisable()
        {
            GameManager.OnGameStart -= ResetScore;
            GameManager.OnGameOver -= CheckHighScore;
        }

        private void Update()
        {
            if (ComboCount == 0) return;
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f) ResetCombo();
        }

        public void AddPoints(int basePoints, Vector3 worldPosition)
        {
            ComboCount++;
            comboTimer = comboResetTime;

            int earned = basePoints * Multiplier;
            CurrentScore += earned;

            OnScoreChanged?.Invoke(CurrentScore);
            OnComboChanged?.Invoke(ComboCount, Multiplier);
            OnPointsEarned?.Invoke(earned, worldPosition);
        }

        public void MissShot()
        {
            ResetCombo();
        }

        private void ResetCombo()
        {
            ComboCount = 0;
            comboTimer = 0f;
            OnComboChanged?.Invoke(0, 1);
        }

        private void ResetScore()
        {
            CurrentScore = 0;
            ResetCombo();
            OnScoreChanged?.Invoke(0);
        }

        private void CheckHighScore()
        {
            if (CurrentScore <= HighScore) return;
            HighScore = CurrentScore;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
            OnNewHighScore?.Invoke(HighScore);
        }
    }
}
