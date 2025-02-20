using Unity.MLAgents.Sensors;
using UnityEngine;

public class UJointLink : MonoBehaviour
{
    [System.Serializable]
    public struct ObservationSettings
    {
        public bool observePosition;
        public bool observeRotation;
        public bool observeVelocity;
        public bool observeAngularVelocity;
    }

    [System.Serializable]
    public struct ActionSettings
    {
        public bool applyActions;
        public Vector3 torqueAxis;
        public float maxTorque;
    }

    public ObservationSettings observationSettings;
    public ActionSettings actionSettings;

    [HideInInspector] public Rigidbody rb;
    private PhysicsEngineBase _physicsEngine;

    public void Initialize(PhysicsEngineBase physics)
    {
        _physicsEngine = physics;
        rb = GetComponent<Rigidbody>();
    }

    public void ResetState(Vector3 basePosition)
    {
        // Initialize with default positions
        transform.position = basePosition + Vector3.up * 0.55f;
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void AddObservations(VectorSensor sensor)
    {
        if (observationSettings.observePosition)
            sensor.AddObservation(transform.localPosition);

        if (observationSettings.observeRotation)
            sensor.AddObservation(transform.rotation);

        if (observationSettings.observeVelocity && rb != null)
            sensor.AddObservation(rb.velocity);

        if (observationSettings.observeAngularVelocity && rb != null)
            sensor.AddObservation(rb.angularVelocity);
    }

    public void ApplyAction(float actionValue)
    {
        if (actionSettings.applyActions && rb != null)
        {
            Vector3 torque = actionSettings.torqueAxis * (actionValue * actionSettings.maxTorque);

            _physicsEngine.ApplyTorque(rb, torque);
        }
    }
}
