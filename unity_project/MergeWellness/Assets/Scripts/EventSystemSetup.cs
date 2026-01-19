using UnityEngine;
using UnityEngine.EventSystems;

namespace MergeWellness
{
    /// <summary>
    /// Stellt sicher, dass EventSystem für Drag-Drop vorhanden ist
    /// </summary>
    public class EventSystemSetup : MonoBehaviour
    {
        private void Awake()
        {
            // Prüfe ob EventSystem vorhanden ist
            EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
                Debug.Log("EventSystem automatisch erstellt");
            }
        }
    }
}
