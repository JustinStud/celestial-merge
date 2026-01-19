using UnityEngine;

namespace CelestialMerge.Physics
{
    /// <summary>
    /// Physik-Datenstruktur für ein Item mit Position, Velocity und Bounds
    /// </summary>
    [System.Serializable]
    public struct PhysicsItem
    {
        public Transform Transform;
        public CelestialItem ItemData;
        public Vector3 Position;
        public Vector3 Velocity;
        public bool IsStatic;
        public bool IsSettled;
        public float Mass;

        /// <summary>
        /// Gibt Bounding Box für Kollisionserkennung zurück
        /// </summary>
        public Bounds GetBounds()
        {
            // Standard-Größe für Items (kann später angepasst werden)
            float size = 0.8f;
            return new Bounds(Position, Vector3.one * size);
        }

        /// <summary>
        /// Prüft ob Item Bewegung hat
        /// </summary>
        public bool IsMoving(float threshold = 0.1f)
        {
            return Velocity.magnitude > threshold;
        }
    }
}
