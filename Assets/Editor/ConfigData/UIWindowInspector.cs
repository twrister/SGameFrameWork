using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using UnityEditor;

namespace SthGame
{
    [CustomEditor(typeof(UIBaseView), true)]
    public class UIWindowInspector : Editor
    {
        bool toolToggle;
        static string searchedMemberName = "";

        public override void OnInspectorGUI()
        {
            UIBaseView view = target as UIBaseView;

            searchedMemberName = EditorGUILayout.TextField("Quick Search", searchedMemberName);

            EditorGUILayout.Space();

            if (!string.IsNullOrEmpty(searchedMemberName))
            {
                bool enterChildren = true;
                SerializedProperty iterator = serializedObject.GetIterator();
                while (iterator.NextVisible(enterChildren))
                {
                    bool matched = false;
                    if (iterator.isArray)
                    {
                        FieldInfo field = target.GetType().GetField(iterator.name);
                        IEnumerable enumerable = field.GetValue(target) as IEnumerable;
                        if (enumerable != null)
                        {
                            foreach (var element in enumerable)
                            {
                                Object obj = element as Object;
                                if (obj != null && IsMatchSearch(obj.name, searchedMemberName))
                                {
                                    matched = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Object obj = iterator.objectReferenceValue as Object;
                        if (obj != null && IsMatchSearch(obj.name, searchedMemberName))
                        {
                            matched = true;
                        }
                    }

                    if (matched || IsMatchSearch(iterator.propertyPath, searchedMemberName))
                    {
                        using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                        {
                            EditorGUILayout.PropertyField(iterator, true);
                        }
                    }
                    enterChildren = false;
                }
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                base.OnInspectorGUI();
            }
        }

        bool IsMatchSearch(string src, string key)
        {
            return src.ToLower().Contains(key.ToLower());
        }
    }
}