using UnityEngine;
using System.Collections.Generic;
using CelestialMerge;

namespace CelestialMerge.Story
{
    /// <summary>
    /// Zentrale Story Management Komponente
    /// Verwaltet Chapters, Story Beats, Lore Entries
    /// Integriert mit CelestialProgressionManager für Level-basierte Trigger
    /// </summary>
    public class StoryManager : MonoBehaviour
    {
        public static StoryManager Instance { get; private set; }

        [Header("Story Database")]
        [SerializeField] private StoryDatabase storyDatabase;

        [Header("References")]
        [SerializeField] private CelestialProgressionManager progressionManager;
        [SerializeField] private StoryUIManager storyUI;

        private Dictionary<int, bool> completedBeats = new Dictionary<int, bool>();
        private int currentChapter = 1;
        private int playerLevel = 1;

        // Events
        public System.Action<StoryChapter> OnChapterUnlocked;
        public System.Action<StoryBeat> OnStoryBeatTriggered;
        public System.Action<LoreEntry> OnLoreUnlocked;

        #region Initialization

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Auto-Find References
            if (progressionManager == null)
            {
                progressionManager = FindFirstObjectByType<CelestialProgressionManager>();
            }

            if (storyUI == null)
            {
                storyUI = FindFirstObjectByType<StoryUIManager>();
            }

            // Auto-Find StoryDatabase falls nicht zugewiesen
            if (storyDatabase == null)
            {
                // Versuche StoryDatabase im Project zu finden
                storyDatabase = Resources.FindObjectsOfTypeAll<StoryDatabase>().Length > 0 
                    ? Resources.FindObjectsOfTypeAll<StoryDatabase>()[0] 
                    : null;
                
                if (storyDatabase == null)
                {
                    // Versuche über CelestialGameManager
                    CelestialGameManager gameManager = FindFirstObjectByType<CelestialGameManager>();
                    if (gameManager != null)
                    {
                        storyDatabase = gameManager.GetStoryDatabase();
                    }
                }
                
                if (storyDatabase == null)
                {
                    Debug.LogWarning("⚠️ StoryDatabase nicht gefunden! Bitte erstelle ein StoryDatabase Asset und weise es im Inspector zu.");
                }
            }

            // Load Story Database
            if (storyDatabase != null)
            {
                LoadStoryDatabase();
                
                // Prüfe ob Story Database initialisiert ist
                if (storyDatabase.GetChapterCount() == 0)
                {
                    Debug.LogWarning("⚠️ StoryDatabase ist leer! Bitte im Inspector: Rechtsklick → 'Initialize Story Content'");
                }
            }

            // Subscribe to Progression Events
            if (progressionManager != null)
            {
                progressionManager.OnLevelUp += OnPlayerLevelUp;
                playerLevel = progressionManager.PlayerLevel;
                currentChapter = progressionManager.CurrentChapter;
                
                // Prüfe initial Story Beats
                CheckStoryBeatTriggers();
                
                Debug.Log($"📖 StoryManager initialisiert: Level {playerLevel}, Chapter {currentChapter}");
            }
            else
            {
                Debug.LogWarning("⚠️ CelestialProgressionManager nicht gefunden! Story-System kann nicht funktionieren.");
            }

            // Load completed beats from PlayerPrefs
            LoadCompletedBeats();
        }

        private void OnDestroy()
        {
            if (progressionManager != null)
            {
                progressionManager.OnLevelUp -= OnPlayerLevelUp;
            }
        }

        #endregion

        #region Story Progression

        /// <summary>
        /// Wird aufgerufen wenn Spieler Level aufsteigt
        /// </summary>
        private void OnPlayerLevelUp(int newLevel)
        {
            playerLevel = newLevel;

            // Prüfe Chapter-Unlock
            CheckChapterUnlock();

            // Prüfe Story Beat Triggers
            CheckStoryBeatTriggers();
        }

        /// <summary>
        /// Prüft ob neues Chapter freigeschaltet werden kann
        /// </summary>
        private void CheckChapterUnlock()
        {
            if (storyDatabase == null) return;

            StoryChapter newChapter = storyDatabase.GetChapterForLevel(playerLevel);
            if (newChapter != null && newChapter.ChapterId > currentChapter)
            {
                UnlockChapter(newChapter);
            }
        }

        /// <summary>
        /// Schaltet Chapter frei
        /// </summary>
        private void UnlockChapter(StoryChapter chapter)
        {
            currentChapter = chapter.ChapterId;
            OnChapterUnlocked?.Invoke(chapter);

            // Zeige Chapter-Unlock UI
            if (storyUI != null)
            {
                storyUI.ShowChapterUnlock(chapter);
            }

            Debug.Log($"📖 Chapter {chapter.ChapterId} freigeschaltet: {chapter.ChapterTitle}");
        }

        /// <summary>
        /// Prüft ob Story Beats getriggert werden müssen
        /// </summary>
        private void CheckStoryBeatTriggers()
        {
            if (storyDatabase == null) return;

            StoryChapter currentChapterData = storyDatabase.GetChapter(currentChapter);
            if (currentChapterData == null) return;

            foreach (var beat in currentChapterData.Beats)
            {
                // Prüfe ob Beat bereits completed ist
                if (completedBeats.ContainsKey(beat.BeatId) && completedBeats[beat.BeatId])
                {
                    continue;
                }

                // Prüfe ob Trigger-Level erreicht wurde
                if (playerLevel >= beat.TriggerLevel)
                {
                    TriggerStoryBeat(beat);
                }
            }
        }

