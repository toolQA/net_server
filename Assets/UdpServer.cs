using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UdpServer : MonoBehaviour
{
    ToolDelegate.String recvCB = null;
    private Socket socket = null;
    private IPEndPoint serverEP = null;
    private EndPoint clientEP = null;
    private string m_ip = "127.0.0.1";
    private int port = 8080;
    private Thread socketThread = null;
    private bool isRunning = false;

    byte[] recvData = new byte[1024];
    byte[] sendData = new byte[1024];

    public void Init(string selfIp, ToolDelegate.String _recvCB)
    {
        recvCB = _recvCB;

        if (!string.IsNullOrEmpty(selfIp))
        {
            m_ip = selfIp;
        }

        serverEP = new IPEndPoint(IPAddress.Parse(m_ip), port);
        clientEP = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(serverEP);

        socketThread = new Thread(new ThreadStart(Receive));
        socketThread.Start();

        isRunning = true;

        Invoke("【服务器启动】 " + m_ip);
    }

    void Send(string info)
    {
        try
        {
            sendData = System.Text.Encoding.UTF8.GetBytes(info);

            socket.SendTo(sendData, SocketFlags.None, clientEP);

            Invoke("发送 " + clientEP.ToString() + " " + info);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("send " + e.Message);
        }
    }

    void Receive()
    {
        while (true)
        {
            try
            {
                int len = socket.ReceiveFrom(recvData, ref clientEP);

                string info = System.Text.Encoding.UTF8.GetString(recvData, 0, len);

                Invoke("【接收 " + clientEP.ToString() + "】 " + info);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("receive " + e.Message);
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
            socket.Close();
            socketThread.Interrupt();
            socketThread.Abort();

            isRunning = false;
        }
    }
}