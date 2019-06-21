using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowInfo : MonoBehaviour {


    public GameObject area;
    public Text showTxt;

    // Update is called once per frame
    void Update () {
        if(area!=null || showTxt!=null)
        {
            if(showTxt.text!="")
            {
                area.SetActive(true);
            }
            else
            {
                area.SetActive(false);
            }
		
        if(TouchCtrl.isCurMode)
        {
            showTxt.text = "光标模式";
        }
        else if(XT_AllButton.isPrintMode)
        {
            showTxt.text = "画图模式";
        }
        else if(SceneModels.instance!=null && SceneModels.instance.get_Multi_Selection())
        {

            showTxt.text = "多选模式";//：<size=20>可批量选择并隐藏模型，点击右下角多选退出选择</size>";

        }
        else if(SceneModels.instance!=null && SceneModels.instance.get_Split_mode())
        {

            showTxt.text = "拆分模式";//: <size=20>旋转视图不可用，点击结构拖动拆分，双击结构还原位置</size>";

        }
        else
        {
            showTxt.text = "";
        }
        }

	}
}
