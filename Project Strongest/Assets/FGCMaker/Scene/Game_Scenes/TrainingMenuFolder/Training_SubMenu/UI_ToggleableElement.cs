using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEditor;

public class UI_ToggleableElement : MonoBehaviour
{
    public TMP_Text _elementTextField;
    public Image _elementBackgroundImage;
    public ToggledElement _toggleStyle;
    #region if Slider Toggle
    public Slider _elementSlider;
    #endregion
    #region if Slider Toggle
    public Button _leftButton,_rightButton;
    #endregion
#if UNITY_EDITOR
    [CustomEditor(typeof(UI_ToggleableElement)),CanEditMultipleObjects]
    public class ToggleableElementEditor : Editor
    {
        private SerializedProperty textField;
        private SerializedProperty elementImage;


        private SerializedProperty sliderVal;
        private SerializedProperty leftButton;
        private SerializedProperty rightButton;
        private void OnEnable()
        {
            textField = serializedObject.FindProperty("_elementTextField");
            elementImage = serializedObject.FindProperty("_elementBackgroundImage");

            sliderVal = serializedObject.FindProperty("_elementSlider");
            leftButton = serializedObject.FindProperty("_leftButton");
            rightButton = serializedObject.FindProperty("_rightButton");
        }
        public override void OnInspectorGUI()
        {
            UI_ToggleableElement element = (UI_ToggleableElement)target;
            element._toggleStyle = (ToggledElement)EditorGUILayout.EnumPopup(element._toggleStyle);
            EditorGUILayout.PropertyField(textField);
            EditorGUILayout.PropertyField(elementImage);

            if (element._toggleStyle == ToggledElement.ButtonToggle)
            {
                element._elementSlider = null;
                element._leftButton = (Button)EditorGUILayout.ObjectField(element._leftButton,typeof(Button),true);
                element._rightButton = (Button)EditorGUILayout.ObjectField(element._rightButton, typeof(Button), true);
            }
            if (element._toggleStyle == ToggledElement.SliderToggle)
            {
                element._elementSlider = (Slider)EditorGUILayout.ObjectField(element._elementSlider, typeof(Slider), true);
                element._leftButton = null;
                element._rightButton = null;
            }

        }
    }
#endif
}
[Serializable]
public enum ToggledElement 
{
    SliderToggle = 0,
    ButtonToggle = 1,
}
