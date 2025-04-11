using UnityEngine;

public class UTJointController: MonoBehaviour
{
    [Header("Joint Drive Settings")]
    [Space(10)]
    public float maxJointSpring;

    public float jointDampen;
    public float maxJointForceLimit;

    public void SetUpJoint(ConfigurableJoint joint)
    {
        var jd = new JointDrive
        {
            positionSpring = maxJointSpring,
            positionDamper = jointDampen,
            maximumForce = maxJointForceLimit
        };
        joint.slerpDrive = jd;
    }
    
    public void SetJointTargetRotation(ConfigurableJoint joint, float x, float y, float z)
    {
        x = (x + 1f) * 0.5f;
        y = (y + 1f) * 0.5f;
        z = (z + 1f) * 0.5f;

        var xRot = Mathf.Lerp(-180, 180, x);
        var yRot = Mathf.Lerp(-180, 180, y);
        var zRot = Mathf.Lerp(-180, 180, z);

        joint.targetRotation = Quaternion.Euler(xRot, yRot, zRot);
    }

    public void SetJointStrength(ConfigurableJoint joint ,float strength)
    {
        var rawVal = (strength + 1f) * 0.5f * maxJointForceLimit;
        var jd = new JointDrive
        {
            positionSpring = maxJointSpring,
            positionDamper = jointDampen,
            maximumForce = rawVal
        };
        joint.slerpDrive = jd;
    }
}
