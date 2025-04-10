using UnityEngine;

public class HingeJointController : MonoBehaviour
{
    private HingeJoint hinge;
    public float TargetAngle;
    public float Stiffness = 500f;
    public float Damping = 50f;

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;
        hinge.useMotor = false;
        
        JointSpring spring = new JointSpring();
        spring.spring = Stiffness;
        spring.damper = Damping;
        hinge.spring = spring;
    }

    void FixedUpdate()
    {
        JointSpring spring = hinge.spring;
        spring.targetPosition = TargetAngle;
        hinge.spring = spring;
    }
}