using netcomm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour {

    public string role_message;
    string ip;
    public Program program;
    public CheckConnecter checkConnecter;
    public FrameMessageManager fm_manager;
    public bool checkConnect = false;
    float timer = 5f;
    public bool worker;

    // Use this for initialization
    void Start () {
        
    }

    //start client
    public void start_connect(string ip)
    {
        this.ip = ip;
        DontDestroyOnLoad(this);
        role_message = "client";
        program = new Program();
        program.OnpeerBroken += OnpeerBroken;
        program.OnpeerRecive += GetMessage;
        checkConnecter.ConnectOutTime += OnpeerBroken;
        //start_connect();
        worker = false;
    }

    public void start_connect()
    {
        worker = false;
    }

    public void SendMessageToServer(byte cmd, string msg)
    {
        vesal_socket vs = program.GetClientSocket();
        if (vs == null)
        {
            checkConnecter.CloseCheck();
        }
        else
        {
            program.send_cmd_fd(vs.fd(), cmd, msg);
        }
    }

    void InitClient()
    {
        string[] args = { role_message };
        bool startFlag = program.Main_start(args, ip);
        if (startFlag)
        {
            program.Work();
            worker = true;
            //启动连接检查器
            Debug.Log("client connect seccess");
            checkConnect = true;
        }
        else
        {
            //初次连接失败
            worker = false;
        }
    }

    //客户端结束控制操作
    void OnpeerBroken()
    {
        Debug.Log("client broken");
        EndControl();
        Debug.Log("start new socket connect");
        string[] args = { role_message };
        bool startFlag = program.Main_start(args, ip);
        //启动连接检查器
        checkConnect = true;
    }

    void EndControl()
    {
        fm_manager.EndControl();
    }

    void GetMessage(vesal_socket vsock, byte cmd, byte[] str)
    {
        //end
        if (cmd == (byte)6)
        {
            fm_manager.EndClient();
        }//frame
        else if (cmd == (byte)3)
        {
            fm_manager.ClientGetCommandData(str);
        }//start
        else if (cmd == (byte)1)
        {
            fm_manager.ClientStartEvent(str);
        }
        else if (cmd == (byte)8)
        {
            //Debug.Log(cmd + "接受到check包");
            //开启一次超时检测
            checkConnecter.CanGetMessage();
        }
        else
        {
            fm_manager.ClientEndRecieve();
            Debug.Log(cmd + " cmd code error");
        }
    }

    void Update () {
        if (!worker)
        {
            Reconnect();
        }
        if (checkConnect)
        {
            //调用连接检查器
            checkConnecter.StartCheck(program);
            checkConnect = false;
        }
    }

    void Reconnect()
    {
        timer += Time.deltaTime;
        if (timer > 3)
        {
            InitClient();
            timer = 0;
        }
    }

    private void OnDestroy()
    {
        program.StopWork();
        program.CloseClientSocket();
        program.OnpeerBroken -= OnpeerBroken;
        program.OnpeerBroken -= start_connect;
        program.OnpeerRecive -= GetMessage;
        checkConnecter.ConnectOutTime -= OnpeerBroken;
    }
}