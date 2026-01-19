using UnityEngine;
using System.Collections.Generic;
using CelestialMerge.Audio;
using CelestialMerge.Visual;

namespace CelestialMerge
{
    /// <summary>
    /// Verwaltet 3× Merge-Mechanik für Celestial Merge
    /// Unterstützt sowohl 2× als auch 3× Merges (3× gibt Bonus)
    /// </summary>
    public class CelestialMergeManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CelestialItemDatabase itemDatabase;
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private CelestialProgressionManager progressionManager;
        [SerializeField] private CelestialAudioManager audioManager;
        [SerializeField] private ExpandableBoardManager boardManager; // Für Merge-Position

        private void Awake()
        {
            // Auto-Find References
            if (itemDatabase == null)
            {
                itemDatabase = FindFirstObjectByType<CelestialItemDatabase>();
            }

            if (currencyManager == null)
            {
                currencyManager = FindFirstObjectByType<CurrencyManager>();
            }

            if (progressionManager == null)
            {
                progressionManager = FindFirstObjectByType<CelestialProgressionManager>();
            }

            if (audioManager == null)
            {
                audioManager = CelestialAudioManager.Instance;
            }

            if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<ExpandableBoardManager>();
            }
        }

        [Header("Merge Settings")]
        [SerializeField] private bool allowTwoMerge = true; // 2× Merge erlauben
        [SerializeField] private bool allowThreeMerge = true; // 3× Merge erlauben
        [SerializeField] private float threeMergeBonusMultiplier = 1.5f; // +50% Bonus für 3× Merge

        // Events
        public event System.Action<CelestialItem, CelestialItem, CelestialItem> OnThreeMerge;
        public event System.Action<CelestialItem, CelestialItem> OnTwoMerge;

        /// <summary>
        /// Führt 2× Merge durch
        /// </summary>
        public MergeResult PerformTwoMerge(CelestialItem item1, CelestialItem item2)
        {
            if (item1 == null || item2 == null)
            {
                return new MergeResult { Success = false, ErrorMessage = "Items sind null!" };
            }

            if (!item1.CanMergeWith(item2))
            {
                return new MergeResult { Success = false, ErrorMessage = "Items können nicht gemerged werden!" };
            }

            if (!allowTwoMerge)
            {
                return new MergeResult { Success = false, ErrorMessage = "2× Merge ist deaktiviert!" };
            }

            // Erstelle gemergtes Item
            string mergedItemId = itemDatabase.GetMergedItemId(item1, item2, false);
            if (string.IsNullOrEmpty(mergedItemId))
            {
                return new MergeResult { Success = false, ErrorMessage = "Merged Item nicht gefunden!" };
            }

            CelestialItem mergedItem = itemDatabase.CreateItem(mergedItemId);
            if (mergedItem == null)
            {
                return new MergeResult { Success = false, ErrorMessage = "Merged Item konnte nicht erstellt werden!" };
            }

            // Berechne Rewards (Standard)
            int stardustReward = CalculateStardustReward(item1, item2, false);
            int xpReward = CalculateXpReward(item1, item2, false);

            // Gebe Rewards
            if (currencyManager != null)
            {
                currencyManager.AddStardust(stardustReward);
                Debug.Log($"💰 Stardust Reward: +{stardustReward} (Total: {currencyManager.Stardust})");
                
                // Audio Feedback
                if (audioManager != null)
                {
                    audioManager.PlayCoinCollectSound();
                }
            }
            else
            {
                Debug.LogWarning("⚠️ CurrencyManager ist null - Stardust Reward nicht vergeben!");
            }

            if (progressionManager != null)
            {
                long xpBefore = progressionManager.CurrentXP;
                progressionManager.AddXP(xpReward);
                long xpAfter = progressionManager.CurrentXP;
                Debug.Log($"⭐ XP Reward: +{xpReward} (Vorher: {xpBefore}, Nachher: {xpAfter}, Level: {progressionManager.PlayerLevel})");
            }
            else
            {
                Debug.LogWarning("⚠️ ProgressionManager ist null - XP Reward nicht vergeben!");
            }

            // Audio: Merge Sound
            if (audioManager != null)
            {
                audioManager.PlayMergeSound();
            }

            // Visual Feedback: Merge Particles & Effects
            if (Visual.MergeFeedbackSystem.Instance != null)
            {
                Vector3 mergePosition = GetMergePosition(item1, item2);
                Visual.MergeFeedbackSystem.Instance.ShowMergeFeedback(mergePosition, mergedItem.Rarity, false);
                Visual.MergeFeedbackSystem.Instance.ShowStardustReward(mergePosition, stardustReward);
                Visual.MergeFeedbackSystem.Instance.ShowXPReward(mergePosition, xpReward);
            }

            OnTwoMerge?.Invoke(item1, item2);

            return new MergeResult
            {
                Success = true,
                MergedItem = mergedItem,
                StardustReward = stardustReward,
                XpReward = xpReward,
                IsThreeMerge = false
            };
        }

        /// <summary>
        /// Führt 3× Merge durch (mit Bonus)
        /// </summary>
        public MergeResult PerformThreeMerge(CelestialItem item1, CelestialItem item2, CelestialItem item3)
        {
            if (item1 == null || item2 == null || item3 == null)
            {
                return new MergeResult { Success = false, ErrorMessage = "Items sind null!" };
            }

            if (!CelestialItem.CanMergeThree(item1, item2, item3))
            {
                return new MergeResult { Success = false, ErrorMessage = "Items können nicht gemerged werden!" };
            }

            if (!allowThreeMerge)
            {
                return new MergeResult { Success = false, ErrorMessage = "3× Merge ist deaktiviert!" };
            }

            // Erstelle gemergtes Item (gleiche Logik wie 2× Merge)
            string mergedItemId = itemDatabase.GetMergedItemId(item1, item2, true);
            if (string.IsNullOrEmpty(mergedItemId))
            {
                return new MergeResult { Success = false, ErrorMessage = "Merged Item nicht gefunden!" };
            }

            CelestialItem mergedItem = itemDatabase.CreateItem(mergedItemId);
            if (mergedItem == null)
            {
                return new MergeResult { Success = false, ErrorMessage = "Merged Item konnte nicht erstellt werden!" };
            }

            // Berechne Rewards (mit Bonus für 3× Merge)
            int stardustReward = CalculateStardustReward(item1, item2, true);
            int xpReward = CalculateXpReward(item1, item2, true);
            int bonusCrystals = CalculateBonusCrystals(item1);

            // Gebe Rewards
            if (currencyManager != null)
            {
                currencyManager.AddStardust(stardustReward);
                if (bonusCrystals > 0)
                {
                    currencyManager.AddCrystals(bonusCrystals);
                }
                Debug.Log($"💰 3× Merge Rewards: +{stardustReward} Stardust, +{bonusCrystals} Crystals (Total: {currencyManager.Stardust})");
            }
            else
            {
                Debug.LogWarning("⚠️ CurrencyManager ist null - Rewards nicht vergeben!");
            }

            if (progressionManager != null)
            {
                long xpBefore = progressionManager.CurrentXP;
                progressionManager.AddXP(xpReward);
                long xpAfter = progressionManager.CurrentXP;
                Debug.Log($"⭐ 3× Merge XP Reward: +{xpReward} (Vorher: {xpBefore}, Nachher: {xpAfter}, Level: {progressionManager.PlayerLevel})");
            }
            else
            {
                Debug.LogWarning("⚠️ ProgressionManager ist null - XP Reward nicht vergeben!");
            }

            // Audio: Merge Sound
            if (audioManager != null)
            {
                audioManager.PlayMergeSound();
            }

            // Visual Feedback: Merge Particles & Effects
            if (Visual.MergeFeedbackSystem.Instance != null)
            {
                Vector3 mergePosition = GetMergePosition(item1, item2);
                Visual.MergeFeedbackSystem.Instance.ShowMergeFeedback(mergePosition, mergedItem.Rarity, true);
                Visual.MergeFeedbackSystem.Instance.ShowStardustReward(mergePosition, stardustReward);
                Visual.MergeFeedbackSystem.Instance.ShowXPReward(mergePosition, xpReward);
                if (bonusCrystals > 0)
                {
                    Visual.MergeFeedbackSystem.Instance.ShowCrystalReward(mergePosition, bonusCrystals);
                }
            }

            OnThreeMerge?.Invoke(item1, item2, item3);

            return new MergeResult
            {
                Success = true,
                MergedItem = mergedItem,
                StardustReward = stardustReward,
                XpReward = xpReward,
                CrystalReward = bonusCrystals,
                IsThreeMerge = true
            };
        }

        /// <summary>
        /// Berechnet Stardust-Reward für Merge
        /// </summary>
        private int CalculateStardustReward(CelestialItem item1, CelestialItem item2, bool isThreeMerge)
        {
            int baseReward = item1.StardustValue + item2.StardustValue;
            
            if (isThreeMerge)
            {
                // 3× Merge gibt Bonus
                baseReward = Mathf.RoundToInt(baseReward * threeMergeBonusMultiplier);
            }

            // Rarity Bonus
            float rarityMultiplier = GetRarityMultiplier(item1.Rarity);
            baseReward = Mathf.RoundToInt(baseReward * rarityMultiplier);

            return baseReward;
        }

        /// <summary>
        /// Berechnet XP-Reward für Merge
        /// </summary>
        private int CalculateXpReward(CelestialItem item1, CelestialItem item2, bool isThreeMerge)
        {
            int baseReward = item1.XpReward + item2.XpReward;

            if (isThreeMerge)
            {
                baseReward = Mathf.RoundToInt(baseReward * threeMergeBonusMultiplier);
            }

            // Rarity Bonus
            float rarityMultiplier = GetRarityMultiplier(item1.Rarity);
            baseReward = Mathf.RoundToInt(baseReward * rarityMultiplier);

            return baseReward;
        }

        /// <summary>
        /// Berechnet Bonus-Crystals für 3× Merge
        /// </summary>
        private int CalculateBonusCrystals(CelestialItem item)
        {
            // 3× Merge gibt +5-25 Crystals basierend auf Level
            int baseCrystals = Mathf.Clamp(item.Level, 1, 5);
            return baseCrystals * 5; // 5, 10, 15, 20, 25
        }

        /// <summary>
        /// Gibt Rarity-Multiplier zurück
        /// </summary>
        private float GetRarityMultiplier(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common: return 1.0f;
                case ItemRarity.Uncommon: return 1.05f; // +5%
                case ItemRarity.Rare: return 1.15f; // +15%
                case ItemRarity.Epic: return 1.30f; // +30%
                case ItemRarity.Legendary: return 1.50f; // +50%
                case ItemRarity.Mythic: return 2.0f; // +100%
                default: return 1.0f;
            }
        }

        /// <summary>
        /// Gibt Merge-Position zurück (für Visual Feedback)
        /// </summary>
        private Vector3 GetMergePosition(CelestialItem item1, CelestialItem item2)
        {
            // Versuche Position vom Board zu bekommen
            if (boardManager != null)
            {
                // Finde Slots mit diesen Items (via Reflection oder public Method)
                CelestialBoardSlot[] allSlots = FindObjectsByType<CelestialBoardSlot>(FindObjectsSortMode.None);
                foreach (var slot in allSlots)
                {
                    if (slot != null && slot.CurrentItem == item1)
                    {
                        // Konvertiere UI Position zu World Position
                        RectTransform slotRect = slot.GetComponent<RectTransform>();
                        if (slotRect != null)
                        {
                            return slotRect.position;
                        }
                    }
                }
            }

            // Fallback: Screen Center
            return new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        }
    }

    /// <summary>
    /// Ergebnis eines Merge-Vorgangs
    /// </summary>
    public class MergeResult
    {
        public bool Success;
        public string ErrorMessage;
        public CelestialItem MergedItem;
        public int StardustReward;
        public int XpReward;
        public int CrystalReward;
        public bool IsThreeMerge;
    }
}
