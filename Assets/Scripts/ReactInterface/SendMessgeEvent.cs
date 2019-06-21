using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Hotfix]

public class SendMessgeEvent : MonoBehaviour {

    public static SendMessgeEvent Instance;
    public delegate void MessageCallBackDelegate();
    public event MessageCallBackDelegate SendMessageCallBack;

    [LuaCallCSharp]
    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    [LuaCallCSharp]
    void Start () {
		
	}


    [LuaCallCSharp]
    public void ShowRelationAppButton()
    {
        //UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
        //{
        //    name = "show",
        //    callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
        //});
    }

}