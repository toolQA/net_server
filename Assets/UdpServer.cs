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
    [SerializeField]
    private string m_ip = "127.0.0.1";
    [SerializeField]
    private int port = 8080;
    private Thread socketThread = null;
    private bool isRunning = false;

    byte[] recvData = new byte[1024];
    byte[] sendData = new byte[1024];

    public SocketType socketType = SocketType.Dgram;
    public AddressFamily addressFamily = AddressFamily.InterNetwork;

    public void Init(string selfIp, ToolDelegate.String _recvCB)
    {
        recvCB = _recvCB;

        if (!string.IsNullOrEmpty(selfIp))
        {
            m_ip = selfIp;
        }

        serverEP = new IPEndPoint(IPAddress.Parse(m_ip), port);
        clientEP = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
        //在服务器端创建一个负责监听ip和端口号的socket
        socket = new Socket(addressFamily, socketType, ProtocolType.Udp);
        //绑定端口号
        socket.Bind(serverEP);

        //创建监听线程
        socketThread = new Thread(new ThreadStart(Receive));
        socketThread.IsBackground = true;
        socketThread.Start();

        isRunning = true;

        Invoke("【UDP服务器启动】 " + m_ip + ":" + port.ToString());
    }

    public void Send(string info)
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

                if (info == "init")
                {
                    Send("1");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
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