using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using Assets.Scripts.Public;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VesalCommon;
using VesalDecrypt;
using Assets.Scripts.Model;
using Assets.Scripts.Repository;

public class LinkStart : MonoBehaviour
{

    [Header("DownLoad")]
    public GameObject BlankCanvas;
    //下载传输协议实例化
    private HttpDownLoad http;
    //下载计数
    private int ndr = 0;
    string dd_config_path = PublicClass.filePath + "download.dat";
    List<Download_Vesal_info> Ready_Download_list = new List<Download_Vesal_info> { };
    List<Download_Vesal_info> Download_list = new List<Download_Vesal_info> { };
    public ShowProgress showProgress;


    // public HotFixControll hotFixControll;
    public void GetScreenResolution()
    {
        Resolution resolutions = Screen.currentResolution;
        Debug.Log("分辨率：" + resolutions.width + " : " + resolutions.height);
    }

    //资源下载更新
    private void AnalysisResourceList()
    {

        //检查更新
        dd_config_path = PublicClass.filePath + "download.dat";

        Ready_Download_list = load_download_list_from_file(dd_config_path);
        DebugLog.DebugLogInfo(dd_config_path + Ready_Download_list.Count);
        foreach (Download_Vesal_info temp in Ready_Download_list)
            DebugLog.DebugLogInfo(temp.name + temp.version);

        if (Ready_Download_list == null)
            Ready_Download_list = new List<Download_Vesal_info>();
        Download_list = DataManager.GetStructInfo(PublicClass.platform);
        if (!PublicClass.online_mode)
        {
            PublicClass.DataManagerStatus = DataIntState.GoNextScence; //启动场景跳转 
            return;
        }
        //本地
        //网络        
        // downloadText.text = "正在分析数据...";
        DebugLog.DebugLogInfo("数据长度..." + Download_list.Count);

        for (int i = 0; i < Download_list.Count; i++)
        {
            //            int founded = 0;
            Download_list[i].isNeedDownload = true;
            for (int k = 0; k < Ready_Download_list.Count; k++)
            {
                if (Ready_Download_list[k].name.Replace("s_", "") == Download_list[i].name.Replace("s_", ""))
                {
                    //本地存在，进行比较
                    Debug.Log(Ready_Download_list[k].name + "--" + Download_list[i].name + "--" + Ready_Download_list[k].version + "--" + Download_list[i].version);
                    if (Ready_Download_list[k].version == Download_list[i].version)
                    {
                        Download_list[i].isNeedDownload = false;
                    }
                    break;
                }
            }
        }

        Ready_Download_list.Clear();
        for (int i = 0; i < Download_list.Count; i++)
        {
            if (Download_list[i].isNeedDownload)
                Ready_Download_list.Add(Download_list[i]);
        }
        DebugLog.DebugLogInfo("生成下载列表长度 " + Ready_Download_list.Count);

        if (!MainConfig.isDownloadModel)
        {
            Ready_Download_list.Clear();
        }

        //如果资源列表为空
        if (Ready_Download_list.Count == 0)
        {
            PublicClass.DataManagerStatus = DataIntState.GoNextScence; //启动场景跳转              
            //PublicClass.DataManagerStatus = DataIntState.HotFix;
            // PublicClass.DataManagerStatus = DataIntState.HotFix;//HotFix; //启动场景跳转              
        }
        else
        {
            PublicClass.DataManagerStatus = DataIntState.Downloading; //开始下载        
            DownloadNow();
        }
    }

    //开启下载
    public void DownloadNow()
    {
        //初始化
        showProgress.Set_Progress("正在下载模型..."); //, CannelDownload);
        ndr = 0;
        http = new HttpDownLoad();
        DebugLog.DebugLogInfo("开始下载！");
        DownSwitch();

    }


