using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using vesal_network;

public class InternalSocketMananger : MonoBehaviour {

    public static InternalSocketMananger instance;
    public GameObject SocketMoudle;
    public delegate void InternalGetMessage(string msg);
    public static event InternalGetMessage InternalMessageEvent;
    public bool isShowUnityWindow = false;
    vesal_socket _conn_sock = new vesal_socket();

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR
#elif  UNITY_STANDALONE_WIN
        bool result = _conn_sock.connect("127.0.0.1", 16667);
        if (!result)
        {
            UnityEngine.Debug.Log("cant conn");
            vesal_log.vesal_write_log("cant conn");
        }
		//send_cmd((byte)VESAL_CMD_CODE.MSG_CMD, "hide");
 #endif
    }

    //socket 调用
	void GetMessage(string str)
	{
        App mm = JsonConvert.DeserializeObject<App>(str);
        if (mm.app_type=="web_socket")
        {
            //启动外部 socket通讯连接
            SocketMoudle.SetActive(true);
        }
        InternalMessageEvent(str);
	}
    public void HideUnityWidows()
    {
        isShowUnityWindow = false;
        try
        {
            send_cmd((byte)VESAL_CMD_CODE.MSG_CMD, "hide");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    public void ShowUnityWidows()
    {
        try
        {
            //isShowUnityWindow = true;
            send_cmd((byte)VESAL_CMD_CODE.MSG_CMD, "show");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    public bool IsWindowShow()
    {
        return isShowUnityWindow;
    }

#if UNITY_EDITOR
#elif UNITY_STANDALONE_WIN
    private void FixedUpdate()
    {
        try
        {
            ArrayList clientList = new ArrayList();
            clientList.Add(_conn_sock._sock);
            Socket.Select(clientList, null, null, 1);
            if (clientList.Count > 0)
            {
                try
                {
                    packet pk = new packet();
                    bool get_packet = _conn_sock.recv_packet(ref pk);
                    //UnityEngine.Debug.Log(get_packet);
                    if (get_packet)
                    {
                        byte[] buff = new byte[pk._data_len];

                        for (int j = 0; j < pk._data_len; j++)
                        {
                            buff[j] = pk._data[j];
                        }
                        //接收
                        string str = System.Text.Encoding.UTF8.GetString(buff);
                        UnityEngine.Debug.Log(pk._cmd_code == (byte)VESAL_CMD_CODE.MSG_CMD);
                        //vesal_log.vesal_write_log("sub"+pk._cmd_code+"  "+str);
                        UnityEngine.Debug.Log(pk._cmd_code);
                        UnityEngine.Debug.Log((byte)VESAL_CMD_CODE.MSG_CMD);
                        //
                        if (pk._cmd_code == (byte)VESAL_CMD_CODE.MSG_CMD)
                        {
                            UnityEngine.Debug.Log("realMessage:" + str);
                            vesal_log.vesal_write_log("realMessage:" + str);
                            GetMessage(str);
                        }
                    }
                }
                catch (Exception e)
                {
                    //UnityEngine.Debug.Log(e.Message);
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
        }
    }
#endif

    public bool send_cmd(byte cmd, String str)
    {
#if UNITY_EDITOR
#elif UNITY_STANDALONE_WIN
        byte[] data = packet.create_output_from_string(cmd, str);
        _conn_sock.send_data(data);
#endif
        return true;
    }
}
