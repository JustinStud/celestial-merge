using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MergeWellness
{
    /// <summary>
    /// Verwaltet das 5×5 Grid-System mit Merge-Mechanik
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int gridWidth = 5;
        #pragma warning disable 0414 // gridHeight wird für zukünftige Features verwendet
        [SerializeField] private int gridHeight = 5; // Wird für zukünftige Erweiterungen verwendet (z.B. dynamische Grid-Größe)
        #pragma warning restore 0414
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform gridParent;
        [SerializeField] private GridLayoutGroup gridLayout;

        [Header("Item Database")]
        [SerializeField] private ItemDatabase itemDatabase;

        [Header("Overflow Inventory")]
        [SerializeField] private Transform overflowParent;
        [SerializeField] private int maxOverflowSlots = 10;

        private GridSlot[] gridSlots;
        private List<WellnessItem> overflowItems = new List<WellnessItem>();
        private GameplayManager gameplayManager;

        private const int GRID_SIZE = 25; // 5×5

        private void Awake()
        {
            gameplayManager = FindFirstObjectByType<GameplayManager>();
            gridSlots = new GridSlot[GRID_SIZE];
        }

        private void Start()
        {
            // Validiere ItemDatabase
            if (itemDatabase == null)
            {
                itemDatabase = FindFirstObjectByType<ItemDatabase>();
                if (itemDatabase == null)
                {
                    Debug.LogError("❌ ItemDatabase nicht gefunden! Bitte im Inspector zuweisen oder in der Szene platzieren.");
                }
            }
            else
            {
                // Prüfe ob Datenbank initialisiert ist
                var starterIds = itemDatabase.GetStarterItemIds();
                if (starterIds == null || starterIds.Count == 0)
                {
                    Debug.LogWarning("⚠️ ItemDatabase ist leer! Initialisiere Standard-Items...");
                    itemDatabase.InitializeDefaultItems();
                }
            }

            InitializeGrid();
            Debug.Log($"GridManager Start abgeschlossen. GridSlots: {gridSlots?.Length ?? 0}");
        }

        private void InitializeGrid()
        {
            if (gridParent == null)
            {
                Debug.LogError("GridParent nicht gesetzt!");
                return;
            }

            // Setup GridLayoutGroup
            if (gridLayout == null)
            {
                gridLayout = gridParent.GetComponent<GridLayoutGroup>();
                if (gridLayout == null)
                {
                    gridLayout = gridParent.gameObject.AddComponent<GridLayoutGroup>();
                }
            }

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = gridWidth;
            gridLayout.cellSize = new Vector2(100, 100);
            gridLayout.spacing = new Vector2(10, 10);
            gridLayout.padding = new RectOffset(10, 10, 10, 10);

            // Erstelle Slots
            if (slotPrefab == null)
            {
                CreateDefaultSlotPrefab();
            }

            for (int i = 0; i < GRID_SIZE; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, gridParent);
                slotObj.name = $"Slot_{i}";

                GridSlot slot = slotObj.GetComponent<GridSlot>();
                if (slot == null)
                {
                    slot = slotObj.AddComponent<GridSlot>();
                }

                slot.Initialize(i, this);
                gridSlots[i] = slot;
            }

            Debug.Log($"Grid initialisiert: {GRID_SIZE} Slots erstellt");
            
            // Validierung
            int validSlots = 0;
            for (int i = 0; i < gridSlots.Length; i++)
            {
                if (gridSlots[i] != null)
                {
                    validSlots++;
                }
            }
            Debug.Log($"✓ Grid-Validierung: {validSlots}/{GRID_SIZE} Slots sind gültig");
        }

        private void CreateDefaultSlotPrefab()
        {
            // Erstelle Default-Slot Prefab zur Laufzeit
            GameObject slot = new GameObject("SlotPrefab");
            RectTransform rect = slot.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);

            Image bg = slot.AddComponent<Image>();
            bg.color = new Color(0.9f, 0.9f, 0.9f, 0.5f);

            // Item Image
            GameObject itemImgObj = new GameObject("ItemImage");
            itemImgObj.transform.SetParent(slot.transform, false);
            RectTransform itemRect = itemImgObj.AddComponent<RectTransform>();
            itemRect.anchorMin = Vector2.zero;
            itemRect.anchorMax = Vector2.one;
            itemRect.sizeDelta = Vector2.zero;
            Image itemImg = itemImgObj.AddComponent<Image>();
            itemImg.enabled = false;

            // Item Text
            GameObject itemTextObj = new GameObject("ItemText");
            itemTextObj.transform.SetParent(slot.transform, false);
            RectTransform textRect = itemTextObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            Text itemText = itemTextObj.AddComponent<Text>();
            itemText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            itemText.fontSize = 14;
            itemText.alignment = TextAnchor.MiddleCenter;
            itemText.color = Color.white;
            itemText.enabled = false;

            slotPrefab = slot;
        }

        /// <summary>
        /// Behandelt Drag-Drop zwischen Slots
        /// </summary>
        public void HandleSlotDrop(GridSlot sourceSlot, GridSlot targetSlot)
        {
            if (sourceSlot == null)
            {
                Debug.LogError("HandleSlotDrop: sourceSlot ist null!");
                return;
            }

            if (targetSlot == null)
            {
                Debug.LogError("HandleSlotDrop: targetSlot ist null!");
                return;
            }

            if (sourceSlot.IsEmpty)
            {
                Debug.LogWarning("HandleSlotDrop: sourceSlot ist leer!");
                return;
            }

            WellnessItem sourceItem = sourceSlot.CurrentItem;
            Debug.Log($"HandleSlotDrop: Slot_{sourceSlot.SlotIndex} → Slot_{targetSlot.SlotIndex}");
            Debug.Log($"  Source Item: {sourceItem?.ItemName} (Tier {sourceItem?.Tier}, Type: {sourceItem?.ItemType})");

            if (targetSlot.IsEmpty)
            {
                // Einfaches Verschieben
                Debug.Log("  → Einfaches Verschieben");
                MoveItem(sourceSlot, targetSlot);
            }
            else
            {
                // Merge-Versuch
                WellnessItem targetItem = targetSlot.CurrentItem;
                Debug.Log($"  Target Item: {targetItem?.ItemName} (Tier {targetItem?.Tier}, Type: {targetItem?.ItemType})");

                if (sourceItem.CanMergeWith(targetItem))
                {
                    Debug.Log("  → Merge möglich! Führe Merge durch...");
                    PerformMerge(sourceSlot, targetSlot);
                }
                else
                {
                    Debug.Log("  → Merge nicht möglich - Swap Items");
                    // Swap Items
                    SwapItems(sourceSlot, targetSlot);
                }
            }
        }

        private void MoveItem(GridSlot from, GridSlot to)
        {
            WellnessItem item = from.CurrentItem;
            from.ClearItem();
            to.SetItem(item);
        }

        private void SwapItems(GridSlot slot1, GridSlot slot2)
        {
            WellnessItem item1 = slot1.CurrentItem;
            WellnessItem item2 = slot2.CurrentItem;

            slot1.SetItem(item2);
            slot2.SetItem(item1);
        }

        /// <summary>
        /// Führt Merge durch: 2×N → N+1
        /// </summary>
        private void PerformMerge(GridSlot slot1, GridSlot slot2)
        {
            WellnessItem item1 = slot1.CurrentItem;
            WellnessItem item2 = slot2.CurrentItem;

            if (item1 == null || item2 == null)
            {
                Debug.LogError("PerformMerge: Eines der Items ist null!");
                return;
            }

            if (!item1.CanMergeWith(item2))
            {
                Debug.LogWarning($"Items können nicht gemerged werden: {item1.ItemName} (Tier {item1.Tier}, Type: {item1.ItemType}) + {item2.ItemName} (Tier {item2.Tier}, Type: {item2.ItemType})");
                return;
            }

            Debug.Log($"✓ Merge-Validierung erfolgreich: {item1.ItemName} + {item2.ItemName}");

            if (itemDatabase == null)
            {
                Debug.LogError("PerformMerge: ItemDatabase ist null!");
                return;
            }

            // Erstelle gemergtes Item
            string mergedItemId = itemDatabase.GetMergedItemId(item1, item2);
            if (string.IsNullOrEmpty(mergedItemId))
            {
                Debug.LogError($"❌ Merged Item konnte nicht erstellt werden!");
                Debug.LogError($"   Item1: {item1.ItemId} (Tier {item1.Tier}, Type: {item1.ItemType})");
                Debug.LogError($"   Item2: {item2.ItemId} (Tier {item2.Tier}, Type: {item2.ItemType})");
                Debug.LogError($"   Erwartete ID: {item1.ItemType}_tier{item1.Tier + 1}");
                Debug.LogError($"   Bitte stelle sicher, dass das Item in der ItemDatabase existiert!");
                return;
            }

            Debug.Log($"Merged Item ID: {mergedItemId}");

            WellnessItem mergedItem = itemDatabase.CreateItem(mergedItemId);
            if (mergedItem == null)
            {
                Debug.LogError($"Merged Item ist null! ID: {mergedItemId}");
                return;
            }

            Debug.Log($"✓ Merged Item erstellt: {mergedItem.ItemName} (Tier {mergedItem.Tier})");

            // Entferne beide Items
            slot1.ClearItem();
            slot2.ClearItem();

            // Platziere gemergtes Item im ersten Slot
            slot1.SetItem(mergedItem);

            // Benachrichtige GameplayManager
            if (gameplayManager != null)
            {
                gameplayManager.OnItemMerged(item1, item2, mergedItem);
            }
            else
            {
                Debug.LogWarning("GameplayManager ist null - Merge-Event nicht gesendet");
            }

            Debug.Log($"✅ Merge erfolgreich: {item1.ItemName} + {item2.ItemName} → {mergedItem.ItemName}");
        }

        /// <summary>
        /// Fügt Item zum Grid hinzu (erster freier Slot)
        /// </summary>
        public bool AddItemToGrid(WellnessItem item)
        {
            if (item == null)
            {
                Debug.LogError("AddItemToGrid: Item ist null!");
                return false;
            }

            if (gridSlots == null || gridSlots.Length == 0)
            {
                Debug.LogError("AddItemToGrid: GridSlots nicht initialisiert!");
                return false;
            }

            Debug.Log($"Versuche Item hinzuzufügen: {item.ItemName} (Tier {item.Tier})");

            // Suche ersten freien Slot
            for (int i = 0; i < gridSlots.Length; i++)
            {
                if (gridSlots[i] == null)
                {
                    Debug.LogWarning($"Slot_{i} ist null!");
                    continue;
                }

                if (gridSlots[i].IsEmpty)
                {
                    Debug.Log($"✓ Freier Slot gefunden: Slot_{i}");
                    gridSlots[i].SetItem(item);
                    Debug.Log($"✅ Item erfolgreich zu Slot_{i} hinzugefügt: {item.ItemName}");
                    return true;
                }
            }

            Debug.LogWarning("⚠️ Kein freier Slot gefunden - Grid ist voll!");
            // Grid voll → Overflow
            return AddToOverflow(item);
        }

        /// <summary>
        /// Fügt Item zum Overflow-Inventory hinzu
        /// </summary>
        private bool AddToOverflow(WellnessItem item)
        {
            if (overflowItems.Count >= maxOverflowSlots)
            {
                Debug.LogWarning("Overflow-Inventory ist voll!");
                return false;
            }

            overflowItems.Add(item);
            UpdateOverflowUI();
            return true;
        }

        private void UpdateOverflowUI()
        {
            // TODO: Overflow-UI aktualisieren
            Debug.Log($"Overflow: {overflowItems.Count} Items");
        }

        /// <summary>
        /// Prüft ob Grid voll ist
        /// </summary>
        public bool IsGridFull()
        {
            for (int i = 0; i < gridSlots.Length; i++)
            {
                if (gridSlots[i].IsEmpty)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gibt Anzahl freier Slots zurück
        /// </summary>
        public int GetFreeSlotCount()
        {
            int count = 0;
            for (int i = 0; i < gridSlots.Length; i++)
            {
                if (gridSlots[i].IsEmpty)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
