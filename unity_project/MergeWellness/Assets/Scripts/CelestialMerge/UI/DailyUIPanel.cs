using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace CelestialMerge.UI
{
    /// <summary>
    /// UI Panel für Daily System: Login Bonus und Daily Quests
    /// </summary>
    public class DailyUIPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DailySystemManager dailyManager;

        [Header("Daily Login UI")]
        [SerializeField] private GameObject dailyLoginPanel;
        [SerializeField] private Button claimLoginButton;
        [SerializeField] private TextMeshProUGUI loginDayText;
        [SerializeField] private TextMeshProUGUI loginRewardText;
        [SerializeField] private TextMeshProUGUI streakText;
        [SerializeField] private Image[] loginDayIcons = new Image[7]; // 7 Tage

        [Header("Daily Quests UI")]
        [SerializeField] private GameObject dailyQuestPanel;
        [SerializeField] private Transform questContainer;
        [SerializeField] private GameObject questPrefab;
        [SerializeField] private TextMeshProUGUI completedQuestsText;
        [Header("Quest Buttons")]
        [SerializeField] public Button openQuestButton; // Button in Haupt-UI zum Öffnen des Quest Panels (public für Auto-Setup)
        [SerializeField] private Button closeQuestButton; // Button im Quest Panel zum Schließen

        private List<GameObject> questUIInstances = new List<GameObject>();

        private void Awake()
        {
            if (dailyManager == null)
            {
                dailyManager = FindFirstObjectByType<DailySystemManager>();
            }
        }

        private void Start()
        {
            // Stelle sicher, dass Panels initial deaktiviert sind
            if (dailyLoginPanel != null)
            {
                dailyLoginPanel.SetActive(false);
            }
            if (dailyQuestPanel != null)
            {
                dailyQuestPanel.SetActive(false);
            }

            SetupButtons();
            
            // Update Quest Button Text (zeige Anzahl abgeschlossener Quests)
            UpdateQuestButtonText();
            
            // Nur UI updaten wenn Panel sichtbar ist
            // UpdateUI() wird erst aufgerufen wenn Panel aktiviert wird
        }

        /// <summary>
        /// Aktualisiert Quest Button Text (zeigt Quest-Progress)
        /// </summary>
        private void UpdateQuestButtonText()
        {
            if (openQuestButton != null && dailyManager != null)
            {
                var quests = dailyManager.GetActiveQuests();
                int completedCount = quests.Count(q => q.isCompleted);
                int totalCount = quests.Count;

                TextMeshProUGUI buttonText = openQuestButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    if (totalCount > 0)
                    {
                        buttonText.text = $"📋 Quests ({completedCount}/{totalCount})";
                    }
                    else
                    {
                        buttonText.text = "📋 Quests";
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (dailyManager != null)
            {
                dailyManager.OnLoginBonusClaimed += OnLoginBonusClaimed;
                dailyManager.OnQuestCompleted += OnQuestCompleted;
                dailyManager.OnAllQuestsCompleted += OnAllQuestsCompleted;
            }
            UpdateUI();
        }

        private void OnDisable()
        {
            if (dailyManager != null)
            {
                dailyManager.OnLoginBonusClaimed -= OnLoginBonusClaimed;
                dailyManager.OnQuestCompleted -= OnQuestCompleted;
                dailyManager.OnAllQuestsCompleted -= OnAllQuestsCompleted;
            }
        }

        private void SetupButtons()
        {
            if (claimLoginButton != null)
            {
                claimLoginButton.onClick.RemoveAllListeners();
                claimLoginButton.onClick.AddListener(OnClaimLoginClicked);
                
                // Stelle sicher, dass Button vor anderen UI-Elementen ist (höhere Sibling Order)
                claimLoginButton.transform.SetAsLastSibling();
                
                // Stelle sicher, dass Button nicht von Raycast blockiert wird
                CanvasGroup buttonCanvasGroup = claimLoginButton.GetComponent<CanvasGroup>();
                if (buttonCanvasGroup == null)
                {
                    buttonCanvasGroup = claimLoginButton.gameObject.AddComponent<CanvasGroup>();
                }
                buttonCanvasGroup.blocksRaycasts = true;
                buttonCanvasGroup.interactable = true;
            }

            // Setup Quest Button (öffnet Quest Panel)
            if (openQuestButton != null)
            {
                openQuestButton.onClick.RemoveAllListeners();
                openQuestButton.onClick.AddListener(OnOpenQuestButtonClicked);
            }

            // Setup Close Quest Button (schließt Quest Panel)
            if (closeQuestButton != null)
            {
                closeQuestButton.onClick.RemoveAllListeners();
                closeQuestButton.onClick.AddListener(OnCloseQuestButtonClicked);
            }
        }

        /// <summary>
        /// Aktualisiert gesamte Daily UI
        /// </summary>
        public void UpdateUI()
        {
            UpdateDailyLoginUI();
            UpdateDailyQuestsUI();
        }

        /// <summary>
        /// Aktualisiert Daily Login UI (Merge-App-Stil: Professionelles Layout)
        /// </summary>
        private void UpdateDailyLoginUI()
        {
            if (dailyManager == null) return;

            int currentDay = dailyManager.GetCurrentLoginDay();
            bool isClaimed = dailyManager.IsTodayClaimed();

            // Update Day Text (oben, klar getrennt)
            if (loginDayText != null)
            {
                loginDayText.text = $"Tag {currentDay} von 7";
                // Style: Große Schrift, oben im Panel
                loginDayText.fontSize = 32;
                loginDayText.color = Color.white;
                loginDayText.fontStyle = FontStyles.Bold;
            }

            // Update Reward Text (klar getrennt, große Schrift)
            if (loginRewardText != null)
            {
                long stardust = GetLoginStardust(currentDay);
                int crystals = GetLoginCrystals(currentDay);
                
                // Professionelles Format wie bei Merge Dragons
                string rewardText = "";
                if (stardust > 0)
                {
                    rewardText += $"💰 {stardust:N0} Stardust";
                }
                if (crystals > 0)
                {
                    if (!string.IsNullOrEmpty(rewardText)) rewardText += "\n";
                    rewardText += $"💎 {crystals:N0} Crystals";
                }
                
                loginRewardText.text = rewardText;
                // Style: Lesbar, große Schrift, gute Kontrastfarbe
                loginRewardText.fontSize = 28;
                loginRewardText.color = new Color(1f, 0.84f, 0f); // Gold-Gelb für Rewards
                loginRewardText.fontStyle = FontStyles.Bold;
                
                // Position: Unter Day Text, über Button
                RectTransform rewardRect = loginRewardText.GetComponent<RectTransform>();
                if (rewardRect != null)
                {
                    // Stelle sicher, dass Text nicht über Grid liegt
                    rewardRect.anchorMin = new Vector2(0.1f, 0.5f);
                    rewardRect.anchorMax = new Vector2(0.9f, 0.7f);
                    rewardRect.anchoredPosition = Vector2.zero;
                }
            }

            // Update Streak (optional, unten angezeigt)
            if (streakText != null)
            {
                streakText.text = $"Streak: {currentDay} Tage";
                streakText.fontSize = 18;
                streakText.color = new Color(0.7f, 0.7f, 0.7f); // Grau für Info-Text
            }

            // Update Claim Button (groß, klar sichtbar, unten)
            if (claimLoginButton != null)
            {
                claimLoginButton.interactable = !isClaimed;
                TextMeshProUGUI buttonText = claimLoginButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = isClaimed ? "✓ Bereits abgeholt" : "🎁 Abholen";
                    buttonText.fontSize = 24;
                    buttonText.fontStyle = FontStyles.Bold;
                }
                
                // Button-Style: Groß, unten im Panel
                RectTransform buttonRect = claimLoginButton.GetComponent<RectTransform>();
                if (buttonRect != null)
                {
                    buttonRect.anchorMin = new Vector2(0.25f, 0.05f);
                    buttonRect.anchorMax = new Vector2(0.75f, 0.20f);
                    buttonRect.anchoredPosition = Vector2.zero;
                    
                    // Button-Größe
                    buttonRect.sizeDelta = new Vector2(0, 60);
                }
                
                // Button-Farbe basierend auf Status
                Image buttonImage = claimLoginButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = isClaimed 
                        ? new Color(0.5f, 0.5f, 0.5f) // Grau wenn abgeholt
                        : new Color(0.2f, 0.8f, 0.3f); // Grün für "Abholen"
                }
            }

            // Update Day Icons (optional)
            UpdateLoginDayIcons(currentDay, isClaimed);
        }

        /// <summary>
        /// Aktualisiert Daily Quests UI
        /// </summary>
        private void UpdateDailyQuestsUI()
        {
            if (dailyManager == null || questContainer == null) return;

            // Lösche alte Quest UI
            foreach (var questUI in questUIInstances)
            {
                if (questUI != null)
                {
                    Destroy(questUI);
                }
            }
            questUIInstances.Clear();

            // Erstelle Quest UI für jede aktive Quest
            List<DailyQuest> quests = dailyManager.GetActiveQuests();
            int completedCount = quests.Count(q => q.isCompleted);

            if (completedQuestsText != null)
            {
                completedQuestsText.text = $"Abgeschlossen: {completedCount}/{quests.Count}";
            }

            foreach (var quest in quests)
            {
                if (questPrefab != null)
                {
                    GameObject questUI = Instantiate(questPrefab, questContainer);
                    SetupQuestUI(questUI, quest);
                    questUIInstances.Add(questUI);
                }
                else
                {
                    // Fallback: Text-Only Display
                    Debug.LogWarning($"Quest Prefab fehlt! Zeige Quest: {quest.questName} ({quest.currentValue}/{quest.targetValue})");
                }
            }
        }

        /// <summary>
        /// Setup einzelne Quest UI
        /// </summary>
        private void SetupQuestUI(GameObject questUI, DailyQuest quest)
        {
            if (questUI == null) return;

            // Finde Text-Komponenten (mit robustem Find)
            TextMeshProUGUI nameText = questUI.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            if (nameText == null)
            {
                // Fallback: Suche in allen Children
                nameText = questUI.GetComponentInChildren<TextMeshProUGUI>();
            }

            TextMeshProUGUI progressText = questUI.transform.Find("ProgressText")?.GetComponent<TextMeshProUGUI>();
            Slider progressBar = questUI.GetComponentInChildren<Slider>();
            Image completedIcon = questUI.transform.Find("CompletedIcon")?.GetComponent<Image>();

            // WICHTIG: Stelle sicher, dass keine falschen Texte angezeigt werden
            // Prüfe alle TextMeshPro-Komponenten und setze nur die richtigen
            TextMeshProUGUI[] allTexts = questUI.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var text in allTexts)
            {
                // Überspringe Texte, die nicht zu Quest gehören
                if (text.name.Contains("Close") || text.name.Contains("Stardust") || text.name.Contains("Button"))
                {
                    // Diese Texte sollten nicht in Quest-UI sein
                    if (text.transform.parent == questUI.transform)
                    {
                        Debug.LogWarning($"⚠️ Falscher Text in Quest UI gefunden: {text.name}. Sollte entfernt werden.");
                        // Verstecke oder entferne falsche Texte
                        text.gameObject.SetActive(false);
                    }
                }
            }

            if (nameText != null && !nameText.name.Contains("Close") && !nameText.name.Contains("Stardust"))
            {
                nameText.text = quest.questName;
                nameText.gameObject.SetActive(true);
            }

            if (progressText != null && !progressText.name.Contains("Close") && !progressText.name.Contains("Stardust"))
            {
                progressText.text = $"{quest.currentValue}/{quest.targetValue}";
                progressText.gameObject.SetActive(true);
            }

            if (progressBar != null)
            {
                progressBar.value = quest.targetValue > 0 
                    ? (float)quest.currentValue / quest.targetValue 
                    : 0f;
            }

            if (completedIcon != null)
            {
                completedIcon.gameObject.SetActive(quest.isCompleted);
            }
        }

        /// <summary>
        /// Aktualisiert Login Day Icons
        /// </summary>
        private void UpdateLoginDayIcons(int currentDay, bool isClaimed)
        {
            for (int i = 0; i < loginDayIcons.Length; i++)
            {
                if (loginDayIcons[i] != null)
                {
                    // Tag i+1 ist erreicht wenn i+1 <= currentDay
                    bool isReached = (i + 1) <= currentDay;
                    // Visuelles Feedback (z.B. Farbwechsel)
                    // Color targetColor = isReached ? Color.white : Color.gray;
                    // loginDayIcons[i].color = targetColor;
                }
            }
        }

        /// <summary>
        /// Callback: Claim Login Button clicked
        /// </summary>
        private void OnClaimLoginClicked()
        {
            if (dailyManager != null && !dailyManager.IsTodayClaimed())
            {
                dailyManager.ClaimDailyLoginBonus();
                UpdateUI();
            }
        }

        /// <summary>
        /// Callback: Login Bonus claimed
        /// </summary>
        private void OnLoginBonusClaimed(int day)
        {
            UpdateUI();
            Debug.Log($"✅ Daily Login Bonus Tag {day} abgeholt!");
        }

        /// <summary>
        /// Callback: Quest completed
        /// </summary>
        private void OnQuestCompleted(DailyQuest quest)
        {
            UpdateDailyQuestsUI();
            UpdateQuestButtonText(); // Update Button Text mit neuem Progress
            Debug.Log($"✅ Quest abgeschlossen: {quest.questName}");
        }

        /// <summary>
        /// Callback: All quests completed
        /// </summary>
        private void OnAllQuestsCompleted()
        {
            UpdateDailyQuestsUI();
            Debug.Log("🎉 Alle Daily Quests abgeschlossen!");
        }

        /// <summary>
        /// Helper: Gibt Stardust für Login-Tag zurück
        /// </summary>
        private long GetLoginStardust(int day)
        {
            switch (day)
            {
                case 1: return 100;
                case 2: return 100;
                case 3: return 150;
                case 4: return 150;
                case 5: return 200;
                case 6: return 250;
                case 7: return 500;
                default: return 100;
            }
        }

        /// <summary>
        /// Helper: Gibt Crystals für Login-Tag zurück
        /// </summary>
        private int GetLoginCrystals(int day)
        {
            switch (day)
            {
                case 1: return 0;
                case 2: return 5;
                case 3: return 10;
                case 4: return 0;
                case 5: return 25;
                case 6: return 50;
                case 7: return 100;
                default: return 0;
            }
        }

        /// <summary>
        /// Zeigt Daily Login Panel (verwendet PanelManager)
        /// </summary>
        public void ShowDailyLogin()
        {
            if (dailyLoginPanel != null)
            {
                // Verwende PanelManager falls vorhanden
                if (CelestialUIPanelManager.Instance != null)
                {
                    CelestialUIPanelManager.Instance.ShowDailyLogin();
                }
                else
                {
                    dailyLoginPanel.SetActive(true);
                    EnsurePanelOnTop(dailyLoginPanel);
                }
                
                // Stelle sicher, dass Button klickbar ist
                if (claimLoginButton != null)
                {
                    claimLoginButton.transform.SetAsLastSibling();
                    
                    // Deaktiviere Raycast Target auf Panel-Hintergrund falls vorhanden
                    Image panelImage = dailyLoginPanel.GetComponent<Image>();
                    if (panelImage != null)
                    {
                        panelImage.raycastTarget = false; // Lässt Klicks durch
                    }
                }
                
                UpdateUI();
            }
        }

        /// <summary>
        /// Stellt sicher, dass Panel über anderen UI-Elementen ist
        /// </summary>
        private void EnsurePanelOnTop(GameObject panel)
        {
            if (panel == null) return;

            // Setze höhere Canvas Sort Order falls vorhanden
            Canvas canvas = panel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                // Stelle sicher, dass Panel-Hintergrund kein Raycast Target ist (außer es ist das Panel selbst)
                Image[] allImages = panel.GetComponentsInChildren<Image>();
                foreach (Image img in allImages)
                {
                    // Hintergrund-Images sollen keine Raycasts blockieren
                    if (img.transform == panel.transform || img.name.Contains("Background"))
                    {
                        img.raycastTarget = false;
                    }
                }
            }

            // Setze Panel als letztes Sibling (wird über anderen gerendert)
            if (panel.transform.parent != null)
            {
                panel.transform.SetAsLastSibling();
            }
        }

        /// <summary>
        /// Versteckt Daily Login Panel
        /// </summary>
        public void HideDailyLogin()
        {
            if (dailyLoginPanel != null)
            {
                dailyLoginPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Zeigt Daily Quest Panel (verwendet PanelManager)
        /// </summary>
        public void ShowDailyQuests()
        {
            if (dailyQuestPanel != null)
            {
                // Verwende PanelManager falls vorhanden
                if (CelestialUIPanelManager.Instance != null)
                {
                    CelestialUIPanelManager.Instance.ShowDailyQuest();
                }
                else
                {
                    dailyQuestPanel.SetActive(true);
                    EnsurePanelOnTop(dailyQuestPanel);
                }
                
                // Bereinige falsche UI-Elemente im Quest Container
                CleanQuestContainer();
                
                UpdateUI();
            }
        }

        /// <summary>
        /// Bereinigt Quest Container von falschen UI-Elementen (z.B. "Close", "Stardust" Text)
        /// </summary>
        private void CleanQuestContainer()
        {
            if (questContainer == null) return;

            // Finde und entferne falsche Texte/Buttons im Container
            foreach (Transform child in questContainer)
            {
                if (child == null) continue;

                // Prüfe ob Child falsche Texte enthält
                TextMeshProUGUI[] texts = child.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var text in texts)
                {
                    string textContent = text.text.ToLower();
                    // Wenn Text "close" oder "stardust" enthält und nicht Teil einer Quest ist
                    if ((textContent.Contains("close") || textContent.Contains("stardust")) && 
                        !text.name.Contains("Quest") && 
                        !text.name.Contains("Name") && 
                        !text.name.Contains("Progress"))
                    {
                        Debug.LogWarning($"⚠️ Falscher Text im Quest Container gefunden: '{text.text}' auf {text.name}. Wird entfernt.");
                        Destroy(text.gameObject);
                    }
                }

                // Prüfe ob Child falsche Buttons enthält (die nicht Quest-Buttons sind)
                Button[] buttons = child.GetComponentsInChildren<Button>(true);
                foreach (var button in buttons)
                {
                    TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        string buttonTextContent = buttonText.text.ToLower();
                        // Wenn Button "close" sagt und nicht der CloseQuestButton ist
                        if (buttonTextContent.Contains("close") && button != closeQuestButton)
                        {
                            Debug.LogWarning($"⚠️ Falscher Close Button im Quest Container gefunden: {button.name}. Wird entfernt.");
                            Destroy(button.gameObject);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Versteckt Daily Quest Panel (verwendet PanelManager)
        /// </summary>
        public void HideDailyQuests()
        {
            if (dailyQuestPanel != null)
            {
                if (CelestialUIPanelManager.Instance != null)
                {
                    CelestialUIPanelManager.Instance.HidePanel(dailyQuestPanel);
                }
                else
                {
                    dailyQuestPanel.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Callback: Open Quest Button clicked (in Haupt-UI)
        /// </summary>
        private void OnOpenQuestButtonClicked()
        {
            ShowDailyQuests();
        }

        /// <summary>
        /// Callback: Close Quest Button clicked (im Quest Panel)
        /// </summary>
        private void OnCloseQuestButtonClicked()
        {
            HideDailyQuests();
        }
    }
}
