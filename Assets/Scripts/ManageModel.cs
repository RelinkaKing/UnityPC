using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Model;
using Assets.Scripts.Public;
using Assets.Scripts.Repository;
using UnityEngine;
using UnityEngine.SceneManagement;
using VesalCommon;
using VesalDecrypt;

//加载模型，加载数据（模型信息）
public class ManageModel : MonoBehaviour
{
    public static ManageModel Instance;
    // public static List<Download_Vesal_info> Asset_loaded_list=new List<Download_Vesal_info>();//成功加载的资源列表
    //public static List<Download_Vesal_info> total_load_list = new List<Download_Vesal_info>();//异步加载assetbundle资源列表
    //public static Transform Transform_parent;
    //public static List<Model> AllModels = new List<Model>();
    //public static Dictionary<string, ModelInfo> infoDic;
    ////以下字典提供通过名称找到在Allmodes中的位置
    //public static Dictionary<string, int> id_model_dic;
    ShowProgress progress;
    int count = 0;
    int length = 0;
    //加载标记
    bool start_load_model = false;
    //加载计时器
    int timer = 0;
    void Awake()
    {
        Instance = this;
        InitData();
    }

    void InitData()
    {
        progress = GameObject.Find("LoadingCanvas_pc").transform.GetComponent<ShowProgress>();
        if (GameObject.Find("ModelParent") == null)
        {
            //模型管理父物体
            PublicClass.Transform_parent = new GameObject("ModelParent").transform;
            // PublicClass.Model_A = Transform_parent.gameObject;
            PublicClass.Transform_parent.position = Vector3.zero;
        }
        if (GameObject.Find("ModelParent_temp") == null)
        {
            //模型管理父物体
            PublicClass.Transform_temp = new GameObject("ModelParent_temp").transform;
            // PublicClass.Model_A = Transform_parent.gameObject;
            PublicClass.Transform_temp.position = Vector3.zero;
        }
        DontDestroyOnLoad(PublicClass.Transform_parent.gameObject);
        DontDestroyOnLoad(PublicClass.Transform_temp.gameObject);
    }

    //销毁 PublicClass.Transform_temp 下子模型
    public void Destory_Transform_temp_child()
    {
        Transform[] objects = new Transform[PublicClass.Transform_temp.transform.childCount];
        for (int i = 0; i < PublicClass.Transform_temp.transform.childCount; i++)
        {
            objects[i] = PublicClass.Transform_temp.transform.GetChild(i);
        }
        Debug.Log(" sub model count " + objects.Length);
        //Transform[] objects = PublicClass.Transform_temp.transform.GetChild<Transform>();//GetChild(PublicClass.Transform_temp.transform.childCount);
        // GameObject[] objects = PublicClass.Transform_temp.transform.GetComponentsInChildren<GameObject>();//GetChild(PublicClass.Transform_temp.transform.childCount);
        //  Debug.Log(" sub model count "+ objects.Length);
        for (int i = 0; i < objects.Length; i++)
        {
            Destroy(objects[i].gameObject);
        }
    }

    List<Download_Vesal_info> load_list;//异步加载assetbundle资源列表

