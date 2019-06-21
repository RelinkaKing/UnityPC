using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{

    public void Back_platform()
    {
        DebugLog.DebugLogInfo("back SceneSwitch");
        SceneManager.LoadScene("SceneSwitch");
    }
}
