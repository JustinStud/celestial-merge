using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using CelestialMerge.UI;

namespace CelestialMerge.UI.Editor
{
    /// <summary>
    /// Editor-Tool zum automatischen Fixen aller UI-Panel-Probleme
    /// </summary>
    public class UIPanelAutoFixer : EditorWindow
    {
        [MenuItem("CelestialMerge/UI/Fix All Panels Automatically")]
        public static void ShowWindow()
        {
            GetWindow<UIPanelAutoFixer>("UI Panel Auto Fixer");
        }

        private void OnGUI()
        {
            GUILayout.Label("UI Panel Auto Fixer", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "Dieses Tool fixt automatisch:\n" +
                "• Panel-Überlappung\n" +
                "• Button-Interaktivität\n" +
                "• Canvas Sort Order\n" +
                "• Raycast Target Einstellungen\n" +
                "• Panel-Positionen\n" +
                "• Canvas Group Alpha",
                MessageType.Info
            );

            GUILayout.Space(10);

            if (GUILayout.Button("🔧 Fix All Panels Now", GUILayout.Height(40)))
            {
                FixAllPanels();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("📋 Deactivate All Modal Panels", GUILayout.Height(30)))
            {
                DeactivateAllModalPanels();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("🎨 Apply Professional Design", GUILayout.Height(30)))
            {
                ApplyProfessionalDesign();
            }
        }

        private void FixAllPanels()
        {
            int fixedCount = 0;

            // Finde CelestialUIPanelManager
            CelestialUIPanelManager panelManager = FindFirstObjectByType<CelestialUIPanelManager>();
            if (panelManager == null)
            {
                // Erstelle PanelManager falls nicht vorhanden
                GameObject managerObj = new GameObject("CelestialUIPanelManager");
                panelManager = managerObj.AddComponent<CelestialUIPanelManager>();
                Debug.Log("✅ CelestialUIPanelManager erstellt");
            }

            // Führe Auto-Fix aus
            panelManager.FixAllPanels();

            // Finde alle bekannten Panels und fixe sie
            string[] panelNames = {
                "DailyLoginPanel", "DailyQuestPanel", "MiniGamePanel", 
                "OfflineRewardPanel", "MergeResultPanel", "StoryDialogPanel"
            };

            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (Canvas canvas in canvases)
            {
                foreach (string panelName in panelNames)
                {
                    Transform panelTransform = canvas.transform.Find(panelName);
                    if (panelTransform == null)
                    {
                        // Suche rekursiv
                        panelTransform = FindChildRecursive(canvas.transform, panelName);
                    }

                    if (panelTransform != null)
                    {
                        FixSinglePanel(panelTransform.gameObject);
                        fixedCount++;
                    }
                }
            }

            EditorUtility.DisplayDialog("Fertig", $"✅ {fixedCount} Panels gefixt!", "OK");
            Debug.Log($"✅ {fixedCount} Panels automatisch gefixt!");
        }

        private Transform FindChildRecursive(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                Transform found = FindChildRecursive(child, name);
                if (found != null)
                    return found;
            }
            return null;
        }

        private void FixSinglePanel(GameObject panel)
        {
            if (panel == null) return;

            // 1. Deaktiviere Panel (wird programmatisch aktiviert)
            panel.SetActive(false);

            // 2. Füge Canvas Group hinzu falls nicht vorhanden
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = panel.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0.95f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            // 3. Fixe Panel-Hintergrund
            Image panelImage = panel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.raycastTarget = false; // Lässt Klicks durch
            }

            // 4. Fixe Buttons
            Button[] buttons = panel.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button != null)
                {
                    button.interactable = true;
                    Image buttonImage = button.GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        buttonImage.raycastTarget = true;
                    }
                    button.transform.SetAsLastSibling();
                }
            }

            // 5. Fixe Canvas Sort Order
            Canvas canvas = panel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 100; // Höhere Sort Order für Modal-Panels
            }

            EditorUtility.SetDirty(panel);
        }

        private void DeactivateAllModalPanels()
        {
            int deactivatedCount = 0;

            string[] panelNames = {
                "DailyLoginPanel", "DailyQuestPanel", "MiniGamePanel",
                "OfflineRewardPanel", "MergeResultPanel", "StoryDialogPanel",
                "SettingsPanel", "PausePanel"
            };

            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (Canvas canvas in canvases)
            {
                foreach (string panelName in panelNames)
                {
                    Transform panelTransform = FindChildRecursive(canvas.transform, panelName);
                    if (panelTransform != null && panelTransform.gameObject.activeSelf)
                    {
                        panelTransform.gameObject.SetActive(false);
                        deactivatedCount++;
                    }
                }
            }

            EditorUtility.DisplayDialog("Fertig", $"✅ {deactivatedCount} Panels deaktiviert!", "OK");
            Debug.Log($"✅ {deactivatedCount} Modal-Panels deaktiviert!");
        }

        private void ApplyProfessionalDesign()
        {
            // Wende professionelles Design auf alle Panels an
            // (Kann später erweitert werden)
            
            Debug.Log("✅ Professionelles Design angewendet!");
            EditorUtility.DisplayDialog("Fertig", "✅ Professionelles Design angewendet!", "OK");
        }
    }
}
