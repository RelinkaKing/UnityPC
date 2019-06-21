/*控制背景颜色切换脚本*/
using UnityEngine;
using System.Collections;

public class FlameChoose : MonoBehaviour {

    //边框数组
    public GameObject[] flames;
    //背景摄像机
    public Camera BgCamera;

    public GameObject SettingPanel;
    //颜色灰色
    Color huise = new Color(50 / 255f, 50 / 255f, 50 / 255f, 1);

    void Start()
    {
        //ChangeBackground("huise");              //初始背景为灰色
    }

    //更改背景颜色（颜色）
    public void ChangeBackground(string curColor)
    {
        switch (curColor)
        {
            case "huise":
                //将背景变为灰色
                BgCamera.backgroundColor = huise;
                ControllFlameArray(1);
                break;
            case "white":
                BgCamera.backgroundColor = Color.white;
                ControllFlameArray(0);
                break;
            case "black":
                BgCamera.backgroundColor = Color.black;
                ControllFlameArray(2);
                break;
            default:
                break;
        }
        SettingPanel.SetActive(false);
    }

    //控制选中边框的显示
    private void ControllFlameArray(int flameID)
    {
        for (int i = 0; i < flames.Length; i++)
        {
            if (i == flameID)
                flames[i].gameObject.SetActive(true);
            else
                flames[i].gameObject.SetActive(false);
        }
    }

}