        /// <summary>
        /// Triggert einen Story Beat
        /// </summary>
        private void TriggerStoryBeat(StoryBeat beat)
        {
            if (beat == null) return;

            // Markiere als completed
            completedBeats[beat.BeatId] = true;
            beat.IsCompleted = true;
            SaveCompletedBeat(beat.BeatId);

            // Zeige Dialog UI
            if (storyUI != null)
            {
                storyUI.ShowDialog(beat);
            }

            // Unlock Lore Entry falls vorhanden
            if (!string.IsNullOrEmpty(beat.LoreCategory))
            {
                UnlockLoreEntry(beat.LoreCategory);
            }

            OnStoryBeatTriggered?.Invoke(beat);
            Debug.Log($"📚 Story Beat getriggert: {beat.BeatId} - {beat.NPCName}");
        }

        /// <summary>
        /// Schaltet Lore Entry frei
        /// </summary>
        private void UnlockLoreEntry(string category)
        {
            if (storyDatabase == null) return;

            LoreEntry entry = storyDatabase.GetLoreEntryByCategory(category);
            if (entry != null && !entry.IsUnlocked)
            {
                entry.IsUnlocked = true;
                OnLoreUnlocked?.Invoke(entry);

                // Zeige Lore-Unlock Notification
                if (storyUI != null)
                {
                    storyUI.NotifyLoreUnlock(entry);
                }

                Debug.Log($"📖 Lore Entry freigeschaltet: {entry.Title}");
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Gibt aktuelles Chapter zurück
        /// </summary>
        public StoryChapter GetCurrentChapter()
        {
            if (storyDatabase == null) return null;
            return storyDatabase.GetChapter(currentChapter);
        }

        /// <summary>
        /// Gibt alle freigeschalteten Lore Entries zurück
        /// </summary>
        public List<LoreEntry> GetUnlockedLore()
        {
            if (storyDatabase == null) return new List<LoreEntry>();
            return storyDatabase.GetUnlockedLore();
        }

        /// <summary>
        /// Gibt alle Lore Entries einer Kategorie zurück
        /// </summary>
        public List<LoreEntry> GetLoreByCategory(string category)
        {
            if (storyDatabase == null) return new List<LoreEntry>();
            return storyDatabase.GetLoreByCategory(category);
        }

        /// <summary>
        /// Wird aufgerufen wenn Spieler Dialog Choice wählt
        /// </summary>
        public void OnDialogChoiceSelected(DialogChoice choice)
        {
            if (choice == null) return;

            // Reward Stardust falls vorhanden
            if (choice.RewardStardust > 0)
            {
                CurrencyManager currencyManager = FindFirstObjectByType<CurrencyManager>();
                if (currencyManager != null)
                {
                    currencyManager.AddStardust(choice.RewardStardust);
                    Debug.Log($"💰 Dialog-Reward: {choice.RewardStardust} Stardust");
                }
            }

            // TODO: Unlock Cosmetic falls vorhanden
            if (!string.IsNullOrEmpty(choice.UnlockCosmetic))
            {
                Debug.Log($"🎨 Cosmetic freigeschaltet: {choice.UnlockCosmetic}");
            }

            // TODO: Branching Narrative - Trigger nächsten Beat falls vorhanden
            if (choice.NextBeatId > 0)
            {
                StoryBeat nextBeat = storyDatabase?.GetBeat(choice.NextBeatId);
                if (nextBeat != null)
                {
                    TriggerStoryBeat(nextBeat);
                }
            }
        }

        /// <summary>
        /// Prüft ob Beat bereits completed ist
        /// </summary>
        public bool IsBeatCompleted(int beatId)
        {
            return completedBeats.ContainsKey(beatId) && completedBeats[beatId];
        }

        /// <summary>
        /// Gibt Anzahl completed Beats zurück
        /// </summary>
        public int GetCompletedBeatCount()
        {
            return completedBeats.Count;
        }

        #endregion

        #region Save/Load

        /// <summary>
        /// Lädt Story Database aus ScriptableObject
        /// </summary>
        private void LoadStoryDatabase()
        {
            if (storyDatabase != null)
            {
                // Database wird direkt aus ScriptableObject gelesen
                Debug.Log($"✅ Story Database geladen: {storyDatabase.GetChapterCount()} Chapters");
            }
        }

        /// <summary>
        /// Speichert completed Beat
        /// </summary>
        private void SaveCompletedBeat(int beatId)
        {
            PlayerPrefs.SetInt($"StoryBeat_{beatId}_Completed", 1);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Lädt completed Beats aus PlayerPrefs
        /// </summary>
        private void LoadCompletedBeats()
        {
            if (storyDatabase == null) return;

            foreach (var chapter in storyDatabase.GetAllChapters())
            {
                foreach (var beat in chapter.Beats)
                {
                    if (PlayerPrefs.GetInt($"StoryBeat_{beat.BeatId}_Completed", 0) == 1)
                    {
                        completedBeats[beat.BeatId] = true;
                        beat.IsCompleted = true;
                    }
                }
            }
        }

        #endregion
    }
}
