using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ScriptsGenerator : EditorWindow
{
    [MenuItem("Tools/Scripts Generator")]
    public static void CreateWindow()
    {
        var window = GetWindow<ScriptsGenerator>(false, "Scripts Generator", true);
        window.minSize = new Vector2(350, 200);
    }

    string fileName = "NewScript";
    string folderName = "_Temp";

    void OnGUI()
    {
        EditorGUILayout.LabelField("输入这组Controller与View的名称");
        EditorGUILayout.LabelField("（输入Mission 生成MissionController.cs和MissionView.cs）");
        fileName = EditorGUILayout.TextField(fileName);

        GUILayout.Space(10);

        EditorGUILayout.LabelField("输出位置：/Scripts/Game/" + folderName);

        GUILayout.Space(10);

        if (GUILayout.Button("生成"))
        {
            string ctrlTemp = null;
            string viewTemp = null;
            using (StreamReader sr = new StreamReader(Application.dataPath + "/Editor/ConfigData/Controller.txt"))
            {
                ctrlTemp = sr.ReadToEnd();

                using (StreamWriter sw = new StreamWriter(Application.dataPath + "/Scripts/Game/" + folderName + "/" + fileName + "Controller.cs"))
                {
                    string ret = ctrlTemp.Replace("{name}", fileName);
                    sw.Write(ret);
                    Debug.Log("generate controller success!");
                }
            }

            using (StreamReader sr = new StreamReader(Application.dataPath + "/Editor/ConfigData/View.txt"))
            {
                viewTemp = sr.ReadToEnd();

                using (StreamWriter sw = new StreamWriter(Application.dataPath + "/Scripts/Game/" + folderName + "/" + fileName + "View.cs"))
                {
                    string ret = viewTemp.Replace("{name}", fileName);
                    sw.Write(ret);
                    Debug.Log("generate view success!");
                }
            }
        }
    }
}
