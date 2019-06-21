using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour {

	public GameObject TriggleObje;
	public GameObject TriggleTigger;
	public GameObject DebugPanelObj;	

    bool OpenDebugValuePanel=false;
		
	public void OpenTimerFlag()
	{
		DebugLog.DebugLogInfo("click");
		OpenTimer=true;
	}
	public void ClosePanel()
	{
		DebugPanelObj.SetActive (false);
	}

	float timer=0;
	bool OpenTimer=false;
	void Timer()
	{
		timer+=Time.deltaTime;
		if(timer>5f)
		{
			OpenDebugValuePanel=true;
			timer=0;
		}
	}

	// Update is called once per frame
	void Update () {
		if(OpenTimer)
		{
			Timer();
		}
		if(OpenDebugValuePanel)
		{
			DebugPanelObj.gameObject.SetActive(true);
			OpenDebugValuePanel=false;
			OpenTimer=false;
		}
	}
}


public class Console : MonoBehaviour
{
	string Str;
	Vector2 v2;
	bool IsShow;


	void Start()
	{
		IsShow = true;
		Str = "";
		v2 = Vector2.zero;
	}


	void Update()
	{
		//当按下退格键时显示或隐藏控制台
		if (Input.GetKey(KeyCode.Backspace))
		IsShow = !IsShow;
	}


	//当脚本启用时注册控制台信息输出的委托
	void OnEnable()
	{
		Application.logMessageReceived += Application_logMessageReceived;
	}


	//当脚本禁用时取消控制台信息输出的委托
	void OnDisable()
	{
		Application.logMessageReceived -= Application_logMessageReceived;
	}


	private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
	{
		//输入控制台的信息
		Str += condition + "\n" + stackTrace + "\n---------------------------------------------------------------\n";
	}


	void OnGUI()
	{
		//绘制控制台窗口
		if (IsShow)
		{
			v2 = GUILayout.BeginScrollView(v2, GUILayout.MinWidth(Screen.width - 5), GUILayout.MaxHeight(400));
			GUILayout.TextArea(Str, GUILayout.MinWidth(Screen.width - 100));
			GUILayout.EndScrollView();
		}
	}
}
