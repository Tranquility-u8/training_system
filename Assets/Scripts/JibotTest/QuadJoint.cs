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
        // 获取目标线段信息
        Vector3 target1Pos = target1.position;
        Vector3 target2Pos = target2.position;
        Vector3 midTarget = (target1Pos + target2Pos) * 0.5f;
        Vector3 dirTarget = target2Pos - target1Pos;

        // 获取本地线段信息
        Vector3 e1Local = end1.localPosition;
        Vector3 e2Local = end2.localPosition;
        Vector3 deltaLocal = e2Local - e1Local;

        float dy = deltaLocal.y;
        float dz = deltaLocal.z;
        float ty = dirTarget.y;
        float tz = dirTarget.z;

        // 处理零向量情况
        if (Mathf.Approximately(dy * dy + dz * dz, 0f) || 
            Mathf.Approximately(ty * ty + tz * tz, 0f)) return;

        // 计算需要的旋转角度
        float beta = Mathf.Atan2(dz, dy);
        float alpha = Mathf.Atan2(tz, ty);
        float theta = alpha - beta;
        float thetaDegrees = theta * Mathf.Rad2Deg;

        // 创建新的旋转（绕X轴）
        Quaternion newRotation = Quaternion.AngleAxis(thetaDegrees, Vector3.right);

        // 计算本地中点并旋转
        Vector3 midLocal = (e1Local + e2Local) * 0.5f;
        Vector3 rotatedMidLocal = newRotation * midLocal;

        // 计算需要的新位置
        Vector3 desiredWorldPosition = midTarget - rotatedMidLocal;

        // 转换为本地坐标并保持X轴不变
        if (transform.parent != null)
        {
            Vector3 newLocalPosition = transform.parent.InverseTransformPoint(desiredWorldPosition);
            transform.localPosition = new Vector3(
                transform.localPosition.x, 
                newLocalPosition.y, 
                newLocalPosition.z
            );
        }
        else
        {
            transform.position = new Vector3(
                transform.position.x,
                desiredWorldPosition.y,
                desiredWorldPosition.z
            );
        }

        // 应用新的旋转
        transform.localRotation = newRotation;
    }
}
