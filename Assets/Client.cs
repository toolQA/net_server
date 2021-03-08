using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

[Serializable]
public class Client
{
    public string ip = "";
    //public string Mip = "";

    byte[] sendData = new byte[1024];
    byte[] recvData = new byte[1024];
    public Thread recvMsgThread = null;
    public Socket clientSocket = null;
    ToolDelegate.String cb_recv = null;


    public bool isFinished = false;


    public Client(Socket _clientSocket, ToolDelegate.String recvCB)
    {
        clientSocket = _clientSocket;
        cb_recv = recvCB;
        //客户端的ip
        ip = clientSocket.RemoteEndPoint.ToString();
        //服务器的ip
        //Mip = clientSocket.LocalEndPoint.ToString();

        Debug.LogWarning(ip);

        recvMsgThread = new Thread(new ThreadStart(Receive));
        //recvMsgThread.IsBackground = true;
        recvMsgThread.Start();
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
            Invoke("【发送" + ip + "】" + info);
        }
        catch (Exception e)
        {
            Debug.LogError("send " + ip + " " + e.Message);
        }
    }

    private void Receive()
    {
        while (!isFinished)
        {
            try
            {
                if (!clientSocket.Connected)
                {
                    continue;
                }
                //获取到数据的量
                if (clientSocket.Available <= 0)
                {
                    continue;
                }

                int len = clientSocket.Receive(recvData);
                if (len > 0)
                {
                    string info = System.Text.Encoding.UTF8.GetString(recvData, 0, len);
                    Invoke("【接收" + ip + "】" + info);
                    if (info == "init")
                    {
                        Send("1");
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                Debug.LogError("abort " + ip + e.ToString());
            }
            catch (Exception e)
            {
                Debug.LogError("receive " + ip + " " + e.ToString());
            }
        }

        Debug.Log("断开连接 " + ip);
    }

    void Invoke(string info)
    {
        Debug.Log(info);

        cb_recv?.Invoke(info);
    }

    public void Quit()
    {
        isFinished = true;
        recvMsgThread = null;
        if (clientSocket != null)
        {
            if (clientSocket.Connected)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Disconnect(false);
            }

            clientSocket.Close();
            clientSocket = null;
        }

    }
}