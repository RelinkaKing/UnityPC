using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetNextScene : MonoBehaviour {

	string NextSceneName="RecordPlay";
    private void Awake() {
		DebugLog.DebugLogInfo("empty scene awake");
		SceneManager.LoadScene("RecordPlay");
	}
	void Start () {
		// DebugLog.DebugLogInfo("empty scene");
		DebugLog.DebugLogInfo("empty scene start");
	}
	
}
