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

    public int clientNum = 10;

    public List<Client> cs;

    Thread clientConnectedThread = null;

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
        clientConnectedThread = new Thread(new ThreadStart(Receive));
        //clientConnectedThread.IsBackground = true;
        clientConnectedThread.Start();

        Invoke("【TCP服务器启动】 " + serverEP.ToString());
    }

    private void Receive()
    {
        while (!isFinished)
        {
            try
            {
                if (cs.Count >= clientNum)
                {
                    Debug.LogError("已达连接上限");
                    continue;
                }

                //同步Accept，当关闭的时候会出现阻塞操作被中断的异常SocketException
                Socket clientSocket = serverSocket.Accept();//这里还是有问题，晚点换成异步试下

                cs.Add(new Client(clientSocket, recvCB));
            }
            catch (SocketException e)
            {
                Debug.LogError("////" + e.ErrorCode);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
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
        isFinished = true;
        clientConnectedThread = null;

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
            Debug.Log(serverSocket.Available);
            Debug.Log(serverSocket.AddressFamily);
            Debug.Log(serverSocket.Blocking);
            Debug.Log(serverSocket.Connected);
            Debug.Log(serverSocket.DontFragment);

            Debug.Log(serverSocket.ExclusiveAddressUse);
            Debug.Log(serverSocket.Handle);
            Debug.Log(serverSocket.IsBound);
            Debug.Log(serverSocket.LingerState);
            Debug.Log(serverSocket.LocalEndPoint);

            Debug.Log(serverSocket.NoDelay);
            Debug.Log(serverSocket.ProtocolType);
            Debug.Log(serverSocket.ReceiveBufferSize);
            Debug.Log(serverSocket.ReceiveTimeout);
            Debug.Log(serverSocket.RemoteEndPoint);
            Debug.Log(serverSocket.SendBufferSize);
            Debug.Log(serverSocket.SendTimeout);
            Debug.Log(serverSocket.SocketType);
            Debug.Log(serverSocket.Ttl);
            Debug.Log(serverSocket.UseOnlyOverlappedIO);

            //NotSupportedException: This protocol version is not supported.
            //Debug.Log(serverSocket.DualMode);
            //SocketException: 在 getsockopt 或 setsockopt 调用中指定的一个未知的、无效的或不受支持的选项或层次。
            //Debug.Log(serverSocket.EnableBroadcast);
            //SocketException: 在 getsockopt 或 setsockopt 调用中指定的一个未知的、无效的或不受支持的选项或层次。
            //Debug.Log(serverSocket.MulticastLoopback);
        }
    }
}
