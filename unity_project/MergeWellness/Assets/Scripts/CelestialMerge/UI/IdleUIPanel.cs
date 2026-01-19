using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace CelestialMerge.UI
{
    /// <summary>
    /// UI Panel für Idle Production System
    /// </summary>
    public class IdleUIPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private IdleProductionManager idleManager;

        [Header("Production Display")]
        [SerializeField] private TextMeshProUGUI productionRateText;
        [SerializeField] private TextMeshProUGUI currentProductionText;

        [Header("Offline Reward Panel")]
        [SerializeField] private GameObject offlineRewardPanel;
        [SerializeField] private TextMeshProUGUI offlineTimeText;
        [SerializeField] private TextMeshProUGUI offlineRewardText;
        [SerializeField] private Button claimOfflineButton;
        [SerializeField] private Button closeOfflinePanelButton;

        private DateTime lastCheckTime;
        private bool offlineRewardAvailable = false;

        private void Awake()
        {
            if (idleManager == null)
            {
                idleManager = FindFirstObjectByType<IdleProductionManager>();
            }
        }

        private void Start()
        {
            // Stelle sicher, dass Panel initial deaktiviert ist
            if (offlineRewardPanel != null)
            {
                offlineRewardPanel.SetActive(false);
            }

            SetupButtons();
            CheckOfflineProduction();
            UpdateProductionDisplay();
        }

        private void Update()
        {
            UpdateProductionDisplay();
        }

        private void SetupButtons()
        {
            if (claimOfflineButton != null)
            {
                claimOfflineButton.onClick.RemoveAllListeners();
                claimOfflineButton.onClick.AddListener(OnClaimOfflineRewardClicked);
            }

            if (closeOfflinePanelButton != null)
            {
                closeOfflinePanelButton.onClick.RemoveAllListeners();
                closeOfflinePanelButton.onClick.AddListener(OnCloseOfflinePanelClicked);
            }
        }

        /// <summary>
        /// Aktualisiert Production Display
        /// </summary>
        private void UpdateProductionDisplay()
        {
            if (idleManager == null) return;

            float productionRate = idleManager.CurrentProductionRate;

            if (productionRateText != null)
            {
                productionRateText.text = $"Production Rate: {productionRate:F1} Stardust/Min";
            }

            // Berechne aktuelle Production (kontinuierlich)
            float stardustPerSecond = productionRate / 60f;
            if (currentProductionText != null)
            {
                // Zeige Production Rate (live)
                currentProductionText.text = $"+{stardustPerSecond:F2} Stardust/Sek";
            }
        }

        /// <summary>
        /// Prüft Offline-Production beim Start
        /// </summary>
        private void CheckOfflineProduction()
        {
            // Offline-Production wird automatisch von IdleProductionManager verarbeitet
            // Hier zeigen wir nur das Result Panel falls nötig

            // Prüfe ob Offline-Production vorhanden war (wird in IdleProductionManager verarbeitet)
            // TODO: Erweitere IdleProductionManager um GetLastOfflineReward() Methode
            // Für jetzt: Zeige Panel nicht automatisch
        }

        /// <summary>
        /// Zeigt Offline Reward Panel (verwendet PanelManager)
        /// </summary>
        public void ShowOfflineReward(long stardustEarned, TimeSpan offlineTime)
        {
            offlineRewardAvailable = true;

            if (offlineRewardPanel == null) return;

            // Verwende PanelManager falls vorhanden
            if (CelestialUIPanelManager.Instance != null)
            {
                CelestialUIPanelManager.Instance.ShowPanel(offlineRewardPanel, true);
            }
            else
            {
                offlineRewardPanel.SetActive(true);
            }

            // Update Offline Time Text
            if (offlineTimeText != null)
            {
                if (offlineTime.TotalHours >= 1)
                {
                    offlineTimeText.text = $"Offline: {(int)offlineTime.TotalHours}h {(int)offlineTime.Minutes}m";
                }
                else
                {
                    offlineTimeText.text = $"Offline: {(int)offlineTime.TotalMinutes}m";
                }
            }

            // Update Reward Text
            if (offlineRewardText != null)
            {
                offlineRewardText.text = $"+{stardustEarned} Stardust";
            }
        }

        /// <summary>
        /// Callback: Claim Offline Reward clicked
        /// </summary>
        private void OnClaimOfflineRewardClicked()
        {
            // Reward wurde bereits von IdleProductionManager vergeben
            // Hier nur Panel schließen
            HideOfflineRewardPanel();
        }

        /// <summary>
        /// Callback: Close Offline Panel clicked
        /// </summary>
        private void OnCloseOfflinePanelClicked()
        {
            HideOfflineRewardPanel();
        }

        /// <summary>
        /// Versteckt Offline Reward Panel (verwendet PanelManager)
        /// </summary>
        private void HideOfflineRewardPanel()
        {
            if (offlineRewardPanel != null)
            {
                if (CelestialUIPanelManager.Instance != null)
                {
                    CelestialUIPanelManager.Instance.HidePanel(offlineRewardPanel);
                }
                else
                {
                    offlineRewardPanel.SetActive(false);
                }
            }
            offlineRewardAvailable = false;
        }

        /// <summary>
        /// Prüft ob Offline Reward verfügbar ist
        /// </summary>
        public bool HasOfflineReward()
        {
            return offlineRewardAvailable;
        }
    }
}
