// URobotEditor.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(URobot))]
public class URobotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Robot Data"))
        {
            GenerateRobotData((URobot)target);
        }
    }

    void GenerateRobotData(URobot robot)
    {
        robot.GenerateRobotData();
        EditorUtility.SetDirty(robot);
    }
}
#endif