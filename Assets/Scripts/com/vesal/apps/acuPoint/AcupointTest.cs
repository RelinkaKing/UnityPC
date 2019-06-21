using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Repository;
using Assets.Scripts.Model;
using System.IO;
using UnityEngine.UI;
using VesalCommon;
using Newtonsoft.Json;

public class AcupointTest : MonoBehaviour
{
    public static AcupointTest instance;
    public GameObject acuParent;

    public GameObject merParent;

    //关联小球坐标和穴位名称
    [HideInInspector]
    public Dictionary<Vector3, string> acpoint_dic = new Dictionary<Vector3, string>();

    public Image showinfoBG;
    [HideInInspector]
    public Text showTxt;

    public Text acuChName;
    public Text acuCode;
    public Text acuDes;

    [Tooltip("菜单切换栏")]
    public GameObject ExpPanel;
    [Tooltip("下方信息显示框")]
    public GameObject MenuPanel;
    [Tooltip("经络信息显示框")]
    public GameObject MerPanel;

    public Button i;
    public Button ii;
    protected bool isListOpen = false;

    public GameObject sphere;
    DbRepository<AcupointInfo> local_db;
    [HideInInspector]
    public List<AcupointInfo> acuInfoList;
    //
    List<Transform> acuPosList;

    [HideInInspector]
    public List<AcuElement> acuEleList;


    Vector2 txtPos;
    public Vector3 objPos = new Vector3(0, 0, 0);
    public string db_path;

    [HideInInspector]
    public GameObject lastLabel;
    [HideInInspector]
    public GameObject currentLabel;
    [HideInInspector]
    public GameObject lastSphere;
    [HideInInspector]
    public GameObject currentSphere;

    private void Awake()
    {
        instance = this;
    }


