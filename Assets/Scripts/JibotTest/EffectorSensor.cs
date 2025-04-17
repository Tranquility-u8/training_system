using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EffectorSensor : MonoBehaviour
{
    
    public bool IsClamped = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Claw"))
            IsClamped = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Claw"))
            IsClamped = false;
    }
    
}