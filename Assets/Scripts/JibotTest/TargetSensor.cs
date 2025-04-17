using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TargetSensor : MonoBehaviour
{

    public bool IsNear = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Effector"))
            IsNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Effector"))
            IsNear = false;
    }
}
