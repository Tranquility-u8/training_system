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
        // 获取父物体坐标系下的目标点位置
        Transform parent = transform.parent;
        Vector3 t1Local = parent ? parent.InverseTransformPoint(target1.position) : target1.position;
        Vector3 t2Local = parent ? parent.InverseTransformPoint(target2.position) : target2.position;

        // 计算目标线段参数（在父物体坐标系）
        Vector3 midTarget = (t1Local + t2Local) * 0.5f;
        Vector3 dirTarget = t2Local - t1Local;

        // 获取本地线段参数
        Vector3 e1Local = end1.localPosition;
        Vector3 e2Local = end2.localPosition;
        Vector3 deltaLocal = e2Local - e1Local;

        // 仅使用YZ平面分量
        float dy = deltaLocal.y;
        float dz = deltaLocal.z;
        float ty = dirTarget.y;
        float tz = dirTarget.z;

        // 处理零向量情况
        if (Mathf.Approximately(dy*dy + dz*dz, 0f) || 
            Mathf.Approximately(ty*ty + tz*tz, 0f)) return;

        // 计算旋转角度（YZ平面）
        float currentAngle = Mathf.Atan2(dz, dy);
        float targetAngle = Mathf.Atan2(tz, ty);
        float angleDelta = targetAngle - currentAngle;
        float angleDegrees = angleDelta * Mathf.Rad2Deg;

        // 创建绕X轴的本地旋转
        Quaternion newRotation = Quaternion.AngleAxis(angleDegrees, Vector3.right);

        // 计算旋转后的本地中点偏移
        Vector3 midLocal = (e1Local + e2Local) * 0.5f;
        Vector3 rotatedMidLocal = newRotation * midLocal;

        // 计算需要的本地位置调整
        Vector3 requiredLocalPos = midTarget - rotatedMidLocal;

        // 应用本地变换（保持x轴不变）
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            requiredLocalPos.y,
            requiredLocalPos.z
        );

        // 应用本地旋转
        transform.localRotation = newRotation;
    }
}
