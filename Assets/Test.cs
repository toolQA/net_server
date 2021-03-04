using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;


public enum HttpType
{
    TCP_O = 0,
    TCP_M = 1,
    UDP = 2
}

public class Test : MonoBehaviour
{
    public UdpServer udp = null;

    public TcpServer tcp = null;

    public TcpServer_One tcp_one = null;

    public Text infoText = null;

    public string str = "";

    public HttpType httpType = HttpType.TCP_O;

    // Start is called before the first frame update
    void Start()
    {
        switch (httpType)
        {
            case HttpType.TCP_O:
                tcp_one.Init(Tools.GetLocalIP(true)[0], UpdateText);
                break;
            case HttpType.TCP_M:
                tcp.Init(Tools.GetLocalIP(true)[0], UpdateText);
                break;
            case HttpType.UDP:
                udp.Init(Tools.GetLocalIP(true)[0], UpdateText);
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (infoText.text != str)
        {
            infoText.text = str;
        }

        if (Input.GetKeyDown("u"))
        {
            if (udp != null)
            {
                udp.Send("test");
            }
        }

        if (Input.GetKeyDown("o"))
        {
            if (tcp_one != null)
            {
                tcp_one.Send("test");
            }
        }
    }

    void UpdateText(string info)
    {
        //str += info + "\n";

        str = info + "\n" + str;
    }

    private void OnApplicationQuit()
    {
        if (udp != null)
        {
            udp.Quit();
        }

        if (tcp != null)
        {
            tcp.Quit();
        }

        if (tcp_one != null)
        {
            tcp_one.Quit();
        }
    }
}