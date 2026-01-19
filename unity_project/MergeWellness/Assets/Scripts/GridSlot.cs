using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MergeWellness
{
    /// <summary>
    /// Einzelner Slot im 5×5 Grid mit Drag-Drop-Funktionalität
    /// </summary>
    public class GridSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private int slotIndex;
        [SerializeField] private Image slotImage;
        [SerializeField] private Image itemImage;
        [SerializeField] private Text itemText;
        [SerializeField] private CanvasGroup canvasGroup;

        private WellnessItem currentItem;
        private GridManager gridManager;
        private GameObject dragObject;
        private Canvas dragCanvas;

        public int SlotIndex => slotIndex;
        public WellnessItem CurrentItem => currentItem;
        public bool IsEmpty => currentItem == null;

        private void Awake()
        {
            if (slotImage == null) slotImage = GetComponent<Image>();
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

            // WICHTIG: Stelle sicher, dass Image raycastTarget aktiviert ist für Drag-Drop
            if (slotImage != null)
            {
                slotImage.raycastTarget = true;
            }

            // Finde ItemImage und ItemText in Children
            if (itemImage == null)
            {
                Transform itemImgTransform = transform.Find("ItemImage");
                if (itemImgTransform != null)
                {
                    itemImage = itemImgTransform.GetComponent<Image>();
                }
            }

            if (itemText == null)
            {
                Transform itemTextTransform = transform.Find("ItemText");
                if (itemTextTransform != null)
                {
                    itemText = itemTextTransform.GetComponent<Text>();
                }
            }

            // Finde Drag Canvas
            dragCanvas = FindFirstObjectByType<Canvas>();

            // Stelle sicher, dass EventSystem vorhanden ist
            if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                Debug.LogWarning("EventSystem nicht gefunden! Drag-Drop funktioniert möglicherweise nicht.");
            }
        }

        public void Initialize(int index, GridManager manager)
        {
            slotIndex = index;
            gridManager = manager;
        }

        public void SetItem(WellnessItem item)
        {
            currentItem = item;
            UpdateVisual();
        }

        public void ClearItem()
        {
            currentItem = null;
            UpdateVisual();
        }

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
                if (slotImage != null)
                {
                    slotImage.color = new Color(0.8f, 0.8f, 0.8f, 0.5f); // Transparentes Grau
                }
            }
            else
            {
                // Item vorhanden
                if (itemImage != null)
                {
                    itemImage.enabled = true;
                    itemImage.sprite = currentItem.ItemSprite;
                    if (itemImage.sprite == null)
                    {
                        // Fallback: Farbiges Quadrat basierend auf Tier
                        itemImage.color = GetTierColor(currentItem.Tier);
                    }
                }

                if (itemText != null)
                {
                    itemText.text = $"{currentItem.ItemName}\nT{currentItem.Tier}";
                    itemText.enabled = true;
                    itemText.color = Color.white;
                    itemText.fontSize = 14;
                }

                if (slotImage != null)
                {
                    slotImage.color = Color.white;
                }
            }
        }

        private Color GetTierColor(int tier)
        {
            // Farben basierend auf Tier
            switch (tier)
            {
                case 1: return new Color(0.5f, 0.7f, 1f); // Hellblau
                case 2: return new Color(0.5f, 1f, 0.7f); // Hellgrün
                case 3: return new Color(1f, 0.9f, 0.5f); // Gelb
                case 4: return new Color(1f, 0.7f, 0.5f); // Orange
                case 5: return new Color(1f, 0.5f, 0.5f); // Rot
                default: return new Color(0.8f, 0.5f, 1f); // Lila
            }
        }

        #region Drag & Drop

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (currentItem == null)
            {
                Debug.Log($"OnBeginDrag: Slot_{slotIndex} ist leer - kein Drag möglich");
                return;
            }

            Debug.Log($"OnBeginDrag: Slot_{slotIndex} - Item: {currentItem.ItemName}");

            // Erstelle Drag-Objekt
            dragObject = new GameObject("DragItem");
            dragObject.transform.SetParent(dragCanvas.transform, false);
            dragObject.transform.SetAsLastSibling();

            RectTransform dragRect = dragObject.AddComponent<RectTransform>();
            dragRect.sizeDelta = new Vector2(80, 80);

            Image dragImage = dragObject.AddComponent<Image>();
            dragImage.sprite = currentItem.ItemSprite;
            if (dragImage.sprite == null)
            {
                dragImage.color = GetTierColor(currentItem.Tier);
            }

            CanvasGroup dragCanvasGroup = dragObject.AddComponent<CanvasGroup>();
            dragCanvasGroup.alpha = 0.8f;
            dragCanvasGroup.blocksRaycasts = false;

            canvasGroup.alpha = 0.5f; // Original-Slot transparent machen
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragObject == null) return;

            dragObject.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (dragObject != null)
            {
                Destroy(dragObject);
                dragObject = null;
            }

            canvasGroup.alpha = 1f; // Original-Slot wieder sichtbar

            // Prüfe ob über einem anderen Slot gedroppt wurde
            // Verwende pointerCurrentRaycast statt pointerEnter für bessere Erkennung
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                GridSlot targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<GridSlot>();
                if (targetSlot == null)
                {
                    // Versuche auch in Parent zu suchen
                    targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<GridSlot>();
                }

                if (targetSlot != null && targetSlot != this)
                {
                    Debug.Log($"Drag beendet: Slot_{slotIndex} → Slot_{targetSlot.slotIndex}");
                    gridManager?.HandleSlotDrop(this, targetSlot);
                    return;
                }
            }

            // Fallback: Prüfe pointerEnter
            if (eventData.pointerEnter != null)
            {
                GridSlot targetSlot = eventData.pointerEnter.GetComponent<GridSlot>();
                if (targetSlot == null)
                {
                    targetSlot = eventData.pointerEnter.GetComponentInParent<GridSlot>();
                }

                if (targetSlot != null && targetSlot != this)
                {
                    Debug.Log($"Drag beendet (Fallback): Slot_{slotIndex} → Slot_{targetSlot.slotIndex}");
                    gridManager?.HandleSlotDrop(this, targetSlot);
                }
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            // Wird von GridManager in OnEndDrag gehandhabt
            // Diese Methode wird aufgerufen, wenn ein Item auf diesen Slot gedroppt wird
            Debug.Log($"OnDrop aufgerufen für Slot_{slotIndex}");
        }

        #endregion
    }
}
