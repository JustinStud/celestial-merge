using UnityEngine;
using System.Collections.Generic;
using CelestialMerge;

namespace CelestialMerge.Physics
{
    /// <summary>
    /// Zentrale Physics-Engine für Celestial Merge
    /// Simuliert Gravity, Collisions, Bounce und Settlement
    /// Performance-optimiert für 100+ simultane Items
    /// </summary>
    public class CelestialPhysicsManager : MonoBehaviour
    {
        public static CelestialPhysicsManager Instance { get; private set; }

        [Header("Physics Configuration")]
        [SerializeField] private float gravity = 9.81f;              // m/s²
        [SerializeField] private float drag = 0.1f;                  // Air resistance (0-1)
        [SerializeField] private float restitution = 0.6f;           // Bounce factor (0-1, where 1 = perfect bounce)
        [SerializeField] private float settlementThreshold = 0.1f;   // Velocity threshold for settling
        [SerializeField] private float fixedDeltaTime = 0.02f;       // Physics timestep (50 Hz)
        [SerializeField] private float cellSize = 1.0f;              // Board grid size for snapping

        [Header("Performance")]
        [SerializeField] private float physicsCheckInterval = 0.1f;  // Update physics every 100ms
        [SerializeField] private int maxPhysicsItems = 200;          // Maximum items to simulate

        [Header("References")]
        [SerializeField] private ExpandableBoardManager boardManager;

        private List<PhysicsItem> physicsItems = new List<PhysicsItem>();
        private float nextPhysicsCheckTime = 0f;
        private bool isInitialized = false;

        // Events
        public System.Action<PhysicsItem, PhysicsItem> OnCollision;
        public System.Action<PhysicsItem> OnSettlement;

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
            if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<ExpandableBoardManager>();
            }

