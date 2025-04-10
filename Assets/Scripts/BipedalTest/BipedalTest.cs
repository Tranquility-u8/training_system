using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Mujoco;
using Unity.MLAgents.Policies;
using UnityEngine.Serialization;

public class BipedalAgent : UTAgent
{
    [SerializeField]
    List<UTHingeJoint> utHinges;
    List<HingeJoint> hingeJoints = new List<HingeJoint>();
    List<HingeJointController> hingeJointControllers = new List<HingeJointController>();
    
    [SerializeField]
    public GameObject body;

    [Header("Reward Parameters")]
    [SerializeField] private float targetHeight = 1.5f;   
    [SerializeField] private float maxTiltAngle = 30f;    
    [SerializeField] private float forwardSpeedWeight = 1.0f;
    [SerializeField] private float uprightWeight = 0.5f;
    [SerializeField] private float energyEfficiencyWeight = 0.1f;
    [SerializeField] private float gaitSymmetryWeight = 0.3f;
    
    private Vector3 lastBodyPosition;
    private float episodeStartTime;
    
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

    private void Update()
    {
        //Test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UTData data = MjScene.Instance.getUTData();
            foreach (var uth in utHinges)
            {
                data.resetJoint(uth);
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
        
        lastBodyPosition = body.transform.position;
        episodeStartTime = Time.time;
    }

    public override void CollectObservations(VectorSensor sensor) {
        base.CollectObservations(sensor);
        
        if (UTrainWindow.IsPhysX || UTrainWindow.IsDamps)
        {
            sensor.AddObservation(body.transform.up);        
            sensor.AddObservation(body.transform.forward);
            
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
            /*
            float factor = 100f;
            var torque0 = Mathf.Clamp(vectorAction[0], -1f, 1f) * factor;
            var torque1 = Mathf.Clamp(vectorAction[1], -1f, 1f) * factor;
            var torque2 = Mathf.Clamp(vectorAction[2], -1f, 1f) * factor;
            var torque3 = Mathf.Clamp(vectorAction[3], -1f, 1f) * factor;
            var torque4 = Mathf.Clamp(vectorAction[4], -1f, 1f) * factor;
            var torque5 = Mathf.Clamp(vectorAction[5], -1f, 1f) * factor;
            
            j1.GetComponent<Rigidbody>().AddTorque(new Vector3(torque0, 0f, 0f));
            j2.GetComponent<Rigidbody>().AddTorque(new Vector3(torque1, 0f, 0f));
            j3.GetComponent<Rigidbody>().AddTorque(new Vector3(torque2, 0f, 0f));
            j4.GetComponent<Rigidbody>().AddTorque(new Vector3(torque3, 0f, 0f));
            j5.GetComponent<Rigidbody>().AddTorque(new Vector3(torque4, 0f, 0f));
            j6.GetComponent<Rigidbody>().AddTorque(new Vector3(torque5, 0f, 0f));
            */
            float angleRange = 120f;

            for (int i = 0; i < hingeJointControllers.Count; i++)
            {
                hingeJointControllers[i].TargetAngle = Mathf.Clamp(vectorAction[i], -1f, 1f) * angleRange;
            }
            
        }
        
        // Reward
        float totalReward = 0f;
        
        // 1. 前进速度奖励
        Vector3 bodyVelocity = (body.transform.position - lastBodyPosition) / Time.fixedDeltaTime;
        float forwardSpeed = Vector3.Dot(bodyVelocity, new Vector3(0, 0, -1));
        totalReward += forwardSpeed * forwardSpeedWeight;

        // 2. 直立稳定性奖励
        float heightReward = Mathf.Exp(-Mathf.Abs(body.transform.position.y - targetHeight));
        float tiltAngle = Vector3.Angle(body.transform.up, Vector3.up);
        float tiltReward = Mathf.Exp(-Mathf.Abs(tiltAngle) / maxTiltAngle);
        totalReward += (heightReward + tiltReward) * uprightWeight;

        // 3. 能量效率
        float energyCost = 0f;
        foreach (var hinge in utHinges)
        {
            HingeJoint hj = hinge.Child.GetComponent<HingeJoint>();
            energyCost += Mathf.Abs(hj.velocity) * Time.fixedDeltaTime;
        }
        totalReward -= energyCost * energyEfficiencyWeight;

        // 4. 步态对称性奖励
        float gaitReward = CalculateGaitSymmetry();
        totalReward += gaitReward * gaitSymmetryWeight;
        
        // 5. 存活奖励
        totalReward += 0.1f * (Time.time - episodeStartTime);

        
        bool isFallen = body.transform.position.y < targetHeight;
        if (isFallen)
        {
            AddReward(-1f);
            EndEpisode();
            Debug.Log("End Episode");
        }
        
        AddReward(totalReward);
        lastBodyPosition = body.transform.position;
    }
    
    private float CalculateGaitSymmetry(){
        if (UTrainWindow.IsPhysX)
        {
            float phaseDifference = 0f;
            for(int i = 0; i < hingeJoints.Count - 3; i++){
                float leftPhase = Mathf.Sin(hingeJoints[i].angle * Mathf.Deg2Rad);
                float rightPhase = Mathf.Sin(hingeJoints[i + 3].angle * Mathf.Deg2Rad);
                phaseDifference += Mathf.Abs(leftPhase - rightPhase);
            }
            return Mathf.Exp(-phaseDifference); 
        }
        return 0f;
    }
}
