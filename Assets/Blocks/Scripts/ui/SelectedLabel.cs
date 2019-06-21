using UnityEngine;
using System.Collections;

public class SelectedLabel : MonoBehaviour 
{
    public static SelectedLabel instance;
    public UILabel label;

	// Use this for initialization
	void Start () 
    {
        instance = this;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ShowSelectedName()
    {
        int count = SuperGameManager.Instance.selectedObjects.Count;
        if (count != 0)
        {
            //label.text = Localization.Get(SuperGameManager.instance.selectedObjects[count - 1].name);
            //根据结构名称的L或者R后缀，对应加上左或右
            label.text = SuperGameManager.Instance.selectedObjects[count - 1].GetComponent<TouchableObject>().ModelName;
        }
        else
        {
            label.text = "";
        }
    }
}
