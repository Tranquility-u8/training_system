using Mujoco;
using UnityEngine;

public struct UnityData
{
    
}

public struct DampsData
{
    
}

public unsafe class UTData
{
    private UnityData* physXData;
    
    private MujocoLib.mjData_* mjData;
    
    private DampsData* dampsData;
    
    public UTData(UnityData* data)
    {
        setData(data);
    }
    
    public UTData(MujocoLib.mjData_* data)
    {
        setData(data);
    }
    
    public UTData(DampsData* data)
    {
        setData(data);
    }
    
    public void setData(UnityData* data)
    {
        physXData = data;
    }
    
    public void setData(MujocoLib.mjData_* data)
    {
        mjData = data;
    }

    public void setData(DampsData* data)
    {
        dampsData = data;
    }

    public void resetJoint(UTHingeJoint joint)
    {
        switch (UTrainWindow.engineType)
        {
            case "PhysX":
                joint.reset();
                break;
            case "MuJoCo":
                //Debug.LogWarning("engineType not supported");
                break;
            default:
                //Debug.LogWarning("engineType not supported");
                break;
        }
    }
    
    public void setJointPos(UTHingeJoint joint, double val)
    {
        switch (UTrainWindow.engineType)
        {
            case "PhysX":
                joint.setPosition(val);
                break;
            case "MuJoCo":
                mjData->qpos[joint.QposAddress] = val;
                break;
            default:
                //Debug.LogWarning("engineType not supported");
                break;
        }
    }
    
    public void setJointVel(UTHingeJoint joint, float val)
    {
        switch (UTrainWindow.engineType)
        {
            case "PhysX":
                joint.setVelocity(val);
                break;
            case "MuJoCo":
                mjData->qvel[joint.DofAddress] = val;
                break;
            default:
                //Debug.LogWarning("engineType not supported");
                break;
        }
    }
    
    public void setJointAcc(UTHingeJoint joint, double val)
    {
        switch (UTrainWindow.engineType)
        {
            case "PhysX":
                break;
            case "MuJoCo":
                mjData->qacc[joint.DofAddress] = val;
                break;
            default:
                //Debug.LogWarning("engineType not supported");
                break;
        }
    }
}

