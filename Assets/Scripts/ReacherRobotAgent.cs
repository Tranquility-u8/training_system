using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ReacherRobot : Agent
{
    public GameObject pendulumA;
    public GameObject pendulumB;
    public GameObject pendulumC;
    public GameObject pendulumD;
    public GameObject pendulumE;
    public GameObject pendulumF;

    public GameObject effector;
    public GameObject goal;
    
    Rigidbody m_RbA;
    Rigidbody m_RbB;
    Rigidbody m_RbC;
    Rigidbody m_RbD;
    Rigidbody m_RbE;
    Rigidbody m_RbF;

    public float m_GoalHeight = 1.2f;
    
    float m_GoalRadius;
    float m_GoalDegree;
    float m_GoalOmega;
    float m_GoalDeviation;
    float m_GoalDeviationFreq;
    
    public override void Initialize()
    {
        m_RbA = pendulumA.GetComponent<Rigidbody>();
        m_RbB = pendulumB.GetComponent<Rigidbody>();
        m_RbC = pendulumC.GetComponent<Rigidbody>();
        m_RbD = pendulumD.GetComponent<Rigidbody>();
        m_RbE = pendulumE.GetComponent<Rigidbody>();
        m_RbF = pendulumF.GetComponent<Rigidbody>();
        
        SetOrResetGoal();
    }

    public void SetOrResetGoal()
    {
        m_GoalRadius = Random.Range(1f, 1.3f);
        m_GoalDegree = Random.Range(0f, 360f);
        m_GoalOmega = Random.Range(-2f, 2f);
        m_GoalDeviation = Random.Range(-1f, 1f);
        m_GoalDeviationFreq = Random.Range(0f, 3.14f);
    }
    
    public override void OnEpisodeBegin()
    {
        pendulumA.transform.position = transform.position + new Vector3(0f, 0.55f, 0f);
        pendulumA.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        m_RbA.velocity = Vector3.zero;
        m_RbA.angularVelocity = Vector3.zero;
        
        pendulumB.transform.position = transform.position + new Vector3(-0.15f, 0.55f, 0f);
        pendulumB.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        m_RbB.velocity = Vector3.zero;
        m_RbB.angularVelocity = Vector3.zero;
        
        pendulumC.transform.position = transform.position + new Vector3(-0.15f, 1.375f, 0f);
        pendulumC.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        m_RbC.velocity = Vector3.zero;
        m_RbC.angularVelocity = Vector3.zero;
        
        pendulumD.transform.position = transform.position + new Vector3(-0.15f, 1.375f, 0f);
        pendulumD.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        m_RbD.velocity = Vector3.zero;
        m_RbD.angularVelocity = Vector3.zero;
        
        pendulumE.transform.position = transform.position + new Vector3(-0.15f, 2f, 0f);
        pendulumE.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        m_RbE.velocity = Vector3.zero;
        m_RbE.angularVelocity = Vector3.zero;
        
        pendulumF.transform.position = transform.position + new Vector3(-0.15f, 2.11f, 0f);
        pendulumF.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        m_RbF.velocity = Vector3.zero;
        m_RbF.angularVelocity = Vector3.zero;
        
        SetOrResetGoal();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(pendulumA.transform.localPosition);
        sensor.AddObservation(pendulumA.transform.rotation);
        sensor.AddObservation(m_RbA.angularVelocity);
        sensor.AddObservation(m_RbA.velocity);
        
        sensor.AddObservation(pendulumB.transform.localPosition);
        sensor.AddObservation(pendulumB.transform.rotation);
        sensor.AddObservation(m_RbB.angularVelocity);
        sensor.AddObservation(m_RbB.velocity);
        
        sensor.AddObservation(pendulumC.transform.localPosition);
        sensor.AddObservation(pendulumC.transform.rotation);
        sensor.AddObservation(m_RbC.angularVelocity);
        sensor.AddObservation(m_RbC.velocity);
        
        sensor.AddObservation(pendulumD.transform.localPosition);
        sensor.AddObservation(pendulumD.transform.rotation);
        sensor.AddObservation(m_RbD.angularVelocity);
        sensor.AddObservation(m_RbD.velocity);
        
        sensor.AddObservation(pendulumE.transform.localPosition);
        sensor.AddObservation(pendulumE.transform.rotation);
        sensor.AddObservation(m_RbE.angularVelocity);
        sensor.AddObservation(m_RbE.velocity);
        
        sensor.AddObservation(pendulumF.transform.localPosition);
        sensor.AddObservation(pendulumF.transform.rotation);
        sensor.AddObservation(m_RbF.angularVelocity);
        sensor.AddObservation(m_RbF.velocity);
        
        sensor.AddObservation(goal.transform.localPosition);
        sensor.AddObservation(effector.transform.localPosition);
        
        sensor.AddObservation(m_GoalOmega);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var vectorAction = actions.ContinuousActions;
        
        var torque = Mathf.Clamp(vectorAction[0], -1f, 1f) * 150f;
        m_RbA.AddTorque(new Vector3(0f, torque, 0f));
        
        torque = Mathf.Clamp(vectorAction[1], -1f, 1f) * 150f;
        m_RbB.AddTorque(new Vector3(0f, torque, 0f));
        
        torque = Mathf.Clamp(vectorAction[2], -1f, 1f) * 150f;
        m_RbC.AddTorque(new Vector3(0f, torque, 0f));
        
        torque = Mathf.Clamp(vectorAction[3], -1f, 1f) * 150f;
        m_RbD.AddTorque(new Vector3(0f, torque, 0f));
        
        torque = Mathf.Clamp(vectorAction[4], -1f, 1f) * 150f;
        m_RbE.AddTorque(new Vector3(0f, torque, 0f));
        
        torque = Mathf.Clamp(vectorAction[5], -1f, 1f) * 150f;
        m_RbF.AddTorque(new Vector3(0f, torque, 0f));
        
        updateGoalPosition();
    }
    
    void updateGoalPosition()
    {
        m_GoalDegree += m_GoalOmega;
        
        var m_GoalDegree_rad = m_GoalDegree * Mathf.PI / 180f;
        var m_GoalX = m_GoalRadius * Mathf.Cos(m_GoalDegree_rad);
        var m_GoalZ = m_GoalRadius * Mathf.Sin(m_GoalDegree_rad);
        var m_GoalY = m_GoalHeight + m_GoalDeviation * Mathf.Cos(m_GoalDeviationFreq * m_GoalDegree_rad);
        
        goal.transform.position = new Vector3(m_GoalX, m_GoalY, m_GoalZ);
    }
}
