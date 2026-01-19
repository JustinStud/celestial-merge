using UnityEngine;
using UnityEngine.SceneManagement;

namespace MergeWellness
{
    /// <summary>
    /// Verwaltet Menü-Navigation und Scene-Management
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Settings")]
        #pragma warning disable 0414 // pauseOnStart wird für zukünftige Features reserviert
        [SerializeField] private bool pauseOnStart = false;
        #pragma warning restore 0414

        private bool isPaused = false;
        private string gameplaySceneName = "Gameplay";

        private void Awake()
        {
            // Stelle sicher, dass nur ein MenuManager existiert
            MenuManager[] managers = FindObjectsByType<MenuManager>(FindObjectsSortMode.None);
            if (managers.Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Initialisiere Menü basierend auf aktueller Szene
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == gameplaySceneName || currentScene.Contains("Gameplay"))
            {
                // Wir sind im Gameplay
                if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
                if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
                if (settingsPanel != null) settingsPanel.SetActive(false);
            }
            else
            {
                // Wir sind im Main Menu
                ShowMainMenu();
            }
        }

        private void Update()
        {
            // ESC-Taste für Pause-Menü
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                string currentScene = SceneManager.GetActiveScene().name;
                if (currentScene == gameplaySceneName || currentScene.Contains("Gameplay"))
                {
                    TogglePause();
                }
            }
        }

        #region Main Menu

        public void ShowMainMenu()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        public void OnPlayButtonClicked()
        {
            Debug.Log("Play Button geklickt - Lade Gameplay-Szene");
            LoadGameplayScene();
        }

        public void OnSettingsButtonClicked()
        {
            ShowSettings();
        }

        public void OnQuitButtonClicked()
        {
            Debug.Log("Quit Button geklickt");
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        #endregion

        #region Pause Menu

        public void TogglePause()
        {
            isPaused = !isPaused;
            
            if (isPaused)
            {
                ShowPauseMenu();
            }
            else
            {
                HidePauseMenu();
            }

            Time.timeScale = isPaused ? 0f : 1f;
        }

        public void ShowPauseMenu()
        {
            isPaused = true;
            Time.timeScale = 0f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        }

        public void HidePauseMenu()
        {
            isPaused = false;
            Time.timeScale = 1f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        public void OnResumeButtonClicked()
        {
            HidePauseMenu();
        }

        public void OnPauseSettingsButtonClicked()
        {
            ShowSettings();
        }

        public void OnPauseMainMenuButtonClicked()
        {
            Time.timeScale = 1f;
            LoadMainMenuScene();
        }

        #endregion

        #region Settings

        public void ShowSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        public void HideSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        public void OnSettingsBackButtonClicked()
        {
            HideSettings();
        }

        #endregion

        #region Scene Management

        public void LoadGameplayScene()
        {
            Time.timeScale = 1f;
            isPaused = false;
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void LoadMainMenuScene()
        {
            Time.timeScale = 1f;
            isPaused = false;
            SceneManager.LoadScene("MainMenu");
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            isPaused = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        #endregion

        #region Game Over

        public void ShowGameOver()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        public void HideGameOver()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            Time.timeScale = 1f;
        }

        public void OnGameOverRestartButtonClicked()
        {
            RestartGame();
        }

        public void OnGameOverMainMenuButtonClicked()
        {
            LoadMainMenuScene();
        }

        #endregion
    }
}
