using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MergeWellness
{
    /// <summary>
    /// Haupt-Gameplay-Manager: Progression, Daily Rewards, Merge-Milestones, Wellness-Facts
    /// </summary>
    public class GameplayManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private UIManager uiManager;

        [Header("Progression")]
        [SerializeField] private int totalMerges = 0;
        [SerializeField] private int totalScore = 0;
        [SerializeField] private List<int> mergeMilestones = new List<int> { 10, 25, 50, 100, 250, 500 };

        [Header("Daily Rewards")]
        [SerializeField] private DateTime lastDailyRewardDate;
        [SerializeField] private bool dailyRewardClaimed = false;

        private Dictionary<string, int> itemTypeCounts = new Dictionary<string, int>();

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Validiere ItemDatabase
            if (itemDatabase != null)
            {
                // Prüfe ob Datenbank initialisiert ist
                var starterIds = itemDatabase.GetStarterItemIds();
                if (starterIds == null || starterIds.Count == 0)
                {
                    Debug.LogWarning("⚠️ ItemDatabase ist leer! Initialisiere Standard-Items...");
                    itemDatabase.InitializeDefaultItems();
                }
            }
            else
            {
                Debug.LogError("❌ ItemDatabase nicht gesetzt! Bitte im Inspector zuweisen.");
            }

            // Lade Spielstand
            LoadGameState();

            // Prüfe Daily Reward
            CheckDailyReward();

            // Initialisiere UI
            if (uiManager != null)
            {
                uiManager.UpdateScore(totalScore);
                uiManager.UpdateMergeCount(totalMerges);
            }

            Debug.Log("GameplayManager initialisiert");
        }

        /// <summary>
        /// Wird aufgerufen wenn Items gemerged werden
        /// </summary>
        public void OnItemMerged(WellnessItem item1, WellnessItem item2, WellnessItem mergedItem)
        {
            totalMerges++;
            totalScore += CalculateMergeScore(mergedItem.Tier);

            // Update UI
            if (uiManager != null)
            {
                uiManager.UpdateScore(totalScore);
                uiManager.UpdateMergeCount(totalMerges);
            }

            // Prüfe Merge-Milestones
            CheckMergeMilestones();

            // Zeige Wellness-Fact
            if (!string.IsNullOrEmpty(mergedItem.WellnessFact))
            {
                ShowWellnessFact(mergedItem);
            }

            // Speichere Spielstand
            SaveGameState();

            Debug.Log($"Merge abgeschlossen! Score: +{CalculateMergeScore(mergedItem.Tier)}, Total Merges: {totalMerges}");
        }

        private int CalculateMergeScore(int tier)
        {
            // Höhere Tiers = mehr Punkte
            return tier * 10;
        }

        /// <summary>
        /// Prüft ob Merge-Milestone erreicht wurde
        /// </summary>
        private void CheckMergeMilestones()
        {
            foreach (int milestone in mergeMilestones)
            {
                if (totalMerges == milestone)
                {
                    OnMilestoneReached(milestone);
                    break;
                }
            }
        }

        private void OnMilestoneReached(int milestone)
        {
            Debug.Log($"🎉 Milestone erreicht: {milestone} Merges!");

            // Belohnung geben
            GiveMilestoneReward(milestone);

            // UI-Benachrichtigung
            if (uiManager != null)
            {
                uiManager.ShowMilestoneNotification(milestone);
            }
        }

        private void GiveMilestoneReward(int milestone)
        {
            // Beispiel: Höherwertiges Item als Belohnung
            List<string> starterIds = itemDatabase.GetStarterItemIds();
            if (starterIds.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, starterIds.Count);
                WellnessItem reward = itemDatabase.CreateItem(starterIds[randomIndex]);
                gridManager?.AddItemToGrid(reward);
            }
        }

        /// <summary>
        /// Zeigt Wellness-Fact Pop-up
        /// </summary>
        private void ShowWellnessFact(WellnessItem item)
        {
            if (uiManager != null)
            {
                uiManager.ShowWellnessFact(item.ItemName, item.WellnessFact);
            }
            else
            {
                Debug.Log($"💡 {item.ItemName}: {item.WellnessFact}");
            }
        }

        /// <summary>
        /// Prüft ob Daily Reward verfügbar ist
        /// </summary>
        private void CheckDailyReward()
        {
            DateTime today = DateTime.Today;
            if (lastDailyRewardDate != today)
            {
                dailyRewardClaimed = false;
            }

            if (!dailyRewardClaimed)
            {
                // Zeige Daily Reward Button
                if (uiManager != null)
                {
                    uiManager.ShowDailyRewardButton(true);
                }
            }
        }

        /// <summary>
        /// Gibt Daily Reward aus
        /// </summary>
        public void ClaimDailyReward()
        {
            if (dailyRewardClaimed)
            {
                Debug.LogWarning("Daily Reward bereits abgeholt!");
                return;
            }

            // Wähle zufälliges Starter-Item
            List<string> starterIds = itemDatabase.GetStarterItemIds();
            if (starterIds.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, starterIds.Count);
                WellnessItem reward = itemDatabase.CreateItem(starterIds[randomIndex]);

                if (gridManager != null)
                {
                    bool added = gridManager.AddItemToGrid(reward);
                    if (!added)
                    {
                        Debug.LogWarning("Grid ist voll! Item konnte nicht hinzugefügt werden.");
                    }
                }

                dailyRewardClaimed = true;
                lastDailyRewardDate = DateTime.Today;
                SaveGameState();

                if (uiManager != null)
                {
                    uiManager.ShowDailyRewardButton(false);
                }

                Debug.Log($"Daily Reward abgeholt: {reward.ItemName}");
            }
        }

        /// <summary>
        /// Spawnt ein zufälliges Starter-Item (für Tests/Cheats)
        /// </summary>
        [ContextMenu("Spawn Random Item")]
        public void SpawnRandomItem()
        {
            if (itemDatabase == null || gridManager == null)
            {
                Debug.LogError("ItemDatabase oder GridManager nicht gesetzt!");
                return;
            }

            List<string> starterIds = itemDatabase.GetStarterItemIds();
            if (starterIds.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, starterIds.Count);
                WellnessItem item = itemDatabase.CreateItem(starterIds[randomIndex]);

                if (gridManager.AddItemToGrid(item))
                {
                    Debug.Log($"✓ Item gespawnt: {item.ItemName}");
                }
                else
                {
                    Debug.LogWarning("Grid ist voll!");
                }
            }
        }

        #region Save/Load

        private void SaveGameState()
        {
            PlayerPrefs.SetInt("TotalMerges", totalMerges);
            PlayerPrefs.SetInt("TotalScore", totalScore);
            PlayerPrefs.SetString("LastDailyRewardDate", lastDailyRewardDate.ToString("yyyy-MM-dd"));
            PlayerPrefs.SetInt("DailyRewardClaimed", dailyRewardClaimed ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadGameState()
        {
            totalMerges = PlayerPrefs.GetInt("TotalMerges", 0);
            totalScore = PlayerPrefs.GetInt("TotalScore", 0);

            string dateStr = PlayerPrefs.GetString("LastDailyRewardDate", "");
            if (!string.IsNullOrEmpty(dateStr))
            {
                if (DateTime.TryParse(dateStr, out DateTime date))
                {
                    lastDailyRewardDate = date;
                }
            }

            dailyRewardClaimed = PlayerPrefs.GetInt("DailyRewardClaimed", 0) == 1;
        }

        #endregion
    }
}
