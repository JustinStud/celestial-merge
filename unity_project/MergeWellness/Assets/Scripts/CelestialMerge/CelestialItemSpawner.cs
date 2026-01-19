using UnityEngine;

namespace CelestialMerge
{
    /// <summary>
    /// Item-Spawner für Celestial Merge System
    /// </summary>
    public class CelestialItemSpawner : MonoBehaviour
    {
        [ContextMenu("Spawn Celestial Item - Quick Test")]
        public void SpawnCelestialItemQuickTest()
        {
            Debug.Log("=== Celestial Item Spawn Test ===");

            // Finde neue Systeme
            ExpandableBoardManager boardManager = FindFirstObjectByType<ExpandableBoardManager>();
            CelestialItemDatabase itemDatabase = FindFirstObjectByType<CelestialItemDatabase>();

            if (itemDatabase == null)
            {
                // Versuche ScriptableObject zu finden
                CelestialItemDatabase[] databases = Resources.FindObjectsOfTypeAll<CelestialItemDatabase>();
                if (databases.Length > 0)
                {
                    itemDatabase = databases[0];
                    Debug.Log($"✓ CelestialItemDatabase gefunden: {itemDatabase.name}");
                }
                else
                {
                    Debug.LogError("❌ CelestialItemDatabase nicht gefunden! Erstelle Asset im Project-Fenster.");
                    return;
                }
            }

            if (boardManager == null)
            {
                Debug.LogError("❌ ExpandableBoardManager nicht gefunden!");
                return;
            }

            // Prüfe ob Board voll ist
            if (boardManager.IsBoardFull())
            {
                Debug.LogWarning("⚠️ Board ist voll! Entferne zuerst Items.");
                return;
            }

            // Hole Starter-Items
            var starterIds = itemDatabase.GetStarterItemIds();
            if (starterIds == null || starterIds.Count == 0)
            {
                Debug.LogError("❌ Keine Starter-Items gefunden! Initialisiere die CelestialItemDatabase.");
                return;
            }

            Debug.Log($"✓ {starterIds.Count} Starter-Items gefunden");

            // Spawne zufälliges Item
            int randomIndex = Random.Range(0, starterIds.Count);
            string itemId = starterIds[randomIndex];
            Debug.Log($"Versuche Item zu erstellen: {itemId}");

            CelestialItem item = itemDatabase.CreateItem(itemId);
            if (item == null)
            {
                Debug.LogError($"❌ Item konnte nicht erstellt werden: {itemId}");
                return;
            }

            Debug.Log($"✓ Item erstellt: {item.ItemName} (Level {item.Level})");

            // Füge zum Board hinzu
            CelestialBoardSlot freeSlot = boardManager.GetFreeSlot();
            if (freeSlot != null)
            {
                freeSlot.SetItem(item);
                Debug.Log($"✅ Item erfolgreich zum Board hinzugefügt: {item.ItemName}");
            }
            else
            {
                Debug.LogError($"❌ Kein freier Slot gefunden!");
            }
        }

        [ContextMenu("Spawn 3 Starter Items")]
        public void Spawn3StarterItems()
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnCelestialItemQuickTest();
            }
        }

        [ContextMenu("Spawn Merge Test (2x Stardust Particle)")]
        public void SpawnMergeTest()
        {
            ExpandableBoardManager boardManager = FindFirstObjectByType<ExpandableBoardManager>();
            CelestialItemDatabase itemDatabase = FindFirstObjectByType<CelestialItemDatabase>();

            if (itemDatabase == null)
            {
                CelestialItemDatabase[] databases = Resources.FindObjectsOfTypeAll<CelestialItemDatabase>();
                if (databases.Length > 0)
                {
                    itemDatabase = databases[0];
                }
                else
                {
                    Debug.LogError("❌ CelestialItemDatabase nicht gefunden!");
                    return;
                }
            }

            if (boardManager == null)
            {
                Debug.LogError("❌ ExpandableBoardManager nicht gefunden!");
                return;
            }

            // Spawne 2x Stardust Particle (Level 1)
            CelestialItem item1 = itemDatabase.CreateItem("celestial_bodies_level1_common");
            CelestialItem item2 = itemDatabase.CreateItem("celestial_bodies_level1_common");

            if (item1 != null && item2 != null)
            {
                boardManager.AddItemToBoard(item1);
                boardManager.AddItemToBoard(item2);
                Debug.Log("✅ 2x Stardust Particle gespawnt - Bereit zum Mergen!");
            }
        }

        private void Start()
        {
            // Spawne automatisch 3 Starter-Items beim Start
            Invoke(nameof(Spawn3StarterItems), 0.5f);
        }

        private void Update()
        {
            // Keyboard Shortcuts für schnelles Testen
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnCelestialItemQuickTest();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                SpawnMergeTest();
            }
        }
    }
}
