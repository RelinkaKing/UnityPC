using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartInit : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        PublicClass.InitStaticData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