    //下载完成回调
    private void Download()
    {
        download_trys = 0;
        //if (Ready_Download_list[ndr].type.ToUpper() == "ZIP")
        //{
        //    string tmpName = Ready_Download_list[ndr].name;
        //    string tmpUrl = Ready_Download_list[ndr].url;// + "?v=" + System.DateTime.Now;
        //    string filePath = PublicClass.filePath + tmpName;
        //    Vesal_DirFiles.UnZip(filePath, Vesal_DirFiles.get_dir_from_full_path(filePath), true);
        //    Vesal_DirFiles.DelFile(filePath);
        //}

        ndr++;
        if (ndr < Ready_Download_list.Count)
        {
            http = new HttpDownLoad();
            DownSwitch();
        }
        else
        {
            PublicClass.DataManagerStatus = DataIntState.Update;
            save_download_list_to_file(dd_config_path, Download_list);
        }
    }

    //下载错误回调
    int download_trys = 0;
    private void Download_error(Exception e)
    {
        //下载失败，最多尝试3次
        if (download_trys < PublicClass.MaxTryDownloadNums)
        {
            http = new HttpDownLoad();
            DownSwitch();
        }
        else
        {
            //彻底退出
            Unity_Tools.ui_return_to_platform();
            Application.Quit();
        }
        download_trys++;
    }

    int moveIndex = 0;

    //下载切换
    void DownSwitch()
    {
        string tmpName = Ready_Download_list[ndr].name;
        string tmpUrl = Ready_Download_list[ndr].url;//+ "?v=" + System.DateTime.Now;

        string filePath;
        switch (Ready_Download_list[ndr].type.ToLower())
        {
            case "anim_ab":
                filePath = PublicClass.Anim_TimelinePath + tmpName;
                PublicClass.Anim_ABPath = filePath;
                break;
            case "ab":
                filePath = PublicClass.filePath + tmpName;
                break;
            case "db":
                filePath = PublicClass.vesal_db_path + tmpName;
                break;
            case "update":
                filePath = PublicClass.filePath + tmpName;
                break;
            case "lua":
                filePath = PublicClass.filePath + tmpName;
                Vesal_DirFiles.ClearFolder(PublicClass.xluaPath);
                break;
            default:
                filePath = PublicClass.filePath + tmpName;
                break;
        }


        Debug.Log("当前下载资源：" + tmpName + " 路径：" + tmpUrl + "version " + Ready_Download_list[ndr].version);
        Vesal_DirFiles.DelFile(filePath);

        if (!http.DownLoad(tmpUrl, filePath, Download, Download_error))
        {
            Unity_Tools.ui_return_to_platform();
            Application.Quit(); //下载过程中，连接断开，重连失败
        }

    }

    public void CannelDownload()
    {
        Debug.Log("-------------------------cancel_download");
        http.Close();
        Application.Quit();
    }

    //[DllImport("__Internal")]
    //private static extern void _PressButton();

    //关闭按钮
    public void ExitProgram() //关闭场景
    {
        // _PressButton();
        //SceneManager.LoadScene("Start");
    }

    private void Update()
    {
        switch (PublicClass.DataManagerStatus)
        {
            case DataIntState.FetchingDb:
                if (PublicClass.data_list_count < PublicClass.tableCount)
                {
                    showProgress.current_progress = (float)PublicClass.data_list_count / PublicClass.tableCount;
                }
                else
                    PublicClass.DataManagerStatus = DataIntState.GoABdownload;
                break;
            case DataIntState.GoABdownload:
                Unity_Tools.CanculateLoadTime();

                // DebugLog.DebugLogInfo("本地数据地址 "+PublicClass.filePath);
                DebugLog.DebugLogInfo("数据加载完成==============================================");
                AnalysisResourceList(); //加载模型资源
                //PublicClass.data_list_count = 0;
                break;
            case DataIntState.Downloading:
                showProgress.current_progress = (http.progress + ndr) / Ready_Download_list.Count;
                Mathf.Clamp(showProgress.current_progress, 0f, 1f);
                break;
            case DataIntState.Update:
                showProgress.Set_Progress("正在更新补丁数据...");
                PublicClass.DataManagerStatus = DataIntState.Updating;
                DataUpdating();
                break;
            case DataIntState.HotFix:
                showProgress.Set_Progress("正在更新补丁数据...");
                PublicClass.DataManagerStatus = DataIntState.HotFixing;
                // hotFixControll.loads();
                break;
            case DataIntState.GoNextScence:
                PublicTools.Model_AB_dic_update();
                ManageModel.Instance.load_assets_A(Download_list);
                PublicClass.DataManagerStatus = DataIntState.Null;
                break;
            default:
                break;

        }

    }

