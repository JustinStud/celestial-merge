using UnityEngine;
using UnityEditor;
using MergeWellness;

namespace MergeWellnessEditor
{
    /// <summary>
    /// Editor-Script für ItemSpawner - Fügt Buttons zum Spawnen hinzu
    /// </summary>
    [CustomEditor(typeof(ItemSpawner))]
    public class ItemSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ItemSpawner spawner = (ItemSpawner)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Item Spawning", EditorStyles.boldLabel);

            // Spawn Buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🎲 Spawn Random Item", GUILayout.Height(30)))
            {
                spawner.SpawnRandomStarterItem();
            }
            if (GUILayout.Button("🎲 Spawn 3 Random", GUILayout.Height(30)))
            {
                spawner.SpawnRandomStarterItems(3);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("📦 Fill Grid", GUILayout.Height(30)))
            {
                spawner.FillGridWithRandomItems();
            }
            if (GUILayout.Button("🔗 Merge Test (2x)", GUILayout.Height(30)))
            {
                spawner.SpawnMergeTestItems();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spawn Specific Items", EditorStyles.boldLabel);

            // Spezifische Items
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Yoga Mat (T1)"))
            {
                spawner.SpawnItemById("yoga_mat_tier1");
            }
            if (GUILayout.Button("Meditation Stone (T1)"))
            {
                spawner.SpawnItemById("meditation_stone_tier1");
            }
            if (GUILayout.Button("Herbal Tea (T1)"))
            {
                spawner.SpawnItemById("herbal_tea_tier1");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Tipp: Du kannst auch im Play-Mode die Buttons verwenden!\n" +
                "Oder nutze das Context Menu (Rechtsklick auf ItemSpawner → Spawn...).",
                MessageType.Info
            );
        }
    }
}
