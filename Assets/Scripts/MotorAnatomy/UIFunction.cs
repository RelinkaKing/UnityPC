using Assets.Scripts.Public;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;
[Hotfix]
public class UIFunction : MonoBehaviour {
    bool isshowbones = true;
    bool isshowmuscle = true;
    public List<GameObject> muscles = new List<GameObject>();
    public List<GameObject> bones = new List<GameObject>();
    public showMode showMode;
    public TimelineControll timelineControll;

    public static UIFunction Instance;
    [LuaCallCSharp]
    private void Awake()
    {
        Instance = this;
    }

    [LuaCallCSharp]
    public void ShowMuscle(Image image)
    {
        if (isshowmuscle)
        {
            image.enabled = false;
            isshowmuscle = false;
            ShowMuscle(false);
        }
        else
        {
            image.enabled = true;
            isshowmuscle = true;
            ShowMuscle(true);
        }
    }
    [LuaCallCSharp]
    public void ShowBones(Image image)
    {
        if (isshowbones)
        {
            image.enabled = false;
            isshowbones = false;
            ShowBones(false);
        }
        else
        {
            image.enabled = true;
            isshowbones = true;
            ShowBones(true);
        }
    }
    [LuaCallCSharp]
    void ShowMuscle(bool isshow)
    {
        int num = muscles.Count;
        for (int i = 0; i < num; i++)
        {
            muscles[i].SetActive(isshow);
        }
    }
    [LuaCallCSharp]
    void ShowBones(bool isshow)
    {
        int num = bones.Count;
        for (int i = 0; i < num; i++)
        {
            bones[i].SetActive(isshow);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Escape))
        {
            ExitPrograms(true);
        }
    }
    [LuaCallCSharp]
    public void ExitPrograms(bool isback)
    {
        ExitProgram();
        backMessage(isback);
    }


    [LuaCallCSharp]
    public void ExitProgram()
    {
        showMode.closeSence();
        timelineControll.close();
        Interaction.AnimationRotateY = 1;
        DebugLog.DebugLogInfo("back SceneSwitch");
        try
        {
            ManageModel.Instance.Destory_Transform_temp_child();
            DestroyImmediate(GameObject.Find("temp_parent"));
        }
        catch (Exception e)
        {
            DebugLog.DebugLogInfo(e.Message);
            DebugLog.DebugLogInfo(e.StackTrace);
        }
        Unity_Tools.ui_return_sceneSwitch();
    }
    [LuaCallCSharp]
    public void backMessage(bool isback)
    {
        // if (isback)
        // {
        //     Debug.Log("back");
        //     UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
        //     {
        //         name = "back",
        //         callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
        //     });
        // }
        // else
        // {
        //     Debug.Log("exit");
        //     UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
        //     {
        //         name = "exit",
        //         callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
        //     });
        // }

    }
    

    //IEnumerator exit()
    //{

    //    yield return 
    //}

}