    public void load_assets_A(List<Download_Vesal_info> targetList, Action load_complete = null)
    {
        if (PublicClass.id_model_dic == null)
        {
            PublicClass.id_model_dic = new Dictionary<string, int>();
        }

        load_list = new List<Download_Vesal_info>();
        foreach (Download_Vesal_info temp in targetList)
        {
            if ((temp.type == null) || (temp.type.ToUpper() == "") || (temp.type.ToUpper() == "AB"))
            {
                load_list.Add(temp);
                PublicClass.total_load_list.Add(temp);
            }
        }
        //DebugLog.DebugLogInfo("--------------------可以加载模型长度：" + PublicClass.total_load_list.Count);
        //foreach (Download_Vesal_info temp in PublicClass.total_load_list)
        //{
        //    DebugLog.DebugLogInfo("total list：" + temp.name);
        //}

        //去掉多余的加载项
        int max = load_list.Count;
        for (int k = PublicClass.int_load_AB_nums; k < max; k++)
        {
            load_list.RemoveAt(PublicClass.int_load_AB_nums);
        }

        Unity_Tools.StarTime("获取远程数据计时-------");
        progress.Set_Progress("模型正在加载中...", Call);
        length = 0;
        count = 0;
        length = load_list.Count;
        DebugLog.DebugLogInfo("--------------------加载模型长度：" + length);
        ////加载公共库异常处理
        //if (load_list.Count == 0)
        //{
        //    progress.Set_Progress("加载数据错误...", Call);
        //}
//这是在未PPTPlayer 播放时，启动时指定vsl文件而给与解压
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer || PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.WeiKePlayer)
        {
            isUnzipDown = true;
            string[] CommandLineArgs = Environment.GetCommandLineArgs();
            for (int i = 0; i < CommandLineArgs.Length; i++)
            {
                string tmpStr = CommandLineArgs[i];
                //tmpStr = "C:\\vesalplayer\\WK0000001_5.vsl";
                Debug.Log("GetCommandLineArgs::" + tmpStr);
                vesal_log.vesal_write_log("GetCommandLineArgs::" + tmpStr);
                if (tmpStr.Contains(".vsl") || tmpStr.Contains(".VSL"))
                {
                    Debug.Log("GetCommandLineArgs::" + tmpStr);
                    //SelectFile(tmpStr);
                    PPTHomePageController.TempFilePath = PPTHomePageController.getTempPath();
                    vesal_log.vesal_write_log("PPTHomePageController.TempFilePath " + PPTHomePageController.TempFilePath);
                    StartCoroutine(Vesal_DirFiles.UnZipAsync(tmpStr, PPTHomePageController.TempFilePath, ManagerModelUnzipCall, true));
                    PPTResourcePool.isSkipUnzip = true;
                    isUnzipDown = false;
                    PPTHomePageController.currentFilePath = tmpStr;
                    break;
                }
            }

        }



