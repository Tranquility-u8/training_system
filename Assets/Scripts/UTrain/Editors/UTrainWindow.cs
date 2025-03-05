#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using Mujoco;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;
using Debug = UnityEngine.Debug;

public class UTrainWindow : EditorWindow
{
    private Vector2 scrollPos;  
 
    public static bool isRunning = false;
    private string selectedBehaviorName;  
    
    public IPhysicsEngine ActiveEngine { get; private set; }
    public static bool IsPhysX => engineType == "PhysX";
    public static bool IsMuJoCo => engineType == "MuJoCo";
    public static MjGlobalSettings mjGlobalSetting;
    
    public static string engineType 
    {
        get => EditorPrefs.GetString("EngineType", "PhysX"); // 默认值
        set => EditorPrefs.SetString("EngineType", value);
    }
    [Header("Physics Engine Settings")]
    private readonly string[] engineOptions = { "PhysX", "MuJoCo", "Damps" };
    
    [Header("Trainer Settings")]
    private string trainerType = "ppo";
    private readonly string[] trainerOptions = { "ppo", "sac", "ddpg" };
    private int keepCheckpoints = 5;
    private int checkpointInterval = 100000;
    private int maxSteps = 10000000;
    private int timeHorizon = 1000;
    private int summaryFreq = 60000;
    
    [Header("Hyper Parameters")]
    private int batchSize = 512;
    private int bufferSize = 20480;
    private float learningRate = 0.0003f;
    private float beta = 0.001f;
    private float epsilon = 0.2f;
    
    [Header("Network Settings")]
    private int hiddenUnits = 128;
    private int numLayers = 2;
    

    [Header("Reward Signals")]
    private float gamma = 0.995f;
    private float strength = 1.0f;

    [MenuItem("Tools/UTrain")]
    public static void ShowWindow()
    {
        GetWindow<UTrainWindow>("UTrain Platform");
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        DrawModelSection();
        DrawBehaviourSelection();
        DrawTrainingParamsSection();
        DrawConfigOperations();
        DrawRunBackendOperations();
        DrawDisplayTensorBoardOperations();
        
        EditorGUILayout.EndScrollView();  
    }

