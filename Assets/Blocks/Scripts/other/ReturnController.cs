using UnityEngine;
using System.Collections;

public class ReturnController : MonoBehaviour {
	//Android的Activity
	private AndroidJavaObject activity;

	void Awake()
	{
		//当前游戏体的名字
		//this.name = "AndroidManager";
		#if !UNITY_EDITOR && ( UNITY_iOS || UNITY_ANDROID)
		//获得Android Activity
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
		#endif
		
	}
    void OnClick()
    {
		ShowGoToMainMenuDialog ();
    }
	public void ReturnToMainMenu()
	{

		// if (Screen.orientation == ScreenOrientation.Portrait)
		// {
		// 	Screen.orientation = ScreenOrientation.LandscapeLeft;
		// }
		//SuperGameManager.Instance.ReturnToMenu();



	}
	public void ShowGoToMainMenuDialog()
	{
		activity.Call("showGoToMainMenuDialog");
	}

}
