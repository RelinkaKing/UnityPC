using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyController : MonoBehaviour {
	
	public List<GameObject> EnableObjects;
	public List<GameObject> CloseObjects;
	public void DifficultyIndex(int index)
	{
		//调节距离参数，控制难度
		
		//模型变化位置（可固定）
	}

	//临时用于切换ngui  和 ugui 界面主题
	public void OperatEnableObject(bool isEnable)
	{
		// for (int i = 0; i < EnableObjects.Count; i++)
		// {
		// 	EnableObjects[i].SetActive (isEnable);
		// }
		// for (int i = 0; i < CloseObjects.Count; i++)
		// {
		// 	CloseObjects[i].SetActive (!isEnable);
		// }
		
	}

	public void OpenChoose_platform()
	{
		SceneManager.LoadScene("Block_UI");
		// OperatEnableObject(false);
		// NotificationManager.instance.EndAll_element();
		// Block_loadmodel.Instance.Reset_Manager_Data();
	}
}
