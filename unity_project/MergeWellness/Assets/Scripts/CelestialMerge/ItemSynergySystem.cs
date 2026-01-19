using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CelestialMerge
{
    /// <summary>
    /// Item Synergy System: Passive Boni wenn verwandte Items nebeneinander platziert werden
    /// </summary>
    public class ItemSynergySystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private CelestialProgressionManager progressionManager;

        private Dictionary<string, SynergyEffect> activeSynergies = new Dictionary<string, SynergyEffect>();
        private List<SynergyDefinition> synergyDefinitions = new List<SynergyDefinition>();

        // Events
        public event System.Action<SynergyDefinition> OnSynergyActivated;
        public event System.Action<SynergyDefinition> OnSynergyDeactivated;

        private void Awake()
        {
            InitializeSynergyDefinitions();
        }

        /// <summary>
        /// Initialisiert Synergy Definitions (aus GDD)
        /// </summary>
        private void InitializeSynergyDefinitions()
        {
            // Stellar Alignment: 3x Star-Kategorie nebeneinander
            synergyDefinitions.Add(new SynergyDefinition
            {
                synergyId = "stellar_alignment",
                synergyName = "Stellar Alignment",
                requiredCategory = "celestial_bodies",
                requiredCount = 3,
                bonusType = SynergyBonusType.StardustPerMinute,
                bonusValue = 50f,
                isPermanent = true
            });

            // Organic Growth: 3x Lifeform Kategorie
            synergyDefinitions.Add(new SynergyDefinition
            {
                synergyId = "organic_growth",
                synergyName = "Organic Growth",
                requiredCategory = "lifeforms",
                requiredCount = 3,
                bonusType = SynergyBonusType.MergeXPMultiplier,
                bonusValue = 0.2f, // +20%
                isPermanent = true
            });

            // Dimensional Link: Wormhole + Portal Artifact
            synergyDefinitions.Add(new SynergyDefinition
            {
                synergyId = "dimensional_link",
                synergyName = "Dimensional Link",
                requiredCategory = "celestial_bodies",
                requiredCount = 1,
                requiredLevel = 30, // Wormhole
                bonusType = SynergyBonusType.UnlockHiddenBoard,
                bonusValue = 1f,
                isPermanent = false // Einmalig
            });

            // Crystalline Resonance: 5x Crystals zusammen
            synergyDefinitions.Add(new SynergyDefinition
            {
                synergyId = "crystalline_resonance",
                synergyName = "Crystalline Resonance",
                requiredCategory = "artifacts",
                requiredCount = 5,
                bonusType = SynergyBonusType.ExtraCrystalPerMinigame,
                bonusValue = 1f,
                isPermanent = false,
                cooldownHours = 4
            });
        }

        /// <summary>
        /// Prüft Board auf Synergies
        /// </summary>
        public void CheckBoardForSynergies(List<CelestialItem> boardItems)
        {
            // Deaktiviere alle aktuellen Synergies
            DeactivateAllSynergies();

            // Prüfe jede Synergy Definition
            foreach (var synergyDef in synergyDefinitions)
            {
                if (CheckSynergyCondition(boardItems, synergyDef))
                {
                    ActivateSynergy(synergyDef);
                }
            }
        }

        /// <summary>
        /// Prüft ob Synergy-Bedingung erfüllt ist
        /// </summary>
        private bool CheckSynergyCondition(List<CelestialItem> items, SynergyDefinition synergy)
        {
            // Zähle Items die zur Synergy passen
            int matchingCount = items.Count(item =>
                item != null &&
                item.Category == synergy.requiredCategory &&
                (synergy.requiredLevel == 0 || item.Level == synergy.requiredLevel)
            );

            return matchingCount >= synergy.requiredCount;
        }

        /// <summary>
        /// Aktiviert Synergy
        /// </summary>
        private void ActivateSynergy(SynergyDefinition synergy)
        {
            if (activeSynergies.ContainsKey(synergy.synergyId))
            {
                return; // Bereits aktiv
            }

            SynergyEffect effect = new SynergyEffect
            {
                synergyDefinition = synergy,
                activationTime = System.DateTime.Now
            };

            activeSynergies[synergy.synergyId] = effect;
            ApplySynergyBonus(synergy);
            OnSynergyActivated?.Invoke(synergy);

            Debug.Log($"✨ Synergy aktiviert: {synergy.synergyName}");
        }

        /// <summary>
        /// Deaktiviert alle Synergies
        /// </summary>
        private void DeactivateAllSynergies()
        {
            foreach (var synergy in activeSynergies.Values)
            {
                RemoveSynergyBonus(synergy.synergyDefinition);
                OnSynergyDeactivated?.Invoke(synergy.synergyDefinition);
            }
            activeSynergies.Clear();
        }

        /// <summary>
        /// Wendet Synergy Bonus an
        /// </summary>
        private void ApplySynergyBonus(SynergyDefinition synergy)
        {
            switch (synergy.bonusType)
            {
                case SynergyBonusType.StardustPerMinute:
                    // Wird von IdleProductionManager verwendet
                    break;
                case SynergyBonusType.MergeXPMultiplier:
                    // Wird von MergeManager verwendet
                    break;
                case SynergyBonusType.UnlockHiddenBoard:
                    // Board Expansion Event
                    Debug.Log($"🔓 Hidden Board Section freigeschaltet durch {synergy.synergyName}!");
                    break;
                case SynergyBonusType.ExtraCrystalPerMinigame:
                    // Wird von MiniGameManager verwendet
                    break;
            }
        }

        /// <summary>
        /// Entfernt Synergy Bonus
        /// </summary>
        private void RemoveSynergyBonus(SynergyDefinition synergy)
        {
            // Cleanup falls nötig
        }

        /// <summary>
        /// Gibt aktive Synergies zurück
        /// </summary>
        public List<SynergyDefinition> GetActiveSynergies()
        {
            return activeSynergies.Values.Select(e => e.synergyDefinition).ToList();
        }
    }

    /// <summary>
    /// Synergy Definition
    /// </summary>
    [System.Serializable]
    public class SynergyDefinition
    {
        public string synergyId;
        public string synergyName;
        public string requiredCategory;
        public int requiredCount;
        public int requiredLevel; // 0 = any level
        public SynergyBonusType bonusType;
        public float bonusValue;
        public bool isPermanent;
        public int cooldownHours; // 0 = no cooldown
    }

    /// <summary>
    /// Synergy Effect (aktiv)
    /// </summary>
    [System.Serializable]
    public class SynergyEffect
    {
        public SynergyDefinition synergyDefinition;
        public System.DateTime activationTime;
    }

    /// <summary>
    /// Synergy Bonus Types
    /// </summary>
    public enum SynergyBonusType
    {
        StardustPerMinute,
        MergeXPMultiplier,
        UnlockHiddenBoard,
        ExtraCrystalPerMinigame,
        MergeSpeedBoost
    }
}
