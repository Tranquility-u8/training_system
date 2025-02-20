using UnityEngine;  
using UnityEditor;  

[CustomEditor(typeof(UJointLink))]  
public class UJointLinkEditor : Editor  
{  
    public override void OnInspectorGUI()  
    {  
        base.OnInspectorGUI();  

        ULink uLink = (ULink)target;  

        // 检查是否有collider  
        if (uLink.GetComponents<Collider>().Length == 0)  
        {  
            EditorGUILayout.HelpBox("UJointLink requires at least one collider!", MessageType.Warning);  
        }  
    }  
}