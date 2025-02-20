#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

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
                //ProcessFBXModel(path);
            }
        }
    }

    void ProcessFBXModel(string fbxPath)
    {
        // Load Model
        string localPath = "Assets/" + Path.GetFileName(fbxPath);
        FileUtil.CopyFileOrDirectory(fbxPath, localPath);
        AssetDatabase.ImportAsset(localPath);

        // Create Prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(
            Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(localPath)),
            $"Assets/Robot_{System.DateTime.Now:yyyyMMddHHmmss}.prefab"
        );

        // Formalize
        URobot robot = prefab.AddComponent<URobot>();
        int linkIndex = 0;
        foreach (Transform child in prefab.transform)
        {
            child.gameObject.AddComponent<ULink>();
            child.name = $"link{linkIndex++}";
        }
        
        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
    }
}
#endif