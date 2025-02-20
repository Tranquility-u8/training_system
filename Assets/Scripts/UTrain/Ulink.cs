using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
public class ULink : MonoBehaviour
{
    public Collider[] colliders;
    public ULink connectedBody;
    
    private void OnValidate()
    {
    }
}

[System.Serializable]
public class UJointConfig
{
    public string linkName;
    public Vector3 anchorPosition;
    public Vector3 axis;
    public float mass;
    public Vector3 colliderSize;
    public JointDrive angularXDrive;
}