using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Mujoco;
using System.Linq;

public class JibotObservations : SensorComponent {

  public override ISensor[] CreateSensors() {
    jibotSensor = new JibotSensor(transforms);

    return new ISensor[] { jibotSensor };
  }
  
  private JibotSensor jibotSensor;

  public List<float> Observations =>
      jibotSensor.Observations; // Exposed in case needed for visualisation

  [SerializeField]
  Transform[] transforms;
  
  private class JibotSensor : ISensor {

    Transform[] transforms;
    
    public List<float> Observations {
      //get => Positions.Concat(Velocities).ToList();
      get => null;
    } //Combine positions and velocities

    public int Write(ObservationWriter writer) {
      writer.AddList(Observations);

      return 3; // Return number of written observations (2 for hinge pos, 1 for hinge vel)
    }

    // Only needed for visual observations (e.g. camera feed)
    public byte[] GetCompressedObservation() {
      throw new NotImplementedException();
    }

    public CompressionSpec GetCompressionSpec() {
      return CompressionSpec.Default();
    }

    public string GetName() {
      return "JibotSensor";
    }

    ObservationSpec observationSpec;

    public JibotSensor(Transform[] _transforms) {

      transforms = _transforms;
      observationSpec = new ObservationSpec(shape: new InplaceArray<int>(3),
          dimensionProperties: new InplaceArray<DimensionProperty>(DimensionProperty.None));
    }

    public ObservationSpec GetObservationSpec() {
      return observationSpec;
    }

    public void Reset() {
    }

    public void Update() {
    }


  }

}