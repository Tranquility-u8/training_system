using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberController : MonoBehaviour
{
    [SerializeField] 
    [Range(45f, 120f)] 
    private float angle = 0;
    
    public float Angle
    {
        get => angle;
    }

    public void UpdatePosition(float factor, float dx, float dy, float dz)
    {
        Vector3 newPos = transform.position + factor * new Vector3(dx, dy, dz);
        transform.position = new Vector3(Mathf.Clamp(newPos.x, -2f, 4f), 
            Mathf.Clamp(newPos.x, 0.5f, 3f), 
            Mathf.Clamp(newPos.x, -4f, 2f));
    }

    public void UpdateRotation(float factor, float dx, float dy, float dz)
    {
        Vector3 newRot = transform.eulerAngles + factor * new Vector3(dx, dy, dz);
        transform.rotation = Quaternion.Euler(newRot);
    }
        
    public void UpdateAngle(float factor, float increment)
    {
        this.angle += factor * increment;
        this.angle = Mathf.Clamp(this.angle, 45f, 120f);
    }
    

    
}
