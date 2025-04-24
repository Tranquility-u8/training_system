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
using UnityEngine.Serialization;

public class JibotAgent_x : UTAgent
{

    [SerializeField]
    private Transform effector;
    private EffectorSensor es;
    
    [SerializeField]
    private Transform target;
    private TargetSensor ts;
    
    [SerializeField]
    private GrabberController gc;
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
    }

    public override void CollectObservations(VectorSensor sensor) {
        base.CollectObservations(sensor);
        sensor.AddObservation(target.position);
        sensor.AddObservation(target.rotation);
        sensor.AddObservation(effector.position);
        sensor.AddObservation(effector.rotation);
        sensor.AddObservation(gc.Angle);
    }

    public override void OnActionReceived(ActionBuffers actions) {

        base.OnActionReceived(actions);
        actionNum++;
        
        // Action
        gc.UpdatePosition(1.0f, actions.ContinuousActions[0], actions.ContinuousActions[1], actions.ContinuousActions[2]);
        gc.UpdateRotation(1.0f, actions.ContinuousActions[3], actions.ContinuousActions[4], actions.ContinuousActions[5]);
        gc.UpdateAngle(1.0f, actions.ContinuousActions[6]);
        
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
            Debug.Log("End Episode: Out of Time");
            EndEpisode();
        }
        

    }
}
