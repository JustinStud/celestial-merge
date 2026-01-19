using UnityEngine;
using UnityEngine.UI;

namespace CelestialMerge
{
    /// <summary>
    /// Helper-Script zum automatischen Setup des Boards (UI-Layout, Zentrierung)
    /// </summary>
    public class BoardSetupHelper : MonoBehaviour
    {
        [ContextMenu("Setup Board Parent - Zentrieren")]
        public void SetupBoardParent()
        {
            ExpandableBoardManager boardManager = FindFirstObjectByType<ExpandableBoardManager>();
            if (boardManager == null)
            {
                Debug.LogError("ExpandableBoardManager nicht gefunden!");
                return;
            }

            // Finde oder erstelle BoardParent
            Transform boardParent = null;
            
            // Versuche BoardParent aus ExpandableBoardManager zu holen
            var boardParentField = typeof(ExpandableBoardManager).GetField("boardParent", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (boardParentField != null)
            {
                boardParent = boardParentField.GetValue(boardManager) as Transform;
            }

            // Falls nicht gefunden, suche im Canvas
            if (boardParent == null)
            {
                Canvas canvas = FindFirstObjectByType<Canvas>();
                if (canvas != null)
                {
                    // Suche nach BoardParent
                    Transform found = canvas.transform.Find("BoardParent");
                    if (found == null)
                    {
                        // Erstelle BoardParent
                        GameObject boardParentObj = new GameObject("BoardParent");
                        boardParentObj.transform.SetParent(canvas.transform, false);
                        boardParent = boardParentObj.transform;
                    }
                    else
                    {
                        boardParent = found;
                    }
                }
            }

            if (boardParent == null)
            {
                Debug.LogError("BoardParent konnte nicht erstellt/gefunden werden!");
                return;
            }

            // Stelle sicher, dass es ein RectTransform hat
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

            // Setze BoardParent im ExpandableBoardManager
            if (boardParentField != null)
            {
                boardParentField.SetValue(boardManager, boardParent);
                Debug.Log("✅ BoardParent gesetzt und zentriert!");
            }

            // Erstelle Slot Prefab falls nicht vorhanden
            CreateSlotPrefabIfNeeded();
        }

        [ContextMenu("Create Slot Prefab")]
        private void CreateSlotPrefabIfNeeded()
        {
            ExpandableBoardManager boardManager = FindFirstObjectByType<ExpandableBoardManager>();
            if (boardManager == null) return;

            // Prüfe ob Slot Prefab gesetzt ist
            var slotPrefabField = typeof(ExpandableBoardManager).GetField("slotPrefab",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (slotPrefabField != null)
            {
                GameObject existingPrefab = slotPrefabField.GetValue(boardManager) as GameObject;
                if (existingPrefab != null) return; // Prefab existiert bereits
            }

            // Erstelle Slot Prefab
            GameObject slotPrefab = new GameObject("BoardSlotPrefab");
            RectTransform rect = slotPrefab.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);

            Image bg = slotPrefab.AddComponent<Image>();
            bg.color = new Color(0.9f, 0.9f, 0.9f, 0.5f);

            // Item Image
            GameObject itemImgObj = new GameObject("ItemImage");
            itemImgObj.transform.SetParent(slotPrefab.transform, false);
            RectTransform itemRect = itemImgObj.AddComponent<RectTransform>();
            itemRect.anchorMin = Vector2.zero;
            itemRect.anchorMax = Vector2.one;
            itemRect.sizeDelta = Vector2.zero;
            Image itemImg = itemImgObj.AddComponent<Image>();
            itemImg.enabled = false;

            // Item Text
            GameObject itemTextObj = new GameObject("ItemText");
            itemTextObj.transform.SetParent(slotPrefab.transform, false);
            RectTransform textRect = itemTextObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            Text itemText = itemTextObj.AddComponent<Text>();
            itemText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            itemText.fontSize = 12;
            itemText.alignment = TextAnchor.MiddleCenter;
            itemText.color = Color.white;
            itemText.enabled = false;

            // Füge CelestialBoardSlot Script hinzu
            slotPrefab.AddComponent<CelestialBoardSlot>();

            // Setze Prefab im Manager
            if (slotPrefabField != null)
            {
                slotPrefabField.SetValue(boardManager, slotPrefab);
                Debug.Log("✅ Slot Prefab erstellt!");
            }
        }
    }
}
