using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Mujoco;
public class UTAgent : Agent
{
    
    // Since we are accessing memory shared with the MuJoCo simulation we have to do it in an "unsafe" context (You may need to enable this in Project Settings).
    public unsafe override void OnEpisodeBegin() {
        base.OnEpisodeBegin();

        // In case this is the first frame and the MuJoCo simulation didn't start yet, 
        // we know we will start in the correct state so we can skip it.
        if (UTrainWindow.IsMuJoCo && !(MjScene.InstanceExists && MjScene.Instance.Data != null)) return;
 
    }

    public override void Initialize() {
        base.Initialize();
    }
    
    public override void CollectObservations(VectorSensor sensor) {
        base.CollectObservations(sensor);
        // If you wanted to collect observations from the Agent class, you can add them one by one to the sensor
        // Note that if you do this, and not via separate SensorComponents, you will have to update the BehaviourParameter's observation size
    }

    public override void OnActionReceived(ActionBuffers actions) {
        // You could process your agent's actions directly here, and assign reward as well
        // base.OnActionReceived(actions);
    }
}
