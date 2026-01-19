using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CelestialMerge;

namespace CelestialMerge.Physics
{
    /// <summary>
    /// Verwaltet visuelles und auditives Feedback für Physics Events
    /// - Collision Particles basierend auf Velocity und Rarity
    /// - Audio Sounds mit Pitch-Anpassung
    /// - Camera Shake für starke Kollisionen
    /// - Settlement Particles
    /// </summary>
    public class CollisionFeedbackManager : MonoBehaviour
    {
        public static CollisionFeedbackManager Instance { get; private set; }

        [Header("Particle Prefabs")]
        [SerializeField] private GameObject smallParticlePrefab;   // Velocity < 2
        [SerializeField] private GameObject mediumParticlePrefab;  // Velocity 2-5
        [SerializeField] private GameObject largeParticlePrefab;   // Velocity > 5

        [Header("Audio Configuration")]
        [SerializeField] private AudioClip collisionSoftClip;
        [SerializeField] private AudioClip collisionMediumClip;
        [SerializeField] private AudioClip collisionHardClip;
        [SerializeField] private AudioClip settlementClip;
        [SerializeField] private AudioSource audioSource;

        [Header("Camera Shake")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float shakeIntensityMultiplier = 0.1f;
        [SerializeField] private float maxShake = 0.3f;

        [Header("Rarity Colors")]
        [SerializeField] private Color commonColor = new Color(0.5f, 0.5f, 1f);      // Blue
        [SerializeField] private Color uncommonColor = new Color(0.2f, 0.9f, 0.3f); // Green
        [SerializeField] private Color rareColor = new Color(0.2f, 0.5f, 1f);       // Blue
        [SerializeField] private Color epicColor = new Color(0.7f, 0.2f, 1f);       // Purple
        [SerializeField] private Color legendaryColor = new Color(1f, 0.5f, 0.1f);  // Orange/Gold
        [SerializeField] private Color mythicColor = new Color(1f, 0.1f, 0.1f);     // Red

        private Queue<GameObject> particlePool = new Queue<GameObject>();
        private const int POOL_SIZE = 50;

        #region Initialization

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Auto-Find AudioSource
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            // Auto-Find Camera
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            // Initialize Particle Pool
            InitializeParticlePool();

            // Subscribe to Physics Events
            if (CelestialPhysicsManager.Instance != null)
            {
                CelestialPhysicsManager.Instance.OnCollision += HandleCollision;
                CelestialPhysicsManager.Instance.OnSettlement += HandleSettlement;
            }
        }

        private void OnDestroy()
        {
            if (CelestialPhysicsManager.Instance != null)
            {
                CelestialPhysicsManager.Instance.OnCollision -= HandleCollision;
                CelestialPhysicsManager.Instance.OnSettlement -= HandleSettlement;
            }
        }

        #endregion

        #region Particle Pooling

        /// <summary>
        /// Initialisiert Object Pool für Particle-Effekte (Performance-Optimierung)
        /// </summary>
        private void InitializeParticlePool()
        {
            // Verwende Medium-Prefab als Standard (kann erweitert werden)
            GameObject basePrefab = mediumParticlePrefab != null ? mediumParticlePrefab : CreateFallbackParticlePrefab();

            for (int i = 0; i < POOL_SIZE; i++)
            {
                GameObject particle = Instantiate(basePrefab);
                particle.SetActive(false);
                particle.transform.SetParent(transform);
                particlePool.Enqueue(particle);
            }
        }

