using UnityEngine;
using UnityEditor;
using CelestialMerge;

namespace CelestialMerge.Editor
{
    /// <summary>
    /// Custom Editor für CelestialItemDatabase um Sprite-Zuweisung zu erleichtern
    /// </summary>
    [CustomEditor(typeof(CelestialItemDatabase))]
    public class CelestialItemDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Zeige Standard-Inspector
            DrawDefaultInspector();

            CelestialItemDatabase database = (CelestialItemDatabase)target;

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("💡 Tipp: Ziehe Sprites aus dem Project-Fenster in die 'Item Sprite' Felder der Items!", MessageType.Info);

            // Prüfe ob Items vorhanden sind
            if (database != null)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("🔍 Prüfe Items ohne Sprites", GUILayout.Height(30)))
                {
                    CheckMissingSprites(database);
                }
            }
        }

        private void CheckMissingSprites(CelestialItemDatabase database)
        {
            int missingCount = 0;
            System.Text.StringBuilder missingItems = new System.Text.StringBuilder();

            // Verwende Reflection um auf private 'items' Liste zuzugreifen
            var itemsField = typeof(CelestialItemDatabase).GetField("items", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (itemsField != null)
            {
                var items = itemsField.GetValue(database) as System.Collections.IList;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var spriteField = item.GetType().GetField("itemSprite");
                        if (spriteField != null)
                        {
                            Sprite sprite = spriteField.GetValue(item) as Sprite;
                            if (sprite == null)
                            {
                                var nameField = item.GetType().GetField("itemName");
                                if (nameField != null)
                                {
                                    string itemName = nameField.GetValue(item) as string;
                                    missingItems.AppendLine($"  - {itemName}");
                                    missingCount++;
                                }
                            }
                        }
                    }
                }
            }

            if (missingCount > 0)
            {
                EditorUtility.DisplayDialog("Items ohne Sprites", 
                    $"Es wurden {missingCount} Items ohne Sprites gefunden:\n\n{missingItems.ToString()}", 
                    "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Alle Items haben Sprites!", 
                    "Alle Items in der Database haben Sprites zugewiesen. ✅", 
                    "OK");
            }
        }
    }
}
