using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponetBtnController : MonoBehaviour {
    
    //用于关闭场景功能
    public GameObject[] componets;

	void Start () {

#if UNITY_EDITOR
        return;
#elif UNITY_IOS
        for (int i = 0; i < componets.Length; i++)
        {
            if(componets[i].gameObject!=null)
                componets[i].SetActive(false);
        }
#endif
    }
}
