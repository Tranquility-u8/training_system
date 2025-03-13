using UnityEngine;
using System;
using Mujoco;

public class UTHingeJoint : MjHingeJoint
{
    Vector3 initialPosition;
    Quaternion initialRotation;

    public Vector3 Anchor => this.transform.position;
    public Vector3 LocalPointA => Parent.transform.InverseTransformPoint(Anchor);
    public Vector3 LocalPointB => Child.transform.InverseTransformPoint(Anchor);
    public Vector3 Axis => this.transform.right;
    
    private void Awake()
    {
        initialPosition = Child.transform.position;
        initialRotation = Child.transform.rotation;
    }

    // Robot DOF -> Unity Transform
    public void setPosition(double val)
    {
        Child.transform.position = initialPosition; // TODO
        Child.transform.rotation = initialRotation;
    }

    public void setVelocity(double val)
    {
        
    }

    public void setAcceleration(double val)
    {
        
    }
}
