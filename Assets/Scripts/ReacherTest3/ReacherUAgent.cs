using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Mujoco;
public class ReacherUAgent : UAgent
{
    [SerializeField]
    MjHingeJoint hinge01;
    
    [SerializeField]
    MjHingeJoint hinge12;
    
    
    [SerializeField]
    MjHingeJoint hinge23;

    [SerializeField]
    MjHingeJoint hinge34;

    [SerializeField]
    MjHingeJoint hinge45;
    
    [SerializeField]
    public GameObject effector;
    
    [SerializeField]
    public GameObject goal;
    
    public unsafe override void OnEpisodeBegin() {
        base.OnEpisodeBegin();
        
        var data = MjScene.Instance.Data;
        
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

    public override void CollectObservations(VectorSensor sensor) {
        base.CollectObservations(sensor);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        // You could process your agent's actions directly here, and assign reward as well
        base.OnActionReceived(actions);
        
        // TODO
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
