using UnityEngine;
using System.Collections.Generic;

namespace MergeWellness
{
    /// <summary>
    /// Helper-Script zum Erstellen und Spawnen von Items für Tests und Gameplay
    /// </summary>
    public class ItemSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private ItemDatabase itemDatabase;

        [Header("Spawn Settings")]
        [SerializeField] private bool spawnOnStart = false;
        [SerializeField] private int itemsToSpawnOnStart = 3;

        private void Start()
        {
            if (gridManager == null)
            {
                gridManager = FindFirstObjectByType<GridManager>();
            }

            if (itemDatabase == null)
            {
                itemDatabase = FindFirstObjectByType<ItemDatabase>();
                if (itemDatabase == null)
                {
                    // Versuche ScriptableObject zu finden
                    itemDatabase = Resources.FindObjectsOfTypeAll<ItemDatabase>()[0];
                }
            }

            if (spawnOnStart && gridManager != null && itemDatabase != null)
            {
                SpawnRandomStarterItems(itemsToSpawnOnStart);
            }
        }

        /// <summary>
        /// Spawnt ein zufälliges Starter-Item (Tier 1)
        /// </summary>
        [ContextMenu("Spawn Random Starter Item")]
        public void SpawnRandomStarterItem()
        {
            if (gridManager == null || itemDatabase == null)
            {
                Debug.LogError("GridManager oder ItemDatabase nicht gefunden!");
                return;
            }

            List<string> starterIds = itemDatabase.GetStarterItemIds();
            if (starterIds.Count == 0)
            {
                Debug.LogError("Keine Starter-Items in der Datenbank gefunden!");
                return;
            }

            int randomIndex = Random.Range(0, starterIds.Count);
            string itemId = starterIds[randomIndex];
            WellnessItem item = itemDatabase.CreateItem(itemId);

            if (item != null)
            {
                bool added = gridManager.AddItemToGrid(item);
                if (added)
                {
                    Debug.Log($"✓ Item gespawnt: {item.ItemName}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Grid ist voll! Item konnte nicht hinzugefügt werden: {item.ItemName}");
                }
            }
        }

        /// <summary>
        /// Spawnt mehrere zufällige Starter-Items
        /// </summary>
        public void SpawnRandomStarterItems(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnRandomStarterItem();
            }
        }

        /// <summary>
        /// Spawnt ein spezifisches Item nach ID
        /// </summary>
        public void SpawnItemById(string itemId)
        {
            if (gridManager == null || itemDatabase == null)
            {
                Debug.LogError("GridManager oder ItemDatabase nicht gefunden!");
                return;
            }

            WellnessItem item = itemDatabase.CreateItem(itemId);
            if (item != null)
            {
                bool added = gridManager.AddItemToGrid(item);
                if (added)
                {
                    Debug.Log($"✓ Item gespawnt: {item.ItemName}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Grid ist voll! Item konnte nicht hinzugefügt werden: {item.ItemName}");
                }
            }
            else
            {
                Debug.LogError($"Item mit ID '{itemId}' nicht gefunden!");
            }
        }

        /// <summary>
        /// Spawnt ein Item eines bestimmten Typs und Tiers
        /// </summary>
        public void SpawnItemByTypeAndTier(string itemType, int tier)
        {
            string itemId = $"{itemType}_tier{tier}";
            SpawnItemById(itemId);
        }

        /// <summary>
        /// Füllt das Grid mit zufälligen Starter-Items (bis Grid voll ist)
        /// </summary>
        [ContextMenu("Fill Grid with Random Items")]
        public void FillGridWithRandomItems()
        {
            if (gridManager == null)
            {
                Debug.LogError("GridManager nicht gefunden!");
                return;
            }

            int freeSlots = gridManager.GetFreeSlotCount();
            SpawnRandomStarterItems(freeSlots);
        }

        /// <summary>
        /// Spawnt Items für Merge-Test (2 gleiche Items)
        /// </summary>
        [ContextMenu("Spawn Merge Test Items")]
        public void SpawnMergeTestItems()
        {
            // Spawnt 2x Yoga Mat (für Merge-Test)
            SpawnItemById("yoga_mat_tier1");
            SpawnItemById("yoga_mat_tier1");
            Debug.Log("✓ Merge-Test Items gespawnt (2x Yoga Mat)");
        }
    }
}
