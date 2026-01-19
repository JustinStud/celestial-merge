using UnityEngine;
using UnityEngine.UI;

namespace MergeWellness
{
    /// <summary>
    /// Einstellungs-Menü für Audio, Grafik, etc.
    /// </summary>
    public class SettingsMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button backButton;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private UnityEngine.UI.Dropdown qualityDropdown;

        [Header("Settings")]
        [SerializeField] private float defaultMusicVolume = 0.7f;
        [SerializeField] private float defaultSfxVolume = 0.8f;

        private MenuManager menuManager;

        private void Awake()
        {
            menuManager = FindFirstObjectByType<MenuManager>();
        }

        private void Start()
        {
            SetupUI();
            LoadSettings();
        }

        private void SetupUI()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(OnBackClicked);
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.onValueChanged.RemoveAllListeners();
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.RemoveAllListeners();
                sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            }

            if (fullscreenToggle != null)
            {
                fullscreenToggle.onValueChanged.RemoveAllListeners();
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
            }

            if (qualityDropdown != null)
            {
                qualityDropdown.onValueChanged.RemoveAllListeners();
                qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
                SetupQualityDropdown();
            }
        }

        private void SetupQualityDropdown()
        {
            if (qualityDropdown == null) return;

            qualityDropdown.ClearOptions();
            string[] qualityNames = QualitySettings.names;
            foreach (string name in qualityNames)
            {
                qualityDropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData(name));
            }
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.RefreshShownValue();
        }

        private void LoadSettings()
        {
            // Lade gespeicherte Einstellungen
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
            float sfxVol = PlayerPrefs.GetFloat("SfxVolume", defaultSfxVolume);
            bool fullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
            int quality = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());

            if (musicVolumeSlider != null) musicVolumeSlider.value = musicVol;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVol;
            if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;
            if (qualityDropdown != null) qualityDropdown.value = quality;

            ApplySettings(musicVol, sfxVol, fullscreen, quality);
        }

        private void SaveSettings()
        {
            float musicVol = musicVolumeSlider != null ? musicVolumeSlider.value : defaultMusicVolume;
            float sfxVol = sfxVolumeSlider != null ? sfxVolumeSlider.value : defaultSfxVolume;
            bool fullscreen = fullscreenToggle != null ? fullscreenToggle.isOn : Screen.fullScreen;
            int quality = qualityDropdown != null ? qualityDropdown.value : QualitySettings.GetQualityLevel();

            PlayerPrefs.SetFloat("MusicVolume", musicVol);
            PlayerPrefs.SetFloat("SfxVolume", sfxVol);
            PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
            PlayerPrefs.SetInt("QualityLevel", quality);
            PlayerPrefs.Save();
        }

        private void ApplySettings(float musicVol, float sfxVol, bool fullscreen, int quality)
        {
            // Audio-Einstellungen (wenn AudioManager vorhanden)
            CelestialMerge.Audio.CelestialAudioManager audioManager = 
                CelestialMerge.Audio.CelestialAudioManager.Instance;
            
            if (audioManager != null)
            {
                audioManager.SetMusicVolume(musicVol);
                audioManager.SetSFXVolume(sfxVol);
            }

            // Grafik-Einstellungen
            Screen.fullScreen = fullscreen;
            QualitySettings.SetQualityLevel(quality);
        }

        private void OnBackClicked()
        {
            SaveSettings();
            if (menuManager != null)
            {
                menuManager.OnSettingsBackButtonClicked();
            }
            else if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        private void OnMusicVolumeChanged(float value)
        {
            // AudioManager.Instance?.SetMusicVolume(value);
            PlayerPrefs.SetFloat("MusicVolume", value);
        }

        private void OnSfxVolumeChanged(float value)
        {
            // AudioManager.Instance?.SetSfxVolume(value);
            PlayerPrefs.SetFloat("SfxVolume", value);
        }

        private void OnFullscreenToggled(bool value)
        {
            Screen.fullScreen = value;
            PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
        }

        private void OnQualityChanged(int value)
        {
            QualitySettings.SetQualityLevel(value);
            PlayerPrefs.SetInt("QualityLevel", value);
        }
    }
}
