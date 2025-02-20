using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class URobot : MonoBehaviour
{
    [SerializeField] private List<UJointConfig> jointConfigs = new List<UJointConfig>();
    
    public void GenerateRobotData()
    {
        jointConfigs.Clear();
    
        foreach (Transform child in transform)
        {
            ULink link = child.GetComponent<ULink>();
            if (!ValidateLink(link)) continue;

            UJointConfig config = new UJointConfig
            {
                linkName = child.name,
                mass = link.GetComponent<Rigidbody>().mass,
                anchorPosition = link.GetComponent<ConfigurableJoint>().anchor,
                axis = link.GetComponent<ConfigurableJoint>().axis,
                colliderSize = CalculateColliderSize(link.colliders),
                angularXDrive = link.GetComponent<ConfigurableJoint>().angularXDrive
            };
        
            jointConfigs.Add(config);
        }
        
        RobotData data = ScriptableObject.CreateInstance<RobotData>();
        data.jointConfigs = jointConfigs;
        AssetDatabase.CreateAsset(data, $"Assets/{name}_Data.asset");
    }

    bool ValidateLink(ULink link)
    {
        bool isValid = true;
        if (link.GetComponent<Rigidbody>() == null)
        {
            Debug.LogError($"Missing Rigidbody on {link.name}");
            isValid = false;
        }
        if (link.GetComponent<ConfigurableJoint>() == null)
        {
            Debug.LogError($"Missing ConfigurableJoint on {link.name}");
            isValid = false;
        }
        if (link.colliders.Length == 0)
        {
            Debug.LogError($"Missing Colliders on {link.name}");
            isValid = false;
        }
        return isValid;
    }

    Vector3 CalculateColliderSize(Collider[] colliders)
    {
        // 实现碰撞体尺寸计算逻辑
        // 示例返回第一个碰撞体的尺寸
        if (colliders.Length > 0)
        {
            if (colliders[0] is BoxCollider box)
                return box.size;
            if (colliders[0] is SphereCollider sphere)
                return Vector3.one * sphere.radius;
        }
        return Vector3.zero;
    }
}
