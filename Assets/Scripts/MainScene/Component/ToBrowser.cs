using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class ToBrowser : MonoBehaviour {
    
	public void OpenURLOnBrowser (string url) {
#if UNITY_ANDROID
        DebugLog.DebugLogInfo("kaishi调用android函数");
        AndroidJavaClass jc = new AndroidJavaClass("com.vesal.vesal3dsystemanatomy04");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("MainActivity");
        jo.Call("GetUnityWebParam", url);       //调用android函数
        DebugLog.DebugLogInfo("调用wangcheng");
#endif
    }

    public void ToBrowserBtn()
    {
        OpenURLOnBrowser("www.vesal.cn");
    }
}