        if (load_list.Count > 0)
        {
            //设置加载标记
            start_load_model = true;
            timer = 0;
            //            progress = GameObject.Find("LoadingCanvas").transform.GetComponent<ShowProgress>();

            StartCoroutine(LoadPrefabModel(PublicClass.filePath + load_list[0].name, Vesal_DirFiles.remove_name_suffix(load_list[0].name), count_load));
        }
        else
        {
            start_load_model = false;
            this.ReadModelInfo();
            if(PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.demo_pc)
                SceneManager.LoadScene("SceneSwitchInteral");
            else
                SceneManager.LoadScene("SceneSwitch");
        }
    }
    public void ManagerModelUnzipCall(float pg)
    {
        if (pg == 1.0F)
        {
            isUnzipDown = true;
        }
        if (isLoadAbDown)
        {
            progress.Title.text = "资源解析中......";
            progress.current_progress = pg;
        }
        if (isLoadAbDown && isUnzipDown)
        {
            isLoadAbDown = false;
            SceneManager.LoadScene("SceneSwitch");
        }
    }
    bool isLoadAbDown = false;
    bool isUnzipDown = true;

    public void Call()
    {
        DebugLog.DebugLogInfo("cancel loading ----------------------");
    }

    public string sa_name = string.Empty;

    bool load_error_flag = false;
    //加载完毕回调
    void count_load()
    {
        print(load_list.Count + " ---------- count");
        if (load_error_flag == false)
            PublicClass.Asset_loaded_list.Add(load_list[count]);
        count++;
        progress.current_progress = (float)count / (float)length;
        if (load_list.Count > count)
        {
            timer = 0;
            StartCoroutine(LoadPrefabModel(PublicClass.filePath + load_list[count].name, Vesal_DirFiles.remove_name_suffix(load_list[count].name), count_load));

        }
        if (count == length)
        {
#if UNITY_EDITOR
            //Debug.LogError(LoadResult);
#endif
            start_load_model = false;
            this.ReadModelInfo();
            PublicClass.Transform_parent.gameObject.SetActive(false);
            ReadModel(PublicClass.Transform_parent);

#if UNITY_ANDROID
            // DebugLog.DebugLogInfo("返回平台！");
            Unity_Tools.ui_return_to_platform();
#endif
            isLoadAbDown = true;
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer || PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.WeiKePlayer)
            {

                //if (PPTResourcePool.isSkipUnzip == true)
                //{
                //    isLoadAbDown = true;

                //}
                if (isLoadAbDown && isUnzipDown)
                {
                    isLoadAbDown = false;
                    if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.demo_pc)
                        SceneManager.LoadScene("SceneSwitchInteral");
                    else
                        SceneManager.LoadScene("SceneSwitch");
                }
                return;
            }

            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.demo_pc)
                SceneManager.LoadScene("SceneSwitchInteral");
            else
                SceneManager.LoadScene("SceneSwitch");
            
        }
    }


    string LoadResult = "";
    //public static bool stepLoad = false;
    IEnumerator LoadPrefabModel(string path, string name, Action count_load = null)
    {
        Debug.Log(name);
        //encrypt.DecryptFile(path, path.Remove(path.LastIndexOf('/')) + "/vesal.temp", "Vesal17788051918");
        //解压ab文件
        //yield return null;  
        //防止卡死
        load_error_flag = false;
        //while (!stepLoad) {
        //    yield return null;
        //}

        yield return new WaitForEndOfFrame();
        string temp_path = PublicClass.tempPath + name + "temp.dat";
        vesal_log.vesal_write_log(name);
        //vesal_log.vesal_write_log("load model phase 1 start: " + DateTime.Now.Ticks);
        //yield return null;
        AssetBundleCreateRequest assetBundleCreateRequest = null;
        try
        {
            Vesal_DirFiles.GetAbfile2(path, temp_path);
            //if (File.Exists(temp_path))
            //    Debug.Log(temp_path + "exsit！！！！！！！！！！！！！");

            assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(temp_path);

            //assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(Vesal_DirFiles.GetAbfile(path, temp_path));
        }
        catch (Exception ex)
        {
            load_error_flag = true;
            vesal_log.vesal_write_log("assetbundle load error in :" + name);
            vesal_log.vesal_write_log("error message:" + ex.Message);
        }
        //Vesal_DirFiles.DelFile(temp_path);

        if (assetBundleCreateRequest != null && load_error_flag == false)
        {
            yield return assetBundleCreateRequest;

            GameObject CurrentObj = null;
            GameObject realObj = null;
            AssetBundleRequest abr = null;
            try
            {
                // abr = assetBundleCreateRequest.assetBundle.LoadAssetAsync(name, typeof(GameObject));
                CurrentObj = (GameObject)assetBundleCreateRequest.assetBundle.LoadAsset(name, typeof(GameObject)); ;
                realObj = Instantiate(CurrentObj);
#if UNITY_EDITOR
                MeshFilter[] filters = null;
                filters = realObj.GetComponentsInChildren<MeshFilter>(true);
                int topCount = 0;
                int triCount = 0;
                int meshCount = 0;
                if (filters != null)
                {
                    for (int j = 0; j < filters.Length; j++)
                    {
                        MeshFilter f = filters[j];
                        topCount += f.sharedMesh.vertexCount;
                        triCount += f.sharedMesh.triangles.Length / 3;
                        meshCount++;
                    }
                }
                LoadResult += string.Format("{0,20}\t{1,20}\t{2,20}", name, topCount, topCount / 8881.123208f) + "\n";
                //Debug.LogError(name + "总共Mesh=" + meshCount + "   总共顶点=" + topCount + "   总共三角形=" + triCount);
                //Debug.LogError(string.Format("{0,20}\t{1,20}\t{2,20}M", name, topCount, topCount / 8881.123208f));
                // Debug.LogError(name + " 总共顶点=" + topCount +" 占用内存:" + topCount / 8881.123208f + "M");


#endif

            }
            catch (Exception ex)
            {
                load_error_flag = true;
                vesal_log.vesal_write_log("assetbundle load error in :" + name);
                vesal_log.vesal_write_log("error message:" + ex.Message);
            }

            //yield return abr;
            //if (abr != null && abr.asset != null)
            //{
            //    CurrentObj = (GameObject)abr.asset;
            //    realObj = Instantiate(CurrentObj);
            //}
            //else
            //{
            //    load_error_flag = true;
            //    vesal_log.vesal_write_log("assetbundle load error in :" + name);
            //}

            if (CurrentObj != null && realObj != null && load_error_flag == false)
            {
                yield return realObj;
                realObj.name = realObj.name.Replace("(Clone)", "");
                realObj.transform.SetParent(PublicClass.Transform_parent);
                assetBundleCreateRequest.assetBundle.Unload(false);

                load_list[count].source = CurrentObj;
                load_list[count].instance = realObj;
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

    public void UpdateModelInfo()
    {
        //读取所有模型信息
        DebugLog.DebugLogInfo("--------------------ReadModelInfo");
        //连接本地数据库
        var local_db = new DbRepository<GetSubModel>();
        local_db.DataService("vesali.db");
        var dbs = local_db.Select<GetSubModel>((tempt) =>
        {
            return true;
        });
        if (PublicClass.infoDic == null)
        {
            Debug.Log("init info dic faile");
        }
        foreach (var i in dbs)
        {
            if (PublicClass.infoDic.ContainsKey(i.name))
            {
                continue;
            }
            ModelInfo tempInfo = new ModelInfo();
            tempInfo.ModelName = i.name;
            tempInfo.English = i.enName;
            tempInfo.model_Id = i.submodelId;
            tempInfo.littleMap = i.partitionName; // "Abdomen";
            // Debug.Log(i.partitionName);
            try
            {
                tempInfo.Note = i.description;
                tempInfo.Chinese = i.chName;
            }
            catch { }
            try
            {
                PublicClass.infoDic.Add(tempInfo.ModelName, tempInfo);
            }
            catch { Debug.Log("重复 " + tempInfo.ModelName); }
        }
        local_db.Close();

        DebugLog.DebugLogInfo(PublicClass.infoDic.Count + "  总文档长度");
    }


    public void ReadModelInfo()
    {
        //读取所有模型信息
        DebugLog.DebugLogInfo("--------------------ReadModelInfo");
        //连接本地数据库
        var local_db = new DbRepository<GetSubModel>();
        local_db.DataService("vesali.db");
        var dbs = local_db.Select<GetSubModel>((tempt) =>
        {
            return true;
        });
        int n = 0;
        if (PublicClass.infoDic == null)
        {
            PublicClass.infoDic = new Dictionary<string, ModelInfo>();
        }
        foreach (var i in dbs)
        {
            n++;
            ModelInfo tempInfo = new ModelInfo();
            tempInfo.ModelName = i.name;
            tempInfo.English = i.enName;
            tempInfo.model_Id = i.submodelId;
            tempInfo.littleMap = i.partitionName; // "Abdomen";
            // Debug.Log(i.partitionName);
            try
            {
                tempInfo.Note = i.description;
                tempInfo.Chinese = i.chName;
            }
            catch { }
            try
            {
                PublicClass.infoDic.Add(tempInfo.ModelName, tempInfo);
            }
            catch { Debug.Log("重复 " + tempInfo.ModelName); }
        }
        local_db.Close();

        DebugLog.DebugLogInfo(PublicClass.infoDic.Count + "  总文档长度");
    }

    public void ReadModel(Transform parent)
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
        ReadLocalSignModel();
    }

    public void ReadLocalSignModel()
    {
        Transform parent = PublicClass.Transform_temp;
        AssetBundle texture_bundle = null;
        texture_bundle = AssetBundle.LoadFromFile(PublicClass.filePath + "sign/pc_old.pc");
        GameObject obj = texture_bundle.LoadAsset<GameObject>("pc_old");
        GameObject temp_obj=Instantiate(obj, parent);
        Model[] models = parent.GetComponentsInChildren<Model>();
        DebugLog.DebugLogInfo(models.Length + "  总模型长度");
        for (int j = 0; j < models.Length; j++)
        {
            models[j].name = "o_" + models[j].name;
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

        if (texture_bundle != null)
        {
            texture_bundle.Unload(false);
        }
        temp_obj.transform.parent= PublicClass.Transform_parent;
    }

    //public void GetModelArray(Transform parent)
    //{
    //    Model[] models = Resources.FindObjectsOfTypeAll<Model>();
    //    DebugLog.DebugLogInfo(models.Length + "  总模型长度");
    //    for (int j = 0; j < models.Length; j++)
    //    {
    //        ModelInfo info = null;
    //        try
    //        {
    //            info = infoDic[models[j].name];
    //            models[j].Info = info;
    //            AllModels.Add(models[j]);
    //        }
    //        catch
    //        {
    //            Debug.Log(models[j].name);
    //        }
    //        try
    //        {
    //            id_model_dic.Add(info.model_Id, models[j]);
    //        }
    //        catch
    //        {
    //            // Debug.Log(info.model_Id+" 模型id重复");
    //        }
    //    }
    //}

    //IEnumerator Copy(string scr_path, string des_path, Action load_complete = null)
    //{
    //    WWW www = new WWW(scr_path);
    //    yield return www;
    //    if (!string.IsNullOrEmpty(www.error))
    //    {
    //        Debug.Log("www.error:" + www.error);
    //    }
    //    else
    //    {
    //        if (File.Exists(des_path))
    //        {
    //            File.Delete(des_path);
    //        }
    //        FileStream fsDes = File.Create(des_path);
    //        fsDes.Write(www.bytes, 0, www.bytes.Length);
    //        fsDes.Flush();
    //        fsDes.Close();
    //        count++;
    //        if (count == length)
    //        {
    //            if (load_complete != null)
    //                load_complete();
    //        }
    //    }
    //    www.Dispose();
    //}
}