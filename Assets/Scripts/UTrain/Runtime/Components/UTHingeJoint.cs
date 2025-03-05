using UnityEngine;
using System;
using Mujoco;

public class UTHingeJoint : MjHingeJoint
{
    Vector3 initialPosition;
    Quaternion initialRotation;

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
