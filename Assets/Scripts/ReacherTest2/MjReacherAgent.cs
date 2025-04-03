using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Mujoco;
public class MjReacherAgent : Agent
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
    
    // Start is called before the first frame update
    void Start() {
    }

    //  Initialize() is called once when the agent is first enabled (after every other GameObject has called their own Start). 
    public override void Initialize() {
        base.Initialize();
    }

    // Since we are accessing memory shared with the MuJoCo simulation we have to do it in an "unsafe" context (You may need to enable this in Project Settings).
    public unsafe override void OnEpisodeBegin() {
        base.OnEpisodeBegin();

        // In case this is the first frame and the MuJoCo simulation didn't start yet, 
        // we know we will start in the correct state so we can skip it.
        if (!(MjScene.InstanceExists && MjScene.Instance.Data != null)) return;

        // Get the reference to the bindings of the mjData structure https://mujoco.readthedocs.io/en/latest/APIreference.html#mjdata
        var data = MjScene.Instance.Data;

        // reset kinematics to 0
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
        // If you wanted to collect observations from the Agent class, you can add them one by one to the sensor
        // Note that if you do this, and not via separate SensorComponents, you will have to update the BehaviourParameter's observation size
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
