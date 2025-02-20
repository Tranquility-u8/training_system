using UnityEngine;

public class MuJoCoPE : PhysicsEngineBase
{
    public override void ApplyTorque(Rigidbody rb, Vector3 torque)
    {
        rb.AddTorque(torque);
    }
}