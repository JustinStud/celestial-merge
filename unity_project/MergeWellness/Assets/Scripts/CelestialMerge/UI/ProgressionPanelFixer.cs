using UnityEngine;
using UnityEngine.UI;

namespace CelestialMerge.UI
{
    /// <summary>
    /// Automatischer Fix für ProgressionPanel Position
    /// Stellt sicher, dass das Panel vollständig sichtbar ist
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ProgressionPanelFixer : MonoBehaviour
    {
        [Header("Auto-Fix Settings")]
        [SerializeField] private bool autoFixOnStart = true;
        [SerializeField] private bool autoFixOnEnable = true;

        [Header("Target Position (Top-Left Anchor)")]
        [SerializeField] private float posX = 10f;
        [SerializeField] private float posY = -10f;
        [SerializeField] private float width = 320f; // Etwas breiter für bessere Sichtbarkeit
        [SerializeField] private float height = 150f;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
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
        /// Fixiert Position und Größe des Panels
        /// </summary>
        [ContextMenu("Fix ProgressionPanel Position")]
        public void FixPosition()
        {
            if (rectTransform == null)
            {
                Debug.LogError("ProgressionPanelFixer: RectTransform ist null!");
                return;
            }

            // Setze Anchor auf Top-Left
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f); // WICHTIG: Pivot muss (0,1) sein!

            // Setze Position und Größe
            rectTransform.anchoredPosition = new Vector2(posX, posY);
            rectTransform.sizeDelta = new Vector2(width, height);

            // Force Update
            rectTransform.ForceUpdateRectTransforms();

            Debug.Log($"✅ ProgressionPanel Position gefixt: Pos=({posX}, {posY}), Size=({width}, {height})");
        }

        /// <summary>
        /// Prüft ob Position korrekt ist
        /// </summary>
        [ContextMenu("Check Position")]
        public void CheckPosition()
        {
            if (rectTransform == null)
            {
                Debug.LogError("ProgressionPanelFixer: RectTransform ist null!");
                return;
            }

            Debug.Log($"=== ProgressionPanel Position Check ===");
            Debug.Log($"Anchor Min: {rectTransform.anchorMin}");
            Debug.Log($"Anchor Max: {rectTransform.anchorMax}");
            Debug.Log($"Pivot: {rectTransform.pivot}");
            Debug.Log($"Anchored Position: {rectTransform.anchoredPosition}");
            Debug.Log($"Size Delta: {rectTransform.sizeDelta}");
            Debug.Log($"Rect: {rectTransform.rect}");
        }
    }
}
