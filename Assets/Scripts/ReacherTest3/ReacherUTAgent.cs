using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Mujoco;
using Unity.MLAgents.Policies;

public class ReacherUTAgent : UTAgent
{
    [SerializeField]
    UTHingeJoint hinge01;
    
    [SerializeField]
    UTHingeJoint hinge12;
    
    [SerializeField]
    UTHingeJoint hinge23;

    [SerializeField]
    UTHingeJoint hinge34;

    [SerializeField]
    UTHingeJoint hinge45;
    
    [SerializeField]
    public GameObject effector;
    
    [SerializeField]
    public GameObject goal;

    [SerializeField]
    public GameObject j1;

    private void Awake()
    {
        if (UTrainWindow.IsMuJoCo)
        {
            this.GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize = 0;
        }
    }

    public unsafe override void OnEpisodeBegin() {
        base.OnEpisodeBegin();
        
        UTData data = MjScene.Instance.getUTData();

        data.setJointPos(hinge01, 0);
        data.setJointPos(hinge12, 0);
        data.setJointPos(hinge23, 0);
        data.setJointPos(hinge34, 0);
        data.setJointPos(hinge45, 0);
        
        data.setJointVel(hinge01, 0);
        data.setJointVel(hinge12, 0);
        data.setJointVel(hinge23, 0);
        data.setJointVel(hinge34, 0);
        data.setJointVel(hinge45, 0);
        
        data.setJointAcc(hinge01, 0);
        data.setJointAcc(hinge12, 0);
        data.setJointAcc(hinge23, 0);
        data.setJointAcc(hinge34, 0);
        data.setJointAcc(hinge45, 0);
        /*
        if (UTrainWindow.IsMuJoCo)
        {
            data->qpos[hinge01.QposAddress] = 0;
            data->qpos[hinge12.QposAddress] = 0;
            data->qpos[hinge23.QposAddress] = 0;
            data->qpos[hinge34.QposAddress] = 0;
            data->qpos[hinge45.QposAddress] = 0;

            data->qvel[hinge01.DofAddress] = 0;
            data->qvel[hinge12.DofAddress] = 0;
            data->qvel[hinge23.DofAddress] = 0;
            data->qvel[hinge34.DofAddress] = 0;
            data->qvel[hinge45.DofAddress] = 0;

            data->qacc[hinge01.DofAddress] = 0;
            data->qacc[hinge12.DofAddress] = 0;
            data->qacc[hinge23.DofAddress] = 0;
            data->qacc[hinge34.DofAddress] = 0;
            data->qacc[hinge45.DofAddress] = 0; 
        }
        */
        
        
    }

    public override void CollectObservations(VectorSensor sensor) {
        base.CollectObservations(sensor);
        
        // TEST
        if (UTrainWindow.IsPhysX)
        {
            sensor.AddObservation(hinge01.transform.localPosition);
            sensor.AddObservation(hinge01.transform.rotation);
        }
    }

    public override void OnActionReceived(ActionBuffers actions) {
        base.OnActionReceived(actions);
        
        // TEST
        if (UTrainWindow.IsPhysX)
        {
            var vectorAction = actions.ContinuousActions;
            var torque = Mathf.Clamp(vectorAction[0], -1f, 1f) * 150f;
        
            j1.GetComponent<Rigidbody>().AddTorque(new Vector3(0f, torque, 0f));
        }
        
        // TEST
        float dis = Vector3.Distance(goal.transform.position, effector.transform.position);
        if (dis < 0.5f)
        {
            SetReward(1.0f);
            //Debug.Log("End Episode: Success");
            EndEpisode();
        }
        else if(dis > 3.0f)
        {
            SetReward(-0.2f);
            //Debug.Log("End Episode: Out of Range");
            EndEpisode();
        }
    }
}
