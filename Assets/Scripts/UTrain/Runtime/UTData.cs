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

    public void ResetHingeJoint(UTHingeJoint joint)
    {
        switch (UTrainWindow.engineType)
        {
            case "PhysX":
                joint.reset();
                break;
            case "MuJoCo":
                setJointPos(joint, 0);
                setJointVel(joint, 0);
                setJointAcc(joint, 0);
                break;
            default:
                Debug.LogWarning("engineType not supported");
                break;
        }
    }

    public void ResetFreeJoint(MjFreeJoint joint)
    {
        switch (UTrainWindow.engineType)
        {
            case "PhysX":
                Debug.LogWarning("engineType not supported");
                break;
            case "MuJoCo":
                mjData->qpos[joint.QposAddress] = 0f;
                mjData->qvel[joint.DofAddress] = 0f;
                mjData->qacc[joint.DofAddress] = 0f;
                break;
            default:
                Debug.LogWarning("engineType not supported");
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

    public float getJointVel(UTHingeJoint joint)
    {
        switch (UTrainWindow.engineType)
        {
            case "PhysX":
                return joint.Child.GetComponent<Rigidbody>().velocity.z;
            case "MuJoCo":
                return (float)mjData->qvel[joint.DofAddress];
                break;
            default:
                Debug.LogWarning("engineType not supported");
                break;
        }
        return 0.0f;
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
                Debug.LogWarning("engineType not supported");
                break;
        }
    }
}

