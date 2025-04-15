using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Mujoco;

public class JibotActuator : ActuatorComponent {
    // Reference value for the overall ActionSpec of the combined IActuators associated with the component
    public override ActionSpec ActionSpec => new ActionSpec(numContinuousActions: 7);

    // We need to add the actuator from the Editor
    [SerializeField]
    private MjActuator actuator0;
    
    [SerializeField]
    private MjActuator actuator1;
    
    [SerializeField]
    private MjActuator actuator2;
    
    [SerializeField]
    private MjActuator actuator3;
    
    [SerializeField]
    private MjActuator actuator4;
    
    [SerializeField]
    private MjActuator actuator5;
    
    [SerializeField]
    private MjActuator actuator6;
    
    private ControlledMjActuator[]
        controlledMjActuators = new ControlledMjActuator[7]; // Wrapper object that applies actions from the Agent to the MjActuator.

    public override IActuator[] CreateActuators() {
        controlledMjActuators[0] = new ControlledMjActuator(actuator0, 0);
        controlledMjActuators[1] = new ControlledMjActuator(actuator1, 1);
        controlledMjActuators[2] = new ControlledMjActuator(actuator2, 2);
        controlledMjActuators[3] = new ControlledMjActuator(actuator3, 3);
        controlledMjActuators[4] = new ControlledMjActuator(actuator4, 4);
        controlledMjActuators[5] = new ControlledMjActuator(actuator5, 5);
        controlledMjActuators[6] = new ControlledMjActuator(actuator6, 6);
        
        return new[]
        {
            controlledMjActuators[0],
            controlledMjActuators[1],
            controlledMjActuators[2],
            controlledMjActuators[3],
            controlledMjActuators[4],
            controlledMjActuators[5],
            controlledMjActuators[6]
        }; // Could create and return multiple IActuators if needed
    }

    private class ControlledMjActuator : IActuator {
        private ActionSpec actionSpec;

        public ActionSpec ActionSpec {
            get => actionSpec;
        }

        private MjActuator wrappedActuator;

        public int id;
        public string Name => wrappedActuator.name;


        // Used when no model is connected to the BrainParameters component.
        public void Heuristic(in ActionBuffers actionBuffersOut) {
        }


        // The agent distributes its actions among all of its IActuators, each receiving a segment based on their ActionSpecs.
        public void OnActionReceived(ActionBuffers actionBuffers) {
            wrappedActuator.Control = actionBuffers.ContinuousActions[id];
        }


        // Called at the end of an episode.
        public void ResetData() {
        }


        // Limit the number of possible actions if taking discrete actions (e.g. prevent gridworld agent from walking into a wall), not applicable.
        public void WriteDiscreteActionMask(IDiscreteActionMask actionMask) {
        }


        public ControlledMjActuator(MjActuator actuator, int num) {
            actionSpec = new ActionSpec(numContinuousActions: 7);
            wrappedActuator = actuator;
            id = num;
        }

    }

}
