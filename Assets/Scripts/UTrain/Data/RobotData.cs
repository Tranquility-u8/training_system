// RobotData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Robot/Configuration")]
public class RobotData : ScriptableObject
{
    public List<UJointLinkConfig> jointConfigs;
    
    public float globalMassScale = 1.0f;
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public float defaultFriction = 0.5f;
}