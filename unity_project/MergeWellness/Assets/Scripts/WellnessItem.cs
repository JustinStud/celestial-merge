using System;
using UnityEngine;

namespace MergeWellness
{
    /// <summary>
    /// Repräsentiert ein Wellness-Item mit Tier-System (1-10)
    /// </summary>
    [Serializable]
    public class WellnessItem
    {
        [SerializeField] private string itemId;
        [SerializeField] private string itemName;
        [SerializeField] private int tier;
        [SerializeField] private string itemType; // yoga, meditation, herbal, etc.
        [SerializeField] private string wellnessFact; // Fakten-Text für Pop-ups
        [SerializeField] private Sprite itemSprite;

        public string ItemId => itemId;
        public string ItemName => itemName;
        public int Tier => tier;
        public string ItemType => itemType;
        public string WellnessFact => wellnessFact;
        public Sprite ItemSprite => itemSprite;

        public WellnessItem(string id, string name, int tier, string type, string fact = "")
        {
            this.itemId = id;
            this.itemName = name;
            this.tier = tier;
            this.itemType = type;
            this.wellnessFact = fact;
        }

        /// <summary>
        /// Prüft ob zwei Items gemerged werden können (gleicher Typ und Tier)
        /// </summary>
        public bool CanMergeWith(WellnessItem other)
        {
            if (other == null) return false;
            return this.itemType == other.itemType && this.tier == other.tier;
        }

        /// <summary>
        /// Gibt die Item-ID des gemergten Items zurück (Tier + 1)
        /// </summary>
        public string GetMergedResultId()
        {
            // Beispiel: yoga_mat_tier1 + yoga_mat_tier1 → meditation_space_tier2
            // Die Logik wird vom ItemDatabase verwaltet
            return $"{itemType}_{tier + 1}";
        }

        public override string ToString()
        {
            return $"{itemName} (Tier {tier}, Type: {itemType})";
        }
    }
}
