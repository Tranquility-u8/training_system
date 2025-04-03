using UnityEngine;

public class HingeJointController : MonoBehaviour
{
    private HingeJoint hinge;
    public float TargetAngle { get; set; }  
    public float Stiffness = 1000f;         
    public float Damping = 50f;             

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = false;
        hinge.useMotor = true;  
    }

    void FixedUpdate()
    {
        float currentAngle = hinge.angle;
        currentAngle = (currentAngle > 180) ? currentAngle - 360 : currentAngle;
        
        float error = TargetAngle - currentAngle;
        float velocity = error * Stiffness - hinge.velocity * Damping;
        
        JointMotor motor = hinge.motor;
        motor.targetVelocity = velocity;
        motor.force = Stiffness * 2;  
        hinge.motor = motor;
    }
}