using UnityEngine;
using System.Collections.Generic;

namespace CelestialMerge
{
    /// <summary>
    /// Cross-Item Crafting System: Kombiniert Items aus verschiedenen Kategorien
    /// </summary>
    public class CraftingSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CelestialItemDatabase itemDatabase; // ScriptableObject Asset
        [SerializeField] private CurrencyManager currencyManager;

        private Dictionary<string, CraftingRecipe> recipes = new Dictionary<string, CraftingRecipe>();

        // Events
        public event System.Action<CraftingRecipe, CelestialItem> OnItemCrafted;

        private void Awake()
        {
            InitializeRecipes();
        }

        /// <summary>
        /// Initialisiert Crafting Recipes (aus GDD)
        /// </summary>
        private void InitializeRecipes()
        {
            // Stellar Engine: Proto-Star Lvl3 + Energy Beacon Lvl3 + Ancient Rune Lvl2
            recipes["stellar_engine"] = new CraftingRecipe
            {
                recipeId = "stellar_engine",
                recipeName = "Stellar Engine",
                inputItems = new List<CraftingInput>
                {
                    new CraftingInput { category = "celestial_bodies", level = 3 },
                    new CraftingInput { category = "structures", level = 3 },
                    new CraftingInput { category = "artifacts", level = 2 }
                },
                outputItemId = "structures_level10_rare", // Warp Drive
                rewardStardust = 1000,
                rewardXP = 500
            };

            // Life Sanctuary: Microbe Spore Lvl5 + Solar Collector Lvl5 + Glowing Totem Lvl3
            recipes["life_sanctuary"] = new CraftingRecipe
            {
                recipeId = "life_sanctuary",
                recipeName = "Life Sanctuary",
                inputItems = new List<CraftingInput>
                {
                    new CraftingInput { category = "lifeforms", level = 5 },
                    new CraftingInput { category = "structures", level = 5 },
                    new CraftingInput { category = "artifacts", level = 3 }
                },
                outputItemId = "lifeforms_level10_rare", // Genesis Chamber
                rewardStardust = 2000,
                rewardXP = 1000
            };

            // Dimensional Rift: Wormhole Lvl25 + Infinity Stone Lvl15 + Cosmic Guardian Lvl18
            recipes["dimensional_rift"] = new CraftingRecipe
            {
                recipeId = "dimensional_rift",
                recipeName = "Dimensional Rift",
                inputItems = new List<CraftingInput>
                {
                    new CraftingInput { category = "celestial_bodies", level = 25 },
                    new CraftingInput { category = "artifacts", level = 15 },
                    new CraftingInput { category = "lifeforms", level = 18 }
                },
                outputItemId = "artifacts_level20_legendary", // Reality Breach
                rewardStardust = 10000,
                rewardXP = 5000
            };
        }

        /// <summary>
        /// Versucht Items zu craften
        /// </summary>
        public CraftingResult TryCraft(List<CelestialItem> inputItems)
        {
            if (inputItems == null || inputItems.Count < 2)
            {
                return new CraftingResult { Success = false, ErrorMessage = "Nicht genug Items!" };
            }

            // Suche passendes Recipe
            foreach (var recipe in recipes.Values)
            {
                if (MatchesRecipe(inputItems, recipe))
                {
                    return ExecuteCraft(recipe, inputItems);
                }
            }

            return new CraftingResult { Success = false, ErrorMessage = "Kein passendes Recipe gefunden!" };
        }

        /// <summary>
        /// Prüft ob Items zu Recipe passen
        /// </summary>
        private bool MatchesRecipe(List<CelestialItem> items, CraftingRecipe recipe)
        {
            if (items.Count != recipe.inputItems.Count) return false;

            // Erstelle Kopie für Matching
            List<CraftingInput> remainingInputs = new List<CraftingInput>(recipe.inputItems);

            foreach (var item in items)
            {
                bool matched = false;
                for (int i = 0; i < remainingInputs.Count; i++)
                {
                    var input = remainingInputs[i];
                    if (item.Category == input.category && item.Level == input.level)
                    {
                        remainingInputs.RemoveAt(i);
                        matched = true;
                        break;
                    }
                }
                if (!matched) return false;
            }

            return remainingInputs.Count == 0;
        }

        /// <summary>
        /// Führt Crafting aus
        /// </summary>
        private CraftingResult ExecuteCraft(CraftingRecipe recipe, List<CelestialItem> inputItems)
        {
            // Erstelle Output Item
            CelestialItem outputItem = itemDatabase.CreateItem(recipe.outputItemId);
            if (outputItem == null)
            {
                return new CraftingResult { Success = false, ErrorMessage = "Output Item konnte nicht erstellt werden!" };
            }

            // Gebe Rewards
            if (currencyManager != null)
            {
                currencyManager.AddStardust(recipe.rewardStardust);
            }

            OnItemCrafted?.Invoke(recipe, outputItem);

            return new CraftingResult
            {
                Success = true,
                CraftedItem = outputItem,
                StardustReward = recipe.rewardStardust,
                XpReward = recipe.rewardXP
            };
        }

        /// <summary>
        /// Gibt alle verfügbaren Recipes zurück
        /// </summary>
        public List<CraftingRecipe> GetAvailableRecipes()
        {
            return new List<CraftingRecipe>(recipes.Values);
        }
    }

    /// <summary>
    /// Crafting Recipe Datenstruktur
    /// </summary>
    [System.Serializable]
    public class CraftingRecipe
    {
        public string recipeId;
        public string recipeName;
        public List<CraftingInput> inputItems;
        public string outputItemId;
        public long rewardStardust;
        public int rewardXP;
    }

    /// <summary>
    /// Crafting Input Anforderung
    /// </summary>
    [System.Serializable]
    public class CraftingInput
    {
        public string category;
        public int level;
    }

    /// <summary>
    /// Crafting Result
    /// </summary>
    public class CraftingResult
    {
        public bool Success;
        public string ErrorMessage;
        public CelestialItem CraftedItem;
        public long StardustReward;
        public int XpReward;
    }
}
