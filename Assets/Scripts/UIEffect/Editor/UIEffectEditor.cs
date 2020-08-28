using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SthGame
{
    [CustomEditor(typeof(UIEffect))]
    [CanEditMultipleObjects]
    public class UIEffectEditor : Editor
    {
        SerializedProperty _spEffectMode;
        SerializedProperty _spEffectFactor;

        protected void OnEnable()
        {
            _spEffectMode = serializedObject.FindProperty("m_EffectMode");
            _spEffectFactor = serializedObject.FindProperty("m_EffectFactor");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_spEffectMode);

            if (_spEffectMode.intValue != (int)EffectMode.None)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_spEffectFactor);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}