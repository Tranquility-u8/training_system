using System;
using Mujoco;
using UnityEngine;

public class UTFreeJoint : MjFreeJoint
{
    public Vector3 ParentPos;

    private void Awake()
    {
        ParentPos = transform.parent.position;
    }
}