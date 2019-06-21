using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommand : BaseCommand {
    SceneModelState modelsState;
    SceneBtnState btnState;
    CameraParams cameraParams;

    public PlayerCommand () {
        modelsState = new SceneModelState ();
        
    }

    public PlayerCommand (SceneModelState State) {
        modelsState = State;
    }

    public PlayerCommand(bool isRecord)
    {
        if (isRecord)
        {
            modelsState = new SceneModelState();
            btnState = new SceneBtnState();
            cameraParams = new CameraParams();
        }

    }
    public PlayerCommand(int recordBtn)
    {
        if(recordBtn==1)
        {
            btnState = new SceneBtnState();
        }
    }

    public PlayerCommand(SceneModelState _modelState,SceneBtnState _btnState,CameraParams _cameraParams )
    {
        modelsState = _modelState;
        btnState = _btnState;
        cameraParams = _cameraParams;

    }
    /// <summary>  
    /// 执行命令  
    /// </summary>  
    public override void ExecuteCommand () {
        base.ExecuteCommand ();
    }

    /// <summary>  
    /// 撤销命令  
    /// </summary>  
    public override void RevocationCommand () {
        base.RevocationCommand ();
        Model[] tempAllmodel = SceneModels.instance.Get_scope_models ();
        //解析场景模型信息
        for (int i = 0; i < tempAllmodel.Length; i++) {
            modelState outState = modelsState.SceneModelStateDict[tempAllmodel[i].name];
            if (outState.isActive) {
                tempAllmodel[i].BecomeDisplay ();
                // if (outState.isSeleted)
                // {
                //     tempAllmodel[i].BecomeHight();
                // }
                // else
                // {
                tempAllmodel[i].BecomeNormal ();
                // }
                if (outState.isFade) {
                    tempAllmodel[i].BecomeTranslucent ();
                }
            } else {
                tempAllmodel[i].BecomeHide ();
            }
        }
    }

    public void ReadBottomBtnState()
    {
        

    }

    

    public void ReadBookMark()
    {

        if(btnState.isSplitBtnUI!=SplitMode.isSpliteMode)
        {
            //SceneModels.instance.set_Multi_Selection(false);

            PublicClass.splitmode.OpenSplitMode();

        }

       
        if (btnState.isMultiBtnUI == false)
        {
            SceneModels.instance.set_Multi_Selection(false);
            // SceneModels.instance.CancleSelect();
            UIChangeTool.ShowOneObject(XT_AllButton.Instance.openMu, XT_AllButton.Instance.closeMu, false);
            // XT_AllButton.Instance.multiSelectBtn.GetComponent<Image>().color = Color.white;
            //XT_TouchContorl.Instance.expPanel.SetActive(false);
        }
        else
        {
            SceneModels.instance.set_Multi_Selection(true);
            // XT_AllButton.Instance.multiSelectBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            UIChangeTool.ShowOneObject(XT_AllButton.Instance.openMu, XT_AllButton.Instance.closeMu, true);
            //进入多选模式将下方解释窗口置为true
            XT_TouchContorl.Instance.expPanel.SetActive(true);
        }

        DebugLog.DebugLogInfo("交互模块赋值");
        Interaction.instance.gameObject.transform.localPosition = new Vector3(cameraParams.positionx, cameraParams.positiony, cameraParams.distance);

        Interaction.instance.rotateAxis.transform.localPosition = new Vector3(cameraParams.rotateAxis.x, cameraParams.rotateAxis.y, cameraParams.rotateAxis.z);
        Interaction.instance.y = cameraParams.rotation.x;
        Interaction.instance.x = cameraParams.rotation.y;
        Interaction.instance.rotateAxis.transform.rotation = Quaternion.Euler(-cameraParams.rotation.x, cameraParams.rotation.y, 0);

        DebugLog.DebugLogInfo("场景模型信息");
        Model[] tempAllmodel = SceneModels.instance.Get_scope_models();
        DebugLog.DebugLogInfo("场景模型长度 " + tempAllmodel.Length);

        try
        {
            //解析场景模型信息
            for (int i = 0; i < tempAllmodel.Length; i++)
            {
                modelState outState = modelsState.SceneModelStateDict[tempAllmodel[i].name];
                if (outState.isActive)
                {

                    tempAllmodel[i].gameObject.transform.position = new Vector3(outState.position.x, outState.position.y, outState.position.z);
                    tempAllmodel[i].BecomeDisplay();
                    if (outState.isSeleted)
                    {
                        SceneModels.instance.ChooseModel(tempAllmodel[i]);
                    }
                    else
                    {
                        tempAllmodel[i].BecomeNormal();
                    }
                    if (outState.isFade)
                    {
                        tempAllmodel[i].BecomeTranslucent();
                    }
                }
                else
                {
                    tempAllmodel[i].BecomeHide();
                }
            }
        }
        catch (System.Exception e)
        {
            DebugLog.DebugLogInfo("Message  " + e.Message);
            DebugLog.DebugLogInfo("StackTrace  " + e.StackTrace);
        }


    }

}