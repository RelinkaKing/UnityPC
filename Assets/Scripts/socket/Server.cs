using netcomm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour {

    string ip;
    public Program program;
    public CheckConnecter checkConnecter;

    // Use this for initialization
    void Start()
    {
        
    }

    public void InitServer(string ip)
    {
        DontDestroyOnLoad(this);
        program = new Program();
        program.OnpeerBroken += OnpeerBroken;
        program.OnpeerRecive += GetMessage;
        checkConnecter.Getprogram(program);

        string[] args = { "server" };
        bool startFlag = program.Main_start(args, ip);
        if (startFlag)
        {
            program.Work();
        }
    }

    //客户端结束控制操作
    void OnpeerBroken()
    {
        Debug.Log("client broken");
    }

    void GetMessage(vesal_socket vsock, byte cmd, byte[] str)
    {
        checkConnecter.RecieveMessageFromServer(vsock, cmd, str);
    }

}
