using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using Assets.Scripts.Public;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using VesalCommon;
using UnityEngine.EventSystems;
using XLua;
using Newtonsoft.Json.Linq;

[Hotfix]
public class ChooseModel : MonoBehaviour
{

    public static EventSystem eventsystem;
    public static GraphicRaycaster RaycastInCanvas;

    public GameObject model_ui;
    public GameObject sign_ui;
    public GameObject ssp_ui;
    public GameObject ssp_ctrl;
    public GameObject sign_new_ctrl;
    public GameObject acu_ctrl;

    public LoadAndInit loadAndInit;

    GameObject obj;
    SceneModels temp;
    public static ChooseModel instance;

    bool hasNewModelLoaded = false;
    public bool isShowSignModel = false;
    public bool isShowTissueModel = false;
    public bool isShowNewSignModel = false;
    public bool isSelectCFD = false;
    private void Awake()
    {
        instance = this;
    }
    [LuaCallCSharp]
    void Start()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "WeiKePlayer":
            case "totalScence":
            case "PPTPlayer":
            PublicClass.appstate = App_State.Running;
                UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
                {
                    name = "title",
                    callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
                });
                break;
            case "MotorAnatomy":
                obj = new GameObject("SceneModels");
                temp = obj.AddComponent<SceneModels>();
                DebugLog.DebugLogInfo("加载模型管理器------------------");
                InitData(PublicClass.app.app_id);
                break;
            default:
                // model_ui.SetActive(false);
                // sign_ui.SetActive(false);
                if (ssp_ui != null)
                {
                    RaycastInCanvas = ssp_ui.GetComponent<GraphicRaycaster>();
                    ssp_ui.SetActive(false);
                }
                obj = new GameObject("SceneModels");
                temp = obj.AddComponent<SceneModels>();
                DebugLog.DebugLogInfo("加载模型管理器------------------");
                InitData(PublicClass.app.app_id);


                Debug.Log("JumpState:" + PublicClass.app.JumpState);

                //if (PublicClass.app.JumpState == "return")
                //    StateData.Instance.isReturn = true;

             //PublicClass.app.signModelName = "CFD008,CFD007";
           //     PublicClass.app.tissueModelName = "股直肌";
                if (!string.IsNullOrEmpty(PublicClass.app.signModelName))
                {
                    if (PublicClass.app.app_type == "model" || PublicClass.app.app_type == "medical" || PublicClass.app.app_type == "acu")
                        isShowSignModel = true;
                    else if (SceneManager.GetActiveScene().name == "UI_sign_all")
                        isShowNewSignModel = true;
                    else if (SceneManager.GetActiveScene().name == "CFD")
                        isSelectCFD = true;
                }

                if(!string.IsNullOrEmpty(PublicClass.app.tissueModelName)&&( PublicClass.app.app_type == "model" || PublicClass.app.app_type == "medical" || PublicClass.app.app_type == "acu"))
                {
                    isShowTissueModel = true;
                }          
                break;
        }
        if (AsyncLoadModel.instance == null)
        {
            AsyncLoadModel.instance = new AsyncLoadModel();
        }
    }
    //bool isStartSC=false;
    bool isLastScDown = true;
    bool isSendTittle = true;
  //  bool isLoad = true;
    [LuaCallCSharp]
    void Update()
    {
        //UnityEngine.Debug.Log(PublicClass.appstate);
        switch (PublicClass.appstate)
        {
            case App_State.Init:
                break;
            case App_State.LoadAsset:
                StartAssetLoad();
                break;
            case App_State.Init_2:
                //if (PublicClass.Quality == Run_Quality.GOOD)
                //{
                    if (hasNewModelLoaded && isLastScDown)
                    {
                        //if (temp.init2())
                        isLastScDown = false;
                        hasNewModelLoaded = false;
                        //Debug.LogError("StartCoroutine(temp.init2(isInitDown))");
                        if (load_count == load_list.Count)
                            StartCoroutine(temp.init2(isInitDown, true));
                        else
                            StartCoroutine(temp.init2(isInitDown));
                    }
                //}
                //else {
                //    //Debug.LogError(hasAssetLoadCompleted);
                //    if (hasAssetLoadCompleted)
                //    {
                //        if (temp.init2()) {
                //            PublicClass.appstate = App_State.Init_3;
                //        }
                //    }
                //    hasAssetLoadCompleted = false;
                //}
               
                break;
            case App_State.Init_3:
                Debug.Log("init3处理。。。。。");
                
                StartCoroutine(temp.init3(PublicClass.app.app_id, AsyncLoadDown));
                PublicClass.appstate = App_State.Init_4;
                break;
            case App_State.Init_4:
                break;
            case App_State.Running:

                //  PublicClass.appstate = App_State.Running;
                //if (isSendTittle)
                //{
                //    isSendTittle = false;                                   
                //    Debug.Log("title");
                //    UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
                //    {
                //        name = "title",                      
                //        callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
                //    });
                //}

               // if (isLoad)
                //{
                //    if (isShowSignModel ||isShowTissueModel)
                //    {
                //        isLoad = false;
                //        break;
                //    }
                //    String path = PublicClass.filePath + "StateSave/" + PublicClass.app.app_id + ".Json";
                //    Debug.Log(File.Exists(path));
                //    isLoad = false;
                //    if (File.Exists(path))
                //    {
                //        LoadBookmark(path);
                //    }
                //    isLoad = false;
                //}

                if (isShowTissueModel)
                {
                    if (showTissueModel())
                    {
                        Debug.Log("111111111111111111111111");
                    }
                    else
                         if (isShowSignModel)
                    {
                        showSignModel();
                    }
                }
                else
                {
                    if (isShowSignModel)
                    {
                        showSignModel();
                    }
                }
                if (isSelectCFD)
                {
                    isSelectCFD = false;
                    CFDControll.instance.ChooseEvent(PublicClass.app.signModelName);
                }

                if (isShowNewSignModel)
                {
                    Debug.Log(PublicClass.app.app_type);
                    ShowNewSignModel();
                }
             
                break;
        }
    }
    private string ToSixteen(string input)
    {
        char[] values = input.ToCharArray();
        string end = string.Empty;
        foreach (char letter in values)
        {
            int value = Convert.ToInt32(letter);
            string hexoutput = string.Format("{0:X}", value); //0 表示占位符 x或X表示十六进制
            end += hexoutput + "_";
        }
        end = end.Remove(end.Length - 1);
        return end;
    }
    [LuaCallCSharp]
    void LoadBookmark(string path)
    {
        saveSate.instance.SetMarkData(path);
    }
    [LuaCallCSharp]
    bool istoulu()
    {

        string a = ((TextAsset)Resources.Load("Txt/toulu")).text;
        string[] b = a.Split(',');
        string name = PublicClass.app.signModelName;
        foreach (string c in b)
        {
            if (c == name)
                return true;
        }
        return false;
    }
    [LuaCallCSharp]
    bool isgutou(string names)
    {
        Debug.Log(name);
        List<Model> guge = Guxue.GetComponent<RightMenuItem>().getguge();
        foreach (Model tmp in guge)
        {
            if (names == tmp.name)
                return true;
        }
        return false;
    }
    GameObject Guxue;
    [LuaCallCSharp]
    bool showSignModel()
    {
        int num=-1;
        isShowSignModel = false;
        DbRepository<GetSubModel> local_db2;
        //local_db2 = new DbRepository<MotorAnatomy_submodel>();
        local_db2 = new DbRepository<GetSubModel>();
        local_db2.DataService("vesali.db");
        Debug.Log(PublicClass.app.signModelName);
        var tmpssp2 = local_db2.Select<GetSubModel>((tmp) =>
        {
            if (tmp.chName == PublicClass.app.signModelName)
            {
                Debug.Log(tmp.chName);
                //DebugLog.DebugLogInfo(ss_id.bone_name+" "+ss_id.main_tex+" "+ ss_id.mask_tex);
                return true;
            }
            else
            {
                return false;
            }
        });
        List<GetSubModel> texInfoList = new List<GetSubModel>();
        foreach (GetSubModel tmp in tmpssp2)
        {
            texInfoList.Add(tmp);
        }
        local_db2.Close();
        if (texInfoList.Count == 0)
            return false;

        for (int i = 0; i < texInfoList.Count; i++)
        {
            Debug.Log(texInfoList[i].name);

            if (SceneModels.instance.IsChooseModelByName(texInfoList[i].name))
            {
                num = i;
                break;
            }
        }
        Guxue = GameObject.Find("骨学");
        if (Guxue != null&&!istoulu())
        {
            if (isgutou(texInfoList[num].name))
            {
                SceneModels.instance.ChooseModelByName(texInfoList[num].name);
                XT_AllButton.Instance.OpenMultipleModel();
                List<Model> guge = Guxue.GetComponent<RightMenuItem>().getguge();
                foreach (Model tmp in guge)
                {
                    SceneModels.instance.ChooseModelByName(tmp.gameObject.name);
                }
                XT_AllButton.Instance.ISO();
                foreach (Model tmp in guge)
                {
                    SceneModels.instance.ChooseModelByName(tmp.gameObject.name);
                }
                XT_AllButton.Instance.OpenMultipleModel();
                SceneModels.instance.ChooseModelByName(texInfoList[num].name);
            }
            else
            {
                XT_AllButton.Instance.OpenMultipleModel();
           
                List<Model> guge = Guxue.GetComponent<RightMenuItem>().getguge();
                foreach (Model tmp in guge)
                {
                    SceneModels.instance.ChooseModelByName(tmp.gameObject.name);
                }
                XT_AllButton.Instance.ISO();
                foreach (Model tmp in guge)
                {
                    SceneModels.instance.ChooseModelByName(tmp.gameObject.name);
                }
                XT_AllButton.Instance.OpenMultipleModel();
                SceneModels.instance.ChooseModelByName(texInfoList[num].name);
            }
        }
        else
        {          
            XT_AllButton.Instance.ISO();
        }
        Debug.Log(texInfoList[0].name);
        XT_MouseFollowRotation.instance.To_360();
      //  SceneModels.instance.ChooseModelByName(texInfoList[0].name);
        XT_TouchContorl.Instance.GiveUiValue_new(SceneModels.instance.GetModel(texInfoList[num].name));
        return true;
    }
    [LuaCallCSharp]
    void ShowNewSignModel()
    {
        isShowNewSignModel = false;
        if (PublicClass.app.app_type == "sign_new")
        {
            foreach (SignNewElement tmp in SignNewTest.instance.sgnEleList)
            {
                if (tmp.gameObject.name == PublicClass.app.signModelName)
                {
                    tmp.Start();
                   //  StartCoroutine(onclick(tmp));
                    tmp.OnClick();
                    SignNewTest.instance.sgnExpPanel.SetActive(true);
                    break;
                }
            }
        }else if(PublicClass.app.app_type == "sign_acu")
        {
            if (PublicClass.app.app_id == "AA0100000")
            {
                foreach (MerElement tmp in AcupointTest.instance.merEleList)
                {
                    if (tmp.gameObject.name == PublicClass.app.signModelName)
                    {
                        tmp.Start();
                        tmp.OnClick();
                        AcupointTest.instance.ExpPanel.SetActive(true);
                        break;
                    }
                }
            }
            else
            {
                foreach (AcuElement tmp in AcupointTest.instance.acuEleList)
                {
                    if (tmp.gameObject.name == PublicClass.app.signModelName)
                    {
                        tmp.Start();
                        tmp.OnClick();
                        AcupointTest.instance.ExpPanel.SetActive(true);
                        break;
                    }
                }
            }
        }
      
    }
    IEnumerator onclick(SignNewElement tmp)
    {
      //  Interaction.instance.setParamValue3();
        tmp.OnClick();
        yield return new WaitForSeconds(1);
        SignNewTest.instance.sgnExpPanel.SetActive(true);
        Interaction.instance.setParamValue3();
        Debug.Log(Camera.main.transform.parent.eulerAngles);
    }

    [LuaCallCSharp]
    bool showTissueModel()
    {
        isShowTissueModel = false;
        DbRepository<GetStructList> local_db2;
        //local_db2 = new DbRepository<MotorAnatomy_submodel>();
        local_db2 = new DbRepository<GetStructList>();
        local_db2.DataService("vesali.db");
        Debug.Log(PublicClass.app.signModelName);
        GetStructList scope_Ie = local_db2.SelectOne<GetStructList>((tempNo) =>
        {
            if (tempNo.nounName == PublicClass.app.tissueModelName)
            {
                Debug.Log(tempNo.nounName);
                //DebugLog.DebugLogInfo(ss_id.bone_name+" "+ss_id.main_tex+" "+ ss_id.mask_tex);
                return true;
            }
            else
            {
                return false;
            }
        });


        List<GetStructList> texInfoList = new List<GetStructList>();
       
        List<string> initList = SceneModels.instance.getListByObj(scope_Ie);//getListByNo(nounNo);
        if (initList.Count == 0)
            return false;
        Vector3 campos = PublicTools.Str2Vector3(scope_Ie.camPos);
        //  Camera.main.transform.parent.localPosition = PublicTools.Str2Vector3(texInfoList[0].camParentPos);

        Debug.Log(PublicTools.Str2Vector3(scope_Ie.camPos));
        Debug.Log(initList.Count);
        local_db2.Close();
        bool ishave =false;
        XT_AllButton.Instance.OpenMultipleModel();
        foreach (string tmp in initList)
        {
           if( SceneModels.instance.IsChooseModelByName(tmp))
                ishave =true;
        }
        if (ishave)
        {
            // XT_AllButton.Instance.TranslucentOther();
            XT_AllButton.Instance.ISO();
            foreach (string tmp in initList)
            {
                SceneModels.instance.ChooseModelByName(tmp);
            }
            XT_AllButton.Instance.OpenMultipleModel();

            if (campos != null)
            {
                Camera.main.transform.localPosition = campos;
                Interaction.instance.setParamValue2();
            }
        }
        //  XT_MouseFollowRotation.instance.To_360();
        isShowSignModel = false;
        return true;
    }

    [LuaCallCSharp]
    void isInitDown(bool result)
    {
        if (result)
        {
            if (PublicClass.app.app_type == "animation")
            {
                GameObject tmp_obj = PublicClass.Transform_parent.GetComponentInChildren<Model>().gameObject;
                loadAndInit.Init("vesali.db", tmp_obj, PublicClass.TimelineFilePath +PublicClass.app.app_id+ "_1.timeline");
                PublicClass.appstate = App_State.Running;
                Debug.Log("tittle");
                UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
                {
                    
                    name = "title",
                    callBack = (data) => { DebugLog.DebugLogInfo("message : " + data); }
                });
            }
            else
            {
                PublicClass.appstate = App_State.Init_3;
            }
        }
            isLastScDown = true;
    }
    [LuaCallCSharp]
    void AsyncLoadDown()
    {
        AsyncLoadModel.instance.close_message();
        AsyncLoadModel.instance.toggleOther(true);
        PublicClass.appstate = App_State.Running;      
    }

    //public void loadModel()
    //{
    //    temp.destoryTempParent();
    //    DestroyImmediate(obj);
    //    obj = new GameObject("SceneModels");
    //    temp = obj.AddComponent<SceneModels>();
    //    temp.Init_SceneModels(PublicClass.app.app_id);
    //}
    [LuaCallCSharp]
    public static bool CheckGuiRaycastObjects()
    {

        PointerEventData eventData = new PointerEventData(eventsystem);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();
        if (RaycastInCanvas == null && PPTPlayer.ModelController.tmp_sign_new_cvs !=null) {
            RaycastInCanvas = PPTPlayer.ModelController.tmp_sign_new_cvs.GetComponent<GraphicRaycaster>();
        }
        RaycastInCanvas.Raycast(eventData, list);
        return list.Count > 0;

    }


    //加载人体展示模型
    [LuaCallCSharp]
    public void InitData(string nounNo)
    {
        //0 模型
        switch (PublicClass.app.app_type)
        {
            case "animation":
                    temp.Init_SceneModels(nounNo);
                PublicClass.appstate = App_State.LoadAsset;
                break;
            case "model":
            case "acu":
            case "cfd":
            case "medical":
                {
                    model_ui.SetActive(true);
                    PublicClass.currentModle = ModleChoose.MainModel;
                    temp.Init_SceneModels(nounNo);
                    // temp.Init_SceneModels("SA0601013");
                    temp.openTempParent();
                    PublicClass.appstate = App_State.LoadAsset;
                    Camera.main.SendMessage("toggleOther", false);
                }
                break;
            case "demo":
                {
                    model_ui.SetActive(true);
                    PublicClass.currentModle = ModleChoose.MainModel;
                    temp.Init_SceneModels(nounNo);
                    // temp.Init_SceneModels("SA0601013");
                    temp.openTempParent();
                    PublicClass.appstate = App_State.LoadAsset;
                    Camera.main.SendMessage("toggleOther", false);
                }
                break;
            case "sign":
                {
                    sign_ui.SetActive(true);
                    PublicClass.currentModle = ModleChoose.SignModel;
                    this.GetComponent<ShowSignModel>().Load_ps(nounNo, PublicClass.app.xml_path);
                }
                break;
            case "sign_ssp":
                {
                    PublicClass.currentModle = ModleChoose.MainModel;
                    temp.Init_SceneModels(nounNo, false);
                    // temp.Init_SceneModelsByList(new List<string>(){""},MakeStaticDBTable.GetInintModelList(AppOpera.sign_ssp_path+PublicClass.app.ab_path), null, null, null,false);

                    ssp_ui.SetActive(true);
                    //this.gameObject.GetComponent<XT_TouchContorl>().enabled = false;
                    ssp_ctrl.SetActive(true);
                    GameObject.Find("XiaoRen").SetActive(false);
                    ssp_ui.SetActive(true);
                }
                break;
            case "sign_new":
                {
                    PublicClass.currentModle = ModleChoose.MainModel;
                    temp.Init_SceneModels(nounNo, false);
                    ssp_ui.SetActive(true);
                    //this.gameObject.GetComponent<XT_TouchContorl>().enabled = false;
                    sign_new_ctrl.SetActive(true);
                }
                break;
            case "sign_acu":
                {
                    PublicClass.currentModle = ModleChoose.MainModel;
                    //加载公共信息库
                    temp.Init_SceneModels(nounNo);
                    ssp_ui.SetActive(true);
                   // this.gameObject.GetComponent<XT_TouchContorl>().enabled = false;
                    temp.openTempParent();
                    PublicClass.appstate = App_State.LoadAsset;
                    // print(AppOpera.acu_path+PublicClass.app.ab_path);
                    //穴位私有信息库
                    // List<string> submodel_list=new List<string>();
                    // submodel_list.AddRange(GetInintModelList(AppOpera.acu_path+"SignAcu.db"));
                    // temp.Init_SceneModelsByList(submodel_list,submodel_list, null, null,false);
                    // this.gameObject.GetComponent<XT_TouchContorl>().enabled = false;
                    // //GameObject.Find("XiaoRen").SetActive(false);
                    acu_ctrl.SetActive(true);
                }
                break;
            default:
                break;
        }
    }


    //获取穴位私有模型列表
    [LuaCallCSharp]
    List<string> GetInintModelList(string db_path)
    {
        DbRepository<AcupointInfo> local_db = new DbRepository<AcupointInfo>();
        // local_db.CreateDb(AppOpera.sign_ssp_path+"StartStop.db");
        local_db.CreateDb(db_path);
        var tmpssp = local_db.Select<AcupointInfo>((mod_name) =>
           {
               if (mod_name != null)
               {
                   return true;
               }
               else
               {
                   return false;
               }

           });

        List<string> sspInfoList = new List<string>();

        foreach (AcupointInfo tmp in tmpssp)
        {
            if (tmp != null)
            {
                sspInfoList.Add(tmp.sm_name);
            }

        }
        return sspInfoList;
    }

    List<Download_Vesal_info> load_list;
    public GameObject showInfo;
    public Text text;
    bool isLoading = true;
    // Use this for initialization
    [LuaCallCSharp]
    void StartAssetLoad()
    {
        load_count = 0;
        cacaulte_load_list();
        if (load_list.Count > 0)
        {
            StartCoroutine(LoadPrefabModel(PublicClass.filePath + load_list[0].name, Vesal_DirFiles.remove_name_suffix(load_list[0].name), count_load));
            AsyncLoadModel.instance.show_message("<size=26>正在加载模型，请稍等...</size>\r\n<size=30>已经完成</size>" + (load_count / (float)load_list.Count).ToString("00%"));
            try
            {
            AsyncLoadModel.instance.img.fillAmount = 0;
            }
            catch (Exception)
            {
            }
            //Debug.Log(AsyncLoadModel.instance.img.fillAmount);
            // Debug.Log(load_count / (float)load_list.Count);
        }
        else
        {
            AsyncLoadModel.instance.show_message("<size=26>正在加载模型，请稍等...</size>\r\n<size=30>已经完成</size>" + (0.9).ToString("00%"));
            try
            {
            AsyncLoadModel.instance.img.fillAmount = 0.9f;
            }
            catch (Exception)
            {
            }
            hasNewModelLoaded = true;
        }
        PublicClass.appstate = App_State.Init_2;
    }
    [LuaCallCSharp]
    void cacaulte_load_list()
    {
        load_list = new List<Download_Vesal_info>();
        if (PublicClass.app.app_type == "demo" )
            return;
        string downloaded_str = "";
        foreach (Download_Vesal_info download in PublicClass.Asset_loaded_list)
            downloaded_str += download.name;

        Debug.Log("loaded ablist---:" + downloaded_str);

        //if (PublicClass.app.ab_list == null || (PublicClass.app.ab_list.Trim() == ""))
        //{
            string ab="";
            List<string> ablist=new List<string>();
            foreach (string model in temp.scope_scene_model_name)
            {
                if (!PublicClass.Model_AB_dic.ContainsKey(model))
                {
                    Debug.Log("the model not founded in ab !!!!!!!!!!!!!!!"+model);
                    continue;
                }
                string modelab=PublicClass.Model_AB_dic[model];
                if (ab != modelab)
                {
                    if (!ablist.Contains(modelab))
                    {
                        ab = modelab;
                        ablist.Add(modelab);
                        Download_Vesal_info item = new Download_Vesal_info();
                        item.type = "AB";
                        item.name =  ab + ".assetbundle";
                        if(!downloaded_str.Contains(item.name))
                            load_list.Add(item);
                    }
                }
            }
            //foreach (Download_Vesal_info download in PublicClass.total_load_list)
            //{
            //    if (!downloaded_str.Contains(Vesal_DirFiles.remove_name_suffix(download.name)))
            //    {
            //        load_list.Add(download);
            //    }
            //}

        //}
        //else
        //{
        //    Debug.Log("app ablist---:" + PublicClass.app.ab_list);
        //    string[] ab_files = PublicClass.app.ab_list.Split(',');
        //    string ablist_str = "";
        //    Debug.Log("app ablist---number:" + ab_files.Length);
        //    for (int k = 0; k < ab_files.Length; k++)
        //    {
        //        //if (PublicClass.low_res_ablist.Contains(ab_files[k]))
        //        //    ab_files[k] = "s_"+ab_files[k];
        //        //ablist_str += ab_files[k];
        //        if (!downloaded_str.Contains(ab_files[k] + ".assetbundle"))
        //        {
        //            bool founded = false;
        //            foreach (Download_Vesal_info download in PublicClass.total_load_list)
        //            {
        //                if (download.name.Contains(ab_files[k] + ".assetbundle"))
        //                {
        //                    founded = true;
        //                    load_list.Add(download);
        //                    break;
        //                }
        //            }
        //            if (!founded)
        //            {
        //                Debug.Log("ab list error:" + ab_files[k]);
        //            }
        //        }
        //    }
        //}
            //卸载掉不用的ab包模型数据
            string ab_list_string = "";
            foreach (string str in ablist)
            {
                ab_list_string += (str + ",");
            }

        if ((PublicClass.Quality == Run_Quality.POOL) && (load_list.Count + PublicClass.Asset_loaded_list.Count() >= PublicClass.MAX_Ab_num))
        {
            bool unload = false;
            for (int k = PublicClass.Asset_loaded_list.Count() - 1; k >= 0; k--)
            {
                Download_Vesal_info tmp = PublicClass.Asset_loaded_list[k];

                if (!PublicTools.isTargetInSourceList(Vesal_DirFiles.remove_name_suffix(tmp.name), ab_list_string))
                {
                    Debug.Log("POOL Destroy::" + tmp.name);
                    try
                    {
                        DestroyImmediate(tmp.instance);
                        //Destroy(temp.source);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    Resources.UnloadUnusedAssets();
                    PublicClass.Asset_loaded_list.RemoveAt(k);
                    unload = true;
                }
            }
            if (unload)
                ReadModelAgain();
        }


        

    }

    // Update is called once per frame
    int load_count = 0;
    bool load_error_flag = false;
    //加载完毕回调
    [LuaCallCSharp]
    void count_load()
    {
        if (load_error_flag == false)
            PublicClass.Asset_loaded_list.Add(load_list[load_count]);
        load_count++;
        //if (PublicClass.Quality == Run_Quality.GOOD) {
            hasNewModelLoaded = true;
        //}
        //        Camera.main.gameObject.GetComponent<ChooseModel>().loadModel();
        if (load_list.Count > load_count)
        {
            AsyncLoadModel.instance.show_message("<size=26>正在加载模型，请稍等...</size>\r\n<size=30>已经完成</size>" + Math.Min((load_count / (float)load_list.Count), 0.9f).ToString("00%"));
            StartCoroutine(LoadPrefabModel(PublicClass.filePath + load_list[load_count].name, Vesal_DirFiles.remove_name_suffix(load_list[load_count].name), count_load));
            try
            {
            AsyncLoadModel.instance.img.fillAmount = Math.Min((load_count / (float)load_list.Count), 0.9f);
            }
            catch (Exception)
            {
            }
        }
        if (load_count == load_list.Count)
        {
            Debug.Log("加载完毕");
            AsyncLoadModel.instance.show_message("<size=26>正在加载模型，请稍等...</size>\r\n<size=30>已经完成</size>" + (0.9f).ToString("00%"));

            try
            {

            AsyncLoadModel.instance.img.fillAmount = 0.9f;
            }
            catch (Exception)
            {
            }
            //AsyncLoadModel.instance.close_message();
        }

    }
    [LuaCallCSharp]
    IEnumerator LoadPrefabModel(string path, string name, Action count_load = null)
    {
        load_error_flag = false;
        //防止卡死
        yield return new WaitForEndOfFrame();

        DebugLog.DebugLogInfo("00FF00","assetbundle loading ---" + path + name);

        string temp_path = PublicClass.tempPath + name + "temp.dat";
        AssetBundleCreateRequest assetBundleCreateRequest = null;
        try
        {
            Vesal_DirFiles.GetAbfile2(path, temp_path);
            assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(temp_path);

            //assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(Vesal_DirFiles.GetAbfile(path, temp_path));
        }
        catch (Exception ex)
        {
            load_error_flag = true;
            vesal_log.vesal_write_log("assetbundle load error in :" + name);
            vesal_log.vesal_write_log("error message:" + ex.Message);
        }


        if (assetBundleCreateRequest != null && load_error_flag == false)
        {
            yield return assetBundleCreateRequest;

            GameObject CurrentObj = null;
            GameObject realObj = null;
            AssetBundleRequest abr = null;
            try
            {
                abr = assetBundleCreateRequest.assetBundle.LoadAssetAsync(name, typeof(GameObject));


            }
            catch (Exception ex)
            {
                load_error_flag = true;
                vesal_log.vesal_write_log("assetbundle load error in :" + name);
                vesal_log.vesal_write_log("error message:" + ex.Message);
            }
            yield return abr;
            if (abr != null && abr.asset != null)
            {
                CurrentObj = (GameObject)abr.asset;
                realObj = Instantiate(CurrentObj);
            }
            else
            {
                load_error_flag = true;
                vesal_log.vesal_write_log("assetbundle load error in :" + name);
            }
            if (CurrentObj != null && realObj != null & load_error_flag == false)
            {
                realObj.name = realObj.name.Replace("(Clone)", "");

                ReadModel(realObj);

                Transform Transform_parent = PublicClass.Transform_parent;
                realObj.transform.SetParent(Transform_parent);

                assetBundleCreateRequest.assetBundle.Unload(false);
                //Transform_parent.gameObject.SetActive(false);

                load_list[load_count].source = CurrentObj;
                load_list[load_count].instance = realObj;
                if (File.Exists(temp_path))
                {
                    try
                    {
                        Vesal_DirFiles.DelFile(temp_path);
                    }
                    catch { }
                }
            }
        }
        if (count_load != null)
        {
            count_load();
        }
    }

    [LuaCallCSharp]
    public void ReadModelAgain()
    {

        PublicClass.id_model_dic.Clear();

        List<Model> tmpModelList = new List<Model>();
        for (int i = 0; i < PublicClass.AllModels.Count; ++i)
        {
            try
            {
                if (PublicClass.AllModels[i].gameObject.name != "")
                {
                    tmpModelList.Add(PublicClass.AllModels[i]);
                }
            }
            catch
            {
            }
        }
        PublicClass.AllModels.Clear();
        Model[] models = tmpModelList.ToArray();
        //Model[] models = PublicClass.Transform_parent.GetComponentsInChildren<Model>();
        DebugLog.DebugLogInfo(models.Length + " 重新刷新模型-ID表  总模型长度");
        for (int j = 0; j < models.Length; j++)
        {
            if (!PublicClass.id_model_dic.ContainsKey(models[j].name))
            {
                PublicClass.AllModels.Add(models[j]); //  A
                PublicClass.id_model_dic.Add(models[j].name, PublicClass.AllModels.Count - 1);
            }
            else
            {
                //                vesal_log.vesal_write_log("founed error model" + models[j].name);
            }
        }
    }

    [LuaCallCSharp]
    public void ReadModel(GameObject parent)
    {
        Model[] models = parent.GetComponentsInChildren<Model>();
        DebugLog.DebugLogInfo(models.Length + "  模型长度  "+parent.name);
        for (int j = 0; j < models.Length; j++)
        {
            if (!PublicClass.id_model_dic.ContainsKey(models[j].name))
            {
                PublicClass.AllModels.Add(models[j]); //  A
                PublicClass.id_model_dic.Add(models[j].name, PublicClass.AllModels.Count - 1);
                models[j].BecomeHide();
            }
            else
            {
                vesal_log.vesal_write_log("founed error model" + models[j].name);
            }
        }
    }

    [LuaCallCSharp]
    public void SaveCameraPara()
    {

        //Vector3 camera_pos, camera_parent_rot, camera_parent_pos;
        //camera_pos = new Vector3();
        //camera_pos.x = Camera.main.transform.position.x;
        //camera_pos.y = Camera.main.transform.position.y;
        //camera_pos.z = Camera.main.transform.position.z;

        //camera_parent_rot = new Vector3();
        //camera_parent_rot.x = Camera.main.transform.parent.transform.eulerAngles.x;
        //camera_parent_rot.y = Camera.main.transform.parent.transform.eulerAngles.y;
        //camera_parent_rot.z = Camera.main.transform.parent.transform.eulerAngles.z;

        //camera_parent_pos = new Vector3();
        //camera_parent_pos.x = Camera.main.transform.parent.transform.position.x;
        //camera_parent_pos.y = Camera.main.transform.parent.transform.position.y;
        //camera_parent_pos.z = Camera.main.transform.parent.transform.position.z;

        //var local_db = new DbRepository<GetStructList>();
        ////获取名词范围
        //local_db.DataService("vesali.db");
        //GetStructList tmpIe = local_db.SelectOne<GetStructList>((tempNo) =>
        //{
        //    if (tempNo.nounNo == PublicClass.app.app_id)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //});

        //if (tmpIe == null)
        //{
        //}
        //else
        //{
        //    tmpIe.camPos = PublicTools.Vector32Str(camera_pos);
        //    tmpIe.camParentRot = PublicTools.Vector32Str(camera_parent_rot);
        //    tmpIe.camParentPos = PublicTools.Vector32Str(camera_parent_pos);
        //    local_db.Update(tmpIe);
        //}
        //local_db.Close();

    }

}
