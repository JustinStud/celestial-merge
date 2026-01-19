using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CelestialMerge
{
    /// <summary>
    /// Verwaltet expandierbares Board (4×5 → 8×10)
    /// Start: 4×5 (20 Felder), expandiert alle 4 Level
    /// </summary>
    public class ExpandableBoardManager : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField] private int currentWidth = 4; // Sollte 4 sein (nicht 5!)
        [SerializeField] private int currentHeight = 5; // Sollte 5 sein (nicht 4!)
        [SerializeField] private int maxWidth = 8;
        [SerializeField] private int maxHeight = 10;

        [Header("Grid Settings")]
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform boardParent;
        [SerializeField] private GridLayoutGroup gridLayout;

        [Header("Expansion")]
        [SerializeField] private int expansionLevelInterval = 4; // Expandiert alle 4 Level
        [SerializeField] private bool autoExpand = true;

        [Header("References")]
        [SerializeField] private CelestialProgressionManager progressionManager;

        private List<CelestialBoardSlot> boardSlots = new List<CelestialBoardSlot>();

        public int CurrentWidth => currentWidth;
        public int CurrentHeight => currentHeight;
        public int TotalSlots => currentWidth * currentHeight;

        private void Awake()
        {
            // Auto-Find ProgressionManager falls nicht gesetzt
            if (progressionManager == null)
            {
                progressionManager = FindFirstObjectByType<CelestialProgressionManager>();
            }
        }

        private void Start()
        {
            InitializeBoard();
            LoadBoardState();

            // Subscribe to Level Up
            if (progressionManager != null)
            {
                progressionManager.OnLevelUp += OnPlayerLevelUp;
            }
        }

        private void OnDestroy()
        {
            if (progressionManager != null)
            {
                progressionManager.OnLevelUp -= OnPlayerLevelUp;
            }
        }

        /// <summary>
        /// Initialisiert das Board
        /// </summary>
        private void InitializeBoard()
        {
            if (boardParent == null)
            {
                Debug.LogError("BoardParent nicht gesetzt!");
                return;
            }

            // Stelle sicher, dass boardParent ein RectTransform hat (für UI)
            RectTransform rectTransform = boardParent.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = boardParent.gameObject.AddComponent<RectTransform>();
            }

            // Zentriere das Board
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;

            // Berechne Board-Größe basierend auf Slots
            float cellWidth = 100f;
            float cellHeight = 100f;
            float spacing = 10f;
            float padding = 10f;
            
            float totalWidth = (currentWidth * cellWidth) + ((currentWidth - 1) * spacing) + (padding * 2);
            float totalHeight = (currentHeight * cellHeight) + ((currentHeight - 1) * spacing) + (padding * 2);
            
            rectTransform.sizeDelta = new Vector2(totalWidth, totalHeight);

            // Setup GridLayoutGroup
            if (gridLayout == null)
            {
                gridLayout = boardParent.GetComponent<GridLayoutGroup>();
                if (gridLayout == null)
                {
                    gridLayout = boardParent.gameObject.AddComponent<GridLayoutGroup>();
                }
            }

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = currentWidth;
            gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
            gridLayout.spacing = new Vector2(spacing, spacing);
            gridLayout.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);
            gridLayout.childAlignment = TextAnchor.MiddleCenter; // Zentriert die Slots

            CreateSlots();
        }

        /// <summary>
        /// Erstellt Slots für aktuelles Board
        /// </summary>
        private void CreateSlots()
        {
            // Lösche alte Slots
            foreach (var slot in boardSlots)
            {
                if (slot != null) Destroy(slot.gameObject);
            }
            boardSlots.Clear();

            // Erstelle neue Slots
            int totalSlots = currentWidth * currentHeight;
            for (int i = 0; i < totalSlots; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, boardParent);
                slotObj.name = $"BoardSlot_{i}";

                CelestialBoardSlot slot = slotObj.GetComponent<CelestialBoardSlot>();
                if (slot == null)
                {
                    slot = slotObj.AddComponent<CelestialBoardSlot>();
                }

                slot.Initialize(i, this);
                boardSlots.Add(slot);
            }

            Debug.Log($"Board initialisiert: {currentWidth}×{currentHeight} = {totalSlots} Slots");
        }

        /// <summary>
        /// Wird aufgerufen wenn Spieler Level aufsteigt
        /// </summary>
        private void OnPlayerLevelUp(int newLevel)
        {
            if (autoExpand && newLevel % expansionLevelInterval == 0)
            {
                TryExpandBoard();
            }
        }

        /// <summary>
        /// Versucht Board zu erweitern
        /// </summary>
        public bool TryExpandBoard()
        {
            int newWidth = currentWidth;
            int newHeight = currentHeight;

            // Expansion Logic basierend auf Level
            if (progressionManager != null)
            {
                int level = progressionManager.PlayerLevel;
                
                // Expansion Schedule aus GDD
                if (level >= 6 && level < 16 && currentWidth == 4 && currentHeight == 5)
                {
                    newHeight = 6; // 4×6
                }
                else if (level >= 16 && level < 31 && currentWidth == 4)
                {
                    newWidth = 5; // 5×6
                }
                else if (level >= 31 && level < 51 && currentHeight == 6)
                {
                    newHeight = 7; // 5×7
                }
                else if (level >= 51 && level < 76)
                {
                    newWidth = 6;
                    newHeight = 8; // 6×8
                }
                else if (level >= 76 && level < 101)
                {
                    newWidth = 7;
                    newHeight = 8; // 7×8
                }
                else if (level >= 101 && level < 150)
                {
                    newWidth = 8;
                    newHeight = 9; // 8×9
                }
                else if (level >= 150)
                {
                    newWidth = 8;
                    newHeight = 10; // 8×10 (Max)
                }
            }

            // Prüfe ob Expansion möglich
            if (newWidth > maxWidth) newWidth = maxWidth;
            if (newHeight > maxHeight) newHeight = maxHeight;

            if (newWidth > currentWidth || newHeight > currentHeight)
            {
                ExpandBoard(newWidth, newHeight);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Erweitert Board auf neue Größe
        /// </summary>
        private void ExpandBoard(int newWidth, int newHeight)
        {
            int oldSlots = currentWidth * currentHeight;
            currentWidth = newWidth;
            currentHeight = newHeight;
            int newSlots = currentWidth * currentHeight;

            // Update GridLayout
            if (gridLayout != null)
            {
                gridLayout.constraintCount = currentWidth;
            }

            // Erstelle zusätzliche Slots
            int slotsToAdd = newSlots - oldSlots;
            for (int i = 0; i < slotsToAdd; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, boardParent);
                slotObj.name = $"BoardSlot_{boardSlots.Count}";

                CelestialBoardSlot slot = slotObj.GetComponent<CelestialBoardSlot>();
                if (slot == null)
                {
                    slot = slotObj.AddComponent<CelestialBoardSlot>();
                }

                slot.Initialize(boardSlots.Count, this);
                boardSlots.Add(slot);
            }

            SaveBoardState();
            Debug.Log($"✅ Board erweitert: {oldSlots} → {newSlots} Slots ({currentWidth}×{currentHeight})");
        }

        /// <summary>
        /// Gibt ersten freien Slot zurück
        /// </summary>
        public CelestialBoardSlot GetFreeSlot()
        {
            foreach (var slot in boardSlots)
            {
                if (slot != null && slot.IsEmpty)
                {
                    return slot;
                }
            }
            return null;
        }

        /// <summary>
        /// Fügt Item zum Board hinzu (erster freier Slot)
        /// </summary>
        public bool AddItemToBoard(CelestialItem item)
        {
            if (item == null)
            {
                Debug.LogError("AddItemToBoard: Item ist null!");
                return false;
            }

            CelestialBoardSlot freeSlot = GetFreeSlot();
            if (freeSlot != null)
            {
                freeSlot.SetItem(item);
                Debug.Log($"✅ Item erfolgreich zum Board hinzugefügt: {item.ItemName}");
                return true;
            }

            Debug.LogWarning("⚠️ Kein freier Slot gefunden - Board ist voll!");
            return false;
        }

        /// <summary>
        /// Behandelt Drag-Drop zwischen Slots
        /// </summary>
        public void HandleSlotDrop(CelestialBoardSlot sourceSlot, CelestialBoardSlot targetSlot)
        {
            if (sourceSlot == null || targetSlot == null)
            {
                Debug.LogError("HandleSlotDrop: Slot ist null!");
                return;
            }

            if (sourceSlot.IsEmpty)
            {
                Debug.LogWarning("HandleSlotDrop: sourceSlot ist leer!");
                return;
            }

            CelestialItem sourceItem = sourceSlot.CurrentItem;
            Debug.Log($"HandleSlotDrop: Slot_{sourceSlot.SlotIndex} → Slot_{targetSlot.SlotIndex}");

            if (targetSlot.IsEmpty)
            {
                // Einfaches Verschieben
                Debug.Log("  → Einfaches Verschieben");
                MoveItem(sourceSlot, targetSlot);
            }
            else
            {
                // Merge-Versuch
                CelestialItem targetItem = targetSlot.CurrentItem;
                Debug.Log($"  Target Item: {targetItem?.ItemName} (Level {targetItem?.Level}, Type: {targetItem?.Category})");

                if (sourceItem.CanMergeWith(targetItem))
                {
                    Debug.Log("  → Merge möglich! Führe Merge durch...");
                    PerformMerge(sourceSlot, targetSlot);
                }
                else
                {
                    Debug.Log("  → Merge nicht möglich - Swap Items");
                    SwapItems(sourceSlot, targetSlot);
                }
            }
        }

        private void MoveItem(CelestialBoardSlot from, CelestialBoardSlot to)
        {
            CelestialItem item = from.CurrentItem;
            from.ClearItem();
            to.SetItem(item);
        }

        private void SwapItems(CelestialBoardSlot slot1, CelestialBoardSlot slot2)
        {
            CelestialItem item1 = slot1.CurrentItem;
            CelestialItem item2 = slot2.CurrentItem;

            slot1.SetItem(item2);
            slot2.SetItem(item1);
        }

        /// <summary>
        /// Führt Merge durch
        /// </summary>
        private void PerformMerge(CelestialBoardSlot slot1, CelestialBoardSlot slot2)
        {
            CelestialItem item1 = slot1.CurrentItem;
            CelestialItem item2 = slot2.CurrentItem;

            if (item1 == null || item2 == null)
            {
                Debug.LogError("PerformMerge: Eines der Items ist null!");
                return;
            }

            if (!item1.CanMergeWith(item2))
            {
                Debug.LogWarning($"Items können nicht gemerged werden!");
                return;
            }

            // Verwende CelestialMergeManager für Merge
            CelestialMergeManager mergeManager = FindFirstObjectByType<CelestialMergeManager>();
            if (mergeManager == null)
            {
                Debug.LogError("CelestialMergeManager nicht gefunden!");
                return;
            }

            MergeResult result = mergeManager.PerformTwoMerge(item1, item2);
            if (result.Success)
            {
                // Entferne beide Items
                slot1.ClearItem();
                slot2.ClearItem();

                // Platziere gemergtes Item im ersten Slot
                slot1.SetItem(result.MergedItem);

                // Registriere Merge für Progression (Milestones)
                if (progressionManager != null)
                {
                    progressionManager.RegisterMerge();
                }

                Debug.Log($"✅ Merge erfolgreich: {item1.ItemName} + {item2.ItemName} → {result.MergedItem.ItemName} (+{result.XpReward} XP)");
            }
            else
            {
                Debug.LogError($"❌ Merge fehlgeschlagen: {result.ErrorMessage}");
            }
        }

        /// <summary>
        /// Prüft ob Board voll ist
        /// </summary>
        public bool IsBoardFull()
        {
            foreach (var slot in boardSlots)
            {
                if (slot != null && slot.IsEmpty)
                {
                    return false;
                }
            }
            return true;
        }

        #region Save/Load

        private void SaveBoardState()
        {
            PlayerPrefs.SetInt("BoardWidth", currentWidth);
            PlayerPrefs.SetInt("BoardHeight", currentHeight);
            PlayerPrefs.Save();
        }

        private void LoadBoardState()
        {
            int savedWidth = PlayerPrefs.GetInt("BoardWidth", 4);
            int savedHeight = PlayerPrefs.GetInt("BoardHeight", 5);

            if (savedWidth != currentWidth || savedHeight != currentHeight)
            {
                currentWidth = savedWidth;
                currentHeight = savedHeight;
                CreateSlots();
            }
        }

        #endregion
    }

    // CelestialBoardSlot wurde in separate Datei verschoben: CelestialBoardSlot.cs
}
