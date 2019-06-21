using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenData : MonoBehaviour {

    public static ScreenData instance;
    [NonSerialized]
    public int high;
    [NonSerialized]
    public int width;
    public float ratio;

    private void Awake()
    {
        instance = this;
        InternalSocketMananger.InternalMessageEvent += Get_app;
        high = Screen.height;
        width = Screen.width;
    }

    private void OnDestroy()
    {
        InternalSocketMananger.InternalMessageEvent -= Get_app;
    }

    private void Start()
    {
        // SetChangeWindowsSize((int)ScreenData.instance.width, (int)ScreenData.instance.high);
    }

    public void SetLandScreen()
    {
        if (high > width)
        {
            SwitchHW();
        }
    }

    public void SetPortrit()
    {
        if (high < width)
        {
            SwitchHW();
        }
    }

    //切换横竖屏
    private void SwitchHW()
    {
        int temp = high;
        high=width;
        width = temp;
    }

    string Message;
    public Text screenRes;
    bool start = false;

    public float oldheight=0;
    public float oldwidth=0;

    public void Get_app(string get_message)
    {
        DebugLog.DebugLogInfo(get_message);
        App mm = JsonConvert.DeserializeObject<App>(get_message);
        if (!string.IsNullOrEmpty(mm.width))
        {
            //SetChangeWindowsSize((int)ScreenData.instance.width, (int)ScreenData.instance.high);
            SetExtralWindowsSize(int.Parse(mm.width), int.Parse(mm.height));
            //ScreenData.instance.SetChangeWindowsSize(int.Parse(mm.width), int.Parse(mm.height));
        }
    }

    public void SetChangeWindowsSize(int w,int h,int high_fix=0)
    {
        //强制设为接收到的分辨率
        Screen.SetResolution((int)(WindowControl.GetWindowsScaling() * width), (int)(WindowControl.GetWindowsScaling() * (high - high_fix)), MainConfig.isFullScreen);
        //窗口变化后，进行分辨率设置
        width = w;
        high = h;
        Debug.Log("--------------------------" + width + "x" + high+ "------dpi:------"+ WindowControl.GetWindowsScaling());
    }

    public void SetExtralWindowsSize(int w, int h, int high_fix = 0)
    {
        //强制设为接收到的分辨率
        Screen.SetResolution(w, h, MainConfig.isFullScreen);
        //窗口变化后，进行分辨率设置
        width = w;
        high = h;
        Debug.Log("----------Extral----------" + width + "x" + high + "------dpi:------" + WindowControl.GetWindowsScaling());
    }

    public int fix = 0;

    private void Update() {

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("--------------------------" + width + "x" + high);
            //SetChangeWindowsSize(width, high, true, fix++);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("--------------------------" + width + "x" + high);
            //SetChangeWindowsSize(width, high, false, fix++);
        }
        if (start)
        {
            UnityEngine.Debug.Log(Message);
            showText(); 
            start=false;
        }
    }



    public void ShowText(string str)
    {
        Message=str;
        start=true;
    }

    void showText()
    {
        //screenRes.gameObject.SetActive (true);
        screenRes.text=Message;
        Invoke("WaitScecond",1f);
    }
    void WaitScecond()
    {
        screenRes.gameObject.SetActive (false);
    }

}
