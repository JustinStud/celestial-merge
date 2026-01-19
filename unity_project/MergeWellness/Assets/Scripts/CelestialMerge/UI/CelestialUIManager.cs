using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace CelestialMerge.UI
{
    /// <summary>
    /// Zentrale UI-Verwaltung für alle Celestial Merge UI-Elemente
    /// </summary>
    public class CelestialUIManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CelestialGameManager gameManager;

        [Header("Currency UI")]
        [SerializeField] private TextMeshProUGUI stardustText;
        [SerializeField] private TextMeshProUGUI crystalsText;
        [SerializeField] private Image stardustIcon;
        [SerializeField] private Image crystalsIcon;

        [Header("Progression UI")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI chapterText;
        [SerializeField] private Slider xpProgressBar;
        [SerializeField] private TextMeshProUGUI xpText;

        [Header("Merge UI")]
        [SerializeField] private GameObject mergeResultPanel;
        [SerializeField] private TextMeshProUGUI mergeResultText;
        [SerializeField] private Image mergeResultItemImage;
        [SerializeField] private TextMeshProUGUI mergeRewardText;

        [Header("Daily UI")]
        [SerializeField] private GameObject dailyLoginPanel;
        [SerializeField] private Button dailyLoginButton;
        [SerializeField] private TextMeshProUGUI dailyLoginDayText;
        [SerializeField] private GameObject dailyQuestPanel;
        [SerializeField] private Transform dailyQuestContainer;

        [Header("Mini-Game UI")]
        [SerializeField] private GameObject miniGamePanel;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] public Button playMiniGameButton; // Button in Haupt-UI zum Öffnen (public für Auto-Setup)
        [SerializeField] private MiniGameUIPanel miniGameUIPanel; // MiniGameUIPanel Script-Referenz

        [Header("Board UI")]
        [SerializeField] private TextMeshProUGUI boardSizeText;
        [SerializeField] private TextMeshProUGUI freeSlotsText;

        [Header("Idle UI")]
        [SerializeField] private TextMeshProUGUI idleProductionText;
        [SerializeField] private GameObject offlineRewardPanel;
        [SerializeField] private TextMeshProUGUI offlineRewardText;

        [Header("Synergy UI")]
        [SerializeField] private GameObject synergyPanel;
        [SerializeField] private Transform synergyContainer;
        [SerializeField] private GameObject synergyPrefab;

        private CurrencyManager currencyManager;
        private CelestialProgressionManager progressionManager;
        private DailySystemManager dailyManager;
        private MiniGameManager miniGameManager;
        private ExpandableBoardManager boardManager;
        private IdleProductionManager idleManager;
        private ItemSynergySystem synergySystem;

        private void Awake()
        {
            // Finde GameManager
            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<CelestialGameManager>();
            }
        }

        private void Start()
        {
            // Stelle sicher, dass alle Panels initial deaktiviert sind
            DeactivateAllPanelsOnStart();

            InitializeUI();
            SubscribeToEvents();
            
            // Force initial UI update nach kurzer Verzögerung (falls Systeme noch nicht bereit sind)
            Invoke(nameof(ForceUIUpdate), 0.1f);
        }

        /// <summary>
        /// Deaktiviert alle Modal-Panels beim Start
        /// </summary>
        private void DeactivateAllPanelsOnStart()
        {
            if (dailyLoginPanel != null) dailyLoginPanel.SetActive(false);
            if (dailyQuestPanel != null) dailyQuestPanel.SetActive(false);
            if (miniGamePanel != null) miniGamePanel.SetActive(false);
            if (offlineRewardPanel != null) offlineRewardPanel.SetActive(false);
            if (mergeResultPanel != null) mergeResultPanel.SetActive(false);
            if (synergyPanel != null) synergyPanel.SetActive(false);
        }
        
        /// <summary>
        /// Erzwingt UI-Update (für Initialisierung)
        /// </summary>
        private void ForceUIUpdate()
        {
            UpdateCurrencyUI();
            UpdateProgressionUI();
            
            // Debug Log
            if (progressionManager != null)
            {
                Debug.Log($"🔍 UI Update: Level={progressionManager.PlayerLevel}, XP={progressionManager.CurrentXP}/{progressionManager.XPToNextLevel}");
            }
            if (currencyManager != null)
            {
                Debug.Log($"🔍 UI Update: Stardust={currencyManager.Stardust}, Crystals={currencyManager.Crystals}");
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        /// <summary>
        /// Initialisiert UI und holt System-Referenzen
        /// </summary>
        private void InitializeUI()
        {
            if (gameManager == null)
            {
                Debug.LogError("CelestialGameManager nicht gefunden!");
                return;
            }

            // Hole System-Referenzen
            currencyManager = gameManager.GetCurrencyManager();
            progressionManager = gameManager.GetProgressionManager();
            dailyManager = gameManager.GetDailyManager();
            miniGameManager = gameManager.GetMiniGameManager();
            boardManager = gameManager.GetBoardManager();
            idleManager = gameManager.GetIdleManager();
            synergySystem = gameManager.GetSynergySystem();

            // Auto-Find MiniGameUIPanel falls nicht zugewiesen
            if (miniGameUIPanel == null)
            {
                miniGameUIPanel = FindFirstObjectByType<MiniGameUIPanel>();
            }

            // Initialisiere UI-Werte
            UpdateCurrencyUI();
            UpdateProgressionUI();
            UpdateDailyUI();
            UpdateMiniGameUI();
            UpdateBoardUI();
            UpdateIdleUI();

            // Setup Buttons
            if (dailyLoginButton != null)
            {
                dailyLoginButton.onClick.RemoveAllListeners();
                dailyLoginButton.onClick.AddListener(OnDailyLoginClicked);
            }

            if (playMiniGameButton != null)
            {
                playMiniGameButton.onClick.RemoveAllListeners();
                playMiniGameButton.onClick.AddListener(OnPlayMiniGameClicked);
                
                // Stelle sicher, dass Button funktioniert
                playMiniGameButton.interactable = true;
                Image buttonImage = playMiniGameButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.raycastTarget = true;
                }
                playMiniGameButton.transform.SetAsLastSibling();
            }
        }

        /// <summary>
        /// Abonniert Events von allen Systemen
        /// </summary>
        private void SubscribeToEvents()
        {
            // Currency Events
            if (currencyManager != null)
            {
                currencyManager.OnStardustChanged += UpdateStardustUI;
                currencyManager.OnCrystalsChanged += UpdateCrystalsUI;
            }

            // Progression Events
            if (progressionManager != null)
            {
                progressionManager.OnLevelUp += OnLevelUp;
                progressionManager.OnXPChanged += OnXPChanged;
                progressionManager.OnChapterUnlocked += OnChapterUnlocked;
                progressionManager.OnMilestoneReached += OnMilestoneReached;
            }

            // Daily Events
            if (dailyManager != null)
            {
                dailyManager.OnLoginBonusClaimed += OnDailyLoginClaimed;
                dailyManager.OnQuestCompleted += OnQuestCompleted;
                dailyManager.OnAllQuestsCompleted += OnAllQuestsCompleted;
            }

            // Mini-Game Events
            if (miniGameManager != null)
            {
                miniGameManager.OnEnergyChanged += UpdateEnergyUI;
                miniGameManager.OnMiniGameCompleted += OnMiniGameCompleted;
            }

            // Merge Events
            if (gameManager != null && gameManager.GetMergeManager() != null)
            {
                var mergeManager = gameManager.GetMergeManager();
                mergeManager.OnTwoMerge += OnMergeCompleted;
                mergeManager.OnThreeMerge += OnThreeMergeCompleted;
            }

            // Synergy Events
            if (synergySystem != null)
            {
                synergySystem.OnSynergyActivated += OnSynergyActivated;
                synergySystem.OnSynergyDeactivated += OnSynergyDeactivated;
            }
        }

        /// <summary>
        /// Entabonniert Events
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (currencyManager != null)
            {
                currencyManager.OnStardustChanged -= UpdateStardustUI;
                currencyManager.OnCrystalsChanged -= UpdateCrystalsUI;
            }

            if (progressionManager != null)
            {
                progressionManager.OnLevelUp -= OnLevelUp;
                progressionManager.OnXPChanged -= OnXPChanged;
                progressionManager.OnChapterUnlocked -= OnChapterUnlocked;
                progressionManager.OnMilestoneReached -= OnMilestoneReached;
            }

            if (dailyManager != null)
            {
                dailyManager.OnLoginBonusClaimed -= OnDailyLoginClaimed;
                dailyManager.OnQuestCompleted -= OnQuestCompleted;
                dailyManager.OnAllQuestsCompleted -= OnAllQuestsCompleted;
            }

            if (miniGameManager != null)
            {
                miniGameManager.OnEnergyChanged -= UpdateEnergyUI;
                miniGameManager.OnMiniGameCompleted -= OnMiniGameCompleted;
            }
        }

        #region Currency UI Updates

        private void UpdateCurrencyUI()
        {
            UpdateStardustUI(currencyManager != null ? currencyManager.Stardust : 0);
            UpdateCrystalsUI(currencyManager != null ? currencyManager.Crystals : 0);
        }

        private void UpdateStardustUI(long stardust)
        {
            if (stardustText != null)
            {
                stardustText.text = FormatLargeNumber(stardust);
                Debug.Log($"💰 Stardust UI aktualisiert: {stardust} → {FormatLargeNumber(stardust)}");
            }
            else
            {
                Debug.LogWarning("⚠️ StardustText ist nicht zugewiesen!");
            }
        }

        private void UpdateCrystalsUI(int crystals)
        {
            if (crystalsText != null)
            {
                crystalsText.text = crystals.ToString();
            }
        }

        #endregion

        #region Progression UI Updates

        private void UpdateProgressionUI()
        {
            if (progressionManager == null)
            {
                Debug.LogWarning("⚠️ UpdateProgressionUI: progressionManager ist null!");
                return;
            }

            // Level
            if (levelText != null)
            {
                levelText.text = $"Level {progressionManager.PlayerLevel}";
                Debug.Log($"📊 Level Text aktualisiert: {progressionManager.PlayerLevel}");
            }
            else
            {
                Debug.LogWarning("⚠️ LevelText ist nicht zugewiesen!");
            }

            // Chapter
            if (chapterText != null)
            {
                chapterText.text = $"Chapter {progressionManager.CurrentChapter}";
            }

            // XP Progress
            if (xpProgressBar != null)
            {
                float progress = 0f;
                if (progressionManager.XPToNextLevel > 0)
                {
                    progress = Mathf.Clamp01((float)progressionManager.CurrentXP / (float)progressionManager.XPToNextLevel);
                }
                xpProgressBar.value = progress;
                Debug.Log($"📊 XP Progress Bar aktualisiert: {progress:F2} ({progressionManager.CurrentXP}/{progressionManager.XPToNextLevel})");
            }
            else
            {
                Debug.LogWarning("⚠️ XPProgressBar ist nicht zugewiesen!");
            }

            if (xpText != null)
            {
                xpText.text = $"{FormatLargeNumber(progressionManager.CurrentXP)} / {FormatLargeNumber(progressionManager.XPToNextLevel)} XP";
            }
        }

        private void OnLevelUp(int newLevel)
        {
            UpdateProgressionUI();
            ShowLevelUpNotification(newLevel);
        }

        private void OnXPChanged(long newXP)
        {
            // Aktualisiere UI bei jeder XP-Änderung (für Progress Bar)
            UpdateProgressionUI();
        }

        private void OnChapterUnlocked(int chapter)
        {
            UpdateProgressionUI(); // Chapter Text aktualisieren
            ShowChapterUnlockedNotification(chapter);
        }

        private void OnMilestoneReached(int milestone)
        {
            ShowMilestoneNotification(milestone);
        }

        #endregion

        #region Daily UI Updates

        private void UpdateDailyUI()
        {
            if (dailyManager == null) return;

            // Update Daily Login Button
            if (dailyLoginButton != null)
            {
                dailyLoginButton.interactable = !dailyManager.IsTodayClaimed();
            }

            if (dailyLoginDayText != null)
            {
                dailyLoginDayText.text = $"Day {dailyManager.GetCurrentLoginDay()}";
            }

            // Update Daily Quests
            UpdateDailyQuestsUI();
        }

        private void UpdateDailyQuestsUI()
        {
            if (dailyQuestContainer == null || dailyManager == null) return;

            // Lösche alte Quest-UI
            foreach (Transform child in dailyQuestContainer)
            {
                Destroy(child.gameObject);
            }

            // Erstelle Quest-UI für jede aktive Quest
            var quests = dailyManager.GetActiveQuests();
            foreach (var quest in quests)
            {
                CreateQuestUI(quest);
            }
        }

        private void CreateQuestUI(DailyQuest quest)
        {
            // Erstelle Quest-UI Element (vereinfacht)
            GameObject questObj = new GameObject($"Quest_{quest.questId}");
            questObj.transform.SetParent(dailyQuestContainer, false);

            // Füge UI-Komponenten hinzu
            // (Hier würde man normalerweise ein Prefab instantiieren)
        }

        private void OnDailyLoginClicked()
        {
            if (dailyManager != null)
            {
                dailyManager.ClaimDailyLoginBonus();
            }
        }

        private void OnDailyLoginClaimed(int day)
        {
            UpdateDailyUI();
            ShowNotification($"Daily Login Bonus Tag {day} erhalten!");
        }

        private void OnQuestCompleted(DailyQuest quest)
        {
            UpdateDailyQuestsUI();
            ShowNotification($"Quest abgeschlossen: {quest.questName}");
        }

        private void OnAllQuestsCompleted()
        {
            ShowNotification("🎉 Alle Daily Quests abgeschlossen! Bonus erhalten!");
        }

        #endregion

        #region Mini-Game UI Updates

        private void UpdateMiniGameUI()
        {
            UpdateEnergyUI(miniGameManager != null ? miniGameManager.Energy : 0);
        }

        private void UpdateEnergyUI(int energy)
        {
            if (energyText != null)
            {
                energyText.text = $"Energy: {energy}/{miniGameManager.MaxEnergy}";
            }

            if (playMiniGameButton != null)
            {
                playMiniGameButton.interactable = energy > 0;
            }
        }

        private void OnPlayMiniGameClicked()
        {
            // Versuche MiniGameUIPanel zu verwenden (besser)
            if (miniGameUIPanel != null)
            {
                miniGameUIPanel.Show();
                return;
            }

            // Fallback: Direkt über PanelManager
            if (CelestialUIPanelManager.Instance != null)
            {
                CelestialUIPanelManager.Instance.ShowMiniGame();
                return;
            }

            // Fallback: Direkt SetActive
            if (miniGamePanel != null)
            {
                miniGamePanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("⚠️ Mini-Game Panel nicht gefunden! Bitte MiniGameUIPanel oder miniGamePanel zuweisen.");
            }
        }

        private void OnMiniGameCompleted(MiniGameResult result)
        {
            UpdateEnergyUI(miniGameManager.Energy);
            ShowNotification($"Mini-Game: {result.crystalReward} Crystals erhalten!");
        }

        #endregion

        #region Merge UI Updates

        private void OnMergeCompleted(CelestialItem item1, CelestialItem item2)
        {
            // Aktualisiere Progression UI nach Merge (XP wurde vergeben)
            UpdateProgressionUI();
            ShowMergeResult(item1, item2, null, false);
        }

        private void OnThreeMergeCompleted(CelestialItem item1, CelestialItem item2, CelestialItem item3)
        {
            // Aktualisiere Progression UI nach Merge (XP wurde vergeben)
            UpdateProgressionUI();
            ShowMergeResult(item1, item2, item3, true);
        }

        private void ShowMergeResult(CelestialItem item1, CelestialItem item2, CelestialItem item3, bool isThreeMerge)
        {
            if (mergeResultPanel != null)
            {
                mergeResultPanel.SetActive(true);

                if (mergeResultText != null)
                {
                    string mergeType = isThreeMerge ? "3× Merge" : "2× Merge";
                    mergeResultText.text = $"{mergeType} erfolgreich!";
                }
            }
        }

        #endregion

        #region Board UI Updates

        private void UpdateBoardUI()
        {
            if (boardManager == null) return;

            if (boardSizeText != null)
            {
                boardSizeText.text = $"Board: {boardManager.CurrentWidth}×{boardManager.CurrentHeight}";
            }

            // Berechne freie Slots
            int freeSlots = 0;
            // (Hier würde man durch alle Slots iterieren und leere zählen)

            if (freeSlotsText != null)
            {
                freeSlotsText.text = $"Free Slots: {freeSlots}";
            }
        }

        #endregion

        #region Idle UI Updates

        private void UpdateIdleUI()
        {
            if (idleManager == null) return;

            if (idleProductionText != null)
            {
                idleProductionText.text = $"Production: {idleManager.CurrentProductionRate:F1} Stardust/Min";
            }
        }

        #endregion

        #region Synergy UI Updates

        private void OnSynergyActivated(SynergyDefinition synergy)
        {
            UpdateSynergyUI();
            ShowNotification($"✨ Synergy aktiviert: {synergy.synergyName}");
        }

        private void OnSynergyDeactivated(SynergyDefinition synergy)
        {
            UpdateSynergyUI();
        }

        private void UpdateSynergyUI()
        {
            if (synergyContainer == null || synergySystem == null) return;

            // Lösche alte Synergy-UI
            foreach (Transform child in synergyContainer)
            {
                Destroy(child.gameObject);
            }

            // Erstelle UI für aktive Synergies
            var activeSynergies = synergySystem.GetActiveSynergies();
            foreach (var synergy in activeSynergies)
            {
                CreateSynergyUI(synergy);
            }
        }

        private void CreateSynergyUI(SynergyDefinition synergy)
        {
            // Erstelle Synergy-UI Element
            if (synergyPrefab != null)
            {
                GameObject synergyObj = Instantiate(synergyPrefab, synergyContainer);
                // Setze Synergy-Daten
            }
        }

        #endregion

        #region Helper Methods

        private string FormatLargeNumber(long number)
        {
            if (number >= 1000000)
                return $"{number / 1000000f:F1}M";
            if (number >= 1000)
                return $"{number / 1000f:F1}K";
            return number.ToString();
        }

        private void ShowNotification(string message)
        {
            Debug.Log($"📢 {message}");
            // Hier könnte man ein Toast-Notification-System einbauen
        }

        private void ShowLevelUpNotification(int level)
        {
            ShowNotification($"🎉 Level Up! Jetzt Level {level}");
        }

        private void ShowChapterUnlockedNotification(int chapter)
        {
            ShowNotification($"📖 Chapter {chapter} freigeschaltet!");
        }

        private void ShowMilestoneNotification(int milestone)
        {
            ShowNotification($"🏆 Milestone erreicht: {milestone} Merges!");
        }

        #endregion
    }
}
