using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPTGlobal : MonoBehaviour
{

    //ppt播放状态
    public static PPTStatus pptStatus = PPTStatus.unziping;
    //当前Ppt 幻灯片总页数
    public static int SLIDE_SUM = 0;
    //默认幻灯片宽度（实际取屏幕宽高）
    public static int SLIDE_WIDTH = 1920;
    //高度
    public static int SLIDE_Height = 1080;
    //读取的控制信息xml 幻灯片实际宽高
    public static float Set_WIDTH;
    public static float Set_Height;
    //Ppt插件旧数据库密码
    public static string liteDbPwd = string.Empty;
    //当前播放文件夹基础路径
    public static string PPTPath = string.Empty;
    //当前页基础路径
    public static string pagePath = string.Empty;
    //旧数据库位置
    public static string Vesal_Data_Path = string.Empty;
    //Ppt播放平台
    public static PPTEnvironment PPTEnv = PPTEnvironment.pc;
    /// <summary>
    /// 全局信息初始化
    /// </summary>
    public static void init()
    {
        SLIDE_WIDTH = Screen.width;
        SLIDE_Height = Screen.height;

        Vesal_Data_Path = string.Empty;
        liteDbPwd = string.Empty;
        pptStatus = PPTStatus.unziping;
        PPTPath = string.Empty;
        pagePath = string.Empty;
    }
    //ppt播放状态
    public enum PPTStatus
    {
        unziping,
        initial,
        loading,
        play,
        change,
        pause,
        qa
    }
    //ppt播放环境
    public enum PPTEnvironment
    {
        plugin,
        PPTPlayer,
        WeiKePlayer,
        android,
        ios,
        pc,
        demo_pc
    }
    public static void ColorLog(string message, string color = "red")
    {
        Debug.Log("<color=" + color + ">" + message + "</color>");
    }
}
