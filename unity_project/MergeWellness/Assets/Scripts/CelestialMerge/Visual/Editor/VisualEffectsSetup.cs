using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using CelestialMerge.Visual;

namespace CelestialMerge.Visual.Editor
{
    /// <summary>
    /// Editor-Tool zum automatischen Setup von Visual Effects
    /// </summary>
    public class VisualEffectsSetup : EditorWindow
    {
        [MenuItem("CelestialMerge/Visual/Setup Item Visual Effects")]
        public static void ShowWindow()
        {
            GetWindow<VisualEffectsSetup>("Visual Effects Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Visual Effects Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "Dieses Tool fügt automatisch ItemVisualEffects zu allen Items hinzu.\n" +
                "Erstellt auch Rarity Borders und Glows.",
                MessageType.Info
            );

            GUILayout.Space(10);

            if (GUILayout.Button("🔧 Setup All Items", GUILayout.Height(40)))
            {
                SetupAllItems();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("🎨 Setup MergeFeedbackSystem", GUILayout.Height(30)))
            {
                SetupMergeFeedbackSystem();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("✅ Verify Setup", GUILayout.Height(30)))
            {
                VerifySetup();
            }
        }

        private void SetupAllItems()
        {
            int setupCount = 0;

            // Finde alle Items (GameObjects mit Image Component)
            Image[] allImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
            
            foreach (Image img in allImages)
            {
                // Prüfe ob es ein Item ist (hat RectTransform und ist auf Board)
                RectTransform rect = img.GetComponent<RectTransform>();
                if (rect == null) continue;

                // Prüfe ob bereits ItemVisualEffects vorhanden
                ItemVisualEffects existing = img.GetComponent<ItemVisualEffects>();
                if (existing != null) continue;

                // Füge ItemVisualEffects hinzu
                ItemVisualEffects effects = img.gameObject.AddComponent<ItemVisualEffects>();

                // Erstelle Rarity Border (optional)
                CreateRarityBorder(img.gameObject);

                // Erstelle Rarity Glow (optional)
                CreateRarityGlow(img.gameObject);

                setupCount++;
                EditorUtility.SetDirty(img.gameObject);
            }

            EditorUtility.DisplayDialog("Erfolg", 
                $"✅ {setupCount} Items mit Visual Effects ausgestattet!\n\n" +
                $"Rarity Borders und Glows wurden erstellt.",
                "OK");
            
            Debug.Log($"✅ {setupCount} Items mit Visual Effects ausgestattet");
        }

        private void CreateRarityBorder(GameObject itemObj)
        {
            // Prüfe ob Border bereits existiert
            if (itemObj.transform.Find("RarityBorder") != null) return;

            GameObject borderObj = new GameObject("RarityBorder");
            borderObj.transform.SetParent(itemObj.transform, false);

            RectTransform borderRect = borderObj.AddComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.sizeDelta = Vector2.zero;
            borderRect.anchoredPosition = Vector2.zero;

            Image borderImage = borderObj.AddComponent<Image>();
            borderImage.color = new Color(1f, 1f, 1f, 0.5f);
            borderImage.raycastTarget = false;

            // Border ist etwas größer als Item
            borderRect.offsetMin = new Vector2(-5, -5);
            borderRect.offsetMax = new Vector2(5, 5);
        }

        private void CreateRarityGlow(GameObject itemObj)
        {
            // Prüfe ob Glow bereits existiert
            if (itemObj.transform.Find("RarityGlow") != null) return;

            GameObject glowObj = new GameObject("RarityGlow");
            glowObj.transform.SetParent(itemObj.transform, false);

            RectTransform glowRect = glowObj.AddComponent<RectTransform>();
            glowRect.anchorMin = Vector2.zero;
            glowRect.anchorMax = Vector2.one;
            glowRect.sizeDelta = Vector2.zero;
            glowRect.anchoredPosition = Vector2.zero;

            Image glowImage = glowObj.AddComponent<Image>();
            glowImage.color = new Color(1f, 1f, 1f, 0.3f);
            glowImage.raycastTarget = false;

            // Glow ist größer als Item
            glowRect.offsetMin = new Vector2(-10, -10);
            glowRect.offsetMax = new Vector2(10, 10);
        }

        private void SetupMergeFeedbackSystem()
        {
            // Prüfe ob bereits vorhanden
            MergeFeedbackSystem existing = FindFirstObjectByType<MergeFeedbackSystem>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("Info", 
                    "✅ MergeFeedbackSystem bereits vorhanden!\n\n" +
                    $"GameObject: {existing.name}",
                    "OK");
                return;
            }

            // Erstelle MergeFeedbackSystem
            GameObject systemObj = new GameObject("MergeFeedbackSystem");
            MergeFeedbackSystem system = systemObj.AddComponent<MergeFeedbackSystem>();

            EditorUtility.DisplayDialog("Erfolg", 
                "✅ MergeFeedbackSystem erstellt!\n\n" +
                "System ist bereit für Visual Feedback.",
                "OK");
            
            Debug.Log("✅ MergeFeedbackSystem erstellt");
        }

        private void VerifySetup()
        {
            System.Text.StringBuilder report = new System.Text.StringBuilder();
            report.AppendLine("🔍 Visual Effects Setup Verification:\n");

            // Prüfe MergeFeedbackSystem
            MergeFeedbackSystem feedbackSystem = FindFirstObjectByType<MergeFeedbackSystem>();
            if (feedbackSystem != null)
            {
                report.AppendLine("✅ MergeFeedbackSystem vorhanden");
            }
            else
            {
                report.AppendLine("❌ MergeFeedbackSystem fehlt");
            }

            // Prüfe Items mit Visual Effects
            ItemVisualEffects[] allEffects = FindObjectsByType<ItemVisualEffects>(FindObjectsSortMode.None);
            report.AppendLine($"✅ {allEffects.Length} Items mit Visual Effects");

            // Prüfe DOTween (optional)
            bool hasDOTween = System.Type.GetType("DG.Tweening.DOTween") != null;
            if (hasDOTween)
            {
                report.AppendLine("✅ DOTween installiert");
            }
            else
            {
                report.AppendLine("⚠️ DOTween nicht installiert (Animationen funktionieren trotzdem)");
            }

            EditorUtility.DisplayDialog("Verification", report.ToString(), "OK");
        }
    }
}
