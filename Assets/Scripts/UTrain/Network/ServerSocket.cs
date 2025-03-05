using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;

public class ServerSocket
{
    Socket serverSocket;
    Socket clientSocket;
    Thread thread;
    IPEndPoint clientip;
    
    string returnStr;
    string receiveStr;
    string sendStr;
 
    public bool IsClientConnected 
    {
        get 
        { 
            return clientSocket != null && 
                   clientSocket.Connected;
        }
    }

    int recv;
    byte[] receiveData = new byte[1024];
    byte[] sendData = new byte[1024];
    
    public void Init()
    {
        returnStr = null;
        receiveStr = null;
        
        string hostName = System.Net.Dns.GetHostName();
        System.Net.IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(hostName);        
        System.Net.IPAddress[] addr = ipEntry.AddressList;
        
        IPEndPoint ipep = new IPEndPoint(addr[0], 8080);
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(ipep);
        serverSocket.Listen(10);
        
        thread = new Thread(new ThreadStart(GoClient));

        thread.Start();
        
        // 在ServerSocket.Init()中添加
        Debug.Log($"服务器实际绑定地址: {ipep}");
        Debug.Log($"Socket地址族: {serverSocket.AddressFamily}");
        Debug.Log($"协议类型: {serverSocket.ProtocolType}");
        Debug.Log($"Socket是否阻塞: {serverSocket.Blocking}");

    }
 
    void GoClient()
    {
        ConnetClient();
        while (true)
        {
            receiveData = new byte[1024];
            recv = clientSocket.Receive(receiveData);
            if (recv == 0)
            {
                ConnetClient();
                continue;
            }
            receiveStr = Encoding.ASCII.GetString(receiveData, 0, recv);
        }
    }
    
    void ConnetClient()
    {
        if (clientSocket != null)
        {
            clientSocket.Close();
        }
        clientSocket = serverSocket.Accept();
    }
    
// 修改后的ServerSocket.SendClient方法
    public void SendClient(string str) 
    {
        try 
        {
            // 添加三重验证
            if (clientSocket == null || 
                !clientSocket.Connected) 
            {
                Debug.LogWarning("发送失败：客户端未连接");
                return;
            }

            byte[] sendData = Encoding.UTF8.GetBytes(str);
            clientSocket.Send(sendData, sendData.Length, SocketFlags.None);
        }
        catch (SocketException e) 
        {
            Debug.LogError($"网络错误: {e.ErrorCode} - {e.Message}");
            // 发生异常时重置连接
            SocketQuit();
        }
        catch (NullReferenceException e) 
        {
            Debug.LogError($"空引用异常: {e.Message}");
            SocketQuit();
        }
    }

    
    public string ReturnStr()
    {
        lock (this)
        {
            returnStr = receiveStr;
            receiveStr = null;
        }
        return returnStr;
    }
    public void SocketQuit()
    {
        if (clientSocket != null)
        {
            clientSocket.Close();
        }
        if (thread != null)
        {
            thread.Interrupt();
            thread.Abort();
        }
        serverSocket.Close();
    }
}