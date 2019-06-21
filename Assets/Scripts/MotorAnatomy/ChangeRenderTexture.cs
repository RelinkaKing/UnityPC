using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Hotfix]
public class ChangeRenderTexture : MonoBehaviour {

    public RenderTexture renderTexture;
    // Use this for initialization
    [LuaCallCSharp]
    void Start ()
    {
        renderTexture.height = Screen.height;
        renderTexture.width = Screen.width;

    }
	

}
