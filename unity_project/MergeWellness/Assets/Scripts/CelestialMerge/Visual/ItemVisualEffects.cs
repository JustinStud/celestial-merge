using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if DOTWEEN_AVAILABLE
using DG.Tweening;
#endif

namespace CelestialMerge.Visual
{
    /// <summary>
    /// Visuelle Effekte für Items (Animationen, Glow, etc.)
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class ItemVisualEffects : MonoBehaviour
    {
        [Header("Rarity Visuals")]
        [SerializeField] private Image rarityBorder;
        [SerializeField] private Image rarityGlow;
        [SerializeField] private GameObject rarityIcon;

        [Header("Animation Settings")]
        [SerializeField] private float spawnScaleDuration = 0.3f;
        [SerializeField] private float pulseDuration = 1f;

        private Image itemImage;
        private RectTransform rectTransform;
        private ItemRarity currentRarity;
        private MergeFeedbackSystem feedbackSystem;

        private void Awake()
        {
            itemImage = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            feedbackSystem = MergeFeedbackSystem.Instance;
        }

        /// <summary>
        /// Setzt Rarity und aktualisiert visuelle Effekte
        /// </summary>
        public void SetRarity(ItemRarity rarity)
        {
            currentRarity = rarity;
            UpdateRarityVisuals();
        }

        /// <summary>
        /// Spawn-Animation (Scale Up)
        /// </summary>
        public void PlaySpawnAnimation()
        {
            if (rectTransform == null) return;

            // Starte bei Scale 0
            rectTransform.localScale = Vector3.zero;

#if DOTWEEN_AVAILABLE
            // Scale Up Animation mit DOTween
            rectTransform.DOScale(Vector3.one, spawnScaleDuration)
                .SetEase(Ease.OutBack);
#else
            // Alternative: Coroutine-basierte Animation
            StartCoroutine(ScaleUpAnimation());
#endif
        }

#if !DOTWEEN_AVAILABLE
        private IEnumerator ScaleUpAnimation()
        {
            float elapsed = 0f;
            while (elapsed < spawnScaleDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / spawnScaleDuration;
                // Ease Out Back Approximation
                t = 1f - Mathf.Pow(1f - t, 3f);
                rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return null;
            }
            rectTransform.localScale = Vector3.one;
        }
#endif

        /// <summary>
        /// Pulse-Animation für höhere Rarities
        /// </summary>
        public void PlayPulseAnimation()
        {
            if (currentRarity < ItemRarity.Epic) return;

            if (rectTransform != null)
            {
#if DOTWEEN_AVAILABLE
                rectTransform.DOScale(Vector3.one * 1.1f, pulseDuration / 2f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
#else
                StartCoroutine(PulseAnimation());
#endif
            }
        }

#if !DOTWEEN_AVAILABLE
        private IEnumerator PulseAnimation()
        {
            Vector3 baseScale = Vector3.one;
            while (true)
            {
                float elapsed = 0f;
                while (elapsed < pulseDuration / 2f)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Sin(elapsed / (pulseDuration / 2f) * Mathf.PI);
                    rectTransform.localScale = Vector3.Lerp(baseScale, baseScale * 1.1f, t);
                    yield return null;
                }
            }
        }
#endif

        /// <summary>
        /// Hover-Glow Effekt
        /// </summary>
        public void SetHoverGlow(bool active)
        {
            if (rarityGlow != null)
            {
                rarityGlow.gameObject.SetActive(active);
                if (active)
                {
#if DOTWEEN_AVAILABLE
                    rarityGlow.DOFade(0.5f, 0.2f);
#else
                    Color c = rarityGlow.color;
                    c.a = 0.5f;
                    rarityGlow.color = c;
#endif
                }
            }
        }

        /// <summary>
        /// Merge-Animation (Items verschmelzen)
        /// </summary>
        public void PlayMergeAnimation(System.Action onComplete = null)
        {
            if (rectTransform == null)
            {
                onComplete?.Invoke();
                return;
            }

#if DOTWEEN_AVAILABLE
            // Scale Down + Fade Out mit DOTween
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rectTransform.DOScale(Vector3.zero, 0.3f));
            sequence.Join(itemImage.DOFade(0f, 0.3f));
            sequence.OnComplete(() => onComplete?.Invoke());
#else
            // Alternative: Coroutine-basierte Animation
            StartCoroutine(MergeAnimationCoroutine(onComplete));
#endif
        }

#if !DOTWEEN_AVAILABLE
        private IEnumerator MergeAnimationCoroutine(System.Action onComplete)
        {
            float duration = 0.3f;
            float elapsed = 0f;
            Vector3 startScale = rectTransform.localScale;
            Color startColor = itemImage.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                rectTransform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                Color c = startColor;
                c.a = Mathf.Lerp(startColor.a, 0f, t);
                itemImage.color = c;

                yield return null;
            }

            onComplete?.Invoke();
        }
#endif

        /// <summary>
        /// Aktualisiert Rarity-basierte visuelle Effekte
        /// </summary>
        private void UpdateRarityVisuals()
        {
            if (feedbackSystem == null) return;

            Color rarityColor = feedbackSystem.GetRarityColor(currentRarity);

            // Item Farbe
            if (itemImage != null)
            {
                // Leichte Tönung basierend auf Rarity
                itemImage.color = Color.Lerp(Color.white, rarityColor, 0.2f);
            }

            // Rarity Border
            if (rarityBorder != null)
            {
                rarityBorder.color = rarityColor;
                rarityBorder.gameObject.SetActive(currentRarity >= ItemRarity.Rare);
            }

            // Rarity Glow (nur Epic+)
            if (rarityGlow != null)
            {
                rarityGlow.color = new Color(rarityColor.r, rarityColor.g, rarityColor.b, 0.3f);
                rarityGlow.gameObject.SetActive(currentRarity >= ItemRarity.Epic);
                
                if (currentRarity >= ItemRarity.Epic)
                {
#if DOTWEEN_AVAILABLE
                    // Pulsierender Glow mit DOTween
                    rarityGlow.DOFade(0.6f, pulseDuration)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);
#else
                    // Alternative: Coroutine-basierter Glow
                    StartCoroutine(GlowPulseAnimation());
#endif
                }
            }

            // Rarity Icon
            if (rarityIcon != null)
            {
                rarityIcon.SetActive(currentRarity >= ItemRarity.Rare);
            }

            // Pulse Animation für höhere Rarities
            if (currentRarity >= ItemRarity.Epic)
            {
                PlayPulseAnimation();
            }
        }

        private void OnDestroy()
        {
#if DOTWEEN_AVAILABLE
            // Cleanup DOTween
            rectTransform?.DOKill();
            itemImage?.DOKill();
            rarityGlow?.DOKill();
#endif
        }

#if !DOTWEEN_AVAILABLE
        private IEnumerator GlowPulseAnimation()
        {
            while (true)
            {
                float elapsed = 0f;
                float startAlpha = 0.3f;
                float endAlpha = 0.6f;

                while (elapsed < pulseDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Sin(elapsed / pulseDuration * Mathf.PI);
                    if (rarityGlow != null)
                    {
                        Color c = rarityGlow.color;
                        c.a = Mathf.Lerp(startAlpha, endAlpha, t);
                        rarityGlow.color = c;
                    }
                    yield return null;
                }
            }
        }
#endif
    }
}
