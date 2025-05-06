using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoteTypeStats))]
public class NoteTypeStatsEditor : Editor
{
    private SerializedProperty noteImage_Prop;
    private SerializedProperty noteSuccessImage_Prop;
    private SerializedProperty noteFailImage_Prop;
    private SerializedProperty isRest_Prop;
    private SerializedProperty type_Prop;
    private SerializedProperty ability_Prop;
    private SerializedProperty pressedLowerBound_Prop;
    private SerializedProperty pressedUpperBound_Prop;
    private SerializedProperty releaseLowerBound_Prop;
    private SerializedProperty releaseUpperBound_Prop;

    private void OnEnable()
    {
        noteImage_Prop = serializedObject.FindProperty("noteImage");
        noteSuccessImage_Prop = serializedObject.FindProperty("noteSuccessImage");
        noteFailImage_Prop = serializedObject.FindProperty("noteFailImage");
        isRest_Prop = serializedObject.FindProperty("isRest");
        type_Prop = serializedObject.FindProperty("type");
        ability_Prop = serializedObject.FindProperty("ability");
        pressedLowerBound_Prop = serializedObject.FindProperty("PressedLowerBound");
        pressedUpperBound_Prop = serializedObject.FindProperty("PressedUpperBound");
        releaseLowerBound_Prop = serializedObject.FindProperty("ReleaseLowerBound");
        releaseUpperBound_Prop = serializedObject.FindProperty("ReleaseUpperBound");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(noteImage_Prop, new GUIContent("Note Image"));
        EditorGUILayout.PropertyField(noteSuccessImage_Prop, new GUIContent("Note Success Image"));
        EditorGUILayout.PropertyField(noteFailImage_Prop, new GUIContent("Note Fail Image"));
        EditorGUILayout.PropertyField(type_Prop, new GUIContent("Note Type"));
        EditorGUILayout.PropertyField(isRest_Prop, new GUIContent("Is Rest"));

        if (!isRest_Prop.boolValue)
        {
            EditorGUILayout.PropertyField(ability_Prop, new GUIContent("Ability"));

            EditorGUILayout.PropertyField(pressedLowerBound_Prop, new GUIContent("Pressed Lower Bound"));
            EditorGUILayout.PropertyField(pressedUpperBound_Prop, new GUIContent("Pressed Upper Bound"));
            if ((NoteTypeStats.Type)type_Prop.enumValueIndex == NoteTypeStats.Type.Whole || (NoteTypeStats.Type)type_Prop.enumValueIndex == NoteTypeStats.Type.Half)
            {
                EditorGUILayout.PropertyField(releaseLowerBound_Prop, new GUIContent("Release Lower Bound"));
                EditorGUILayout.PropertyField(releaseUpperBound_Prop, new GUIContent("Release Upper Bound"));
            }

        }

        serializedObject.ApplyModifiedProperties();
    }
}
