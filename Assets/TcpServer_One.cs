using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TcpServer_One : MonoBehaviour
{
    ToolDelegate.String cb_recv = null;

    public string m_ip = "127.0.0.1";
    public int m_port = 8080;

    Socket serverSocket = null;
    Socket clientSocket = null;
    Thread connectThread = null;

    byte[] sendData = new byte[1024];
    byte[] recvData = new byte[1024];

    public void Init(string selfIp, ToolDelegate.String recvCB)
    {
        cb_recv = recvCB;

        if (!string.IsNullOrEmpty(selfIp))
        {
            m_ip = selfIp;
        }


        IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(m_ip), m_port);
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(serverEP);
        serverSocket.Listen(10);


        connectThread = new Thread(new ThreadStart(Receive));
        connectThread.IsBackground = true;
        connectThread.Start();

        Invoke("【TCP服务器启动】 " + serverEP.ToString());
    }

    private void Invoke(string v)
    {
        Debug.Log(v);

        cb_recv?.Invoke(v);
    }

    public void Send(string info)
    {
        if (string.IsNullOrEmpty(info))
        {
            return;
        }

        try
        {
            sendData = System.Text.Encoding.UTF8.GetBytes(info);
            clientSocket.Send(sendData);
            Invoke("【发送 " + clientSocket.RemoteEndPoint.ToString() + "】 " + info);
        }
        catch (System.Exception e)
        {
            Debug.LogError("send " + e.Message);
        }
    }

    void Receive()
    {
        while (true)
        {
            try
            {
                if (clientSocket == null)
                {
                    clientSocket = serverSocket.Accept();
                }

                int len = clientSocket.Receive(recvData);
                if (len > 0)
                {
                    string info = System.Text.Encoding.UTF8.GetString(recvData, 0, len);
                    Invoke("【接收 " + clientSocket.RemoteEndPoint.ToString() + "】 " + info);

                    if (info == "init")
                    {
                        Send("1");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("recv " + e.Message);
            }
        }
    }

    public void Quit()
    {
        if (serverSocket != null)
        {
            serverSocket.Close();
            serverSocket = null;
        }

        if (clientSocket != null)
        {
            clientSocket.Close();
            clientSocket = null;
        }

        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
    }
}