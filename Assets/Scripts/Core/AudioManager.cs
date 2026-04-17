using UnityEngine;

namespace TiroAlBlanco.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("SFX Clips")]
        [SerializeField] private AudioClip shootClip;
        [SerializeField] private AudioClip hitClip;
        [SerializeField] private AudioClip missClip;
        [SerializeField] private AudioClip reloadClip;
        [SerializeField] private AudioClip bonusHitClip;
        [SerializeField] private AudioClip comboClip;
        [SerializeField] private AudioClip targetPopUpClip;
        [SerializeField] private AudioClip gameOverClip;
        [SerializeField] private AudioClip countdownClip;

        [Header("Music")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameplayMusic;

        [Header("Volume")]
        [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.5f;

        private AudioSource sfxSource;
        private AudioSource musicSource;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            sfxSource = GetComponent<AudioSource>();
            sfxSource.playOnAwake = false;

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume;
        }

        private void OnEnable()
        {
            GameManager.OnGameStart += PlayGameplayMusic;
            GameManager.OnGameOver += OnGameOver;
            GameManager.OnGamePause += () => musicSource.Pause();
            GameManager.OnGameResume += () => musicSource.UnPause();
        }

        private void OnDisable()
        {
            GameManager.OnGameStart -= PlayGameplayMusic;
            GameManager.OnGameOver -= OnGameOver;
        }

        public void PlayShoot() => PlaySFX(shootClip);
        public void PlayHit() => PlaySFX(hitClip);
        public void PlayMiss() => PlaySFX(missClip);
        public void PlayReload() => PlaySFX(reloadClip);
        public void PlayBonusHit() => PlaySFX(bonusHitClip);
        public void PlayCombo() => PlaySFX(comboClip);
        public void PlayTargetPopUp() => PlaySFX(targetPopUpClip);
        public void PlayGameOver() => PlaySFX(gameOverClip);
        public void PlayCountdown() => PlaySFX(countdownClip);

        private void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        public void PlayMenuMusic()
        {
            if (menuMusic == null) return;
            musicSource.clip = menuMusic;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

        private void PlayGameplayMusic()
        {
            if (gameplayMusic == null) return;
            musicSource.clip = gameplayMusic;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

        private void OnGameOver()
        {
            musicSource.Stop();
            PlayGameOver();
        }

        public void SetSFXVolume(float vol)
        {
            sfxVolume = Mathf.Clamp01(vol);
        }

        public void SetMusicVolume(float vol)
        {
            musicVolume = Mathf.Clamp01(vol);
            musicSource.volume = musicVolume;
        }
    }
}
