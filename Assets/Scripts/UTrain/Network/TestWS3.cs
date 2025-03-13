using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class TestWS3 : MonoBehaviour
{
    public List<GameObject> cubes = new List<GameObject>();
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
        
        // 添加Cube数量及参数
        initMsg.AppendFormat("|{0}", cubes.Count);
        foreach (GameObject cube in cubes)
        {
            Vector3 pos = cube.transform.position;
            Vector3 scale = cube.transform.localScale;
            string cubeParams = $"{pos.x},{pos.y},{pos.z}," +
                               $"{scale.x/2},{scale.y/2},{scale.z/2}," +
                               "0,0,0"; // 初始速度
            initMsg.AppendFormat("|{0}", cubeParams);
        }

        // 添加Hinge数量及参数
        initMsg.AppendFormat("|{0}", hinges.Count);
        foreach (UTHingeJoint hinge in hinges)
        {
            int parentIdx = cubes.IndexOf(hinge.Parent);
            int childIdx = cubes.IndexOf(hinge.Child);
            
            if(parentIdx == -1 || childIdx == -1){
                Debug.LogError("Hinge关节引用了不存在的Cube!");
                continue;
            }

            // 这里需要根据你的铰链参数实际情况调整
            string hingeParams = $"{parentIdx},{childIdx}," +
                                $"{hinge.LocalPointA.x},{hinge.LocalPointA.y},{hinge.LocalPointA.z}," +
                                $"{hinge.LocalPointB.x},{hinge.LocalPointB.y},{hinge.LocalPointB.z}," +
                                $"{hinge.Axis.x},{hinge.Axis.y},{hinge.Axis.z}";
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
            int expectedLength = 1 + cubes.Count * 7; // Header + n*(position+rotation)
            
            if (parts.Length >= expectedLength)
            {
                int index = 1;
                foreach (GameObject cube in cubes)
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
        //t.transform.position = pos;
        t.SetPositionAndRotation(pos, rot);
    }

}
