using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using UnityEngine.UI;
using Newtonsoft.Json;


public class AcuElement : MonoBehaviour
{

    string modName;
    string chName;
    string isoCode;
    string position;
    string cure;
    string handle;

    camera_params m_params;

    AcupointTest test;






    // Use this for initialization
    public void Start()
    {

        test = GameObject.Find("AcupointCtrl").GetComponent<AcupointTest>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetData(AcupointInfo info)

    {
        //point = new StartStopPoint(info);
        //sgnCol = new Color(float.Parse(info.r) / 255.0f, float.Parse(info.g) / 255.0f, float.Parse(info.b) / 255.0f,1);
        modName = info.sm_name;

        chName = info.sm_ch_name;
        isoCode = info.iso_code;
        position = info.position;
        cure = info.cure;
        handle = info.handle;

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

        //for (int i = 0; i < test.acuEleList.Count; i++)
        //{

        //    test.acuEleList[i].gameObject.GetComponent<Image>().color = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f, 85 / 255.0f);
        //    //sgnTest.dic[sspTest.sspInfoList[i].mod_name].gameObject.SetActive(false);


        //   // test.ToNormalState(GameObject.Find(test.acuInfoList[i].sm_name).GetComponentInChildren<MeshRenderer>().material);

        //}
        ChangeLabelCol();


        for (int i = 0; i < test.acuInfoList.Count; i++)
        {
            if (GameObject.Find(test.acuInfoList[i].sm_name) != null)
                test.ToNormalState(GameObject.Find(test.acuInfoList[i].sm_name).GetComponentInChildren<MeshRenderer>().material);
        }
        if (test.lastSphere != null)
        {
            test.ToNormalState(test.lastSphere.GetComponentInChildren<MeshRenderer>().material);
            //test.lastMuscle.SetActive(false);
        }

        test.currentSphere = GameObject.Find(modName);
        test.ToSelectState(GameObject.Find(modName).GetComponentInChildren<MeshRenderer>().material);
        test.lastSphere = test.currentSphere;
        if (!test.showinfoBG.gameObject.activeInHierarchy)
        {
            test.showinfoBG.gameObject.SetActive(true);
        }
        test.objPos = GameObject.Find(modName).GetComponentInChildren<Transform>().position;
        test.showTxt.text = chName;


        //   Debug.Log(chName);
        //Debug.Log(sspTest.muscleChName);
        //GameObject currentModel;
        test.acuChName.text = chName;
        test.acuCode.text = isoCode;
        test.acuDes.text = string.Format("<size=48><color=#00FFFF>定位</color></size>：\r\n{0}\r\n<size=48><color=#00FFFF>主治</color></size>：\r\n{1}\r\n<size=48><color=#00FFFF>操作</color></size>：\r\n{2}", position, cure, handle);
        //gameObject.GetComponent<Image>().color = new Color(33 / 255.0f, 167 / 255.0f, 212 / 255.0f, 85 / 255.0f);
        if(m_params!=null)
        SceneModels.instance.SetCameraPosition(SceneModels.instance.Get_init_models()[0].transform.parent.transform, PublicTools.vector2Vecotor(m_params.camera_pos),
                      PublicTools.vector2Vecotor(m_params.camera_parent_pos), PublicTools.vector2Vecotor(m_params.camera_parent_rot));


    }
}