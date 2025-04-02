#if UNITY_EDITOR
using Mujoco;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MjGlobalSettings))]
public class MjGlobalSettingsEditor : Editor 
{
    private SerializedProperty _flagGravityProp;
    private EnableDisableFlag _lastGravityState;

    private void OnEnable()
    {
        _flagGravityProp = serializedObject.FindProperty("GlobalOptions.Flag.Gravity");
        _lastGravityState = (EnableDisableFlag)_flagGravityProp.enumValueIndex;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        DrawDefaultInspector();
        
        var currentGravityState = (EnableDisableFlag)_flagGravityProp.enumValueIndex;
        if (currentGravityState != _lastGravityState)
        {
            _lastGravityState = currentGravityState;
            
            var settings = (MjGlobalSettings)target;
            settings.OnGravityToggled();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif