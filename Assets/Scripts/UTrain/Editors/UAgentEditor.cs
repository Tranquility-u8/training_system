// UAgentEditor.cs
/*

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


[CustomEditor(typeof(UTAgent))]
public class UAgentEditor : Editor
{
    private UTAgent _agent;

    void OnEnable()
    {
        _agent = (UTAgent)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        if (GUILayout.Button("Bind Child Links"))
        {
            BindChildJoints();
        }

        if (GUILayout.Button("Generate Robot Data"))
        {
            GenerateRobotData();
        }
    }

    void BindChildJoints()
    {
        _agent.joints.Clear();
        foreach (Transform child in _agent.transform)
        {
            var link = child.GetComponent<UJointLink>();
            if (link == null) continue;
            _agent.joints.Add(link);
        }
        EditorUtility.SetDirty(_agent);
    }

    void GenerateRobotData()
    {
        var data = ScriptableObject.CreateInstance<RobotData>();
        
        foreach (var joint in _agent.joints)
        {
            var config = new UJointLinkConfig()
            {
                mass = joint.rb.mass, //TODO
            };
            
            data.jointConfigs.Add(config);
        }
        
        AssetDatabase.CreateAsset(data, "Assets/RobotConfig.asset");
        AssetDatabase.SaveAssets();
    }

    string GetColliderType(UJointLink link)
    {
        var collider = link.GetComponent<Collider>();
        return collider != null ? collider.GetType().Name : "None";
    }
}
#endif

*/