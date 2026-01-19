using System.Collections.Generic;
using System;
using UnityEngine;

namespace CelestialMerge
{
    /// <summary>
    /// Datenbank für 500+ Celestial Items in 6 Kategorien
    /// </summary>
    [CreateAssetMenu(fileName = "CelestialItemDatabase", menuName = "CelestialMerge/ItemDatabase")]
    public class CelestialItemDatabase : ScriptableObject
    {
        [System.Serializable]
        public class ItemData
        {
            [Tooltip("Eindeutige Item-ID (z.B. 'elements_level1_common')")]
            public string itemId = "";
            
            [Tooltip("Anzeigename des Items")]
            public string itemName = "";
            
            [Tooltip("Item-Level (1-500+)")]
            public int level = 1;
            
            [Tooltip("Item-Kategorie (elements, structures, etc.)")]
            public string category = "";
            
            [Tooltip("Item-Seltenheit")]
            public ItemRarity rarity = ItemRarity.Common;
            
            [TextArea(2, 4)]
            [Tooltip("Lore-Beschreibung des Items")]
            public string loreDescription = "";
            
            [Tooltip("Sprite-Bild für das Item (ZIEHE SPRITE HIER REIN!)")]
            [SerializeField]
            public Sprite itemSprite;
            
            [Tooltip("Stardust-Wert beim Verkauf")]
            public int stardustValue = 0;
            
            [Tooltip("Crystal-Wert beim Verkauf")]
            public int crystalValue = 0;
            
            [Tooltip("XP-Belohnung beim Merge")]
            public int xpReward = 0;
        }

        [SerializeField] 
        [Tooltip("Liste aller Items in der Database. Erweitere Items um Sprites zuzuweisen.")]
        private List<ItemData> items = new List<ItemData>();
        private Dictionary<string, ItemData> itemLookup;
        private Dictionary<string, List<ItemData>> itemsByCategory;

        private void OnEnable()
        {
            BuildLookup();
        }

