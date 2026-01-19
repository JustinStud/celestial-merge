using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CelestialMerge
{
    /// <summary>
    /// Einzelner Board-Slot für Celestial Merge
    /// Unterstützt Drag-Drop und Item-Verwaltung
    /// </summary>
    public class CelestialBoardSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [Header("Slot Settings")]
        [SerializeField] private int slotIndex;
        [SerializeField] private Image slotBackground;
        [SerializeField] private Image itemImage;
        [SerializeField] private Text itemText; // Optional: Text für Item-Name
        [SerializeField] private CanvasGroup canvasGroup;

        private CelestialItem currentItem;
        private ExpandableBoardManager boardManager;
        private GameObject dragObject;
        private Canvas dragCanvas;

        // Auto-Find boardManager falls nicht gesetzt
        private void Start()
        {
            if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<ExpandableBoardManager>();
            }
        }

        public int SlotIndex => slotIndex;
        public CelestialItem CurrentItem => currentItem;
        public bool IsEmpty => currentItem == null;

        private void Awake()
        {
            // Stelle sicher, dass GameObject RectTransform hat
            RectTransform rect = GetComponent<RectTransform>();
            if (rect == null)
            {
                rect = gameObject.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(100, 100);
            }

            // Auto-Find Components
            if (slotBackground == null)
            {
                slotBackground = GetComponent<Image>();
                if (slotBackground == null)
                {
                    slotBackground = gameObject.AddComponent<Image>();
                    slotBackground.color = new Color(0.9f, 0.9f, 0.9f, 0.5f);
                }
            }
            
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            // Finde oder erstelle ItemImage
            if (itemImage == null)
            {
                Transform itemImgTransform = transform.Find("ItemImage");
                if (itemImgTransform != null)
                {
                    itemImage = itemImgTransform.GetComponent<Image>();
                }
                else
                {
                    // Erstelle ItemImage
                    GameObject itemImgObj = new GameObject("ItemImage");
                    itemImgObj.transform.SetParent(transform, false);
                    RectTransform itemRect = itemImgObj.AddComponent<RectTransform>();
                    itemRect.anchorMin = Vector2.zero;
                    itemRect.anchorMax = Vector2.one;
                    itemRect.sizeDelta = Vector2.zero;
                    itemImage = itemImgObj.AddComponent<Image>();
                    itemImage.enabled = false;
                }
            }

            // Finde oder erstelle ItemText
            if (itemText == null)
            {
                Transform itemTextTransform = transform.Find("ItemText");
                if (itemTextTransform != null)
                {
                    itemText = itemTextTransform.GetComponent<Text>();
                }
                else
                {
                    // Erstelle ItemText
                    GameObject itemTextObj = new GameObject("ItemText");
                    itemTextObj.transform.SetParent(transform, false);
                    RectTransform textRect = itemTextObj.AddComponent<RectTransform>();
                    textRect.anchorMin = Vector2.zero;
                    textRect.anchorMax = Vector2.one;
                    textRect.sizeDelta = Vector2.zero;
                    itemText = itemTextObj.AddComponent<Text>();
                    itemText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    itemText.fontSize = 12;
                    itemText.alignment = TextAnchor.MiddleCenter;
                    itemText.color = Color.white;
                    itemText.enabled = false;
                }
            }

            // Finde Drag Canvas
            dragCanvas = FindFirstObjectByType<Canvas>();

            // Stelle sicher, dass Image raycastTarget aktiviert ist
            if (slotBackground != null)
            {
                slotBackground.raycastTarget = true;
            }
        }

        /// <summary>
        /// Initialisiert den Slot
        /// </summary>
        public void Initialize(int index, ExpandableBoardManager manager)
        {
            slotIndex = index;
            boardManager = manager;
        }

        /// <summary>
        /// Setzt Item in Slot
        /// </summary>
        public void SetItem(CelestialItem item)
        {
            currentItem = item;
            Debug.Log($"SetItem auf Slot_{slotIndex}: {item?.ItemName ?? "null"}");
            UpdateVisual();
            
            // Force Update - stelle sicher, dass UI aktualisiert wird
            if (item != null)
            {
                // Erstelle UI-Komponenten falls nötig
                CreateItemImageIfNeeded();
                CreateItemTextIfNeeded();
                UpdateVisual(); // Nochmal aufrufen nach Erstellung
            }
        }

        /// <summary>
        /// Entfernt Item aus Slot
        /// </summary>
        public void ClearItem()
        {
            currentItem = null;
            UpdateVisual();
        }

        /// <summary>
        /// Aktualisiert visuelle Darstellung
        /// </summary>
        private void UpdateVisual()
        {
            if (currentItem == null)
            {
                // Leerer Slot
                if (itemImage != null)
                {
                    itemImage.enabled = false;
                }
                if (itemText != null)
                {
                    itemText.text = "";
                    itemText.enabled = false;
                }
                if (slotBackground != null)
                {
                    slotBackground.color = new Color(0.8f, 0.8f, 0.8f, 0.5f); // Transparentes Grau
                }
            }
            else
            {
                // Item vorhanden
                if (itemImage != null)
                {
                    itemImage.enabled = true;
                    
                    // Prüfe ob Sprite vorhanden ist
                    if (currentItem.ItemSprite != null)
                    {
                        // SPRITE VORHANDEN: Verwende das Sprite
                        itemImage.sprite = currentItem.ItemSprite;
                        itemImage.color = Color.white; // Reset color für Sprite
                        itemImage.type = Image.Type.Simple;
                    }
                    else
                    {
                        // KEIN SPRITE: Erstelle farbiges Quadrat basierend auf Rarity
                        Color rarityColor = GetRarityColor(currentItem.Rarity);
                        itemImage.color = rarityColor;
                        itemImage.type = Image.Type.Simple;
                        
                        // Erstelle ein einfarbiges Sprite als Fallback
                        Texture2D texture = new Texture2D(64, 64);
                        Color[] pixels = new Color[64 * 64];
                        for (int i = 0; i < pixels.Length; i++)
                        {
                            pixels[i] = rarityColor;
                        }
                        texture.SetPixels(pixels);
                        texture.Apply();
                        Sprite fallbackSprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
                        itemImage.sprite = fallbackSprite;
                    }
                }
                else
                {
                    // Erstelle ItemImage falls nicht vorhanden
                    CreateItemImageIfNeeded();
                }

                if (itemText != null)
                {
                    itemText.text = $"{currentItem.ItemName}\nLvl {currentItem.Level}";
                    itemText.enabled = true;
                    itemText.color = Color.white;
                }
                else
                {
                    // Erstelle ItemText falls nicht vorhanden
                    CreateItemTextIfNeeded();
                }

                if (slotBackground != null)
                {
                    slotBackground.color = Color.white;
                }
            }
        }

        /// <summary>
        /// Erstellt ItemImage falls nicht vorhanden
        /// </summary>
        private void CreateItemImageIfNeeded()
        {
            if (itemImage != null) return;

            Transform itemImgTransform = transform.Find("ItemImage");
            if (itemImgTransform == null)
            {
                GameObject itemImgObj = new GameObject("ItemImage");
                itemImgObj.transform.SetParent(transform, false);
                itemImgTransform = itemImgObj.transform;
            }

            itemImage = itemImgTransform.GetComponent<Image>();
            if (itemImage == null)
            {
                itemImage = itemImgTransform.gameObject.AddComponent<Image>();
            }

            RectTransform itemRect = itemImgTransform.GetComponent<RectTransform>();
            if (itemRect != null)
            {
                itemRect.anchorMin = Vector2.zero;
                itemRect.anchorMax = Vector2.one;
                itemRect.sizeDelta = Vector2.zero;
            }

            // Update Visual erneut
            if (currentItem != null)
            {
                UpdateVisual();
            }
        }

        /// <summary>
        /// Erstellt ItemText falls nicht vorhanden
        /// </summary>
        private void CreateItemTextIfNeeded()
        {
            if (itemText != null) return;

            Transform itemTextTransform = transform.Find("ItemText");
            if (itemTextTransform == null)
            {
                GameObject itemTextObj = new GameObject("ItemText");
                itemTextObj.transform.SetParent(transform, false);
                itemTextTransform = itemTextObj.transform;
            }

            itemText = itemTextTransform.GetComponent<Text>();
            if (itemText == null)
            {
                itemText = itemTextTransform.gameObject.AddComponent<Text>();
                itemText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                itemText.fontSize = 12;
                itemText.alignment = TextAnchor.MiddleCenter;
                itemText.color = Color.white;
            }

            RectTransform textRect = itemTextTransform.GetComponent<RectTransform>();
            if (textRect != null)
            {
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
            }

            // Update Visual erneut
            if (currentItem != null)
            {
                UpdateVisual();
            }
        }

        /// <summary>
        /// Gibt Farbe basierend auf Rarity zurück
        /// </summary>
        private Color GetRarityColor(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common: return new Color(0.7f, 0.7f, 0.7f); // Gray
                case ItemRarity.Uncommon: return new Color(0.5f, 1f, 0.7f); // Green
                case ItemRarity.Rare: return new Color(0.5f, 0.7f, 1f); // Blue
                case ItemRarity.Epic: return new Color(0.8f, 0.5f, 1f); // Purple
                case ItemRarity.Legendary: return new Color(1f, 0.9f, 0.5f); // Gold
                case ItemRarity.Mythic: return new Color(1f, 0.5f, 1f); // Rainbow/Pink
                default: return Color.white;
            }
        }

        #region Drag & Drop

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (currentItem == null)
            {
                return;
            }

            // Erstelle Drag-Objekt
            if (dragCanvas != null)
            {
                dragObject = new GameObject("DragItem");
                dragObject.transform.SetParent(dragCanvas.transform, false);
                dragObject.transform.SetAsLastSibling();

                RectTransform dragRect = dragObject.AddComponent<RectTransform>();
                dragRect.sizeDelta = new Vector2(80, 80);

                Image dragImage = dragObject.AddComponent<Image>();
                dragImage.sprite = currentItem.ItemSprite;
                if (dragImage.sprite == null)
                {
                    dragImage.color = GetRarityColor(currentItem.Rarity);
                }

                CanvasGroup dragCanvasGroup = dragObject.AddComponent<CanvasGroup>();
                dragCanvasGroup.alpha = 0.8f;
                dragCanvasGroup.blocksRaycasts = false;

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0.5f; // Original-Slot transparent machen
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragObject != null)
            {
                dragObject.transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (dragObject != null)
            {
                Destroy(dragObject);
                dragObject = null;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f; // Original-Slot wieder sichtbar
            }

            // Prüfe ob über einem anderen Slot gedroppt wurde
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                CelestialBoardSlot targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<CelestialBoardSlot>();
                if (targetSlot == null)
                {
                    targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<CelestialBoardSlot>();
                }

                if (targetSlot != null && targetSlot != this && boardManager != null)
                {
                    // Handle Drop - wird von BoardManager gehandhabt
                    Debug.Log($"Drag beendet: Slot_{slotIndex} → Slot_{targetSlot.slotIndex}");
                    boardManager.HandleSlotDrop(this, targetSlot);
                }
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            // Wird von BoardManager in OnEndDrag gehandhabt
            Debug.Log($"OnDrop aufgerufen für Slot_{slotIndex}");
        }

        #endregion
    }
}
