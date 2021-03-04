using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

[Serializable]
public class Client
{
    public string address = "";
    public int port = 0;

    byte[] sendData = new byte[1024];
    byte[] recvData = new byte[1024];
    Thread recvMsgThread = null;
    Socket clientSocket = null;
    ToolDelegate.String cb_recv = null;

    public Client(Socket _clientSocket, ToolDelegate.String recvCB)
    {
        clientSocket = _clientSocket;
        cb_recv = recvCB;

        recvMsgThread = new Thread(new ThreadStart(Receive));
        recvMsgThread.IsBackground = true;
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
            Invoke("【发送" + clientSocket.RemoteEndPoint.ToString() + "】" + info);
        }
        catch (Exception e)
        {
            Debug.LogError("send " + clientSocket.RemoteEndPoint.ToString() + " " + e.Message);
        }
    }

    private void Receive()
    {
        while (true)
        {
            try
            {
                int len = clientSocket.Receive(recvData);
                if (len > 0)
                {
                    string info = System.Text.Encoding.UTF8.GetString(recvData, 0, len);
                    Invoke("【接收" + clientSocket.RemoteEndPoint.ToString() + "】" + info);
                    if (info == "init")
                    {
                        Send("1");
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogError("receive " + clientSocket.RemoteEndPoint.ToString() + " " + e.Message);
            }
        }
    }

    void Invoke(string info)
    {
        Debug.Log(info);

        cb_recv?.Invoke(info);
    }
}