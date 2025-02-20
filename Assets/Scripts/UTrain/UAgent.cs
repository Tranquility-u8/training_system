using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

[RequireComponent(typeof(DecisionRequester))]
public class UAgent : Agent
{
    [Header("Physics Settings")]
    public bool UseUnityPhysX = true;
    public bool IsHeuristic = false;

    [Header("Robot Components")]
    public List<UJointLink> joints = new List<UJointLink>();
    public GameObject effector;
    public GameObject goal;

    [Header("Goal Settings")]
    public float GoalHeight = 1.2f;
    
    private float goalRadius;
    private float goalDegree;
    private float goalOmega;
    private float goalDeviation;
    private float goalDeviationFreq;

    private PhysicsEngineBase _physicsEngine;

    public override void Initialize()
    {
        InitializePhysicsSystem();
        InitializeJoints();
        SetOrResetGoal();
    }

    void InitializePhysicsSystem()
    {
        if (UseUnityPhysX)
            _physicsEngine = new PhysXPE();
        else
        {
            _physicsEngine = new MuJoCoPE(); 
            Debug.LogWarning("Doesn't support MuJoCoPE");
        }
            
    }

    void InitializeJoints()
    {
        foreach (var joint in joints)
        {
            joint.Initialize(_physicsEngine);
        }
    }

    public void SetOrResetGoal()
    {
        // TODO
        goalRadius = Random.Range(1f, 1.3f);
        goalDegree = Random.Range(0f, 360f);
        goalOmega = Random.Range(-2f, 2f);
        goalDeviation = Random.Range(-1f, 1f);
        goalDeviationFreq = Random.Range(0f, 3.14f);
    }

    public override void OnEpisodeBegin()
    {
        ResetJoints();
        SetOrResetGoal();
    }

    void ResetJoints()
    {
        foreach (var joint in joints)
        {
            joint.ResetState(transform.position);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Joint observations
        foreach (var joint in joints)
        {
            joint.AddObservations(sensor);
        }
        
        // Goal observations
        sensor.AddObservation(goal.transform.localPosition);
        sensor.AddObservation(effector.transform.localPosition);
        sensor.AddObservation(goalOmega);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var continuousActions = actions.ContinuousActions;
        int actionIndex = 0;

        foreach (var joint in joints)
        {
            if (joint.actionSettings.applyActions)
            {
                float actionValue = Mathf.Clamp(continuousActions[actionIndex], -1f, 1f);
                joint.ApplyAction(actionValue);
                actionIndex++;
            }
        }
        
        UpdateGoalPosition();
        
        // TODO
        float dis = Vector3.Distance(goal.transform.position, effector.transform.position);
        if (dis < 0.5f)
        {
            //SetReward(0.1f);
        }
        else if(dis > 2.5f)
        {
            SetReward(-0.2f);
            Debug.Log("End Episode: Out of Range");
            EndEpisode();
        }
    }

    void UpdateGoalPosition()
    {
        if(IsHeuristic) return;
        
        goalDegree += goalOmega;
        var rad = goalDegree * Mathf.PI / 180f;
        var pos = new Vector3(
            goalRadius * Mathf.Cos(rad),
            GoalHeight + goalDeviation * Mathf.Cos(goalDeviationFreq * rad),
            goalRadius * Mathf.Sin(rad)
        );
        
        goal.transform.position = pos;
    }
}
