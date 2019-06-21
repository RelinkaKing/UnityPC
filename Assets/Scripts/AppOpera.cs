using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Model;
using Assets.Scripts.Network;
using Assets.Scripts.Public;
using Assets.Scripts.Repository;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using vesal_network;
using VesalCommon;

public enum AppOperState
{
    Init,
    Download,
    Downloading,
    AssetLoad,
    AssetLoading,
    UpdateData,
    UpdataDating,
    UnZip,
    UnZiping,
    Apprun,
    LoadScene,
    Appruning,
    Loop
}

//解析app 参数，分发场景
public class AppOpera : MonoBehaviour
{
    public GameObject[] BlackPanels;
    public static string model_path;
    public static string sign_path;
    public static string sign_ssp_path;
    public static string acu_path;
    public static string WeiKePlayer_path;
    public static string QA_path;
    public static string game_path;
    //下载进度条
    public ShowProgress show_progress;

    public static AppOperState OperaStatus = AppOperState.Init;

    App this_app = null;
    HttpDownLoad http = null;
    // void Awake()
    // {
    // 	PublicClass.appOpera=this;
    // }
    long timer = 0;
    private void OnDestroy()
    {
        InternalSocketMananger.InternalMessageEvent -= Get_app;
        UnityMessageManager.InternalMessageEvent -= Get_app;
        if (show_progress != null)
        {
            show_progress.CloseProgress();
            //Destroy(show_progress.gameObject);
        }
    }
    private void OnEnable()
    {
        if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.plugin)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        foreach (GameObject obj in BlackPanels)
        {
            obj.SetActive(false);
        }
    }

    private void Awake()
    {
        InternalSocketMananger.InternalMessageEvent += Get_app;
        UnityMessageManager.InternalMessageEvent += Get_app;
    }

#if UNITY_EDITOR
#endif
    static bool firstTest = true;
    void Start()
    {
        OperaStatus = AppOperState.Init;
        DebugLog.DebugLogInfo("-------------------------------进入场景分发器---------------------------------------");
        //创建资源文件夹
        Create_assets_path();

        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            SceneManager.LoadScene("WeiKePlayer");
            return;
        }
        show_progress = Unity_Tools.Get_scene_instance<ShowProgress>("LoadingCanvas_pc");
#if UNITY_EDITOR
        if (firstTest)
        {
            //StartCoroutine(App_tool.testMethod("游戏", "2", "game", "GA0101001", Get_app));//SA0101047//SA0000000//RA0100001//SA0C02001//

            // StartCoroutine(App_tool.testMethod("经络穴位", "24", "acu", "AA0100012N", Get_app));
            //StartCoroutine(App_tool.testMethod_acu("acu", "AA0100000N", Get_app));
            //StartCoroutine(App_tool.testMethod("人体构造", "44", "medical", "RA0801001", Get_app));
            //Get_app(App_tool.testMethod());
            //StartCoroutine(App_tool.testMethod_wk_lz("微课课程", "13", "microlesson", "WK0000001", Get_app));
            //StartCoroutine(App_tool.testMethod("肌肉动画", "29", "equip_drill", "MAO10001", Get_app));
            //StartCoroutine(App_tool.testMethod("测验练习", "exam", "qa001", Get_app));
            //StartCoroutine(App_tool.testMethod("人体构造", "2", "model", "SA0A01001", Get_app));//SA0201007//SA0202010
            // StartCoroutine(App_tool.testMethod("人体构造", "2", "animation", "DA0100039", Get_app));
            // StartCoroutine(App_tool.testMethod("微课课程", "2", "microlesson", "HMFWK001", Get_app));//HMFWK002
            //  StartCoroutine(App_tool.testMethod("人体构造", "44", "medical", "RA0801001", Get_app));
            //StartCoroutine(App_tool.testMethod("人体构造", "2", "sign_ssp", "SA0C01007", Get_app));//SA0101047//SA0000000//RA0100001//SA0C02001
            //  StartCoroutine(App_tool.testMethod("经络穴位", "24", "sign_acu", "AA0100002", Get_app));//SA0101047//SA0000000//RA0100001//SA0C02001//
            //StartCoroutine(App_tool.testMethod("人体构造", "2", "sign", "SA0604002", Get_app));//SA0903001//SA0903005//SA0903006//SA0903007
            Get_app(App_tool.testMethod_signnew("sign_new", "SA090C003", Get_app));//SA0101047//SA0000000//RA0100001//SA0C02001//SA0C02000//SA0C04002//
            //StartCoroutine(App_tool.testMethod("人体构造", "2", "model", "SA0000000", Get_app));//SA0101047//SA0000000//RA0100001//SA0801002
            firstTest = false;
        }
        else
        {
            firstTest = true;
            // StartCoroutine(App_tool.testMethod("人体构造", "44", "medical", "RA0801004", Get_app));
            // StartCoroutine(App_tool.testMethod("人体构造", "2", "model", "SA0801003", Get_app));
            // StartCoroutine(App_tool.testMethod("人体构造", "model", "SA0101047", Get_app));//SA0101047//SA0000000//RA0100001
            //StartCoroutine(App_tool.testMethod("经络穴位", "24", "sign_acu", "AA0100000", Get_app));//SA0101047//SA0000000//RA0100001//SA0C02001//
            //  StartCoroutine(App_tool.testMethod("人体构造", "2", "model", "SA0000000", Get_app));
            // StartCoroutine(App_tool.testMethod("微课课程", "2", "microlesson", "HMFWK001", Get_app));//HMFWK002
            // StartCoroutine(App_tool.testMethod("人体构造", "2", "model", "SA0101047", Get_app));//SA0103043
            // StartCoroutine(App_tool.testMethod("人体构造", "2", "model", "SA0101047", Get_app));
            //StartCoroutine(App_tool.testMethod("人体构造", "2", "animation", "DA0100050", Get_app));
            // StartCoroutine(App_tool.testMethod("人体构造", "44", "medical", get_id_list(), Get_app));
            // StartCoroutine(App_tool.testMethod("人体构造", "44", "medical", "RA0801001", Get_app));
            // StartCoroutine(App_tool.testMethod("测验练习", "2", "exam", "qa001", Get_app));
            // StartCoroutine(App_tool.testMethod("经络穴位", "24", "sign_acu", "AA0100005", Get_app));
            //StartCoroutine(App_tool.testMethod("经络穴位", "24", "acu", "AA0100009N", Get_app));
        }
