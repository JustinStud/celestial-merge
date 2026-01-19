using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace CelestialMerge.UI
{
    /// <summary>
    /// Zentrales UI-Panel-Management-System
    /// Verhindert Panel-Überlappung, verwaltet Panel-Stacking, stellt sicher dass Buttons funktionieren
    /// Professionelles Design wie bei Top Merge-Apps
    /// </summary>
    public class CelestialUIPanelManager : MonoBehaviour
    {
        public static CelestialUIPanelManager Instance { get; private set; }

        [Header("Panel Management")]
        [SerializeField] private List<UIPanelInfo> managedPanels = new List<UIPanelInfo>();

        [Header("Settings")]
        [SerializeField] private int defaultCanvasSortOrder = 0;
        [SerializeField] private int overlayCanvasSortOrder = 100;
        [SerializeField] private bool autoFixOnStart = true;

        // Panel Stack (für Modal-Panels)
        private Stack<GameObject> panelStack = new Stack<GameObject>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            if (autoFixOnStart)
            {
                AutoFindAndRegisterPanels();
                FixAllPanels();
            }
        }

        #region Auto-Find & Register

        /// <summary>
        /// Findet automatisch alle UI-Panels und registriert sie
        /// </summary>
        private void AutoFindAndRegisterPanels()
        {
            managedPanels.Clear();

            // Finde alle Panels im Canvas
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (Canvas canvas in canvases)
            {
                FindPanelsInCanvas(canvas.transform);
            }

            Debug.Log($"✅ {managedPanels.Count} UI-Panels gefunden und registriert");
        }

        /// <summary>
        /// Findet Panels rekursiv in einem Transform
        /// </summary>
        private void FindPanelsInCanvas(Transform parent)
        {
            foreach (Transform child in parent)
            {
                // Prüfe ob GameObject ein Panel ist (hat Image Component und ist groß genug)
                Image img = child.GetComponent<Image>();
                RectTransform rect = child.GetComponent<RectTransform>();
                
                if (img != null && rect != null)
                {
                    // Prüfe ob es ein Panel ist (groß genug, nicht nur ein Button)
                    Vector2 size = rect.sizeDelta;
                    if (size.x > 200 && size.y > 200) // Mindestgröße für Panel
                    {
                        string panelName = child.name.ToLower();
                        
                        // Erkenne Panel-Typ
                        UIPanelType panelType = DetectPanelType(panelName);
                        
                        // Nur registrieren wenn es ein bekanntes Panel ist
                        if (panelType != UIPanelType.Unknown)
                        {
                            RegisterPanel(child.gameObject, panelType);
                        }
                    }
                }

                // Rekursiv durchsuchen
                if (child.childCount > 0)
                {
                    FindPanelsInCanvas(child);
                }
            }
        }

        /// <summary>
        /// Erkennt Panel-Typ basierend auf Namen
        /// </summary>
        private UIPanelType DetectPanelType(string panelName)
        {
            if (panelName.Contains("daily") && panelName.Contains("login")) return UIPanelType.DailyLogin;
            if (panelName.Contains("daily") && panelName.Contains("quest")) return UIPanelType.DailyQuest;
            if (panelName.Contains("mini") && panelName.Contains("game")) return UIPanelType.MiniGame;
            if (panelName.Contains("offline") || panelName.Contains("idle")) return UIPanelType.OfflineReward;
            if (panelName.Contains("merge") && panelName.Contains("result")) return UIPanelType.MergeResult;
            if (panelName.Contains("story") || panelName.Contains("dialog") || panelName.Contains("lore")) return UIPanelType.StoryDialog;
            if (panelName.Contains("settings")) return UIPanelType.Settings;
            if (panelName.Contains("pause")) return UIPanelType.Pause;
            
            return UIPanelType.Unknown;
        }

        /// <summary>
        /// Registriert ein Panel im Management-System
        /// </summary>
        private void RegisterPanel(GameObject panel, UIPanelType type)
        {
            if (panel == null) return;

            // Prüfe ob bereits registriert
            if (managedPanels.Any(p => p.panel == panel))
            {
                return;
            }

            UIPanelInfo info = new UIPanelInfo
            {
                panel = panel,
                type = type,
                priority = GetPanelPriority(type),
                shouldBeHiddenOnStart = ShouldBeHiddenOnStart(type)
            };

            managedPanels.Add(info);
            Debug.Log($"📋 Panel registriert: {panel.name} (Type: {type}, Priority: {info.priority})");
        }

        /// <summary>
        /// Gibt Panel-Priority zurück (höher = wichtiger, wird über anderen angezeigt)
        /// </summary>
        private int GetPanelPriority(UIPanelType type)
        {
            return type switch
            {
                UIPanelType.StoryDialog => 100,      // Höchste Priority (Story ist wichtig)
                UIPanelType.DailyLogin => 90,
                UIPanelType.OfflineReward => 85,
                UIPanelType.MiniGame => 80,
                UIPanelType.DailyQuest => 75,
                UIPanelType.MergeResult => 70,
                UIPanelType.Settings => 60,
                UIPanelType.Pause => 50,
                _ => 10
            };
        }

        /// <summary>
        /// Prüft ob Panel beim Start versteckt sein sollte
        /// </summary>
        private bool ShouldBeHiddenOnStart(UIPanelType type)
        {
            return type switch
            {
                UIPanelType.DailyLogin => true,
                UIPanelType.DailyQuest => true,
                UIPanelType.MiniGame => true,
                UIPanelType.OfflineReward => true,
                UIPanelType.MergeResult => true,
                UIPanelType.StoryDialog => true,
                UIPanelType.Settings => true,
                UIPanelType.Pause => true,
                _ => false
            };
        }

        #endregion

        #region Panel Fixes

        /// <summary>
        /// Fixt alle Panels automatisch
        /// </summary>
        public void FixAllPanels()
        {
            Debug.Log("🔧 Starte automatische Panel-Fixes...");

            foreach (var panelInfo in managedPanels)
            {
                if (panelInfo.panel == null) continue;

                FixPanel(panelInfo);
            }

            Debug.Log($"✅ {managedPanels.Count} Panels gefixt!");
        }

        /// <summary>
        /// Fixt ein einzelnes Panel
        /// </summary>
        private void FixPanel(UIPanelInfo panelInfo)
        {
            GameObject panel = panelInfo.panel;
            if (panel == null) return;

            // 1. Stelle sicher, dass Panel initial versteckt ist (falls nötig)
            if (panelInfo.shouldBeHiddenOnStart && panel.activeSelf)
            {
                panel.SetActive(false);
                Debug.Log($"✅ Panel deaktiviert: {panel.name}");
            }

            // 2. Fixe Canvas Sort Order
            FixCanvasSortOrder(panel, panelInfo.priority);

            // 3. Fixe Panel-Hintergrund (Raycast Target)
            FixPanelBackground(panel);

            // 4. Fixe Buttons (Interaktabilität)
            FixButtons(panel);

            // 5. Fixe Canvas Group (Transparenz)
            FixCanvasGroup(panel);

            // 6. Stelle sicher, dass Panel richtig positioniert ist
            FixPanelPosition(panel, panelInfo.type);
        }

        /// <summary>
        /// Fixt Canvas Sort Order für Panel
        /// </summary>
        private void FixCanvasSortOrder(GameObject panel, int priority)
        {
            Canvas canvas = panel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                // Setze Sort Order basierend auf Priority
                canvas.sortingOrder = defaultCanvasSortOrder + priority;
                
                // Falls Panel ein Modal ist, verwende Overlay Canvas
                if (priority >= 70) // Modal-Panels
                {
                    canvas.sortingOrder = overlayCanvasSortOrder + priority;
                }
            }
        }

        /// <summary>
        /// Fixt Panel-Hintergrund (Raycast Target deaktivieren für bessere Button-Klickbarkeit)
        /// </summary>
        private void FixPanelBackground(GameObject panel)
        {
            Image panelImage = panel.GetComponent<Image>();
            if (panelImage != null)
            {
                // Panel-Hintergrund soll keine Raycasts blockieren (außer bei Modal-Panels)
                UIPanelInfo info = managedPanels.FirstOrDefault(p => p.panel == panel);
                bool isModal = info != null && info.priority >= 70;
                
                panelImage.raycastTarget = isModal; // Nur Modal-Panels blockieren Raycasts
            }
        }

        /// <summary>
        /// Fixt alle Buttons im Panel
        /// </summary>
        private void FixButtons(GameObject panel)
        {
            Button[] buttons = panel.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button == null) continue;

                // Stelle sicher, dass Button interaktabel ist
                if (!button.interactable)
                {
                    button.interactable = true;
                }

                // Stelle sicher, dass Button Raycast Target hat
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.raycastTarget = true;
                }

                // Stelle sicher, dass Button vorne ist (höhere Sibling Order)
                button.transform.SetAsLastSibling();
            }

            Debug.Log($"✅ {buttons.Length} Buttons gefixt in {panel.name}");
        }

        /// <summary>
        /// Fixt Canvas Group (Transparenz)
        /// </summary>
        private void FixCanvasGroup(GameObject panel)
        {
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                // Füge Canvas Group hinzu falls nicht vorhanden
                canvasGroup = panel.AddComponent<CanvasGroup>();
            }

            // Setze Alpha auf 0.95 (etwas transparent)
            canvasGroup.alpha = 0.95f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// Fixt Panel-Position basierend auf Typ
        /// </summary>
        private void FixPanelPosition(GameObject panel, UIPanelType type)
        {
            RectTransform rect = panel.GetComponent<RectTransform>();
            if (rect == null) return;

            // Setze Position basierend auf Panel-Typ
            switch (type)
            {
                case UIPanelType.DailyLogin:
                case UIPanelType.DailyQuest:
                case UIPanelType.MiniGame:
                case UIPanelType.OfflineReward:
                case UIPanelType.MergeResult:
                case UIPanelType.StoryDialog:
                    // Modal-Panels: Center
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.anchoredPosition = Vector2.zero;
                    break;

                case UIPanelType.Settings:
                case UIPanelType.Pause:
                    // Settings/Pause: Center
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.anchoredPosition = Vector2.zero;
                    break;
            }
        }

        #endregion

        #region Panel Management

        /// <summary>
        /// Zeigt Panel (schließt andere Modal-Panels automatisch)
        /// </summary>
        public void ShowPanel(GameObject panel, bool isModal = true)
        {
            if (panel == null) return;

            // Schließe andere Modal-Panels
            if (isModal)
            {
                CloseAllModalPanels();
                panelStack.Push(panel);
            }

            panel.SetActive(true);
            FixPanel(GetPanelInfo(panel));

            // Stelle sicher, dass Panel über anderen ist
            EnsurePanelOnTop(panel);

            Debug.Log($"📋 Panel geöffnet: {panel.name}");
        }

        /// <summary>
        /// Versteckt Panel
        /// </summary>
        public void HidePanel(GameObject panel)
        {
            if (panel == null) return;

            panel.SetActive(false);

            // Entferne aus Stack
            if (panelStack.Count > 0 && panelStack.Peek() == panel)
            {
                panelStack.Pop();
            }

            Debug.Log($"📋 Panel geschlossen: {panel.name}");
        }

        /// <summary>
        /// Schließt alle Modal-Panels
        /// </summary>
        public void CloseAllModalPanels()
        {
            while (panelStack.Count > 0)
            {
                GameObject panel = panelStack.Pop();
                if (panel != null)
                {
                    panel.SetActive(false);
                }
            }

            // Schließe auch alle anderen Modal-Panels
            foreach (var panelInfo in managedPanels)
            {
                if (panelInfo.panel != null && panelInfo.priority >= 70) // Modal-Panels
                {
                    if (panelInfo.panel.activeSelf)
                    {
                        panelInfo.panel.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// Stellt sicher, dass Panel über anderen UI-Elementen ist
        /// </summary>
        private void EnsurePanelOnTop(GameObject panel)
        {
            if (panel == null) return;

            // Setze als letztes Sibling (wird über anderen gerendert)
            if (panel.transform.parent != null)
            {
                panel.transform.SetAsLastSibling();
            }

            // Setze Canvas Sort Order
            Canvas canvas = panel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                UIPanelInfo info = GetPanelInfo(panel);
                if (info != null)
                {
                    canvas.sortingOrder = overlayCanvasSortOrder + info.priority;
                }
            }
        }

        /// <summary>
        /// Gibt PanelInfo für Panel zurück
        /// </summary>
        private UIPanelInfo GetPanelInfo(GameObject panel)
        {
            return managedPanels.FirstOrDefault(p => p.panel == panel);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Zeigt Daily Login Panel
        /// </summary>
        public void ShowDailyLogin()
        {
            var panel = managedPanels.FirstOrDefault(p => p.type == UIPanelType.DailyLogin);
            if (panel != null)
            {
                ShowPanel(panel.panel, true);
            }
        }

        /// <summary>
        /// Zeigt Daily Quest Panel
        /// </summary>
        public void ShowDailyQuest()
        {
            var panel = managedPanels.FirstOrDefault(p => p.type == UIPanelType.DailyQuest);
            if (panel != null)
            {
                ShowPanel(panel.panel, true);
            }
        }

        /// <summary>
        /// Zeigt Mini-Game Panel
        /// </summary>
        public void ShowMiniGame()
        {
            var panel = managedPanels.FirstOrDefault(p => p.type == UIPanelType.MiniGame);
            if (panel != null)
            {
                ShowPanel(panel.panel, true);
            }
        }

        /// <summary>
        /// Zeigt Offline Reward Panel
        /// </summary>
        public void ShowOfflineReward()
        {
            var panel = managedPanels.FirstOrDefault(p => p.type == UIPanelType.OfflineReward);
            if (panel != null)
            {
                ShowPanel(panel.panel, true);
            }
        }

        /// <summary>
        /// Schließt aktuelles Modal-Panel
        /// </summary>
        public void CloseCurrentPanel()
        {
            if (panelStack.Count > 0)
            {
                GameObject panel = panelStack.Pop();
                HidePanel(panel);
            }
            else
            {
                // Schließe das zuletzt geöffnete Modal-Panel
                CloseAllModalPanels();
            }
        }

        #endregion
    }

    /// <summary>
    /// UI Panel Information
    /// </summary>
    [System.Serializable]
    public class UIPanelInfo
    {
        public GameObject panel;
        public UIPanelType type;
        public int priority;
        public bool shouldBeHiddenOnStart;
    }

    /// <summary>
    /// UI Panel Types
    /// </summary>
    public enum UIPanelType
    {
        Unknown,
        DailyLogin,
        DailyQuest,
        MiniGame,
        OfflineReward,
        MergeResult,
        StoryDialog,
        Settings,
        Pause
    }
}
