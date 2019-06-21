using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageProgress : MonoBehaviour {


    public Image image; 
    public Slider ProgreesSlider;
	// Use this for initialization
	void Start () {
    }

    public void ChangeValue()
    {
        image.fillAmount = ProgreesSlider.value;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
