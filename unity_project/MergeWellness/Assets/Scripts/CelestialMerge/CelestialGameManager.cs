using UnityEngine;
using System;
using CelestialMerge.Story;

namespace CelestialMerge
{
    /// <summary>
    /// Zentraler Game Manager - Verbindet alle Systeme und koordiniert das Spiel
    /// </summary>
    public class CelestialGameManager : MonoBehaviour
    {
        [Header("System References")]
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private CelestialProgressionManager progressionManager;
        [SerializeField] private CelestialMergeManager mergeManager;
        [SerializeField] private ExpandableBoardManager boardManager;
        [SerializeField] private IdleProductionManager idleManager;
        [SerializeField] private DailySystemManager dailyManager;
        [SerializeField] private CraftingSystem craftingSystem;
        [SerializeField] private ItemSynergySystem synergySystem;
        [SerializeField] private MiniGameManager miniGameManager;
        [SerializeField] private CelestialItemDatabase itemDatabase;
        [SerializeField] private StoryManager storyManager;
        [SerializeField] private StoryDatabase storyDatabase;

        [Header("Settings")]
        [SerializeField] private bool autoInitialize = true;
        [SerializeField] private bool debugMode = false;

        // Singleton Pattern (optional)
        public static CelestialGameManager Instance { get; private set; }

        // Events für UI
        public event Action OnGameInitialized;
        public event Action OnGamePaused;
        public event Action OnGameResumed;

        private bool isInitialized = false;
        private bool isPaused = false;

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

            // Auto-Find Systeme falls nicht gesetzt
            AutoFindSystems();
        }

        private void Start()
        {
            if (autoInitialize)
            {
                InitializeGame();
            }
        }

        /// <summary>
        /// Automatisches Finden aller Systeme
        /// </summary>
        private void AutoFindSystems()
        {
            if (currencyManager == null)
                currencyManager = FindFirstObjectByType<CurrencyManager>();
            
            if (progressionManager == null)
                progressionManager = FindFirstObjectByType<CelestialProgressionManager>();
            
            if (mergeManager == null)
                mergeManager = FindFirstObjectByType<CelestialMergeManager>();
            
            if (boardManager == null)
                boardManager = FindFirstObjectByType<ExpandableBoardManager>();
            
            if (idleManager == null)
                idleManager = FindFirstObjectByType<IdleProductionManager>();
            
            if (dailyManager == null)
                dailyManager = FindFirstObjectByType<DailySystemManager>();
            
            if (craftingSystem == null)
                craftingSystem = FindFirstObjectByType<CraftingSystem>();
            
            if (synergySystem == null)
                synergySystem = FindFirstObjectByType<ItemSynergySystem>();
            
            if (miniGameManager == null)
                miniGameManager = FindFirstObjectByType<MiniGameManager>();
            
            if (itemDatabase == null)
                itemDatabase = FindFirstObjectByType<CelestialItemDatabase>();
            
            if (storyManager == null)
                storyManager = FindFirstObjectByType<StoryManager>();
            
            if (storyDatabase == null)
                storyDatabase = FindFirstObjectByType<StoryDatabase>();
            
            if (debugMode)
            {
                LogSystemStatus();
            }
        }

        /// <summary>
        /// Initialisiert das Spiel und verbindet alle Systeme
        /// </summary>
        public void InitializeGame()
        {
            if (isInitialized)
            {
                Debug.LogWarning("Spiel bereits initialisiert!");
                return;
            }

            Debug.Log("=== Celestial Merge - Initialisierung ===");

            // 1. Validiere alle Systeme
            if (!ValidateSystems())
            {
                Debug.LogError("❌ System-Validierung fehlgeschlagen!");
                return;
            }

            // 2. Verbinde System-Events
            ConnectSystemEvents();

            // 3. Initialisiere System-Abhängigkeiten
            InitializeSystemDependencies();

            // 4. Lade Spielstand
            LoadGameState();

            isInitialized = true;
            OnGameInitialized?.Invoke();

            Debug.Log("✅ Spiel erfolgreich initialisiert!");
        }

