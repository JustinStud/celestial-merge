using UnityEngine;
using UnityEngine.UI;

namespace CelestialMerge.UI
{
    /// <summary>
    /// Automatischer Fix für XP Progress Bar Position
    /// Setzt Position und Größe beim Start automatisch
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class XPProgressBarFixer : MonoBehaviour
    {
        [Header("Auto-Fix Settings")]
        [SerializeField] private bool autoFixOnStart = true;
        [SerializeField] private bool autoFixOnEnable = true;

        [Header("Target Position (Top-Left Anchor)")]
        [SerializeField] private float posX = 10f;
        [SerializeField] private float posY = -75f;
        [SerializeField] private float width = 280f;
        [SerializeField] private float height = 20f;

        private RectTransform rectTransform;
        private Slider slider;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            slider = GetComponent<Slider>();
            
            // Sicherstellen dass RectTransform vorhanden ist
            if (rectTransform == null)
            {
                Debug.LogError("XPProgressBarFixer: RectTransform Component fehlt! Stelle sicher dass das Script auf einem GameObject mit RectTransform ist.");
            }
        }

        private void Start()
        {
            if (autoFixOnStart)
            {
                FixPosition();
            }
        }

        private void OnEnable()
        {
            if (autoFixOnEnable)
            {
                FixPosition();
            }
        }

        /// <summary>
        /// Fixiert Position und Größe der Progress Bar
        /// </summary>
        [ContextMenu("Fix XP Progress Bar Position")]
        public void FixPosition()
        {
            // Versuche RectTransform zu holen falls null
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            
            if (rectTransform == null)
            {
                Debug.LogError("XPProgressBarFixer: RectTransform ist null! Stelle sicher dass das Script auf einem GameObject mit RectTransform ist.");
                return;
            }

            // Setze Anchor auf Top-Left
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f); // WICHTIG: Pivot muss (0,1) sein!

            // Setze Position und Größe
            // Pos Y ist negativ weil wir von oben nach unten gehen
            rectTransform.anchoredPosition = new Vector2(posX, posY);
            rectTransform.sizeDelta = new Vector2(width, height);
            
            // Force Update
            rectTransform.ForceUpdateRectTransforms();

            // Prüfe Fill Area
            Transform fillArea = transform.Find("Fill Area");
            if (fillArea != null)
            {
                RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
                if (fillAreaRect != null)
                {
                    // Fill Area sollte Stretch-Stretch sein
                    fillAreaRect.anchorMin = new Vector2(0f, 0f);
                    fillAreaRect.anchorMax = new Vector2(1f, 1f);
                    fillAreaRect.offsetMin = Vector2.zero;
                    fillAreaRect.offsetMax = Vector2.zero;
                }
            }

            // Prüfe Fill
            Transform fill = transform.Find("Fill Area/Fill");
            if (fill != null)
            {
                RectTransform fillRect = fill.GetComponent<RectTransform>();
                if (fillRect != null)
                {
                    // Fill sollte Left-Stretch sein
                    fillRect.anchorMin = new Vector2(0f, 0f);
                    fillRect.anchorMax = new Vector2(0f, 1f);
                    fillRect.pivot = new Vector2(0f, 0.5f);
                    fillRect.offsetMin = Vector2.zero;
                    fillRect.offsetMax = Vector2.zero;
                }
            }

            Debug.Log($"✅ XP Progress Bar Position gefixt: Pos=({posX}, {posY}), Size=({width}, {height})");
        }

        /// <summary>
        /// Prüft ob Position korrekt ist
        /// </summary>
        [ContextMenu("Check Position")]
        public void CheckPosition()
        {
            if (rectTransform == null)
            {
                Debug.LogError("XPProgressBarFixer: RectTransform ist null!");
                return;
            }

            Debug.Log($"=== XP Progress Bar Position Check ===");
            Debug.Log($"Anchor Min: {rectTransform.anchorMin}");
            Debug.Log($"Anchor Max: {rectTransform.anchorMax}");
            Debug.Log($"Pivot: {rectTransform.pivot}");
            Debug.Log($"Anchored Position: {rectTransform.anchoredPosition}");
            Debug.Log($"Size Delta: {rectTransform.sizeDelta}");
            Debug.Log($"Rect: {rectTransform.rect}");
            Debug.Log($"World Position: {rectTransform.position}");
            Debug.Log($"Screen Position: {RectTransformUtility.WorldToScreenPoint(null, rectTransform.position)}");
        }
    }
}
