using UnityEngine;
using System.Collections;

public class TestWS1 : MonoBehaviour
{
    ServerSocket ssock;
    public GameObject obj;
    
    void Start()
    {
        ssock = new ServerSocket();
        ssock.Init();
        StartCoroutine(SendInitialData());
    }

    IEnumerator SendInitialData()
    {
        while (!ssock.IsClientConnected)
        {
            yield return null;
        }
        
        Vector3 pos = obj.transform.position;
        string initMsg = $"INIT,{pos.x},{pos.y},{pos.z},0,0,0";
        ssock.SendClient(initMsg);
    }

    void Update()
    {
        string msg = ssock.ReturnStr();
        if (!string.IsNullOrEmpty(msg))
        {
            if (msg.StartsWith("POS"))
            {
                string[] parts = msg.Split(',');
                if (parts.Length == 4)
                {
                    Vector3 newPos = new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])
                    );
                    obj.transform.position = newPos;
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        ssock.SocketQuit();
    }
}