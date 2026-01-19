using UnityEngine;
using System;

namespace CelestialMerge
{
    /// <summary>
    /// Verwaltet Dual Currency System: Stardust (Primary) + Crystals (Secondary)
    /// </summary>
    public class CurrencyManager : MonoBehaviour
    {
        [Header("Currency")]
        [SerializeField] private long stardust = 0;
        [SerializeField] private int crystals = 0;

        [Header("Settings")]
        [SerializeField] private long maxStardustCapacity = 5000; // Erweitert sich mit Level
        [SerializeField] private bool unlimitedStardust = false; // Soft Cap entfernen

        // Events
        public event Action<long> OnStardustChanged;
        public event Action<int> OnCrystalsChanged;

        public long Stardust => stardust;
        public int Crystals => crystals;
        public long MaxStardustCapacity => maxStardustCapacity;

        private void Awake()
        {
            LoadCurrency();
            
            // Speichere unlimitedStardust falls es im Inspector gesetzt wurde
            if (unlimitedStardust)
            {
                SaveCurrency(); // Speichere sofort wenn im Inspector aktiviert
                Debug.Log("✅ Unlimited Stardust aktiviert und gespeichert");
            }
            
            // Trigger initial Event für UI-Update
            OnStardustChanged?.Invoke(stardust);
            OnCrystalsChanged?.Invoke(crystals);
        }

        /// <summary>
        /// Fügt Stardust hinzu (mit Capacity-Check)
        /// </summary>
        public bool AddStardust(long amount, bool bypassCapacity = false)
        {
            if (amount <= 0) return false;

            long newAmount = stardust + amount;

            // Capacity Check (nur wenn nicht unlimited)
            if (!unlimitedStardust && !bypassCapacity && newAmount > maxStardustCapacity)
            {
                // Spieler hat Capacity erreicht - könnte Premium Upgrade anbieten
                Debug.LogWarning($"⚠️ Stardust Capacity erreicht! ({maxStardustCapacity}) - Setze 'Unlimited Stardust' im Inspector um Capacity zu umgehen!");
                stardust = maxStardustCapacity;
            }
            else
            {
                stardust = newAmount;
                if (unlimitedStardust)
                {
                    Debug.Log($"💰 Stardust hinzugefügt: +{amount} → {stardust} (Unlimited aktiviert)");
                }
            }

            SaveCurrency();
            OnStardustChanged?.Invoke(stardust);
            return true;
        }

        /// <summary>
        /// Entfernt Stardust
        /// </summary>
        public bool SpendStardust(long amount)
        {
            if (amount <= 0 || stardust < amount)
            {
                return false;
            }

            stardust -= amount;
            SaveCurrency();
            OnStardustChanged?.Invoke(stardust);
            return true;
        }

        /// <summary>
        /// Fügt Crystals hinzu
        /// </summary>
        public bool AddCrystals(int amount)
        {
            if (amount <= 0) return false;

            crystals += amount;
            SaveCurrency();
            OnCrystalsChanged?.Invoke(crystals);
            return true;
        }

        /// <summary>
        /// Entfernt Crystals
        /// </summary>
        public bool SpendCrystals(int amount)
        {
            if (amount <= 0 || crystals < amount)
            {
                return false;
            }

            crystals -= amount;
            SaveCurrency();
            OnCrystalsChanged?.Invoke(crystals);
            return true;
        }

        /// <summary>
        /// Setzt Capacity (wird mit Player Level erhöht)
        /// </summary>
        public void SetMaxStardustCapacity(long capacity)
        {
            maxStardustCapacity = capacity;
            SaveCurrency();
        }

        /// <summary>
        /// Aktiviert Unlimited Stardust (Premium Feature)
        /// </summary>
        public void EnableUnlimitedStardust()
        {
            unlimitedStardust = true;
            SaveCurrency();
        }

        #region Save/Load

        private void SaveCurrency()
        {
            PlayerPrefs.SetString("Stardust", stardust.ToString());
            PlayerPrefs.SetInt("Crystals", crystals);
            PlayerPrefs.SetString("MaxStardustCapacity", maxStardustCapacity.ToString());
            PlayerPrefs.SetInt("UnlimitedStardust", unlimitedStardust ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadCurrency()
        {
            string stardustStr = PlayerPrefs.GetString("Stardust", "0");
            if (long.TryParse(stardustStr, out long loadedStardust))
            {
                stardust = loadedStardust;
            }

            crystals = PlayerPrefs.GetInt("Crystals", 0);

            string capacityStr = PlayerPrefs.GetString("MaxStardustCapacity", "5000");
            if (long.TryParse(capacityStr, out long loadedCapacity))
            {
                maxStardustCapacity = loadedCapacity;
            }

            unlimitedStardust = PlayerPrefs.GetInt("UnlimitedStardust", 0) == 1;
            
            // Prüfe ob Stardust über Capacity ist (kann passieren wenn Capacity später gesetzt wird)
            if (!unlimitedStardust && stardust > maxStardustCapacity)
            {
                Debug.LogWarning($"⚠️ Stardust ({stardust}) über Capacity ({maxStardustCapacity}) - wird auf Capacity gesetzt");
                stardust = maxStardustCapacity;
            }
        }

        #endregion
    }
}
