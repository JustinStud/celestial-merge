using UnityEngine;

namespace MergeWellness
{
    /// <summary>
    /// Stellt sicher, dass nur ein AudioListener in der Szene aktiv ist
    /// </summary>
    public class AudioListenerManager : MonoBehaviour
    {
        private void Awake()
        {
            // Finde alle AudioListener in der Szene
            AudioListener[] audioListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
            
            if (audioListeners.Length > 1)
            {
                Debug.LogWarning($"Mehrere AudioListener gefunden ({audioListeners.Length}). Deaktiviere alle bis auf den ersten.");
                
                // Behalte nur den ersten AudioListener aktiv (normalerweise die Main Camera)
                // Deaktiviere alle anderen
                for (int i = 1; i < audioListeners.Length; i++)
                {
                    audioListeners[i].enabled = false;
                    Debug.Log($"AudioListener deaktiviert auf: {audioListeners[i].gameObject.name}");
                }
                
                Debug.Log($"✓ AudioListener-Manager: {audioListeners.Length - 1} AudioListener(s) deaktiviert. Nur noch 1 aktiv.");
            }
            else if (audioListeners.Length == 1)
            {
                Debug.Log($"✓ AudioListener-Manager: Genau ein AudioListener gefunden auf {audioListeners[0].gameObject.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ AudioListener-Manager: Kein AudioListener gefunden!");
            }
        }
    }
}