            Time.fixedDeltaTime = fixedDeltaTime;
            isInitialized = true;
            Debug.Log($"✅ CelestialPhysicsManager initialisiert (Gravity: {gravity} m/s², Restitution: {restitution})");
        }

        #endregion

        #region Physics Update Loop

        private void FixedUpdate()
        {
            if (!isInitialized || physicsItems.Count == 0) return;

            // Performance: Update physics nur in Intervallen
            if (Time.time >= nextPhysicsCheckTime)
            {
                UpdatePhysics();
                nextPhysicsCheckTime = Time.time + physicsCheckInterval;
            }
        }

        /// <summary>
        /// Haupt-Physics-Update: Gravity → Collisions → Position Update
        /// </summary>
        private void UpdatePhysics()
        {
            if (physicsItems.Count == 0) return;

            // Step 1: Apply gravity and drag
            for (int i = 0; i < physicsItems.Count; i++)
            {
                PhysicsItem item = physicsItems[i];
                if (!item.IsSettled && !item.IsStatic)
                {
                    ApplyGravity(ref item);
                    ApplyDrag(ref item);
                    physicsItems[i] = item;
                }
            }

            // Step 2: Detect and resolve collisions
            ResolveCollisions();

            // Step 3: Update positions
            UpdatePositions();

            // Step 4: Check settlements and board boundaries
            CheckSettlementsAndBoundaries();
        }

        /// <summary>
        /// Wendet Schwerkraft an: v += g * dt
        /// </summary>
        private void ApplyGravity(ref PhysicsItem item)
        {
            // Gravity wirkt nach unten (Y-Achse)
            item.Velocity.y -= gravity * Time.fixedDeltaTime;
        }

        /// <summary>
        /// Wendet Luftwiderstand an: v *= (1 - drag * dt)
        /// </summary>
        private void ApplyDrag(ref PhysicsItem item)
        {
            float dragFactor = 1f - (drag * Time.fixedDeltaTime);
            item.Velocity *= Mathf.Max(0f, dragFactor); // Verhindere negative Velocity
        }

        /// <summary>
        /// Erkennt und löst Kollisionen zwischen Items
        /// </summary>
        private void ResolveCollisions()
        {
            // Prüfe alle Item-Paare für Kollisionen
            for (int i = 0; i < physicsItems.Count; i++)
            {
                for (int j = i + 1; j < physicsItems.Count; j++)
                {
                    PhysicsItem item1 = physicsItems[i];
                    PhysicsItem item2 = physicsItems[j];

                    if (item1.IsStatic && item2.IsStatic) continue; // Skip static items

                    if (IsColliding(item1, item2))
                    {
                        ResolveCollision(ref item1, ref item2);
                        physicsItems[i] = item1;
                        physicsItems[j] = item2;
                    }
                }

            }
        }

        /// <summary>
        /// AABB (Axis-Aligned Bounding Box) Kollisionserkennung
        /// </summary>
        private bool IsColliding(PhysicsItem item1, PhysicsItem item2)
        {
            Bounds bounds1 = item1.GetBounds();
            Bounds bounds2 = item2.GetBounds();
            return bounds1.Intersects(bounds2);
        }

        /// <summary>
        /// Löst Kollision zwischen zwei Items
        /// </summary>
        private void ResolveCollision(ref PhysicsItem item1, ref PhysicsItem item2)
        {
            // Berechne Kollisionsnormal
            Vector3 normal = (item2.Position - item1.Position).normalized;
            if (normal == Vector3.zero) normal = Vector3.up; // Fallback

            // Relative Velocity
            Vector3 relativeVelocity = item2.Velocity - item1.Velocity;

            // Velocity entlang Kollisionsnormal
            float velAlongNormal = Vector3.Dot(relativeVelocity, normal);

            // Überspringe wenn Objekte sich trennen
            if (velAlongNormal > 0) return;

            // Impuls-Skalar berechnen
            float newVelAlongNormal = -velAlongNormal * restitution;
            float velChangePerObject = (newVelAlongNormal - velAlongNormal) / 2f;

            // Impuls anwenden
            if (!item1.IsStatic)
            {
                item1.Velocity -= velChangePerObject * normal;
            }
            if (!item2.IsStatic)
            {
                item2.Velocity += velChangePerObject * normal;
            }

            // Objekte trennen um Überlappung zu verhindern
            SeparateObjects(ref item1, ref item2);

            // Trigger Event
            OnCollision?.Invoke(item1, item2);
        }

        /// <summary>
        /// Trennt überlappende Objekte
        /// </summary>
        private void SeparateObjects(ref PhysicsItem item1, ref PhysicsItem item2)
        {
            Bounds bounds1 = item1.GetBounds();
            Bounds bounds2 = item2.GetBounds();

            Vector3 toItem1 = (item1.Position - item2.Position).normalized;
            if (toItem1 == Vector3.zero) toItem1 = Vector3.up; // Fallback

            float distance = Vector3.Distance(item1.Position, item2.Position);
            float minDistance = bounds1.extents.magnitude + bounds2.extents.magnitude;
            float overlap = minDistance - distance;

            if (overlap > 0)
            {
                float separation = (overlap / 2f) + 0.01f; // Kleine Puffer
                
                if (!item1.IsStatic)
                {
                    item1.Position += toItem1 * separation;
                }
                if (!item2.IsStatic)
                {
                    item2.Position -= toItem1 * separation;
                }
            }
        }

        /// <summary>
        /// Behandelt Kollisionen mit Board-Grenzen
        /// </summary>
        private void HandleBoardCollisions(ref PhysicsItem item)
        {
            if (item.IsStatic) return;

            // Boden-Kollision
            if (item.Position.y <= 0)
            {
                item.Position.y = 0;
                item.Velocity.y *= -restitution; // Bounce
            }

            // Seitliche Grenzen (kann erweitert werden basierend auf Board-Größe)
            if (boardManager != null)
            {
                float boardWidth = boardManager.CurrentWidth * cellSize;
                float boardHeight = boardManager.CurrentHeight * cellSize;

                // X-Grenzen
                if (item.Position.x < 0)
                {
                    item.Position.x = 0;
                    item.Velocity.x *= -restitution;
                }
                else if (item.Position.x > boardWidth)
                {
                    item.Position.x = boardWidth;
                    item.Velocity.x *= -restitution;
                }
            }
        }

        /// <summary>
        /// Aktualisiert Positionen: x += v * dt
        /// </summary>
        private void UpdatePositions()
        {
            for (int i = 0; i < physicsItems.Count; i++)
            {
                PhysicsItem item = physicsItems[i];
                
                if (!item.IsSettled && !item.IsStatic)
                {
                    // x = x + v * t
                    item.Position += item.Velocity * Time.fixedDeltaTime;

                    // Transform aktualisieren
                    if (item.Transform != null)
                    {
                        item.Transform.position = item.Position;
                    }
                }

                physicsItems[i] = item;
            }
        }

        /// <summary>
        /// Prüft ob Items settled sind und snapped zum Grid, sowie Board-Grenzen
        /// </summary>
        private void CheckSettlementsAndBoundaries()
        {
            for (int i = 0; i < physicsItems.Count; i++)
            {
                PhysicsItem item = physicsItems[i];
                
                // Prüfe Board-Grenzen
                HandleBoardCollisions(ref item);
                
                if (!item.IsSettled && !item.IsStatic && item.Position.y <= 0.1f)
                {
                    // Prüfe ob Velocity unter Threshold
                    if (!item.IsMoving(settlementThreshold))
                    {
                        item.IsSettled = true;
                        item.Velocity = Vector3.zero;
                        item.Position = SnapToGrid(item.Position);

                        if (item.Transform != null)
                        {
                            item.Transform.position = item.Position;
                        }

                        OnSettlement?.Invoke(item);
                    }
                }
                
                // Zurück schreiben
                physicsItems[i] = item;
            }
        }

        /// <summary>
        /// Snapped Position zum Grid
        /// </summary>
        private Vector3 SnapToGrid(Vector3 position)
        {
            float snappedX = Mathf.Round(position.x / cellSize) * cellSize;
            float snappedY = Mathf.Max(0, Mathf.Round(position.y / cellSize) * cellSize);
            float snappedZ = Mathf.Round(position.z / cellSize) * cellSize;
            return new Vector3(snappedX, snappedY, snappedZ);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Registriert ein Item für Physics-Simulation
        /// </summary>
        public void RegisterItem(Transform itemTransform, CelestialItem itemData, bool isStatic = false)
        {
            if (itemTransform == null || itemData == null)
            {
                Debug.LogError("RegisterItem: Transform oder ItemData ist null!");
                return;
            }

            if (physicsItems.Count >= maxPhysicsItems)
            {
                Debug.LogWarning($"⚠️ Maximale Anzahl Physics-Items erreicht ({maxPhysicsItems})!");
                return;
            }

            var physicsItem = new PhysicsItem
            {
                Transform = itemTransform,
                ItemData = itemData,
                Position = itemTransform.position,
                Velocity = Vector3.zero,
                IsStatic = isStatic,
                IsSettled = isStatic,
                Mass = 1f // Standard-Masse
            };

            physicsItems.Add(physicsItem);
            Debug.Log($"✅ Item registriert für Physics: {itemData.ItemName} (Static: {isStatic})");
        }

        /// <summary>
        /// Entfernt Item aus Physics-Simulation
        /// </summary>
        public void UnregisterItem(Transform itemTransform)
        {
            if (itemTransform == null) return;

            int removed = physicsItems.RemoveAll(item => item.Transform == itemTransform);
            if (removed > 0)
            {
                Debug.Log($"✅ Item aus Physics entfernt: {itemTransform.name}");
            }
        }

        /// <summary>
        /// Fügt Kraft zu einem Item hinzu
        /// </summary>
        public void AddForce(Transform itemTransform, Vector3 force)
        {
            if (itemTransform == null) return;

            for (int i = 0; i < physicsItems.Count; i++)
            {
                if (physicsItems[i].Transform == itemTransform && !physicsItems[i].IsStatic)
                {
                    PhysicsItem item = physicsItems[i];
                    item.Velocity += force / item.Mass; // F = m * a, daher a = F / m
                    item.IsSettled = false; // Item bewegt sich wieder
                    physicsItems[i] = item;
                    return;
                }
            }
        }

        /// <summary>
        /// Setzt alle Items sofort auf settled
        /// </summary>
        public void SettleAllItems()
        {
            for (int i = 0; i < physicsItems.Count; i++)
            {
                PhysicsItem item = physicsItems[i];
                item.Velocity = Vector3.zero;
                item.IsSettled = true;
                item.Position = SnapToGrid(item.Position);

                if (item.Transform != null)
                {
                    item.Transform.position = item.Position;
                }

                physicsItems[i] = item;
                OnSettlement?.Invoke(item);
            }
        }

        /// <summary>
        /// Gibt Anzahl aktiver Physics-Items zurück
        /// </summary>
        public int GetActivePhysicsItemCount()
        {
            return physicsItems.Count;
        }

        #endregion

        #region Debug

        private void OnDrawGizmos()
        {
            // Zeichne Physics-Items im Editor (nur für Debugging)
            if (!Application.isPlaying) return;

            foreach (var item in physicsItems)
            {
                if (item.IsSettled)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawWireSphere(item.Position, 0.2f);
            }
        }

        #endregion
    }
}