#elif UNITY_ANDROID
        show_progress = Unity_Tools.Get_scene_instance<ShowProgress>("LoadingCanvas");
#elif UNITY_STANDALONE_WIN
        show_progress = Unity_Tools.Get_scene_instance<ShowProgress>("LoadingCanvas_pc");
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.plugin)
        {
            if(PublicClass.app!=null){
                Get_app(PublicClass.app);
            }
        }
        else
        {
            if(!PublicClass.is_enter_server_control)
            {
                InternalSocketMananger.instance.HideUnityWidows();
                DebugLog.DebugLogInfo("HideUnityWidows");
            }
        }
#endif
        if (PublicClass.app == null)
        {
            SetOperaToLoop();
        }
    }

    public void BackPlatform()
    {
        InternalSocketMananger.instance.HideUnityWidows();
        DebugLog.DebugLogInfo("HideUnityWidows");
    }


    void Create_assets_path()
    {
        model_path = PublicClass.filePath + "model/";
        sign_path = PublicClass.filePath + "sign/";
        sign_ssp_path = PublicClass.filePath + "sign_ssp_path/";
        acu_path = PublicClass.filePath + "acu_path/";
        WeiKePlayer_path = PublicClass.filePath + "microlesson/";
        game_path = PublicClass.filePath + "game/";
        QA_path = PublicClass.filePath + "qa/";
        //生成资源文件夹
        Vesal_DirFiles.CreateDir(QA_path);
        Vesal_DirFiles.CreateDir(model_path);
        Vesal_DirFiles.CreateDir(sign_ssp_path);
        Vesal_DirFiles.CreateDir(sign_path);
        Vesal_DirFiles.CreateDir(WeiKePlayer_path);
        Vesal_DirFiles.CreateDir(game_path);
        Vesal_DirFiles.CreateDir(acu_path);
    }

    // {"app_type":null,"app_version":null,"youke_use":"enabled","is_integral":null,"icon_name":null,"platform":"pc,app","struct_state":"enabled", "fy_id":5,"Introduce":null,"is_charge":null,"is_hot":null,"struct_id":26,"struct_flag":null,"struct_sort":"2","app_id":null,"function_type":"5","update_flag":null,"download_count":null,"struct_sell_amount":0,"first_icon_url":"http://www.vesal.cn:8080/vesal//file/home/vincent2/59b0e2295f6f4c9794d06a0655310ea3/SA0102021/SA0102021.jpg","last_update_time":null,"is_recommended":null,"model_scope":"SA0101004","visible_identity":"1,2,3,4","struct_size":null,"struct_name":"全身骨连结","add_time":null}
    //分发场景
    public void Get_app(string get_message)
    {
        DebugLog.DebugLogInfo(get_message);
        App mm = JsonConvert.DeserializeObject<App>(get_message);
        if (mm.app_type != "app_screen")
        {

            if (mm != null && mm.app_id != null && mm.app_id != "")
            {
                Get_app(mm);
            }
            else
            {
                MyClass mc = JsonConvert.DeserializeObject<MyClass>(get_message);
                if (mc != null && mc.rykjMemberId != null && mc.rykjMemberId != "")
                {
                    myClass = mc;
                    GoMyClass();
                }
            }
        }
    }

    //获取ablist
    public string SearchPublicAppAblist(string app_id, string platform, string version, string level)
    {
        string ab_list = string.Empty;
        var stuct_local_db = new DbRepository<GetStructAbList>();
        //获取名词范围
        stuct_local_db.DataService("vesali.db");
        GetStructAbList stuct_tmpSt = stuct_local_db.SelectOne<GetStructAbList>((tempNo) =>
        {
            if (tempNo.app_id == app_id && tempNo.level == level)
            {
                return true;
            }
            else
            {
                return false;
            }

        });
        if (stuct_tmpSt != null)
        {
            ab_list = stuct_tmpSt.ab_list;
            if (stuct_tmpSt.replace_app_id != "" && stuct_tmpSt.replace_app_id != null)
                app_id = stuct_tmpSt.replace_app_id;
        }
        else
        {
            DebugLog.DebugLogInfo("GetStructAbList no id: " + PublicClass.app.app_id);
        }

        stuct_local_db.Close();

        return app_id + "|" + ab_list;
    }

    public static MyClass myClass;
    public class MyClass
    {
        public string rykjMemberId;
        public string mbEmail;
        public string mbTell;
    }
    public void GoMyClass()
    {
        Load_Scene("MyClass");
        SetOperaToRuning();
    }
    public void Get_app(App app)
    {
        // string abtemp = SearchPublicAppAblist(app.app_id, PublicClass.platform.ToString(), PublicClass.get_version(), ((int)PublicClass.Quality).ToString());
        // string[] abinfo = abtemp.Split('|');
        // app.app_id = abinfo[0];
        // if (abinfo[1] != null && abinfo[1] != "")
        //     app.ab_list = abinfo[1];
        this.this_app = app;
        PublicClass.app = this_app;
        DebugLog.DebugLogInfo("this_app.ab_path :" + this_app.ab_path);
        DebugLog.DebugLogInfo("type " + this_app.app_type + " version  " + this_app.app_version + " id " + this_app.app_id);
        switch (PublicClass.app.app_type)
        {
            case "acu":
            case "model":
                //检测本地文件，存在，版本替换，不存在，下载
                CheckAssetAB(model_path + this_app.app_id + ".xml", this_app.app_version, true);
                break;
            case "sign":
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                CheckAssetAB(sign_path + this_app.app_id + ".assetbundle", this_app.app_version);
#elif UNITY_IOS ||UNITY_ANDROID
                CheckAssetAB(sign_path + this_app.app_id + ".assetbundle", this_app.app_version, true);
#endif
                break;
            // case "ppt":
            //     CheckAssetAB(this_app.app_id,this_app.app_version);
            // break;
            case "microlesson":
                CheckAssetAB(WeiKePlayer_path + this_app.app_id + ".vsl", this_app.app_version);
                break;
            case "exam":
                CheckAssetAB(QA_path + this_app.app_id + ".zip", this_app.app_version);
                break;
            case "game":
                CheckAssetAB(game_path + this_app.app_id + ".assetbundle", this_app.app_version, true);
                break;
            case "sign_ssp":
                CheckAssetAB(sign_ssp_path + "StartStop.db", this_app.app_version);
                break;
            case "sign_new":
                CheckAssetAB(sign_ssp_path + "SignNew.db", this_app.app_version, true);
                break;
            case "sign_acu":
                CheckAssetAB(acu_path + "SignAcu.zip", this_app.app_version);
                break;
            case "medical":
                CheckAssetAB(PublicClass.MedicalPath + this_app.app_id + ".zip", "1");
                break;
            case "animation":
                CheckAssetAB(PublicClass.Anim_TimelinePath + this_app.app_id + ".timeline", this_app.app_version);
                break;
            default:
                //检测本地文件，存在，版本替换，不存在，下载
                CheckAssetAB(model_path + this_app.app_id, this_app.app_version);
                break;
        }
    }

    bool start_load_scene = false;
    void Update()
    {
        //UnityEngine.Debug.Log(OperaStatus);
        if (OperaStatus != AppOperState.Loop)
            timer = 0;
        switch (OperaStatus)
        {
            case AppOperState.Loop:
                timer++;
#if UNITY_EDITOR
#elif UNITY_IOS
                string message = Unity_Tools.get_message_from_platform_for_ios();
                if (message != "")
                {
                    DebugLog.DebugLogInfo("-------------------------------get_message---------------------------------------");
                    Get_app(message);
                    Unity_Tools.clear_message_from_platform_for_ios();
                }
                if (timer > 60)
                {
                    Unity_Tools.ui_return_to_platform();
                    timer = 0;
                }
#elif UNITY_ANDROID
        string message = Unity_Tools.get_message_from_platform_for_android();
        if (message != "")
        {
            DebugLog.DebugLogInfo("-------------------------------get_message---------------------------------------");
            Get_app(message);
            Unity_Tools.clear_message_from_platform_for_android();
        }
        if ( timer>60)
        {
            Unity_Tools.ui_return_to_platform();
            timer=0;
        }

#endif

                break;
            case AppOperState.Apprun:

                DebugLog.DebugLogInfo(" 资源完备加载 加载场景 " + this_app.app_type);
                UnityEngine.Debug.Log(" 资源完备加载 加载场景 " + this_app.app_type);
                switch (PublicClass.app.app_type)
                {
                    case "model":
                        //SetOperaToRuning();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.plugin)
                        {
                            Load_Scene("UI4_PptPlugin");
                        }
                        else
                        {
                            show_progress.CloseBtn(true);
                            Load_Scene("UI3");
                        }
#elif UNITY_IOS ||UNITY_ANDROID
                        Load_Scene("UI2",false);
#endif
                        //show_progress.StopLoad();
                        // Load_Scene("sign");
                        break;
                    case "sign":
                        show_progress.CloseBtn(true);
                        Load_Scene("UI_sign", false);
                        break;
                    case "sign_ssp":
                    case "sign_new":
                    case "sign_acu":
                        show_progress.CloseBtn(true);
                        Load_Scene("UI_sign_all");
                        // Load_Scene("sign");
                        break;
                    case "ppt":
                        Load_Scene("ppt");
                        break;
                    case "microlesson":
                        show_progress.CloseBtn(false);
                        Load_Scene("WeiKePlayer");
                        break;
                    case "game":
                        show_progress.CloseBtn(false);
                        Load_Scene("Block_UI");
                        break;
                    case "exam":
                        Load_Scene("totalScence");
                        break;
                    case "medical":
                        Load_Scene("Medical", true);
                        break;
                    case "animation":
                        Load_Scene("MotorAnatomy", true);
                        break;
                    case "acu":
                        Load_Scene("NewAcu", true);
                        break;
                    case "cfd":
                        Load_Scene("CFD", true);
                        break;
                    default:
                        break;
                }

                //SetOperaToRuning();
                break;
            case AppOperState.LoadScene:
                if (show_progress != null)
                {
                    show_progress.current_progress = _async.progress;
                }
                //vesal_log.vesal_write_log("load scenec" + _async.progress);
                break;
            case AppOperState.Appruning:
                //if (show_progress.current_progress <= 0.8f)
                if (_async != null && _async.progress >= 0.85f && _async.allowSceneActivation == false)
                {

                    _async.allowSceneActivation = true;
                }
                break;
            case AppOperState.UpdateData:
                show_progress.Set_Progress("系统正在为下载后首次运行准备数据，请稍等...");
                update_app_fix();
                OperaStatus = AppOperState.UpdataDating;
                break;
            case AppOperState.Downloading:
                if (http.progress <= 1.0f)
                    show_progress.current_progress = http.progress;
                break;

            case AppOperState.AssetLoad:
                UnityEngine.Debug.Log("模型正在加载中..." + PublicClass.app.app_type);
                show_progress.Set_Progress("模型正在加载中...", Call);
                switch (PublicClass.app.app_type)
                {
                    case "acu":
                    case "model":
                    // OperaStatus = AppOperState.Apprun;
                    // break;
                    case "sign":
                    case "sign_ssp":
                    case "sign_new":
                    case "sign_acu":
                    case "ppt":
                    case "microlesson":
                    case "game":
                    case "exam":
                        UnityEngine.Debug.Log("cacaulte_load_list();");
                        cacaulte_load_list();
                        if (load_list.Count() == 0)
                        {
                            UnityEngine.Debug.Log("OperaStatus = AppOperState.Apprun;");
                            OperaStatus = AppOperState.Apprun;
                        }
                        else
                        {
                            OperaStatus = AppOperState.AssetLoading;
                            UnityEngine.Debug.Log("OperaStatus = AppOperState.AssetLoading;");
                            load_assets_A(count_load);
                        }
                        break;
                    case "cfd":
                        OperaStatus = AppOperState.Apprun;
                        //cacaulte_load_list();
                        if (load_list.Count() == 0)
                        {
                            UnityEngine.Debug.Log("OperaStatus = AppOperState.Apprun;");
                            OperaStatus = AppOperState.Apprun;
                        }
                        else
                        {
                            OperaStatus = AppOperState.AssetLoading;
                            UnityEngine.Debug.Log("OperaStatus = AppOperState.AssetLoading;");
                            load_assets_A(count_load);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case AppOperState.UnZip:
                OperaStatus = AppOperState.UnZiping;
                if (File.Exists(downloadFilePath))
                {
                    StartCoroutine(Vesal_DirFiles.UnZipAsync(downloadFilePath, targetSourcePath + "/", unzipProgress, true));
                    //StartCoroutine(StartUnzip(downloadFilePath, targetSourcePath));
                }
                else
                {
                    OperaStatus = AppOperState.AssetLoad;
                }
                break;
            default:
                break;
        }
    }

    void update_app_fix()
    {
        StartCoroutine(update_db_process());
    }
    IEnumerator update_db_process()
    {

        string fixdb = PublicClass.MedicalPath + "vesali.db";
        string tmpdb = PublicClass.vesal_db_path + "temp.db";
        Vesal_DirFiles.DelFile(tmpdb);
        if (File.Exists(fixdb))
        {
            File.Move(fixdb, tmpdb);
        }
        show_progress.current_progress = 0.03f;
        string[] fname = Directory.GetFiles(PublicClass.MedicalPath);
        foreach (string file in fname)
        {
            string name = Vesal_DirFiles.get_file_name_from_full_path(file);
            string suffix = Vesal_DirFiles.get_name_suffix(name);
            if (suffix.ToLower() == "assetbundle")
            {
                Vesal_DirFiles.DelFile(PublicClass.filePath + name);
                Vesal_DirFiles.SaveAbfile(file, PublicClass.filePath + name, false);
                Vesal_DirFiles.DelFile(file);
            }
        }

        show_progress.current_progress = 0.1f;
        //        string tmpdb = PublicClass.vesal_db_path + "temp.db";
        //打开数据库进行更新
        List<string> fix_tab_list = new List<string>();
        if (File.Exists(tmpdb))
        {
            fix_tab_list = PublicTools.get_table_list("temp.db");
            if (fix_tab_list.Count != 0)
            {
                int tab_count = 0;
                foreach (string tab_name in fix_tab_list)
                {
                    switch (tab_name)
                    {
                        case "GetSubModel":
                            PublicTools.update_GetSubModel_db("temp.db");
                            break;
                        case "GetStructList":
                            PublicTools.update_GetStructList_db("temp.db");
                            break;
                        case "GetStructAbList":
                            PublicTools.update_GetStructAbList_db("temp.db");
                            break;
                        case "LayserSubModel":
                            PublicTools.update_LayserSubModel_db("temp.db");
                            break;
                        case "ModelRelationModel":
                            PublicTools.update_ModelRelationModel_db("temp.db");
                            break;
                        case "RightMenuLayerModel":
                            PublicTools.update_RightMenuLayerModel_db("temp.db");
                            break;
                        case "RightMenuModel":
                            PublicTools.update_RightMenuModel_db("temp.db");
                            break;
                        case "SignNewInfo":
                            PublicTools.update_SignNewInfo_db("temp.db");
                            break;
                        case "GetTextureModelList":
                            PublicTools.update_GetTextureModelList_db("temp.db");
                            break;
                        case "noun_no_info":
                            PublicTools.update_noun_no_info_db("temp.db");
                            break;
                        case "AbInfo":
                            PublicTools.update_ab_info_db("temp.db");
                            break;
                    }
                    tab_count++;
                    show_progress.current_progress = 0.1f + 0.9f * tab_count / fix_tab_list.Count;
                    yield return null;
                }
            }
            else
            {
                PublicTools.update_GetSubModel_db("temp.db");
                PublicTools.update_GetStructList_db("temp.db");
                show_progress.current_progress = 0.35f;
                yield return null;
                PublicTools.update_GetStructAbList_db("temp.db");
                PublicTools.update_LayserSubModel_db("temp.db");
                show_progress.current_progress = 0.5f;
                yield return null;
                PublicTools.update_ModelRelationModel_db("temp.db");
                PublicTools.update_RightMenuLayerModel_db("temp.db");
                show_progress.current_progress = 0.7f;
                yield return null;
                PublicTools.update_RightMenuModel_db("temp.db");
                PublicTools.update_SignNewInfo_db("temp.db");
                show_progress.current_progress = 0.85f;
                yield return null;
                PublicTools.update_GetTextureModelList_db("temp.db");
                PublicTools.update_noun_no_info_db("temp.db");
                PublicTools.update_ab_info_db("temp.db");
                show_progress.current_progress = 0.99f;
                yield return null;
            }
            Vesal_DirFiles.DelFile(tmpdb);
            PublicTools.Model_AB_dic_update();
            ManageModel.Instance.UpdateModelInfo();
        }
        OperaStatus = AppOperState.Apprun;
    }


    //下载完回调
    void DownLoad_complete()
    {
        vesal_log.vesal_write_log("--download completed  ");
        string fileSuffix = "";

        if (this_app.ab_path != null && this_app.ab_path != "")
        {
            fileSuffix = Vesal_DirFiles.get_name_suffix(this_app.ab_path).ToLower();
        }
        switch (PublicClass.app.app_type)
        {
            case "microlesson":
                //微课解压播放
                OperaStatus = AppOperState.UnZip;
                //微课自动播放
                //OperaStatus = AppOperState.Apprun;
                break;
            case "medical":
                if (targetSourcePath=="" || Directory.Exists(targetSourcePath))
                {
                    OperaStatus = AppOperState.Apprun;
                }
                else
                {
                    Directory.CreateDirectory(targetSourcePath);
                    OperaStatus = AppOperState.UpdateData;
                }
                break;
            case "animation":
                OperaStatus = AppOperState.Apprun;
                break;
            default:
                OperaStatus = AppOperState.AssetLoad;
                break;
        }
    }

    void unzipProgress(float pg)
    {
        show_progress.current_progress = pg;
        if (pg == 1.0f)
        {
            OperaStatus = AppOperState.AssetLoad;
            try
            {
                if (File.Exists(downloadFilePath))
                {
                    File.Delete(downloadFilePath);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                UnityEngine.Debug.Log(e.StackTrace);
            }
        }
    }
    string downloadFilePath = "";
    string targetSourcePath = "";
    //本地路径检测下载
    void CheckAssetAB(string ab_path, string app_version, bool CancelDownload = false)
    {
        

        DebugLog.DebugLogInfo("本地路径检测下载------------------" + this_app.ab_path);
        if (this_app.ab_path == "" || this_app.ab_path == null || CancelDownload)
        {
            DebugLog.DebugLogInfo("无私有资源：" + this_app.ab_path);
            DownLoad_complete();
            return;
        }
        string path = Vesal_DirFiles.get_dir_from_full_path(ab_path);
        string source_name = Vesal_DirFiles.get_file_name_from_full_path(ab_path);
        string short_name = Vesal_DirFiles.remove_name_suffix(source_name);
        string suffix = Vesal_DirFiles.get_name_suffix(source_name);
        downloadFilePath = path + short_name + "_" + app_version + "." + suffix;
        targetSourcePath = path + short_name + "_" + app_version;

        DebugLog.DebugLogInfo("targetSourcePath:" + targetSourcePath);
        //        if (!PublicClass.online_mode)
        //        {
        //            DebugLog.DebugLogInfo("离线模式");
        //            DebugLog.DebugLogInfo("资源路径：" + path);
        //            DebugLog.DebugLogInfo("short_name  " + short_name);

        //            int founded = 0;
        //            DirectoryInfo dif = new DirectoryInfo(path);
        //            FileSystemInfo[] fsis = dif.GetFileSystemInfos();
        //            for (int i = 0; i < fsis.Length; i++)
        //            {
        //                FileSystemInfo tmp = fsis[i];
        //                if (tmp.FullName == downloadFilePath || tmp.FullName == targetSourcePath || tmp.FullName.Replace("\\", "/") == targetSourcePath || tmp.FullName.Replace("\\", "/") == downloadFilePath)
        //                {
        //                    //PublicClass.app.struct_name = short_name;
        //                    PublicClass.app.ab_path = Vesal_DirFiles.get_file_name_from_full_path(tmp.FullName);
        //                    PublicClass.app.xml_path = tmp.FullName;
        //                    founded = 1;
        //                    break;
        //                }
        //            }
        //            if (founded == 0)
        //            {
        //                this_app = null;
        //                PublicClass.app = null;
        //                SetOperaToLoop();
        //                //发送
        //#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //                if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.plugin)
        //                {
        //                    send_cmd((byte)VESAL_CMD_CODE.MSG_CMD, "hide");
        //                }
        //#elif UNITY_IOS
        //                    Unity_Tools.clear_message_from_platform_for_ios();
        //                    Unity_Tools.ui_return_to_platform();
        //#elif UNITY_ANDROID
        //                    Unity_Tools.clear_message_from_platform_for_android();
        //                    Unity_Tools.ui_return_to_platform();
        //#else
        //#endif
        //                return;
        //            }
        //            else
        //            {
        //                DownLoad_complete();
        //            }
        //        }
        //        else
        //        {
        DebugLog.DebugLogInfo("在线模式");
        switch (PublicClass.app.app_type)
        {
            // case "medical":
            //     //目录存在，说明已经成功下载
            //     if (Directory.Exists(targetSourcePath))
            //     {
            //         DownLoad_complete();
            //     }
            //     else
            //     {
            //         DownLoadSignGroup(PublicClass.app.ab_path, downloadFilePath);
            //     }
            //     break;
            default:
                // PublicClass.TimelineFilePath = targetSourcePath;
                PublicClass.app.xml_path = downloadFilePath;
                string tmpPath = this_app.ab_path + "";
                PublicClass.app.ab_path = short_name + "_" + app_version + "." + suffix;
                //删除旧版本
                int currentVersion = int.Parse(app_version);
                string lastVersionPath = path + short_name + "_";
                for (int i = 0; i < currentVersion; i++)
                {
                    if (Directory.Exists(lastVersionPath + i))
                    {
                        Directory.Delete(lastVersionPath + i, true);
                    }
                }
                if (!File.Exists(downloadFilePath) && !Directory.Exists(targetSourcePath))
                {
                    string[] files = Directory.GetFiles(path);
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (Vesal_DirFiles.get_file_name_from_full_path(files[i]).StartsWith(short_name))
                            Vesal_DirFiles.DelFile(files[i]);
                    }
                    //下载远程
                    DownLoadSignGroup(tmpPath, downloadFilePath);
                }
                else
                {
                    DownLoad_complete();
                }
                break;
                //}
        }
    }

    //下载 ab  包
    private void DownLoadSignGroup(string resource_path, string tartget_path)
    {
        OperaStatus = AppOperState.Downloading;
        DebugLog.DebugLogInfo("开启下载" + this_app.app_type);
        if (File.Exists(tartget_path))
        {
            DebugLog.DebugLogInfo("删除文件  ");
            File.Delete(tartget_path);
        }
        http = new HttpDownLoad();
        //开启下载，能取消下载
        show_progress.Set_Progress("资源下载中...", CancelDownload);

        DebugLog.DebugLogInfo(resource_path + "  " + tartget_path);
        //下载源路径文件到  目标地址，进行回调验证下载完成
        http.DownLoad(resource_path, tartget_path, DownLoad_complete, download_error);
    }

    //下载错误
    void download_error(Exception e)
    {
        vesal_log.vesal_write_log("下载文件异常" + e.Message);
        vesal_log.vesal_write_log("下载文件异常" + e.StackTrace);
        SetOperaToLoop();
        Unity_Tools.ui_return_to_platform("alert", "网络下载异常，请重试。");
    }

    //异步加载所有场景，解析后，跳转对应场景
    void Load_Scene(string scene_name, bool showProgress = true)
    {
        if (scene_name == "WeiKePlayer")
        {
            StartCoroutine(loadSceneWithBlackPanel(scene_name));
        }
        else
        {
            //  SceneManager.LoadSceneAsync("ManagerScene",LoadSceneMode.Additive);
            //SceneManager.LoadScene(scene_name);
            if (showProgress)
            {
                show_progress.Set_Progress("加载场景... ");
            }
            else
            {
                show_progress.StopLoad();
            }
            OperaStatus = AppOperState.LoadScene;
            vesal_log.vesal_write_log("开始异步加载场景...");
            UnityEngine.Debug.Log("开始异步加载场景...");
            StartCoroutine("LoadScene", scene_name);

        }
    }
    private AsyncOperation _async = new AsyncOperation();
    IEnumerator LoadScene(string sceneName)
    {
        InternalSocketMananger.instance.isShowUnityWindow = true;
        InternalSocketMananger.instance.ShowUnityWidows();
        _async = SceneManager.LoadSceneAsync(sceneName);
        yield return _async;
        SetOperaToRuning();
    }

    IEnumerator loadSceneWithBlackPanel(string scene_name)
    {
        foreach (GameObject obj in BlackPanels)
        {
            obj.transform.SetAsLastSibling();
            obj.SetActive(true);
        }
        //等一帧至画面黑屏
        yield return null;
        if (show_progress != null)
        {
            show_progress.Set_Progress("加载场景... ");
        }
        OperaStatus = AppOperState.LoadScene;
        StartCoroutine("LoadScene", scene_name);
        //SceneManager.LoadScene(scene_name);
    }
    public void CancelDownload()
    {
        http.Close();
        this_app = null;
        PublicClass.app = null;
        //发送
        send_cmd((byte)VESAL_CMD_CODE.MSG_CMD, "hide");
    }

    ushort port;
    public void getPort()
    {
        string filepath = Application.streamingAssetsPath + "/port.txt";
        try
        {
            if (File.Exists(filepath))
            {
                port = ushort.Parse(File.ReadAllText(Application.streamingAssetsPath + "/port.txt"));
            }
            else
            {
                port = 8900;
            }
        }
        catch
        {
            // DebugLog.DebugLogInfo(port);
        }
    }

    vesal_socket _conn_sock = new vesal_socket();

    public bool send_cmd(byte cmd, String str)
    {
        byte[] data = packet.create_output_from_string(cmd, str);
        _conn_sock.send_data(data);
        return true;
    }

    bool send_cmd_and_wait_response(byte cmd, String str, ref packet pk, int micro_seconds)
    {
        vesal_socket sock = new vesal_socket();
        bool result = sock.connect("127.0.0.1", port);
        if (!result)
        {
            vesal_log.vesal_write_log("cant conn");
            return false;
        }

        byte[] data = packet.create_output_from_string(cmd, str);
        sock.send_data(data);

        long cur = DateTime.Now.Ticks / 10000;
        bool ret = false;
        while (true)
        {
            System.Threading.Thread.Sleep(1);
            ArrayList sockList = new ArrayList();
            sockList.Add(sock._sock);
            Socket.Select(sockList, null, null, 1);
            if (sockList.Count > 0)
            {
                try
                {
                    ret = sock.recv_packet(ref pk);
                }
                catch
                {
                    vesal_log.vesal_write_log("recv failed!!!");
                    return false;
                }
            }

            if (ret)
            {
                vesal_log.vesal_write_log("get response");
                break;
            }

            long cur2 = DateTime.Now.Ticks / 10000;
            if (cur2 >= cur + micro_seconds)
            {
                vesal_log.vesal_write_log("time out");
                break;
            }
        }

        sock.close();
        return ret;
    }


    //ab包加载

    List<Download_Vesal_info> load_list;
    int load_count = 0;
    int load_length = 0;
    void cacaulte_load_list()
    {
        load_list = new List<Download_Vesal_info>();
        string downloaded_str = "";
        foreach (Download_Vesal_info download in PublicClass.Asset_loaded_list)
            downloaded_str += download.name;
        print("PublicClass.total_load_list :" + PublicClass.total_load_list.Count);
        if (this_app.ab_list == null || (this_app.ab_list.Trim() == ""))
        {
            foreach (Download_Vesal_info download in PublicClass.total_load_list)
            {
                if (!downloaded_str.Contains(Vesal_DirFiles.remove_name_suffix(download.name)))
                {
                    download.type = "AB";
                    load_list.Add(download);
                }
            }

        }
        else
        {
            string[] ab_files = this_app.ab_list.Split(',');
            string ablist_str = "";
            for (int k = 0; k < ab_files.Length; k++)
            {
                //if (PublicClass.low_res_ablist.Contains(ab_files[k]))
                //    ab_files[k] = "s_"+ab_files[k];
                ablist_str += ab_files[k];
                if (!downloaded_str.Contains(ab_files[k]))
                {
                    foreach (Download_Vesal_info download in PublicClass.total_load_list)
                    {
                        if (download.name.Contains(ab_files[k]))
                        {
                            download.type = "AB";
                            load_list.Add(download);
                            break;
                        }
                    }
                }
            }


            // //对于性能差的机型，卸载掉不用的ab包模型数据
            // if ((PublicClass.Quality == Run_Quality.POOL) && (load_list.Count + PublicClass.Asset_loaded_list.Count() >= PublicClass.MAX_Ab_num))
            // {
            //     bool unload = false;
            //     for (int k = PublicClass.Asset_loaded_list.Count() - 1; k >= 0; k--)
            //     {
            //         Download_Vesal_info temp = PublicClass.Asset_loaded_list[k];
            //         if (!PublicTools.isTargetInSourceList(Vesal_DirFiles.remove_name_suffix(temp.name), PublicClass.app.ab_list))
            //         {
            //             UnityEngine.Debug.Log("POOL Destroy::" + temp.name);
            //             try
            //             {
            //                 DestroyImmediate(temp.instance);
            //                 //Destroy(temp.source);
            //             }
            //             catch (Exception e)
            //             {
            //                 UnityEngine.Debug.Log(e.Message);
            //             }
            //             Resources.UnloadUnusedAssets();
            //             PublicClass.Asset_loaded_list.RemoveAt(k);
            //             unload = true;
            //         }
            //     }
            //     if (unload)
            //         ReadModelAgain();
            // }

        }


        if (PublicClass.app.app_type == "sign_acu")
        {
            Download_Vesal_info download = new Download_Vesal_info();
            download.name = "acu_path/" + Vesal_DirFiles.remove_name_suffix(PublicClass.app.ab_path) + "/acupoint.assetbundle";
            download.type = "TEMP";
            load_list.Add(download);
        }

        print("downloaded_strdownloaded_str :" + downloaded_str);

        UnityEngine.Debug.Log("加载ab 包 长度：" + load_list.Count);
    }

    //异步加载assetbundle资源列表
    public void load_assets_A(Action load_complete = null)
    {


        Unity_Tools.StarTime("获取远程数据计时-------");

        load_length = 0;
        load_count = 0;
        load_length = load_list.Count;
        DebugLog.DebugLogInfo("--------------------加载模型长度：" + load_length);
        for (int i = 0; i < load_list.Count; i++)
        {
            DebugLog.DebugLogInfo("--------------------: " + load_list[i].name);

        }

        StartCoroutine(LoadPrefabModel(PublicClass.filePath + load_list[0].name, Vesal_DirFiles.remove_name_suffix(load_list[0].name), count_load));
    }

    public void Call()
    {
        DebugLog.DebugLogInfo("cancel loading ----------------------");
    }

    public static void SetOperaToRuning()
    {
        OperaStatus = AppOperState.Appruning;
        Unity_Tools.set_value_to_platform("Unity_state", "Run");
        Unity_Tools.send_message_to_platform("setUnityState", "Run");
    }

    public static void SetOperaToLoop()
    {
        OperaStatus = AppOperState.Loop;
        Unity_Tools.set_value_to_platform("Unity_state", "Loop");
        Unity_Tools.send_message_to_platform("setUnityState", "Loop");
    }

    //加载完毕回调
    void count_load()
    {
        if (load_error_flag == false)
        {
            if (load_list[load_count].type != "TEMP")
                PublicClass.Asset_loaded_list.Add(load_list[load_count]);
        }
        else
        {
            //资源加载出错，放弃继续
            SetOperaToLoop();
            UnityEngine.Debug.Log("load error " + load_list[load_count].name);
            Unity_Tools.ui_return_to_platform("alert", "资源加载错误，请重试。");
            return;
        }

        load_count++;
        show_progress.current_progress = (float)load_count / (float)load_length;
        if (load_list.Count > load_count)
        {
            timer = 0;
            StartCoroutine(LoadPrefabModel(PublicClass.filePath + load_list[load_count].name, Vesal_DirFiles.remove_name_suffix(load_list[load_count].name), count_load));

        }
        if (load_count == load_length)
        {
            DebugLog.DebugLogInfo("加载完毕 获取平台参数：");
            //后台加载完成后,进入场景分发器，异步加载多个场景
            OperaStatus = AppOperState.Apprun;
        }

    }

    bool load_error_flag = false; //记录模型加载过程中的异常错误
    IEnumerator LoadPrefabModel(string path, string name, Action count_load = null)
    {
        load_error_flag = false;
        //encrypt.DecryptFile(path, path.Remove(path.LastIndexOf('/')) + "/vesal.temp", "Vesal17788051918");
        //解压ab文件
        //yield return null;  
        //防止卡死
        yield return new WaitForEndOfFrame();

        string temp_path = PublicClass.tempPath + name + "temp.dat";
        //vesal_log.vesal_write_log("load model phase 1 start: " + DateTime.Now.Ticks);
        //yield return null;
        AssetBundleCreateRequest assetBundleCreateRequest = null;
        try
        {
            if (load_list[load_count].type == "TEMP")
                assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(path);
            else
            {
                Vesal_DirFiles.GetAbfile2(path, temp_path);
                assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(temp_path);
            }
            //assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(Vesal_DirFiles.GetAbfile(path, temp_path));
        }
        catch (Exception ex)
        {

            load_error_flag = true;
            UnityEngine.Debug.Log(ex.Message);
            UnityEngine.Debug.Log(ex.StackTrace);
            vesal_log.vesal_write_log("assetbundle load error in :" + name);
            vesal_log.vesal_write_log("error message:" + ex.Message);
            if (File.Exists(temp_path))
            {
                try
                {
                    Vesal_DirFiles.DelFile(temp_path);
                }
                catch { }
            }
        }
        //Vesal_DirFiles.DelFile(temp_path);

        if (assetBundleCreateRequest != null && load_error_flag == false)
        {
            yield return assetBundleCreateRequest;

            //vesal_log.vesal_write_log("load model phase 3 start: " + DateTime.Now.Ticks);

            //AssetBundle curBundleObj = AssetBundle.LoadFromMemory(Vesal_DirFiles.GetAbfile(path, temp_path));
            GameObject CurrentObj = null;
            GameObject realObj = null;
            AssetBundleRequest abr = null;
            try
            {
                if (load_list[load_count].type == "TEMP")
                {
                    abr = assetBundleCreateRequest.assetBundle.LoadAssetAsync(Vesal_DirFiles.get_file_name_from_full_path(name), typeof(GameObject));
                }
                else
                {
                    abr = assetBundleCreateRequest.assetBundle.LoadAssetAsync(name, typeof(GameObject));

                    //vesal_log.vesal_write_log("load model phase 4 start: " + DateTime.Now.Ticks);
                }

            }
            catch (Exception ex)
            {
                load_error_flag = true;
                UnityEngine.Debug.Log(ex.Message);
                UnityEngine.Debug.Log(ex.StackTrace);
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
                yield return realObj;
                //vesal_log.vesal_write_log("load model phase 5 start: " + DateTime.Now.Ticks);
                realObj.name = realObj.name.Replace("(Clone)", "");

                Transform Transform_parent;

                print("name :" + load_list[load_count].name + " type: " + load_list[load_count].type);
                if (load_list[load_count].type == "TEMP")
                {
                    print("---------- temp model 管理器");
                    Transform_parent = PublicClass.Transform_temp;
                }
                else
                {
                    ReadModel(realObj);
                    Transform_parent = PublicClass.Transform_parent;
                    load_list[load_count].source = CurrentObj;
                    load_list[load_count].instance = realObj;
                }
                realObj.transform.SetParent(Transform_parent);

                assetBundleCreateRequest.assetBundle.Unload(false);

                Transform_parent.gameObject.SetActive(false);


                if ((load_list[load_count].type != "TEMP") && (File.Exists(temp_path)))
                {
                    try
                    {
                        Vesal_DirFiles.DelFile(temp_path);
                    }
                    catch { }
                }
                //vesal_log.vesal_write_log("load model phase 5 end: " + DateTime.Now.Ticks);
            }
        }
        if (count_load != null)
        {
            count_load();
        }
    }

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



    public void ReadModel(GameObject parent)
    {
        Model[] models = parent.GetComponentsInChildren<Model>();
        DebugLog.DebugLogInfo(models.Length + "  总模型长度");
        for (int j = 0; j < models.Length; j++)
        {
            if (!PublicClass.id_model_dic.ContainsKey(models[j].name))
            {
                PublicClass.AllModels.Add(models[j]); //  A
                PublicClass.id_model_dic.Add(models[j].name, PublicClass.AllModels.Count - 1);
            }
            else
            {
                vesal_log.vesal_write_log("founed error model" + models[j].name);
            }
        }
    }

    public void exitSceneSwitch()
    {
        timer = 0;
        try
        {
            if (http != null)
            {
                http.Close();
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
            UnityEngine.Debug.Log(e.StackTrace);
        }
        Unity_Tools.clear_message_from_platform();
        Unity_Tools.ui_return_to_platform();
    }
}