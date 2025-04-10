using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Mujoco;
using Unity.MLAgents.Policies;

public class JibotAgent : UTAgent
{
    [SerializeField]
    List<UTHingeJoint> utHinges;
    List<HingeJoint> hingeJoints = new List<HingeJoint>();
    List<HingeJointController> hingeJointControllers = new List<HingeJointController>();
    
    [SerializeField]
    public GameObject effector;
    
    [SerializeField]
    public GameObject target;
    

    private void Awake()
    {
        if (UTrainWindow.IsMuJoCo)
        {
            this.GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize = 0;
        }

        foreach (var utHinge in utHinges)
        {
            HingeJoint hj = utHinge.Child.GetComponent<HingeJoint>();
            if (hj)
            {
                hingeJoints.Add(hj);
            }

            HingeJointController hjc = utHinge.Child.GetComponent<HingeJointController>();
            if (hjc)
            {
                hingeJointControllers.Add(hjc);
            }
        }
    }

    public unsafe override void OnEpisodeBegin() {
        base.OnEpisodeBegin();
        
        UTData data = MjScene.Instance.getUTData();

        foreach (var uth in utHinges)
        {
            data.resetJoint(uth);
        }
    }

    public override void CollectObservations(VectorSensor sensor) {
        base.CollectObservations(sensor);
        
        if (UTrainWindow.IsPhysX || UTrainWindow.IsDamps)
        {
            foreach (var hj in hingeJoints)
            {
                sensor.AddObservation(hj.angle);
                sensor.AddObservation(hj.velocity);
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions) {
        base.OnActionReceived(actions);
        
        // Action
        if (UTrainWindow.IsPhysX)
        {
            var vectorAction = actions.ContinuousActions;
            
            float angleRange = 120f;
            
            for (int i = 0; i < hingeJointControllers.Count; i++)
            {
                //hingeJointControllers[i].TargetAngle = 0f;
                //hingeJointControllers[i].TargetAngle = Mathf.Clamp(vectorAction[i], -1f, 1f) * angleRange;
            }
            
        }
        
        // Reward
        /*
        float dis = Vector3.Distance(goal.transform.position, effector.transform.position);
        if (dis < 0.5f)
        {
            SetReward(1.0f);
            Debug.Log("End Episode: Success");
            EndEpisode();
        }
        else if(dis > 3.0f)
        {
            SetReward(-0.2f);
            Debug.Log("End Episode: Out of Range");
            EndEpisode();
        }
        */
    }
}
