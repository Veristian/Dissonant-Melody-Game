using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DisappearingPlatform))]
[CanEditMultipleObjects]
public class DisappearingPlatformEditor : Editor
{
    private SerializedProperty auto_Prop;
    private SerializedProperty autoSwitchTime_Prop;
    private SerializedProperty disappearAutomatically_Prop;
    private SerializedProperty disappearTime_Prop;
    private SerializedProperty appearAutomatically_Prop;
    private SerializedProperty appearTime_Prop;
    private SerializedProperty startState_Prop;
    private SerializedProperty playerLayer_Prop;
    private SerializedProperty startTimeOffset_Prop;
    private SerializedProperty onSprite_Prop;
    private SerializedProperty offSprite_Prop;

    private bool showSettings = true;
    private bool showSprites = true;

    private void OnEnable()
    {
        auto_Prop = serializedObject.FindProperty("auto");
        autoSwitchTime_Prop = serializedObject.FindProperty("autoSwitchTime");
        disappearAutomatically_Prop = serializedObject.FindProperty("disappearAutomatically");
        disappearTime_Prop = serializedObject.FindProperty("disappearTime");
        appearAutomatically_Prop = serializedObject.FindProperty("appearAutomatically");
        appearTime_Prop = serializedObject.FindProperty("appearTime");
        startState_Prop = serializedObject.FindProperty("startState");
        playerLayer_Prop = serializedObject.FindProperty("playerLayer");
        startTimeOffset_Prop = serializedObject.FindProperty("startTimeOffset");
        onSprite_Prop = serializedObject.FindProperty("OnSprite");
        offSprite_Prop = serializedObject.FindProperty("OffSprite");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        showSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showSettings, "Settings");
        if (showSettings)
        {
            EditorGUILayout.PropertyField(auto_Prop, new GUIContent("Auto"));
            if (auto_Prop.boolValue)
            {
                EditorGUILayout.PropertyField(autoSwitchTime_Prop, new GUIContent("Auto Switch Time"));
                

            } else
            {
                EditorGUILayout.PropertyField(disappearAutomatically_Prop, new GUIContent("Disappear Automatically"));
                if (disappearAutomatically_Prop.boolValue)
                {
                    EditorGUILayout.PropertyField(disappearTime_Prop, new GUIContent("Disappear Time"));
                }

                EditorGUILayout.PropertyField(appearAutomatically_Prop, new GUIContent("Appear Automatically"));
                if (appearAutomatically_Prop.boolValue)
                {
                    EditorGUILayout.PropertyField(appearTime_Prop, new GUIContent("Appear Time"));
                }
            }

            
            EditorGUILayout.PropertyField(startState_Prop, new GUIContent("Start State"));
            EditorGUILayout.PropertyField(startTimeOffset_Prop, new GUIContent("Start Time Offset"));
            EditorGUILayout.PropertyField(playerLayer_Prop, new GUIContent("Player Layer"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showSprites = EditorGUILayout.BeginFoldoutHeaderGroup(showSprites, "Sprites");
        if (showSprites)
        {
            EditorGUILayout.PropertyField(onSprite_Prop, new GUIContent("On Sprite"));
            EditorGUILayout.PropertyField(offSprite_Prop, new GUIContent("Off Sprite"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
