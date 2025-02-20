using UnityEngine;

public class PhysXPE : PhysicsEngineBase
{
    public override void ApplyTorque(Rigidbody rb, Vector3 torque)
    {
        rb.AddTorque(torque);
    }
}