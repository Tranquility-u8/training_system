using System;
using System.Collections;
using System.Collections.Generic;
using Cartpole;
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
    List<ConfigurableJoint> cjoints = new List<ConfigurableJoint>();
    
    [SerializeField]
    UTJointController jointController;
    
    [SerializeField]
    Rigidbody rb1;
    [SerializeField]
    Rigidbody rb2;
    [SerializeField]
    Rigidbody rb3;
    [SerializeField]
    Rigidbody rb4;
    [SerializeField]
    Rigidbody rb5;
    [SerializeField]
    Rigidbody rb6;
    [SerializeField]
    Rigidbody rb7;
    
    [SerializeField]
    public GameObject effector;
    
    [SerializeField]
    public GameObject target;
    
    public CartpoleActuator actuator;

    private void Awake()
    {
        if (UTrainWindow.IsMuJoCo)
        {
            this.GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize = 0;
        }

        foreach (var utHinge in utHinges)
        {
            ConfigurableJoint cj = utHinge.Child.GetComponent<ConfigurableJoint>();
            if (cj)
            {
                cjoints.Add(cj);
            }
            
            //jointController.SetUpJoint(cj);
        }
        

    }

    public override void Initialize() {
        base.Initialize();
    }
    
    public unsafe override void OnEpisodeBegin() {
        base.OnEpisodeBegin();
        
        UTData data = MjScene.Instance.getUTData();

        foreach (var uth in utHinges)
        {
            data.ResetJoint(uth);
        }

        //Debug.Log("Reset");
    }

    public override void CollectObservations(VectorSensor sensor) {
        base.CollectObservations(sensor);
        
        if (UTrainWindow.IsPhysX || UTrainWindow.IsDamps)
        {
            foreach (var cj in cjoints)
            {
                sensor.AddObservation(cj.targetAngularVelocity);
                sensor.AddObservation(cj.targetVelocity);
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions) {
        base.OnActionReceived(actions);

        // Action
        if (UTrainWindow.IsPhysX)
        {
            var continuousActions = actions.ContinuousActions;

            /*
            float factor = 15000f;
            int i = 0;
            rb1.AddTorque(new Vector3(Mathf.Clamp(continuousActions[0], -1f, 1f), 0, 0) * factor);
            rb2.AddTorque(new Vector3(0, Mathf.Clamp(continuousActions[1], -1f, 1f), 0) * factor);
            rb3.AddTorque(new Vector3(0, Mathf.Clamp(continuousActions[2], -1f, 1f), 0) * factor);
            rb4.AddTorque(new Vector3(0, Mathf.Clamp(continuousActions[3], -1f, 1f), 0) * factor);
            rb5.AddTorque(new Vector3(Mathf.Clamp(continuousActions[4], -1f, 1f), 0) * factor);
            rb6.AddTorque(new Vector3(0, 0, Mathf.Clamp(continuousActions[5], -1f, 1f)) * factor);
            rb7.AddTorque(new Vector3(0, 0, Mathf.Clamp(-continuousActions[5], -1f, 1f)) * factor);
            */
        }
        
        // Reward
    }
}
