using UnityEngine;
using UnityEngine.Serialization;

public class HingeJointController : MonoBehaviour
{
    private HingeJoint joint;
    
    [Header("Joint Settings")]
    [SerializeField] private float minAngle = -90f;
    [SerializeField] private float maxAngle = 90f;
    private float maxSpringForce = 100f;
    private float damping = 500000f;

    void Start()
    {
        joint = GetComponent<HingeJoint>();
        joint.useSpring = true;
        
        var spring = new JointSpring();
        spring.spring = maxSpringForce;
        spring.damper = damping;
        spring.targetPosition = 0;
        joint.spring = spring;
    }

    public void SetJointTarget(float normalizedAngle, float strength)
    {
        float targetAngle = Mathf.Lerp(minAngle, maxAngle, (normalizedAngle + 1f) * 0.5f);
        
        var spring = joint.spring;
        spring.targetPosition = targetAngle;
        spring.spring = Mathf.Lerp(0, maxSpringForce, (strength + 1f) * 0.5f);
        joint.spring = spring;
    }
}