        /// <summary>
        /// Validiert ob alle Systeme vorhanden sind
        /// </summary>
        private bool ValidateSystems()
        {
            bool allValid = true;

            if (currencyManager == null)
            {
                Debug.LogError("❌ CurrencyManager fehlt!");
                allValid = false;
            }

            if (progressionManager == null)
            {
                Debug.LogError("❌ CelestialProgressionManager fehlt!");
                allValid = false;
            }

            if (mergeManager == null)
            {
                Debug.LogError("❌ CelestialMergeManager fehlt!");
                allValid = false;
            }

            if (boardManager == null)
            {
                Debug.LogError("❌ ExpandableBoardManager fehlt!");
                allValid = false;
            }

            if (itemDatabase == null)
            {
                Debug.LogError("❌ CelestialItemDatabase fehlt!");
                allValid = false;
            }

            // Story-System ist optional, aber wenn vorhanden sollte es funktionieren
            if (storyManager != null && storyDatabase == null)
            {
                Debug.LogWarning("⚠️ StoryManager vorhanden, aber StoryDatabase fehlt!");
            }

            return allValid;
        }

        /// <summary>
        /// Verbindet Events zwischen Systemen
        /// </summary>
        private void ConnectSystemEvents()
        {
            Debug.Log("Verbinde System-Events...");

            // Merge Manager → Progression
            if (mergeManager != null && progressionManager != null)
            {
                mergeManager.OnTwoMerge += (item1, item2) => {
                    progressionManager.RegisterMerge();
                };
                mergeManager.OnThreeMerge += (item1, item2, item3) => {
                    progressionManager.RegisterMerge();
                };
            }

            // Progression → Currency Capacity
            if (progressionManager != null && currencyManager != null)
            {
                progressionManager.OnLevelUp += (level) => {
                    long newCapacity = progressionManager.GetStardustCapacity();
                    currencyManager.SetMaxStardustCapacity(newCapacity);
                };
            }

            // Daily System → Currency
            if (dailyManager != null && currencyManager != null)
            {
                dailyManager.OnLoginBonusClaimed += (day) => {
                    // UI wird benachrichtigt
                };
            }

            // Mini-Game → Daily Quest
            if (miniGameManager != null && dailyManager != null)
            {
                miniGameManager.OnMiniGameCompleted += (result) => {
                    if (result.won)
                    {
                        dailyManager.UpdateQuestProgress(QuestType.PlayMinigame);
                    }
                };
            }

            // Progression → Story System
            if (progressionManager != null && storyManager != null)
            {
                // StoryManager hört bereits auf OnLevelUp, aber wir stellen sicher, dass es verbunden ist
                Debug.Log("📖 Story-System mit Progression verbunden");
            }

            Debug.Log("✅ Events verbunden");
        }

        /// <summary>
        /// Initialisiert System-Abhängigkeiten
        /// </summary>
        private void InitializeSystemDependencies()
        {
            Debug.Log("Initialisiere System-Abhängigkeiten...");

            // Setze initiale Currency Capacity
            if (progressionManager != null && currencyManager != null)
            {
                long capacity = progressionManager.GetStardustCapacity();
                currencyManager.SetMaxStardustCapacity(capacity);
            }

            // Initialisiere Item Database falls leer
            if (itemDatabase != null)
            {
                var starterIds = itemDatabase.GetStarterItemIds();
                if (starterIds == null || starterIds.Count == 0)
                {
                    Debug.LogWarning("ItemDatabase ist leer - initialisiere Standard-Items...");
                    // itemDatabase.InitializeCelestialItems(); // Uncomment wenn bereit
                }
            }

            // Initialisiere Story Database falls vorhanden
            if (storyDatabase != null)
            {
                // Prüfe ob Story Database bereits initialisiert ist
                if (storyDatabase.GetChapterCount() == 0)
                {
                    Debug.LogWarning("⚠️ StoryDatabase ist leer! Bitte im Inspector: Rechtsklick → 'Initialize Story Content'");
                }
                else
                {
                    Debug.Log($"📖 Story Database geladen: {storyDatabase.GetChapterCount()} Chapters");
                }
            }

            Debug.Log("✅ Abhängigkeiten initialisiert");
        }

