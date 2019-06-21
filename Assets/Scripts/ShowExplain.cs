using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowExplain : MonoBehaviour {

    public Text Label;
    public Text Explain;

    private void Awake()
    {
        UnityMessageManager.ShowMessageEvent += ShowUnityMessage;
    }

    // Use this for initialization
    void Start () {
		
	}

    public void ShowUnityMessage(UnityMessage msg)
    {
        DebugLog.DebugLogInfo("FF0000",msg.name+" "+msg.data.ToString());
    }

    private void OnDestroy()
    {
        UnityMessageManager.ShowMessageEvent -= ShowUnityMessage;
    }

}
