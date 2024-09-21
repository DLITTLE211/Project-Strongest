using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[Serializable]
public class Amplifiers : StatusEffect 
{
    public Effect_Amplify amplifier;
    public DurationType durationType;
    public ActiveState currentState = ActiveState.Inactive;
    public int fillTime;
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
        private void OnEnable()
        {
            Amplifiers element = (Amplifiers)target;
        }
        public override void OnInspectorGUI()
        {
            Amplifiers element = (Amplifiers)target;
            element.amplifier = (Effect_Amplify)EditorGUILayout.EnumPopup(element.amplifier);
            element.fillTime = EditorGUILayout.IntSlider(element.fillTime,0, 5);
            element.durationType = (DurationType)EditorGUILayout.EnumPopup(element.durationType);
            element.currentState = (ActiveState)EditorGUILayout.EnumPopup(element.currentState);

            if (element.durationType == DurationType.Permenant)
            {
            }
            if (element.durationType == DurationType.Timed)
            {
                element.activeDuration = (float)EditorGUILayout.Slider(element.activeDuration,1,7);
            }
        }
    }
    #endif
}
