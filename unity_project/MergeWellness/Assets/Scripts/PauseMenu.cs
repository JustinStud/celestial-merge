using UnityEngine;
using UnityEngine.UI;

namespace MergeWellness
{
    /// <summary>
    /// Pause-Menü-Controller für Gameplay-Szene
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button restartButton;

        private MenuManager menuManager;
        private bool isPaused = false;

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
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }

        private void Update()
        {
            // ESC-Taste für Pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        private void SetupButtons()
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveAllListeners();
                resumeButton.onClick.AddListener(OnResumeClicked);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveAllListeners();
                settingsButton.onClick.AddListener(OnSettingsClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveAllListeners();
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(OnRestartClicked);
            }
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
            
            if (pausePanel != null)
            {
                pausePanel.SetActive(isPaused);
            }

            Time.timeScale = isPaused ? 0f : 1f;

            if (menuManager != null)
            {
                if (isPaused)
                {
                    menuManager.ShowPauseMenu();
                }
                else
                {
                    menuManager.HidePauseMenu();
                }
            }
        }

        private void OnResumeClicked()
        {
            TogglePause();
        }

        private void OnSettingsClicked()
        {
            if (menuManager != null)
            {
                menuManager.OnPauseSettingsButtonClicked();
            }
        }

        private void OnMainMenuClicked()
        {
            if (menuManager != null)
            {
                menuManager.OnPauseMainMenuButtonClicked();
            }
        }

        private void OnRestartClicked()
        {
            if (menuManager != null)
            {
                menuManager.RestartGame();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }
        }
    }
}
