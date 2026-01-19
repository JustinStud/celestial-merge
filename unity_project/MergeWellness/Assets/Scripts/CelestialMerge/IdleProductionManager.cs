using UnityEngine;
using System;
using System.Collections.Generic;

namespace CelestialMerge
{
    /// <summary>
    /// Idle/AFK Production System: Generiert Stardust auch offline
    /// </summary>
    public class IdleProductionManager : MonoBehaviour
    {
        [Header("Production Settings")]
        [SerializeField] private float baseProductionRate = 10f; // Stardust pro Minute
        [SerializeField] private float productionMultiplier = 1.0f;
        [SerializeField] private float maxOfflineTime = 24f; // Max 24 Stunden offline

        [Header("Buildings")]
        [SerializeField] private List<ProductionBuilding> activeBuildings = new List<ProductionBuilding>();

        private CurrencyManager currencyManager;
        private DateTime lastProductionTime;
        private bool isInitialized = false;

        public float CurrentProductionRate => baseProductionRate * productionMultiplier;

        private void Awake()
        {
            currencyManager = FindFirstObjectByType<CurrencyManager>();
            LoadProductionState();
        }

        private void Start()
        {
            ProcessOfflineProduction();
            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized) return;

            // Produziere kontinuierlich (jede Sekunde)
            ProcessContinuousProduction();
        }

        /// <summary>
        /// Verarbeitet Offline-Production beim Start
        /// </summary>
        private void ProcessOfflineProduction()
        {
            if (currencyManager == null) return;

            DateTime now = DateTime.Now;
            TimeSpan offlineTime = now - lastProductionTime;

            // Begrenze auf maxOfflineTime
            if (offlineTime.TotalHours > maxOfflineTime)
            {
                offlineTime = TimeSpan.FromHours(maxOfflineTime);
            }

            // Berechne Offline-Production
            float minutesOffline = (float)offlineTime.TotalMinutes;
            long stardustEarned = Mathf.RoundToInt(minutesOffline * CurrentProductionRate);

            if (stardustEarned > 0)
            {
                currencyManager.AddStardust(stardustEarned, bypassCapacity: true);
                Debug.Log($"💰 Offline Production: {stardustEarned} Stardust ({minutesOffline:F1} Minuten offline)");
            }

            lastProductionTime = now;
            SaveProductionState();
        }

        /// <summary>
        /// Verarbeitet kontinuierliche Production (jede Sekunde)
        /// </summary>
        private void ProcessContinuousProduction()
        {
            if (currencyManager == null) return;

            // Produziere basierend auf Production Rate
            float stardustPerSecond = CurrentProductionRate / 60f;
            long stardustEarned = Mathf.RoundToInt(stardustPerSecond * Time.deltaTime);

            if (stardustEarned > 0)
            {
                currencyManager.AddStardust(stardustEarned, bypassCapacity: false);
            }
        }

        /// <summary>
        /// Fügt Production Building hinzu
        /// </summary>
        public void AddBuilding(ProductionBuilding building)
        {
            if (!activeBuildings.Contains(building))
            {
                activeBuildings.Add(building);
                RecalculateProductionRate();
            }
        }

        /// <summary>
        /// Entfernt Production Building
        /// </summary>
        public void RemoveBuilding(ProductionBuilding building)
        {
            if (activeBuildings.Remove(building))
            {
                RecalculateProductionRate();
            }
        }

        /// <summary>
        /// Berechnet Production Rate neu basierend auf Buildings
        /// </summary>
        private void RecalculateProductionRate()
        {
            float totalRate = baseProductionRate;

            foreach (var building in activeBuildings)
            {
                if (building != null)
                {
                    totalRate += building.productionRate; // productionRate ist public field
                }
            }

            productionMultiplier = totalRate / baseProductionRate;
            SaveProductionState();
        }

        #region Save/Load

        private void SaveProductionState()
        {
            PlayerPrefs.SetString("LastProductionTime", lastProductionTime.ToString("O"));
            PlayerPrefs.SetFloat("ProductionMultiplier", productionMultiplier);
            PlayerPrefs.Save();
        }

        private void LoadProductionState()
        {
            string timeStr = PlayerPrefs.GetString("LastProductionTime", "");
            if (!string.IsNullOrEmpty(timeStr) && DateTime.TryParse(timeStr, out DateTime loadedTime))
            {
                lastProductionTime = loadedTime;
            }
            else
            {
                lastProductionTime = DateTime.Now;
            }

            productionMultiplier = PlayerPrefs.GetFloat("ProductionMultiplier", 1.0f);
        }

        #endregion
    }

    /// <summary>
    /// Production Building (z.B. Energy Beacon, Solar Collector)
    /// </summary>
    [System.Serializable]
    public class ProductionBuilding
    {
        public string buildingId;
        public string buildingName;
        public float productionRate; // Stardust pro Minute
        public int level;

        public ProductionBuilding(string id, string name, float rate, int lvl)
        {
            buildingId = id;
            buildingName = name;
            productionRate = rate;
            level = lvl;
        }
    }
}
