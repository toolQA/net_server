using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TcpServer : MonoBehaviour
{
    ToolDelegate.String recvCB = null;
    byte[] sendData = new byte[1024];
    byte[] recvData = new byte[1024];

    IPEndPoint serverEP = null;
    Socket serverSocket = null;

    public string m_ip = "127.0.0.1";
    public int m_port = 8080;

    public bool isRunning = false;

    public int clientNum = 10;
    public Dictionary<string, Client> dic_client;

    Thread clientConnectedThread = null;

    public SocketType socketType = SocketType.Stream;
    public AddressFamily addressFamily = AddressFamily.InterNetwork;


    public void Init(string selfIp, ToolDelegate.String _recvCB)
    {
        recvCB = _recvCB;

        if (!string.IsNullOrEmpty(selfIp))
        {
            m_ip = selfIp;
        }

        serverEP = new IPEndPoint(IPAddress.Parse(m_ip), m_port);
        //在服务器端创建一个负责监听ip和端口号的socket
        serverSocket = new Socket(addressFamily, socketType, ProtocolType.Tcp);
        //绑定端口号
        serverSocket.Bind(serverEP);
        //tcp还需要设置监听客户端的最大数量
        //设置监听，最大同时连接100台
        serverSocket.Listen(clientNum);

        dic_client = new Dictionary<string, Client>();

        //创建监听线程
        clientConnectedThread = new Thread(new ThreadStart(Receive));
        clientConnectedThread.IsBackground = true;
        clientConnectedThread.Start();

        isRunning = true;

        Invoke("【TCP服务器启动】 " + serverEP.ToString());
    }

    private void Receive()
    {
        while (true)
        {
            try
            {
                Socket clientSocket = serverSocket.Accept();

                string clientIP = clientSocket.RemoteEndPoint.ToString();

                Client client = new Client(clientSocket, recvCB);

                if (dic_client.ContainsKey(clientIP))
                {
                    dic_client.Add(clientIP, client);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }   
    }

    void Invoke(string info)
    {
        //Debug.Log(info);

        if (recvCB != null)
        {
            recvCB(info);
        }
    }

    public void Quit()
    {
        if (isRunning)
        {
            serverSocket.Close();
            clientConnectedThread.Interrupt();
            clientConnectedThread.Abort();

            isRunning = false;
        }
    }
}
