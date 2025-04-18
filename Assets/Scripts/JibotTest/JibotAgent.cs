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
    [Header("MuJoCo Settings")]
    [SerializeField]
    List<UTHingeJoint> utHinges;
    List<ConfigurableJoint> cjoints = new List<ConfigurableJoint>();

    [SerializeField] 
    private UTFreeJoint freeJoint;
    
    [SerializeField]
    public Transform effector;
    private EffectorSensor es;
    
    [SerializeField]
    public Transform target;
    private TargetSensor ts;
    
    [Header("Unity Settings")]
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
    
    private float lastTargetY;

    
    [Header("Reward")]
    private float episodeTimer;
    private int actionNum = 0;
    
    private float R = 0f;
    private float R_reach = 0f;
    private float R_grasp = 0f;
    private float R_lift = 0f;
    
    [SerializeField]
    private bool IsLogReward = false;
    
    [SerializeField]
    private int LogRewardInterval = 1000;
    
    [SerializeField]
    [Range(10f, 120f)]
    private float maxEpisodeTime = 25f;
    
    [SerializeField]
    [Range(0f, 10f)]
    private float f_reach = 2f;
    
    [SerializeField]
    [Range(0f, 10f)]
    private float f_near = 5f;
    
    [SerializeField]
    [Range(0f, 1.0f)]
    private float f_near_radius = 0.2f;
    
    [SerializeField]
    [Range(0f, 10f)]
    private float f_grasp_1 = 2f;
    
    [SerializeField]
    [Range(0f, 10f)]
    private float f_grasp_2 = 1f;
    
    [SerializeField]
    [Range(0f, 10f)]
    private float f_grasp_3 = 1f;
    
    [SerializeField]
    [Range(0f, 10f)]
    private float f_grasp_4 = 2f;
    
    [SerializeField]
    [Range(0f, 20f)]
    private float f_lift = 10f;
    
    private void Awake()
    {
        if (UTrainWindow.IsMuJoCo)
        {
            //this.GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize = 0;
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
        
        ts = target.GetComponent<TargetSensor>();
        es = effector.GetComponent<EffectorSensor>();
        lastTargetY = target.position.y;
    }

    public override void Initialize() {
        base.Initialize();
    }

    private void Update()
    {
        episodeTimer += Time.fixedDeltaTime;
        
        // Test
        if (Input.GetKeyDown(KeyCode.E))
        {
            EndEpisode();
        }
    }

    public unsafe override void OnEpisodeBegin() {
        
        base.OnEpisodeBegin();
        episodeTimer = 0f;
        
        UTData data = MjScene.Instance.getUTData();

        foreach (var uth in utHinges)
        {
            data.ResetHingeJoint(uth);
        }
        data.ResetFreeJoint(freeJoint);
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

        if (UTrainWindow.IsMuJoCo)
        {
            MjGeom[] geoms = FindObjectsOfType<MjGeom>();
            foreach (var geom in geoms)
            {
                Transform geomTransform = geom.transform;
                sensor.AddObservation(geomTransform.rotation);
            }
            
            sensor.AddObservation(ts.transform.position);
            sensor.AddObservation(ts.transform.rotation);

            UTData data = MjScene.Instance.getUTData();
            foreach (var hinge in utHinges)
            {
                sensor.AddObservation(data.getJointVel(hinge));
            }
            
        }
    }

    public override void OnActionReceived(ActionBuffers actions) {

        base.OnActionReceived(actions);
        actionNum++;
        
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
        float dis = Vector3.Distance(target.position, effector.position);
        
        R_reach = -f_reach * Mathf.Abs(dis - f_near_radius) + (ts.IsNear ? f_near : 0f);

        R_grasp = ts.IsNear ? (es.IsClamped ? f_grasp_1 : -f_grasp_2) : (es.IsClamped ? -f_grasp_3 : f_grasp_4);

        float lift = target.position.y - lastTargetY;
        R_lift = lift >= 0 ? (f_lift * (lift) * (ts.IsNear ? 1.0f : -0.25f)) : -0.5f;
        
        R = R_reach + R_grasp + R_lift;
        
        AddReward(R);
        
        if (actionNum >= LogRewardInterval && IsLogReward)
        {
            Debug.Log($"Reward: {R} = {R_reach} + {R_grasp} + {R_lift}" );
            actionNum = 0;
        }
        
        lastTargetY = target.position.y;
        
        if (dis > 6)
        {
            Debug.Log("End Episode: Out of Range");
            EndEpisode();
        }
        
        if (episodeTimer >= maxEpisodeTime)
        {
            //Debug.Log("End Episode: Out of Time");
            //EndEpisode();
        }
        

    }
}
