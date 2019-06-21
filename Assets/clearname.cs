using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clearname : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var childs = this.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in childs)
        {
            t.name = t.name.Split(' ')[0];
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
