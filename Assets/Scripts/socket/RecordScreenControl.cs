using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordScreenControl : MonoBehaviour {

    public static RecordScreenControl instance;
    float CurrentRatio;
    public float RemoteRatio;
    public int high;
    public int width;

    private void Awake()
    {
        instance = this;
    }

    void Start () {
        ReFrashRatio();
    }

    void ReFrashRatio()
    {
        CurrentRatio = Screen.height / (1f * Screen.width);
    }

    byte[] ratioB;
    float ratio = 1.2f;
    bool isRecord = false;


    bool getRatio=false;
    public void GetRemoteRatio(byte[] ratio=null, bool _isRecord = false)
    {
        if (ratio != null)
        {
            ratioB = ratio;
        }
        isRecord = _isRecord;
        getRatio = true;
    }

    void Update()
    {
        if (getRatio)
        {
            ratio = float.Parse(System.Text.Encoding.UTF8.GetString(ratioB));
            SetRatio();
            getRatio = false;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SetPortrit();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetLandSpace();
        }
    }

    public void SetRatio()
    {
        RemoteRatio = ratio;
        if (isRecord)
        {
            //远程竖屏,本地横屏
            if (RemoteRatio > 1 && CurrentRatio < 1)
            {
                SetPortrit();
            }
            if (RemoteRatio < 1 && CurrentRatio > 1)
            {
                SetLandSpace();
            }
        }
        else
        {
            if (CurrentRatio > 1)
            {
                SetPortrit();
            }
            else
            {
                SetLandSpace();
            }
        }
    }

    //设置横屏
    public void SetLandSpace()
    {
        ScreenData.instance.ShowText("set landSpace，ratio:");
        ScreenData.instance.SetLandScreen();
        ReFrashRatio();
    }

    //设置竖屏
    public void SetPortrit()
    {
        ScreenData.instance.ShowText("set poritSpace，ratio:");
        ScreenData.instance.SetPortrit();
        ReFrashRatio();
    }
}
