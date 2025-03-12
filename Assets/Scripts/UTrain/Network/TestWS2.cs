// Unity端的TestWS.cs修改版
using UnityEngine;
using System.Collections;

public class TestWS2 : MonoBehaviour
{
    public GameObject cube1;
    public GameObject cube2;
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

        // 构造初始化参数：位置、尺寸、速度 | 铰链参数
        Vector3 pos1 = cube1.transform.position;
        Vector3 scale1 = cube1.transform.localScale;
        string cube1Params = $"{pos1.x},{pos1.y},{pos1.z},{scale1.x/2},{scale1.y/2},{scale1.z/2},0,0,0.1";

        Vector3 pos2 = cube2.transform.position;
        Vector3 scale2 = cube2.transform.localScale;
        string cube2Params = $"{pos2.x},{pos2.y},{pos2.z},{scale2.x/2},{scale2.y/2},{scale2.z/2},0,0,0";

        // 铰链参数（根据实际连接点设置）
        string hingeParams = "-0.6,-0.2,0,0,0.5,0,0,1,0,0,1,0";

        string initMsg = $"INIT|{cube1Params}|{cube2Params}|{hingeParams}";
        ssock.SendClient(initMsg);
    }

    void Update()
    {
        string msg = ssock.ReturnStr();
        if (!string.IsNullOrEmpty(msg))
        {
            if (msg.StartsWith("OBJ_UPDATE"))
            {
                string[] parts = msg.Split(',');
                if (parts.Length >= 15) // 两个物体的数据
                {
                    // 更新Cube1
                    int index = 1;
                    UpdateTransform(cube1.transform, parts, ref index);
                    
                    // 更新Cube2
                    UpdateTransform(cube2.transform, parts, ref index);
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

    void OnApplicationQuit()
    {
        ssock.SocketQuit();
    }
}
