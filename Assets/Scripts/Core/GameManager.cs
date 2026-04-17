using UnityEngine;
using System;

namespace TiroAlBlanco.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private float gameDuration = 60f;

        public static event Action OnGameStart;
        public static event Action OnGameOver;
        public static event Action OnGamePause;
        public static event Action OnGameResume;
        public static event Action<float> OnTimerUpdate;

        public GameState CurrentState { get; private set; } = GameState.Menu;
        public float TimeRemaining { get; private set; }
        public float GameDuration => gameDuration;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            if (CurrentState != GameState.Playing) return;

            TimeRemaining -= Time.deltaTime;
            OnTimerUpdate?.Invoke(TimeRemaining);

            if (TimeRemaining <= 0f)
            {
                TimeRemaining = 0f;
                EndGame();
            }
        }

        public void StartGame()
        {
            Debug.Log("GameManager.StartGame() llamado - Iniciando juego");
            TimeRemaining = gameDuration;
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log($"Invocando evento OnGameStart. Suscriptores: {OnGameStart?.GetInvocationList().Length ?? 0}");
            OnGameStart?.Invoke();
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;
            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnGamePause?.Invoke();
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            OnGameResume?.Invoke();
        }

        public void ReturnToMenu()
        {
            CurrentState = GameState.Menu;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void EndGame()
        {
            CurrentState = GameState.GameOver;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnGameOver?.Invoke();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }

    public enum GameState { Menu, Playing, Paused, GameOver }
}
