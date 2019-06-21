using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using UnityEngine.UI;
using Newtonsoft.Json;

public class MerElement : MonoBehaviour {

    AcupointTest test;
    AcupointInfo merInfo;


    camera_params m_params;

    // Use this for initialization
    public void Start () {
        test = GameObject.Find("AcupointCtrl").GetComponent<AcupointTest>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetData(AcupointInfo info)
    {

        merInfo= info;
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

        ChangeLabelCol();
        //for (int i = 0; i < test.merEleList.Count; i++)
        //{

        //    test.merEleList[i].gameObject.GetComponent<Image>().color = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f, 85 / 255.0f);
        //    //sgnTest.dic[sspTest.sspInfoList[i].mod_name].gameObject.SetActive(false);

        //    //if (GameObject.Find(test.acuInfoList[i].sm_name) != null)
        //    try
        //    {
        //        test.ToNormalState(GameObject.Find(test.acuInfoList[i].sm_name).GetComponentInChildren<MeshRenderer>().material);
        //    }
        //    catch
        //    {
        //        Debug.Log(GameObject.Find(test.acuInfoList[i].sm_name));

        //    }

        //}
        for (int i = 0; i < test.acuInfoList.Count; i++)
        {
            test.ToNormalState(GameObject.Find(test.acuInfoList[i].sm_name).GetComponentInChildren<MeshRenderer>().material);
        }

        for (int i = 0; i < GameObject.Find(merInfo.sm_name).transform.childCount; i++)
        {

            //Debug.Log(GameObject.Find(merInfo.sm_name).name);
            test.ToSelectState(GameObject.Find(merInfo.sm_name).transform.GetChild(i).GetComponentInChildren<MeshRenderer>().material);
           
        }

       

        if (test.showinfoBG.gameObject.activeInHierarchy)
        {
            test.showinfoBG.gameObject.SetActive(false);
        }

      
       


        //   Debug.Log(chName);
        //Debug.Log(sspTest.muscleChName);
        //GameObject currentModel;
        test.acuChName.text = merInfo.sm_ch_name;
        test.acuCode.text = "";
        test.acuDes.text = "";


        //gameObject.GetComponent<Image>().color = new Color(33 / 255.0f, 167 / 255.0f, 212 / 255.0f, 85 / 255.0f);
        if (m_params != null)
            SceneModels.instance.SetCameraPosition(SceneModels.instance.Get_init_models()[0].transform.parent.transform, PublicTools.vector2Vecotor(m_params.camera_pos),
                      PublicTools.vector2Vecotor(m_params.camera_parent_pos), PublicTools.vector2Vecotor(m_params.camera_parent_rot));



    }
}
