using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using VesalCommon;

public class RoleController : MonoBehaviour {

    string role_message;
    public Client client;
    public Server server;

    // Use this for initialization
    void Start () {
        role_message = GetRole();
        DebugLog.DebugLogInfo("ff0000", PublicClass.filePath + " role:" + role_message + " start");
        if (role_message == "server")
        {
            server.InitServer(Read());
        }
        else
        {
            client.start_connect(Read());
        }
    }
	
    string Read()
    {
        string target_file = PublicClass.fileLocalStreamingPath + "server_ip.txt";
        List<string> tempList = Vesal_DirFiles.ReadFileWithLine(target_file);
        string str = "";
        for (int i = 0; i < tempList.Count; i++)
        {
            str += tempList[i];
        }
        if (str.Contains("server"))
        {
            return str.Split('s')[0];
        }
        else
        {
            return str.Split('c')[0];
        }
    }

    string GetRole()
    {
        string target_file = PublicClass.fileLocalStreamingPath + "server_ip.txt";
        Debug.Log("server_ip info path " + target_file);
        List<string> tempList = Vesal_DirFiles.ReadFileWithLine(target_file);
        string str = "";
        for (int i = 0; i < tempList.Count; i++)
        {
            str += tempList[i];
        }
        if (str.Contains("server"))
        {
            return "server";
        }
        else
        {
            return "client";
        }
    }

}
