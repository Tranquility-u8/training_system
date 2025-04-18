using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSensor : MonoBehaviour
{
    private UTAgent agent;

    private void Awake()
    {
        agent = FindObjectOfType<UTAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Body"))
        {
            //Debug.Log("Penalty");
            agent.AddReward(-0.5f);
        }
    }
}
