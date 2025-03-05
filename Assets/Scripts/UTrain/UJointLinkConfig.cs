using UnityEngine;

[System.Serializable]
public class UJointLinkConfig
{
    public string linkName;
    public Vector3 anchorPosition;
    public Vector3 axis;
    public float mass;
    public Vector3 colliderSize;
    public JointDrive angularXDrive;
}
