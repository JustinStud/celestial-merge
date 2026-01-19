using UnityEngine;
using System;
using System.Collections.Generic;

namespace CelestialMerge
{
    /// <summary>
    /// Verwaltet Player Progression: Level, XP, Chapters, Milestones
    /// </summary>
    public class CelestialProgressionManager : MonoBehaviour
    {
        [Header("Progression")]
        [SerializeField] private int playerLevel = 1;
        [SerializeField] private long currentXP = 0;
        [SerializeField] private long xpToNextLevel = 100;

        [Header("Chapter System")]
        [SerializeField] private int currentChapter = 1;
        // currentLevelInChapter wird für zukünftige Features verwendet
        // [SerializeField] private int currentLevelInChapter = 1; // Level 1-10 pro Chapter

        [Header("Milestones")]
        [SerializeField] private List<int> mergeMilestones = new List<int> { 10, 25, 50, 100, 250, 500, 1000 };
        [SerializeField] private int totalMerges = 0;

        // Events
        public event Action<int> OnLevelUp;
        public event Action<long> OnXPChanged; // Wird bei jeder XP-Änderung aufgerufen
        public event Action<int> OnChapterUnlocked;
        public event Action<int> OnMilestoneReached;

        public int PlayerLevel => playerLevel;
        public long CurrentXP => currentXP;
        public long XPToNextLevel => xpToNextLevel;
        public int CurrentChapter => currentChapter;
        public int TotalMerges => totalMerges;

        /// <summary>
        /// Gibt aktuellen XP-Fortschritt zurück (0-1)
        /// </summary>
        public float GetCurrentLevelProgress()
        {
            if (xpToNextLevel <= 0) return 0f;
            return Mathf.Clamp01((float)currentXP / (float)xpToNextLevel);
        }

        private void Awake()
        {
            LoadProgression();
            
            // Stelle sicher, dass XPToNextLevel korrekt berechnet ist
            if (xpToNextLevel <= 0)
            {
                CalculateXPToNextLevel();
            }
            
            // Prüfe initial Chapter
            int initialChapter = GetChapterForLevel(playerLevel);
            if (initialChapter > currentChapter)
            {
                currentChapter = initialChapter;
            }
            
            // Trigger initial Events für UI-Update
            OnXPChanged?.Invoke(currentXP);
            OnLevelUp?.Invoke(playerLevel); // Trigger für UI-Update des Levels
        }

        /// <summary>
        /// Fügt XP hinzu und prüft Level-Up
        /// </summary>
        public void AddXP(long amount)
        {
            if (amount <= 0) return;

            currentXP += amount;
            
            // Trigger XP Changed Event
            OnXPChanged?.Invoke(currentXP);

            // Prüfe Level-Up
            while (currentXP >= xpToNextLevel && playerLevel < 500) // Max Level 500
            {
                currentXP -= xpToNextLevel;
                LevelUp();
            }

            SaveProgression();
        }

        /// <summary>
        /// Level-Up Logik
        /// </summary>
        private void LevelUp()
        {
            playerLevel++;
            CalculateXPToNextLevel();

            // Prüfe Chapter-Unlock
            CheckChapterUnlock();

            // Prüfe Board Expansion (alle 4 Level)
            if (playerLevel % 4 == 0)
            {
                // Board Expansion Event
                Debug.Log($"Level {playerLevel}: Board kann erweitert werden!");
            }

            OnLevelUp?.Invoke(playerLevel);

            // Audio: Level Up Sound
            var audioManager = CelestialMerge.Audio.CelestialAudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.PlayLevelUpSound();
            }

            Debug.Log($"🎉 Level Up! Jetzt Level {playerLevel}");
        }

        /// <summary>
        /// Berechnet XP für nächstes Level (exponentielles Wachstum)
        /// </summary>
        private void CalculateXPToNextLevel()
        {
            // Exponentielle Formel: baseXP * (1.1 ^ level)
            xpToNextLevel = Mathf.RoundToInt(100 * Mathf.Pow(1.1f, playerLevel - 1));
        }

        /// <summary>
        /// Prüft ob neues Chapter freigeschaltet werden kann
        /// </summary>
        private void CheckChapterUnlock()
        {
            int newChapter = GetChapterForLevel(playerLevel);
            if (newChapter > currentChapter)
            {
                currentChapter = newChapter;
                OnChapterUnlocked?.Invoke(currentChapter);
                Debug.Log($"📖 Chapter {currentChapter} freigeschaltet!");
            }
        }

        /// <summary>
        /// Gibt Chapter für gegebenes Level zurück
        /// </summary>
        public int GetChapterForLevel(int level)
        {
            if (level <= 10) return 1;
            if (level <= 25) return 2;
            if (level <= 45) return 3;
            if (level <= 70) return 4;
            if (level <= 100) return 5;
            return 6; // Post-Game
        }

        /// <summary>
        /// Registriert einen Merge
        /// </summary>
        public void RegisterMerge()
        {
            totalMerges++;

            // Prüfe Milestones
            foreach (int milestone in mergeMilestones)
            {
                if (totalMerges == milestone)
                {
                    OnMilestoneReached?.Invoke(milestone);
                    Debug.Log($"🏆 Milestone erreicht: {milestone} Merges!");
                    break;
                }
            }

            SaveProgression();
        }

        /// <summary>
        /// Gibt Stardust Capacity für aktuelles Level zurück
        /// </summary>
        public long GetStardustCapacity()
        {
            if (playerLevel <= 10) return 5000;
            if (playerLevel <= 30) return 15000;
            if (playerLevel <= 60) return 40000;
            if (playerLevel <= 100) return 100000;
            return 500000; // Level 101+
        }

        /// <summary>
        /// Gibt Board Expansion Speed Multiplier zurück
        /// </summary>
        public float GetBoardExpansionSpeed()
        {
            if (playerLevel <= 10) return 1.0f;
            if (playerLevel <= 30) return 1.1f; // +10%
            if (playerLevel <= 60) return 1.25f; // +25%
            if (playerLevel <= 100) return 1.5f; // +50%
            return 2.0f; // Unlimited
        }

        #region Save/Load

        private void SaveProgression()
        {
            PlayerPrefs.SetInt("PlayerLevel", playerLevel);
            PlayerPrefs.SetString("CurrentXP", currentXP.ToString());
            PlayerPrefs.SetString("XPToNextLevel", xpToNextLevel.ToString());
            PlayerPrefs.SetInt("CurrentChapter", currentChapter);
            PlayerPrefs.SetInt("TotalMerges", totalMerges);
            PlayerPrefs.Save();
        }

        private void LoadProgression()
        {
            playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
            currentChapter = PlayerPrefs.GetInt("CurrentChapter", 1);
            totalMerges = PlayerPrefs.GetInt("TotalMerges", 0);

            string xpStr = PlayerPrefs.GetString("CurrentXP", "0");
            if (long.TryParse(xpStr, out long loadedXP))
            {
                currentXP = loadedXP;
            }

            string xpToNextStr = PlayerPrefs.GetString("XPToNextLevel", "");
            if (!string.IsNullOrEmpty(xpToNextStr) && long.TryParse(xpToNextStr, out long loadedXPToNext))
            {
                xpToNextLevel = loadedXPToNext;
            }
            else
            {
                // Berechne XPToNextLevel basierend auf aktuellem Level
                CalculateXPToNextLevel();
            }
            
            Debug.Log($"📊 Progression geladen: Level {playerLevel}, XP {currentXP}/{xpToNextLevel}, Chapter {currentChapter}, Merges {totalMerges}");
        }

        #endregion
    }
}
