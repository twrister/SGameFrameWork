using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SthGame
{
    [CustomEditor(typeof(UIToneEffect))]
    //[CanEditMultipleObjects]告诉Unity你可以用这个编辑器选择多个对象并同时改变它们
    [CanEditMultipleObjects]
    public class UIToneEffectEditor : Editor
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
            using (new MaterialDirtyScope(targets))
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