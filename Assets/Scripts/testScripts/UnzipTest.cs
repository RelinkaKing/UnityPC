using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VesalCommon;

public class UnzipTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Vesal_DirFiles.UnZipAsync("D:\\vesalplayer\\李哲微课第一讲——骨骼系统简介.vsl", "D:\\vesalplayer\\", unzipProgress, true));
    }
    void unzipProgress(float p) {
        Debug.LogError(p);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
