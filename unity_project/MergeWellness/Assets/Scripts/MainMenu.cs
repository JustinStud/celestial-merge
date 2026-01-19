using UnityEngine;
using UnityEngine.UI;

namespace MergeWellness
{
    /// <summary>
    /// Hauptmenü-Controller für Start-Szene
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Text titleText;
        [SerializeField] private Text versionText;

        [Header("Settings")]
        [SerializeField] private string gameVersion = "1.0.0";

        private MenuManager menuManager;

        private void Awake()
        {
            menuManager = FindFirstObjectByType<MenuManager>();
            if (menuManager == null)
            {
                GameObject menuManagerObj = new GameObject("MenuManager");
                menuManager = menuManagerObj.AddComponent<MenuManager>();
            }
        }

        private void Start()
        {
            SetupButtons();
            SetupUI();
        }

        private void SetupButtons()
        {
            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(OnPlayClicked);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveAllListeners();
                settingsButton.onClick.AddListener(OnSettingsClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(OnQuitClicked);
            }
        }

        private void SetupUI()
        {
            if (versionText != null)
            {
                versionText.text = $"Version {gameVersion}";
            }

            if (titleText != null)
            {
                titleText.text = "Merge Wellness";
            }
        }

        private void OnPlayClicked()
        {
            Debug.Log("Play Button geklickt");
            if (menuManager != null)
            {
                menuManager.OnPlayButtonClicked();
            }
            else
            {
                // Fallback: Direkt Szene laden
                UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
            }
        }

        private void OnSettingsClicked()
        {
            Debug.Log("Settings Button geklickt");
            if (menuManager != null)
            {
                menuManager.OnSettingsButtonClicked();
            }
        }

        private void OnQuitClicked()
        {
            Debug.Log("Quit Button geklickt");
            if (menuManager != null)
            {
                menuManager.OnQuitButtonClicked();
            }
            else
            {
                Application.Quit();
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }
        }
    }
}
