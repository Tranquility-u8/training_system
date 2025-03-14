using UnityEngine;

[ExecuteInEditMode]
public class LocalCubeGizmo : MonoBehaviour
{
    [Header("Cube Settings")]
    public Vector3 scale = Vector3.one;
    public Vector3 centerOffset = Vector3.zero;

    public Vector3 testVel = Vector3.zero;
    
    [Header("Gizmo Settings")]
    public Color gizmoColor = new Color(0, 1, 1, 0.5f);
    [Range(0, 1)] public float wireframeAlpha = 0.8f;

    void OnDrawGizmos()
    {
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Color originalColor = Gizmos.color;

        Gizmos.matrix = Matrix4x4.TRS(
            transform.position,
            transform.rotation,
            Vector3.one
        );

        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(centerOffset, scale);

        Gizmos.color = new Color(1, 1, 1, wireframeAlpha);
        Gizmos.DrawWireCube(centerOffset, scale);
        
        //DrawOffsetGuideLines();

        Gizmos.matrix = originalMatrix;
        Gizmos.color = originalColor;
    }
    
}