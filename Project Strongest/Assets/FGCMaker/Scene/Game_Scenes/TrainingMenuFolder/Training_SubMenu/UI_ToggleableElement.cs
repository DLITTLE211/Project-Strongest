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
    List<object> activeObject = new List<object>();
    #endregion
    public List<object> GetActiveObject() 
    {
        if(activeObject.Count != 0) 
        {
            return activeObject;
        }
        if (_toggleStyle == ToggledElement.ButtonToggle) 
        {
            activeObject.Add(_elementSlider);
        }
        if(_toggleStyle == ToggledElement.SliderToggle) 
        {
            activeObject.Add(_leftButton);
            activeObject.Add(_rightButton);
        }
        return activeObject;
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UI_ToggleableElement)),CanEditMultipleObjects]
    public class ToggleableElementEditor : Editor
    {
        private SerializedProperty textField;
        private SerializedProperty elementImage;
        private void OnEnable()
        {
            textField = serializedObject.FindProperty("_elementTextField");
            elementImage = serializedObject.FindProperty("_elementBackgroundImage");
        }
        public override void OnInspectorGUI()
        {
            UI_ToggleableElement element = (UI_ToggleableElement)target;
            element._toggleStyle = (ToggledElement)EditorGUILayout.EnumPopup(element._toggleStyle);
            EditorGUILayout.PropertyField(textField);
            EditorGUILayout.PropertyField(elementImage);

            if (element._toggleStyle == ToggledElement.ButtonToggle)
            {
                element._leftButton = (Button)EditorGUILayout.ObjectField(element._leftButton,typeof(Button),true);
                element._rightButton = (Button)EditorGUILayout.ObjectField(element._rightButton, typeof(Button), true);
            }
            if (element._toggleStyle == ToggledElement.SliderToggle)
            {
                element._elementSlider = (Slider)EditorGUILayout.ObjectField(element._elementSlider, typeof(Slider), true);
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