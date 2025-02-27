using System.Xml;
using Mujoco;
using UnityEngine;

public class PhysXPhysicsEngine : PhysicsEngineBase {
    public override void CreateConstraint(Transform transform, MjBaseJoint jointConfig) {
        var joint = transform.gameObject.AddComponent<ConfigurableJoint>();
        ConfigureJoint(joint, jointConfig);
    }

    private void ConfigureJoint(ConfigurableJoint joint, MjBaseJoint config) {
        // 通用关节配置
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        
        // 根据具体类型细化配置
        if (config is MjHingeJoint hinge) {
            var limit = new SoftJointLimit();
            limit.limit = hinge.RangeUpper;
            joint.highAngularXLimit = limit;
            limit.limit = hinge.RangeLower;
            joint.lowAngularXLimit = limit;
        }
    }

    public override void CreateRigidbody(Transform transform, MjGeom geomConfig) {
        var rb = transform.gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.mass = geomConfig.Mass > 0 ? geomConfig.Mass : geomConfig.Density;
    }
    
    // 其他方法实现...
    public override void InitializeScene()
    {
        
    }

    public override void SyncState()
    {
      
    }

    public override void StepSimulation()
    {
  
    }

    public override void DestroyScene()
    {
   
    }
    
}