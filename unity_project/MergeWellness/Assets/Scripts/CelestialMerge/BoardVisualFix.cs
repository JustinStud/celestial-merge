using UnityEngine;
using UnityEngine.UI;

namespace CelestialMerge
{
    /// <summary>
    /// Helper-Script zum Reparieren der Board-Visualisierung
    /// </summary>
    public class BoardVisualFix : MonoBehaviour
    {
        [ContextMenu("Fix All Board Slots - Visual")]
        public void FixAllBoardSlots()
        {
            ExpandableBoardManager boardManager = FindFirstObjectByType<ExpandableBoardManager>();
            if (boardManager == null)
            {
                Debug.LogError("ExpandableBoardManager nicht gefunden!");
                return;
            }

            // Finde alle BoardSlots
            CelestialBoardSlot[] allSlots = FindObjectsByType<CelestialBoardSlot>(FindObjectsSortMode.None);
            Debug.Log($"Gefundene Slots: {allSlots.Length}");

            int fixedCount = 0;
            foreach (var slot in allSlots)
            {
                if (slot != null)
                {
                    FixSlotVisual(slot);
                    fixedCount++;
                }
            }

            Debug.Log($"✅ {fixedCount} Slots repariert!");
        }

        [ContextMenu("Fix Board Size (4x5)")]
        public void FixBoardSize()
        {
            ExpandableBoardManager boardManager = FindFirstObjectByType<ExpandableBoardManager>();
            if (boardManager == null)
            {
                Debug.LogError("ExpandableBoardManager nicht gefunden!");
                return;
            }

            // Setze Board-Größe auf 4x5
            var widthField = typeof(ExpandableBoardManager).GetField("currentWidth",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var heightField = typeof(ExpandableBoardManager).GetField("currentHeight",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (widthField != null) widthField.SetValue(boardManager, 4);
            if (heightField != null) heightField.SetValue(boardManager, 5);

            Debug.Log("✅ Board-Größe auf 4×5 gesetzt!");
        }

        private void FixSlotVisual(CelestialBoardSlot slot)
        {
            if (slot == null) return;

            // Stelle sicher, dass Slot Background existiert
            Image slotBg = slot.GetComponent<Image>();
            if (slotBg == null)
            {
                slotBg = slot.gameObject.AddComponent<Image>();
                slotBg.color = new Color(0.9f, 0.9f, 0.9f, 0.5f);
            }

            // Stelle sicher, dass RectTransform richtig konfiguriert ist
            RectTransform rect = slot.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(100, 100);
            }

            // Force Update Visual
            var currentItem = slot.CurrentItem;
            if (currentItem != null)
            {
                slot.SetItem(currentItem); // Trigger Update
            }
        }
    }
}
