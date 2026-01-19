using UnityEngine;
using UnityEditor;
using MergeWellness;

namespace MergeWellnessEditor
{
    /// <summary>
    /// Editor-Script für ItemDatabase - Fügt Context Menu hinzu
    /// </summary>
    [CustomEditor(typeof(ItemDatabase))]
    public class ItemDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ItemDatabase itemDatabase = (ItemDatabase)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Item Database Actions", EditorStyles.boldLabel);

            // Button zum Initialisieren
            if (GUILayout.Button("Initialize Default Items", GUILayout.Height(30)))
            {
                InitializeDefaultItems(itemDatabase);
            }

            EditorGUILayout.Space();

            // Button zum Leeren
            if (GUILayout.Button("Clear All Items", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Clear Items", 
                    "Möchten Sie wirklich alle Items löschen?", 
                    "Ja", "Abbrechen"))
                {
                    ClearItems(itemDatabase);
                }
            }
        }

        private void InitializeDefaultItems(ItemDatabase itemDatabase)
        {
            // Verwende Reflection um die private Methode aufzurufen
            var method = typeof(ItemDatabase).GetMethod("InitializeDefaultItems", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (method != null)
            {
                method.Invoke(itemDatabase, null);
                EditorUtility.SetDirty(itemDatabase);
                AssetDatabase.SaveAssets();
                Debug.Log("✓ Default Items initialisiert!");
            }
            else
            {
                // Fallback: Direkt initialisieren
                InitializeItemsDirectly(itemDatabase);
            }
        }

        private void InitializeItemsDirectly(ItemDatabase itemDatabase)
        {
            // Verwende SerializedObject um auf private Felder zuzugreifen
            SerializedObject serializedObject = new SerializedObject(itemDatabase);
            SerializedProperty itemsProperty = serializedObject.FindProperty("items");

            if (itemsProperty == null)
            {
                Debug.LogError("Items Property nicht gefunden!");
                return;
            }

            itemsProperty.ClearArray();

            // Füge Standard-Items hinzu
            AddItemToProperty(itemsProperty, "yoga_mat_tier1", "Yoga Mat", 1, "yoga", "Yoga verbessert Flexibilität und reduziert Stress.");
            AddItemToProperty(itemsProperty, "meditation_stone_tier1", "Meditation Stone", 1, "meditation", "Meditation reduziert nachweislich Angstzustände.");
            AddItemToProperty(itemsProperty, "herbal_tea_tier1", "Herbal Tea", 1, "herbal", "Kräutertee kann Entzündungen reduzieren.");

            AddItemToProperty(itemsProperty, "yoga_tier2", "Meditation Space", 2, "yoga", "Ein ruhiger Raum verbessert die Meditation.");
            AddItemToProperty(itemsProperty, "meditation_tier2", "Wellness Kit", 2, "meditation", "Wellness-Routinen stärken das Immunsystem.");
            AddItemToProperty(itemsProperty, "herbal_tier2", "Herbal Tea Blend", 2, "herbal", "Kräutermischungen haben synergetische Effekte.");

            AddItemToProperty(itemsProperty, "yoga_tier3", "Yoga Studio", 3, "yoga", "Regelmäßiges Yoga verbessert die Herzgesundheit.");
            AddItemToProperty(itemsProperty, "meditation_tier3", "Healing Garden", 3, "meditation", "Naturkontakt reduziert Cortisol-Level.");
            AddItemToProperty(itemsProperty, "herbal_tier3", "Premium Wellness Retreat", 3, "herbal", "Auszeiten fördern mentale Gesundheit.");

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(itemDatabase);
            AssetDatabase.SaveAssets();

            Debug.Log($"✓ {itemsProperty.arraySize} Default Items initialisiert!");
        }

        private void AddItemToProperty(SerializedProperty itemsProperty, string id, string name, int tier, string type, string fact)
        {
            int index = itemsProperty.arraySize;
            itemsProperty.InsertArrayElementAtIndex(index);

            SerializedProperty item = itemsProperty.GetArrayElementAtIndex(index);
            item.FindPropertyRelative("itemId").stringValue = id;
            item.FindPropertyRelative("itemName").stringValue = name;
            item.FindPropertyRelative("tier").intValue = tier;
            item.FindPropertyRelative("itemType").stringValue = type;
            item.FindPropertyRelative("wellnessFact").stringValue = fact;
        }

        private void ClearItems(ItemDatabase itemDatabase)
        {
            SerializedObject serializedObject = new SerializedObject(itemDatabase);
            SerializedProperty itemsProperty = serializedObject.FindProperty("items");

            if (itemsProperty != null)
            {
                itemsProperty.ClearArray();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(itemDatabase);
                AssetDatabase.SaveAssets();
                Debug.Log("✓ Alle Items gelöscht!");
            }
        }
    }
}
