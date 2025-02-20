#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

public class RobotEditorWindow : EditorWindow
{
    [MenuItem("Tools/UTrain")]
    public static void ShowWindow()
    {
        GetWindow<RobotEditorWindow>("UTrain Editor");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Upload FBX Model"))
        {
            string path = EditorUtility.OpenFilePanel("Select Model", "", "fbx");
            if (!string.IsNullOrEmpty(path))
            {
                ProcessFBXModel(path);
            }
        }
    }

    void ProcessFBXModel(string fbxPath)  
    {  
        // Load Model  
        string localPath = "Assets/" + Path.GetFileName(fbxPath);  
        FileUtil.CopyFileOrDirectory(fbxPath, localPath);  
        AssetDatabase.ImportAsset(localPath);  

        // Load the model as a GameObject  
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(localPath);  
        GameObject prefabInstance = Instantiate(model);  

        // Check if the root object has a MeshFilter  
        MeshFilter rootMeshFilter = prefabInstance.GetComponent<MeshFilter>();  
        GameObject newParent = prefabInstance;  

        if (rootMeshFilter != null)  
        {  
            newParent = new GameObject(prefabInstance.name);  
            newParent.transform.position = prefabInstance.transform.position;  
            newParent.transform.rotation = prefabInstance.transform.rotation;  
            newParent.transform.localScale = prefabInstance.transform.localScale;  
            
            prefabInstance.transform.SetParent(newParent.transform);  
        }  
        
        URobot robot = newParent.AddComponent<URobot>();  
        
        ProcessChildren(prefabInstance.transform, newParent.transform);
        for (int i = 0; i < newParent.transform.childCount; i++)
        {
            Transform child = newParent.transform.GetChild(i);
            child.name = "link" + i;
            child.AddComponent<ULink>();
        }
        
        
        // Create Prefab  
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(  
            newParent,  
            $"Assets/Robot_{System.DateTime.Now:yyyyMMddHHmmss}.prefab"  
        );  

        // Save changes  
        EditorUtility.SetDirty(prefab);  
        AssetDatabase.SaveAssets();  
    }  

    void ProcessChildren(Transform parent, Transform newParent)  
    {
        while (parent.childCount > 0)
        {  
            Transform child = parent.GetChild(0);  
            ProcessChildren(child, newParent);  
        }  
        parent.SetParent(newParent);
    }
    
}
#endif