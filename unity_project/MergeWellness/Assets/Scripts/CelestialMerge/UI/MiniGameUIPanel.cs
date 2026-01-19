using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CelestialMerge.UI
{
    /// <summary>
    /// UI Panel für Mini-Game System
    /// </summary>
    public class MiniGameUIPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MiniGameManager miniGameManager;

        [Header("Main Panel")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private Button closeButton;

        [Header("Energy Display")]
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private Slider energyBar;
        [SerializeField] private TextMeshProUGUI energyRegenText;

        [Header("Game Type Buttons")]
        [SerializeField] private Button easyGameButton;
        [SerializeField] private Button mediumGameButton;
        [SerializeField] private Button hardGameButton;
        [SerializeField] private TextMeshProUGUI gameTypeDescription;

        [Header("Result Panel")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Button closeResultButton;
        [SerializeField] private Button playAgainButton;

        private MiniGameType selectedGameType = MiniGameType.Easy;

        private void Awake()
        {
            if (miniGameManager == null)
            {
                miniGameManager = FindFirstObjectByType<MiniGameManager>();
            }
        }

        private void Start()
        {
            // Stelle sicher, dass Panel initial deaktiviert ist
            if (mainPanel != null)
            {
                mainPanel.SetActive(false);
            }
            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }

            SetupButtons();
            UpdateEnergyDisplay();
        }

        private void OnEnable()
        {
            if (miniGameManager != null)
            {
                miniGameManager.OnEnergyChanged += OnEnergyChanged;
                miniGameManager.OnMiniGameCompleted += OnMiniGameCompleted;
            }
            UpdateEnergyDisplay();
        }

        private void OnDisable()
        {
            if (miniGameManager != null)
            {
                miniGameManager.OnEnergyChanged -= OnEnergyChanged;
                miniGameManager.OnMiniGameCompleted -= OnMiniGameCompleted;
            }
        }

        private void Update()
        {
            // Update Energy Regen Time
            UpdateEnergyRegenTime();
        }

        private void SetupButtons()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseClicked);
            }

            if (easyGameButton != null)
            {
                easyGameButton.onClick.RemoveAllListeners();
                easyGameButton.onClick.AddListener(() => OnGameTypeSelected(MiniGameType.Easy));
            }

            if (mediumGameButton != null)
            {
                mediumGameButton.onClick.RemoveAllListeners();
                mediumGameButton.onClick.AddListener(() => OnGameTypeSelected(MiniGameType.Medium));
            }

            if (hardGameButton != null)
            {
                hardGameButton.onClick.RemoveAllListeners();
                hardGameButton.onClick.AddListener(() => OnGameTypeSelected(MiniGameType.Hard));
            }

            if (closeResultButton != null)
            {
                closeResultButton.onClick.RemoveAllListeners();
                closeResultButton.onClick.AddListener(OnCloseResultClicked);
            }

            if (playAgainButton != null)
            {
                playAgainButton.onClick.RemoveAllListeners();
                playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            }
        }

        /// <summary>
        /// Aktualisiert Energy Display
        /// </summary>
        private void UpdateEnergyDisplay()
        {
            if (miniGameManager == null) return;

            int energy = miniGameManager.Energy;
            int maxEnergy = miniGameManager.MaxEnergy;

            if (energyText != null)
            {
                energyText.text = $"{energy}/{maxEnergy} Energy";
            }

            if (energyBar != null)
            {
                energyBar.value = maxEnergy > 0 ? (float)energy / maxEnergy : 0f;
            }

            // Update Button States
            bool hasEnergy = energy > 0;
            if (easyGameButton != null) easyGameButton.interactable = hasEnergy;
            if (mediumGameButton != null) mediumGameButton.interactable = hasEnergy;
            if (hardGameButton != null) hardGameButton.interactable = hasEnergy;
        }

        /// <summary>
        /// Aktualisiert Energy Regen Time
        /// </summary>
        private void UpdateEnergyRegenTime()
        {
            if (miniGameManager == null || energyRegenText == null) return;

            // Energy regeniert alle 20 Minuten
            // Zeige Countdown bis nächste Energy
            if (miniGameManager.Energy < miniGameManager.MaxEnergy)
            {
                // TODO: Berechne verbleibende Zeit bis nächste Energy
                energyRegenText.text = "Energy regeneriert in: --:--"; // Placeholder
            }
            else
            {
                energyRegenText.text = "Max Energy erreicht!";
            }
        }

        /// <summary>
        /// Callback: Game Type selected
        /// </summary>
        private void OnGameTypeSelected(MiniGameType gameType)
        {
            selectedGameType = gameType;
            UpdateGameTypeDescription(gameType);

            // Starte Mini-Game
            if (miniGameManager != null && miniGameManager.Energy > 0)
            {
                bool started = miniGameManager.StartMiniGame(gameType);
                if (started)
                {
                    // TODO: Starte tatsächliches Mini-Game
                    // Für jetzt: Simuliere Game Completion nach 2 Sekunden
                    Invoke(nameof(SimulateGameCompletion), 2f);
                }
                else
                {
                    Debug.LogWarning("Mini-Game konnte nicht gestartet werden!");
                }
            }
        }

        /// <summary>
        /// Simuliert Game Completion (für Testing)
        /// </summary>
        private void SimulateGameCompletion()
        {
            if (miniGameManager != null)
            {
                // Simuliere Gewinn mit zufälligem Score
                int score = Random.Range(100, 1000);
                miniGameManager.CompleteMiniGame(won: true, score: score);
            }
        }

        /// <summary>
        /// Aktualisiert Game Type Description
        /// </summary>
        private void UpdateGameTypeDescription(MiniGameType gameType)
        {
            if (gameTypeDescription == null) return;

            string description = gameType switch
            {
                MiniGameType.Easy => "Einfach - 2-3 Minuten\nBelohnung: 5-10 Crystals",
                MiniGameType.Medium => "Mittel - 4-6 Minuten\nBelohnung: 15-30 Crystals",
                MiniGameType.Hard => "Schwer - 8-10 Minuten\nBelohnung: 50-100 Crystals",
                _ => "Wähle einen Schwierigkeitsgrad"
            };

            gameTypeDescription.text = description;
        }

        /// <summary>
        /// Callback: Energy changed
        /// </summary>
        private void OnEnergyChanged(int energy)
        {
            UpdateEnergyDisplay();
        }

        /// <summary>
        /// Callback: Mini-Game completed
        /// </summary>
        private void OnMiniGameCompleted(MiniGameResult result)
        {
            ShowResult(result);
            UpdateEnergyDisplay();
        }

        /// <summary>
        /// Zeigt Result Panel
        /// </summary>
        private void ShowResult(MiniGameResult result)
        {
            if (resultPanel == null) return;

            resultPanel.SetActive(true);

            if (resultText != null)
            {
                resultText.text = result.won ? "✅ Gewonnen!" : "❌ Verloren";
            }

            if (rewardText != null)
            {
                if (result.won)
                {
                    rewardText.text = $"Belohnung:\n{result.crystalReward} Crystals\n{result.stardustReward} Stardust\n+{result.xpReward} XP";
                }
                else
                {
                    rewardText.text = "Keine Belohnung";
                }
            }
        }

        /// <summary>
        /// Callback: Close clicked (verwendet PanelManager)
        /// </summary>
        private void OnCloseClicked()
        {
            Hide();
        }

        /// <summary>
        /// Callback: Close result clicked
        /// </summary>
        private void OnCloseResultClicked()
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Callback: Play again clicked
        /// </summary>
        private void OnPlayAgainClicked()
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }

            // Starte Mini-Game erneut
            OnGameTypeSelected(selectedGameType);
        }

        /// <summary>
        /// Zeigt Panel (verwendet PanelManager für korrektes Layering)
        /// </summary>
        public void Show()
        {
            if (mainPanel != null)
            {
                // Verwende PanelManager falls vorhanden
                if (CelestialUIPanelManager.Instance != null)
                {
                    CelestialUIPanelManager.Instance.ShowPanel(mainPanel, true);
                }
                else
                {
                    mainPanel.SetActive(true);
                }
                
                UpdateEnergyDisplay();
            }
        }

        /// <summary>
        /// Versteckt Panel (verwendet PanelManager)
        /// </summary>
        public void Hide()
        {
            if (mainPanel != null)
            {
                if (CelestialUIPanelManager.Instance != null)
                {
                    CelestialUIPanelManager.Instance.HidePanel(mainPanel);
                }
                else
                {
                    mainPanel.SetActive(false);
                }
            }

            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }
        }
    }
}
