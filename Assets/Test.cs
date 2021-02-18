using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public UdpServer server = null;

    public Text infoText = null;

    public string str = "";

    // Start is called before the first frame update
    void Start()
    {
        server.Init(Tools.GetLocalIP(true)[0], UpdateText);
    }

    // Update is called once per frame
    void Update()
    {
        if (infoText.text != str)
        {
            infoText.text = str;
        }
    }

    void UpdateText(string info)
    {
        //str += info + "\n";

        str = info + "\n" + str;
    }
}