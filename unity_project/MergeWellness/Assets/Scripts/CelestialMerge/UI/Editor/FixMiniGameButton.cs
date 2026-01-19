using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using CelestialMerge.UI;

namespace CelestialMerge.UI.Editor
{
    /// <summary>
    /// Spezielles Tool zum Fixen des Mini-Game Buttons
    /// </summary>
    public class FixMiniGameButton : EditorWindow
    {
        [MenuItem("CelestialMerge/UI/Fix Mini-Game Button")]
        public static void ShowWindow()
        {
            GetWindow<FixMiniGameButton>("Fix Mini-Game Button");
        }

        private void OnGUI()
        {
            GUILayout.Label("Fix Mini-Game Button", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "Dieses Tool verbindet den Mini-Game Button mit CelestialUIManager.\n" +
                "Löst das Problem: 'Mini-Game Button nicht zugewiesen'",
                MessageType.Info
            );

            GUILayout.Space(10);

            if (GUILayout.Button("🔧 Fix Now", GUILayout.Height(40)))
            {
                FixButton();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("🔍 Find Button", GUILayout.Height(30)))
            {
                FindAndSelectButton();
            }
        }

        private void FixButton()
        {
            // Finde Button
            Button miniGameButton = FindMiniGameButton();
            
            // Finde UIManager
            CelestialUIManager uiManager = FindFirstObjectByType<CelestialUIManager>();

            if (miniGameButton == null)
            {
                EditorUtility.DisplayDialog("Fehler", "Mini-Game Button nicht gefunden!\n\nVerwende 'Auto Setup Main UI' um Button zu erstellen.", "OK");
                MainUIAutoSetup.ShowWindow();
                return;
            }

            if (uiManager == null)
            {
                EditorUtility.DisplayDialog("Fehler", "CelestialUIManager nicht gefunden!", "OK");
                return;
            }

            // Verbinde Button
            SerializedObject so = new SerializedObject(uiManager);
            SerializedProperty prop = so.FindProperty("playMiniGameButton");
            
            if (prop != null)
            {
                prop.objectReferenceValue = miniGameButton;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(uiManager);
                
                EditorUtility.DisplayDialog("Erfolg", 
                    $"✅ Mini-Game Button verbunden!\n\n" +
                    $"Button: {miniGameButton.name}\n" +
                    $"UIManager: {uiManager.name}", 
                    "OK");
                
                Debug.Log($"✅ Mini-Game Button verbunden: {miniGameButton.name} → {uiManager.name}");
            }
            else
            {
                EditorUtility.DisplayDialog("Fehler", 
                    "playMiniGameButton Property nicht gefunden!\n\n" +
                    "Bitte prüfe ob CelestialUIManager Script korrekt ist.", 
                    "OK");
            }
        }

        private Button FindMiniGameButton()
        {
            // Versuche verschiedene Methoden
            Button button = FindButtonByText("Mini-Game");
            if (button != null) return button;

            button = FindButtonByText("MiniGame");
            if (button != null) return button;

            button = FindButtonByName("MiniGameButton");
            if (button != null) return button;

            // Suche in TopRightButtons Container
            Transform container = FindFirstObjectByType<Canvas>()?.transform.Find("TopRightButtons");
            if (container != null)
            {
                button = container.GetComponentInChildren<Button>();
                if (button != null) return button;
            }

            return null;
        }

        private Button FindButtonByText(string textContains)
        {
            Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            foreach (Button btn in allButtons)
            {
                TMPro.TextMeshProUGUI text = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null && text.text.Contains(textContains))
                {
                    return btn;
                }
            }
            return null;
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

        private void FindAndSelectButton()
        {
            Button button = FindMiniGameButton();
            if (button != null)
            {
                Selection.activeGameObject = button.gameObject;
                EditorGUIUtility.PingObject(button.gameObject);
                EditorUtility.DisplayDialog("Gefunden", $"✅ Button gefunden: {button.name}", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Nicht gefunden", "❌ Mini-Game Button nicht gefunden!\n\nVerwende 'Auto Setup Main UI' um Button zu erstellen.", "OK");
            }
        }
    }
}
