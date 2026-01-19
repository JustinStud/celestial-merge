using UnityEngine;
using System;

namespace CelestialMerge
{
    /// <summary>
    /// Mini-Match-3 Game Manager für Crystal-Verdienung
    /// </summary>
    public class MiniGameManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private DailySystemManager dailySystem;

        [Header("Mini-Game Settings")]
        [SerializeField] private int energy = 5;
        [SerializeField] private int maxEnergy = 10;
        [SerializeField] private float energyRegenMinutes = 20f;
        [SerializeField] private DateTime lastEnergyRegenTime;

        [Header("Game Types")]
        [SerializeField] private MiniGameType currentGameType = MiniGameType.Easy;

        // Events
        public event Action<int> OnEnergyChanged;
        public event Action<MiniGameResult> OnMiniGameCompleted;

        public int Energy => energy;
        public int MaxEnergy => maxEnergy;

        private void Awake()
        {
            LoadEnergyState();
        }

        private void Start()
        {
            ProcessEnergyRegen();
        }

        private void Update()
        {
            // Regeneriere Energy über Zeit
            ProcessEnergyRegen();
        }

        /// <summary>
        /// Startet Mini-Game
        /// </summary>
        public bool StartMiniGame(MiniGameType gameType)
        {
            if (energy <= 0)
            {
                Debug.LogWarning("Keine Energy mehr! Warte auf Regeneration.");
                return false;
            }

            currentGameType = gameType;
            energy--;
            SaveEnergyState();
            OnEnergyChanged?.Invoke(energy);

            // Update Daily Quest
            if (dailySystem != null)
            {
                dailySystem.UpdateQuestProgress(QuestType.PlayMinigame);
            }

            Debug.Log($"🎮 Mini-Game gestartet: {gameType}");
            return true;
        }

        /// <summary>
        /// Beendet Mini-Game mit Ergebnis
        /// </summary>
        public void CompleteMiniGame(bool won, int score = 0)
        {
            MiniGameResult result = new MiniGameResult
            {
                gameType = currentGameType,
                won = won,
                score = score,
                crystalReward = won ? GetCrystalReward(currentGameType) : 0,
                stardustReward = won ? GetStardustReward(currentGameType) : 0,
                xpReward = won ? GetXpReward(currentGameType) : 0
            };

            if (won && currencyManager != null)
            {
                currencyManager.AddCrystals(result.crystalReward);
                currencyManager.AddStardust(result.stardustReward);
            }

            OnMiniGameCompleted?.Invoke(result);
            Debug.Log($"✅ Mini-Game abgeschlossen: {(won ? "Gewonnen" : "Verloren")} - {result.crystalReward} Crystals");
        }

        /// <summary>
        /// Gibt Crystal-Reward für Game Type zurück
        /// </summary>
        private int GetCrystalReward(MiniGameType gameType)
        {
            switch (gameType)
            {
                case MiniGameType.Easy: return UnityEngine.Random.Range(5, 11); // 5-10
                case MiniGameType.Medium: return UnityEngine.Random.Range(15, 31); // 15-30
                case MiniGameType.Hard: return UnityEngine.Random.Range(50, 101); // 50-100
                case MiniGameType.Puzzle: return 20;
                case MiniGameType.TimedRush: return 35;
                default: return 5;
            }
        }

        /// <summary>
        /// Gibt Stardust-Reward zurück
        /// </summary>
        private long GetStardustReward(MiniGameType gameType)
        {
            switch (gameType)
            {
                case MiniGameType.Easy: return 100;
                case MiniGameType.Medium: return 250;
                case MiniGameType.Hard: return 500;
                case MiniGameType.Puzzle: return 200;
                case MiniGameType.TimedRush: return 300;
                default: return 100;
            }
        }

        /// <summary>
        /// Gibt XP-Reward zurück
        /// </summary>
        private int GetXpReward(MiniGameType gameType)
        {
            switch (gameType)
            {
                case MiniGameType.Easy: return 50;
                case MiniGameType.Medium: return 100;
                case MiniGameType.Hard: return 200;
                case MiniGameType.Puzzle: return 75;
                case MiniGameType.TimedRush: return 150;
                default: return 50;
            }
        }

        /// <summary>
        /// Verarbeitet Energy-Regeneration
        /// </summary>
        private void ProcessEnergyRegen()
        {
            DateTime now = DateTime.Now;
            TimeSpan timeSinceLastRegen = now - lastEnergyRegenTime;

            float minutesPassed = (float)timeSinceLastRegen.TotalMinutes;
            int energyToRegen = Mathf.FloorToInt(minutesPassed / energyRegenMinutes);

            if (energyToRegen > 0 && energy < maxEnergy)
            {
                energy = Mathf.Min(energy + energyToRegen, maxEnergy);
                lastEnergyRegenTime = now.AddMinutes(-(minutesPassed % energyRegenMinutes));
                SaveEnergyState();
                OnEnergyChanged?.Invoke(energy);
            }
        }

        #region Save/Load

        private void SaveEnergyState()
        {
            PlayerPrefs.SetInt("Energy", energy);
            PlayerPrefs.SetInt("MaxEnergy", maxEnergy);
            PlayerPrefs.SetString("LastEnergyRegenTime", lastEnergyRegenTime.ToString("O"));
            PlayerPrefs.Save();
        }

        private void LoadEnergyState()
        {
            energy = PlayerPrefs.GetInt("Energy", 5);
            maxEnergy = PlayerPrefs.GetInt("MaxEnergy", 10);

            string timeStr = PlayerPrefs.GetString("LastEnergyRegenTime", "");
            if (!string.IsNullOrEmpty(timeStr) && DateTime.TryParse(timeStr, out DateTime loadedTime))
            {
                lastEnergyRegenTime = loadedTime;
            }
            else
            {
                lastEnergyRegenTime = DateTime.Now;
            }
        }

        #endregion
    }

    /// <summary>
    /// Mini-Game Types
    /// </summary>
    public enum MiniGameType
    {
        Easy,        // 2-3 Min, 5-10 Crystals
        Medium,      // 4-6 Min, 15-30 Crystals
        Hard,        // 8-10 Min, 50-100 Crystals
        Puzzle,      // 5 Min, 20 Crystals
        TimedRush    // 3 Min, 35 Crystals
    }

    /// <summary>
    /// Mini-Game Result
    /// </summary>
    public class MiniGameResult
    {
        public MiniGameType gameType;
        public bool won;
        public int score;
        public int crystalReward;
        public long stardustReward;
        public int xpReward;
    }
}
