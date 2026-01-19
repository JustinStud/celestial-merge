using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MergeWellness
{
    /// <summary>
    /// Verwaltet alle UI-Elemente: Score, Merge-Count, Daily Rewards, Wellness-Facts, Leaderboard
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Score UI")]
        [SerializeField] private Text scoreText;
        [SerializeField] private Text mergeCountText;
        [SerializeField] private TextMeshProUGUI scoreTextPro; // TextMeshPro Alternative
        [SerializeField] private TextMeshProUGUI mergeCountTextPro; // TextMeshPro Alternative

        [Header("Daily Reward")]
        [SerializeField] private Button dailyRewardButton;
        [SerializeField] private GameObject dailyRewardPanel;

        [Header("Wellness Facts")]
        [SerializeField] private GameObject wellnessFactPanel;
        [SerializeField] private Text wellnessFactTitle;
        [SerializeField] private Text wellnessFactText;
        [SerializeField] private TextMeshProUGUI wellnessFactTitlePro; // TextMeshPro Alternative
        [SerializeField] private TextMeshProUGUI wellnessFactTextPro; // TextMeshPro Alternative
        [SerializeField] private Button closeFactButton;

        [Header("Milestones")]
        [SerializeField] private GameObject milestonePanel;
        [SerializeField] private Text milestoneText;
        [SerializeField] private TextMeshProUGUI milestoneTextPro; // TextMeshPro Alternative

        [Header("Leaderboard")]
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private Transform leaderboardContent;

        private GameplayManager gameplayManager;

        private void Awake()
        {
            // Automatisch UI-Komponenten finden, falls nicht gesetzt
            AutoFindUIComponents();
        }

        private void Start()
        {
            gameplayManager = FindFirstObjectByType<GameplayManager>();

            // Setup Daily Reward Button
            if (dailyRewardButton != null)
            {
                dailyRewardButton.onClick.AddListener(OnDailyRewardClicked);
            }

            // Setup Close Fact Button
            if (closeFactButton != null)
            {
                closeFactButton.onClick.AddListener(CloseWellnessFact);
            }

            // Initial verstecken
            if (wellnessFactPanel != null) wellnessFactPanel.SetActive(false);
            if (milestonePanel != null) milestonePanel.SetActive(false);
            if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        }

        /// <summary>
        /// Findet automatisch UI-Komponenten im Canvas, falls nicht manuell gesetzt
        /// </summary>
        private void AutoFindUIComponents()
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null) return;

            // Suche ScoreText
            if (scoreTextPro == null && scoreText == null)
            {
                Transform scoreTextTransform = canvas.transform.Find("ScoreText");
                if (scoreTextTransform != null)
                {
                    // Versuche TextMeshPro zuerst
                    scoreTextPro = scoreTextTransform.GetComponent<TextMeshProUGUI>();
                    if (scoreTextPro == null)
                    {
                        // Fallback zu normalem Text
                        scoreText = scoreTextTransform.GetComponent<Text>();
                    }
                }
            }

            // Suche MergeCountText
            if (mergeCountTextPro == null && mergeCountText == null)
            {
                Transform mergeCountTransform = canvas.transform.Find("MergeCountText");
                if (mergeCountTransform != null)
                {
                    // Versuche TextMeshPro zuerst
                    mergeCountTextPro = mergeCountTransform.GetComponent<TextMeshProUGUI>();
                    if (mergeCountTextPro == null)
                    {
                        // Fallback zu normalem Text
                        mergeCountText = mergeCountTransform.GetComponent<Text>();
                    }
                }
            }

            // Suche Wellness Fact Komponenten
            if (wellnessFactPanel != null)
            {
                if (wellnessFactTitlePro == null && wellnessFactTitle == null)
                {
                    Transform titleTransform = wellnessFactPanel.transform.Find("Title") 
                        ?? wellnessFactPanel.transform.Find("WellnessFactTitle");
                    if (titleTransform != null)
                    {
                        wellnessFactTitlePro = titleTransform.GetComponent<TextMeshProUGUI>();
                        if (wellnessFactTitlePro == null)
                        {
                            wellnessFactTitle = titleTransform.GetComponent<Text>();
                        }
                    }
                }

                if (wellnessFactTextPro == null && wellnessFactText == null)
                {
                    Transform textTransform = wellnessFactPanel.transform.Find("Text")
                        ?? wellnessFactPanel.transform.Find("WellnessFactText");
                    if (textTransform != null)
                    {
                        wellnessFactTextPro = textTransform.GetComponent<TextMeshProUGUI>();
                        if (wellnessFactTextPro == null)
                        {
                            wellnessFactText = textTransform.GetComponent<Text>();
                        }
                    }
                }
            }

            // Suche Milestone Text
            if (milestonePanel != null)
            {
                if (milestoneTextPro == null && milestoneText == null)
                {
                    Transform milestoneTextTransform = milestonePanel.transform.Find("Text")
                        ?? milestonePanel.transform.Find("MilestoneText");
                    if (milestoneTextTransform != null)
                    {
                        milestoneTextPro = milestoneTextTransform.GetComponent<TextMeshProUGUI>();
                        if (milestoneTextPro == null)
                        {
                            milestoneText = milestoneTextTransform.GetComponent<Text>();
                        }
                    }
                }
            }

            Debug.Log("UI-Komponenten automatisch gefunden und zugewiesen");
        }

        /// <summary>
        /// Öffentliche Methode zum manuellen Finden (kann aus Inspector aufgerufen werden)
        /// </summary>
        [ContextMenu("Find UI Components Automatically")]
        public void FindUIComponentsManually()
        {
            AutoFindUIComponents();
            Debug.Log("✓ UI-Komponenten manuell gesucht und zugewiesen!");
        }

        public void UpdateScore(int score)
        {
            // Bevorzuge TextMeshPro, fallback zu normalem Text
            if (scoreTextPro != null)
            {
                scoreTextPro.text = $"Score: {score:N0}";
            }
            else if (scoreText != null)
            {
                scoreText.text = $"Score: {score:N0}";
            }
        }

        public void UpdateMergeCount(int count)
        {
            // Bevorzuge TextMeshPro, fallback zu normalem Text
            if (mergeCountTextPro != null)
            {
                mergeCountTextPro.text = $"Merges: {count}";
            }
            else if (mergeCountText != null)
            {
                mergeCountText.text = $"Merges: {count}";
            }
        }

        public void ShowDailyRewardButton(bool show)
        {
            if (dailyRewardButton != null)
            {
                dailyRewardButton.gameObject.SetActive(show);
            }
        }

        private void OnDailyRewardClicked()
        {
            gameplayManager?.ClaimDailyReward();
        }

        public void ShowWellnessFact(string itemName, string fact)
        {
            if (wellnessFactPanel == null) return;

            // Bevorzuge TextMeshPro, fallback zu normalem Text
            if (wellnessFactTitlePro != null)
            {
                wellnessFactTitlePro.text = itemName;
            }
            else if (wellnessFactTitle != null)
            {
                wellnessFactTitle.text = itemName;
            }

            if (wellnessFactTextPro != null)
            {
                wellnessFactTextPro.text = fact;
            }
            else if (wellnessFactText != null)
            {
                wellnessFactText.text = fact;
            }

            wellnessFactPanel.SetActive(true);
        }

        private void CloseWellnessFact()
        {
            if (wellnessFactPanel != null)
            {
                wellnessFactPanel.SetActive(false);
            }
        }

        public void ShowMilestoneNotification(int milestone)
        {
            if (milestonePanel == null) return;

            string message = $"🎉 Milestone erreicht!\n{milestone} Merges abgeschlossen!";

            // Bevorzuge TextMeshPro, fallback zu normalem Text
            if (milestoneTextPro != null)
            {
                milestoneTextPro.text = message;
            }
            else if (milestoneText != null)
            {
                milestoneText.text = message;
            }

            milestonePanel.SetActive(true);

            // Auto-Close nach 3 Sekunden
            Invoke(nameof(CloseMilestoneNotification), 3f);
        }

        private void CloseMilestoneNotification()
        {
            if (milestonePanel != null)
            {
                milestonePanel.SetActive(false);
            }
        }

        public void ShowLeaderboard()
        {
            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(true);
                // TODO: Lade Leaderboard-Daten von Backend
            }
        }

        public void HideLeaderboard()
        {
            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(false);
            }
        }
    }
}
