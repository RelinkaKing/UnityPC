using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Public;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetParamsToPublic : MonoBehaviour
{

    void Start()
    {
        Unity_Tools.Get_scene_instance<ShowProgress>("LoadingCanvas_pc").CloseProgress();
    }
    public void Get_Params(int index)
    {
        DebugLog.DebugLogInfo("LoadScene(Blocks)");
        PublicClass.Difficult_Index = index;
        SceneManager.LoadScene("Blocks");
    }


    //返回平台
    public void ExitProgram()
    {
        DebugLog.DebugLogInfo("back SceneSwitch");
        SceneManager.LoadScene("SceneSwitch");
    }
}
