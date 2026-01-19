using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using MergeWellness;
using TMPro;

namespace MergeWellnessEditor
{
    /// <summary>
    /// Editor-Script für UIManager - Hilft beim Verbinden von UI-Komponenten
    /// </summary>
    [CustomEditor(typeof(UIManager))]
    public class UIManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            UIManager uiManager = (UIManager)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UI Component Helper", EditorStyles.boldLabel);

            // Button zum automatischen Finden
            if (GUILayout.Button("🔍 Find UI Components Automatically", GUILayout.Height(30)))
            {
                FindUIComponents(uiManager);
            }

            EditorGUILayout.Space();

            // Manuelle Verbindung für ScoreText
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ScoreText:", GUILayout.Width(100));
            if (GUILayout.Button("Find ScoreText", GUILayout.Width(150)))
            {
                FindScoreText(uiManager);
            }
            EditorGUILayout.EndHorizontal();

            // Manuelle Verbindung für MergeCountText
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MergeCountText:", GUILayout.Width(100));
            if (GUILayout.Button("Find MergeCountText", GUILayout.Width(150)))
            {
                FindMergeCountText(uiManager);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Tipp: Stelle sicher, dass deine UI-Elemente 'ScoreText' und 'MergeCountText' heißen.\n" +
                "Das Script findet automatisch TextMeshPro oder normales Text.",
                MessageType.Info
            );
        }

        private void FindUIComponents(UIManager uiManager)
        {
            uiManager.FindUIComponentsManually();
            EditorUtility.SetDirty(uiManager);
        }

        private void FindScoreText(UIManager uiManager)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Kein Canvas gefunden!");
                return;
            }

            Transform scoreTextTransform = canvas.transform.Find("ScoreText");
            if (scoreTextTransform == null)
            {
                Debug.LogWarning("ScoreText GameObject nicht gefunden! Stelle sicher, dass es 'ScoreText' heißt.");
                return;
            }

            // Versuche TextMeshPro zuerst
            TextMeshProUGUI tmpText = scoreTextTransform.GetComponent<TextMeshProUGUI>();
            if (tmpText != null)
            {
                SerializedProperty prop = serializedObject.FindProperty("scoreTextPro");
                if (prop != null)
                {
                    prop.objectReferenceValue = tmpText;
                    Debug.Log("✓ ScoreText (TextMeshPro) gefunden und zugewiesen!");
                }
            }
            else
            {
                // Fallback zu normalem Text
                Text text = scoreTextTransform.GetComponent<Text>();
                if (text != null)
                {
                    SerializedProperty prop = serializedObject.FindProperty("scoreText");
                    if (prop != null)
                    {
                        prop.objectReferenceValue = text;
                        Debug.Log("✓ ScoreText (Text) gefunden und zugewiesen!");
                    }
                }
                else
                {
                    Debug.LogError("ScoreText hat weder TextMeshPro noch Text Komponente!");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void FindMergeCountText(UIManager uiManager)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Kein Canvas gefunden!");
                return;
            }

            Transform mergeCountTransform = canvas.transform.Find("MergeCountText");
            if (mergeCountTransform == null)
            {
                Debug.LogWarning("MergeCountText GameObject nicht gefunden! Stelle sicher, dass es 'MergeCountText' heißt.");
                return;
            }

            // Versuche TextMeshPro zuerst
            TextMeshProUGUI tmpText = mergeCountTransform.GetComponent<TextMeshProUGUI>();
            if (tmpText != null)
            {
                SerializedProperty prop = serializedObject.FindProperty("mergeCountTextPro");
                if (prop != null)
                {
                    prop.objectReferenceValue = tmpText;
                    Debug.Log("✓ MergeCountText (TextMeshPro) gefunden und zugewiesen!");
                }
            }
            else
            {
                // Fallback zu normalem Text
                Text text = mergeCountTransform.GetComponent<Text>();
                if (text != null)
                {
                    SerializedProperty prop = serializedObject.FindProperty("mergeCountText");
                    if (prop != null)
                    {
                        prop.objectReferenceValue = text;
                        Debug.Log("✓ MergeCountText (Text) gefunden und zugewiesen!");
                    }
                }
                else
                {
                    Debug.LogError("MergeCountText hat weder TextMeshPro noch Text Komponente!");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
