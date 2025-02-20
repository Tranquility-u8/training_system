#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using Debug = UnityEngine.Debug;

public class UTrainWindow : EditorWindow
{
    [Header("Trainer Settings")]
    private string trainerType = "ppo";
    private readonly string[] trainerOptions = { "ppo", "sac", "ddpg" };
    private int keepCheckpoints = 5;
    private int maxSteps = 20000000;
    private int timeHorizon = 1000;
    private int summaryFreq = 60000;
    
    [Header("Hyper Parameters")]
    private int batchSize = 512;
    private float learningRate = 0.0003f;
    
    [Header("Network Settings")]
    private int hiddenUnits = 128;
    private int numLayers = 2;
    
    private string configPath = "Train/test.yaml";

    [Header("Reward Signals")]
    private float gamma = 0.995f;
    private float strength = 1.0f;

    [MenuItem("Tools/UTrain")]
    public static void ShowWindow()
    {
        GetWindow<UTrainWindow>("UTrain Editor");
    }

    void OnGUI()
    {
        DrawModelSection();
        DrawTrainingParamsSection();
        DrawConfigOperations();
        DrawRunBackendOperations();
        DrawDisplayTensorBoardOperations();
    }

    void DrawModelSection()
    {
        GUILayout.Label("Model Configuration", EditorStyles.boldLabel);
        if (GUILayout.Button("Upload Model"))
        {
            string path = EditorUtility.OpenFilePanel("Select Model", "", "fbx");
            if (!string.IsNullOrEmpty(path))
            {
                ProcessFBXModel(path);
            }
        }
    }

    void DrawTrainingParamsSection()
    {
        GUILayout.Space(10);
        GUILayout.Label("Trainer Settings", EditorStyles.boldLabel);
        
        int selectedType = System.Array.IndexOf(trainerOptions, trainerType);
        selectedType = EditorGUILayout.Popup("Trainer Type", selectedType, trainerOptions);
        trainerType = trainerOptions[selectedType];
        
        keepCheckpoints = EditorGUILayout.IntField("Keep Checkpoints", keepCheckpoints);
        maxSteps = EditorGUILayout.IntField("Max Steps", maxSteps);
        timeHorizon = EditorGUILayout.IntField("Time Horizon", timeHorizon);
        summaryFreq = EditorGUILayout.IntField("Summary Freq", summaryFreq);
        
        GUILayout.Space(10);
        GUILayout.Label("Hyper Parameters", EditorStyles.boldLabel);
        
        batchSize = EditorGUILayout.IntField("Batch Size", batchSize);
        learningRate = EditorGUILayout.FloatField("Learning Rate", learningRate);
        
        GUILayout.Space(10);
        GUILayout.Label("Network Settings", EditorStyles.boldLabel);
        
        hiddenUnits = EditorGUILayout.IntField("Hidden Units", hiddenUnits);
        numLayers = EditorGUILayout.IntField("Network Layers", numLayers);
        
        GUILayout.Space(10);
        GUILayout.Label("Reward Signals", EditorStyles.boldLabel);
        gamma = EditorGUILayout.FloatField("Gamma", gamma);
        strength = EditorGUILayout.FloatField("Strength", strength);
    }

    void DrawConfigOperations()
    {
        GUILayout.Space(20);
        GUILayout.Label("Training Configuration", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Save"))
            {
                SaveYamlConfig();
            }
            
            if (GUILayout.Button("Load"))
            {
                LoadYamlConfig();
            }
        }
        GUILayout.EndHorizontal();
    }

    void SaveYamlConfig()
    {
        string fullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), configPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

