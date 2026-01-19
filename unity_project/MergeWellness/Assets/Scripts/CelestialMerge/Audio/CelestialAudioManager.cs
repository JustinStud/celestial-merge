using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace CelestialMerge.Audio
{
    /// <summary>
    /// Zentrale Audio-Verwaltung für Background Music und Sound Effects
    /// Integriert mit SettingsMenu für Volume Control
    /// </summary>
    public class CelestialAudioManager : MonoBehaviour
    {
        public static CelestialAudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Audio Mixer (Optional)")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Music Clips")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private AudioClip menuMusic;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip mergeSound;
        [SerializeField] private AudioClip levelUpSound;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip errorSound;
        [SerializeField] private AudioClip coinCollectSound;

        [Header("Volume Settings")]
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 0.8f;
        [SerializeField] private bool musicEnabled = true;
        [SerializeField] private bool sfxEnabled = true;

        // Audio Pool für effiziente Sound-Playback
        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private List<AudioSource> activeAudioSources = new List<AudioSource>();
        private const int POOL_SIZE = 10;

        private void Awake()
        {
            // Singleton Setup
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Erstelle Audio Sources falls nicht vorhanden
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            // Initialisiere Audio Pool
            InitializeAudioPool();

            // Lade gespeicherte Einstellungen
            LoadAudioSettings();
        }

        private void Start()
        {
            // Starte Background Music falls vorhanden
            if (backgroundMusic != null && musicEnabled)
            {
                PlayBackgroundMusic();
            }
        }

        #region Audio Pool

        /// <summary>
        /// Initialisiert Audio Source Pool für effiziente Sound-Playback
        /// </summary>
        private void InitializeAudioPool()
        {
            for (int i = 0; i < POOL_SIZE; i++)
            {
                GameObject poolObj = new GameObject($"SFXPool_{i}");
                poolObj.transform.SetParent(transform);
                AudioSource source = poolObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                audioSourcePool.Enqueue(source);
            }
        }

        /// <summary>
        /// Holt AudioSource aus Pool
        /// </summary>
        private AudioSource GetPooledAudioSource()
        {
            if (audioSourcePool.Count > 0)
            {
                AudioSource source = audioSourcePool.Dequeue();
                activeAudioSources.Add(source);
                return source;
            }

            // Falls Pool leer, erstelle neue
            GameObject newObj = new GameObject("SFXPool_New");
            newObj.transform.SetParent(transform);
            AudioSource newSource = newObj.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            activeAudioSources.Add(newSource);
            return newSource;
        }

        /// <summary>
        /// Gibt AudioSource zurück in Pool
        /// </summary>
        private void ReturnToPool(AudioSource source)
        {
            if (source != null)
            {
                source.Stop();
                source.clip = null;
                activeAudioSources.Remove(source);
                audioSourcePool.Enqueue(source);
            }
        }

        /// <summary>
        /// Bereinigt beendete AudioSources
        /// </summary>
        private void Update()
        {
            for (int i = activeAudioSources.Count - 1; i >= 0; i--)
            {
                AudioSource source = activeAudioSources[i];
                if (source != null && !source.isPlaying)
                {
                    ReturnToPool(source);
                }
            }
        }

        #endregion

        #region Music

        /// <summary>
        /// Spielt Background Music
        /// </summary>
        public void PlayBackgroundMusic()
        {
            if (musicSource == null || backgroundMusic == null || !musicEnabled) return;

            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume;
            musicSource.Play();
            Debug.Log($"🎵 Background Music gestartet: {backgroundMusic.name}");
        }

        /// <summary>
        /// Spielt Menu Music
        /// </summary>
        public void PlayMenuMusic()
        {
            if (musicSource == null || menuMusic == null || !musicEnabled) return;

            musicSource.clip = menuMusic;
            musicSource.volume = musicVolume;
            musicSource.Play();
            Debug.Log($"🎵 Menu Music gestartet: {menuMusic.name}");
        }

        /// <summary>
        /// Stoppt Music
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }

        /// <summary>
        /// Fade Out Music
        /// </summary>
        public void FadeOutMusic(float duration = 1f)
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                StartCoroutine(FadeOutCoroutine(musicSource, duration));
            }
        }

        private System.Collections.IEnumerator FadeOutCoroutine(AudioSource source, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            source.Stop();
            source.volume = startVolume;
        }

        #endregion

        #region Sound Effects

        /// <summary>
        /// Spielt Sound Effect mit Volume und Pitch Variation
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f, float pitchVariation = 0.1f)
        {
            if (clip == null || !sfxEnabled) return;

            AudioSource source = GetPooledAudioSource();
            source.clip = clip;
            source.volume = sfxVolume * volumeMultiplier;
            source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            source.Play();

            // Auto-Return nach Clip-Dauer
            StartCoroutine(ReturnAfterPlay(source, clip.length));
        }

        private System.Collections.IEnumerator ReturnAfterPlay(AudioSource source, float duration)
        {
            yield return new WaitForSeconds(duration + 0.1f);
            ReturnToPool(source);
        }

        /// <summary>
        /// Spielt Merge Sound
        /// </summary>
        public void PlayMergeSound()
        {
            PlaySFX(mergeSound, 1f, 0.2f);
        }

        /// <summary>
        /// Spielt Level Up Sound
        /// </summary>
        public void PlayLevelUpSound()
        {
            PlaySFX(levelUpSound, 1.2f);
        }

        /// <summary>
        /// Spielt Button Click Sound
        /// </summary>
        public void PlayButtonClickSound()
        {
            PlaySFX(buttonClickSound, 0.8f, 0.05f);
        }

        /// <summary>
        /// Spielt Error Sound
        /// </summary>
        public void PlayErrorSound()
        {
            PlaySFX(errorSound, 1f, 0.05f);
        }

        /// <summary>
        /// Spielt Coin Collect Sound
        /// </summary>
        public void PlayCoinCollectSound()
        {
            PlaySFX(coinCollectSound, 1f, 0.15f);
        }

        #endregion

        #region Volume Control

        /// <summary>
        /// Setzt Music Volume (0-1)
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }

            // Optional: Audio Mixer verwenden
            if (audioMixer != null)
            {
                audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20f);
            }

            SaveAudioSettings();
        }

        /// <summary>
        /// Setzt SFX Volume (0-1)
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            SaveAudioSettings();
        }

        /// <summary>
        /// Aktiviert/Deaktiviert Music
        /// </summary>
        public void SetMusicEnabled(bool enabled)
        {
            musicEnabled = enabled;
            if (!enabled && musicSource != null)
            {
                musicSource.Stop();
            }
            else if (enabled && musicSource != null && musicSource.clip != null && !musicSource.isPlaying)
            {
                musicSource.Play();
            }
            SaveAudioSettings();
        }

        /// <summary>
        /// Aktiviert/Deaktiviert SFX
        /// </summary>
        public void SetSFXEnabled(bool enabled)
        {
            sfxEnabled = enabled;
            SaveAudioSettings();
        }

        /// <summary>
        /// Gibt aktuellen Music Volume zurück
        /// </summary>
        public float GetMusicVolume() => musicVolume;

        /// <summary>
        /// Gibt aktuellen SFX Volume zurück
        /// </summary>
        public float GetSFXVolume() => sfxVolume;

        /// <summary>
        /// Ist Music aktiviert?
        /// </summary>
        public bool IsMusicEnabled() => musicEnabled;

        /// <summary>
        /// Ist SFX aktiviert?
        /// </summary>
        public bool IsSFXEnabled() => sfxEnabled;

        #endregion

        #region Save/Load

        /// <summary>
        /// Lädt Audio-Einstellungen aus PlayerPrefs
        /// </summary>
        private void LoadAudioSettings()
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.8f);
            musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            sfxEnabled = PlayerPrefs.GetInt("SfxEnabled", 1) == 1;

            // Wende Einstellungen an
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);
            SetMusicEnabled(musicEnabled);
            SetSFXEnabled(sfxEnabled);
        }

        /// <summary>
        /// Speichert Audio-Einstellungen in PlayerPrefs
        /// </summary>
        private void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
            PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
            PlayerPrefs.SetInt("SfxEnabled", sfxEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        #endregion
    }
}
