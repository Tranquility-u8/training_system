using System.Collections.Generic;
using Mujoco;
using Unity.VisualScripting;
using UnityEngine;


[ExecuteInEditMode]  
public class PhysicsManager : MonoBehaviour {
    
    public static bool isRunning = false;
    public static PhysicsManager instance;
    public static PhysicsEngineType CurrentEngine { get; private set; }
    public IPhysicsEngine ActiveEngine { get; private set; }
    
    [SerializeField, Header("Physics Engine Settings")]
    private string _engineType = "PhysX";
    private readonly string[] _engineOptions = { "PhysX", "MuJoCo", "Damps" };

    private Dictionary<string, IPhysicsEngine> _engines = new Dictionary<string, IPhysicsEngine>();

    void Awake() {
        Debug.Log("PhysicsManager Awake called");
        instance = this;
        InitializeEngines();
        SetEngine(_engineType);
    }

    private void InitializeEngines() {
        _engines.Add("PhysX", new PhysXPhysicsEngine());
        _engines.Add("MuJoCo", new MujocoPhysicsEngine());
    }

    public void SetEngine(string engineName) {
        if (_engines.TryGetValue(engineName, out var engine)) {
            ActiveEngine?.DestroyScene();
            ActiveEngine = engine;
            ActiveEngine.InitializeScene();
            UpdateSceneComponents();
        }
    }

    private void UpdateSceneComponents() {
        var isPhysX = ActiveEngine is PhysXPhysicsEngine;
        
        HandleRigidbodyComponents(isPhysX);
        HandleJointComponents(isPhysX);
    }

    void FixedUpdate() {
        if(!isRunning) return;
        ActiveEngine?.StepSimulation();
    }
    
    void HandleRigidbodyComponents(bool isPhysX) {
        var geoms = FindObjectsOfType<MjGeom>();
        foreach (var geom in geoms) {
            var rb = geom.GetComponent<Rigidbody>();
            if (isPhysX && !rb) {
                rb = geom.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
            } else if (!isPhysX && rb) {
                Destroy(rb);
            }
        }
    }

    void HandleJointComponents(bool isPhysX) {
        var joints = FindObjectsOfType<MjHingeJoint>();
        foreach (var joint in joints) {
            var cj = joint.Child.GetComponent<ConfigurableJoint>();
            if (isPhysX && !cj)
            {
                cj = joint.Child.AddComponent<ConfigurableJoint>();
                cj.xMotion = ConfigurableJointMotion.Locked;
                cj.yMotion = ConfigurableJointMotion.Locked;
                cj.zMotion = ConfigurableJointMotion.Locked;
                cj.angularXMotion = ConfigurableJointMotion.Locked;
                cj.angularZMotion = ConfigurableJointMotion.Locked;
            } else if (!isPhysX && cj) {
                Destroy(cj);
            }
        }
    }

}
