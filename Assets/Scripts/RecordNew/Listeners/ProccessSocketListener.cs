using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using vesal_network;
using XLua;

[Hotfix]
public class ProccessSocketListener : MonoBehaviour {
    vesal_socket _listen_sock = new vesal_socket();
    public static GameObject proccessLisen;
    // Use this for initialization
    [LuaCallCSharp]
    void Start ()
    {
        if (proccessLisen == null)
        {

            DontDestroyOnLoad(this.gameObject);
            proccessLisen = this.gameObject;
            //初始化网络协议
            bool ret = _listen_sock.listen(vesal_socket.get_WeiKePlayer_port());
            if (ret)
            {
                Debug.Log("listen ok");
            }
            else
            {
                Debug.Log("listen failed");
            }
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
    }

    // Update is called once per frame
    [LuaCallCSharp]
    void Update () {
		
	}
    ArrayList _client_socks = new ArrayList();
    [LuaCallCSharp]
    private void FixedUpdate()
    {
        try
        {
            ArrayList listenList = new ArrayList();
            ArrayList clientList = new ArrayList();
            listenList.Add(_listen_sock._sock);
            Socket.Select(listenList, null, null, 1000);
            if (listenList.Count > 0)
            {
                vesal_network.vesal_socket sok = new vesal_network.vesal_socket();
                sok._sock = _listen_sock.accept();
                _client_socks.Add(sok);
                Debug.Log("peer connect!");
            }

            for (int i = 0; i < _client_socks.Count; i++)
            {
                clientList.Add(((vesal_network.vesal_socket)_client_socks[i])._sock);
            }
            if (clientList.Count <= 0 ) {
                return;
            }
            Socket.Select(clientList, null, null, 1000);
            for (int i = 0; i < clientList.Count; i++)
            {
                int index = find_sock_seted((Socket)clientList[i]);
                bool get_packet = false;
                try
                {
                    packet pk = new packet();
                    get_packet = ((vesal_network.vesal_socket)_client_socks[index]).recv_packet(ref pk);
                    if (get_packet)
                    {
                        byte[] buff = new byte[pk._data_len];
                        for (int j = 0; j < pk._data_len; j++)
                        {
                            buff[j] = pk._data[j];
                        }
                        string str = System.Text.Encoding.Default.GetString(buff);
                        string log_content = string.Format("get pack {0}:{1}", pk._cmd_code, str);
                        Debug.Log(log_content);
                        if (pk._cmd_code == (byte)VESAL_CMD_CODE.MSG_CMD)
                        {
                            Debug.Log(str);
                            if (str == "loadScene")
                            {
                                SceneManager.LoadScene("WeiKePlayer");
                            }
                        }
                        else if (pk._cmd_code == (byte)VESAL_CMD_CODE.CTRL_CMD)
                        {
                           
                        }
                        else if (pk._cmd_code == (byte)VESAL_CMD_CODE.WIN_HWND)
                        {
                         
                        }
                    }
                }
                catch(Exception e)
                {
                    Debug.Log(e.Message);
                    // link broken.
                    ((vesal_network.vesal_socket)_client_socks[index]).close();
                    remove_sock_list((Socket)clientList[i]);
                    Debug.Log("link broken");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    [LuaCallCSharp]
    private int find_sock_seted(Socket sock)
    {
        for (int i = 0; i < _client_socks.Count; i++)
        {
            if (sock.Handle == ((vesal_network.vesal_socket)_client_socks[i])._sock.Handle)
                return i;
        }
        return -1;
    }

    [LuaCallCSharp]
    private void remove_sock_list(Socket sock)
    {
        for (int i = 0; i < _client_socks.Count; i++)
        {
            if (sock.Handle == ((vesal_network.vesal_socket)_client_socks[i])._sock.Handle)
            {
                _client_socks.RemoveAt(i);
                return;
            }
        }
    }


}
