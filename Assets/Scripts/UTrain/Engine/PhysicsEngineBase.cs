using UnityEngine;

public abstract class PhysicsEngineBase
{
    public abstract void ApplyTorque(Rigidbody rb, Vector3 torque);
}
