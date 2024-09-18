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

    #region if Button Toggle
    public Button _leftButton,_rightButton;
    #endregion

    List<GameObject> activeObject = new List<GameObject>();
    public List<GameObject> GetActiveObject() 
    {
        if(activeObject.Count != 0) 
        {
            return activeObject;
        }
        if (_toggleStyle == ToggledElement.SliderToggle) 
        {
            activeObject.Add(_elementSlider.gameObject);
        }
        if(_toggleStyle == ToggledElement.ButtonToggle) 
        {
            activeObject.Add(_leftButton.gameObject);
            activeObject.Add(_rightButton.gameObject);
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
