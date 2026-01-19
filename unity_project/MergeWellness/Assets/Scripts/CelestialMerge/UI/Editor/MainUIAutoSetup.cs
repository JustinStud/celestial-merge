using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using CelestialMerge.UI;

namespace CelestialMerge.UI.Editor
{
    /// <summary>
    /// Editor-Tool zum automatischen Setup aller Haupt-UI-Elemente
    /// </summary>
    public class MainUIAutoSetup : EditorWindow
    {
        [MenuItem("CelestialMerge/UI/Auto Setup Main UI (App Store Ready)")]
        public static void ShowWindow()
        {
            GetWindow<MainUIAutoSetup>("Main UI Auto Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Main UI Auto Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "Dieses Tool erstellt automatisch:\n" +
                "• Quest Button (Top-Right)\n" +
                "• Mini-Game Button (Top-Right)\n" +
                "• Daily Login Button (Top-Right)\n" +
                "• Verbindet Buttons mit Panels\n" +
                "• Stellt sicher, dass alles funktioniert\n\n" +
                "Macht die App App Store fertig! 🎨",
                MessageType.Info
            );

            GUILayout.Space(10);

            if (GUILayout.Button("🚀 Setup All UI Now", GUILayout.Height(50)))
            {
                SetupAllUI();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("🔍 Find & Connect Existing Buttons", GUILayout.Height(30)))
            {
                FindAndConnectButtons();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("✅ Verify UI Setup", GUILayout.Height(30)))
            {
                VerifyUISetup();
            }
        }

        private void SetupAllUI()
        {
            // Finde Canvas
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                EditorUtility.DisplayDialog("Fehler", "Kein Canvas gefunden! Bitte erstelle zuerst einen Canvas.", "OK");
                return;
            }

            // Erstelle Buttons direkt im Editor (nicht zur Laufzeit)
            int createdCount = 0;
            System.Text.StringBuilder report = new System.Text.StringBuilder();

            // Finde oder erstelle TopRightButtons Container
            Transform container = canvas.transform.Find("TopRightButtons");
            if (container == null)
            {
                GameObject containerObj = new GameObject("TopRightButtons");
                containerObj.transform.SetParent(canvas.transform, false);
                
                RectTransform rect = containerObj.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(1f, 1f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(300, 300);
                
                container = containerObj.transform;
                report.AppendLine("✅ TopRightButtons Container erstellt");
            }

            // Erstelle Mini-Game Button (falls nicht vorhanden)
            Button miniGameButton = FindButtonInContainer(container, "MiniGameButton", "Mini-Game");
            if (miniGameButton == null)
            {
                miniGameButton = CreateButtonInEditor(container, "MiniGameButton", "🎮 Mini-Game", new Vector2(-150, -110));
                createdCount++;
                report.AppendLine("✅ Mini-Game Button erstellt");
            }
            else
            {
                report.AppendLine("✅ Mini-Game Button bereits vorhanden");
            }

            // Erstelle Quest Button (falls nicht vorhanden)
            Button questButton = FindButtonInContainer(container, "QuestButton", "Quest");
            if (questButton == null)
            {
                questButton = CreateButtonInEditor(container, "QuestButton", "📋 Quests", new Vector2(-150, -50));
                createdCount++;
                report.AppendLine("✅ Quest Button erstellt");
            }
            else
            {
                report.AppendLine("✅ Quest Button bereits vorhanden");
            }

            // Erstelle Daily Button (optional)
            Button dailyButton = FindButtonInContainer(container, "DailyLoginButton", "Daily");
            if (dailyButton == null)
            {
                dailyButton = CreateButtonInEditor(container, "DailyLoginButton", "📅 Daily", new Vector2(-150, -170));
                createdCount++;
                report.AppendLine("✅ Daily Button erstellt");
            }

            // Verbinde Buttons mit Panels
            int connectedCount = 0;
            
            // Verbinde Mini-Game Button
            if (miniGameButton != null)
            {
                CelestialUIManager uiManager = FindFirstObjectByType<CelestialUIManager>();
                if (uiManager != null)
                {
                    SerializedObject so = new SerializedObject(uiManager);
                    SerializedProperty prop = so.FindProperty("playMiniGameButton");
                    if (prop != null)
                    {
                        prop.objectReferenceValue = miniGameButton;
                        so.ApplyModifiedProperties();
                        EditorUtility.SetDirty(uiManager);
                        connectedCount++;
                        report.AppendLine("✅ Mini-Game Button verbunden");
                    }
                }
            }

            // Verbinde Quest Button
            if (questButton != null)
            {
                DailyUIPanel dailyUIPanel = FindFirstObjectByType<DailyUIPanel>();
                if (dailyUIPanel != null)
                {
                    SerializedObject so = new SerializedObject(dailyUIPanel);
                    SerializedProperty prop = so.FindProperty("openQuestButton");
                    if (prop != null)
                    {
                        prop.objectReferenceValue = questButton;
                        so.ApplyModifiedProperties();
                        EditorUtility.SetDirty(dailyUIPanel);
                        connectedCount++;
                        report.AppendLine("✅ Quest Button verbunden");
                    }
                }
            }

            // Markiere Scene als dirty (wichtig für Speichern!)
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            EditorUtility.DisplayDialog("Erfolg", 
                $"✅ UI-Setup abgeschlossen!\n\n" +
                $"Erstellt: {createdCount} Buttons\n" +
                $"Verbunden: {connectedCount} Buttons\n\n" +
                $"{report}",
                "OK");
            
            Debug.Log($"✅ UI-Setup: {createdCount} Buttons erstellt, {connectedCount} verbunden");
        }

