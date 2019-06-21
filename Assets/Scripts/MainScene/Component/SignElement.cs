using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SignElement : MonoBehaviour {

    public Text Text;
    public Text Label;
    public string num;
    public string _name;
    public string modelName;
    public List<Model> hightLightArray;
    public TextureIcon whichSignIcon;
    public GameObject ExplainPanel;
    public GameObject SignListPanel;
    public bool IsType2 = false;
	// Use this for initialization
	void Start() {
        Label.text =num+"."+ _name;
    }

    /*2.0*/
    public void OnClick()
    {
        GameObject currentModel;

 
            currentModel = GameObject.Find(whichSignIcon.transform.parent.name);
            Camera.main.GetComponent<XT_TouchContorl>().ChooseSignModel(currentModel, TouchRotationModel.signTouch);
        DebugLog.DebugLogInfo(currentModel.name);
        
         
    }
    Color blue = new Color (0,1,1,1);
    public void IsChoosed(bool isChoosed)
    {
        if (isChoosed)
        {
            Text.color = blue;
            Label.color = blue;
        }
        else
        {
            Text.color = Color.black ;
            Label.color = Color.black ;
        }
    }

    public void ChangeTextureUI(string UITextureName)
    {
        this.GetComponent<Image>().sprite = Resources.Load<Sprite>("SignTexture/" + UITextureName);
    }

    public void ChangeStatesUI()
    {
        PublicClass.currentState = RunState.UI;
        DebugLog.DebugLogInfo("EnterUI");
    }
    public void ChangeStatesPlay()
    {
        PublicClass.currentState = RunState.Playing;
        DebugLog.DebugLogInfo("ExitUI");
    }
}
