using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Commandbuild{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    [MenuItem("Commandbuild/Commandbuild")]
    public static void BuildAndroid()
    {
        string[] levels = { "Assets/Scenes/DownLoadCommonData.unity",
        "Assets/Scenes/MyClass.unity",
        "Assets/Scenes/SceneSwitch.unity",
        "Assets/Scenes/totalScence.unity",
        "Assets/Scenes/UI2.unity",
        "Assets/Scenes/WeiKePlayer.unity",
        };
        BuildPipeline.BuildPlayer(levels, "D:\\git\\unityAndPorjectOutput", BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
    }
}