        private void BuildLookup()
        {
            itemLookup = new Dictionary<string, ItemData>();
            itemsByCategory = new Dictionary<string, List<ItemData>>();

            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.itemId))
                {
                    itemLookup[item.itemId] = item;

                    if (!itemsByCategory.ContainsKey(item.category))
                    {
                        itemsByCategory[item.category] = new List<ItemData>();
                    }
                    itemsByCategory[item.category].Add(item);
                }
            }
        }

        /// <summary>
        /// Erstellt ein CelestialItem aus der Datenbank
        /// </summary>
        public CelestialItem CreateItem(string itemId)
        {
            if (itemLookup == null) BuildLookup();

            if (itemLookup.TryGetValue(itemId, out ItemData data))
            {
                return new CelestialItem(
                    data.itemId,
                    data.itemName,
                    data.level,
                    data.category,
                    data.rarity,
                    data.loreDescription,
                    data.itemSprite,  // SPRITE WIRD JETZT ÜBERGEBEN!
                    data.stardustValue,
                    data.crystalValue,
                    data.xpReward
                );
            }

            Debug.LogError($"Item mit ID '{itemId}' nicht gefunden!");
            return null;
        }

        /// <summary>
        /// Gibt das gemergte Item zurück (Level + 1, gleiche Category & Rarity)
        /// </summary>
        public string GetMergedItemId(CelestialItem item1, CelestialItem item2, bool isThreeMerge = false)
        {
            if (!item1.CanMergeWith(item2))
            {
                Debug.LogError($"Items können nicht gemerged werden!");
                return null;
            }

            int nextLevel = item1.Level + 1;
            
            // Versuche verschiedene ID-Formate
            string[] possibleIds = {
                $"{item1.Category}_level{nextLevel}_{item1.Rarity.ToString().ToLower()}",  // Standard-Format
                $"{item1.Category}_level{nextLevel}_common",  // Fallback auf Common
                $"{item1.Category}level{nextLevel}",  // Alternative
            };

            // Prüfe jedes mögliche Format
            foreach (string mergedId in possibleIds)
            {
                if (itemLookup != null && itemLookup.ContainsKey(mergedId))
                {
                    Debug.Log($"✓ Merged Item ID gefunden: {mergedId}");
                    return mergedId;
                }
            }

            // Fallback: Suche nach Item mit gleicher Category und nächsthöherem Level (Rarity ignoriert)
            foreach (var itemData in items)
            {
                if (itemData.category == item1.Category && itemData.level == nextLevel)
                {
                    Debug.Log($"✓ Merged Item gefunden (Fallback): {itemData.itemId}");
                    return itemData.itemId;
                }
            }

            Debug.LogWarning($"Merged Item nicht gefunden! Item1: {item1.ItemId} (Level {item1.Level}, Category: {item1.Category}) → Erwartet: Level {nextLevel}, Category: {item1.Category}");
            Debug.LogWarning($"Versuchte IDs: {string.Join(", ", possibleIds)}");
            return null;
        }

        /// <summary>
        /// Gibt alle Starter-Items zurück (Level 1)
        /// </summary>
        public List<string> GetStarterItemIds()
        {
            List<string> starterIds = new List<string>();
            foreach (var item in items)
            {
                if (item.level == 1)
                {
                    starterIds.Add(item.itemId);
                }
            }
            return starterIds;
        }

        /// <summary>
        /// Initialisiert Standard-Items für Celestial Merge (500+ Items)
        /// </summary>
        [ContextMenu("Initialize Celestial Items")]
        public void InitializeCelestialItems()
        {
            items.Clear();

            // Category 1: Celestial Bodies (Sterne, Planeten, Monde)
            InitializeCelestialBodies();
            
            // Category 2: Structures (Gebäude für Empire)
            InitializeStructures();
            
            // Category 3: Lifeforms (Aliens & Creatures)
            InitializeLifeforms();
            
            // Category 4: Artifacts (Mystische Gegenstände)
            InitializeArtifacts();
            
            // Category 5: Elements (Naturkräfte)
            InitializeElements();
            
            // Category 6: Decorations (Cosmetics)
            InitializeDecorations();

            BuildLookup();
            Debug.Log($"✓ CelestialItemDatabase initialisiert: {items.Count} Items geladen");
        }

        /// <summary>
        /// Initialisiert alle 125 Items aus dem Designer-Dokument (ItemStack_125Items.md)
        /// </summary>
        [ContextMenu("Initialize All 125 Items (Designer)")]
        public void InitializeAll125Items()
        {
            items.Clear();

            // Category 1: Celestial Bodies (25 items: 1-25)
            InitializeCelestialBodies125();
            
            // Category 2: Structures (20 items: 26-45)
            InitializeStructures125();
            
            // Category 3: Lifeforms (18 items: 46-63)
            InitializeLifeforms125();
            
            // Category 4: Artifacts (15 items: 64-80)
            InitializeArtifacts125();
            
            // Category 5: Elements (17 items: 81-97)
            InitializeElements125();
            
            // Category 6: Decorations (15 items: 98-115)
            InitializeDecorations125();

            // UI Icons (116-125) werden nicht als spielbare Items hinzugefügt

            BuildLookup();
            Debug.Log($"✅ CelestialItemDatabase initialisiert: {items.Count} Items geladen (125 Items aus Designer-Dokument)");
        }

        private void InitializeCelestialBodies()
        {
            // Level 1-25: Celestial Bodies Chain
            AddItem("celestial_bodies_level1_common", "Stardust Particle", 1, "celestial_bodies", ItemRarity.Common, 
                "Die Grundbausteine des Universums.", 5, 0, 1);
            AddItem("celestial_bodies_level2_common", "Cosmic Dust Cloud", 2, "celestial_bodies", ItemRarity.Common, 
                "Eine Wolke aus kosmischem Staub.", 10, 0, 2);
            AddItem("celestial_bodies_level3_common", "Proto-Star", 3, "celestial_bodies", ItemRarity.Common, 
                "Ein Stern in der Entstehung.", 20, 0, 5);
            AddItem("celestial_bodies_level4_common", "Young Star", 4, "celestial_bodies", ItemRarity.Common, 
                "Ein junger, leuchtender Stern.", 40, 0, 10);
            AddItem("celestial_bodies_level3_common", "Proto-Star", 3, "celestial_bodies", ItemRarity.Common, 
                "Ein Stern in der Entstehung.", 20, 0, 5);
            AddItem("celestial_bodies_level4_common", "Young Star", 4, "celestial_bodies", ItemRarity.Common, 
                "Ein junger, leuchtender Stern.", 40, 0, 10);
            AddItem("celestial_bodies_level5_common", "Main Sequence Star", 5, "celestial_bodies", ItemRarity.Common, 
                "Ein stabiler Stern in der Hauptreihe.", 80, 0, 20);
            AddItem("celestial_bodies_level6_common", "Red Giant", 6, "celestial_bodies", ItemRarity.Common, 
                "Ein alternder Riesenstern.", 160, 0, 40);
            AddItem("celestial_bodies_level7_common", "White Dwarf", 7, "celestial_bodies", ItemRarity.Common, 
                "Der Überrest eines Sterns.", 320, 0, 80);
            AddItem("celestial_bodies_level8_common", "Neutron Star", 8, "celestial_bodies", ItemRarity.Common, 
                "Ein extrem dichter Stern.", 640, 0, 160);
            AddItem("celestial_bodies_level9_common", "Pulsar", 9, "celestial_bodies", ItemRarity.Common, 
                "Ein rotierender Neutronenstern.", 1280, 0, 320);
            AddItem("celestial_bodies_level10_common", "Quasar", 10, "celestial_bodies", ItemRarity.Common, 
                "Eine leuchtende Galaxie.", 2560, 0, 640);
            AddItem("celestial_bodies_level11_common", "Star Cluster", 11, "celestial_bodies", ItemRarity.Common, 
                "Ein Sternhaufen.", 5000, 0, 1250);
            AddItem("celestial_bodies_level12_common", "Nebula", 12, "celestial_bodies", ItemRarity.Common, 
                "Eine riesige Gaswolke.", 10000, 0, 2500);
            AddItem("celestial_bodies_level15_common", "Galaxy", 15, "celestial_bodies", ItemRarity.Common, 
                "Eine ganze Galaxie.", 50000, 0, 12500);
            AddItem("celestial_bodies_level20_common", "Black Hole", 20, "celestial_bodies", ItemRarity.Common, 
                "Ein schwarzes Loch.", 250000, 0, 62500);
            AddItem("celestial_bodies_level25_common", "Wormhole", 25, "celestial_bodies", ItemRarity.Legendary, 
                "Ein Portal durch Raum und Zeit.", 1000000, 100, 250000);
        }

        private void InitializeStructures()
        {
            AddItem("structures_level1_common", "Energy Beacon", 1, "structures", ItemRarity.Common, 
                "Ein Leuchtfeuer der Energie.", 5, 0, 1);
            AddItem("structures_level2_common", "Solar Collector", 2, "structures", ItemRarity.Common, 
                "Sammelt Sonnenenergie.", 10, 0, 2);
            AddItem("structures_level3_common", "Energy Station", 3, "structures", ItemRarity.Common, 
                "Eine Energie-Station.", 20, 0, 5);
            AddItem("structures_level4_common", "Power Plant", 4, "structures", ItemRarity.Common, 
                "Ein Kraftwerk.", 40, 0, 10);
            AddItem("structures_level5_common", "Space Station", 5, "structures", ItemRarity.Common, 
                "Eine Raumstation.", 80, 0, 20);
            AddItem("structures_level6_common", "Orbital Base", 6, "structures", ItemRarity.Common, 
                "Eine Orbital-Basis.", 160, 0, 40);
            AddItem("structures_level7_common", "Stellar Forge", 7, "structures", ItemRarity.Rare, 
                "Schmiedet Sterne.", 320, 0, 80);
            AddItem("structures_level10_common", "Dyson Sphere", 10, "structures", ItemRarity.Epic, 
                "Eine Sphäre um einen Stern.", 2560, 0, 640);
            AddItem("structures_level15_common", "Mega-Transmitter", 15, "structures", ItemRarity.Legendary, 
                "Sendet Signale durch das Universum.", 10000, 0, 2500);
        }

        private void InitializeLifeforms()
        {
            AddItem("lifeforms_level1_common", "Microbe Spore", 1, "lifeforms", ItemRarity.Common, 
                "Ein winziger Lebenskeim.", 5, 0, 1);
            AddItem("lifeforms_level2_common", "Sentient Crystal", 2, "lifeforms", ItemRarity.Common, 
                "Ein bewusster Kristall.", 10, 0, 2);
            AddItem("lifeforms_level3_common", "Crystal Cluster", 3, "lifeforms", ItemRarity.Common, 
                "Ein Kristall-Cluster.", 20, 0, 5);
            AddItem("lifeforms_level4_common", "Living Crystal", 4, "lifeforms", ItemRarity.Common, 
                "Ein lebender Kristall.", 40, 0, 10);
            AddItem("lifeforms_level5_common", "Cosmic Jellyfish", 5, "lifeforms", ItemRarity.Uncommon, 
                "Eine kosmische Qualle.", 80, 0, 20);
            AddItem("lifeforms_level6_common", "Space Organism", 6, "lifeforms", ItemRarity.Uncommon, 
                "Ein Raum-Organismus.", 160, 0, 40);
            AddItem("lifeforms_level7_common", "Star Whale", 7, "lifeforms", ItemRarity.Rare, 
                "Ein Wal, der durch Sterne schwimmt.", 320, 0, 80);
            AddItem("lifeforms_level10_common", "Dimensional Being", 10, "lifeforms", ItemRarity.Epic, 
                "Ein Wesen aus einer anderen Dimension.", 2560, 0, 640);
            AddItem("lifeforms_level15_common", "Cosmic Guardian", 15, "lifeforms", ItemRarity.Legendary, 
                "Ein Wächter des Universums.", 10000, 0, 2500);
        }

        private void InitializeArtifacts()
        {
            AddItem("artifacts_level1_common", "Ancient Rune", 1, "artifacts", ItemRarity.Common, 
                "Eine uralte Rune.", 5, 0, 1);
            AddItem("artifacts_level2_common", "Glowing Totem", 2, "artifacts", ItemRarity.Common, 
                "Ein leuchtender Totem.", 10, 0, 2);
            AddItem("artifacts_level3_common", "Mystic Rune", 3, "artifacts", ItemRarity.Common, 
                "Eine mystische Rune.", 20, 0, 5);
            AddItem("artifacts_level4_common", "Power Totem", 4, "artifacts", ItemRarity.Common, 
                "Ein mächtiger Totem.", 40, 0, 10);
            AddItem("artifacts_level5_common", "Reality Shard", 5, "artifacts", ItemRarity.Uncommon, 
                "Ein Splitter der Realität.", 80, 0, 20);
            AddItem("artifacts_level6_common", "Dimensional Shard", 6, "artifacts", ItemRarity.Uncommon, 
                "Ein dimensionaler Splitter.", 160, 0, 40);
            AddItem("artifacts_level7_common", "Time Crystal", 7, "artifacts", ItemRarity.Rare, 
                "Ein Kristall der Zeit.", 320, 0, 80);
            AddItem("artifacts_level10_common", "Infinity Stone", 10, "artifacts", ItemRarity.Legendary, 
                "Ein Stein der Unendlichkeit.", 2560, 0, 640);
        }

        private void InitializeElements()
        {
            // Fire Chain (Haupt-Kette für Merging)
            AddItem("elements_level1_common", "Fire Ember", 1, "elements", ItemRarity.Common, 
                "Eine Feuerflamme.", 5, 0, 1);
            AddItem("elements_level2_common", "Fire Flame", 2, "elements", ItemRarity.Common, 
                "Eine stärkere Flamme.", 10, 0, 2);
            AddItem("elements_level3_common", "Fire Blaze", 3, "elements", ItemRarity.Common, 
                "Ein Feuersturm.", 20, 0, 5);
            AddItem("elements_level4_common", "Inferno", 4, "elements", ItemRarity.Common, 
                "Ein Inferno.", 40, 0, 10);
            AddItem("elements_level5_common", "Solar Flare", 5, "elements", ItemRarity.Uncommon, 
                "Ein Sonnenflare.", 80, 0, 20);
            AddItem("elements_level6_common", "Plasma Core", 6, "elements", ItemRarity.Uncommon, 
                "Ein Plasmakern.", 160, 0, 40);
            AddItem("elements_level7_common", "Lightning Bolt", 7, "elements", ItemRarity.Rare, 
                "Ein Blitz.", 320, 0, 80);
            
            // Water Chain (separate Kette)
            AddItem("elements_water_level1_common", "Water Droplet", 1, "elements", ItemRarity.Common, 
                "Ein Wassertropfen.", 5, 0, 1);
            AddItem("elements_water_level2_common", "Water Stream", 2, "elements", ItemRarity.Common, 
                "Ein Wasserstrom.", 10, 0, 2);
            AddItem("elements_water_level3_common", "Waterfall", 3, "elements", ItemRarity.Common, 
                "Ein Wasserfall.", 20, 0, 5);
            
            // Wind Chain (separate Kette)
            AddItem("elements_wind_level1_common", "Wind Gust", 1, "elements", ItemRarity.Common, 
                "Eine Windböe.", 5, 0, 1);
            AddItem("elements_wind_level2_common", "Wind Storm", 2, "elements", ItemRarity.Common, 
                "Ein Windsturm.", 10, 0, 2);
            AddItem("elements_wind_level3_common", "Tornado", 3, "elements", ItemRarity.Common, 
                "Ein Tornado.", 20, 0, 5);
            
            // Earth Chain (separate Kette)
            AddItem("elements_earth_level1_common", "Earth Core", 1, "elements", ItemRarity.Common, 
                "Ein Erdkern.", 5, 0, 1);
            AddItem("elements_earth_level2_common", "Earth Stone", 2, "elements", ItemRarity.Common, 
                "Ein Erdstein.", 10, 0, 2);
            AddItem("elements_earth_level3_common", "Mountain", 3, "elements", ItemRarity.Common, 
                "Ein Berg.", 20, 0, 5);
        }

        private void InitializeDecorations()
        {
            AddItem("decorations_level1_common", "Nebula", 1, "decorations", ItemRarity.Common, 
                "Eine dekorative Nebelwolke.", 5, 0, 1);
            AddItem("decorations_level2_common", "Stardust Fountain", 2, "decorations", ItemRarity.Common, 
                "Ein Brunnen aus Sternenstaub.", 10, 0, 2);
            AddItem("decorations_level3_common", "Crystal Garden", 3, "decorations", ItemRarity.Uncommon, 
                "Ein Garten aus Kristallen.", 20, 0, 5);
            AddItem("decorations_level4_common", "Cosmic Fountain", 4, "decorations", ItemRarity.Uncommon, 
                "Ein kosmischer Brunnen.", 40, 0, 10);
            AddItem("decorations_level5_common", "Stellar Garden", 5, "decorations", ItemRarity.Rare, 
                "Ein Garten der Sterne.", 80, 0, 20);
        }

        private void AddItem(string id, string name, int level, string category, ItemRarity rarity, 
            string lore, int stardust = 0, int crystal = 0, int xp = 0)
        {
            items.Add(new ItemData
            {
                itemId = id,
                itemName = name,
                level = level,
                category = category,
                rarity = rarity,
                loreDescription = lore,
                stardustValue = stardust,
                crystalValue = crystal,
                xpReward = xp
            });
        }

        #region Initialize 125 Items from Designer Document

        private void InitializeCelestialBodies125()
        {
            // Items 1-25: Celestial Bodies Chain (Level 1-25)
            AddItem("celestial_bodies_level1_common", "Stardust Particle", 1, "celestial_bodies", ItemRarity.Common, 
                "Die Grundbausteine des Universums.", 50, 0, 1);
            AddItem("celestial_bodies_level2_uncommon", "Cosmic Dust Cloud", 2, "celestial_bodies", ItemRarity.Uncommon, 
                "Eine Wolke aus kosmischem Staub.", 100, 0, 2);
            AddItem("celestial_bodies_level3_rare", "Proto-Star", 3, "celestial_bodies", ItemRarity.Rare, 
                "Ein Stern in der Entstehung.", 250, 0, 5);
            AddItem("celestial_bodies_level4_epic", "Dwarf Star", 4, "celestial_bodies", ItemRarity.Epic, 
                "Ein Zwergstern.", 500, 0, 10);
            AddItem("celestial_bodies_level5_legendary", "Main Sequence Star", 5, "celestial_bodies", ItemRarity.Legendary, 
                "Ein stabiler Stern in der Hauptreihe.", 1000, 0, 20);
            AddItem("celestial_bodies_level6_epic", "Giant Star", 6, "celestial_bodies", ItemRarity.Epic, 
                "Ein Riesenstern.", 500, 0, 10);
            AddItem("celestial_bodies_level7_legendary", "Supergiant", 7, "celestial_bodies", ItemRarity.Legendary, 
                "Ein Überriesenstern.", 1000, 0, 20);
            AddItem("celestial_bodies_level8_mythic", "Hypergiant", 8, "celestial_bodies", ItemRarity.Mythic, 
                "Ein Hyperriesenstern.", 2500, 0, 50);
            AddItem("celestial_bodies_level9_rare", "Neutron Star", 9, "celestial_bodies", ItemRarity.Rare, 
                "Ein extrem dichter Stern.", 250, 0, 5);
            AddItem("celestial_bodies_level10_uncommon", "Pulsar", 10, "celestial_bodies", ItemRarity.Uncommon, 
                "Ein rotierender Neutronenstern.", 100, 0, 2);
            AddItem("celestial_bodies_level11_common", "Quasar", 11, "celestial_bodies", ItemRarity.Common, 
                "Eine leuchtende Galaxie.", 50, 0, 1);
            AddItem("celestial_bodies_level12_epic", "White Dwarf", 12, "celestial_bodies", ItemRarity.Epic, 
                "Der Überrest eines Sterns.", 500, 0, 10);
            AddItem("celestial_bodies_level13_legendary", "Red Giant", 13, "celestial_bodies", ItemRarity.Legendary, 
                "Ein alternder Riesenstern.", 1000, 0, 20);
            AddItem("celestial_bodies_level14_mythic", "Blue Giant", 14, "celestial_bodies", ItemRarity.Mythic, 
                "Ein blauer Riesenstern.", 2500, 0, 50);
            AddItem("celestial_bodies_level15_rare", "Nebula Core", 15, "celestial_bodies", ItemRarity.Rare, 
                "Der Kern einer Nebelwolke.", 250, 0, 5);
            AddItem("celestial_bodies_level16_uncommon", "Planetary Nebula", 16, "celestial_bodies", ItemRarity.Uncommon, 
                "Eine planetarische Nebelwolke.", 100, 0, 2);
            AddItem("celestial_bodies_level17_common", "Supernova Remnant", 17, "celestial_bodies", ItemRarity.Common, 
                "Der Überrest einer Supernova.", 50, 0, 1);
            AddItem("celestial_bodies_level18_epic", "Black Hole", 18, "celestial_bodies", ItemRarity.Epic, 
                "Ein schwarzes Loch.", 500, 0, 10);
            AddItem("celestial_bodies_level19_legendary", "Wormhole", 19, "celestial_bodies", ItemRarity.Legendary, 
                "Ein Portal durch Raum und Zeit.", 1000, 0, 20);
            AddItem("celestial_bodies_level20_mythic", "Singularity", 20, "celestial_bodies", ItemRarity.Mythic, 
                "Eine Singularität.", 2500, 0, 50);
            AddItem("celestial_bodies_level21_rare", "Event Horizon", 21, "celestial_bodies", ItemRarity.Rare, 
                "Der Ereignishorizont eines schwarzen Lochs.", 250, 0, 5);
            AddItem("celestial_bodies_level22_uncommon", "Hawking Radiation", 22, "celestial_bodies", ItemRarity.Uncommon, 
                "Hawking-Strahlung.", 100, 0, 2);
            AddItem("celestial_bodies_level23_common", "Quantum Foam", 23, "celestial_bodies", ItemRarity.Common, 
                "Quantenschaum.", 50, 0, 1);
            AddItem("celestial_bodies_level24_epic", "Cosmic String", 24, "celestial_bodies", ItemRarity.Epic, 
                "Eine kosmische Saite.", 500, 0, 10);
            AddItem("celestial_bodies_level25_legendary", "Multiverse Nexus", 25, "celestial_bodies", ItemRarity.Legendary, 
                "Ein Nexus zwischen Universen.", 1000, 0, 20);
        }

        private void InitializeStructures125()
        {
            // Items 26-45: Structures Chain (Level 1-20)
            AddItem("structures_level1_common", "Energy Beacon", 1, "structures", ItemRarity.Common, 
                "Ein Leuchtfeuer der Energie.", 50, 0, 1);
            AddItem("structures_level2_uncommon", "Solar Collector", 2, "structures", ItemRarity.Uncommon, 
                "Sammelt Sonnenenergie.", 100, 0, 2);
            AddItem("structures_level3_rare", "Power Node", 3, "structures", ItemRarity.Rare, 
                "Ein Energieknoten.", 250, 0, 5);
            AddItem("structures_level4_epic", "Fusion Reactor", 4, "structures", ItemRarity.Epic, 
                "Ein Fusionsreaktor.", 500, 0, 10);
            AddItem("structures_level5_legendary", "Quantum Generator", 5, "structures", ItemRarity.Legendary, 
                "Ein Quantengenerator.", 1000, 0, 20);
            AddItem("structures_level6_mythic", "Antimatter Plant", 6, "structures", ItemRarity.Mythic, 
                "Eine Antimaterie-Anlage.", 2500, 0, 50);
            AddItem("structures_level7_rare", "Dyson Sphere Segment", 7, "structures", ItemRarity.Rare, 
                "Ein Segment einer Dyson-Sphäre.", 250, 0, 5);
            AddItem("structures_level8_uncommon", "Ringworld Habitat", 8, "structures", ItemRarity.Uncommon, 
                "Ein Ringwelt-Habitat.", 100, 0, 2);
            AddItem("structures_level9_common", "Stellar Forge", 9, "structures", ItemRarity.Common, 
                "Schmiedet Sterne.", 50, 0, 1);
            AddItem("structures_level10_epic", "Galactic Foundry", 10, "structures", ItemRarity.Epic, 
                "Eine galaktische Gießerei.", 500, 0, 10);
            AddItem("structures_level11_rare", "Mega-Transmitter", 11, "structures", ItemRarity.Rare, 
                "Ein Mega-Transmitter.", 250, 0, 5);
            AddItem("structures_level12_uncommon", "Hyperspace Gate", 12, "structures", ItemRarity.Uncommon, 
                "Ein Hyperraum-Tor.", 100, 0, 2);
            AddItem("structures_level13_common", "Matter Converter", 13, "structures", ItemRarity.Common, 
                "Ein Materie-Konverter.", 50, 0, 1);
            AddItem("structures_level14_epic", "Void Factory", 14, "structures", ItemRarity.Epic, 
                "Eine Leere-Fabrik.", 500, 0, 10);
            AddItem("structures_level15_legendary", "Reality Engine", 15, "structures", ItemRarity.Legendary, 
                "Eine Realitäts-Maschine.", 1000, 0, 20);
            AddItem("structures_level16_mythic", "Omega Station", 16, "structures", ItemRarity.Mythic, 
                "Eine Omega-Station.", 2500, 0, 50);
            AddItem("structures_level17_rare", "Null-Point Generator", 17, "structures", ItemRarity.Rare, 
                "Ein Nullpunkt-Generator.", 250, 0, 5);
            AddItem("structures_level18_uncommon", "Temporal Accelerator", 18, "structures", ItemRarity.Uncommon, 
                "Ein Zeitbeschleuniger.", 100, 0, 2);
            AddItem("structures_level19_common", "Singularity Forge", 19, "structures", ItemRarity.Common, 
                "Eine Singularitäts-Schmiede.", 50, 0, 1);
            AddItem("structures_level20_epic", "Cosmic Ascension Core", 20, "structures", ItemRarity.Epic, 
                "Ein kosmischer Aufstiegs-Kern.", 500, 0, 10);
        }

        private void InitializeLifeforms125()
        {
            // Items 46-63: Lifeforms Chain (Level 1-18)
            AddItem("lifeforms_level1_common", "Microbe Spore", 1, "lifeforms", ItemRarity.Common, 
                "Ein winziger Lebenskeim.", 50, 0, 1);
            AddItem("lifeforms_level2_uncommon", "Sentient Crystal", 2, "lifeforms", ItemRarity.Uncommon, 
                "Ein bewusster Kristall.", 100, 0, 2);
            AddItem("lifeforms_level3_rare", "Proto-Cell", 3, "lifeforms", ItemRarity.Rare, 
                "Eine Urzelle.", 250, 0, 5);
            AddItem("lifeforms_level4_epic", "Multicellular Organism", 4, "lifeforms", ItemRarity.Epic, 
                "Ein mehrzelliger Organismus.", 500, 0, 10);
            AddItem("lifeforms_level5_legendary", "Aquatic Lifeform", 5, "lifeforms", ItemRarity.Legendary, 
                "Eine aquatische Lebensform.", 1000, 0, 20);
            AddItem("lifeforms_level6_mythic", "Terrestrial Plant", 6, "lifeforms", ItemRarity.Mythic, 
                "Eine terrestrische Pflanze.", 2500, 0, 50);
            AddItem("lifeforms_level7_rare", "Invertebrate", 7, "lifeforms", ItemRarity.Rare, 
                "Ein Wirbelloses.", 250, 0, 5);
            AddItem("lifeforms_level8_uncommon", "Vertebrate", 8, "lifeforms", ItemRarity.Uncommon, 
                "Ein Wirbeltier.", 100, 0, 2);
            AddItem("lifeforms_level9_common", "Sentient Being", 9, "lifeforms", ItemRarity.Common, 
                "Ein bewusstes Wesen.", 50, 0, 1);
            AddItem("lifeforms_level10_epic", "Hive Mind", 10, "lifeforms", ItemRarity.Epic, 
                "Ein Schwarmbewusstsein.", 500, 0, 10);
            AddItem("lifeforms_level11_rare", "Cosmic Guardian", 11, "lifeforms", ItemRarity.Rare, 
                "Ein kosmischer Wächter.", 250, 0, 5);
            AddItem("lifeforms_level12_uncommon", "Ethereal Being", 12, "lifeforms", ItemRarity.Uncommon, 
                "Ein ätherisches Wesen.", 100, 0, 2);
            AddItem("lifeforms_level13_common", "Star Whale", 13, "lifeforms", ItemRarity.Common, 
                "Ein Wal, der durch Sterne schwimmt.", 50, 0, 1);
            AddItem("lifeforms_level14_epic", "Void Dragon", 14, "lifeforms", ItemRarity.Epic, 
                "Ein Drache der Leere.", 500, 0, 10);
            AddItem("lifeforms_level15_legendary", "Galactic Phoenix", 15, "lifeforms", ItemRarity.Legendary, 
                "Ein galaktischer Phönix.", 1000, 0, 20);
            AddItem("lifeforms_level16_mythic", "Quantum Collective", 16, "lifeforms", ItemRarity.Mythic, 
                "Ein Quantenkollektiv.", 2500, 0, 50);
            AddItem("lifeforms_level17_rare", "Time Weaver", 17, "lifeforms", ItemRarity.Rare, 
                "Ein Zeitweber.", 250, 0, 5);
            AddItem("lifeforms_level18_uncommon", "Reality Shaper", 18, "lifeforms", ItemRarity.Uncommon, 
                "Ein Realitätsformer.", 100, 0, 2);
        }

        private void InitializeArtifacts125()
        {
            // Items 64-80: Artifacts Chain (Level 1-15)
            AddItem("artifacts_level1_common", "Ancient Rune", 1, "artifacts", ItemRarity.Common, 
                "Eine uralte Rune.", 50, 0, 1);
            AddItem("artifacts_level2_uncommon", "Glowing Totem", 2, "artifacts", ItemRarity.Uncommon, 
                "Ein leuchtender Totem.", 100, 0, 2);
            AddItem("artifacts_level3_rare", "Crystal Obelisk", 3, "artifacts", ItemRarity.Rare, 
                "Ein Kristall-Obelisk.", 250, 0, 5);
            AddItem("artifacts_level4_epic", "Star Map", 4, "artifacts", ItemRarity.Epic, 
                "Eine Sternenkarte.", 500, 0, 10);
            AddItem("artifacts_level5_legendary", "Void Compass", 5, "artifacts", ItemRarity.Legendary, 
                "Ein Kompass der Leere.", 1000, 0, 20);
            AddItem("artifacts_level6_mythic", "Eternity Dial", 6, "artifacts", ItemRarity.Mythic, 
                "Ein Zifferblatt der Ewigkeit.", 2500, 0, 50);
            AddItem("artifacts_level7_rare", "Fate Weaver", 7, "artifacts", ItemRarity.Rare, 
                "Ein Schicksalsweber.", 250, 0, 5);
            AddItem("artifacts_level8_uncommon", "Reality Shard", 8, "artifacts", ItemRarity.Uncommon, 
                "Ein Splitter der Realität.", 100, 0, 2);
            AddItem("artifacts_level9_common", "Time Crystal", 9, "artifacts", ItemRarity.Common, 
                "Ein Kristall der Zeit.", 50, 0, 1);
            AddItem("artifacts_level10_epic", "Dimension Key", 10, "artifacts", ItemRarity.Epic, 
                "Ein Dimensionsschlüssel.", 500, 0, 10);
            AddItem("artifacts_level11_rare", "Infinity Stone", 11, "artifacts", ItemRarity.Rare, 
                "Ein Stein der Unendlichkeit.", 250, 0, 5);
            AddItem("artifacts_level12_uncommon", "Omniscience Orb", 12, "artifacts", ItemRarity.Uncommon, 
                "Eine Kugel der Allwissenheit.", 100, 0, 2);
            AddItem("artifacts_level13_common", "Chaos Vortex", 13, "artifacts", ItemRarity.Common, 
                "Ein Chaos-Wirbel.", 50, 0, 1);
            AddItem("artifacts_level14_epic", "Fate Seal", 14, "artifacts", ItemRarity.Epic, 
                "Ein Schicksalssiegel.", 500, 0, 10);
            AddItem("artifacts_level15_legendary", "Genesis Cradle", 15, "artifacts", ItemRarity.Legendary, 
                "Eine Genesis-Wiege.", 1000, 0, 20);
        }

        private void InitializeElements125()
        {
            // Items 81-97: Elements (Level 1-5, verschiedene Chains)
            // Fire Chain
            AddItem("elements_fire_level1_common", "Fire Chain", 1, "elements", ItemRarity.Common, 
                "Eine Feuerkette.", 50, 0, 1);
            
            // Water Chain
            AddItem("elements_water_level1_uncommon", "Water Vortex", 1, "elements", ItemRarity.Uncommon, 
                "Ein Wasserwirbel.", 100, 0, 2);
            
            // Wind Chain
            AddItem("elements_wind_level1_rare", "Wind Spiral", 1, "elements", ItemRarity.Rare, 
                "Eine Windspirale.", 250, 0, 5);
            
            // Earth Chain
            AddItem("elements_earth_level1_epic", "Earth Core", 1, "elements", ItemRarity.Epic, 
                "Ein Erdkern.", 500, 0, 10);
            
            // Level 2 Combinations
            AddItem("elements_level2_legendary", "Plasma Bolt", 2, "elements", ItemRarity.Legendary, 
                "Ein Plasmabolt.", 1000, 0, 20);
            AddItem("elements_level2_mythic", "Ice Shard", 2, "elements", ItemRarity.Mythic, 
                "Ein Eissplitter.", 2500, 0, 50);
            AddItem("elements_level2_rare", "Lightning Arc", 2, "elements", ItemRarity.Rare, 
                "Ein Blitzbogen.", 250, 0, 5);
            AddItem("elements_level2_uncommon", "Crystal Lattice", 2, "elements", ItemRarity.Uncommon, 
                "Ein Kristallgitter.", 100, 0, 2);
            
            // Level 3
            AddItem("elements_level3_common", "Lava Flow", 3, "elements", ItemRarity.Common, 
                "Ein Lavastrom.", 50, 0, 1);
            AddItem("elements_level3_epic", "Tidal Wave", 3, "elements", ItemRarity.Epic, 
                "Eine Flutwelle.", 500, 0, 10);
            AddItem("elements_level3_legendary", "Hurricane", 3, "elements", ItemRarity.Legendary, 
                "Ein Hurrikan.", 1000, 0, 20);
            AddItem("elements_level3_mythic", "Earthquake", 3, "elements", ItemRarity.Mythic, 
                "Ein Erdbeben.", 2500, 0, 50);
            
            // Level 4
            AddItem("elements_level4_rare", "Solar Flare", 4, "elements", ItemRarity.Rare, 
                "Ein Sonnenflare.", 250, 0, 5);
            AddItem("elements_level4_uncommon", "Meteor Shower", 4, "elements", ItemRarity.Uncommon, 
                "Ein Meteoritenschauer.", 100, 0, 2);
            AddItem("elements_level4_common", "Aurora", 4, "elements", ItemRarity.Common, 
                "Eine Aurora.", 50, 0, 1);
            
            // Level 5
            AddItem("elements_level5_epic", "Black Hole Mini", 5, "elements", ItemRarity.Epic, 
                "Ein Mini-Schwarzes-Loch.", 500, 0, 10);
            
            // Level 6+ (fehlende Items für Merging)
            AddItem("elements_level6_epic", "Quantum Singularity", 6, "elements", ItemRarity.Epic, 
                "Eine Quantensingularität.", 1000, 0, 20);
            AddItem("elements_level6_common", "Nebula Core", 6, "elements", ItemRarity.Common, 
                "Ein Nebelkern.", 200, 0, 4);
            AddItem("elements_level6_rare", "Void Essence", 6, "elements", ItemRarity.Rare, 
                "Leere-Essenz.", 500, 0, 10);
            AddItem("elements_level6_legendary", "Cosmic Storm", 6, "elements", ItemRarity.Legendary, 
                "Ein kosmischer Sturm.", 2000, 0, 40);
            
            // Level 7+
            AddItem("elements_level7_epic", "Galactic Nucleus", 7, "elements", ItemRarity.Epic, 
                "Ein galaktischer Kern.", 2000, 0, 40);
            AddItem("elements_level7_common", "Stellar Fragment", 7, "elements", ItemRarity.Common, 
                "Ein Sternenfragment.", 400, 0, 8);
            AddItem("elements_level7_rare", "Plasma Cascade", 7, "elements", ItemRarity.Rare, 
                "Eine Plasmakaskade.", 1000, 0, 20);
            
            // Level 8+
            AddItem("elements_level8_epic", "Event Horizon", 8, "elements", ItemRarity.Epic, 
                "Ein Ereignishorizont.", 4000, 0, 80);
            AddItem("elements_level8_common", "Light Echo", 8, "elements", ItemRarity.Common, 
                "Ein Lichtecho.", 800, 0, 16);
            
            // Level 9+
            AddItem("elements_level9_epic", "Supernova Remnant", 9, "elements", ItemRarity.Epic, 
                "Ein Supernova-Überrest.", 8000, 0, 160);
            AddItem("elements_level9_common", "Cosmic Dust Cloud", 9, "elements", ItemRarity.Common, 
                "Eine kosmische Staubwolke.", 1600, 0, 32);
            
            // Level 10+
            AddItem("elements_level10_epic", "Wormhole Gateway", 10, "elements", ItemRarity.Epic, 
                "Ein Wurmloch-Portal.", 16000, 0, 320);
        }

        private void InitializeDecorations125()
        {
            // Items 98-115: Decorations Chain (Level 1-15)
            AddItem("decorations_level1_legendary", "Nebula Cloud", 1, "decorations", ItemRarity.Legendary, 
                "Eine Nebelwolke.", 1000, 0, 20);
            AddItem("decorations_level2_uncommon", "Stardust Fountain", 2, "decorations", ItemRarity.Uncommon, 
                "Ein Brunnen aus Sternenstaub.", 100, 0, 2);
            AddItem("decorations_level3_rare", "Crystal Garden", 3, "decorations", ItemRarity.Rare, 
                "Ein Garten aus Kristallen.", 250, 0, 5);
            AddItem("decorations_level4_epic", "Aurora Veil", 4, "decorations", ItemRarity.Epic, 
                "Ein Auroraschleier.", 500, 0, 10);
            AddItem("decorations_level5_legendary", "Comet Tail", 5, "decorations", ItemRarity.Legendary, 
                "Ein Kometenschweif.", 1000, 0, 20);
            AddItem("decorations_level6_mythic", "Galaxy Spiral", 6, "decorations", ItemRarity.Mythic, 
                "Eine Galaxienspirale.", 2500, 0, 50);
            AddItem("decorations_level7_rare", "Pulsar Ring", 7, "decorations", ItemRarity.Rare, 
                "Ein Pulsarring.", 250, 0, 5);
            AddItem("decorations_level8_uncommon", "Void Rift", 8, "decorations", ItemRarity.Uncommon, 
                "Ein Leere-Riss.", 100, 0, 2);
            AddItem("decorations_level9_common", "Meteor Field", 9, "decorations", ItemRarity.Common, 
                "Ein Meteoritenfeld.", 50, 0, 1);
            AddItem("decorations_level10_epic", "Supernova Burst", 10, "decorations", ItemRarity.Epic, 
                "Ein Supernova-Ausbruch.", 500, 0, 10);
            AddItem("decorations_level11_legendary", "Wormhole Arc", 11, "decorations", ItemRarity.Legendary, 
                "Ein Wurmlochbogen.", 1000, 0, 20);
            AddItem("decorations_level12_mythic", "Starfield", 12, "decorations", ItemRarity.Mythic, 
                "Ein Sternenfeld.", 2500, 0, 50);
            AddItem("decorations_level13_rare", "Cosmic Dust", 13, "decorations", ItemRarity.Rare, 
                "Kosmischer Staub.", 250, 0, 5);
            AddItem("decorations_level14_uncommon", "Light Beam", 14, "decorations", ItemRarity.Uncommon, 
                "Ein Lichtstrahl.", 100, 0, 2);
            AddItem("decorations_level15_common", "Void Essence", 15, "decorations", ItemRarity.Common, 
                "Leere-Essenz.", 50, 0, 1);
        }

        #endregion

        /// <summary>
        /// Gibt alle Items einer Kategorie zurück
        /// </summary>
        public List<CelestialItem> GetItemsByCategory(string category)
        {
            List<CelestialItem> result = new List<CelestialItem>();
            if (itemsByCategory != null && itemsByCategory.TryGetValue(category, out List<ItemData> categoryItems))
            {
                foreach (var itemData in categoryItems)
                {
                    result.Add(CreateItem(itemData.itemId));
                }
            }
            return result;
        }

        public bool ItemExists(string itemId)
        {
            if (itemLookup == null) BuildLookup();
            return itemLookup.ContainsKey(itemId);
        }
    }
}