    void Start()
    {
        if (PublicClass.app.app_type != "sign_acu")
            return;

        try
        {

            GameObject.Find("RightMenuManager").SetActive(false);
            GameObject.Find("XiaoRen").SetActive(false);
            GameObject.Find("CursorModeCanvas").SetActive(false);
        }
        catch (System.Exception)
        {
            Debug.Log("rightmenumanager error");
        }

        if(PublicClass.app.app_id== "AA0100000")
        {

            i.gameObject.SetActive(false);
            ii.gameObject.SetActive(true);


        }

        db_path = Vesal_DirFiles.remove_name_suffix(PublicClass.app.xml_path) + "/SignAcu.db";//AppOpera.acu_path+"SignAcu.db";
        PublicClass.Transform_temp.gameObject.SetActive(true);
        if (showTxt == null) showTxt = new GameObject().AddComponent<Text>();
        showTxt.gameObject.SetActive(false);

        
        ReadAcuTable();
       


        Debug.Log("---------链接数据库成功-----------");
        showTxt = showinfoBG.GetComponentInChildren<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        if (PublicClass.app.app_type != "sign_acu")
            return;

#if UNITY_DEBUG
        if (Input.GetKeyDown(KeyCode.T))
        {
            MakeStaticDBTable<AcupointInfo>.SetTextureDataToDB(PublicClass.filePath + "StartStop", db_path);
        }


        if (Input.GetKeyDown(KeyCode.F1))
        {
            //http://118.24.119.234:8083/vesal-jiepao-test/v1/app/xml/getTempAcupoint
            // MakeStaticDBTable<AcupointInfo>.sign_UpdateTable(PublicClass.server_ip + "v1/app/xml/getTempAcupoint", db_path);
// http://118.24.119.234:8083/vesal-jiepao-test/v1/app/xml/getTempAcupoint?type=acu
// http://118.24.119.234:8083/vesal-jiepao-test/v1/app/xml/getTempAcupoint?type=noun
            // MakeStaticDBTable<AcupointInfo>.sign_UpdateTable(PublicClass.server_ip + "v1/app/xml/getTempAcupoint?type=acu", db_path,true);
            // MakeStaticDBTable<AcuListInfo>.sign_UpdateTable(PublicClass.server_ip + "v1/app/xml/getTempAcupoint?type=noun", db_path,true);

            string db_path=PublicClass.filePath+"sign_ssp_path/StartStop_1.db";
            MakeStaticDBTable<StartStopInfo>.opera_condition(db_path,(list)=>{
                //list startstopinfo
                List<string> opera_list=new List<string>();
                foreach (var item in list)
                {
                    if(item.qr=="0")
                    {
                        opera_list.Add(item.mod_name);
                        print(item.mod_name+" info missing ");
                    }
                }
            });
        }


        if (Input.GetKeyDown(KeyCode.R))
        {

            Debug.Log("r已被按下");
            for (int i = 0; i < acuEleList.Count; i++)
            {
                Debug.Log("进入循环，查找目前记录的物体");
                if (acuEleList[i].gameObject.GetComponent<Image>().color == new Color(33 / 255.0f, 167 / 255.0f, 212 / 255.0f, 85 / 255.0f))
                {
                    camera_params str = new camera_params();
                    str.camera_pos = new vector3();
                    str.camera_pos.x = Camera.main.transform.position.x;
                    str.camera_pos.y = Camera.main.transform.position.y;
                    str.camera_pos.z = Camera.main.transform.position.z;

                    str.camera_parent_rot = new vector3();
                    str.camera_parent_rot.x = Camera.main.transform.parent.transform.eulerAngles.x;
                    str.camera_parent_rot.y = Camera.main.transform.parent.transform.eulerAngles.y;
                    str.camera_parent_rot.z = Camera.main.transform.parent.transform.eulerAngles.z;

                    str.camera_parent_pos = new vector3();
                    str.camera_parent_pos.x = Camera.main.transform.parent.transform.position.x;
                    str.camera_parent_pos.y = Camera.main.transform.parent.transform.position.y;
                    str.camera_parent_pos.z = Camera.main.transform.parent.transform.position.z;

                    acuInfoList[i].camera_params = JsonConvert.SerializeObject(str);
                    // sspInfoList[i].app_id="SA0C01001";
                    Debug.Log("记录摄像机数据成功");
                }
            }


        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            // local_db.DataService("StartStop.db");
            local_db.CreateDb(db_path);
            UpdateSSPTable();
            Debug.Log("写入摄像机数据成功");
            local_db.Close();

        }


     


#endif


        //将显示框的位置设置为物体位置的屏幕坐标
        //Debug.Log(objPos);
        //showinfoBG.transform.position = Camera.main.WorldToScreenPoint(objPos);
        txtPos = Camera.main.WorldToScreenPoint(objPos);

        showinfoBG.rectTransform.anchoredPosition = new Vector2(txtPos.x / Screen.width * ScreenData.instance.high, txtPos.y / Screen.height * ScreenData.instance.width) +new Vector2(100,50);
        //Debug.Log(showinfoBG.rectTransform.localPosition);
        // Debug.Log(showinfoBG.rectTransform.anchoredPosition);

        //Debug.Log(txtPos);
        //showinfoBG.rectTransform.pivot = new Vector2(0.5f, 0.5f);

        //Debug.Log(showinfoBG.transform.position);
        if (ChooseModel.CheckGuiRaycastObjects()) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

               
                if (acpoint_dic.ContainsKey(hit.transform.position))
                {

                    ExpPanel.SetActive(true);
                    MenuPanel.SetActive(false);
                    isListOpen = false;

 //for (int i = 0; i < acuInfoList.Count; i++)
 //               {
 //                   if(GameObject.Find(acuInfoList[i].sm_name)!=null)
 //                   ToNormalState(GameObject.Find(acuInfoList[i].sm_name).GetComponentInChildren<MeshRenderer>().material);


 //               }
                    if (lastSphere != null)
                    {
                       ToNormalState(lastSphere.GetComponentInChildren<MeshRenderer>().material);
                        //test.lastMuscle.SetActive(false);
                    }

                    //lastTrans = currentTrans;
                    //currentTrans = hit.transform;
                    objPos = hit.transform.position;
                    showinfoBG.gameObject.SetActive(true);
                    showTxt.text = acpoint_dic[hit.transform.position];
                    //ToSelectState(currentTrans.GetComponent<MeshRenderer>().material);
                    currentSphere = hit.transform.gameObject;
                    ToSelectState(hit.transform.GetComponent<MeshRenderer>().material);
                    lastSphere = currentSphere;

                    //if(lastTrans!=null)
                    //ToNormalState (lastTrans.GetComponent<MeshRenderer>().material);
                    for (int i = 0; i < acuInfoList.Count; i++)
                    {
                        if (acuInfoList[i].sm_ch_name == acpoint_dic[hit.transform.position])
                        {
                            //print(acuInfoList[i].sm_ch_name);
                            acuChName.text = acuInfoList[i].sm_ch_name;

                            acuCode.text = acuInfoList[i].iso_code;
                            acuDes.text = string.Format("<size=48><color=#00FFFF>定位</color></size>：\r\n{0}\r\n<size=48><color=#00FFFF>主治</color></size>：\r\n{1}\r\n<size=48><color=#00FFFF>操作</color></size>：\r\n{2}", acuInfoList[i].position, acuInfoList[i].cure, acuInfoList[i].handle);

                        }
                    }

                }

            }
            else
            {
                for (int i = 0; i < acuInfoList.Count; i++)
                {
                    if (GameObject.Find(acuInfoList[i].sm_name) != null)
                        ToNormalState(GameObject.Find(acuInfoList[i].sm_name).GetComponentInChildren<MeshRenderer>().material);


                }
                showinfoBG.gameObject.SetActive(false);

            }
        }


    }
    public void ToSelectState(Material mat)
    {
        mat.SetColor("_Color", Color.red);
        mat.SetFloat("_AlphaScale", 0.5f);
        mat.SetFloat("_OutLineScale", 0.2f);

    }

    public void ToNormalState(Material mat)
    {
        mat.SetColor("_Color", Color.yellow);
        mat.SetFloat("_AlphaScale", 0.6f);
        mat.SetFloat("_OutLineScale", 0f);

    }

    void UpdateSSPTable()
    {
        for (int i = 0; i < acuInfoList.Count; i++)
        { local_db.Update(acuInfoList[i]); }
    }

    //菜单切换
    public void OpenSSPList()
    {

        if (!isListOpen)
        {
            MerPanel.SetActive(true);
            isListOpen = true;
        }
        else
        {

            MerPanel.SetActive(false);
            isListOpen = false;
        }

    }


    public List<AcupointInfo> jingluoInfoList;
    public List<MerElement> merEleList=new List<MerElement>();
    void ReadAcuTable()
    {
        local_db = new DbRepository<AcupointInfo>();
        local_db.CreateDb(db_path);
        var tmpssp = local_db.Select<AcupointInfo>((tmp) =>
        {
            if(tmp.sm_type!=null)
            {
                //DebugLog.DebugLogInfo(ss_id.bone_name+" "+ss_id.main_tex+" "+ ss_id.mask_tex);
                return true;
            }
            else
            {
                return false;
            }

        });
        acuInfoList = new List<AcupointInfo>();
        jingluoInfoList = new List<AcupointInfo>();
        foreach (AcupointInfo tmp in tmpssp)
        {
            if (tmp.sm_type == "XW")
            {
                acuInfoList.Add(tmp);
            }
            else if (tmp.sm_type=="JL")
            {
                jingluoInfoList.Add(tmp);

            }

        }
        local_db.Close();
        List<string> jingmaiList=new List<string>();
        string[] ids_array=null;
        DbRepository<AcuListInfo> temp_db=new DbRepository<AcuListInfo>();
        temp_db.CreateDb(db_path);
        IEnumerable<AcuListInfo> aculist = temp_db.Select<AcuListInfo>((tmp) =>
        {
            if(tmp!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        });
        foreach (var item in aculist)
        {
            if(item.noun_no==PublicClass.app.app_id)
            {
                ids_array = item.ids.Split(',');
                string[] temp_str=item.line_list.Split(',');
                for (int i = 0; i < temp_str.Length; i++)
                {
                    jingmaiList.Add(temp_str[i]);
                }
                break;
            }
        } 
        print("jingmai  list "+jingmaiList.Count);

        temp_db.Close();
        acuPosList = new List<Transform>();

        SetSecondchildActived(PublicClass.Transform_temp,jingmaiList);

        List<string> jingmai_name_list=new List<string>();
        for (int i = 0; i < jingmaiList.Count; i++)
        {
            Transform tmp_trans=GameObject.Find(jingmaiList[i]).transform;
            int[] temp_trans=new int[tmp_trans.childCount];
            for (int j = 0; j < temp_trans.Length; j++)
            {
                jingmai_name_list.Add(tmp_trans.GetChild(j).name);
            }
        }

        for (int i = 0; i < acuInfoList.Count; i++)
        {
            if(jingmai_name_list.Contains(acuInfoList[i].sm_name))
            {
                //读取小球位置生成小球
                Transform tran = GameObject.Find(acuInfoList[i].sm_name).transform;
                acuPosList.Add(tran);
                GameObject gobj = Instantiate(sphere, tran);
                gobj.transform.localPosition = Vector3.zero;
                gobj.transform.localScale = gobj.transform.localScale * 0.35f;
                gobj.GetComponent<SphereCollider>().radius = 0.7f;
                acpoint_dic.Add(tran.position, acuInfoList[i].sm_ch_name);
            
                //读取下方菜单预制体生成菜单
                GameObject tempMark = Instantiate(Resources.Load<GameObject>("Prefab/ACUElement"));
                // DebugLog.DebugLogInfo("读取下方菜单预制体");
                tempMark.transform.SetParent(acuParent.transform);
                tempMark.transform.localScale = Vector3.one;
                tempMark.transform.localPosition = new Vector3(tempMark.transform.localPosition.x, tempMark.transform.localPosition.y, 1);
                Debug.Log(tempMark);
                tempMark.GetComponent<AcuElement>().SetData(acuInfoList[i]);
            
                tempMark.GetComponentInChildren<Text>().text = acuInfoList[i].sm_ch_name;
                acuEleList.Add(tempMark.GetComponent<AcuElement>());
            }
        }

        for(int j=0;j<jingluoInfoList.Count;j++)
        {
            GameObject tempMark = Instantiate(Resources.Load<GameObject>("Prefab/MerElement"));
            // DebugLog.DebugLogInfo("读取下方菜单预制体");
            tempMark.transform.SetParent(merParent.transform);
            tempMark.transform.localScale = Vector3.one;
            tempMark.transform.localPosition = new Vector3(tempMark.transform.localPosition.x, tempMark.transform.localPosition.y, 1);
            Debug.Log(tempMark);
            tempMark.GetComponent<MerElement>().SetData(jingluoInfoList[j]);

            tempMark.GetComponentInChildren<Text>().text = jingluoInfoList[j].sm_ch_name;
            merEleList.Add(tempMark.GetComponent<MerElement>());
            


        }
        Debug.Log(merEleList.Count);
    }

    public void SetSecondchildActived(Transform trans,List<string> nameList)
    {
        Transform[] objects=new Transform[trans.GetChild(0).childCount];
        for (int i = 0; i < trans.GetChild(0).childCount; i++)
        {
            objects[i]=trans.GetChild(0).GetChild(i);
        }
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < objects.Length; i++)
        {
            for (int j = 0; j < nameList.Count; j++)
            {
                if(objects[i].name==nameList[j])
                {
                    objects[i].gameObject.SetActive(true);
                }
            }
        }
    }

}