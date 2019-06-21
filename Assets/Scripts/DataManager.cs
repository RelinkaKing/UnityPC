using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Assets.Scripts.DbDao;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Model;
using Assets.Scripts.Public;
using Assets.Scripts.Repository;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VesalCommon;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

public enum DataIntState
{
    Init,
    StreamCopy,
    StreamCopying,
    FetchDb,
    FetchingDb,
    GoABdownload,
    Downloading,
    Update,
    Updating,
    HotFix,
    HotFixing,
    LoadAsset,
    LoadAsseting,
    GoNextScence,
    Null
}

public class DataManager : MonoBehaviour
{
    List<string> copy_list = new List<string>(); 
    // public static DataManager instance;
    public ShowProgress progress;
    // public DataIntState DataManagerStatus = DataIntState.Init;
    List<string> stream_copy_list = new List<string>();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
    public const int SW_SHOWMAXIMIZED = 3;
    void Awake()
    {
        PublicClass.InitStaticData();
        PublicClass.InitDirFiles();
        //DebugLog.DebugLogInfo("初始化全局变量---------------------------------");
        //DebugLog.DebugLogInfo("------------------------------------------------------------" + PublicClass.tempPath + "unity.log");
    }

  
    void Start()
    {
#if UNITY_EDITOR  
        //生成copylist，，，，测试
        StartCoroutine(WWWLoad());
#elif UNITY_STANDALONE_WIN
        PublicClass.DataManagerStatus = DataIntState.Init;
#elif UNITY_ANDROID
        DebugLog.DebugLogInfo("---------------当前加载模型质量-------------------" + PublicClass.Quality );
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            DebugLog.DebugLogInfo("hide  splash---------------------------------");
            jo.Call("hideSplash");
        }
        catch (System.Exception e)
        {
            DebugLog.DebugLogInfo(e.Message);
        }
        StartCoroutine(WWWLoad());
#elif UNITY_IOS
        StartCoroutine(WWWLoad());
#endif
    }

    void Update()
    {
        switch (PublicClass.DataManagerStatus)
        {
            case DataIntState.Init:
                if (check_is_fist_runing())
                {
                    DebugLog.DebugLogInfo("first running---------------------------------");
                    PublicClass.DataManagerStatus = DataIntState.StreamCopy;
                    Debug.Log("first running---------------------------------");
                    if (progress != null)
                    {

                        progress.Set_Progress("系统正在为首次运行准备环境 ......");
                    }
                }
                else
                    PublicClass.DataManagerStatus = DataIntState.FetchDb;
                break;
            case DataIntState.StreamCopy:
                PreAssets();
                break;
            case DataIntState.StreamCopying:
                if (stream_copy_list.Count > 0)
                {
                    if (progress != null)
                    {
                        progress.current_progress = stream_copy_count / (float)stream_copy_list.Count;
                    }
                }
                break;
            case DataIntState.FetchDb:
                DownLoadRemoteData();
                if (progress != null)
                {
                    progress.Set_Progress("正在获取关键数据...");
                }
                break;
            case DataIntState.FetchingDb:
                break;
            case DataIntState.GoABdownload:
                break;
            default:
                break;
        }



    }

    bool check_is_fist_runing()
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer || PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.plugin
            || PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.demo_pc)
            return false;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return false;
