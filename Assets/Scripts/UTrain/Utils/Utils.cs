using UnityEngine;

public static class VectorUtils
{
    public static Vector3 RoundTinyToZero(Vector3 v, float threshold = 1e-5f)
    {
        return new Vector3(
            Mathf.Abs(v.x) < threshold ? 0 : v.x,
            Mathf.Abs(v.y) < threshold ? 0 : v.y,
            Mathf.Abs(v.z) < threshold ? 0 : v.z
        );
    }
}