using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChangeTool {

	public  static void ShowOneObject(GameObject obj_1,GameObject obj_2,bool openFirst)
	{
		obj_1.SetActive(openFirst);
		obj_2.SetActive(!openFirst);
	}
}
