using netcomm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//启动检查，发送package 计时 超时，断链 ，未超时，计时清零，继续发送
public class CheckConnect : MonoBehaviour
{
    public delegate void CheckConnectDelegate();
    public event CheckConnectDelegate ConnectOutTime;
    public role Program_manager;
    //计时器
    float timer = 0;
    bool start_timer = false;

    // Use this for initialization
    public void StartCheck()
    {
        SendMessageToServer();
       
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
        Program_manager.SendMessageToServer((byte)7, "ck");
        timer = 0; start_timer = true;
    }

    public void RecieveMessageFromServer(vesal_socket vsock, byte cmd, byte[] ma)
    {
        if (cmd == (byte)7)
        {
            //server get check backpack
            //Debug.Log("get check backpack--");
            Program_manager.SendMessageToClientFd(vsock, (byte)8, "ck");
        }
    }

    public void CloseCurrentSocket()
    {
        Debug.Log("close current socket link");
        Program_manager.program.CloseClientSocket();
    }
}
