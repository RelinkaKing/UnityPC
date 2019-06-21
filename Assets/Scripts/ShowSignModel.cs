using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Assets.Scripts.Model;
using Assets.Scripts.Repository;
using LiteDB;
using UnityEngine;
using UnityEngine.UI;
using VesalCommon;

public class ShowSignModel : MonoBehaviour
{
    public GameObject SignElement_parent;
    public GameObject Bookmark;

    void Start()
    {
        PublicClass.showSignModel = this;
        if (Bookmark != null)
        {
            Bookmark.SetActive(false);
        }
    }

    // public void Init_UI () {
    //     Bookmark.gameObject.SetActive (true);
    // }
    //流转字符组
    public byte[] StreamToBytes(LiteFileStream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        return bytes;
    }
    GameObject CurrentObj = null;

    public void DestroyCurrentSign()
    {
        if (CurrentObj != null)
        {
            DestroyImmediate(CurrentObj);
        }
    }
    AssetBundle curBundleObj = null;
    //加载图谱模式模型
    //加载图谱模式模型
    public AssetBundle LoadPSModel(string url, string password, string sceneName)
    {
        string secUrl = string.Empty;
        secUrl = url.Insert(url.LastIndexOf("\\") + 1, "vesal");
        vesal_log.vesal_write_log("开始获取加密文件：" + DateTime.Now.TimeOfDay.ToString());
        ConnectionString connect1 = new ConnectionString();
        connect1.Filename = secUrl;
        connect1.LimitSize = 10000000000;
        connect1.Password = password;
        connect1.Journal = false;
        connect1.Mode = LiteDB.FileMode.ReadOnly;
        byte[] streams1, streams2;
        using (var db = new LiteDatabase(connect1))
        {
            var stream = db.FileStorage.OpenRead(sceneName + ".assetbundle");
            streams1 = StreamToBytes(stream);
            stream.Dispose();
        }
        vesal_log.vesal_write_log("开始获取未加密文件：" + DateTime.Now.TimeOfDay.ToString());
        ConnectionString connect2 = new ConnectionString();
        connect2.Filename = url;
        connect2.LimitSize = 10000000000;
        connect2.Journal = false;
        connect2.Mode = LiteDB.FileMode.ReadOnly;
        using (var db = new LiteDatabase(connect2))
        {
            var stream = db.FileStorage.OpenRead(sceneName + ".assetbundle");
            streams2 = StreamToBytes(stream);
            stream.Dispose();
            byte[] streams = new byte[streams1.Length + streams2.Length];
            streams1.CopyTo(streams, 0);
            streams2.CopyTo(streams, streams1.Length);
            AssetBundle curBundleObj = AssetBundle.LoadFromMemory(streams);
            //curBundleObj.Unload(false);
            db.Dispose();
            return curBundleObj;
        }
    }

    /// <summary>
    /// 加载标注
    /// </summary>
    /// <param name="downLoad_name">标注ab</param>
    public void Load_ps(string downLoad_name, string ab_path)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        var local_db = new DbRepository<Abfiles>();
        string temp_path = PublicClass.tempPath + "temp.dat";
        local_db.DataService("vesali.db");
        Abfiles res = local_db.SelectOne<Abfiles>((temp) =>
        {
            if (temp.file_name == Vesal_DirFiles.get_file_name_from_full_path(ab_path))
                return true;
            else
                return false;
        });
        local_db.Close();
        if (res == null || !File.Exists(ab_path))
        {
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.plugin)
            {
                try
                {
                    curBundleObj = LoadPSModel(LoadModel_ppt.url, LoadModel_ppt.password, LoadModel_ppt.sceneName);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }
        else
        {
            //解压ab文件
            psmodel = null;
            ReadTextureAB(PublicClass.filePath + "sign/");
            Vesal_DirFiles.GetAbfile_Synchronize(ab_path, temp_path);
            curBundleObj = AssetBundle.LoadFromFile(temp_path);
        }
        GameObject realObj = (GameObject)curBundleObj.LoadAsset(downLoad_name, typeof(GameObject));
        CurrentObj = Instantiate(realObj);
        CurrentObj.name = "temp_parent";
        CurrentObj.name = CurrentObj.name.Replace("(Clone)", "");
        // realObj.transform.SetParent(Transform_parent);
        // Camera.main.GetComponent<ChooseModel>().SetCameraPosition(CurrentObj.transform);
        Reset();
        GetMaterialsFromModel(downLoad_name);
        curBundleObj.Unload(false);//卸载内存中的资源
        if (File.Exists(temp_path))
        {
            Vesal_DirFiles.DelFile(temp_path);
        }
#elif UNITY_ANDROID || UNITY_IOS
        downLoad_name=GetSAFromSignidTable(PublicClass.app.struct_name);
        curBundleObj = AssetBundle.LoadFromFile (PublicClass.filePath+"Android_sign/"+downLoad_name+".assetbundle");
        GameObject realObj = (GameObject) curBundleObj.LoadAsset (downLoad_name, typeof (GameObject));
        CurrentObj = Instantiate (realObj);
        CurrentObj.name = "temp_parent";
        CurrentObj.name = CurrentObj.name.Replace ("(Clone)", "");
        Reset ();
        GetMaterialsFromModel (downLoad_name);
        curBundleObj.Unload (false); //卸载内存中的资源
#endif
    }


