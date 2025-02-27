using System;
using System.Linq;
using System.Xml;
using Mujoco;
using UnityEngine;

public enum PhysicsEngineType { PhysX, MuJoCo, Others }

public interface IPhysicsEngine {
    void InitializeScene();
    void CreateConstraint(Transform transform, MjBaseJoint jointConfig);
    void CreateRigidbody(Transform transform, MjGeom geomConfig);
    void SyncState();
    void StepSimulation();
    void DestroyScene();
}

public abstract class PhysicsEngineBase : IPhysicsEngine {
    public abstract void InitializeScene();
    public abstract void CreateConstraint(Transform transform, MjBaseJoint jointConfig);
    public abstract void CreateRigidbody(Transform transform, MjGeom geomConfig);
    public abstract void SyncState();
    public abstract void StepSimulation();
    public abstract void DestroyScene();
    
    protected void HandleComponentTransition<T>(bool addComponent) where T : Component {
        var components = GameObject.FindObjectsOfType<T>();
        foreach (var comp in components) {
            if (addComponent && comp.gameObject.GetComponent<T>() == null) {
                comp.gameObject.AddComponent<T>();
            } else if (!addComponent) {
                GameObject.Destroy(comp);
            }
        }
    }
}