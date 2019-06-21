using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using netcomm;

public class CheckConnecter : MonoBehaviour {

    public delegate void CheckConnectDelegate();
    public event CheckConnectDelegate ConnectOutTime;
    //计时器
    float timer = 0;
    bool start_timer = false;
    Program program;

    // Use this for initialization
    public void StartCheck(Program program)
    {
        Getprogram(program);
        SendMessageToServer();
    }

    public void Getprogram(Program program)
    {
        this.program = program;
    }

    // Update is called once per frame
    void Update()
    {
        if (start_timer)
        {
            if (timer > 10)
            {
                OutTime();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }

        if (_start_invoke)
        {
            Invoke("SendMessageToServer", 1);
            _start_invoke = false;
        }
    }

    bool _start_invoke = false;

    public void CanGetMessage()
    {
        //client get check backpack
        //Debug.Log(cmd + "接受到check包");
        start_timer = false;
        _start_invoke = true;
    }

    public void OutTime()
    {
        Debug.Log("connect outtime ,maybe server link broken");
        CloseCurrentSocket();
        ConnectOutTime();
        start_timer = false;
    }

    public void CloseCheck()
    {
        Debug.Log("socket broken ,disable send message to server");
        start_timer = false;
    }

    public void SendMessageToServer()
    {
        vesal_socket vs = program.GetClientSocket();
        if (vs == null)
        {
            CloseCheck();
        }
        else
        {
            program.send_cmd_fd(vs.fd(), (byte)7, "ck");
        }
        timer = 0; start_timer = true;
    }

    public void RecieveMessageFromServer(vesal_socket vsock, byte cmd, byte[] ma)
    {
        if (cmd == (byte)7)
        {
            //server get check backpack
            //Debug.Log("get check backpack--");
            program.send_cmd_fd(vsock.fd(), (byte)8, "ck");
        }
    }

    public void CloseCurrentSocket()
    {
        Debug.Log("close current socket link");
        program.CloseClientSocket();
    }
}
