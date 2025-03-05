using UnityEngine;
using System.Collections;
public class TestWS : MonoBehaviour {
    
    ServerSocket ssock;
    string str=null;
    GameObject obj;
    int rspeed = 100;
    float mspeed = 0.1f;
    
    void Start () {
        ssock = new ServerSocket();
        ssock.Init();
        obj = GameObject.Find("Cube");
    }
    
    void OnGUI()
    {
           
            //接收消息并处理
            if (ssock.ReturnStr() != null)
            {
                str = ssock.ReturnStr();
                if("RXR"==str )
                {
                    obj.transform.Rotate(Vector3.right * Time.deltaTime * rspeed);
                }
                if ("RXL" == str)
                {
                    obj.transform.Rotate(-Vector3.right * Time.deltaTime * rspeed);
 
                }
                if ("RYR" == str )
                {
                    obj.transform.Rotate(Vector3.up * Time.deltaTime * rspeed);
                }
                if ("RYL" == str)
                {
                    obj.transform.Rotate(-Vector3.up * Time.deltaTime * rspeed);
                }
                if ("RZR" == str )
                {
                    obj.transform.Rotate(Vector3.forward * Time.deltaTime * rspeed);
                }
                if ("RZL" == str)
                {
                    obj.transform.Rotate(-Vector3.forward * Time.deltaTime * rspeed);
                }
                if ("MXR" == str )
                {
                    obj.transform.Translate(Vector3.right * Time.deltaTime * mspeed);
                }
                if ("MXL" == str)
                {
                    obj.transform.Translate(-Vector3.right * Time.deltaTime * mspeed);
                }
                if ("MYR" == str )
                {
                    obj.transform.Translate(Vector3.up * Time.deltaTime * mspeed);
                }
                if ("MYL" == str)
                {
                    obj.transform.Translate(-Vector3.up * Time.deltaTime * mspeed);
                }
                if ("MZR" == str )
                {
                    obj.transform.Translate(Vector3.forward * Time.deltaTime * mspeed);
                }
                if ("MZL" == str)
                {
                    obj.transform.Translate(-Vector3.forward * Time.deltaTime * mspeed);
                } 
            }
            ssock.SendClient("hello1");
        //GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200), str);
    }
    
    void OnApplicationQuit()
    {
        ssock.SocketQuit();
    }
}