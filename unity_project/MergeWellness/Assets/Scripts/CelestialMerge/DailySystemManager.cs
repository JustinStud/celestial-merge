using UnityEngine;
using System;
using System.Collections.Generic;

namespace CelestialMerge
{
    /// <summary>
    /// Verwaltet Daily Systems: Login Bonuses, Daily Quests, Streaks
    /// </summary>
    public class DailySystemManager : MonoBehaviour
    {
        [Header("Daily Login")]
        [SerializeField] private int currentLoginDay = 0;
        [SerializeField] private DateTime lastLoginDate;
        [SerializeField] private bool todayClaimed = false;

        [Header("Daily Quests")]
        [SerializeField] private List<DailyQuest> activeQuests = new List<DailyQuest>();
        [SerializeField] private int completedQuestsToday = 0;

        [Header("Streaks")]
        [SerializeField] private int loginStreak = 0;
        // mergeStreak wird für zukünftige Features verwendet
        // [SerializeField] private int mergeStreak = 0;

        private CurrencyManager currencyManager;
        private CelestialProgressionManager progressionManager;

        // Events
        public event Action<int> OnLoginBonusClaimed;
        public event Action<DailyQuest> OnQuestCompleted;
        public event Action OnAllQuestsCompleted;

        private void Awake()
        {
            currencyManager = FindFirstObjectByType<CurrencyManager>();
            progressionManager = FindFirstObjectByType<CelestialProgressionManager>();
            LoadDailyState();
        }

        private void Start()
        {
            CheckDailyReset();
            InitializeDailyQuests();
        }

        /// <summary>
        /// Prüft ob Daily Reset nötig ist
        /// </summary>
        private void CheckDailyReset()
        {
            DateTime today = DateTime.Today;
            
            if (lastLoginDate != today)
            {
                // Neuer Tag - Reset
                if (lastLoginDate != default(DateTime))
                {
                    // Prüfe Streak
                    TimeSpan timeSinceLastLogin = today - lastLoginDate;
                    if (timeSinceLastLogin.TotalDays == 1)
                    {
                        // Streak fortgesetzt
                        loginStreak++;
                    }
                    else
                    {
                        // Streak verloren
                        loginStreak = 0;
                    }
                }
                else
                {
                    loginStreak = 1;
                }

                lastLoginDate = today;
                todayClaimed = false;
                completedQuestsToday = 0;
                ResetDailyQuests();
            }
        }

        /// <summary>
        /// Initialisiert Daily Quests
        /// </summary>
        private void InitializeDailyQuests()
        {
            if (activeQuests.Count == 0)
            {
                CreateDefaultQuests();
            }
        }

        /// <summary>
        /// Erstellt Standard Daily Quests
        /// </summary>
        private void CreateDefaultQuests()
        {
            activeQuests.Clear();

            activeQuests.Add(new DailyQuest
            {
                questId = "merge_10",
                questName = "Merge 10 Objects",
                questType = QuestType.MergeCount,
                targetValue = 10,
                currentValue = 0,
                rewardStardust = 50,
                rewardCrystals = 0,
                isCompleted = false
            });

            activeQuests.Add(new DailyQuest
            {
                questId = "play_minigame",
                questName = "Play Mini-Game Once",
                questType = QuestType.PlayMinigame,
                targetValue = 1,
                currentValue = 0,
                rewardStardust = 0,
                rewardCrystals = 5,
                isCompleted = false
            });

            activeQuests.Add(new DailyQuest
            {
                questId = "reach_synergy",
                questName = "Reach a Synergy",
                questType = QuestType.ReachSynergy,
                targetValue = 1,
                currentValue = 0,
                rewardStardust = 100,
                rewardCrystals = 0,
                isCompleted = false
            });

            activeQuests.Add(new DailyQuest
            {
                questId = "craft_item",
                questName = "Craft an Item",
                questType = QuestType.CraftItem,
                targetValue = 1,
                currentValue = 0,
                rewardStardust = 75,
                rewardCrystals = 0,
                isCompleted = false
            });

            activeQuests.Add(new DailyQuest
            {
                questId = "watch_ad",
                questName = "Watch an Ad",
                questType = QuestType.WatchAd,
                targetValue = 1,
                currentValue = 0,
                rewardStardust = 200,
                rewardCrystals = 0,
                isCompleted = false
            });
        }

        /// <summary>
        /// Reset Daily Quests
        /// </summary>
        private void ResetDailyQuests()
        {
            foreach (var quest in activeQuests)
            {
                quest.currentValue = 0;
                quest.isCompleted = false;
            }
            completedQuestsToday = 0;
        }

