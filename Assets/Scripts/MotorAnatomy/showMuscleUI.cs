using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Hotfix]
public class showMuscleUI : MonoBehaviour {
    bool isshow = true;
    public Transform muscleUI;
    public Transform muscleButton;
    public Transform muscleButtonIcon;
    [LuaCallCSharp]
    public void showmuscleUI()
    {
        if (isshow)
        {
            muscleUI.GetComponent<RectTransform>().sizeDelta -= new Vector2(302, 0);
            muscleButton.localPosition -= new Vector3(302, 0, 0);
            muscleButtonIcon.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            isshow = false;
        }
        else
        {
            muscleUI.GetComponent<RectTransform>().sizeDelta += new Vector2(302, 0);
            muscleButton.localPosition += new Vector3(302, 0, 0);
            muscleButtonIcon.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            isshow = true;
        }
    }
}
