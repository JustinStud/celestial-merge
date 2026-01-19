using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CelestialMerge.Visual
{
    /// <summary>
    /// System für visuelles Feedback bei Merges
    /// Particle Effects, Animations, Reward Pop-ups
    /// </summary>
    public class MergeFeedbackSystem : MonoBehaviour
    {
        public static MergeFeedbackSystem Instance { get; private set; }

        [Header("Particle Effects")]
        [SerializeField] private GameObject mergeParticlePrefab;
        [SerializeField] private GameObject epicMergeParticlePrefab;
        [SerializeField] private GameObject legendaryMergeParticlePrefab;
        [SerializeField] private int particleCount = 20;

        [Header("Reward Pop-ups")]
        [SerializeField] private GameObject rewardTextPrefab;
        [SerializeField] private Transform rewardTextParent;
        [SerializeField] private float popupDuration = 2f;

        [Header("Screen Shake")]
        [SerializeField] private bool enableScreenShake = true;
        [SerializeField] private float shakeIntensity = 0.1f;
        [SerializeField] private float shakeDuration = 0.3f;

        [Header("Colors")]
        [SerializeField] private Color commonColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Grau
        [SerializeField] private Color uncommonColor = new Color(0f, 1f, 0f, 1f); // Grün
        [SerializeField] private Color rareColor = new Color(0f, 0.5f, 1f, 1f); // Blau
        [SerializeField] private Color epicColor = new Color(0.5f, 0f, 1f, 1f); // Lila
        [SerializeField] private Color legendaryColor = new Color(1f, 0.84f, 0f, 1f); // Gold
        [SerializeField] private Color mythicColor = new Color(1f, 0f, 1f, 1f); // Magenta

        private Camera mainCamera;
        private Vector3 originalCameraPosition;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                originalCameraPosition = mainCamera.transform.position;
            }

            // Erstelle Reward Text Parent falls nicht vorhanden
            if (rewardTextParent == null)
            {
                GameObject parent = new GameObject("RewardTextParent");
                parent.transform.SetParent(transform);
                Canvas canvas = FindFirstObjectByType<Canvas>();
                if (canvas != null)
                {
                    parent.transform.SetParent(canvas.transform, false);
                    RectTransform rect = parent.AddComponent<RectTransform>();
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.sizeDelta = Vector2.zero;
                    rect.anchoredPosition = Vector2.zero;
                    rewardTextParent = parent.transform;
                }
            }
        }

        /// <summary>
        /// Zeigt Merge-Feedback (Particles, Animation, etc.)
        /// </summary>
        public void ShowMergeFeedback(Vector3 worldPosition, ItemRarity rarity, bool isThreeMerge = false)
        {
            // Particle Effects
            ShowMergeParticles(worldPosition, rarity, isThreeMerge);

            // Screen Shake (nur bei höheren Rarities)
            if (rarity >= ItemRarity.Epic && enableScreenShake)
            {
                StartCoroutine(ScreenShake());
            }
        }

        /// <summary>
        /// Zeigt Reward Pop-up (Stardust, XP, etc.)
        /// </summary>
        public void ShowRewardPopup(Vector3 worldPosition, string text, Color color)
        {
            if (rewardTextPrefab == null)
            {
                // Erstelle einfachen Text Pop-up falls kein Prefab vorhanden
                CreateSimpleRewardText(worldPosition, text, color);
                return;
            }

            // Konvertiere World Position zu Screen Position
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);

            // Instantiere Reward Text
            GameObject rewardObj = Instantiate(rewardTextPrefab, rewardTextParent);
            RectTransform rect = rewardObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = screenPos;
            }

            // Setze Text
            TMPro.TextMeshProUGUI textComponent = rewardObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = text;
                textComponent.color = color;
            }

            // Starte Animation
            StartCoroutine(AnimateRewardText(rewardObj));
        }

        /// <summary>
        /// Zeigt Stardust Reward
        /// </summary>
        public void ShowStardustReward(Vector3 worldPosition, long amount)
        {
            ShowRewardPopup(worldPosition, $"+{FormatNumber(amount)} Stardust", Color.yellow);
        }

        /// <summary>
        /// Zeigt XP Reward
        /// </summary>
        public void ShowXPReward(Vector3 worldPosition, long amount)
        {
            ShowRewardPopup(worldPosition, $"+{FormatNumber(amount)} XP", new Color(0.2f, 0.8f, 1f));
        }

        /// <summary>
        /// Zeigt Crystal Reward
        /// </summary>
        public void ShowCrystalReward(Vector3 worldPosition, int amount)
        {
            ShowRewardPopup(worldPosition, $"+{amount} Crystals", new Color(0.5f, 0.8f, 1f));
        }

        /// <summary>
        /// Zeigt Merge Particles
        /// </summary>
        private void ShowMergeParticles(Vector3 position, ItemRarity rarity, bool isThreeMerge)
        {
            GameObject particlePrefab = GetParticlePrefabForRarity(rarity);
            if (particlePrefab == null)
            {
                // Erstelle einfache Partikel falls kein Prefab vorhanden
                CreateSimpleParticles(position, GetRarityColor(rarity));
                return;
            }

            GameObject particles = Instantiate(particlePrefab, position, Quaternion.identity);
            Destroy(particles, 3f);
        }

        /// <summary>
        /// Erstellt einfache Partikel (falls kein Prefab vorhanden)
        /// </summary>
        private void CreateSimpleParticles(Vector3 position, Color color)
        {
            // Erstelle Partikel-System zur Laufzeit
            GameObject particleObj = new GameObject("MergeParticles");
            particleObj.transform.position = position;

            ParticleSystem particles = particleObj.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particles.main;
            main.startColor = color;
            main.startSize = 0.2f;
            main.startLifetime = 1f;
            main.maxParticles = particleCount;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, particleCount)
            });

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.5f;

            Destroy(particleObj, 2f);
        }

        /// <summary>
        /// Erstellt einfachen Reward Text (falls kein Prefab vorhanden)
        /// </summary>
        private void CreateSimpleRewardText(Vector3 worldPosition, string text, Color color)
        {
            GameObject textObj = new GameObject("RewardText");
            textObj.transform.SetParent(rewardTextParent, false);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);
            rect.anchoredPosition = screenPos;
            rect.sizeDelta = new Vector2(200, 50);

            TMPro.TextMeshProUGUI textComponent = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 24;
            textComponent.fontStyle = TMPro.FontStyles.Bold;
            textComponent.color = color;
            textComponent.alignment = TMPro.TextAlignmentOptions.Center;
            textComponent.raycastTarget = false;

            StartCoroutine(AnimateRewardText(textObj));
        }

        /// <summary>
        /// Animiert Reward Text (Float Up + Fade Out)
        /// </summary>
        private IEnumerator AnimateRewardText(GameObject textObj)
        {
            if (textObj == null) yield break;

            RectTransform rect = textObj.GetComponent<RectTransform>();
            TMPro.TextMeshProUGUI text = textObj.GetComponent<TMPro.TextMeshProUGUI>();
            if (rect == null || text == null) yield break;

            Vector2 startPos = rect.anchoredPosition;
            Vector2 endPos = startPos + new Vector2(0, 100);
            float elapsed = 0f;

            while (elapsed < popupDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / popupDuration;

                // Float Up
                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

                // Fade Out
                Color c = text.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                text.color = c;

                yield return null;
            }

            Destroy(textObj);
        }

        /// <summary>
        /// Screen Shake Effect
        /// </summary>
        private IEnumerator ScreenShake()
        {
            if (mainCamera == null) yield break;

            float elapsed = 0f;
            Vector3 originalPos = mainCamera.transform.position;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                Vector3 offset = Random.insideUnitSphere * shakeIntensity;
                mainCamera.transform.position = originalPos + offset;
                yield return null;
            }

            mainCamera.transform.position = originalPos;
        }

        /// <summary>
        /// Gibt Particle Prefab für Rarity zurück
        /// </summary>
        private GameObject GetParticlePrefabForRarity(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Epic => epicMergeParticlePrefab,
                ItemRarity.Legendary => legendaryMergeParticlePrefab,
                ItemRarity.Mythic => legendaryMergeParticlePrefab,
                _ => mergeParticlePrefab
            };
        }

        /// <summary>
        /// Gibt Farbe für Rarity zurück
        /// </summary>
        public Color GetRarityColor(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => commonColor,
                ItemRarity.Uncommon => uncommonColor,
                ItemRarity.Rare => rareColor,
                ItemRarity.Epic => epicColor,
                ItemRarity.Legendary => legendaryColor,
                ItemRarity.Mythic => mythicColor,
                _ => Color.white
            };
        }

        /// <summary>
        /// Formatiert Zahl (K, M)
        /// </summary>
        private string FormatNumber(long number)
        {
            if (number >= 1000000)
                return $"{number / 1000000f:F1}M";
            if (number >= 1000)
                return $"{number / 1000f:F1}K";
            return number.ToString();
        }
    }
}