        /// <summary>
        /// Gibt Daily Login Bonus
        /// </summary>
        public void ClaimDailyLoginBonus()
        {
            if (todayClaimed)
            {
                Debug.LogWarning("Daily Login Bonus bereits abgeholt!");
                return;
            }

            // Berechne Bonus basierend auf Tag
            long stardustReward = GetLoginBonusStardust(currentLoginDay);
            int crystalReward = GetLoginBonusCrystals(currentLoginDay);

            if (currencyManager != null)
            {
                currencyManager.AddStardust(stardustReward);
                if (crystalReward > 0)
                {
                    currencyManager.AddCrystals(crystalReward);
                }
            }

            todayClaimed = true;
            currentLoginDay = (currentLoginDay % 7) + 1; // Zyklus 1-7
            SaveDailyState();

            OnLoginBonusClaimed?.Invoke(currentLoginDay);
            Debug.Log($"✅ Daily Login Bonus Tag {currentLoginDay}: {stardustReward} Stardust, {crystalReward} Crystals");
        }

        /// <summary>
        /// Gibt Stardust-Reward für Login-Tag zurück
        /// </summary>
        private long GetLoginBonusStardust(int day)
        {
            switch (day)
            {
                case 1: return 100;
                case 2: return 100;
                case 3: return 150;
                case 4: return 150;
                case 5: return 200;
                case 6: return 250;
                case 7: return 500;
                default: return 100;
            }
        }

        /// <summary>
        /// Gibt Crystal-Reward für Login-Tag zurück
        /// </summary>
        private int GetLoginBonusCrystals(int day)
        {
            switch (day)
            {
                case 1: return 0;
                case 2: return 5;
                case 3: return 10;
                case 4: return 0;
                case 5: return 25;
                case 6: return 50;
                case 7: return 100;
                default: return 0;
            }
        }

        /// <summary>
        /// Aktualisiert Quest-Progress
        /// </summary>
        public void UpdateQuestProgress(QuestType type, int amount = 1)
        {
            foreach (var quest in activeQuests)
            {
                if (quest.questType == type && !quest.isCompleted)
                {
                    quest.currentValue += amount;
                    
                    if (quest.currentValue >= quest.targetValue)
                    {
                        CompleteQuest(quest);
                    }
                }
            }
        }

        /// <summary>
        /// Schließt Quest ab
        /// </summary>
        private void CompleteQuest(DailyQuest quest)
        {
            quest.isCompleted = true;
            completedQuestsToday++;

            // Gebe Rewards
            if (currencyManager != null)
            {
                if (quest.rewardStardust > 0)
                {
                    currencyManager.AddStardust(quest.rewardStardust);
                }
                if (quest.rewardCrystals > 0)
                {
                    currencyManager.AddCrystals(quest.rewardCrystals);
                }
            }

            OnQuestCompleted?.Invoke(quest);

            // Prüfe ob alle Quests abgeschlossen
            if (completedQuestsToday >= activeQuests.Count)
            {
                // Bonus für alle Quests
                if (currencyManager != null)
                {
                    currencyManager.AddStardust(200); // Bonus Stardust
                }
                OnAllQuestsCompleted?.Invoke();
                Debug.Log("🎉 Alle Daily Quests abgeschlossen! Bonus erhalten!");
            }
        }

        /// <summary>
        /// Prüft ob heute bereits abgeholt wurde
        /// </summary>
        public bool IsTodayClaimed()
        {
            return todayClaimed;
        }

        /// <summary>
        /// Gibt aktuellen Login-Tag zurück
        /// </summary>
        public int GetCurrentLoginDay()
        {
            return currentLoginDay;
        }

        /// <summary>
        /// Gibt alle aktiven Quests zurück
        /// </summary>
        public List<DailyQuest> GetActiveQuests()
        {
            return new List<DailyQuest>(activeQuests);
        }

        #region Save/Load

        private void SaveDailyState()
        {
            PlayerPrefs.SetInt("CurrentLoginDay", currentLoginDay);
            PlayerPrefs.SetString("LastLoginDate", lastLoginDate.ToString("yyyy-MM-dd"));
            PlayerPrefs.SetInt("TodayClaimed", todayClaimed ? 1 : 0);
            PlayerPrefs.SetInt("LoginStreak", loginStreak);
            PlayerPrefs.SetInt("CompletedQuestsToday", completedQuestsToday);
            PlayerPrefs.Save();
        }

        private void LoadDailyState()
        {
            currentLoginDay = PlayerPrefs.GetInt("CurrentLoginDay", 0);
            loginStreak = PlayerPrefs.GetInt("LoginStreak", 0);
            completedQuestsToday = PlayerPrefs.GetInt("CompletedQuestsToday", 0);
            todayClaimed = PlayerPrefs.GetInt("TodayClaimed", 0) == 1;

            string dateStr = PlayerPrefs.GetString("LastLoginDate", "");
            if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out DateTime loadedDate))
            {
                lastLoginDate = loadedDate;
            }
        }

        #endregion
    }

    /// <summary>
    /// Daily Quest Datenstruktur
    /// </summary>
    [System.Serializable]
    public class DailyQuest
    {
        public string questId;
        public string questName;
        public QuestType questType;
        public int targetValue;
        public int currentValue;
        public long rewardStardust;
        public int rewardCrystals;
        public bool isCompleted;
    }

    /// <summary>
    /// Quest Types
    /// </summary>
    public enum QuestType
    {
        MergeCount,
        PlayMinigame,
        ReachSynergy,
        CraftItem,
        WatchAd,
        CollectStardust,
        LevelUp
    }
}
