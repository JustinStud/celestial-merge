using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace CelestialMerge.UI
{
    /// <summary>
    /// Initialisiert automatisch alle Haupt-UI-Elemente (Buttons, Panels, etc.)
    /// Erstellt fehlende Buttons automatisch und stellt sicher, dass alles funktioniert
    /// </summary>
    public class CelestialMainUIInitializer : MonoBehaviour
    {
        [Header("Auto-Create Settings")]
        [SerializeField] private bool autoCreateMissingButtons = true;
        [SerializeField] private bool autoSetupOnStart = true;

        [Header("Button References (werden automatisch gefunden/erstellt)")]
        [SerializeField] private Button questButton;
        [SerializeField] private Button miniGameButton;
        [SerializeField] private Button dailyLoginButton;
        [SerializeField] private Button settingsButton;

        [Header("Panel References")]
        [SerializeField] private DailyUIPanel dailyUIPanel;
        [SerializeField] private MiniGameUIPanel miniGameUIPanel;
        [SerializeField] private CelestialUIManager uiManager;

        private Canvas mainCanvas;

        private void Awake()
        {
            // Finde Canvas
            mainCanvas = GetComponentInParent<Canvas>();
            if (mainCanvas == null)
            {
                mainCanvas = FindFirstObjectByType<Canvas>();
            }

            // Finde Referenzen
            if (dailyUIPanel == null)
            {
                dailyUIPanel = FindFirstObjectByType<DailyUIPanel>();
            }

            if (miniGameUIPanel == null)
            {
                miniGameUIPanel = FindFirstObjectByType<MiniGameUIPanel>();
            }

            if (uiManager == null)
            {
                uiManager = FindFirstObjectByType<CelestialUIManager>();
            }
        }

        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupAllUI();
            }
        }

        /// <summary>
        /// Setzt alle UI-Elemente auf
        /// </summary>
        public void SetupAllUI()
        {
            Debug.Log("🎨 Starte automatisches UI-Setup...");

            // 1. Erstelle fehlende Buttons
            if (autoCreateMissingButtons)
            {
                CreateMissingButtons();
            }

            // 2. Verbinde Buttons mit Panels
            ConnectButtonsToPanels();

            // 3. Stelle sicher, dass Buttons sichtbar und funktionsfähig sind
            EnsureButtonsVisibleAndFunctional();

            Debug.Log("✅ UI-Setup abgeschlossen!");
        }

        /// <summary>
        /// Erstellt fehlende Buttons automatisch
        /// </summary>
        private void CreateMissingButtons()
        {
            if (mainCanvas == null)
            {
                Debug.LogError("❌ Canvas nicht gefunden! Kann Buttons nicht erstellen.");
                return;
            }

            // Finde oder erstelle Top-Right Container für Buttons
            Transform topRightContainer = FindOrCreateTopRightContainer();

            // Quest Button
            if (questButton == null)
            {
                questButton = FindButtonByName("QuestButton", "Quests", "📋 Quests");
                if (questButton == null && autoCreateMissingButtons)
                {
                    questButton = CreateButton(topRightContainer, "QuestButton", "📋 Quests", new Vector2(-150, -50));
                }
            }

            // Mini-Game Button
            if (miniGameButton == null)
            {
                miniGameButton = FindButtonByName("MiniGameButton", "MiniGame", "🎮 Mini-Game");
                if (miniGameButton == null && autoCreateMissingButtons)
                {
                    miniGameButton = CreateButton(topRightContainer, "MiniGameButton", "🎮 Mini-Game", new Vector2(-150, -110));
                }
            }

            // Daily Login Button (optional)
            if (dailyLoginButton == null)
            {
                dailyLoginButton = FindButtonByName("DailyLoginButton", "Daily", "📅 Daily");
                if (dailyLoginButton == null && autoCreateMissingButtons)
                {
                    dailyLoginButton = CreateButton(topRightContainer, "DailyLoginButton", "📅 Daily", new Vector2(-150, -170));
                }
            }
        }

        /// <summary>
        /// Findet oder erstellt Top-Right Container für Buttons
        /// </summary>
        private Transform FindOrCreateTopRightContainer()
        {
            // Suche nach existierendem Container
            Transform container = mainCanvas.transform.Find("TopRightButtons");
            if (container == null)
            {
                // Erstelle neuen Container
                GameObject containerObj = new GameObject("TopRightButtons");
                containerObj.transform.SetParent(mainCanvas.transform, false);

                RectTransform rect = containerObj.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(1f, 1f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(300, 300);

                container = containerObj.transform;
            }

            return container;
        }

        /// <summary>
        /// Findet Button nach Name
        /// </summary>
        private Button FindButtonByName(string exactName, string containsName, string buttonText)
        {
            // Suche nach exaktem Namen
            Transform found = mainCanvas.transform.Find(exactName);
            if (found != null)
            {
                return found.GetComponent<Button>();
            }

            // Suche rekursiv nach Namen der "containsName" enthält
            Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            foreach (Button btn in allButtons)
            {
                if (btn.name.Contains(containsName) || btn.name.Contains(exactName))
                {
                    TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();
                    if (text != null && text.text.Contains(buttonText))
                    {
                        return btn;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Erstellt einen neuen Button mit professionellem Design
        /// </summary>
        private Button CreateButton(Transform parent, string name, string text, Vector2 position)
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
            
            // Button Colors (Hover, Pressed, etc.)
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

            // Stelle sicher, dass Button vorne ist
            buttonObj.transform.SetAsLastSibling();

            Debug.Log($"✅ Button erstellt: {name} ({text})");
            return button;
        }

        /// <summary>
        /// Verbindet Buttons mit ihren Panels
        /// </summary>
        private void ConnectButtonsToPanels()
        {
            // Quest Button → DailyUIPanel (jetzt public, direkter Zugriff)
            if (questButton != null && dailyUIPanel != null)
            {
                dailyUIPanel.openQuestButton = questButton;
                
                // Stelle sicher, dass Button auch im DailyUIPanel registriert ist
                dailyUIPanel.SendMessage("SetupButtons", SendMessageOptions.DontRequireReceiver);
                
                Debug.Log("✅ Quest Button mit DailyUIPanel verbunden");
            }

            // Mini-Game Button → CelestialUIManager (jetzt public, direkter Zugriff)
            if (miniGameButton != null && uiManager != null)
            {
                uiManager.playMiniGameButton = miniGameButton;
                
                // Stelle sicher, dass Button auch im UIManager registriert ist
                // Verwende Invoke für bessere Kompatibilität
                if (Application.isPlaying)
                {
                    uiManager.SendMessage("InitializeUI", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    // Im Editor: Markiere als dirty
                    #if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(uiManager);
                    #endif
                }
                
                Debug.Log($"✅ Mini-Game Button mit CelestialUIManager verbunden (Button: {miniGameButton.name})");
            }
            else
            {
                if (miniGameButton == null)
                {
                    Debug.LogWarning("⚠️ Mini-Game Button ist null! Kann nicht verbinden.");
                }
                if (uiManager == null)
                {
                    Debug.LogWarning("⚠️ CelestialUIManager ist null! Kann nicht verbinden.");
                }
            }

            // Mini-Game Button → MiniGameUIPanel (falls vorhanden)
            if (miniGameButton != null && miniGameUIPanel != null)
            {
                // Direkte Verbindung über Button onClick
                miniGameButton.onClick.RemoveAllListeners();
                miniGameButton.onClick.AddListener(() => {
                    if (miniGameUIPanel != null)
                    {
                        miniGameUIPanel.Show();
                    }
                });
                Debug.Log("✅ Mini-Game Button onClick Listener hinzugefügt");
            }

            // Daily Login Button → DailyUIPanel
            if (dailyLoginButton != null && dailyUIPanel != null)
            {
                dailyLoginButton.onClick.RemoveAllListeners();
                dailyLoginButton.onClick.AddListener(() => {
                    if (dailyUIPanel != null)
                    {
                        dailyUIPanel.ShowDailyLogin();
                    }
                });
                Debug.Log("✅ Daily Login Button onClick Listener hinzugefügt");
            }
        }

        /// <summary>
        /// Stellt sicher, dass Buttons sichtbar und funktionsfähig sind
        /// </summary>
        private void EnsureButtonsVisibleAndFunctional()
        {
            List<Button> allButtons = new List<Button>();
            if (questButton != null) allButtons.Add(questButton);
            if (miniGameButton != null) allButtons.Add(miniGameButton);
            if (dailyLoginButton != null) allButtons.Add(dailyLoginButton);

            foreach (Button button in allButtons)
            {
                if (button == null) continue;

                // Stelle sicher, dass Button aktiv ist
                button.gameObject.SetActive(true);

                // Stelle sicher, dass Button interaktabel ist
                button.interactable = true;

                // Stelle sicher, dass Button Image Raycast Target hat
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.raycastTarget = true;
                }

                // Stelle sicher, dass Button vorne ist
                button.transform.SetAsLastSibling();

                // Stelle sicher, dass Button Canvas Group hat (für bessere Interaktion)
                CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            Debug.Log($"✅ {allButtons.Count} Buttons sind sichtbar und funktionsfähig");
        }

        #region Public API

        /// <summary>
        /// Gibt Quest Button zurück (für externe Verwendung)
        /// </summary>
        public Button GetQuestButton()
        {
            return questButton;
        }

        /// <summary>
        /// Gibt Mini-Game Button zurück (für externe Verwendung)
        /// </summary>
        public Button GetMiniGameButton()
        {
            return miniGameButton;
        }

        #endregion
    }
}
