using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[System.Serializable]
[CreateAssetMenu(fileName = "New Status Effect", menuName = "Amplifier")]
public class Amplifiers : StatusEffect 
{
    public Effect_Amplify amplifier;
    public AmplifyType amplifyType;
    [Range(1f,5f)] public float fillTime;
    public float activeDuration;
    void SendEffect()
    {
        AssignStatusEffect(amplifier);
    }
    public void AssignStatusEffect(Effect_Amplify effect)
    {
    }
    #if UNITY_EDITOR
    [CustomEditor(typeof(Amplifiers)), CanEditMultipleObjects]
    public class AmplifiersEditor : Editor
    {
        private SerializedProperty currentAmplifer;
        private SerializedProperty fillTime;
        private void OnEnable()
        {
            currentAmplifer = serializedObject.FindProperty("amplifier");
            fillTime = serializedObject.FindProperty("fillTime");
        }
        public override void OnInspectorGUI()
        {
            Amplifiers element = (Amplifiers)target;
            element.amplifier = (Effect_Amplify)EditorGUILayout.EnumPopup(element.amplifier);
            EditorGUILayout.PropertyField(fillTime);
            element.amplifyType = (AmplifyType)EditorGUILayout.EnumPopup(element.amplifyType);

            if (element.amplifyType == AmplifyType.Permenant)
            {
            }
            if (element.amplifyType == AmplifyType.Timed)
            {
                element.activeDuration = (float)EditorGUILayout.FloatField(element.activeDuration);
            }
        }
    }
    #endif
}
[Serializable]
public enum AmplifyType 
{
    Permenant,
    Timed,
}