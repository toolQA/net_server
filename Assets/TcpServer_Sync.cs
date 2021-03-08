using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TcpServer_Sync : MonoBehaviour
{
    ToolDelegate.String recvCB = null;
    byte[] sendData = new byte[1024];
    byte[] recvData = new byte[1024];

    IPEndPoint serverEP = null;
    Socket serverSocket = null;

    public string m_ip = "127.0.0.1";
    public int m_port = 8080;

    public int clientNum = 10;

    public List<Client> cs;

    public SocketType socketType = SocketType.Stream;
    public AddressFamily addressFamily = AddressFamily.InterNetwork;

    public bool isFinished = false;


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

        cs = new List<Client>();
        cs.Capacity = clientNum;

        //创建监听线程
        serverSocket.BeginAccept(Accept_Callback, null);

        Invoke("【TCP服务器启动】 " + serverEP.ToString());
    }

    void Accept_Callback(IAsyncResult ar)
    {
        try
        {
            if (serverSocket != null)
            {
                Socket c = serverSocket.EndAccept(ar);

                cs.Add(new Client(c, Invoke));

                serverSocket.BeginAccept(Accept_Callback, null);
            }
        }
        catch (Exception e)
        {
            Debug.Log("async accept " + e.Message);
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
        isFinished = true;

        if (serverSocket != null)
        {
            serverSocket.Close();
            serverSocket = null;
        }

        if (cs.Count > 0)
        {
            for (int i = 0; i < cs.Count; i++)
            {
                cs[i].Quit();
            }
            cs.Clear();
        }
        cs = null;
    }

    private void Update()
    {
        if (serverSocket != null)
        {
            //Debug.Log(serverSocket.Available);//获取到数据的量
            //Debug.Log(serverSocket.AddressFamily);
            //Debug.Log(serverSocket.SocketType);
            //Debug.Log(serverSocket.ProtocolType);
            //Debug.Log(serverSocket.LocalEndPoint);//服务器的ip
            //Debug.Log(serverSocket.RemoteEndPoint);//客户端的ip

            //Debug.Log(serverSocket.SendTimeout);
            //Debug.Log(serverSocket.SendBufferSize);
            //Debug.Log(serverSocket.ReceiveTimeout);
            //Debug.Log(serverSocket.ReceiveBufferSize);

            //Debug.Log(serverSocket.Handle);
            //Debug.Log(serverSocket.IsBound);
            //Debug.Log(serverSocket.Connected);

            //Debug.Log(serverSocket.Ttl);
            //Debug.Log(serverSocket.NoDelay);
            //Debug.Log(serverSocket.Blocking);
            //Debug.Log(serverSocket.LingerState);
            //Debug.Log(serverSocket.DontFragment);
            //Debug.Log(serverSocket.ExclusiveAddressUse);
            //Debug.Log(serverSocket.UseOnlyOverlappedIO);

            //NotSupportedException: This protocol version is not supported.
            //Debug.Log(serverSocket.DualMode);
            //SocketException: 在 getsockopt 或 setsockopt 调用中指定的一个未知的、无效的或不受支持的选项或层次。
            //Debug.Log(serverSocket.EnableBroadcast);
            //SocketException: 在 getsockopt 或 setsockopt 调用中指定的一个未知的、无效的或不受支持的选项或层次。
            //Debug.Log(serverSocket.MulticastLoopback);


            // 没有关掉线程是因为此时阻塞在Accept操作上了
            //if (Input.GetKeyDown("j"))
            //{
            //    isFinished = true;
            //    clientConnectedThread = null;
            //}
        }
    }
}

//thread
// thread.abort :抛出threadAbortException异常
// thread.interrupt:抛出threadInterruptException异常
