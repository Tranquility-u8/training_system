using UnityEngine;
using System;
using Mujoco;

public class UTHingeJoint : MjHingeJoint
{
    Vector3 childInitialPosition;
    Quaternion childInitialRotation;

    Vector3 parentInitialPosition;
    Quaternion parentInitialRotation;
    
    public Vector3 Anchor => this.transform.position;
    public Vector3 LocalPointA => Parent.transform.InverseTransformPoint(Anchor);
    public Vector3 LocalPointB => Child.transform.InverseTransformPoint(Anchor);
    public Vector3 Axis => this.transform.right;
    public Vector3 AxisA => Quaternion.Inverse(Parent.transform.rotation) * Axis;
    public Vector3 AxisB => Quaternion.Inverse(Child.transform.rotation) * Axis;
    
    private void Awake()
    {
        childInitialPosition = Child.transform.position;
        childInitialRotation = Child.transform.rotation;
        
        parentInitialPosition = Parent.transform.position;
        parentInitialRotation = Parent.transform.rotation;
    }

    // Robot DOF -> Unity Transform
    public void reset()
    {
        Child.transform.position = childInitialPosition;
        Child.transform.rotation = childInitialRotation;
        
        Parent.transform.position = parentInitialPosition;
        Parent.transform.rotation = parentInitialRotation;
        
        Child.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Child.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        
        Parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Parent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        
        JointMotor motor = Child.GetComponent<HingeJoint>().motor;
        motor.targetVelocity = 0;
        motor.force = 0;  
    }
    
    public void setPosition(double val)
    {

    }

    public void setVelocity(float val)
    {
        JointMotor motor = Child.GetComponent<HingeJoint>().motor;
        motor.targetVelocity = val;
    }

    public void setAcceleration(double val)
    {
        
    }
}