        private Button FindButtonInContainer(Transform container, string name, string textContains)
        {
            if (container == null) return null;

            // Suche nach exaktem Namen
            Transform found = container.Find(name);
            if (found != null)
            {
                return found.GetComponent<Button>();
            }

            // Suche nach Text
            Button[] buttons = container.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null && text.text.Contains(textContains))
                {
                    return btn;
                }
            }

            return null;
        }

        private Button CreateButtonInEditor(Transform parent, string name, string text, Vector2 position)
        {
            // Erstelle Button GameObject
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            // RectTransform
            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(180, 50);

            // Image (Button Background)
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.29f, 0.62f, 1f, 1f); // Blau (#4A9EFF)
            buttonImage.raycastTarget = true;

            // Button Component
            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            
            // Button Colors
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.29f, 0.62f, 1f, 1f);
            colors.highlightedColor = new Color(0.4f, 0.7f, 1f, 1f);
            colors.pressedColor = new Color(0.2f, 0.5f, 0.9f, 1f);
            colors.selectedColor = new Color(0.29f, 0.62f, 1f, 1f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            colors.fadeDuration = 0.1f;
            button.colors = colors;

            // Button Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;

            TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = text;
            buttonText.fontSize = 22;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.raycastTarget = false;

            // Markiere als dirty
            EditorUtility.SetDirty(buttonObj);

            return button;
        }

        private void FindAndConnectButtons()
        {
            // Finde Buttons (versuche verschiedene Namen)
            Button questButton = FindButtonByText("Quest") ?? FindButtonByName("QuestButton");
            Button miniGameButton = FindButtonByText("Mini-Game") ?? FindButtonByText("MiniGame") ?? FindButtonByName("MiniGameButton");

            // Finde Panels
            DailyUIPanel dailyUIPanel = FindFirstObjectByType<DailyUIPanel>();
            CelestialUIManager uiManager = FindFirstObjectByType<CelestialUIManager>();

            int connectedCount = 0;
            System.Text.StringBuilder report = new System.Text.StringBuilder();

            // Verbinde Quest Button
            if (questButton != null && dailyUIPanel != null)
            {
                SerializedObject so = new SerializedObject(dailyUIPanel);
                SerializedProperty prop = so.FindProperty("openQuestButton");
                if (prop != null)
                {
                    prop.objectReferenceValue = questButton;
                    so.ApplyModifiedProperties();
                    connectedCount++;
                    report.AppendLine("✅ Quest Button verbunden");
                    Debug.Log("✅ Quest Button verbunden");
                }
                else
                {
                    report.AppendLine("⚠️ openQuestButton Property nicht gefunden");
                }
            }
            else
            {
                if (questButton == null) report.AppendLine("❌ Quest Button nicht gefunden");
                if (dailyUIPanel == null) report.AppendLine("❌ DailyUIPanel nicht gefunden");
            }

            // Verbinde Mini-Game Button
            if (miniGameButton != null && uiManager != null)
            {
                SerializedObject so = new SerializedObject(uiManager);
                SerializedProperty prop = so.FindProperty("playMiniGameButton");
                if (prop != null)
                {
                    prop.objectReferenceValue = miniGameButton;
                    so.ApplyModifiedProperties();
                    
                    // Markiere als dirty für Save
                    EditorUtility.SetDirty(uiManager);
                    
                    connectedCount++;
                    report.AppendLine("✅ Mini-Game Button verbunden");
                    Debug.Log("✅ Mini-Game Button verbunden");
                }
                else
                {
                    report.AppendLine("⚠️ playMiniGameButton Property nicht gefunden");
                    Debug.LogWarning("⚠️ playMiniGameButton Property nicht gefunden in CelestialUIManager");
                }
            }
            else
            {
                if (miniGameButton == null) 
                {
                    report.AppendLine("❌ Mini-Game Button nicht gefunden");
                    Debug.LogWarning("⚠️ Mini-Game Button nicht gefunden! Wird erstellt...");
                    
                    // Erstelle Button automatisch
                    SetupAllUI();
                    return;
                }
                if (uiManager == null) report.AppendLine("❌ CelestialUIManager nicht gefunden");
            }

            if (connectedCount > 0)
            {
                EditorUtility.DisplayDialog("Erfolg", $"✅ {connectedCount} Buttons verbunden!\n\n{report}", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Warnung", $"⚠️ Keine Buttons verbunden!\n\n{report}\n\nVersuche automatisches Setup...", "OK");
                SetupAllUI();
            }
        }

        private Button FindButtonByName(string name)
        {
            Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            foreach (Button btn in allButtons)
            {
                if (btn.name == name || btn.name.Contains(name))
                {
                    return btn;
                }
            }
            return null;
        }

        private Button FindButtonByText(string textContains)
        {
            Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            foreach (Button btn in allButtons)
            {
                TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null && text.text.Contains(textContains))
                {
                    return btn;
                }
            }
            return null;
        }

        private void VerifyUISetup()
        {
            int issues = 0;
            System.Text.StringBuilder report = new System.Text.StringBuilder();

            // Prüfe Quest Button
            DailyUIPanel dailyUIPanel = FindFirstObjectByType<DailyUIPanel>();
            if (dailyUIPanel != null)
            {
                SerializedObject so = new SerializedObject(dailyUIPanel);
                SerializedProperty questButtonProp = so.FindProperty("openQuestButton");
                if (questButtonProp == null || questButtonProp.objectReferenceValue == null)
                {
                    issues++;
                    report.AppendLine("❌ Quest Button nicht zugewiesen in DailyUIPanel");
                }
                else
                {
                    report.AppendLine("✅ Quest Button zugewiesen");
                }
            }

            // Prüfe Mini-Game Button
            CelestialUIManager uiManager = FindFirstObjectByType<CelestialUIManager>();
            if (uiManager != null)
            {
                SerializedObject so = new SerializedObject(uiManager);
                SerializedProperty miniGameButtonProp = so.FindProperty("playMiniGameButton");
                if (miniGameButtonProp == null || miniGameButtonProp.objectReferenceValue == null)
                {
                    issues++;
                    report.AppendLine("❌ Mini-Game Button nicht zugewiesen in CelestialUIManager");
                }
                else
                {
                    report.AppendLine("✅ Mini-Game Button zugewiesen");
                }
            }

            // Prüfe Panels
            MiniGameUIPanel miniGameUIPanel = FindFirstObjectByType<MiniGameUIPanel>();
            if (miniGameUIPanel == null)
            {
                issues++;
                report.AppendLine("⚠️ MiniGameUIPanel nicht gefunden");
            }
            else
            {
                report.AppendLine("✅ MiniGameUIPanel gefunden");
            }

            if (issues == 0)
            {
                EditorUtility.DisplayDialog("Verification", $"✅ Alles OK!\n\n{report}", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Verification", $"⚠️ {issues} Probleme gefunden:\n\n{report}", "OK");
            }
        }
    }
}
