using System.Collections.Generic;
using UnityEngine;

namespace MergeWellness
{
    /// <summary>
    /// Datenbank für alle verfügbaren Wellness-Items (Tier 1-10)
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "MergeWellness/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        [System.Serializable]
        public class ItemData
        {
            public string itemId;
            public string itemName;
            public int tier;
            public string itemType;
            public string wellnessFact;
            public Sprite itemSprite;
        }

        [SerializeField] private List<ItemData> items = new List<ItemData>();

        private Dictionary<string, ItemData> itemLookup;

        private void OnEnable()
        {
            BuildLookup();
        }

        private void BuildLookup()
        {
            itemLookup = new Dictionary<string, ItemData>();
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.itemId))
                {
                    itemLookup[item.itemId] = item;
                }
            }
        }

        /// <summary>
        /// Erstellt ein WellnessItem aus der Datenbank
        /// </summary>
        public WellnessItem CreateItem(string itemId)
        {
            if (itemLookup == null) BuildLookup();

            if (itemLookup.TryGetValue(itemId, out ItemData data))
            {
                WellnessItem item = new WellnessItem(
                    data.itemId,
                    data.itemName,
                    data.tier,
                    data.itemType,
                    data.wellnessFact
                );
                return item;
            }

            Debug.LogError($"Item mit ID '{itemId}' nicht gefunden!");
            return null;
        }

        /// <summary>
        /// Gibt alle Starter-Items zurück (Tier 1)
        /// </summary>
        public List<string> GetStarterItemIds()
        {
            List<string> starterIds = new List<string>();
            foreach (var item in items)
            {
                if (item.tier == 1)
                {
                    starterIds.Add(item.itemId);
                }
            }
            return starterIds;
        }

        /// <summary>
        /// Gibt das gemergte Item zurück (Tier + 1, gleicher Typ)
        /// </summary>
        public string GetMergedItemId(WellnessItem item1, WellnessItem item2)
        {
            if (!item1.CanMergeWith(item2))
            {
                Debug.LogError($"Items können nicht gemerged werden! Item1: {item1.ItemName} (Tier {item1.Tier}, Type: {item1.ItemType}), Item2: {item2.ItemName} (Tier {item2.Tier}, Type: {item2.ItemType})");
                return null;
            }

            int nextTier = item1.Tier + 1;
            
            // Versuche verschiedene ID-Formate
            string[] possibleIds = {
                $"{item1.ItemType}_tier{nextTier}",  // Format: yoga_tier2, herbal_tier2
                $"{item1.ItemType}{nextTier}",       // Format: yoga2, herbal2
                $"{item1.ItemType}_{nextTier}"       // Format: yoga_2, herbal_2
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

            // Fallback: Suche nach Item mit gleichem Typ und nächsthöherem Tier
            foreach (var itemData in items)
            {
                if (itemData.itemType == item1.ItemType && itemData.tier == nextTier)
                {
                    Debug.Log($"✓ Merged Item gefunden (Fallback): {itemData.itemId}");
                    return itemData.itemId;
                }
            }

            Debug.LogError($"Merged Item nicht gefunden! Item1: {item1.ItemId} (Tier {item1.Tier}, Type: {item1.ItemType}) → Erwartet: Tier {nextTier}, Type: {item1.ItemType}");
            Debug.LogError($"Versuchte IDs: {string.Join(", ", possibleIds)}");
            return null;
        }

        /// <summary>
        /// Initialisiert die Standard-Items (kann auch aus JSON geladen werden)
        /// </summary>
        [ContextMenu("Initialize Default Items")]
        public void InitializeDefaultItems()
        {
            items.Clear();

            // Tier 1 - Starter Items (konsistentes Format: {type}_tier{tier})
            AddItem("yoga_tier1", "Yoga Mat", 1, "yoga", "Yoga verbessert Flexibilität und reduziert Stress.");
            AddItem("meditation_tier1", "Meditation Stone", 1, "meditation", "Meditation reduziert nachweislich Angstzustände.");
            AddItem("herbal_tier1", "Herbal Tea", 1, "herbal", "Kräutertee kann Entzündungen reduzieren.");

            // Tier 2
            AddItem("yoga_tier2", "Meditation Space", 2, "yoga", "Ein ruhiger Raum verbessert die Meditation.");
            AddItem("meditation_tier2", "Wellness Kit", 2, "meditation", "Wellness-Routinen stärken das Immunsystem.");
            AddItem("herbal_tier2", "Herbal Tea Blend", 2, "herbal", "Kräutermischungen haben synergetische Effekte.");

            // Tier 3
            AddItem("yoga_tier3", "Yoga Studio", 3, "yoga", "Regelmäßiges Yoga verbessert die Herzgesundheit.");
            AddItem("meditation_tier3", "Healing Garden", 3, "meditation", "Naturkontakt reduziert Cortisol-Level.");
            AddItem("herbal_tier3", "Premium Wellness Retreat", 3, "herbal", "Auszeiten fördern mentale Gesundheit.");

            // Tier 4
            AddItem("yoga_tier4", "Advanced Yoga Center", 4, "yoga", "Fortgeschrittenes Yoga fördert mentale Klarheit.");
            AddItem("meditation_tier4", "Zen Sanctuary", 4, "meditation", "Meditation verbessert die Konzentration.");
            AddItem("herbal_tier4", "Master Herbal Blend", 4, "herbal", "Premium-Kräuter unterstützen die Gesundheit.");

            // Tier 5
            AddItem("yoga_tier5", "Master Yoga Temple", 5, "yoga", "Meister-Yoga führt zu innerem Frieden.");
            AddItem("meditation_tier5", "Enlightenment Garden", 5, "meditation", "Tiefe Meditation bringt spirituelle Erfüllung.");
            AddItem("herbal_tier5", "Ultimate Wellness Elixir", 5, "herbal", "Das ultimative Wellness-Elixir für vollkommene Gesundheit.");

            // Weitere Tiers können hier hinzugefügt werden...
            BuildLookup();
            Debug.Log($"✓ ItemDatabase initialisiert: {items.Count} Items geladen");
        }

        private void AddItem(string id, string name, int tier, string type, string fact)
        {
            items.Add(new ItemData
            {
                itemId = id,
                itemName = name,
                tier = tier,
                itemType = type,
                wellnessFact = fact
            });
        }

        /// <summary>
        /// Validiert die Datenbank und prüft ob alle Merge-Ketten vollständig sind
        /// </summary>
        [ContextMenu("Validate Database")]
        public void ValidateDatabase()
        {
            if (itemLookup == null) BuildLookup();

            Debug.Log("=== ItemDatabase Validierung ===");
            Debug.Log($"Gesamt Items: {items.Count}");

            // Gruppiere Items nach Typ
            Dictionary<string, List<ItemData>> itemsByType = new Dictionary<string, List<ItemData>>();
            foreach (var item in items)
            {
                if (!itemsByType.ContainsKey(item.itemType))
                {
                    itemsByType[item.itemType] = new List<ItemData>();
                }
                itemsByType[item.itemType].Add(item);
            }

            Debug.Log($"Item-Typen: {itemsByType.Count}");

            // Prüfe jede Merge-Kette
            int missingItems = 0;
            foreach (var typeGroup in itemsByType)
            {
                string type = typeGroup.Key;
                var typeItems = typeGroup.Value;
                typeItems.Sort((a, b) => a.tier.CompareTo(b.tier));

                Debug.Log($"\nTyp '{type}': {typeItems.Count} Items");
                for (int i = 0; i < typeItems.Count; i++)
                {
                    int currentTier = typeItems[i].tier;
                    int nextTier = currentTier + 1;
                    string expectedNextId = $"{type}_tier{nextTier}";

                    bool hasNextTier = false;
                    foreach (var item in typeItems)
                    {
                        if (item.tier == nextTier)
                        {
                            hasNextTier = true;
                            break;
                        }
                    }

                    if (!hasNextTier && currentTier < 10) // Max Tier 10
                    {
                        Debug.LogWarning($"  ⚠️ Tier {currentTier} → Tier {nextTier} fehlt! (Erwartet: {expectedNextId})");
                        missingItems++;
                    }
                    else if (hasNextTier)
                    {
                        Debug.Log($"  ✓ Tier {currentTier} → Tier {nextTier} vorhanden");
                    }
                }
            }

            if (missingItems == 0)
            {
                Debug.Log("\n✅ Datenbank-Validierung erfolgreich! Alle Merge-Ketten sind vollständig.");
            }
            else
            {
                Debug.LogWarning($"\n⚠️ Datenbank-Validierung: {missingItems} fehlende Items gefunden!");
            }
        }

        /// <summary>
        /// Prüft ob ein Item mit der gegebenen ID existiert
        /// </summary>
        public bool ItemExists(string itemId)
        {
            if (itemLookup == null) BuildLookup();
            return itemLookup.ContainsKey(itemId);
        }
    }
}
