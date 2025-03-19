using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mujoco;
using UnityEngine.Serialization;

public class TestWS4 : MonoBehaviour
{
    public List<GameObject> links = new List<GameObject>();
    public List<UTHingeJoint> hinges = new List<UTHingeJoint>();
    
    ServerSocket ssock;

    void Start()
    {
        ssock = new ServerSocket();
        ssock.Init();
        StartCoroutine(InitSimulation());
    }

    IEnumerator InitSimulation()
    {
        yield return new WaitUntil(() => ssock.IsClientConnected);
        
        StringBuilder initMsg = new StringBuilder("INIT");
        
        // Cube
        initMsg.AppendFormat("|{0}", links.Count);
        foreach (GameObject link in links)
        {
            LocalCubeGizmo localCube = link.GetComponent<LocalCubeGizmo>();
            
            Quaternion rot = link.transform.rotation;
            Vector3 pos = link.transform.position + rot * localCube.centerOffset;
            Vector3 scale = localCube.scale;
            int isKinematic = link.GetComponent<MjGeom>().Settings.IsKinematic ? 1 : 0;
            
            string cubeParams = $"{pos.x},{pos.y},{-pos.z}," +
                               $"{scale.x/2},{scale.y/2},{scale.z/2}," +
                               $"{rot.x},{rot.y},{rot.z},{rot.w}," +
                               $"{isKinematic}," +
                               $"{localCube.testVel.x},{localCube.testVel.y},{-localCube.testVel.z}";

            initMsg.AppendFormat("|{0}", cubeParams);
        }

        // Hinge
        initMsg.AppendFormat("|{0}", hinges.Count);
        foreach (UTHingeJoint hinge in hinges)
        {
            int parentIdx = links.IndexOf(hinge.Parent);
            int childIdx = links.IndexOf(hinge.Child);
            
            if(parentIdx == -1 || childIdx == -1){
                Debug.LogError("Cube not existed!");
                continue;
            }
            
            string hingeParams = $"{parentIdx},{childIdx}," +
                                $"{hinge.LocalPointA.x},{hinge.LocalPointA.y},{-hinge.LocalPointA.z}," +
                                $"{hinge.LocalPointB.x},{hinge.LocalPointB.y},{-hinge.LocalPointB.z}," +
                                $"{hinge.Axis.x},{hinge.Axis.y},{-hinge.Axis.z}," +
                                $"{hinge.Axis.x},{hinge.Axis.y},{-hinge.Axis.z}";
            
            initMsg.AppendFormat("|{0}", hingeParams);
        }

        ssock.SendClient(initMsg.ToString());
    }

    void Update()
    {
        string msg = ssock.ReturnStr();
        if (!string.IsNullOrEmpty(msg) && msg.StartsWith("OBJ_UPDATE"))
        {
            string[] parts = msg.Split(',');
            int expectedLength = 1 + links.Count * 7; // Header + n*(position+rotation)
            
            if (parts.Length >= expectedLength)
            {
                int index = 1;
                foreach (GameObject cube in links)
                {
                    if (index + 6 >= parts.Length) break;
                    UpdateTransform(cube.transform, parts, ref index);
                }
            }
        }
    }

    void UpdateTransform(Transform t, string[] parts, ref int index)
    {
        Vector3 pos = new Vector3(
            float.Parse(parts[index++]),
            float.Parse(parts[index++]),
            float.Parse(parts[index++])
        );

        Quaternion rot = new Quaternion(
            float.Parse(parts[index++]),
            float.Parse(parts[index++]),
            float.Parse(parts[index++]),
            float.Parse(parts[index++])
        );
        t.rotation = rot;
        t.position = pos;
        //t.SetPositionAndRotation(pos, rot);
    }

}
