using UnityEngine;

public class QuadrilateralController : MonoBehaviour
{
    public Transform pointA, pointB, pointC, pointD;
    public float value;

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
}