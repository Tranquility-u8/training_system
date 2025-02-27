using System.Xml;
using Mujoco;
using UnityEngine;

public class MujocoPhysicsEngine : PhysicsEngineBase {
    public override void InitializeScene() {
        
        if(MjScene.Instance == null)
            MjScene.Instance.CreateScene();
    }

    public override void CreateConstraint(Transform transform, MjBaseJoint jointConfig) {

        //jointConfig.GenerateMjcf(MjScene.Instance.GenerationContext, new XmlDocument());
    }

    public override void CreateRigidbody(Transform transform, MjGeom geomConfig)
    {
        throw new System.NotImplementedException();
    }

    public override void SyncState()
    {
     
    }

    public override void StepSimulation() {
        
        //MjScene.Instance.FixedUpdate();
    }

    public override void DestroyScene()
    {

    }
    
}