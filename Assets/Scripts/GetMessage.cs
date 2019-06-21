using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetMessage : MonoBehaviour
{
    public static string temp_msg = "";
    //前台再次进入调用
    public void GetAppAssets(string msg)
    {
        temp_msg = "";
        DebugLog.DebugLogInfo("点击前台菜单界面，接收app  " + msg);
        temp_msg = msg;
        if (SceneManager.GetActiveScene().name != "SceneSwitch" && SceneManager.GetActiveScene().name != "DownLoadCommonData") {
            SceneManager.LoadScene("SceneSwitch");
        }
    }

    //首次进入和强杀unity后传递msg调用  加载资源
    public void LoadReasource(string invalid_msg)
    {
        temp_msg = "";
        DebugLog.DebugLogInfo("初次加载  " + invalid_msg);
        // DataManager.instance.PreAssets(); //预加载模型
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public bool isUnityPlaying()
    {
        if (AppOpera.OperaStatus == AppOperState.Apprun)
            return true;
        else
            return false;
    }
}