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
    private MjActuator[] actuators;
    
    private ControlledMjActuator
        controlledMjActuator; // Wrapper object that applies actions from the Agent to the MjActuator.

    public override IActuator[] CreateActuators() {
        controlledMjActuator = new ControlledMjActuator(actuators);
        
        return new[]
        {
            controlledMjActuator,
        }; // Could create and return multiple IActuators if needed
    }

    private class ControlledMjActuator : IActuator {
        private ActionSpec actionSpec;

        public ActionSpec ActionSpec {
            get => actionSpec;
        }

        private MjActuator[] wrappedActuators = new MjActuator[7];
        
        public string Name => wrappedActuators[0].name;


        // Used when no model is connected to the BrainParameters component.
        public void Heuristic(in ActionBuffers actionBuffersOut) {
        }


        // The agent distributes its actions among all of its IActuators, each receiving a segment based on their ActionSpecs.
        public void OnActionReceived(ActionBuffers actionBuffers) {
            wrappedActuators[0].Control = actionBuffers.ContinuousActions[0];
            wrappedActuators[1].Control = actionBuffers.ContinuousActions[1];
            wrappedActuators[2].Control = actionBuffers.ContinuousActions[2];
            wrappedActuators[3].Control = actionBuffers.ContinuousActions[3];
            wrappedActuators[4].Control = actionBuffers.ContinuousActions[4];
            wrappedActuators[5].Control = actionBuffers.ContinuousActions[5];
            wrappedActuators[6].Control = -actionBuffers.ContinuousActions[5];
        }


        // Called at the end of an episode.
        public void ResetData() {
        }


        // Limit the number of possible actions if taking discrete actions (e.g. prevent gridworld agent from walking into a wall), not applicable.
        public void WriteDiscreteActionMask(IDiscreteActionMask actionMask) {
        }


        public ControlledMjActuator(MjActuator[] actuators) {
            actionSpec = new ActionSpec(numContinuousActions: 7);
            wrappedActuators = actuators;
        }

    }

}
