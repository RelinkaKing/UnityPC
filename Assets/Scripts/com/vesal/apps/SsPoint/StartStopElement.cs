using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using UnityEngine.UI;
using VesalCommon;
using Newtonsoft.Json;

public class StartStopElement : MonoBehaviour
{

    StartStopPoint point;
    string modName;
    string chName;
    string enName;
    string startDes;
    string stopDes;
    StartStopTest test;
    StartStopInfo sspinfo;

    camera_params m_params;

   
    
    
    // Use this for initialization
    void Start () {

        test = GameObject.Find("StartStopCtrl").GetComponent<StartStopTest>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetData(StartStopInfo  info)

    {

        sspinfo = info;
        point = new StartStopPoint(info);

        modName = info.mod_name;
        chName = info.ch_name;
        enName = info.en_name;
        startDes = info.start_desc;
        stopDes = info.stop_desc;
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

        for (int i = 0; i < test.sspEleList.Count; i++)
        {

            //test.sspEleList[i].gameObject.GetComponent<Image>().color = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f, 85 / 255.0f);
             test.dic[test.sspInfoList[i].mod_name].gameObject.SetActive(false);
        }
        ChangeLabelCol();
        //if (test.lastLabel!=null)
        //{
        //    test.lastLabel.GetComponent<Image>().color = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f, 85 / 255.0f);
        //}
        //if (test.lastMuscle != null)
        //{

        //    test.lastMuscle.SetActive(false);
        //}
        Debug.Log(chName);
        //Debug.Log(test.muscleChName);
        //GameObject currentModel;
        test.ChName.text =chName;
        test.BiName.text = enName;
        test.Description.text = string.Format("<size=48><color=#00FFFF>起点</color></size>：\r\n{0}\r\n<size=48><color=#00FFFF>止点</color></size>：\r\n{1}", startDes, stopDes);
        //test.currentLabel = this.gameObject;
        //gameObject.GetComponent<Image>().color = new Color(33 / 255.0f, 167 / 255.0f, 212 / 255.0f, 85 / 255.0f);
        //test.lastLabel = test.currentLabel;
        //test.dic[modName].gameObject.SetActive(true);
        test.ShowMuscle(sspinfo);
       // test.dic[modName].BecomeTranslucent();
        DebugLog.DebugLogInfo(chName);
        if (new Color(point.startCol.x, point.startCol.y, point.startCol.z, 1) != Color.black)
            test.objPa.BroadcastMessage("ChangeChoseColor", new Color(point.startCol.x, point.startCol.y, point.startCol.z, 1));
        else
            test.objPa.BroadcastMessage("ChangeChoseColor", Color.magenta);
        if (new Color(point.stopCol.x, point.stopCol.y, point.stopCol.z, 1) != Color.black)
            test.objPa.BroadcastMessage("ChangeResColor", new Color(point.stopCol.x, point.stopCol.y, point.stopCol.z, 1));
       else
            test.objPa.BroadcastMessage("ChangeResColor", Color.magenta);

       // Debug.Log(m_params);
        //Debug.Log( SceneModels.instance.Get_init_models()[0].transform.parent.transform);
        if (m_params != null)
            SceneModels.instance.SetSignPosition(SceneModels.instance.Get_init_models()[0].transform.parent.transform,PublicTools.vector2Vecotor(m_params.camera_pos),
                    PublicTools.vector2Vecotor(m_params.camera_parent_pos), PublicTools.vector2Vecotor(m_params.camera_parent_rot));
      
    }

    
}
 