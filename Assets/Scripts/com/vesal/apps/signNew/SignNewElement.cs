using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using UnityEngine.UI;
using Newtonsoft.Json;


public class SignNewElement: MonoBehaviour
{

    //StartStopPoint point;
    //string modName;
    string chName;
    string enName;
    string sgnDes;
    //string stopDes;
    SignNewTest test;
    Color sgnCol;


    Color hightCol;
    camera_params m_params;


    // Use this for initialization
    public void Start () {

        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            test = GameObject.Find("SSPCanvas(Clone)").GetComponent<SignNewTest>();
        }
        else {
            test = GameObject.Find("SignNewCtrl").GetComponent<SignNewTest>();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetData(SignNewInfo  info)

    {
        //point = new StartStopPoint(info);
       sgnCol = new Color(float.Parse(info.r) / 255.0f, float.Parse(info.g) / 255.0f, float.Parse(info.b) / 255.0f,1);
       hightCol=new Color(float.Parse(info.high_r) / 255.0f, float.Parse(info.high_g) / 255.0f, float.Parse(info.high_b) / 255.0f,1);
      
        //modName = info.mod_name;
        chName = info.sm_ch_name;
        enName = info.sm_en_name;
        sgnDes = info.description;
        try
        {
            
       m_params = JsonConvert.DeserializeObject<camera_params>(info.camera_params);
        }
        catch (System.Exception)
        {
        }


    }


    public void ChangeLabelCol()
    {
        if (test.lastLabel != null)
        {
            test.lastLabel.GetComponent<Image>().color = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f, 85 / 255.0f);
        }
        test.currentLabel = this.gameObject;
        gameObject.GetComponent<Image>().color = new Color(33 / 255.0f, 167 / 255.0f, 212 / 255.0f, 85 / 255.0f);
        test.lastLabel = test.currentLabel;


    }

    

    
    public void OnClick()
    {

        //for (int i = 0; i < test.sgnEleList.Count; i++)
        //{

        //    test.sgnEleList[i].gameObject.GetComponent<Image>().color = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f, 85 / 255.0f);
        //    //test.dic[sspTest.sspInfoList[i].mod_name].gameObject.SetActive(false);
        //}
        ChangeLabelCol();


        Debug.Log(chName);
        //Debug.Log(sspTest.muscleChName);
        //GameObject currentModel;
        test.sgnChName.text = chName;
        test.sgnEnName.text = enName;
        test.sgnDes.text = string.Format("<size=48><color=#00FFFF>相关介绍</color></size>：\r\n{0}", sgnDes);
        
        DebugLog.DebugLogInfo(chName);
        
        if (sgnCol != Color.black)
        {
           
            test.objPa.BroadcastMessage("ChangeChoseColor", sgnCol);

             test.objPa.BroadcastMessage("ChangeHightColor",hightCol);
        }
        else
            test.objPa.BroadcastMessage("_ChooseColor", Color.magenta);
        if (m_params != null)
            SceneModels.instance.SetSignPosition(SceneModels.instance.Get_init_models()[0].transform.parent.transform, PublicTools.vector2Vecotor(m_params.camera_pos),
                     PublicTools.vector2Vecotor(m_params.camera_parent_pos), PublicTools.vector2Vecotor(m_params.camera_parent_rot));

    }
}
