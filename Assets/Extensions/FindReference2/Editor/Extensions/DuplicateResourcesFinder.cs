using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using vietlabs.fr2;

public class DuplicateResourcesFinder : EditorWindow
{
    private string m_OutputPath = "/DuplicateResources.csv";
    private FR2_Cache m_Cache;
    private HashSet<string> m_NotUsedAssetGUIDs = new HashSet<string>();
    private List<string> m_SelectedPaths = new List<string>();

    private Vector2 m_ScrollPos;
    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Find References2 缓存");
        m_Cache = (FR2_Cache)EditorGUILayout.ObjectField(m_Cache, typeof(FR2_Cache), false);
        EditorGUILayout.LabelField("输出路径");
        m_OutputPath = GUILayout.TextField(m_OutputPath);
        EditorGUILayout.LabelField(Application.dataPath + m_OutputPath);

        GUILayout.Label("将文件拖拽至Drag Area后点击扫描");
        if (GUILayout.Button("扫描"))
        {
            m_NotUsedAssetGUIDs.Clear();
            if (null != m_Cache)
            {
                foreach (var asset in m_Cache.AssetList)
                {
                    if (asset.UsageCount() == 0)
                        m_NotUsedAssetGUIDs.Add(asset.guid);
                }
            }
            var selectedGUIDs = new string[m_SelectedPaths.Count];
            for (int i = 0, length = m_SelectedPaths.Count; i < length; i++)
            {
                selectedGUIDs[i] = AssetDatabase.AssetPathToGUID(m_SelectedPaths[i]);
            }
            var GUIDs = GetAllGUIDsReclusively(selectedGUIDs);
            var duplicatesResources = FindDuplicateResources(GUIDs);
            ExportCSV(Application.dataPath + m_OutputPath, duplicatesResources);
        }

        if (GUILayout.Button("清除已选中"))
        {
            m_SelectedPaths.Clear();
        }

        var dragGUIDs = GetDragPaths("Drag Area");
        if (dragGUIDs.Length > 0)
        {
            for (int i = 0, length = dragGUIDs.Length; i < length; i++)
            {
                if(!m_SelectedPaths.Contains(dragGUIDs[i]))
                    m_SelectedPaths.Add(dragGUIDs[i]);
            }
        }
        
        EditorGUILayout.BeginVertical();
        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
        var removedPaths = new List<string>();
        foreach (var selectedPath in m_SelectedPaths)
        {
            EditorGUILayout.BeginHorizontal();
            var obj = AssetDatabase.LoadMainAssetAtPath(selectedPath);
            EditorGUILayout.ObjectField(obj, obj.GetType(), false);
            if (GUILayout.Button("x"))
            {
                removedPaths.Add(selectedPath);
            }
            EditorGUILayout.EndHorizontal();
        }

        foreach (var removedPath in removedPaths)
        {
            m_SelectedPaths.Remove(removedPath);
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
    
    private string[] GetAllGUIDsReclusively(string[] inputGUIDs)
    {
        var selectionGUIDs = new List<string>();
        foreach (var GUID in inputGUIDs)
        {
            selectionGUIDs.AddRange(FolderGUIDToAssetGUIDs(GUID));
        }

        return selectionGUIDs.ToArray();
    }

    private List<string> FolderGUIDToAssetGUIDs(string GUID)
    {
        var list = new List<string>();
        string path = AssetDatabase.GUIDToAssetPath(GUID);
        if (AssetDatabase.IsValidFolder(path))
        {
            var assetPaths = new List<string>();
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if(!file.EndsWith(".meta"))
                    assetPaths.Add(file);
            }

            var dirs = Directory.GetDirectories(path);
            assetPaths.AddRange(dirs);
            
            foreach (var assetPath in assetPaths)
            {
                list.AddRange(FolderGUIDToAssetGUIDs(AssetDatabase.AssetPathToGUID(SubAssetPath(assetPath))));
            }
        }
        else
        {
            list.Add(GUID);
        }

        return list;
    }

    private Dictionary<string, List<string>> FindDuplicateResources(string[] inputGUIDs)
    {
        var dependenciesDict = new Dictionary<string, List<string>>();
        foreach (var GUID in inputGUIDs)
        {
            if(m_NotUsedAssetGUIDs.Contains(GUID))
                continue;
            
            var path = AssetDatabase.GUIDToAssetPath(GUID);
            var dependencies = AssetDatabase.GetDependencies(AssetDatabase.GUIDToAssetPath(GUID));
            foreach (var dependency in dependencies)
            {
                // 排除掉脚本文件
                if(dependency.EndsWith(".cs"))
                    continue;

                List<string> list;
                if (dependenciesDict.TryGetValue(dependency, out list))
                {
                    list.Add(path);
                }
                else
                {
                    dependenciesDict.Add(dependency, new List<string>{path});
                }
            }
        }

        var onlyOneList = new List<string>();
        foreach (var dependency in dependenciesDict)
        {
            if(dependency.Value.Count > 1)
                continue;
            onlyOneList.Add(dependency.Key);
        }

        foreach (var onlyOne in onlyOneList)
        {
            dependenciesDict.Remove(onlyOne);
        }

        return dependenciesDict;
    }

    private void ExportCSV(string path, Dictionary<string, List<string>> input)
    {
        if(File.Exists(path))
            File.Delete(path);
        
        using (var csv = File.AppendText(path))
        {
            foreach (var pair in input)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(pair.Key);
                sb.Append(",");
                foreach (var prefab in pair.Value)
                {
                    sb.Append(prefab);
                    sb.Append("|");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("\n");
                csv.Write(sb.ToString());
            }
        }

        EditorUtility.DisplayDialog("扫描已完成", "扫描已完成，Csv输出目录：" + path, "确定");
        Logger.Log("Export Csv Success:" + path);
    }

    private static string SubAssetPath(string path)
    {
        int assetStartIndex = path.IndexOf("/Assets/");
        if (assetStartIndex > 0)
            return path.Substring(assetStartIndex + 1);
        return path;
    }
    
    public static string[] GetDragPaths(string msg)
    {
        Event aEvent;
        aEvent = Event.current;
 
        GUI.contentColor = Color.white;
 
        var dragArea = GUILayoutUtility.GetRect(0f, 35f, GUILayout.ExpandWidth(true));
 
        GUIContent title = new GUIContent(msg);
        if (string.IsNullOrEmpty(msg))
        {
            title = new GUIContent("Drag Object here from Project view to get the object");
        }

        var retList = new List<string>();
        
        GUI.Box(dragArea, title);
        switch (aEvent.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dragArea.Contains(aEvent.mousePosition))
                {
                    break;
                }
 
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (aEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
 
                    for (int i = 0; i < DragAndDrop.paths.Length; ++i)
                    {
                        retList.Add(DragAndDrop.paths[i]);
                    }
                }
 
                Event.current.Use();
                break;
            default:
                break;
        }
 
        return retList.ToArray();
    }

    [MenuItem("Tools/Resources/DuplicateResourcesFinder")]
    private static void OpenWindow()
    {
        var window = EditorWindow.GetWindow<DuplicateResourcesFinder>();
        window.Show();
    }
}