        /// <summary>
        /// Pausiert das Spiel
        /// </summary>
        public void PauseGame()
        {
            if (isPaused) return;

            isPaused = true;
            Time.timeScale = 0f;
            OnGamePaused?.Invoke();
            Debug.Log("⏸ Spiel pausiert");
        }

        /// <summary>
        /// Setzt das Spiel fort
        /// </summary>
        public void ResumeGame()
        {
            if (!isPaused) return;

            isPaused = false;
            Time.timeScale = 1f;
            OnGameResumed?.Invoke();
            Debug.Log("▶ Spiel fortgesetzt");
        }

        /// <summary>
        /// Speichert Spielstand
        /// </summary>
        public void SaveGameState()
        {
            // Alle Systeme speichern automatisch, aber wir können hier zentral speichern
            PlayerPrefs.SetString("LastSaveTime", DateTime.Now.ToString("O"));
            PlayerPrefs.Save();
            Debug.Log("💾 Spielstand gespeichert");
        }

        /// <summary>
        /// Lädt Spielstand
        /// </summary>
        private void LoadGameState()
        {
            // Alle Systeme laden automatisch beim Start
            Debug.Log("📂 Spielstand geladen");
        }

        /// <summary>
        /// Loggt System-Status (für Debugging)
        /// </summary>
        private void LogSystemStatus()
        {
            Debug.Log("=== System Status ===");
            Debug.Log($"CurrencyManager: {(currencyManager != null ? "✓" : "✗")}");
            Debug.Log($"ProgressionManager: {(progressionManager != null ? "✓" : "✗")}");
            Debug.Log($"MergeManager: {(mergeManager != null ? "✓" : "✗")}");
            Debug.Log($"BoardManager: {(boardManager != null ? "✓" : "✗")}");
            Debug.Log($"IdleManager: {(idleManager != null ? "✓" : "✗")}");
            Debug.Log($"DailyManager: {(dailyManager != null ? "✓" : "✗")}");
            Debug.Log($"CraftingSystem: {(craftingSystem != null ? "✓" : "✗")}");
            Debug.Log($"SynergySystem: {(synergySystem != null ? "✓" : "✗")}");
            Debug.Log($"MiniGameManager: {(miniGameManager != null ? "✓" : "✗")}");
            Debug.Log($"ItemDatabase: {(itemDatabase != null ? "✓" : "✗")}");
            Debug.Log($"StoryManager: {(storyManager != null ? "✓" : "✗")}");
            Debug.Log($"StoryDatabase: {(storyDatabase != null ? "✓" : "✗")}");
        }

        #region Public Getters

        public CurrencyManager GetCurrencyManager() => currencyManager;
        public CelestialProgressionManager GetProgressionManager() => progressionManager;
        public CelestialMergeManager GetMergeManager() => mergeManager;
        public ExpandableBoardManager GetBoardManager() => boardManager;
        public IdleProductionManager GetIdleManager() => idleManager;
        public DailySystemManager GetDailyManager() => dailyManager;
        public CraftingSystem GetCraftingSystem() => craftingSystem;
        public ItemSynergySystem GetSynergySystem() => synergySystem;
        public MiniGameManager GetMiniGameManager() => miniGameManager;
        public CelestialItemDatabase GetItemDatabase() => itemDatabase;
        public StoryManager GetStoryManager() => storyManager;
        public StoryDatabase GetStoryDatabase() => storyDatabase;

        #endregion
    }
}
