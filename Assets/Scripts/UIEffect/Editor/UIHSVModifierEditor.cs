using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SthGame
{
    [CustomEditor(typeof(UIHSVModifier))]
    [CanEditMultipleObjects]
    public class UIHSVModifierEditor : Editor
    {
        SerializedProperty _spTargetColor;
        SerializedProperty _spRange;
        SerializedProperty _spHue;
        SerializedProperty _spSaturation;
        SerializedProperty _spValue;

        protected void OnEnable()
        {
            _spTargetColor = serializedObject.FindProperty("m_TargetColor");
            _spRange = serializedObject.FindProperty("m_Range");
            _spHue = serializedObject.FindProperty("m_Hue");
            _spSaturation = serializedObject.FindProperty("m_Saturation");
            _spValue = serializedObject.FindProperty("m_Value");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_spTargetColor);
            EditorGUILayout.PropertyField(_spRange);
            EditorGUILayout.PropertyField(_spHue);
            EditorGUILayout.PropertyField(_spSaturation);
            EditorGUILayout.PropertyField(_spValue);

            serializedObject.ApplyModifiedProperties();
        }
    }
}