    public string GetSAFromSignidTable(string struct_name)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(PublicClass.filePath + "Android_sign/SAToSignId.xml");
        XmlNodeList mainList = doc.SelectNodes("//Atlas");
        for (int i = 0; i < mainList.Count; i++)
        {
            if (mainList.Item(i).Attributes["signName"].Value == struct_name)
            {
                DebugLog.DebugLogInfo("查询 sa " + mainList.Item(i).Attributes["signId"].Value);
                return mainList.Item(i).Attributes["signId"].Value;
            }
        }
        Debug.LogError("标注 " + struct_name + "  异常");
        return "15";
    }

    //标注模型解释
    static Dictionary<string, ModelInfo> sign_info_dic = new Dictionary<string, ModelInfo>();
    MeshCollider[] psmodel;

    /// <summary>
    /// 获取材质球
    /// </summary>
    /// <param name="nounNo"></param>
    public void GetMaterialsFromModel(string nounNo)
    {
        //关闭场景模型
        psmodel = CurrentObj.GetComponentsInChildren<MeshCollider>();//获取钉子信息脚本（位置）; 
        for (int i = 0; i < psmodel.Length; i++)
        {
            ModelInfo info = null;
            try
            {
                info = PublicClass.infoDic[psmodel[i].name];
                sign_info_dic.Add(psmodel[i].name, info);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                Debug.Log(psmodel[i].name);
            }
        }
        DebugLog.DebugLogInfo("ps  模型解释长度 ：" + sign_info_dic.Count);
        littleMapInfo = null;
        GameObject DuanNao_model = null;
        //附着材质，（果本身是模型（mesh）没有材质，在本地寻找）
        MeshRenderer[] models = CurrentObj.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < models.Length; i++)
        {
            if (models[i].name == "DuanNao_L" || models[i].name == "DuanNao_R")
            {
                DuanNao_model = models[i].gameObject;
                try
                {
                    DuanNao_model.gameObject.name = DuanNao_model.gameObject.name.Replace("DuanNao", "DongNao");
                }
                catch (System.Exception)
                {
                }
            }
        }

        for (int i = 0; i < models.Length; i++)
        {
            if (models[i].name == "NaoGan_R" || models[i].name == "NaoGan_L")
            {
                DuanNao_model = models[i].gameObject;
                try
                {
                    DuanNao_model.gameObject.name = "NaoGan";
                }
                catch (System.Exception)
                {
                }
            }
        }

        for (int i = 0; i < models.Length; i++)
        {
            // DebugLog.DebugLogInfo("默认材质名称：" + models[i].name);
            if (!models[i].name.Contains("PS"))
            {
                for (int j = 0; j < PublicClass.AllModels.Count; j++)
                {
                    if (PublicClass.AllModels[j].name == models[i].name)
                    {
                        bool isfind = false;
                        models[i].material = GetTextureFromLocal(models[i].name, PublicClass.AllModels[j].GetComponent<MeshRenderer>().material,out isfind);
                        if (isfind)
                        {
                            //models[i].sharedMaterial.SetTexture("_MetallicGlossMap", null);
                        }
                        if (littleMapInfo == null)
                        {
                            string modelname = PublicClass.AllModels[j].GetComponent<Model>().name;
                            if (PublicClass.infoDic.ContainsKey(modelname))
                            {
                                littleMapInfo = PublicClass.infoDic[modelname];
                            }
                            print(littleMapInfo.littleMap + "-------------------========================");
                        }
                        // models[i].sharedMaterial = PublicClass.AllModels[j].GetComponent<MeshRenderer>().sharedMaterial;
                        // DebugLog.DebugLogInfo(models[i].name);
                        // DebugLog.DebugLogInfo(PublicClass.AllModels[j].GetComponent<MeshRenderer>().sharedMaterial.name);
                        // DebugLog.DebugLogInfo(models[i].sharedMaterial.name);
                        break;
                    }
                }
            }
            else
            {
                // DebugLog.DebugLogInfo(models[i].material.name + "  " + models[i].sharedMaterial.name + " " + models[i].material.shader.name);
            }
        }
        CreateSignList();
    }


    class texture_table
    {
        public string model_name { get; set; }
        public string tex_name { get; set; }
    }

    Dictionary<string, string> name_texture_dic = new Dictionary<string, string>();

    Material GetTextureFromLocal(string name, Material mt,out bool _isfind)
    {
        string texture_name = string.Empty;
        Material _mt= new Material(Shader.Find("Standard"));

        _mt.SetTexture("_BumpMap", mt.GetTexture("_BumpMap"));

        bool isfind = name_texture_dic.TryGetValue(name, out texture_name);
        if (!isfind)
        {
            _isfind = false;
            return mt;
        }
        for (int i = 0; i < texture_list.Length; i++)
        {
            if (texture_list[i].name == texture_name)
            {
                _mt.mainTexture = texture_list[i];
                _isfind = true;
                return _mt;
            }
        }
        for (int i = 0; i < texture_list2.Length; i++)
        {
            if (texture_list2[i].name == texture_name)
            {
                _mt.mainTexture = texture_list2[i];
                _isfind = true;
                return _mt;
            }
        }
        
        _isfind = false;
        return mt;
    }

    Texture[] texture_list = null;
    Texture[] texture_list2 = null;
    void ReadTextureAB(string filepath)
    {
        //read db  return texture name
        DbRepository<texture_table> temp_db = new DbRepository<texture_table>();
        temp_db.CreateDb(filepath + "Sign_old.db");
        IEnumerable<texture_table> aculist = temp_db.Select<texture_table>((tmp) =>
        {
            if (tmp != null)
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
            if (!name_texture_dic.ContainsKey(item.model_name))
            {
                name_texture_dic.Add(item.model_name, item.tex_name);
            }
        }
        print("name_texture_dic  list " + name_texture_dic.Count);
        temp_db.Close();

        //load ab --find texture with name --
        AssetBundle texture_bundle = null;
        AssetBundle texture_bundle2 = null;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        texture_bundle = AssetBundle.LoadFromFile(filepath + "bonetex.pc");
        texture_bundle2 = AssetBundle.LoadFromFile(filepath + "hearttex.pc");
#elif UNITY_IOS
        texture_bundle=AssetBundle.LoadFromFile(filepath + "bonetex.ios");
#else
        texture_bundle = AssetBundle.LoadFromFile(filepath + "bonetex.android");
#endif
        texture_list = texture_bundle.LoadAllAssets<Texture>();
        texture_list2 = texture_bundle2.LoadAllAssets<Texture>();
        print("texture_list count : " + texture_list.Length);

        if (texture_bundle != null)
        {
            texture_bundle.Unload(false);
            texture_bundle2.Unload(false);
        }
    }


    public static ModelInfo littleMapInfo;


    public string GetSignMainModelInfo(GameObject sign_g)
    {
        return sign_g.transform.root.name;
    }


    public void CreateSignList()
    {
        if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer)
        {
            for (int i = 0; i < SignElement_parent.transform.childCount; ++i)
            {
                GameObject tmp = SignElement_parent.transform.GetChild(i).gameObject;
                if (Bookmark.name != tmp.name)
                {
                    DestroyImmediate(tmp);
                }
            }
        }
        if (SignElement_parent.transform.childCount > 1)
        {
            return;
        }
        //生成钉子
        for (int j = 0; j < psmodel.Length; j++)                                                       //批量赋值，在空物体下生成面片模型
        {
            if (psmodel[j].name.Contains("PS"))
            {
                GameObject sign = Instantiate(Resources.Load<GameObject>("Prefab/mianpian"));             //面片三维展示
                TextureIcon tempSign = sign.GetComponent<TextureIcon>();
                GameObject signElement = Instantiate(Bookmark);
                signElement.SetActive(true);
                SignElement signElem = signElement.GetComponent<SignElement>();                           //滑动栏ui脚本
                signElement.transform.SetParent(SignElement_parent.transform);
                signElement.transform.localScale = Vector3.one;
                signElement.transform.localEulerAngles = Vector3.zero;
                signElement.transform.localPosition = new Vector3(signElement.transform.localPosition.x, signElement.transform.localPosition.y, 0);
                sign.name = j.ToString();
                ModelInfo tempSignInfo;
                tempSign.whichSign = signElem;
                if (sign_info_dic.ContainsKey(psmodel[j].name))           //非整个模型
                {
                    tempSignInfo = sign_info_dic[psmodel[j].name];
                    tempSign.Info = tempSignInfo;
                    signElem._name = tempSign.Info.Chinese;
                }
                signElem.num = (j + 1).ToString();
                signElem.whichSignIcon = tempSign;
                //图钉 ui 属性赋值
                sign.transform.parent = psmodel[j].transform;
                sign.transform.localPosition = Vector3.zero;
                sign.transform.localScale = Vector3.one;
            }
            else
            {
                DestroyImmediate(psmodel[j]);
            }
        }
        PublicClass.appstate = App_State.Running;
    }

    public void Reset()
    {
        DebugLog.DebugLogInfo("________________________________________________");
        // GetStructList structList=SceneModels.instance.GetStructListFromdb(PublicClass.app.app_id);
        // SceneModels.instance. Auto_position(CurrentObj.transform,structList);
        //重新定位距离
        // SceneModels.SetCameraPosition(CurrentObj.transform,Vector3.zero,PublicTools.Str2Vector3(structList.camPos).z);
        SceneModels.SetCameraPosition(CurrentObj.transform, Vector3.zero);
    }

    void OnDestroy()
    {
        if (curBundleObj != null)
            curBundleObj.Unload(true);//卸载内存中的资源
    }
}
