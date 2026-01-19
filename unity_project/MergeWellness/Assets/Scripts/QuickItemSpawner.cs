using UnityEngine;

namespace MergeWellness
{
    /// <summary>
    /// Einfacher Item-Spawner für schnelle Tests - Funktioniert auch ohne Referenzen
    /// </summary>
    public class QuickItemSpawner : MonoBehaviour
    {
        [ContextMenu("Spawn Item - Quick Test")]
        public void SpawnItemQuickTest()
        {
            Debug.Log("=== Quick Item Spawn Test ===");

            // Finde alle notwendigen Komponenten
            GridManager gridManager = FindFirstObjectByType<GridManager>();
            ItemDatabase itemDatabase = FindFirstObjectByType<ItemDatabase>();

            if (itemDatabase == null)
            {
                // Versuche ScriptableObject zu finden
                ItemDatabase[] databases = Resources.FindObjectsOfTypeAll<ItemDatabase>();
                if (databases.Length > 0)
                {
                    itemDatabase = databases[0];
                    Debug.Log($"✓ ItemDatabase gefunden: {itemDatabase.name}");
                }
                else
                {
                    Debug.LogError("❌ ItemDatabase nicht gefunden! Stelle sicher, dass es erstellt und initialisiert ist.");
                    return;
                }
            }

            if (gridManager == null)
            {
                Debug.LogError("❌ GridManager nicht gefunden!");
                return;
            }

            // Prüfe ob Grid voll ist
            if (gridManager.IsGridFull())
            {
                Debug.LogWarning("⚠️ Grid ist voll! Entferne zuerst Items.");
                return;
            }

            // Hole Starter-Items
            var starterIds = itemDatabase.GetStarterItemIds();
            if (starterIds == null || starterIds.Count == 0)
            {
                Debug.LogError("❌ Keine Starter-Items gefunden! Initialisiere die ItemDatabase.");
                return;
            }

            Debug.Log($"✓ {starterIds.Count} Starter-Items gefunden");

            // Spawne zufälliges Item
            int randomIndex = Random.Range(0, starterIds.Count);
            string itemId = starterIds[randomIndex];
            Debug.Log($"Versuche Item zu erstellen: {itemId}");

            WellnessItem item = itemDatabase.CreateItem(itemId);
            if (item == null)
            {
                Debug.LogError($"❌ Item konnte nicht erstellt werden: {itemId}");
                return;
            }

            Debug.Log($"✓ Item erstellt: {item.ItemName} (Tier {item.Tier})");

            // Füge zum Grid hinzu
            bool added = gridManager.AddItemToGrid(item);
            if (added)
            {
                Debug.Log($"✅ Item erfolgreich zum Grid hinzugefügt: {item.ItemName}");
            }
            else
            {
                Debug.LogError($"❌ Item konnte nicht zum Grid hinzugefügt werden: {item.ItemName}");
            }
        }

        [ContextMenu("Spawn 3 Items - Quick Test")]
        public void Spawn3ItemsQuickTest()
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnItemQuickTest();
            }
        }

        [ContextMenu("Spawn Merge Test (2x Yoga Mat)")]
        public void SpawnMergeTest()
        {
            GridManager gridManager = FindFirstObjectByType<GridManager>();
            ItemDatabase itemDatabase = FindFirstObjectByType<ItemDatabase>();

            if (itemDatabase == null)
            {
                ItemDatabase[] databases = Resources.FindObjectsOfTypeAll<ItemDatabase>();
                if (databases.Length > 0)
                {
                    itemDatabase = databases[0];
                }
                else
                {
                    Debug.LogError("❌ ItemDatabase nicht gefunden!");
                    return;
                }
            }

            if (gridManager == null)
            {
                Debug.LogError("❌ GridManager nicht gefunden!");
                return;
            }

            // Spawne 2x Yoga Mat
            WellnessItem item1 = itemDatabase.CreateItem("yoga_mat_tier1");
            WellnessItem item2 = itemDatabase.CreateItem("yoga_mat_tier1");

            if (item1 != null && item2 != null)
            {
                gridManager.AddItemToGrid(item1);
                gridManager.AddItemToGrid(item2);
                Debug.Log("✅ 2x Yoga Mat gespawnt - Bereit zum Mergen!");
            }
        }

        private void Update()
        {
            // Keyboard Shortcuts für schnelles Testen
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnItemQuickTest();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                SpawnMergeTest();
            }
        }
    }
}
