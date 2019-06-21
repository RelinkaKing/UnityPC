using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHelp : MonoBehaviour {


    public GameObject HelpCavas;

    public Image img;

    public Image[] btn;

    public Sprite[] pages;

    public Sprite[] btnspr;

    public Sprite[] btnspr2;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowHelpPanel()
    {
        HelpCavas.SetActive(true);
    }

    public void CloseHelpPanel()
    {
        HelpCavas.SetActive(false);
    }

    public void ChangePage(int i)
    {
        for (int j = 0; j < 3; j++)
            btn[j].sprite = btnspr2[j];

        
        btn[i].sprite = btnspr[i];
        img.sprite = pages[i];
    }

   
}
