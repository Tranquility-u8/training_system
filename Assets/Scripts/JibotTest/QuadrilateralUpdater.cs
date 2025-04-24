using UnityEngine;
using UnityEngine.Serialization;

public class QuadrilateralUpdater : MonoBehaviour
{
    public Transform pointA, pointB, pointC, pointD;
    
    [SerializeField]
    private bool IsMirror = true;
    
    [SerializeField]
    private GrabberController grabberController;

    void Update()
    {
        Vector3 A = pointA.localPosition;
        Vector3 D = pointD.localPosition;

        Vector3 adVector = D - A;
        float L = adVector.magnitude;

        if (L == 0)
        {
            Debug.LogError("A == D");
            return;
        }

        Vector3 adDir = adVector.normalized;
        float theta = grabberController.Angle;
        if(IsMirror)
            theta = -theta;
        Quaternion rot = Quaternion.Euler(theta, 0, 0);
        Vector3 abDir = rot * adDir;
        Vector3 B = A + abDir * L;
        pointB.localPosition = B;

        pointC.localPosition = B + adVector;
    }
    
    /*
    void Update()
    {
        Vector3 A = pointA.position;
        Vector3 D = pointD.position;

        Vector3 adVector = D - A;
        float L = adVector.magnitude;

        if (L == 0)
        {
            Debug.LogError("A == D");
            return;
        }

        Vector3 adDir = adVector.normalized;
        float theta = value;
        Quaternion rot = Quaternion.Euler(theta, 0, 0);
        Vector3 abDir = rot * adDir;
        Vector3 B = A + abDir * L;
        pointB.position = B;

        pointC.position = B + adVector;
    }
    */
}