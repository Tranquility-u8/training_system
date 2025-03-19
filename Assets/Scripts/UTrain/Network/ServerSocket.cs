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

    }
 
    void GoClient()
    {
        ConnectClient();
        while (true)
        {
            receiveData = new byte[1024];
            recv = clientSocket.Receive(receiveData);
            if (recv == 0)
            {
                ConnectClient();
                continue;
            }
            receiveStr = Encoding.ASCII.GetString(receiveData, 0, recv);
        }
    }
    
    void ConnectClient()
    {
        if (clientSocket != null)
        {
            clientSocket.Close();
        }
        clientSocket = serverSocket.Accept();
    }
    

    public void SendClient(string str) 
    {
        try 
        {
            if (clientSocket == null || 
                !clientSocket.Connected) 
            {
                Debug.LogWarning("Network failure：Server Disconnected");
                return;
            }

            byte[] sendData = Encoding.UTF8.GetBytes(str);
            clientSocket.Send(sendData, sendData.Length, SocketFlags.None);
        }
        catch (SocketException e) 
        {
            Debug.LogError($"Network failure: {e.ErrorCode} - {e.Message}");
            SocketQuit();
        }
        catch (NullReferenceException e) 
        {
            Debug.LogError($"Null error: {e.Message}");
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