    void DrawBehaviourSelection()
    {
        GUILayout.Label("Select a Behavior", EditorStyles.boldLabel);  
        
        BehaviorParameters[] behaviors = FindObjectsOfType<BehaviorParameters>();  
        
        string[] behaviorNames = new string[behaviors.Length];  
        for (int i = 0; i < behaviors.Length; i++)  
        {  
            behaviorNames[i] = behaviors[i].BehaviorName;  
        }  
        
        if (behaviorNames.Length > 0)  
        {  
            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(behaviorNames, selectedBehaviorName));  
            selectedIndex = EditorGUILayout.Popup("Select Behavior Name", selectedIndex, behaviorNames);  
            selectedBehaviorName = behaviorNames[selectedIndex];  
        }  
        else  
        {  
            EditorGUILayout.LabelField("No BehaviorParameters found in the scene.");  
        }  
    }
    
    void DrawModelSection()
    {
        GUILayout.Space(10);
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
        GUILayout.Label("Physics Engine Settings", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        int selectedEngineType = System.Array.IndexOf(engineOptions,engineType);
        selectedEngineType = EditorGUILayout.Popup("Physics Engine Type", selectedEngineType, engineOptions);
        engineType = engineOptions[selectedEngineType];

        var manager = PhysicsManager.instance;
        if (manager == null)
        {
            //Debug.LogWarning("Physics Engine Settings not found.");
        }
        if (EditorGUI.EndChangeCheck()) {
            //Undo.RecordObject(manager, "Change Physics Engine");
            SetEngine(engineType);
        }
        
        GUILayout.Space(10); 
        GUILayout.Label("Trainer Settings", EditorStyles.boldLabel);
        
        int selectedTrainerType = System.Array.IndexOf(trainerOptions, trainerType);
        selectedTrainerType = EditorGUILayout.Popup("Trainer Type", selectedTrainerType, trainerOptions);
        trainerType = trainerOptions[selectedTrainerType];
        
        keepCheckpoints = EditorGUILayout.IntField("Keep Checkpoints", keepCheckpoints);
        checkpointInterval = EditorGUILayout.IntField("Checkpoint Interval", checkpointInterval);
        maxSteps = EditorGUILayout.IntField("Max Steps", maxSteps);
        timeHorizon = EditorGUILayout.IntField("Time Horizon", timeHorizon);
        summaryFreq = EditorGUILayout.IntField("Summary Freq", summaryFreq);
        
        GUILayout.Space(10);
        GUILayout.Label("Hyper Parameters", EditorStyles.boldLabel);
        
        batchSize = EditorGUILayout.IntField("Batch Size", batchSize);
        bufferSize = EditorGUILayout.IntField("Buffer Size", bufferSize);
        learningRate = EditorGUILayout.FloatField("Learning Rate", learningRate);
        beta = EditorGUILayout.FloatField("Beta", beta);
        epsilon = EditorGUILayout.FloatField("Epsilon", epsilon);
        
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
        string fullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Train/" + selectedBehaviorName + ".yaml");
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

        StringBuilder yamlContent = new StringBuilder();
        yamlContent.AppendLine("behaviors:");
        yamlContent.AppendLine($"  {selectedBehaviorName}:");
        yamlContent.AppendLine($"    trainer_type: {trainerType}");
        yamlContent.AppendLine("    hyperparameters:");
        yamlContent.AppendLine($"      batch_size: {batchSize}");
        yamlContent.AppendLine($"      buffer_size: {bufferSize}");
        yamlContent.AppendLine($"      learning_rate: {learningRate}");
        yamlContent.AppendLine($"      beta: {beta}");
        yamlContent.AppendLine($"      epsilon: {epsilon}");
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
        yamlContent.AppendLine($"    checkpoint_interval: {checkpointInterval}");
        yamlContent.AppendLine($"    max_steps: {maxSteps}");
        yamlContent.AppendLine($"    time_horizon: {timeHorizon}");
        yamlContent.AppendLine($"    summary_freq: {summaryFreq}");

        File.WriteAllText(fullPath, yamlContent.ToString());
        AssetDatabase.Refresh();
        
        Debug.Log($"Configuration saved to {fullPath}");
    }

    void LoadYamlConfig()
    {
        string fullPath = Path.Combine(Application.dataPath, "Train/" + selectedBehaviorName + ".yaml");
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
        
        UTAgent agent = newParent.AddComponent<UTAgent>();  
        
        ProcessChildren(prefabInstance.transform, newParent.transform);
        for (int i = 0; i < newParent.transform.childCount; i++)
        {
            Transform child = newParent.transform.GetChild(i);
            child.name = "link" + i;
            child.AddComponent<UJointLink>();
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
            RunTrainBatch(selectedBehaviorName);  
        }  
    }
    
    void DrawDisplayTensorBoardOperations()
    {
        GUILayout.Space(20);
        GUILayout.Label("TensorBoard", EditorStyles.boldLabel);
        if (GUILayout.Button("Run TensorBoard"))  
        {  
            RunBatchByName("display");
            Application.OpenURL("http://localhost:6006");
        } 
    }
    
    private void RunBatchByName(string name)  
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

    private void RunDisplayBatch()  
    {  
        string batFilePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Train/display.bat");;  

        string batContent = $@"  
@echo off  
call conda activate ml
cd D:\Unity\Project\training_system\Train  
tensorboard --logdir results --port 6006
pause
";  
        File.WriteAllText(batFilePath, batContent);  
    
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
            // process.WaitForExit();  
        }  
        catch (System.Exception ex)  
        {  
            UnityEngine.Debug.LogError("Failed to run batch file: " + ex.Message);  
        }  
    }
    
    private void RunTrainBatch(string runId)  
    {  
        string batFilePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Train/train.bat");;  
        
        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string batContent = $@"  
@echo off  
call conda activate ml  
cd D:\Unity\Project\training_system\Train  
mlagents-learn {runId}.yaml --run-id {runId + "_" + timeStamp} --torch-device cuda:0  
pause  
";  
        File.WriteAllText(batFilePath, batContent);  
    
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
            // process.WaitForExit();  
        }  
        catch (System.Exception ex)  
        {  
            UnityEngine.Debug.LogError("Failed to run batch file: " + ex.Message);  
        }  
    }

    public void SetEngine(string engineName) {
        UpdateSceneComponents();
    }

    private void UpdateSceneComponents()
    {
        bool isPhysX = IsPhysX;
        HandleMjComponents(isPhysX);
        if (isPhysX)
        {
            HandleBodyComponents(isPhysX);
            HandleJointComponents(isPhysX);
        }
        else
        {
            HandleJointComponents(isPhysX);
            HandleBodyComponents(isPhysX);
        }

        LateHandleJointComponents(isPhysX);
    }
    
    void HandleJointComponents(bool isPhysX)
    {
        var mjjs = FindObjectsOfType<MjHingeJoint>();
        foreach (var mjj in mjjs)
        {
            var parent = mjj.Parent;
            var child = mjj.Child;
            var hj = child.GetComponent<HingeJoint>();

            if (isPhysX)
            {
                if (!hj)
                {
                    hj = child.AddComponent<HingeJoint>();
                }
                Vector3 worldAxis = mjj.transform.right;
                hj.axis = VectorUtils.RoundTinyToZero(child.transform.InverseTransformDirection(worldAxis).normalized);
                hj.anchor = VectorUtils.RoundTinyToZero(child.transform.InverseTransformPoint(mjj.transform.position)); 
            }
            else
            {
                if(hj)
                    DestroyImmediate(hj);
            }

        }
    }

    void HandleBodyComponents(bool isPhysX) {
        var objs = FindObjectsOfType<MjMeshFilter>();
        foreach (var obj in objs) {
            var rb = obj.GetComponent<Rigidbody>();
            var cld = obj.GetComponent<MeshCollider>();
            if (isPhysX)
            {
                if (!rb)
                {
                    rb = obj.AddComponent<Rigidbody>();
                }
                rb.useGravity = false;
                rb.isKinematic = false;
                
                if (!cld)
                {
                    cld = obj.AddComponent<MeshCollider>();
                    cld.convex = true;
                }
                
            } else{
                if(cld)
                    DestroyImmediate(cld);
                if(rb)
                    DestroyImmediate(rb);
            }
        }
    }

    void LateHandleJointComponents(bool isPhysX)
    {
        if(!isPhysX) return;
        
        var mjJoints = FindObjectsOfType<MjHingeJoint>();
        foreach (var mjj in mjJoints) {
            var hj = mjj.Child.GetComponent<HingeJoint>();
            GameObject parent = mjj.Parent.gameObject;
            hj.connectedBody = parent.GetComponent<Rigidbody>();
        } 
    }
        
    void HandleMjComponents(bool isPhysX)
    {
        SetAllMjComponentsEnabled(!isPhysX);
    }
    
    static void SetAllMjComponentsEnabled(bool enabled)
    {
        System.Type mjType = typeof(MjComponent);
        foreach (GameObject go in GetAllSceneObjects())
        {
            foreach (MonoBehaviour mb in go.GetComponents<MonoBehaviour>())
            {
                if (mb == null) continue; 
                
                if (mjType.IsAssignableFrom(mb.GetType()))
                {
                    Undo.RecordObject(mb, enabled ? "Enable MjComponent" : "Disable MjComponent");
                    mb.enabled = enabled;
                }
            }
        }
    }
    
    static IEnumerable<GameObject> GetAllSceneObjects()
    {
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        foreach (GameObject root in activeScene.GetRootGameObjects())
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                yield return child.gameObject;
            }
        }
    }

    
}
#endif