    List<Download_Vesal_info> update_list = new List<Download_Vesal_info>();
    int update_index = 0;
    void DataUpdating()
    {

        foreach (Download_Vesal_info temp in Ready_Download_list)
        {
            if (temp.type.ToLower() == "update")
            {
                update_list.Add(temp);
            }
        }
        if (update_list.Count == 0)
        {
            // Vesal_DirFiles.DelectDir(PublicClass.UpdatePath);
            PublicClass.DataManagerStatus = DataIntState.GoNextScence;
            // PublicClass.DataManagerStatus = DataIntState.HotFix;
            return;
        }
        //启动正式升级数据

        StartCoroutine(update_process(update_index, update_count));

        PublicClass.DataManagerStatus = DataIntState.Updating;

    }

    void update_count()
    {
        update_index++;
        if (update_index < update_list.Count)
        {
            StartCoroutine(update_process(update_index, update_count));
        }
        else
        {
            // Vesal_DirFiles.DelectDir(PublicClass.UpdatePath);
            PublicClass.DataManagerStatus = DataIntState.GoNextScence;
            // PublicClass.DataManagerStatus = DataIntState.HotFix;
        }
    }

    IEnumerator update_process(int index, Action count_load = null)
    {
        string fix_file = PublicClass.filePath + Vesal_DirFiles.get_file_name_from_full_path(update_list[index].name);
        if (Directory.Exists(PublicClass.UpdatePath))
        {
            Vesal_DirFiles.CreateDir(PublicClass.UpdatePath);
        }
        try
        {
            Vesal_DirFiles.UnZip(fix_file, PublicClass.UpdatePath, true);
            Vesal_DirFiles.DelFile(fix_file);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
        yield return null;
        //打开数据库进行更新
        string[] files = Directory.GetFiles(PublicClass.UpdatePath);
        List<string> fix_tab_list = new List<string>();
        foreach (string tagetfile in files)
        {
            string target = Vesal_DirFiles.get_file_name_from_full_path(tagetfile);
            if (target == "vesali.db")
            {
                File.Copy(tagetfile, PublicClass.vesal_db_path + "temp.db", true);

                fix_tab_list = PublicTools.get_table_list("temp.db");
                int tab_count=0;
                if (fix_tab_list.Count != 0)
                {
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
                        showProgress.current_progress = (float)index / update_list.Count + (float)tab_count / fix_tab_list.Count / update_list.Count;
                        yield return null;
                    }
                }
                else
                {
                    PublicTools.update_GetSubModel_db("temp.db");
                    showProgress.current_progress = (float)index / update_list.Count + 0.1f / update_list.Count;
                    PublicTools.update_GetStructList_db("temp.db");
                    showProgress.current_progress = (float)index / update_list.Count + 0.3f / update_list.Count; 
                    yield return null;
                    PublicTools.update_GetStructAbList_db("temp.db");
                    PublicTools.update_LayserSubModel_db("temp.db");
                    showProgress.current_progress = (float)index / update_list.Count + 0.5f / update_list.Count;
                    yield return null;
                    PublicTools.update_ModelRelationModel_db("temp.db");
                    PublicTools.update_RightMenuLayerModel_db("temp.db");
                    showProgress.current_progress = (float)index / update_list.Count + 0.65f / update_list.Count;
                    yield return null;
                    PublicTools.update_RightMenuModel_db("temp.db");
                    PublicTools.update_SignNewInfo_db("temp.db");
                    showProgress.current_progress = (float)index / update_list.Count + 0.8f / update_list.Count;
                    yield return null;
                    PublicTools.update_GetTextureModelList_db("temp.db");
                    PublicTools.update_noun_no_info_db("temp.db");
                    PublicTools.update_ab_info_db("temp.db");
                    showProgress.current_progress = (float)index / update_list.Count + 0.99f / update_list.Count;
                    yield return null;
                }
            }
            //拷贝文件
        }
        string[] dirs = Directory.GetDirectories(PublicClass.UpdatePath);
        foreach (string targetdir in dirs)
        {
            string subdir = Vesal_DirFiles.get_file_name_from_full_path(targetdir);
            if (subdir.ToUpper() == "ANDROID_SIGN")
            {
                Vesal_DirFiles.Vesal_DirCopy(targetdir, PublicClass.SignPath);
            }
            if (subdir.ToUpper() == "MODEL")
            {
                Vesal_DirFiles.Vesal_DirCopy(targetdir, PublicClass.ModelPath);
            }
        }

        yield return null;
        //删除update数据
        try
        {
            Directory.Delete(PublicClass.UpdatePath, true);
            Vesal_DirFiles.DelFile(PublicClass.vesal_db_path + "temp.db");
        }
        catch (System.Exception e)
        {

            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }

        yield return null;
        if (count_load != null)
            count_load();
    }



