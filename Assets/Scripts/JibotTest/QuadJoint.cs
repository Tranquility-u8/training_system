using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadJoint : MonoBehaviour
{
    [SerializeField]
    private Transform end1;
    
    [SerializeField]
    private Transform end2;
    
    [SerializeField]
    private Transform target1;
    
    [SerializeField]
    private Transform target2;
    
    
    void Update()
    {
        Transform parent = transform.parent;
        Vector3 t1Local = parent ? parent.InverseTransformPoint(target1.position) : target1.position;
        Vector3 t2Local = parent ? parent.InverseTransformPoint(target2.position) : target2.position;

        Vector3 midTarget = (t1Local + t2Local) * 0.5f;
        Vector3 dirTarget = t2Local - t1Local;
        
        Vector3 e1Local = end1.localPosition;
        Vector3 e2Local = end2.localPosition;
        Vector3 deltaLocal = e2Local - e1Local;
        
        float dy = deltaLocal.y;
        float dz = deltaLocal.z;
        float ty = dirTarget.y;
        float tz = dirTarget.z;
        
        if (Mathf.Approximately(dy*dy + dz*dz, 0f) || 
            Mathf.Approximately(ty*ty + tz*tz, 0f)) return;
        
        float currentAngle = Mathf.Atan2(dz, dy);
        float targetAngle = Mathf.Atan2(tz, ty);
        float angleDelta = targetAngle - currentAngle;
        float angleDegrees = angleDelta * Mathf.Rad2Deg;
        
        Quaternion newRotation = Quaternion.AngleAxis(angleDegrees, Vector3.right);
        
        Vector3 midLocal = (e1Local + e2Local) * 0.5f;
        Vector3 rotatedMidLocal = newRotation * midLocal;
        
        Vector3 requiredLocalPos = midTarget - rotatedMidLocal;
        
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            requiredLocalPos.y,
            requiredLocalPos.z
        );
        
        transform.localRotation = newRotation;
    }
}