        /// <summary>
        /// Erstellt Fallback Particle Prefab falls kein Prefab zugewiesen wurde
        /// </summary>
        private GameObject CreateFallbackParticlePrefab()
        {
            GameObject prefab = new GameObject("FallbackParticle");
            ParticleSystem ps = prefab.AddComponent<ParticleSystem>();
            
            var main = ps.main;
            main.startLifetime = 1f;
            main.startSpeed = 5f;
            main.startSize = 0.1f;
            main.maxParticles = 20;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 20)
            });

            return prefab;
        }

        /// <summary>
        /// Holt Particle aus Pool oder erstellt neuen
        /// </summary>
        private GameObject GetParticleFromPool()
        {
            if (particlePool.Count > 0)
            {
                return particlePool.Dequeue();
            }
            else
            {
                // Pool erschöpft - erstelle neuen
                GameObject prefab = mediumParticlePrefab != null ? mediumParticlePrefab : CreateFallbackParticlePrefab();
                return Instantiate(prefab);
            }
        }

        /// <summary>
        /// Gibt Particle zurück an Pool
        /// </summary>
        private void ReturnParticleToPool(GameObject particle)
        {
            if (particle != null)
            {
                particle.SetActive(false);
                particle.transform.SetParent(transform);
                particlePool.Enqueue(particle);
            }
        }

        #endregion

        #region Collision Feedback

        /// <summary>
        /// Behandelt Collision Event aus Physics Manager
        /// </summary>
        private void HandleCollision(PhysicsItem item1, PhysicsItem item2)
        {
            // Berechne Collision Velocity
            float collisionVelocity = (item1.Velocity - item2.Velocity).magnitude;

            // Spawn Particles basierend auf Velocity
            Vector3 collisionPoint = (item1.Position + item2.Position) / 2f;
            SpawnCollisionParticles(collisionPoint, collisionVelocity, item1.ItemData.Rarity);

            // Play Sound basierend auf Velocity
            PlayCollisionSound(collisionVelocity, item1.ItemData.Rarity);

            // Camera Shake für starke Kollisionen
            if (collisionVelocity > 5f)
            {
                float shakeIntensity = Mathf.Min(collisionVelocity * shakeIntensityMultiplier, maxShake);
                float shakeDuration = Mathf.Clamp(collisionVelocity * 0.1f, 0.1f, 0.5f);
                StartCoroutine(CameraShakeCoroutine(shakeDuration, shakeIntensity));
            }
        }

        /// <summary>
        /// Spawnt Particle-Effekt basierend auf Velocity und Rarity
        /// </summary>
        private void SpawnCollisionParticles(Vector3 position, float velocity, ItemRarity rarity)
        {
            // Wähle Particle Prefab basierend auf Velocity
            GameObject prefab = null;
            if (velocity > 5f)
            {
                prefab = largeParticlePrefab != null ? largeParticlePrefab : mediumParticlePrefab;
            }
            else if (velocity >= 2f)
            {
                prefab = mediumParticlePrefab;
            }
            else
            {
                prefab = smallParticlePrefab != null ? smallParticlePrefab : mediumParticlePrefab;
            }

            // Hole Particle aus Pool
            GameObject particle = GetParticleFromPool();
            if (particle != null)
            {
                particle.transform.position = position;
                particle.SetActive(true);

                // Setze Farbe basierend auf Rarity
                ParticleSystem ps = particle.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    main.startColor = GetRarityColor(rarity);
                }

                // Return zu Pool nach Particle Lifetime
                StartCoroutine(ReturnParticleAfterDelay(particle, 2f));
            }
        }

        /// <summary>
        /// Spielt Collision Sound basierend auf Velocity und Rarity
        /// </summary>
        private void PlayCollisionSound(float velocity, ItemRarity rarity)
        {
            if (audioSource == null) return;

            AudioClip clip = null;
            float pitch = 1f;

            // Wähle Clip basierend auf Velocity
            if (velocity > 5f)
            {
                clip = collisionHardClip;
                pitch = 1.2f; // Höherer Pitch für starke Kollision
            }
            else if (velocity >= 2f)
            {
                clip = collisionMediumClip;
                pitch = 1f;
            }
            else
            {
                clip = collisionSoftClip;
                pitch = 0.8f; // Niedrigerer Pitch für leichte Kollision
            }

            // Pitch-Anpassung basierend auf Rarity
            switch (rarity)
            {
                case ItemRarity.Common:
                    pitch *= 0.9f; // Niedriger Pitch
                    break;
                case ItemRarity.Uncommon:
                    pitch *= 0.95f;
                    break;
                case ItemRarity.Rare:
                    pitch *= 1.0f;
                    break;
                case ItemRarity.Epic:
                    pitch *= 1.1f; // Höherer Pitch
                    break;
                case ItemRarity.Legendary:
                    pitch *= 1.2f;
                    break;
                case ItemRarity.Mythic:
                    pitch *= 1.3f; // Höchster Pitch
                    break;
            }

            if (clip != null)
            {
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(clip, 0.5f); // Volume: 50%
            }
        }

        #endregion

        #region Settlement Feedback

        /// <summary>
        /// Behandelt Settlement Event (Item landet auf Board)
        /// </summary>
        private void HandleSettlement(PhysicsItem item)
        {
            // Spawnt kleine Dust Cloud
            SpawnSettlementParticles(item.Position);

            // Spielt weichen Landing Sound
            PlaySettlementSound();

            // Visueller Glow Pulse (optional - kann später erweitert werden)
            // StartCoroutine(GlowPulseCoroutine(item.Transform, 0.3f));
        }

        /// <summary>
        /// Spawnt Settlement Particle (kleine Dust Cloud)
        /// </summary>
        private void SpawnSettlementParticles(Vector3 position)
        {
            GameObject particle = GetParticleFromPool();
            if (particle != null)
            {
                particle.transform.position = position;
                particle.SetActive(true);

                ParticleSystem ps = particle.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    main.startColor = new Color(0.8f, 0.8f, 0.8f, 0.5f); // Grau für Dust
                    main.startSize = 0.05f; // Kleinere Particles für Settlement
                    main.maxParticles = 10; // Weniger Particles
                }

                StartCoroutine(ReturnParticleAfterDelay(particle, 1f));
            }
        }

        /// <summary>
        /// Spielt Settlement Sound
        /// </summary>
        private void PlaySettlementSound()
        {
            if (audioSource != null && settlementClip != null)
            {
                audioSource.pitch = 0.9f; // Niedriger, weicher Pitch
                audioSource.PlayOneShot(settlementClip, 0.3f); // Volume: 30% (leise)
            }
        }

        #endregion

        #region Camera Shake

        /// <summary>
        /// Camera Shake Coroutine
        /// </summary>
        private IEnumerator CameraShakeCoroutine(float duration, float intensity)
        {
            if (mainCamera == null) yield break;

            Vector3 originalPosition = mainCamera.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;

                mainCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0f);

                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCamera.transform.localPosition = originalPosition;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gibt Farbe basierend auf Rarity zurück
        /// </summary>
        private Color GetRarityColor(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common: return commonColor;
                case ItemRarity.Uncommon: return uncommonColor;
                case ItemRarity.Rare: return rareColor;
                case ItemRarity.Epic: return epicColor;
                case ItemRarity.Legendary: return legendaryColor;
                case ItemRarity.Mythic: return mythicColor;
                default: return Color.white;
            }
        }

        /// <summary>
        /// Coroutine: Gibt Particle nach Delay zurück an Pool
        /// </summary>
        private IEnumerator ReturnParticleAfterDelay(GameObject particle, float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnParticleToPool(particle);
        }

        #endregion
    }
}
