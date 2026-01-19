using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace CelestialMerge.Editor
{
    /// <summary>
    /// Editor-Tool zum automatischen Zuweisen von Sprites zu Items basierend auf Namen
    /// </summary>
    public class SpriteAutoAssigner : EditorWindow
    {
        private CelestialItemDatabase database;
        private string searchPath = "Assets/Sprites";

        [MenuItem("CelestialMerge/Tools/Auto-Assign Sprites")]
        public static void ShowWindow()
        {
            GetWindow<SpriteAutoAssigner>("Sprite Auto-Assigner");
        }

        private void OnGUI()
        {
            GUILayout.Label("Sprite Auto-Assigner", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Database auswählen
            database = (CelestialItemDatabase)EditorGUILayout.ObjectField(
                "Celestial Item Database", 
                database, 
                typeof(CelestialItemDatabase), 
                false
            );

            GUILayout.Space(10);

            // Search Path
            searchPath = EditorGUILayout.TextField("Sprite Search Path", searchPath);

            GUILayout.Space(10);

            if (database == null)
            {
                EditorGUILayout.HelpBox("Bitte wähle eine CelestialItemDatabase aus!", MessageType.Warning);
                return;
            }

            // Auto-Assign Button
            if (GUILayout.Button("Auto-Assign Sprites by Name", GUILayout.Height(30)))
            {
                AutoAssignSprites();
            }

            GUILayout.Space(10);

            // Find Duplicates Button
            if (GUILayout.Button("Find Duplicate Sprite Assignments", GUILayout.Height(30)))
            {
                FindDuplicateAssignments();
            }

            GUILayout.Space(10);

            // Fix Duplicates Button
            if (GUILayout.Button("Fix Duplicate Assignments (Clear & Reassign)", GUILayout.Height(30)))
            {
                FixDuplicateAssignments();
            }

            GUILayout.Space(10);

            // Manual Assign Button
            if (GUILayout.Button("Open Database in Inspector", GUILayout.Height(30)))
            {
                Selection.activeObject = database;
                EditorGUIUtility.PingObject(database);
            }
        }

        private void AutoAssignSprites()
        {
            if (database == null)
            {
                Debug.LogError("Database ist null!");
                return;
            }

            // Lade alle Sprites aus dem Pfad
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { searchPath });
            Sprite[] allSprites = guids.Select(guid => 
                AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid))
            ).Where(s => s != null).ToArray();

            Debug.Log($"Gefunden: {allSprites.Length} Sprites");

            // Verwende Reflection um auf private items Liste zuzugreifen
            var itemsField = typeof(CelestialItemDatabase).GetField(
                "items", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            if (itemsField == null)
            {
                Debug.LogError("Konnte 'items' Feld nicht finden!");
                return;
            }

            var items = itemsField.GetValue(database) as System.Collections.IList;
            if (items == null)
            {
                Debug.LogError("Items Liste ist null!");
                return;
            }

            // Set zum Verfolgen bereits zugewiesener Sprites
            HashSet<Sprite> usedSprites = new HashSet<Sprite>();
            
            // Sammle alle bereits zugewiesenen Sprites
            foreach (var item in items)
            {
                var itemType = item.GetType();
                var spriteField = itemType.GetField("itemSprite");
                if (spriteField != null)
                {
                    Sprite currentSprite = spriteField.GetValue(item) as Sprite;
                    if (currentSprite != null)
                    {
                        usedSprites.Add(currentSprite);
                    }
                }
            }

            int assignedCount = 0;
            int skippedCount = 0;

            // Iteriere durch alle Items
            foreach (var item in items)
            {
                var itemType = item.GetType();
                var itemIdField = itemType.GetField("itemId");
                var spriteField = itemType.GetField("itemSprite");

                if (itemIdField == null || spriteField == null) continue;

                string itemId = itemIdField.GetValue(item) as string;
                if (string.IsNullOrEmpty(itemId)) continue;

                // Prüfe ob Sprite bereits zugewiesen ist
                Sprite currentSprite = spriteField.GetValue(item) as Sprite;
                if (currentSprite != null) continue; // Überspringe wenn bereits zugewiesen

                // Suche passendes Sprite nach Name
                // Format aus Dokument: stardust_particle_l1_common.png
                // Item ID Format: celestial_bodies_level1_common
                
                string itemName = itemId.ToLower();
                var nameField = itemType.GetField("itemName");
                string displayName = nameField?.GetValue(item) as string ?? "";
                
                // Finde alle passenden Sprites und sortiere nach Match-Qualität
                var matchingSprites = allSprites
                    .Where(s => !usedSprites.Contains(s)) // Nur noch nicht verwendete Sprites
                    .Select(s => new { Sprite = s, Score = CalculateMatchScore(itemName, displayName, s.name.ToLower()) })
                    .Where(m => m.Score > 0)
                    .OrderByDescending(m => m.Score)
                    .ToList();
                
                Sprite matchingSprite = matchingSprites.FirstOrDefault()?.Sprite;

                if (matchingSprite != null)
                {
                    spriteField.SetValue(item, matchingSprite);
                    usedSprites.Add(matchingSprite); // Markiere als verwendet
                    assignedCount++;
                    Debug.Log($"✅ Sprite zugewiesen: {itemId} → {matchingSprite.name} (Score: {matchingSprites.First().Score})");
                }
                else
                {
                    skippedCount++;
                    Debug.LogWarning($"⚠️ Kein passendes Sprite gefunden für: {itemId} ({displayName})");
                }
            }

            // Markiere Database als dirty (damit Änderungen gespeichert werden)
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();

            Debug.Log($"🎉 Fertig! {assignedCount} Sprites automatisch zugewiesen, {skippedCount} übersprungen!");
            EditorUtility.DisplayDialog(
                "Sprite Auto-Assign", 
                $"{assignedCount} Sprites wurden automatisch zugewiesen!\n{skippedCount} Items ohne Sprite.\n\nBitte prüfe die CelestialItemDatabase im Inspector.", 
                "OK"
            );
        }

        /// <summary>
        /// Berechnet einen Match-Score für ein Sprite basierend auf Item-ID und Display-Name
        /// Höhere Scores = bessere Matches
        /// </summary>
        private int CalculateMatchScore(string itemId, string displayName, string spriteName)
        {
            int score = 0;
            
            // Strategie 1: Exakter Match mit Display Name (höchste Priorität)
            if (!string.IsNullOrEmpty(displayName))
            {
                string normalizedDisplayName = displayName.ToLower()
                    .Replace(" ", "_")
                    .Replace("-", "_")
                    .Replace("'", "")
                    .Replace(".", "");
                
                if (spriteName == normalizedDisplayName)
                {
                    score += 1000; // Exakter Match
                }
                else if (spriteName.Contains(normalizedDisplayName))
                {
                    score += 500; // Teilstring-Match
                }
            }
            
            // Strategie 2: Match mit Level und Rarity aus Item ID
            string[] itemParts = itemId.Split('_');
            if (itemParts.Length >= 3)
            {
                string levelPart = "";
                string rarityPart = "";
                
                foreach (string part in itemParts)
                {
                    if (part.StartsWith("level") || part.All(char.IsDigit))
                    {
                        levelPart = part.Replace("level", "");
                    }
                    else if (part == "common" || part == "uncommon" || part == "rare" || 
                             part == "epic" || part == "legendary" || part == "mythic")
                    {
                        rarityPart = part;
                    }
                }
                
                if (!string.IsNullOrEmpty(levelPart) && spriteName.Contains($"l{levelPart}"))
                {
                    score += 100; // Level Match
                }
                
                if (!string.IsNullOrEmpty(rarityPart) && spriteName.Contains(rarityPart))
                {
                    score += 50; // Rarity Match
                }
            }
            
            // Strategie 3: Fuzzy Match - zähle übereinstimmende Wörter
            string[] spriteWords = spriteName.Split('_', ' ', '-');
            string[] itemWords = itemId.Split('_');
            
            int matchCount = 0;
            foreach (string itemWord in itemWords)
            {
                // Überspringe Kategorie/Rarity Wörter
                if (itemWord == "level" || itemWord == "common" || itemWord == "uncommon" || 
                    itemWord == "rare" || itemWord == "epic" || itemWord == "legendary" || 
                    itemWord == "mythic" || itemWord == "celestial" || itemWord == "bodies" ||
                    itemWord == "structures" || itemWord == "lifeforms" || itemWord == "artifacts" ||
                    itemWord == "elements" || itemWord == "decorations")
                {
                    continue;
                }
                
                if (spriteWords.Any(sw => sw.Contains(itemWord) || itemWord.Contains(sw)))
                {
                    matchCount++;
                }
            }
            
            score += matchCount * 10; // 10 Punkte pro übereinstimmendem Wort
            
            return score;
        }

        /// <summary>
        /// Findet alle Items mit doppelten Sprite-Zuweisungen
        /// </summary>
        private void FindDuplicateAssignments()
        {
            if (database == null)
            {
                Debug.LogError("Database ist null!");
                return;
            }

            var itemsField = typeof(CelestialItemDatabase).GetField(
                "items", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            if (itemsField == null)
            {
                Debug.LogError("Konnte 'items' Feld nicht finden!");
                return;
            }

            var items = itemsField.GetValue(database) as System.Collections.IList;
            if (items == null)
            {
                Debug.LogError("Items Liste ist null!");
                return;
            }

            // Gruppiere Items nach Sprite
            Dictionary<Sprite, List<string>> spriteToItems = new Dictionary<Sprite, List<string>>();

            foreach (var item in items)
            {
                var itemType = item.GetType();
                var itemIdField = itemType.GetField("itemId");
                var spriteField = itemType.GetField("itemSprite");

                if (itemIdField == null || spriteField == null) continue;

                string itemId = itemIdField.GetValue(item) as string;
                Sprite sprite = spriteField.GetValue(item) as Sprite;

                if (sprite != null)
                {
                    if (!spriteToItems.ContainsKey(sprite))
                    {
                        spriteToItems[sprite] = new List<string>();
                    }
                    spriteToItems[sprite].Add(itemId);
                }
            }

            // Finde Duplikate
            var duplicates = spriteToItems.Where(kvp => kvp.Value.Count > 1).ToList();

            if (duplicates.Count == 0)
            {
                Debug.Log("✅ Keine doppelten Sprite-Zuweisungen gefunden!");
                EditorUtility.DisplayDialog(
                    "Duplicate Check",
                    "✅ Keine doppelten Sprite-Zuweisungen gefunden!",
                    "OK"
                );
                return;
            }

            // Logge alle Duplikate
            Debug.LogWarning($"⚠️ {duplicates.Count} Sprites wurden mehrfach zugewiesen:");
            string report = $"⚠️ {duplicates.Count} Sprites wurden mehrfach zugewiesen:\n\n";

            foreach (var duplicate in duplicates)
            {
                string itemList = string.Join(", ", duplicate.Value);
                Debug.LogWarning($"  Sprite '{duplicate.Key.name}' wird verwendet von: {itemList}");
                report += $"Sprite '{duplicate.Key.name}':\n  {itemList}\n\n";
            }

            EditorUtility.DisplayDialog(
                "Duplicate Sprite Assignments Found",
                report,
                "OK"
            );
        }

        /// <summary>
        /// Behebt doppelte Sprite-Zuweisungen, indem alle Sprites entfernt und neu zugewiesen werden
        /// </summary>
        private void FixDuplicateAssignments()
        {
            if (database == null)
            {
                Debug.LogError("Database ist null!");
                return;
            }

            bool confirmed = EditorUtility.DisplayDialog(
                "Fix Duplicate Assignments",
                "Dies wird ALLE Sprite-Zuweisungen entfernen und neu zuweisen.\n\nFortfahren?",
                "Ja, alle entfernen und neu zuweisen",
                "Abbrechen"
            );

            if (!confirmed) return;

            var itemsField = typeof(CelestialItemDatabase).GetField(
                "items", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            if (itemsField == null)
            {
                Debug.LogError("Konnte 'items' Feld nicht finden!");
                return;
            }

            var items = itemsField.GetValue(database) as System.Collections.IList;
            if (items == null)
            {
                Debug.LogError("Items Liste ist null!");
                return;
            }

            // Entferne alle Sprite-Zuweisungen
            int clearedCount = 0;
            foreach (var item in items)
            {
                var itemType = item.GetType();
                var spriteField = itemType.GetField("itemSprite");
                if (spriteField != null)
                {
                    spriteField.SetValue(item, null);
                    clearedCount++;
                }
            }

            Debug.Log($"🧹 {clearedCount} Sprite-Zuweisungen entfernt. Starte Neu-Zuweisung...");

            // Führe Auto-Assign erneut aus
            AutoAssignSprites();
        }
    }
}
