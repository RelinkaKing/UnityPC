using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPTAniPanelControl : MonoBehaviour {
    
    public Camera UICamera;
    public Camera UIBGCamera;
    public GameObject acCamera;
    public GameObject acCameraBox;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
   
    public void initAc(string[] param) {
        AnimationControl.url = param[0];
        AnimationControl.pwd = param[1];
        AnimationControl.aniNo = param[2];
    }

    public void DestroyPPTAniPanel()
    {
        try
        {
            if (AnimationControl.instance!=null) {
                AnimationControl.instance.animationGameObject.SetActive(false);
                DestroyImmediate(AnimationControl.instance.animationGameObject);
                AnimationControl.instance = null;
            }
            DestroyImmediate(this.gameObject);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
        
    }
    public void ToggleAniObj(bool active) {
        //if (AnimationControl.instance != null)
        //{
        //    try {
        //        AnimationControl.instance.animationGameObject.SetActive(active);
        //    }
        //    catch (Exception e) {
        //        Debug.Log(e.Message);
        //        Debug.Log(e.StackTrace);
        //    }
        //}
        StartCoroutine(ToggleAniObjAsync(active));
    }
    public IEnumerator ToggleAniObjAsync(bool active) {
        
        while (true) {
            if (AnimationControl.instance != null && AnimationControl.instance.animationGameObject!=null) {
                AnimationControl.instance.animationGameObject.SetActive(active);
                yield return null;
            }

            yield return null;
        }
    }
}
