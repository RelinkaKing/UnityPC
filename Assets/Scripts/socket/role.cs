using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using VesalCommon;

namespace netcomm
{
    //生成角色控制器
    public class role : MonoBehaviour
    {
        public string role_message;
        public Program program;
        public FrameMessageManager fm_manager;
        public CheckConnect serverMsgManager;
        public bool worker;
        public bool checkConnect = false;
        float timer = 5f;

        void Start()
        {
            program = new Program();
            program.OnpeerBroken += OnpeerBroken;
            program.OnpeerRecive += GetMessage;
            serverMsgManager.ConnectOutTime += fm_manager.EndControl;
            serverMsgManager.ConnectOutTime += OnpeerBroken;
            //获取角色，启动外部通信socket(默认自启动)
            role_message = GetRole();
            DebugLog.DebugLogInfo("ff0000", PublicClass.filePath + " role:" + role_message + " start");
            start_Reconnect();
        }

        void Update()
        {
            if (!worker)
            {
                Reconnect();
            }
            if (checkConnect)
            {
                //调用连接检查器
                serverMsgManager.StartCheck();
                checkConnect = false;
            }
        }

        public void start_Reconnect()
        {
            worker = false;
        }

        void Reconnect()
        {
            timer += Time.deltaTime;
            if (timer > 3)
            {
                InitRoleManager();
                timer = 0;
            }
        }

        //客户端结束控制操作
        public void OnpeerBroken()
        {
            if (role_message == "client")
            {
                Debug.Log("client broken");
                fm_manager.EndControl();
                Debug.Log("start new socket connect");
                string[] args = { role_message };
                bool startFlag = program.Main_start(args, Read());
                //启动连接检查器
                checkConnect = true;
            }
            else
            {

            }
        }

        void InitRoleManager()
        {
            string[] args = { role_message };
            bool startFlag = program.Main_start(args, Read());
            if (startFlag)
            {
                program.Work();
                worker = true;
                if (role_message == "client")
                {
                    //启动连接检查器
                    Debug.Log("client connect seccess");
                    checkConnect = true;
                }
                else
                {
                    Debug.Log("server start seccess");
                }
            }
            else
            {
                //初次连接失败
                worker = false;
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

        public void SendMessageToClient(byte cmd, string msg)
        {
            program.send_cmd(cmd, msg);
        }

        public void SendMessageToClientFd(vesal_socket vsock, byte cmd, string msg)
        {
            program.send_cmd_fd(vsock.fd(), cmd, msg);
        }

        public void SendMessageToServer(byte cmd, string msg)
        {
            vesal_socket vs= program.GetClientSocket();
            if (vs == null)
            {
                serverMsgManager.CloseCheck();
            }
            else
            {
                program.send_cmd_fd(vs.fd(), cmd, msg);
            }
        }

        public void SendByteToClient(byte[] msg)
        {
            program.send_byte(3, msg);
        }

        void GetMessage(vesal_socket vsock, byte cmd, byte[] str)
        {
            if (role_message == "server")
                serverMsgManager.RecieveMessageFromServer(vsock,cmd, str);
            else
                fm_manager.ClientResloveMessage(cmd, str);
        }

        private void OnDestroy()
        {
            program.OnpeerBroken -= OnpeerBroken;
            program.OnpeerBroken -= start_Reconnect;
            program.OnpeerRecive -= GetMessage;
            serverMsgManager.ConnectOutTime -= fm_manager.EndControl;
            serverMsgManager.ConnectOutTime -= OnpeerBroken;
            program.StopWork();
            program.CloseClientSocket();
            Screen.SetResolution((int)ScreenData.instance.width, (int)ScreenData.instance.high, false);
        }

        private void OnApplicationQuit()
        {
            Screen.SetResolution((int)ScreenData.instance.width, (int)ScreenData.instance.high, false);
        }
    }
}
