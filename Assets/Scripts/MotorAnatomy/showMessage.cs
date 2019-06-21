using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[Hotfix]
public class showMessage : MonoBehaviour {

    public string msg;
    public string SportsName;
    public string JointName;
    public string MaxAngle;
    bool isshow;
    public GameObject MessageUI;
    public GameObject muscleUI;
    public Image messageButtonImage;
    public Text messageText;
    public Text SportsNameText;
    public Text JointNameText;
    public Text MaxAngleText;

    [LuaCallCSharp]
    public void init()
    {
        messageText.text = msg;
        SportsNameText.text = SportsName;
        JointNameText.text = JointName;
        MaxAngleText.text = MaxAngle;
    }
    [LuaCallCSharp]
    public void ShowMessage()
    {
        if (!isshow)
        {
            MessageUI.SetActive(true);
            muscleUI.SetActive(false);
            messageButtonImage.enabled = true;
            isshow = true;
        }
        else
        {
            MessageUI.SetActive(false);
            muscleUI.SetActive(true);
            messageButtonImage.enabled = false;
            isshow = false;
        }
    }

}
