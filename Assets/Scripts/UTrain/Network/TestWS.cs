using UnityEngine;  

public class TestWS : MonoBehaviour   
{  
    ServerSocket ssock;  
    string str = null;  

    public GameObject obj;  
    float mspeed = 0.01f;  

    void Start()   
    {  
        ssock = new ServerSocket();  
        ssock.Init();  
    }  

    void Update()  
    {  
        // 检查是否有新消息  
        string currentCommand = ssock.ReturnStr();  

        // 处理新接收到的消息  
        if (currentCommand != null)  
        {  
            str = currentCommand;  
            if (str == "MXR")   
            {  
                obj.transform.Translate(Vector3.right * mspeed);  
                Debug.Log(str);  
            }  
            // 还可以在这里添加处理其他命令的逻辑  
        }  
    }  

    void OnApplicationQuit()  
    {  
        ssock.SocketQuit();  
    }  
}