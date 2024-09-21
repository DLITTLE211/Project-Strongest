using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[Serializable]
public class Affliction : StatusEffect
{
    public Effect_Affliction affliction;
    public DurationType durationType;
    public ActiveState currentState;
    public float activeDuration;
    public float damageValue;
    public bool _isConsumed;
    void SendEffect() 
    {
        AssignStatusEffect(affliction);
    }
    public void AssignStatusEffect(Effect_Affliction effect)
    {
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(Affliction)), CanEditMultipleObjects]
    public class AfflictionEditor : Editor
    {
        private void OnEnable()
        {
        }
        public override void OnInspectorGUI()
        {
            Affliction element = (Affliction)target;
            element.affliction = (Effect_Affliction)EditorGUILayout.EnumPopup("AfflictionType", element.affliction);
            element.durationType = (DurationType)EditorGUILayout.EnumPopup("DurationType", element.durationType);
            element.currentState = (ActiveState)EditorGUILayout.EnumPopup("ActiveState", element.currentState);
            element.damageValue = EditorGUILayout.FloatField("DamageValue", element.damageValue);
            element._isConsumed = EditorGUILayout.Toggle("IsConsumed",element._isConsumed);
            if (element.durationType == DurationType.Permenant)
            {
            }
            if (element.durationType == DurationType.Timed)
            {
                element.activeDuration = (float)EditorGUILayout.Slider("DurationActiveTime",element.activeDuration, 1, 7);
            }
        }
    }
#endif
}
