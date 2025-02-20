using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ReacherGoal : MonoBehaviour
{
    public GameObject agent;
    public GameObject effector;
    public GameObject goalOn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == effector)
        {
            goalOn.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == effector)
        {
            goalOn.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == effector)
        {
            agent.GetComponent<ReacherRobot>().AddReward(0.01f);
        }
    }
}
