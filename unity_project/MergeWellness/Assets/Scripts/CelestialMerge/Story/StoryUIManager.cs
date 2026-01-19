using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace CelestialMerge.Story
{
    /// <summary>
    /// UI Manager für Story System
    /// Verwaltet Dialog-Anzeige, Typewriter-Effekt, Lore Encyclopedia
    /// </summary>
    public class StoryUIManager : MonoBehaviour
    {
        [Header("Dialog UI")]
        [SerializeField] private GameObject dialogPanel;
        [SerializeField] private Image npcPortraitImage;
        [SerializeField] private TextMeshProUGUI npcNameText;
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] private Button[] choiceButtons;
        [SerializeField] private Button continueButton; // Button zum Weiterklicken (wenn keine Choices)
        [SerializeField] private CanvasGroup dialogCanvasGroup;
        
        [Header("Dialog Settings")]
        [SerializeField] private bool enableClickToContinue = true; // Klick auf Panel zum Fortfahren
        [SerializeField] private int dialogCanvasSortOrder = 100; // Höhere Sort Order = über anderen UI

        [Header("Chapter Unlock UI")]
        [SerializeField] private GameObject chapterUnlockPanel;
        [SerializeField] private TextMeshProUGUI chapterTitleText;
        [SerializeField] private TextMeshProUGUI chapterDescriptionText;
        [SerializeField] private Image chapterImage;

        [Header("Lore Notification")]
        [SerializeField] private GameObject loreNotificationPanel;
        [SerializeField] private TextMeshProUGUI loreNotificationText;

        [Header("Typewriter Settings")]
        [SerializeField] private float typewriterSpeed = 0.05f; // Sekunden pro Zeichen

        private StoryManager storyManager;
        private Coroutine typewriterCoroutine;

        #region Initialization

        private void Awake()
        {
            // Auto-Find StoryManager
            storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                storyManager = FindFirstObjectByType<StoryManager>();
            }
        }

        private void Start()
        {
            // Initialisiere UI als versteckt
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(false);
                
                // Stelle sicher, dass Dialog-Panel über anderen UI-Elementen ist
                EnsureDialogPanelOnTop();
            }

            if (chapterUnlockPanel != null)
            {
                chapterUnlockPanel.SetActive(false);
            }

            if (loreNotificationPanel != null)
            {
                loreNotificationPanel.SetActive(false);
            }
            
            // Setup Continue Button
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(false);
                continueButton.onClick.AddListener(OnContinueButtonClicked);
            }
            
            // Setup Click-to-Continue auf Panel
            if (dialogPanel != null && enableClickToContinue)
            {
                Button panelButton = dialogPanel.GetComponent<Button>();
                if (panelButton == null)
                {
                    panelButton = dialogPanel.AddComponent<Button>();
                    panelButton.transition = Selectable.Transition.None; // Keine visuelle Reaktion
                }
                panelButton.onClick.AddListener(OnDialogPanelClicked);
            }
        }
        
        /// <summary>
        /// Stellt sicher, dass Dialog-Panel über allen anderen UI-Elementen gerendert wird
        /// </summary>
        private void EnsureDialogPanelOnTop()
        {
            if (dialogPanel == null) return;
            
            // Finde Canvas des Dialog-Panels
            Canvas canvas = dialogPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                // Setze höhere Sort Order für Canvas
                canvas.sortingOrder = dialogCanvasSortOrder;
                Debug.Log($"📖 Dialog Canvas Sort Order auf {dialogCanvasSortOrder} gesetzt");
            }
            else
            {
                // Falls kein Canvas gefunden, erstelle eines oder verschiebe Panel in späterer Hierarchie
                Debug.LogWarning("⚠️ Dialog Panel hat kein Canvas! Stelle sicher, dass es unter einem Canvas ist.");
            }
            
            // Stelle sicher, dass Panel ganz oben in der Hierarchy ist (später = über anderen)
            if (dialogPanel.transform.parent != null)
            {
                dialogPanel.transform.SetAsLastSibling();
            }
        }

        #endregion

        #region Dialog System

        /// <summary>
        /// Zeigt Dialog für Story Beat
        /// </summary>
        public void ShowDialog(StoryBeat beat)
        {
            if (beat == null || dialogPanel == null) return;

            // Setze NPC Portrait und Name
            if (npcPortraitImage != null && beat.NPCPortrait != null)
            {
                npcPortraitImage.sprite = beat.NPCPortrait;
                npcPortraitImage.enabled = true;
            }
            else if (npcPortraitImage != null)
            {
                npcPortraitImage.enabled = false;
            }

            if (npcNameText != null)
            {
                npcNameText.text = beat.NPCName;
            }

            // Stelle sicher, dass Panel über anderen UI-Elementen ist
            EnsureDialogPanelOnTop();
            
            // Zeige Dialog Panel
            dialogPanel.SetActive(true);

            // Starte Typewriter-Effekt
            if (dialogText != null)
            {
                if (typewriterCoroutine != null)
                {
                    StopCoroutine(typewriterCoroutine);
                }
                
                // Debug: Prüfe ob Dialog-Text vorhanden ist
                if (string.IsNullOrEmpty(beat.DialogText))
                {
                    Debug.LogWarning("⚠️ Story Beat hat keinen Dialog-Text!");
                }
                else
                {
                    Debug.Log($"📖 Zeige Dialog-Text: '{beat.DialogText.Substring(0, Mathf.Min(50, beat.DialogText.Length))}...' (Länge: {beat.DialogText.Length})");
                }
                
                typewriterCoroutine = StartCoroutine(TypewriterEffect(beat.DialogText, () => {
                    // Nach Typewriter: Zeige Continue-Button wenn keine Choices vorhanden
                    OnTypewriterFinished(beat);
                }));
            }
            else
            {
                Debug.LogError("❌ dialogText ist NULL! Bitte im Inspector zuweisen (StoryUIManager → Dialog Text)");
            }

            // Setup Choice Buttons
            SetupChoiceButtons(beat);
            
            // Verstecke Continue-Button zunächst (wird nach Typewriter angezeigt)
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(false);
            }

            // Fade In Animation
            if (dialogCanvasGroup != null)
            {
                StartCoroutine(FadeIn(dialogCanvasGroup, 0.3f));
            }
        }
        
        /// <summary>
        /// Wird aufgerufen wenn Typewriter-Effekt beendet ist
        /// </summary>
        private void OnTypewriterFinished(StoryBeat beat)
        {
            // Zeige Continue-Button nur wenn keine Choices vorhanden
            if (beat.Choices == null || beat.Choices.Count == 0)
            {
                if (continueButton != null)
                {
                    continueButton.gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Typewriter-Effekt für Dialog-Text
        /// </summary>
        private IEnumerator TypewriterEffect(string fullText, System.Action onComplete = null)
        {
            if (dialogText == null)
            {
                Debug.LogError("❌ dialogText ist NULL in TypewriterEffect!");
                yield break;
            }

            // Debug: Prüfe TextMeshPro-Einstellungen
            Debug.Log($"📖 Typewriter startet: Text-Länge={fullText?.Length ?? 0}, TextMeshPro-Active={dialogText.gameObject.activeInHierarchy}, TextColor={dialogText.color}");

            dialogText.text = "";

            foreach (char c in fullText)
            {
                dialogText.text += c;
                yield return new WaitForSeconds(typewriterSpeed);
            }
            
            // Rufe Callback auf wenn Typewriter beendet ist
            onComplete?.Invoke();
        }

        /// <summary>
        /// Setup Choice Buttons für Dialog
        /// </summary>
        private void SetupChoiceButtons(StoryBeat beat)
        {
            if (choiceButtons == null) return;

            // Deaktiviere alle Buttons zuerst
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (choiceButtons[i] != null)
                {
                    choiceButtons[i].gameObject.SetActive(false);
                    choiceButtons[i].onClick.RemoveAllListeners();
                }
            }

            // Prüfe ob Choices vorhanden sind
            if (beat.Choices != null && beat.Choices.Count > 0)
            {
                // Aktiviere Buttons für verfügbare Choices
                for (int i = 0; i < beat.Choices.Count && i < choiceButtons.Length; i++)
                {
                    if (choiceButtons[i] != null)
                    {
                        DialogChoice choice = beat.Choices[i];
                        choiceButtons[i].gameObject.SetActive(true);

                        // Setze Button Text
                        TextMeshProUGUI buttonText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                        if (buttonText != null)
                        {
                            buttonText.text = choice.ChoiceText;
                        }

                        // Setup OnClick Event
                        int choiceIndex = i; // Capture für Closure
                        choiceButtons[i].onClick.AddListener(() => OnChoiceButtonClicked(choice, beat));
                    }
                }
            }
            // Falls keine Choices, wird Continue-Button nach Typewriter angezeigt
        }

        /// <summary>
        /// Wird aufgerufen wenn Choice Button geklickt wird
        /// </summary>
        private void OnChoiceButtonClicked(DialogChoice choice, StoryBeat beat)
        {
            // Informiere StoryManager
            if (storyManager != null)
            {
                storyManager.OnDialogChoiceSelected(choice);
            }

            // Schließe Dialog
            CloseDialog();
        }

        /// <summary>
        /// Wird aufgerufen wenn Continue-Button geklickt wird
        /// </summary>
        private void OnContinueButtonClicked()
        {
            CloseDialog();
        }
        
        /// <summary>
        /// Wird aufgerufen wenn Dialog-Panel angeklickt wird (Click-to-Continue)
        /// </summary>
        private void OnDialogPanelClicked()
        {
            // Nur weiterklicken wenn Typewriter fertig ist und keine Choices vorhanden
            if (typewriterCoroutine == null)
            {
                // Prüfe ob Choices vorhanden (dann nicht per Klick schließen)
                bool hasChoices = false;
                if (choiceButtons != null)
                {
                    foreach (var button in choiceButtons)
                    {
                        if (button != null && button.gameObject.activeSelf)
                        {
                            hasChoices = true;
                            break;
                        }
                    }
                }
                
                if (!hasChoices)
                {
                    CloseDialog();
                }
            }
        }
        
        /// <summary>
        /// Schließt Dialog Panel
        /// </summary>
        public void CloseDialog()
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }
            
            // Verstecke Continue-Button
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(false);
            }

            if (dialogCanvasGroup != null)
            {
                StartCoroutine(FadeOut(dialogCanvasGroup, 0.2f, () =>
                {
                    if (dialogPanel != null)
                    {
                        dialogPanel.SetActive(false);
                    }
                }));
            }
            else
            {
                if (dialogPanel != null)
                {
                    dialogPanel.SetActive(false);
                }
            }
        }

        #endregion

        #region Chapter Unlock

        /// <summary>
        /// Zeigt Chapter-Unlock Screen
        /// </summary>
        public void ShowChapterUnlock(StoryChapter chapter)
        {
            if (chapter == null || chapterUnlockPanel == null) return;

            // Setze Chapter-Daten
            if (chapterTitleText != null)
            {
                chapterTitleText.text = $"Chapter {chapter.ChapterId}: {chapter.ChapterTitle}";
            }

            if (chapterDescriptionText != null)
            {
                chapterDescriptionText.text = chapter.ChapterDescription;
            }

            if (chapterImage != null && chapter.ChapterImage != null)
            {
                chapterImage.sprite = chapter.ChapterImage;
                chapterImage.enabled = true;
            }

            // Zeige Panel
            chapterUnlockPanel.SetActive(true);

            // Auto-Close nach 3 Sekunden
            StartCoroutine(AutoCloseChapterUnlock(3f));
        }

        /// <summary>
        /// Schließt Chapter-Unlock automatisch nach Delay
        /// </summary>
        private IEnumerator AutoCloseChapterUnlock(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (chapterUnlockPanel != null)
            {
                chapterUnlockPanel.SetActive(false);
            }
        }

        #endregion

        #region Lore Notification

        /// <summary>
        /// Zeigt Lore-Unlock Notification
        /// </summary>
        public void NotifyLoreUnlock(LoreEntry entry)
        {
            if (entry == null || loreNotificationPanel == null) return;

            // Setze Notification Text
            if (loreNotificationText != null)
            {
                loreNotificationText.text = $"📖 Lore freigeschaltet: {entry.Title}";
            }

            // Zeige Panel
            loreNotificationPanel.SetActive(true);

            // Auto-Close nach 2 Sekunden
            StartCoroutine(AutoCloseLoreNotification(2f));
        }

        /// <summary>
        /// Schließt Lore-Notification automatisch nach Delay
        /// </summary>
        private IEnumerator AutoCloseLoreNotification(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (loreNotificationPanel != null)
            {
                loreNotificationPanel.SetActive(false);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Fade In Animation für CanvasGroup
        /// </summary>
        private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
        {
            float elapsed = 0f;
            canvasGroup.alpha = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// Fade Out Animation für CanvasGroup mit Callback
        /// </summary>
        private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration, System.Action onComplete = null)
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            onComplete?.Invoke();
        }

        #endregion
    }
}
