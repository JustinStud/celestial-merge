using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace CelestialMerge.UI.Editor
{
    /// <summary>
    /// Tool zum Finden und Markieren von UI-Buttons in der Hierarchy
    /// </summary>
    public class FindUIButtons : EditorWindow
    {
        [MenuItem("CelestialMerge/UI/Find UI Buttons in Hierarchy")]
        public static void ShowWindow()
        {
            GetWindow<FindUIButtons>("Find UI Buttons");
        }

        private void OnGUI()
        {
            GUILayout.Label("Find UI Buttons", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "Dieses Tool findet alle wichtigen UI-Buttons in der Hierarchy und markiert sie.\n" +
                "Nützlich wenn Buttons zur Laufzeit erstellt wurden.",
                MessageType.Info
            );

            GUILayout.Space(10);

            if (GUILayout.Button("🔍 Find Mini-Game Button", GUILayout.Height(40)))
            {
                FindAndSelectButton("MiniGameButton", "Mini-Game", "🎮");
            }

            GUILayout.Space(5);

            if (GUILayout.Button("🔍 Find Quest Button", GUILayout.Height(40)))
            {
                FindAndSelectButton("QuestButton", "Quest", "📋");
            }

            GUILayout.Space(5);

            if (GUILayout.Button("🔍 Find Daily Button", GUILayout.Height(40)))
            {
                FindAndSelectButton("DailyLoginButton", "Daily", "📅");
            }

            GUILayout.Space(10);

            if (GUILayout.Button("📋 Find All Buttons", GUILayout.Height(30)))
            {
                FindAllButtons();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("📍 Show Button Locations", GUILayout.Height(30)))
            {
                ShowButtonLocations();
            }
        }

        private void FindAndSelectButton(string buttonName, string buttonText, string emoji)
        {
            Button button = null;

            // Methode 1: Suche nach exaktem Namen
            GameObject found = GameObject.Find(buttonName);
            if (found != null)
            {
                button = found.GetComponent<Button>();
            }

            // Methode 2: Suche in TopRightButtons Container
            if (button == null)
            {
                Transform container = FindTopRightContainer();
                if (container != null)
                {
                    Transform buttonTransform = container.Find(buttonName);
                    if (buttonTransform != null)
                    {
                        button = buttonTransform.GetComponent<Button>();
                    }
                }
            }

            // Methode 3: Suche nach Text-Inhalt
            if (button == null)
            {
                button = FindButtonByText(buttonText);
            }

            // Methode 4: Suche alle Buttons und prüfe Namen
            if (button == null)
            {
                Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
                foreach (Button btn in allButtons)
                {
                    if (btn.name.Contains(buttonName) || btn.name.Contains(buttonText))
                    {
                        button = btn;
                        break;
                    }
                }
            }

            if (button != null)
            {
                Selection.activeGameObject = button.gameObject;
                EditorGUIUtility.PingObject(button.gameObject);
                
                // Zeige Pfad in Hierarchy
                string path = GetGameObjectPath(button.gameObject);
                EditorUtility.DisplayDialog("Gefunden", 
                    $"{emoji} {buttonName} gefunden!\n\n" +
                    $"Pfad: {path}\n\n" +
                    $"Button wurde in der Hierarchy markiert.",
                    "OK");
                
                Debug.Log($"✅ {buttonName} gefunden: {path}");
            }
            else
            {
                EditorUtility.DisplayDialog("Nicht gefunden", 
                    $"❌ {buttonName} nicht gefunden!\n\n" +
                    $"Der Button wurde möglicherweise zur Laufzeit erstellt.\n" +
                    $"Verwende 'Auto Setup Main UI' um Button zu erstellen.",
                    "OK");
            }
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

        private Transform FindTopRightContainer()
        {
            // Suche nach TopRightButtons Container
            GameObject container = GameObject.Find("TopRightButtons");
            if (container != null)
            {
                return container.transform;
            }

            // Suche in Canvas
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                Transform found = canvas.transform.Find("TopRightButtons");
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private void FindAllButtons()
        {
            System.Text.StringBuilder report = new System.Text.StringBuilder();
            report.AppendLine("🔍 Gefundene Buttons:\n");

            // Finde TopRightButtons Container
            Transform container = FindTopRightContainer();
            if (container != null)
            {
                report.AppendLine($"✅ TopRightButtons Container gefunden:");
                report.AppendLine($"   Pfad: {GetGameObjectPath(container.gameObject)}\n");

                // Liste alle Buttons im Container
                Button[] buttons = container.GetComponentsInChildren<Button>(true);
                foreach (Button btn in buttons)
                {
                    report.AppendLine($"   • {btn.name} ({GetGameObjectPath(btn.gameObject)})");
                }
            }
            else
            {
                report.AppendLine("⚠️ TopRightButtons Container nicht gefunden\n");
            }

            // Finde spezifische Buttons
            Button miniGameButton = FindButtonByText("Mini-Game");
            Button questButton = FindButtonByText("Quest");
            Button dailyButton = FindButtonByText("Daily");

            report.AppendLine("\n📋 Spezifische Buttons:");
            report.AppendLine(miniGameButton != null ? $"✅ Mini-Game Button: {GetGameObjectPath(miniGameButton.gameObject)}" : "❌ Mini-Game Button nicht gefunden");
            report.AppendLine(questButton != null ? $"✅ Quest Button: {GetGameObjectPath(questButton.gameObject)}" : "❌ Quest Button nicht gefunden");
            report.AppendLine(dailyButton != null ? $"✅ Daily Button: {GetGameObjectPath(dailyButton.gameObject)}" : "❌ Daily Button nicht gefunden");

            EditorUtility.DisplayDialog("Button Report", report.ToString(), "OK");
        }

        private void ShowButtonLocations()
        {
            System.Text.StringBuilder locations = new System.Text.StringBuilder();
            locations.AppendLine("📍 Button Locations:\n");

            locations.AppendLine("Erwartete Locations:");
            locations.AppendLine("Canvas → TopRightButtons → MiniGameButton");
            locations.AppendLine("Canvas → TopRightButtons → QuestButton");
            locations.AppendLine("Canvas → TopRightButtons → DailyLoginButton\n");

            // Prüfe ob Container existiert
            Transform container = FindTopRightContainer();
            if (container != null)
            {
                locations.AppendLine($"✅ TopRightButtons Container existiert");
                locations.AppendLine($"   Pfad: {GetGameObjectPath(container.gameObject)}\n");

                // Zeige alle Children
                locations.AppendLine("Children:");
                for (int i = 0; i < container.childCount; i++)
                {
                    Transform child = container.GetChild(i);
                    locations.AppendLine($"   {i + 1}. {child.name}");
                }
            }
            else
            {
                locations.AppendLine("❌ TopRightButtons Container existiert NICHT");
                locations.AppendLine("   → Buttons wurden zur Laufzeit erstellt");
                locations.AppendLine("   → Verwende 'Auto Setup Main UI' um Buttons zu erstellen");
            }

            EditorUtility.DisplayDialog("Button Locations", locations.ToString(), "OK");
        }

        private string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }
    }
}
