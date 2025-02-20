using UnityEngine;  
using UnityEditor;  

[CustomEditor(typeof(ULink))]  
public class ULinkEditor : Editor  
{  
    public override void OnInspectorGUI()  
    {  
        base.OnInspectorGUI();  

        ULink uLink = (ULink)target;  

        // 检查是否有collider  
        if (uLink.GetComponents<Collider>().Length == 0)  
        {  
            EditorGUILayout.HelpBox("ULink requires at least one collider!", MessageType.Warning);  
        }  
    }  
}