        StringBuilder yamlContent = new StringBuilder();
        yamlContent.AppendLine("behaviors:");
        yamlContent.AppendLine("  RobotReacher:");
        yamlContent.AppendLine($"    trainer_type: {trainerType}");
        yamlContent.AppendLine("    hyperparameters:");
        yamlContent.AppendLine($"      batch_size: {batchSize}");
        yamlContent.AppendLine("      buffer_size: 20480");
        yamlContent.AppendLine($"      learning_rate: {learningRate}");
        yamlContent.AppendLine("      beta: 0.001");
        yamlContent.AppendLine("      epsilon: 0.2");
        yamlContent.AppendLine("      lambd: 0.95");
        yamlContent.AppendLine("      num_epoch: 3");
        yamlContent.AppendLine("      learning_rate_schedule: linear");
        yamlContent.AppendLine("    network_settings:");
        yamlContent.AppendLine("      normalize: true");
        yamlContent.AppendLine($"      hidden_units: {hiddenUnits}");
        yamlContent.AppendLine($"      num_layers: {numLayers}");
        yamlContent.AppendLine("      vis_encode_type: simple");
        yamlContent.AppendLine("    reward_signals:");
        yamlContent.AppendLine("      extrinsic:");
        yamlContent.AppendLine($"        gamma: {gamma}");
        yamlContent.AppendLine($"        strength: {strength}");
        yamlContent.AppendLine($"    keep_checkpoints: {keepCheckpoints}");
        yamlContent.AppendLine($"    max_steps: {maxSteps}");
        yamlContent.AppendLine($"    time_horizon: {timeHorizon}");
        yamlContent.AppendLine($"    summary_freq: {summaryFreq}");

        File.WriteAllText(fullPath, yamlContent.ToString());
        AssetDatabase.Refresh();
        
        Debug.Log($"Configuration saved to {fullPath}");
    }

    void LoadYamlConfig()
    {
        string fullPath = Path.Combine(Application.dataPath, configPath);
        if (!File.Exists(fullPath))
        {
            Debug.LogWarning("Config file not found!");
            return;
        }

        string[] lines = File.ReadAllLines(fullPath);
        foreach (string line in lines)
        {
            if (line.Contains("trainer_type:"))
                trainerType = GetValue(line);
            else if (line.Contains("batch_size:"))
                batchSize = int.Parse(GetValue(line));
            else if (line.Contains("learning_rate:"))
                learningRate = float.Parse(GetValue(line));
            else if (line.Contains("hidden_units:"))
                hiddenUnits = int.Parse(GetValue(line));
            else if (line.Contains("num_layers:"))
                numLayers = int.Parse(GetValue(line));
        }
        
        Debug.Log("Configuration loaded successfully");
    }

    string GetValue(string line)
    {
        return line.Split(':')[1].Trim();
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

    void DrawRunBackendOperations()
    {
        GUILayout.Space(20);
        GUILayout.Label("ML-Agent Backend", EditorStyles.boldLabel);
        if (GUILayout.Button("Run Backend"))  
        {  
            RunBatchFile("train");  
        }  
    }
    
    void DrawDisplayTensorBoardOperations()
    {
        GUILayout.Space(20);
        GUILayout.Label("TensorBoard", EditorStyles.boldLabel);
        if (GUILayout.Button("Run TensorBoard"))  
        {  
            RunBatchFile("display");
            Application.OpenURL("http://localhost:6006");
        } 
    }
    
    private IEnumerator OpenURLAfterDelay(string url, float delay)  
    {  
        yield return new WaitForSeconds(delay);  
        Application.OpenURL(url);  
    }  
    
    private void RunBatchFile(string name)  
    {  
        string batFilePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Train/" + name + ".bat");;

        ProcessStartInfo startInfo = new ProcessStartInfo  
        {  
            FileName = batFilePath,  
            UseShellExecute = true,  
            RedirectStandardOutput = false,  
            CreateNoWindow = false  
        };  

        try  
        {  
            Process process = Process.Start(startInfo);  
            //process.WaitForExit();  
        }  
        catch (System.Exception ex)  
        {  
            UnityEngine.Debug.LogError("Failed to run batch file: " + ex.Message);  
        }  
    }


}
#endif