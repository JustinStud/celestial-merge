using System;
using UnityEngine;

namespace CelestialMerge
{
    /// <summary>
    /// Celestial Item mit Rarity, Category und erweiterten Properties
    /// </summary>
    [Serializable]
    public class CelestialItem
    {
        [SerializeField] private string itemId;
        [SerializeField] private string itemName;
        [SerializeField] private int level; // Level statt Tier (1-500+)
        [SerializeField] private string category; // celestial_bodies, structures, lifeforms, artifacts, elements, decorations
        [SerializeField] private ItemRarity rarity;
        [SerializeField] private string loreDescription;
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private int stardustValue;
        [SerializeField] private int crystalValue;
        [SerializeField] private int xpReward;

        // Public Properties
        public string ItemId => itemId;
        public string ItemName => itemName;
        public int Level => level;
        public string Category => category;
        public ItemRarity Rarity => rarity;
        public string LoreDescription => loreDescription;
        public Sprite ItemSprite => itemSprite;
        public int StardustValue => stardustValue;
        public int CrystalValue => crystalValue;
        public int XpReward => xpReward;

        public CelestialItem(string id, string name, int level, string category, ItemRarity rarity, 
            string lore = "", Sprite sprite = null, int stardust = 0, int crystal = 0, int xp = 0)
        {
            this.itemId = id;
            this.itemName = name;
            this.level = level;
            this.category = category;
            this.rarity = rarity;
            this.loreDescription = lore;
            this.itemSprite = sprite;  // SPRITE WIRD JETZT GESETZT!
            this.stardustValue = stardust;
            this.crystalValue = crystal;
            this.xpReward = xp;
        }

        /// <summary>
        /// Prüft ob zwei Items gemerged werden können (gleiche Category, Level und Rarity)
        /// </summary>
        public bool CanMergeWith(CelestialItem other)
        {
            if (other == null) return false;
            return this.category == other.category && 
                   this.level == other.level && 
                   this.rarity == other.rarity;
        }

        /// <summary>
        /// Prüft ob 3 Items gemerged werden können (für 3× Merge mit Bonus)
        /// </summary>
        public static bool CanMergeThree(CelestialItem item1, CelestialItem item2, CelestialItem item3)
        {
            if (item1 == null || item2 == null || item3 == null) return false;
            return item1.CanMergeWith(item2) && item2.CanMergeWith(item3);
        }

        public override string ToString()
        {
            return $"{itemName} (Lvl {level}, {category}, {rarity})";
        }
    }

    /// <summary>
    /// Item Rarity System (Common → Mythic)
    /// </summary>
    public enum ItemRarity
    {
        Common,      // 60% - Gray
        Uncommon,    // 25% - Green, +5% Boost
        Rare,         // 10% - Blue, +15% Boost
        Epic,         // 3% - Purple, +30% Boost
        Legendary,   // 1% - Gold, +50% Boost
        Mythic       // 0.1% - Rainbow, +100% Boost
    }
}
