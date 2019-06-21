using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using vesal_network;

public class AppConnect : MonoBehaviour
{
    vesal_socket _conn_sock = new vesal_socket();
    // Use this for initialization
    void Awake()
    {
        InternalSocketMananger.InternalMessageEvent+=Get_app;
    }
    public void Get_app(string get_message)
    {
        App mm = JsonConvert.DeserializeObject<App>(get_message);
        if (!string.IsNullOrEmpty(mm.width))
        {
            ScreenData.instance.high =int.Parse(mm.height);
            ScreenData.instance.width = int.Parse(mm.width);
        }
        else
        {

        }

    }
    private void OnDestroy() {
        InternalSocketMananger.InternalMessageEvent-=Get_app;
    }
}