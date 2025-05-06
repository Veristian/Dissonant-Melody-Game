using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEditor.Rendering;

[CustomEditor(typeof(Button))]
public class ButtonEditor : Editor
{
    private SerializedProperty CanActivateOnStep_Prop;

    private SerializedProperty type_Prop;
    private SerializedProperty hasExitTime_Prop;
    private SerializedProperty exitTime_Prop;
    private SerializedProperty onActivationEvent_Prop;
    private SerializedProperty onDeactivationEvent_Prop;
    private SerializedProperty onSprite_Prop;
    private SerializedProperty offSprite_Prop;

    private void OnEnable()
    {
        CanActivateOnStep_Prop = serializedObject.FindProperty("CanActivateOnStep");
        type_Prop = serializedObject.FindProperty("type");
        hasExitTime_Prop = serializedObject.FindProperty("hasExitTime");
        exitTime_Prop = serializedObject.FindProperty("exitTime");
        onActivationEvent_Prop = serializedObject.FindProperty("OnActivationEvent");
        onDeactivationEvent_Prop = serializedObject.FindProperty("OnDeactivationEvent");
        onSprite_Prop = serializedObject.FindProperty("onSprite");
        offSprite_Prop = serializedObject.FindProperty("offSprite");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(CanActivateOnStep_Prop, new GUIContent("Can Activate On Step"));
        EditorGUILayout.PropertyField(type_Prop, new GUIContent("Type"));

        if (type_Prop.GetEnumValue<Button.Type>() == Button.Type.Button)
        {
            EditorGUILayout.PropertyField(hasExitTime_Prop, new GUIContent("Has Exit Time"));
            if (hasExitTime_Prop.boolValue)
            {
                EditorGUILayout.PropertyField(exitTime_Prop, new GUIContent("Exit Time"));
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(onActivationEvent_Prop, new GUIContent("On Activation Event"));
        EditorGUILayout.PropertyField(onDeactivationEvent_Prop, new GUIContent("On Deactivation Event"));
        
        EditorGUILayout.LabelField("Sprite", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(onSprite_Prop, new GUIContent("On Sprite"));
        EditorGUILayout.PropertyField(offSprite_Prop, new GUIContent("Off Sprite"));

        serializedObject.ApplyModifiedProperties();
    }
}
