using UnityEngine;  
using UnityEditor;  

[CustomEditor(typeof(UJointLink))]  
public class UJointLinkEditor : Editor  
{  
    public override void OnInspectorGUI()  
    {  
        base.OnInspectorGUI();  

        UJointLink uLink = (UJointLink)target;  
        
        if (uLink.GetComponents<Collider>().Length == 0)  
        {  
            EditorGUILayout.HelpBox("UJointLink requires at least one collider!", MessageType.Warning);  
        }  
    }  
}