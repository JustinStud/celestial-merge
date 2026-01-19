using UnityEngine;
using UnityEditor;
using TMPro;

namespace CelestialMerge.Story.Editor
{
    /// <summary>
    /// Editor-Tool zum automatischen Korrigieren der Dialog-Text-Position im StoryUIManager
    /// </summary>
    public class StoryUILayoutFixer : EditorWindow
    {
        [MenuItem("CelestialMerge/Story UI/Fix Dialog Text Layout")]
        public static void ShowWindow()
        {
            GetWindow<StoryUILayoutFixer>("Story UI Layout Fixer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Dialog Text Layout Fixer", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Finde StoryUIManager und fixe Layout", GUILayout.Height(30)))
            {
                FixDialogTextLayout();
            }

            GUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "Dieses Tool stellt sicher, dass dialogText und npcNameText korrekt innerhalb des dialogPanel positioniert sind.",
                MessageType.Info
            );
        }

        private void FixDialogTextLayout()
        {
            // Finde StoryUIManager in der Scene
            StoryUIManager storyUI = FindFirstObjectByType<StoryUIManager>();
            if (storyUI == null)
            {
                Debug.LogError("❌ StoryUIManager nicht in der Scene gefunden!");
                EditorUtility.DisplayDialog("Fehler", "StoryUIManager nicht in der Scene gefunden!", "OK");
                return;
            }

            // Verwende Reflection um private Felder zu lesen
            var storyUIType = typeof(StoryUIManager);
            var dialogPanelField = storyUIType.GetField("dialogPanel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dialogTextField = storyUIType.GetField("dialogText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var npcNameTextField = storyUIType.GetField("npcNameText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (dialogPanelField == null || dialogTextField == null)
            {
                Debug.LogError("❌ Dialog-Felder konnten nicht gefunden werden!");
                return;
            }

            GameObject dialogPanel = dialogPanelField.GetValue(storyUI) as GameObject;
            TextMeshProUGUI dialogText = dialogTextField.GetValue(storyUI) as TextMeshProUGUI;
            TextMeshProUGUI npcNameText = npcNameTextField?.GetValue(storyUI) as TextMeshProUGUI;

            if (dialogPanel == null)
            {
                Debug.LogError("❌ dialogPanel ist null! Bitte im Inspector zuweisen.");
                EditorUtility.DisplayDialog("Fehler", "dialogPanel ist null! Bitte im Inspector zuweisen.", "OK");
                return;
            }

            if (dialogText == null)
            {
                Debug.LogError("❌ dialogText ist null! Bitte im Inspector zuweisen.");
                EditorUtility.DisplayDialog("Fehler", "dialogText ist null! Bitte im Inspector zuweisen.", "OK");
                return;
            }

            // Stelle sicher, dass dialogText ein Child von dialogPanel ist
            if (dialogText.transform.parent != dialogPanel.transform)
            {
                Debug.LogWarning($"⚠️ dialogText ist nicht Child von dialogPanel! Verschiebe...");
                dialogText.transform.SetParent(dialogPanel.transform, true);
            }

            // Fixe RectTransform von dialogText
            RectTransform dialogTextRect = dialogText.GetComponent<RectTransform>();
            if (dialogTextRect != null)
            {
                // Setze Anchor auf Stretch (fülle Panel)
                dialogTextRect.anchorMin = new Vector2(0.1f, 0.2f); // Links-rechts Margin
                dialogTextRect.anchorMax = new Vector2(0.9f, 0.8f); // Oben-unten Margin
                dialogTextRect.offsetMin = Vector2.zero;
                dialogTextRect.offsetMax = Vector2.zero;
                dialogTextRect.anchoredPosition = Vector2.zero;

                // Setze Alignment für Text (oben-links für Dialog-Text)
                dialogText.alignment = TextAlignmentOptions.TopLeft;
                dialogText.textWrappingMode = TextWrappingModes.Normal;

                Debug.Log("✅ dialogText RectTransform gefixt!");
            }

            // Fixe npcNameText falls vorhanden
            if (npcNameText != null)
            {
                // Stelle sicher, dass npcNameText ein Child von dialogPanel ist
                if (npcNameText.transform.parent != dialogPanel.transform)
                {
                    Debug.LogWarning($"⚠️ npcNameText ist nicht Child von dialogPanel! Verschiebe...");
                    npcNameText.transform.SetParent(dialogPanel.transform, true);
                }

                RectTransform npcNameRect = npcNameText.GetComponent<RectTransform>();
                if (npcNameRect != null)
                {
                    // NPC Name oben im Panel
                    npcNameRect.anchorMin = new Vector2(0.1f, 0.85f);
                    npcNameRect.anchorMax = new Vector2(0.9f, 0.95f);
                    npcNameRect.offsetMin = Vector2.zero;
                    npcNameRect.offsetMax = Vector2.zero;
                    npcNameRect.anchoredPosition = Vector2.zero;

                    npcNameText.alignment = TextAlignmentOptions.TopLeft;

                    Debug.Log("✅ npcNameText RectTransform gefixt!");
                }
            }

            EditorUtility.SetDirty(dialogPanel);
            if (dialogText != null) EditorUtility.SetDirty(dialogText.gameObject);
            if (npcNameText != null) EditorUtility.SetDirty(npcNameText.gameObject);

            Debug.Log("✅ Dialog-Text-Layout erfolgreich gefixt!");
            EditorUtility.DisplayDialog("Erfolg", "Dialog-Text-Layout erfolgreich gefixt!", "OK");
        }
    }
}