    bool isOpenCopyCheck = false;
    bool CopyComplete = false;

    public enum UrlEnverioment
    {
        Local_url,
        Remote_url,
    }

    private List<Download_Vesal_info> load_download_list_from_file(string fname_path)
    {
        try
        {
            List<Download_info> temp;
            if (!File.Exists(fname_path))
                return new List<Download_Vesal_info>();
            FileStream fs = new FileStream(fname_path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            temp = bf.Deserialize(fs) as List<Download_info>;
            fs.Close();
            if (temp == null)
            {
                return new List<Download_Vesal_info>();
            }
            List<Download_Vesal_info> res = new List<Download_Vesal_info>();

            //Debug.Log("temp count" + temp.Count);

            for (int i = 0; i < temp.Count; i++)
            {
                Download_Vesal_info item = new Download_Vesal_info();
                //Debug.Log(temp[i].name + temp[i].version);
                item.name = temp[i].name;
                item.url = temp[i].url;
                item.version = temp[i].version;
                item.type = temp[i].type;
                item.isNeedDownload = temp[i].isNeedDownload;
                res.Add(item);
            }
            return res;
        }
        catch
        {
            return new List<Download_Vesal_info>();
        }
    }

    private bool save_download_list_to_file(string fname_path, List<Download_Vesal_info> dd_list)
    {
        Debug.Log("本地资源列表 " + dd_list.Count);

        List<Download_info> temp_list = new List<Download_info>();

        for (int i = 0; i < dd_list.Count; i++)
        {
            Download_info tmp = new Download_info();
            tmp.name = dd_list[i].name;
            tmp.url = dd_list[i].url;
            tmp.version = dd_list[i].version;
            tmp.type = dd_list[i].type;
            tmp.isNeedDownload = dd_list[i].isNeedDownload;
            temp_list.Add(tmp);
        }

        Debug.Log("save_download_list_to_file");
        if (File.Exists(fname_path))
        {
            File.Delete(fname_path);
        }
        try
        {
            FileStream fs = new FileStream(fname_path, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, temp_list);
            fs.Flush();
            fs.Close();
            return true;
        }
        catch
        {
            Debug.LogError("save_download_list_to_file");
            return false;
        }
    }
    /// <summary>
    /// 读取目标路径下的所有lua文件
    /// </summary>
    //LuaEnv luaEnv;
    //public void loads()
    //{
    //    luaEnv = new LuaEnv();
    //    StartCoroutine(load(PublicClass.xluaPath));
    //}

    //IEnumerator load(string luaPath)
    //{
    //    DirectoryInfo direction = new DirectoryInfo(luaPath);
    //    FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
    //    int num = files.Length;
    //    luaEnv.AddLoader(MyLoader);
    //    for (int i = 0; i < num; i++)
    //    {
    //        Debug.Log(files[i]);
    //        luaEnv.DoString("require '" + files[i].Name.Split('.')[0] + "'");
            
    //    }
    //    yield return 0;
    //    PublicClass.DataManagerStatus = DataIntState.GoNextScence;

    //}
    private byte[] MyLoader(ref string filepath)
    {
        string absPath = @PublicClass.xluaPath + filepath + ".lua.txt";
        StreamReader sr = new StreamReader(absPath);
        return System.Text.Encoding.UTF8.GetBytes(sr.ReadToEnd());
    }
    //private void OnDestroy()
    //{
    //    if (luaEnv != null)
    //        luaEnv.Dispose();
    //}
}