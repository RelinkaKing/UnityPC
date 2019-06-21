using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[Hotfix]
public class AsyncLoadModel : MonoBehaviour
{
    List<Download_Vesal_info> load_list;
    public GameObject showInfo;
    public static AsyncLoadModel instance;
    public Text text;

    public Image img;
    bool isLoading = true;
    public List<GameObject> needCloseWhenAsyncLoad = new List<GameObject>();
    [LuaCallCSharp]
    void Start()
    {
        instance = this;
    }
    [LuaCallCSharp]
    public void toggleOther(bool flag)
    {
        for (int i = 0; i < needCloseWhenAsyncLoad.Count; i++)
        {
            needCloseWhenAsyncLoad[i].SetActive(flag);
        }
        if (!MainConfig.isShowToggle)
            return;
        if (flag) {
        close_message();
            try
            {
        DestroyImmediate(showInfo.gameObject);

            }
            catch (Exception)
            {

            }
        }
    }
    [LuaCallCSharp]
    public void show_message(string message)
    {
        if (!MainConfig.isShowToggle)
            return;
        if (showInfo!=null) {
        showInfo.SetActive(true);
        text.text = message;
        }

    }
    [LuaCallCSharp]
    public void close_message()
    {
        if (!MainConfig.isShowToggle)
            return;
        if (showInfo != null)
        {
            showInfo.SetActive(false);
            text.text = "";
        }
    }
}