#elif UNITY_IOS
#elif UNITY_ANDROID
#endif

        string fname = PublicClass.filePath + "version.dat";
        string version = PublicTools.read_version_from_file(fname);
        DebugLog.DebugLogInfo("--" + fname);
        if (version == null)
            return true;
        DebugLog.DebugLogInfo("founded-versions--------------------------------" + version + "---" + PublicClass.get_assets_version());

        if (PublicTools.version_is_larger(PublicClass.get_assets_version(), version))
        {
            return true;
        }
        else
        {
            //            PublicClass.set_version(version);
            return false;
        }
    }


    int stream_copy_count = 0;
    public void PreAssets()
    {
        DebugLog.DebugLogInfo("preasset---------------------------------");

        PublicClass.DataManagerStatus = DataIntState.StreamCopying;

        string source_path = string.Empty;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        source_path = PublicClass.fileLocalStreamingPath.Replace("file://", "");
#elif UNITY_IOS
        source_path = PublicClass.fileLocalStreamingPath;
#elif UNITY_ANDROID
        source_path = PublicClass.fileLocalStreamingPath;
#endif
        string target_path = PublicClass.filePath;
        try
        {
            Vesal_DirFiles.DelectDirFiles(target_path, new List<string> { "PPT", "temp" });
            DebugLog.DebugLogInfo("delete files ok---------------------------------");
            UnityEngine.Debug.Log(source_path);
            UnityEngine.Debug.Log(copy_list.Count);
            PublicClass.InitDirFiles();

            stream_copy_list = PublicTools.DeepCopy<List<string>>(copy_list);
            for (int i = 0; i < stream_copy_list.Count; i++)
            {
                stream_copy_list[i] = source_path + stream_copy_list[i];
            }
            if (copy_list.Count > 0)
            {
                string source = stream_copy_list[stream_copy_count];
                string target = target_path + copy_list[stream_copy_count];
                StartCoroutine(Vesal_DirFiles.Vesal_FileCopy(source, target, count_stream_copy));
            }
            else
            {
                DebugLog.DebugLogInfo("stream copy 异常--------------------------------");

                PublicClass.DataManagerStatus = DataIntState.FetchDb;
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("current count :"+stream_copy_count);
            UnityEngine.Debug.Log(e.Message);
            UnityEngine.Debug.Log(e.StackTrace);
            PublicClass.DataManagerStatus = DataIntState.FetchDb;
        }
    }

    void count_stream_copy()
    {
        stream_copy_count++;
        if (stream_copy_count >= stream_copy_list.Count)
        {
            PublicClass.DataManagerStatus = DataIntState.FetchDb;
            PublicTools.save_version_to_file(PublicClass.filePath + "version.dat", PublicClass.get_assets_version());
        }
        else
        {
            string source = stream_copy_list[stream_copy_count];
            string target = PublicClass.filePath + copy_list[stream_copy_count];

            StartCoroutine((Vesal_DirFiles.Vesal_FileCopy(source, target, count_stream_copy)));
        }

    }




    private void DownLoadRemoteData()
    {
        Unity_Tools.StarTime("获取远程数据计时-------");
        //判断有网时操作

        if (!MainConfig.isFetchDb)
        {
            PublicClass.server_ip = vesal_network.Vesal_Network.get_ipfromlist(PublicClass.get_server_interface);

            PublicClass.server_test_url = PublicClass.server_ip + PublicClass.fix_server_interface;
            PublicClass.data_list_count = PublicClass.tableCount;
            PublicClass.DataManagerStatus = DataIntState.GoABdownload;
        }

        PublicClass.online_mode = MainConfig.isOnline;

        if (PublicClass.online_mode && vesal_network.Vesal_Network.get_network_is_acitve() & PPTGlobal.PPTEnv!= PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            try
            {
                PublicClass.server_ip = vesal_network.Vesal_Network.get_ipfromlist(PublicClass.get_server_interface);
                if(PublicClass.server_ip!="")
                {
                    PublicClass.server_test_url = PublicClass.server_ip + PublicClass.fix_server_interface;
                    DebugLog.DebugLogInfo("unity  --------------------在线启动");
                    StartCoroutine(StartUpdateTable());
                    PublicClass.DataManagerStatus = DataIntState.FetchingDb;
                    PublicClass.low_res_ablist = "";
                }
                else
                {
                    DebugLog.DebugLogInfo("unity  --------------------离线启动");
                    PublicClass.online_mode = false;
                    PublicClass.data_list_count = PublicClass.tableCount;
                    PublicClass.DataManagerStatus = DataIntState.GoABdownload;
                }
            }
            catch
            {
                DebugLog.DebugLogInfo("unity  --------------------离线启动");
            }
        }
        else
        {
            PublicClass.online_mode = false;

            DebugLog.DebugLogInfo("unity  --------------------离线启动");
            PublicClass.data_list_count = PublicClass.tableCount;
            PublicClass.DataManagerStatus = DataIntState.GoABdownload;
        }
    }
    public IEnumerator StartUpdateTable()
    {
        yield return null;
        var cache_CommonAssetLib = new CacheOption<CommonAssetLib>();
        cache_CommonAssetLib.setCache(PublicClass.server_ip + "v1/app/struct/getPublicFile");
        print(PublicClass.server_ip + "v1/app/struct/getPublicFile");
        yield return null;
#if UNITY_EDITOR
        PublicClass.tableCount = 19;
        #else
                // if (PPTGlobal.PPTEnv== PPTGlobal.PPTEnvironment.demo_pc || PPTGlobal.PPTEnv== PPTGlobal.PPTEnvironment.pc )
                if (PPTGlobal.PPTEnv== PPTGlobal.PPTEnvironment.demo_pc )
                    PublicClass.tableCount =PublicClass.online_mode? 12:1;
                else
                    PublicClass.tableCount = 1;
        #endif
        if (PublicClass.tableCount == 19)
        {
            var cache_abinfo = new CacheOption<AbInfo>();
            cache_abinfo.setCache(PublicClass.server_ip + "v1/app/xml/getAllAbSubmodel");
            yield return null;
            var cache_structlist = new CacheOption<GetStructList>();
            cache_structlist.setCache(PublicClass.server_ip + "v1/app/xml/getNoun");
            yield return null;
            var cache_sub_model = new CacheOption<GetSubModel>();
            cache_sub_model.setCache(PublicClass.server_ip + "v1/app/xml/getSubModel");
            yield return null;
            var cache_layer_sub = new CacheOption<LayserSubModel>();
            cache_layer_sub.setCache(PublicClass.server_ip + "v1/app/xml/getAllLayerSubmodel");
            yield return null;
            var cache_RightMenuModel = new CacheOption<RightMenuModel>();
            cache_RightMenuModel.setCache(PublicClass.server_ip + "v1/app/xml/getAllRightMenu");
            yield return null;
            var cache_RightMenuLayerModel = new CacheOption<RightMenuLayerModel>();
            cache_RightMenuLayerModel.setCache(PublicClass.server_ip + "v1/app/xml/getAllRightMenuLayer");
            yield return null;
            var cache_ModelRelationModel = new CacheOption<ModelRelationModel>();
            cache_ModelRelationModel.setCache(PublicClass.server_ip + "v1/app/xml/getAllAssHide");
            yield return null;
            var cache_getStructAbList = new CacheOption<GetStructAbList>();
            cache_getStructAbList.setCache(PublicClass.server_ip + "v1/app/xml/getStructAbList");
            print(PublicClass.server_ip + "v1/app/xml/getStructAbList");
            yield return null;
            var cache_getNoun_no_info = new CacheOption<noun_no_info>();
            cache_getNoun_no_info.setCache(PublicClass.server_ip + "v1/app/xml/getxmlSignNewIds");
            print(PublicClass.server_ip + "v1/app/xml/getTempSignNewIds");
            yield return null;
            var cache_getSignNewInfo = new CacheOption<SignNewInfo>();
            cache_getSignNewInfo.setCache(PublicClass.server_ip + "v1/app/xml/getXmlSignNew");
            yield return null;
            var cache_getTextureModelList = new CacheOption<GetTextureModelList>();
            cache_getTextureModelList.setCache(PublicClass.server_ip + "v1/app/xml/getXmlSignSubmodel");
            yield return null;
            var cache_getMotorAnatomy = new CacheOption<MotorAnatomy>();
            cache_getMotorAnatomy.setCache(PublicClass.server_ip + "v1/app/xml/getAllMotorAnatomy");
            yield return null;
            var cache_getMotorSubmodel = new CacheOption<MotorAnatomy_submodel>();
            cache_getMotorSubmodel.setCache(PublicClass.server_ip + "v1/app/xml/getAllMotorSubmodel");
            yield return null;
            //http://118.24.119.234:8083/vesal-jiepao-test/v1/app/xml/getAcuNounSubmodel?Version=-1
            var cache_getAcuNounSubmodel = new CacheOption<AcuNounSubmodel>();
            cache_getAcuNounSubmodel.setCache(PublicClass.server_ip + "v1/app/xml/getAcuNounSubmodel");
            yield return null;
            //http://118.24.119.234:8083/vesal-jiepao-test/v1/app/xml/getAcuSubmodel?Version=1
            var cache_getAcuSubmodel = new CacheOption<AcuSubmodel>();
            cache_getAcuSubmodel.setCache(PublicClass.server_ip + "v1/app/xml/getAcuSubmodel");
            yield return null;
            var cache_getAcuLine = new CacheOption<AcuLine>();
            cache_getAcuLine.setCache(PublicClass.server_ip + "v1/app/xml/getAcuLine");
            yield return null;
            // http://114.115.210.145:8083/vesal-jiepao-test//v1/app/xml/getMarkNoun?Version=11
            var cache_getMarkNoun = new CacheOption<MarkNoun>();
            cache_getMarkNoun.setCache(PublicClass.server_ip + "v1/app/xml/getMarkNoun");
            yield return null;
            //http://114.115.210.145:8083/vesal-jiepao-test/v1/app/xml/getTriggerSubmodel?Version=11
            var cache_getTriggerSubmodel = new CacheOption<TriggerSubmodel>();
            cache_getTriggerSubmodel.setCache(PublicClass.server_ip + "v1/app/xml/getTriggerSubmodel");
            yield return null;

        }
    //    PublicClass.DataManagerStatus = DataIntState.GoABdownload;
    }


    public static List<Download_Vesal_info> Download_list;

    //获取公共资源列表
    public static List<Download_Vesal_info> GetStructInfo(asset_platform platform)
    {
        print("当前平台：" + Enum.GetName(typeof(asset_platform), platform));
        List<Download_Vesal_info> list = new List<Download_Vesal_info>();
        var local_db = new DbRepository<CommonAssetLib>();
        local_db.DataService("vesali.db");
        var tmpIe = local_db.Select<CommonAssetLib>((tempNo) =>
        {
            if (tempNo.url != null && tempNo.platform == Enum.GetName(typeof(asset_platform), platform))
            {
                // DebugLog.DebugLogInfo("公共资源 名称：" + tempNo.ab_name + " " + tempNo.url + " version " + tempNo.version);
                return true;
            }
            else
            {
                return false;
            }

        });
        foreach (var i in tmpIe)
        {
            Download_Vesal_info temp_info = new Download_Vesal_info();


            if ((PublicClass.Quality == Run_Quality.POOL) && PublicTools.isTargetInSourceList(i.ab_name, PublicClass.low_res_ablist, ',') && (i.type == "AB"))
            {
                temp_info.url = i.url.Insert(i.url.LastIndexOf(i.ab_name), "s_");
                temp_info.name = "s_" + i.ab_name;
            }
            else
            {
                temp_info.name = i.ab_name;
                temp_info.url = i.url;
            }
            temp_info.version = i.version;
            temp_info.isNeedDownload = false;
            temp_info.type = i.type;
            temp_info.instance = null;
            temp_info.source = null;
            // print(temp_info.name);
            list.Add(temp_info);
        }
        local_db.Close();
        return list;
    }


    void ReadCopyList()
    {
        string target_file = PublicClass.filePath + "filelist.txt";
        StartCoroutine((Vesal_DirFiles.Vesal_FileCopy(PublicClass.fileLocalStreamingPath + "filelist.txt", target_file,() => {
                copy_list.AddRange(Vesal_DirFiles.ReadFileWithLine(target_file));
                DebugLog.DebugLogInfo("copy_list count -----------------:"+copy_list.Count);
                PublicClass.DataManagerStatus = DataIntState.Init;
                Vesal_DirFiles.DelFile(target_file);
        })));
    }

    //www加载filelist.txt文件，生成拷贝列表
    IEnumerator WWWLoad()
    {
        WWW www = new WWW(PublicClass.fileLocalStreamingPath + "filelist.txt");
        yield return www;
        if (www.error != null)
        {
            Debug.Log("load error " + www.error);
            Debug.Log("load error : filelist.txt not found");
        }
        else
        {
            string[] copy_arr = www.text.Split(',');
            for (int i = 0; i < copy_arr.Length; i++)
            {
                copy_list.Add(copy_arr[i]);
            }
        }
        www.Dispose(); 
        PublicClass.DataManagerStatus = DataIntState.Init;
    }

    [Serializable]
    public class asset_top_info
    {
        public string msg;
        public string code;
        public Asset_deserialize[] list; //资源列表
        public StructInfo Struct; //产品信息
    }

    [Serializable]
    public class Asset_deserialize
    {
        public string file_version;
        public string file_url;
        public string file_range;
        public string file_name;
        public string file_type;
        public string file_id;
        public string struct_id;
        public string file_sort;
        public string add_time;
        public string file_size;
    }

    [Serializable]
    public class StructInfo
    {
        public string struct_name;
        public string struct_version;
        public string struct_code;
        public string platform;
    }
}


public class Download_Vesal_info
{
    public string name;
    public string url;
    public string version;
    public string type;
    public bool isNeedDownload;
    public GameObject instance;
    public GameObject source;
}

[Serializable]
public class Download_info
{
    public string name;
    public string url;
    public string version;
    public string type;
    public bool isNeedDownload;
}


public enum asset_platform
{
    pc,
    android,
    ios
}