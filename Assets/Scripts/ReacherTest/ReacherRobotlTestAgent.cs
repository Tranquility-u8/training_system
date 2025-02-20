using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReacherGoalTest : MonoBehaviour
{
    public GameObject goal;
    
    public float m_GoalHeight = 1.2f;
    
    float m_GoalRadius;
    float m_GoalDegree;
    float m_GoalOmega;
    float m_GoalDeviation;
    float m_GoalDeviationFreq;
    
    void Start()
    {
        SetOrResetParams();
    }
    
    void Update()
    {
        m_GoalDegree += m_GoalOmega;
        updateGoalPosition();
    }

    public void SetOrResetParams()
    {
        m_GoalRadius = Random.Range(1f, 1.3f);
        m_GoalDegree = Random.Range(0f, 360f);
        m_GoalOmega = Random.Range(-2f, 2f);
        m_GoalDeviation = Random.Range(-1f, 1f);
        m_GoalDeviationFreq = Random.Range(0f, 3.14f);
    }

    void updateGoalPosition()
    {
        var m_GoalDegree_rad = m_GoalDegree * Mathf.PI / 180f;
        var m_GoalX = m_GoalRadius * Mathf.Cos(m_GoalDegree_rad);
        var m_GoalZ = m_GoalRadius * Mathf.Sin(m_GoalDegree_rad);
        var m_GoalY = m_GoalHeight + m_GoalDeviation * Mathf.Cos(m_GoalDeviationFreq * m_GoalDegree_rad);
        
        goal.transform.position = new Vector3(m_GoalX, m_GoalY, m_GoalZ);
    }
}
